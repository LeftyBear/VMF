using System.Diagnostics;

namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed record ProcessIdentityObservation(
    int ProcessId,
    DateTimeOffset ProcessStartedUtc,
    string CanonicalExecutablePath,
    DateTimeOffset ObservationStartedUtc,
    DateTimeOffset ObservationCompletedUtc,
    string StartTimeAcquisitionProvenance,
    string ExecutableAcquisitionProvenance,
    bool StableIdentity,
    bool PidReuseSuspected);

public static class ProcessIdentityObserver
{
    public static ProcessIdentityObservation Observe(int processId)
    {
        if (processId <= 0) throw new InvalidOperationException("Application HWND-owner PID is invalid.");
        var observationStartedUtc = DateTimeOffset.UtcNow;
        using var process = Process.GetProcessById(processId);
        process.Refresh();
        var firstStart = new DateTimeOffset(process.StartTime.ToUniversalTime(), TimeSpan.Zero);
        var firstExecutable = ProcessIdentity.Canonical(process.MainModule?.FileName ?? throw new InvalidOperationException("Application process executable identity is unavailable."));
        process.Refresh();
        if (process.HasExited) throw new InvalidOperationException("Application process exited during identity observation.");
        var secondStart = new DateTimeOffset(process.StartTime.ToUniversalTime(), TimeSpan.Zero);
        var secondExecutable = ProcessIdentity.Canonical(process.MainModule?.FileName ?? throw new InvalidOperationException("Application process executable identity reread is unavailable."));
        var stable = firstStart == secondStart && firstExecutable == secondExecutable;
        return new ProcessIdentityObservation(
            processId,
            secondStart,
            secondExecutable,
            observationStartedUtc,
            DateTimeOffset.UtcNow,
            "Process.GetProcessById(HWND-owner PID).StartTime",
            "Process.GetProcessById(HWND-owner PID).MainModule.FileName",
            stable,
            !stable);
    }
}
