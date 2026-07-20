using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class DocumentCompilerTests
{
    [Fact]
    public void Compile_ProducesOrderedTextAndStyleOperations()
    {
        var document = new DocumentModel(
        [
            new DocumentBlock(
                DocumentBlockKind.Heading,
                [new InlineElement(InlineElementKind.Text, "Title")],
                2),
            new DocumentBlock(
                DocumentBlockKind.BulletListItem,
                [new InlineElement(InlineElementKind.Text, "Item")]),
        ]);

        var compiled = new DocumentCompiler().Compile(document, "Sample");

        Assert.Equal("Sample", compiled.Title);
        Assert.Collection(
            compiled.Operations,
            operation =>
            {
                Assert.Equal(DocumentOperationKind.InsertText, operation.Kind);
                Assert.Equal(1, operation.StartIndex);
                Assert.Equal("Title\n", operation.Text);
            },
            operation =>
            {
                Assert.Equal(DocumentOperationKind.ApplyHeading, operation.Kind);
                Assert.Equal(7, operation.EndIndex);
                Assert.Equal(2, operation.Level);
            },
            operation =>
            {
                Assert.Equal(DocumentOperationKind.InsertText, operation.Kind);
                Assert.Equal(7, operation.StartIndex);
            },
            operation =>
            {
                Assert.Equal(DocumentOperationKind.CreateBullet, operation.Kind);
                Assert.Equal(12, operation.EndIndex);
            });
    }

    [Fact]
    public void Compile_RendersNestedListWithoutShiftingFollowingParagraph()
    {
        var document = new DocumentModel(
        [
            new DocumentBlock(new ListBlock(
            [
                new ListItem(
                    ListKind.Unordered,
                    [new InlineElement(InlineElementKind.Text, "Root")],
                    0),
                new ListItem(
                    ListKind.Ordered,
                    [new InlineElement(InlineElementKind.Text, "Nested")],
                    1),
            ])),
            new DocumentBlock(
                DocumentBlockKind.Paragraph,
                [new InlineElement(InlineElementKind.Text, "After")]),
        ]);

        var compiled = new DocumentCompiler().Compile(document, "Sample");

        Assert.Equal("Root\n\tNested\n", compiled.Operations[0].Text);
        Assert.Equal(ListKind.Unordered, compiled.Operations[1].ListKind);
        Assert.Equal(ListKind.Ordered, compiled.Operations[2].ListKind);
        Assert.Equal(DocumentOperationKind.InsertText, compiled.Operations[3].Kind);
        Assert.Equal(13, compiled.Operations[3].StartIndex);
        Assert.Equal("After\n", compiled.Operations[3].Text);
    }

    [Fact]
    public void Compile_InlineStylesDoNotShiftFollowingBlockIndex()
    {
        var document = new DocumentModel(
        [
            new DocumentBlock(new ParagraphBlock(
            [
                new BoldInline([
                    new ItalicInline([new TextInline("Styled")]),
                ]),
            ])),
            new DocumentBlock(ParagraphBlock.FromText("After")),
        ]);

        var compiled = new DocumentCompiler().Compile(document, "Sample");

        Assert.Equal(DocumentOperationKind.InsertText, compiled.Operations[0].Kind);
        Assert.Equal("Styled\n", compiled.Operations[0].Text);
        Assert.Equal(DocumentOperationKind.UpdateTextStyle, compiled.Operations[1].Kind);
        Assert.Equal(DocumentOperationKind.UpdateTextStyle, compiled.Operations[2].Kind);
        Assert.Equal(DocumentOperationKind.InsertText, compiled.Operations[3].Kind);
        Assert.Equal(8, compiled.Operations[3].StartIndex);
        Assert.Equal("After\n", compiled.Operations[3].Text);
    }
}
