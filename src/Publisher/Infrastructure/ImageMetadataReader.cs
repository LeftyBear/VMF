using System.Net;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Reads PNG, JPEG, and GIF metadata from local or validated remote images.</summary>
public sealed class ImageMetadataReader : IImageMetadataReader
{
    private const int MaxAttempts = 3;
    private const int MaxRedirects = 5;
    private const int MaxImageBytes = 25 * 1024 * 1024;
    private readonly HttpClient httpClient;
    private readonly IImageSourceResolver sourceResolver;

    /// <summary>Initializes a metadata reader.</summary>
    public ImageMetadataReader(HttpClient httpClient, IImageSourceResolver sourceResolver)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.sourceResolver = sourceResolver ?? throw new ArgumentNullException(nameof(sourceResolver));
    }

    /// <inheritdoc />
    public async Task<ImageMetadata> ReadAsync(
        ImageSource source,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        try
        {
            return source switch
            {
                LocalImageSource local => ImageMetadataParser.Parse(
                    await File.ReadAllBytesAsync(local.Path, cancellationToken).ConfigureAwait(false),
                    local),
                RemoteImageSource remote => await ReadRemoteAsync(remote, cancellationToken)
                    .ConfigureAwait(false),
                GoogleDriveImageSource drive => await ReadGoogleDriveAsync(drive, cancellationToken)
                    .ConfigureAwait(false),
                EmbeddedImageSource embedded => ImageMetadataParser.Parse(embedded.Content.ToArray(), embedded),
                _ => throw new InvalidOperationException("Unsupported image source type."),
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PublishPipelineException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PublishPipelineException(
                PublishErrorCodes.ImageMetadataReadFailed,
                "Image metadata could not be read.",
                exception);
        }
    }

    private async Task<ImageMetadata> ReadRemoteAsync(
        RemoteImageSource initialSource,
        CancellationToken cancellationToken)
    {
        var result = await ReadRemoteBytesAsync(initialSource, cancellationToken).ConfigureAwait(false);
        return ImageMetadataParser.Parse(result.Bytes, result.FinalSource);
    }

    private async Task<ImageMetadata> ReadGoogleDriveAsync(
        GoogleDriveImageSource source,
        CancellationToken cancellationToken)
    {
        var result = await ReadRemoteBytesAsync(new RemoteImageSource(source.PublicUri), cancellationToken)
            .ConfigureAwait(false);
        return ImageMetadataParser.Parse(result.Bytes, source);
    }

    private async Task<(byte[] Bytes, RemoteImageSource FinalSource)> ReadRemoteBytesAsync(
        RemoteImageSource initialSource,
        CancellationToken cancellationToken)
    {
        var current = initialSource;
        for (var redirect = 0; redirect <= MaxRedirects; redirect++)
        {
            using var response = await SendWithRetryAsync(current.Uri, cancellationToken)
                .ConfigureAwait(false);
            if (IsRedirect(response.StatusCode))
            {
                if (redirect == MaxRedirects || response.Headers.Location is null)
                {
                    throw new PublishPipelineException(
                        PublishErrorCodes.ImageUriResolutionFailed,
                        "Remote image redirect chain was invalid or too long.");
                }

                var redirectedUri = response.Headers.Location.IsAbsoluteUri
                    ? response.Headers.Location
                    : new Uri(current.Uri, response.Headers.Location);
                var validated = await sourceResolver.ResolveAsync(
                    new RemoteImageSource(redirectedUri),
                    Path.Combine(Environment.CurrentDirectory, "remote-image.md"),
                    cancellationToken).ConfigureAwait(false);
                current = (RemoteImageSource)validated;
                continue;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.ImageMetadataReadFailed,
                    $"Remote image request failed with HTTP {(int)response.StatusCode}.");
            }

            if (response.Content.Headers.ContentLength is > MaxImageBytes)
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.ImageMetadataReadFailed,
                    "Remote image exceeds the maximum inspection size.");
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken)
                .ConfigureAwait(false);
            if (bytes.Length > MaxImageBytes)
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.ImageMetadataReadFailed,
                    "Remote image exceeds the maximum inspection size.");
            }

            return (bytes, current);
        }

        throw new InvalidOperationException("Remote image redirect loop terminated unexpectedly.");
    }

    private async Task<HttpResponseMessage> SendWithRetryAsync(
        Uri uri,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);
            if (!IsRetryable(response.StatusCode) || attempt == MaxAttempts)
            {
                return response;
            }

            response.Dispose();
            await Task.Delay(TimeSpan.FromMilliseconds(100 * (1 << (attempt - 1))), cancellationToken)
                .ConfigureAwait(false);
        }

        throw new InvalidOperationException("Remote image retry loop terminated unexpectedly.");
    }

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode is
        HttpStatusCode.MovedPermanently or HttpStatusCode.Redirect or HttpStatusCode.SeeOther or
        HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect;

    private static bool IsRetryable(HttpStatusCode statusCode) => statusCode is
        HttpStatusCode.TooManyRequests or HttpStatusCode.InternalServerError or
        HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or
        HttpStatusCode.GatewayTimeout;
}
