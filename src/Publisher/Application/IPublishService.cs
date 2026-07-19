namespace Vmf.Publisher.Application;

/// <summary>Publishes a Markdown document through the configured publication target.</summary>
public interface IPublishService
{
    /// <summary>Publishes the document described by <paramref name="request"/>.</summary>
    /// <param name="request">The publication request.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>The publication result.</returns>
    Task<PublishResult> PublishAsync(PublishRequest request, CancellationToken cancellationToken);
}
