using System.Text.Json;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Maps compiled document operations to Google Docs REST requests.</summary>
public sealed class GoogleDocsRequestMapper : IGoogleDocsRequestMapper
{
    /// <inheritdoc />
    public string MapBatchUpdate(IReadOnlyList<DocumentOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);
        var requests = new List<object>(operations.Count);

        foreach (var operation in operations)
        {
            requests.Add(operation.Kind switch
            {
                DocumentOperationKind.InsertText => MapInsertText(operation),
                DocumentOperationKind.ApplyHeading => MapHeading(operation),
                DocumentOperationKind.CreateBullet => MapBullet(operation),
                DocumentOperationKind.UpdateTextStyle => MapTextStyle(operation),
                DocumentOperationKind.UpdateParagraphAlignment => MapParagraphAlignment(operation),
                DocumentOperationKind.ApplyCodeBlockStyle => MapCodeBlockStyle(operation),
                DocumentOperationKind.ApplyQuoteBlockStyle => MapQuoteBlockStyle(operation),
                _ => throw new InvalidOperationException($"Unsupported operation: {operation.Kind}"),
            });
        }

        return JsonSerializer.Serialize(new { requests });
    }

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
        var level = operation.Level;
        if (operation.EndIndex is null || level is null or < 1 or > 6)
        {
            throw new InvalidOperationException("ApplyHeading requires a range and heading level 1 through 6.");
        }

        return new
        {
            updateParagraphStyle = new
            {
                range = new { startIndex = operation.StartIndex, endIndex = operation.EndIndex.Value },
                paragraphStyle = new { namedStyleType = $"HEADING_{level.Value}" },
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
                range = new { startIndex = operation.StartIndex, endIndex = operation.EndIndex.Value },
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
            InlineTextStyle.Link => throw new InvalidOperationException(
                "A link text style requires a URL."),
            _ => throw new InvalidOperationException(
                $"Unsupported inline text style: {operation.InlineStyle}"),
        };

        var fields = operation.InlineStyle switch
        {
            InlineTextStyle.Bold => "bold",
            InlineTextStyle.Italic => "italic",
            InlineTextStyle.Code => "weightedFontFamily,backgroundColor",
            InlineTextStyle.Link => "link",
            _ => throw new InvalidOperationException(
                $"Unsupported inline text style: {operation.InlineStyle}"),
        };

        return new
        {
            updateTextStyle = new
            {
                range = new { startIndex = operation.StartIndex, endIndex = operation.EndIndex.Value },
                textStyle,
                fields,
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
                range = new { startIndex = operation.StartIndex, endIndex = operation.EndIndex.Value },
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
            throw new InvalidOperationException(
                "ApplyQuoteBlockStyle requires a range and quote level 1 through 6.");
        }

        return new
        {
            updateParagraphStyle = new
            {
                range = new { startIndex = operation.StartIndex, endIndex = operation.EndIndex.Value },
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

    private static object MapParagraphAlignment(DocumentOperation operation)
    {
        if (operation.EndIndex is null || operation.TableAlignment is null)
        {
            throw new InvalidOperationException(
                "UpdateParagraphAlignment requires a range and table alignment.");
        }

        return new
        {
            updateParagraphStyle = new
            {
                range = new { startIndex = operation.StartIndex, endIndex = operation.EndIndex.Value },
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

    private static object Dimension(double magnitude) => new { magnitude, unit = "PT" };
}
