using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses header-based Markdown tables.</summary>
public sealed class MarkdownTableParser
{
    private readonly IMarkdownInlineParser inlineParser;

    /// <summary>Initializes a table parser with the default inline parser.</summary>
    public MarkdownTableParser()
        : this(new MarkdownInlineParser())
    {
    }

    /// <summary>Initializes a table parser.</summary>
    /// <param name="inlineParser">The inline parser used for every cell.</param>
    public MarkdownTableParser(IMarkdownInlineParser inlineParser)
    {
        this.inlineParser = inlineParser ?? throw new ArgumentNullException(nameof(inlineParser));
    }

    /// <summary>Attempts to parse a table beginning at the specified line.</summary>
    /// <param name="lines">All Markdown lines.</param>
    /// <param name="startIndex">The candidate header line index.</param>
    /// <param name="table">The parsed table when successful.</param>
    /// <param name="consumedLineCount">The number of table lines consumed.</param>
    /// <returns><see langword="true"/> when a valid table was parsed.</returns>
    public bool TryParse(
        IReadOnlyList<string> lines,
        int startIndex,
        out TableBlock? table,
        out int consumedLineCount)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (startIndex < 0 || startIndex >= lines.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        table = null;
        consumedLineCount = 0;
        if (startIndex + 1 >= lines.Count ||
            !TrySplitRow(lines[startIndex], out var headerCells) ||
            !TrySplitRow(lines[startIndex + 1], out var delimiterCells) ||
            headerCells.Count != delimiterCells.Count ||
            !TryParseColumns(delimiterCells, out var columns))
        {
            return false;
        }

        var rows = new List<TableRow>();
        var lineIndex = startIndex + 2;
        while (lineIndex < lines.Count && !string.IsNullOrWhiteSpace(lines[lineIndex]))
        {
            if (!TrySplitRow(lines[lineIndex], out var cells))
            {
                break;
            }

            rows.Add(CreateRow(cells, columns.Count));
            lineIndex++;
        }

        table = new TableBlock(
            columns,
            CreateRow(headerCells, columns.Count),
            rows);
        consumedLineCount = lineIndex - startIndex;
        return true;
    }

    private TableRow CreateRow(IReadOnlyList<string> sourceCells, int columnCount)
    {
        var cells = new List<TableCell>(columnCount);
        for (var index = 0; index < columnCount; index++)
        {
            var value = index < sourceCells.Count ? sourceCells[index].Trim() : string.Empty;
            cells.Add(value.Length == 0
                ? TableCell.Empty()
                : new TableCell(inlineParser.Parse(value)));
        }

        return new TableRow(cells);
    }

    private static bool TryParseColumns(
        IReadOnlyList<string> delimiters,
        out IReadOnlyList<TableColumn> columns)
    {
        var result = new List<TableColumn>(delimiters.Count);
        foreach (var source in delimiters)
        {
            var value = source.Trim();
            var leftColon = value.StartsWith(':');
            var rightColon = value.EndsWith(':');
            var dashStart = leftColon ? 1 : 0;
            var dashLength = value.Length - dashStart - (rightColon ? 1 : 0);
            if (dashLength < 3 || value.AsSpan(dashStart, dashLength).IndexOfAnyExcept('-') >= 0)
            {
                columns = Array.Empty<TableColumn>();
                return false;
            }

            var alignment = (leftColon, rightColon) switch
            {
                (true, true) => TableAlignment.Center,
                (false, true) => TableAlignment.Right,
                _ => TableAlignment.Left,
            };
            result.Add(new TableColumn(alignment));
        }

        columns = Array.AsReadOnly(result.ToArray());
        return true;
    }

    private static bool TrySplitRow(string line, out IReadOnlyList<string> cells)
    {
        var trimmed = line.Trim();
        var hasLeadingPipe = trimmed.StartsWith('|');
        var hasTrailingPipe = HasUnescapedTrailingPipe(trimmed);
        var content = trimmed;
        if (hasLeadingPipe)
        {
            content = content[1..];
        }

        if (hasTrailingPipe && content.Length > 0)
        {
            content = content[..^1];
        }

        var result = new List<string>();
        var cellStart = 0;
        var hasSeparator = false;
        for (var index = 0; index < content.Length; index++)
        {
            if (content[index] == '\\')
            {
                index++;
                continue;
            }

            if (content[index] != '|')
            {
                continue;
            }

            hasSeparator = true;
            result.Add(content[cellStart..index]);
            cellStart = index + 1;
        }

        result.Add(content[cellStart..]);
        if (!hasSeparator && !(hasLeadingPipe && hasTrailingPipe))
        {
            cells = Array.Empty<string>();
            return false;
        }

        cells = Array.AsReadOnly(result.ToArray());
        return result.Count > 0;
    }

    private static bool HasUnescapedTrailingPipe(string value)
    {
        if (!value.EndsWith('|'))
        {
            return false;
        }

        var backslashes = 0;
        for (var index = value.Length - 2; index >= 0 && value[index] == '\\'; index--)
        {
            backslashes++;
        }

        return backslashes % 2 == 0;
    }
}
