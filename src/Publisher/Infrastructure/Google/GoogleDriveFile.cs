namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Identifies a file created in Google Drive.</summary>
public sealed class GoogleDriveFile
{
    /// <summary>Initializes Google Drive file information.</summary>
    /// <param name="id">The file identifier.</param>
    /// <param name="webViewLink">The browser URL for the file.</param>
    public GoogleDriveFile(string id, string webViewLink)
    {
        Id = id;
        WebViewLink = webViewLink;
    }

    /// <summary>Gets the file identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the browser URL for the file.</summary>
    public string WebViewLink { get; }
}
