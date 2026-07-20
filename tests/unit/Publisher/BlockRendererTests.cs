using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class BlockRendererTests
{
    [Fact]
    public void ParagraphRenderer_InsertsTextAndAppliesRelativeInlineRanges()
    {
        var block = new ParagraphBlock(
        [
            new TextInline("A"),
            new BoldInline([new TextInline("B")]),
        ]);
        var operations = new List<DocumentOperation>();

        var nextIndex = new ParagraphBlockRenderer().Render(block, 5, operations);

        Assert.Equal(8, nextIndex);
        Assert.Collection(
            operations,
            operation =>
            {
                Assert.Equal(DocumentOperationKind.InsertText, operation.Kind);
                Assert.Equal(5, operation.StartIndex);
                Assert.Equal("AB\n", operation.Text);
            },
            operation => AssertStyle(operation, 6, 7, InlineTextStyle.Bold));
    }

    [Fact]
    public void HeadingRenderer_AppliesParagraphAndInlineStylesToIndependentRanges()
    {
        var block = new HeadingBlock(
            2,
            [new ItalicInline([new TextInline("Title")])]);
        var operations = new List<DocumentOperation>();

        var nextIndex = new HeadingBlockRenderer().Render(block, 1, operations);

        Assert.Equal(7, nextIndex);
        Assert.Collection(
            operations,
            operation => Assert.Equal(DocumentOperationKind.InsertText, operation.Kind),
            operation =>
            {
                Assert.Equal(DocumentOperationKind.ApplyHeading, operation.Kind);
                Assert.Equal(1, operation.StartIndex);
                Assert.Equal(7, operation.EndIndex);
                Assert.Equal(2, operation.Level);
            },
            operation => AssertStyle(operation, 1, 6, InlineTextStyle.Italic));
    }

    private static void AssertStyle(
        DocumentOperation operation,
        int startIndex,
        int endIndex,
        InlineTextStyle style)
    {
        Assert.Equal(DocumentOperationKind.UpdateTextStyle, operation.Kind);
        Assert.Equal(startIndex, operation.StartIndex);
        Assert.Equal(endIndex, operation.EndIndex);
        Assert.Equal(style, operation.InlineStyle);
    }
}
