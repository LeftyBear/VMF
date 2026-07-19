namespace Vmf.Publisher.Domain;

/// <summary>Represents a document compiled into target-neutral operations.</summary>
public sealed class CompiledDocument
{
    /// <summary>Initializes a compiled document.</summary>
    /// <param name="title">The destination document title.</param>
    /// <param name="operations">The ordered document operations.</param>
    public CompiledDocument(string title, IEnumerable<DocumentOperation> operations)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        ArgumentNullException.ThrowIfNull(operations);
        Operations = Array.AsReadOnly(operations.ToArray());
    }

    /// <summary>Gets the destination document title.</summary>
    public string Title { get; }

    /// <summary>Gets the ordered document operations.</summary>
    public IReadOnlyList<DocumentOperation> Operations { get; }
}
