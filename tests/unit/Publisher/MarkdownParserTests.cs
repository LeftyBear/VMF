using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownParserTests
{
    [Fact]
    public void Parse_MapsHeadingsParagraphsAndBullets()
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
            block => Assert.Equal(DocumentBlockKind.BulletListItem, block.Kind),
            block => Assert.Equal(DocumentBlockKind.BulletListItem, block.Kind));
    }

    [Fact]
    public void Parse_EmptyMarkdown_ReturnsEmptyDocument()
    {
        var document = new SimpleMarkdownParser().Parse(string.Empty);

        Assert.Empty(document.Blocks);
    }
}
