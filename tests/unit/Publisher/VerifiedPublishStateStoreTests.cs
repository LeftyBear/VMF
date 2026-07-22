using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class VerifiedPublishStateStoreTests : IDisposable
{
    private readonly string root = Path.Combine(
        Path.GetTempPath(),
        "vmf-publisher-state-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task LoadAsync_RestoresValidState()
    {
        var store = Store();
        var state = State();
        await store.SaveAsync(state, CancellationToken.None);

        var restored = await store.LoadAsync(Request(), CancellationToken.None);

        AssertStateEqual(state, Assert.IsType<VerifiedPublishState>(restored));
    }

    [Fact]
    public async Task LoadAsync_ReturnsNullWhenStateDoesNotExist()
    {
        var restored = await Store().LoadAsync(Request(), CancellationToken.None);

        Assert.Null(restored);
    }

    [Fact]
    public async Task LoadAsync_RejectsMalformedJson()
    {
        var store = Store();
        await WriteRawAsync(store, "{not-json");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsMissingRequiredField()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, "\"publisherVersion\":\"1.0.0\",", string.Empty);

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsUnsupportedSchemaVersion()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, "\"schemaVersion\":\"1\"", "\"schemaVersion\":\"0\"");

        await AssertCodeAsync(
            StateErrorCodes.SchemaVersionUnsupported,
            () => store.LoadAsync(Request(), default));
    }

    [Theory]
    [InlineData("publicationId", "publication", "other-publication")]
    [InlineData("documentId", "document", "other-document")]
    public async Task LoadAsync_RejectsLogicalIdentityMismatch(
        string property,
        string current,
        string replacement)
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(
            store,
            $"\"{property}\":\"{current}\"",
            $"\"{property}\":\"{replacement}\"");

        await AssertCodeAsync(
            StateErrorCodes.DocumentIdentityMismatch,
            () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsDifferentGoogleDocumentId()
    {
        var store = await SavedStoreAsync();

        await AssertCodeAsync(
            StateErrorCodes.DocumentIdentityMismatch,
            () => store.LoadAsync(Request("other-google-document"), default));
    }

    [Fact]
    public async Task LoadAsync_UsesOrdinalDocumentIdentityComparison()
    {
        var store = await SavedStoreAsync();

        await AssertCodeAsync(
            StateErrorCodes.DocumentIdentityMismatch,
            () => store.LoadAsync(Request("Google-document"), default));
    }

    [Fact]
    public async Task LoadAsync_AllowsBothGoogleDocumentIdsNull()
    {
        var store = Store();
        await store.SaveAsync(State(googleDocumentId: null), default);

        var restored = await store.LoadAsync(Request(null), default);

        Assert.Null(Assert.IsType<VerifiedPublishState>(restored).Identity.GoogleDocumentId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task LoadAsync_RejectsOneSidedNullGoogleDocumentId(bool persistedIsNull)
    {
        var store = Store();
        await store.SaveAsync(State(googleDocumentId: persistedIsNull ? null : "google-document"), default);

        await AssertCodeAsync(
            StateErrorCodes.DocumentIdentityMismatch,
            () => store.LoadAsync(Request(persistedIsNull ? "google-document" : null), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsInvalidFingerprint()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, Fingerprint('a'), "v1:sha256:ABCDEF");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsDuplicateExplicitId()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, "\"explicitId\":null", "\"explicitId\":\"intro\"");
        await ReplaceAsync(store, $"\"generatedId\":\"{Generated('b')}\"", "\"generatedId\":null");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsDuplicateGeneratedId()
    {
        var store = Store();
        var state = State(blocks:
        [
            new BlockIdentity(null, Generated('b'), Content('c')),
            new BlockIdentity(null, Generated('d'), Content('e')),
        ]);
        await store.SaveAsync(state, default);
        await ReplaceAsync(store, Generated('d'), Generated('b'));

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsInvalidContentHash()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, Content('c'), "content-hash");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsInvalidExplicitId()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, "\"explicitId\":\"intro\"", "\"explicitId\":\"bad id\"");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsInvalidGeneratedId()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, Generated('b'), "generated-id");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsUnknownDocumentState()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, "\"documentState\":\"active\"", "\"documentState\":\"invalid\"");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task LoadAsync_RejectsNonContiguousBlockOrder()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(store, "\"order\":1", "\"order\":4");

        await AssertCodeAsync(StateErrorCodes.Corrupted, () => store.LoadAsync(Request(), default));
    }

    [Fact]
    public async Task SaveAsync_OverwritesSameKeyAndLoadReturnsNewValue()
    {
        var store = Store();
        await store.SaveAsync(State(fingerprintDigit: 'a'), default);
        await store.SaveAsync(State(fingerprintDigit: 'f'), default);

        var restored = await store.LoadAsync(Request(), default);

        Assert.Equal(Fingerprint('f'), Assert.IsType<VerifiedPublishState>(restored).Fingerprint.Value);
        Assert.Empty(Directory.GetFiles(root, "*.tmp"));
    }

    [Theory]
    [InlineData(DocumentState.Active)]
    [InlineData(DocumentState.Missing)]
    [InlineData(DocumentState.Archived)]
    public async Task SaveAndLoad_PreservesEveryPersistentDocumentState(DocumentState documentState)
    {
        var store = Store();
        var state = State(documentState: documentState);

        await store.SaveAsync(state, default);
        var restored = await store.LoadAsync(Request(), default);

        Assert.Equal(documentState, Assert.IsType<VerifiedPublishState>(restored).Identity.State);
    }

    [Fact]
    public async Task SaveAsync_FailurePreservesExistingState()
    {
        var healthy = Store();
        await healthy.SaveAsync(State(fingerprintDigit: 'a'), default);
        var failing = new JsonVerifiedPublishStateStore(Options(), new FailingAtomicWriter());

        await AssertCodeAsync(
            StateErrorCodes.SaveFailed,
            () => failing.SaveAsync(State(fingerprintDigit: 'f'), default));
        var restored = await healthy.LoadAsync(Request(), default);

        Assert.Equal(Fingerprint('a'), Assert.IsType<VerifiedPublishState>(restored).Fingerprint.Value);
    }

    [Fact]
    public async Task SaveAsync_UnsupportedSchemaDoesNotReplaceExistingState()
    {
        var store = Store();
        await store.SaveAsync(State(fingerprintDigit: 'a'), default);
        var unsupported = new VerifiedPublishState(
            new DocumentIdentity("publication", "document", "google-document", DocumentState.Active),
            new PublishStateVersions("0", "1", "1", "1", "1.0", "1.0.0"),
            new PublishFingerprint(Fingerprint('f')),
            [new BlockIdentity("intro", null, Content('c'))]);

        await AssertCodeAsync(
            StateErrorCodes.SchemaVersionUnsupported,
            () => store.SaveAsync(unsupported, default));
        var restored = await store.LoadAsync(Request(), default);

        Assert.Equal(Fingerprint('a'), Assert.IsType<VerifiedPublishState>(restored).Fingerprint.Value);
    }

    [Fact]
    public async Task SaveAsync_SeparatesPublicationAndDocumentKeys()
    {
        var store = Store();
        await store.SaveAsync(State("publication-a", "document", fingerprintDigit: 'a'), default);
        await store.SaveAsync(State("publication", "document-b", fingerprintDigit: 'b'), default);

        var first = await store.LoadAsync(Request("google-document", "publication-a", "document"), default);
        var second = await store.LoadAsync(Request("google-document", "publication", "document-b"), default);

        Assert.Equal(Fingerprint('a'), Assert.IsType<VerifiedPublishState>(first).Fingerprint.Value);
        Assert.Equal(Fingerprint('b'), Assert.IsType<VerifiedPublishState>(second).Fingerprint.Value);
        Assert.Equal(2, Directory.GetFiles(root, "*.json").Length);
    }

    [Fact]
    public void Serialize_IsDeterministicUtf8WithoutBomAndWithLfTerminator()
    {
        var first = JsonVerifiedPublishStateStore.Serialize(State());
        var second = JsonVerifiedPublishStateStore.Serialize(State());

        Assert.Equal(first, second);
        Assert.False(first.AsSpan().StartsWith(Encoding.UTF8.GetPreamble()));
        Assert.Equal((byte)'\n', first[^1]);
        Assert.DoesNotContain((byte)'\r', first);
        Assert.Contains("\"googleDocumentId\":\"google-document\"", Encoding.UTF8.GetString(first));
    }

    [Fact]
    public void Serialize_DistinguishesNullFromEmptyAndRejectsEmptyAtModelBoundary()
    {
        var json = Encoding.UTF8.GetString(JsonVerifiedPublishStateStore.Serialize(State(googleDocumentId: null)));

        Assert.Contains("\"googleDocumentId\":null", json);
        Assert.Throws<ArgumentException>(() => State(googleDocumentId: string.Empty));
    }

    [Fact]
    public async Task LoadAsync_RejectsUnsupportedAlgorithmVersion()
    {
        var store = await SavedStoreAsync();
        await ReplaceAsync(
            store,
            "\"generatedIdAlgorithmVersion\":\"1\"",
            "\"generatedIdAlgorithmVersion\":\"2\"");

        await AssertCodeAsync(
            StateErrorCodes.AlgorithmVersionUnsupported,
            () => store.LoadAsync(Request(), default));
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private JsonVerifiedPublishStateStore Store() => new(Options());

    private VerifiedPublishStateStoreOptions Options() => new(root, "1", "1", "1", "1");

    private async Task<JsonVerifiedPublishStateStore> SavedStoreAsync()
    {
        var store = Store();
        await store.SaveAsync(State(), default);
        return store;
    }

    private static VerifiedPublishState State(
        string publicationId = "publication",
        string documentId = "document",
        string? googleDocumentId = "google-document",
        char fingerprintDigit = 'a',
        DocumentState documentState = DocumentState.Active,
        IReadOnlyList<BlockIdentity>? blocks = null) => new(
            new DocumentIdentity(publicationId, documentId, googleDocumentId, documentState),
            Versions(),
            new PublishFingerprint(Fingerprint(fingerprintDigit)),
            blocks ??
            [
                new BlockIdentity("intro", null, Content('c')),
                new BlockIdentity(null, Generated('b'), Content('d')),
            ]);

    private static PublishStateVersions Versions() => new("1", "1", "1", "1", "1.0", "1.0.0");

    private static PublishStateLoadRequest Request(
        string? googleDocumentId = "google-document",
        string publicationId = "publication",
        string documentId = "document") => new(
            new PublishStateKey(publicationId, documentId),
            googleDocumentId);

    private static string Fingerprint(char digit) => "v1:sha256:" + new string(digit, 64);

    private static string Generated(char digit) => "gid-v1:sha256:" + new string(digit, 64);

    private static string Content(char digit) => "ch-v1:sha256:" + new string(digit, 64);

    private static void AssertStateEqual(VerifiedPublishState expected, VerifiedPublishState actual)
    {
        Assert.Equal(expected.Identity.PublicationId, actual.Identity.PublicationId);
        Assert.Equal(expected.Identity.DocumentId, actual.Identity.DocumentId);
        Assert.Equal(expected.Identity.GoogleDocumentId, actual.Identity.GoogleDocumentId);
        Assert.Equal(expected.Identity.State, actual.Identity.State);
        Assert.Equal(expected.Fingerprint, actual.Fingerprint);
        Assert.Equal(expected.Versions.SchemaVersion, actual.Versions.SchemaVersion);
        Assert.Equal(
            expected.Blocks.Select(BlockSignature),
            actual.Blocks.Select(BlockSignature));
    }

    private static string BlockSignature(BlockIdentity block) => string.Join(
        "|",
        block.ExplicitId ?? "<null>",
        block.GeneratedId ?? "<null>",
        block.ContentHash);

    private static async Task AssertCodeAsync(string code, Func<Task> action)
    {
        var exception = await Assert.ThrowsAsync<StateLifecycleException>(action);
        Assert.Equal(code, exception.Code);
    }

    private static async Task WriteRawAsync(JsonVerifiedPublishStateStore store, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(store.GetStatePath(Request().Key))!);
        await File.WriteAllTextAsync(
            store.GetStatePath(Request().Key),
            content,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static async Task ReplaceAsync(
        JsonVerifiedPublishStateStore store,
        string oldValue,
        string newValue)
    {
        var path = store.GetStatePath(Request().Key);
        var content = await File.ReadAllTextAsync(path, Encoding.UTF8);
        Assert.Contains(oldValue, content);
        await File.WriteAllTextAsync(
            path,
            content.Replace(oldValue, newValue, StringComparison.Ordinal),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private sealed class FailingAtomicWriter : IAtomicStateFileWriter
    {
        public Task WriteAsync(string path, byte[] content, CancellationToken cancellationToken) =>
            throw new IOException("Injected save failure.");
    }
}
