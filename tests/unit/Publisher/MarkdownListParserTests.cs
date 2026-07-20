using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownListParserTests
{
    [Fact]
    public void Options_ExposePhaseDefaults()
    {
        var options = new MarkdownListParserOptions();

        Assert.Equal(2, options.ListIndentSize);
        Assert.Equal(6, options.MaxListDepth);
    }

    [Theory]
    [InlineData("- Bullet", ListKind.Unordered)]
    [InlineData("+ Bullet", ListKind.Unordered)]
    [InlineData("* Bullet", ListKind.Unordered)]
    [InlineData("1. Ordered", ListKind.Ordered)]
    [InlineData("42) Ordered", ListKind.Ordered)]
    public void ParseLine_RecognizesSupportedMarkers(string markdown, ListKind expectedKind)
    {
        var item = new MarkdownListParser().ParseLine(markdown, null);

        Assert.NotNull(item);
        Assert.Equal(expectedKind, item.Kind);
        Assert.Equal(0, item.Depth);
    }

    [Fact]
    public void ParseLine_NormalizesHierarchyJumpsAndMaximumDepth()
    {
        var parser = new MarkdownListParser(new MarkdownListParserOptions
        {
            ListIndentSize = 2,
            MaxListDepth = 3,
        });

        var first = parser.ParseLine("    - Indented first item", null);
        var jump = parser.ParseLine("          - Invalid jump", first!.Depth);
        var maximum = parser.ParseLine("                    - Beyond maximum", jump!.Depth);
        var capped = parser.ParseLine("                      - Still beyond maximum", maximum!.Depth);
        var dedented = parser.ParseLine("- Root again", capped!.Depth);

        Assert.Equal(0, first.Depth);
        Assert.Equal(1, jump.Depth);
        Assert.Equal(2, maximum.Depth);
        Assert.Equal(2, capped.Depth);
        Assert.Equal(0, dedented!.Depth);
    }

    [Fact]
    public void ParseLine_UsesConfiguredIndentWidthAndTabStops()
    {
        var parser = new MarkdownListParser(new MarkdownListParserOptions
        {
            ListIndentSize = 4,
            MaxListDepth = 6,
        });

        var spaceIndented = parser.ParseLine("    - Spaces", 0);
        var tabIndented = parser.ParseLine("\t- Tab", 0);

        Assert.Equal(1, spaceIndented!.Depth);
        Assert.Equal(1, tabIndented!.Depth);
    }

    [Fact]
    public void Constructor_RejectsInvalidSettings()
    {
        var options = new MarkdownListParserOptions { ListIndentSize = 0 };

        Assert.Throws<ArgumentOutOfRangeException>(() => new MarkdownListParser(options));
    }
}
