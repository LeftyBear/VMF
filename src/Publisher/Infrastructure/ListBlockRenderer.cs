using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Renders list blocks into target-neutral document operations.</summary>
public sealed class ListBlockRenderer
{
    private readonly InlineContentRenderer inlineRenderer;

    /// <summary>Initializes a renderer with the default inline renderer.</summary>
    public ListBlockRenderer()
        : this(new InlineContentRenderer())
    {
    }

    /// <summary>Initializes a renderer with an explicitly registered inline renderer.</summary>
    /// <param name="inlineRenderer">The inline content renderer.</param>
    public ListBlockRenderer(InlineContentRenderer inlineRenderer)
    {
        this.inlineRenderer = inlineRenderer ?? throw new ArgumentNullException(nameof(inlineRenderer));
    }

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
        var renderedItems = new List<RenderedItem>(block.Items.Count);
        var finalOffset = 0;
        foreach (var item in block.Items)
        {
            var offset = text.Length;
            var rendered = inlineRenderer.Render(item.Content);
            text.Append('\t', item.Depth);
            text.Append(rendered.Text);
            text.Append('\n');
            itemRanges.Add(new ItemRange(offset, text.Length, item.Depth, item.Kind));
            renderedItems.Add(new RenderedItem(finalOffset, rendered));
            finalOffset += rendered.Text.Length + 1;
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

        // Bullet creation removes the leading depth tabs. Style ranges are therefore
        // emitted against the final, tab-free positions after every bullet request.
        foreach (var item in renderedItems)
        {
            InlineStyleOperationAppender.Append(
                item.Content,
                startIndex + item.FinalOffset,
                operations);
        }

        return startIndex + text.Length - removedTabs;
    }

    private sealed record ItemRange(int StartOffset, int EndOffset, int Depth, ListKind Kind);

    private sealed record RenderedItem(int FinalOffset, RenderedInlineContent Content);
}
