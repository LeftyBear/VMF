using System.Globalization;
using System.Security.Cryptography;
using System.Text;
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

        using var writer = new CanonicalWriter();
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
        CanonicalWriter writer,
        int index,
        DocumentBlock block,
        BlockIdentity identity)
    {
        writer.WriteInteger("blockIndex", index);
        writer.Write("blockExplicitId", identity.ExplicitId);
        writer.Write("blockGeneratedId", identity.GeneratedId);
        writer.Write("blockContentHash", identity.ContentHash);
        writer.Write("blockKind", BlockKind(block.Kind));
        writer.WriteInteger("blockLevel", block.Level);

        switch (block.Kind)
        {
            case DocumentBlockKind.Heading:
            case DocumentBlockKind.Paragraph:
            case DocumentBlockKind.BulletListItem:
                WriteInlineCollection(writer, block.Content);
                break;
            case DocumentBlockKind.List:
                WriteList(writer, block.List ?? throw MissingContent(block.Kind));
                break;
            case DocumentBlockKind.Table:
                WriteTable(writer, block.Table ?? throw MissingContent(block.Kind));
                break;
            case DocumentBlockKind.Code:
                WriteCode(writer, block.Code ?? throw MissingContent(block.Kind));
                break;
            case DocumentBlockKind.Quote:
                WriteQuote(writer, block.Quote ?? throw MissingContent(block.Kind));
                break;
            case DocumentBlockKind.Image:
                WriteImage(writer, block.Image ?? throw MissingContent(block.Kind));
                break;
            default:
                throw new InvalidOperationException($"Unsupported document block kind: {block.Kind}");
        }
    }

    private static void WriteList(CanonicalWriter writer, ListBlock list)
    {
        writer.WriteInteger("listItemCount", list.Items.Count);
        foreach (var item in list.Items)
        {
            writer.Write("listItemKind", item.Kind == ListKind.Unordered ? "unordered" : "ordered");
            writer.WriteInteger("listItemDepth", item.Depth);
            WriteInlineCollection(writer, item.Content);
        }
    }

    private static void WriteTable(CanonicalWriter writer, TableBlock table)
    {
        writer.WriteInteger("tableColumnCount", table.Columns.Count);
        foreach (var column in table.Columns)
        {
            writer.Write("tableColumnAlignment", column.Alignment switch
            {
                TableAlignment.Left => "left",
                TableAlignment.Center => "center",
                TableAlignment.Right => "right",
                _ => throw new InvalidOperationException(
                    $"Unsupported table alignment: {column.Alignment}"),
            });
        }

        writer.WriteInteger("tableRowCount", table.AllRows.Count);
        foreach (var row in table.AllRows)
        {
            writer.WriteInteger("tableCellCount", row.Cells.Count);
            foreach (var cell in row.Cells)
            {
                WriteInlineCollection(writer, cell.Content);
            }
        }
    }

    private static void WriteCode(CanonicalWriter writer, CodeBlock code)
    {
        writer.Write("codeLanguage", code.Language);
        writer.Write("codeText", code.Text);
    }

    private static void WriteQuote(CanonicalWriter writer, QuoteBlock quote)
    {
        writer.WriteInteger("quoteLevel", quote.Level);
        WriteInlineCollection(writer, quote.Content);
    }

    private static void WriteImage(CanonicalWriter writer, ImageBlock image)
    {
        writer.Write("imageAltText", image.AltText);
        writer.Write("imageSourceKind", image.Source switch
        {
            LocalImageSource => "local",
            RemoteImageSource => "remote",
            _ => throw new InvalidOperationException(
                $"Unsupported image source: {image.Source.GetType().Name}"),
        });
        writer.Write("imageSourceValue", image.Source switch
        {
            RemoteImageSource remote => remote.Uri.AbsoluteUri,
            _ => image.Source.Value,
        });
        writer.WriteBoolean("imageHasSize", image.Size is not null);
        if (image.Size is not null)
        {
            writer.WriteDouble("imageWidthPoints", image.Size.WidthPoints);
            writer.WriteDouble("imageHeightPoints", image.Size.HeightPoints);
        }
    }

    private static void WriteInlineCollection(
        CanonicalWriter writer,
        IReadOnlyList<InlineContent> content)
    {
        writer.WriteInteger("inlineCount", content.Count);
        foreach (var item in content)
        {
            switch (item)
            {
                case TextInline text:
                    writer.Write("inlineKind", "text");
                    writer.Write("inlineText", text.Text);
                    break;
                case CodeInline code:
                    writer.Write("inlineKind", "code");
                    writer.Write("inlineText", code.Text);
                    break;
                case BoldInline bold:
                    writer.Write("inlineKind", "bold");
                    WriteInlineCollection(writer, bold.Content);
                    break;
                case ItalicInline italic:
                    writer.Write("inlineKind", "italic");
                    WriteInlineCollection(writer, italic.Content);
                    break;
                case LinkInline link:
                    writer.Write("inlineKind", "link");
                    writer.Write("inlineLinkUrl", link.Url.AbsoluteUri);
                    WriteInlineCollection(writer, link.Content);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported inline content: {item.GetType().Name}");
            }
        }
    }

    private static string BlockKind(DocumentBlockKind kind) => kind switch
    {
        DocumentBlockKind.Heading => "heading",
        DocumentBlockKind.Paragraph => "paragraph",
        DocumentBlockKind.BulletListItem => "bullet-list-item",
        DocumentBlockKind.List => "list",
        DocumentBlockKind.Table => "table",
        DocumentBlockKind.Code => "code",
        DocumentBlockKind.Quote => "quote",
        DocumentBlockKind.Image => "image",
        _ => throw new InvalidOperationException($"Unsupported document block kind: {kind}"),
    };

    private static InvalidOperationException MissingContent(DocumentBlockKind kind) =>
        new($"Document block {kind} is missing its strongly typed content.");

    private sealed class CanonicalWriter : IDisposable
    {
        private static readonly byte[] Colon = [(byte)':'];
        private static readonly byte[] NewLine = [(byte)'\n'];
        private readonly MemoryStream stream = new();

        internal void Write(string name, string? value)
        {
            WriteUtf8(name);
            stream.Write(Colon);
            if (value is null)
            {
                WriteUtf8("-1");
                stream.Write(Colon);
                stream.Write(NewLine);
                return;
            }

            value = NormalizeLineEndings(value);
            var valueBytes = Encoding.UTF8.GetBytes(value);
            WriteUtf8(valueBytes.Length.ToString(CultureInfo.InvariantCulture));
            stream.Write(Colon);
            stream.Write(valueBytes);
            stream.Write(NewLine);
        }

        internal void WriteBoolean(string name, bool value) =>
            Write(name, value ? "true" : "false");

        internal void WriteDouble(string name, double value) =>
            Write(name, value.ToString("R", CultureInfo.InvariantCulture));

        internal void WriteInteger(string name, int value) =>
            Write(name, value.ToString(CultureInfo.InvariantCulture));

        internal byte[] ToArray() => stream.ToArray();

        public void Dispose() => stream.Dispose();

        private void WriteUtf8(string value) => stream.Write(Encoding.UTF8.GetBytes(value));

        private static string NormalizeLineEndings(string value) =>
            value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
    }
}
