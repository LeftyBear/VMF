namespace Vmf.Publisher.Domain;

/// <summary>Identifies one independently applicable inline text style.</summary>
public enum InlineTextStyle
{
    /// <summary>Bold text.</summary>
    Bold,

    /// <summary>Italic text.</summary>
    Italic,

    /// <summary>Linked text.</summary>
    Link,
}

/// <summary>Represents one relative inline style range.</summary>
public sealed class InlineStyleRange
{
    /// <summary>Initializes an inline style range.</summary>
    /// <param name="startOffset">The inclusive zero-based text offset.</param>
    /// <param name="endOffset">The exclusive zero-based text offset.</param>
    /// <param name="style">The style to apply.</param>
    /// <param name="url">The HTTP or HTTPS URL required by link styles.</param>
    public InlineStyleRange(
        int startOffset,
        int endOffset,
        InlineTextStyle style,
        Uri? url = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(startOffset);
        if (endOffset <= startOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(endOffset));
        }

        if (style == InlineTextStyle.Link)
        {
            ArgumentNullException.ThrowIfNull(url);
            if (!url.IsAbsoluteUri ||
                (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException(
                    "A link style requires an absolute HTTP or HTTPS URI.",
                    nameof(url));
            }
        }
        else if (url is not null)
        {
            throw new ArgumentException("Only link styles can specify a URL.", nameof(url));
        }

        StartOffset = startOffset;
        EndOffset = endOffset;
        Style = style;
        Url = url;
    }

    /// <summary>Gets the inclusive zero-based text offset.</summary>
    public int StartOffset { get; }

    /// <summary>Gets the exclusive zero-based text offset.</summary>
    public int EndOffset { get; }

    /// <summary>Gets the style to apply.</summary>
    public InlineTextStyle Style { get; }

    /// <summary>Gets the link URL, when the style is a link.</summary>
    public Uri? Url { get; }
}

/// <summary>Represents flattened inline text and its relative style ranges.</summary>
public sealed class RenderedInlineContent
{
    /// <summary>Initializes rendered inline content.</summary>
    /// <param name="text">The flattened text.</param>
    /// <param name="styleRanges">The relative style ranges.</param>
    public RenderedInlineContent(string text, IEnumerable<InlineStyleRange> styleRanges)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        ArgumentNullException.ThrowIfNull(styleRanges);
        var ranges = styleRanges.ToArray();
        if (ranges.Any(range => range is null || range.EndOffset > text.Length))
        {
            throw new ArgumentException(
                "Inline style ranges must be non-null and contained by the rendered text.",
                nameof(styleRanges));
        }

        StyleRanges = Array.AsReadOnly(ranges);
    }

    /// <summary>Gets the flattened text.</summary>
    public string Text { get; }

    /// <summary>Gets the relative style ranges.</summary>
    public IReadOnlyList<InlineStyleRange> StyleRanges { get; }
}
