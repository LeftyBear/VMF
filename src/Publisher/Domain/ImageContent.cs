namespace Vmf.Publisher.Domain;

/// <summary>Represents the source of an image in Markdown.</summary>
public abstract class ImageSource
{
    private protected ImageSource(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets the source value.</summary>
    public string Value { get; }
}

/// <summary>Represents an image stored on the local file system.</summary>
public sealed class LocalImageSource : ImageSource
{
    /// <summary>Initializes a local image source.</summary>
    public LocalImageSource(string path)
        : base(path)
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
        : base((uri ?? throw new ArgumentNullException(nameof(uri))).OriginalString)
    {
        Uri = uri;
    }

    /// <summary>Gets the remote URI.</summary>
    public Uri Uri { get; }
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

/// <summary>Represents one standalone Markdown image.</summary>
public sealed class ImageBlock
{
    /// <summary>Initializes an image block.</summary>
    public ImageBlock(string altText, ImageSource source, ImageSize? size = null)
    {
        AltText = altText ?? throw new ArgumentNullException(nameof(altText));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Size = size;
    }

    /// <summary>Gets the Markdown alternative text.</summary>
    public string AltText { get; }

    /// <summary>Gets the image source.</summary>
    public ImageSource Source { get; }

    /// <summary>Gets the calculated size, when image preparation has completed.</summary>
    public ImageSize? Size { get; }
}
