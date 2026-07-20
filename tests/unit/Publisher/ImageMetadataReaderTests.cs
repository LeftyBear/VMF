using System.Net;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class ImageMetadataReaderTests
{
    [Fact]
    public async Task ReadAsync_PngReadsPixelsDpiAndMimeType()
    {
        var bytes = CreatePng(1200, 600, 3780, 3780);
        var path = await WriteTemporaryImageAsync(".png", bytes);
        try
        {
            var metadata = await CreateReader().ReadAsync(
                new LocalImageSource(path), CancellationToken.None);

            Assert.Equal(1200, metadata.PixelWidth);
            Assert.Equal(600, metadata.PixelHeight);
            Assert.Equal(96.012, metadata.HorizontalDpi, 3);
            Assert.Equal(96.012, metadata.VerticalDpi, 3);
            Assert.Equal("image/png", metadata.MimeType);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ReadAsync_GifWithoutDpi_Uses96Dpi()
    {
        byte[] bytes = [.. "GIF89a"u8.ToArray(), 0x20, 0x03, 0x58, 0x02];
        var path = await WriteTemporaryImageAsync(".gif", bytes);
        try
        {
            var metadata = await CreateReader().ReadAsync(
                new LocalImageSource(path), CancellationToken.None);

            Assert.Equal(800, metadata.PixelWidth);
            Assert.Equal(600, metadata.PixelHeight);
            Assert.Equal(96, metadata.HorizontalDpi);
            Assert.Equal("image/gif", metadata.MimeType);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ReadAsync_JpegReadsJfifDpiAndDimensions()
    {
        byte[] bytes =
        [
            0xFF, 0xD8,
            0xFF, 0xE0, 0x00, 0x10,
            (byte)'J', (byte)'F', (byte)'I', (byte)'F', 0,
            1, 1, 1, 0, 72, 0, 72, 0, 0,
            0xFF, 0xC0, 0x00, 0x0B,
            8, 0x02, 0x58, 0x03, 0x20, 1, 1, 0x11, 0,
            0xFF, 0xD9,
        ];
        var path = await WriteTemporaryImageAsync(".jpg", bytes);
        try
        {
            var metadata = await CreateReader().ReadAsync(
                new LocalImageSource(path), CancellationToken.None);

            Assert.Equal(800, metadata.PixelWidth);
            Assert.Equal(600, metadata.PixelHeight);
            Assert.Equal(72, metadata.HorizontalDpi);
            Assert.Equal(72, metadata.VerticalDpi);
            Assert.Equal("image/jpeg", metadata.MimeType);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task ReadAsync_RemoteRedirect_RevalidatesTargetAndPreservesFinalUri()
    {
        var handler = new QueueHandler(
            new HttpResponseMessage(HttpStatusCode.Redirect)
            {
                Headers = { Location = new Uri("https://cdn.example.com/final.png") },
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(CreatePng(400, 200, null, null)),
            });
        var resolver = new RecordingResolver();
        var reader = new ImageMetadataReader(new HttpClient(handler), resolver);

        var metadata = await reader.ReadAsync(
            new RemoteImageSource(new Uri("https://example.com/image.png")),
            CancellationToken.None);

        Assert.Equal(new Uri("https://cdn.example.com/final.png"), resolver.LastUri);
        Assert.Equal(
            resolver.LastUri,
            Assert.IsType<RemoteImageSource>(metadata.Source).Uri);
    }

    [Fact]
    public void Calculate_CapsWidthWithoutUpscalingAndPreservesAspectRatio()
    {
        var calculator = new ImageSizeCalculator(new PublisherOptions
        {
            ImageMaxWidthPoints = 450,
            AllowImageUpscale = false,
        });
        var large = calculator.Calculate(new ImageMetadata(
            new LocalImageSource("large.png"), 1200, 600, 96, 96, "image/png"));
        var small = calculator.Calculate(new ImageMetadata(
            new LocalImageSource("small.png"), 400, 200, 96, 96, "image/png"));

        Assert.Equal(450, large.WidthPoints);
        Assert.Equal(225, large.HeightPoints);
        Assert.Equal(300, small.WidthPoints);
        Assert.Equal(150, small.HeightPoints);
    }

    private static ImageMetadataReader CreateReader() => new(
        new HttpClient(new QueueHandler()),
        new RecordingResolver());

    private static byte[] CreatePng(int width, int height, int? xPpm, int? yPpm)
    {
        var bytes = new List<byte>(
        [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
         0x00, 0x00, 0x00, 0x0D, .. "IHDR"u8.ToArray()]);
        AddBigEndian(bytes, width);
        AddBigEndian(bytes, height);
        bytes.AddRange([8, 2, 0, 0, 0, 0, 0, 0, 0]);
        if (xPpm is not null && yPpm is not null)
        {
            bytes.AddRange([0, 0, 0, 9, .. "pHYs"u8.ToArray()]);
            AddBigEndian(bytes, xPpm.Value);
            AddBigEndian(bytes, yPpm.Value);
            bytes.AddRange([1, 0, 0, 0, 0]);
        }

        return bytes.ToArray();
    }

    private static void AddBigEndian(List<byte> bytes, int value) => bytes.AddRange(
        [(byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value]);

    private static async Task<string> WriteTemporaryImageAsync(string extension, byte[] bytes)
    {
        var path = Path.Combine(Path.GetTempPath(), $"vmf-metadata-{Guid.NewGuid():N}{extension}");
        await File.WriteAllBytesAsync(path, bytes);
        return path;
    }

    private sealed class QueueHandler(params HttpResponseMessage[] responses) : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> responses = new(responses);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(responses.Dequeue());
        }
    }

    private sealed class RecordingResolver : IImageSourceResolver
    {
        internal Uri? LastUri { get; private set; }

        public Task<ImageSource> ResolveAsync(
            ImageSource source,
            string markdownFilePath,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastUri = Assert.IsType<RemoteImageSource>(source).Uri;
            return Task.FromResult(source);
        }
    }
}
