using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Renders block quotes into target-neutral document operations.</summary>
public sealed class QuoteBlockRenderer
{
    private readonly InlineContentRenderer inlineRenderer;

    /// <summary>Initializes a renderer with the default inline renderer.</summary>
    public QuoteBlockRenderer()
        : this(new InlineContentRenderer())
    {
    }

    /// <summary>Initializes a renderer with an explicitly registered inline renderer.</summary>
    public QuoteBlockRenderer(InlineContentRenderer inlineRenderer)
    {
        this.inlineRenderer = inlineRenderer ?? throw new ArgumentNullException(nameof(inlineRenderer));
    }

    /// <summary>Appends operations for one quote block.</summary>
    public int Render(
        QuoteBlock block,
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
            DocumentOperationKind.ApplyQuoteBlockStyle,
            startIndex,
            endIndex,
            level: block.Level));
        if (rendered.Text.Length > 0)
        {
            operations.Add(new DocumentOperation(
                DocumentOperationKind.UpdateTextStyle,
                startIndex,
                endIndex - 1,
                inlineStyle: InlineTextStyle.Italic));
        }

        InlineStyleOperationAppender.Append(rendered, startIndex, operations);
        return endIndex;
    }
}
