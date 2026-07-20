using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Temporarily exposes local images to Google Docs.</summary>
public interface ITemporaryImageHost
{
    /// <summary>Uploads and temporarily publishes a local image.</summary>
    Task<TemporaryHostedImage> HostAsync(
        LocalImageSource source,
        CancellationToken cancellationToken);

    /// <summary>Deletes a previously hosted temporary image.</summary>
    Task DeleteAsync(TemporaryHostedImage image, CancellationToken cancellationToken);
}

/// <summary>Represents a temporary public image lease.</summary>
public sealed class TemporaryHostedImage
{
    /// <summary>Initializes a temporary hosted image.</summary>
    public TemporaryHostedImage(string resourceId, Uri publicUri)
    {
        ResourceId = string.IsNullOrWhiteSpace(resourceId)
            ? throw new ArgumentException("Resource ID is required.", nameof(resourceId))
            : resourceId;
        PublicUri = publicUri ?? throw new ArgumentNullException(nameof(publicUri));
    }

    /// <summary>Gets the opaque host resource identifier.</summary>
    public string ResourceId { get; }

    /// <summary>Gets the URI used only for the insertion request.</summary>
    public Uri PublicUri { get; }
}

/// <summary>Records safe Publisher diagnostics.</summary>
public interface IPublisherLogger
{
    /// <summary>Records a warning without secrets or image URIs.</summary>
    void Warning(string code, string message);
}

/// <summary>Discards Publisher diagnostics.</summary>
public sealed class NullPublisherLogger : IPublisherLogger
{
    /// <inheritdoc />
    public void Warning(string code, string message)
    {
    }
}
