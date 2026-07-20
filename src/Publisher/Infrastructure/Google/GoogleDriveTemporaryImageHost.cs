using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Hosts a local image briefly in a configured Google Drive folder.</summary>
public sealed class GoogleDriveTemporaryImageHost : ITemporaryImageHost
{
    private const int MaxAttempts = 3;
    private readonly IGoogleCredentialProvider credentialProvider;
    private readonly HttpClient httpClient;
    private readonly GooglePublisherOptions googleOptions;
    private readonly PublisherOptions publisherOptions;
    private readonly IPublisherLogger logger;

    /// <summary>Initializes a Google Drive temporary-image host.</summary>
    public GoogleDriveTemporaryImageHost(
        IGoogleCredentialProvider credentialProvider,
        HttpClient httpClient,
        GooglePublisherOptions googleOptions,
        PublisherOptions publisherOptions,
        IPublisherLogger? logger = null)
    {
        this.credentialProvider = credentialProvider
            ?? throw new ArgumentNullException(nameof(credentialProvider));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.googleOptions = googleOptions ?? throw new ArgumentNullException(nameof(googleOptions));
        this.publisherOptions = publisherOptions ?? throw new ArgumentNullException(nameof(publisherOptions));
        this.logger = logger ?? new NullPublisherLogger();
    }

    /// <inheritdoc />
    public async Task<TemporaryHostedImage> HostAsync(
        LocalImageSource source,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!publisherOptions.AllowTemporaryPublicImageHosting)
        {
            throw Error(
                PublishErrorCodes.ImagePublicAccessDenied,
                "Temporary public image hosting is disabled by configuration.");
        }

        if (string.IsNullOrWhiteSpace(googleOptions.TemporaryImageFolderId))
        {
            throw Error(
                PublishErrorCodes.ImageUploadFailed,
                "GoogleApi:TemporaryImageFolderId is required for local images.");
        }

        byte[] bytes;
        try
        {
            bytes = await File.ReadAllBytesAsync(source.Path, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw Error(PublishErrorCodes.ImageUploadFailed, "Local image could not be read.", exception);
        }

        var extension = Path.GetExtension(source.Path).ToLowerInvariant();
        var fileName = $"publisher-temp-{Guid.NewGuid():N}{extension}";
        var sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        string fileId;
        try
        {
            fileId = await UploadAsync(fileName, bytes, sha256, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw Error(PublishErrorCodes.ImageUploadFailed, "Temporary image upload failed.", exception);
        }

        try
        {
            await CreatePublicPermissionAsync(fileId, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await TryDeleteAfterFailureAsync(fileId).ConfigureAwait(false);
            throw;
        }
        catch (Exception exception)
        {
            await TryDeleteAfterFailureAsync(fileId).ConfigureAwait(false);
            throw Error(
                PublishErrorCodes.ImagePublicAccessDenied,
                "Temporary image public access could not be granted.",
                exception);
        }

        var publicUri = new Uri(
            $"https://drive.google.com/uc?export=download&id={Uri.EscapeDataString(fileId)}");
        return new TemporaryHostedImage(fileId, publicUri);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        TemporaryHostedImage image,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(image);
        try
        {
            using var response = await SendAsync(
                () => new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"https://www.googleapis.com/drive/v3/files/{Uri.EscapeDataString(image.ResourceId)}" +
                    "?supportsAllDrives=true"),
                cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);
                throw GoogleApiError.Create("Google Drive API", response.StatusCode, responseBody);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw Error(
                PublishErrorCodes.ImageTempFileDeleteFailed,
                "Temporary Drive image could not be deleted.",
                exception);
        }
    }

    private async Task<string> UploadAsync(
        string fileName,
        byte[] bytes,
        string sha256,
        CancellationToken cancellationToken)
    {
        var metadata = JsonSerializer.Serialize(new
        {
            name = fileName,
            parents = new[] { googleOptions.TemporaryImageFolderId },
            appProperties = new Dictionary<string, string>
            {
                ["vmfPublisherTemporary"] = "true",
                ["vmfPublisherSha256"] = sha256,
            },
        });
        using var response = await SendAsync(
            () => CreateUploadRequest(metadata, bytes, GetMimeType(fileName)),
            cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw GoogleApiError.Create("Google Drive API", response.StatusCode, responseBody);
        }

        using var document = JsonDocument.Parse(responseBody);
        var fileId = document.RootElement.TryGetProperty("id", out var idProperty)
            ? idProperty.GetString()
            : null;
        return string.IsNullOrWhiteSpace(fileId)
            ? throw new InvalidOperationException("Google Drive returned no temporary file identifier.")
            : fileId;
    }

    private async Task CreatePublicPermissionAsync(
        string fileId,
        CancellationToken cancellationToken)
    {
        using var response = await SendAsync(
            () => new HttpRequestMessage(
                HttpMethod.Post,
                $"https://www.googleapis.com/drive/v3/files/{Uri.EscapeDataString(fileId)}" +
                "/permissions?supportsAllDrives=true")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new { type = "anyone", role = "reader" }),
                    Encoding.UTF8,
                    "application/json"),
            },
            cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);
            throw GoogleApiError.Create("Google Drive API", response.StatusCode, responseBody);
        }
    }

    private async Task<HttpResponseMessage> SendAsync(
        Func<HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken)
    {
        var credential = await credentialProvider.GetCredentialAsync(cancellationToken)
            .ConfigureAwait(false);
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            using var request = requestFactory();
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                credential.AccessToken);
            var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!IsRetryable(response.StatusCode) || attempt == MaxAttempts)
            {
                return response;
            }

            response.Dispose();
            await Task.Delay(TimeSpan.FromMilliseconds(100 * (1 << (attempt - 1))), cancellationToken)
                .ConfigureAwait(false);
        }

        throw new InvalidOperationException("Google Drive retry loop terminated unexpectedly.");
    }

    private static HttpRequestMessage CreateUploadRequest(
        string metadata,
        byte[] bytes,
        string mimeType)
    {
        var content = new MultipartContent("related");
        content.Add(new StringContent(metadata, Encoding.UTF8, "application/json"));
        var imageContent = new ByteArrayContent(bytes);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
        content.Add(imageContent);
        return new HttpRequestMessage(
            HttpMethod.Post,
            "https://www.googleapis.com/upload/drive/v3/files" +
            "?uploadType=multipart&fields=id&supportsAllDrives=true")
        {
            Content = content,
        };
    }

    private async Task TryDeleteAfterFailureAsync(string fileId)
    {
        try
        {
            await DeleteAsync(
                new TemporaryHostedImage(fileId, new Uri("https://invalid.example")),
                CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception)
        {
            logger.Warning(
                PublishErrorCodes.ImageTempFileDeleteFailed,
                "Temporary Drive image cleanup failed after an earlier image-hosting failure.");
        }
    }

    private static string GetMimeType(string fileName) => Path.GetExtension(fileName) switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        _ => throw new InvalidOperationException("Unsupported temporary image extension."),
    };

    private static bool IsRetryable(HttpStatusCode statusCode) => statusCode is
        HttpStatusCode.TooManyRequests or HttpStatusCode.InternalServerError or
        HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or
        HttpStatusCode.GatewayTimeout;

    private static PublishPipelineException Error(
        string code,
        string message,
        Exception? exception = null) => new(code, message, exception);
}
