using System.Text.RegularExpressions;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses the PoC subset of Markdown: headings, paragraphs, and unordered lists.</summary>
public sealed partial class SimpleMarkdownParser : IMarkdownParser
{
    /// <inheritdoc />
    public DocumentModel Parse(string markdown)
    {
        ArgumentNullException.ThrowIfNull(markdown);

        var normalized = markdown.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var lines = normalized.Split('\n');
        var blocks = new List<DocumentBlock>();
        var paragraph = new List<string>();

        void FlushParagraph()
        {
            if (paragraph.Count == 0)
            {
                return;
            }

            blocks.Add(CreateBlock(DocumentBlockKind.Paragraph, string.Join(" ", paragraph)));
            paragraph.Clear();
        }

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                FlushParagraph();
                continue;
            }

            var headingMatch = HeadingPattern().Match(line);
            if (headingMatch.Success)
            {
                FlushParagraph();
                blocks.Add(new DocumentBlock(
                    DocumentBlockKind.Heading,
                    [new InlineElement(InlineElementKind.Text, headingMatch.Groups[2].Value.Trim())],
                    headingMatch.Groups[1].Value.Length));
                continue;
            }

            var bulletMatch = BulletPattern().Match(line);
            if (bulletMatch.Success)
            {
                FlushParagraph();
                blocks.Add(CreateBlock(DocumentBlockKind.BulletListItem, bulletMatch.Groups[1].Value.Trim()));
                continue;
            }

            paragraph.Add(line.Trim());
        }

        FlushParagraph();
        return new DocumentModel(blocks);
    }

    private static DocumentBlock CreateBlock(DocumentBlockKind kind, string text) =>
        new(kind, [new InlineElement(InlineElementKind.Text, text)]);

    [GeneratedRegex("^(#{1,6})[ \\t]+(.+?)\\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex HeadingPattern();

    [GeneratedRegex("^[ \\t]{0,3}[-+*][ \\t]+(.+?)\\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex BulletPattern();
}
