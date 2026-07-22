using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Applies and verifies one target-neutral differential plan.</summary>
public interface IPublishPlanApplicationVerifier
{
    /// <summary>Reads and validates the current snapshot before logical planning.</summary>
    Task<ManagedDocumentSnapshot> PrepareAsync(
        PublishCandidate candidate,
        VerifiedPublishState? baseline,
        CancellationToken cancellationToken);

    /// <summary>Applies the plan externally and returns readback verification evidence.</summary>
    Task<PublishApplicationVerification> ApplyAndVerifyAsync(
        PublishCandidate candidate,
        VerifiedPublishState? baseline,
        DiffPlan plan,
        ManagedDocumentSnapshot preparedSnapshot,
        CancellationToken cancellationToken);

    /// <summary>Creates a non-mutating physical-plan preview.</summary>
    PhysicalUpdateDryRunResult CreateDryRun(
        PublishCandidate candidate,
        VerifiedPublishState? baseline,
        DiffPlan plan,
        ManagedDocumentSnapshot preparedSnapshot);
}

/// <summary>Represents a lifecycle operation that became durable.</summary>
public sealed class VerifiedPublishLifecycleResult
{
    internal VerifiedPublishLifecycleResult(DiffPlan plan, VerifiedPublishState state)
    {
        Plan = plan;
        State = state;
    }

    /// <summary>Gets the applied logical plan.</summary>
    public DiffPlan Plan { get; }

    /// <summary>Gets the state saved before success was returned.</summary>
    public VerifiedPublishState State { get; }
}

/// <summary>Coordinates restore, plan, external application, verification, promotion, and commit.</summary>
public interface IVerifiedPublishLifecycle
{
    /// <summary>Executes one complete verified-state lifecycle operation.</summary>
    Task<VerifiedPublishLifecycleResult> ExecuteAsync(
        PublishCandidate candidate,
        CancellationToken cancellationToken);

    /// <summary>Builds and validates plans without applying or saving state.</summary>
    Task<PhysicalUpdateDryRunResult> DryRunAsync(
        PublishCandidate candidate,
        CancellationToken cancellationToken);
}

/// <summary>Ensures state becomes successful only after an atomic verified-state save.</summary>
public sealed class VerifiedPublishLifecycle : IVerifiedPublishLifecycle
{
    private readonly IVerifiedPublishStateReader reader;
    private readonly IVerifiedPublishStateWriter writer;
    private readonly IDiffEngine diffEngine;
    private readonly IPublishPlanApplicationVerifier applicationVerifier;
    private readonly IPublishResultVerifier resultVerifier;
    private readonly IVerifiedPublishStatePromoter promoter;

    /// <summary>Initializes the verified lifecycle coordinator.</summary>
    public VerifiedPublishLifecycle(
        IVerifiedPublishStateReader reader,
        IVerifiedPublishStateWriter writer,
        IDiffEngine diffEngine,
        IPublishPlanApplicationVerifier applicationVerifier,
        IPublishResultVerifier resultVerifier,
        IVerifiedPublishStatePromoter promoter)
    {
        this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        this.diffEngine = diffEngine ?? throw new ArgumentNullException(nameof(diffEngine));
        this.applicationVerifier = applicationVerifier
            ?? throw new ArgumentNullException(nameof(applicationVerifier));
        this.resultVerifier = resultVerifier ?? throw new ArgumentNullException(nameof(resultVerifier));
        this.promoter = promoter ?? throw new ArgumentNullException(nameof(promoter));
    }

    /// <inheritdoc />
    public async Task<VerifiedPublishLifecycleResult> ExecuteAsync(
        PublishCandidate candidate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        cancellationToken.ThrowIfCancellationRequested();

        var request = new PublishStateLoadRequest(
            new PublishStateKey(candidate.Identity.PublicationId, candidate.Identity.DocumentId),
            candidate.Identity.GoogleDocumentId);
        var baseline = await reader.LoadAsync(request, cancellationToken).ConfigureAwait(false);
        if (!DocumentStateTransitionRules.IsAllowed(baseline?.Identity.State, DocumentState.Active))
        {
            throw VerifiedPublishStatePromoter.InvalidTransition(
                baseline?.Identity.State,
                DocumentState.Active);
        }

        var preparedSnapshot = await applicationVerifier
            .PrepareAsync(candidate, baseline, cancellationToken)
            .ConfigureAwait(false);
        var plan = diffEngine.CreatePlan(baseline, candidate);
        var applicationResult = await applicationVerifier
            .ApplyAndVerifyAsync(candidate, baseline, plan, preparedSnapshot, cancellationToken)
            .ConfigureAwait(false);
        var verifiedResult = resultVerifier.Verify(candidate, plan, applicationResult);
        var state = promoter.Promote(baseline, verifiedResult);
        await writer.SaveAsync(state, cancellationToken).ConfigureAwait(false);
        return new VerifiedPublishLifecycleResult(plan, state);
    }

    /// <inheritdoc />
    public async Task<PhysicalUpdateDryRunResult> DryRunAsync(
        PublishCandidate candidate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        cancellationToken.ThrowIfCancellationRequested();
        var request = new PublishStateLoadRequest(
            new PublishStateKey(candidate.Identity.PublicationId, candidate.Identity.DocumentId),
            candidate.Identity.GoogleDocumentId);
        var baseline = await reader.LoadAsync(request, cancellationToken).ConfigureAwait(false);
        if (!DocumentStateTransitionRules.IsAllowed(baseline?.Identity.State, DocumentState.Active))
        {
            throw VerifiedPublishStatePromoter.InvalidTransition(
                baseline?.Identity.State,
                DocumentState.Active);
        }

        ManagedDocumentSnapshot preparedSnapshot;
        try
        {
            preparedSnapshot = await applicationVerifier
                .PrepareAsync(candidate, baseline, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (PhysicalUpdateException exception)
        {
            return new PhysicalUpdateDryRunResult(null, null, [exception.Code]);
        }

        var plan = diffEngine.CreatePlan(baseline, candidate);
        return applicationVerifier.CreateDryRun(candidate, baseline, plan, preparedSnapshot);
    }
}
