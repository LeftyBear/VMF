using System.Net;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Represents the terminal status of a revision-bound physical update execution.</summary>
public enum PhysicalUpdateExecutionStatus
{
    /// <summary>The update was accepted by the remote API.</summary>
    Applied,

    /// <summary>The plan contained no physical operations.</summary>
    NoChange,

    /// <summary>The required revision no longer matches the remote document.</summary>
    RevisionConflict,

    /// <summary>The API rejected the request permanently.</summary>
    Rejected,

    /// <summary>The request was not sent and may be retried by policy.</summary>
    TransientFailure,

    /// <summary>The request may have been applied and must be reconciled before retry.</summary>
    IndeterminateFailure,
}

/// <summary>Represents whether a failed request reached Google Docs.</summary>
public enum RequestDeliveryState
{
    /// <summary>The request was not sent.</summary>
    NotSent,

    /// <summary>The request was sent.</summary>
    Sent,

    /// <summary>The caller cannot determine whether the request was sent.</summary>
    Unknown,
}

/// <summary>Represents one Google Docs request and its source physical operation.</summary>
public sealed class PhysicalUpdateRequestTrace
{
    /// <summary>Initializes a request trace.</summary>
    public PhysicalUpdateRequestTrace(
        int requestIndex,
        int sourceOperationIndex,
        PhysicalOperationReason operationKind,
        BlockIdentity blockIdentity,
        string requestKind)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(requestIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(sourceOperationIndex);
        ArgumentException.ThrowIfNullOrWhiteSpace(requestKind);
        RequestIndex = requestIndex;
        SourceOperationIndex = sourceOperationIndex;
        OperationKind = operationKind;
        BlockIdentity = blockIdentity ?? throw new ArgumentNullException(nameof(blockIdentity));
        RequestKind = requestKind;
    }

    /// <summary>Gets the zero-based Google Docs request index.</summary>
    public int RequestIndex { get; }

    /// <summary>Gets the source physical operation sequence.</summary>
    public int SourceOperationIndex { get; }

    /// <summary>Gets the logical operation reason.</summary>
    public PhysicalOperationReason OperationKind { get; }

    /// <summary>Gets the traced block identity.</summary>
    public BlockIdentity BlockIdentity { get; }

    /// <summary>Gets the Google Docs request kind.</summary>
    public string RequestKind { get; }
}

/// <summary>Represents one immutable Google Docs batchUpdate request batch.</summary>
public sealed class PhysicalUpdateRequestBatch
{
    /// <summary>Initializes a request batch.</summary>
    public PhysicalUpdateRequestBatch(
        string documentId,
        string requiredRevisionId,
        IEnumerable<object> requests,
        int sourceOperationCount,
        IEnumerable<PhysicalUpdateRequestTrace> traces)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredRevisionId);
        ArgumentNullException.ThrowIfNull(requests);
        ArgumentNullException.ThrowIfNull(traces);
        ArgumentOutOfRangeException.ThrowIfNegative(sourceOperationCount);
        var requestItems = requests.ToArray();
        var traceItems = traces.ToArray();
        if (requestItems.Any(item => item is null))
        {
            throw new ArgumentException("Requests must not contain null items.", nameof(requests));
        }

        if (traceItems.Any(item => item is null))
        {
            throw new ArgumentException("Traces must not contain null items.", nameof(traces));
        }

        if (requestItems.Length != traceItems.Length)
        {
            throw new ArgumentException("Request count and trace count must match.", nameof(traces));
        }

        DocumentId = documentId;
        RequiredRevisionId = requiredRevisionId;
        Requests = Array.AsReadOnly(requestItems);
        SourceOperationCount = sourceOperationCount;
        Traces = Array.AsReadOnly(traceItems);
    }

    /// <summary>Gets the Google Docs document identifier.</summary>
    public string DocumentId { get; }

    /// <summary>Gets the required Google Docs revision identifier.</summary>
    public string RequiredRevisionId { get; }

    /// <summary>Gets the Google Docs requests.</summary>
    public IReadOnlyList<object> Requests { get; }

    /// <summary>Gets the number of source physical operations.</summary>
    public int SourceOperationCount { get; }

    /// <summary>Gets the request traces.</summary>
    public IReadOnlyList<PhysicalUpdateRequestTrace> Traces { get; }
}

/// <summary>Represents a successful Google Docs batchUpdate response.</summary>
public sealed class GoogleDocsBatchUpdateResponse
{
    /// <summary>Initializes a batchUpdate response.</summary>
    public GoogleDocsBatchUpdateResponse(string revisionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(revisionId);
        RevisionId = revisionId;
    }

    /// <summary>Gets the revision returned after update application.</summary>
    public string RevisionId { get; }
}

/// <summary>Normalizes Google API failures for deterministic classification.</summary>
public sealed class GoogleDocsBatchUpdateException : Exception
{
    /// <summary>Initializes a normalized Google Docs batchUpdate exception.</summary>
    public GoogleDocsBatchUpdateException(
        HttpStatusCode? httpStatusCode,
        string? googleErrorReason,
        TimeSpan? retryAfter,
        RequestDeliveryState deliveryState,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        HttpStatusCode = httpStatusCode;
        GoogleErrorReason = googleErrorReason;
        RetryAfter = retryAfter;
        DeliveryState = deliveryState;
    }

    /// <summary>Gets the HTTP status, when available.</summary>
    public HttpStatusCode? HttpStatusCode { get; }

    /// <summary>Gets the Google error reason, when available.</summary>
    public string? GoogleErrorReason { get; }

    /// <summary>Gets the Retry-After delay, when supplied.</summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>Gets whether the request reached Google Docs.</summary>
    public RequestDeliveryState DeliveryState { get; }
}

/// <summary>Controls retry delays for physical update execution.</summary>
public sealed record PhysicalUpdateRetryPolicy(
    int MaxAttempts,
    TimeSpan InitialDelay,
    TimeSpan MaxDelay,
    double BackoffFactor,
    bool UseJitter)
{
    /// <summary>Gets the default retry policy.</summary>
    public static PhysicalUpdateRetryPolicy Default { get; } = new(
        3,
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromSeconds(5),
        2,
        true);
}

/// <summary>Waits asynchronously between retry attempts.</summary>
public interface IAsyncDelay
{
    /// <summary>Waits for the supplied delay.</summary>
    Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken);
}

/// <summary>Maps a physical update plan to Google Docs requests without I/O.</summary>
public interface IPhysicalUpdateRequestMapper
{
    /// <summary>Maps a plan to one request batch.</summary>
    PhysicalUpdateRequestBatch Map(PhysicalUpdatePlan plan);
}

/// <summary>Executes a Google Docs batchUpdate request batch.</summary>
public interface IGoogleDocsBatchUpdateClient
{
    /// <summary>Executes one batchUpdate request.</summary>
    Task<GoogleDocsBatchUpdateResponse> ExecuteAsync(
        PhysicalUpdateRequestBatch batch,
        CancellationToken cancellationToken);
}

/// <summary>Represents a Google Docs batchUpdate request body.</summary>
public sealed class BatchUpdateDocumentRequest
{
    /// <summary>Initializes a batchUpdate request body.</summary>
    public BatchUpdateDocumentRequest(
        IEnumerable<object> requests,
        BatchUpdateWriteControl writeControl)
    {
        ArgumentNullException.ThrowIfNull(requests);
        WriteControl = writeControl ?? throw new ArgumentNullException(nameof(writeControl));
        var items = requests.ToArray();
        if (items.Any(item => item is null))
        {
            throw new ArgumentException("Requests must not contain null items.", nameof(requests));
        }

        Requests = Array.AsReadOnly(items);
    }

    /// <summary>Gets the Google Docs request sequence.</summary>
    public IReadOnlyList<object> Requests { get; }

    /// <summary>Gets the optimistic-concurrency write control.</summary>
    public BatchUpdateWriteControl WriteControl { get; }
}

/// <summary>Represents Google Docs batchUpdate write control.</summary>
public sealed class BatchUpdateWriteControl
{
    /// <summary>Initializes write control.</summary>
    public BatchUpdateWriteControl(string requiredRevisionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredRevisionId);
        RequiredRevisionId = requiredRevisionId;
    }

    /// <summary>Gets the required revision ID.</summary>
    public string RequiredRevisionId { get; }
}

/// <summary>Represents the gateway response for Google Docs batchUpdate.</summary>
public sealed class BatchUpdateDocumentResponse
{
    /// <summary>Initializes a batchUpdate response.</summary>
    public BatchUpdateDocumentResponse(string revisionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(revisionId);
        RevisionId = revisionId;
    }

    /// <summary>Gets the revision returned after the update.</summary>
    public string RevisionId { get; }
}

/// <summary>Executes Google Docs API operations through the infrastructure gateway boundary.</summary>
public interface IGoogleDocsGateway
{
    /// <summary>Executes documents.batchUpdate.</summary>
    Task<BatchUpdateDocumentResponse> BatchUpdateDocumentAsync(
        string documentId,
        BatchUpdateDocumentRequest request,
        CancellationToken cancellationToken);
}

/// <summary>Builds a Google Docs batchUpdate request from a physical plan without I/O.</summary>
public interface IGoogleDocsBatchRequestBuilder
{
    /// <summary>Builds one batchUpdate request.</summary>
    BatchUpdateDocumentRequest Build(PhysicalUpdatePlan plan);
}

/// <summary>Executes a revision-bound physical plan.</summary>
public interface IPhysicalPlanExecutor
{
    /// <summary>Applies or previews one physical plan.</summary>
    Task<ApplyResult> ExecuteAsync(
        PhysicalUpdatePlan plan,
        bool dryRun,
        CancellationToken cancellationToken);
}

/// <summary>Represents the result of physical-plan application.</summary>
public sealed class ApplyResult
{
    /// <summary>Initializes an apply result.</summary>
    public ApplyResult(
        PhysicalUpdateExecutionStatus status,
        bool dryRun,
        string documentId,
        string requiredRevisionId,
        string? appliedRevisionId,
        int plannedOperationCount,
        int submittedRequestCount,
        string? errorCode,
        string message,
        RequestDeliveryState? deliveryState = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredRevisionId);
        ArgumentOutOfRangeException.ThrowIfNegative(plannedOperationCount);
        ArgumentOutOfRangeException.ThrowIfNegative(submittedRequestCount);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Status = status;
        DryRun = dryRun;
        DocumentId = documentId;
        RequiredRevisionId = requiredRevisionId;
        AppliedRevisionId = appliedRevisionId;
        PlannedOperationCount = plannedOperationCount;
        SubmittedRequestCount = submittedRequestCount;
        ErrorCode = errorCode;
        Message = message;
        DeliveryState = deliveryState;
    }

    /// <summary>Gets the terminal status.</summary>
    public PhysicalUpdateExecutionStatus Status { get; }

    /// <summary>Gets whether the executor ran as a non-mutating preview.</summary>
    public bool DryRun { get; }

    /// <summary>Gets the target document ID.</summary>
    public string DocumentId { get; }

    /// <summary>Gets the required revision ID sent in write control.</summary>
    public string RequiredRevisionId { get; }

    /// <summary>Gets the applied revision ID, when known.</summary>
    public string? AppliedRevisionId { get; }

    /// <summary>Gets the number of source physical operations.</summary>
    public int PlannedOperationCount { get; }

    /// <summary>Gets the number of Google Docs requests submitted or previewed.</summary>
    public int SubmittedRequestCount { get; }

    /// <summary>Gets the stable error code, when execution failed.</summary>
    public string? ErrorCode { get; }

    /// <summary>Gets a safe diagnostic message.</summary>
    public string Message { get; }

    /// <summary>Gets the request delivery state carried from a failed batchUpdate request, when known.</summary>
    public RequestDeliveryState? DeliveryState { get; }

    /// <summary>Gets whether the plan was applied.</summary>
    public bool Applied => Status == PhysicalUpdateExecutionStatus.Applied;

    /// <summary>Gets whether execution stopped because the required revision mismatched.</summary>
    public bool Conflict => Status == PhysicalUpdateExecutionStatus.RevisionConflict;
}

/// <summary>Applies a physical update plan through a deterministic execution boundary.</summary>
public interface IPhysicalUpdateExecutor
{
    /// <summary>Executes one physical update plan.</summary>
    Task<PhysicalUpdateExecutionResult> ExecuteAsync(
        PhysicalUpdatePlan plan,
        CancellationToken cancellationToken);
}

/// <summary>Represents the result of physical update execution.</summary>
public sealed class PhysicalUpdateExecutionResult
{
    /// <summary>Initializes an execution result.</summary>
    public PhysicalUpdateExecutionResult(
        PhysicalUpdateExecutionStatus status,
        string documentId,
        string requiredRevisionId,
        string? appliedRevisionId,
        int submittedOperationCount,
        int submittedRequestCount,
        int attemptCount,
        string diagnosticCode,
        string diagnosticMessage,
        IEnumerable<PhysicalUpdateRequestTrace>? requestTraces = null,
        IEnumerable<string>? operationIds = null,
        RequestDeliveryState? deliveryState = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredRevisionId);
        ArgumentOutOfRangeException.ThrowIfNegative(submittedOperationCount);
        ArgumentOutOfRangeException.ThrowIfNegative(submittedRequestCount);
        ArgumentOutOfRangeException.ThrowIfNegative(attemptCount);
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnosticCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnosticMessage);
        Status = status;
        DocumentId = documentId;
        RequiredRevisionId = requiredRevisionId;
        AppliedRevisionId = appliedRevisionId;
        SubmittedOperationCount = submittedOperationCount;
        SubmittedRequestCount = submittedRequestCount;
        AttemptCount = attemptCount;
        DiagnosticCode = diagnosticCode;
        DiagnosticMessage = diagnosticMessage;
        RequestTraces = Array.AsReadOnly((requestTraces ?? []).ToArray());
        OperationIds = Array.AsReadOnly((operationIds ?? []).ToArray());
        DeliveryState = deliveryState;
    }

    /// <summary>Gets the terminal status.</summary>
    public PhysicalUpdateExecutionStatus Status { get; }

    /// <summary>Gets the target document ID.</summary>
    public string DocumentId { get; }

    /// <summary>Gets the required revision ID.</summary>
    public string RequiredRevisionId { get; }

    /// <summary>Gets the applied revision ID, when known.</summary>
    public string? AppliedRevisionId { get; }

    /// <summary>Gets the number of source operations submitted.</summary>
    public int SubmittedOperationCount { get; }

    /// <summary>Gets the number of Google requests submitted.</summary>
    public int SubmittedRequestCount { get; }

    /// <summary>Gets the number of client attempts.</summary>
    public int AttemptCount { get; }

    /// <summary>Gets the stable diagnostic code.</summary>
    public string DiagnosticCode { get; }

    /// <summary>Gets the diagnostic message.</summary>
    public string DiagnosticMessage { get; }

    /// <summary>Gets request traces retained for diagnostics.</summary>
    public IReadOnlyList<PhysicalUpdateRequestTrace> RequestTraces { get; }

    /// <summary>Gets source operation identifiers retained for idempotent recovery diagnostics.</summary>
    public IReadOnlyList<string> OperationIds { get; }

    /// <summary>Gets the request delivery state carried from a failed batchUpdate request, when known.</summary>
    public RequestDeliveryState? DeliveryState { get; }
}

/// <summary>Provides production delay behavior.</summary>
public sealed class SystemAsyncDelay : IAsyncDelay
{
    /// <inheritdoc />
    public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) =>
        Task.Delay(delay, cancellationToken);
}
