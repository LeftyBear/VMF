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
        Kind = kind;
        Level = level;
        Inlines = Array.AsReadOnly(inlines.ToArray());
    }

    /// <summary>Gets the block kind.</summary>
    public DocumentBlockKind Kind { get; }

    /// <summary>Gets the heading level, or zero for non-headings.</summary>
    public int Level { get; }

    /// <summary>Gets the inline content.</summary>
    public IReadOnlyList<InlineElement> Inlines { get; }
}
