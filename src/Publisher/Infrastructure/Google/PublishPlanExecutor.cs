using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Executes ordinary batches and two-stage table insertion steps.</summary>
public sealed class PublishPlanExecutor : IPublishPlanExecutor
{
    private readonly IGoogleDocsClient docsClient;
    private readonly InlineContentRenderer inlineRenderer;

    /// <summary>Initializes a publish-plan executor.</summary>
    /// <param name="docsClient">The Google Docs client.</param>
    public PublishPlanExecutor(IGoogleDocsClient docsClient)
        : this(docsClient, new InlineContentRenderer())
    {
    }

    /// <summary>Initializes a publish-plan executor.</summary>
    /// <param name="docsClient">The Google Docs client.</param>
    /// <param name="inlineRenderer">The inline renderer used for table cells.</param>
    public PublishPlanExecutor(
        IGoogleDocsClient docsClient,
        InlineContentRenderer inlineRenderer)
    {
        this.docsClient = docsClient ?? throw new ArgumentNullException(nameof(docsClient));
        this.inlineRenderer = inlineRenderer ?? throw new ArgumentNullException(nameof(inlineRenderer));
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(
        string documentId,
        IReadOnlyList<PublishStep> steps,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        ArgumentNullException.ThrowIfNull(steps);
        var currentIndex = 1;

        foreach (var step in steps)
        {
            cancellationToken.ThrowIfCancellationRequested();
            switch (step)
            {
                case BatchUpdateStep batch:
                    await docsClient.ApplyOperationsAsync(
                        documentId,
                        Rebase(batch.Operations, currentIndex - 1),
                        cancellationToken).ConfigureAwait(false);
                    currentIndex += batch.ContentLength;
                    break;

                case InsertTableStep tableStep:
                    currentIndex = await ExecuteTableAsync(
                        documentId,
                        tableStep.Table,
                        currentIndex,
                        cancellationToken).ConfigureAwait(false);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported publish step: {step.GetType().Name}");
            }
        }
    }

    private async Task<int> ExecuteTableAsync(
        string documentId,
        TableBlock table,
        int insertionIndex,
        CancellationToken cancellationToken)
    {
        await docsClient.InsertTableAsync(
            documentId,
            table.AllRows.Count,
            table.Columns.Count,
            insertionIndex,
            cancellationToken).ConfigureAwait(false);

        var insertedDocument = await docsClient.GetDocumentAsync(documentId, cancellationToken)
            .ConfigureAwait(false);
        var insertedTable = FindInsertedTable(insertedDocument, insertionIndex);
        ValidateDimensions(insertedTable, table);
        var operations = CreateCellOperations(table, insertedTable);

        try
        {
            await docsClient.ApplyOperationsAsync(documentId, operations, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PublishPipelineException(
                PublishErrorCodes.TableContentUpdateFailed,
                "Google Docs could not update the inserted table content.",
                exception);
        }

        var updatedDocument = await docsClient.GetDocumentAsync(documentId, cancellationToken)
            .ConfigureAwait(false);
        var updatedTable = FindInsertedTable(
            updatedDocument,
            insertedTable.StartIndex ?? insertionIndex);
        ValidateDimensions(updatedTable, table);
        return updatedTable.EndIndex ?? throw new PublishPipelineException(
            PublishErrorCodes.TableCellIndexMissing,
            "The inserted table did not expose an end index.");
    }

    private IReadOnlyList<DocumentOperation> CreateCellOperations(
        TableBlock source,
        GoogleTableSnapshot target)
    {
        var sourceRows = source.AllRows;
        var operations = new List<DocumentOperation>();
        for (var rowIndex = sourceRows.Count - 1; rowIndex >= 0; rowIndex--)
        {
            for (var columnIndex = source.Columns.Count - 1; columnIndex >= 0; columnIndex--)
            {
                var targetCell = target.Rows[rowIndex].Cells[columnIndex];
                var startIndex = targetCell.StartIndex ?? throw MissingCellIndex(rowIndex, columnIndex);
                var endIndex = targetCell.EndIndex ?? throw MissingCellIndex(rowIndex, columnIndex);
                if (startIndex < 1 || endIndex <= startIndex)
                {
                    throw MissingCellIndex(rowIndex, columnIndex);
                }

                var rendered = inlineRenderer.Render(sourceRows[rowIndex].Cells[columnIndex].Content);
                if (rendered.Text.Length > 0)
                {
                    operations.Add(new DocumentOperation(
                        DocumentOperationKind.InsertText,
                        startIndex,
                        text: rendered.Text));
                    InlineStyleOperationAppender.Append(rendered, startIndex, operations);
                    if (rowIndex == 0)
                    {
                        // Keep the header bold operation after code font-family changes.
                        operations.Add(new DocumentOperation(
                            DocumentOperationKind.UpdateTextStyle,
                            startIndex,
                            startIndex + rendered.Text.Length,
                            inlineStyle: InlineTextStyle.Bold));
                    }
                }

                operations.Add(new DocumentOperation(
                    DocumentOperationKind.UpdateParagraphAlignment,
                    startIndex,
                    endIndex + rendered.Text.Length,
                    tableAlignment: source.Columns[columnIndex].Alignment));
            }
        }

        return Array.AsReadOnly(operations.ToArray());
    }

    private static GoogleTableSnapshot FindInsertedTable(
        GoogleDocumentSnapshot document,
        int insertionIndex)
    {
        var table = document.Tables
            .Where(candidate => candidate.StartIndex is not null &&
                candidate.StartIndex.Value >= insertionIndex &&
                candidate.StartIndex.Value <= insertionIndex + 1)
            .OrderBy(candidate => candidate.StartIndex)
            .FirstOrDefault();
        return table ?? throw new PublishPipelineException(
            PublishErrorCodes.TableNotFoundAfterInsert,
            "The inserted table was not found in the Google Docs readback.");
    }

    private static void ValidateDimensions(GoogleTableSnapshot target, TableBlock source)
    {
        if (target.Rows.Count != source.AllRows.Count ||
            target.Rows.Any(row => row.Cells.Count != source.Columns.Count))
        {
            throw new PublishPipelineException(
                PublishErrorCodes.TableDimensionMismatch,
                "The inserted table dimensions did not match the publish plan.");
        }
    }

    private static PublishPipelineException MissingCellIndex(int rowIndex, int columnIndex) =>
        new(
            PublishErrorCodes.TableCellIndexMissing,
            $"The inserted table cell at row {rowIndex} and column {columnIndex} did not expose a valid index.");

    private static IReadOnlyList<DocumentOperation> Rebase(
        IEnumerable<DocumentOperation> operations,
        int offset) => operations.Select(operation => new DocumentOperation(
            operation.Kind,
            operation.StartIndex + offset,
            operation.EndIndex + offset,
            operation.Text,
            operation.Level,
            operation.ListKind,
            operation.InlineStyle,
            operation.Url,
            operation.TableAlignment)).ToArray();
}
