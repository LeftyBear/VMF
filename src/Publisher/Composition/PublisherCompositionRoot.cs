using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.Composition;

/// <summary>Composes target-neutral Publisher application services.</summary>
public static class PublisherCompositionRoot
{
    /// <summary>Creates the Phase 3-2A candidate identity pipeline.</summary>
    public static IPublishCandidateBuilder CreatePublishCandidateBuilder()
    {
        IPublishFingerprintGenerator fingerprintGenerator = new PublishFingerprintGenerator();
        IPublishCandidateFactory candidateFactory = new PublishCandidateFactory(fingerprintGenerator);
        return new PublishCandidateBuilder(
            new BlockContentHashGenerator(),
            new GeneratedBlockIdGenerator(),
            candidateFactory);
    }

    /// <summary>Creates the Phase 3-2B local verified-state store.</summary>
    public static IVerifiedPublishStateStore CreateVerifiedPublishStateStore(string rootDirectory)
    {
        var options = new VerifiedPublishStateStoreOptions(
            rootDirectory,
            PublishStateSchema.CurrentVersion,
            GeneratedBlockIdGenerator.AlgorithmVersion,
            BlockContentHashGenerator.AlgorithmVersion,
            PublishFingerprintGenerator.AlgorithmVersion);
        return new JsonVerifiedPublishStateStore(options);
    }

    /// <summary>Creates the target-neutral verified-state lifecycle coordinator.</summary>
    public static IVerifiedPublishLifecycle CreateVerifiedPublishLifecycle(
        IVerifiedPublishStateStore store,
        IPublishPlanApplicationVerifier applicationVerifier)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(applicationVerifier);
        return new VerifiedPublishLifecycle(
            store,
            store,
            new DiffEngine(),
            applicationVerifier,
            new PublishResultVerifier(),
            new VerifiedPublishStatePromoter());
    }

    /// <summary>Creates a Phase 3-2C physical-update lifecycle with dry-run support.</summary>
    public static IVerifiedPublishLifecycle CreatePhysicalUpdateLifecycle(
        IVerifiedPublishStateStore store,
        IManagedDocumentAdapter adapter)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(adapter);
        return CreateVerifiedPublishLifecycle(
            store,
            new PhysicalUpdateApplicationVerifier(adapter, new PhysicalUpdatePlanner()));
    }

    /// <summary>Creates a Phase 3-2E crash-safe transaction coordinator.</summary>
    public static PublishTransactionCoordinator CreatePublishTransactionCoordinator(
        IVerifiedPublishStateStore store,
        IManagedDocumentAdapter adapter,
        IPhysicalUpdateExecutor executor,
        string journalRootDirectory)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(executor);
        if (adapter is not IDocumentSnapshotReader snapshotReader)
        {
            throw new ArgumentException(
                "A crash-safe transaction coordinator requires a snapshot reader.",
                nameof(adapter));
        }

        var journal = new JsonPublishTransactionJournal(new PublishTransactionJournalOptions(journalRootDirectory));
        var diffEngine = new DiffEngine();
        var planner = new PhysicalUpdatePlanner();
        var verifier = new PhysicalUpdateApplicationVerifier(adapter, planner);
        var resultVerifier = new PublishResultVerifier();
        var promoter = new VerifiedPublishStatePromoter();
        var snapshotVerifier = new PhysicalUpdateApplicationSnapshotVerifier();
        var recoveryReconciler = new PhysicalUpdateRecoveryReconciler();
        return new PublishTransactionCoordinator(
            store,
            diffEngine,
            verifier,
            new PhysicalUpdateApplicationService(
                executor,
                snapshotReader,
                recoveryReconciler,
                snapshotVerifier,
                resultVerifier,
                promoter,
                store),
            journal,
            new FileDocumentPublishLockManager(new DocumentLockFileOptions(journalRootDirectory)),
            new PublishRecoveryEngine(
                snapshotReader,
                recoveryReconciler,
                snapshotVerifier,
                resultVerifier,
                promoter,
                store,
                journal));
    }
}
