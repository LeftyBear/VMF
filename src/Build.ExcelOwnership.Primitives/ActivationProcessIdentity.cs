namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed record ActivationProcessIdentity(
    int ProcessId,
    DateTimeOffset ProcessStartedUtc,
    string CanonicalExecutablePath,
    DateTimeOffset LaunchRequestedUtc,
    DateTimeOffset LaunchReturnedUtc,
    string LaunchMethod,
    string LaunchArguments,
    string AcquisitionProvenance,
    DateTimeOffset LifetimeObservedUtc,
    bool? HasExited,
    ResidualState ResidualState);
