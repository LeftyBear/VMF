using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Renders paragraph blocks into target-neutral document operations.</summary>
public sealed class ParagraphBlockRenderer
{
    private readonly InlineContentRenderer inlineRenderer;

    /// <summary>Initializes a renderer with the default inline renderer.</summary>
    public ParagraphBlockRenderer()
        : this(new InlineContentRenderer())
    {
    }

    /// <summary>Initializes a renderer with an explicitly registered inline renderer.</summary>
    /// <param name="inlineRenderer">The inline content renderer.</param>
    public ParagraphBlockRenderer(InlineContentRenderer inlineRenderer)
    {
        this.inlineRenderer = inlineRenderer ?? throw new ArgumentNullException(nameof(inlineRenderer));
    }

    /// <summary>Appends operations for one paragraph and returns the next document index.</summary>
    /// <param name="block">The paragraph block.</param>
    /// <param name="startIndex">The first Google Docs text index available to the block.</param>
    /// <param name="operations">The destination operation collection.</param>
    /// <returns>The first text index after the rendered paragraph.</returns>
    public int Render(
        ParagraphBlock block,
        int startIndex,
        ICollection<DocumentOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(operations);
        if (startIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        var rendered = inlineRenderer.Render(block.Content);
        operations.Add(new DocumentOperation(
            DocumentOperationKind.InsertText,
            startIndex,
            text: rendered.Text + "\n"));
        InlineStyleOperationAppender.Append(rendered, startIndex, operations);
        return startIndex + rendered.Text.Length + 1;
    }
}
