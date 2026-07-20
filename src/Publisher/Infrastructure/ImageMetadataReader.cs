using System.Buffers.Binary;
using System.Net;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Reads PNG, JPEG, and GIF metadata from local or validated remote images.</summary>
public sealed class ImageMetadataReader : IImageMetadataReader
{
    private const int DefaultDpi = 96;
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
                LocalImageSource local => Parse(
                    await File.ReadAllBytesAsync(local.Path, cancellationToken).ConfigureAwait(false),
                    local),
                RemoteImageSource remote => await ReadRemoteAsync(remote, cancellationToken)
                    .ConfigureAwait(false),
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

            return Parse(bytes, current);
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

    private static ImageMetadata Parse(byte[] bytes, ImageSource source)
    {
        var mimeType = ImageSourceResolver.DetectMimeType(bytes);
        return mimeType switch
        {
            "image/png" => ParsePng(bytes, source),
            "image/jpeg" => ParseJpeg(bytes, source),
            "image/gif" => ParseGif(bytes, source),
            _ => throw new PublishPipelineException(
                PublishErrorCodes.ImageFormatNotSupported,
                "Only PNG, JPEG, and GIF images are supported."),
        };
    }

    private static ImageMetadata ParsePng(byte[] bytes, ImageSource source)
    {
        if (bytes.Length < 24)
        {
            throw MetadataFailure();
        }

        var width = ReadPositiveUInt32(bytes.AsSpan(16, 4));
        var height = ReadPositiveUInt32(bytes.AsSpan(20, 4));
        double horizontalDpi = DefaultDpi;
        double verticalDpi = DefaultDpi;
        var offset = 8;
        while (offset + 12 <= bytes.Length)
        {
            var length = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(offset, 4));
            if (length > int.MaxValue || offset + 12L + length > bytes.Length)
            {
                throw MetadataFailure();
            }

            var dataLength = (int)length;
            var type = bytes.AsSpan(offset + 4, 4);
            if (type.SequenceEqual("pHYs"u8) && dataLength == 9 && bytes[offset + 16] == 1)
            {
                var pixelsPerMeterX = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(offset + 8, 4));
                var pixelsPerMeterY = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(offset + 12, 4));
                if (pixelsPerMeterX > 0 && pixelsPerMeterY > 0)
                {
                    horizontalDpi = pixelsPerMeterX * 0.0254d;
                    verticalDpi = pixelsPerMeterY * 0.0254d;
                }
            }

            offset += dataLength + 12;
        }

        return new ImageMetadata(source, width, height, horizontalDpi, verticalDpi, "image/png");
    }

    private static ImageMetadata ParseGif(byte[] bytes, ImageSource source)
    {
        if (bytes.Length < 10)
        {
            throw MetadataFailure();
        }

        var width = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(6, 2));
        var height = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(8, 2));
        if (width == 0 || height == 0)
        {
            throw MetadataFailure();
        }

        return new ImageMetadata(source, width, height, DefaultDpi, DefaultDpi, "image/gif");
    }

    private static ImageMetadata ParseJpeg(byte[] bytes, ImageSource source)
    {
        double horizontalDpi = DefaultDpi;
        double verticalDpi = DefaultDpi;
        int? width = null;
        int? height = null;
        var offset = 2;
        while (offset + 4 <= bytes.Length)
        {
            if (bytes[offset] != 0xFF)
            {
                offset++;
                continue;
            }

            while (offset < bytes.Length && bytes[offset] == 0xFF)
            {
                offset++;
            }

            if (offset >= bytes.Length)
            {
                break;
            }

            var marker = bytes[offset++];
            if (marker is 0xD8 or 0xD9 || marker is >= 0xD0 and <= 0xD7)
            {
                continue;
            }

            if (offset + 2 > bytes.Length)
            {
                throw MetadataFailure();
            }

            var length = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(offset, 2));
            if (length < 2 || offset + length > bytes.Length)
            {
                throw MetadataFailure();
            }

            var dataOffset = offset + 2;
            var dataLength = length - 2;
            if (marker == 0xE0 && dataLength >= 12 &&
                bytes.AsSpan(dataOffset, 5).SequenceEqual("JFIF\0"u8))
            {
                var units = bytes[dataOffset + 7];
                var xDensity = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(dataOffset + 8, 2));
                var yDensity = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(dataOffset + 10, 2));
                if (xDensity > 0 && yDensity > 0 && units is 1 or 2)
                {
                    var multiplier = units == 1 ? 1d : 2.54d;
                    horizontalDpi = xDensity * multiplier;
                    verticalDpi = yDensity * multiplier;
                }
            }

            if (IsStartOfFrame(marker) && dataLength >= 5)
            {
                height = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(dataOffset + 1, 2));
                width = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(dataOffset + 3, 2));
            }

            offset += length;
            if (width > 0 && height > 0)
            {
                return new ImageMetadata(
                    source,
                    width.Value,
                    height.Value,
                    horizontalDpi,
                    verticalDpi,
                    "image/jpeg");
            }
        }

        throw MetadataFailure();
    }

    private static int ReadPositiveUInt32(ReadOnlySpan<byte> bytes)
    {
        var value = BinaryPrimitives.ReadUInt32BigEndian(bytes);
        return value is > 0 and <= int.MaxValue ? (int)value : throw MetadataFailure();
    }

    private static bool IsStartOfFrame(byte marker) => marker is
        0xC0 or 0xC1 or 0xC2 or 0xC3 or 0xC5 or 0xC6 or 0xC7 or
        0xC9 or 0xCA or 0xCB or 0xCD or 0xCE or 0xCF;

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode is
        HttpStatusCode.MovedPermanently or HttpStatusCode.Redirect or HttpStatusCode.SeeOther or
        HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect;

    private static bool IsRetryable(HttpStatusCode statusCode) => statusCode is
        HttpStatusCode.TooManyRequests or HttpStatusCode.InternalServerError or
        HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or
        HttpStatusCode.GatewayTimeout;

    private static PublishPipelineException MetadataFailure() => new(
        PublishErrorCodes.ImageMetadataReadFailed,
        "Image dimensions could not be read.");
}
