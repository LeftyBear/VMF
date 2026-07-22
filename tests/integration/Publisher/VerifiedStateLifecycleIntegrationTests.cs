using Vmf.Publisher.Application;
using Vmf.Publisher.Composition;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.IntegrationTests;

public sealed class VerifiedStateLifecycleIntegrationTests : IDisposable
{
    private readonly string root = Path.Combine(
        Path.GetTempPath(),
        "vmf-publisher-state-integration-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task MarkdownCandidate_VerifiesPersistsAndRestoresAsBaseline()
    {
        var document = new SimpleMarkdownParser().Parse(
            "<!-- vmf:block-id=intro -->\n# Introduction\n\nBody.\n");
        var candidate = PublisherCompositionRoot.CreatePublishCandidateBuilder().Create(
            new DocumentIdentity(
                "publication",
                "document",
                "google-document",
                DocumentState.Active),
            document,
            Options());
        var store = PublisherCompositionRoot.CreateVerifiedPublishStateStore(root);
        var lifecycle = PublisherCompositionRoot.CreateVerifiedPublishLifecycle(
            store,
            new SuccessfulApplicationVerifier());

        var result = await lifecycle.ExecuteAsync(candidate, CancellationToken.None);
        var restored = await store.LoadAsync(
            new PublishStateLoadRequest(
                new PublishStateKey("publication", "document"),
                "google-document"),
            CancellationToken.None);

        var state = Assert.IsType<VerifiedPublishState>(restored);
        Assert.Equal(DocumentState.Active, state.Identity.State);
        Assert.Equal(result.State.Fingerprint, state.Fingerprint);
        Assert.Equal(candidate.Blocks.Select(Signature), state.Blocks.Select(Signature));
        Assert.All(result.Plan.Operations, operation => Assert.Equal(DiffOperationKind.Insert, operation.Kind));
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static PublishCandidateBuildOptions Options() => new(
        "1.0.0",
        "1.0",
        "1",
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

    private sealed class SuccessfulApplicationVerifier : IPublishPlanApplicationVerifier
    {
        public Task<PublishApplicationVerification> ApplyAndVerifyAsync(
            PublishCandidate candidate,
            DiffPlan plan,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new PublishApplicationVerification(
                candidate.Identity,
                plan,
                isLogicalPlanApplied: true,
                isReadbackVerified: true,
                candidate.Fingerprint.Value,
                candidate.Blocks));
        }
    }
}
