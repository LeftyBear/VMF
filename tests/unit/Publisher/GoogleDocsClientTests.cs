using System.Net;
using System.Text;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsClientTests
{
    [Fact]
    public async Task ApplyOperationsAsync_RetriesOnlyTransientStatusCodes()
    {
        var handler = new SequenceHandler(
            Response(HttpStatusCode.TooManyRequests),
            Response(HttpStatusCode.BadGateway),
            Response(HttpStatusCode.OK));
        using var httpClient = new HttpClient(handler);
        var client = CreateClient(httpClient);

        await client.InsertTableAsync("document-id", 2, 3, 1, CancellationToken.None);

        Assert.Equal(3, handler.CallCount);
    }

    [Fact]
    public async Task ApplyOperationsAsync_DoesNotRetryAuthenticationOrPermissionErrors()
    {
        var handler = new SequenceHandler(Response(HttpStatusCode.Forbidden));
        using var httpClient = new HttpClient(handler);
        var client = CreateClient(httpClient);

        await Assert.ThrowsAnyAsync<HttpRequestException>(() =>
            client.InsertTableAsync("document-id", 2, 3, 1, CancellationToken.None));

        Assert.Equal(1, handler.CallCount);
    }

    [Fact]
    public async Task GetDocumentAsync_ReadsTableAndCellIndexes()
    {
        const string json =
            "{\"body\":{\"content\":[{\"startIndex\":1,\"endIndex\":8," +
            "\"table\":{\"tableRows\":[{\"tableCells\":[{" +
            "\"content\":[{\"startIndex\":3,\"endIndex\":4,\"paragraph\":{}}]}]}]}}]}}";
        var handler = new SequenceHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        });
        using var httpClient = new HttpClient(handler);

        var snapshot = await CreateClient(httpClient)
            .GetDocumentAsync("document-id", CancellationToken.None);

        var table = Assert.Single(snapshot.Tables);
        Assert.Equal(1, table.StartIndex);
        Assert.Equal(8, table.EndIndex);
        var cell = Assert.Single(Assert.Single(table.Rows).Cells);
        Assert.Equal(3, cell.StartIndex);
        Assert.Equal(4, cell.EndIndex);
    }

    private static GoogleDocsClient CreateClient(HttpClient httpClient) => new(
        new StubCredentialProvider(),
        new GoogleDocsRequestMapper(),
        httpClient);

    private static HttpResponseMessage Response(HttpStatusCode statusCode) => new(statusCode)
    {
        Content = new StringContent(
            $"{{\"error\":{{\"status\":\"HTTP_{(int)statusCode}\"}}}}",
            Encoding.UTF8,
            "application/json"),
    };

    private sealed class StubCredentialProvider : IGoogleCredentialProvider
    {
        public Task<GoogleCredential> GetCredentialAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new GoogleCredential(
                "not-a-secret",
                DateTimeOffset.UtcNow.AddHours(1)));
        }
    }

    private sealed class SequenceHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> responses;

        internal SequenceHandler(params HttpResponseMessage[] responses)
        {
            this.responses = new Queue<HttpResponseMessage>(responses);
        }

        internal int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return Task.FromResult(responses.Dequeue());
        }
    }
}
