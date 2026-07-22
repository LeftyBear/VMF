using System.Text.RegularExpressions;
using System.Text;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class BlockContentHashGeneratorTests
{
    private readonly BlockContentHashGenerator generator = new();

    [Fact]
    public void Generate_SamePayloadProducesSameHash()
    {
        Assert.Equal(generator.Generate(Paragraph("same")), generator.Generate(Paragraph("same")));
    }

    [Fact]
    public void Generate_ContentChangeChangesHash()
    {
        Assert.NotEqual(generator.Generate(Paragraph("before")), generator.Generate(Paragraph("after")));
    }

    [Fact]
    public void Generate_ExplicitIdentityChangeDoesNotChangeHash()
    {
        var first = new DocumentBlock(ParagraphBlock.FromText("same"), "first");
        var second = new DocumentBlock(ParagraphBlock.FromText("same"), "second");

        Assert.Equal(generator.Generate(first), generator.Generate(second));
    }

    [Fact]
    public void Generate_DocumentIndexChangeDoesNotChangeHash()
    {
        var target = Paragraph("target");
        var before = new DocumentModel([target]);
        var after = new DocumentModel([Paragraph("inserted"), target]);

        Assert.Equal(generator.Generate(before.Blocks[0]), generator.Generate(after.Blocks[1]));
    }

    [Fact]
    public void Generate_NormalizesCrLfCrAndLf()
    {
        var crlf = new DocumentBlock(new CodeBlock("one\r\ntwo\rthree", "text"));
        var lf = new DocumentBlock(new CodeBlock("one\ntwo\nthree", "text"));

        Assert.Equal(generator.Generate(crlf), generator.Generate(lf));
    }

    [Fact]
    public void CanonicalWriter_DistinguishesNullAndEmptyString()
    {
        using var nullWriter = new CanonicalValueWriter();
        using var emptyWriter = new CanonicalValueWriter();
        nullWriter.Write("value", null);
        emptyWriter.Write("value", string.Empty);

        Assert.Equal("value:-1:\n", Encoding.UTF8.GetString(nullWriter.ToArray()));
        Assert.Equal("value:0:\n", Encoding.UTF8.GetString(emptyWriter.ToArray()));
    }

    [Fact]
    public void Generate_SerializesInlineListTableCodeQuoteAndImagePayloads()
    {
        var blocks = EverySupportedBlock().ToArray();
        var hashes = blocks.Select(generator.Generate).ToArray();

        Assert.Equal(blocks.Length, hashes.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Generate_UsesKnownSha256Vector()
    {
        var hash = generator.Generate(Paragraph("日本語\r\ntext"));

        Assert.Equal(
            "ch-v1:sha256:c1068d6adc725a4bc7cce58d3d2a2677b73344c6fb12bbb9559aa9ee04141b9c",
            hash);
    }

    [Fact]
    public void Generate_UsesLowercaseHexAndVersionPrefix()
    {
        var hash = generator.Generate(Paragraph("text"));

        Assert.Matches(
            new Regex("^ch-v1:sha256:[0-9a-f]{64}$", RegexOptions.CultureInvariant),
            hash);
    }

    private static IEnumerable<DocumentBlock> EverySupportedBlock()
    {
        yield return new DocumentBlock(new HeadingBlock(2, [
            new BoldInline([new ItalicInline([
                new LinkInline([new CodeInline("code")], new Uri("https://example.com/path")),
            ])]),
        ]));
        yield return Paragraph("paragraph");
        yield return new DocumentBlock(DocumentBlockKind.BulletListItem, [new TextInline("legacy")]);
        yield return new DocumentBlock(new ListBlock([
            new ListItem(ListKind.Unordered, [new TextInline("root")], 0),
            new ListItem(ListKind.Ordered, [new TextInline("nested")], 1),
        ]));
        yield return new DocumentBlock(new TableBlock(
            [new TableColumn(TableAlignment.Left), new TableColumn(TableAlignment.Right)],
            new TableRow([
                new TableCell([new TextInline("header")]),
                new TableCell([new TextInline("value")]),
            ]),
            [new TableRow([TableCell.Empty(), new TableCell([new CodeInline("cell")])])]));
        yield return new DocumentBlock(new CodeBlock("line one\nline two", "csharp"));
        yield return new DocumentBlock(new QuoteBlock(3, [new TextInline("quote")]));
        yield return new DocumentBlock(new ImageBlock(
            "image",
            new RemoteImageSource(new Uri("https://example.com/image.png")),
            new ImageSize(120, 60)));
    }

    private static DocumentBlock Paragraph(string text) => new(ParagraphBlock.FromText(text));
}
