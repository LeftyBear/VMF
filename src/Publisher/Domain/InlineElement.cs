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
