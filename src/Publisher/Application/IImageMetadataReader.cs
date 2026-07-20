using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Reads dimensions and format metadata from an image.</summary>
public interface IImageMetadataReader
{
    /// <summary>Reads metadata from a validated image source.</summary>
    Task<ImageMetadata> ReadAsync(ImageSource source, CancellationToken cancellationToken);
}

/// <summary>Calculates a target image size from decoded metadata.</summary>
public interface IImageSizeCalculator
{
    /// <summary>Calculates width and height while preserving aspect ratio.</summary>
    ImageSize Calculate(ImageMetadata metadata);
}

/// <summary>Contains target-neutral Publisher behavior settings.</summary>
public sealed class PublisherOptions
{
    /// <summary>Gets or sets whether local images may be temporarily public.</summary>
    public bool AllowTemporaryPublicImageHosting { get; set; }

    /// <summary>Gets or sets the maximum image width in points.</summary>
    public double ImageMaxWidthPoints { get; set; } = 450;

    /// <summary>Gets or sets whether images smaller than the maximum may be enlarged.</summary>
    public bool AllowImageUpscale { get; set; }
}
