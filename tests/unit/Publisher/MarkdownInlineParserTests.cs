using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownInlineParserTests
{
    [Fact]
    public void Parse_MapsAsteriskAndUnderscoreStylesAndHttpLinks()
    {
        const string markdown =
            "**bold** *italic* ***both*** __under bold__ _under italic_ ___under both___ " +
            "[label](https://example.com/path)";

        var content = new MarkdownInlineParser().Parse(markdown);

        Assert.Collection(
            content,
            inline => AssertStyled<BoldInline>(inline, "bold"),
            inline => AssertText(inline, " "),
            inline => AssertStyled<ItalicInline>(inline, "italic"),
            inline => AssertText(inline, " "),
            inline => AssertBoldItalic(inline, "both"),
            inline => AssertText(inline, " "),
            inline => AssertStyled<BoldInline>(inline, "under bold"),
            inline => AssertText(inline, " "),
            inline => AssertStyled<ItalicInline>(inline, "under italic"),
            inline => AssertText(inline, " "),
            inline => AssertBoldItalic(inline, "under both"),
            inline => AssertText(inline, " "),
            inline =>
            {
                var link = Assert.IsType<LinkInline>(inline);
                Assert.Equal("label", Text(link.Content));
                Assert.Equal("https://example.com/path", link.Url.AbsoluteUri);
            });
    }

    [Fact]
    public void Parse_ParsesStyledLinkLabelAndPreservesOverlappingStyles()
    {
        var content = new MarkdownInlineParser().Parse(
            "[**bold** and _italic_](http://example.com)");

        var link = Assert.IsType<LinkInline>(Assert.Single(content));
        Assert.Equal("bold and italic", Text(link.Content));
        Assert.IsType<BoldInline>(link.Content[0]);
        Assert.IsType<TextInline>(link.Content[1]);
        Assert.IsType<ItalicInline>(link.Content[2]);
    }

    [Fact]
    public void Parse_MapsInlineCodeBeforeLinksAndEmphasisWithoutParsingItsBody()
    {
        const string markdown = @"Use `**not bold** [not a link](https://example.com) \*literal` now.";

        var content = new MarkdownInlineParser().Parse(markdown);

        Assert.Collection(
            content,
            inline => AssertText(inline, "Use "),
            inline => Assert.Equal(
                @"**not bold** [not a link](https://example.com) \*literal",
                Assert.IsType<CodeInline>(inline).Text),
            inline => AssertText(inline, " now."));
    }

    [Fact]
    public void Parse_AllowsCodeToOverlapOuterBoldAndLinkStyles()
    {
        var content = new MarkdownInlineParser().Parse(
            "**`bold code`** [`linked code`](https://example.com)");

        var bold = Assert.IsType<BoldInline>(content[0]);
        Assert.IsType<CodeInline>(Assert.Single(bold.Content));
        var link = Assert.IsType<LinkInline>(content[2]);
        Assert.IsType<CodeInline>(Assert.Single(link.Content));
    }

    [Fact]
    public void Parse_SupportsMatchingMultiBacktickDelimiters()
    {
        var content = new MarkdownInlineParser().Parse("``one ` two``");

        Assert.Equal("one ` two", Assert.IsType<CodeInline>(Assert.Single(content)).Text);
    }

    [Fact]
    public void Parse_UnescapesMarkdownPunctuationAndCoalescesAdjacentText()
    {
        var content = new MarkdownInlineParser().Parse(
            @"\*plain\* \_plain\_ \[label\]\(url\) \\");

        var text = Assert.IsType<TextInline>(Assert.Single(content));
        Assert.Equal("*plain* _plain_ [label](url) \\", text.Text);
    }

    [Theory]
    [InlineData("**unclosed")]
    [InlineData("***unclosed")]
    [InlineData("** **")]
    [InlineData("****")]
    [InlineData("[](https://example.com)")]
    [InlineData("[label]()")]
    [InlineData("[label](relative/path)")]
    [InlineData("[label](mailto:user@example.com)")]
    [InlineData("[label](https://)")]
    [InlineData("[label](https://example.com")]
    [InlineData("``")]
    [InlineData("`unclosed")]
    [InlineData("``unclosed`")]
    public void Parse_InvalidOrIncompleteMarkupFallsBackToPlainText(string markdown)
    {
        var content = new MarkdownInlineParser().Parse(markdown);

        Assert.Equal(markdown, Text(content));
        Assert.All(content, inline => Assert.IsType<TextInline>(inline));
        Assert.Single(content);
    }

    [Fact]
    public void Parse_InvalidUrlKeepsStyledLabelSyntaxAsPlainText()
    {
        const string markdown = "[**label**](relative/path)";

        var content = new MarkdownInlineParser().Parse(markdown);

        Assert.Equal(markdown, Assert.IsType<TextInline>(Assert.Single(content)).Text);
    }

    [Fact]
    public void Parse_AllowsBalancedParenthesesInAbsoluteUrl()
    {
        var content = new MarkdownInlineParser().Parse(
            "[label](https://example.com/a_(b))");

        var link = Assert.IsType<LinkInline>(Assert.Single(content));
        Assert.Equal("label", Text(link.Content));
        Assert.Contains("a_(b)", link.Url.AbsoluteUri, StringComparison.Ordinal);
    }

    [Fact]
    public void Parse_StopsParsingStylesAtConfiguredMaximumDepth()
    {
        var parser = new MarkdownInlineParser(new MarkdownInlineParserOptions
        {
            MaxInlineDepth = 1,
        });

        var content = parser.Parse("**outer _inner_**");

        var bold = Assert.IsType<BoldInline>(Assert.Single(content));
        Assert.Equal("outer _inner_", Text(bold.Content));
        Assert.All(bold.Content, inline => Assert.IsType<TextInline>(inline));
    }

    [Fact]
    public void Constructor_RejectsNonPositiveMaximumDepth()
    {
        var options = new MarkdownInlineParserOptions { MaxInlineDepth = 0 };

        Assert.Throws<ArgumentOutOfRangeException>(() => new MarkdownInlineParser(options));
    }

    [Fact]
    public void Parse_DefaultMaximumDepthIsEight()
    {
        Assert.Equal(8, new MarkdownInlineParserOptions().MaxInlineDepth);
        var eightLevels = WrapAlternating("x", 8);
        var nineLevels = WrapAlternating("x", 9);

        var parsedEight = new MarkdownInlineParser().Parse(eightLevels);
        var parsedNine = new MarkdownInlineParser().Parse(nineLevels);

        Assert.Equal(8, CountStyles(Assert.Single(parsedEight)));
        Assert.Equal("x", Text(parsedEight));
        Assert.Equal(8, CountStyles(Assert.Single(parsedNine)));
        Assert.Contains('*', Text(parsedNine));
    }

    private static void AssertText(InlineContent inline, string expected)
    {
        Assert.Equal(expected, Assert.IsType<TextInline>(inline).Text);
    }

    private static void AssertStyled<TInline>(InlineContent inline, string expected)
        where TInline : InlineContent
    {
        var styled = Assert.IsType<TInline>(inline);
        var content = styled switch
        {
            BoldInline bold => bold.Content,
            ItalicInline italic => italic.Content,
            _ => throw new InvalidOperationException(),
        };
        Assert.Equal(expected, Text(content));
    }

    private static void AssertBoldItalic(InlineContent inline, string expected)
    {
        var bold = Assert.IsType<BoldInline>(inline);
        var italic = Assert.IsType<ItalicInline>(Assert.Single(bold.Content));
        Assert.Equal(expected, Text(italic.Content));
    }

    private static string Text(IEnumerable<InlineContent> content) =>
        string.Concat(content.Select(Text));

    private static string Text(InlineContent content) => content switch
    {
        TextInline text => text.Text,
        CodeInline code => code.Text,
        BoldInline bold => Text(bold.Content),
        ItalicInline italic => Text(italic.Content),
        LinkInline link => Text(link.Content),
        _ => throw new InvalidOperationException(),
    };

    private static string WrapAlternating(string text, int levels)
    {
        for (var level = 0; level < levels; level++)
        {
            var delimiter = level % 2 == 0 ? "**" : "_";
            text = delimiter + text + delimiter;
        }

        return text;
    }

    private static int CountStyles(InlineContent content) => content switch
    {
        BoldInline bold => 1 + CountStyles(Assert.Single(bold.Content)),
        ItalicInline italic => 1 + CountStyles(Assert.Single(italic.Content)),
        LinkInline link => 1 + CountStyles(Assert.Single(link.Content)),
        CodeInline => 0,
        TextInline => 0,
        _ => throw new InvalidOperationException(),
    };
}
