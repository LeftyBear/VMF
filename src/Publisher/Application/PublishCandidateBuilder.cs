using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Builds a publish candidate from one canonical document model.</summary>
public interface IPublishCandidateBuilder
{
    /// <summary>Generates block identities and creates an atomic publish candidate.</summary>
    PublishCandidate Create(
        DocumentIdentity identity,
        DocumentModel document,
        PublishCandidateBuildOptions options);
}

/// <summary>Contains versioned and output-affecting inputs for candidate construction.</summary>
public sealed class PublishCandidateBuildOptions
{
    /// <summary>Initializes candidate build options.</summary>
    public PublishCandidateBuildOptions(
        string publisherVersion,
        string transformationSpecificationVersion,
        string publishStateSchemaVersion,
        IEnumerable<PublishFingerprintSetting> outputSettings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publisherVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(transformationSpecificationVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(publishStateSchemaVersion);
        ArgumentNullException.ThrowIfNull(outputSettings);

        var settings = outputSettings.ToArray();
        if (settings.Any(setting => setting is null))
        {
            throw new ArgumentException("Candidate settings must not contain null items.", nameof(outputSettings));
        }

        PublisherVersion = publisherVersion;
        TransformationSpecificationVersion = transformationSpecificationVersion;
        PublishStateSchemaVersion = publishStateSchemaVersion;
        OutputSettings = Array.AsReadOnly(settings);
    }

    /// <summary>Gets the Publisher implementation version.</summary>
    public string PublisherVersion { get; }

    /// <summary>Gets the Publisher transformation specification version.</summary>
    public string TransformationSpecificationVersion { get; }

    /// <summary>Gets the PublishState schema version.</summary>
    public string PublishStateSchemaVersion { get; }

    /// <summary>Gets output-affecting Publisher settings.</summary>
    public IReadOnlyList<PublishFingerprintSetting> OutputSettings { get; }
}

/// <summary>Coordinates canonical block identity and fingerprint generation.</summary>
public sealed class PublishCandidateBuilder : IPublishCandidateBuilder
{
    private readonly IBlockContentHashGenerator contentHashGenerator;
    private readonly IGeneratedBlockIdGenerator generatedIdGenerator;
    private readonly IPublishCandidateFactory candidateFactory;

    /// <summary>Initializes the candidate builder.</summary>
    public PublishCandidateBuilder(
        IBlockContentHashGenerator contentHashGenerator,
        IGeneratedBlockIdGenerator generatedIdGenerator,
        IPublishCandidateFactory candidateFactory)
    {
        this.contentHashGenerator = contentHashGenerator
            ?? throw new ArgumentNullException(nameof(contentHashGenerator));
        this.generatedIdGenerator = generatedIdGenerator
            ?? throw new ArgumentNullException(nameof(generatedIdGenerator));
        this.candidateFactory = candidateFactory
            ?? throw new ArgumentNullException(nameof(candidateFactory));
    }

    /// <inheritdoc />
    public PublishCandidate Create(
        DocumentIdentity identity,
        DocumentModel document,
        PublishCandidateBuildOptions options)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(options);

        EnsureUniqueExplicitIds(document.Blocks);
        var generatedIds = generatedIdGenerator.Generate(document);
        if (generatedIds.Count != document.Blocks.Count)
        {
            throw new InvalidOperationException(
                "Generated identifiers must align one-to-one with document blocks.");
        }

        var identities = new BlockIdentity[document.Blocks.Count];
        var uniqueGeneratedIds = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < document.Blocks.Count; index++)
        {
            var block = document.Blocks[index];
            var generatedId = generatedIds[index];
            if (block.ExplicitId is null && generatedId is null)
            {
                throw new InvalidOperationException(
                    $"Block {index} has neither an explicit nor generated identifier.");
            }

            if (block.ExplicitId is not null && generatedId is not null)
            {
                throw new InvalidOperationException(
                    $"Block {index} must not combine an explicit and generated identifier.");
            }

            if (generatedId is not null && !uniqueGeneratedIds.Add(generatedId))
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.BlockGeneratedIdDuplicate,
                    $"The canonical document contains a duplicate generated block identifier: {generatedId}");
            }

            identities[index] = new BlockIdentity(
                block.ExplicitId,
                generatedId,
                contentHashGenerator.Generate(block));
        }

        var fingerprintInput = new PublishFingerprintInput(
            identity.PublicationId,
            identity.DocumentId,
            document,
            identities,
            options.PublisherVersion,
            options.TransformationSpecificationVersion,
            options.PublishStateSchemaVersion,
            options.OutputSettings);
        return candidateFactory.Create(identity, fingerprintInput);
    }

    private static void EnsureUniqueExplicitIds(IReadOnlyList<DocumentBlock> blocks)
    {
        var values = new HashSet<string>(StringComparer.Ordinal);
        foreach (var block in blocks)
        {
            if (block.ExplicitId is not null && !values.Add(block.ExplicitId))
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.BlockExplicitIdDuplicate,
                    $"The canonical document contains a duplicate explicit block identifier: {block.ExplicitId}");
            }
        }
    }
}
