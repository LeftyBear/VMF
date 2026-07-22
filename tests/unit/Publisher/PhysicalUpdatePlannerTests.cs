using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class PhysicalUpdatePlannerTests
{
    private readonly PhysicalUpdatePlanner planner = new();
    private readonly DiffEngine diffEngine = new();

    [Fact]
    public void CreatePlan_AllInsert_UsesReverseCandidateOrderAtRegionStart()
    {
        var candidate = Candidate(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var logical = diffEngine.CreatePlan(null, candidate);

        var plan = planner.CreatePlan(null, candidate, logical, EmptySnapshot());

        Assert.Equal(3, plan.Operations.Count);
        Assert.All(plan.Operations, operation => Assert.Equal(PhysicalOperationKind.InsertBlock, operation.Kind));
        Assert.Equal([2, 1, 0], plan.Operations.Select(operation => operation.CurrentIndex));
        Assert.All(plan.Operations, operation => Assert.Equal(10, operation.AffectedRange.StartIndex));
    }

    [Fact]
    public void CreatePlan_AllDelete_UsesDescendingSourceIndexes()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var candidate = Candidate();
        var logical = diffEngine.CreatePlan(baseline, candidate);

        var plan = planner.CreatePlan(baseline, candidate, logical, Snapshot(baseline));

        Assert.Equal([30, 20, 10], plan.Operations.Select(item => item.AffectedRange.StartIndex));
        Assert.All(plan.Operations, item => Assert.Equal(PhysicalOperationReason.Delete, item.Reason));
    }

    [Fact]
    public void CreatePlan_UpdateOnly_DeletesThenInsertsAtReducedRange()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));

        var plan = Plan(baseline, candidate);

        Assert.Collection(
            plan.Operations,
            item => AssertOperation(item, PhysicalOperationKind.DeleteRange, PhysicalOperationReason.Update, 10),
            item => AssertOperation(item, PhysicalOperationKind.InsertBlock, PhysicalOperationReason.Update, 10));
    }

    [Fact]
    public void CreatePlan_MoveOnly_ExpandsToDeleteAndInsert()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var candidate = Candidate(("b", "hb"), ("a", "ha"), ("c", "hc"));

        var plan = Plan(baseline, candidate);
        var moved = plan.Operations.Where(item => item.Reason == PhysicalOperationReason.Move).ToArray();

        Assert.Equal(2, moved.Length);
        Assert.Equal([PhysicalOperationKind.DeleteRange, PhysicalOperationKind.InsertBlock], moved.Select(x => x.Kind));
    }

    [Fact]
    public void CreatePlan_MoveAndUpdate_UsesOneDeleteAndCandidateInsert()
    {
        var baseline = Baseline(("a", "ha"), ("b", "old"), ("c", "hc"));
        var candidate = Candidate(("b", "new"), ("a", "ha"), ("c", "hc"));

        var plan = Plan(baseline, candidate);
        var combined = plan.Operations.Where(item => item.Reason == PhysicalOperationReason.MoveAndUpdate).ToArray();

        Assert.Equal(2, combined.Length);
        Assert.Equal(PhysicalOperationKind.DeleteRange, combined[0].Kind);
        Assert.Equal(PhysicalOperationKind.InsertBlock, combined[1].Kind);
        Assert.Equal("b", combined[1].TraceIdentity.ExplicitId);
        Assert.NotNull(combined[1].CandidateBlock);
    }

    [Fact]
    public void CreatePlan_InsertAndDelete_PreservesSurvivorAnchor()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var candidate = Candidate(("a", "ha"), ("x", "hx"), ("c", "hc"));

        var plan = Plan(baseline, candidate);

        Assert.Equal(PhysicalOperationKind.DeleteRange, plan.Operations[0].Kind);
        Assert.Equal(20, plan.Operations[0].AffectedRange.StartIndex);
        var insert = Assert.Single(plan.Operations.Where(item => item.Kind == PhysicalOperationKind.InsertBlock));
        Assert.Equal(20, insert.AffectedRange.StartIndex);
    }

    [Fact]
    public void CreatePlan_MultipleMove_ProducesOneDeleteAndInsertPerMovedBlock()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"), ("d", "hd"));
        var candidate = Candidate(("c", "hc"), ("d", "hd"), ("a", "ha"), ("b", "hb"));

        var plan = Plan(baseline, candidate);
        var moves = plan.Operations.Where(item => item.Reason == PhysicalOperationReason.Move).ToArray();

        Assert.True(moves.Length >= 4);
        Assert.Equal(
            moves.Count(item => item.Kind == PhysicalOperationKind.DeleteRange),
            moves.Count(item => item.Kind == PhysicalOperationKind.InsertBlock));
    }

    [Fact]
    public void CreatePlan_CompleteReverse_IsIndexSafe()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"), ("d", "hd"));
        var candidate = Candidate(("d", "hd"), ("c", "hc"), ("b", "hb"), ("a", "ha"));

        var plan = Plan(baseline, candidate);
        var deletes = plan.Operations.Where(x => x.Kind == PhysicalOperationKind.DeleteRange).ToArray();
        var inserts = plan.Operations.Where(x => x.Kind == PhysicalOperationKind.InsertBlock).ToArray();

        Assert.Equal(deletes.OrderByDescending(x => x.AffectedRange.StartIndex), deletes);
        Assert.Equal(inserts.OrderByDescending(x => x.CurrentIndex), inserts);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 20)]
    [InlineData(3, 40)]
    public void CreatePlan_InsertAtHeadMiddleOrTail_UsesStableAnchor(int insertionIndex, int expectedIndex)
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var values = new List<(string, string)> { ("a", "ha"), ("b", "hb"), ("c", "hc") };
        values.Insert(insertionIndex, ("x", "hx"));

        var insert = Assert.Single(Plan(baseline, Candidate(values.ToArray())).Operations);

        Assert.Equal(PhysicalOperationKind.InsertBlock, insert.Kind);
        Assert.Equal(expectedIndex, insert.AffectedRange.StartIndex);
    }

    [Fact]
    public void CreatePlan_NoChange_ProducesNoPhysicalOperations()
    {
        var baseline = BaselineWithFingerprint("same", ("a", "ha"));
        var candidate = CandidateWithFingerprint("same", ("a", "ha"));

        var plan = Plan(baseline, candidate);

        Assert.Empty(plan.Operations);
        Assert.False(plan.IsPublishRequired);
    }

    [Fact]
    public void CreatePlan_SameInputProducesSamePhysicalSequence()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var candidate = Candidate(("c", "new"), ("a", "ha"), ("x", "hx"));

        var first = Plan(baseline, candidate);
        var second = Plan(baseline, candidate);

        Assert.Equal(first.Operations.Select(Signature), second.Operations.Select(Signature));
    }

    [Fact]
    public void CreatePlan_DeletesRemainDescendingBeforeAnyInsert()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var candidate = Candidate(("x", "hx"));

        var plan = Plan(baseline, candidate);
        var firstInsert = plan.Operations.ToList().FindIndex(item => item.Kind == PhysicalOperationKind.InsertBlock);

        Assert.True(firstInsert >= 0);
        Assert.All(plan.Operations.Take(firstInsert), item => Assert.Equal(PhysicalOperationKind.DeleteRange, item.Kind));
        Assert.Equal(
            plan.Operations.Take(firstInsert).OrderByDescending(item => item.AffectedRange.StartIndex),
            plan.Operations.Take(firstInsert));
    }

    [Fact]
    public void CreatePlan_MultipleDeleteThenInsert_UsesReducedRegionEnd()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"), ("c", "hc"));
        var candidate = Candidate(("a", "ha"), ("x", "hx"));

        var insert = Plan(baseline, candidate).Operations.Last();

        Assert.Equal(20, insert.AffectedRange.StartIndex);
    }

    [Fact]
    public void CreatePlan_UpdateAndInsert_DeletesUpdateBeforeBothInsertions()
    {
        var baseline = Baseline(("a", "old"), ("b", "hb"));
        var candidate = Candidate(("a", "new"), ("x", "hx"), ("b", "hb"));

        var plan = Plan(baseline, candidate);

        Assert.Equal(PhysicalOperationKind.DeleteRange, plan.Operations[0].Kind);
        Assert.All(plan.Operations.Skip(1), item => Assert.Equal(PhysicalOperationKind.InsertBlock, item.Kind));
    }

    [Fact]
    public void CreatePlan_RejectsOverlappingBlockRanges()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"));
        var snapshot = Snapshot(
            baseline,
            new DocumentTextRange(10, 25),
            [
                new ManagedBlockSnapshot(baseline.Blocks[0], new DocumentTextRange(10, 20)),
                new ManagedBlockSnapshot(baseline.Blocks[1], new DocumentTextRange(19, 25)),
            ]);

        AssertCode(
            UpdateErrorCodes.ManagedRegionMismatch,
            () => planner.CreatePlan(baseline, Candidate(("a", "ha"), ("b", "hb")),
                diffEngine.CreatePlan(baseline, Candidate(("a", "ha"), ("b", "hb"))), snapshot));
    }

    [Fact]
    public void CreatePlan_RejectsUntrackedGapInsideManagedRegion()
    {
        var baseline = Baseline(("a", "ha"), ("b", "hb"));
        var snapshot = Snapshot(
            baseline,
            new DocumentTextRange(10, 31),
            [
                new ManagedBlockSnapshot(baseline.Blocks[0], new DocumentTextRange(10, 20)),
                new ManagedBlockSnapshot(baseline.Blocks[1], new DocumentTextRange(21, 31)),
            ]);

        AssertCode(
            UpdateErrorCodes.ManagedRegionMismatch,
            () => planner.ValidateSnapshot(baseline, snapshot));
    }

    [Fact]
    public void CreatePlan_RejectsBlockOutsideManagedRegion()
    {
        var baseline = Baseline(("a", "ha"));
        var candidate = Candidate(("a", "new"));
        var snapshot = Snapshot(
            baseline,
            new DocumentTextRange(11, 20),
            [new ManagedBlockSnapshot(baseline.Blocks[0], new DocumentTextRange(10, 20))]);

        AssertCode(
            UpdateErrorCodes.ManagedRegionMismatch,
            () => planner.CreatePlan(baseline, candidate, diffEngine.CreatePlan(baseline, candidate), snapshot));
    }

    private PhysicalUpdatePlan Plan(VerifiedPublishState baseline, PublishCandidate candidate)
    {
        var logical = diffEngine.CreatePlan(baseline, candidate);
        return planner.CreatePlan(baseline, candidate, logical, Snapshot(baseline));
    }

    private static VerifiedPublishState Baseline(params (string Id, string Hash)[] blocks) =>
        BaselineWithFingerprint("baseline", blocks);

    private static VerifiedPublishState BaselineWithFingerprint(
        string fingerprint,
        params (string Id, string Hash)[] blocks) => new(
            Identity(),
            Versions(),
            Revision(),
            Fingerprint(fingerprint),
            blocks.Select(Block));

    private static PublishCandidate Candidate(params (string Id, string Hash)[] blocks) =>
        CandidateWithFingerprint("candidate", blocks);

    private static PublishCandidate CandidateWithFingerprint(
        string fingerprint,
        params (string Id, string Hash)[] blocks) => new(
            Identity(),
            Versions(),
            Fingerprint(fingerprint),
            blocks.Select(Block),
            new DocumentModel(blocks.Select(item =>
                new DocumentBlock(ParagraphBlock.FromText(item.Id), item.Id))));

    private static ManagedDocumentSnapshot EmptySnapshot() => new(
        Identity(),
        new DocumentRevision("revision-0", 0),
        new DocumentTextRange(10, 10),
        Fingerprint("empty").Value,
        Array.Empty<ManagedBlockSnapshot>());

    private static ManagedDocumentSnapshot Snapshot(VerifiedPublishState baseline)
    {
        var blocks = baseline.Blocks.Select((block, index) => new ManagedBlockSnapshot(
            block,
            new DocumentTextRange(10 + (index * 10), 20 + (index * 10)))).ToArray();
        return Snapshot(
            baseline,
            new DocumentTextRange(10, 10 + (blocks.Length * 10)),
            blocks);
    }

    private static ManagedDocumentSnapshot Snapshot(
        VerifiedPublishState baseline,
        DocumentTextRange region,
        IEnumerable<ManagedBlockSnapshot> blocks) => new(
            baseline.Identity,
            baseline.Revision,
            region,
            baseline.Fingerprint.Value,
            blocks);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static PublishStateVersions Versions() => new("2", "1", "1", "1", "1.0", "test");

    private static DocumentRevision Revision() => new("revision-1", 1);

    private static BlockIdentity Block((string Id, string Hash) item) =>
        new(item.Id, null, "ch-v1:sha256:" + Hash(item.Hash));

    private static PublishFingerprint Fingerprint(string seed) =>
        new("v1:sha256:" + Hash(seed));

    private static string Hash(string seed) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(seed))).ToLowerInvariant();

    private static void AssertOperation(
        PhysicalUpdateOperation operation,
        PhysicalOperationKind kind,
        PhysicalOperationReason reason,
        int index)
    {
        Assert.Equal(kind, operation.Kind);
        Assert.Equal(reason, operation.Reason);
        Assert.Equal(index, operation.AffectedRange.StartIndex);
    }

    private static string Signature(PhysicalUpdateOperation item) => string.Join(
        ":",
        item.Sequence,
        item.Kind,
        item.Reason,
        item.PreviousIndex?.ToString() ?? "-",
        item.CurrentIndex?.ToString() ?? "-",
        item.AffectedRange.StartIndex,
        item.AffectedRange.EndIndex,
        item.TraceIdentity.ExplicitId);

    private static void AssertCode(string code, Action action)
    {
        var exception = Assert.Throws<PhysicalUpdateException>(action);
        Assert.Equal(code, exception.Code);
    }
}
