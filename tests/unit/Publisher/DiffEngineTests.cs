using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class DiffEngineTests
{
    private readonly DiffEngine engine = new();

    [Fact]
    public void CreatePlan_WithoutBaseline_InsertsEveryBlock()
    {
        var current = State("current", Block("one", "generated-one", "hash-one"),
            Block("two", "generated-two", "hash-two"));

        var plan = engine.CreatePlan(null, current);

        Assert.True(plan.IsPublishRequired);
        Assert.False(plan.IsFingerprintMatch);
        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(operation, DiffOperationKind.Insert, null, 0, null),
            operation => AssertOperation(operation, DiffOperationKind.Insert, null, 1, null));
    }

    [Fact]
    public void CreatePlan_WithSameFingerprint_SkipsBlockComparison()
    {
        var previous = State("same", Block("one", null, "old-hash"));
        var current = State("same", Block("one", null, "new-hash"));

        var plan = engine.CreatePlan(previous, current);

        Assert.True(plan.IsFingerprintMatch);
        Assert.False(plan.IsPublishRequired);
        Assert.Empty(plan.Operations);
    }

    [Fact]
    public void CreatePlan_MatchesByExplicitIdBeforeOtherIdentities()
    {
        var previous = State("previous",
            Block("stable", "generated-old", "old-hash"),
            Block(null, "generated-shared", "shared-hash"));
        var current = State("current",
            Block("stable", "generated-shared", "new-hash"),
            Block(null, "generated-new", "shared-hash"));

        var plan = engine.CreatePlan(previous, current);

        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(
                operation,
                DiffOperationKind.Update,
                0,
                0,
                BlockMatchKind.ExplicitId),
            operation => AssertOperation(
                operation,
                DiffOperationKind.NoChange,
                1,
                1,
                BlockMatchKind.ContentHash));
    }

    [Fact]
    public void CreatePlan_MatchesByGeneratedIdThenContentHash()
    {
        var previous = State("previous",
            Block(null, "generated", "old-hash"),
            Block(null, null, "content-hash"));
        var current = State("current",
            Block(null, "generated", "new-hash"),
            Block(null, null, "content-hash"));

        var plan = engine.CreatePlan(previous, current);

        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(
                operation,
                DiffOperationKind.Update,
                0,
                0,
                BlockMatchKind.GeneratedId),
            operation => AssertOperation(
                operation,
                DiffOperationKind.NoChange,
                1,
                1,
                BlockMatchKind.ContentHash));
    }

    [Fact]
    public void CreatePlan_ReportsMoveAndUpdateForRelocatedChangedBlock()
    {
        var previous = State("previous",
            Block("one", null, "hash-one"),
            Block("two", null, "old-hash-two"));
        var current = State("current",
            Block("two", null, "new-hash-two"),
            Block("one", null, "hash-one"));

        var plan = engine.CreatePlan(previous, current);

        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(
                operation,
                DiffOperationKind.Move,
                1,
                0,
                BlockMatchKind.ExplicitId),
            operation => AssertOperation(
                operation,
                DiffOperationKind.Update,
                1,
                0,
                BlockMatchKind.ExplicitId),
            operation => AssertOperation(
                operation,
                DiffOperationKind.NoChange,
                0,
                1,
                BlockMatchKind.ExplicitId));
    }

    [Fact]
    public void CreatePlan_DoesNotReportMovesForIndexShiftCausedByInsertion()
    {
        var previous = State("previous",
            Block("one", null, "hash-one"),
            Block("two", null, "hash-two"));
        var current = State("current",
            Block("inserted", null, "hash-inserted"),
            Block("one", null, "hash-one"),
            Block("two", null, "hash-two"));

        var plan = engine.CreatePlan(previous, current);

        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(operation, DiffOperationKind.Insert, null, 0, null),
            operation => AssertOperation(
                operation,
                DiffOperationKind.NoChange,
                0,
                1,
                BlockMatchKind.ExplicitId),
            operation => AssertOperation(
                operation,
                DiffOperationKind.NoChange,
                1,
                2,
                BlockMatchKind.ExplicitId));
    }

    [Fact]
    public void CreatePlan_ReportsInsertAndDeleteForUnmatchedBlocks()
    {
        var previous = State("previous", Block("removed", null, "old-hash"));
        var current = State("current", Block("inserted", null, "new-hash"));

        var plan = engine.CreatePlan(previous, current);

        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(operation, DiffOperationKind.Insert, null, 0, null),
            operation => AssertOperation(operation, DiffOperationKind.Delete, 0, null, null));
    }

    [Fact]
    public void CreatePlan_RejectsDifferentDocumentIdentity()
    {
        var previous = State("previous", Block("one", null, "hash"));
        var current = new PublishState(
            new DocumentIdentity("other-publication", "document", "google-document", DocumentState.Existing),
            new PublishFingerprint("current"),
            [Block("one", null, "hash")]);

        var exception = Assert.Throws<DiffConflictException>(() =>
            engine.CreatePlan(previous, current));

        Assert.Equal("DIFF_DOCUMENT_IDENTITY_MISMATCH", exception.Code);
    }

    private static PublishState State(string fingerprint, params BlockIdentity[] blocks) =>
        new(
            new DocumentIdentity(
                "publication",
                "document",
                "google-document",
                DocumentState.Existing),
            new PublishFingerprint(fingerprint),
            blocks);

    private static BlockIdentity Block(string? explicitId, string? generatedId, string hash) =>
        new(explicitId, generatedId, hash);

    private static void AssertOperation(
        DiffOperation operation,
        DiffOperationKind kind,
        int? previousIndex,
        int? currentIndex,
        BlockMatchKind? matchKind)
    {
        Assert.Equal(kind, operation.Kind);
        Assert.Equal(previousIndex, operation.PreviousIndex);
        Assert.Equal(currentIndex, operation.CurrentIndex);
        Assert.Equal(matchKind, operation.MatchKind);
    }
}
