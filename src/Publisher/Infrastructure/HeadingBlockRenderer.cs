using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Renders heading blocks into target-neutral document operations.</summary>
public sealed class HeadingBlockRenderer
{
    private readonly InlineContentRenderer inlineRenderer;

    /// <summary>Initializes a renderer with the default inline renderer.</summary>
    public HeadingBlockRenderer()
        : this(new InlineContentRenderer())
    {
    }

    /// <summary>Initializes a renderer with an explicitly registered inline renderer.</summary>
    /// <param name="inlineRenderer">The inline content renderer.</param>
    public HeadingBlockRenderer(InlineContentRenderer inlineRenderer)
    {
        this.inlineRenderer = inlineRenderer ?? throw new ArgumentNullException(nameof(inlineRenderer));
    }

    /// <summary>Appends operations for one heading and returns the next document index.</summary>
    /// <param name="block">The heading block.</param>
    /// <param name="startIndex">The first Google Docs text index available to the block.</param>
    /// <param name="operations">The destination operation collection.</param>
    /// <returns>The first text index after the rendered heading.</returns>
    public int Render(
        HeadingBlock block,
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
        var endIndex = startIndex + rendered.Text.Length + 1;
        operations.Add(new DocumentOperation(
            DocumentOperationKind.InsertText,
            startIndex,
            text: rendered.Text + "\n"));
        operations.Add(new DocumentOperation(
            DocumentOperationKind.ApplyHeading,
            startIndex,
            endIndex,
            level: block.Level));
        InlineStyleOperationAppender.Append(rendered, startIndex, operations);
        return endIndex;
    }
}
