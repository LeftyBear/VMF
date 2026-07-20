using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownImageParserTests
{
    [Theory]
    [InlineData("![](image.png)", "", "image.png")]
    [InlineData(@"![a\]b](images/photo.jpg)", "a]b", "images/photo.jpg")]
    public void TryParse_ParsesStandaloneLocalImage(string markdown, string altText, string source)
    {
        var parsed = new MarkdownImageParser().TryParse(markdown, out var image);

        Assert.True(parsed);
        Assert.Equal(altText, image?.AltText);
        Assert.Equal(source, Assert.IsType<LocalImageSource>(image?.Source).Path);
    }

    [Fact]
    public void TryParse_ParsesAbsoluteRemoteImage()
    {
        var parsed = new MarkdownImageParser().TryParse(
            "![Remote](https://example.com/image.png)",
            out var image);

        Assert.True(parsed);
        Assert.Equal(
            new Uri("https://example.com/image.png"),
            Assert.IsType<RemoteImageSource>(image?.Source).Uri);
    }

    [Theory]
    [InlineData("Text ![alt](image.png)")]
    [InlineData("![alt](image.png) trailing")]
    [InlineData("![alt](image.png")]
    [InlineData("![alt] image.png")]
    public void TryParse_NonStandaloneOrMalformedImage_ReturnsFalse(string markdown)
    {
        Assert.False(new MarkdownImageParser().TryParse(markdown, out var image));
        Assert.Null(image);
    }

    [Fact]
    public void Parse_OnlyStandaloneImagesBecomeImageBlocks()
    {
        var document = new SimpleMarkdownParser().Parse(
            "Before ![inline](inline.png)\n\n" +
            "![standalone](standalone.png)\n\n" +
            "![trailing](trailing.png) text\n");

        Assert.Equal(
            [DocumentBlockKind.Paragraph, DocumentBlockKind.Image, DocumentBlockKind.Paragraph],
            document.Blocks.Select(block => block.Kind));
        Assert.Equal("standalone", document.Blocks[1].Image?.AltText);
    }
}
