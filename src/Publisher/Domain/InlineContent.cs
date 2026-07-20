namespace Vmf.Publisher.Domain;

/// <summary>Represents publisher-neutral inline content.</summary>
public abstract class InlineContent
{
    private protected InlineContent()
    {
    }
}

/// <summary>Represents plain inline text.</summary>
public sealed class TextInline : InlineContent
{
    /// <summary>Initializes plain inline text.</summary>
    /// <param name="text">The text.</param>
    public TextInline(string text)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>Gets the text.</summary>
    public string Text { get; }
}

/// <summary>Represents bold inline content.</summary>
public sealed class BoldInline : InlineContent
{
    /// <summary>Initializes bold inline content.</summary>
    /// <param name="content">The nested inline content.</param>
    public BoldInline(IEnumerable<InlineContent> content)
    {
        Content = InlineContentCollection.Create(content, nameof(content));
    }

    /// <summary>Gets the nested inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }
}

/// <summary>Represents italic inline content.</summary>
public sealed class ItalicInline : InlineContent
{
    /// <summary>Initializes italic inline content.</summary>
    /// <param name="content">The nested inline content.</param>
    public ItalicInline(IEnumerable<InlineContent> content)
    {
        Content = InlineContentCollection.Create(content, nameof(content));
    }

    /// <summary>Gets the nested inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }
}

/// <summary>Represents linked inline content.</summary>
public sealed class LinkInline : InlineContent
{
    /// <summary>Initializes linked inline content.</summary>
    /// <param name="content">The link label content.</param>
    /// <param name="url">The absolute HTTP or HTTPS target.</param>
    public LinkInline(IEnumerable<InlineContent> content, Uri url)
    {
        Content = InlineContentCollection.Create(content, nameof(content));
        Url = ValidateUrl(url);
    }

    /// <summary>Gets the link label content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }

    /// <summary>Gets the absolute HTTP or HTTPS target.</summary>
    public Uri Url { get; }

    private static Uri ValidateUrl(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        if (!url.IsAbsoluteUri ||
            (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("A link URL must be an absolute HTTP or HTTPS URI.", nameof(url));
        }

        return url;
    }
}

internal static class InlineContentCollection
{
    internal static IReadOnlyList<InlineContent> Create(
        IEnumerable<InlineContent> content,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(content, parameterName);
        var items = content.ToArray();
        if (items.Length == 0)
        {
            throw new ArgumentException("Inline content must not be empty.", parameterName);
        }

        if (items.Any(item => item is null))
        {
            throw new ArgumentException("Inline content must not contain null items.", parameterName);
        }

        return Array.AsReadOnly(items);
    }
}
