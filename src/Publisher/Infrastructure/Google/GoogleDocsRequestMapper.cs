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
                bulletPreset = "BULLET_DISC_CIRCLE_SQUARE",
            },
        };
    }
}
