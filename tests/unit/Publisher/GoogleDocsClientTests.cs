using System.Net;
using System.Text;
using Vmf.Publisher.Domain;
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
            "\"startIndex\":2,\"endIndex\":5," +
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

    [Fact]
    public async Task InsertInlineImageAsync_MapsSizeLocationAndStartAlignment()
    {
        var handler = new SequenceHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}"),
        });
        using var httpClient = new HttpClient(handler);

        await CreateClient(httpClient).InsertInlineImageAsync(
            "document-id",
            new Uri("https://example.com/image.png"),
            new ImageSize(450, 225),
            12,
            CancellationToken.None);

        var body = Assert.Single(handler.Bodies);
        Assert.Contains("insertInlineImage", body, StringComparison.Ordinal);
        Assert.Contains("https://example.com/image.png", body, StringComparison.Ordinal);
        Assert.Contains("\"magnitude\":450", body, StringComparison.Ordinal);
        Assert.Contains("\"magnitude\":225", body, StringComparison.Ordinal);
        Assert.Contains("\"index\":12", body, StringComparison.Ordinal);
        Assert.Contains("\"alignment\":\"START\"", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetDocumentAsync_ReadsInlineObjectParagraphIndexesAndActualSize()
    {
        const string json =
            "{\"body\":{\"content\":[{\"startIndex\":8,\"endIndex\":10," +
            "\"paragraph\":{\"elements\":[{\"startIndex\":8,\"endIndex\":9," +
            "\"inlineObjectElement\":{\"inlineObjectId\":\"object-id\"}}]}}]}," +
            "\"inlineObjects\":{\"object-id\":{\"inlineObjectProperties\":{" +
            "\"embeddedObject\":{\"title\":\"title\",\"description\":\"description\"," +
            "\"size\":{\"width\":{\"magnitude\":450,\"unit\":\"PT\"}," +
            "\"height\":{\"magnitude\":225,\"unit\":\"PT\"}}}}}}}";
        var handler = new SequenceHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        });
        using var httpClient = new HttpClient(handler);

        var snapshot = await CreateClient(httpClient)
            .GetDocumentAsync("document-id", CancellationToken.None);

        var image = Assert.Single(snapshot.Images);
        Assert.Equal(8, image.ElementStartIndex);
        Assert.Equal(9, image.ElementEndIndex);
        Assert.Equal(10, image.ParagraphEndIndex);
        Assert.Equal("object-id", image.InlineObjectId);
        Assert.Equal(450, image.ActualSize?.WidthPoints);
        Assert.Equal(225, image.ActualSize?.HeightPoints);
        Assert.Equal("title", image.Title);
        Assert.Equal("description", image.Description);
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

        internal List<string> Bodies { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            Bodies.Add(request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken));
            return responses.Dequeue();
        }
    }
}
