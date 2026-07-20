using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class PublishPlanExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_UsesReadTableIndexesAndReverseCellInsertionOrder()
    {
        var table = CreateTable();
        var client = new RecordingDocsClient(
            Snapshot(8, 40, 10, 12, 14, 20, 22, 24),
            Snapshot(8, 100, 10, 13, 16, 23, 25, 28));
        var executor = new PublishPlanExecutor(client);
        PublishStep[] steps =
        [
            new BatchUpdateStep(
                [new DocumentOperation(DocumentOperationKind.InsertText, 1, text: "Before\n")],
                7),
            new InsertTableStep(table),
            new BatchUpdateStep(
                [new DocumentOperation(DocumentOperationKind.InsertText, 1, text: "After\n")],
                6),
        ];

        await executor.ExecuteAsync("document-id", steps, CancellationToken.None);

        Assert.Equal((2, 3, 8), client.InsertedTable);
        Assert.Equal(3, client.Applied.Count);
        var tableOperations = client.Applied[1];
        Assert.Equal(
            [24, 20, 14, 12, 10],
            tableOperations
                .Where(operation => operation.Kind == DocumentOperationKind.InsertText)
                .Select(operation => operation.StartIndex));
        Assert.DoesNotContain(
            tableOperations,
            operation => operation.Kind == DocumentOperationKind.InsertText && operation.StartIndex == 22);
        Assert.Equal(
            6,
            tableOperations.Count(operation =>
                operation.Kind == DocumentOperationKind.UpdateParagraphAlignment));
        Assert.Contains(
            tableOperations,
            operation => operation.Kind == DocumentOperationKind.UpdateParagraphAlignment &&
                operation.TableAlignment == TableAlignment.Center);
        Assert.Contains(
            tableOperations,
            operation => operation.Kind == DocumentOperationKind.UpdateParagraphAlignment &&
                operation.TableAlignment == TableAlignment.Right);
        Assert.Equal(
            3,
            tableOperations.Count(operation =>
                operation.Kind == DocumentOperationKind.UpdateTextStyle &&
                operation.InlineStyle == InlineTextStyle.Bold &&
                operation.StartIndex is 10 or 12 or 14));
        Assert.Contains(
            tableOperations,
            operation => operation.Kind == DocumentOperationKind.UpdateTextStyle &&
                operation.InlineStyle == InlineTextStyle.Italic &&
                operation.StartIndex == 24);
        Assert.Contains(
            tableOperations,
            operation => operation.Kind == DocumentOperationKind.UpdateTextStyle &&
                operation.InlineStyle == InlineTextStyle.Code &&
                operation.StartIndex == 24 &&
                operation.EndIndex == 25);
        var tableOperationItems = tableOperations.ToArray();
        Assert.True(
            Array.FindIndex(tableOperationItems, operation =>
                operation.InlineStyle == InlineTextStyle.Code && operation.StartIndex == 24) <
            Array.FindIndex(tableOperationItems, operation =>
                operation.InlineStyle == InlineTextStyle.Italic && operation.StartIndex == 24));
        Assert.Equal(100, Assert.Single(client.Applied[2]).StartIndex);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTableIsMissing_StopsFollowingSteps()
    {
        var client = new RecordingDocsClient(new GoogleDocumentSnapshot([]));
        var executor = new PublishPlanExecutor(client);
        PublishStep[] steps =
        [
            new InsertTableStep(CreateTable()),
            new BatchUpdateStep(
                [new DocumentOperation(DocumentOperationKind.InsertText, 1, text: "Never\n")],
                6),
        ];

        var exception = await Assert.ThrowsAsync<PublishPipelineException>(() =>
            executor.ExecuteAsync("document-id", steps, CancellationToken.None));

        Assert.Equal(PublishErrorCodes.TableNotFoundAfterInsert, exception.Code);
        Assert.Empty(client.Applied);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDimensionsDiffer_ReportsStableError()
    {
        var client = new RecordingDocsClient(Snapshot(1, 20, 3, 5));

        var exception = await Assert.ThrowsAsync<PublishPipelineException>(() =>
            new PublishPlanExecutor(client).ExecuteAsync(
                "document-id",
                [new InsertTableStep(CreateTable())],
                CancellationToken.None));

        Assert.Equal(PublishErrorCodes.TableDimensionMismatch, exception.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCellIndexIsMissing_ReportsStableError()
    {
        var rows = new[]
        {
            new GoogleTableRowSnapshot([
                new GoogleTableCellSnapshot(null, 4),
                new GoogleTableCellSnapshot(5, 6),
                new GoogleTableCellSnapshot(7, 8),
            ]),
            new GoogleTableRowSnapshot([
                new GoogleTableCellSnapshot(9, 10),
                new GoogleTableCellSnapshot(11, 12),
                new GoogleTableCellSnapshot(13, 14),
            ]),
        };
        var client = new RecordingDocsClient(
            new GoogleDocumentSnapshot([new GoogleTableSnapshot(1, 20, rows)]));

        var exception = await Assert.ThrowsAsync<PublishPipelineException>(() =>
            new PublishPlanExecutor(client).ExecuteAsync(
                "document-id",
                [new InsertTableStep(CreateTable())],
                CancellationToken.None));

        Assert.Equal(PublishErrorCodes.TableCellIndexMissing, exception.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContentBatchFails_ReportsStableError()
    {
        var client = new RecordingDocsClient(Snapshot(1, 40, 3, 5, 7, 9, 11, 13))
        {
            FailApply = true,
        };

        var exception = await Assert.ThrowsAsync<PublishPipelineException>(() =>
            new PublishPlanExecutor(client).ExecuteAsync(
                "document-id",
                [new InsertTableStep(CreateTable())],
                CancellationToken.None));

        Assert.Equal(PublishErrorCodes.TableContentUpdateFailed, exception.Code);
    }

    private static TableBlock CreateTable() => new(
        [
            new TableColumn(TableAlignment.Left),
            new TableColumn(TableAlignment.Center),
            new TableColumn(TableAlignment.Right),
        ],
        new TableRow([
            new TableCell([new TextInline("H1")]),
            new TableCell([new TextInline("H2")]),
            new TableCell([new TextInline("H3")]),
        ]),
        [new TableRow([
            new TableCell([new TextInline("A")]),
            TableCell.Empty(),
            new TableCell([new ItalicInline([new CodeInline("B")])]),
        ])]);

    private static GoogleDocumentSnapshot Snapshot(
        int startIndex,
        int endIndex,
        params int[] cellStartIndexes)
    {
        var cells = cellStartIndexes
            .Select(index => new GoogleTableCellSnapshot(index, index + 1))
            .ToArray();
        var rows = cells
            .Chunk(3)
            .Select(row => new GoogleTableRowSnapshot(row))
            .ToArray();
        return new GoogleDocumentSnapshot([new GoogleTableSnapshot(startIndex, endIndex, rows)]);
    }

    private sealed class RecordingDocsClient : IGoogleDocsClient
    {
        private readonly Queue<GoogleDocumentSnapshot> snapshots;

        internal RecordingDocsClient(params GoogleDocumentSnapshot[] snapshots)
        {
            this.snapshots = new Queue<GoogleDocumentSnapshot>(snapshots);
        }

        internal List<IReadOnlyList<DocumentOperation>> Applied { get; } = [];

        internal (int Rows, int Columns, int Index)? InsertedTable { get; private set; }

        internal bool FailApply { get; init; }

        public Task ApplyOperationsAsync(
            string documentId,
            IReadOnlyList<DocumentOperation> operations,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (FailApply)
            {
                throw new HttpRequestException("failure");
            }

            Applied.Add(operations);
            return Task.CompletedTask;
        }

        public Task InsertTableAsync(
            string documentId,
            int rows,
            int columns,
            int index,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            InsertedTable = (rows, columns, index);
            return Task.CompletedTask;
        }

        public Task<GoogleDocumentSnapshot> GetDocumentAsync(
            string documentId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(snapshots.Dequeue());
        }
    }
}
