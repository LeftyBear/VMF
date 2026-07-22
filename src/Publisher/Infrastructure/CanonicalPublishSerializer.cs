using System.Globalization;
using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Writes the shared canonical record format used by Publisher hashes.</summary>
internal sealed class CanonicalValueWriter : IDisposable
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

/// <summary>Serializes canonical document-block content without identity or position.</summary>
internal static class CanonicalBlockSerializer
{
    internal static void WriteBlockContent(CanonicalValueWriter writer, DocumentBlock block)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(block);

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

    internal static string BlockKind(DocumentBlockKind kind) => kind switch
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

    private static void WriteList(CanonicalValueWriter writer, ListBlock list)
    {
        writer.WriteInteger("listItemCount", list.Items.Count);
        foreach (var item in list.Items)
        {
            writer.Write("listItemKind", item.Kind == ListKind.Unordered ? "unordered" : "ordered");
            writer.WriteInteger("listItemDepth", item.Depth);
            WriteInlineCollection(writer, item.Content);
        }
    }

    private static void WriteTable(CanonicalValueWriter writer, TableBlock table)
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

    private static void WriteCode(CanonicalValueWriter writer, CodeBlock code)
    {
        writer.Write("codeLanguage", code.Language);
        writer.Write("codeText", code.Text);
    }

    private static void WriteQuote(CanonicalValueWriter writer, QuoteBlock quote)
    {
        writer.WriteInteger("quoteLevel", quote.Level);
        WriteInlineCollection(writer, quote.Content);
    }

    private static void WriteImage(CanonicalValueWriter writer, ImageBlock image)
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
        CanonicalValueWriter writer,
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

    private static InvalidOperationException MissingContent(DocumentBlockKind kind) =>
        new($"Document block {kind} is missing its strongly typed content.");
}
