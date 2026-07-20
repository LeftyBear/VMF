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

    [Fact]
    public void Render_AdjustsInlineStyleRangesAfterEveryLeadingTabIsRemoved()
    {
        var url = new Uri("https://example.com/");
        var block = new ListBlock(
        [
            ListItem.FromText(ListKind.Unordered, "Root", 0),
            new ListItem(ListKind.Ordered, [new BoldInline([new CodeInline("One")])], 1),
            new ListItem(ListKind.Ordered, [new ItalicInline([new TextInline("Deep")])], 2),
            new ListItem(ListKind.Unordered, [new LinkInline([new TextInline("Tail")], url)], 1),
        ]);
        var operations = new List<DocumentOperation>();

        var nextIndex = new ListBlockRenderer().Render(block, 1, operations);

        Assert.Equal(20, nextIndex);
        Assert.Equal("Root\n\tOne\n\t\tDeep\n\tTail\n", operations[0].Text);
        Assert.Collection(
            operations.Skip(4),
            operation => AssertStyleOperation(operation, 6, 9, InlineTextStyle.Code),
            operation => AssertStyleOperation(operation, 6, 9, InlineTextStyle.Bold),
            operation => AssertStyleOperation(operation, 10, 14, InlineTextStyle.Italic),
            operation => AssertStyleOperation(operation, 15, 19, InlineTextStyle.Link, url));
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

    private static void AssertStyleOperation(
        DocumentOperation operation,
        int startIndex,
        int endIndex,
        InlineTextStyle style,
        Uri? url = null)
    {
        Assert.Equal(DocumentOperationKind.UpdateTextStyle, operation.Kind);
        Assert.Equal(startIndex, operation.StartIndex);
        Assert.Equal(endIndex, operation.EndIndex);
        Assert.Equal(style, operation.InlineStyle);
        Assert.Equal(url, operation.Url);
    }
}
