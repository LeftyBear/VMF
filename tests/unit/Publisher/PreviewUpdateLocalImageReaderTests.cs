using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class PreviewUpdateLocalImageReaderTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "vmf-b2-" + Guid.NewGuid());
    public PreviewUpdateLocalImageReaderTests() => Directory.CreateDirectory(directory);
    public void Dispose() => Directory.Delete(directory, true);
    private string Markdown => Path.Combine(directory, "input.md");
    private string Create(string extension)
    {
        var path = Path.Combine(directory, "image" + extension);
        File.WriteAllBytes(path, [0]);
        return path;
    }

    [Theory]
    [MemberData(nameof(ImageMetadataReaderTests.ParserCases), MemberType = typeof(ImageMetadataReaderTests))]
    public async Task ReadsOnceAndNeverReopensPath(byte[] bytes, string mime, int width, int height, double x, double y)
    {
        var extension = mime == "image/png" ? ".png" : mime == "image/jpeg" ? ".jpg" : ".gif";
        var path = Create(extension);
        var opens = 0;
        var reader = new PreviewUpdateLocalImageReader(new(), p =>
        {
            Assert.Equal(path, p);
            opens++;
            File.Delete(p); // Any subsequent path read would fail.
            return new MemoryStream(bytes, writable: false);
        });
        var actual = await reader.ReadAsync(new("image" + extension), Markdown, default);
        var expected = ImageMetadataParser.Parse(bytes, new LocalImageSource(path));
        Assert.Equal(1, opens);
        Assert.IsType<LocalImageSource>(actual.Source);
        Assert.Equal(path, actual.Source.Value);
        Assert.Equal(width, actual.PixelWidth);
        Assert.Equal(height, actual.PixelHeight);
        Assert.Equal(x, actual.HorizontalDpi, 6);
        Assert.Equal(y, actual.VerticalDpi, 6);
        Assert.Equal(expected.ContentHash, actual.ContentHash);
    }

    [Theory]
    [MemberData(nameof(ImageMetadataReaderTests.InvalidParserCases), MemberType = typeof(ImageMetadataReaderTests))]
    public async Task InvalidBytesPreserveErrors(byte[] bytes, string code)
    {
        var mime = ImageSourceResolver.DetectMimeType(bytes);
        var extension = mime == "image/gif" ? ".gif" : mime == "image/jpeg" ? ".jpg" : ".png";
        Create(extension);
        var reader = new PreviewUpdateLocalImageReader(new(), _ => new MemoryStream(bytes));
        var error = await Assert.ThrowsAsync<PublishPipelineException>(() => reader.ReadAsync(new("image" + extension), Markdown, default));
        Assert.Equal(code, error.Code);
    }

    [Theory]
    [InlineData(0, PublishErrorCodes.ImagePathInvalid)]
    [InlineData(1, PublishErrorCodes.ImagePathInvalid)]
    [InlineData(2, PublishErrorCodes.ImageMetadataReadFailed)]
    public async Task AcquisitionErrorsAreConverted(int kind, string code)
    {
        Create(".png");
        Exception failure = kind == 0 ? new IOException("secret") : kind == 1 ? new UnauthorizedAccessException("secret") : new InvalidOperationException("secret");
        var reader = new PreviewUpdateLocalImageReader(new(), _ => throw failure);
        var error = await Assert.ThrowsAsync<PublishPipelineException>(() => reader.ReadAsync(new("image.png"), Markdown, default));
        Assert.Equal(code, error.Code);
        Assert.DoesNotContain("secret", error.Message);
        Assert.Same(failure, error.InnerException);
    }

    [Fact]
    public async Task RejectsMismatch()
    {
        Create(".png");
        var reader = new PreviewUpdateLocalImageReader(new(), _ => new MemoryStream("GIF89a"u8.ToArray()));
        var error = await Assert.ThrowsAsync<PublishPipelineException>(() => reader.ReadAsync(new("image.png"), Markdown, default));
        Assert.Equal(PublishErrorCodes.ImageFormatNotSupported, error.Code);
    }

    [Theory]
    [InlineData("", PublishErrorCodes.ImageSourceEmpty)]
    [InlineData("missing.png", PublishErrorCodes.ImageFileNotFound)]
    [InlineData("image.bin", PublishErrorCodes.ImageFormatNotSupported)]
    public async Task RejectsBeforeOpen(string path, string code)
    {
        Create(".bin");
        var reader = new PreviewUpdateLocalImageReader(new(), _ => throw new Exception("Must not open"));
        var error = await Assert.ThrowsAsync<PublishPipelineException>(() => reader.ReadAsync(new(path), Markdown, default));
        Assert.Equal(code, error.Code);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadFailureDisposesStream(bool cancel)
    {
        Create(".png");
        using var stream = new FailingStream(cancel);
        var reader = new PreviewUpdateLocalImageReader(new(), _ => stream);
        if (cancel)
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reader.ReadAsync(new("image.png"), Markdown, default));
        }
        else
        {
            var error = await Assert.ThrowsAsync<PublishPipelineException>(() => reader.ReadAsync(new("image.png"), Markdown, default));
            Assert.Equal(PublishErrorCodes.ImageMetadataReadFailed, error.Code);
            Assert.Equal("Image metadata could not be read.", error.Message);
            Assert.IsType<IOException>(error.InnerException);
            Assert.DoesNotContain("secret", error.Message);
        }
        Assert.True(stream.Disposed);
    }

    private sealed class FailingStream(bool cancel) : MemoryStream(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })
    {
        internal bool Disposed { get; private set; }
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            throw (cancel ? new OperationCanceledException() : new IOException("secret"));
        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SignatureFailurePreservesErrorAndDisposesStream(bool unauthorized)
    {
        Create(".png");
        Exception failure = unauthorized ? new UnauthorizedAccessException("secret") : new IOException("secret");
        using var stream = new SignatureStream(failure);
        var reader = new PreviewUpdateLocalImageReader(new(), _ => stream);
        var error = await Assert.ThrowsAsync<PublishPipelineException>(() =>
            reader.ReadAsync(new("image.png"), Markdown, default));
        Assert.Equal(PublishErrorCodes.ImagePathInvalid, error.Code);
        Assert.Equal("Local image file could not be inspected.", error.Message);
        Assert.Same(failure, error.InnerException);
        Assert.DoesNotContain("secret", error.Message);
        Assert.True(stream.Disposed);
    }

    [Fact]
    public async Task SignatureCancellationDisposesStream()
    {
        Create(".png");
        using var stream = new SignatureStream(new OperationCanceledException());
        var reader = new PreviewUpdateLocalImageReader(new(), _ => stream);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            reader.ReadAsync(new("image.png"), Markdown, default));
        Assert.True(stream.Disposed);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public async Task ShortSignatureReadsPreserveBytes(int chunkSize)
    {
        Create(".gif");
        var bytes = "GIF89a"u8.ToArray().Concat(new byte[] { 1, 0, 2, 0 }).ToArray();
        using var stream = new ShortReadStream(bytes, chunkSize);
        var opens = 0;
        var reader = new PreviewUpdateLocalImageReader(new(), _ => { opens++; return stream; });
        var actual = await reader.ReadAsync(new("image.gif"), Markdown, default);
        var expected = ImageMetadataParser.Parse(bytes, actual.Source);
        Assert.Equal(expected.ContentHash, actual.ContentHash);
        Assert.Equal(expected.PixelWidth, actual.PixelWidth);
        Assert.Equal(expected.PixelHeight, actual.PixelHeight);
        Assert.Equal(expected.MimeType, actual.MimeType);
        Assert.Equal(1, opens);
        Assert.False(stream.CanRead);
    }

    [Theory]
    [InlineData("GI", PublishErrorCodes.ImageFormatNotSupported)]
    [InlineData("GIF89a", PublishErrorCodes.ImageMetadataReadFailed)]
    public async Task EarlyEndOfStreamPreservesErrorAndDisposesStream(string content, string code)
    {
        Create(".gif");
        using var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(content));
        var reader = new PreviewUpdateLocalImageReader(new(), _ => stream);
        var error = await Assert.ThrowsAsync<PublishPipelineException>(() =>
            reader.ReadAsync(new("image.gif"), Markdown, default));
        Assert.Equal(code, error.Code);
        Assert.False(stream.CanRead);
    }

    private sealed class SignatureStream(Exception failure) : MemoryStream
    {
        internal bool Disposed { get; private set; }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            throw failure;
        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class ShortReadStream(byte[] bytes, int chunkSize) : MemoryStream(bytes)
    {
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            base.ReadAsync(buffer[..Math.Min(buffer.Length, chunkSize)], cancellationToken);
    }

    [Fact]
    public async Task CancellationDoesNotOpen()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var reader = new PreviewUpdateLocalImageReader(new(), _ => throw new Exception("Must not open"));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reader.ReadAsync(new("image.png"), Markdown, cancellation.Token));
    }

    [Fact]
    public async Task CancellationFromAcquisitionIsPreserved()
    {
        Create(".png");
        var reader = new PreviewUpdateLocalImageReader(new(), _ => throw new OperationCanceledException());
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reader.ReadAsync(new("image.png"), Markdown, default));
    }

    [Fact]
    public async Task RechecksImmediatelyBeforeOpen()
    {
        var path = Create(".png");
        var visits = 0;
        var guard = new PreviewUpdateImagePathGuard(p => p == path && ++visits == 2
            ? FileAttributes.ReparsePoint : File.GetAttributes(p));
        var reader = new PreviewUpdateLocalImageReader(guard, _ => throw new Exception("Must not open"));
        var error = await Assert.ThrowsAsync<PublishPipelineException>(() => reader.ReadAsync(new("image.png"), Markdown, default));
        Assert.Equal(PublishErrorCodes.ImagePathInvalid, error.Code);
        Assert.Equal(2, visits);
    }
}
