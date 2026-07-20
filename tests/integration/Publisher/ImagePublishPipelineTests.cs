using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.IntegrationTests;

public sealed class ImagePublishPipelineTests
{
    [Fact]
    public async Task PublishAsync_LocalImageIsResolvedSizedAndSplitBetweenBatches()
    {
        var directory = CreateTemporaryDirectory();
        var markdownPath = Path.Combine(directory, "document.md");
        var imagePath = Path.Combine(directory, "image.png");
        await File.WriteAllTextAsync(
            markdownPath,
            "Before.\n\n![Local alt](image.png)\n\nAfter.\n");
        await File.WriteAllBytesAsync(imagePath, CreatePng(1200, 600));
        var target = new RecordingPublisher();
        var resolver = new ImageSourceResolver();
        using var httpClient = new HttpClient(new RejectingHandler());
        var publisherOptions = new PublisherOptions
        {
            ImageMaxWidthPoints = 450,
            AllowImageUpscale = false,
        };
        var service = new PublishService(
            new MarkdownFileDocumentLoader(),
            new SimpleMarkdownParser(),
            new DocumentCompiler(),
            target,
            resolver,
            new ImageMetadataReader(httpClient, resolver),
            new ImageSizeCalculator(publisherOptions));

        try
        {
            var result = await service.PublishAsync(
                new PublishRequest(markdownPath), CancellationToken.None);

            Assert.True(result.IsSuccess);
            var document = Assert.IsType<CompiledDocument>(target.Document);
            Assert.Collection(
                document.Steps,
                step => Assert.IsType<BatchUpdateStep>(step),
                step =>
                {
                    var image = Assert.IsType<InsertImageStep>(step).Image;
                    Assert.Equal("Local alt", image.AltText);
                    Assert.Equal(Path.GetFullPath(imagePath),
                        Assert.IsType<LocalImageSource>(image.Source).Path);
                    Assert.Equal(450, image.Size?.WidthPoints);
                    Assert.Equal(225, image.Size?.HeightPoints);
                },
                step => Assert.IsType<BatchUpdateStep>(step));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task PublishAsync_RemoteImagePreservesAltAndCalculatedAspectRatio()
    {
        var directory = CreateTemporaryDirectory();
        var markdownPath = Path.Combine(directory, "document.md");
        await File.WriteAllTextAsync(
            markdownPath,
            "![Remote alt](https://example.com/image.png)\n");
        var target = new RecordingPublisher();
        var finalSource = new RemoteImageSource(new Uri("https://cdn.example.com/final.png"));
        var service = new PublishService(
            new MarkdownFileDocumentLoader(),
            new SimpleMarkdownParser(),
            new DocumentCompiler(),
            target,
            new PassthroughResolver(),
            new FixedMetadataReader(new ImageMetadata(
                finalSource, 800, 400, 96, 96, "image/png")),
            new ImageSizeCalculator(new PublisherOptions
            {
                ImageMaxWidthPoints = 450,
                AllowImageUpscale = false,
            }));

        try
        {
            var result = await service.PublishAsync(
                new PublishRequest(markdownPath), CancellationToken.None);

            Assert.True(result.IsSuccess);
            var step = Assert.IsType<InsertImageStep>(
                Assert.Single(Assert.IsType<CompiledDocument>(target.Document).Steps));
            Assert.Equal("Remote alt", step.Image.AltText);
            Assert.Equal(finalSource.Uri, Assert.IsType<RemoteImageSource>(step.Image.Source).Uri);
            Assert.Equal(450, step.Image.Size?.WidthPoints);
            Assert.Equal(225, step.Image.Size?.HeightPoints);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static byte[] CreatePng(int width, int height)
    {
        byte[] bytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0, 0, 0, 13, (byte)'I', (byte)'H', (byte)'D', (byte)'R',
            (byte)(width >> 24), (byte)(width >> 16), (byte)(width >> 8), (byte)width,
            (byte)(height >> 24), (byte)(height >> 16), (byte)(height >> 8), (byte)height,
            8, 2, 0, 0, 0, 0, 0, 0, 0,
        ];
        return bytes;
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"vmf-image-pipeline-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private sealed class RecordingPublisher : IGoogleDocsPublisher
    {
        internal CompiledDocument? Document { get; private set; }

        public Task<PublishedDocument> PublishAsync(
            CompiledDocument document,
            CancellationToken cancellationToken)
        {
            Document = document;
            return Task.FromResult(new PublishedDocument(
                "document-id",
                "https://docs.google.com/document/d/document-id/edit"));
        }
    }

    private sealed class PassthroughResolver : IImageSourceResolver
    {
        public Task<ImageSource> ResolveAsync(
            ImageSource source,
            string markdownFilePath,
            CancellationToken cancellationToken) => Task.FromResult(source);
    }

    private sealed class FixedMetadataReader(ImageMetadata metadata) : IImageMetadataReader
    {
        public Task<ImageMetadata> ReadAsync(
            ImageSource source,
            CancellationToken cancellationToken) => Task.FromResult(metadata);
    }

    private sealed class RejectingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => throw new InvalidOperationException(
                "Local image test must not use HTTP.");
    }
}
