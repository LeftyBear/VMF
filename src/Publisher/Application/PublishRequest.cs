namespace Vmf.Publisher.Application;

/// <summary>Describes a Markdown publication request.</summary>
public sealed class PublishRequest
{
    /// <summary>Initializes a publication request.</summary>
    /// <param name="markdownFilePath">The Markdown source file path.</param>
    /// <param name="documentTitle">An optional Google Docs title.</param>
    public PublishRequest(string markdownFilePath, string? documentTitle = null)
    {
        MarkdownFilePath = markdownFilePath;
        DocumentTitle = documentTitle;
    }

    /// <summary>Gets the Markdown source file path.</summary>
    public string MarkdownFilePath { get; }

    /// <summary>Gets the optional destination document title.</summary>
    public string? DocumentTitle { get; }
}
