using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Maps physical update plans to Google Docs batchUpdate requests.</summary>
public sealed class GoogleDocsPhysicalUpdateRequestMapper : IPhysicalUpdateRequestMapper
{
    private readonly GeneratedBlockRenderer renderer;

    /// <summary>Initializes a Google Docs physical update request mapper.</summary>
    public GoogleDocsPhysicalUpdateRequestMapper()
        : this(new GeneratedBlockRenderer())
    {
    }

    /// <summary>Initializes a Google Docs physical update request mapper.</summary>
    public GoogleDocsPhysicalUpdateRequestMapper(GeneratedBlockRenderer renderer)
    {
        this.renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    /// <inheritdoc />
    public PhysicalUpdateRequestBatch Map(PhysicalUpdatePlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var requests = new List<object>();
        var traces = new List<PhysicalUpdateRequestTrace>();

        foreach (var operation in plan.Operations)
        {
            switch (operation.Kind)
            {
                case PhysicalOperationKind.DeleteRange:
                    AddRequest(
                        requests,
                        traces,
                        operation,
                        "deleteContentRange",
                        new
                        {
                            deleteContentRange = new
                            {
                                range = new
                                {
                                    startIndex = operation.AffectedRange.StartIndex,
                                    endIndex = operation.AffectedRange.EndIndex,
                                },
                            },
                        });
                    break;
                case PhysicalOperationKind.InsertBlock:
                    MapInsert(operation, requests, traces);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported physical operation: {operation.Kind}");
            }
        }

        return new PhysicalUpdateRequestBatch(
            plan.Identity.GoogleDocumentId
                ?? throw new InvalidOperationException("The physical plan document ID is missing."),
            plan.RequiredRevision.RevisionId
                ?? throw new InvalidOperationException("The physical plan required revision is missing."),
            requests,
            plan.Operations.Count,
            traces);
    }

    private void MapInsert(
        PhysicalUpdateOperation operation,
        List<object> requests,
        List<PhysicalUpdateRequestTrace> traces)
    {
        var block = operation.CandidateBlock ?? throw new InvalidOperationException(
            "Insert operations require canonical candidate payload.");
        var steps = renderer.Render(new DocumentModel([block]));
        if (steps.Count != 1 || steps[0] is not BatchUpdateStep batchStep)
        {
            throw new InvalidOperationException(
                "Physical update batchUpdate mapping supports canonical text-style blocks only.");
        }

        foreach (var documentOperation in batchStep.Operations.Select(item =>
                     Shift(item, operation.AffectedRange.StartIndex - 1)))
        {
            var request = MapDocumentOperation(documentOperation);
            AddRequest(
                requests,
                traces,
                operation,
                RequestKind(documentOperation.Kind),
                request);
        }
    }

    private static DocumentOperation Shift(DocumentOperation operation, int offset) => new(
        operation.Kind,
        operation.StartIndex + offset,
        operation.EndIndex is null ? null : operation.EndIndex.Value + offset,
        operation.Text,
        operation.Level,
        operation.ListKind,
        operation.InlineStyle,
        operation.Url,
        operation.TableAlignment);

    private static void AddRequest(
        List<object> requests,
        List<PhysicalUpdateRequestTrace> traces,
        PhysicalUpdateOperation operation,
        string requestKind,
        object request)
    {
        requests.Add(request);
        traces.Add(new PhysicalUpdateRequestTrace(
            requests.Count - 1,
            operation.Sequence,
            operation.Reason,
            operation.TraceIdentity,
            requestKind));
    }

    private static object MapDocumentOperation(DocumentOperation operation) => operation.Kind switch
    {
        DocumentOperationKind.InsertText => MapInsertText(operation),
        DocumentOperationKind.ApplyHeading => MapHeading(operation),
        DocumentOperationKind.CreateBullet => MapBullet(operation),
        DocumentOperationKind.UpdateTextStyle => MapTextStyle(operation),
        DocumentOperationKind.UpdateParagraphAlignment => MapParagraphAlignment(operation),
        DocumentOperationKind.ApplyCodeBlockStyle => MapCodeBlockStyle(operation),
        DocumentOperationKind.ApplyQuoteBlockStyle => MapQuoteBlockStyle(operation),
        _ => throw new InvalidOperationException($"Unsupported operation: {operation.Kind}"),
    };

    private static string RequestKind(DocumentOperationKind kind) => kind switch
    {
        DocumentOperationKind.InsertText => "insertText",
        DocumentOperationKind.ApplyHeading => "updateParagraphStyle",
        DocumentOperationKind.CreateBullet => "createParagraphBullets",
        DocumentOperationKind.UpdateTextStyle => "updateTextStyle",
        DocumentOperationKind.UpdateParagraphAlignment => "updateParagraphStyle",
        DocumentOperationKind.ApplyCodeBlockStyle => "updateParagraphStyle",
        DocumentOperationKind.ApplyQuoteBlockStyle => "updateParagraphStyle",
        _ => throw new InvalidOperationException($"Unsupported operation: {kind}"),
    };

    private static object MapInsertText(DocumentOperation operation)
    {
        if (operation.Text is null)
        {
            throw new InvalidOperationException("InsertText requires text.");
        }

        return new
        {
            insertText = new
            {
                location = new { index = operation.StartIndex },
                text = operation.Text,
            },
        };
    }

    private static object MapHeading(DocumentOperation operation)
    {
        if (operation.EndIndex is null || operation.Level is null or < 1 or > 6)
        {
            throw new InvalidOperationException("ApplyHeading requires a range and heading level 1 through 6.");
        }

        return new
        {
            updateParagraphStyle = new
            {
                range = Range(operation),
                paragraphStyle = new { namedStyleType = $"HEADING_{operation.Level.Value}" },
                fields = "namedStyleType",
            },
        };
    }

    private static object MapBullet(DocumentOperation operation)
    {
        if (operation.EndIndex is null)
        {
            throw new InvalidOperationException("CreateBullet requires a range.");
        }

        return new
        {
            createParagraphBullets = new
            {
                range = Range(operation),
                bulletPreset = operation.ListKind switch
                {
                    ListKind.Ordered => "NUMBERED_DECIMAL_ALPHA_ROMAN",
                    ListKind.Unordered or null => "BULLET_DISC_CIRCLE_SQUARE",
                    _ => throw new InvalidOperationException($"Unsupported list kind: {operation.ListKind}"),
                },
            },
        };
    }

    private static object MapTextStyle(DocumentOperation operation)
    {
        if (operation.EndIndex is null || operation.InlineStyle is null)
        {
            throw new InvalidOperationException("UpdateTextStyle requires a range and inline style.");
        }

        var textStyle = operation.InlineStyle switch
        {
            InlineTextStyle.Bold => (object)new { bold = true },
            InlineTextStyle.Italic => new { italic = true },
            InlineTextStyle.Code => new
            {
                weightedFontFamily = new { fontFamily = "Roboto Mono" },
                backgroundColor = new
                {
                    color = new
                    {
                        rgbColor = new { red = 0.95, green = 0.95, blue = 0.95 },
                    },
                },
            },
            InlineTextStyle.Link when operation.Url is not null => new
            {
                link = new { url = operation.Url.AbsoluteUri },
            },
            InlineTextStyle.Link => throw new InvalidOperationException("A link text style requires a URL."),
            _ => throw new InvalidOperationException($"Unsupported inline text style: {operation.InlineStyle}"),
        };
        var fields = operation.InlineStyle switch
        {
            InlineTextStyle.Bold => "bold",
            InlineTextStyle.Italic => "italic",
            InlineTextStyle.Code => "weightedFontFamily,backgroundColor",
            InlineTextStyle.Link => "link",
            _ => throw new InvalidOperationException($"Unsupported inline text style: {operation.InlineStyle}"),
        };

        return new
        {
            updateTextStyle = new
            {
                range = Range(operation),
                textStyle,
                fields,
            },
        };
    }

    private static object MapParagraphAlignment(DocumentOperation operation)
    {
        if (operation.EndIndex is null || operation.TableAlignment is null)
        {
            throw new InvalidOperationException("UpdateParagraphAlignment requires a range and table alignment.");
        }

        return new
        {
            updateParagraphStyle = new
            {
                range = Range(operation),
                paragraphStyle = new
                {
                    alignment = operation.TableAlignment switch
                    {
                        TableAlignment.Left => "START",
                        TableAlignment.Center => "CENTER",
                        TableAlignment.Right => "END",
                        _ => throw new InvalidOperationException(
                            $"Unsupported table alignment: {operation.TableAlignment}"),
                    },
                },
                fields = "alignment",
            },
        };
    }

    private static object MapCodeBlockStyle(DocumentOperation operation)
    {
        if (operation.EndIndex is null)
        {
            throw new InvalidOperationException("ApplyCodeBlockStyle requires a range.");
        }

        return new
        {
            updateParagraphStyle = new
            {
                range = Range(operation),
                paragraphStyle = new
                {
                    indentStart = Dimension(18),
                    indentEnd = Dimension(18),
                    spaceAbove = Dimension(6),
                    spaceBelow = Dimension(6),
                },
                fields = "indentStart,indentEnd,spaceAbove,spaceBelow",
            },
        };
    }

    private static object MapQuoteBlockStyle(DocumentOperation operation)
    {
        if (operation.EndIndex is null || operation.Level is null or < 1 or > 6)
        {
            throw new InvalidOperationException("ApplyQuoteBlockStyle requires a range and quote level 1 through 6.");
        }

        return new
        {
            updateParagraphStyle = new
            {
                range = Range(operation),
                paragraphStyle = new
                {
                    indentStart = Dimension(18 * operation.Level.Value),
                    indentFirstLine = Dimension(0),
                    spaceAbove = Dimension(3),
                    spaceBelow = Dimension(3),
                },
                fields = "indentStart,indentFirstLine,spaceAbove,spaceBelow",
            },
        };
    }

    private static object Range(DocumentOperation operation) => new
    {
        startIndex = operation.StartIndex,
        endIndex = operation.EndIndex!.Value,
    };

    private static object Dimension(double magnitude) => new { magnitude, unit = "PT" };
}
