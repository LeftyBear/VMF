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
}
