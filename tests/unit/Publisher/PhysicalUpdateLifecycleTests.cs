using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class PhysicalUpdateLifecycleTests
{
    [Fact]
    public async Task ExecuteAsync_SavesOnlyAfterSuccessfulPhysicalReadback()
    {
        var baseline = Baseline();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var lifecycle = Lifecycle(store, adapter);

        var result = await lifecycle.ExecuteAsync(Candidate(), default);

        Assert.Equal(1, adapter.ApplyCount);
        Assert.Equal(1, store.SaveCount);
        Assert.Equal(2, result.State.Revision.Sequence);
        Assert.Same(result.State, store.SavedState);
    }

    [Fact]
    public async Task ExecuteAsync_ApplicationFailureDoesNotSaveState()
    {
        var baseline = Baseline();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline))
        {
            FailNextApply = true,
        };

        await Assert.ThrowsAsync<PhysicalUpdateException>(() => Lifecycle(store, adapter).ExecuteAsync(
            Candidate(), default));

        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task ExecuteAsync_RevisionConflictDoesNotSaveState()
    {
        var baseline = Baseline();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline, Revision(2)));

        var exception = await Assert.ThrowsAsync<PhysicalUpdateException>(
            () => Lifecycle(store, adapter).ExecuteAsync(Candidate(), default));

        Assert.Equal(UpdateErrorCodes.RevisionConflict, exception.Code);
        Assert.Equal(0, adapter.ApplyCount);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidPhysicalPlanDoesNotSaveState()
    {
        var baseline = Baseline();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var candidate = Candidate(includeDocument: false);

        var exception = await Assert.ThrowsAsync<PhysicalUpdateException>(
            () => Lifecycle(store, adapter).ExecuteAsync(candidate, default));

        Assert.Equal(UpdateErrorCodes.PhysicalPlanInvalid, exception.Code);
        Assert.Equal(0, adapter.ApplyCount);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task ExecuteAsync_ReadbackFailureDoesNotSaveState()
    {
        var baseline = Baseline();
        var store = new RecordingStore(baseline);
        var inner = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var adapter = new FailingReadbackAdapter(inner, failReadNumber: 3);

        var exception = await Assert.ThrowsAsync<PhysicalUpdateException>(
            () => Lifecycle(store, adapter).ExecuteAsync(Candidate(), default));

        Assert.Equal(UpdateErrorCodes.ReadbackFailed, exception.Code);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task DryRunAsync_DoesNotApplyOrSaveState()
    {
        var baseline = Baseline();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));

        var result = await Lifecycle(store, adapter).DryRunAsync(Candidate(), default);

        Assert.True(result.IsPublishRequired);
        Assert.Equal(2, result.PhysicalOperationCount);
        Assert.Equal(0, adapter.ApplyCount);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task DryRunAsync_RevisionConflictIsReportedWithoutDiffApplyOrSave()
    {
        var baseline = Baseline();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline, Revision(2)));

        var result = await Lifecycle(store, adapter).DryRunAsync(Candidate(), default);

        Assert.Equal([UpdateErrorCodes.RevisionConflict], result.Conflicts);
        Assert.Null(result.LogicalPlan);
        Assert.Null(result.PhysicalPlan);
        Assert.False(result.IsPublishRequired);
        Assert.Equal(0, adapter.ApplyCount);
        Assert.Equal(0, store.SaveCount);
    }

    private static VerifiedPublishLifecycle Lifecycle(
        IVerifiedPublishStateStore store,
        IManagedDocumentAdapter adapter) => new(
            store,
            store,
            new DiffEngine(),
            new PhysicalUpdateApplicationVerifier(adapter, new PhysicalUpdatePlanner()),
            new PublishResultVerifier(),
            new VerifiedPublishStatePromoter());

    private static VerifiedPublishState Baseline() => new(
        Identity(),
        Versions(),
        Revision(1),
        Fingerprint("baseline"),
        [Block("a", "old")]);

    private static PublishCandidate Candidate(bool includeDocument = true) => new(
        Identity(),
        Versions(),
        Fingerprint("candidate"),
        [Block("a", "new")],
        includeDocument
            ? new DocumentModel([new DocumentBlock(ParagraphBlock.FromText("new"), "a")])
            : null);

    private static ManagedDocumentSnapshot Snapshot(
        VerifiedPublishState baseline,
        DocumentRevision? revision = null) => new(
            baseline.Identity,
            revision ?? baseline.Revision,
            new DocumentTextRange(10, 20),
            baseline.Fingerprint.Value,
            [new ManagedBlockSnapshot(baseline.Blocks[0], new DocumentTextRange(10, 20))]);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static PublishStateVersions Versions() => new("2", "1", "1", "1", "1.0", "test");

    private static DocumentRevision Revision(long sequence) => new("revision-" + sequence, sequence);

    private static BlockIdentity Block(string id, string hash) =>
        new(id, null, "ch-v1:sha256:" + Hash(hash));

    private static PublishFingerprint Fingerprint(string seed) => new("v1:sha256:" + Hash(seed));

    private static string Hash(string seed) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(seed))).ToLowerInvariant();

    private sealed class RecordingStore : IVerifiedPublishStateStore
    {
        private readonly VerifiedPublishState baseline;

        internal RecordingStore(VerifiedPublishState baseline)
        {
            this.baseline = baseline;
        }

        internal int SaveCount { get; private set; }

        internal VerifiedPublishState? SavedState { get; private set; }

        public Task<VerifiedPublishState?> LoadAsync(
            PublishStateLoadRequest request,
            CancellationToken cancellationToken) => Task.FromResult<VerifiedPublishState?>(baseline);

        public Task SaveAsync(VerifiedPublishState state, CancellationToken cancellationToken)
        {
            SaveCount++;
            SavedState = state;
            return Task.CompletedTask;
        }
    }

    private sealed class FailingReadbackAdapter : IManagedDocumentAdapter
    {
        private readonly IManagedDocumentAdapter inner;
        private readonly int failReadNumber;
        private int readCount;

        internal FailingReadbackAdapter(IManagedDocumentAdapter inner, int failReadNumber)
        {
            this.inner = inner;
            this.failReadNumber = failReadNumber;
        }

        public Task<ManagedDocumentSnapshot> GetSnapshotAsync(
            DocumentIdentity identity,
            CancellationToken cancellationToken)
        {
            readCount++;
            return readCount == failReadNumber
                ? throw new InvalidOperationException("Injected readback failure.")
                : inner.GetSnapshotAsync(identity, cancellationToken);
        }

        public Task<PhysicalApplyReceipt> ApplyAsync(
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken) => inner.ApplyAsync(plan, cancellationToken);
    }
}
