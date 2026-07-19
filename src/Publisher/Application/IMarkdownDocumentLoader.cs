namespace Vmf.Publisher.Application;

/// <summary>Loads Markdown source text from a document resource.</summary>
public interface IMarkdownDocumentLoader
{
    /// <summary>Loads Markdown source text.</summary>
    /// <param name="path">The source file path.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>The loaded Markdown text.</returns>
    Task<string> LoadAsync(string path, CancellationToken cancellationToken);
}
