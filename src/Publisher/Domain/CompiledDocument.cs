namespace Vmf.Publisher.Domain;

/// <summary>Represents a document compiled into target-neutral operations.</summary>
public sealed class CompiledDocument
{
    /// <summary>Initializes a compiled document.</summary>
    /// <param name="title">The destination document title.</param>
    /// <param name="operations">The ordered document operations.</param>
    public CompiledDocument(string title, IEnumerable<DocumentOperation> operations)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        ArgumentNullException.ThrowIfNull(operations);
        var operationItems = operations.ToArray();
        Operations = Array.AsReadOnly(operationItems);
        Steps = operationItems.Length == 0
            ? Array.Empty<PublishStep>()
            : Array.AsReadOnly<PublishStep>(
                [new BatchUpdateStep(operationItems, InferContentLength(operationItems))]);
    }

    private CompiledDocument(string title, IEnumerable<PublishStep> steps)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        ArgumentNullException.ThrowIfNull(steps);
        var stepItems = steps.ToArray();
        Steps = Array.AsReadOnly(stepItems);
        Operations = Array.AsReadOnly(stepItems
            .OfType<BatchUpdateStep>()
            .SelectMany(step => step.Operations)
            .ToArray());
    }

    /// <summary>Gets the destination document title.</summary>
    public string Title { get; }

    /// <summary>Gets the ordered document operations.</summary>
    public IReadOnlyList<DocumentOperation> Operations { get; }

    /// <summary>Gets the ordered publish plan.</summary>
    public IReadOnlyList<PublishStep> Steps { get; }

    /// <summary>Creates a compiled document from an ordered publish plan.</summary>
    /// <param name="title">The destination document title.</param>
    /// <param name="steps">The ordered publish steps.</param>
    /// <returns>The compiled document.</returns>
    public static CompiledDocument FromSteps(string title, IEnumerable<PublishStep> steps) =>
        new(title, steps);

    private static int InferContentLength(IEnumerable<DocumentOperation> operations) =>
        operations
            .Where(operation => operation.Kind == DocumentOperationKind.InsertText)
            .Select(operation => operation.StartIndex - 1 + (operation.Text?.Length ?? 0))
            .DefaultIfEmpty(0)
            .Max();
}
