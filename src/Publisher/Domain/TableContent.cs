namespace Vmf.Publisher.Domain;

/// <summary>Identifies the horizontal alignment of a table column.</summary>
public enum TableAlignment
{
    /// <summary>Left-aligned content.</summary>
    Left,

    /// <summary>Centered content.</summary>
    Center,

    /// <summary>Right-aligned content.</summary>
    Right,
}

/// <summary>Describes one table column.</summary>
public sealed class TableColumn
{
    /// <summary>Initializes a table column.</summary>
    /// <param name="alignment">The column alignment.</param>
    public TableColumn(TableAlignment alignment)
    {
        Alignment = alignment;
    }

    /// <summary>Gets the column alignment.</summary>
    public TableAlignment Alignment { get; }
}

/// <summary>Represents one table cell.</summary>
public sealed class TableCell
{
    /// <summary>Initializes a table cell.</summary>
    /// <param name="content">The inline content; an empty sequence represents an empty cell.</param>
    public TableCell(IEnumerable<InlineContent> content)
    {
        ArgumentNullException.ThrowIfNull(content);
        var items = content.ToArray();
        if (items.Any(item => item is null))
        {
            throw new ArgumentException("Table cell content must not contain null items.", nameof(content));
        }

        Content = Array.AsReadOnly(items);
    }

    /// <summary>Gets the inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }

    /// <summary>Creates an empty table cell.</summary>
    /// <returns>An empty cell.</returns>
    public static TableCell Empty() => new(Array.Empty<InlineContent>());
}

/// <summary>Represents one table row.</summary>
public sealed class TableRow
{
    /// <summary>Initializes a table row.</summary>
    /// <param name="cells">The cells in column order.</param>
    public TableRow(IEnumerable<TableCell> cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        var items = cells.ToArray();
        if (items.Length == 0 || items.Any(item => item is null))
        {
            throw new ArgumentException("A table row requires non-null cells.", nameof(cells));
        }

        Cells = Array.AsReadOnly(items);
    }

    /// <summary>Gets the cells in column order.</summary>
    public IReadOnlyList<TableCell> Cells { get; }
}

/// <summary>Represents a Markdown table with a required header row.</summary>
public sealed class TableBlock
{
    /// <summary>Initializes a table block.</summary>
    /// <param name="columns">The table columns.</param>
    /// <param name="header">The required header row.</param>
    /// <param name="rows">The body rows.</param>
    public TableBlock(
        IEnumerable<TableColumn> columns,
        TableRow header,
        IEnumerable<TableRow> rows)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(rows);

        var columnItems = columns.ToArray();
        var rowItems = rows.ToArray();
        if (columnItems.Length == 0 || columnItems.Any(item => item is null))
        {
            throw new ArgumentException("A table requires non-null columns.", nameof(columns));
        }

        if (header.Cells.Count != columnItems.Length ||
            rowItems.Any(row => row is null || row.Cells.Count != columnItems.Length))
        {
            throw new ArgumentException("Every table row must match the table column count.", nameof(rows));
        }

        Columns = Array.AsReadOnly(columnItems);
        Header = header;
        Rows = Array.AsReadOnly(rowItems);
    }

    /// <summary>Gets the table columns.</summary>
    public IReadOnlyList<TableColumn> Columns { get; }

    /// <summary>Gets the header row.</summary>
    public TableRow Header { get; }

    /// <summary>Gets the body rows.</summary>
    public IReadOnlyList<TableRow> Rows { get; }

    /// <summary>Gets all rows with the header first.</summary>
    public IReadOnlyList<TableRow> AllRows => Array.AsReadOnly(new[] { Header }.Concat(Rows).ToArray());
}
