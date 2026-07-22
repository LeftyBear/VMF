using System.Security.Cryptography;
using System.Globalization;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Generates deterministic structural-content fallback block identifiers.</summary>
public sealed class GeneratedBlockIdGenerator : IGeneratedBlockIdGenerator
{
    /// <summary>The current generated-identifier algorithm version.</summary>
    public const string AlgorithmVersion = "1";

    /// <summary>The current generated-identifier value prefix.</summary>
    public const string ValuePrefix = "gid-v1:sha256:";

    private readonly string algorithmVersion;
    private readonly string valuePrefix;

    /// <summary>Initializes the current generated-identifier algorithm.</summary>
    public GeneratedBlockIdGenerator()
        : this(AlgorithmVersion)
    {
    }

    internal GeneratedBlockIdGenerator(string algorithmVersion)
    {
        this.algorithmVersion = ValidateVersion(algorithmVersion);
        valuePrefix = $"gid-v{this.algorithmVersion}:sha256:";
    }

    /// <inheritdoc />
    string IGeneratedBlockIdGenerator.AlgorithmVersion => algorithmVersion;

    /// <inheritdoc />
    public IReadOnlyList<string?> Generate(DocumentModel document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var result = new string?[document.Blocks.Count];
        var headingAnchors = new string?[6];
        var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);
        var emitted = new HashSet<string>(StringComparer.Ordinal);

        for (var index = 0; index < document.Blocks.Count; index++)
        {
            var block = document.Blocks[index];
            if (block.Kind == DocumentBlockKind.Heading)
            {
                Array.Clear(headingAnchors, block.Level - 1, headingAnchors.Length - block.Level + 1);
            }

            var parents = ParentAnchors(headingAnchors);
            string anchor;
            if (block.ExplicitId is not null)
            {
                anchor = "explicit:" + block.ExplicitId;
            }
            else
            {
                var featureDigest = FeatureDigest(block);
                var equivalenceKey = EquivalenceKey(parents, block.Kind, featureDigest);
                occurrences.TryGetValue(equivalenceKey, out var occurrence);
                occurrences[equivalenceKey] = occurrence + 1;
                var generatedId = GenerateId(parents, block.Kind, featureDigest, occurrence);
                if (!emitted.Add(generatedId))
                {
                    throw new PublishPipelineException(
                        PublishErrorCodes.BlockGeneratedIdDuplicate,
                        "The generated block identifier algorithm produced a duplicate identifier.");
                }

                result[index] = generatedId;
                anchor = "generated:" + generatedId;
            }

            if (block.Kind == DocumentBlockKind.Heading)
            {
                headingAnchors[block.Level - 1] = anchor;
            }
        }

        return Array.AsReadOnly(result);
    }

    private string FeatureDigest(DocumentBlock block)
    {
        using var writer = new CanonicalValueWriter();
        writer.Write("format", "vmf-publisher-generated-id-feature-canonical");
        writer.Write("generatedIdAlgorithmVersion", algorithmVersion);
        writer.Write("hashAlgorithm", "sha-256");
        CanonicalBlockSerializer.WriteBlockContent(writer, block);
        return LowerSha256(writer.ToArray());
    }

    private string EquivalenceKey(
        IReadOnlyList<ParentAnchor> parents,
        DocumentBlockKind kind,
        string featureDigest)
    {
        using var writer = new CanonicalValueWriter();
        WriteIdentityInput(writer, parents, kind, featureDigest);
        return LowerSha256(writer.ToArray());
    }

    private string GenerateId(
        IReadOnlyList<ParentAnchor> parents,
        DocumentBlockKind kind,
        string featureDigest,
        int occurrence)
    {
        using var writer = new CanonicalValueWriter();
        writer.Write("format", "vmf-publisher-generated-block-id-canonical");
        writer.Write("generatedIdAlgorithmVersion", algorithmVersion);
        writer.Write("hashAlgorithm", "sha-256");
        WriteIdentityInput(writer, parents, kind, featureDigest);
        writer.WriteInteger("equivalentBlockOccurrence", occurrence);
        return valuePrefix + LowerSha256(writer.ToArray());
    }

    private static void WriteIdentityInput(
        CanonicalValueWriter writer,
        IReadOnlyList<ParentAnchor> parents,
        DocumentBlockKind kind,
        string featureDigest)
    {
        writer.WriteInteger("parentHeadingCount", parents.Count);
        foreach (var parent in parents)
        {
            writer.WriteInteger("parentHeadingLevel", parent.Level);
            writer.Write("parentHeadingAnchor", parent.Anchor);
        }

        writer.Write("blockKind", CanonicalBlockSerializer.BlockKind(kind));
        writer.Write("blockIdentityFeatureDigest", featureDigest);
    }

    private static IReadOnlyList<ParentAnchor> ParentAnchors(string?[] headingAnchors)
    {
        var result = new List<ParentAnchor>();
        for (var index = 0; index < headingAnchors.Length; index++)
        {
            if (headingAnchors[index] is not null)
            {
                result.Add(new ParentAnchor(index + 1, headingAnchors[index]!));
            }
        }

        return result;
    }

    private static string LowerSha256(byte[] input) =>
        Convert.ToHexString(SHA256.HashData(input)).ToLowerInvariant();

    private static string ValidateVersion(string value)
    {
        if (!int.TryParse(
                value,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var number) ||
            number < 1 ||
            number.ToString(CultureInfo.InvariantCulture) != value)
        {
            throw new ArgumentException("An algorithm version must be a positive canonical integer.", nameof(value));
        }

        return value;
    }

    private readonly record struct ParentAnchor(int Level, string Anchor);
}
