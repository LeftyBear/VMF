using System.Net;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class ImageSourceResolverTests
{
    [Fact]
    public async Task ResolveAsync_NormalizesRelativeLocalPathAgainstMarkdownFile()
    {
        var directory = CreateTemporaryDirectory();
        var imagePath = Path.Combine(directory, "image.png");
        await File.WriteAllBytesAsync(imagePath, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);

        try
        {
            var resolved = await new ImageSourceResolver().ResolveAsync(
                new LocalImageSource("image.png"),
                Path.Combine(directory, "document.md"),
                CancellationToken.None);

            Assert.Equal(Path.GetFullPath(imagePath), Assert.IsType<LocalImageSource>(resolved).Path);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Theory]
    [InlineData("missing.png", PublishErrorCodes.ImageFileNotFound)]
    [InlineData("image.bmp", PublishErrorCodes.ImageFormatNotSupported)]
    public async Task ResolveAsync_InvalidLocalImage_ReportsStableError(string fileName, string code)
    {
        var directory = CreateTemporaryDirectory();
        if (fileName.EndsWith(".bmp", StringComparison.Ordinal))
        {
            await File.WriteAllBytesAsync(Path.Combine(directory, fileName), [0x42, 0x4D]);
        }

        try
        {
            var exception = await Assert.ThrowsAsync<PublishPipelineException>(() =>
                new ImageSourceResolver().ResolveAsync(
                    new LocalImageSource(fileName),
                    Path.Combine(directory, "document.md"),
                    CancellationToken.None));

            Assert.Equal(code, exception.Code);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task ResolveAsync_ExtensionAndSignatureMismatch_IsRejected()
    {
        var directory = CreateTemporaryDirectory();
        await File.WriteAllBytesAsync(Path.Combine(directory, "image.png"), "GIF89a"u8.ToArray());

        try
        {
            var exception = await Assert.ThrowsAsync<PublishPipelineException>(() =>
                new ImageSourceResolver().ResolveAsync(
                    new LocalImageSource("image.png"),
                    Path.Combine(directory, "document.md"),
                    CancellationToken.None));

            Assert.Equal(PublishErrorCodes.ImageFormatNotSupported, exception.Code);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Theory]
    [InlineData("ftp://example.com/image.png", PublishErrorCodes.ImageRemoteUriInvalid)]
    [InlineData("https://user:password@example.com/image.png", PublishErrorCodes.ImageRemoteUriInvalid)]
    [InlineData("http://localhost/image.png", PublishErrorCodes.ImageRemoteHostNotAllowed)]
    [InlineData("http://127.0.0.1/image.png", PublishErrorCodes.ImageRemoteHostNotAllowed)]
    [InlineData("http://10.0.0.1/image.png", PublishErrorCodes.ImageRemoteHostNotAllowed)]
    [InlineData("http://192.168.1.1/image.png", PublishErrorCodes.ImageRemoteHostNotAllowed)]
    [InlineData("http://[::1]/image.png", PublishErrorCodes.ImageRemoteHostNotAllowed)]
    public async Task ResolveAsync_UnsafeRemoteUri_IsRejected(string value, string code)
    {
        var exception = await Assert.ThrowsAsync<PublishPipelineException>(() =>
            new ImageSourceResolver(new FixedHostResolver(IPAddress.Parse("93.184.216.34")))
                .ResolveAsync(
                    new RemoteImageSource(new Uri(value)),
                    "document.md",
                    CancellationToken.None));

        Assert.Equal(code, exception.Code);
    }

    [Fact]
    public async Task ResolveAsync_DnsPrivateAddress_IsRejected()
    {
        var resolver = new ImageSourceResolver(new FixedHostResolver(IPAddress.Parse("172.16.1.2")));

        var exception = await Assert.ThrowsAsync<PublishPipelineException>(() => resolver.ResolveAsync(
            new RemoteImageSource(new Uri("https://example.com/image.png")),
            "document.md",
            CancellationToken.None));

        Assert.Equal(PublishErrorCodes.ImageRemoteHostNotAllowed, exception.Code);
    }

    [Fact]
    public async Task ResolveAsync_PublicDnsAddress_IsAllowed()
    {
        var resolver = new ImageSourceResolver(new FixedHostResolver(IPAddress.Parse("93.184.216.34")));

        var resolved = await resolver.ResolveAsync(
            new RemoteImageSource(new Uri("https://example.com/image.png")),
            "document.md",
            CancellationToken.None);

        Assert.Equal(
            new Uri("https://example.com/image.png"),
            Assert.IsType<RemoteImageSource>(resolved).Uri);
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), $"vmf-image-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private sealed class FixedHostResolver(params IPAddress[] addresses) : IHostAddressResolver
    {
        public Task<IPAddress[]> GetHostAddressesAsync(
            string host,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(addresses);
        }
    }
}
