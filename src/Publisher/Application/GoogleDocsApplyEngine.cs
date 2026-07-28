using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Executes one physical operation through an operation-specific boundary.</summary>
public interface IPhysicalOperationExecutor
{
    /// <summary>Gets the physical operation kind handled by this executor.</summary>
    PhysicalOperationKind OperationKind { get; }

    /// <summary>Executes one physical operation.</summary>
    Task<PhysicalOperationExecutionResult> ExecuteAsync(
        PhysicalUpdatePlan plan,
        PhysicalUpdateOperation operation,
        int operationIndex,
        CancellationToken cancellationToken);
}

/// <summary>Represents the structured result of executing one physical operation.</summary>
public sealed class PhysicalOperationExecutionResult
{
    /// <summary>Initializes an operation execution result.</summary>
    public PhysicalOperationExecutionResult(
        bool success,
        int operationIndex,
        string operationIdentity,
        PhysicalOperationKind operationKind,
        string? errorCode,
        string message)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(operationIndex);
        ArgumentException.ThrowIfNullOrWhiteSpace(operationIdentity);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        if (!success && string.IsNullOrWhiteSpace(errorCode))
        {
            throw new ArgumentException("Failed operation results require an error code.", nameof(errorCode));
        }

        Success = success;
        OperationIndex = operationIndex;
        OperationIdentity = operationIdentity;
        OperationKind = operationKind;
        ErrorCode = errorCode;
        Message = message;
    }

    /// <summary>Gets whether the operation succeeded.</summary>
    public bool Success { get; }

    /// <summary>Gets the zero-based operation index from the physical plan.</summary>
    public int OperationIndex { get; }

    /// <summary>Gets the executed operation identity.</summary>
    public string OperationIdentity { get; }

    /// <summary>Gets the physical operation kind.</summary>
    public PhysicalOperationKind OperationKind { get; }

    /// <summary>Gets the stable error code when the operation failed.</summary>
    public string? ErrorCode { get; }

    /// <summary>Gets the safe diagnostic message.</summary>
    public string Message { get; }

    /// <summary>Creates a successful operation result.</summary>
    public static PhysicalOperationExecutionResult Succeeded(
        PhysicalUpdateOperation operation,
        int operationIndex) => new(
            success: true,
            operationIndex,
            operation.OperationId,
            operation.Kind,
            errorCode: null,
            "The physical operation was applied.");

    /// <summary>Creates a failed operation result.</summary>
    public static PhysicalOperationExecutionResult Failed(
        PhysicalUpdateOperation operation,
        int operationIndex,
        string errorCode,
        string message) => new(
            success: false,
            operationIndex,
            operation.OperationId,
            operation.Kind,
            errorCode,
            message);
}

/// <summary>Selects the executor registered for each physical operation kind.</summary>
public sealed class PhysicalOperationDispatcher
{
    private readonly IReadOnlyDictionary<PhysicalOperationKind, IPhysicalOperationExecutor> executors;

    /// <summary>Initializes a dispatcher from operation executors.</summary>
    public PhysicalOperationDispatcher(IEnumerable<IPhysicalOperationExecutor> executors)
    {
        ArgumentNullException.ThrowIfNull(executors);
        var items = executors.ToArray();
        if (items.Any(item => item is null))
        {
            throw new ArgumentException("Operation executors must not contain null items.", nameof(executors));
        }

        var duplicate = items
            .GroupBy(item => item.OperationKind)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.DuplicateOperationExecutor,
                $"Multiple executors are registered for physical operation kind {duplicate.Key}.");
        }

        this.executors = items.ToDictionary(item => item.OperationKind);
    }

    /// <summary>Dispatches one operation without changing physical-plan order.</summary>
    public async Task<PhysicalOperationExecutionResult> DispatchAsync(
        PhysicalUpdatePlan plan,
        PhysicalUpdateOperation operation,
        int operationIndex,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(operation);
        if (!executors.TryGetValue(operation.Kind, out var executor))
        {
            return PhysicalOperationExecutionResult.Failed(
                operation,
                operationIndex,
                UpdateErrorCodes.OperationExecutorNotRegistered,
                $"No physical operation executor is registered for kind {operation.Kind}.");
        }

        try
        {
            var result = await executor.ExecuteAsync(plan, operation, operationIndex, cancellationToken)
                .ConfigureAwait(false);
            return result ?? PhysicalOperationExecutionResult.Failed(
                operation,
                operationIndex,
                UpdateErrorCodes.OperationExecutorFailed,
                "The physical operation executor returned no result.");
        }
        catch (PhysicalUpdateException exception)
        {
            return PhysicalOperationExecutionResult.Failed(
                operation,
                operationIndex,
                exception.Code,
                exception.Message);
        }
        catch (OperationCanceledException exception)
        {
            return PhysicalOperationExecutionResult.Failed(
                operation,
                operationIndex,
                UpdateErrorCodes.OperationExecutorFailed,
                exception.Message);
        }
        catch (Exception exception)
        {
            return PhysicalOperationExecutionResult.Failed(
                operation,
                operationIndex,
                UpdateErrorCodes.OperationExecutorFailed,
                exception.Message);
        }
    }
}

/// <summary>Represents the structured result of an apply-engine run.</summary>
public sealed class GoogleDocsApplyResult
{
    /// <summary>Initializes an apply result.</summary>
    public GoogleDocsApplyResult(
        bool success,
        bool dryRun,
        bool conflict,
        string? errorCode,
        string message,
        int plannedOperationCount,
        int appliedOperationCount,
        int? failedOperationIndex,
        string? failedOperationIdentity,
        bool verifiedStateCommitAllowed)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentOutOfRangeException.ThrowIfNegative(plannedOperationCount);
        ArgumentOutOfRangeException.ThrowIfNegative(appliedOperationCount);
        if (failedOperationIndex is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(failedOperationIndex));
        }

        Success = success;
        DryRun = dryRun;
        Conflict = conflict;
        ErrorCode = errorCode;
        Message = message;
        PlannedOperationCount = plannedOperationCount;
        AppliedOperationCount = appliedOperationCount;
        FailedOperationIndex = failedOperationIndex;
        FailedOperationIdentity = failedOperationIdentity;
        VerifiedStateCommitAllowed = verifiedStateCommitAllowed;
    }

    /// <summary>Gets whether the apply-engine run succeeded.</summary>
    public bool Success { get; }

    /// <summary>Gets whether the run was a dry-run.</summary>
    public bool DryRun { get; }

    /// <summary>Gets whether the plan was rejected by revision conflict.</summary>
    public bool Conflict { get; }

    /// <summary>Gets the stable error code when the run failed.</summary>
    public string? ErrorCode { get; }

    /// <summary>Gets the safe diagnostic message.</summary>
    public string Message { get; }

    /// <summary>Gets the number of operations in the physical plan.</summary>
    public int PlannedOperationCount { get; }

    /// <summary>Gets the number of operations executed successfully.</summary>
    public int AppliedOperationCount { get; }

    /// <summary>Gets the zero-based failed operation index, when applicable.</summary>
    public int? FailedOperationIndex { get; }

    /// <summary>Gets the failed operation identity, when applicable.</summary>
    public string? FailedOperationIdentity { get; }

    /// <summary>Gets whether a verified-state commit is allowed by this result.</summary>
    public bool VerifiedStateCommitAllowed { get; }
}

/// <summary>Validates and applies Google Docs physical plans through operation dispatch.</summary>
public sealed class GoogleDocsApplyEngine
{
    private readonly IDocumentSnapshotReader snapshotReader;
    private readonly PhysicalOperationDispatcher dispatcher;

    /// <summary>Initializes an apply engine.</summary>
    public GoogleDocsApplyEngine(
        IDocumentSnapshotReader snapshotReader,
        PhysicalOperationDispatcher dispatcher)
    {
        this.snapshotReader = snapshotReader ?? throw new ArgumentNullException(nameof(snapshotReader));
        this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    /// <summary>Applies or previews one revision-bound physical plan.</summary>
    public async Task<GoogleDocsApplyResult> ApplyAsync(
        PhysicalUpdatePlan plan,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(plan);
        cancellationToken.ThrowIfCancellationRequested();
        var plannedCount = plan.Operations.Count;

        ManagedDocumentSnapshot current;
        try
        {
            current = await snapshotReader.GetSnapshotAsync(plan.Identity, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException exception)
        {
            return Failed(
                dryRun,
                plannedCount,
                appliedCount: 0,
                failedIndex: null,
                failedIdentity: null,
                UpdateErrorCodes.ReadbackFailed,
                exception.Message);
        }
        catch (Exception exception)
        {
            return Failed(
                dryRun,
                plannedCount,
                appliedCount: 0,
                failedIndex: null,
                failedIdentity: null,
                UpdateErrorCodes.ReadbackFailed,
                exception.Message);
        }

        if (!plan.RequiredRevision.Equals(current.Revision))
        {
            return new GoogleDocsApplyResult(
                success: false,
                dryRun,
                conflict: true,
                UpdateErrorCodes.RevisionConflict,
                "The current document revision differs from the physical-plan precondition.",
                plannedCount,
                appliedOperationCount: 0,
                failedOperationIndex: null,
                failedOperationIdentity: null,
                verifiedStateCommitAllowed: false);
        }

        var validation = ValidatePlan(plan);
        if (validation is not null)
        {
            return Failed(
                dryRun,
                plannedCount,
                appliedCount: 0,
                validation.OperationIndex,
                validation.OperationIdentity,
                validation.ErrorCode!,
                validation.Message);
        }

        if (dryRun)
        {
            return new GoogleDocsApplyResult(
                success: true,
                dryRun: true,
                conflict: false,
                errorCode: null,
                "The physical plan is valid and was not applied because dry-run is enabled.",
                plannedCount,
                appliedOperationCount: 0,
                failedOperationIndex: null,
                failedOperationIdentity: null,
                verifiedStateCommitAllowed: false);
        }

        if (plannedCount == 0)
        {
            return new GoogleDocsApplyResult(
                success: true,
                dryRun: false,
                conflict: false,
                errorCode: null,
                "The physical plan contains no operations.",
                plannedCount,
                appliedOperationCount: 0,
                failedOperationIndex: null,
                failedOperationIdentity: null,
                verifiedStateCommitAllowed: true);
        }

        var appliedCount = 0;
        for (var index = 0; index < plan.Operations.Count; index++)
        {
            var operation = plan.Operations[index];
            var result = await dispatcher.DispatchAsync(plan, operation, index, cancellationToken)
                .ConfigureAwait(false);
            if (!result.Success)
            {
                return Failed(
                    dryRun,
                    plannedCount,
                    appliedCount,
                    result.OperationIndex,
                    result.OperationIdentity,
                    result.ErrorCode ?? UpdateErrorCodes.OperationExecutorFailed,
                    result.Message);
            }

            appliedCount++;
        }

        return new GoogleDocsApplyResult(
            success: true,
            dryRun: false,
            conflict: false,
            errorCode: null,
            "All physical operations were applied.",
            plannedCount,
            appliedCount,
            failedOperationIndex: null,
            failedOperationIdentity: null,
            verifiedStateCommitAllowed: true);
    }

    private static PhysicalOperationExecutionResult? ValidatePlan(PhysicalUpdatePlan plan)
    {
        for (var index = 0; index < plan.Operations.Count; index++)
        {
            var operation = plan.Operations[index];
            if (operation.Sequence != index)
            {
                return PhysicalOperationExecutionResult.Failed(
                    operation,
                    index,
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Physical operation sequence values must be contiguous.");
            }

            if (operation.Kind == PhysicalOperationKind.InsertBlock && operation.CandidateBlock is null)
            {
                return PhysicalOperationExecutionResult.Failed(
                    operation,
                    index,
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Insert operations require canonical candidate payload.");
            }

            if ((operation.Kind is PhysicalOperationKind.ReplaceInlineContent or
                    PhysicalOperationKind.InsertInlineText or
                    PhysicalOperationKind.UpdateInlineStyle) &&
                operation.InlineUpdate is null)
            {
                return PhysicalOperationExecutionResult.Failed(
                    operation,
                    index,
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Inline operations require inline update payload.");
            }

            if (operation.Kind == PhysicalOperationKind.DeleteRange && operation.AffectedRange.Length == 0)
            {
                return PhysicalOperationExecutionResult.Failed(
                    operation,
                    index,
                    UpdateErrorCodes.PhysicalPlanInvalid,
                    "Delete operations require a non-empty range.");
            }
        }

        return null;
    }

    private static GoogleDocsApplyResult Failed(
        bool dryRun,
        int plannedCount,
        int appliedCount,
        int? failedIndex,
        string? failedIdentity,
        string errorCode,
        string message) => new(
            success: false,
            dryRun,
            conflict: false,
            errorCode,
            message,
            plannedCount,
            appliedCount,
            failedIndex,
            failedIdentity,
            verifiedStateCommitAllowed: false);
}
