using System.Security.Cryptography;
using System.Globalization;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Generates SHA-256 hashes from canonical block content.</summary>
public sealed class BlockContentHashGenerator : IBlockContentHashGenerator
{
    /// <summary>The current content-hash algorithm version.</summary>
    public const string AlgorithmVersion = "1";

    /// <summary>The current content-hash value prefix.</summary>
    public const string ValuePrefix = "ch-v1:sha256:";

    private readonly string algorithmVersion;
    private readonly string valuePrefix;

    /// <summary>Initializes the current content-hash algorithm.</summary>
    public BlockContentHashGenerator()
        : this(AlgorithmVersion)
    {
    }

    internal BlockContentHashGenerator(string algorithmVersion)
    {
        this.algorithmVersion = ValidateVersion(algorithmVersion);
        valuePrefix = $"ch-v{this.algorithmVersion}:sha256:";
    }

    /// <inheritdoc />
    public string Generate(DocumentBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);

        using var writer = new CanonicalValueWriter();
        writer.Write("format", "vmf-publisher-block-content-canonical");
        writer.Write("contentHashAlgorithmVersion", algorithmVersion);
        writer.Write("hashAlgorithm", "sha-256");
        CanonicalBlockSerializer.WriteBlockContent(writer, block);

        var hash = SHA256.HashData(writer.ToArray());
        return valuePrefix + Convert.ToHexString(hash).ToLowerInvariant();
    }

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
}
