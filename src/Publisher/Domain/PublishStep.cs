namespace Vmf.Publisher.Domain;

/// <summary>Represents one ordered step in a publish plan.</summary>
public abstract class PublishStep
{
    private protected PublishStep()
    {
    }
}

/// <summary>Applies a contiguous batch of ordinary document operations.</summary>
public sealed class BatchUpdateStep : PublishStep
{
    /// <summary>Initializes a batch-update step.</summary>
    /// <param name="operations">Operations whose indexes are relative to document index one.</param>
    /// <param name="contentLength">The resulting UTF-16 content length.</param>
    public BatchUpdateStep(IEnumerable<DocumentOperation> operations, int contentLength)
    {
        ArgumentNullException.ThrowIfNull(operations);
        ArgumentOutOfRangeException.ThrowIfNegative(contentLength);
        var items = operations.ToArray();
        if (items.Length == 0)
        {
            throw new ArgumentException("A batch-update step requires at least one operation.", nameof(operations));
        }

        Operations = Array.AsReadOnly(items);
        ContentLength = contentLength;
    }

    /// <summary>Gets operations whose indexes are relative to document index one.</summary>
    public IReadOnlyList<DocumentOperation> Operations { get; }

    /// <summary>Gets the resulting UTF-16 content length.</summary>
    public int ContentLength { get; }
}

/// <summary>Inserts and populates a table through the two-stage Google Docs flow.</summary>
public sealed class InsertTableStep : PublishStep
{
    /// <summary>Initializes a table insertion step.</summary>
    /// <param name="table">The table to insert.</param>
    public InsertTableStep(TableBlock table)
    {
        Table = table ?? throw new ArgumentNullException(nameof(table));
    }

    /// <summary>Gets the table to insert.</summary>
    public TableBlock Table { get; }
}

/// <summary>Inserts one standalone image and reads its resulting structure.</summary>
public sealed class InsertImageStep : PublishStep
{
    /// <summary>Initializes an image insertion step.</summary>
    public InsertImageStep(ImageBlock image)
    {
        Image = image ?? throw new ArgumentNullException(nameof(image));
        if (image.Size is null)
        {
            throw new ArgumentException("An image publish step requires a calculated size.", nameof(image));
        }
    }

    /// <summary>Gets the prepared image.</summary>
    public ImageBlock Image { get; }
}
