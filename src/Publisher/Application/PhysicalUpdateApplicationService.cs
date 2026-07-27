using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Represents recovery reconciliation status for an indeterminate update.</summary>
public enum RecoveryReconciliationStatus
{
    /// <summary>The candidate snapshot is fully present.</summary>
    Applied,

    /// <summary>The baseline snapshot is still fully present.</summary>
    NotApplied,

    /// <summary>The document matches neither baseline nor candidate.</summary>
    Diverged,
}

/// <summary>Reads managed document snapshots for application-level verification.</summary>
public interface IDocumentSnapshotReader
{
    /// <summary>Reads the current snapshot.</summary>
    Task<ManagedDocumentSnapshot> GetSnapshotAsync(
        DocumentIdentity identity,
        CancellationToken cancellationToken);
}

/// <summary>Reconciles indeterminate physical update execution against known states.</summary>
public interface IPhysicalUpdateRecoveryReconciler
{
    /// <summary>Compares the current snapshot with baseline and candidate evidence.</summary>
    RecoveryReconciliationStatus Reconcile(
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        ManagedDocumentSnapshot currentSnapshot);
}

/// <summary>Verifies that a post-apply snapshot fully matches the candidate.</summary>
public interface IPhysicalUpdateApplicationSnapshotVerifier
{
    /// <summary>Verifies the snapshot and returns lifecycle evidence.</summary>
    PublishApplicationVerification VerifyApplied(
        PublishCandidate candidate,
        DiffPlan plan,
        ManagedDocumentSnapshot readback);
}

/// <summary>Represents the application-level outcome of executing a physical update.</summary>
public sealed class PhysicalUpdateApplicationResult
{
    /// <summary>Initializes an application result.</summary>
    public PhysicalUpdateApplicationResult(
        PhysicalUpdateExecutionResult executionResult,
        RecoveryReconciliationStatus? recoveryStatus,
        VerifiedPublishState? savedState,
        bool replanRequired)
    {
        ExecutionResult = executionResult ?? throw new ArgumentNullException(nameof(executionResult));
        RecoveryStatus = recoveryStatus;
        SavedState = savedState;
        ReplanRequired = replanRequired;
    }

    /// <summary>Gets the physical execution result.</summary>
    public PhysicalUpdateExecutionResult ExecutionResult { get; }

    /// <summary>Gets recovery status, when reconciliation was required.</summary>
    public RecoveryReconciliationStatus? RecoveryStatus { get; }

    /// <summary>Gets the verified state saved at the end of the flow.</summary>
    public VerifiedPublishState? SavedState { get; }

    /// <summary>Gets whether an upper layer must create a new plan.</summary>
    public bool ReplanRequired { get; }
}

/// <summary>Coordinates execution, readback verification, recovery, and verified-state save.</summary>
public sealed class PhysicalUpdateApplicationService
{
    private readonly IPhysicalUpdateExecutor executor;
    private readonly IDocumentSnapshotReader snapshotReader;
    private readonly IPhysicalUpdateRecoveryReconciler recoveryReconciler;
    private readonly IPhysicalUpdateApplicationSnapshotVerifier applicationVerifier;
    private readonly IPublishResultVerifier resultVerifier;
    private readonly IVerifiedPublishStatePromoter promoter;
    private readonly IVerifiedPublishStateWriter stateWriter;

    /// <summary>Initializes the application service.</summary>
    public PhysicalUpdateApplicationService(
        IPhysicalUpdateExecutor executor,
        IDocumentSnapshotReader snapshotReader,
        IPhysicalUpdateRecoveryReconciler recoveryReconciler,
        IPhysicalUpdateApplicationSnapshotVerifier applicationVerifier,
        IPublishResultVerifier resultVerifier,
        IVerifiedPublishStatePromoter promoter,
        IVerifiedPublishStateWriter stateWriter)
    {
        this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
        this.snapshotReader = snapshotReader ?? throw new ArgumentNullException(nameof(snapshotReader));
        this.recoveryReconciler = recoveryReconciler
            ?? throw new ArgumentNullException(nameof(recoveryReconciler));
        this.applicationVerifier = applicationVerifier
            ?? throw new ArgumentNullException(nameof(applicationVerifier));
        this.resultVerifier = resultVerifier ?? throw new ArgumentNullException(nameof(resultVerifier));
        this.promoter = promoter ?? throw new ArgumentNullException(nameof(promoter));
        this.stateWriter = stateWriter ?? throw new ArgumentNullException(nameof(stateWriter));
    }

    /// <summary>Executes a prepared physical plan and saves verified state only after complete verification.</summary>
    public async Task<PhysicalUpdateApplicationResult> ExecuteAsync(
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        DiffPlan logicalPlan,
        PhysicalUpdatePlan physicalPlan,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(logicalPlan);
        ArgumentNullException.ThrowIfNull(physicalPlan);

        var execution = await executor.ExecuteAsync(physicalPlan, cancellationToken).ConfigureAwait(false);
        if (execution.Status is PhysicalUpdateExecutionStatus.NoChange or PhysicalUpdateExecutionStatus.Applied)
        {
            var state = await VerifyAndSaveAsync(
                baseline,
                candidate,
                logicalPlan,
                cancellationToken).ConfigureAwait(false);
            return new PhysicalUpdateApplicationResult(execution, null, state, replanRequired: false);
        }

        if (execution.Status == PhysicalUpdateExecutionStatus.IndeterminateFailure)
        {
            var current = await snapshotReader.GetSnapshotAsync(candidate.Identity, cancellationToken)
                .ConfigureAwait(false);
            var recovery = recoveryReconciler.Reconcile(baseline, candidate, current);
            if (recovery == RecoveryReconciliationStatus.Applied)
            {
                var state = await VerifyAndSaveAsync(
                    baseline,
                    candidate,
                    logicalPlan,
                    cancellationToken).ConfigureAwait(false);
                return new PhysicalUpdateApplicationResult(execution, recovery, state, replanRequired: false);
            }

            return new PhysicalUpdateApplicationResult(
                execution,
                recovery,
                savedState: null,
                replanRequired: recovery == RecoveryReconciliationStatus.NotApplied);
        }

        return new PhysicalUpdateApplicationResult(
            execution,
            recoveryStatus: null,
            savedState: null,
            replanRequired: execution.Status == PhysicalUpdateExecutionStatus.TransientFailure);
    }

    private async Task<VerifiedPublishState> VerifyAndSaveAsync(
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        DiffPlan logicalPlan,
        CancellationToken cancellationToken)
    {
        var readback = await snapshotReader.GetSnapshotAsync(candidate.Identity, cancellationToken)
            .ConfigureAwait(false);
        var evidence = applicationVerifier.VerifyApplied(candidate, logicalPlan, readback);
        var verifiedResult = resultVerifier.Verify(candidate, logicalPlan, evidence);
        var state = promoter.Promote(baseline, verifiedResult);
        await stateWriter.SaveAsync(state, cancellationToken).ConfigureAwait(false);
        return state;
    }
}

/// <summary>Compares readback snapshots using canonical identity, order, hashes, and fingerprint.</summary>
public sealed class PhysicalUpdateRecoveryReconciler : IPhysicalUpdateRecoveryReconciler
{
    /// <inheritdoc />
    public RecoveryReconciliationStatus Reconcile(
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        ManagedDocumentSnapshot currentSnapshot)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(currentSnapshot);
        if (MatchesCandidate(candidate, currentSnapshot))
        {
            return RecoveryReconciliationStatus.Applied;
        }

        if (baseline is not null && MatchesBaseline(baseline, currentSnapshot))
        {
            return RecoveryReconciliationStatus.NotApplied;
        }

        return RecoveryReconciliationStatus.Diverged;
    }

    private static bool MatchesCandidate(PublishCandidate candidate, ManagedDocumentSnapshot snapshot) =>
        SameDocument(candidate.Identity, snapshot.Identity) &&
        string.Equals(snapshot.PublishFingerprint, candidate.Fingerprint.Value, StringComparison.Ordinal) &&
        SameBlocks(candidate.Blocks, snapshot.Blocks.Select(item => item.Identity));

    private static bool MatchesBaseline(VerifiedPublishState baseline, ManagedDocumentSnapshot snapshot) =>
        SameDocument(baseline.Identity, snapshot.Identity) &&
        string.Equals(snapshot.PublishFingerprint, baseline.Fingerprint.Value, StringComparison.Ordinal) &&
        SameBlocks(baseline.Blocks, snapshot.Blocks.Select(item => item.Identity));

    private static bool SameBlocks(
        IReadOnlyList<BlockIdentity> expected,
        IEnumerable<BlockIdentity> actual)
    {
        var actualItems = actual.ToArray();
        return expected.Count == actualItems.Length &&
            expected.Zip(actualItems).All(pair =>
                string.Equals(pair.First.ExplicitId, pair.Second.ExplicitId, StringComparison.Ordinal) &&
                string.Equals(pair.First.GeneratedId, pair.Second.GeneratedId, StringComparison.Ordinal) &&
                string.Equals(pair.First.ContentHash, pair.Second.ContentHash, StringComparison.Ordinal));
    }

    private static bool SameDocument(DocumentIdentity expected, DocumentIdentity actual) =>
        string.Equals(expected.PublicationId, actual.PublicationId, StringComparison.Ordinal) &&
        string.Equals(expected.DocumentId, actual.DocumentId, StringComparison.Ordinal) &&
        string.Equals(expected.GoogleDocumentId, actual.GoogleDocumentId, StringComparison.Ordinal) &&
        expected.State == actual.State;
}

/// <summary>Verifies readback snapshots before state promotion.</summary>
public sealed class PhysicalUpdateApplicationSnapshotVerifier :
    IPhysicalUpdateApplicationSnapshotVerifier
{
    /// <inheritdoc />
    public PublishApplicationVerification VerifyApplied(
        PublishCandidate candidate,
        DiffPlan plan,
        ManagedDocumentSnapshot readback)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(readback);
        if (!SameDocument(candidate.Identity, readback.Identity) ||
            !string.Equals(readback.PublishFingerprint, candidate.Fingerprint.Value, StringComparison.Ordinal) ||
            !SameBlocks(candidate.Blocks, readback.Blocks.Select(item => item.Identity)))
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ReadbackMismatch,
                "The post-apply snapshot does not fully match the candidate.");
        }

        ValidateRanges(readback);
        return new PublishApplicationVerification(
            readback.Identity,
            plan,
            isLogicalPlanApplied: true,
            isReadbackVerified: true,
            readback.PublishFingerprint,
            readback.Blocks.Select(item => item.Identity),
            readback.Revision);
    }

    private static void ValidateRanges(ManagedDocumentSnapshot snapshot)
    {
        var previousEnd = snapshot.ManagedRegion.StartIndex;
        foreach (var block in snapshot.Blocks)
        {
            if (block.Range.StartIndex != previousEnd ||
                block.Range.StartIndex < snapshot.ManagedRegion.StartIndex ||
                block.Range.EndIndex > snapshot.ManagedRegion.EndIndex)
            {
                throw new PhysicalUpdateException(
                    UpdateErrorCodes.ManagedRegionMismatch,
                    "Readback block ranges are not contiguous within the managed region.");
            }

            previousEnd = block.Range.EndIndex;
        }

        if (previousEnd != snapshot.ManagedRegion.EndIndex)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ManagedRegionMismatch,
                "Readback block ranges do not cover the managed region.");
        }
    }

    private static bool SameBlocks(
        IReadOnlyList<BlockIdentity> expected,
        IEnumerable<BlockIdentity> actual)
    {
        var actualItems = actual.ToArray();
        return expected.Count == actualItems.Length &&
            expected.Zip(actualItems).All(pair =>
                string.Equals(pair.First.ExplicitId, pair.Second.ExplicitId, StringComparison.Ordinal) &&
                string.Equals(pair.First.GeneratedId, pair.Second.GeneratedId, StringComparison.Ordinal) &&
                string.Equals(pair.First.ContentHash, pair.Second.ContentHash, StringComparison.Ordinal));
    }

    private static bool SameDocument(DocumentIdentity expected, DocumentIdentity actual) =>
        string.Equals(expected.PublicationId, actual.PublicationId, StringComparison.Ordinal) &&
        string.Equals(expected.DocumentId, actual.DocumentId, StringComparison.Ordinal) &&
        string.Equals(expected.GoogleDocumentId, actual.GoogleDocumentId, StringComparison.Ordinal) &&
        expected.State == actual.State;
}
