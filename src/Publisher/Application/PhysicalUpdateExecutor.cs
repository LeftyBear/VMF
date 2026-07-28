using System.Net;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Executes revision-bound physical updates with deterministic failure classification.</summary>
public sealed class PhysicalUpdateExecutor : IPhysicalUpdateExecutor
{
    private readonly IPhysicalUpdateRequestMapper mapper;
    private readonly IGoogleDocsBatchUpdateClient client;
    private readonly PhysicalUpdateRetryPolicy retryPolicy;
    private readonly IAsyncDelay delay;
    private readonly Random random;
    private readonly Action<PhysicalUpdateExecutionResult>? resultLogger;

    /// <summary>Initializes the executor.</summary>
    public PhysicalUpdateExecutor(
        IPhysicalUpdateRequestMapper mapper,
        IGoogleDocsBatchUpdateClient client,
        PhysicalUpdateRetryPolicy? retryPolicy = null,
        IAsyncDelay? delay = null,
        Random? random = null,
        Action<PhysicalUpdateExecutionResult>? resultLogger = null)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.retryPolicy = retryPolicy ?? PhysicalUpdateRetryPolicy.Default;
        this.delay = delay ?? new SystemAsyncDelay();
        this.random = random ?? Random.Shared;
        this.resultLogger = resultLogger;
        ValidatePolicy(this.retryPolicy);
    }

    /// <inheritdoc />
    public async Task<PhysicalUpdateExecutionResult> ExecuteAsync(
        PhysicalUpdatePlan plan,
        CancellationToken cancellationToken)
    {
        ValidatePlan(plan);
        var documentId = plan.Identity.GoogleDocumentId
            ?? throw new PhysicalUpdateException(
                UpdateErrorCodes.PhysicalPlanInvalid,
                "The physical plan document ID is missing.");
        var requiredRevisionId = plan.RequiredRevision.RevisionId
            ?? throw new PhysicalUpdateException(
                UpdateErrorCodes.PhysicalPlanInvalid,
                "The physical plan required revision is missing.");
        var operationIds = plan.Operations.Select(operation => operation.OperationId).ToArray();

        if (!plan.IsPublishRequired)
        {
            return LogSafe(new PhysicalUpdateExecutionResult(
                PhysicalUpdateExecutionStatus.NoChange,
                documentId,
                requiredRevisionId,
                appliedRevisionId: null,
                submittedOperationCount: 0,
                submittedRequestCount: 0,
                attemptCount: 0,
                "NO_CHANGE",
                "The physical update plan contains no operations.",
                operationIds: operationIds));
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return LogSafe(new PhysicalUpdateExecutionResult(
                PhysicalUpdateExecutionStatus.TransientFailure,
                documentId,
                requiredRevisionId,
                appliedRevisionId: null,
                submittedOperationCount: 0,
                submittedRequestCount: 0,
                attemptCount: 0,
                "CANCELED_BEFORE_SEND",
                "Execution was canceled before the batch was sent.",
                operationIds: operationIds));
        }

        PhysicalUpdateRequestBatch batch;
        try
        {
            batch = mapper.Map(plan);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return LogSafe(new PhysicalUpdateExecutionResult(
                PhysicalUpdateExecutionStatus.Rejected,
                documentId,
                requiredRevisionId,
                appliedRevisionId: null,
                submittedOperationCount: plan.Operations.Count,
                submittedRequestCount: 0,
                attemptCount: 0,
                "REQUEST_MAPPING_REJECTED",
                exception.Message,
                operationIds: operationIds));
        }

        var attempt = 0;
        while (attempt < retryPolicy.MaxAttempts)
        {
            attempt++;
            try
            {
                var response = await client.ExecuteAsync(batch, cancellationToken).ConfigureAwait(false);
                return LogSafe(new PhysicalUpdateExecutionResult(
                    PhysicalUpdateExecutionStatus.Applied,
                    batch.DocumentId,
                    batch.RequiredRevisionId,
                    response.RevisionId,
                    batch.SourceOperationCount,
                    batch.Requests.Count,
                    attempt,
                    "APPLIED",
                    "The batchUpdate request was accepted.",
                    batch.Traces,
                    operationIds));
            }
            catch (OperationCanceledException exception)
            {
                return LogSafe(new PhysicalUpdateExecutionResult(
                    PhysicalUpdateExecutionStatus.IndeterminateFailure,
                    batch.DocumentId,
                    batch.RequiredRevisionId,
                    appliedRevisionId: null,
                    batch.SourceOperationCount,
                    batch.Requests.Count,
                    attempt,
                    "CANCELED_DURING_SEND",
                    exception.Message,
                    batch.Traces,
                    operationIds));
            }
            catch (GoogleDocsBatchUpdateException exception)
            {
                var classification = Classify(exception);
                if (IsCancellationBeforeSend(exception))
                {
                    return LogSafe(new PhysicalUpdateExecutionResult(
                        PhysicalUpdateExecutionStatus.TransientFailure,
                        batch.DocumentId,
                        batch.RequiredRevisionId,
                        appliedRevisionId: null,
                        batch.SourceOperationCount,
                        batch.Requests.Count,
                        attempt,
                        "CANCELED_BEFORE_SEND",
                        exception.Message,
                        batch.Traces,
                        operationIds));
                }

                if (classification != PhysicalUpdateExecutionStatus.TransientFailure ||
                    attempt >= retryPolicy.MaxAttempts)
                {
                    return LogSafe(Result(batch, classification, attempt, exception, operationIds));
                }

                try
                {
                    await delay.DelayAsync(NextDelay(attempt, exception.RetryAfter), cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException canceled)
                {
                    return LogSafe(new PhysicalUpdateExecutionResult(
                        PhysicalUpdateExecutionStatus.TransientFailure,
                        batch.DocumentId,
                        batch.RequiredRevisionId,
                        appliedRevisionId: null,
                        batch.SourceOperationCount,
                        batch.Requests.Count,
                        attempt,
                        "CANCELED_DURING_RETRY_DELAY",
                        canceled.Message,
                        batch.Traces,
                        operationIds));
                }
            }
            catch (Exception exception)
            {
                return LogSafe(new PhysicalUpdateExecutionResult(
                    PhysicalUpdateExecutionStatus.Rejected,
                    batch.DocumentId,
                    batch.RequiredRevisionId,
                    appliedRevisionId: null,
                    batch.SourceOperationCount,
                    batch.Requests.Count,
                    attempt,
                    "UNCLASSIFIED_FAILURE",
                    exception.Message,
                    batch.Traces,
                    operationIds));
            }
        }

        throw new InvalidOperationException("The physical update retry loop terminated unexpectedly.");
    }

    private static void ValidatePolicy(PhysicalUpdateRetryPolicy policy)
    {
        if (policy.MaxAttempts < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(policy), "MaxAttempts must be at least one.");
        }

        if (policy.InitialDelay < TimeSpan.Zero ||
            policy.MaxDelay < TimeSpan.Zero ||
            policy.BackoffFactor < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(policy), "Retry delay values are invalid.");
        }
    }

    private static void ValidatePlan(PhysicalUpdatePlan? plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentException.ThrowIfNullOrWhiteSpace(plan.Identity.GoogleDocumentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(plan.RequiredRevision.RevisionId);

        for (var index = 0; index < plan.Operations.Count; index++)
        {
            var operation = plan.Operations[index];
            if (operation.Sequence != index)
            {
                throw new PhysicalUpdateException(
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Physical operation sequence values must be contiguous.");
            }

            if (operation.Kind == PhysicalOperationKind.InsertBlock && operation.CandidateBlock is null)
            {
                throw new PhysicalUpdateException(
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Insert operations require canonical candidate payload.");
            }

            if ((operation.Kind is PhysicalOperationKind.ReplaceInlineContent or
                    PhysicalOperationKind.InsertInlineText or
                    PhysicalOperationKind.UpdateInlineStyle) &&
                operation.InlineUpdate is null)
            {
                throw new PhysicalUpdateException(
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Inline operations require inline update payload.");
            }

            if (operation.Kind == PhysicalOperationKind.DeleteRange && operation.AffectedRange.Length == 0)
            {
                throw new PhysicalUpdateException(
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Delete operations require a non-empty range.");
            }
        }
    }

    private static PhysicalUpdateExecutionStatus Classify(GoogleDocsBatchUpdateException exception)
    {
        if (IsRevisionConflict(exception))
        {
            return PhysicalUpdateExecutionStatus.RevisionConflict;
        }

        if (IsPermanentRejection(exception))
        {
            return PhysicalUpdateExecutionStatus.Rejected;
        }

        if (IsRetryableNotSent(exception))
        {
            return PhysicalUpdateExecutionStatus.TransientFailure;
        }

        if (exception.DeliveryState is RequestDeliveryState.Sent or RequestDeliveryState.Unknown)
        {
            return PhysicalUpdateExecutionStatus.IndeterminateFailure;
        }

        return PhysicalUpdateExecutionStatus.Rejected;
    }

    private static bool IsRevisionConflict(GoogleDocsBatchUpdateException exception) =>
        (exception.HttpStatusCode == HttpStatusCode.BadRequest &&
        string.Equals(exception.GoogleErrorReason, "FAILED_PRECONDITION", StringComparison.OrdinalIgnoreCase)) ||
        string.Equals(exception.GoogleErrorReason, "ABORTED", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(exception.GoogleErrorReason, "revisionMismatch", StringComparison.OrdinalIgnoreCase);

    private static bool IsPermanentRejection(GoogleDocsBatchUpdateException exception) =>
        (exception.HttpStatusCode is HttpStatusCode.BadRequest or
            HttpStatusCode.Unauthorized or
            HttpStatusCode.Forbidden or
            HttpStatusCode.NotFound or
            HttpStatusCode.Conflict) &&
        !IsRevisionConflict(exception);

    private static bool IsRetryableNotSent(GoogleDocsBatchUpdateException exception) =>
        exception.DeliveryState == RequestDeliveryState.NotSent &&
        exception.HttpStatusCode is HttpStatusCode.TooManyRequests or HttpStatusCode.ServiceUnavailable;

    private static bool IsCancellationBeforeSend(GoogleDocsBatchUpdateException exception) =>
        exception.DeliveryState == RequestDeliveryState.NotSent &&
        string.Equals(exception.GoogleErrorReason, "CANCELED", StringComparison.OrdinalIgnoreCase);

    private PhysicalUpdateExecutionResult Result(
        PhysicalUpdateRequestBatch batch,
        PhysicalUpdateExecutionStatus status,
        int attempt,
        GoogleDocsBatchUpdateException exception,
        IEnumerable<string> operationIds)
    {
        var code = status switch
        {
            PhysicalUpdateExecutionStatus.RevisionConflict => "REVISION_CONFLICT",
            PhysicalUpdateExecutionStatus.Rejected => "REQUEST_REJECTED",
            PhysicalUpdateExecutionStatus.TransientFailure => "TRANSIENT_FAILURE",
            PhysicalUpdateExecutionStatus.IndeterminateFailure => "INDETERMINATE_FAILURE",
            _ => "EXECUTION_FAILURE",
        };
        return new PhysicalUpdateExecutionResult(
            status,
            batch.DocumentId,
            batch.RequiredRevisionId,
            appliedRevisionId: null,
            batch.SourceOperationCount,
            batch.Requests.Count,
            attempt,
            code,
            exception.Message,
            batch.Traces,
            operationIds);
    }

    private TimeSpan NextDelay(int completedAttempt, TimeSpan? retryAfter)
    {
        if (retryAfter is { } explicitDelay)
        {
            return explicitDelay <= retryPolicy.MaxDelay ? explicitDelay : retryPolicy.MaxDelay;
        }

        var milliseconds = retryPolicy.InitialDelay.TotalMilliseconds *
            Math.Pow(retryPolicy.BackoffFactor, Math.Max(0, completedAttempt - 1));
        milliseconds = Math.Min(milliseconds, retryPolicy.MaxDelay.TotalMilliseconds);
        if (retryPolicy.UseJitter && milliseconds > 0)
        {
            milliseconds *= 0.5 + random.NextDouble();
        }

        return TimeSpan.FromMilliseconds(Math.Min(milliseconds, retryPolicy.MaxDelay.TotalMilliseconds));
    }

    private PhysicalUpdateExecutionResult LogSafe(PhysicalUpdateExecutionResult result)
    {
        try
        {
            resultLogger?.Invoke(result);
        }
        catch
        {
            // Diagnostics must never change execution classification.
        }

        return result;
    }
}
