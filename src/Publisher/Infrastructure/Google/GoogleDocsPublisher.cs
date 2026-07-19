using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Coordinates separated Google Drive creation and Google Docs content updates.</summary>
public sealed class GoogleDocsPublisher : IGoogleDocsPublisher
{
    private readonly IGoogleServiceFactory serviceFactory;
    private readonly GooglePublisherOptions options;

    /// <summary>Initializes the Google Docs publisher.</summary>
    /// <param name="serviceFactory">The Google service factory.</param>
    /// <param name="options">Google publisher settings.</param>
    public GoogleDocsPublisher(IGoogleServiceFactory serviceFactory, GooglePublisherOptions options)
    {
        this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public async Task<PublishedDocument> PublishAsync(
        CompiledDocument document,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (string.IsNullOrWhiteSpace(options.FolderId))
        {
            throw new InvalidOperationException(
                "GoogleApi:FolderId is required and must identify a folder accessible to the authenticated identity.");
        }

        var driveClient = serviceFactory.CreateDriveClient();
        var docsClient = serviceFactory.CreateDocsClient();
        var driveFile = await driveClient.CreateDocumentAsync(
            document.Title,
            options.FolderId,
            cancellationToken).ConfigureAwait(false);
        await docsClient.ApplyOperationsAsync(
            driveFile.Id,
            document.Operations,
            cancellationToken).ConfigureAwait(false);
        return new PublishedDocument(driveFile.Id, driveFile.WebViewLink);
    }
}
