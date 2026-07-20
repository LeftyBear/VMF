using System.Text.RegularExpressions;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses headings, paragraphs, and ordered, unordered, or mixed lists.</summary>
public sealed partial class SimpleMarkdownParser : IMarkdownParser
{
    private readonly MarkdownListParser _listParser;
    private readonly MarkdownInlineParser inlineParser;

    /// <summary>Initializes a parser with the default block parsers.</summary>
    public SimpleMarkdownParser()
        : this(new MarkdownInlineParser())
    {
    }

    /// <summary>Initializes a parser with an explicitly registered inline parser.</summary>
    /// <param name="inlineParser">The Markdown inline parser.</param>
    public SimpleMarkdownParser(MarkdownInlineParser inlineParser)
        : this(
            new MarkdownListParser(new MarkdownListParserOptions(), inlineParser),
            inlineParser)
    {
    }

    /// <summary>Initializes a parser with an explicitly registered list parser.</summary>
    /// <param name="listParser">The Markdown list parser.</param>
    public SimpleMarkdownParser(MarkdownListParser listParser)
        : this(listParser, new MarkdownInlineParser())
    {
    }

    /// <summary>Initializes a parser with explicitly registered block and inline parsers.</summary>
    /// <param name="listParser">The Markdown list parser.</param>
    /// <param name="inlineParser">The Markdown inline parser.</param>
    public SimpleMarkdownParser(
        MarkdownListParser listParser,
        MarkdownInlineParser inlineParser)
    {
        _listParser = listParser ?? throw new ArgumentNullException(nameof(listParser));
        this.inlineParser = inlineParser ?? throw new ArgumentNullException(nameof(inlineParser));
    }

    /// <inheritdoc />
    public DocumentModel Parse(string markdown)
    {
        ArgumentNullException.ThrowIfNull(markdown);

        var normalized = markdown.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var lines = normalized.Split('\n');
        var blocks = new List<DocumentBlock>();
        var paragraph = new List<string>();
        var listItems = new List<ListItem>();

        void FlushParagraph()
        {
            if (paragraph.Count == 0)
            {
                return;
            }

            blocks.Add(new DocumentBlock(
                new ParagraphBlock(inlineParser.Parse(string.Join(" ", paragraph)))));
            paragraph.Clear();
        }

        void FlushList()
        {
            if (listItems.Count == 0)
            {
                return;
            }

            blocks.Add(new DocumentBlock(new ListBlock(listItems)));
            listItems.Clear();
        }

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                FlushParagraph();
                FlushList();
                continue;
            }

            var listItem = _listParser.ParseLine(
                line,
                listItems.Count == 0 ? null : listItems[^1].Depth);
            if (listItem is not null)
            {
                FlushParagraph();
                listItems.Add(listItem);
                continue;
            }

            var headingMatch = HeadingPattern().Match(line);
            if (headingMatch.Success)
            {
                FlushParagraph();
                FlushList();
                blocks.Add(new DocumentBlock(new HeadingBlock(
                    headingMatch.Groups[1].Value.Length,
                    inlineParser.Parse(headingMatch.Groups[2].Value.Trim()))));
                continue;
            }

            FlushList();
            paragraph.Add(line.Trim());
        }

        FlushParagraph();
        FlushList();
        return new DocumentModel(blocks);
    }

    [GeneratedRegex("^(#{1,6})[ \\t]+(.+?)\\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex HeadingPattern();
}
