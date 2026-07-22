using Vmf.Publisher.Application;
using Vmf.Publisher.Composition;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.IntegrationTests;

public sealed class PhysicalUpdateLifecycleIntegrationTests : IDisposable
{
    private readonly string root = Path.Combine(
        Path.GetTempPath(),
        "vmf-publisher-physical-integration-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task MarkdownThroughReadbackAndAtomicSave_CompletesPhysicalLifecycle()
    {
        var builder = PublisherCompositionRoot.CreatePublishCandidateBuilder();
        var baselineCandidate = builder.Create(
            Identity(),
            Parse("<!-- vmf:block-id=intro -->\nBefore.\n"),
            Options());
        var candidate = builder.Create(
            Identity(),
            Parse("<!-- vmf:block-id=intro -->\nAfter.\n\nNew block.\n"),
            Options());
        var store = PublisherCompositionRoot.CreateVerifiedPublishStateStore(root);
        var adapter = new InMemoryManagedDocumentAdapter(EmptySnapshot());
        var lifecycle = PublisherCompositionRoot.CreatePhysicalUpdateLifecycle(store, adapter);
        await lifecycle.ExecuteAsync(baselineCandidate, default);

        var result = await lifecycle.ExecuteAsync(candidate, default);
        var restored = await store.LoadAsync(Request(), default);

        var state = Assert.IsType<VerifiedPublishState>(restored);
        Assert.Equal(2, adapter.ApplyCount);
        Assert.NotEmpty(result.Plan.Operations);
        Assert.Equal(2, state.Revision.Sequence);
        Assert.Equal(candidate.Fingerprint, state.Fingerprint);
        Assert.Equal(candidate.Blocks.Select(Signature), state.Blocks.Select(Signature));
    }

    [Fact]
    public async Task RevisionConflict_StopsBeforeApplyAndPreservesBaseline()
    {
        var builder = PublisherCompositionRoot.CreatePublishCandidateBuilder();
        var baselineCandidate = builder.Create(
            Identity(),
            Parse("<!-- vmf:block-id=intro -->\nBefore.\n"),
            Options());
        var candidate = builder.Create(
            Identity(),
            Parse("<!-- vmf:block-id=intro -->\nAfter.\n"),
            Options());
        var store = PublisherCompositionRoot.CreateVerifiedPublishStateStore(root);
        var adapter = new InMemoryManagedDocumentAdapter(EmptySnapshot());
        var lifecycle = PublisherCompositionRoot.CreatePhysicalUpdateLifecycle(store, adapter);
        var baselineResult = await lifecycle.ExecuteAsync(baselineCandidate, default);
        adapter.SimulateExternalEdit();

        var exception = await Assert.ThrowsAsync<PhysicalUpdateException>(
            () => lifecycle.ExecuteAsync(candidate, default));
        var restored = await store.LoadAsync(Request(), default);

        Assert.Equal(UpdateErrorCodes.RevisionConflict, exception.Code);
        Assert.Equal(1, adapter.ApplyCount);
        Assert.Equal(Revision(1), Assert.IsType<VerifiedPublishState>(restored).Revision);
        Assert.Equal(baselineResult.State.Fingerprint, restored!.Fingerprint);
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static ManagedDocumentSnapshot EmptySnapshot() => new(
        Identity(),
        new DocumentRevision("revision-0", 0),
        new DocumentTextRange(10, 10),
        "v1:sha256:" + new string('0', 64),
        Array.Empty<ManagedBlockSnapshot>());

    private static DocumentModel Parse(string markdown) => new SimpleMarkdownParser().Parse(markdown);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static DocumentRevision Revision(long sequence) => new("revision-" + sequence, sequence);

    private static PublishStateLoadRequest Request() => new(
        new PublishStateKey("publication", "document"),
        "google-document");

    private static PublishCandidateBuildOptions Options() => new(
        "1.0.0",
        "1.0",
        "2",
        [
            new(PublishFingerprintSettingNames.MarkdownInlineMaxDepth, "8"),
            new(PublishFingerprintSettingNames.MarkdownListIndentSize, "2"),
            new(PublishFingerprintSettingNames.MarkdownListMaxDepth, "6"),
            new(PublishFingerprintSettingNames.PublisherAllowImageUpscale, "false"),
            new(PublishFingerprintSettingNames.PublisherImageMaxWidthPoints, "450"),
        ]);

    private static string Signature(BlockIdentity block) => string.Join(
        "|",
        block.ExplicitId ?? "<null>",
        block.GeneratedId ?? "<null>",
        block.ContentHash);
}
