using System.Net;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Executes physical plans through Google Docs documents.batchUpdate.</summary>
public sealed class GoogleDocsPhysicalPlanExecutor : IPhysicalPlanExecutor
{
    private readonly IGoogleDocsBatchRequestBuilder builder;
    private readonly IGoogleDocsGateway gateway;

    /// <summary>Initializes a Google Docs physical-plan executor.</summary>
    public GoogleDocsPhysicalPlanExecutor(
        IGoogleDocsBatchRequestBuilder builder,
        IGoogleDocsGateway gateway)
    {
        this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
        this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
    }

    /// <inheritdoc />
    public async Task<ApplyResult> ExecuteAsync(
        PhysicalUpdatePlan plan,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        ValidatePlan(plan);
        cancellationToken.ThrowIfCancellationRequested();

        var documentId = plan.Identity.GoogleDocumentId!;
        var requiredRevisionId = plan.RequiredRevision.RevisionId;
        if (!plan.IsPublishRequired)
        {
            return new ApplyResult(
                PhysicalUpdateExecutionStatus.NoChange,
                dryRun,
                documentId,
                requiredRevisionId,
                appliedRevisionId: null,
                plannedOperationCount: 0,
                submittedRequestCount: 0,
                errorCode: null,
                "The physical plan contains no operations.");
        }

        BatchUpdateDocumentRequest request;
        try
        {
            request = builder.Build(plan);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Failed(
                PhysicalUpdateExecutionStatus.Rejected,
                dryRun,
                documentId,
                requiredRevisionId,
                plan.Operations.Count,
                submittedRequestCount: 0,
                UpdateErrorCodes.PhysicalPlanInvalid,
                exception.Message);
        }

        if (!string.Equals(
            requiredRevisionId,
            request.WriteControl.RequiredRevisionId,
            StringComparison.Ordinal))
        {
            return Failed(
                PhysicalUpdateExecutionStatus.Rejected,
                dryRun,
                documentId,
                requiredRevisionId,
                plan.Operations.Count,
                request.Requests.Count,
                UpdateErrorCodes.PhysicalPlanInvalid,
                "The batchUpdate write control required revision does not match the physical plan.");
        }

        if (dryRun)
        {
            return new ApplyResult(
                PhysicalUpdateExecutionStatus.NoChange,
                dryRun: true,
                documentId,
                requiredRevisionId,
                appliedRevisionId: null,
                plan.Operations.Count,
                request.Requests.Count,
                errorCode: null,
                "The batchUpdate request was built but not sent because dry-run is enabled.");
        }

        try
        {
            var response = await gateway.BatchUpdateDocumentAsync(documentId, request, cancellationToken)
                .ConfigureAwait(false);
            return new ApplyResult(
                PhysicalUpdateExecutionStatus.Applied,
                dryRun: false,
                documentId,
                requiredRevisionId,
                response.RevisionId,
                plan.Operations.Count,
                request.Requests.Count,
                errorCode: null,
                "The batchUpdate request was accepted.");
        }
        catch (GoogleDocsBatchUpdateException exception)
        {
            var status = IsRevisionConflict(exception)
                ? PhysicalUpdateExecutionStatus.RevisionConflict
                : PhysicalUpdateExecutionStatus.Rejected;
            return Failed(
                status,
                dryRun: false,
                documentId,
                requiredRevisionId,
                plan.Operations.Count,
                request.Requests.Count,
                status == PhysicalUpdateExecutionStatus.RevisionConflict
                    ? UpdateErrorCodes.RevisionConflict
                    : UpdateErrorCodes.ApplicationFailed,
                exception.Message,
                exception.DeliveryState);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Failed(
                PhysicalUpdateExecutionStatus.Rejected,
                dryRun: false,
                documentId,
                requiredRevisionId,
                plan.Operations.Count,
                request.Requests.Count,
                UpdateErrorCodes.ApplicationFailed,
                exception.Message);
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
        }
    }

    private static bool IsRevisionConflict(GoogleDocsBatchUpdateException exception) =>
        (exception.HttpStatusCode == HttpStatusCode.BadRequest &&
            string.Equals(exception.GoogleErrorReason, "FAILED_PRECONDITION", StringComparison.OrdinalIgnoreCase)) ||
        string.Equals(exception.GoogleErrorReason, "ABORTED", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(exception.GoogleErrorReason, "revisionMismatch", StringComparison.OrdinalIgnoreCase);

    private static ApplyResult Failed(
        PhysicalUpdateExecutionStatus status,
        bool dryRun,
        string documentId,
        string requiredRevisionId,
        int plannedOperationCount,
        int submittedRequestCount,
        string errorCode,
        string message,
        RequestDeliveryState? deliveryState = null) => new(
            status,
            dryRun,
            documentId,
            requiredRevisionId,
            appliedRevisionId: null,
            plannedOperationCount,
            submittedRequestCount,
            errorCode,
            message,
            deliveryState);
}
