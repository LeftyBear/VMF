using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class PhysicalUpdateApplicationVerifierTests
{
    [Fact]
    public async Task ApplyAndVerify_CompleteMatchReturnsRevisionBoundEvidence()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var verifier = Verifier(adapter);
        var prepared = await verifier.PrepareAsync(candidate, baseline, default);
        var logical = new DiffEngine().CreatePlan(baseline, candidate);

        var result = await verifier.ApplyAndVerifyAsync(candidate, baseline, logical, prepared, default);

        Assert.True(result.IsLogicalPlanApplied);
        Assert.True(result.IsReadbackVerified);
        Assert.Equal(2, result.AppliedRevision.Sequence);
        Assert.Equal(candidate.Blocks, result.AppliedBlocks);
        Assert.Equal(1, adapter.ApplyCount);
    }

    [Fact]
    public async Task Prepare_RejectsBaselineRevisionConflictBeforeLogicalPlanning()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var current = Snapshot(baseline, revision: Revision(2));
        var adapter = new InMemoryManagedDocumentAdapter(current);
        var verifier = Verifier(adapter);
        await AssertCodeAsync(
            UpdateErrorCodes.RevisionConflict,
            () => verifier.PrepareAsync(candidate, baseline, default));
        Assert.Equal(0, adapter.ApplyCount);
    }

    [Fact]
    public async Task ApplyAndVerify_RejectsEditImmediatelyBeforeApply()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline))
        {
            AdvanceRevisionBeforeApply = true,
        };

        await AssertApplyCodeAsync(UpdateErrorCodes.RevisionConflict, baseline, candidate, adapter);
        Assert.Equal(0, adapter.ApplyCount);
    }

    [Fact]
    public async Task ApplyAndVerify_RejectsEditBetweenApplyAndReadback()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline))
        {
            AdvanceRevisionAfterApply = true,
        };

        await AssertApplyCodeAsync(UpdateErrorCodes.RevisionConflict, baseline, candidate, adapter);
        Assert.Equal(1, adapter.ApplyCount);
    }

    [Fact]
    public async Task ApplyAndVerify_RejectsUnchangedApplyRevision()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline))
        {
            KeepRevisionAfterApply = true,
        };

        await AssertApplyCodeAsync(UpdateErrorCodes.RevisionConflict, baseline, candidate, adapter);
    }

    [Fact]
    public async Task ApplyAndVerify_RejectsRegressedApplyRevision()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline))
        {
            RegressRevisionAfterApply = true,
        };

        await AssertApplyCodeAsync(UpdateErrorCodes.RevisionConflict, baseline, candidate, adapter);
    }

    [Fact]
    public void DocumentRevision_RejectsMissingRevision()
    {
        Assert.Throws<ArgumentException>(() => new DocumentRevision(string.Empty, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DocumentRevision("revision", -1));
    }

    [Fact]
    public async Task ApplyAndVerify_WrapsPhysicalApplicationFailure()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline))
        {
            FailNextApply = true,
        };

        await AssertApplyCodeAsync(UpdateErrorCodes.ApplicationFailed, baseline, candidate, adapter);
    }

    [Fact]
    public async Task ApplyAndVerify_AcceptsNoChangeWithoutRevisionAdvanceOrApply()
    {
        var baseline = BaselineWithFingerprint("same", ("a", "hash"));
        var candidate = CandidateWithFingerprint("same", ("a", "hash"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var verifier = Verifier(adapter);
        var prepared = await verifier.PrepareAsync(candidate, baseline, default);
        var logical = new DiffEngine().CreatePlan(baseline, candidate);

        var result = await verifier.ApplyAndVerifyAsync(candidate, baseline, logical, prepared, default);

        Assert.Equal(baseline.Revision, result.AppliedRevision);
        Assert.Equal(0, adapter.ApplyCount);
    }

    [Theory]
    [InlineData(ReadbackMutation.BlockCount, UpdateErrorCodes.ReadbackMismatch)]
    [InlineData(ReadbackMutation.BlockOrder, UpdateErrorCodes.ReadbackMismatch)]
    [InlineData(ReadbackMutation.Identity, UpdateErrorCodes.ReadbackMismatch)]
    [InlineData(ReadbackMutation.ContentHash, UpdateErrorCodes.ReadbackMismatch)]
    [InlineData(ReadbackMutation.ManagedRegion, UpdateErrorCodes.ManagedRegionMismatch)]
    [InlineData(ReadbackMutation.ManagedRegionEnd, UpdateErrorCodes.ManagedRegionMismatch)]
    [InlineData(ReadbackMutation.DocumentIdentity, UpdateErrorCodes.ReadbackMismatch)]
    [InlineData(ReadbackMutation.Revision, UpdateErrorCodes.RevisionConflict)]
    public async Task ApplyAndVerify_RejectsReadbackMismatch(
        ReadbackMutation mutation,
        string expectedCode)
    {
        var baseline = Baseline(("a", "old-a"), ("b", "old-b"));
        var candidate = Candidate(("a", "new-a"), ("b", "new-b"));
        var prepared = Snapshot(baseline);
        var correct = CandidateSnapshot(candidate, Revision(2), 10);
        var adapter = new ScriptedAdapter(
            [prepared, Mutate(correct, mutation)],
            new PhysicalApplyReceipt(Revision(2)));
        var verifier = Verifier(adapter);

        await AssertCodeAsync(
            expectedCode,
            () => verifier.ApplyAndVerifyAsync(
                candidate,
                baseline,
                new DiffEngine().CreatePlan(baseline, candidate),
                prepared,
                default));
    }

    [Fact]
    public async Task ApplyAndVerify_ReportsReadbackFailure()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var prepared = Snapshot(baseline);
        var adapter = new ScriptedAdapter(
            [prepared],
            new PhysicalApplyReceipt(Revision(2)),
            failReadNumber: 2);
        var verifier = Verifier(adapter);

        await AssertCodeAsync(
            UpdateErrorCodes.ReadbackFailed,
            () => verifier.ApplyAndVerifyAsync(
                candidate,
                baseline,
                new DiffEngine().CreatePlan(baseline, candidate),
                prepared,
                default));
    }

    [Fact]
    public async Task DryRun_DoesNotApplyAndReportsCountsRangesAndRevision()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"), ("b", "hb"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var verifier = Verifier(adapter);
        var prepared = await verifier.PrepareAsync(candidate, baseline, default);
        var logical = new DiffEngine().CreatePlan(baseline, candidate);

        var dryRun = verifier.CreateDryRun(candidate, baseline, logical, prepared);

        Assert.Empty(dryRun.Conflicts);
        Assert.True(dryRun.IsPublishRequired);
        Assert.Equal(logical.Operations.Count, dryRun.LogicalOperationCount);
        Assert.Equal(3, dryRun.PhysicalOperationCount);
        Assert.Equal(1, dryRun.Count(DiffOperationKind.Insert));
        Assert.Equal(1, dryRun.Count(DiffOperationKind.Update));
        Assert.Equal(baseline.Revision, dryRun.PhysicalPlan?.RequiredRevision);
        Assert.All(dryRun.PhysicalPlan!.Operations, operation => Assert.NotNull(operation.TraceIdentity));
        Assert.All(dryRun.PhysicalPlan.Operations, operation =>
        {
            Assert.InRange(
                operation.AffectedRange.StartIndex,
                prepared.ManagedRegion.StartIndex,
                prepared.ManagedRegion.EndIndex);
            Assert.True(operation.AffectedRange.EndIndex <= prepared.ManagedRegion.EndIndex);
        });
        Assert.Empty(dryRun.Warnings);
        Assert.Equal(0, adapter.ApplyCount);
    }

    [Fact]
    public void DryRun_RevisionConflictIsReportedWithoutApplication()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline, revision: Revision(2)));
        var verifier = Verifier(adapter);
        var prepared = Snapshot(baseline, revision: Revision(2));
        var logical = new DiffEngine().CreatePlan(baseline, candidate);

        var dryRun = verifier.CreateDryRun(candidate, baseline, logical, prepared);

        Assert.Equal([UpdateErrorCodes.RevisionConflict], dryRun.Conflicts);
        Assert.Null(dryRun.PhysicalPlan);
        Assert.True(dryRun.IsPublishRequired);
        Assert.Equal(0, adapter.ApplyCount);
    }

    [Fact]
    public async Task DryRun_NoChangeProducesEmptyPhysicalPlan()
    {
        var baseline = BaselineWithFingerprint("same", ("a", "hash"));
        var candidate = CandidateWithFingerprint("same", ("a", "hash"));
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var verifier = Verifier(adapter);
        var prepared = await verifier.PrepareAsync(candidate, baseline, default);

        var result = verifier.CreateDryRun(
            candidate,
            baseline,
            new DiffEngine().CreatePlan(baseline, candidate),
            prepared);

        Assert.False(result.IsPublishRequired);
        Assert.Equal(0, result.PhysicalOperationCount);
        Assert.Empty(result.Conflicts);
    }

    private static PhysicalUpdateApplicationVerifier Verifier(IManagedDocumentAdapter adapter) =>
        new(adapter, new PhysicalUpdatePlanner());

    private static async Task AssertApplyCodeAsync(
        string code,
        VerifiedPublishState baseline,
        PublishCandidate candidate,
        IManagedDocumentAdapter adapter)
    {
        var verifier = Verifier(adapter);
        var prepared = await verifier.PrepareAsync(candidate, baseline, default);
        await AssertCodeAsync(
            code,
            () => verifier.ApplyAndVerifyAsync(
                candidate,
                baseline,
                new DiffEngine().CreatePlan(baseline, candidate),
                prepared,
                default));
    }

    private static async Task AssertCodeAsync(string code, Func<Task> action)
    {
        var exception = await Assert.ThrowsAsync<PhysicalUpdateException>(action);
        Assert.Equal(code, exception.Code);
    }

    private static ManagedDocumentSnapshot Mutate(
        ManagedDocumentSnapshot source,
        ReadbackMutation mutation)
    {
        var blocks = source.Blocks.ToArray();
        return mutation switch
        {
            ReadbackMutation.BlockCount => SnapshotFrom(source, blocks.Take(1)),
            ReadbackMutation.BlockOrder => SnapshotFrom(source, blocks.Reverse().Select((block, index) =>
                new ManagedBlockSnapshot(block.Identity, new DocumentTextRange(10 + index, 11 + index)))),
            ReadbackMutation.Identity => SnapshotFrom(source,
                ReplaceFirst(blocks, new BlockIdentity("other", null, blocks[0].Identity.ContentHash))),
            ReadbackMutation.ContentHash => SnapshotFrom(source,
                ReplaceFirst(blocks, new BlockIdentity("a", null, ContentHash("other")))),
            ReadbackMutation.ManagedRegion => new ManagedDocumentSnapshot(
                source.Identity,
                source.Revision,
                new DocumentTextRange(11, source.ManagedRegion.EndIndex + 1),
                source.PublishFingerprint,
                blocks.Select((block, index) => new ManagedBlockSnapshot(
                    block.Identity,
                    new DocumentTextRange(11 + index, 12 + index)))),
            ReadbackMutation.ManagedRegionEnd => new ManagedDocumentSnapshot(
                source.Identity,
                source.Revision,
                new DocumentTextRange(
                    source.ManagedRegion.StartIndex,
                    source.ManagedRegion.EndIndex + 1),
                source.PublishFingerprint,
                blocks),
            ReadbackMutation.DocumentIdentity => new ManagedDocumentSnapshot(
                new DocumentIdentity("other", "document", "google-document", DocumentState.Active),
                source.Revision,
                source.ManagedRegion,
                source.PublishFingerprint,
                blocks),
            ReadbackMutation.Revision => new ManagedDocumentSnapshot(
                source.Identity,
                Revision(3),
                source.ManagedRegion,
                source.PublishFingerprint,
                blocks),
            _ => throw new ArgumentOutOfRangeException(nameof(mutation)),
        };
    }

    private static IEnumerable<ManagedBlockSnapshot> ReplaceFirst(
        IReadOnlyList<ManagedBlockSnapshot> blocks,
        BlockIdentity replacement) => blocks.Select((block, index) =>
            index == 0 ? new ManagedBlockSnapshot(replacement, block.Range) : block);

    private static ManagedDocumentSnapshot SnapshotFrom(
        ManagedDocumentSnapshot source,
        IEnumerable<ManagedBlockSnapshot> blocks) => new(
            source.Identity,
            source.Revision,
            source.ManagedRegion,
            source.PublishFingerprint,
            blocks);

    private static VerifiedPublishState Baseline(params (string Id, string Hash)[] blocks) =>
        BaselineWithFingerprint("baseline", blocks);

    private static VerifiedPublishState BaselineWithFingerprint(
        string fingerprint,
        params (string Id, string Hash)[] blocks) => new(
            Identity(),
            Versions(),
            Revision(1),
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

    private static ManagedDocumentSnapshot Snapshot(
        VerifiedPublishState baseline,
        DocumentRevision? revision = null)
    {
        var blocks = baseline.Blocks.Select((block, index) => new ManagedBlockSnapshot(
            block,
            new DocumentTextRange(10 + index, 11 + index))).ToArray();
        return new ManagedDocumentSnapshot(
            baseline.Identity,
            revision ?? baseline.Revision,
            new DocumentTextRange(10, 10 + blocks.Length),
            baseline.Fingerprint.Value,
            blocks);
    }

    private static ManagedDocumentSnapshot CandidateSnapshot(
        PublishCandidate candidate,
        DocumentRevision revision,
        int start)
    {
        var blocks = candidate.Blocks.Select((block, index) => new ManagedBlockSnapshot(
            block,
            new DocumentTextRange(start + index, start + index + 1))).ToArray();
        return new ManagedDocumentSnapshot(
            candidate.Identity,
            revision,
            new DocumentTextRange(start, start + blocks.Length),
            candidate.Fingerprint.Value,
            blocks);
    }

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static PublishStateVersions Versions() => new("2", "1", "1", "1", "1.0", "test");

    private static DocumentRevision Revision(long sequence) => new("revision-" + sequence, sequence);

    private static BlockIdentity Block((string Id, string Hash) item) =>
        new(item.Id, null, ContentHash(item.Hash));

    private static string ContentHash(string seed) => "ch-v1:sha256:" + Hash(seed);

    private static PublishFingerprint Fingerprint(string seed) => new("v1:sha256:" + Hash(seed));

    private static string Hash(string seed) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(seed))).ToLowerInvariant();

    public enum ReadbackMutation
    {
        BlockCount,
        BlockOrder,
        Identity,
        ContentHash,
        ManagedRegion,
        ManagedRegionEnd,
        DocumentIdentity,
        Revision,
    }

    private sealed class ScriptedAdapter : IManagedDocumentAdapter
    {
        private readonly Queue<ManagedDocumentSnapshot> snapshots;
        private readonly PhysicalApplyReceipt receipt;
        private readonly int? failReadNumber;
        private int readCount;

        internal ScriptedAdapter(
            IEnumerable<ManagedDocumentSnapshot> snapshots,
            PhysicalApplyReceipt receipt,
            int? failReadNumber = null)
        {
            this.snapshots = new Queue<ManagedDocumentSnapshot>(snapshots);
            this.receipt = receipt;
            this.failReadNumber = failReadNumber;
        }

        public Task<ManagedDocumentSnapshot> GetSnapshotAsync(
            DocumentIdentity identity,
            CancellationToken cancellationToken)
        {
            readCount++;
            if (readCount == failReadNumber)
            {
                throw new InvalidOperationException("Injected readback failure.");
            }

            return Task.FromResult(snapshots.Dequeue());
        }

        public Task<PhysicalApplyReceipt> ApplyAsync(
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken) => Task.FromResult(receipt);
    }
}
