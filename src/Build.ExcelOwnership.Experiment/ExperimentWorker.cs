using Vmf.Build.ExcelOwnership.Primitives;

namespace Vmf.Build.ExcelOwnership.Experiment;

internal static class ExperimentWorker
{
    internal static int Run(string requestPath, string token)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) return RunSta(requestPath, token);
        var result = 2;
        var thread = new Thread(() => result = RunSta(requestPath, token));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        return result;
    }

    private static int RunSta(string requestPath, string token)
    {
        WorkerRequest? request = null;
        ExcelOwnershipLease? lease = null;
        LifecycleJournal? journal = null;
        EvidencePersistenceConfirmation? ownershipConfirmation = null;
        EvidencePersistenceConfirmation? topologyConfirmation = null;
        try
        {
            request = ExperimentContracts.ValidateWorkerRequest(requestPath, token);
            var log = new ExperimentEvidence(request.InvocationId, request.Paths);
            log.WorkerEvent("WorkerRequestValidated");
            journal = new LifecycleJournal(request.Paths.OwnershipLifecycle, request.InvocationId);
            journal.Record("InvocationStarted");
            lease = new ExcelOwnershipLease(journal, request.InvocationId);
            log.WorkerEvent("ExcelActivationRequested");
            lease.Start(request.Paths.Sentinel);

            ownershipConfirmation = PersistOwnershipEvidence(request, lease);
            lease.ConfirmOwnershipEvidencePersisted(ownershipConfirmation);
            log.WorkerEvent("ApplicationProcessIdentityValidated");

            var topology = lease.ObserveCleanupTopology();
            topologyConfirmation = PersistCleanupTopologyEvidence(request, lease, topology);
            if (!topology.AggregateMatch) throw new InvalidOperationException("Excel cleanup topology mismatch.");
            lease.ConfirmCleanupTopologyEvidencePersisted(topologyConfirmation);
            log.WorkerEvent("CleanupTopologyValidated");

            journal.Record("ExcelOperationStarted", lease.Identity);
            ObserveCleanupRequest(request, log);
            lease.RequestNormalCleanup();
            PersistPostCleanupEvidence(request, lease, ownershipConfirmation, topologyConfirmation, null);
            journal.Record("InvocationSucceeded", lease.Identity);
            WriteResult(request, lease, "Succeeded", null);
            return 0;
        }
        catch (Exception exception)
        {
            if (request is null) return 2;
            if (lease?.ApplicationIdentityValidation is not null && ownershipConfirmation is null)
                try { ownershipConfirmation = PersistOwnershipEvidence(request, lease); } catch { }
            try { lease?.RequestNormalCleanup(); } catch { }
            if (lease is not null)
                try { PersistPostCleanupEvidence(request, lease, ownershipConfirmation, topologyConfirmation, exception); } catch { }
            try { journal?.Record("InvocationFailed", lease?.Identity); } catch { }
            var code = exception.Message switch
            {
                "Excel application identity mismatch." => "ApplicationProcessIdentityMismatch",
                "Excel cleanup topology mismatch." => "CleanupTopologyMismatch",
                _ => exception.GetType().Name
            };
            try { WriteResult(request, lease, "Failed", new FailureInformation(code, exception.Message)); } catch { }
            return 1;
        }
        finally
        {
            try { lease?.Dispose(); } catch { }
        }
    }

    private static void ObserveCleanupRequest(WorkerRequest request, ExperimentEvidence log)
    {
        if (!File.Exists(request.Paths.CleanupRequest)) return;
        var cleanup = ExperimentEvidence.ReadSingle<CleanupRequest>(request.Paths.CleanupRequest);
        if (cleanup.SchemaVersion != ExperimentContracts.SchemaVersion || cleanup.InvocationId != request.InvocationId || cleanup.CorrelationToken != request.CorrelationToken)
            throw new InvalidDataException("Cleanup request correlation mismatch.");
        log.WorkerEvent("CooperativeCleanupRequestObserved");
    }

    private static EvidencePersistenceConfirmation PersistOwnershipEvidence(WorkerRequest request, ExcelOwnershipLease lease)
    {
        var evidence = OwnershipValidationEvidence.From(request, lease);
        return ExperimentEvidence.PersistAndConfirm(request.Paths.IdentityValidation,
            ExperimentEvidence.OwnershipPersistedPath(request.Paths), evidence, request.InvocationId, "OwnershipValidation", evidence.RecordedUtc);
    }

    private static EvidencePersistenceConfirmation PersistCleanupTopologyEvidence(WorkerRequest request, ExcelOwnershipLease lease, CleanupTopologyValidation topology)
    {
        var evidence = new CleanupTopologyEvidence(ExperimentContracts.SchemaVersion, request.InvocationId, "CleanupTopology",
            lease.ActivationProcessIdentity ?? throw new InvalidDataException("Activation identity is unavailable."),
            lease.ApplicationProcessIdentity ?? throw new InvalidDataException("Application identity is unavailable."),
            lease.ActivationProcessReobservation ?? throw new InvalidDataException("Activation reobservation is unavailable."),
            lease.ApplicationProcessReobservation ?? throw new InvalidDataException("Application reobservation is unavailable."),
            topology, DateTimeOffset.UtcNow);
        return ExperimentEvidence.PersistAndConfirm(ExperimentEvidence.CleanupTopologyPath(request.Paths),
            ExperimentEvidence.CleanupTopologyPersistedPath(request.Paths), evidence, request.InvocationId, "CleanupTopology", evidence.RecordedUtc);
    }

    private static void PersistPostCleanupEvidence(WorkerRequest request, ExcelOwnershipLease lease,
        EvidencePersistenceConfirmation? ownership, EvidencePersistenceConfirmation? topology, Exception? failure)
    {
        var ownershipHash = ownership?.Sha256 ?? string.Empty;
        var topologyHash = topology?.Sha256 ?? string.Empty;
        var attempted = lease.CleanupEligible;
        ExperimentEvidence.WriteSingle(ExperimentEvidence.CleanupAttemptPath(request.Paths), new CleanupAttemptEvidence(
            ExperimentContracts.SchemaVersion, request.InvocationId, "CleanupAttempt", ownershipHash, topologyHash,
            lease.CleanupEligible, attempted, attempted, lease.CleanupCompleted, lease.CleanupTimingTelemetry,
            failure?.Message, DateTimeOffset.UtcNow));
        ExperimentEvidence.WriteSingle(ExperimentEvidence.ResidualObservationPath(request.Paths), new ResidualObservationEvidence(
            ExperimentContracts.SchemaVersion, request.InvocationId, "ResidualObservation", ownershipHash, topologyHash,
            lease.RotExactMatchCount, lease.ResidualObservationTelemetry,
            lease.ActivationProcessResidualState, lease.ApplicationProcessResidualState, lease.CleanupCompleted, DateTimeOffset.UtcNow));
    }

    private static void WriteResult(WorkerRequest request, ExcelOwnershipLease? lease, string outcome, FailureInformation? failure) =>
        ExperimentEvidence.WriteSingle(request.Paths.Result, new ExperimentResult(
            ExperimentContracts.SchemaVersion, request.InvocationId, outcome, lease?.Identity, lease?.ActivationProcessIdentity,
            lease?.ApplicationProcessIdentity, lease?.AttachmentValidated ?? false, lease?.CleanupCompleted ?? false,
            lease?.ActivationProcessResidualState ?? ResidualState.NotApplicable,
            lease?.ApplicationProcessResidualState ?? ResidualState.NotApplicable, request.Paths, failure));
}
