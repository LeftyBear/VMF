using Vmf.Publisher.Application;
using Vmf.Publisher.Composition;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class VerifiedPublishLifecycleTests : IDisposable
{
    private readonly string root = Path.Combine(
        Path.GetTempPath(),
        "vmf-publisher-lifecycle-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task ExecuteAsync_SavesAndReturnsSuccessOnlyAfterCompleteVerification()
    {
        var calls = new List<string>();
        var candidate = Candidate();
        var reader = new RecordingStore(calls);
        var diff = new RecordingDiffEngine(calls);
        var application = new RecordingApplication(calls, VerificationMode.Success);
        var lifecycle = Lifecycle(reader, reader, diff, application);

        var result = await lifecycle.ExecuteAsync(candidate, default);

        Assert.Equal(["load", "prepare", "plan", "apply", "save"], calls);
        Assert.Same(reader.SavedState, result.State);
        Assert.Equal(candidate.Fingerprint, result.State.Fingerprint);
        Assert.Equal(DocumentState.Active, result.State.Identity.State);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotSaveWhenExternalApplicationFails()
    {
        var store = new RecordingStore([]);
        var lifecycle = Lifecycle(
            store,
            store,
            new DiffEngine(),
            new RecordingApplication([], VerificationMode.ThrowDuringApply));

        await Assert.ThrowsAsync<InvalidOperationException>(() => lifecycle.ExecuteAsync(Candidate(), default));

        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotSaveWhenReadbackFails()
    {
        var store = new RecordingStore([]);
        var lifecycle = Lifecycle(
            store,
            store,
            new DiffEngine(),
            new RecordingApplication([], VerificationMode.ReadbackFailed));

        var exception = await Assert.ThrowsAsync<StateLifecycleException>(
            () => lifecycle.ExecuteAsync(Candidate(), default));

        Assert.Equal(StateErrorCodes.VerificationRequired, exception.Code);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotSaveWhenVerifiedPayloadMismatches()
    {
        var store = new RecordingStore([]);
        var lifecycle = Lifecycle(
            store,
            store,
            new DiffEngine(),
            new RecordingApplication([], VerificationMode.PayloadMismatch));

        var exception = await Assert.ThrowsAsync<StateLifecycleException>(
            () => lifecycle.ExecuteAsync(Candidate(), default));

        Assert.Equal(StateErrorCodes.VerificationMismatch, exception.Code);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotReturnSuccessWhenSaveFails()
    {
        var store = new RecordingStore([], failSave: true);
        var lifecycle = Lifecycle(
            store,
            store,
            new DiffEngine(),
            new RecordingApplication([], VerificationMode.Success));

        var exception = await Assert.ThrowsAsync<StateLifecycleException>(
            () => lifecycle.ExecuteAsync(Candidate(), default));

        Assert.Equal(StateErrorCodes.SaveFailed, exception.Code);
        Assert.Null(store.SavedState);
    }

    [Fact]
    public async Task ExecuteAsync_LoadsBaselineBeforePlanningAndApplication()
    {
        var calls = new List<string>();
        var candidate = Candidate();
        var baseline = State(DocumentState.Active, 'b');
        var store = new RecordingStore(calls, baseline);
        var lifecycle = Lifecycle(
            store,
            store,
            new RecordingDiffEngine(calls),
            new RecordingApplication(calls, VerificationMode.Success));

        var result = await lifecycle.ExecuteAsync(candidate, default);

        Assert.Equal(["load", "prepare", "plan", "apply", "save"], calls);
        Assert.Equal(baseline.Fingerprint, result.Plan.PreviousFingerprint);
    }

    [Fact]
    public async Task ExecuteAsync_ArchivedBaselineCannotBeImplicitlyReactivatedOrSaved()
    {
        var calls = new List<string>();
        var store = new RecordingStore(calls, State(DocumentState.Archived, 'b'));
        var lifecycle = Lifecycle(
            store,
            store,
            new DiffEngine(),
            new RecordingApplication(calls, VerificationMode.Success));

        var exception = await Assert.ThrowsAsync<StateLifecycleException>(
            () => lifecycle.ExecuteAsync(Candidate(), default));

        Assert.Equal(StateErrorCodes.InvalidTransition, exception.Code);
        Assert.Equal(0, store.SaveCount);
        Assert.Equal(["load"], calls);
    }

    [Fact]
    public async Task CompositionRoot_WiresStoreAndLifecycleWithoutGoogleImplementation()
    {
        var store = PublisherCompositionRoot.CreateVerifiedPublishStateStore(root);
        var lifecycle = PublisherCompositionRoot.CreateVerifiedPublishLifecycle(
            store,
            new RecordingApplication([], VerificationMode.Success));

        var result = await lifecycle.ExecuteAsync(Candidate(), default);
        var restored = await store.LoadAsync(Request(), default);

        Assert.Equal(result.State.Fingerprint, Assert.IsType<VerifiedPublishState>(restored).Fingerprint);
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static VerifiedPublishLifecycle Lifecycle(
        IVerifiedPublishStateReader reader,
        IVerifiedPublishStateWriter writer,
        IDiffEngine diffEngine,
        IPublishPlanApplicationVerifier application) => new(
            reader,
            writer,
            diffEngine,
            application,
            new PublishResultVerifier(),
            new VerifiedPublishStatePromoter());

    private static PublishCandidate Candidate() => new(
        Identity(DocumentState.Active),
        Versions(),
        Fingerprint('a'),
        Blocks());

    private static VerifiedPublishState State(DocumentState state, char fingerprintDigit) => new(
        Identity(state),
        Versions(),
        Revision(1),
        Fingerprint(fingerprintDigit),
        Blocks());

    private static DocumentIdentity Identity(DocumentState state) =>
        new("publication", "document", "google-document", state);

    private static PublishStateVersions Versions() => new("2", "1", "1", "1", "1.0", "1.0.0");

    private static DocumentRevision Revision(long sequence) => new("revision-" + sequence, sequence);

    private static PublishFingerprint Fingerprint(char digit) =>
        new("v1:sha256:" + new string(digit, 64));

    private static BlockIdentity[] Blocks() =>
    [
        new BlockIdentity("intro", null, "ch-v1:sha256:" + new string('c', 64)),
        new BlockIdentity(
            null,
            "gid-v1:sha256:" + new string('d', 64),
            "ch-v1:sha256:" + new string('e', 64)),
    ];

    private static PublishStateLoadRequest Request() => new(
        new PublishStateKey("publication", "document"),
        "google-document");

    private sealed class RecordingStore : IVerifiedPublishStateStore
    {
        private readonly List<string> calls;
        private readonly VerifiedPublishState? baseline;
        private readonly bool failSave;

        internal RecordingStore(
            List<string> calls,
            VerifiedPublishState? baseline = null,
            bool failSave = false)
        {
            this.calls = calls;
            this.baseline = baseline;
            this.failSave = failSave;
        }

        internal int SaveCount { get; private set; }

        internal VerifiedPublishState? SavedState { get; private set; }

        public Task<VerifiedPublishState?> LoadAsync(
            PublishStateLoadRequest request,
            CancellationToken cancellationToken)
        {
            calls.Add("load");
            return Task.FromResult(baseline);
        }

        public Task SaveAsync(VerifiedPublishState state, CancellationToken cancellationToken)
        {
            calls.Add("save");
            SaveCount++;
            if (failSave)
            {
                throw new StateLifecycleException(StateErrorCodes.SaveFailed, "Injected save failure.");
            }

            SavedState = state;
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingDiffEngine : IDiffEngine
    {
        private readonly List<string> calls;

        internal RecordingDiffEngine(List<string> calls)
        {
            this.calls = calls;
        }

        public DiffPlan CreatePlan(VerifiedPublishState? baseline, PublishCandidate candidate)
        {
            calls.Add("plan");
            return new DiffEngine().CreatePlan(baseline, candidate);
        }
    }

    private sealed class RecordingApplication : IPublishPlanApplicationVerifier
    {
        private readonly List<string> calls;
        private readonly VerificationMode mode;

        internal RecordingApplication(List<string> calls, VerificationMode mode)
        {
            this.calls = calls;
            this.mode = mode;
        }

        public Task<ManagedDocumentSnapshot> PrepareAsync(
            PublishCandidate candidate,
            VerifiedPublishState? baseline,
            CancellationToken cancellationToken)
        {
            calls.Add("prepare");
            var blocks = (baseline?.Blocks ?? Array.Empty<BlockIdentity>())
                .Select((block, index) => new ManagedBlockSnapshot(
                    block,
                    new DocumentTextRange(index + 1, index + 2)))
                .ToArray();
            return Task.FromResult(new ManagedDocumentSnapshot(
                candidate.Identity,
                baseline?.Revision ?? Revision(0),
                new DocumentTextRange(1, blocks.Length + 1),
                baseline?.Fingerprint.Value ?? candidate.Fingerprint.Value,
                blocks));
        }

        public Task<PublishApplicationVerification> ApplyAndVerifyAsync(
            PublishCandidate candidate,
            VerifiedPublishState? baseline,
            DiffPlan plan,
            ManagedDocumentSnapshot preparedSnapshot,
            CancellationToken cancellationToken)
        {
            calls.Add("apply");
            if (mode == VerificationMode.ThrowDuringApply)
            {
                throw new InvalidOperationException("Injected application failure.");
            }

            var blocks = mode == VerificationMode.PayloadMismatch
                ? candidate.Blocks.Reverse()
                : candidate.Blocks;
            return Task.FromResult(new PublishApplicationVerification(
                candidate.Identity,
                plan,
                isLogicalPlanApplied: true,
                isReadbackVerified: mode != VerificationMode.ReadbackFailed,
                candidate.Fingerprint.Value,
                blocks,
                Revision(preparedSnapshot.Revision.Sequence + 1)));
        }

        public PhysicalUpdateDryRunResult CreateDryRun(
            PublishCandidate candidate,
            VerifiedPublishState? baseline,
            DiffPlan plan,
            ManagedDocumentSnapshot preparedSnapshot) => new(plan, null, Array.Empty<string>());
    }

    private enum VerificationMode
    {
        Success,
        ThrowDuringApply,
        ReadbackFailed,
        PayloadMismatch,
    }
}
