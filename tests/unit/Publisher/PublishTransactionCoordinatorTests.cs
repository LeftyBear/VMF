using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class PublishTransactionCoordinatorTests : IDisposable
{
    private readonly string root = Path.Combine(
        Path.GetTempPath(),
        "vmf-publisher-transaction-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task ExecuteAsync_JournalsCommitUnknownBeforeRemoteMutationAndCompletesAfterStateSave()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var executor = new AdapterExecutor(adapter);
        var journal = Journal();
        var coordinator = Coordinator(store, adapter, executor, journal);

        var result = await coordinator.ExecuteAsync(candidate, default);
        var entry = await journal.LoadAsync(Key(), "google-document", default);

        Assert.True(result.PublishExecuted);
        Assert.NotNull(result.ApplicationResult?.SavedState);
        Assert.Equal(1, executor.CallCount);
        Assert.Equal(1, store.SaveCount);
        Assert.Equal(PublishTransactionStatus.Completed, Assert.IsType<PublishTransactionJournalEntry>(entry).Status);
        Assert.NotEmpty(entry.OperationIds);
        Assert.Contains(PublishTransactionStatus.CommitUnknown, journal.SavedStatuses);
        Assert.All(journal.SavedEntries, saved => Assert.Equal(entry.TransactionId, saved.TransactionId));
    }

    [Fact]
    public async Task ExecuteAsync_CommitUnknownRecoverySavesStateWithoutReexecutingPhysicalUpdate()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(candidate, Revision(2)));
        var executor = new AdapterExecutor(adapter);
        var journal = Journal();
        await journal.SaveAsync(new PublishTransactionJournalEntry(
            Key(),
            "google-document",
            "pending-transaction",
            PublishTransactionStatus.CommitUnknown,
            candidate.Fingerprint.Value,
            baseline.Fingerprint.Value,
            "revision-1"), default);
        var coordinator = Coordinator(store, adapter, executor, journal);

        var result = await coordinator.ExecuteAsync(candidate, default);

        Assert.False(result.PublishExecuted);
        Assert.Equal(PublishRecoveryStatus.AppliedAndStateSaved, result.RecoveryResult.Status);
        Assert.Equal(0, executor.CallCount);
        Assert.Equal(1, store.SaveCount);
        Assert.Contains(PublishTransactionStatus.StatePersistencePending, journal.SavedStatuses);
    }

    [Fact]
    public async Task ExecuteAsync_StatePersistencePendingReceiptRecoveryDoesNotReexecutePhysicalUpdate()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(candidate, Revision(2)));
        var executor = new AdapterExecutor(adapter);
        var journal = Journal();
        await journal.SaveAsync(new PublishTransactionJournalEntry(
            Key(),
            "google-document",
            "pending-transaction",
            PublishTransactionStatus.StatePersistencePending,
            candidate.Fingerprint.Value,
            baseline.Fingerprint.Value,
            "revision-1",
            "APPLIED",
            ["operation-id-1"]), default);
        var coordinator = Coordinator(store, adapter, executor, journal);

        var result = await coordinator.ExecuteAsync(candidate, default);
        var entry = await journal.LoadAsync(Key(), "google-document", default);

        Assert.False(result.PublishExecuted);
        Assert.Equal(PublishRecoveryStatus.AppliedAndStateSaved, result.RecoveryResult.Status);
        Assert.Equal(0, executor.CallCount);
        Assert.Equal(1, store.SaveCount);
        Assert.Equal(["operation-id-1"], Assert.IsType<PublishTransactionJournalEntry>(entry).OperationIds);
        Assert.Contains(PublishTransactionStatus.Completed, journal.SavedStatuses);
    }

    [Fact]
    public async Task ExecuteAsync_CommitUnknownBaselineMatchReplansAndExecutesNewPlan()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var executor = new AdapterExecutor(adapter);
        var journal = Journal();
        await journal.SaveAsync(new PublishTransactionJournalEntry(
            Key(),
            "google-document",
            "pending-transaction",
            PublishTransactionStatus.CommitUnknown,
            candidate.Fingerprint.Value,
            baseline.Fingerprint.Value,
            "revision-1"), default);
        var coordinator = Coordinator(store, adapter, executor, journal);

        var result = await coordinator.ExecuteAsync(candidate, default);

        Assert.True(result.PublishExecuted);
        Assert.Equal(PublishRecoveryStatus.SafeReplan, result.RecoveryResult.Status);
        Assert.Equal(1, executor.CallCount);
        Assert.Equal(1, store.SaveCount);
        Assert.Contains(PublishTransactionStatus.ReplanRequired, journal.SavedStatuses);
    }

    [Fact]
    public async Task ExecuteAsync_CommitUnknownDivergenceStopsBeforePhysicalUpdate()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(candidate, Revision(3), Fingerprint("third")));
        var executor = new AdapterExecutor(adapter);
        var journal = Journal();
        await journal.SaveAsync(new PublishTransactionJournalEntry(
            Key(),
            "google-document",
            "pending-transaction",
            PublishTransactionStatus.CommitUnknown,
            candidate.Fingerprint.Value,
            baseline.Fingerprint.Value,
            "revision-1"), default);
        var coordinator = Coordinator(store, adapter, executor, journal);

        var result = await coordinator.ExecuteAsync(candidate, default);

        Assert.False(result.PublishExecuted);
        Assert.Equal(PublishRecoveryStatus.Diverged, result.RecoveryResult.Status);
        Assert.Equal(0, executor.CallCount);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task DocumentLock_SerializesSameDocument()
    {
        var locks = LockManager();
        await using var first = await locks.AcquireAsync(Key(), default);

        var exception = await Assert.ThrowsAsync<DocumentLockException>(
            () => locks.AcquireAsync(Key(), default));

        Assert.Equal("LOCK_ALREADY_HELD", exception.Code);
    }

    [Fact]
    public async Task DocumentLock_AllowsDifferentDocumentIds()
    {
        var locks = LockManager();
        await using var first = await locks.AcquireAsync(Key(), default);
        await using var second = await locks.AcquireAsync(new PublishStateKey("publication", "other"), default);

        Assert.NotNull(second);
    }

    [Fact]
    public async Task DocumentLock_RejectsReleaseWhenLockIdChanged()
    {
        var locks = LockManager();
        var documentLock = await locks.AcquireAsync(Key(), "transaction", default);
        var path = locks.GetLockPath("document");
        await File.WriteAllBytesAsync(
            path,
            FileDocumentPublishLockManager.Serialize(new FileDocumentPublishLockManager.DocumentLockFile(
                "other-lock-id",
                "document",
                "transaction",
                Environment.ProcessId,
                Environment.MachineName,
                DateTimeOffset.UtcNow)));

        var exception = await Assert.ThrowsAsync<DocumentLockException>(
            async () => await documentLock.DisposeAsync());

        Assert.Equal("LOCK_RELEASE_NOT_OWNER", exception.Code);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task DocumentLock_CorruptionStopsAndPreservesEvidence()
    {
        var locks = LockManager();
        var documentLock = await locks.AcquireAsync(Key(), default);
        var path = locks.GetLockPath("document");
        var content = await File.ReadAllTextAsync(path, Encoding.UTF8);
        await File.WriteAllTextAsync(
            path,
            content.Replace("\"integrityHash\":\"sha256:", "\"integrityHash\":\"sha256:bad", StringComparison.Ordinal),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var exception = await Assert.ThrowsAsync<DocumentLockException>(
            () => locks.AcquireAsync(Key(), default));

        Assert.Equal("LOCK_CORRUPTED", exception.Code);
        Assert.True(File.Exists(path));
        var release = await Assert.ThrowsAsync<DocumentLockException>(
            async () => await documentLock.DisposeAsync());
        Assert.Equal("LOCK_CORRUPTED", release.Code);
    }

    [Fact]
    public async Task DocumentLock_ResidualLockStopsWithoutDeletingEvidence()
    {
        var locks = LockManager();
        var path = locks.GetLockPath("document");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using (File.Create(path))
        {
        }

        var exception = await Assert.ThrowsAsync<DocumentLockException>(
            () => locks.AcquireAsync(Key(), default));

        Assert.Equal("LOCK_CORRUPTED", exception.Code);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task Coordinator_DoesNotLoadStateBeforeLockIsAcquired()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var journal = Journal();
        var locks = new FailingLockManager();
        var coordinator = Coordinator(store, adapter, new AdapterExecutor(adapter), journal, locks);

        var exception = await Assert.ThrowsAsync<DocumentLockException>(
            () => coordinator.ExecuteAsync(candidate, default));

        Assert.Equal("LOCK_ALREADY_HELD", exception.Code);
        Assert.Equal(0, store.LoadCount);
        Assert.Equal(0, journal.LoadCount);
    }

    [Fact]
    public async Task Coordinator_ReleasesOwnedLockAfterFailure()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var locks = LockManager();
        var coordinator = Coordinator(
            store,
            adapter,
            new FailingExecutor(),
            Journal(),
            locks);

        await Assert.ThrowsAsync<InvalidOperationException>(() => coordinator.ExecuteAsync(candidate, default));
        await using var reacquired = await locks.AcquireAsync(Key(), default);

        Assert.NotNull(reacquired);
    }

    [Fact]
    public async Task Coordinator_ReleasesOwnedLockAfterCommit()
    {
        var baseline = Baseline();
        var candidate = Candidate();
        var store = new RecordingStore(baseline);
        var adapter = new InMemoryManagedDocumentAdapter(Snapshot(baseline));
        var locks = LockManager();
        var coordinator = Coordinator(store, adapter, new AdapterExecutor(adapter), Journal(), locks);

        await coordinator.ExecuteAsync(candidate, default);
        await using var reacquired = await locks.AcquireAsync(Key(), default);

        Assert.NotNull(reacquired);
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private RecordingJournal Journal() => new(new JsonPublishTransactionJournal(
        new PublishTransactionJournalOptions(root)));

    private FileDocumentPublishLockManager LockManager() => new(new DocumentLockFileOptions(root));

    private static PublishTransactionCoordinator Coordinator(
        IVerifiedPublishStateStore store,
        IManagedDocumentAdapter adapter,
        IPhysicalUpdateExecutor executor,
        IPublishTransactionJournal journal,
        IDocumentPublishLockManager? lockManager = null)
    {
        var diff = new DiffEngine();
        var planVerifier = new PhysicalUpdateApplicationVerifier(adapter, new PhysicalUpdatePlanner());
        var snapshotReader = Assert.IsAssignableFrom<IDocumentSnapshotReader>(adapter);
        var resultVerifier = new PublishResultVerifier();
        var promoter = new VerifiedPublishStatePromoter();
        var snapshotVerifier = new PhysicalUpdateApplicationSnapshotVerifier();
        var reconciler = new PhysicalUpdateRecoveryReconciler();
        return new PublishTransactionCoordinator(
            store,
            diff,
            planVerifier,
            new PhysicalUpdateApplicationService(
                executor,
                snapshotReader,
                reconciler,
                snapshotVerifier,
                resultVerifier,
                promoter,
                store),
            journal,
            lockManager ?? new FileDocumentPublishLockManager(new DocumentLockFileOptions(
                Path.Combine(Path.GetTempPath(), "vmf-publisher-lock-" + Guid.NewGuid().ToString("N")))),
            new PublishRecoveryEngine(
                snapshotReader,
                reconciler,
                snapshotVerifier,
                resultVerifier,
                promoter,
                store,
                journal));
    }

    private static VerifiedPublishState Baseline() => new(
        Identity(),
        Versions(),
        Revision(1),
        Fingerprint("baseline"),
        [Block("a", "old")]);

    private static PublishCandidate Candidate() => new(
        Identity(),
        Versions(),
        Fingerprint("candidate"),
        [Block("a", "new")],
        new DocumentModel([new DocumentBlock(ParagraphBlock.FromText("new"), "a")]));

    private static ManagedDocumentSnapshot Snapshot(
        VerifiedPublishState state,
        DocumentRevision? revision = null,
        PublishFingerprint? fingerprint = null) => new(
        state.Identity,
        revision ?? state.Revision,
        new DocumentTextRange(10, 20),
        (fingerprint ?? state.Fingerprint).Value,
        [new ManagedBlockSnapshot(state.Blocks[0], new DocumentTextRange(10, 20))]);

    private static ManagedDocumentSnapshot Snapshot(
        PublishCandidate candidate,
        DocumentRevision revision,
        PublishFingerprint? fingerprint = null) => new(
        candidate.Identity,
        revision,
        new DocumentTextRange(10, 20),
        (fingerprint ?? candidate.Fingerprint).Value,
        [new ManagedBlockSnapshot(candidate.Blocks[0], new DocumentTextRange(10, 20))]);

    private static PublishStateKey Key() => new("publication", "document");

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
        private VerifiedPublishState? state;

        internal RecordingStore(VerifiedPublishState? state)
        {
            this.state = state;
        }

        internal int SaveCount { get; private set; }

        internal int LoadCount { get; private set; }

        public Task<VerifiedPublishState?> LoadAsync(
            PublishStateLoadRequest request,
            CancellationToken cancellationToken)
        {
            LoadCount++;
            return Task.FromResult(state);
        }

        public Task SaveAsync(VerifiedPublishState state, CancellationToken cancellationToken)
        {
            SaveCount++;
            this.state = state;
            return Task.CompletedTask;
        }
    }

    private sealed class AdapterExecutor : IPhysicalUpdateExecutor
    {
        private readonly IManagedDocumentAdapter adapter;

        internal AdapterExecutor(IManagedDocumentAdapter adapter)
        {
            this.adapter = adapter;
        }

        internal int CallCount { get; private set; }

        public async Task<PhysicalUpdateExecutionResult> ExecuteAsync(
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken)
        {
            CallCount++;
            var receipt = await adapter.ApplyAsync(plan, cancellationToken).ConfigureAwait(false);
            return new PhysicalUpdateExecutionResult(
                PhysicalUpdateExecutionStatus.Applied,
                plan.Identity.GoogleDocumentId!,
                plan.RequiredRevision.RevisionId,
                receipt.Revision.RevisionId,
                plan.Operations.Count,
                plan.Operations.Count,
                1,
                "APPLIED",
                "adapter applied",
                operationIds: receipt.OperationIds);
        }
    }

    private sealed class FailingExecutor : IPhysicalUpdateExecutor
    {
        public Task<PhysicalUpdateExecutionResult> ExecuteAsync(
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken) => throw new InvalidOperationException("Injected failure.");
    }

    private sealed class RecordingJournal : IPublishTransactionJournal
    {
        private readonly IPublishTransactionJournal inner;

        internal RecordingJournal(IPublishTransactionJournal inner)
        {
            this.inner = inner;
        }

        internal List<PublishTransactionStatus> SavedStatuses { get; } = [];

        internal List<PublishTransactionJournalEntry> SavedEntries { get; } = [];

        internal int LoadCount { get; private set; }

        public Task<PublishTransactionJournalEntry?> LoadAsync(
            PublishStateKey key,
            string? expectedGoogleDocumentId,
            CancellationToken cancellationToken)
        {
            LoadCount++;
            return inner.LoadAsync(key, expectedGoogleDocumentId, cancellationToken);
        }

        public Task SaveAsync(PublishTransactionJournalEntry entry, CancellationToken cancellationToken)
        {
            SavedStatuses.Add(entry.Status);
            SavedEntries.Add(entry);
            return inner.SaveAsync(entry, cancellationToken);
        }

        public Task CompleteAsync(PublishTransactionJournalEntry entry, CancellationToken cancellationToken)
        {
            SavedStatuses.Add(PublishTransactionStatus.Completed);
            SavedEntries.Add(entry.With(PublishTransactionStatus.Completed));
            return inner.CompleteAsync(entry, cancellationToken);
        }
    }

    private sealed class FailingLockManager : IDocumentPublishLockManager
    {
        public Task<IDocumentPublishLock> AcquireAsync(
            PublishStateKey key,
            CancellationToken cancellationToken) => throw new DocumentLockException(
            "LOCK_ALREADY_HELD",
            "Injected lock failure.");

        public Task<IDocumentPublishLock> AcquireAsync(
            PublishStateKey key,
            string transactionId,
            CancellationToken cancellationToken) => throw new DocumentLockException(
            "LOCK_ALREADY_HELD",
            "Injected lock failure.");
    }
}
