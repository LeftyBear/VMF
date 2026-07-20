using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Renders fenced code blocks into target-neutral document operations.</summary>
public sealed class CodeBlockRenderer
{
    /// <summary>Appends operations for one fenced code block.</summary>
    public int Render(
        CodeBlock block,
        int startIndex,
        ICollection<DocumentOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(block);
        ArgumentNullException.ThrowIfNull(operations);
        if (startIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        var endIndex = startIndex + block.Text.Length + 1;
        operations.Add(new DocumentOperation(
            DocumentOperationKind.InsertText,
            startIndex,
            text: block.Text + "\n"));
        operations.Add(new DocumentOperation(
            DocumentOperationKind.ApplyCodeBlockStyle,
            startIndex,
            endIndex));
        if (block.Text.Length > 0)
        {
            operations.Add(new DocumentOperation(
                DocumentOperationKind.UpdateTextStyle,
                startIndex,
                endIndex - 1,
                inlineStyle: InlineTextStyle.Code));
        }

        return endIndex;
    }
}
