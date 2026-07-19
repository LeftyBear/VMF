using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Publishes a compiled document to Google Docs.</summary>
public interface IGoogleDocsPublisher
{
    /// <summary>Publishes a compiled document.</summary>
    /// <param name="document">The compiled document.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>The created Google document.</returns>
    Task<PublishedDocument> PublishAsync(CompiledDocument document, CancellationToken cancellationToken);
}
