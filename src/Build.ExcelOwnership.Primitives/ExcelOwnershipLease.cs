namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed class ExcelOwnershipLease : IDisposable
{
    private readonly ExcelOwnershipSession session;

    public Ownership? Identity => session.Identity;
    public ActivationProcessIdentity? ActivationProcessIdentity => session.ActivationProcessIdentity;
    public ApplicationProcessIdentity? ApplicationProcessIdentity => session.ApplicationProcessIdentity;
    public ApplicationIdentityValidation? ApplicationIdentityValidation => session.ApplicationIdentityValidation;
    public CleanupTopologyValidation? CleanupTopologyValidation => session.CleanupTopologyValidation;
    public ProcessIdentityObservation? ActivationProcessReobservation => session.ActivationProcessReobservation;
    public ProcessIdentityObservation? ApplicationProcessReobservation => session.ApplicationProcessReobservation;
    public bool CleanupTopologyValidated => session.CleanupTopologyValidated;
    public bool AttachmentValidated => session.AttachmentValidated;
    public bool CleanupEligible => session.CleanupEligible;
    public bool CleanupCompleted => session.Clean;
    public ResidualState ActivationProcessResidualState => session.ActivationProcessResidualState;
    public ResidualState ApplicationProcessResidualState => session.ApplicationProcessResidualState;
    public int? RotExactMatchCount => session.RotExactMatchCount;
    public ResidualObservationTelemetry? ResidualObservationTelemetry => session.ResidualObservationTelemetry;
    public CleanupTimingTelemetry CleanupTimingTelemetry => session.CleanupTimingTelemetry;
    public ResidualState ResidualState => session.ActivationProcessResidualState == ResidualState.Clear && session.ApplicationProcessResidualState == ResidualState.Clear
        ? ResidualState.Clear
        : session.ApplicationProcessResidualState != ResidualState.NotApplicable ? session.ApplicationProcessResidualState : session.ActivationProcessResidualState;

    internal ExcelOwnershipSession Session => session;

    public ExcelOwnershipLease(LifecycleJournal journal, Guid invocationId)
    {
        session = new ExcelOwnershipSession(journal, invocationId);
    }

    public void Start(string sentinelPath)
    {
        session.Start(sentinelPath);
    }

    [Obsolete("Use ConfirmOwnershipEvidencePersisted with a readback confirmation.")]
    public void ConfirmIdentityEvidencePersisted() => throw new InvalidOperationException("A durable ownership-evidence confirmation is required.");

    public void ConfirmOwnershipEvidencePersisted(EvidencePersistenceConfirmation confirmation) => session.ConfirmOwnershipEvidencePersisted(confirmation);

    public CleanupTopologyValidation ObserveCleanupTopology() => session.ObserveCleanupTopology();

    public void ConfirmCleanupTopologyEvidencePersisted(EvidencePersistenceConfirmation confirmation) => session.ConfirmCleanupTopologyEvidencePersisted(confirmation);

    public void RequestNormalCleanup() => Dispose();

    public void Dispose() => session.Dispose();
}
