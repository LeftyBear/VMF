using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Creates candidates whose blocks and fingerprint share one canonical input.</summary>
public interface IPublishCandidateFactory
{
    /// <summary>Gets the fingerprint algorithm version used by this factory.</summary>
    string FingerprintAlgorithmVersion { get; }

    /// <summary>Creates an unverified candidate from complete canonical input.</summary>
    PublishCandidate Create(
        DocumentIdentity identity,
        PublishFingerprintInput input,
        PublishStateVersions versions);
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
    public string FingerprintAlgorithmVersion => fingerprintGenerator.AlgorithmVersion;

    /// <inheritdoc />
    public PublishCandidate Create(
        DocumentIdentity identity,
        PublishFingerprintInput input,
        PublishStateVersions versions)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(versions);
        if (identity.State != DocumentState.Active)
        {
            throw new ArgumentException("A publish candidate must target the active state.", nameof(identity));
        }

        if (!string.Equals(identity.PublicationId, input.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(identity.DocumentId, input.DocumentId, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Candidate identity must match the canonical fingerprint input.",
                nameof(input));
        }

        if (!string.Equals(versions.SchemaVersion, input.PublishStateSchemaVersion, StringComparison.Ordinal) ||
            !string.Equals(
                versions.TransformationSpecificationVersion,
                input.TransformationSpecificationVersion,
                StringComparison.Ordinal) ||
            !string.Equals(versions.PublisherVersion, input.PublisherVersion, StringComparison.Ordinal) ||
            !string.Equals(
                versions.FingerprintAlgorithmVersion,
                FingerprintAlgorithmVersion,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Candidate versions must match the canonical fingerprint pipeline.",
                nameof(versions));
        }

        return new PublishCandidate(
            identity,
            versions,
            fingerprintGenerator.Generate(input),
            input.Blocks);
    }
}
