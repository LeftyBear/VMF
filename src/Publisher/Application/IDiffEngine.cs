using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Calculates a block-level differential publish plan.</summary>
public interface IDiffEngine
{
    /// <summary>Creates a plan from an optional verified baseline and the desired state.</summary>
    /// <param name="baseline">The verified baseline, or <see langword="null"/> for a new publication.</param>
    /// <param name="candidate">The desired, unverified publish candidate.</param>
    /// <returns>The differential publish plan.</returns>
    DiffPlan CreatePlan(VerifiedPublishState? baseline, PublishCandidate candidate);
}
