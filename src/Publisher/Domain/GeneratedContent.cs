namespace Vmf.Publisher.Domain;

/// <summary>Identifies the marker style of a Markdown list item.</summary>
public enum ListKind
{
    /// <summary>An item rendered with a bullet marker.</summary>
    Unordered,

    /// <summary>An item rendered with an ordered marker.</summary>
    Ordered,
}

/// <summary>Represents one item in a publisher-neutral list.</summary>
public sealed class ListItem
{
    /// <summary>Initializes a list item.</summary>
    /// <param name="kind">The list marker kind.</param>
    /// <param name="content">The inline content.</param>
    /// <param name="depth">The zero-based normalized nesting depth.</param>
    public ListItem(ListKind kind, IEnumerable<InlineContent> content, int depth)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(depth);

        Kind = kind;
        Content = InlineContentCollection.Create(content, nameof(content));
        Depth = depth;
    }

    /// <summary>Initializes a list item from the legacy plain-text inline model.</summary>
    /// <param name="kind">The list marker kind.</param>
    /// <param name="inlines">The plain inline content.</param>
    /// <param name="depth">The zero-based normalized nesting depth.</param>
    public ListItem(ListKind kind, IEnumerable<InlineElement> inlines, int depth)
        : this(kind, InlineContentCompatibility.Convert(inlines), depth)
    {
    }

    /// <summary>Creates a plain-text list item.</summary>
    /// <param name="kind">The list marker kind.</param>
    /// <param name="text">The item text.</param>
    /// <param name="depth">The zero-based normalized nesting depth.</param>
    /// <returns>The list item.</returns>
    public static ListItem FromText(ListKind kind, string text, int depth) =>
        new(kind, [new TextInline(text)], depth);

    /// <summary>Gets the list marker kind.</summary>
    public ListKind Kind { get; }

    /// <summary>Gets the inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }

    /// <summary>Gets a flattened legacy plain-text view of the inline content.</summary>
    public IReadOnlyList<InlineElement> Inlines => InlineContentCompatibility.Flatten(Content);

    /// <summary>Gets the zero-based normalized nesting depth.</summary>
    public int Depth { get; }
}

/// <summary>Represents a contiguous sequence of Markdown list items.</summary>
public sealed class ListBlock
{
    /// <summary>Initializes a list block.</summary>
    /// <param name="items">The list items in source order.</param>
    public ListBlock(IEnumerable<ListItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        var itemArray = items.ToArray();
        if (itemArray.Length == 0)
        {
            throw new ArgumentException("A list block requires at least one item.", nameof(items));
        }

        Items = Array.AsReadOnly(itemArray);
    }

    /// <summary>Gets the list items in source order.</summary>
    public IReadOnlyList<ListItem> Items { get; }
}
