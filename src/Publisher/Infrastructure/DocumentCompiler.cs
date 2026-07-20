using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Compiles supported document blocks into Google-Docs-compatible neutral operations.</summary>
public sealed class DocumentCompiler : IDocumentCompiler
{
    private readonly IGeneratedBlockRenderer generatedBlockRenderer;

    /// <summary>Initializes a compiler with the default block renderers.</summary>
    public DocumentCompiler()
        : this(new InlineContentRenderer())
    {
    }

    private DocumentCompiler(InlineContentRenderer inlineRenderer)
        : this(
            new GeneratedBlockRenderer(
                new ParagraphBlockRenderer(inlineRenderer),
                new HeadingBlockRenderer(inlineRenderer),
                new ListBlockRenderer(inlineRenderer)))
    {
    }

    /// <summary>Initializes a compiler with an explicitly registered list renderer.</summary>
    /// <param name="listBlockRenderer">The list block renderer.</param>
    public DocumentCompiler(ListBlockRenderer listBlockRenderer)
        : this(
            new GeneratedBlockRenderer(
                new ParagraphBlockRenderer(),
                new HeadingBlockRenderer(),
                listBlockRenderer))
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
        : this(new GeneratedBlockRenderer(
            paragraphBlockRenderer,
            headingBlockRenderer,
            listBlockRenderer))
    {
    }

    /// <summary>Initializes a compiler with a generated-block renderer.</summary>
    /// <param name="generatedBlockRenderer">The renderer that produces publish steps.</param>
    public DocumentCompiler(IGeneratedBlockRenderer generatedBlockRenderer)
    {
        this.generatedBlockRenderer = generatedBlockRenderer
            ?? throw new ArgumentNullException(nameof(generatedBlockRenderer));
    }

    /// <inheritdoc />
    public CompiledDocument Compile(DocumentModel document, string title)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return CompiledDocument.FromSteps(title, generatedBlockRenderer.Render(document));
    }
}
