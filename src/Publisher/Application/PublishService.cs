namespace Vmf.Publisher.Application;

/// <summary>Coordinates the Markdown-to-Google-Docs publication use case.</summary>
public sealed class PublishService : IPublishService
{
    private readonly IMarkdownDocumentLoader loader;
    private readonly IMarkdownParser parser;
    private readonly IDocumentCompiler compiler;
    private readonly IGoogleDocsPublisher publisher;

    /// <summary>Initializes the publication service.</summary>
    /// <param name="loader">The Markdown document loader.</param>
    /// <param name="parser">The Markdown parser.</param>
    /// <param name="compiler">The document compiler.</param>
    /// <param name="publisher">The Google Docs publisher.</param>
    public PublishService(
        IMarkdownDocumentLoader loader,
        IMarkdownParser parser,
        IDocumentCompiler compiler,
        IGoogleDocsPublisher publisher)
    {
        this.loader = loader;
        this.parser = parser;
        this.compiler = compiler;
        this.publisher = publisher;
    }

    /// <inheritdoc />
    public async Task<PublishResult> PublishAsync(
        PublishRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.MarkdownFilePath))
        {
            return PublishResult.Failure(new PublishError(
                "PUBLISH_INVALID_PATH",
                "Markdown file path is required."));
        }

        try
        {
            var markdown = await loader.LoadAsync(request.MarkdownFilePath, cancellationToken)
                .ConfigureAwait(false);
            var model = parser.Parse(markdown);
            var title = string.IsNullOrWhiteSpace(request.DocumentTitle)
                ? Path.GetFileNameWithoutExtension(request.MarkdownFilePath)
                : request.DocumentTitle;
            var compiled = compiler.Compile(model, title);
            var published = await publisher.PublishAsync(compiled, cancellationToken)
                .ConfigureAwait(false);

            return PublishResult.Success(published.Id, published.Url);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (FileNotFoundException exception)
        {
            return PublishResult.Failure(new PublishError("PUBLISH_FILE_NOT_FOUND", exception.Message));
        }
        catch (Exception exception)
        {
            return PublishResult.Failure(new PublishError("PUBLISH_FAILED", exception.Message));
        }
    }
}
