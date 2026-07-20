namespace Vmf.Publisher.Domain;

/// <summary>Identifies a supported inline element kind.</summary>
public enum InlineElementKind
{
    /// <summary>A plain text element.</summary>
    Text,
}

/// <summary>Represents inline document content.</summary>
public sealed class InlineElement
{
    /// <summary>Initializes an inline element.</summary>
    /// <param name="kind">The element kind.</param>
    /// <param name="text">The element text.</param>
    public InlineElement(InlineElementKind kind, string text)
    {
        Kind = kind;
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>Gets the element kind.</summary>
    public InlineElementKind Kind { get; }

    /// <summary>Gets the element text.</summary>
    public string Text { get; }
}

internal static class InlineContentCompatibility
{
    internal static IReadOnlyList<InlineContent> Convert(IEnumerable<InlineElement> inlines)
    {
        ArgumentNullException.ThrowIfNull(inlines);
        return Array.AsReadOnly(
            inlines.Select(inline =>
            {
                ArgumentNullException.ThrowIfNull(inline);
                return (InlineContent)new TextInline(inline.Text);
            }).ToArray());
    }

    internal static IReadOnlyList<InlineElement> Flatten(IEnumerable<InlineContent> content)
    {
        ArgumentNullException.ThrowIfNull(content);
        return Array.AsReadOnly(
            content.Select(inline => new InlineElement(InlineElementKind.Text, GetText(inline))).ToArray());
    }

    internal static string GetText(InlineContent content) => content switch
    {
        TextInline text => text.Text,
        CodeInline code => code.Text,
        BoldInline bold => string.Concat(bold.Content.Select(GetText)),
        ItalicInline italic => string.Concat(italic.Content.Select(GetText)),
        LinkInline link => string.Concat(link.Content.Select(GetText)),
        _ => throw new InvalidOperationException($"Unsupported inline content: {content.GetType().Name}"),
    };
}
