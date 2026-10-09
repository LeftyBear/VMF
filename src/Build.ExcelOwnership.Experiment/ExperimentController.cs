using System.Diagnostics;
using System.Security.Cryptography;
using Vmf.Build.ExcelOwnership.Primitives;

namespace Vmf.Build.ExcelOwnership.Experiment;

internal sealed class ExperimentController
{
    internal static readonly TimeSpan LauncherDeadline = TimeSpan.FromSeconds(120);
    internal static readonly TimeSpan CleanupGrace = TimeSpan.FromSeconds(30);

    internal int Run(PublicRequest request)
    {
        var paths = ExperimentEvidence.CreatePaths(request.TemporaryRoot, request.InvocationId);
        var evidence = new ExperimentEvidence(request.InvocationId, paths);
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var workerRequestPath = Path.Combine(request.TemporaryRoot, $"{request.InvocationId:D}.worker-request.json");
        var zeroExcelPrecondition = ObserveZeroExcelPrecondition();
        if (zeroExcelPrecondition.ExcelProcessCount != 0)
            throw new InvalidOperationException("Experiment preflight requires exactly zero Excel processes.");
        ExperimentEvidence.WriteSingle(workerRequestPath, new WorkerRequest(ExperimentContracts.SchemaVersion, request.InvocationId, token, paths, zeroExcelPrecondition));
        evidence.ControllerState("InvocationStarted");

        using var worker = StartWorker(workerRequestPath, token);
        evidence.ControllerState("WorkerStarted", worker.Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (!worker.WaitForExit((int)LauncherDeadline.TotalMilliseconds))
        {
            evidence.ControllerState("LauncherDeadlineExpired");
            ExperimentEvidence.WriteSingle(paths.CleanupRequest, new CleanupRequest(ExperimentContracts.SchemaVersion, request.InvocationId, token, DateTimeOffset.UtcNow));
            evidence.ControllerState("CleanupRequested");
            if (!worker.WaitForExit((int)CleanupGrace.TotalMilliseconds))
            {
                evidence.ControllerState("SafeStopHumanInspection", "Cooperative cleanup was not established; processes left untouched.");
                WriteSafeStopResultIfAbsent(request.InvocationId, paths);
                return 3;
            }
        }

        evidence.ControllerState("WorkerExited", worker.ExitCode.ToString(System.Globalization.CultureInfo.InvariantCulture));
        ExperimentResult? result = null;
        try
        {
            result = ExperimentEvidence.ReadSingle<ExperimentResult>(paths.Result);
            ValidateResult(result, request.InvocationId, paths);
            evidence.ControllerState(result.Outcome == "Succeeded" ? "InvocationSucceeded" : "InvocationFailed", result.Failure?.Message);
            return result.Outcome == "Succeeded" && worker.ExitCode == 0 ? 0 : 1;
        }
        catch (Exception exception)
        {
            var workerFailure = result?.Failure is null ? "none" : $"{result.Failure.Code}: {result.Failure.Message}";
            evidence.ControllerState("InvocationFailed", $"WorkerFailure={workerFailure}; ControllerEvidenceFailure={exception.GetType().Name}: {exception.Message}");
            return 1;
        }
    }

    private static Process StartWorker(string requestPath, string token)
    {
        var assemblyPath = typeof(Program).Assembly.Location;
        var start = new ProcessStartInfo("dotnet") { UseShellExecute = false };
        start.ArgumentList.Add(assemblyPath);
        start.Environment[Program.WorkerRequestEnvironmentVariable] = requestPath;
        start.Environment[Program.WorkerTokenEnvironmentVariable] = token;
        return Process.Start(start) ?? throw new InvalidOperationException("Worker launch failed.");
    }

    private static void WriteSafeStopResultIfAbsent(Guid invocationId, EvidencePaths paths)
    {
        if (File.Exists(paths.Result)) return;
        try
        {
            ExperimentEvidence.WriteSingle(paths.Result, new ExperimentResult(
                ExperimentContracts.SchemaVersion,
                invocationId,
                "SafeStop",
                null,
                null,
                null,
                false,
                false,
                ResidualState.NotApplicable,
                ResidualState.NotApplicable,
                paths,
                new FailureInformation("CleanupGraceExpired", "Cooperative completion was not established; human inspection is required.")));
        }
        catch (IOException) when (File.Exists(paths.Result))
        {
            // The worker won the create-new race; its invocation-correlated result is preserved.
        }
    }

    internal static void ValidateResult(ExperimentResult result, Guid invocationId, EvidencePaths paths)
    {
        if (result.SchemaVersion != ExperimentContracts.SchemaVersion || result.InvocationId != invocationId || result.Paths != paths || result.Outcome is not ("Succeeded" or "Failed"))
            throw new InvalidDataException("Result evidence mismatch.");
        var controller = ExperimentEvidence.ValidateJsonLines<ControllerLifecycleEntry>(paths.ControllerLifecycle, invocationId, x => x.SchemaVersion, x => x.InvocationId, x => x.Sequence, x => x.RecordedUtc);
        var diagnostics = ExperimentEvidence.ValidateJsonLines<DiagnosticEvent>(paths.ExperimentLog, invocationId, x => x.SchemaVersion, x => x.InvocationId, x => x.Sequence, x => x.RecordedUtc);
        var ownership = LifecycleJournal.Validate(paths.OwnershipLifecycle, invocationId);
        if (controller.Count == 0 || diagnostics.Count == 0 || ownership.Count == 0 || result.Ownership?.InvocationId != invocationId ||
            result.ActivationProcessIdentity is null || result.ApplicationProcessIdentity is null)
            throw new InvalidDataException("Required evidence is incomplete.");
        if (result.Outcome == "Succeeded" || result.Failure?.Code == "ApplicationProcessIdentityMismatch" || result.Failure?.Code == "CleanupTopologyMismatch")
            ValidateIdentityEvidence(result, invocationId, paths, ownership[0].RecordedUtc);
        if (result.Outcome == "Succeeded" || result.Failure?.Code == "CleanupTopologyMismatch")
            ValidateCleanupTopologyEvidence(result, invocationId, paths);
        DateTimeOffset? priorOwnershipTimestamp = null;
        foreach (var entry in ownership)
        {
            if (priorOwnershipTimestamp > entry.RecordedUtc) throw new InvalidDataException("Ownership timestamps regress.");
            priorOwnershipTimestamp = entry.RecordedUtc;
        }
        if (result.Outcome == "Succeeded" && (!result.AttachmentValidated || !result.CleanupCompleted ||
            result.ActivationProcessResidualState != ResidualState.Clear || result.ApplicationProcessResidualState != ResidualState.Clear || result.Failure is not null))
            throw new InvalidDataException("Success evidence is inconsistent.");
        var lifecycleCleanupCompleted = ownership.Any(entry => entry.State == "ExcelCleanupCompleted");
        if (result.CleanupCompleted != (lifecycleCleanupCompleted && result.ActivationProcessResidualState == ResidualState.Clear && result.ApplicationProcessResidualState == ResidualState.Clear))
            throw new InvalidDataException("Cleanup result and lifecycle evidence are inconsistent.");
    }

    private static void ValidateIdentityEvidence(ExperimentResult result, Guid invocationId, EvidencePaths paths, DateTimeOffset invocationStartedUtc)
    {
        var identity = ExperimentEvidence.ReadSingle<OwnershipValidationEvidence>(paths.IdentityValidation);
        var activation = identity.ActivationProcessIdentity;
        var application = identity.ApplicationProcessIdentity;
        var comparison = identity.ComparisonResults;
        var allComponentComparisonsMatch = comparison.ExactSentinelMonikerAttachment && comparison.SentinelPathMatch &&
            comparison.InvocationCorrelationMatch && comparison.HwndValid && comparison.HwndOwnerProcessIdValid &&
            comparison.IndependentStartTimeObserved && comparison.IndependentExecutableObserved && comparison.ExpectedExecutableMatch &&
            comparison.StableIdentity && !comparison.PidReuseSuspected;
        if (identity.SchemaVersion != ExperimentContracts.SchemaVersion || identity.InvocationId != invocationId || identity.ArtifactKind != "OwnershipValidation" ||
            identity.RecordedUtc == default || identity.RecordedUtc < invocationStartedUtc || identity.RecordedUtc > DateTimeOffset.UtcNow.AddMinutes(1) ||
            identity.ZeroExcelPrecondition.ExcelProcessCount != 0 || identity.ZeroExcelPrecondition.ObservedUtc == default ||
            string.IsNullOrWhiteSpace(identity.ZeroExcelPrecondition.AcquisitionProvenance) ||
            activation.ProcessId <= 0 || activation.ProcessStartedUtc == default || string.IsNullOrWhiteSpace(activation.CanonicalExecutablePath) ||
            activation.LaunchRequestedUtc == default || activation.LaunchReturnedUtc < activation.LaunchRequestedUtc ||
            string.IsNullOrWhiteSpace(activation.LaunchMethod) || string.IsNullOrWhiteSpace(activation.LaunchArguments) || string.IsNullOrWhiteSpace(activation.AcquisitionProvenance) ||
            application.Hwnd == 0 || application.ProcessId <= 0 || application.IndependentlyObservedStartTimeUtc == default ||
            string.IsNullOrWhiteSpace(application.IndependentlyObservedCanonicalExecutablePath) ||
            string.IsNullOrWhiteSpace(application.ExpectedCanonicalExecutablePath) ||
            application.ObservationStartedUtc == default || application.ObservationCompletedUtc < application.ObservationStartedUtc ||
            !application.ProcessIdAcquisitionProvenance.Contains("GetWindowThreadProcessId", StringComparison.Ordinal) ||
            !application.StartTimeAcquisitionProvenance.Contains("HWND-owner PID", StringComparison.Ordinal) ||
            !application.ExecutableAcquisitionProvenance.Contains("HWND-owner PID", StringComparison.Ordinal) ||
            activation.ProcessId != result.Ownership!.ProcessId || activation.ProcessStartedUtc != result.Ownership.ProcessStartedUtc ||
            activation.CanonicalExecutablePath != result.Ownership.ExecutablePath || application.ExpectedInvocationCorrelationValue != invocationId ||
            application.ExpectedSentinelCanonicalPath != ProcessIdentity.Canonical(result.Ownership.SentinelPath) ||
            comparison.ActivationPidMatchesApplicationPid != (activation.ProcessId == application.ProcessId) ||
            comparison.ExpectedExecutableMatch != (application.ExpectedCanonicalExecutablePath == application.IndependentlyObservedCanonicalExecutablePath) ||
            comparison.InvocationCorrelationMatch != (application.ExpectedInvocationCorrelationValue == application.ActualInvocationCorrelationValue) ||
            comparison.SentinelPathMatch != (application.ExpectedSentinelCanonicalPath == application.ActualBoundWorkbookCanonicalFullPath) ||
            comparison.AggregateMatch != allComponentComparisonsMatch || identity.AggregateValidation != allComponentComparisonsMatch ||
            result.Outcome == "Succeeded" && (!allComponentComparisonsMatch || !identity.AggregateValidation ||
                application.ValidationState != ApplicationIdentityValidationState.Validated) ||
            result.Failure?.Code == "ApplicationProcessIdentityMismatch" && identity.AggregateValidation)
            throw new InvalidDataException("Identity-validation evidence is inconsistent.");
    }

    private static void ValidateCleanupTopologyEvidence(ExperimentResult result, Guid invocationId, EvidencePaths paths)
    {
        var evidence = ExperimentEvidence.ReadSingle<CleanupTopologyEvidence>(ExperimentEvidence.CleanupTopologyPath(paths));
        var ownershipConfirmation = ExperimentEvidence.ReadSingle<EvidencePersistenceConfirmationRecord>(ExperimentEvidence.OwnershipPersistedPath(paths));
        var topologyConfirmation = ExperimentEvidence.ReadSingle<EvidencePersistenceConfirmationRecord>(ExperimentEvidence.CleanupTopologyPersistedPath(paths));
        var attempt = ExperimentEvidence.ReadSingle<CleanupAttemptEvidence>(ExperimentEvidence.CleanupAttemptPath(paths));
        var residual = ExperimentEvidence.ReadSingle<ResidualObservationEvidence>(ExperimentEvidence.ResidualObservationPath(paths));
        if (evidence.SchemaVersion != ExperimentContracts.SchemaVersion || evidence.InvocationId != invocationId || evidence.ArtifactKind != "CleanupTopology" ||
            ownershipConfirmation.InvocationId != invocationId || topologyConfirmation.InvocationId != invocationId ||
            attempt.InvocationId != invocationId || residual.InvocationId != invocationId ||
            evidence.Validation.ProcessIdsMatch != (evidence.ActivationProcessIdentity.ProcessId == evidence.ApplicationProcessIdentity.ProcessId) ||
            result.Failure?.Code == "CleanupTopologyMismatch" && evidence.Validation.AggregateMatch ||
            result.Outcome == "Succeeded" && (!evidence.Validation.AggregateMatch || !attempt.Completed || !residual.CleanupCompleted ||
                residual.ActivationResidual != ResidualState.Clear || residual.ApplicationResidual != ResidualState.Clear ||
                residual.RotExactMatchCount != 1 || residual.Observation is null || residual.Observation.AttemptCount <= 0 ||
                residual.Observation.PollingIntervalMilliseconds != 100 || residual.Observation.MaximumGraceMilliseconds != 20000 ||
                residual.Observation.ObservationStartedUtc == default ||
                residual.Observation.ObservationCompletedUtc < residual.Observation.ObservationStartedUtc ||
                residual.Observation.Targets.Count != 2 ||
                attempt.Timing.WorkbookCloseStartedUtc is null || attempt.Timing.WorkbookCloseCompletedUtc is null ||
                attempt.Timing.WorkbookCloseCompletedUtc < attempt.Timing.WorkbookCloseStartedUtc ||
                attempt.Timing.ApplicationQuitStartedUtc is null || attempt.Timing.ApplicationQuitCompletedUtc is null ||
                attempt.Timing.ApplicationQuitCompletedUtc < attempt.Timing.ApplicationQuitStartedUtc))
            throw new InvalidDataException("Cleanup-topology evidence is inconsistent.");
    }

    private static ZeroExcelPreconditionEvidence ObserveZeroExcelPrecondition()
    {
        var processes = Process.GetProcessesByName("EXCEL");
        try { return new ZeroExcelPreconditionEvidence(processes.Length, DateTimeOffset.UtcNow, "Experiment controller Process.GetProcessesByName(EXCEL)"); }
        finally { foreach (var process in processes) process.Dispose(); }
    }
}
