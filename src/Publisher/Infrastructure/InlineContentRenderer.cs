using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Flattens inline content and calculates relative text style ranges.</summary>
public sealed class InlineContentRenderer
{
    /// <summary>Renders inline content.</summary>
    /// <param name="content">The inline content.</param>
    /// <returns>Flattened text and normalized relative style ranges.</returns>
    public RenderedInlineContent Render(IEnumerable<InlineContent> content)
    {
        ArgumentNullException.ThrowIfNull(content);
        var text = new StringBuilder();
        var ranges = new List<InlineStyleRange>();
        RenderContent(content, text, ranges);
        return new RenderedInlineContent(text.ToString(), MergeRanges(ranges));
    }

    private static void RenderContent(
        IEnumerable<InlineContent> content,
        StringBuilder text,
        ICollection<InlineStyleRange> ranges)
    {
        foreach (var inline in content)
        {
            ArgumentNullException.ThrowIfNull(inline);
            switch (inline)
            {
                case TextInline plainText:
                    text.Append(plainText.Text);
                    break;

                case CodeInline code:
                    RenderCode(code, text, ranges);
                    break;

                case BoldInline bold:
                    RenderStyledContent(bold.Content, InlineTextStyle.Bold, url: null, text, ranges);
                    break;

                case ItalicInline italic:
                    RenderStyledContent(italic.Content, InlineTextStyle.Italic, url: null, text, ranges);
                    break;

                case LinkInline link:
                    RenderStyledContent(link.Content, InlineTextStyle.Link, link.Url, text, ranges);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported inline content: {inline.GetType().Name}");
            }
        }
    }

    private static void RenderCode(
        CodeInline code,
        StringBuilder text,
        ICollection<InlineStyleRange> ranges)
    {
        var start = text.Length;
        text.Append(code.Text);
        ranges.Add(new InlineStyleRange(start, text.Length, InlineTextStyle.Code));
    }

    private static void RenderStyledContent(
        IEnumerable<InlineContent> content,
        InlineTextStyle style,
        Uri? url,
        StringBuilder text,
        ICollection<InlineStyleRange> ranges)
    {
        var start = text.Length;
        RenderContent(content, text, ranges);
        if (text.Length > start)
        {
            ranges.Add(new InlineStyleRange(start, text.Length, style, url));
        }
    }

    private static IReadOnlyList<InlineStyleRange> MergeRanges(IEnumerable<InlineStyleRange> ranges)
    {
        var merged = new List<InlineStyleRange>();
        foreach (var group in ranges.GroupBy(range => new StyleKey(range.Style, range.Url?.AbsoluteUri)))
        {
            InlineStyleRange? current = null;
            foreach (var range in group.OrderBy(range => range.StartOffset).ThenBy(range => range.EndOffset))
            {
                if (current is not null && range.StartOffset <= current.EndOffset)
                {
                    current = new InlineStyleRange(
                        current.StartOffset,
                        Math.Max(current.EndOffset, range.EndOffset),
                        current.Style,
                        current.Url);
                    continue;
                }

                if (current is not null)
                {
                    merged.Add(current);
                }

                current = range;
            }

            if (current is not null)
            {
                merged.Add(current);
            }
        }

        return merged
            .OrderBy(range => range.StartOffset)
            .ThenBy(range => range.EndOffset)
            .ThenBy(range => range.Style)
            .ToArray();
    }

    private sealed record StyleKey(InlineTextStyle Style, string? Url);
}
