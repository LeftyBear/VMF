using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Calculates a Docs image size while preserving pixel aspect ratio.</summary>
public sealed class ImageSizeCalculator
{
    private readonly PublisherOptions options;

    /// <summary>Initializes an image-size calculator.</summary>
    public ImageSizeCalculator(PublisherOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>Calculates an image size in points.</summary>
    public ImageSize Calculate(ImageMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        if (metadata.PixelWidth <= 0 || metadata.PixelHeight <= 0 ||
            !double.IsFinite(metadata.HorizontalDpi) || metadata.HorizontalDpi <= 0 ||
            !double.IsFinite(metadata.VerticalDpi) || metadata.VerticalDpi <= 0 ||
            !double.IsFinite(options.ImageMaxWidthPoints) || options.ImageMaxWidthPoints <= 0)
        {
            throw new PublishPipelineException(
                PublishErrorCodes.ImageSizeInvalid,
                "Image dimensions, DPI, and maximum width must be positive finite values.");
        }

        var naturalWidth = metadata.PixelWidth * 72d / metadata.HorizontalDpi;
        var naturalHeight = naturalWidth * metadata.PixelHeight / metadata.PixelWidth;
        var scale = options.AllowImageUpscale
            ? options.ImageMaxWidthPoints / naturalWidth
            : Math.Min(1d, options.ImageMaxWidthPoints / naturalWidth);
        var width = naturalWidth * scale;
        var height = naturalHeight * scale;
        try
        {
            return new ImageSize(width, height);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            throw new PublishPipelineException(
                PublishErrorCodes.ImageSizeInvalid,
                "Calculated image size is invalid.",
                exception);
        }
    }
}
