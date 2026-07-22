using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Identifies a persisted state record without using a storage path.</summary>
public sealed class PublishStateKey
{
    /// <summary>Initializes a state key.</summary>
    public PublishStateKey(string publicationId, string documentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publicationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        PublicationId = publicationId;
        DocumentId = documentId;
    }

    /// <summary>Gets the publication identifier.</summary>
    public string PublicationId { get; }

    /// <summary>Gets the publication-local document identifier.</summary>
    public string DocumentId { get; }
}

/// <summary>Describes identity expectations for restoring verified state.</summary>
public sealed class PublishStateLoadRequest
{
    /// <summary>Initializes a state load request.</summary>
    public PublishStateLoadRequest(PublishStateKey key, string? expectedGoogleDocumentId)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        if (expectedGoogleDocumentId is not null && string.IsNullOrWhiteSpace(expectedGoogleDocumentId))
        {
            throw new ArgumentException(
                "An expected Google document identifier must not be empty when supplied.",
                nameof(expectedGoogleDocumentId));
        }

        ExpectedGoogleDocumentId = expectedGoogleDocumentId;
    }

    /// <summary>Gets the logical state key.</summary>
    public PublishStateKey Key { get; }

    /// <summary>Gets the expected Google document identifier, including an expected null value.</summary>
    public string? ExpectedGoogleDocumentId { get; }
}

/// <summary>Restores validated verified state.</summary>
public interface IVerifiedPublishStateReader
{
    /// <summary>Loads and validates state, or returns null when the key has no record.</summary>
    Task<VerifiedPublishState?> LoadAsync(
        PublishStateLoadRequest request,
        CancellationToken cancellationToken);
}

/// <summary>Persists verified state only after successful verification.</summary>
public interface IVerifiedPublishStateWriter
{
    /// <summary>
    /// Atomically saves a complete record; a failure must leave any prior record intact.
    /// </summary>
    Task SaveAsync(VerifiedPublishState state, CancellationToken cancellationToken);
}

/// <summary>Combines independently consumable verified-state read and write contracts.</summary>
public interface IVerifiedPublishStateStore :
    IVerifiedPublishStateReader,
    IVerifiedPublishStateWriter
{
}
