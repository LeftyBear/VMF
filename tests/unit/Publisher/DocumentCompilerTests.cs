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
}
