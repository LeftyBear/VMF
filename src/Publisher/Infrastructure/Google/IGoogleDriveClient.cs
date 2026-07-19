namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Defines the Google Drive operations required by the publisher.</summary>
public interface IGoogleDriveClient
{
    /// <summary>Creates an empty native Google document in Drive.</summary>
    /// <param name="title">The document title.</param>
    /// <param name="folderId">An optional destination folder identifier.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>The created Drive file.</returns>
    Task<GoogleDriveFile> CreateDocumentAsync(
        string title,
        string? folderId,
        CancellationToken cancellationToken);
}
