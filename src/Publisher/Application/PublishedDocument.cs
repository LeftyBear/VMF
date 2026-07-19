namespace Vmf.Publisher.Application;

/// <summary>Identifies a published Google document.</summary>
public sealed class PublishedDocument
{
    /// <summary>Initializes published document information.</summary>
    /// <param name="id">The Google document identifier.</param>
    /// <param name="url">The Google document URL.</param>
    public PublishedDocument(string id, string url)
    {
        Id = id;
        Url = url;
    }

    /// <summary>Gets the Google document identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the Google document URL.</summary>
    public string Url { get; }
}
