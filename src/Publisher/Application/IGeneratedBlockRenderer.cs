using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Renders document blocks into an ordered publish-step plan.</summary>
public interface IGeneratedBlockRenderer
{
    /// <summary>Renders all blocks in source order.</summary>
    /// <param name="document">The publisher-neutral document.</param>
    /// <returns>The ordered publish steps.</returns>
    IReadOnlyList<PublishStep> Render(DocumentModel document);
}
