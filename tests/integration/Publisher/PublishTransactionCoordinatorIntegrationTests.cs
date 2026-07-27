using Vmf.Publisher.Application;
using Vmf.Publisher.Composition;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.IntegrationTests;

public sealed class PublishTransactionCoordinatorIntegrationTests : IDisposable
{
    private readonly string root = Path.Combine(
        Path.GetTempPath(),
        "vmf-publisher-transaction-integration-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task MarkdownPublish_UsesJournalLockRecoveryAndVerifiedStatePersistence()
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
        await lifecycle.ExecuteAsync(baselineCandidate, default);
        var executor = new AdapterExecutor(adapter);
        var coordinator = PublisherCompositionRoot.CreatePublishTransactionCoordinator(
            store,
            adapter,
            executor,
            root);

        var result = await coordinator.ExecuteAsync(candidate, default);
        var restored = await store.LoadAsync(Request(), default);

        Assert.True(result.PublishExecuted);
        Assert.Equal(PublishRecoveryStatus.None, result.RecoveryResult.Status);
        Assert.Equal(candidate.Fingerprint, Assert.IsType<VerifiedPublishState>(restored).Fingerprint);
        Assert.Equal(1, executor.CallCount);
        Assert.Single(Directory.GetFiles(root, "transaction-journal-*.json"));
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

    private sealed class AdapterExecutor : IPhysicalUpdateExecutor
    {
        private readonly IManagedDocumentAdapter adapter;

        internal AdapterExecutor(IManagedDocumentAdapter adapter)
        {
            this.adapter = adapter;
        }

        internal int CallCount { get; private set; }

        public async Task<PhysicalUpdateExecutionResult> ExecuteAsync(
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken)
        {
            CallCount++;
            var receipt = await adapter.ApplyAsync(plan, cancellationToken).ConfigureAwait(false);
            return new PhysicalUpdateExecutionResult(
                PhysicalUpdateExecutionStatus.Applied,
                plan.Identity.GoogleDocumentId!,
                plan.RequiredRevision.RevisionId,
                receipt.Revision.RevisionId,
                plan.Operations.Count,
                plan.Operations.Count,
                1,
                "APPLIED",
                "adapter applied");
        }
    }
}
