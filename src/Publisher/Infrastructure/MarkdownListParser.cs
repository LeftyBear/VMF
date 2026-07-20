using System.Text.RegularExpressions;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses and normalizes ordered and unordered Markdown list items.</summary>
public sealed partial class MarkdownListParser
{
    private readonly int _listIndentSize;
    private readonly int _maxListDepth;

    /// <summary>Initializes a parser with the default list settings.</summary>
    public MarkdownListParser()
        : this(new MarkdownListParserOptions())
    {
    }

    /// <summary>Initializes a parser with explicit list settings.</summary>
    /// <param name="options">The list parser settings.</param>
    public MarkdownListParser(MarkdownListParserOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();
        _listIndentSize = options.ListIndentSize;
        _maxListDepth = options.MaxListDepth;
    }

    /// <summary>Parses one Markdown line when it is a list item.</summary>
    /// <param name="line">The source line.</param>
    /// <param name="previousDepth">The previous normalized depth in the current list block.</param>
    /// <returns>The parsed item, or <see langword="null"/> when the line is not a list item.</returns>
    public ListItem? ParseLine(string line, int? previousDepth)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (previousDepth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(previousDepth));
        }

        var match = ListItemPattern().Match(line);
        if (!match.Success)
        {
            return null;
        }

        var requestedDepth = GetIndentWidth(match.Groups["indent"].Value) / _listIndentSize;
        var maximumDepth = _maxListDepth - 1;
        var normalizedDepth = previousDepth is null
            ? 0
            : Math.Min(requestedDepth, previousDepth.Value + 1);
        normalizedDepth = Math.Min(normalizedDepth, maximumDepth);

        var kind = match.Groups["ordered"].Success ? ListKind.Ordered : ListKind.Unordered;
        return new ListItem(
            kind,
            [new InlineElement(InlineElementKind.Text, match.Groups["content"].Value.Trim())],
            normalizedDepth);
    }

    private int GetIndentWidth(string indent)
    {
        var width = 0;
        foreach (var character in indent)
        {
            width = character == '\t'
                ? width + (_listIndentSize - (width % _listIndentSize))
                : width + 1;
        }

        return width;
    }

    [GeneratedRegex(
        "^(?<indent>[ \\t]*)(?:(?<unordered>[-+*])|(?<ordered>[0-9]+[.)]))[ \\t]+(?<content>.+?)\\s*$",
        RegexOptions.CultureInvariant)]
    private static partial Regex ListItemPattern();
}
