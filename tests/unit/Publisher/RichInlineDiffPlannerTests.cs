using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class RichInlineDiffPlannerTests
{
    private readonly DiffEngine diffEngine = new();
    private readonly PhysicalUpdatePlanner planner = new();

    [Fact]
    public void CreatePlan_BoldOnlyUpdate_ProducesInlineStyleOperation()
    {
        var baselineBlock = Paragraph("a", [new TextInline("Hello")]);
        var candidateBlock = Paragraph("a", [new BoldInline([new TextInline("Hello")])]);

        var operation = Assert.Single(Plan(baselineBlock, candidateBlock).Operations);

        Assert.Equal(PhysicalOperationKind.UpdateInlineStyle, operation.Kind);
        Assert.Equal(PhysicalOperationReason.Update, operation.Reason);
        Assert.Equal(new DocumentTextRange(10, 15), operation.AffectedRange);
        Assert.Equal(InlineTextStyle.Bold, operation.InlineUpdate?.Style);
        Assert.True(operation.InlineUpdate?.StyleEnabled);
    }

    [Fact]
    public void CreatePlan_LinkChangeAndCodeRemoval_AreClassifiedDeterministically()
    {
        var firstUrl = new Uri("https://example.com/old");
        var secondUrl = new Uri("https://example.com/new");
        var baselineBlock = Paragraph(
            "a",
            [new LinkInline([new TextInline("Docs")], firstUrl), new TextInline(" "), new CodeInline("api")]);
        var candidateBlock = Paragraph(
            "a",
            [new LinkInline([new TextInline("Docs")], secondUrl), new TextInline(" api")]);

        var operations = Plan(baselineBlock, candidateBlock).Operations;

        Assert.Equal(
            [PhysicalOperationKind.UpdateInlineStyle, PhysicalOperationKind.UpdateInlineStyle],
            operations.Select(item => item.Kind));
        Assert.Equal([10, 15], operations.Select(item => item.AffectedRange.StartIndex));
        Assert.Equal(InlineTextStyle.Link, operations[0].InlineUpdate?.Style);
        Assert.True(operations[0].InlineUpdate?.StyleEnabled);
        Assert.Equal(secondUrl, operations[0].InlineUpdate?.Url);
        Assert.Equal(InlineTextStyle.Code, operations[1].InlineUpdate?.Style);
        Assert.False(operations[1].InlineUpdate?.StyleEnabled);
    }

    [Fact]
    public void CreatePlan_UnicodeStyleUpdate_UsesTextElementBoundary()
    {
        var text = "A 👩‍💻 e\u0301 Z";
        var emojiStart = text.IndexOf("👩", StringComparison.Ordinal);
        var emojiLength = "👩‍💻".Length;
        var baselineBlock = Paragraph("a", [new TextInline(text)]);
        var candidateBlock = Paragraph(
            "a",
            [
                new TextInline(text[..emojiStart]),
                new ItalicInline([new TextInline(text.Substring(emojiStart, emojiLength))]),
                new TextInline(text[(emojiStart + emojiLength)..]),
            ]);

        var operation = Assert.Single(Plan(baselineBlock, candidateBlock).Operations);

        Assert.Equal(PhysicalOperationKind.UpdateInlineStyle, operation.Kind);
        Assert.Equal(10 + emojiStart, operation.AffectedRange.StartIndex);
        Assert.Equal(10 + emojiStart + emojiLength, operation.AffectedRange.EndIndex);
    }

    [Fact]
    public void CreatePlan_TextChange_UsesReplaceInlineContentFallback()
    {
        var baselineBlock = Paragraph("a", [new TextInline("old")]);
        var candidateBlock = Paragraph("a", [new BoldInline([new TextInline("new")])]);

        var operation = Assert.Single(Plan(baselineBlock, candidateBlock).Operations);

        Assert.Equal(PhysicalOperationKind.ReplaceInlineContent, operation.Kind);
        Assert.Equal(new DocumentTextRange(10, 13), operation.AffectedRange);
        Assert.Equal("new", operation.InlineUpdate?.Text);
        var style = Assert.Single(operation.InlineUpdate!.StyleRanges);
        Assert.Equal(InlineTextStyle.Bold, style.Style);
        Assert.Equal(0, style.StartOffset);
        Assert.Equal(3, style.EndOffset);
    }

    [Fact]
    public void CreatePlan_MissingCanonicalReadback_FallsBackToBlockReplace()
    {
        var baselineBlock = Paragraph("a", [new TextInline("Hello")]);
        var candidateBlock = Paragraph("a", [new BoldInline([new TextInline("Hello")])]);
        var baseline = Baseline(baselineBlock, "old");
        var candidate = Candidate(candidateBlock, "new");
        var logical = diffEngine.CreatePlan(baseline, candidate);
        var snapshot = Snapshot(baseline, canonicalBlock: null);

        var plan = planner.CreatePlan(baseline, candidate, logical, snapshot);

        Assert.Equal(
            [PhysicalOperationKind.DeleteRange, PhysicalOperationKind.InsertBlock],
            plan.Operations.Select(item => item.Kind));
    }

    [Fact]
    public void CreatePlan_SameInputProducesSameRichInlineSequence()
    {
        var baselineBlock = Paragraph("a", [new TextInline("A"), new CodeInline("B")]);
        var candidateBlock = Paragraph("a", [new BoldInline([new TextInline("A")]), new TextInline("B")]);

        var first = Plan(baselineBlock, candidateBlock);
        var second = Plan(baselineBlock, candidateBlock);

        Assert.Equal(first.Operations.Select(Signature), second.Operations.Select(Signature));
    }

    private PhysicalUpdatePlan Plan(DocumentBlock baselineBlock, DocumentBlock candidateBlock)
    {
        var baseline = Baseline(baselineBlock, "old");
        var candidate = Candidate(candidateBlock, "new");
        return planner.CreatePlan(
            baseline,
            candidate,
            diffEngine.CreatePlan(baseline, candidate),
            Snapshot(baseline, baselineBlock));
    }

    private static DocumentBlock Paragraph(string id, IEnumerable<InlineContent> content) =>
        new(new ParagraphBlock(content), id);

    private static VerifiedPublishState Baseline(DocumentBlock block, string hashSeed) => new(
        Identity(),
        Versions(),
        Revision(1),
        Fingerprint("baseline"),
        [Block(block.ExplicitId!, hashSeed)]);

    private static PublishCandidate Candidate(DocumentBlock block, string hashSeed) => new(
        Identity(),
        Versions(),
        Fingerprint("candidate"),
        [Block(block.ExplicitId!, hashSeed)],
        new DocumentModel([block]));

    private static ManagedDocumentSnapshot Snapshot(
        VerifiedPublishState baseline,
        DocumentBlock? canonicalBlock) => new(
        Identity(),
        baseline.Revision,
        new DocumentTextRange(10, 30),
        baseline.Fingerprint.Value,
        [new ManagedBlockSnapshot(baseline.Blocks[0], new DocumentTextRange(10, 30), canonicalBlock)]);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static PublishStateVersions Versions() => new("2", "1", "1", "1", "1.0", "test");

    private static DocumentRevision Revision(long sequence) => new("revision-" + sequence, sequence);

    private static BlockIdentity Block(string id, string seed) => new(id, null, ContentHash(seed));

    private static string ContentHash(string seed) => "ch-v1:sha256:" + Hash(seed);

    private static PublishFingerprint Fingerprint(string seed) => new("v1:sha256:" + Hash(seed));

    private static string Hash(string seed) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(seed))).ToLowerInvariant();

    private static string Signature(PhysicalUpdateOperation operation) => string.Join(
        ":",
        operation.Sequence,
        operation.Kind,
        operation.AffectedRange.StartIndex,
        operation.AffectedRange.EndIndex,
        operation.InlineUpdate?.Style?.ToString() ?? "-",
        operation.InlineUpdate?.StyleEnabled?.ToString() ?? "-",
        operation.InlineUpdate?.Url?.AbsoluteUri ?? "-");
}
