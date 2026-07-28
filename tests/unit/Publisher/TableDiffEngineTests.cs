using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class TableDiffEngineTests
{
    private readonly TableDiffEngine engine = new();

    [Fact]
    public void CreatePlan_NoChange_UsesTableHash()
    {
        var table = Table(["Name", "Value"], [TableAlignment.Left, TableAlignment.Right], [["A", "1"]]);

        var plan = engine.CreatePlan(table, table);

        Assert.Equal(TableDiffSafety.Safe, plan.Safety);
        Assert.Equal(TableDiffKind.NoChange, Assert.Single(plan.Operations).Kind);
        Assert.Equal(plan.PreviousHash, plan.CurrentHash);
    }

    [Fact]
    public void CreatePlan_CellUpdate_IdentifiesStableRowAndColumn()
    {
        var previous = Table(["Name", "Value"], [TableAlignment.Left, TableAlignment.Right], [["A", "1"]]);
        var current = Table(["Name", "Value"], [TableAlignment.Left, TableAlignment.Right], [["A", "2"]]);

        var operation = Assert.Single(engine.CreatePlan(previous, current).Operations);

        Assert.Equal(TableDiffKind.CellUpdate, operation.Kind);
        Assert.Equal(1, operation.PreviousRowIndex);
        Assert.Equal(1, operation.CurrentRowIndex);
        Assert.Equal(1, operation.ColumnIndex);
    }

    [Fact]
    public void CreatePlan_RichContentChange_IsCellUpdate()
    {
        var previous = new TableBlock(
            [new TableColumn(TableAlignment.Left)],
            Row("Name"),
            [new TableRow([new TableCell([new TextInline("A")])])]);
        var current = new TableBlock(
            [new TableColumn(TableAlignment.Left)],
            Row("Name"),
            [new TableRow([new TableCell([new BoldInline([new TextInline("A")])])])]);

        var operation = Assert.Single(engine.CreatePlan(previous, current).Operations);

        Assert.Equal(TableDiffKind.CellUpdate, operation.Kind);
    }

    [Fact]
    public void CreatePlan_AlignmentChange_IsColumnOperation()
    {
        var previous = Table(["Name"], [TableAlignment.Left], [["A"]]);
        var current = Table(["Name"], [TableAlignment.Center], [["A"]]);

        var operation = Assert.Single(engine.CreatePlan(previous, current).Operations);

        Assert.Equal(TableDiffKind.AlignmentUpdate, operation.Kind);
        Assert.Equal(0, operation.ColumnIndex);
    }

    [Fact]
    public void CreatePlan_HeaderTextChange_RebuildsBecauseColumnIdentityChanged()
    {
        var previous = Table(["Name"], [TableAlignment.Left], [["A"]]);
        var current = Table(["Title"], [TableAlignment.Left], [["A"]]);

        var operation = Assert.Single(engine.CreatePlan(previous, current).Operations);

        Assert.Equal(TableDiffKind.Rebuild, operation.Kind);
        Assert.Equal(TableDiffSafety.RebuildRequired, engine.CreatePlan(previous, current).Safety);
    }

    [Fact]
    public void CreatePlan_HeaderRichContentChange_IsCellUpdate()
    {
        var previous = new TableBlock(
            [new TableColumn(TableAlignment.Left)],
            Row("Name"),
            [Row("A")]);
        var current = new TableBlock(
            [new TableColumn(TableAlignment.Left)],
            new TableRow([new TableCell([new BoldInline([new TextInline("Name")])])]),
            [Row("A")]);

        var operation = Assert.Single(engine.CreatePlan(previous, current).Operations);

        Assert.Equal(TableDiffKind.CellUpdate, operation.Kind);
        Assert.Equal(0, operation.CurrentRowIndex);
    }

    [Fact]
    public void CreatePlan_InsertDeleteMoveAndMoveAndUpdate_AreClassified()
    {
        var previous = Table(["Key", "Value"], [TableAlignment.Left, TableAlignment.Left],
            [["A", "1"], ["B", "2"], ["C", "3"]]);
        var inserted = Table(["Key", "Value"], [TableAlignment.Left, TableAlignment.Left],
            [["A", "1"], ["B", "2"], ["C", "3"], ["D", "4"]]);
        var deleted = Table(["Key", "Value"], [TableAlignment.Left, TableAlignment.Left],
            [["A", "1"], ["C", "3"]]);
        var moved = Table(["Key", "Value"], [TableAlignment.Left, TableAlignment.Left],
            [["B", "2"], ["A", "1"], ["C", "3"]]);
        var moveAndUpdate = Table(["Key", "Value"], [TableAlignment.Left, TableAlignment.Left],
            [["B", "updated"], ["A", "1"], ["C", "3"]]);

        Assert.Equal(TableDiffKind.InsertRow, Assert.Single(engine.CreatePlan(previous, inserted).Operations).Kind);
        Assert.Contains(engine.CreatePlan(previous, deleted).Operations, item => item.Kind == TableDiffKind.DeleteRow);
        Assert.Contains(engine.CreatePlan(previous, moved).Operations, item => item.Kind == TableDiffKind.Move);
        Assert.Contains(engine.CreatePlan(previous, moveAndUpdate).Operations, item => item.Kind == TableDiffKind.MoveAndUpdate);
    }

    [Fact]
    public void CreatePlan_DuplicateRowIdentity_Rebuilds()
    {
        var previous = Table(["Key"], [TableAlignment.Left], [["A"], ["A"]]);
        var current = Table(["Key"], [TableAlignment.Left], [["A"]]);

        var plan = engine.CreatePlan(previous, current);

        Assert.Equal(TableDiffSafety.RebuildRequired, plan.Safety);
        Assert.Equal(TableDiffKind.Rebuild, Assert.Single(plan.Operations).Kind);
    }

    [Fact]
    public void CreatePlan_ColumnCountChange_Rebuilds()
    {
        var previous = Table(["A"], [TableAlignment.Left], [["1"]]);
        var current = Table(["A", "B"], [TableAlignment.Left, TableAlignment.Left], [["1", "2"]]);

        var plan = engine.CreatePlan(previous, current);

        Assert.Equal(TableDiffKind.Rebuild, Assert.Single(plan.Operations).Kind);
    }

    [Fact]
    public void CreatePhysicalPlan_OrdersDeletesDescendingAndInsertsAscending()
    {
        var logical = new TableDiffPlan(
            TableDiffSafety.Safe,
            "previous",
            "current",
            [
                new TableDiffOperation(TableDiffKind.InsertRow, null, 1, null, "insert"),
                new TableDiffOperation(TableDiffKind.DeleteRow, 4, null, null, "delete"),
                new TableDiffOperation(TableDiffKind.DeleteRow, 2, null, null, "delete"),
                new TableDiffOperation(TableDiffKind.InsertRow, null, 3, null, "insert"),
            ]);

        var physical = engine.CreatePhysicalPlan(logical);

        Assert.Equal([4, 2], physical.Operations.Take(2).Select(item => item.PreviousRowIndex));
        Assert.Equal([1, 3], physical.Operations.Skip(2).Select(item => item.CurrentRowIndex));
    }

    [Fact]
    public void Verify_DetectsAlreadyAppliedRollForwardAndUnsafeMismatch()
    {
        var previous = Table(["Key"], [TableAlignment.Left], [["A"]]);
        var current = Table(["Key"], [TableAlignment.Left], [["B"]]);
        var thirdPartyStructureMatch = Table(["Key"], [TableAlignment.Left], [["C"]]);
        var unsafeStructure = Table(["Key", "Value"], [TableAlignment.Left, TableAlignment.Left], [["C", "1"]]);

        Assert.Equal(TableDiffSafety.Safe, engine.Verify(previous, current, current));
        Assert.Equal(TableDiffSafety.RecoverableMismatch, engine.Verify(previous, current, previous));
        Assert.Equal(TableDiffSafety.RecoverableMismatch, engine.Verify(previous, current, thirdPartyStructureMatch));
        Assert.Equal(TableDiffSafety.UnsafeMismatch, engine.Verify(previous, current, unsafeStructure));
    }

    [Fact]
    public void CanonicalModel_EmptyCell_HasStableHash()
    {
        var first = CanonicalTableBlock.Create(new TableBlock(
            [new TableColumn(TableAlignment.Left)],
            Row("Name"),
            [new TableRow([TableCell.Empty()])]));
        var second = CanonicalTableBlock.Create(new TableBlock(
            [new TableColumn(TableAlignment.Left)],
            Row("Name"),
            [new TableRow([TableCell.Empty()])]));

        Assert.Equal(first.Hash, second.Hash);
        Assert.Empty(first.Body[0].Cells[0].Text);
    }

    private static TableBlock Table(
        string[] header,
        TableAlignment[] alignments,
        string[][] rows) => new(
        alignments.Select(alignment => new TableColumn(alignment)),
        Row(header),
        rows.Select(Row));

    private static TableRow Row(params string[] values) =>
        new(values.Select(value => string.IsNullOrEmpty(value)
            ? TableCell.Empty()
            : new TableCell([new TextInline(value)])));
}
