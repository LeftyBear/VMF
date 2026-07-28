using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Application;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Configures local transaction journal storage.</summary>
public sealed class PublishTransactionJournalOptions
{
    /// <summary>Initializes journal options.</summary>
    public PublishTransactionJournalOptions(string rootDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        RootDirectory = Path.GetFullPath(rootDirectory);
    }

    /// <summary>Gets the non-secret journal root directory.</summary>
    public string RootDirectory { get; }
}

/// <summary>Persists publish transaction progress as deterministic atomic JSON.</summary>
public sealed class JsonPublishTransactionJournal : IPublishTransactionJournal
{
    private const string FormatName = "vmf-publisher-transaction-journal";
    private readonly PublishTransactionJournalOptions options;
    private readonly IAtomicStateFileWriter writer;

    /// <summary>Initializes a JSON transaction journal.</summary>
    public JsonPublishTransactionJournal(PublishTransactionJournalOptions options)
        : this(options, new AtomicStateFileWriter())
    {
    }

    internal JsonPublishTransactionJournal(
        PublishTransactionJournalOptions options,
        IAtomicStateFileWriter writer)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    /// <inheritdoc />
    public async Task<PublishTransactionJournalEntry?> LoadAsync(
        PublishStateKey key,
        string? expectedGoogleDocumentId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);
        var path = GetJournalPath(key);
        if (!File.Exists(path))
        {
            return null;
        }

        var bytes = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        var entry = Restore(bytes);
        if (!string.Equals(entry.Key.PublicationId, key.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(entry.Key.DocumentId, key.DocumentId, StringComparison.Ordinal) ||
            !string.Equals(entry.GoogleDocumentId, expectedGoogleDocumentId, StringComparison.Ordinal))
        {
            throw new StateLifecycleException(
                StateErrorCodes.DocumentIdentityMismatch,
                "Persisted transaction journal does not match the requested document identity.");
        }

        return entry;
    }

    /// <inheritdoc />
    public Task SaveAsync(PublishTransactionJournalEntry entry, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return writer.WriteAsync(GetJournalPath(entry.Key), Serialize(entry), cancellationToken);
    }

    /// <inheritdoc />
    public Task CompleteAsync(PublishTransactionJournalEntry entry, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return SaveAsync(entry.With(PublishTransactionStatus.Completed), cancellationToken);
    }

    internal string GetJournalPath(PublishStateKey key)
    {
        using var canonical = new CanonicalValueWriter();
        canonical.Write("publicationId", key.PublicationId);
        canonical.Write("documentId", key.DocumentId);
        var digest = Convert.ToHexString(SHA256.HashData(canonical.ToArray())).ToLowerInvariant();
        return Path.Combine(options.RootDirectory, "transaction-journal-" + digest + ".json");
    }

    internal static byte[] Serialize(PublishTransactionJournalEntry entry)
    {
        using var stream = new MemoryStream();
        using (var json = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            json.WriteStartObject();
            json.WriteString("format", FormatName);
            json.WriteString("schemaVersion", "1");
            json.WriteString("publicationId", entry.Key.PublicationId);
            json.WriteString("documentId", entry.Key.DocumentId);
            json.WriteString("transactionId", entry.TransactionId);
            if (entry.GoogleDocumentId is null)
            {
                json.WriteNull("googleDocumentId");
            }
            else
            {
                json.WriteString("googleDocumentId", entry.GoogleDocumentId);
            }

            json.WriteString("status", StatusToken(entry.Status));
            json.WriteString("candidateFingerprint", entry.CandidateFingerprint);
            WriteNullable(json, "baselineFingerprint", entry.BaselineFingerprint);
            WriteNullable(json, "requiredRevisionId", entry.RequiredRevisionId);
            WriteNullable(json, "diagnosticCode", entry.DiagnosticCode);
            json.WriteStartArray("operationIds");
            foreach (var operationId in entry.OperationIds)
            {
                json.WriteStringValue(operationId);
            }

            json.WriteEndArray();
            json.WriteEndObject();
        }

        stream.WriteByte((byte)'\n');
        return stream.ToArray();
    }

    private static PublishTransactionJournalEntry Restore(byte[] bytes)
    {
        if (bytes.Length == 0 ||
            bytes[^1] != (byte)'\n' ||
            bytes.AsSpan().StartsWith(Encoding.UTF8.GetPreamble()) ||
            bytes.AsSpan().Contains((byte)'\r'))
        {
            throw new StateLifecycleException(
                StateErrorCodes.Corrupted,
                "Persisted transaction journal is not canonical UTF-8 JSON.");
        }

        using var document = JsonDocument.Parse(bytes);
        var root = document.RootElement;
        var format = RequireString(root, "format");
        var schemaVersion = RequireString(root, "schemaVersion");
        if (!string.Equals(format, FormatName, StringComparison.Ordinal) ||
            !string.Equals(schemaVersion, "1", StringComparison.Ordinal))
        {
            throw new StateLifecycleException(StateErrorCodes.Corrupted, "Persisted transaction journal is invalid.");
        }

        return new PublishTransactionJournalEntry(
            new PublishStateKey(RequireString(root, "publicationId"), RequireString(root, "documentId")),
            RequireNullableString(root, "googleDocumentId"),
            RequireString(root, "transactionId"),
            ParseStatus(RequireString(root, "status")),
            RequireString(root, "candidateFingerprint"),
            RequireNullableString(root, "baselineFingerprint"),
            RequireNullableString(root, "requiredRevisionId"),
            RequireNullableString(root, "diagnosticCode"),
            RestoreOperationIds(root));
    }

    private static IReadOnlyList<string> RestoreOperationIds(JsonElement root)
    {
        if (!root.TryGetProperty("operationIds", out var property))
        {
            return Array.Empty<string>();
        }

        if (property.ValueKind != JsonValueKind.Array)
        {
            throw new StateLifecycleException(StateErrorCodes.Corrupted, "Journal field 'operationIds' is invalid.");
        }

        var values = new List<string>();
        foreach (var item in property.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(item.GetString()))
            {
                throw new StateLifecycleException(
                    StateErrorCodes.Corrupted,
                    "Journal field 'operationIds' contains an invalid value.");
            }

            values.Add(item.GetString()!);
        }

        return values;
    }

    private static void WriteNullable(Utf8JsonWriter json, string name, string? value)
    {
        if (value is null)
        {
            json.WriteNull(name);
        }
        else
        {
            json.WriteString(name, value);
        }
    }

    private static string RequireString(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var property) ||
            property.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new StateLifecycleException(StateErrorCodes.Corrupted, $"Journal field '{name}' is invalid.");
        }

        return property.GetString()!;
    }

    private static string? RequireNullableString(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var property))
        {
            throw new StateLifecycleException(StateErrorCodes.Corrupted, $"Journal field '{name}' is missing.");
        }

        return property.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.String when !string.IsNullOrWhiteSpace(property.GetString()) => property.GetString(),
            _ => throw new StateLifecycleException(StateErrorCodes.Corrupted, $"Journal field '{name}' is invalid."),
        };
    }

    private static string StatusToken(PublishTransactionStatus status) => status switch
    {
        PublishTransactionStatus.Started => "started",
        PublishTransactionStatus.Planned => "planned",
        PublishTransactionStatus.CommitUnknown => "commitUnknown",
        PublishTransactionStatus.StatePersistencePending => "statePersistencePending",
        PublishTransactionStatus.Completed => "completed",
        PublishTransactionStatus.ReplanRequired => "replanRequired",
        PublishTransactionStatus.Diverged => "diverged",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown transaction status."),
    };

    private static PublishTransactionStatus ParseStatus(string value) => value switch
    {
        "started" => PublishTransactionStatus.Started,
        "planned" => PublishTransactionStatus.Planned,
        "commitUnknown" => PublishTransactionStatus.CommitUnknown,
        "statePersistencePending" => PublishTransactionStatus.StatePersistencePending,
        "completed" => PublishTransactionStatus.Completed,
        "replanRequired" => PublishTransactionStatus.ReplanRequired,
        "diverged" => PublishTransactionStatus.Diverged,
        _ => throw new StateLifecycleException(StateErrorCodes.Corrupted, "Persisted transaction status is unknown."),
    };
}

/// <summary>Configures document lock file storage.</summary>
public sealed class DocumentLockFileOptions
{
    /// <summary>Initializes lock options.</summary>
    public DocumentLockFileOptions(string rootDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        RootDirectory = Path.GetFullPath(rootDirectory);
    }

    /// <summary>Gets the lock root directory.</summary>
    public string RootDirectory { get; }
}

/// <summary>Raised when a document lock cannot be safely acquired or released.</summary>
public sealed class DocumentLockException : Exception
{
    /// <summary>Initializes a document lock exception.</summary>
    public DocumentLockException(string code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    /// <summary>Gets the stable lock failure code.</summary>
    public string Code { get; }
}

/// <summary>Provides interprocess document locks using exclusive lock-file creation.</summary>
public sealed class FileDocumentPublishLockManager : IDocumentPublishLockManager
{
    private const string FormatName = "vmf-publisher-document-lock";
    private readonly DocumentLockFileOptions options;

    /// <summary>Initializes a lock manager.</summary>
    public FileDocumentPublishLockManager(DocumentLockFileOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public Task<IDocumentPublishLock> AcquireAsync(
        PublishStateKey key,
        CancellationToken cancellationToken) => AcquireAsync(key, Guid.NewGuid().ToString("N"), cancellationToken);

    /// <inheritdoc />
    public async Task<IDocumentPublishLock> AcquireAsync(
        PublishStateKey key,
        string transactionId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(transactionId);
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(options.RootDirectory);
        var path = GetLockPath(key.DocumentId);
        var info = new DocumentLockFile(
            Guid.NewGuid().ToString("N"),
            key.DocumentId,
            transactionId,
            Environment.ProcessId,
            Environment.MachineName,
            DateTimeOffset.UtcNow);
        var bytes = Serialize(info);
        try
        {
            await using (var stream = new FileStream(
                path,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.Read,
                4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await stream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                stream.Flush(flushToDisk: true);
            }
        }
        catch (IOException exception) when (File.Exists(path))
        {
            try
            {
                _ = Restore(await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false));
            }
            catch (DocumentLockException corrupted)
            {
                throw new DocumentLockException(
                    corrupted.Code,
                    "A malformed document lock exists. The existing lock was preserved.",
                    corrupted);
            }

            throw new DocumentLockException(
                "LOCK_ALREADY_HELD",
                "A document lock already exists. The existing lock was preserved.",
                exception);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new DocumentLockException(
                "LOCK_ACQUIRE_FAILED",
                "The document lock could not be acquired.",
                exception);
        }

        DocumentLockFile persisted;
        try
        {
            persisted = Restore(await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false));
        }
        catch (DocumentLockException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new DocumentLockException(
                "LOCK_VERIFY_FAILED",
                "The acquired document lock could not be reread for verification.",
                exception);
        }

        if (!SameOwner(info, persisted))
        {
            throw new DocumentLockException(
                "LOCK_VERIFY_FAILED",
                "The acquired document lock did not match the expected owner.");
        }

        return new FileDocumentPublishLock(path, persisted);
    }

    internal string GetLockPath(string documentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        using var canonical = new CanonicalValueWriter();
        canonical.Write("documentId", documentId);
        var digest = Convert.ToHexString(SHA256.HashData(canonical.ToArray())).ToLowerInvariant();
        return Path.Combine(options.RootDirectory, "document-lock-" + digest + ".json");
    }

    internal static byte[] Serialize(DocumentLockFile info)
    {
        using var stream = new MemoryStream();
        using (var json = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            json.WriteStartObject();
            json.WriteString("format", FormatName);
            json.WriteString("schemaVersion", "1");
            json.WriteString("lockId", info.LockId);
            json.WriteString("documentId", info.DocumentId);
            json.WriteString("transactionId", info.TransactionId);
            json.WriteNumber("processId", info.ProcessId);
            json.WriteString("hostId", info.HostId);
            json.WriteString("acquiredAtUtc", info.AcquiredAtUtc.ToString("O"));
            json.WriteString("integrityHash", ComputeIntegrityHash(info));
            json.WriteEndObject();
        }

        stream.WriteByte((byte)'\n');
        return stream.ToArray();
    }

    internal static DocumentLockFile Restore(byte[] bytes)
    {
        if (bytes.Length == 0 ||
            bytes[^1] != (byte)'\n' ||
            bytes.AsSpan().StartsWith(Encoding.UTF8.GetPreamble()) ||
            bytes.AsSpan().Contains((byte)'\r'))
        {
            throw new DocumentLockException("LOCK_CORRUPTED", "The document lock is not canonical UTF-8 JSON.");
        }

        using var document = JsonDocument.Parse(bytes);
        var root = document.RootElement;
        if (!string.Equals(RequireString(root, "format"), FormatName, StringComparison.Ordinal) ||
            !string.Equals(RequireString(root, "schemaVersion"), "1", StringComparison.Ordinal))
        {
            throw new DocumentLockException("LOCK_CORRUPTED", "The document lock format is invalid.");
        }

        var info = new DocumentLockFile(
            RequireString(root, "lockId"),
            RequireString(root, "documentId"),
            RequireString(root, "transactionId"),
            RequireInt(root, "processId"),
            RequireString(root, "hostId"),
            DateTimeOffset.Parse(RequireString(root, "acquiredAtUtc"), null, System.Globalization.DateTimeStyles.RoundtripKind));
        var hash = RequireString(root, "integrityHash");
        if (!string.Equals(hash, ComputeIntegrityHash(info), StringComparison.Ordinal))
        {
            throw new DocumentLockException("LOCK_CORRUPTED", "The document lock integrity hash is invalid.");
        }

        return info;
    }

    private static bool SameOwner(DocumentLockFile expected, DocumentLockFile actual) =>
        string.Equals(expected.LockId, actual.LockId, StringComparison.Ordinal) &&
        string.Equals(expected.DocumentId, actual.DocumentId, StringComparison.Ordinal) &&
        string.Equals(expected.TransactionId, actual.TransactionId, StringComparison.Ordinal) &&
        expected.ProcessId == actual.ProcessId &&
        string.Equals(expected.HostId, actual.HostId, StringComparison.Ordinal);

    private static string ComputeIntegrityHash(DocumentLockFile info)
    {
        using var canonical = new CanonicalValueWriter();
        canonical.Write("lockId", info.LockId);
        canonical.Write("documentId", info.DocumentId);
        canonical.Write("transactionId", info.TransactionId);
        canonical.Write("processId", info.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        canonical.Write("hostId", info.HostId);
        canonical.Write("acquiredAtUtc", info.AcquiredAtUtc.ToString("O"));
        return "sha256:" + Convert.ToHexString(SHA256.HashData(canonical.ToArray())).ToLowerInvariant();
    }

    private static string RequireString(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var property) ||
            property.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new DocumentLockException("LOCK_CORRUPTED", $"Lock field '{name}' is invalid.");
        }

        return property.GetString()!;
    }

    private static int RequireInt(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var property) ||
            property.ValueKind != JsonValueKind.Number ||
            !property.TryGetInt32(out var value))
        {
            throw new DocumentLockException("LOCK_CORRUPTED", $"Lock field '{name}' is invalid.");
        }

        return value;
    }

    private sealed class FileDocumentPublishLock : IDocumentPublishLock
    {
        private readonly string path;
        private readonly DocumentLockFile owner;
        private bool disposed;

        internal FileDocumentPublishLock(string path, DocumentLockFile owner)
        {
            this.path = path;
            this.owner = owner;
        }

        public string LockId => owner.LockId;

        public string DocumentId => owner.DocumentId;

        public string TransactionId => owner.TransactionId;

        public async ValueTask DisposeAsync()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            DocumentLockFile current;
            try
            {
                current = Restore(await File.ReadAllBytesAsync(path).ConfigureAwait(false));
            }
            catch (FileNotFoundException exception)
            {
                throw new DocumentLockException(
                    "LOCK_RELEASE_NOT_OWNER",
                    "The document lock disappeared before owner release.",
                    exception);
            }

            if (!SameOwner(owner, current))
            {
                throw new DocumentLockException(
                    "LOCK_RELEASE_NOT_OWNER",
                    "The document lock owner did not match before release.");
            }

            File.Delete(path);
        }
    }

    internal sealed record DocumentLockFile(
        string LockId,
        string DocumentId,
        string TransactionId,
        int ProcessId,
        string HostId,
        DateTimeOffset AcquiredAtUtc);
}
