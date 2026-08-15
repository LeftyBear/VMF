using System.Net;
using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class PhysicalUpdateExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_NoChange_DoesNotCallMapperOrClientButValidatesPlan()
    {
        var executor = new PhysicalUpdateExecutor(new RecordingMapper(), new RecordingClient());
        var result = await executor.ExecuteAsync(Plan(withOperations: false), CancellationToken.None);

        Assert.Equal(PhysicalUpdateExecutionStatus.NoChange, result.Status);
        Assert.Equal(0, result.AttemptCount);
    }

    [Fact]
    public async Task ExecuteAsync_OperationPlan_MapsOnceAndSubmitsTrace()
    {
        var mapper = new RecordingMapper();
        var client = new RecordingClient();
        var executor = new PhysicalUpdateExecutor(mapper, client);

        var result = await executor.ExecuteAsync(Plan(), CancellationToken.None);

        Assert.Equal(PhysicalUpdateExecutionStatus.Applied, result.Status);
        Assert.Equal(1, mapper.CallCount);
        Assert.Equal(1, client.CallCount);
        Assert.Single(result.RequestTraces);
    }

    [Fact]
    public async Task ExecuteAsync_RetryReusesSameBatchInstance()
    {
        var mapper = new RecordingMapper();
        var client = new RecordingClient
        {
            Failures =
            [
                Error(HttpStatusCode.TooManyRequests, RequestDeliveryState.NotSent),
            ],
        };
        var delay = new RecordingDelay();
        var executor = new PhysicalUpdateExecutor(
            mapper,
            client,
            new PhysicalUpdateRetryPolicy(3, TimeSpan.FromMilliseconds(10), TimeSpan.FromSeconds(1), 2, false),
            delay);

        var result = await executor.ExecuteAsync(Plan(), CancellationToken.None);

        Assert.Equal(PhysicalUpdateExecutionStatus.Applied, result.Status);
        Assert.Equal(2, client.CallCount);
        Assert.Same(client.Batches[0], client.Batches[1]);
        Assert.Single(delay.Delays);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotRetryRevisionConflictOrRejected()
    {
        var conflict = await ExecuteFailure(Error(
            HttpStatusCode.BadRequest,
            RequestDeliveryState.NotSent,
            "FAILED_PRECONDITION"));
        var rejected = await ExecuteFailure(Error(HttpStatusCode.Forbidden, RequestDeliveryState.NotSent));

        Assert.Equal(PhysicalUpdateExecutionStatus.RevisionConflict, conflict.Status);
        Assert.Equal(1, conflict.AttemptCount);
        Assert.Equal(PhysicalUpdateExecutionStatus.Rejected, rejected.Status);
        Assert.Equal(1, rejected.AttemptCount);
    }

    [Theory]
    [InlineData(HttpStatusCode.TooManyRequests, PhysicalUpdateExecutionStatus.Applied, 2)]
    [InlineData(HttpStatusCode.ServiceUnavailable, PhysicalUpdateExecutionStatus.Applied, 2)]
    [InlineData(HttpStatusCode.BadGateway, PhysicalUpdateExecutionStatus.Rejected, 1)]
    public async Task ExecuteAsync_RetriesOnlyNotSent429And503(
        HttpStatusCode statusCode,
        PhysicalUpdateExecutionStatus expected,
        int attempts)
    {
        var result = await ExecuteFailure(Error(statusCode, RequestDeliveryState.NotSent));

        Assert.Equal(expected, result.Status);
        Assert.Equal(attempts, result.AttemptCount);
    }

    [Fact]
    public async Task ExecuteAsync_SentTimeoutIsIndeterminate()
    {
        var result = await ExecuteFailure(Error(HttpStatusCode.GatewayTimeout, RequestDeliveryState.Sent));

        Assert.Equal(PhysicalUpdateExecutionStatus.IndeterminateFailure, result.Status);
        Assert.Equal(1, result.AttemptCount);
    }

    [Theory]
    [InlineData(RequestDeliveryState.NotSent, PhysicalUpdateExecutionStatus.TransientFailure, 1)]
    [InlineData(RequestDeliveryState.Sent, PhysicalUpdateExecutionStatus.IndeterminateFailure, 1)]
    [InlineData(RequestDeliveryState.Unknown, PhysicalUpdateExecutionStatus.IndeterminateFailure, 1)]
    public async Task ExecuteAsync_CarriesDeliveryStateWithoutChangingClassification(
        RequestDeliveryState deliveryState,
        PhysicalUpdateExecutionStatus expectedStatus,
        int expectedAttempts)
    {
        var result = await ExecuteFailure(Error(HttpStatusCode.TooManyRequests, deliveryState), expectedAttempts);

        Assert.Equal(expectedStatus, result.Status);
        Assert.Equal(expectedAttempts, result.AttemptCount);
        Assert.Equal(deliveryState, result.DeliveryState);
    }

    [Fact]
    public async Task ExecuteAsync_RetryLimitRetryAfterAndBackoffCapAreApplied()
    {
        var client = new RecordingClient
        {
            Failures =
            [
                Error(HttpStatusCode.TooManyRequests, RequestDeliveryState.NotSent, retryAfter: TimeSpan.FromSeconds(9)),
                Error(HttpStatusCode.ServiceUnavailable, RequestDeliveryState.NotSent),
                Error(HttpStatusCode.TooManyRequests, RequestDeliveryState.NotSent),
            ],
        };
        var delay = new RecordingDelay();
        var executor = new PhysicalUpdateExecutor(
            new RecordingMapper(),
            client,
            new PhysicalUpdateRetryPolicy(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), 4, false),
            delay);

        var result = await executor.ExecuteAsync(Plan(), CancellationToken.None);

        Assert.Equal(PhysicalUpdateExecutionStatus.TransientFailure, result.Status);
        Assert.Equal(3, result.AttemptCount);
        Assert.Equal([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5)], delay.Delays);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationLocationsAreClassified()
    {
        using var before = new CancellationTokenSource();
        before.Cancel();
        var beforeResult = await new PhysicalUpdateExecutor(new RecordingMapper(), new RecordingClient())
            .ExecuteAsync(Plan(), before.Token);
        var duringSend = await ExecuteFailure(new OperationCanceledException("send canceled"));
        var delay = new CancelingDelay();
        var waitClient = new RecordingClient
        {
            Failures = [Error(HttpStatusCode.TooManyRequests, RequestDeliveryState.NotSent)],
        };
        var waitResult = await new PhysicalUpdateExecutor(
            new RecordingMapper(),
            waitClient,
            new PhysicalUpdateRetryPolicy(2, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(1), 1, false),
            delay).ExecuteAsync(Plan(), CancellationToken.None);

        Assert.Equal("CANCELED_BEFORE_SEND", beforeResult.DiagnosticCode);
        Assert.Equal(PhysicalUpdateExecutionStatus.IndeterminateFailure, duringSend.Status);
        Assert.Equal("CANCELED_DURING_RETRY_DELAY", waitResult.DiagnosticCode);
    }

    [Fact]
    public async Task ExecuteAsync_LoggerFailureDoesNotChangeResult()
    {
        var executor = new PhysicalUpdateExecutor(
            new RecordingMapper(),
            new RecordingClient(),
            resultLogger: _ => throw new InvalidOperationException("logger failed"));

        var result = await executor.ExecuteAsync(Plan(), CancellationToken.None);

        Assert.Equal(PhysicalUpdateExecutionStatus.Applied, result.Status);
    }

    private static async Task<PhysicalUpdateExecutionResult> ExecuteFailure(Exception exception, int maxAttempts = 2)
    {
        var client = new RecordingClient { Failures = [exception] };
        var executor = new PhysicalUpdateExecutor(
            new RecordingMapper(),
            client,
            new PhysicalUpdateRetryPolicy(maxAttempts, TimeSpan.Zero, TimeSpan.Zero, 1, false),
            new RecordingDelay());
        return await executor.ExecuteAsync(Plan(), CancellationToken.None);
    }

    private static GoogleDocsBatchUpdateException Error(
        HttpStatusCode statusCode,
        RequestDeliveryState deliveryState,
        string? reason = null,
        TimeSpan? retryAfter = null) => new(
            statusCode,
            reason ?? $"HTTP_{(int)statusCode}",
            retryAfter,
            deliveryState,
            "injected failure");

    private static PhysicalUpdatePlan Plan(bool withOperations = true) => new(
        Identity(),
        new DocumentRevision("required-revision", 1),
        new DocumentTextRange(10, 20),
        new DiffPlan(
            Fingerprint("old"),
            Fingerprint("new"),
            isFingerprintMatch: false,
            withOperations
                ?
                [
                    new DiffOperation(
                        DiffOperationKind.Update,
                        0,
                        0,
                        Block("a", "old"),
                        Block("a", "new"),
                        BlockMatchKind.ExplicitId),
                ]
                :
                [
                    new DiffOperation(
                        DiffOperationKind.NoChange,
                        0,
                        0,
                        Block("a", "old"),
                        Block("a", "old"),
                        BlockMatchKind.ExplicitId),
                ]),
        withOperations
            ?
            [
                new PhysicalUpdateOperation(
                    0,
                    PhysicalOperationKind.DeleteRange,
                    PhysicalOperationReason.Update,
                    0,
                    0,
                    new DocumentTextRange(10, 20),
                    Block("a", "old"),
                    null),
            ]
            : []);

    private static PhysicalUpdateRequestBatch Batch() => new(
        "google-document",
        "required-revision",
        [new { deleteContentRange = new { range = new { startIndex = 10, endIndex = 20 } } }],
        1,
        [new PhysicalUpdateRequestTrace(0, 0, PhysicalOperationReason.Update, Block("a", "old"), "deleteContentRange")]);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static BlockIdentity Block(string id, string hash) =>
        new(id, null, "ch-v1:sha256:" + Hash(hash));

    private static PublishFingerprint Fingerprint(string value) =>
        new("v1:sha256:" + Hash(value));

    private static string Hash(string value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private sealed class RecordingMapper : IPhysicalUpdateRequestMapper
    {
        public int CallCount { get; private set; }

        public PhysicalUpdateRequestBatch Map(PhysicalUpdatePlan plan)
        {
            CallCount++;
            return Batch();
        }
    }

    private sealed class RecordingClient : IGoogleDocsBatchUpdateClient
    {
        public List<Exception> Failures { get; init; } = [];

        public int CallCount { get; private set; }

        public List<PhysicalUpdateRequestBatch> Batches { get; } = [];

        public Task<GoogleDocsBatchUpdateResponse> ExecuteAsync(
            PhysicalUpdateRequestBatch batch,
            CancellationToken cancellationToken)
        {
            CallCount++;
            Batches.Add(batch);
            if (Failures.Count >= CallCount)
            {
                throw Failures[CallCount - 1];
            }

            return Task.FromResult(new GoogleDocsBatchUpdateResponse("applied-revision"));
        }
    }

    private sealed class RecordingDelay : IAsyncDelay
    {
        public List<TimeSpan> Delays { get; } = [];

        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            Delays.Add(delay);
            return Task.CompletedTask;
        }
    }

    private sealed class CancelingDelay : IAsyncDelay
    {
        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) =>
            throw new OperationCanceledException("wait canceled");
    }
}
