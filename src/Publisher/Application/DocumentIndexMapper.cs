using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Maps canonical UTF-16 inline offsets to document indexes within a managed block range.</summary>
internal sealed class DocumentIndexMapper
{
    internal DocumentTextRange MapInlineRange(
        DocumentTextRange blockRange,
        int startOffset,
        int endOffset)
    {
        ArgumentNullException.ThrowIfNull(blockRange);
        ArgumentOutOfRangeException.ThrowIfNegative(startOffset);
        if (endOffset < startOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(endOffset));
        }

        if (endOffset > blockRange.Length - 1)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.PhysicalPlanInvalid,
                "A canonical inline offset extends outside the managed block text.");
        }

        return new DocumentTextRange(
            blockRange.StartIndex + startOffset,
            blockRange.StartIndex + endOffset);
    }
}
