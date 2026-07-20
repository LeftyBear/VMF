using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownCodeBlockParserTests
{
    [Fact]
    public void TryParse_PreservesLanguageAndLiteralBodyUntilLongEnoughClosingFence()
    {
        string[] lines = ["  ```` csharp ", "**not bold**", "```", "````   ", "After"];

        var parsed = new MarkdownCodeBlockParser().TryParse(lines, 0, out var block, out var consumed);

        Assert.True(parsed);
        Assert.NotNull(block);
        Assert.Equal("csharp", block.Language);
        Assert.Equal("**not bold**\n```", block.Text);
        Assert.Equal(4, consumed);
    }

    [Fact]
    public void TryParse_AllowsEmptyAndUnclosedBlocks()
    {
        var parser = new MarkdownCodeBlockParser();

        Assert.True(parser.TryParse(["```", "```"], 0, out var empty, out var emptyLines));
        Assert.Equal(string.Empty, empty?.Text);
        Assert.Equal(2, emptyLines);

        Assert.True(parser.TryParse(["```text", "one", "two"], 0, out var unclosed, out var eofLines));
        Assert.Equal("one\ntwo", unclosed?.Text);
        Assert.Equal(3, eofLines);
    }

    [Theory]
    [InlineData("    ```")]
    [InlineData("  ``")]
    [InlineData("\t```")]
    public void TryParse_RejectsInvalidOpeningFence(string line)
    {
        var parsed = new MarkdownCodeBlockParser().TryParse([line], 0, out var block, out var consumed);

        Assert.False(parsed);
        Assert.Null(block);
        Assert.Equal(0, consumed);
    }
}
