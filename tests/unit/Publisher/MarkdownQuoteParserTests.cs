using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownQuoteParserTests
{
    [Fact]
    public void TryParse_GroupsSameLevelPreservesEmptyLinesAndParsesInlineContent()
    {
        string[] lines = ["> **bold**", ">", "> `code`", ">> nested"];

        var parsed = new MarkdownQuoteParser().TryParse(lines, 0, out var block, out var consumed);

        Assert.True(parsed);
        Assert.NotNull(block);
        Assert.Equal(1, block.Level);
        Assert.Equal(3, consumed);
        Assert.Contains(block.Content, inline => inline is BoldInline);
        Assert.Contains(block.Content, inline => inline is CodeInline);
        Assert.Equal("bold\n\ncode", string.Concat(block.Content.Select(Text)));
    }

    [Theory]
    [InlineData("> quoted", 1, "quoted")]
    [InlineData(">>> nested", 3, "nested")]
    [InlineData(">>>>>>>> normalized", 6, "normalized")]
    [InlineData("   > indented", 1, "indented")]
    public void TryParse_DetectsAndNormalizesContinuousMarkers(
        string line,
        int expectedLevel,
        string expectedText)
    {
        var parsed = new MarkdownQuoteParser().TryParse([line], 0, out var block, out var consumed);

        Assert.True(parsed);
        Assert.Equal(expectedLevel, block?.Level);
        Assert.Equal(expectedText, string.Concat(block?.Content.Select(Text) ?? []));
        Assert.Equal(1, consumed);
    }

    private static string Text(InlineContent content) => content switch
    {
        TextInline text => text.Text,
        CodeInline code => code.Text,
        BoldInline bold => string.Concat(bold.Content.Select(Text)),
        ItalicInline italic => string.Concat(italic.Content.Select(Text)),
        LinkInline link => string.Concat(link.Content.Select(Text)),
        _ => throw new InvalidOperationException(),
    };
}
