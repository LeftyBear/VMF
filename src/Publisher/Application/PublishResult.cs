namespace Vmf.Publisher.Application;

/// <summary>Represents the outcome of a publication request.</summary>
public sealed class PublishResult
{
    private PublishResult(bool isSuccess, string? documentId, string? documentUrl, PublishError? error)
    {
        IsSuccess = isSuccess;
        DocumentId = documentId;
        DocumentUrl = documentUrl;
        Error = error;
    }

    /// <summary>Gets a value indicating whether publication succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the published Google document identifier, when successful.</summary>
    public string? DocumentId { get; }

    /// <summary>Gets the published Google document URL, when successful.</summary>
    public string? DocumentUrl { get; }

    /// <summary>Gets failure information, when unsuccessful.</summary>
    public PublishError? Error { get; }

    /// <summary>Creates a successful result.</summary>
    /// <param name="documentId">The Google document identifier.</param>
    /// <param name="documentUrl">The Google document URL.</param>
    /// <returns>A successful result.</returns>
    public static PublishResult Success(string documentId, string documentUrl) =>
        new(true, documentId, documentUrl, null);

    /// <summary>Creates a failed result.</summary>
    /// <param name="error">The publication error.</param>
    /// <returns>A failed result.</returns>
    public static PublishResult Failure(PublishError error) => new(false, null, null, error);
}
