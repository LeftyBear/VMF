using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

internal sealed class TableDiffEngine
{
    internal TableDiffPlan CreatePlan(TableBlock previous, TableBlock current)
    {
        var baseline = CanonicalTableBlock.Create(previous);
        var candidate = CanonicalTableBlock.Create(current);
        if (string.Equals(baseline.Hash, candidate.Hash, StringComparison.Ordinal))
        {
            return new TableDiffPlan(
                TableDiffSafety.Safe,
                baseline.Hash,
                candidate.Hash,
                [new TableDiffOperation(TableDiffKind.NoChange, null, null, null, "table hash match")]);
        }

        var rebuildReason = FindStructuralRebuildReason(baseline, candidate);
        if (rebuildReason is not null)
        {
            return Rebuild(baseline, candidate, rebuildReason);
        }

        var operations = new List<TableDiffOperation>();
        AddHeaderOperations(baseline, candidate, operations);
        AddAlignmentOperations(baseline, candidate, operations);

        if (!TryMatchRows(baseline, candidate, out var matches, out var ambiguousReason))
        {
            return Rebuild(baseline, candidate, ambiguousReason);
        }

        AddDeleteOperations(baseline, matches, operations);
        AddInsertOperations(candidate, matches, operations);
        AddMatchedRowOperations(baseline, candidate, matches, operations);

        return new TableDiffPlan(
            TableDiffSafety.Safe,
            baseline.Hash,
            candidate.Hash,
            operations.Count == 0
                ? [new TableDiffOperation(TableDiffKind.NoChange, null, null, null, "table content match")]
                : operations.OrderBy(Signature).ToArray());
    }

    internal TablePhysicalUpdatePlan CreatePhysicalPlan(TableDiffPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (plan.RequiresRebuild)
        {
            return new TablePhysicalUpdatePlan([
                new TablePhysicalOperation(TableDiffKind.Rebuild, null, null, null),
            ]);
        }

        var deletes = plan.Operations
            .Where(operation => operation.Kind == TableDiffKind.DeleteRow)
            .OrderByDescending(operation => operation.PreviousRowIndex)
            .Select(operation => new TablePhysicalOperation(
                operation.Kind,
                operation.PreviousRowIndex,
                operation.CurrentRowIndex,
                operation.ColumnIndex));
        var inserts = plan.Operations
            .Where(operation => operation.Kind == TableDiffKind.InsertRow)
            .OrderBy(operation => operation.CurrentRowIndex)
            .Select(operation => new TablePhysicalOperation(
                operation.Kind,
                operation.PreviousRowIndex,
                operation.CurrentRowIndex,
                operation.ColumnIndex));
        var moves = plan.Operations
            .Where(operation => operation.Kind is TableDiffKind.Move or TableDiffKind.MoveAndUpdate)
            .OrderByDescending(operation => operation.PreviousRowIndex)
            .SelectMany(operation => new[]
            {
                new TablePhysicalOperation(TableDiffKind.DeleteRow, operation.PreviousRowIndex, null, null),
                new TablePhysicalOperation(TableDiffKind.InsertRow, null, operation.CurrentRowIndex, null),
            });
        var updates = plan.Operations
            .Where(operation => operation.Kind is TableDiffKind.CellUpdate or TableDiffKind.AlignmentUpdate)
            .OrderBy(operation => operation.CurrentRowIndex)
            .ThenBy(operation => operation.ColumnIndex)
            .Select(operation => new TablePhysicalOperation(
                operation.Kind,
                operation.PreviousRowIndex,
                operation.CurrentRowIndex,
                operation.ColumnIndex));

        return new TablePhysicalUpdatePlan([.. deletes, .. inserts, .. moves, .. updates]);
    }

    internal TableDiffSafety Verify(
        TableBlock previous,
        TableBlock current,
        TableBlock readback)
    {
        var baseline = CanonicalTableBlock.Create(previous);
        var candidate = CanonicalTableBlock.Create(current);
        var actual = CanonicalTableBlock.Create(readback);
        if (string.Equals(actual.Hash, candidate.Hash, StringComparison.Ordinal))
        {
            return TableDiffSafety.Safe;
        }

        if (string.Equals(actual.Hash, baseline.Hash, StringComparison.Ordinal))
        {
            return TableDiffSafety.RecoverableMismatch;
        }

        return SameStructure(candidate, actual)
            ? TableDiffSafety.RecoverableMismatch
            : TableDiffSafety.UnsafeMismatch;
    }

    private static string? FindStructuralRebuildReason(
        CanonicalTableBlock baseline,
        CanonicalTableBlock candidate)
    {
        if (baseline.ColumnCount != candidate.ColumnCount)
        {
            return "column count changed";
        }

        if (!baseline.Header.Cells.Select(cell => cell.Text)
                .SequenceEqual(candidate.Header.Cells.Select(cell => cell.Text), StringComparer.Ordinal))
        {
            return "header identity changed";
        }

        return null;
    }

    private static void AddHeaderOperations(
        CanonicalTableBlock baseline,
        CanonicalTableBlock candidate,
        ICollection<TableDiffOperation> operations)
    {
        for (var column = 0; column < baseline.ColumnCount; column++)
        {
            if (!string.Equals(
                    baseline.Header.Cells[column].Hash,
                    candidate.Header.Cells[column].Hash,
                    StringComparison.Ordinal))
            {
                operations.Add(new TableDiffOperation(
                    TableDiffKind.CellUpdate,
                    0,
                    0,
                    column,
                    "header rich content changed"));
            }
        }
    }

    private static void AddAlignmentOperations(
        CanonicalTableBlock baseline,
        CanonicalTableBlock candidate,
        ICollection<TableDiffOperation> operations)
    {
        for (var column = 0; column < baseline.ColumnCount; column++)
        {
            if (baseline.Alignments[column] != candidate.Alignments[column])
            {
                operations.Add(new TableDiffOperation(
                    TableDiffKind.AlignmentUpdate,
                    null,
                    null,
                    column,
                    "column alignment changed"));
            }
        }
    }

    private static bool TryMatchRows(
        CanonicalTableBlock baseline,
        CanonicalTableBlock candidate,
        out IReadOnlyDictionary<int, int> matches,
        out string reason)
    {
        var baselineGroups = baseline.Body
            .GroupBy(row => row.StableKey, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var candidateGroups = candidate.Body
            .GroupBy(row => row.StableKey, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);

        if (baselineGroups.Any(group => group.Value.Length > 1) ||
            candidateGroups.Any(group => group.Value.Length > 1))
        {
            matches = new Dictionary<int, int>();
            reason = "duplicate row identity";
            return false;
        }

        var result = new SortedDictionary<int, int>();
        foreach (var item in baselineGroups)
        {
            if (candidateGroups.TryGetValue(item.Key, out var target))
            {
                result.Add(item.Value[0].RowIndex, target[0].RowIndex);
            }
        }

        matches = result;
        reason = string.Empty;
        return true;
    }

    private static void AddDeleteOperations(
        CanonicalTableBlock baseline,
        IReadOnlyDictionary<int, int> matches,
        ICollection<TableDiffOperation> operations)
    {
        foreach (var row in baseline.Body.Where(row => !matches.ContainsKey(row.RowIndex)))
        {
            operations.Add(new TableDiffOperation(
                TableDiffKind.DeleteRow,
                row.RowIndex,
                null,
                null,
                "row missing from candidate"));
        }
    }

    private static void AddInsertOperations(
        CanonicalTableBlock candidate,
        IReadOnlyDictionary<int, int> matches,
        ICollection<TableDiffOperation> operations)
    {
        var matchedCurrent = matches.Values.ToHashSet();
        foreach (var row in candidate.Body.Where(row => !matchedCurrent.Contains(row.RowIndex)))
        {
            operations.Add(new TableDiffOperation(
                TableDiffKind.InsertRow,
                null,
                row.RowIndex,
                null,
                "row missing from baseline"));
        }
    }

    private static void AddMatchedRowOperations(
        CanonicalTableBlock baseline,
        CanonicalTableBlock candidate,
        IReadOnlyDictionary<int, int> matches,
        ICollection<TableDiffOperation> operations)
    {
        foreach (var pair in matches)
        {
            var previous = baseline.AllRows[pair.Key];
            var current = candidate.AllRows[pair.Value];
            var moved = pair.Key != pair.Value;
            var updated = !string.Equals(previous.Hash, current.Hash, StringComparison.Ordinal);
            if (moved)
            {
                operations.Add(new TableDiffOperation(
                    updated ? TableDiffKind.MoveAndUpdate : TableDiffKind.Move,
                    pair.Key,
                    pair.Value,
                    null,
                    updated ? "row moved and changed" : "row moved"));
                continue;
            }

            if (!updated)
            {
                continue;
            }

            for (var column = 0; column < previous.Cells.Count; column++)
            {
                if (!string.Equals(previous.Cells[column].Hash, current.Cells[column].Hash, StringComparison.Ordinal))
                {
                    operations.Add(new TableDiffOperation(
                        TableDiffKind.CellUpdate,
                        pair.Key,
                        pair.Value,
                        column,
                        "cell rich content changed"));
                }
            }
        }
    }

    private static bool SameStructure(CanonicalTableBlock expected, CanonicalTableBlock actual) =>
        expected.ColumnCount == actual.ColumnCount &&
        expected.Body.Count == actual.Body.Count &&
        expected.Header.Cells.Count == actual.Header.Cells.Count &&
        expected.Body.Zip(actual.Body).All(pair => pair.First.Cells.Count == pair.Second.Cells.Count);

    private static TableDiffPlan Rebuild(
        CanonicalTableBlock baseline,
        CanonicalTableBlock candidate,
        string reason) => new(
        TableDiffSafety.RebuildRequired,
        baseline.Hash,
        candidate.Hash,
        [new TableDiffOperation(TableDiffKind.Rebuild, null, null, null, reason)]);

    private static string Signature(TableDiffOperation operation) => string.Join(
        ":",
        operation.Kind,
        operation.PreviousRowIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "-",
        operation.CurrentRowIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "-",
        operation.ColumnIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "-",
        operation.Reason);
}
