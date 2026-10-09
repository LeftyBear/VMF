namespace Vmf.Build.ExcelOwnership.Primitives;

public static class ProcessIdentity
{
    public static string Canonical(string path) => Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar).ToUpperInvariant();

    // Compatibility-only schema-3 comparison. Schema-4 Experiment evidence does not consume it.
    public static IdentityValidation Compare(Ownership expected, int windowProcessId, DateTimeOffset validationProcessStartedUtc,
        string actualExecutablePath, Guid actualInvocationId, string actualSentinelPath)
    {
        var expectedExecutablePath = Canonical(expected.ExecutablePath);
        var observedExecutablePath = Canonical(actualExecutablePath);
        var expectedSentinelPath = Canonical(expected.SentinelPath);
        var observedSentinelPath = Canonical(actualSentinelPath);
        var pidMatch = expected.ProcessId == windowProcessId;
        var startMatch = expected.ProcessStartedUtc.UtcDateTime == validationProcessStartedUtc.UtcDateTime;
        var executableMatch = expectedExecutablePath == observedExecutablePath;
        var invocationMatch = expected.InvocationId == actualInvocationId;
        var sentinelMatch = expectedSentinelPath == observedSentinelPath;
        return new IdentityValidation(expected.ProcessId, windowProcessId, expected.ProcessStartedUtc, validationProcessStartedUtc,
            expectedExecutablePath, observedExecutablePath, expected.InvocationId, actualInvocationId, expectedSentinelPath,
            observedSentinelPath, pidMatch, startMatch, executableMatch, invocationMatch, sentinelMatch,
            pidMatch && startMatch && executableMatch && invocationMatch && sentinelMatch);
    }

    public static bool Matches(Ownership expected, int processId, DateTimeOffset processStartedUtc, string executablePath, Guid invocationId, string sentinelPath) =>
        Compare(expected, processId, processStartedUtc, executablePath, invocationId, sentinelPath).AggregateMatch;
}

public sealed record IdentityValidation(
    int LaunchedPid, int HwndDerivedPid, DateTimeOffset LaunchedProcessStartTimeUtc, DateTimeOffset ValidationTimeProcessStartTimeUtc,
    string ExpectedCanonicalExecutablePath, string ActualCanonicalExecutablePath, Guid ExpectedInvocationCorrelationValue,
    Guid ActualInvocationCorrelationValue, string ExpectedSentinelPath, string ActualBoundWorkbookCanonicalFullPath,
    bool PidMatch, bool StartTimeMatch, bool ExecutablePathMatch, bool InvocationCorrelationMatch, bool SentinelPathMatch, bool AggregateMatch);
