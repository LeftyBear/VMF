using System.Net.Http.Headers;
using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Applies document operations through the Google Docs REST API.</summary>
public sealed class GoogleDocsClient : IGoogleDocsClient
{
    private readonly IGoogleCredentialProvider credentialProvider;
    private readonly IGoogleDocsRequestMapper requestMapper;
    private readonly HttpClient httpClient;

    /// <summary>Initializes a Google Docs client.</summary>
    /// <param name="credentialProvider">The Google credential provider.</param>
    /// <param name="requestMapper">The Google Docs request mapper.</param>
    /// <param name="httpClient">The HTTP transport.</param>
    public GoogleDocsClient(
        IGoogleCredentialProvider credentialProvider,
        IGoogleDocsRequestMapper requestMapper,
        HttpClient httpClient)
    {
        this.credentialProvider = credentialProvider ?? throw new ArgumentNullException(nameof(credentialProvider));
        this.requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task ApplyOperationsAsync(
        string documentId,
        IReadOnlyList<DocumentOperation> operations,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        ArgumentNullException.ThrowIfNull(operations);
        if (operations.Count == 0)
        {
            return;
        }

        var credential = await credentialProvider.GetCredentialAsync(cancellationToken).ConfigureAwait(false);
        var requestBody = requestMapper.MapBatchUpdate(operations);
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}:batchUpdate");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credential.AccessToken);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw GoogleApiError.Create("Google Docs API", response.StatusCode, responseBody);
        }
    }
}
