namespace Vmf.Publisher.Domain;

/// <summary>Represents a publisher-neutral document.</summary>
public sealed class DocumentModel
{
    /// <summary>Initializes a document model.</summary>
    /// <param name="blocks">The ordered document blocks.</param>
    public DocumentModel(IEnumerable<DocumentBlock> blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        Blocks = Array.AsReadOnly(blocks.ToArray());
    }

    /// <summary>Gets the ordered document blocks.</summary>
    public IReadOnlyList<DocumentBlock> Blocks { get; }
}
