using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class ImageDiffEngineTests
{
    private readonly ImageDiffEngine engine = new();

    [Fact]
    public void CreatePlan_ClassifiesAllImageDiffKinds()
    {
        AssertKind(ImageDiffKind.NoChange, [Image("a", "one")], [Image("a", "one")]);
        AssertKind(ImageDiffKind.Insert, [], [Image("a", "one")]);
        AssertKind(ImageDiffKind.Delete, [Image("a", "one")], []);
        AssertKind(ImageDiffKind.Move, [Text("x"), Image("a", "one")], [Image("a", "one"), Text("x")]);
        AssertKind(ImageDiffKind.ReplaceContent, [Image("a", "one")], [Image("a", "two")]);
        AssertKind(ImageDiffKind.UpdatePresentation, [Image("a", "one")], [Image("a", "one", alt: "changed")]);
        AssertKind(ImageDiffKind.MoveAndReplaceContent, [Text("x"), Image("a", "one")], [Image("a", "two"), Text("x")]);
        AssertKind(ImageDiffKind.MoveAndUpdatePresentation, [Text("x"), Image("a", "one")], [Image("a", "one", alt: "changed"), Text("x")]);
        AssertKind(ImageDiffKind.ReplaceContentAndUpdatePresentation, [Image("a", "one")], [Image("a", "two", alt: "changed")]);
        AssertKind(ImageDiffKind.MoveReplaceAndUpdatePresentation, [Text("x"), Image("a", "one")], [Image("a", "two", alt: "changed"), Text("x")]);
    }

    [Fact]
    public void CreatePlan_AmbiguousContentHashDoesNotClassifyMove()
    {
        var previous = new[] { Image(null, "same"), Image(null, "same") };
        var current = new[] { Text("x"), Image(null, "same"), Image(null, "same") };

        var kinds = engine.CreatePlan(previous, current).Select(result => result.Kind).ToArray();

        Assert.DoesNotContain(ImageDiffKind.Move, kinds);
        Assert.Equal(2, kinds.Count(kind => kind == ImageDiffKind.Insert));
        Assert.Equal(2, kinds.Count(kind => kind == ImageDiffKind.Delete));
    }

    [Fact]
    public void CreatePhysicalPlan_ExpandsMoveAndReplaceToDeleteInsertAndOrdersDescending()
    {
        var previous = new[] { Text("x"), Image("a", "one"), Image("b", "two") };
        var current = new[] { Image("b", "changed"), Text("x"), Image("a", "one") };

        var physical = engine.CreatePhysicalPlan(engine.CreatePlan(previous, current));

        Assert.Equal(
            [ImageDiffKind.Delete, ImageDiffKind.Delete, ImageDiffKind.Insert, ImageDiffKind.Insert],
            physical.Operations.Select(operation => operation.Kind));
        Assert.Equal([1, 0], physical.Operations.Take(2).Select(operation => operation.PreviousIndex));
        Assert.Equal([1, 0], physical.Operations.Skip(2).Select(operation => operation.CurrentIndex));
    }

    private void AssertKind(ImageDiffKind expected, DocumentBlock[] previous, DocumentBlock[] current)
    {
        var result = engine.CreatePlan(previous, current);
        Assert.Equal(expected, Assert.Single(result).Kind);
    }

    private static DocumentBlock Image(string? stableId, string hashSeed, string alt = "alt") => new(
        new ImageBlock(
            alt,
            new RemoteImageSource(new Uri($"https://example.com/{hashSeed}.png")),
            new ImageSize(10, 20),
            Hash(hashSeed),
            stableId),
        stableId);

    private static DocumentBlock Text(string text) => new(ParagraphBlock.FromText(text));

    private static string Hash(string value) =>
        ImageContentHash.ValuePrefix + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
