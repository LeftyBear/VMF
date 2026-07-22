using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Configures supported versions and the non-secret root for verified state.</summary>
public sealed class VerifiedPublishStateStoreOptions
{
    /// <summary>Initializes verified-state storage options.</summary>
    public VerifiedPublishStateStoreOptions(
        string rootDirectory,
        string schemaVersion,
        string generatedIdAlgorithmVersion,
        string contentHashAlgorithmVersion,
        string fingerprintAlgorithmVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(generatedIdAlgorithmVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentHashAlgorithmVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(fingerprintAlgorithmVersion);

        RootDirectory = Path.GetFullPath(rootDirectory);
        SchemaVersion = schemaVersion;
        GeneratedIdAlgorithmVersion = generatedIdAlgorithmVersion;
        ContentHashAlgorithmVersion = contentHashAlgorithmVersion;
        FingerprintAlgorithmVersion = fingerprintAlgorithmVersion;
    }

    /// <summary>Gets the verified-state root directory.</summary>
    public string RootDirectory { get; }

    /// <summary>Gets the only schema version accepted by this store.</summary>
    public string SchemaVersion { get; }

    /// <summary>Gets the supported generated-identifier algorithm version.</summary>
    public string GeneratedIdAlgorithmVersion { get; }

    /// <summary>Gets the supported content-hash algorithm version.</summary>
    public string ContentHashAlgorithmVersion { get; }

    /// <summary>Gets the supported fingerprint algorithm version.</summary>
    public string FingerprintAlgorithmVersion { get; }
}

internal interface IAtomicStateFileWriter
{
    Task WriteAsync(string path, byte[] content, CancellationToken cancellationToken);
}

internal sealed class AtomicStateFileWriter : IAtomicStateFileWriter
{
    public async Task WriteAsync(string path, byte[] content, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException("A state path must have a parent directory.");
        Directory.CreateDirectory(directory);
        var temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await stream.WriteAsync(content, cancellationToken).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                stream.Flush(flushToDisk: true);
            }

            if (File.Exists(path))
            {
                File.Replace(temporaryPath, path, null, ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(temporaryPath, path);
            }
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}

/// <summary>Persists verified state as deterministic UTF-8 JSON with atomic replacement.</summary>
public sealed class JsonVerifiedPublishStateStore : IVerifiedPublishStateStore
{
    private const string FormatName = "vmf-publisher-verified-state";
    private readonly VerifiedPublishStateStoreOptions options;
    private readonly IAtomicStateFileWriter atomicWriter;

    /// <summary>Initializes a local JSON verified-state store.</summary>
    public JsonVerifiedPublishStateStore(VerifiedPublishStateStoreOptions options)
        : this(options, new AtomicStateFileWriter())
    {
    }

    internal JsonVerifiedPublishStateStore(
        VerifiedPublishStateStoreOptions options,
        IAtomicStateFileWriter atomicWriter)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.atomicWriter = atomicWriter ?? throw new ArgumentNullException(nameof(atomicWriter));
    }

    /// <inheritdoc />
    public async Task<VerifiedPublishState?> LoadAsync(
        PublishStateLoadRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        var path = GetStatePath(request.Key);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            var bytes = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
            return Restore(bytes, request);
        }
        catch (StateLifecycleException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (exception is JsonException or IOException or UnauthorizedAccessException)
        {
            throw Corrupted("Persisted publish state could not be read.", exception);
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(VerifiedPublishState state, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(state);
        cancellationToken.ThrowIfCancellationRequested();
        var key = new PublishStateKey(state.Identity.PublicationId, state.Identity.DocumentId);
        var path = GetStatePath(key);
        byte[] bytes;
        try
        {
            bytes = Serialize(state);
            _ = Restore(
                bytes,
                new PublishStateLoadRequest(key, state.Identity.GoogleDocumentId));
        }
        catch (StateLifecycleException)
        {
            throw;
        }
        catch (ArgumentException exception)
        {
            throw Corrupted("Verified publish state is not persistable.", exception);
        }

        try
        {
            await atomicWriter.WriteAsync(path, bytes, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new StateLifecycleException(
                StateErrorCodes.SaveFailed,
                "Verified publish state could not be saved atomically.",
                exception);
        }
    }

    internal string GetStatePath(PublishStateKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        using var writer = new CanonicalValueWriter();
        writer.Write("publicationId", key.PublicationId);
        writer.Write("documentId", key.DocumentId);
        var digest = Convert.ToHexString(SHA256.HashData(writer.ToArray())).ToLowerInvariant();
        return Path.Combine(options.RootDirectory, "verified-state-" + digest + ".json");
    }

    internal static byte[] Serialize(VerifiedPublishState state)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
        {
            writer.WriteStartObject();
            writer.WriteString("format", FormatName);
            writer.WriteString("schemaVersion", state.Versions.SchemaVersion);
            writer.WriteString("generatedIdAlgorithmVersion", state.Versions.GeneratedIdAlgorithmVersion);
            writer.WriteString("contentHashAlgorithmVersion", state.Versions.ContentHashAlgorithmVersion);
            writer.WriteString("fingerprintAlgorithmVersion", state.Versions.FingerprintAlgorithmVersion);
            writer.WriteString(
                "transformationSpecificationVersion",
                state.Versions.TransformationSpecificationVersion);
            writer.WriteString("publisherVersion", state.Versions.PublisherVersion);
            writer.WriteString("publicationId", state.Identity.PublicationId);
            writer.WriteString("documentId", state.Identity.DocumentId);
            if (state.Identity.GoogleDocumentId is null)
            {
                writer.WriteNull("googleDocumentId");
            }
            else
            {
                writer.WriteString("googleDocumentId", state.Identity.GoogleDocumentId);
            }

            writer.WriteString("documentState", StateToken(state.Identity.State));
            writer.WriteString("publishFingerprint", state.Fingerprint.Value);
            writer.WriteStartArray("blocks");
            for (var index = 0; index < state.Blocks.Count; index++)
            {
                var block = state.Blocks[index];
                writer.WriteStartObject();
                writer.WriteNumber("order", index);
                WriteNullableString(writer, "explicitId", block.ExplicitId);
                WriteNullableString(writer, "generatedId", block.GeneratedId);
                writer.WriteString("contentHash", block.ContentHash);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        stream.WriteByte((byte)'\n');
        return stream.ToArray();
    }

    private VerifiedPublishState Restore(byte[] bytes, PublishStateLoadRequest request)
    {
        EnsureCanonicalJsonBytes(bytes);
        using var document = JsonDocument.Parse(bytes);
        var root = document.RootElement;
        RequireObject(root);
        EnsureProperties(
            root,
            "format",
            "schemaVersion",
            "generatedIdAlgorithmVersion",
            "contentHashAlgorithmVersion",
            "fingerprintAlgorithmVersion",
            "transformationSpecificationVersion",
            "publisherVersion",
            "publicationId",
            "documentId",
            "googleDocumentId",
            "documentState",
            "publishFingerprint",
            "blocks");
        RequireExact(root, "format", FormatName);
        var schemaVersion = RequireString(root, "schemaVersion");
        if (!string.Equals(schemaVersion, options.SchemaVersion, StringComparison.Ordinal))
        {
            throw new StateLifecycleException(
                StateErrorCodes.SchemaVersionUnsupported,
                $"Publish state schema version '{schemaVersion}' is unsupported.");
        }

        var generatedVersion = RequireString(root, "generatedIdAlgorithmVersion");
        var contentVersion = RequireString(root, "contentHashAlgorithmVersion");
        var fingerprintVersion = RequireString(root, "fingerprintAlgorithmVersion");
        EnsureSupportedAlgorithm(
            generatedVersion,
            options.GeneratedIdAlgorithmVersion,
            "generated identifier");
        EnsureSupportedAlgorithm(contentVersion, options.ContentHashAlgorithmVersion, "content hash");
        EnsureSupportedAlgorithm(fingerprintVersion, options.FingerprintAlgorithmVersion, "fingerprint");

        var transformationVersion = RequireString(root, "transformationSpecificationVersion");
        var publisherVersion = RequireString(root, "publisherVersion");
        var publicationId = RequireString(root, "publicationId");
        var documentId = RequireString(root, "documentId");
        var googleDocumentId = RequireNullableString(root, "googleDocumentId");
        if (!string.Equals(publicationId, request.Key.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(documentId, request.Key.DocumentId, StringComparison.Ordinal) ||
            !string.Equals(googleDocumentId, request.ExpectedGoogleDocumentId, StringComparison.Ordinal))
        {
            throw new StateLifecycleException(
                StateErrorCodes.DocumentIdentityMismatch,
                "Persisted state does not match the requested document identity.");
        }

        var state = ParseState(RequireString(root, "documentState"));
        var fingerprintValue = RequireString(root, "publishFingerprint");
        EnsureHashValue(fingerprintValue, $"v{fingerprintVersion}:sha256:", "publish fingerprint");
        var blocks = ParseBlocks(root, generatedVersion, contentVersion);

        try
        {
            return new VerifiedPublishState(
                new DocumentIdentity(publicationId, documentId, googleDocumentId, state),
                new PublishStateVersions(
                    schemaVersion,
                    generatedVersion,
                    contentVersion,
                    fingerprintVersion,
                    transformationVersion,
                    publisherVersion),
                new PublishFingerprint(fingerprintValue),
                blocks);
        }
        catch (ArgumentException exception)
        {
            throw Corrupted("Persisted publish state contains an invalid required value.", exception);
        }
    }

    private static IReadOnlyList<BlockIdentity> ParseBlocks(
        JsonElement root,
        string generatedVersion,
        string contentVersion)
    {
        if (!root.TryGetProperty("blocks", out var blocksElement) ||
            blocksElement.ValueKind != JsonValueKind.Array)
        {
            throw Corrupted("Persisted publish state is missing its block array.");
        }

        var blocks = new List<BlockIdentity>();
        var explicitIds = new HashSet<string>(StringComparer.Ordinal);
        var generatedIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var element in blocksElement.EnumerateArray())
        {
            RequireObject(element);
            EnsureProperties(element, "order", "explicitId", "generatedId", "contentHash");
            var order = RequireInteger(element, "order");
            if (order != blocks.Count)
            {
                throw Corrupted("Persisted block order must be zero-based and contiguous.");
            }

            var explicitId = RequireNullableString(element, "explicitId");
            var generatedId = RequireNullableString(element, "generatedId");
            var contentHash = RequireString(element, "contentHash");
            if ((explicitId is null) == (generatedId is null))
            {
                throw Corrupted("Each persisted block must have exactly one explicit or generated identifier.");
            }

            if (explicitId is not null)
            {
                try
                {
                    if (!string.Equals(
                        explicitId,
                        ExplicitBlockIdRules.NormalizeOptional(explicitId, "explicitId"),
                        StringComparison.Ordinal))
                    {
                        throw Corrupted("A persisted explicit identifier is not canonical Unicode NFC.");
                    }
                }
                catch (ArgumentException exception)
                {
                    throw Corrupted("A persisted explicit identifier is invalid.", exception);
                }

                if (!explicitIds.Add(explicitId))
                {
                    throw Corrupted("Persisted state contains a duplicate ExplicitId.");
                }
            }

            if (generatedId is not null)
            {
                EnsureHashValue(generatedId, $"gid-v{generatedVersion}:sha256:", "generated identifier");
                if (!generatedIds.Add(generatedId))
                {
                    throw Corrupted("Persisted state contains a duplicate GeneratedId.");
                }
            }

            EnsureHashValue(contentHash, $"ch-v{contentVersion}:sha256:", "content hash");
            blocks.Add(new BlockIdentity(explicitId, generatedId, contentHash));
        }

        return blocks;
    }

    private static void EnsureSupportedAlgorithm(string actual, string expected, string name)
    {
        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            throw new StateLifecycleException(
                StateErrorCodes.AlgorithmVersionUnsupported,
                $"The persisted {name} algorithm version '{actual}' is unsupported.");
        }
    }

    private static void EnsureHashValue(string value, string prefix, string name)
    {
        if (value.Length != prefix.Length + 64 ||
            !value.StartsWith(prefix, StringComparison.Ordinal) ||
            value.AsSpan(prefix.Length).ContainsAnyExcept("0123456789abcdef"))
        {
            throw Corrupted($"The persisted {name} is not canonical.");
        }
    }

    private static DocumentState ParseState(string value) => value switch
    {
        "active" => DocumentState.Active,
        "missing" => DocumentState.Missing,
        "archived" => DocumentState.Archived,
        _ => throw Corrupted($"Persisted document state '{value}' is unknown."),
    };

    private static string StateToken(DocumentState value) => value switch
    {
        DocumentState.Active => "active",
        DocumentState.Missing => "missing",
        DocumentState.Archived => "archived",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown document state."),
    };

    private static void WriteNullableString(Utf8JsonWriter writer, string name, string? value)
    {
        if (value is null)
        {
            writer.WriteNull(name);
        }
        else
        {
            writer.WriteString(name, value);
        }
    }

    private static void RequireObject(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw Corrupted("Persisted publish state must use JSON objects.");
        }
    }

    private static void EnsureCanonicalJsonBytes(byte[] bytes)
    {
        if (bytes.Length == 0 ||
            bytes[^1] != (byte)'\n' ||
            bytes.AsSpan().StartsWith(Encoding.UTF8.GetPreamble()) ||
            bytes.AsSpan().Contains((byte)'\r'))
        {
            throw Corrupted("Persisted publish state is not canonical UTF-8 JSON with an LF terminator.");
        }

        try
        {
            _ = new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: true).GetString(bytes);
        }
        catch (DecoderFallbackException exception)
        {
            throw Corrupted("Persisted publish state is not valid UTF-8.", exception);
        }
    }

    private static void EnsureProperties(JsonElement element, params string[] expectedNames)
    {
        var expected = new HashSet<string>(expectedNames, StringComparer.Ordinal);
        var observed = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in element.EnumerateObject())
        {
            if (!expected.Contains(property.Name) || !observed.Add(property.Name))
            {
                throw Corrupted($"Persisted field '{property.Name}' is unknown or duplicated.");
            }
        }

        if (observed.Count != expected.Count)
        {
            throw Corrupted("Persisted publish state is missing required fields.");
        }
    }

    private static void RequireExact(JsonElement element, string name, string expected)
    {
        var actual = RequireString(element, name);
        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            throw Corrupted($"Persisted field '{name}' is invalid.");
        }
    }

    private static string RequireString(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property) ||
            property.ValueKind != JsonValueKind.String)
        {
            throw Corrupted($"Persisted field '{name}' is missing or invalid.");
        }

        var value = property.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw Corrupted($"Persisted field '{name}' must not be empty.");
        }

        return value;
    }

    private static string? RequireNullableString(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property))
        {
            throw Corrupted($"Persisted field '{name}' is missing.");
        }

        if (property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw Corrupted($"Persisted field '{name}' must be null or a non-empty string.");
        }

        return property.GetString();
    }

    private static int RequireInteger(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var property) ||
            property.ValueKind != JsonValueKind.Number ||
            !property.TryGetInt32(out var value))
        {
            throw Corrupted($"Persisted field '{name}' is missing or invalid.");
        }

        return value;
    }

    private static StateLifecycleException Corrupted(string message, Exception? exception = null) => new(
        StateErrorCodes.Corrupted,
        message,
        exception);
}
