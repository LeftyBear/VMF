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

    [Theory]
    [InlineData("line", 5, 10, 3)]
    [InlineData("", 1, 2, 2)]
    public void CodeBlockRenderer_StylesBodyButExcludesRequiredTrailingNewlineFromBackground(
        string text,
        int startIndex,
        int expectedNextIndex,
        int expectedOperationCount)
    {
        var operations = new List<DocumentOperation>();

        var nextIndex = new CodeBlockRenderer().Render(
            new CodeBlock(text, "csharp"),
            startIndex,
            operations);

        Assert.Equal(expectedNextIndex, nextIndex);
        Assert.Equal(text + "\n", operations[0].Text);
        Assert.Equal(DocumentOperationKind.ApplyCodeBlockStyle, operations[1].Kind);
        Assert.Equal(startIndex, operations[1].StartIndex);
        Assert.Equal(expectedNextIndex, operations[1].EndIndex);
        Assert.Equal(expectedOperationCount, operations.Count);
        if (text.Length > 0)
        {
            AssertStyle(operations[2], startIndex, expectedNextIndex - 1, InlineTextStyle.Code);
        }
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
