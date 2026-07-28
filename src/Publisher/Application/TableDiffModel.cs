using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

internal enum TableDiffKind
{
    NoChange,
    CellUpdate,
    AlignmentUpdate,
    InsertRow,
    DeleteRow,
    Move,
    MoveAndUpdate,
    Rebuild,
}

internal enum TableDiffSafety
{
    Safe,
    RebuildRequired,
    RecoverableMismatch,
    UnsafeMismatch,
}

internal sealed class CanonicalTableBlock
{
    private CanonicalTableBlock(
        IReadOnlyList<TableAlignment> alignments,
        CanonicalTableRow header,
        IReadOnlyList<CanonicalTableRow> body,
        string hash)
    {
        Alignments = alignments;
        Header = header;
        Body = body;
        Hash = hash;
    }

    internal IReadOnlyList<TableAlignment> Alignments { get; }

    internal CanonicalTableRow Header { get; }

    internal IReadOnlyList<CanonicalTableRow> Body { get; }

    internal string Hash { get; }

    internal int ColumnCount => Alignments.Count;

    internal IReadOnlyList<CanonicalTableRow> AllRows => [Header, .. Body];

    internal static CanonicalTableBlock Create(TableBlock table)
    {
        ArgumentNullException.ThrowIfNull(table);
        Validate(table);
        var alignments = table.Columns.Select(column => column.Alignment).ToArray();
        var header = CanonicalTableRow.Create(table.Header, isHeader: true, rowIndex: 0);
        var body = table.Rows
            .Select((row, index) => CanonicalTableRow.Create(row, isHeader: false, index + 1))
            .ToArray();
        return new CanonicalTableBlock(
            alignments,
            header,
            body,
            HashParts(
                "table",
                alignments.Select(item => item.ToString()).Concat([header.Hash]).Concat(body.Select(row => row.Hash))));
    }

    private static void Validate(TableBlock table)
    {
        if (table.Columns.Count == 0 ||
            table.Header.Cells.Count != table.Columns.Count ||
            table.Rows.Any(row => row.Cells.Count != table.Columns.Count))
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.PhysicalPlanInvalid,
                "The table canonical model is structurally invalid.");
        }
    }

    internal static string HashParts(string scope, IEnumerable<string> parts)
    {
        var payload = string.Join("\n", [scope, .. parts]);
        return "table-v1:sha256:" + Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
    }
}

internal sealed class CanonicalTableRow
{
    private CanonicalTableRow(
        bool isHeader,
        int rowIndex,
        IReadOnlyList<CanonicalTableCell> cells,
        string stableKey,
        string hash)
    {
        IsHeader = isHeader;
        RowIndex = rowIndex;
        Cells = cells;
        StableKey = stableKey;
        Hash = hash;
    }

    internal bool IsHeader { get; }

    internal int RowIndex { get; }

    internal IReadOnlyList<CanonicalTableCell> Cells { get; }

    internal string StableKey { get; }

    internal string Hash { get; }

    internal static CanonicalTableRow Create(TableRow row, bool isHeader, int rowIndex)
    {
        var cells = row.Cells.Select((cell, index) => CanonicalTableCell.Create(cell, rowIndex, index)).ToArray();
        var hash = CanonicalTableBlock.HashParts("row", cells.Select(cell => cell.Hash));
        return new CanonicalTableRow(
            isHeader,
            rowIndex,
            cells,
            isHeader ? "header" : StableRowKey(cells),
            hash);
    }

    private static string StableRowKey(IReadOnlyList<CanonicalTableCell> cells)
    {
        var firstText = cells[0].Text;
        return string.IsNullOrEmpty(firstText)
            ? CanonicalTableBlock.HashParts("row-key", cells.Select(cell => cell.Hash))
            : CanonicalTableBlock.HashParts("row-key", [firstText]);
    }
}

internal sealed class CanonicalTableCell
{
    private CanonicalTableCell(
        int rowIndex,
        int columnIndex,
        string text,
        IReadOnlyList<InlineStyleRange> styleRanges,
        string hash)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        Text = text;
        StyleRanges = styleRanges;
        Hash = hash;
    }

    internal int RowIndex { get; }

    internal int ColumnIndex { get; }

    internal string Text { get; }

    internal IReadOnlyList<InlineStyleRange> StyleRanges { get; }

    internal string Hash { get; }

    internal string CellIdentity => $"{RowIndex}:{ColumnIndex}";

    internal static CanonicalTableCell Create(TableCell cell, int rowIndex, int columnIndex)
    {
        var rendered = CanonicalInlineText.Render(cell.Content);
        return new CanonicalTableCell(
            rowIndex,
            columnIndex,
            rendered.Text,
            rendered.StyleRanges,
            CanonicalTableBlock.HashParts(
                "cell",
                [columnIndex.ToString(System.Globalization.CultureInfo.InvariantCulture), rendered.Text, .. rendered.StyleRanges.Select(StyleSignature)]));
    }

    private static string StyleSignature(InlineStyleRange range) => string.Join(
        ":",
        range.StartOffset,
        range.EndOffset,
        range.Style,
        range.Url?.AbsoluteUri ?? string.Empty);
}

internal sealed class TableDiffOperation
{
    internal TableDiffOperation(
        TableDiffKind kind,
        int? previousRowIndex,
        int? currentRowIndex,
        int? columnIndex,
        string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        Kind = kind;
        PreviousRowIndex = previousRowIndex;
        CurrentRowIndex = currentRowIndex;
        ColumnIndex = columnIndex;
        Reason = reason;
    }

    internal TableDiffKind Kind { get; }

    internal int? PreviousRowIndex { get; }

    internal int? CurrentRowIndex { get; }

    internal int? ColumnIndex { get; }

    internal string Reason { get; }
}

internal sealed class TableDiffPlan
{
    internal TableDiffPlan(
        TableDiffSafety safety,
        string previousHash,
        string currentHash,
        IEnumerable<TableDiffOperation> operations)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(previousHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(currentHash);
        Safety = safety;
        PreviousHash = previousHash;
        CurrentHash = currentHash;
        Operations = Array.AsReadOnly(operations.ToArray());
    }

    internal TableDiffSafety Safety { get; }

    internal string PreviousHash { get; }

    internal string CurrentHash { get; }

    internal IReadOnlyList<TableDiffOperation> Operations { get; }

    internal bool RequiresRebuild => Operations.Any(operation => operation.Kind == TableDiffKind.Rebuild);
}

internal sealed class TablePhysicalUpdatePlan
{
    internal TablePhysicalUpdatePlan(IEnumerable<TablePhysicalOperation> operations)
    {
        Operations = Array.AsReadOnly(operations.ToArray());
    }

    internal IReadOnlyList<TablePhysicalOperation> Operations { get; }
}

internal sealed record TablePhysicalOperation(
    TableDiffKind Kind,
    int? PreviousRowIndex,
    int? CurrentRowIndex,
    int? ColumnIndex);
