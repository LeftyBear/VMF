using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Builds batches for ordinary blocks and isolated steps for tables.</summary>
public sealed class GeneratedBlockRenderer : IGeneratedBlockRenderer
{
    private readonly ParagraphBlockRenderer paragraphRenderer;
    private readonly HeadingBlockRenderer headingRenderer;
    private readonly ListBlockRenderer listRenderer;
    private readonly CodeBlockRenderer codeRenderer;
    private readonly QuoteBlockRenderer quoteRenderer;

    /// <summary>Initializes a renderer with default block renderers.</summary>
    public GeneratedBlockRenderer()
        : this(
            new ParagraphBlockRenderer(),
            new HeadingBlockRenderer(),
            new ListBlockRenderer(),
            new CodeBlockRenderer(),
            new QuoteBlockRenderer())
    {
    }

    /// <summary>Initializes a renderer with registered block renderers.</summary>
    /// <param name="paragraphRenderer">The paragraph renderer.</param>
    /// <param name="headingRenderer">The heading renderer.</param>
    /// <param name="listRenderer">The list renderer.</param>
    public GeneratedBlockRenderer(
        ParagraphBlockRenderer paragraphRenderer,
        HeadingBlockRenderer headingRenderer,
        ListBlockRenderer listRenderer)
        : this(paragraphRenderer, headingRenderer, listRenderer, new CodeBlockRenderer())
    {
    }

    /// <summary>Initializes a renderer with registered block renderers.</summary>
    /// <param name="paragraphRenderer">The paragraph renderer.</param>
    /// <param name="headingRenderer">The heading renderer.</param>
    /// <param name="listRenderer">The list renderer.</param>
    /// <param name="codeRenderer">The fenced-code renderer.</param>
    public GeneratedBlockRenderer(
        ParagraphBlockRenderer paragraphRenderer,
        HeadingBlockRenderer headingRenderer,
        ListBlockRenderer listRenderer,
        CodeBlockRenderer codeRenderer)
        : this(
            paragraphRenderer,
            headingRenderer,
            listRenderer,
            codeRenderer,
            new QuoteBlockRenderer())
    {
    }

    /// <summary>Initializes a renderer with registered block renderers.</summary>
    /// <param name="paragraphRenderer">The paragraph renderer.</param>
    /// <param name="headingRenderer">The heading renderer.</param>
    /// <param name="listRenderer">The list renderer.</param>
    /// <param name="codeRenderer">The fenced-code renderer.</param>
    /// <param name="quoteRenderer">The block-quote renderer.</param>
    public GeneratedBlockRenderer(
        ParagraphBlockRenderer paragraphRenderer,
        HeadingBlockRenderer headingRenderer,
        ListBlockRenderer listRenderer,
        CodeBlockRenderer codeRenderer,
        QuoteBlockRenderer quoteRenderer)
    {
        this.paragraphRenderer = paragraphRenderer
            ?? throw new ArgumentNullException(nameof(paragraphRenderer));
        this.headingRenderer = headingRenderer
            ?? throw new ArgumentNullException(nameof(headingRenderer));
        this.listRenderer = listRenderer ?? throw new ArgumentNullException(nameof(listRenderer));
        this.codeRenderer = codeRenderer ?? throw new ArgumentNullException(nameof(codeRenderer));
        this.quoteRenderer = quoteRenderer ?? throw new ArgumentNullException(nameof(quoteRenderer));
    }

    /// <inheritdoc />
    public IReadOnlyList<PublishStep> Render(DocumentModel document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var steps = new List<PublishStep>();
        var operations = new List<DocumentOperation>();
        var index = 1;

        void FlushBatch()
        {
            if (operations.Count == 0)
            {
                return;
            }

            steps.Add(new BatchUpdateStep(operations, index - 1));
            operations.Clear();
            index = 1;
        }

        foreach (var block in document.Blocks)
        {
            if (block.Kind == DocumentBlockKind.Image)
            {
                FlushBatch();
                steps.Add(new InsertImageStep(
                    block.Image ?? throw new InvalidOperationException("An image block requires image content.")));
                continue;
            }

            if (block.Kind == DocumentBlockKind.Table)
            {
                FlushBatch();
                steps.Add(new InsertTableStep(
                    block.Table ?? throw new InvalidOperationException("A table block requires table content.")));
                continue;
            }

            if (block.Kind == DocumentBlockKind.List)
            {
                index = listRenderer.Render(
                    block.List ?? throw new InvalidOperationException("A list block requires list content."),
                    index,
                    operations);
                continue;
            }

            if (block.Kind == DocumentBlockKind.Code)
            {
                index = codeRenderer.Render(
                    block.Code ?? throw new InvalidOperationException("A code block requires code content."),
                    index,
                    operations);
                continue;
            }

            if (block.Kind == DocumentBlockKind.Quote)
            {
                index = quoteRenderer.Render(
                    block.Quote ?? throw new InvalidOperationException("A quote block requires quote content."),
                    index,
                    operations);
                continue;
            }

            if (block.Kind == DocumentBlockKind.Heading)
            {
                index = headingRenderer.Render(
                    new HeadingBlock(block.Level, block.Content),
                    index,
                    operations);
                continue;
            }

            var start = index;
            index = paragraphRenderer.Render(
                new ParagraphBlock(block.Content),
                index,
                operations);
            if (block.Kind == DocumentBlockKind.BulletListItem)
            {
                operations.Add(new DocumentOperation(
                    DocumentOperationKind.CreateBullet,
                    start,
                    index));
            }
        }

        FlushBatch();
        return Array.AsReadOnly(steps.ToArray());
    }
}
