namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed record ApplicationIdentityValidation(
    bool ExactSentinelMonikerAttachment,
    bool SentinelPathMatch,
    bool InvocationCorrelationMatch,
    bool HwndValid,
    bool HwndOwnerProcessIdValid,
    bool IndependentStartTimeObserved,
    bool IndependentExecutableObserved,
    bool ExpectedExecutableMatch,
    bool StableIdentity,
    bool PidReuseSuspected,
    bool ActivationPidMatchesApplicationPid,
    bool AggregateMatch)
{
    public static ApplicationIdentityValidation Compare(
        ActivationProcessIdentity activation,
        nint hwnd,
        int applicationProcessId,
        DateTimeOffset independentlyObservedStartTimeUtc,
        string independentlyObservedExecutablePath,
        string expectedExecutablePath,
        Guid expectedInvocationId,
        Guid actualInvocationId,
        string expectedSentinelPath,
        string actualSentinelPath,
        bool exactSentinelMonikerAttachment,
        bool stableIdentity,
        bool pidReuseSuspected)
    {
        var expectedExecutable = ProcessIdentity.Canonical(expectedExecutablePath);
        var applicationExecutable = ProcessIdentity.Canonical(independentlyObservedExecutablePath);
        var expectedSentinel = ProcessIdentity.Canonical(expectedSentinelPath);
        var actualSentinel = ProcessIdentity.Canonical(actualSentinelPath);
        var sentinelMatches = expectedSentinel == actualSentinel;
        var invocationMatches = expectedInvocationId != Guid.Empty && expectedInvocationId == actualInvocationId;
        var hwndValid = hwnd != 0;
        var pidValid = applicationProcessId > 0;
        var startObserved = independentlyObservedStartTimeUtc != default;
        var executableObserved = !string.IsNullOrWhiteSpace(applicationExecutable);
        var executableMatches = expectedExecutable == applicationExecutable;
        var aggregate = exactSentinelMonikerAttachment && sentinelMatches && invocationMatches && hwndValid && pidValid &&
            startObserved && executableObserved && executableMatches && stableIdentity && !pidReuseSuspected;
        return new ApplicationIdentityValidation(
            exactSentinelMonikerAttachment,
            sentinelMatches,
            invocationMatches,
            hwndValid,
            pidValid,
            startObserved,
            executableObserved,
            executableMatches,
            stableIdentity,
            pidReuseSuspected,
            activation.ProcessId == applicationProcessId,
            aggregate);
    }
}
