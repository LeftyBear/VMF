using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

// This path-based guard reduces exposure but cannot close ancestor replacement races.
internal sealed class PreviewUpdateLocalImageReader
{
    private readonly PreviewUpdateImagePathGuard guard;
    private readonly Func<string, Stream> openRead;

    internal PreviewUpdateLocalImageReader(PreviewUpdateImagePathGuard guard,
        Func<string, Stream>? openRead = null)
    {
        this.guard = guard;
        this.openRead = openRead ?? File.OpenRead;
    }

    internal async Task<ImageMetadata> ReadAsync(LocalImageSource source, string markdownPath,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(source.Path))
        {
            throw Error(PublishErrorCodes.ImageSourceEmpty, "Image source is required.");
        }

        guard.Validate(source.Path, markdownPath);
        var path = Path.GetFullPath(source.Path, Path.GetDirectoryName(Path.GetFullPath(markdownPath))!);
        if (!File.Exists(path))
        {
            throw Error(PublishErrorCodes.ImageFileNotFound, "Local image file was not found.");
        }

        var expectedMimeType = Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => throw Error(PublishErrorCodes.ImageFormatNotSupported,
                "Only PNG, JPEG, and GIF image files are supported."),
        };

        byte[] bytes;
        var signatureValidated = false;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            guard.ValidateResolved(path, markdownPath);
            using var stream = openRead(path);
            using var buffer = new MemoryStream();
            var signature = new byte[8];
            var count = 0;
            while (count < signature.Length)
            {
                var read = await stream.ReadAsync(signature.AsMemory(count), cancellationToken)
                    .ConfigureAwait(false);
                if (read == 0) { break; }
                count += read;
            }

            if (!string.Equals(expectedMimeType,
                ImageSourceResolver.DetectMimeType(signature.AsSpan(0, count)), StringComparison.Ordinal))
            {
                throw Error(PublishErrorCodes.ImageFormatNotSupported,
                    "Image file extension does not match its file signature.");
            }

            signatureValidated = true;
            buffer.Write(signature, 0, count);
            await stream.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);
            bytes = buffer.ToArray();
        }
        catch (OperationCanceledException) { throw; }
        catch (PublishPipelineException) { throw; }
        catch (Exception exception) when (!signatureValidated &&
            exception is IOException or UnauthorizedAccessException)
        {
            throw Error(PublishErrorCodes.ImagePathInvalid,
                "Local image file could not be inspected.", exception);
        }
        catch (Exception exception)
        {
            throw Error(PublishErrorCodes.ImageMetadataReadFailed,
                "Image metadata could not be read.", exception);
        }

        try
        {
            return ImageMetadataParser.Parse(bytes, new LocalImageSource(path));
        }
        catch (PublishPipelineException) { throw; }
        catch (Exception exception)
        {
            throw Error(PublishErrorCodes.ImageMetadataReadFailed,
                "Image metadata could not be read.", exception);
        }
    }

    private static PublishPipelineException Error(string code, string message, Exception? inner = null) =>
        new(code, message, inner);
}
