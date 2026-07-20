using System.Text.RegularExpressions;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses headings, paragraphs, and ordered, unordered, or mixed lists.</summary>
public sealed partial class SimpleMarkdownParser : IMarkdownParser
{
    private readonly MarkdownListParser _listParser;
    private readonly MarkdownTableParser tableParser;
    private readonly MarkdownCodeBlockParser codeBlockParser;
    private readonly MarkdownQuoteParser quoteParser;
    private readonly IMarkdownInlineParser inlineParser;

    /// <summary>Initializes a parser with the default block parsers.</summary>
    public SimpleMarkdownParser()
        : this(new MarkdownInlineParser())
    {
    }

    /// <summary>Initializes a parser with an explicitly registered inline parser.</summary>
    /// <param name="inlineParser">The Markdown inline parser.</param>
    public SimpleMarkdownParser(IMarkdownInlineParser inlineParser)
        : this(
            new MarkdownCodeBlockParser(),
            new MarkdownListParser(new MarkdownListParserOptions(), inlineParser),
            new MarkdownTableParser(inlineParser),
            new MarkdownQuoteParser(inlineParser),
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
        IMarkdownInlineParser inlineParser)
        : this(listParser, new MarkdownTableParser(inlineParser), inlineParser)
    {
    }

    /// <summary>Initializes a parser with explicitly registered block and inline parsers.</summary>
    /// <param name="listParser">The Markdown list parser.</param>
    /// <param name="tableParser">The Markdown table parser.</param>
    /// <param name="inlineParser">The Markdown inline parser.</param>
    public SimpleMarkdownParser(
        MarkdownListParser listParser,
        MarkdownTableParser tableParser,
        IMarkdownInlineParser inlineParser)
        : this(
            new MarkdownCodeBlockParser(),
            listParser,
            tableParser,
            new MarkdownQuoteParser(inlineParser),
            inlineParser)
    {
    }

    /// <summary>Initializes a parser with explicitly registered block and inline parsers.</summary>
    /// <param name="codeBlockParser">The fenced code-block parser.</param>
    /// <param name="listParser">The Markdown list parser.</param>
    /// <param name="tableParser">The Markdown table parser.</param>
    /// <param name="quoteParser">The Markdown quote parser.</param>
    /// <param name="inlineParser">The Markdown inline parser.</param>
    public SimpleMarkdownParser(
        MarkdownCodeBlockParser codeBlockParser,
        MarkdownListParser listParser,
        MarkdownTableParser tableParser,
        MarkdownQuoteParser quoteParser,
        IMarkdownInlineParser inlineParser)
    {
        this.codeBlockParser = codeBlockParser
            ?? throw new ArgumentNullException(nameof(codeBlockParser));
        _listParser = listParser ?? throw new ArgumentNullException(nameof(listParser));
        this.tableParser = tableParser ?? throw new ArgumentNullException(nameof(tableParser));
        this.quoteParser = quoteParser ?? throw new ArgumentNullException(nameof(quoteParser));
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

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line))
            {
                FlushParagraph();
                FlushList();
                continue;
            }

            if (codeBlockParser.TryParse(lines, lineIndex, out var code, out var codeLines))
            {
                FlushParagraph();
                FlushList();
                blocks.Add(new DocumentBlock(
                    code ?? throw new InvalidOperationException("A parsed code block requires code content.")));
                lineIndex += codeLines - 1;
                continue;
            }

            if (tableParser.TryParse(lines, lineIndex, out var table, out var consumedLines))
            {
                FlushParagraph();
                FlushList();
                blocks.Add(new DocumentBlock(
                    table ?? throw new InvalidOperationException("A parsed table requires table content.")));
                lineIndex += consumedLines - 1;
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

            var listItem = _listParser.ParseLine(
                line,
                listItems.Count == 0 ? null : listItems[^1].Depth);
            if (listItem is not null)
            {
                FlushParagraph();
                listItems.Add(listItem);
                continue;
            }

            if (quoteParser.TryParse(lines, lineIndex, out var quote, out var quoteLines))
            {
                FlushParagraph();
                FlushList();
                blocks.Add(new DocumentBlock(
                    quote ?? throw new InvalidOperationException("A parsed quote requires quote content.")));
                lineIndex += quoteLines - 1;
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
