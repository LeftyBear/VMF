using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Compiles supported document blocks into Google-Docs-compatible neutral operations.</summary>
public sealed class DocumentCompiler : IDocumentCompiler
{
    private readonly ParagraphBlockRenderer paragraphBlockRenderer;
    private readonly HeadingBlockRenderer headingBlockRenderer;
    private readonly ListBlockRenderer _listBlockRenderer;

    /// <summary>Initializes a compiler with the default block renderers.</summary>
    public DocumentCompiler()
        : this(new InlineContentRenderer())
    {
    }

    private DocumentCompiler(InlineContentRenderer inlineRenderer)
        : this(
            new ParagraphBlockRenderer(inlineRenderer),
            new HeadingBlockRenderer(inlineRenderer),
            new ListBlockRenderer(inlineRenderer))
    {
    }

    /// <summary>Initializes a compiler with an explicitly registered list renderer.</summary>
    /// <param name="listBlockRenderer">The list block renderer.</param>
    public DocumentCompiler(ListBlockRenderer listBlockRenderer)
        : this(
            new ParagraphBlockRenderer(),
            new HeadingBlockRenderer(),
            listBlockRenderer)
    {
    }

    /// <summary>Initializes a compiler with explicitly registered block renderers.</summary>
    /// <param name="paragraphBlockRenderer">The paragraph block renderer.</param>
    /// <param name="headingBlockRenderer">The heading block renderer.</param>
    /// <param name="listBlockRenderer">The list block renderer.</param>
    public DocumentCompiler(
        ParagraphBlockRenderer paragraphBlockRenderer,
        HeadingBlockRenderer headingBlockRenderer,
        ListBlockRenderer listBlockRenderer)
    {
        this.paragraphBlockRenderer = paragraphBlockRenderer
            ?? throw new ArgumentNullException(nameof(paragraphBlockRenderer));
        this.headingBlockRenderer = headingBlockRenderer
            ?? throw new ArgumentNullException(nameof(headingBlockRenderer));
        _listBlockRenderer = listBlockRenderer ?? throw new ArgumentNullException(nameof(listBlockRenderer));
    }

    /// <inheritdoc />
    public CompiledDocument Compile(DocumentModel document, string title)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        var operations = new List<DocumentOperation>();
        var index = 1;

        foreach (var block in document.Blocks)
        {
            if (block.Kind == DocumentBlockKind.List)
            {
                index = _listBlockRenderer.Render(
                    block.List ?? throw new InvalidOperationException("A list block requires list content."),
                    index,
                    operations);
                continue;
            }

            if (block.Kind == DocumentBlockKind.Heading)
            {
                index = headingBlockRenderer.Render(
                    new HeadingBlock(block.Level, block.Content),
                    index,
                    operations);
                continue;
            }

            var start = index;
            index = paragraphBlockRenderer.Render(
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

        return new CompiledDocument(title, operations);
    }
}
