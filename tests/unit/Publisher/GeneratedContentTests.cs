using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class GeneratedContentTests
{
    [Fact]
    public void ListBlock_PreservesOrderedMixedItems()
    {
        var source = new List<ListItem>
        {
            Item(ListKind.Unordered, 0, "Root"),
            Item(ListKind.Ordered, 1, "Nested"),
        };

        var block = new ListBlock(source);
        source.Clear();

        Assert.Collection(
            block.Items,
            item =>
            {
                Assert.Equal(ListKind.Unordered, item.Kind);
                Assert.Equal(0, item.Depth);
                Assert.Equal("Root", item.Inlines[0].Text);
            },
            item =>
            {
                Assert.Equal(ListKind.Ordered, item.Kind);
                Assert.Equal(1, item.Depth);
                Assert.Equal("Nested", item.Inlines[0].Text);
            });
    }

    [Fact]
    public void ListItem_RejectsNegativeDepth()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Item(ListKind.Unordered, -1, "Invalid"));
    }

    private static ListItem Item(ListKind kind, int depth, string text) =>
        new(kind, [new InlineElement(InlineElementKind.Text, text)], depth);
}
