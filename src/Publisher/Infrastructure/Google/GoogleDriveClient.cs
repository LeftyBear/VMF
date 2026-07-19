using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Creates native Google documents through the Google Drive REST API.</summary>
public sealed class GoogleDriveClient : IGoogleDriveClient
{
    private readonly IGoogleCredentialProvider credentialProvider;
    private readonly HttpClient httpClient;

    /// <summary>Initializes a Google Drive client.</summary>
    /// <param name="credentialProvider">The Google credential provider.</param>
    /// <param name="httpClient">The HTTP transport.</param>
    public GoogleDriveClient(IGoogleCredentialProvider credentialProvider, HttpClient httpClient)
    {
        this.credentialProvider = credentialProvider ?? throw new ArgumentNullException(nameof(credentialProvider));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<GoogleDriveFile> CreateDocumentAsync(
        string title,
        string? folderId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        var credential = await credentialProvider.GetCredentialAsync(cancellationToken).ConfigureAwait(false);
        var metadata = new Dictionary<string, object>
        {
            ["name"] = title,
            ["mimeType"] = "application/vnd.google-apps.document",
        };
        if (!string.IsNullOrWhiteSpace(folderId))
        {
            metadata["parents"] = new[] { folderId };
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://www.googleapis.com/drive/v3/files?fields=id%2CwebViewLink");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credential.AccessToken);
        request.Content = new StringContent(
            JsonSerializer.Serialize(metadata),
            Encoding.UTF8,
            "application/json");
        using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Google Drive document creation failed with HTTP {(int)response.StatusCode}.");
        }

        using var document = JsonDocument.Parse(responseBody);
        var id = document.RootElement.GetProperty("id").GetString();
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvalidOperationException("Google Drive returned no document identifier.");
        }

        var url = document.RootElement.TryGetProperty("webViewLink", out var urlProperty)
            ? urlProperty.GetString()
            : null;
        return new GoogleDriveFile(id, url ?? $"https://docs.google.com/document/d/{id}/edit");
    }
}
