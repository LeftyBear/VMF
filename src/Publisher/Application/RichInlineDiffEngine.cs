using System.Globalization;
using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Creates deterministic inline physical updates from canonical inline content.</summary>
internal sealed class RichInlineDiffEngine
{
    private readonly DocumentIndexMapper indexMapper = new();

    internal IReadOnlyList<InlinePhysicalEdit> CreateEdits(
        ManagedBlockSnapshot previous,
        DocumentBlock candidate)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(candidate);
        var baseline = previous.CanonicalBlock;
        if (!CanDiffInline(previous, baseline, candidate))
        {
            return Array.Empty<InlinePhysicalEdit>();
        }

        var previousInline = CanonicalInlineText.Render(baseline!.Content);
        var candidateInline = CanonicalInlineText.Render(candidate.Content);
        if (previousInline.Text.Length > previous.Range.Length - 1)
        {
            return Array.Empty<InlinePhysicalEdit>();
        }

        if (!string.Equals(previousInline.Text, candidateInline.Text, StringComparison.Ordinal))
        {
            return
            [
                InlinePhysicalEdit.Replace(
                    indexMapper.MapInlineRange(previous.Range, 0, previousInline.Text.Length),
                    candidateInline.Text,
                    candidateInline.StyleRanges),
            ];
        }

        var edits = BuildStyleEdits(previous.Range, previousInline, candidateInline, indexMapper);
        return edits.Count == 0 ? Array.Empty<InlinePhysicalEdit>() : edits;
    }

    private static bool CanDiffInline(
        ManagedBlockSnapshot previous,
        DocumentBlock? baseline,
        DocumentBlock candidate)
    {
        if (baseline is null ||
            previous.Range.Length < 1 ||
            baseline.Kind != candidate.Kind ||
            baseline.Level != candidate.Level ||
            !SupportsInlineDiff(candidate.Kind))
        {
            return false;
        }

        return string.Equals(baseline.ExplicitId, candidate.ExplicitId, StringComparison.Ordinal);
    }

    private static bool SupportsInlineDiff(DocumentBlockKind kind) =>
        kind is DocumentBlockKind.Heading or
            DocumentBlockKind.Paragraph or
            DocumentBlockKind.BulletListItem or
            DocumentBlockKind.Quote;

    private static IReadOnlyList<InlinePhysicalEdit> BuildStyleEdits(
        DocumentTextRange blockRange,
        CanonicalInlineText previous,
        CanonicalInlineText candidate,
        DocumentIndexMapper indexMapper)
    {
        if (previous.Elements.Count != candidate.Elements.Count)
        {
            return Array.Empty<InlinePhysicalEdit>();
        }

        var edits = new List<InlinePhysicalEdit>();
        AddBooleanStyleEdits(blockRange, previous, candidate, InlineTextStyle.Bold, edits, indexMapper);
        AddBooleanStyleEdits(blockRange, previous, candidate, InlineTextStyle.Italic, edits, indexMapper);
        AddBooleanStyleEdits(blockRange, previous, candidate, InlineTextStyle.Code, edits, indexMapper);
        AddLinkStyleEdits(blockRange, previous, candidate, edits, indexMapper);
        return edits
            .OrderBy(item => item.Range.StartIndex)
            .ThenBy(item => item.Range.EndIndex)
            .ThenBy(item => item.Update.Style)
            .ThenBy(item => item.Update.Url?.AbsoluteUri, StringComparer.Ordinal)
            .ToArray();
    }

    private static void AddBooleanStyleEdits(
        DocumentTextRange blockRange,
        CanonicalInlineText previous,
        CanonicalInlineText candidate,
        InlineTextStyle style,
        ICollection<InlinePhysicalEdit> edits,
        DocumentIndexMapper indexMapper)
    {
        AddRuns(previous, candidate, edits, (oldElement, newElement) =>
        {
            var oldValue = oldElement.Style.IsEnabled(style);
            var newValue = newElement.Style.IsEnabled(style);
            return oldValue == newValue
                ? null
                : new StyleChange(style, newValue, null);
        }, blockRange, indexMapper);
    }

    private static void AddLinkStyleEdits(
        DocumentTextRange blockRange,
        CanonicalInlineText previous,
        CanonicalInlineText candidate,
        ICollection<InlinePhysicalEdit> edits,
        DocumentIndexMapper indexMapper)
    {
        AddRuns(previous, candidate, edits, (oldElement, newElement) =>
        {
            if (string.Equals(
                oldElement.Style.LinkUrl,
                newElement.Style.LinkUrl,
                StringComparison.Ordinal))
            {
                return null;
            }

            return newElement.Style.LinkUrl is null
                ? new StyleChange(InlineTextStyle.Link, false, null)
                : new StyleChange(
                    InlineTextStyle.Link,
                    true,
                    new Uri(newElement.Style.LinkUrl, UriKind.Absolute));
        }, blockRange, indexMapper);
    }

    private static void AddRuns(
        CanonicalInlineText previous,
        CanonicalInlineText candidate,
        ICollection<InlinePhysicalEdit> edits,
        Func<CanonicalInlineElement, CanonicalInlineElement, StyleChange?> selector,
        DocumentTextRange blockRange,
        DocumentIndexMapper indexMapper)
    {
        StyleChange? active = null;
        int? startOffset = null;
        var endOffset = 0;
        for (var index = 0; index < previous.Elements.Count; index++)
        {
            var change = selector(previous.Elements[index], candidate.Elements[index]);
            if (change is not null && active is not null && active.Equals(change))
            {
                endOffset = candidate.Elements[index].EndOffset;
                continue;
            }

            Flush();
            if (change is not null)
            {
                active = change;
                startOffset = candidate.Elements[index].StartOffset;
                endOffset = candidate.Elements[index].EndOffset;
            }
        }

        Flush();

        void Flush()
        {
            if (active is null || startOffset is null)
            {
                return;
            }

            edits.Add(InlinePhysicalEdit.Style(
                indexMapper.MapInlineRange(blockRange, startOffset.Value, endOffset),
                active.Style,
                active.Enabled,
                active.Url));
            active = null;
            startOffset = null;
        }
    }

    private sealed record StyleChange(InlineTextStyle Style, bool Enabled, Uri? Url);
}

internal sealed class InlinePhysicalEdit
{
    private InlinePhysicalEdit(
        PhysicalOperationKind kind,
        DocumentTextRange range,
        InlinePhysicalUpdate update)
    {
        Kind = kind;
        Range = range;
        Update = update;
    }

    internal PhysicalOperationKind Kind { get; }

    internal DocumentTextRange Range { get; }

    internal InlinePhysicalUpdate Update { get; }

    internal static InlinePhysicalEdit Replace(
        DocumentTextRange range,
        string text,
        IEnumerable<InlineStyleRange> styleRanges) => new(
            PhysicalOperationKind.ReplaceInlineContent,
            range,
            new InlinePhysicalUpdate(text, styleRanges, null, null, null));

    internal static InlinePhysicalEdit Style(
        DocumentTextRange range,
        InlineTextStyle style,
        bool enabled,
        Uri? url) => new(
            PhysicalOperationKind.UpdateInlineStyle,
            range,
            new InlinePhysicalUpdate(null, null, style, enabled, url));
}

internal sealed class CanonicalInlineText
{
    private CanonicalInlineText(
        string text,
        IReadOnlyList<CanonicalInlineElement> elements,
        IReadOnlyList<InlineStyleRange> styleRanges)
    {
        Text = text;
        Elements = elements;
        StyleRanges = styleRanges;
    }

    internal string Text { get; }

    internal IReadOnlyList<CanonicalInlineElement> Elements { get; }

    internal IReadOnlyList<InlineStyleRange> StyleRanges { get; }

    internal static CanonicalInlineText Render(IEnumerable<InlineContent> content)
    {
        var text = new StringBuilder();
        var ranges = new List<InlineStyleRange>();
        RenderContent(content, text, ranges);
        var renderedText = text.ToString();
        return new CanonicalInlineText(
            renderedText,
            BuildElements(renderedText, ranges),
            MergeRanges(ranges));
    }

    private static void RenderContent(
        IEnumerable<InlineContent> content,
        StringBuilder text,
        ICollection<InlineStyleRange> ranges)
    {
        foreach (var inline in content)
        {
            switch (inline)
            {
                case TextInline plain:
                    text.Append(plain.Text);
                    break;
                case CodeInline code:
                    AddStyledText(code.Text, InlineTextStyle.Code, null, text, ranges);
                    break;
                case BoldInline bold:
                    AddStyledContent(bold.Content, InlineTextStyle.Bold, null, text, ranges);
                    break;
                case ItalicInline italic:
                    AddStyledContent(italic.Content, InlineTextStyle.Italic, null, text, ranges);
                    break;
                case LinkInline link:
                    AddStyledContent(link.Content, InlineTextStyle.Link, link.Url, text, ranges);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported inline content: {inline.GetType().Name}");
            }
        }
    }

    private static void AddStyledText(
        string value,
        InlineTextStyle style,
        Uri? url,
        StringBuilder text,
        ICollection<InlineStyleRange> ranges)
    {
        var start = text.Length;
        text.Append(value);
        if (text.Length > start)
        {
            ranges.Add(new InlineStyleRange(start, text.Length, style, url));
        }
    }

    private static void AddStyledContent(
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

    private static IReadOnlyList<CanonicalInlineElement> BuildElements(
        string text,
        IEnumerable<InlineStyleRange> ranges)
    {
        var items = new List<CanonicalInlineElement>();
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var start = enumerator.ElementIndex;
            var value = enumerator.GetTextElement();
            var end = start + value.Length;
            items.Add(new CanonicalInlineElement(
                value,
                start,
                end,
                CanonicalInlineStyle.FromRanges(ranges, start, end)));
        }

        return items;
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
            .ThenBy(range => range.Url?.AbsoluteUri, StringComparer.Ordinal)
            .ToArray();
    }

    private sealed record StyleKey(InlineTextStyle Style, string? Url);
}

internal sealed record CanonicalInlineElement(
    string Text,
    int StartOffset,
    int EndOffset,
    CanonicalInlineStyle Style);

internal sealed record CanonicalInlineStyle(
    bool Bold,
    bool Italic,
    bool Code,
    string? LinkUrl)
{
    internal bool IsEnabled(InlineTextStyle style) => style switch
    {
        InlineTextStyle.Bold => Bold,
        InlineTextStyle.Italic => Italic,
        InlineTextStyle.Code => Code,
        InlineTextStyle.Link => LinkUrl is not null,
        _ => throw new InvalidOperationException($"Unsupported inline style: {style}"),
    };

    internal static CanonicalInlineStyle FromRanges(
        IEnumerable<InlineStyleRange> ranges,
        int startOffset,
        int endOffset)
    {
        var applicable = ranges
            .Where(range => range.StartOffset <= startOffset && range.EndOffset >= endOffset)
            .OrderBy(range => range.Style)
            .ThenBy(range => range.Url?.AbsoluteUri, StringComparer.Ordinal)
            .ToArray();
        return new CanonicalInlineStyle(
            applicable.Any(range => range.Style == InlineTextStyle.Bold),
            applicable.Any(range => range.Style == InlineTextStyle.Italic),
            applicable.Any(range => range.Style == InlineTextStyle.Code),
            applicable.FirstOrDefault(range => range.Style == InlineTextStyle.Link)?.Url?.AbsoluteUri);
    }
}
