using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownTableParserTests
{
    [Fact]
    public void Parse_MapsHeaderRowsAlignmentsAndInlineContent()
    {
        const string markdown =
            "| Name | Status | Note |\n" +
            "| --- | :---: | ---: |\n" +
            "| **Publisher** | Active | *v1.0* |\n" +
            "| [Renderer](https://example.com) | Ready | 100% |\n";

        var document = new SimpleMarkdownParser().Parse(markdown);

        var block = Assert.Single(document.Blocks);
        Assert.Equal(DocumentBlockKind.Table, block.Kind);
        var table = Assert.IsType<TableBlock>(block.Table);
        Assert.Equal(
            [TableAlignment.Left, TableAlignment.Center, TableAlignment.Right],
            table.Columns.Select(column => column.Alignment));
        Assert.Equal("Name", Assert.IsType<TextInline>(table.Header.Cells[0].Content[0]).Text);
        Assert.IsType<BoldInline>(table.Rows[0].Cells[0].Content[0]);
        Assert.IsType<ItalicInline>(table.Rows[0].Cells[2].Content[0]);
        Assert.IsType<LinkInline>(table.Rows[1].Cells[0].Content[0]);
    }

    [Fact]
    public void Parse_AcceptsRowsWithoutOuterPipesAndEscapedCellPipe()
    {
        const string markdown =
            "Name | Note\n" +
            "--- | ---\n" +
            "Publisher | A \\| B\n";

        var table = Assert.IsType<TableBlock>(Assert.Single(new SimpleMarkdownParser()
            .Parse(markdown).Blocks).Table);

        Assert.Equal("A | B", Assert.IsType<TextInline>(table.Rows[0].Cells[1].Content[0]).Text);
    }

    [Fact]
    public void Parse_PadsMissingCellsAndDiscardsExcessCells()
    {
        const string markdown =
            "A | B | C\n" +
            "--- | --- | ---\n" +
            "one | two\n" +
            "1 | 2 | 3 | discarded\n";

        var table = Assert.IsType<TableBlock>(Assert.Single(new SimpleMarkdownParser()
            .Parse(markdown).Blocks).Table);

        Assert.Empty(table.Rows[0].Cells[2].Content);
        Assert.Equal(3, table.Rows[1].Cells.Count);
        Assert.Equal("3", Assert.IsType<TextInline>(table.Rows[1].Cells[2].Content[0]).Text);
    }

    [Theory]
    [InlineData("A | B\n---\none | two\n")]
    [InlineData("A | B\n-- | ---\none | two\n")]
    [InlineData("A | B\n:-:- | ---\none | two\n")]
    public void Parse_InvalidDelimiterFallsBackToParagraph(string markdown)
    {
        var document = new SimpleMarkdownParser().Parse(markdown);

        var paragraph = Assert.Single(document.Blocks);
        Assert.Equal(DocumentBlockKind.Paragraph, paragraph.Kind);
        Assert.Null(paragraph.Table);
    }

    [Fact]
    public void TryParse_RequiresHeaderAndDelimiterRows()
    {
        var parser = new MarkdownTableParser();

        var parsed = parser.TryParse(["A | B"], 0, out var table, out var consumed);

        Assert.False(parsed);
        Assert.Null(table);
        Assert.Equal(0, consumed);
    }
}
