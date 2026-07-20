using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Coordinates separated Google Drive creation and Google Docs content updates.</summary>
public sealed class GoogleDocsPublisher : IGoogleDocsPublisher
{
    private readonly IGoogleServiceFactory serviceFactory;
    private readonly IPublishPlanExecutor planExecutor;
    private readonly GooglePublisherOptions options;

    /// <summary>Initializes the Google Docs publisher.</summary>
    /// <param name="serviceFactory">The Google service factory.</param>
    /// <param name="options">Google publisher settings.</param>
    public GoogleDocsPublisher(IGoogleServiceFactory serviceFactory, GooglePublisherOptions options)
        : this(
            serviceFactory,
            options,
            new PublishPlanExecutor(
                (serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory))).CreateDocsClient()))
    {
    }

    /// <summary>Initializes the Google Docs publisher with an explicit plan executor.</summary>
    /// <param name="serviceFactory">The Google service factory.</param>
    /// <param name="options">Google publisher settings.</param>
    /// <param name="planExecutor">The publish-plan executor.</param>
    public GoogleDocsPublisher(
        IGoogleServiceFactory serviceFactory,
        GooglePublisherOptions options,
        IPublishPlanExecutor planExecutor)
    {
        this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.planExecutor = planExecutor ?? throw new ArgumentNullException(nameof(planExecutor));
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
        var driveFile = await driveClient.CreateDocumentAsync(
            document.Title,
            options.FolderId,
            cancellationToken).ConfigureAwait(false);
        await planExecutor.ExecuteAsync(
            driveFile.Id,
            document.Steps,
            cancellationToken).ConfigureAwait(false);
        return new PublishedDocument(driveFile.Id, driveFile.WebViewLink);
    }
}
