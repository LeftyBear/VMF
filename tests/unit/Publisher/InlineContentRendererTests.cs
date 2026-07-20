using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class InlineContentRendererTests
{
    [Fact]
    public void Render_FlattensContentMergesSameStylesAndKeepsOverlaps()
    {
        var url = new Uri("https://example.com/");
        InlineContent[] content =
        [
            new TextInline("A"),
            new BoldInline([
                new TextInline("B"),
                new ItalicInline([new TextInline("C")]),
            ]),
            new BoldInline([new TextInline("D")]),
            new ItalicInline([new TextInline("E")]),
            new LinkInline([new BoldInline([new TextInline("F")])], url),
        ];

        var rendered = new InlineContentRenderer().Render(content);

        Assert.Equal("ABCDEF", rendered.Text);
        Assert.Collection(
            rendered.StyleRanges,
            range => AssertRange(range, 1, 4, InlineTextStyle.Bold),
            range => AssertRange(range, 2, 3, InlineTextStyle.Italic),
            range => AssertRange(range, 4, 5, InlineTextStyle.Italic),
            range => AssertRange(range, 5, 6, InlineTextStyle.Bold),
            range => AssertRange(range, 5, 6, InlineTextStyle.Link, url));
    }

    [Fact]
    public void Render_MergesAdjacentLinksOnlyWhenUrlsMatch()
    {
        var first = new Uri("https://example.com/first");
        var second = new Uri("https://example.com/second");
        InlineContent[] content =
        [
            new LinkInline([new TextInline("A")], first),
            new LinkInline([new TextInline("B")], first),
            new LinkInline([new TextInline("C")], second),
        ];

        var rendered = new InlineContentRenderer().Render(content);

        Assert.Collection(
            rendered.StyleRanges,
            range => AssertRange(range, 0, 2, InlineTextStyle.Link, first),
            range => AssertRange(range, 2, 3, InlineTextStyle.Link, second));
    }

    [Fact]
    public void Render_PreservesCodeRangesOverlappingOuterStyles()
    {
        InlineContent[] content =
        [
            new BoldInline([new CodeInline("one")]),
            new CodeInline("two"),
        ];

        var rendered = new InlineContentRenderer().Render(content);

        Assert.Equal("onetwo", rendered.Text);
        Assert.Collection(
            rendered.StyleRanges,
            range => AssertRange(range, 0, 3, InlineTextStyle.Bold),
            range => AssertRange(range, 0, 6, InlineTextStyle.Code));
    }

    private static void AssertRange(
        InlineStyleRange range,
        int start,
        int end,
        InlineTextStyle style,
        Uri? url = null)
    {
        Assert.Equal(start, range.StartOffset);
        Assert.Equal(end, range.EndOffset);
        Assert.Equal(style, range.Style);
        Assert.Equal(url, range.Url);
    }
}
