using System.Diagnostics;

namespace Vmf.Build.ExcelOwnership.Primitives;

public enum ResidualIdentityValidationOutcome
{
    NotApplicable,
    Matched,
    Missing,
    StartTimeMismatch,
    CheckFailed
}

public sealed record ResidualObservationTarget(
    string Role,
    int ProcessId,
    DateTimeOffset RecordedProcessStartedUtc,
    ResidualIdentityValidationOutcome FinalIdentityValidationOutcome,
    ResidualState FinalResidualState);

public sealed record ResidualObservationTelemetry(
    DateTimeOffset ObservationStartedUtc,
    DateTimeOffset ObservationCompletedUtc,
    int PollingIntervalMilliseconds,
    int MaximumGraceMilliseconds,
    int AttemptCount,
    IReadOnlyList<ResidualObservationTarget> Targets,
    bool SharedPhysicalProcess);

public sealed record CleanupTimingTelemetry(
    DateTimeOffset? WorkbookCloseStartedUtc,
    DateTimeOffset? WorkbookCloseCompletedUtc,
    DateTimeOffset? ApplicationQuitStartedUtc,
    DateTimeOffset? ApplicationQuitCompletedUtc);

internal sealed record ResidualProcessProbe(bool Exists, DateTimeOffset? ProcessStartedUtc, bool CheckSucceeded);

internal static class ResidualProcessObserver
{
    internal static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);
    internal static readonly TimeSpan MaximumGrace = TimeSpan.FromSeconds(20);

    internal static ResidualObservationTelemetry Observe(
        ActivationProcessIdentity activation,
        ApplicationProcessIdentity? application)
    {
        var stopwatch = Stopwatch.StartNew();
        return Observe(activation, application, Probe, () => stopwatch.Elapsed,
            delay => Thread.Sleep(delay), () => DateTimeOffset.UtcNow);
    }

    internal static ResidualObservationTelemetry Observe(
        ActivationProcessIdentity activation,
        ApplicationProcessIdentity? application,
        Func<int, ResidualProcessProbe> probe,
        Func<TimeSpan> elapsed,
        Action<TimeSpan> delay,
        Func<DateTimeOffset> utcNow)
    {
        var startedUtc = utcNow();
        var shared = application is not null &&
            activation.ProcessId == application.ProcessId &&
            activation.ProcessStartedUtc == application.IndependentlyObservedStartTimeUtc;
        var physicalTargets = new List<(int ProcessId, DateTimeOffset StartedUtc)> { (activation.ProcessId, activation.ProcessStartedUtc) };
        if (application is not null && !shared)
            physicalTargets.Add((application.ProcessId, application.IndependentlyObservedStartTimeUtc));

        var final = new Dictionary<(int, DateTimeOffset), (ResidualIdentityValidationOutcome Validation, ResidualState Residual)>();
        var attempts = 0;
        while (final.Count < physicalTargets.Count)
        {
            attempts++;
            foreach (var target in physicalTargets)
            {
                if (final.ContainsKey(target)) continue;
                var observation = probe(target.ProcessId);
                if (!observation.CheckSucceeded)
                {
                    final[target] = (ResidualIdentityValidationOutcome.CheckFailed, ResidualState.CheckFailed);
                }
                else if (!observation.Exists)
                {
                    final[target] = (ResidualIdentityValidationOutcome.Missing, ResidualState.Clear);
                }
                else if (observation.ProcessStartedUtc != target.StartedUtc)
                {
                    final[target] = (ResidualIdentityValidationOutcome.StartTimeMismatch, ResidualState.CheckFailed);
                }
            }

            if (final.Count == physicalTargets.Count) break;
            var remaining = MaximumGrace - elapsed();
            if (remaining <= TimeSpan.Zero)
            {
                foreach (var target in physicalTargets)
                    if (!final.ContainsKey(target))
                        final[target] = (ResidualIdentityValidationOutcome.Matched, ResidualState.Residual);
                break;
            }
            delay(remaining < PollInterval ? remaining : PollInterval);
        }

        var targets = new List<ResidualObservationTarget>
        {
            ToTarget("Activation", activation.ProcessId, activation.ProcessStartedUtc, final)
        };
        if (application is not null)
            targets.Add(ToTarget("Application", application.ProcessId, application.IndependentlyObservedStartTimeUtc, final));
        return new ResidualObservationTelemetry(startedUtc, utcNow(), (int)PollInterval.TotalMilliseconds,
            (int)MaximumGrace.TotalMilliseconds, attempts, targets, shared);
    }

    private static ResidualObservationTarget ToTarget(string role, int processId, DateTimeOffset startedUtc,
        IReadOnlyDictionary<(int, DateTimeOffset), (ResidualIdentityValidationOutcome Validation, ResidualState Residual)> final)
    {
        var result = final[(processId, startedUtc)];
        return new ResidualObservationTarget(role, processId, startedUtc, result.Validation, result.Residual);
    }

    private static ResidualProcessProbe Probe(int processId)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            process.Refresh();
            if (process.HasExited) return new ResidualProcessProbe(false, null, true);
            return new ResidualProcessProbe(true,
                new DateTimeOffset(process.StartTime.ToUniversalTime(), TimeSpan.Zero), true);
        }
        catch (ArgumentException)
        {
            return new ResidualProcessProbe(false, null, true);
        }
        catch
        {
            return new ResidualProcessProbe(false, null, false);
        }
    }
}
