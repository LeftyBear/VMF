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
        foreach (var range in content.StyleRanges)
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
