using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Parses Markdown inline content independently of block structure.</summary>
public interface IMarkdownInlineParser
{
    /// <summary>Parses one Markdown inline source.</summary>
    /// <param name="markdown">The Markdown inline source.</param>
    /// <returns>The parsed inline content.</returns>
    IReadOnlyList<InlineContent> Parse(string markdown);
}
