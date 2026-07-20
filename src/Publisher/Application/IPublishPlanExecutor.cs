using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Executes an ordered publish-step plan against an existing document.</summary>
public interface IPublishPlanExecutor
{
    /// <summary>Executes all steps and stops at the first failure.</summary>
    /// <param name="documentId">The target Google document identifier.</param>
    /// <param name="steps">The ordered publish steps.</param>
    /// <param name="cancellationToken">A token that cancels execution.</param>
    /// <returns>A task that completes after all steps.</returns>
    Task ExecuteAsync(
        string documentId,
        IReadOnlyList<PublishStep> steps,
        CancellationToken cancellationToken);
}
