namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Creates the separated Google Drive and Google Docs service clients.</summary>
public interface IGoogleServiceFactory
{
    /// <summary>Creates a Google Drive client.</summary>
    /// <returns>The Google Drive client.</returns>
    IGoogleDriveClient CreateDriveClient();

    /// <summary>Creates a Google Docs client.</summary>
    /// <returns>The Google Docs client.</returns>
    IGoogleDocsClient CreateDocsClient();
}
