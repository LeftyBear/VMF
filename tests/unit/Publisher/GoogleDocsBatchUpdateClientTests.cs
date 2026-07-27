using System.Net;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Application;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsBatchUpdateClientTests
{
    [Fact]
    public async Task ExecuteAsync_SendsRequiredRevisionWriteControl()
    {
        var handler = new SequenceHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "{\"writeControl\":{\"requiredRevisionId\":\"applied-revision\"}}",
                Encoding.UTF8,
                "application/json"),
        });
        using var httpClient = new HttpClient(handler);
        var client = new GoogleDocsBatchUpdateClient(new StubCredentialProvider(), httpClient);

        var response = await client.ExecuteAsync(Batch(), CancellationToken.None);

        Assert.Equal("applied-revision", response.RevisionId);
        using var document = JsonDocument.Parse(Assert.Single(handler.Bodies));
        Assert.Equal(
            "required-revision",
            document.RootElement.GetProperty("writeControl")
                .GetProperty("requiredRevisionId").GetString());
    }

    [Fact]
    public async Task ExecuteAsync_NormalizesGoogleErrorReasonAndRetryAfter()
    {
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent(
                "{\"error\":{\"errors\":[{\"reason\":\"rateLimitExceeded\"}]}}",
                Encoding.UTF8,
                "application/json"),
        };
        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(
            TimeSpan.FromSeconds(7));
        var handler = new SequenceHandler(response);
        using var httpClient = new HttpClient(handler);
        var client = new GoogleDocsBatchUpdateClient(new StubCredentialProvider(), httpClient);

        var exception = await Assert.ThrowsAsync<GoogleDocsBatchUpdateException>(
            () => client.ExecuteAsync(Batch(), CancellationToken.None));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.HttpStatusCode);
        Assert.Equal("rateLimitExceeded", exception.GoogleErrorReason);
        Assert.Equal(TimeSpan.FromSeconds(7), exception.RetryAfter);
        Assert.Equal(RequestDeliveryState.Sent, exception.DeliveryState);
    }

    private static PhysicalUpdateRequestBatch Batch() => new(
        "google-document",
        "required-revision",
        [new { deleteContentRange = new { range = new { startIndex = 10, endIndex = 20 } } }],
        1,
        [new PhysicalUpdateRequestTrace(0, 0, Vmf.Publisher.Domain.PhysicalOperationReason.Delete,
            new Vmf.Publisher.Domain.BlockIdentity("a", null, "ch-v1:sha256:" + new string('a', 64)),
            "deleteContentRange")]);

    private sealed class StubCredentialProvider : IGoogleCredentialProvider
    {
        public Task<GoogleCredential> GetCredentialAsync(CancellationToken cancellationToken) =>
            Task.FromResult(new GoogleCredential("not-a-secret", DateTimeOffset.UtcNow.AddHours(1)));
    }

    private sealed class SequenceHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> responses;

        internal SequenceHandler(params HttpResponseMessage[] responses)
        {
            this.responses = new Queue<HttpResponseMessage>(responses);
        }

        internal List<string> Bodies { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Bodies.Add(request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken));
            return responses.Dequeue();
        }
    }
}
