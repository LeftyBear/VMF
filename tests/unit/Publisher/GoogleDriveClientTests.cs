using System.Net;
using System.Text;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDriveClientTests
{
    [Fact]
    public async Task CreateDocumentAsync_EnablesSharedDriveSupport()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "{\"id\":\"document-id\",\"webViewLink\":\"https://docs.google.com/document/d/document-id/edit\"}",
                Encoding.UTF8,
                "application/json"),
        });
        using var httpClient = new HttpClient(handler);
        var client = new GoogleDriveClient(new StubCredentialProvider(), httpClient);

        var file = await client.CreateDocumentAsync("Sample", "shared-drive-folder", CancellationToken.None);

        Assert.Equal("document-id", file.Id);
        Assert.Contains("supportsAllDrives=true", handler.RequestUri?.Query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateDocumentAsync_OnFailure_ReportsOnlySafeApiMetadata()
    {
        const string privateServerMessage = "private folder information must not be logged";
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent(
                $"{{\"error\":{{\"status\":\"PERMISSION_DENIED\",\"message\":\"{privateServerMessage}\"}}}}",
                Encoding.UTF8,
                "application/json"),
        });
        using var httpClient = new HttpClient(handler);
        var client = new GoogleDriveClient(new StubCredentialProvider(), httpClient);

        var exception = await Assert.ThrowsAnyAsync<HttpRequestException>(() =>
            client.CreateDocumentAsync("Sample", "shared-drive-folder", CancellationToken.None));

        Assert.Equal("Google Drive API failed: HTTP 403 (PERMISSION_DENIED).", exception.Message);
        Assert.DoesNotContain(privateServerMessage, exception.Message, StringComparison.Ordinal);
    }

    private sealed class StubCredentialProvider : IGoogleCredentialProvider
    {
        public Task<GoogleCredential> GetCredentialAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new GoogleCredential("not-a-secret", DateTimeOffset.UtcNow.AddHours(1)));
        }
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> responseFactory;

        internal StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            this.responseFactory = responseFactory;
        }

        internal Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RequestUri = request.RequestUri;
            return Task.FromResult(responseFactory(request));
        }
    }
}
