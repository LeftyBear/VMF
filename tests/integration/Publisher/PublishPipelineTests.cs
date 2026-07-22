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
            "# **Sample**\n\n- First\n  1. **Ordered**\n    - [_Deep_](https://example.com)\n\nAfter **list**.\n");
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
                operation => operation.Kind == DocumentOperationKind.UpdateTextStyle
                    && operation.InlineStyle == InlineTextStyle.Bold);
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.UpdateTextStyle
                    && operation.InlineStyle == InlineTextStyle.Italic);
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.UpdateTextStyle
                    && operation.InlineStyle == InlineTextStyle.Link
                    && operation.Url == new Uri("https://example.com/"));
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.InsertText
                    && operation.StartIndex == 27
                    && operation.Text == "After list.\n");
            Assert.Contains(
                target.Document.Operations,
                operation => operation.Kind == DocumentOperationKind.UpdateTextStyle
                    && operation.InlineStyle == InlineTextStyle.Bold
                    && operation.StartIndex == 33
                    && operation.EndIndex == 37);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task PublishAsync_CompilesTableBetweenIndependentOrdinaryBatches()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"vmf-publisher-table-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(
            tempPath,
            "Before.\n\n" +
            "Name | Status | Note\n" +
            "--- | :---: | ---:\n" +
            "**Publisher** | Active | *v1.0*\n" +
            "[Renderer](https://example.com) | Ready |\n\n" +
            "After.\n");
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
            var document = Assert.IsType<CompiledDocument>(target.Document);
            Assert.Collection(
                document.Steps,
                step => Assert.IsType<BatchUpdateStep>(step),
                step =>
                {
                    var table = Assert.IsType<InsertTableStep>(step).Table;
                    Assert.Equal(
                        [TableAlignment.Left, TableAlignment.Center, TableAlignment.Right],
                        table.Columns.Select(column => column.Alignment));
                    Assert.IsType<BoldInline>(table.Rows[0].Cells[0].Content[0]);
                    Assert.IsType<ItalicInline>(table.Rows[0].Cells[2].Content[0]);
                    Assert.IsType<LinkInline>(table.Rows[1].Cells[0].Content[0]);
                    Assert.Empty(table.Rows[1].Cells[2].Content);
                },
                step =>
                {
                    var batch = Assert.IsType<BatchUpdateStep>(step);
                    Assert.Equal(1, Assert.Single(batch.Operations).StartIndex);
                });
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task PublishAsync_CompilesFencedCodeInlineCodeAndNestedQuotes()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"vmf-publisher-code-quote-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(
            tempPath,
            "# `Code and Quote`\n\n" +
            "Use `dotnet test` before publishing.\n\n" +
            "```csharp\n" +
            "var text = \"**not bold**\";\n" +
            "Console.WriteLine(text);\n" +
            "```\n\n" +
            "> A quoted paragraph with **bold** and `inline code`.\n" +
            ">> A nested quote with [a link](https://example.com).\n\n" +
            "After quote.\n");
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
            var document = Assert.IsType<CompiledDocument>(target.Document);
            var operations = Assert.IsType<BatchUpdateStep>(Assert.Single(document.Steps)).Operations;
            Assert.Contains(
                operations,
                operation => operation.Kind == DocumentOperationKind.InsertText &&
                    operation.Text == "var text = \"**not bold**\";\nConsole.WriteLine(text);\n");
            Assert.Contains(
                operations,
                operation => operation.Kind == DocumentOperationKind.ApplyCodeBlockStyle);
            Assert.Equal(
                2,
                operations.Count(operation => operation.Kind == DocumentOperationKind.ApplyQuoteBlockStyle));
            Assert.Contains(
                operations,
                operation => operation.Kind == DocumentOperationKind.ApplyQuoteBlockStyle &&
                    operation.Level == 2);
            Assert.True(
                operations.Count(operation => operation.InlineStyle == InlineTextStyle.Code) >= 4);
            Assert.Contains(
                operations,
                operation => operation.Kind == DocumentOperationKind.InsertText &&
                    operation.Text == "After quote.\n");
            Assert.DoesNotContain(
                operations.Where(operation => operation.Kind == DocumentOperationKind.InsertText),
                operation => operation.Text?.Contains("```", StringComparison.Ordinal) == true ||
                    operation.Text?.Contains("csharp", StringComparison.Ordinal) == true);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task PublishAsync_PreservesPublishPipelineErrorCode()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"vmf-publisher-error-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(tempPath, "Paragraph.\n");
        var service = new PublishService(
            new MarkdownFileDocumentLoader(),
            new SimpleMarkdownParser(),
            new DocumentCompiler(),
            new FailingPublisher());

        try
        {
            var result = await service.PublishAsync(new PublishRequest(tempPath), CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(PublishErrorCodes.TableContentUpdateFailed, result.Error?.Code);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public async Task PublishAsync_ExplicitIdDirectiveDoesNotAlterCreateModeOutput()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"vmf-publisher-identity-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(
            tempPath,
            "<!-- vmf:block-id=intro -->\n# Heading\n\nParagraph.\n");
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
            var document = Assert.IsType<CompiledDocument>(target.Document);
            var insertedText = document.Operations
                .Where(operation => operation.Kind == DocumentOperationKind.InsertText)
                .Select(operation => operation.Text)
                .ToArray();
            Assert.Contains("Heading\n", insertedText);
            Assert.Contains("Paragraph.\n", insertedText);
            Assert.DoesNotContain(insertedText, text =>
                text?.Contains("vmf:block-id", StringComparison.Ordinal) == true);
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

    private sealed class FailingPublisher : IGoogleDocsPublisher
    {
        public Task<PublishedDocument> PublishAsync(
            CompiledDocument document,
            CancellationToken cancellationToken) => throw new PublishPipelineException(
                PublishErrorCodes.TableContentUpdateFailed,
                "Table update failed.");
    }
}
