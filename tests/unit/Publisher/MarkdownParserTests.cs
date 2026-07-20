using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownParserTests
{
    [Fact]
    public void Parse_MapsHeadingsParagraphsAndListBlock()
    {
        const string markdown = "# Title\n\nFirst line\nsecond line\n\n- One\n* Two\n";
        var parser = new SimpleMarkdownParser();

        var document = parser.Parse(markdown);

        Assert.Collection(
            document.Blocks,
            block =>
            {
                Assert.Equal(DocumentBlockKind.Heading, block.Kind);
                Assert.Equal(1, block.Level);
                Assert.Equal("Title", block.Inlines[0].Text);
            },
            block =>
            {
                Assert.Equal(DocumentBlockKind.Paragraph, block.Kind);
                Assert.Equal("First line second line", block.Inlines[0].Text);
            },
            block =>
            {
                Assert.Equal(DocumentBlockKind.List, block.Kind);
                Assert.NotNull(block.List);
                Assert.Collection(
                    block.List.Items,
                    item =>
                    {
                        Assert.Equal(ListKind.Unordered, item.Kind);
                        Assert.Equal("One", item.Inlines[0].Text);
                    },
                    item =>
                    {
                        Assert.Equal(ListKind.Unordered, item.Kind);
                        Assert.Equal("Two", item.Inlines[0].Text);
                    });
            });
    }

    [Fact]
    public void Parse_MapsOrderedNestedAndMixedListsAndFollowingParagraph()
    {
        const string markdown =
            "- Bullet\n  1. Numbered\n    * Deep bullet\n      1) Fourth level\n- Tail\n\nAfter list.\n";

        var document = new SimpleMarkdownParser().Parse(markdown);

        Assert.Collection(
            document.Blocks,
            block =>
            {
                Assert.Equal(DocumentBlockKind.List, block.Kind);
                Assert.NotNull(block.List);
                Assert.Equal(
                    [ListKind.Unordered, ListKind.Ordered, ListKind.Unordered, ListKind.Ordered, ListKind.Unordered],
                    block.List.Items.Select(item => item.Kind));
                Assert.Equal([0, 1, 2, 3, 0], block.List.Items.Select(item => item.Depth));
            },
            block =>
            {
                Assert.Equal(DocumentBlockKind.Paragraph, block.Kind);
                Assert.Equal("After list.", block.Inlines[0].Text);
            });
    }

    [Fact]
    public void Parse_EmptyMarkdown_ReturnsEmptyDocument()
    {
        var document = new SimpleMarkdownParser().Parse(string.Empty);

        Assert.Empty(document.Blocks);
    }
}
