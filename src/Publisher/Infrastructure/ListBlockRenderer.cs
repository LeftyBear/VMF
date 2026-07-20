using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Renders list blocks into target-neutral document operations.</summary>
public sealed class ListBlockRenderer
{
    /// <summary>Appends operations for one list block and returns the next document index.</summary>
    /// <param name="block">The list block.</param>
    /// <param name="startIndex">The first Google Docs text index available to the block.</param>
    /// <param name="operations">The destination operation collection.</param>
    /// <returns>The first text index after the rendered list.</returns>
    public int Render(
        ListBlock block,
        int startIndex,
        ICollection<DocumentOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(operations);
        if (startIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        var text = new StringBuilder();
        var itemRanges = new List<ItemRange>(block.Items.Count);
        foreach (var item in block.Items)
        {
            var offset = text.Length;
            text.Append('\t', item.Depth);
            text.Append(string.Concat(item.Inlines.Select(element => element.Text)));
            text.Append('\n');
            itemRanges.Add(new ItemRange(offset, text.Length, item.Depth, item.Kind));
        }

        operations.Add(new DocumentOperation(
            DocumentOperationKind.InsertText,
            startIndex,
            text: text.ToString()));

        // Google Docs derives list nesting from leading tabs and removes those tabs when
        // CreateParagraphBulletsRequest is applied. Process kind runs from top to bottom
        // and compensate every later range for tabs already removed by earlier requests.
        var removedTabs = 0;
        var runStart = 0;
        while (runStart < itemRanges.Count)
        {
            var kind = itemRanges[runStart].Kind;
            var runEnd = runStart + 1;
            while (runEnd < itemRanges.Count && itemRanges[runEnd].Kind == kind)
            {
                runEnd++;
            }

            var start = startIndex + itemRanges[runStart].StartOffset - removedTabs;
            var end = startIndex + itemRanges[runEnd - 1].EndOffset - removedTabs;
            operations.Add(new DocumentOperation(
                DocumentOperationKind.CreateBullet,
                start,
                end,
                listKind: kind));

            for (var itemIndex = runStart; itemIndex < runEnd; itemIndex++)
            {
                removedTabs += itemRanges[itemIndex].Depth;
            }

            runStart = runEnd;
        }

        return startIndex + text.Length - removedTabs;
    }

    private sealed record ItemRange(int StartOffset, int EndOffset, int Depth, ListKind Kind);
}
