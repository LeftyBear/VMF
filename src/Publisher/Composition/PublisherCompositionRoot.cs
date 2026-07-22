using Vmf.Publisher.Application;
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
}
