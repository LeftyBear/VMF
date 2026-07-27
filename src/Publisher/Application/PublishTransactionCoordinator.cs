using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Represents durable physical publish transaction progress.</summary>
public enum PublishTransactionStatus
{
    /// <summary>The transaction has started but no remote mutation is known to be in flight.</summary>
    Started,

    /// <summary>The physical plan is prepared and recovery may safely replan before mutation.</summary>
    Planned,

    /// <summary>A remote commit may have been sent and must be reconciled before retry.</summary>
    CommitUnknown,

    /// <summary>The remote document was reconciled as candidate-applied and state persistence is pending.</summary>
    StatePersistencePending,

    /// <summary>The transaction completed and may be ignored by recovery.</summary>
    Completed,

    /// <summary>The transaction ended before remote mutation and a fresh plan is allowed.</summary>
    ReplanRequired,

    /// <summary>The transaction could not be reconciled deterministically.</summary>
    Diverged,
}

/// <summary>Represents a persisted transaction journal entry.</summary>
public sealed class PublishTransactionJournalEntry
{
    /// <summary>Initializes a transaction journal entry.</summary>
    public PublishTransactionJournalEntry(
        PublishStateKey key,
        string? googleDocumentId,
        string transactionId,
        PublishTransactionStatus status,
        string candidateFingerprint,
        string? baselineFingerprint = null,
        string? requiredRevisionId = null,
        string? diagnosticCode = null)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        if (googleDocumentId is not null && string.IsNullOrWhiteSpace(googleDocumentId))
        {
            throw new ArgumentException("A Google document identifier must not be empty.", nameof(googleDocumentId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(candidateFingerprint);
        TransactionId = transactionId;
        CandidateFingerprint = candidateFingerprint;
        BaselineFingerprint = baselineFingerprint;
        RequiredRevisionId = requiredRevisionId;
        DiagnosticCode = diagnosticCode;
        GoogleDocumentId = googleDocumentId;
        Status = status;
    }

    /// <summary>Gets the verified-state key.</summary>
    public PublishStateKey Key { get; }

    /// <summary>Gets the expected remote document identifier.</summary>
    public string? GoogleDocumentId { get; }

    /// <summary>Gets the transaction identifier shared with the document lock.</summary>
    public string TransactionId { get; }

    /// <summary>Gets the durable progress state.</summary>
    public PublishTransactionStatus Status { get; }

    /// <summary>Gets the candidate fingerprint associated with this transaction.</summary>
    public string CandidateFingerprint { get; }

    /// <summary>Gets the baseline fingerprint, when one existed.</summary>
    public string? BaselineFingerprint { get; }

    /// <summary>Gets the physical update revision precondition, when planned.</summary>
    public string? RequiredRevisionId { get; }

    /// <summary>Gets the last stable diagnostic code, when available.</summary>
    public string? DiagnosticCode { get; }

    /// <summary>Creates a copy with updated progress fields.</summary>
    public PublishTransactionJournalEntry With(
        PublishTransactionStatus status,
        string? requiredRevisionId = null,
        string? diagnosticCode = null) => new(
            Key,
            GoogleDocumentId,
            TransactionId,
            status,
            CandidateFingerprint,
            BaselineFingerprint,
            requiredRevisionId ?? RequiredRevisionId,
            diagnosticCode ?? DiagnosticCode);
}

/// <summary>Persists and restores transaction journal entries.</summary>
public interface IPublishTransactionJournal
{
    /// <summary>Loads the active journal entry for a document, when present.</summary>
    Task<PublishTransactionJournalEntry?> LoadAsync(
        PublishStateKey key,
        string? expectedGoogleDocumentId,
        CancellationToken cancellationToken);

    /// <summary>Atomically saves transaction progress.</summary>
    Task SaveAsync(PublishTransactionJournalEntry entry, CancellationToken cancellationToken);

    /// <summary>Atomically marks a transaction terminal.</summary>
    Task CompleteAsync(PublishTransactionJournalEntry entry, CancellationToken cancellationToken);
}

/// <summary>Represents an acquired per-document publish lock.</summary>
public interface IDocumentPublishLock : IAsyncDisposable
{
    /// <summary>Gets the lock identifier that must match before release.</summary>
    string LockId { get; }

    /// <summary>Gets the document identifier protected by this lock.</summary>
    string DocumentId { get; }

    /// <summary>Gets the transaction identifier protected by this lock.</summary>
    string TransactionId { get; }
}

/// <summary>Serializes publishers per document.</summary>
public interface IDocumentPublishLockManager
{
    /// <summary>Acquires the lock for one document key.</summary>
    Task<IDocumentPublishLock> AcquireAsync(PublishStateKey key, CancellationToken cancellationToken);

    /// <summary>Acquires the lock for one document key and transaction.</summary>
    Task<IDocumentPublishLock> AcquireAsync(
        PublishStateKey key,
        string transactionId,
        CancellationToken cancellationToken);
}

/// <summary>Represents deterministic recovery status.</summary>
public enum PublishRecoveryStatus
{
    /// <summary>No pending transaction required recovery.</summary>
    None,

    /// <summary>The candidate was already applied remotely and verified state was saved.</summary>
    AppliedAndStateSaved,

    /// <summary>No remote mutation was found; normal publish may safely replan.</summary>
    SafeReplan,

    /// <summary>The remote document matches neither baseline nor candidate.</summary>
    Diverged,
}

/// <summary>Represents recovery execution output.</summary>
public sealed class PublishRecoveryResult
{
    /// <summary>Initializes a recovery result.</summary>
    public PublishRecoveryResult(PublishRecoveryStatus status, VerifiedPublishState? savedState)
    {
        Status = status;
        SavedState = savedState;
    }

    /// <summary>Gets the recovery status.</summary>
    public PublishRecoveryStatus Status { get; }

    /// <summary>Gets the saved state, when recovery committed one.</summary>
    public VerifiedPublishState? SavedState { get; }
}

/// <summary>Owns the only crash-recovery entrypoint for pending publish transactions.</summary>
public interface IPublishRecoveryEngine
{
    /// <summary>Recovers any active journal before normal publish starts.</summary>
    Task<PublishRecoveryResult> RecoverAsync(
        PublishTransactionJournalEntry? entry,
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        DiffPlan logicalPlan,
        CancellationToken cancellationToken);
}

/// <summary>Runs deterministic recovery from the transaction journal.</summary>
public sealed class PublishRecoveryEngine : IPublishRecoveryEngine
{
    private readonly IDocumentSnapshotReader snapshotReader;
    private readonly IPhysicalUpdateRecoveryReconciler reconciler;
    private readonly IPhysicalUpdateApplicationSnapshotVerifier applicationVerifier;
    private readonly IPublishResultVerifier resultVerifier;
    private readonly IVerifiedPublishStatePromoter promoter;
    private readonly IVerifiedPublishStateWriter stateWriter;
    private readonly IPublishTransactionJournal journal;

    /// <summary>Initializes the recovery engine.</summary>
    public PublishRecoveryEngine(
        IDocumentSnapshotReader snapshotReader,
        IPhysicalUpdateRecoveryReconciler reconciler,
        IPhysicalUpdateApplicationSnapshotVerifier applicationVerifier,
        IPublishResultVerifier resultVerifier,
        IVerifiedPublishStatePromoter promoter,
        IVerifiedPublishStateWriter stateWriter,
        IPublishTransactionJournal journal)
    {
        this.snapshotReader = snapshotReader ?? throw new ArgumentNullException(nameof(snapshotReader));
        this.reconciler = reconciler ?? throw new ArgumentNullException(nameof(reconciler));
        this.applicationVerifier = applicationVerifier ?? throw new ArgumentNullException(nameof(applicationVerifier));
        this.resultVerifier = resultVerifier ?? throw new ArgumentNullException(nameof(resultVerifier));
        this.promoter = promoter ?? throw new ArgumentNullException(nameof(promoter));
        this.stateWriter = stateWriter ?? throw new ArgumentNullException(nameof(stateWriter));
        this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
    }

    /// <inheritdoc />
    public async Task<PublishRecoveryResult> RecoverAsync(
        PublishTransactionJournalEntry? entry,
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        DiffPlan logicalPlan,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(logicalPlan);
        if (entry is null || entry.Status == PublishTransactionStatus.Completed)
        {
            return new PublishRecoveryResult(PublishRecoveryStatus.None, null);
        }

        if (!string.Equals(entry.CandidateFingerprint, candidate.Fingerprint.Value, StringComparison.Ordinal))
        {
            await journal.SaveAsync(entry.With(PublishTransactionStatus.Diverged, diagnosticCode: "JOURNAL_CANDIDATE_MISMATCH"), cancellationToken)
                .ConfigureAwait(false);
            return new PublishRecoveryResult(PublishRecoveryStatus.Diverged, null);
        }

        if (entry.Status is PublishTransactionStatus.Started or PublishTransactionStatus.Planned or PublishTransactionStatus.ReplanRequired)
        {
            await journal.SaveAsync(entry.With(PublishTransactionStatus.ReplanRequired, diagnosticCode: "SAFE_REPLAN"), cancellationToken)
                .ConfigureAwait(false);
            return new PublishRecoveryResult(PublishRecoveryStatus.SafeReplan, null);
        }

        var current = await snapshotReader.GetSnapshotAsync(candidate.Identity, cancellationToken)
            .ConfigureAwait(false);
        var status = reconciler.Reconcile(baseline, candidate, current);
        if (status == RecoveryReconciliationStatus.Applied)
        {
            await journal.SaveAsync(entry.With(PublishTransactionStatus.StatePersistencePending, diagnosticCode: "RECOVERED_APPLIED"), cancellationToken)
                .ConfigureAwait(false);
            var evidence = applicationVerifier.VerifyApplied(candidate, logicalPlan, current);
            var verified = resultVerifier.Verify(candidate, logicalPlan, evidence);
            var state = promoter.Promote(baseline, verified);
            await stateWriter.SaveAsync(state, cancellationToken).ConfigureAwait(false);
            await journal.CompleteAsync(entry.With(PublishTransactionStatus.Completed, diagnosticCode: "RECOVERED_APPLIED"), cancellationToken)
                .ConfigureAwait(false);
            return new PublishRecoveryResult(PublishRecoveryStatus.AppliedAndStateSaved, state);
        }

        if (status == RecoveryReconciliationStatus.NotApplied)
        {
            await journal.SaveAsync(entry.With(PublishTransactionStatus.ReplanRequired, diagnosticCode: "RECOVERED_NOT_APPLIED"), cancellationToken)
                .ConfigureAwait(false);
            return new PublishRecoveryResult(PublishRecoveryStatus.SafeReplan, null);
        }

        await journal.SaveAsync(entry.With(PublishTransactionStatus.Diverged, diagnosticCode: "RECOVERED_DIVERGED"), cancellationToken)
            .ConfigureAwait(false);
        return new PublishRecoveryResult(PublishRecoveryStatus.Diverged, null);
    }
}

/// <summary>Represents a crash-safe publish transaction result.</summary>
public sealed class PublishTransactionResult
{
    /// <summary>Initializes a transaction result.</summary>
    public PublishTransactionResult(
        PhysicalUpdateApplicationResult? applicationResult,
        PublishRecoveryResult recoveryResult,
        bool publishExecuted)
    {
        ApplicationResult = applicationResult;
        RecoveryResult = recoveryResult ?? throw new ArgumentNullException(nameof(recoveryResult));
        PublishExecuted = publishExecuted;
    }

    /// <summary>Gets the physical application result, when normal publish executed.</summary>
    public PhysicalUpdateApplicationResult? ApplicationResult { get; }

    /// <summary>Gets recovery result evaluated before normal publish.</summary>
    public PublishRecoveryResult RecoveryResult { get; }

    /// <summary>Gets whether normal publish ran after recovery.</summary>
    public bool PublishExecuted { get; }
}

/// <summary>Coordinates recovery, lock, planning, journaling, remote update, and verified-state persistence.</summary>
public sealed class PublishTransactionCoordinator
{
    private readonly IVerifiedPublishStateReader stateReader;
    private readonly IDiffEngine diffEngine;
    private readonly IPublishPlanApplicationVerifier planVerifier;
    private readonly PhysicalUpdateApplicationService applicationService;
    private readonly IPublishTransactionJournal journal;
    private readonly IDocumentPublishLockManager lockManager;
    private readonly IPublishRecoveryEngine recoveryEngine;

    /// <summary>Initializes a transaction coordinator.</summary>
    public PublishTransactionCoordinator(
        IVerifiedPublishStateReader stateReader,
        IDiffEngine diffEngine,
        IPublishPlanApplicationVerifier planVerifier,
        PhysicalUpdateApplicationService applicationService,
        IPublishTransactionJournal journal,
        IDocumentPublishLockManager lockManager,
        IPublishRecoveryEngine recoveryEngine)
    {
        this.stateReader = stateReader ?? throw new ArgumentNullException(nameof(stateReader));
        this.diffEngine = diffEngine ?? throw new ArgumentNullException(nameof(diffEngine));
        this.planVerifier = planVerifier ?? throw new ArgumentNullException(nameof(planVerifier));
        this.applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
        this.lockManager = lockManager ?? throw new ArgumentNullException(nameof(lockManager));
        this.recoveryEngine = recoveryEngine ?? throw new ArgumentNullException(nameof(recoveryEngine));
    }

    /// <summary>Executes one crash-safe publish transaction.</summary>
    public async Task<PublishTransactionResult> ExecuteAsync(
        PublishCandidate candidate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        var key = new PublishStateKey(candidate.Identity.PublicationId, candidate.Identity.DocumentId);
        var transactionId = Guid.NewGuid().ToString("N");
        await using var documentLock = await lockManager.AcquireAsync(key, transactionId, cancellationToken)
            .ConfigureAwait(false);
        var request = new PublishStateLoadRequest(key, candidate.Identity.GoogleDocumentId);
        var baseline = await stateReader.LoadAsync(request, cancellationToken).ConfigureAwait(false);
        var logicalPlan = diffEngine.CreatePlan(baseline, candidate);
        var pending = await journal.LoadAsync(key, candidate.Identity.GoogleDocumentId, cancellationToken)
            .ConfigureAwait(false);
        var recovery = await recoveryEngine.RecoverAsync(
            pending,
            baseline,
            candidate,
            logicalPlan,
            cancellationToken).ConfigureAwait(false);
        if (recovery.Status is PublishRecoveryStatus.AppliedAndStateSaved or PublishRecoveryStatus.Diverged)
        {
            return new PublishTransactionResult(null, recovery, publishExecuted: false);
        }

        baseline = await stateReader.LoadAsync(request, cancellationToken).ConfigureAwait(false);
        logicalPlan = diffEngine.CreatePlan(baseline, candidate);
        var entry = new PublishTransactionJournalEntry(
            key,
            candidate.Identity.GoogleDocumentId,
            documentLock.TransactionId,
            PublishTransactionStatus.Started,
            candidate.Fingerprint.Value,
            baseline?.Fingerprint.Value);
        await journal.SaveAsync(entry, cancellationToken).ConfigureAwait(false);
        var prepared = await planVerifier.PrepareAsync(candidate, baseline, cancellationToken)
            .ConfigureAwait(false);
        var physicalPlan = planVerifier.CreateDryRun(candidate, baseline, logicalPlan, prepared).PhysicalPlan
            ?? throw new PhysicalUpdateException(UpdateErrorCodes.PhysicalPlanInvalid, "A physical plan is required.");
        entry = entry.With(PublishTransactionStatus.Planned, physicalPlan.RequiredRevision.RevisionId);
        await journal.SaveAsync(entry, cancellationToken).ConfigureAwait(false);
        if (physicalPlan.IsPublishRequired)
        {
            entry = entry.With(PublishTransactionStatus.CommitUnknown);
            await journal.SaveAsync(entry, cancellationToken).ConfigureAwait(false);
        }

        var applicationResult = await applicationService.ExecuteAsync(
            baseline,
            candidate,
            logicalPlan,
            physicalPlan,
            cancellationToken).ConfigureAwait(false);
        var terminalStatus = applicationResult.ReplanRequired
            ? PublishTransactionStatus.ReplanRequired
            : applicationResult.RecoveryStatus == RecoveryReconciliationStatus.Diverged
                ? PublishTransactionStatus.Diverged
                : PublishTransactionStatus.Completed;
        entry = entry.With(terminalStatus, diagnosticCode: applicationResult.ExecutionResult.DiagnosticCode);
        if (terminalStatus == PublishTransactionStatus.Completed)
        {
            await journal.CompleteAsync(entry, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await journal.SaveAsync(entry, cancellationToken).ConfigureAwait(false);
        }

        return new PublishTransactionResult(applicationResult, recovery, publishExecuted: true);
    }
}
