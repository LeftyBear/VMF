using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed record LifecycleEntry(int SchemaVersion, Guid InvocationId, long Sequence, DateTimeOffset RecordedUtc, string State, Ownership? Ownership);

public sealed class LifecycleJournal
{
    public const int SchemaVersion = 2;

    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly Dictionary<string, HashSet<string>> Next = new(StringComparer.Ordinal)
    {
        ["InvocationStarted"] = ["ExcelActivationAboutToBegin", "InvocationFailed"],
        ["ExcelActivationAboutToBegin"] = ["ActivationProcessIdentityObserved", "InvocationFailed"],
        ["ActivationProcessIdentityObserved"] = ["RotAttachmentWaiting", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["RotAttachmentWaiting"] = ["RotAttachmentObserved", "RotAttachmentAmbiguous", "RotAttachmentTimedOut", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["RotAttachmentObserved"] = ["ApplicationProcessIdentityObserved", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["RotAttachmentAmbiguous"] = ["ExcelCleanupSkippedUnsafeTopology"],
        ["RotAttachmentTimedOut"] = ["ExcelCleanupSkippedUnsafeTopology"],
        ["ApplicationProcessIdentityObserved"] = ["ApplicationProcessIdentityRejected", "OwnershipEvidencePersisted", "ExcelCleanupSkippedUnsafeTopology"],
        ["OwnershipEvidencePersisted"] = ["OwnershipPersistenceConfirmed", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["OwnershipPersistenceConfirmed"] = ["ApplicationProcessIdentityValidated", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["ApplicationProcessIdentityValidated"] = ["CleanupTopologyObserved", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["CleanupTopologyObserved"] = ["CleanupTopologyRejected", "CleanupTopologyEvidencePersisted", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["CleanupTopologyRejected"] = ["ExcelCleanupSkippedUnsafeTopology"],
        ["CleanupTopologyEvidencePersisted"] = ["CleanupTopologyPersistenceConfirmed", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["CleanupTopologyPersistenceConfirmed"] = ["CleanupTopologyValidated", "ExcelCleanupSkippedUnsafeTopology", "InvocationFailed"],
        ["CleanupTopologyValidated"] = ["ExcelOperationStarted", "ExcelCleanupStarted", "InvocationFailed"],
        ["ApplicationProcessIdentityRejected"] = ["ExcelCleanupSkippedUnsafeTopology"],
        ["ExcelOperationStarted"] = ["ExcelCleanupStarted", "InvocationFailed"],
        ["ExcelCleanupStarted"] = ["ExcelCleanupActionsCompleted", "ExcelCleanupFailed"],
        ["ExcelCleanupActionsCompleted"] = ["ExcelResidualChecked"],
        ["ExcelCleanupSkippedUnsafeTopology"] = ["ExcelResidualChecked"],
        ["ExcelCleanupFailed"] = ["ExcelResidualChecked", "InvocationFailed"],
        ["ExcelResidualChecked"] = ["ExcelCleanupCompleted", "ExcelCleanupFailed", "InvocationFailed"],
        ["ExcelCleanupCompleted"] = ["InvocationSucceeded", "InvocationFailed"],
        ["LauncherDeadlineExpired"] = ["CleanupRequested"],
        ["CleanupRequested"] = ["ExcelCleanupStarted", "InvocationFailed"]
    };

    private readonly string path;
    private readonly Guid invocationId;
    private readonly HashSet<string> history = new(StringComparer.Ordinal);
    private long sequence;
    private string? state;

    public string State => state ?? "None";

    public LifecycleJournal(string path, Guid invocationId)
    {
        this.path = Path.GetFullPath(path);
        this.invocationId = invocationId;
        Directory.CreateDirectory(Path.GetDirectoryName(this.path)!);
        using var unused = new FileStream(this.path, FileMode.CreateNew);
    }

    public void Record(string nextState, Ownership? ownership = null)
    {
        if (state is null ? nextState != "InvocationStarted" : !Next.TryGetValue(state, out var allowed) || !allowed.Contains(nextState))
            throw new InvalidDataException($"Invalid lifecycle transition: {state} -> {nextState}.");
        ValidateCleanupCompletion(nextState, history);

        var entry = new LifecycleEntry(SchemaVersion, invocationId, ++sequence, DateTimeOffset.UtcNow, nextState, ownership);
        using var file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough);
        using var writer = new StreamWriter(file, new UTF8Encoding(false));
        writer.WriteLine(JsonSerializer.Serialize(entry, Json));
        writer.Flush();
        file.Flush(true);
        state = nextState;
        history.Add(nextState);
    }

    public static IReadOnlyList<LifecycleEntry> Validate(string path, Guid invocationId)
    {
        var entries = File.ReadLines(path).Select(line => JsonSerializer.Deserialize<LifecycleEntry>(line, Json) ?? throw new InvalidDataException()).ToArray();
        if (entries.Length == 0) throw new InvalidDataException("Empty lifecycle.");

        string? prior = null;
        long sequence = 0;
        var history = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entry in entries)
        {
            if (entry.SchemaVersion != SchemaVersion || entry.InvocationId != invocationId || entry.Sequence != ++sequence || entry.RecordedUtc > DateTimeOffset.UtcNow.AddMinutes(1))
                throw new InvalidDataException("Invalid lifecycle evidence.");
            if (prior is null ? entry.State != "InvocationStarted" : !Next.TryGetValue(prior, out var allowed) || !allowed.Contains(entry.State))
                throw new InvalidDataException("Invalid lifecycle transition.");
            ValidateCleanupCompletion(entry.State, history);
            prior = entry.State;
            history.Add(entry.State);
        }

        return entries;
    }

    private static void ValidateCleanupCompletion(string nextState, IReadOnlySet<string> history)
    {
        if (nextState != "ExcelCleanupCompleted") return;
        if (!history.Contains("CleanupTopologyValidated") ||
            !history.Contains("ExcelCleanupStarted") ||
            !history.Contains("ExcelCleanupActionsCompleted") ||
            !history.Contains("ExcelResidualChecked") ||
            history.Contains("ExcelCleanupFailed") ||
            history.Contains("ExcelCleanupSkippedUnsafeTopology"))
            throw new InvalidDataException("Cleanup completion requires validated attachment, successful cleanup actions, and a clear residual check.");
    }
}
