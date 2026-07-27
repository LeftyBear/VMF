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

    [Fact]
    public void Map_InlineStyleUpdate_CanApplyAndClearRichStyles()
    {
        var plan = Plan(
            InlineStyle(
                0,
                10,
                14,
                InlineTextStyle.Link,
                enabled: true,
                new Uri("https://example.com/new")),
            InlineStyle(
                1,
                15,
                18,
                InlineTextStyle.Code,
                enabled: false,
                null));

        var batch = new GoogleDocsPhysicalUpdateRequestMapper().Map(plan);

        Assert.Equal(["updateTextStyle", "updateTextStyle"], batch.Traces.Select(item => item.RequestKind));
        using var link = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[0]));
        using var code = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[1]));
        Assert.Equal(
            "https://example.com/new",
            link.RootElement.GetProperty("updateTextStyle")
                .GetProperty("textStyle").GetProperty("link").GetProperty("url").GetString());
        Assert.True(
            code.RootElement.GetProperty("updateTextStyle")
                .GetProperty("textStyle").GetProperty("weightedFontFamily").ValueKind == JsonValueKind.Null);
    }

    [Fact]
    public void Map_ReplaceInlineContent_DeletesInsertsAndAppliesCandidateStyles()
    {
        var plan = Plan(ReplaceInline(
            0,
            10,
            13,
            "new",
            [new InlineStyleRange(0, 3, InlineTextStyle.Bold)]));

        var batch = new GoogleDocsPhysicalUpdateRequestMapper().Map(plan);

        Assert.Equal(["deleteContentRange", "insertText", "updateTextStyle"], batch.Traces.Select(x => x.RequestKind));
        using var delete = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[0]));
        using var insert = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[1]));
        using var style = JsonDocument.Parse(JsonSerializer.Serialize(batch.Requests[2]));
        Assert.Equal(
            13,
            delete.RootElement.GetProperty("deleteContentRange")
                .GetProperty("range").GetProperty("endIndex").GetInt32());
        Assert.Equal(
            "new",
            insert.RootElement.GetProperty("insertText").GetProperty("text").GetString());
        Assert.True(
            style.RootElement.GetProperty("updateTextStyle")
                .GetProperty("textStyle").GetProperty("bold").GetBoolean());
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

    private static PhysicalUpdateOperation InlineStyle(
        int sequence,
        int start,
        int end,
        InlineTextStyle style,
        bool enabled,
        Uri? url) => new(
        sequence,
        PhysicalOperationKind.UpdateInlineStyle,
        PhysicalOperationReason.Update,
        0,
        0,
        new DocumentTextRange(start, end),
        Block("a"),
        null,
        new InlinePhysicalUpdate(null, null, style, enabled, url));

    private static PhysicalUpdateOperation ReplaceInline(
        int sequence,
        int start,
        int end,
        string text,
        IEnumerable<InlineStyleRange> ranges) => new(
        sequence,
        PhysicalOperationKind.ReplaceInlineContent,
        PhysicalOperationReason.Update,
        0,
        0,
        new DocumentTextRange(start, end),
        Block("a"),
        new DocumentBlock(ParagraphBlock.FromText(text), "a"),
        new InlinePhysicalUpdate(text, ranges, null, null, null));

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static BlockIdentity Block(string id) =>
        new(id, null, "ch-v1:sha256:" + Hash(id));

    private static PublishFingerprint Fingerprint(string value) =>
        new("v1:sha256:" + Hash(value));

    private static string Hash(string value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
