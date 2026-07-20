using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.IntegrationTests;

public sealed class PublishPipelineTests
{
    [Fact]
    public async Task PublishAsync_LoadsParsesCompilesAndPublishesDocument()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"vmf-publisher-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(
            tempPath,
            "# Sample\n\n- First\n  1. Ordered\n    - Deep\n\nAfter list.\n");
        var target = new RecordingPublisher();
        var service = new PublishService(
            new MarkdownFileDocumentLoader(),
            new SimpleMarkdownParser(),
            new DocumentCompiler(),
            target);

        try
        {
            var result = await service.PublishAsync(new PublishRequest(tempPath), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("document-id", result.DocumentId);
            Assert.NotNull(target.Document);
            Assert.Equal(Path.GetFileNameWithoutExtension(tempPath), target.Document.Title);
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.ApplyHeading);
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.CreateBullet);
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.CreateBullet
                    && operation.ListKind == ListKind.Ordered);
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.InsertText
                    && operation.StartIndex == 27
                    && operation.Text == "After list.\n");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    private sealed class RecordingPublisher : IGoogleDocsPublisher
    {
        internal CompiledDocument? Document { get; private set; }

        public Task<PublishedDocument> PublishAsync(
            CompiledDocument document,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Document = document;
            return Task.FromResult(new PublishedDocument(
                "document-id",
                "https://docs.google.com/document/d/document-id/edit"));
        }
    }
}
