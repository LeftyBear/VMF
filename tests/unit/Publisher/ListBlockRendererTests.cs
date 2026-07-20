using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class ListBlockRendererTests
{
    [Fact]
    public void Render_UsesTabsForDepthGroupsKindsAndReturnsPostBulletIndex()
    {
        var block = new ListBlock(
        [
            Item(ListKind.Unordered, 0, "Root"),
            Item(ListKind.Ordered, 1, "One"),
            Item(ListKind.Ordered, 2, "Nested"),
            Item(ListKind.Unordered, 1, "Tail bullet"),
        ]);
        var operations = new List<DocumentOperation>();

        var nextIndex = new ListBlockRenderer().Render(block, 1, operations);

        Assert.Equal(29, nextIndex);
        Assert.Collection(
            operations,
            operation =>
            {
                Assert.Equal(DocumentOperationKind.InsertText, operation.Kind);
                Assert.Equal("Root\n\tOne\n\t\tNested\n\tTail bullet\n", operation.Text);
            },
            operation => AssertListOperation(operation, ListKind.Unordered, 1, 6),
            operation => AssertListOperation(operation, ListKind.Ordered, 6, 20),
            operation => AssertListOperation(operation, ListKind.Unordered, 17, 30));
    }

    private static ListItem Item(ListKind kind, int depth, string text) =>
        new(kind, [new InlineElement(InlineElementKind.Text, text)], depth);

    private static void AssertListOperation(
        DocumentOperation operation,
        ListKind kind,
        int startIndex,
        int endIndex)
    {
        Assert.Equal(DocumentOperationKind.CreateBullet, operation.Kind);
        Assert.Equal(kind, operation.ListKind);
        Assert.Equal(startIndex, operation.StartIndex);
        Assert.Equal(endIndex, operation.EndIndex);
    }
}
