using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses contiguous Markdown quote lines at one normalized level.</summary>
public sealed class MarkdownQuoteParser
{
    /// <summary>The maximum supported quote level.</summary>
    public const int MaxQuoteLevel = 6;

    private readonly IMarkdownInlineParser inlineParser;

    /// <summary>Initializes a quote parser with the default inline parser.</summary>
    public MarkdownQuoteParser()
        : this(new MarkdownInlineParser())
    {
    }

    /// <summary>Initializes a quote parser.</summary>
    /// <param name="inlineParser">The inline parser used for quote content.</param>
    public MarkdownQuoteParser(IMarkdownInlineParser inlineParser)
    {
        this.inlineParser = inlineParser ?? throw new ArgumentNullException(nameof(inlineParser));
    }

    /// <summary>Attempts to parse a quote block at the specified line.</summary>
    public bool TryParse(
        IReadOnlyList<string> lines,
        int startIndex,
        out QuoteBlock? block,
        out int consumedLineCount)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (startIndex < 0 || startIndex >= lines.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        block = null;
        consumedLineCount = 0;
        if (!TryParseLine(lines[startIndex], out var level, out var text))
        {
            return false;
        }

        var quoteLines = new List<string> { text };
        var lineIndex = startIndex + 1;
        while (lineIndex < lines.Count &&
            TryParseLine(lines[lineIndex], out var nextLevel, out var nextText) &&
            nextLevel == level)
        {
            quoteLines.Add(nextText);
            lineIndex++;
        }

        var quoteText = string.Join("\n", quoteLines);
        block = new QuoteBlock(
            level,
            quoteText.Length == 0
                ? Array.Empty<InlineContent>()
                : inlineParser.Parse(quoteText));
        consumedLineCount = lineIndex - startIndex;
        return true;
    }

    private static bool TryParseLine(string line, out int level, out string text)
    {
        var markerIndex = 0;
        while (markerIndex < line.Length && line[markerIndex] == ' ')
        {
            markerIndex++;
        }

        if (markerIndex > 3 || markerIndex >= line.Length || line[markerIndex] != '>')
        {
            level = 0;
            text = string.Empty;
            return false;
        }

        var markerEnd = markerIndex;
        while (markerEnd < line.Length && line[markerEnd] == '>')
        {
            markerEnd++;
        }

        level = Math.Min(markerEnd - markerIndex, MaxQuoteLevel);
        if (markerEnd < line.Length && line[markerEnd] is ' ' or '\t')
        {
            markerEnd++;
        }

        text = line[markerEnd..];
        return true;
    }
}
