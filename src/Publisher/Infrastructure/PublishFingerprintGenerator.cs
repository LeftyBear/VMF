using System.Security.Cryptography;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Generates SHA-256 fingerprints from a versioned canonical byte stream.</summary>
public sealed class PublishFingerprintGenerator : IPublishFingerprintGenerator
{
    /// <summary>The canonical serialization and fingerprint algorithm version.</summary>
    public const string AlgorithmVersion = "1";

    /// <summary>The normalized fingerprint value prefix.</summary>
    public const string ValuePrefix = PublishFingerprint.VersionPrefix;

    /// <inheritdoc />
    public PublishFingerprint Generate(PublishFingerprintInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        using var writer = new CanonicalValueWriter();
        writer.Write("format", "vmf-publisher-fingerprint-canonical");
        writer.Write("fingerprintAlgorithmVersion", AlgorithmVersion);
        writer.Write("hashAlgorithm", "sha-256");
        writer.Write("publisherVersion", input.PublisherVersion);
        writer.Write("transformationSpecificationVersion", input.TransformationSpecificationVersion);
        writer.Write("publishStateSchemaVersion", input.PublishStateSchemaVersion);
        writer.Write("publicationId", input.PublicationId);
        writer.Write("documentId", input.DocumentId);

        var settings = input.OutputSettings
            .OrderBy(setting => setting.Name, StringComparer.Ordinal)
            .ToArray();
        writer.WriteInteger("outputSettingCount", settings.Length);
        foreach (var setting in settings)
        {
            writer.Write("outputSettingName", setting.Name);
            writer.Write("outputSettingValue", setting.Value);
        }

        writer.WriteInteger("documentBlockCount", input.Document.Blocks.Count);
        for (var index = 0; index < input.Document.Blocks.Count; index++)
        {
            WriteBlock(writer, index, input.Document.Blocks[index], input.Blocks[index]);
        }

        var hash = SHA256.HashData(writer.ToArray());
        return new PublishFingerprint(ValuePrefix + Convert.ToHexString(hash).ToLowerInvariant());
    }

    private static void WriteBlock(
        CanonicalValueWriter writer,
        int index,
        DocumentBlock block,
        BlockIdentity identity)
    {
        writer.WriteInteger("blockIndex", index);
        writer.Write("blockExplicitId", identity.ExplicitId);
        writer.Write("blockGeneratedId", identity.GeneratedId);
        writer.Write("blockContentHash", identity.ContentHash);
        CanonicalBlockSerializer.WriteBlockContent(writer, block);
    }
}
