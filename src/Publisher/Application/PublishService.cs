namespace Vmf.Publisher.Application;

/// <summary>Coordinates the Markdown-to-Google-Docs publication use case.</summary>
public sealed class PublishService : IPublishService
{
    private readonly IMarkdownDocumentLoader loader;
    private readonly IMarkdownParser parser;
    private readonly IDocumentCompiler compiler;
    private readonly IGoogleDocsPublisher publisher;
    private readonly IImageSourceResolver? imageSourceResolver;
    private readonly IImageMetadataReader? imageMetadataReader;
    private readonly IImageSizeCalculator? imageSizeCalculator;

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
        : this(loader, parser, compiler, publisher, null, null, null)
    {
    }

    /// <summary>Initializes the publication service with image preparation services.</summary>
    public PublishService(
        IMarkdownDocumentLoader loader,
        IMarkdownParser parser,
        IDocumentCompiler compiler,
        IGoogleDocsPublisher publisher,
        IImageSourceResolver? imageSourceResolver,
        IImageMetadataReader? imageMetadataReader,
        IImageSizeCalculator? imageSizeCalculator)
    {
        this.loader = loader ?? throw new ArgumentNullException(nameof(loader));
        this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
        this.compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
        this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        this.imageSourceResolver = imageSourceResolver;
        this.imageMetadataReader = imageMetadataReader;
        this.imageSizeCalculator = imageSizeCalculator;
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
            var model = await PrepareImagesAsync(
                parser.Parse(markdown),
                request.MarkdownFilePath,
                cancellationToken).ConfigureAwait(false);
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
        catch (PublishPipelineException exception)
        {
            return PublishResult.Failure(new PublishError(exception.Code, exception.Message));
        }
        catch (Exception exception)
        {
            return PublishResult.Failure(new PublishError("PUBLISH_FAILED", exception.Message));
        }
    }

    private async Task<Domain.DocumentModel> PrepareImagesAsync(
        Domain.DocumentModel model,
        string markdownFilePath,
        CancellationToken cancellationToken)
    {
        if (!model.Blocks.Any(block => block.Kind == Domain.DocumentBlockKind.Image))
        {
            return model;
        }

        if (imageSourceResolver is null || imageMetadataReader is null || imageSizeCalculator is null)
        {
            throw new PublishPipelineException(
                PublishErrorCodes.ImageMetadataReadFailed,
                "Image services are not configured.");
        }

        var blocks = new List<Domain.DocumentBlock>(model.Blocks.Count);
        foreach (var block in model.Blocks)
        {
            if (block.Kind != Domain.DocumentBlockKind.Image)
            {
                blocks.Add(block);
                continue;
            }

            var image = block.Image
                ?? throw new InvalidOperationException("An image block requires image content.");
            var resolved = await imageSourceResolver.ResolveAsync(
                image.Source,
                markdownFilePath,
                cancellationToken).ConfigureAwait(false);
            var metadata = await imageMetadataReader.ReadAsync(resolved, cancellationToken)
                .ConfigureAwait(false);
            var size = imageSizeCalculator.Calculate(metadata);
            blocks.Add(new Domain.DocumentBlock(
                new Domain.ImageBlock(image.AltText, metadata.Source, size),
                block.ExplicitId));
        }

        return new Domain.DocumentModel(blocks);
    }
}
