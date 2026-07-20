using System.Net;
using System.Security.Cryptography;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDriveTemporaryImageHostTests
{
    [Fact]
    public async Task HostAsync_UploadsPropertiesGrantsAccessAndDeleteRemovesFile()
    {
        byte[] bytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        var path = await WriteTemporaryImageAsync(bytes);
        var handler = new RecordingHandler(
            Response(HttpStatusCode.OK, "{\"id\":\"temporary-id\"}"),
            Response(HttpStatusCode.OK, "{}"),
            Response(HttpStatusCode.NoContent, ""));
        var host = CreateHost(handler, allow: true);

        try
        {
            var hosted = await host.HostAsync(new LocalImageSource(path), CancellationToken.None);
            await host.DeleteAsync(hosted, CancellationToken.None);

            Assert.Equal("temporary-id", hosted.ResourceId);
            Assert.Equal(3, handler.Requests.Count);
            var uploadBody = handler.Requests[0].Body;
            Assert.Contains("publisher-temp-", uploadBody, StringComparison.Ordinal);
            Assert.Contains("vmfPublisherTemporary", uploadBody, StringComparison.Ordinal);
            Assert.Contains(
                Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),
                uploadBody,
                StringComparison.Ordinal);
            Assert.Contains("folder-id", uploadBody, StringComparison.Ordinal);
            Assert.Contains("\"type\":\"anyone\"", handler.Requests[1].Body, StringComparison.Ordinal);
            Assert.Equal(HttpMethod.Delete, handler.Requests[2].Method);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task HostAsync_WhenPublicHostingDisabled_DoesNotCallDrive()
    {
        var handler = new RecordingHandler();
        var host = CreateHost(handler, allow: false);

        var exception = await Assert.ThrowsAsync<PublishPipelineException>(() => host.HostAsync(
            new LocalImageSource("image.png"), CancellationToken.None));

        Assert.Equal(PublishErrorCodes.ImagePublicAccessDenied, exception.Code);
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task HostAsync_RetriesOnlyTemporaryHttpFailures()
    {
        var path = await WriteTemporaryImageAsync([0xFF, 0xD8, 0xFF]);
        var handler = new RecordingHandler(
            Response(HttpStatusCode.ServiceUnavailable, "{}"),
            Response(HttpStatusCode.OK, "{\"id\":\"temporary-id\"}"),
            Response(HttpStatusCode.OK, "{}"));
        try
        {
            await CreateHost(handler, allow: true).HostAsync(
                new LocalImageSource(path), CancellationToken.None);

            Assert.Equal(3, handler.Requests.Count);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static GoogleDriveTemporaryImageHost CreateHost(
        HttpMessageHandler handler,
        bool allow) => new(
            new FixedCredentialProvider(),
            new HttpClient(handler),
            new GooglePublisherOptions { TemporaryImageFolderId = "folder-id" },
            new PublisherOptions { AllowTemporaryPublicImageHosting = allow });

    private static HttpResponseMessage Response(HttpStatusCode status, string body) => new(status)
    {
        Content = new StringContent(body),
    };

    private static async Task<string> WriteTemporaryImageAsync(byte[] bytes)
    {
        var extension = bytes[0] == 0xFF ? ".jpg" : ".png";
        var path = Path.Combine(Path.GetTempPath(), $"vmf-host-{Guid.NewGuid():N}{extension}");
        await File.WriteAllBytesAsync(path, bytes);
        return path;
    }

    private sealed class FixedCredentialProvider : IGoogleCredentialProvider
    {
        public Task<GoogleCredential> GetCredentialAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new GoogleCredential(
                "secret-token",
                DateTimeOffset.UtcNow.AddHours(1)));
    }

    private sealed class RecordingHandler(params HttpResponseMessage[] responses) : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> responses = new(responses);

        internal List<(HttpMethod Method, string Body)> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var body = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add((request.Method, body));
            return responses.Dequeue();
        }
    }
}
