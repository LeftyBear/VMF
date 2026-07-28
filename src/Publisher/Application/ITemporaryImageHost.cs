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
        : this(resourceId, publicUri, contentHash: null, publisherOwned: true)
    {
    }

    /// <summary>Initializes a temporary hosted image.</summary>
    public TemporaryHostedImage(
        string resourceId,
        Uri publicUri,
        string? contentHash,
        bool publisherOwned,
        DateTimeOffset? expiresAtUtc = null)
    {
        ResourceId = string.IsNullOrWhiteSpace(resourceId)
            ? throw new ArgumentException("Resource ID is required.", nameof(resourceId))
            : resourceId;
        PublicUri = publicUri ?? throw new ArgumentNullException(nameof(publicUri));
        ContentHash = contentHash;
        PublisherOwned = publisherOwned;
        ExpiresAtUtc = expiresAtUtc;
    }

    /// <summary>Gets the opaque host resource identifier.</summary>
    public string ResourceId { get; }

    /// <summary>Gets the URI used only for the insertion request.</summary>
    public Uri PublicUri { get; }

    /// <summary>Gets the uploaded binary content hash, when known.</summary>
    public string? ContentHash { get; }

    /// <summary>Gets a value indicating whether Publisher may delete this artifact.</summary>
    public bool PublisherOwned { get; }

    /// <summary>Gets the URI expiry time, when the host can provide it.</summary>
    public DateTimeOffset? ExpiresAtUtc { get; }
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
