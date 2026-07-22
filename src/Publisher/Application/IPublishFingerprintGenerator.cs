using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Generates a versioned fingerprint from canonical publish input.</summary>
public interface IPublishFingerprintGenerator
{
    /// <summary>Generates a canonical publish fingerprint.</summary>
    /// <param name="input">The complete output-affecting publish input.</param>
    /// <returns>The versioned fingerprint.</returns>
    PublishFingerprint Generate(PublishFingerprintInput input);
}

/// <summary>Defines current output-affecting settings required by canonical input.</summary>
public static class PublishFingerprintSettingNames
{
    /// <summary>Maximum Markdown inline nesting depth.</summary>
    public const string MarkdownInlineMaxDepth = "markdown.inline.maxDepth";

    /// <summary>Markdown list indentation width.</summary>
    public const string MarkdownListIndentSize = "markdown.list.indentSize";

    /// <summary>Maximum Markdown list nesting depth.</summary>
    public const string MarkdownListMaxDepth = "markdown.list.maxDepth";

    /// <summary>Whether rendered images may be enlarged.</summary>
    public const string PublisherAllowImageUpscale = "publisher.allowImageUpscale";

    /// <summary>Maximum rendered image width in points.</summary>
    public const string PublisherImageMaxWidthPoints = "publisher.imageMaxWidthPoints";

    /// <summary>Gets the required setting names for fingerprint algorithm version 1.</summary>
    public static IReadOnlyList<string> Required { get; } = Array.AsReadOnly(new[]
    {
        MarkdownInlineMaxDepth,
        MarkdownListIndentSize,
        MarkdownListMaxDepth,
        PublisherAllowImageUpscale,
        PublisherImageMaxWidthPoints,
    });
}

/// <summary>Represents one output-affecting Publisher setting.</summary>
public sealed class PublishFingerprintSetting
{
    /// <summary>Initializes a canonical setting.</summary>
    /// <param name="name">The stable setting name.</param>
    /// <param name="value">The invariant value; <see langword="null"/> is distinct from empty.</param>
    public PublishFingerprintSetting(string name, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Value = value;
    }

    /// <summary>Gets the stable setting name.</summary>
    public string Name { get; }

    /// <summary>Gets the invariant setting value.</summary>
    public string? Value { get; }
}

/// <summary>Contains the complete canonical input required to generate a publish fingerprint.</summary>
public sealed class PublishFingerprintInput
{
    /// <summary>Initializes canonical publish input.</summary>
    public PublishFingerprintInput(
        string publicationId,
        string documentId,
        DocumentModel document,
        IEnumerable<BlockIdentity> blocks,
        string publisherVersion,
        string transformationSpecificationVersion,
        string publishStateSchemaVersion,
        IEnumerable<PublishFingerprintSetting> outputSettings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publicationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        Document = document ?? throw new ArgumentNullException(nameof(document));
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentException.ThrowIfNullOrWhiteSpace(publisherVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(transformationSpecificationVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(publishStateSchemaVersion);
        ArgumentNullException.ThrowIfNull(outputSettings);

        var blockItems = blocks.ToArray();
        if (blockItems.Any(block => block is null))
        {
            throw new ArgumentException("Fingerprint blocks must not contain null items.", nameof(blocks));
        }

        if (blockItems.Length != document.Blocks.Count)
        {
            throw new ArgumentException(
                "Fingerprint block identities must align one-to-one with document blocks.",
                nameof(blocks));
        }

        var settingItems = outputSettings.ToArray();
        if (settingItems.Any(setting => setting is null))
        {
            throw new ArgumentException(
                "Fingerprint settings must not contain null items.",
                nameof(outputSettings));
        }

        var settingNames = new HashSet<string>(StringComparer.Ordinal);
        if (settingItems.Any(setting => !settingNames.Add(setting.Name)))
        {
            throw new ArgumentException(
                "Fingerprint setting names must be unique using ordinal comparison.",
                nameof(outputSettings));
        }

        var missingSetting = PublishFingerprintSettingNames.Required
            .FirstOrDefault(name => !settingNames.Contains(name));
        if (missingSetting is not null)
        {
            throw new ArgumentException(
                $"Fingerprint input is missing required output setting: {missingSetting}",
                nameof(outputSettings));
        }

        PublicationId = publicationId;
        DocumentId = documentId;
        Blocks = Array.AsReadOnly(blockItems);
        PublisherVersion = publisherVersion;
        TransformationSpecificationVersion = transformationSpecificationVersion;
        PublishStateSchemaVersion = publishStateSchemaVersion;
        OutputSettings = Array.AsReadOnly(settingItems);
    }

    /// <summary>Gets the persistent publication identifier.</summary>
    public string PublicationId { get; }

    /// <summary>Gets the publication-local document identifier.</summary>
    public string DocumentId { get; }

    /// <summary>Gets the ordered canonical document model.</summary>
    public DocumentModel Document { get; }

    /// <summary>Gets block identities aligned with the document block order.</summary>
    public IReadOnlyList<BlockIdentity> Blocks { get; }

    /// <summary>Gets the Publisher implementation version.</summary>
    public string PublisherVersion { get; }

    /// <summary>Gets the Publisher transformation specification version.</summary>
    public string TransformationSpecificationVersion { get; }

    /// <summary>Gets the PublishState schema version.</summary>
    public string PublishStateSchemaVersion { get; }

    /// <summary>Gets all output-affecting Publisher settings.</summary>
    public IReadOnlyList<PublishFingerprintSetting> OutputSettings { get; }
}
