using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsBatchRequestBuilderTests
{
    [Fact]
    public void Build_MapsPhysicalPlanToBatchUpdateDocumentRequest()
    {
        var request = new GoogleDocsBatchRequestBuilder().Build(Plan(
            Delete(0, 30, 40),
            Insert(1, 10)));

        Assert.Equal("required-revision", request.WriteControl.RequiredRevisionId);
        Assert.Equal(2, request.Requests.Count);
        using var first = JsonDocument.Parse(JsonSerializer.Serialize(request.Requests[0]));
        using var second = JsonDocument.Parse(JsonSerializer.Serialize(request.Requests[1]));
        Assert.True(first.RootElement.TryGetProperty("deleteContentRange", out _));
        Assert.Equal(
            10,
            second.RootElement.GetProperty("insertText")
                .GetProperty("location")
                .GetProperty("index")
                .GetInt32());
    }

    [Fact]
    public void Build_RequiresWriteControlRevision()
    {
        var mapper = new StubMapper(new PhysicalUpdateRequestBatch(
            "google-document",
            "required-revision",
            [new { }],
            1,
            [new PhysicalUpdateRequestTrace(0, 0, PhysicalOperationReason.Update, Block("a"), "insertText")]));

        var request = new GoogleDocsBatchRequestBuilder(mapper).Build(Plan(Delete(0, 10, 11)));

        Assert.Equal("required-revision", request.WriteControl.RequiredRevisionId);
    }

    private static PhysicalUpdatePlan Plan(params PhysicalUpdateOperation[] operations) => new(
        Identity(),
        new DocumentRevision("required-revision", 1),
        new DocumentTextRange(10, 50),
        new DiffPlan(
            Fingerprint("old"),
            Fingerprint("new"),
            isFingerprintMatch: false,
            operations.Select(operation => new DiffOperation(
                DiffOperationKind.Update,
                operation.PreviousIndex,
                operation.CurrentIndex,
                operation.TraceIdentity,
                operation.TraceIdentity,
                BlockMatchKind.ExplicitId))),
        operations);

    private static PhysicalUpdateOperation Delete(int sequence, int start, int end) => new(
        sequence,
        PhysicalOperationKind.DeleteRange,
        PhysicalOperationReason.Delete,
        sequence,
        null,
        new DocumentTextRange(start, end),
        Block("block-" + sequence),
        null);

    private static PhysicalUpdateOperation Insert(int sequence, int index) => new(
        sequence,
        PhysicalOperationKind.InsertBlock,
        PhysicalOperationReason.Insert,
        null,
        sequence,
        new DocumentTextRange(index, index),
        Block("block-" + sequence),
        new DocumentBlock(ParagraphBlock.FromText("Inserted"), "block-" + sequence));

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static BlockIdentity Block(string id) =>
        new(id, null, "ch-v1:sha256:" + Hash(id));

    private static PublishFingerprint Fingerprint(string value) =>
        new("v1:sha256:" + Hash(value));

    private static string Hash(string value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private sealed class StubMapper : IPhysicalUpdateRequestMapper
    {
        private readonly PhysicalUpdateRequestBatch batch;

        internal StubMapper(PhysicalUpdateRequestBatch batch)
        {
            this.batch = batch;
        }

        public PhysicalUpdateRequestBatch Map(PhysicalUpdatePlan plan) => batch;
    }
}
