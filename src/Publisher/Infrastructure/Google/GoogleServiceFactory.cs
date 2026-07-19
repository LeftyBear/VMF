namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Creates REST-based Google Drive and Google Docs clients.</summary>
public sealed class GoogleServiceFactory : IGoogleServiceFactory
{
    private readonly IGoogleCredentialProvider credentialProvider;
    private readonly IGoogleDocsRequestMapper requestMapper;
    private readonly HttpClient httpClient;

    /// <summary>Initializes the Google service factory.</summary>
    /// <param name="credentialProvider">The Google credential provider.</param>
    /// <param name="requestMapper">The Google Docs request mapper.</param>
    /// <param name="httpClient">The shared HTTP transport.</param>
    public GoogleServiceFactory(
        IGoogleCredentialProvider credentialProvider,
        IGoogleDocsRequestMapper requestMapper,
        HttpClient httpClient)
    {
        this.credentialProvider = credentialProvider ?? throw new ArgumentNullException(nameof(credentialProvider));
        this.requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public IGoogleDriveClient CreateDriveClient() => new GoogleDriveClient(credentialProvider, httpClient);

    /// <inheritdoc />
    public IGoogleDocsClient CreateDocsClient() =>
        new GoogleDocsClient(credentialProvider, requestMapper, httpClient);
}
