using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Defines the Google Docs operations required by the publisher.</summary>
public interface IGoogleDocsClient
{
    /// <summary>Applies compiled operations to a Google document.</summary>
    /// <param name="documentId">The target document identifier.</param>
    /// <param name="operations">The ordered document operations.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>A task that completes after the batch update.</returns>
    Task ApplyOperationsAsync(
        string documentId,
        IReadOnlyList<DocumentOperation> operations,
        CancellationToken cancellationToken);

    /// <summary>Inserts an empty table at an exact document index.</summary>
    Task InsertTableAsync(
        string documentId,
        int rows,
        int columns,
        int index,
        CancellationToken cancellationToken);

    /// <summary>Inserts a standalone inline image and applies START paragraph alignment.</summary>
    Task InsertInlineImageAsync(
        string documentId,
        Uri imageUri,
        ImageSize size,
        int index,
        CancellationToken cancellationToken);

    /// <summary>Reads the current document table and inline-image structure.</summary>
    Task<GoogleDocumentSnapshot> GetDocumentAsync(
        string documentId,
        CancellationToken cancellationToken);
}
