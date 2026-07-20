using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Compiles supported document blocks into Google-Docs-compatible neutral operations.</summary>
public sealed class DocumentCompiler : IDocumentCompiler
{
    private readonly ListBlockRenderer _listBlockRenderer;

    /// <summary>Initializes a compiler with the default block renderers.</summary>
    public DocumentCompiler()
        : this(new ListBlockRenderer())
    {
    }

    /// <summary>Initializes a compiler with an explicitly registered list renderer.</summary>
    /// <param name="listBlockRenderer">The list block renderer.</param>
    public DocumentCompiler(ListBlockRenderer listBlockRenderer)
    {
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

            var text = string.Concat(block.Inlines.Select(element => element.Text)) + "\n";
            var start = index;
            var end = start + text.Length;
            operations.Add(new DocumentOperation(DocumentOperationKind.InsertText, start, text: text));

            if (block.Kind == DocumentBlockKind.Heading)
            {
                operations.Add(new DocumentOperation(
                    DocumentOperationKind.ApplyHeading,
                    start,
                    end,
                    level: block.Level));
            }
            else if (block.Kind == DocumentBlockKind.BulletListItem)
            {
                operations.Add(new DocumentOperation(DocumentOperationKind.CreateBullet, start, end));
            }

            index = end;
        }

        return new CompiledDocument(title, operations);
    }
}
