using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsPhysicalUpdateRequestMapperTests
{
    [Fact]
    public void Map_SetsRequiredRevisionAndPreservesOperationOrder()
    {
        var plan = Plan(
            Delete(0, 30, 40, "c"),
            Delete(1, 10, 20, "a"),
            Insert(2, 25, "b", new DocumentBlock(ParagraphBlock.FromText("B"), "b")),
            Insert(3, 15, "a", new DocumentBlock(HeadingBlock.FromText(2, "A"), "a")));

        var batch = new GoogleDocsPhysicalUpdateRequestMapper().Map(plan);

        Assert.Equal("google-document", batch.DocumentId);
        Assert.Equal("required-revision", batch.RequiredRevisionId);
        Assert.Equal(4, batch.SourceOperationCount);
        Assert.Equal(batch.Requests.Count, batch.Traces.Count);
        Assert.Equal([0, 1, 2, 3, 3], batch.Traces.Select(item => item.SourceOperationIndex));
        Assert.Equal(
            ["deleteContentRange", "deleteContentRange", "insertText", "insertText", "updateParagraphStyle"],
            batch.Traces.Select(item => item.RequestKind));
        using var firstDelete = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[0]));
        using var secondDelete = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[1]));
        Assert.Equal(
            30,
            firstDelete.RootElement.GetProperty("deleteContentRange")
                .GetProperty("range").GetProperty("startIndex").GetInt32());
        Assert.Equal(
            10,
            secondDelete.RootElement.GetProperty("deleteContentRange")
                .GetProperty("range").GetProperty("startIndex").GetInt32());
    }

    [Fact]
    public void Map_ExpandsCanonicalPayloadAndKeepsOneOperationRequestsContiguous()
    {
        var plan = Plan(Insert(
            0,
            10,
            "a",
            new DocumentBlock(new ParagraphBlock([new TextInline("A "), new BoldInline([new TextInline("B")])]), "a")));

        var batch = new GoogleDocsPhysicalUpdateRequestMapper().Map(plan);

        Assert.Equal(["insertText", "updateTextStyle"], batch.Traces.Select(item => item.RequestKind));
        Assert.All(batch.Traces, trace => Assert.Equal(0, trace.SourceOperationIndex));
        using var insert = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[0]));
        using var style = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[1]));
        Assert.Equal(
            10,
            insert.RootElement.GetProperty("insertText").GetProperty("location").GetProperty("index").GetInt32());
        Assert.Equal(
            12,
            style.RootElement.GetProperty("updateTextStyle").GetProperty("range")
                .GetProperty("startIndex").GetInt32());
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

    private static PhysicalUpdateOperation Delete(int sequence, int start, int end, string id) => new(
        sequence,
        PhysicalOperationKind.DeleteRange,
        PhysicalOperationReason.Delete,
        sequence,
        null,
        new DocumentTextRange(start, end),
        Block(id),
        null);

    private static PhysicalUpdateOperation Insert(
        int sequence,
        int index,
        string id,
        DocumentBlock block) => new(
        sequence,
        PhysicalOperationKind.InsertBlock,
        PhysicalOperationReason.Insert,
        null,
        sequence,
        new DocumentTextRange(index, index),
        Block(id),
        block);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static BlockIdentity Block(string id) =>
        new(id, null, "ch-v1:sha256:" + Hash(id));

    private static PublishFingerprint Fingerprint(string value) =>
        new("v1:sha256:" + Hash(value));

    private static string Hash(string value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
