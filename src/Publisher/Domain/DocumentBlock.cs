namespace Vmf.Publisher.Domain;

/// <summary>Identifies a supported document block kind.</summary>
public enum DocumentBlockKind
{
    /// <summary>A heading block.</summary>
    Heading,
    /// <summary>A paragraph block.</summary>
    Paragraph,
    /// <summary>An unordered list item block.</summary>
    BulletListItem,
    /// <summary>A contiguous ordered, unordered, or mixed list block.</summary>
    List,
}

/// <summary>Represents one block in a document.</summary>
public sealed class DocumentBlock
{
    /// <summary>Initializes a document block.</summary>
    /// <param name="kind">The block kind.</param>
    /// <param name="inlines">The inline content.</param>
    /// <param name="level">The heading level, or zero for non-headings.</param>
    public DocumentBlock(DocumentBlockKind kind, IEnumerable<InlineElement> inlines, int level = 0)
    {
        ArgumentNullException.ThrowIfNull(inlines);
        if (kind == DocumentBlockKind.List)
        {
            throw new ArgumentException(
                "Use the ListBlock constructor for list document blocks.",
                nameof(kind));
        }

        Kind = kind;
        Level = level;
        Inlines = Array.AsReadOnly(inlines.ToArray());
    }

    /// <summary>Initializes a list document block.</summary>
    /// <param name="list">The list content.</param>
    public DocumentBlock(ListBlock list)
    {
        List = list ?? throw new ArgumentNullException(nameof(list));
        Kind = DocumentBlockKind.List;
        Inlines = Array.Empty<InlineElement>();
    }

    /// <summary>Gets the block kind.</summary>
    public DocumentBlockKind Kind { get; }

    /// <summary>Gets the heading level, or zero for non-headings.</summary>
    public int Level { get; }

    /// <summary>Gets the inline content.</summary>
    public IReadOnlyList<InlineElement> Inlines { get; }

    /// <summary>Gets the list content when <see cref="Kind"/> is <see cref="DocumentBlockKind.List"/>.</summary>
    public ListBlock? List { get; }
}
