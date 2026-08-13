using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class PhysicalUpdateRecoveryTests
{
    [Fact]
    public void Reconcile_CandidateFullMatchIsApplied()
    {
        var candidate = Candidate(("a", "new"));
        var status = new PhysicalUpdateRecoveryReconciler().Reconcile(
            Baseline(("a", "old")),
            candidate,
            Snapshot(candidate.Identity, Revision(3), candidate.Fingerprint.Value, candidate.Blocks));

        Assert.Equal(RecoveryReconciliationStatus.Applied, status);
    }

    [Fact]
    public void Reconcile_BaseFullMatchIsNotApplied()
    {
        var baseline = Baseline(("a", "old"));
        var status = new PhysicalUpdateRecoveryReconciler().Reconcile(
            baseline,
            Candidate(("a", "new")),
            Snapshot(baseline.Identity, Revision(2), baseline.Fingerprint.Value, baseline.Blocks));

        Assert.Equal(RecoveryReconciliationStatus.NotApplied, status);
    }

    [Fact]
    public void Reconcile_PartialOrThirdPartyMatchIsDiverged()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var partial = Snapshot(candidate.Identity, Revision(4), candidate.Fingerprint.Value, [Block("a", "old")]);
        var thirdParty = Snapshot(candidate.Identity, Revision(4), Fingerprint("third").Value, [Block("x", "third")]);

        Assert.Equal(
            RecoveryReconciliationStatus.Diverged,
            new PhysicalUpdateRecoveryReconciler().Reconcile(baseline, candidate, partial));
        Assert.Equal(
            RecoveryReconciliationStatus.Diverged,
            new PhysicalUpdateRecoveryReconciler().Reconcile(baseline, candidate, thirdParty));
    }

    [Fact]
    public void Reconcile_DoesNotTreatRevisionOnlyMatchAsApplied()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var snapshot = Snapshot(candidate.Identity, baseline.Revision, baseline.Fingerprint.Value, baseline.Blocks);

        var status = new PhysicalUpdateRecoveryReconciler().Reconcile(baseline, candidate, snapshot);

        Assert.Equal(RecoveryReconciliationStatus.NotApplied, status);
    }

    [Fact]
    public async Task ApplicationService_SavesOnlyAfterReadbackVerification()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var store = new RecordingWriter();
        var service = new PhysicalUpdateApplicationService(
            new StubExecutor(new PhysicalUpdateExecutionResult(
                PhysicalUpdateExecutionStatus.Applied,
                "google-document",
                "required-revision",
                "applied-revision",
                1,
                1,
                1,
                "APPLIED",
                "applied")),
            new StubSnapshotReader(Snapshot(
                candidate.Identity,
                Revision(3),
                candidate.Fingerprint.Value,
                candidate.Blocks)),
            new PhysicalUpdateRecoveryReconciler(),
            new PhysicalUpdateApplicationSnapshotVerifier(),
            new PublishResultVerifier(),
            new VerifiedPublishStatePromoter(),
            store);

        var result = await service.ExecuteAsync(
            baseline,
            candidate,
            LogicalPlan(baseline, candidate),
            PhysicalPlan(baseline, candidate),
            CancellationToken.None);

        Assert.NotNull(result.SavedState);
        Assert.Equal("verified", result.ReadbackReport.Status);
        Assert.Equal("verified-state-save", result.ReadbackReport.Phase);
        Assert.True(result.ReadbackReport.ReadbackVerified);
        Assert.True(result.ReadbackReport.VerifiedStateSaved);
        Assert.False(result.ReadbackReport.PublicationAuthorized);
        Assert.False(result.ReadbackReport.ReleaseClearance);
        Assert.False(result.ReadbackReport.VendorClearance);
        Assert.Equal(1, store.SaveCount);
        Assert.Same(result.SavedState, store.Saved);
    }

    [Fact]
    public async Task ApplicationService_IndeterminateNotAppliedRequestsReplanWithoutSaving()
    {
        var baseline = Baseline(("a", "old"));
        var candidate = Candidate(("a", "new"));
        var store = new RecordingWriter();
        var service = new PhysicalUpdateApplicationService(
            new StubExecutor(new PhysicalUpdateExecutionResult(
                PhysicalUpdateExecutionStatus.IndeterminateFailure,
                "google-document",
                "required-revision",
                null,
                1,
                1,
                1,
                "INDETERMINATE_FAILURE",
                "unknown")),
            new StubSnapshotReader(Snapshot(
                baseline.Identity,
                Revision(2),
                baseline.Fingerprint.Value,
                baseline.Blocks)),
            new PhysicalUpdateRecoveryReconciler(),
            new PhysicalUpdateApplicationSnapshotVerifier(),
            new PublishResultVerifier(),
            new VerifiedPublishStatePromoter(),
            store);

        var result = await service.ExecuteAsync(
            baseline,
            candidate,
            LogicalPlan(baseline, candidate),
            PhysicalPlan(baseline, candidate),
            CancellationToken.None);

        Assert.Equal(RecoveryReconciliationStatus.NotApplied, result.RecoveryStatus);
        Assert.Equal("not-attempted", result.ReadbackReport.Status);
        Assert.Null(result.ReadbackReport.Phase);
        Assert.False(result.ReadbackReport.ReadbackVerified);
        Assert.False(result.ReadbackReport.VerifiedStateSaved);
        Assert.True(result.ReplanRequired);
        Assert.Equal(0, store.SaveCount);
    }

    [Theory]
    [InlineData(UpdateErrorCodes.ReadbackFailed, "failed", "post-apply-readback")]
    [InlineData(UpdateErrorCodes.ReadbackMismatch, "mismatch", "post-apply-readback")]
    [InlineData(UpdateErrorCodes.ManagedRegionMismatch, "mismatch", "post-apply-readback")]
    [InlineData(UpdateErrorCodes.RevisionConflict, "revision-conflict", "pre-apply-read")]
    public void ReadbackStatusReport_MapsStableCodesToClosedVocabulary(
        string code,
        string expectedStatus,
        string expectedPhase)
    {
        var report = ReadbackStatusReport.FromException(new PhysicalUpdateException(
            code,
            "sensitive https://private.example.test C:\\secret token Authorization: Bearer value"));

        Assert.Equal(expectedStatus, report.Status);
        Assert.Equal(expectedPhase, report.Phase);
        Assert.False(report.ReadbackVerified);
        Assert.False(report.VerifiedStateSaved);
        Assert.False(report.PublicationAuthorized);
        Assert.False(report.ReleaseClearance);
        Assert.False(report.VendorClearance);
    }

    private static VerifiedPublishState Baseline(params (string Id, string Hash)[] blocks) => new(
        Identity(),
        Versions(),
        Revision(1),
        Fingerprint("old"),
        blocks.Select(item => Block(item.Id, item.Hash)));

    private static PublishCandidate Candidate(params (string Id, string Hash)[] blocks) => new(
        Identity(),
        Versions(),
        Fingerprint("new"),
        blocks.Select(item => Block(item.Id, item.Hash)),
        new DocumentModel(blocks.Select(item => new DocumentBlock(ParagraphBlock.FromText(item.Id), item.Id))));

    private static DiffPlan LogicalPlan(VerifiedPublishState baseline, PublishCandidate candidate) => new(
        baseline.Fingerprint,
        candidate.Fingerprint,
        isFingerprintMatch: false,
        [new DiffOperation(
            DiffOperationKind.Update,
            0,
            0,
            baseline.Blocks[0],
            candidate.Blocks[0],
            BlockMatchKind.ExplicitId)]);

    private static PhysicalUpdatePlan PhysicalPlan(
        VerifiedPublishState baseline,
        PublishCandidate candidate) => new(
        candidate.Identity,
        baseline.Revision,
        new DocumentTextRange(10, 20),
        LogicalPlan(baseline, candidate),
        [new PhysicalUpdateOperation(
            0,
            PhysicalOperationKind.DeleteRange,
            PhysicalOperationReason.Update,
            0,
            0,
            new DocumentTextRange(10, 20),
            baseline.Blocks[0],
            null)]);

    private static ManagedDocumentSnapshot Snapshot(
        DocumentIdentity identity,
        DocumentRevision revision,
        string fingerprint,
        IReadOnlyList<BlockIdentity> blocks) => new(
        identity,
        revision,
        new DocumentTextRange(10, 10 + (blocks.Count * 10)),
        fingerprint,
        blocks.Select((block, index) => new ManagedBlockSnapshot(
            block,
            new DocumentTextRange(10 + (index * 10), 20 + (index * 10)))));

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static PublishStateVersions Versions() => new("2", "1", "1", "1", "1.0", "test");

    private static DocumentRevision Revision(long sequence) => new("revision-" + sequence, sequence);

    private static BlockIdentity Block(string id, string hash) =>
        new(id, null, "ch-v1:sha256:" + Hash(hash));

    private static PublishFingerprint Fingerprint(string value) =>
        new("v1:sha256:" + Hash(value));

    private static string Hash(string value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private sealed class StubExecutor : IPhysicalUpdateExecutor
    {
        private readonly PhysicalUpdateExecutionResult result;

        internal StubExecutor(PhysicalUpdateExecutionResult result)
        {
            this.result = result;
        }

        public Task<PhysicalUpdateExecutionResult> ExecuteAsync(
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken) => Task.FromResult(result);
    }

    private sealed class StubSnapshotReader : IDocumentSnapshotReader
    {
        private readonly ManagedDocumentSnapshot snapshot;

        internal StubSnapshotReader(ManagedDocumentSnapshot snapshot)
        {
            this.snapshot = snapshot;
        }

        public Task<ManagedDocumentSnapshot> GetSnapshotAsync(
            DocumentIdentity identity,
            CancellationToken cancellationToken) => Task.FromResult(snapshot);
    }

    private sealed class RecordingWriter : IVerifiedPublishStateWriter
    {
        public int SaveCount { get; private set; }

        public VerifiedPublishState? Saved { get; private set; }

        public Task SaveAsync(VerifiedPublishState state, CancellationToken cancellationToken)
        {
            SaveCount++;
            Saved = state;
            return Task.CompletedTask;
        }
    }
}
