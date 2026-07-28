using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsApplyEngineTests
{
    [Fact]
    public async Task ApplyAsync_EmptyPlanSucceeds()
    {
        var executor = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange);
        var result = await Engine(executors: [executor]).ApplyAsync(Plan(), dryRun: false, default);

        Assert.True(result.Success);
        Assert.False(result.DryRun);
        Assert.Equal(0, result.PlannedOperationCount);
        Assert.Equal(0, result.AppliedOperationCount);
        Assert.True(result.VerifiedStateCommitAllowed);
        Assert.Equal(0, executor.CallCount);
    }

    [Fact]
    public async Task ApplyAsync_DryRunDoesNotDispatchGatewayUpdate()
    {
        var executor = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange);
        var result = await Engine(executors: [executor]).ApplyAsync(Plan(Operation()), dryRun: true, default);

        Assert.True(result.Success);
        Assert.True(result.DryRun);
        Assert.Equal(1, result.PlannedOperationCount);
        Assert.Equal(0, result.AppliedOperationCount);
        Assert.False(result.VerifiedStateCommitAllowed);
        Assert.Equal(0, executor.CallCount);
    }

    [Fact]
    public async Task ApplyAsync_ExecutesOnlyWhenRevisionMatches()
    {
        var executor = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange);
        var matched = await Engine(executors: [executor]).ApplyAsync(Plan(Operation()), false, default);
        var conflict = await Engine(Revision(2), [executor]).ApplyAsync(Plan(Operation()), false, default);

        Assert.True(matched.Success);
        Assert.Equal(1, executor.CallCount);
        Assert.False(conflict.Success);
        Assert.True(conflict.Conflict);
        Assert.Equal(UpdateErrorCodes.RevisionConflict, conflict.ErrorCode);
        Assert.Equal(1, executor.CallCount);
    }

    [Fact]
    public async Task ApplyAsync_RevisionConflictRejectsAllOperations()
    {
        var first = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange);
        var second = new RecordingOperationExecutor(PhysicalOperationKind.InsertInlineText);

        var result = await Engine(Revision(2), [first, second]).ApplyAsync(
            Plan(Operation(), Operation(1, PhysicalOperationKind.InsertInlineText)),
            false,
            default);

        Assert.False(result.Success);
        Assert.True(result.Conflict);
        Assert.Equal(0, result.AppliedOperationCount);
        Assert.Equal(0, first.CallCount);
        Assert.Equal(0, second.CallCount);
        Assert.False(result.VerifiedStateCommitAllowed);
    }

    [Fact]
    public async Task ApplyAsync_ExecutesOperationsInPlanOrder()
    {
        var calls = new List<string>();
        var delete = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange, calls);
        var insert = new RecordingOperationExecutor(PhysicalOperationKind.InsertInlineText, calls);

        var result = await Engine(executors: [delete, insert]).ApplyAsync(
            Plan(Operation(), Operation(1, PhysicalOperationKind.InsertInlineText)),
            false,
            default);

        Assert.True(result.Success);
        Assert.Equal(["0:DeleteRange", "1:InsertInlineText"], calls);
    }

    [Fact]
    public async Task ApplyAsync_StopsAfterFirstFailedOperation()
    {
        var first = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange)
        {
            ResultCode = UpdateErrorCodes.ApplicationFailed,
        };
        var second = new RecordingOperationExecutor(PhysicalOperationKind.InsertInlineText);

        var result = await Engine(executors: [first, second]).ApplyAsync(
            Plan(Operation(), Operation(1, PhysicalOperationKind.InsertInlineText)),
            false,
            default);

        Assert.False(result.Success);
        Assert.Equal(0, result.AppliedOperationCount);
        Assert.Equal(0, result.FailedOperationIndex);
        Assert.Equal(Operation().OperationId, result.FailedOperationIdentity);
        Assert.Equal(1, first.CallCount);
        Assert.Equal(0, second.CallCount);
    }

    [Fact]
    public async Task ApplyAsync_UnregisteredOperationSafelyStops()
    {
        var result = await Engine().ApplyAsync(Plan(Operation()), false, default);

        Assert.False(result.Success);
        Assert.Equal(UpdateErrorCodes.OperationExecutorNotRegistered, result.ErrorCode);
        Assert.Equal(0, result.AppliedOperationCount);
        Assert.Equal(0, result.FailedOperationIndex);
        Assert.False(result.VerifiedStateCommitAllowed);
    }

    [Fact]
    public async Task ApplyAsync_ExecutorExceptionBecomesStructuredError()
    {
        var executor = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange)
        {
            Exception = new InvalidOperationException("injected failure"),
        };

        var result = await Engine(executors: [executor]).ApplyAsync(Plan(Operation()), false, default);

        Assert.False(result.Success);
        Assert.Equal(UpdateErrorCodes.OperationExecutorFailed, result.ErrorCode);
        Assert.Equal("injected failure", result.Message);
        Assert.Equal(0, result.FailedOperationIndex);
    }

    [Fact]
    public async Task ApplyAsync_FailureDisallowsVerifiedStateCommit()
    {
        var executor = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange)
        {
            ResultCode = UpdateErrorCodes.ApplicationFailed,
        };

        var result = await Engine(executors: [executor]).ApplyAsync(Plan(Operation()), false, default);

        Assert.False(result.VerifiedStateCommitAllowed);
    }

    [Fact]
    public async Task ApplyAsync_AllOperationsSucceededAllowsVerifiedStateCommit()
    {
        var executor = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange);

        var result = await Engine(executors: [executor]).ApplyAsync(Plan(Operation()), false, default);

        Assert.True(result.Success);
        Assert.Equal(1, result.AppliedOperationCount);
        Assert.True(result.VerifiedStateCommitAllowed);
    }

    [Fact]
    public void Dispatcher_RejectsDuplicateExecutorRegistration()
    {
        var exception = Assert.Throws<PhysicalUpdateException>(() => new PhysicalOperationDispatcher(
        [
            new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange),
            new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange),
        ]));

        Assert.Equal(UpdateErrorCodes.DuplicateOperationExecutor, exception.Code);
    }

    [Fact]
    public async Task ApplyAsync_EmptyPlanDoesNotDispatchGatewayUpdate()
    {
        var executor = new RecordingOperationExecutor(PhysicalOperationKind.DeleteRange);
        var result = await Engine(executors: [executor]).ApplyAsync(Plan(), dryRun: false, default);

        Assert.True(result.Success);
        Assert.Equal(0, executor.CallCount);
    }

    private static GoogleDocsApplyEngine Engine(
        DocumentRevision? currentRevision = null,
        IEnumerable<IPhysicalOperationExecutor>? executors = null) => new(
            new SnapshotReader(Snapshot(currentRevision ?? Revision(1))),
            new PhysicalOperationDispatcher(executors ?? []));

    private static PhysicalUpdatePlan Plan(params PhysicalUpdateOperation[] operations) => new(
        Identity(),
        Revision(1),
        new DocumentTextRange(10, 20),
        new DiffPlan(
            Fingerprint("old"),
            Fingerprint("new"),
            isFingerprintMatch: false,
            operations.Length == 0
                ? [new DiffOperation(
                    DiffOperationKind.NoChange,
                    0,
                    0,
                    Block("a", "old"),
                    Block("a", "old"),
                    BlockMatchKind.ExplicitId)]
                : operations.Select(operation => new DiffOperation(
                    DiffOperationKind.Update,
                    operation.PreviousIndex,
                    operation.CurrentIndex,
                    Block("a", "old"),
                    Block("a", "new"),
                    BlockMatchKind.ExplicitId)).ToArray()),
        operations);

    private static PhysicalUpdateOperation Operation(
        int sequence = 0,
        PhysicalOperationKind kind = PhysicalOperationKind.DeleteRange) => new(
            sequence,
            kind,
            PhysicalOperationReason.Update,
            0,
            0,
            new DocumentTextRange(10 + sequence, 11 + sequence),
            Block("a", "old"),
            kind == PhysicalOperationKind.InsertBlock ? new DocumentBlock(ParagraphBlock.FromText("new"), "a") : null,
            kind is PhysicalOperationKind.InsertInlineText or
                PhysicalOperationKind.ReplaceInlineContent or
                PhysicalOperationKind.UpdateInlineStyle
                ? new InlinePhysicalUpdate("text", [], null, null, null)
                : null);

    private static ManagedDocumentSnapshot Snapshot(DocumentRevision revision) => new(
        Identity(),
        revision,
        new DocumentTextRange(10, 20),
        Fingerprint("old").Value,
        [new ManagedBlockSnapshot(Block("a", "old"), new DocumentTextRange(10, 20))]);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static DocumentRevision Revision(long sequence) => new("revision-" + sequence, sequence);

    private static BlockIdentity Block(string id, string hash) =>
        new(id, null, "ch-v1:sha256:" + Hash(hash));

    private static PublishFingerprint Fingerprint(string seed) => new("v1:sha256:" + Hash(seed));

    private static string Hash(string seed) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(seed))).ToLowerInvariant();

    private sealed class SnapshotReader : IDocumentSnapshotReader
    {
        private readonly ManagedDocumentSnapshot snapshot;

        internal SnapshotReader(ManagedDocumentSnapshot snapshot)
        {
            this.snapshot = snapshot;
        }

        public Task<ManagedDocumentSnapshot> GetSnapshotAsync(
            DocumentIdentity identity,
            CancellationToken cancellationToken) => Task.FromResult(snapshot);
    }

    private sealed class RecordingOperationExecutor : IPhysicalOperationExecutor
    {
        private readonly List<string>? calls;

        internal RecordingOperationExecutor(PhysicalOperationKind operationKind, List<string>? calls = null)
        {
            OperationKind = operationKind;
            this.calls = calls;
        }

        public PhysicalOperationKind OperationKind { get; }

        public int CallCount { get; private set; }

        public string? ResultCode { get; init; }

        public Exception? Exception { get; init; }

        public Task<PhysicalOperationExecutionResult> ExecuteAsync(
            PhysicalUpdatePlan plan,
            PhysicalUpdateOperation operation,
            int operationIndex,
            CancellationToken cancellationToken)
        {
            CallCount++;
            calls?.Add($"{operationIndex}:{operation.Kind}");
            if (Exception is not null)
            {
                throw Exception;
            }

            if (ResultCode is not null)
            {
                return Task.FromResult(PhysicalOperationExecutionResult.Failed(
                    operation,
                    operationIndex,
                    ResultCode,
                    "injected operation failure"));
            }

            return Task.FromResult(PhysicalOperationExecutionResult.Succeeded(operation, operationIndex));
        }
    }
}
