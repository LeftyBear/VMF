using System.Net;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Resolves DNS host names for remote image validation.</summary>
public interface IHostAddressResolver
{
    /// <summary>Returns all addresses currently associated with a host.</summary>
    Task<IPAddress[]> GetHostAddressesAsync(string host, CancellationToken cancellationToken);
}

/// <summary>Uses the operating-system DNS resolver.</summary>
public sealed class SystemHostAddressResolver : IHostAddressResolver
{
    /// <inheritdoc />
    public Task<IPAddress[]> GetHostAddressesAsync(
        string host,
        CancellationToken cancellationToken) => Dns.GetHostAddressesAsync(host, cancellationToken);
}

/// <summary>Normalizes local paths and rejects unsafe remote image endpoints.</summary>
public sealed class ImageSourceResolver : IImageSourceResolver
{
    private readonly IHostAddressResolver hostAddressResolver;

    /// <summary>Initializes a resolver with operating-system DNS.</summary>
    public ImageSourceResolver()
        : this(new SystemHostAddressResolver())
    {
    }

    /// <summary>Initializes a resolver with an explicit DNS resolver.</summary>
    public ImageSourceResolver(IHostAddressResolver hostAddressResolver)
    {
        this.hostAddressResolver = hostAddressResolver
            ?? throw new ArgumentNullException(nameof(hostAddressResolver));
    }

    /// <inheritdoc />
    public async Task<ImageSource> ResolveAsync(
        ImageSource source,
        string markdownFilePath,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(markdownFilePath);
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(source.Value))
        {
            throw Error(PublishErrorCodes.ImageSourceEmpty, "Image source is required.");
        }

        return source switch
        {
            LocalImageSource local => ResolveLocal(local, markdownFilePath),
            RemoteImageSource remote => await ResolveRemoteAsync(remote, cancellationToken)
                .ConfigureAwait(false),
            _ => throw Error(PublishErrorCodes.ImagePathInvalid, "Image source type is invalid."),
        };
    }

    private static LocalImageSource ResolveLocal(
        LocalImageSource source,
        string markdownFilePath)
    {
        string fullPath;
        try
        {
            var markdownFullPath = Path.GetFullPath(markdownFilePath);
            var baseDirectory = Path.GetDirectoryName(markdownFullPath)
                ?? throw new InvalidOperationException("Markdown path has no parent directory.");
            fullPath = Path.GetFullPath(source.Path, baseDirectory);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or
            PathTooLongException or IOException or UnauthorizedAccessException)
        {
            throw Error(
                PublishErrorCodes.ImagePathInvalid,
                "Local image path is invalid.",
                exception);
        }

        if (!File.Exists(fullPath))
        {
            throw Error(PublishErrorCodes.ImageFileNotFound, "Local image file was not found.");
        }

        var expectedMimeType = Path.GetExtension(fullPath).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => throw Error(
                PublishErrorCodes.ImageFormatNotSupported,
                "Only PNG, JPEG, and GIF image files are supported."),
        };

        try
        {
            using var stream = File.OpenRead(fullPath);
            Span<byte> signature = stackalloc byte[8];
            var count = stream.Read(signature);
            var detectedMimeType = DetectMimeType(signature[..count]);
            if (!string.Equals(expectedMimeType, detectedMimeType, StringComparison.Ordinal))
            {
                throw Error(
                    PublishErrorCodes.ImageFormatNotSupported,
                    "Image file extension does not match its file signature.");
            }
        }
        catch (PublishPipelineException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw Error(
                PublishErrorCodes.ImagePathInvalid,
                "Local image file could not be inspected.",
                exception);
        }

        return new LocalImageSource(fullPath);
    }

    private async Task<RemoteImageSource> ResolveRemoteAsync(
        RemoteImageSource source,
        CancellationToken cancellationToken)
    {
        var uri = source.Uri;
        if (!uri.IsAbsoluteUri ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw Error(
                PublishErrorCodes.ImageRemoteUriInvalid,
                "Remote image source must be an absolute HTTP or HTTPS URI.");
        }

        if (!string.IsNullOrEmpty(uri.UserInfo) || string.IsNullOrWhiteSpace(uri.IdnHost))
        {
            throw Error(
                PublishErrorCodes.ImageRemoteUriInvalid,
                "Remote image URI must not contain credentials and must include a host.");
        }

        var host = uri.IdnHost;
        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
            host.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase))
        {
            throw HostNotAllowed();
        }

        IPAddress[] addresses;
        if (IPAddress.TryParse(host, out var literalAddress))
        {
            addresses = [literalAddress];
        }
        else
        {
            try
            {
                addresses = await hostAddressResolver.GetHostAddressesAsync(host, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception) when (exception is System.Net.Sockets.SocketException or
                ArgumentException)
            {
                throw Error(
                    PublishErrorCodes.ImageUriResolutionFailed,
                    "Remote image host could not be resolved.",
                    exception);
            }
        }

        if (addresses.Length == 0)
        {
            throw Error(
                PublishErrorCodes.ImageUriResolutionFailed,
                "Remote image host resolved to no addresses.");
        }

        if (addresses.Any(IsDisallowedAddress))
        {
            throw HostNotAllowed();
        }

        return new RemoteImageSource(uri);
    }

    internal static string? DetectMimeType(ReadOnlySpan<byte> signature)
    {
        ReadOnlySpan<byte> png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        if (signature.StartsWith(png))
        {
            return "image/png";
        }

        if (signature.Length >= 3 && signature[0] == 0xFF &&
            signature[1] == 0xD8 && signature[2] == 0xFF)
        {
            return "image/jpeg";
        }

        if (signature.Length >= 6 &&
            (signature[..6].SequenceEqual("GIF87a"u8) || signature[..6].SequenceEqual("GIF89a"u8)))
        {
            return "image/gif";
        }

        return null;
    }

    private static bool IsDisallowedAddress(IPAddress address)
    {
        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }

        if (IPAddress.IsLoopback(address) || address.Equals(IPAddress.Any) ||
            address.Equals(IPAddress.IPv6Any) || address.Equals(IPAddress.None) ||
            address.IsIPv6LinkLocal || address.IsIPv6SiteLocal)
        {
            return true;
        }

        var bytes = address.GetAddressBytes();
        if (bytes.Length == 4)
        {
            return bytes[0] == 10 ||
                (bytes[0] == 172 && bytes[1] is >= 16 and <= 31) ||
                (bytes[0] == 192 && bytes[1] == 168) ||
                (bytes[0] == 169 && bytes[1] == 254);
        }

        return bytes.Length == 16 && (bytes[0] & 0xFE) == 0xFC;
    }

    private static PublishPipelineException HostNotAllowed() => Error(
        PublishErrorCodes.ImageRemoteHostNotAllowed,
        "Remote image host is not allowed.");

    private static PublishPipelineException Error(
        string code,
        string message,
        Exception? innerException = null) => new(code, message, innerException);
}
