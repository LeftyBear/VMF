using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Parses Markdown source into a publisher-neutral document model.</summary>
public interface IMarkdownParser
{
    /// <summary>Parses Markdown source.</summary>
    /// <param name="markdown">The Markdown text.</param>
    /// <returns>The parsed document model.</returns>
    DocumentModel Parse(string markdown);
}
