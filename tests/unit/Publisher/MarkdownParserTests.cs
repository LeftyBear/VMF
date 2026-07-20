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

    [Fact]
    public void Parse_AppliesInlineParsingToHeadingsParagraphsAndListItems()
    {
        const string markdown =
            "# **Bold heading**\n\nParagraph with _italic_.\n\n- [**linked** label](https://example.com)\n";

        var document = new SimpleMarkdownParser().Parse(markdown);

        Assert.IsType<BoldInline>(document.Blocks[0].Content[0]);
        Assert.IsType<ItalicInline>(document.Blocks[1].Content[1]);
        var link = Assert.IsType<LinkInline>(document.Blocks[2].List?.Items[0].Content[0]);
        Assert.IsType<BoldInline>(link.Content[0]);
    }

    [Fact]
    public void Parse_UsesCodeTableHeadingListQuoteParagraphBlockOrder()
    {
        const string markdown =
            "```csharp\n# literal\n```\n" +
            "\n| H |\n| --- |\n| C |\n" +
            "\n# Heading\n- List\n> Quote\nAfter\n";

        var document = new SimpleMarkdownParser().Parse(markdown);

        Assert.Equal(
            [
                DocumentBlockKind.Code,
                DocumentBlockKind.Table,
                DocumentBlockKind.Heading,
                DocumentBlockKind.List,
                DocumentBlockKind.Quote,
                DocumentBlockKind.Paragraph,
            ],
            document.Blocks.Select(block => block.Kind));
        Assert.Equal("csharp", document.Blocks[0].Code?.Language);
        Assert.Equal("# literal", document.Blocks[0].Code?.Text);
        Assert.Equal("Quote", document.Blocks[4].Inlines[0].Text);
        Assert.Equal("After", document.Blocks[5].Inlines[0].Text);
    }

    [Fact]
    public void Parse_SplitsQuoteBlocksWhenNormalizedLevelChanges()
    {
        var document = new SimpleMarkdownParser().Parse(
            "> One\n> Two\n>> Nested\n>>>>>>>> Deep\n>>>>>>> Still deep\n> Tail\n");

        Assert.Equal([1, 2, 6, 1], document.Blocks.Select(block => block.Quote?.Level));
        Assert.Equal("One\nTwo", string.Concat(document.Blocks[0].Inlines.Select(item => item.Text)));
        Assert.Equal("Deep\nStill deep", string.Concat(document.Blocks[2].Inlines.Select(item => item.Text)));
    }
}
