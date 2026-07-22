using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Creates candidates whose blocks and fingerprint share one canonical input.</summary>
public interface IPublishCandidateFactory
{
    /// <summary>Creates an unverified candidate from complete canonical input.</summary>
    PublishCandidate Create(DocumentIdentity identity, PublishFingerprintInput input);
}

/// <summary>Binds canonical input, its generated fingerprint, and candidate blocks atomically.</summary>
public sealed class PublishCandidateFactory : IPublishCandidateFactory
{
    private readonly IPublishFingerprintGenerator fingerprintGenerator;

    /// <summary>Initializes a candidate factory.</summary>
    public PublishCandidateFactory(IPublishFingerprintGenerator fingerprintGenerator)
    {
        this.fingerprintGenerator = fingerprintGenerator
            ?? throw new ArgumentNullException(nameof(fingerprintGenerator));
    }

    /// <inheritdoc />
    public PublishCandidate Create(DocumentIdentity identity, PublishFingerprintInput input)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(input);
        if (!string.Equals(identity.PublicationId, input.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(identity.DocumentId, input.DocumentId, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Candidate identity must match the canonical fingerprint input.",
                nameof(input));
        }

        return new PublishCandidate(
            identity,
            fingerprintGenerator.Generate(input),
            input.Blocks);
    }
}
