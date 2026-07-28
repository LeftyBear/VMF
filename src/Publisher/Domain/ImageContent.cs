namespace Vmf.Publisher.Domain;

/// <summary>Identifies the source category of a canonical image.</summary>
public enum ImageSourceKind
{
    /// <summary>A local file system image.</summary>
    LocalFile,

    /// <summary>An HTTP or HTTPS image URI.</summary>
    RemoteUri,

    /// <summary>An existing Google Drive image artifact.</summary>
    GoogleDrive,

    /// <summary>An embedded image resource.</summary>
    EmbeddedResource,
}

/// <summary>Represents the source of an image in Markdown.</summary>
public abstract class ImageSource
{
    private protected ImageSource(ImageSourceKind kind, string value)
    {
        Kind = kind;
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets the source kind.</summary>
    public ImageSourceKind Kind { get; }

    /// <summary>Gets the source value.</summary>
    public string Value { get; }

    /// <summary>Gets the canonical source key used after stronger image identities.</summary>
    public virtual string SourceKey => $"{Kind}:{Value}";
}

/// <summary>Represents an image stored on the local file system.</summary>
public sealed class LocalImageSource : ImageSource
{
    /// <summary>Initializes a local image source.</summary>
    public LocalImageSource(string path)
        : base(ImageSourceKind.LocalFile, path)
    {
    }

    /// <summary>Gets the local path.</summary>
    public string Path => Value;
}

/// <summary>Represents an image available through a remote URI.</summary>
public sealed class RemoteImageSource : ImageSource
{
    /// <summary>Initializes a remote image source.</summary>
    public RemoteImageSource(Uri uri)
        : base(ImageSourceKind.RemoteUri, (uri ?? throw new ArgumentNullException(nameof(uri))).OriginalString)
    {
        Uri = uri;
    }

    /// <summary>Gets the remote URI.</summary>
    public Uri Uri { get; }

    /// <inheritdoc />
    public override string SourceKey => $"{Kind}:{Uri.AbsoluteUri}";
}

/// <summary>Represents an image already available as a Google Drive artifact.</summary>
public sealed class GoogleDriveImageSource : ImageSource
{
    /// <summary>Initializes a Google Drive image source.</summary>
    public GoogleDriveImageSource(string fileId, Uri publicUri, bool publisherOwned)
        : base(ImageSourceKind.GoogleDrive, fileId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileId);
        FileId = fileId;
        PublicUri = publicUri ?? throw new ArgumentNullException(nameof(publicUri));
        PublisherOwned = publisherOwned;
    }

    /// <summary>Gets the Drive file identifier.</summary>
    public string FileId { get; }

    /// <summary>Gets the public URI used by Google Docs insertion.</summary>
    public Uri PublicUri { get; }

    /// <summary>Gets a value indicating whether Publisher owns cleanup for this artifact.</summary>
    public bool PublisherOwned { get; }

    /// <inheritdoc />
    public override string SourceKey => $"{Kind}:{FileId}";
}

/// <summary>Represents an embedded image resource decoded before publication.</summary>
public sealed class EmbeddedImageSource : ImageSource
{
    /// <summary>Initializes an embedded image source.</summary>
    public EmbeddedImageSource(string resourceName, byte[] content, string mimeType)
        : base(ImageSourceKind.EmbeddedResource, resourceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);
        ArgumentNullException.ThrowIfNull(content);
        ResourceName = resourceName;
        Content = Array.AsReadOnly(content.ToArray());
        MimeType = mimeType;
    }

    /// <summary>Gets the resource name.</summary>
    public string ResourceName { get; }

    /// <summary>Gets immutable image bytes.</summary>
    public IReadOnlyList<byte> Content { get; }

    /// <summary>Gets the MIME type.</summary>
    public string MimeType { get; }

    /// <inheritdoc />
    public override string SourceKey => $"{Kind}:{ResourceName}";
}

/// <summary>Identifies one canonical image independently from document position.</summary>
public sealed class ImageIdentity
{
    /// <summary>Initializes image identity.</summary>
    public ImageIdentity(string? stableId, string contentHash, string sourceKey)
    {
        if (stableId is not null && string.IsNullOrWhiteSpace(stableId))
        {
            throw new ArgumentException("A stable image ID must not be empty when supplied.", nameof(stableId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceKey);
        StableId = stableId;
        ContentHash = contentHash;
        SourceKey = sourceKey;
    }

    /// <summary>Gets the optional stable image ID.</summary>
    public string? StableId { get; }

    /// <summary>Gets the SHA-256 image content hash.</summary>
    public string ContentHash { get; }

    /// <summary>Gets the source key.</summary>
    public string SourceKey { get; }
}

/// <summary>Represents a canonical SHA-256 image content hash.</summary>
public sealed class ImageContentHash : IEquatable<ImageContentHash>
{
    /// <summary>The current image-content-hash value prefix.</summary>
    public const string ValuePrefix = "image-v1:sha256:";

    /// <summary>Initializes an image content hash.</summary>
    public ImageContentHash(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Length != ValuePrefix.Length + 64 ||
            !value.StartsWith(ValuePrefix, StringComparison.Ordinal) ||
            value.AsSpan(ValuePrefix.Length).ContainsAnyExcept("0123456789abcdef"))
        {
            throw new ArgumentException(
                "An image content hash must use image-v1:sha256 followed by 64 lowercase hexadecimal characters.",
                nameof(value));
        }

        Value = value;
    }

    /// <summary>Gets the canonical hash value.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public bool Equals(ImageContentHash? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ImageContentHash);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
}

/// <summary>Represents image presentation independent from binary content.</summary>
public sealed class ImagePresentation
{
    /// <summary>Initializes image presentation.</summary>
    public ImagePresentation(string altText, ImageSize? size)
    {
        AltText = altText ?? throw new ArgumentNullException(nameof(altText));
        Size = size;
    }

    /// <summary>Gets the alternative text.</summary>
    public string AltText { get; }

    /// <summary>Gets the calculated display size.</summary>
    public ImageSize? Size { get; }
}

/// <summary>Represents a canonical image for image-level diffing.</summary>
public sealed class CanonicalImage
{
    /// <summary>Initializes a canonical image.</summary>
    public CanonicalImage(ImageIdentity identity, ImageSource source, ImagePresentation presentation)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Presentation = presentation ?? throw new ArgumentNullException(nameof(presentation));
    }

    /// <summary>Gets image identity.</summary>
    public ImageIdentity Identity { get; }

    /// <summary>Gets image source.</summary>
    public ImageSource Source { get; }

    /// <summary>Gets image presentation.</summary>
    public ImagePresentation Presentation { get; }
}

/// <summary>Represents an image size in Google Docs points.</summary>
public sealed class ImageSize
{
    /// <summary>Initializes an image size.</summary>
    public ImageSize(double widthPoints, double heightPoints)
    {
        if (!double.IsFinite(widthPoints) || widthPoints <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(widthPoints));
        }

        if (!double.IsFinite(heightPoints) || heightPoints <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heightPoints));
        }

        WidthPoints = widthPoints;
        HeightPoints = heightPoints;
    }

    /// <summary>Gets the width in points.</summary>
    public double WidthPoints { get; }

    /// <summary>Gets the height in points.</summary>
    public double HeightPoints { get; }
}

/// <summary>Contains decoded image metadata.</summary>
public sealed class ImageMetadata
{
    /// <summary>Initializes image metadata.</summary>
    public ImageMetadata(
        ImageSource source,
        int pixelWidth,
        int pixelHeight,
        double horizontalDpi,
        double verticalDpi,
        string mimeType,
        string? contentHash = null)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        PixelWidth = pixelWidth;
        PixelHeight = pixelHeight;
        HorizontalDpi = horizontalDpi;
        VerticalDpi = verticalDpi;
        MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
        if (contentHash is not null)
        {
            _ = new ImageContentHash(contentHash);
        }

        ContentHash = contentHash;
    }

    /// <summary>Gets the final validated source, including any safe redirect target.</summary>
    public ImageSource Source { get; }

    /// <summary>Gets the pixel width.</summary>
    public int PixelWidth { get; }

    /// <summary>Gets the pixel height.</summary>
    public int PixelHeight { get; }

    /// <summary>Gets the horizontal DPI.</summary>
    public double HorizontalDpi { get; }

    /// <summary>Gets the vertical DPI.</summary>
    public double VerticalDpi { get; }

    /// <summary>Gets the detected MIME type.</summary>
    public string MimeType { get; }

    /// <summary>Gets the binary content hash, when metadata was read from bytes.</summary>
    public string? ContentHash { get; }
}

/// <summary>Represents one standalone Markdown image.</summary>
public sealed class ImageBlock
{
    /// <summary>Initializes an image block.</summary>
    public ImageBlock(
        string altText,
        ImageSource source,
        ImageSize? size = null,
        string? contentHash = null,
        string? stableId = null)
    {
        AltText = altText ?? throw new ArgumentNullException(nameof(altText));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Size = size;
        if (contentHash is not null)
        {
            _ = new ImageContentHash(contentHash);
        }

        if (stableId is not null && string.IsNullOrWhiteSpace(stableId))
        {
            throw new ArgumentException("A stable image ID must not be empty when supplied.", nameof(stableId));
        }

        ContentHash = contentHash;
        StableId = stableId;
    }

    /// <summary>Gets the Markdown alternative text.</summary>
    public string AltText { get; }

    /// <summary>Gets the image source.</summary>
    public ImageSource Source { get; }

    /// <summary>Gets the calculated size, when image preparation has completed.</summary>
    public ImageSize? Size { get; }

    /// <summary>Gets the SHA-256 binary content hash, when source resolution has completed.</summary>
    public string? ContentHash { get; }

    /// <summary>Gets the stable image identifier, when supplied by a higher-level boundary.</summary>
    public string? StableId { get; }

    /// <summary>Gets the presentation-only image attributes.</summary>
    public ImagePresentation Presentation => new(AltText, Size);
}
