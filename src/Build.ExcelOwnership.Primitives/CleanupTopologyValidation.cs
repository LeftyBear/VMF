namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed record CleanupTopologyValidation(
    int ActivationProcessId,
    int ApplicationProcessId,
    bool ProcessIdsMatch,
    bool ActivationStartTimeStable,
    bool ActivationExecutableStable,
    bool ApplicationStartTimeStable,
    bool ApplicationExecutableStable,
    bool SentinelRevalidated,
    bool InvocationRevalidated,
    DateTimeOffset ObservationStartedUtc,
    DateTimeOffset ObservationCompletedUtc,
    bool AggregateMatch)
{
    public static CleanupTopologyValidation Compare(
        ActivationProcessIdentity activation,
        ApplicationProcessIdentity application,
        ProcessIdentityObservation activationReobservation,
        ProcessIdentityObservation applicationReobservation,
        bool sentinelRevalidated,
        bool invocationRevalidated)
    {
        var processIdsMatch = activation.ProcessId > 0 && activation.ProcessId == application.ProcessId;
        var activationStartStable = activationReobservation.ProcessId == activation.ProcessId &&
            activationReobservation.ProcessStartedUtc == activation.ProcessStartedUtc && activationReobservation.StableIdentity && !activationReobservation.PidReuseSuspected;
        var activationExecutableStable = StringComparer.OrdinalIgnoreCase.Equals(
            activationReobservation.CanonicalExecutablePath, activation.CanonicalExecutablePath);
        var applicationStartStable = applicationReobservation.ProcessId == application.ProcessId &&
            applicationReobservation.ProcessStartedUtc == application.IndependentlyObservedStartTimeUtc && applicationReobservation.StableIdentity && !applicationReobservation.PidReuseSuspected;
        var applicationExecutableStable = StringComparer.OrdinalIgnoreCase.Equals(
            applicationReobservation.CanonicalExecutablePath, application.IndependentlyObservedCanonicalExecutablePath);
        var started = activationReobservation.ObservationStartedUtc <= applicationReobservation.ObservationStartedUtc
            ? activationReobservation.ObservationStartedUtc : applicationReobservation.ObservationStartedUtc;
        var completed = activationReobservation.ObservationCompletedUtc >= applicationReobservation.ObservationCompletedUtc
            ? activationReobservation.ObservationCompletedUtc : applicationReobservation.ObservationCompletedUtc;
        var timestampsValid = started != default && completed != default && completed >= started;
        var aggregate = processIdsMatch && activationStartStable && activationExecutableStable && applicationStartStable &&
            applicationExecutableStable && sentinelRevalidated && invocationRevalidated && timestampsValid;
        return new CleanupTopologyValidation(activation.ProcessId, application.ProcessId, processIdsMatch,
            activationStartStable, activationExecutableStable, applicationStartStable, applicationExecutableStable,
            sentinelRevalidated, invocationRevalidated, started, completed, aggregate);
    }
}
