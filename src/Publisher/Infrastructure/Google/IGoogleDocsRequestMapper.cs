using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Maps publisher-neutral operations to a Google Docs batch-update payload.</summary>
public interface IGoogleDocsRequestMapper
{
    /// <summary>Maps document operations to a JSON request body.</summary>
    /// <param name="operations">The ordered document operations.</param>
    /// <returns>A Google Docs batch-update JSON body.</returns>
    string MapBatchUpdate(IReadOnlyList<DocumentOperation> operations);
}
