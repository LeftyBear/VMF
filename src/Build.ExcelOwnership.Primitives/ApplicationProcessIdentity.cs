namespace Vmf.Build.ExcelOwnership.Primitives;

public enum ApplicationIdentityValidationState
{
    NotValidated,
    Validated,
    Rejected
}

public sealed record ApplicationProcessIdentity(
    nint Hwnd,
    int ProcessId,
    DateTimeOffset IndependentlyObservedStartTimeUtc,
    string IndependentlyObservedCanonicalExecutablePath,
    DateTimeOffset ObservationStartedUtc,
    DateTimeOffset ObservationCompletedUtc,
    string ProcessIdAcquisitionProvenance,
    string StartTimeAcquisitionProvenance,
    string ExecutableAcquisitionProvenance,
    string ExpectedCanonicalExecutablePath,
    string ExpectedSentinelCanonicalPath,
    string ActualBoundWorkbookCanonicalFullPath,
    Guid ExpectedInvocationCorrelationValue,
    Guid ActualInvocationCorrelationValue,
    ApplicationIdentityValidationState ValidationState,
    bool CleanupEligible,
    ResidualState ResidualState);
