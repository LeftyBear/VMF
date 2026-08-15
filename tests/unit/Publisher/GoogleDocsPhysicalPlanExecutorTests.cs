using System.Net;
using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsPhysicalPlanExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_DryRunBuildsRequestButDoesNotCallGateway()
    {
        var builder = new RecordingBuilder();
        var gateway = new RecordingGateway();
        var result = await Executor(builder, gateway).ExecuteAsync(Plan(Delete(0)), dryRun: true, default);

        Assert.Equal(PhysicalUpdateExecutionStatus.NoChange, result.Status);
        Assert.True(result.DryRun);
        Assert.Equal(1, builder.CallCount);
        Assert.Equal(0, gateway.CallCount);
        Assert.Equal("required-revision", result.RequiredRevisionId);
        Assert.Equal(1, result.SubmittedRequestCount);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyPlanDoesNotBuildOrCallGateway()
    {
        var builder = new RecordingBuilder();
        var gateway = new RecordingGateway();
        var result = await Executor(builder, gateway).ExecuteAsync(Plan(), dryRun: false, default);

        Assert.Equal(PhysicalUpdateExecutionStatus.NoChange, result.Status);
        Assert.Equal(0, builder.CallCount);
        Assert.Equal(0, gateway.CallCount);
    }

    [Fact]
    public async Task ExecuteAsync_SubmitsBatchUpdateThroughGatewayWithRequiredRevision()
    {
        var builder = new RecordingBuilder();
        var gateway = new RecordingGateway();
        var result = await Executor(builder, gateway).ExecuteAsync(Plan(Delete(0)), dryRun: false, default);

        Assert.True(result.Applied);
        Assert.Equal("applied-revision", result.AppliedRevisionId);
        Assert.Equal(1, gateway.CallCount);
        Assert.Equal("google-document", gateway.DocumentIds.Single());
        Assert.Equal("required-revision", gateway.Requests.Single().WriteControl.RequiredRevisionId);
        Assert.Single(gateway.Requests.Single().Requests);
    }

    [Fact]
    public async Task ExecuteAsync_RevisionConflictReturnsConflictWithoutThrowing()
    {
        var gateway = new RecordingGateway
        {
            Exception = new GoogleDocsBatchUpdateException(
                HttpStatusCode.BadRequest,
                "FAILED_PRECONDITION",
                retryAfter: null,
                RequestDeliveryState.Sent,
                "revision conflict"),
        };

        var result = await Executor(new RecordingBuilder(), gateway)
            .ExecuteAsync(Plan(Delete(0)), dryRun: false, default);

        Assert.True(result.Conflict);
        Assert.Equal(UpdateErrorCodes.RevisionConflict, result.ErrorCode);
        Assert.Equal(1, gateway.CallCount);
    }

    [Fact]
    public async Task ExecuteAsync_ApiFailureReturnsRejected()
    {
        var gateway = new RecordingGateway
        {
            Exception = new GoogleDocsBatchUpdateException(
                HttpStatusCode.Forbidden,
                "forbidden",
                retryAfter: null,
                RequestDeliveryState.Sent,
                "api failure"),
        };

        var result = await Executor(new RecordingBuilder(), gateway)
            .ExecuteAsync(Plan(Delete(0)), dryRun: false, default);

        Assert.Equal(PhysicalUpdateExecutionStatus.Rejected, result.Status);
        Assert.Equal(UpdateErrorCodes.ApplicationFailed, result.ErrorCode);
        Assert.False(result.Applied);
    }

    [Theory]
    [InlineData(RequestDeliveryState.NotSent)]
    [InlineData(RequestDeliveryState.Sent)]
    [InlineData(RequestDeliveryState.Unknown)]
    public async Task ExecuteAsync_ApiFailureCarriesDeliveryState(RequestDeliveryState deliveryState)
    {
        var gateway = new RecordingGateway
        {
            Exception = new GoogleDocsBatchUpdateException(
                HttpStatusCode.Forbidden,
                "forbidden",
                retryAfter: null,
                deliveryState,
                "api failure"),
        };

        var result = await Executor(new RecordingBuilder(), gateway)
            .ExecuteAsync(Plan(Delete(0)), dryRun: false, default);

        Assert.Equal(PhysicalUpdateExecutionStatus.Rejected, result.Status);
        Assert.Equal(deliveryState, result.DeliveryState);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsBuilderRevisionMismatch()
    {
        var builder = new RecordingBuilder(requiredRevisionId: "other-revision");
        var gateway = new RecordingGateway();
        var result = await Executor(builder, gateway).ExecuteAsync(Plan(Delete(0)), dryRun: false, default);

        Assert.Equal(PhysicalUpdateExecutionStatus.Rejected, result.Status);
        Assert.Equal(UpdateErrorCodes.PhysicalPlanInvalid, result.ErrorCode);
        Assert.Equal(0, gateway.CallCount);
    }

    private static GoogleDocsPhysicalPlanExecutor Executor(
        IGoogleDocsBatchRequestBuilder builder,
        IGoogleDocsGateway gateway) => new(builder, gateway);

    private static PhysicalUpdatePlan Plan(params PhysicalUpdateOperation[] operations) => new(
        Identity(),
        new DocumentRevision("required-revision", 1),
        new DocumentTextRange(10, 50),
        new DiffPlan(
            Fingerprint("old"),
            Fingerprint("new"),
            isFingerprintMatch: false,
            operations.Length == 0
                ? [new DiffOperation(
                    DiffOperationKind.NoChange,
                    0,
                    0,
                    Block("a"),
                    Block("a"),
                    BlockMatchKind.ExplicitId)]
                : operations.Select(operation => new DiffOperation(
                    DiffOperationKind.Update,
                    operation.PreviousIndex,
                    operation.CurrentIndex,
                    operation.TraceIdentity,
                    operation.TraceIdentity,
                    BlockMatchKind.ExplicitId))),
        operations);

    private static PhysicalUpdateOperation Delete(int sequence) => new(
        sequence,
        PhysicalOperationKind.DeleteRange,
        PhysicalOperationReason.Delete,
        sequence,
        null,
        new DocumentTextRange(10 + sequence, 11 + sequence),
        Block("block-" + sequence),
        null);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static BlockIdentity Block(string id) =>
        new(id, null, "ch-v1:sha256:" + Hash(id));

    private static PublishFingerprint Fingerprint(string value) =>
        new("v1:sha256:" + Hash(value));

    private static string Hash(string value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private sealed class RecordingBuilder : IGoogleDocsBatchRequestBuilder
    {
        private readonly string requiredRevisionId;

        internal RecordingBuilder(string requiredRevisionId = "required-revision")
        {
            this.requiredRevisionId = requiredRevisionId;
        }

        public int CallCount { get; private set; }

        public BatchUpdateDocumentRequest Build(PhysicalUpdatePlan plan)
        {
            CallCount++;
            return new BatchUpdateDocumentRequest(
                [new { deleteContentRange = new { range = new { startIndex = 10, endIndex = 11 } } }],
                new BatchUpdateWriteControl(requiredRevisionId));
        }
    }

    private sealed class RecordingGateway : IGoogleDocsGateway
    {
        public Exception? Exception { get; init; }

        public int CallCount { get; private set; }

        public List<string> DocumentIds { get; } = [];

        public List<BatchUpdateDocumentRequest> Requests { get; } = [];

        public Task<BatchUpdateDocumentResponse> BatchUpdateDocumentAsync(
            string documentId,
            BatchUpdateDocumentRequest request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            DocumentIds.Add(documentId);
            Requests.Add(request);
            if (Exception is not null)
            {
                throw Exception;
            }

            return Task.FromResult(new BatchUpdateDocumentResponse("applied-revision"));
        }
    }
}
