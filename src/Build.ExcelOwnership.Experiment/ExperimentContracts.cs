using Vmf.Build.ExcelOwnership.Primitives;

namespace Vmf.Build.ExcelOwnership.Experiment;

internal sealed record PublicRequest(Guid InvocationId, string TemporaryRoot);
internal sealed record ZeroExcelPreconditionEvidence(int ExcelProcessCount, DateTimeOffset ObservedUtc, string AcquisitionProvenance);
internal sealed record WorkerRequest(int SchemaVersion, Guid InvocationId, string CorrelationToken, EvidencePaths Paths, ZeroExcelPreconditionEvidence ZeroExcelPrecondition);
internal sealed record CleanupRequest(int SchemaVersion, Guid InvocationId, string CorrelationToken, DateTimeOffset RequestedUtc);
internal sealed record EvidencePaths(string Result, string ControllerLifecycle, string OwnershipLifecycle, string ExperimentLog, string IdentityValidation, string CleanupRequest, string Sentinel);
internal sealed record DiagnosticEvent(int SchemaVersion, Guid InvocationId, long Sequence, DateTimeOffset RecordedUtc, string Event, string? Detail);
internal sealed record ControllerLifecycleEntry(int SchemaVersion, Guid InvocationId, long Sequence, DateTimeOffset RecordedUtc, string State, string? Detail);
internal sealed record ExperimentResult(
    int SchemaVersion,
    Guid InvocationId,
    string Outcome,
    Ownership? Ownership,
    ActivationProcessIdentity? ActivationProcessIdentity,
    ApplicationProcessIdentity? ApplicationProcessIdentity,
    bool AttachmentValidated,
    bool CleanupCompleted,
    ResidualState ActivationProcessResidualState,
    ResidualState ApplicationProcessResidualState,
    EvidencePaths Paths,
    FailureInformation? Failure);
internal sealed record FailureInformation(string Code, string Message);
internal sealed record OwnershipValidationEvidence(
    int SchemaVersion,
    Guid InvocationId,
    string ArtifactKind,
    ActivationProcessIdentity ActivationProcessIdentity,
    ApplicationProcessIdentity ApplicationProcessIdentity,
    ZeroExcelPreconditionEvidence ZeroExcelPrecondition,
    ApplicationIdentityValidation ComparisonResults,
    bool AggregateValidation,
    DateTimeOffset RecordedUtc)
{
    internal static OwnershipValidationEvidence From(WorkerRequest request, ExcelOwnershipLease lease) => new(
        ExperimentContracts.SchemaVersion,
        request.InvocationId,
        "OwnershipValidation",
        lease.ActivationProcessIdentity ?? throw new InvalidDataException("Activation identity evidence is unavailable."),
        lease.ApplicationProcessIdentity ?? throw new InvalidDataException("Application identity evidence is unavailable."),
        request.ZeroExcelPrecondition,
        lease.ApplicationIdentityValidation ?? throw new InvalidDataException("Application identity comparison evidence is unavailable."),
        request.ZeroExcelPrecondition.ExcelProcessCount == 0 && lease.ApplicationIdentityValidation?.AggregateMatch == true,
        DateTimeOffset.UtcNow);
}

internal sealed record CleanupTopologyEvidence(
    int SchemaVersion,
    Guid InvocationId,
    string ArtifactKind,
    ActivationProcessIdentity ActivationProcessIdentity,
    ApplicationProcessIdentity ApplicationProcessIdentity,
    ProcessIdentityObservation ActivationProcessReobservation,
    ProcessIdentityObservation ApplicationProcessReobservation,
    CleanupTopologyValidation Validation,
    DateTimeOffset RecordedUtc);

internal sealed record EvidencePersistenceConfirmationRecord(
    int SchemaVersion,
    Guid InvocationId,
    string ArtifactKind,
    string TargetArtifactFileName,
    long ByteLength,
    string Sha256,
    DateTimeOffset TargetRecordedUtc,
    DateTimeOffset RecordedUtc);

internal sealed record CleanupAttemptEvidence(
    int SchemaVersion,
    Guid InvocationId,
    string ArtifactKind,
    string OwnershipConfirmationSha256,
    string CleanupTopologyConfirmationSha256,
    bool CleanupEligible,
    bool CloseAttempted,
    bool QuitAttempted,
    bool Completed,
    CleanupTimingTelemetry Timing,
    string? Failure,
    DateTimeOffset RecordedUtc);

internal sealed record ResidualObservationEvidence(
    int SchemaVersion,
    Guid InvocationId,
    string ArtifactKind,
    string OwnershipConfirmationSha256,
    string CleanupTopologyConfirmationSha256,
    int? RotExactMatchCount,
    ResidualObservationTelemetry? Observation,
    ResidualState ActivationResidual,
    ResidualState ApplicationResidual,
    bool CleanupCompleted,
    DateTimeOffset RecordedUtc);

internal static class ExperimentContracts
{
    internal const int SchemaVersion = 4;

    internal static PublicRequest ParsePublicCommand(string[] args)
    {
        if (args.Length != 5 || !string.Equals(args[0], "run", StringComparison.Ordinal))
            throw new ArgumentException("Expected: run --invocation-id <non-empty-guid> --temporary-root <absolute-isolated-empty-directory>.");
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 1; index < args.Length; index += 2)
        {
            var name = args[index];
            if (name is not "--invocation-id" and not "--temporary-root" || !values.TryAdd(name, args[index + 1]))
                throw new ArgumentException($"Unknown or duplicate argument: {name}.");
        }
        if (!values.TryGetValue("--invocation-id", out var invocationText) || !Guid.TryParse(invocationText, out var invocationId) || invocationId == Guid.Empty)
            throw new ArgumentException("Invocation ID must be a non-empty GUID.");
        if (!values.TryGetValue("--temporary-root", out var temporaryRoot) || string.IsNullOrWhiteSpace(temporaryRoot) || !Path.IsPathFullyQualified(temporaryRoot))
            throw new ArgumentException("Temporary root must be absolute.");
        var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(temporaryRoot));
        if (Path.GetPathRoot(root)?.Equals(root, StringComparison.OrdinalIgnoreCase) == true || !Directory.Exists(root))
            throw new ArgumentException("Temporary root must be an existing isolated directory.");
        if (Directory.EnumerateFileSystemEntries(root).Any()) throw new ArgumentException("Temporary root must be empty.");
        var repositoryRoot = FindRepositoryRoot();
        if (repositoryRoot is not null && (root.Equals(repositoryRoot, StringComparison.OrdinalIgnoreCase) || IsContainedBy(root, repositoryRoot)))
            throw new ArgumentException("Temporary root must be isolated from the repository.");
        return new PublicRequest(invocationId, root);
    }

    internal static WorkerRequest ValidateWorkerRequest(string path, string expectedToken)
    {
        if (!Path.IsPathFullyQualified(path) || !File.Exists(path)) throw new InvalidDataException("Worker request is unavailable.");
        var request = ExperimentEvidence.ReadSingle<WorkerRequest>(path);
        if (request.SchemaVersion != SchemaVersion || request.InvocationId == Guid.Empty || string.IsNullOrWhiteSpace(request.CorrelationToken) ||
            !FixedEquals(request.CorrelationToken, expectedToken) || request.ZeroExcelPrecondition.ExcelProcessCount != 0 ||
            request.ZeroExcelPrecondition.ObservedUtc == default || string.IsNullOrWhiteSpace(request.ZeroExcelPrecondition.AcquisitionProvenance))
            throw new InvalidDataException("Worker correlation or zero-Excel precondition mismatch.");
        ExperimentEvidence.ValidatePaths(request.Paths, request.InvocationId, Path.GetDirectoryName(Path.GetFullPath(path))!);
        return request;
    }

    private static bool FixedEquals(string left, string right) =>
        System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(System.Text.Encoding.UTF8.GetBytes(left), System.Text.Encoding.UTF8.GetBytes(right));
    private static string? FindRepositoryRoot()
    {
        foreach (var start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        for (var directory = new DirectoryInfo(Path.GetFullPath(start)); directory is not null; directory = directory.Parent)
            if (Directory.Exists(Path.Combine(directory.FullName, ".git"))) return Path.TrimEndingDirectorySeparator(directory.FullName);
        return null;
    }
    private static bool IsContainedBy(string candidate, string parent) => candidate.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
}
