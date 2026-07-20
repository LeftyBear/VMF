using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Represents the table structure required from a Google document readback.</summary>
public sealed class GoogleDocumentSnapshot
{
    /// <summary>Initializes a document snapshot.</summary>
    /// <param name="tables">Tables in document order.</param>
    /// <param name="images">Inline images in document order.</param>
    public GoogleDocumentSnapshot(
        IEnumerable<GoogleTableSnapshot> tables,
        IEnumerable<GoogleInlineImageSnapshot>? images = null)
    {
        ArgumentNullException.ThrowIfNull(tables);
        Tables = Array.AsReadOnly(tables.ToArray());
        Images = Array.AsReadOnly((images ?? []).ToArray());
    }

    /// <summary>Gets tables in document order.</summary>
    public IReadOnlyList<GoogleTableSnapshot> Tables { get; }

    /// <summary>Gets standalone inline images in document order.</summary>
    public IReadOnlyList<GoogleInlineImageSnapshot> Images { get; }
}

/// <summary>Represents an inline object element and its containing paragraph.</summary>
public sealed class GoogleInlineImageSnapshot
{
    /// <summary>Initializes an inline-image snapshot.</summary>
    public GoogleInlineImageSnapshot(
        int? paragraphStartIndex,
        int? paragraphEndIndex,
        int? elementStartIndex,
        int? elementEndIndex,
        string? inlineObjectId,
        ImageSize? actualSize,
        string? title = null,
        string? description = null)
    {
        ParagraphStartIndex = paragraphStartIndex;
        ParagraphEndIndex = paragraphEndIndex;
        ElementStartIndex = elementStartIndex;
        ElementEndIndex = elementEndIndex;
        InlineObjectId = inlineObjectId;
        ActualSize = actualSize;
        Title = title;
        Description = description;
    }

    /// <summary>Gets the containing paragraph start index.</summary>
    public int? ParagraphStartIndex { get; }

    /// <summary>Gets the containing paragraph end index.</summary>
    public int? ParagraphEndIndex { get; }

    /// <summary>Gets the inline-object element start index.</summary>
    public int? ElementStartIndex { get; }

    /// <summary>Gets the inline-object element end index.</summary>
    public int? ElementEndIndex { get; }

    /// <summary>Gets the InlineObjectElement identifier.</summary>
    public string? InlineObjectId { get; }

    /// <summary>Gets the actual size returned by Google Docs.</summary>
    public ImageSize? ActualSize { get; }

    /// <summary>Gets the returned embedded-object title.</summary>
    public string? Title { get; }

    /// <summary>Gets the returned embedded-object description.</summary>
    public string? Description { get; }
}

/// <summary>Represents one Google Docs table.</summary>
public sealed class GoogleTableSnapshot
{
    /// <summary>Initializes a table snapshot.</summary>
    public GoogleTableSnapshot(int? startIndex, int? endIndex, IEnumerable<GoogleTableRowSnapshot> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        StartIndex = startIndex;
        EndIndex = endIndex;
        Rows = Array.AsReadOnly(rows.ToArray());
    }

    /// <summary>Gets the table start index.</summary>
    public int? StartIndex { get; }

    /// <summary>Gets the table end index.</summary>
    public int? EndIndex { get; }

    /// <summary>Gets the table rows.</summary>
    public IReadOnlyList<GoogleTableRowSnapshot> Rows { get; }
}

/// <summary>Represents one Google Docs table row.</summary>
public sealed class GoogleTableRowSnapshot
{
    /// <summary>Initializes a row snapshot.</summary>
    public GoogleTableRowSnapshot(IEnumerable<GoogleTableCellSnapshot> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        Cells = Array.AsReadOnly(cells.ToArray());
    }

    /// <summary>Gets the cells.</summary>
    public IReadOnlyList<GoogleTableCellSnapshot> Cells { get; }
}

/// <summary>Represents the editable range of one Google Docs table cell.</summary>
public sealed class GoogleTableCellSnapshot
{
    /// <summary>Initializes a cell snapshot.</summary>
    public GoogleTableCellSnapshot(int? startIndex, int? endIndex)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    /// <summary>Gets the cell content start index.</summary>
    public int? StartIndex { get; }

    /// <summary>Gets the cell content end index.</summary>
    public int? EndIndex { get; }
}
