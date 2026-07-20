using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

internal static class InlineStyleOperationAppender
{
    internal static void Append(
        RenderedInlineContent content,
        int startIndex,
        ICollection<DocumentOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(operations);
        // Google Docs can reset the effective bold weight when weightedFontFamily is
        // applied. Emit code formatting first so overlapping bold remains authoritative.
        foreach (var range in content.StyleRanges
            .OrderBy(range => range.Style == InlineTextStyle.Code ? 0 : 1)
            .ThenBy(range => range.StartOffset)
            .ThenBy(range => range.EndOffset)
            .ThenBy(range => range.Style))
        {
            operations.Add(new DocumentOperation(
                DocumentOperationKind.UpdateTextStyle,
                startIndex + range.StartOffset,
                startIndex + range.EndOffset,
                inlineStyle: range.Style,
                url: range.Url));
        }
    }
}
