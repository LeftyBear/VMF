using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses backtick-fenced Markdown code blocks.</summary>
public sealed class MarkdownCodeBlockParser
{
    /// <summary>Attempts to parse a fenced code block at the specified line.</summary>
    /// <param name="lines">All normalized Markdown lines.</param>
    /// <param name="startIndex">The candidate opening-fence line.</param>
    /// <param name="block">The parsed code block when successful.</param>
    /// <param name="consumedLineCount">The number of source lines consumed.</param>
    /// <returns><see langword="true"/> when an opening fence was found.</returns>
    public bool TryParse(
        IReadOnlyList<string> lines,
        int startIndex,
        out CodeBlock? block,
        out int consumedLineCount)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (startIndex < 0 || startIndex >= lines.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        block = null;
        consumedLineCount = 0;
        if (!TryReadOpeningFence(lines[startIndex], out var fenceLength, out var language))
        {
            return false;
        }

        var bodyLines = new List<string>();
        var lineIndex = startIndex + 1;
        while (lineIndex < lines.Count && !IsClosingFence(lines[lineIndex], fenceLength))
        {
            bodyLines.Add(lines[lineIndex]);
            lineIndex++;
        }

        var isClosed = lineIndex < lines.Count;
        if (!isClosed && bodyLines.Count > 0 && bodyLines[^1].Length == 0)
        {
            // Split preserves one synthetic empty entry for a source-ending newline.
            bodyLines.RemoveAt(bodyLines.Count - 1);
        }

        block = new CodeBlock(string.Join("\n", bodyLines), language);
        consumedLineCount = isClosed
            ? lineIndex - startIndex + 1
            : lines.Count - startIndex;
        return true;
    }

    private static bool TryReadOpeningFence(
        string line,
        out int fenceLength,
        out string language)
    {
        var markerIndex = CountLeadingSpaces(line);
        fenceLength = markerIndex <= 3 ? CountBackticks(line, markerIndex) : 0;
        if (fenceLength < 3)
        {
            language = string.Empty;
            return false;
        }

        language = line[(markerIndex + fenceLength)..].Trim();
        return true;
    }

    private static bool IsClosingFence(string line, int openingFenceLength)
    {
        var markerIndex = CountLeadingSpaces(line);
        if (markerIndex > 3)
        {
            return false;
        }

        var fenceLength = CountBackticks(line, markerIndex);
        return fenceLength >= openingFenceLength &&
            line.AsSpan(markerIndex + fenceLength).Trim().Length == 0;
    }

    private static int CountLeadingSpaces(string line)
    {
        var count = 0;
        while (count < line.Length && line[count] == ' ')
        {
            count++;
        }

        return count;
    }

    private static int CountBackticks(string line, int startIndex)
    {
        var count = 0;
        while (startIndex + count < line.Length && line[startIndex + count] == '`')
        {
            count++;
        }

        return count;
    }
}
