using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using Vmf.Build.ExcelOwnership.Primitives;

namespace Vmf.Build.ExcelOwnership.Experiment;

internal sealed class ExperimentEvidence
{
    internal static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter(), new IntPtrJsonConverter() }
    };

    private readonly Guid invocationId;
    private readonly EvidencePaths paths;
    private long controllerSequence;
    private long workerSequence;

    internal ExperimentEvidence(Guid invocationId, EvidencePaths paths)
    {
        this.invocationId = invocationId;
        this.paths = paths;
    }

    internal void ControllerState(string state, string? detail = null) => Append(paths.ControllerLifecycle, new ControllerLifecycleEntry(ExperimentContracts.SchemaVersion, invocationId, ++controllerSequence, DateTimeOffset.UtcNow, state, detail));
    internal void WorkerEvent(string name, string? detail = null) => Append(paths.ExperimentLog, new DiagnosticEvent(ExperimentContracts.SchemaVersion, invocationId, ++workerSequence, DateTimeOffset.UtcNow, name, detail));

    internal static void WriteSingle<T>(string path, T value)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, Json));
        using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough);
        file.Write(bytes);
        file.Flush(true);
    }

    internal static void ReplaceSingle<T>(string path, T value)
    {
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            WriteSingle(temporary, value);
            File.Move(temporary, path, true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    internal static T ReadSingle<T>(string path) where T : class
    {
        var text = File.ReadAllText(path, Encoding.UTF8);
        if (string.IsNullOrWhiteSpace(text) || text.Contains('\n') || text.Contains('\r')) throw new InvalidDataException("Evidence must be one complete compact JSON object.");
        return JsonSerializer.Deserialize<T>(text, Json) ?? throw new InvalidDataException("Malformed evidence.");
    }

    internal static IReadOnlyList<T> ValidateJsonLines<T>(string path, Guid invocationId, Func<T, int> schema, Func<T, Guid> correlation, Func<T, long> sequence, Func<T, DateTimeOffset> timestamp)
    {
        if (!File.Exists(path)) throw new InvalidDataException("Evidence file is missing.");
        var bytes = File.ReadAllBytes(path);
        if (bytes.Length == 0 || bytes[^1] != (byte)'\n') throw new InvalidDataException("Partial JSONL evidence.");
        var physicalLines = File.ReadAllLines(path, Encoding.UTF8);
        if (physicalLines.Length == 0 || physicalLines.Any(string.IsNullOrWhiteSpace)) throw new InvalidDataException("Empty or partial JSONL evidence.");
        var result = new List<T>();
        DateTimeOffset? previous = null;
        long expected = 0;
        foreach (var line in physicalLines)
        {
            var item = JsonSerializer.Deserialize<T>(line, Json) ?? throw new InvalidDataException("Malformed JSONL evidence.");
            var recorded = timestamp(item);
            if (schema(item) != ExperimentContracts.SchemaVersion || correlation(item) != invocationId || sequence(item) != ++expected || recorded > DateTimeOffset.UtcNow.AddMinutes(1) || previous > recorded)
                throw new InvalidDataException("Stale, mismatched, or non-contiguous evidence.");
            previous = recorded;
            result.Add(item);
        }
        return result;
    }

    internal static void ValidatePaths(EvidencePaths paths, Guid invocationId, string root)
    {
        var canonicalRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        var expected = CreatePaths(canonicalRoot, invocationId);
        if (paths != expected) throw new InvalidDataException("Worker evidence paths are mismatched.");
    }

    internal static EvidencePaths CreatePaths(string root, Guid invocationId)
    {
        var prefix = invocationId.ToString("D");
        return new EvidencePaths(
            Path.Combine(root, $"{prefix}.result.json"),
            Path.Combine(root, $"{prefix}.controller.lifecycle.jsonl"),
            Path.Combine(root, $"{prefix}.ownership.lifecycle.jsonl"),
            Path.Combine(root, $"{prefix}.experiment.jsonl"),
            Path.Combine(root, $"{prefix}.ownership-validation.json"),
            Path.Combine(root, $"{prefix}.cleanup-request.json"),
            Path.Combine(root, $"{prefix}.sentinel.xlsx"));
    }

    internal static string OwnershipPersistedPath(EvidencePaths paths) => Sibling(paths.Result, "ownership-persisted.json");
    internal static string CleanupTopologyPath(EvidencePaths paths) => Sibling(paths.Result, "cleanup-topology.json");
    internal static string CleanupTopologyPersistedPath(EvidencePaths paths) => Sibling(paths.Result, "cleanup-topology-persisted.json");
    internal static string CleanupAttemptPath(EvidencePaths paths) => Sibling(paths.Result, "cleanup-attempt.json");
    internal static string ResidualObservationPath(EvidencePaths paths) => Sibling(paths.Result, "residual-observation.json");

    internal static EvidencePersistenceConfirmation PersistAndConfirm<T>(string targetPath, string confirmationPath, T value,
        Guid invocationId, string artifactKind, DateTimeOffset recordedUtc)
    {
        WriteSingle(targetPath, value);
        var bytes = File.ReadAllBytes(targetPath);
        if (bytes.Length == 0 || bytes.Contains((byte)'\n') || bytes.Contains((byte)'\r')) throw new InvalidDataException("Persisted evidence readback is invalid.");
        _ = JsonDocument.Parse(bytes);
        var sha256 = Convert.ToHexString(SHA256.HashData(bytes));
        var confirmedUtc = DateTimeOffset.UtcNow;
        var record = new EvidencePersistenceConfirmationRecord(ExperimentContracts.SchemaVersion, invocationId,
            artifactKind + "PersistenceConfirmation", Path.GetFileName(targetPath), bytes.LongLength, sha256, recordedUtc, confirmedUtc);
        WriteSingle(confirmationPath, record);
        var confirmationBytes = File.ReadAllBytes(confirmationPath);
        var readback = ReadSingle<EvidencePersistenceConfirmationRecord>(confirmationPath);
        if (readback != record || confirmationBytes.Length == 0) throw new InvalidDataException("Persistence confirmation readback is invalid.");
        return new EvidencePersistenceConfirmation(invocationId, artifactKind, Path.GetFullPath(targetPath), bytes.LongLength, sha256, recordedUtc, confirmedUtc);
    }

    private static string Sibling(string resultPath, string suffix)
    {
        var file = Path.GetFileName(resultPath);
        const string resultSuffix = ".result.json";
        if (!file.EndsWith(resultSuffix, StringComparison.Ordinal)) throw new InvalidDataException("Result path shape is invalid.");
        return Path.Combine(Path.GetDirectoryName(resultPath)!, file[..^resultSuffix.Length] + "." + suffix);
    }

    private static void Append<T>(string path, T value)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, Json) + Environment.NewLine);
        using var file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough);
        file.Write(bytes);
        file.Flush(true);
    }

    private sealed class IntPtrJsonConverter : JsonConverter<IntPtr>
    {
        public override IntPtr Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number || !reader.TryGetInt64(out var value))
                throw new JsonException("HWND must be a valid JSON integer.");

            try
            {
                return checked((nint)value);
            }
            catch (OverflowException exception)
            {
                throw new JsonException("HWND is outside the platform IntPtr range.", exception);
            }
        }

        public override void Write(Utf8JsonWriter writer, IntPtr value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value.ToInt64());
    }
}
