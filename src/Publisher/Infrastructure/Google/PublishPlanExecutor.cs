using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Executes ordinary batches and two-stage table insertion steps.</summary>
public sealed class PublishPlanExecutor : IPublishPlanExecutor
{
    private readonly IGoogleDocsClient docsClient;
    private readonly InlineContentRenderer inlineRenderer;
    private readonly ITemporaryImageHost? temporaryImageHost;
    private readonly IPublisherLogger logger;

    /// <summary>Initializes a publish-plan executor.</summary>
    /// <param name="docsClient">The Google Docs client.</param>
    public PublishPlanExecutor(IGoogleDocsClient docsClient)
        : this(docsClient, new InlineContentRenderer(), null, null)
    {
    }

    /// <summary>Initializes a publish-plan executor.</summary>
    /// <param name="docsClient">The Google Docs client.</param>
    /// <param name="inlineRenderer">The inline renderer used for table cells.</param>
    public PublishPlanExecutor(
        IGoogleDocsClient docsClient,
        InlineContentRenderer inlineRenderer)
        : this(docsClient, inlineRenderer, null, null)
    {
    }

    /// <summary>Initializes a publish-plan executor with image services.</summary>
    public PublishPlanExecutor(
        IGoogleDocsClient docsClient,
        InlineContentRenderer inlineRenderer,
        ITemporaryImageHost? temporaryImageHost,
        IPublisherLogger? logger)
    {
        this.docsClient = docsClient ?? throw new ArgumentNullException(nameof(docsClient));
        this.inlineRenderer = inlineRenderer ?? throw new ArgumentNullException(nameof(inlineRenderer));
        this.temporaryImageHost = temporaryImageHost;
        this.logger = logger ?? new NullPublisherLogger();
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

                case InsertImageStep imageStep:
                    currentIndex = await ExecuteImageAsync(
                        documentId,
                        imageStep.Image,
                        currentIndex,
                        cancellationToken).ConfigureAwait(false);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported publish step: {step.GetType().Name}");
            }
        }
    }

    private async Task<int> ExecuteImageAsync(
        string documentId,
        ImageBlock image,
        int insertionIndex,
        CancellationToken cancellationToken)
    {
        var size = image.Size ?? throw new PublishPipelineException(
            PublishErrorCodes.ImageSizeInvalid,
            "Prepared image has no calculated size.");
        TemporaryHostedImage? hostedImage = null;
        Exception? primaryException = null;
        try
        {
            var insertionUri = image.Source switch
            {
                RemoteImageSource remote => remote.Uri,
                LocalImageSource local when temporaryImageHost is not null =>
                    (hostedImage = await temporaryImageHost.HostAsync(local, cancellationToken)
                        .ConfigureAwait(false)).PublicUri,
                LocalImageSource => throw new PublishPipelineException(
                    PublishErrorCodes.ImageUploadFailed,
                    "Temporary image hosting is not configured."),
                _ => throw new PublishPipelineException(
                    PublishErrorCodes.ImageRemoteUriInvalid,
                    "Prepared image source type is invalid."),
            };

            try
            {
                await docsClient.InsertInlineImageAsync(
                    documentId,
                    insertionUri,
                    size,
                    insertionIndex,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.ImageInsertFailed,
                    "Google Docs could not insert the image.",
                    exception);
            }

            GoogleDocumentSnapshot document;
            try
            {
                document = await docsClient.GetDocumentAsync(documentId, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.ImageInsertFailed,
                    "Google Docs image readback failed.",
                    exception);
            }

            var inserted = document.Images
                .Where(candidate => candidate.ElementStartIndex is not null &&
                    candidate.ElementStartIndex.Value >= insertionIndex &&
                    candidate.ElementStartIndex.Value <= insertionIndex + 1)
                .OrderBy(candidate => candidate.ElementStartIndex)
                .FirstOrDefault();
            if (inserted is null || string.IsNullOrWhiteSpace(inserted.InlineObjectId) ||
                inserted.ActualSize is null)
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.ImageNotFoundAfterInsert,
                    "The inserted inline image and its actual size were not found in the Google Docs readback.");
            }

            if (!ApproximatelyEqual(inserted.ActualSize.WidthPoints, size.WidthPoints) ||
                !ApproximatelyEqual(inserted.ActualSize.HeightPoints, size.HeightPoints))
            {
                throw new PublishPipelineException(
                    PublishErrorCodes.ImageInsertFailed,
                    "The inserted image size did not match the publish plan.");
            }

            if (!string.IsNullOrEmpty(image.AltText))
            {
                logger.Warning(
                    PublishErrorCodes.ImageAltTextUpdateFailed,
                    "Google Docs InsertInlineImageRequest cannot set Title or Description; Alt Text remains in the publish model only.");
            }

            return inserted.ParagraphEndIndex is > 0 and var followingIndex &&
                followingIndex > insertionIndex
                ? followingIndex
                : throw new PublishPipelineException(
                    PublishErrorCodes.ImageFollowingIndexNotFound,
                    "The image paragraph did not expose a valid EndIndex.");
        }
        catch (Exception exception)
        {
            primaryException = exception;
            throw;
        }
        finally
        {
            if (hostedImage is not null && temporaryImageHost is not null)
            {
                try
                {
                    await temporaryImageHost.DeleteAsync(hostedImage, CancellationToken.None)
                        .ConfigureAwait(false);
                }
                catch (Exception)
                {
                    logger.Warning(
                        PublishErrorCodes.ImageTempFileDeleteFailed,
                        "Temporary Drive image cleanup failed.");
                    if (primaryException is null)
                    {
                        throw;
                    }
                }
            }
        }
    }

    private static bool ApproximatelyEqual(double actual, double requested) =>
        Math.Abs(actual - requested) <= 0.5d;

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
