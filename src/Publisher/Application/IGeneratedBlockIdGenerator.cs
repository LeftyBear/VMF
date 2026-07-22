using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Generates deterministic fallback identifiers for canonical document blocks.</summary>
public interface IGeneratedBlockIdGenerator
{
    /// <summary>Generates identifiers aligned with document order.</summary>
    /// <remarks>Blocks carrying an explicit identifier return <see langword="null"/>.</remarks>
    /// <param name="document">The canonical document model.</param>
    /// <returns>Generated identifiers aligned one-to-one with document blocks.</returns>
    IReadOnlyList<string?> Generate(DocumentModel document);
}
