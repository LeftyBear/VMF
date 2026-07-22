using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class GeneratedBlockIdGeneratorTests
{
    private readonly GeneratedBlockIdGenerator generator = new();

    [Fact]
    public void Generate_SameInputProducesSameIdentifiers()
    {
        var document = Document("alpha", "beta");

        Assert.Equal(generator.Generate(document), generator.Generate(document));
    }

    [Fact]
    public void Generate_FrontInsertionDoesNotRenumberDifferentExistingContent()
    {
        AssertStableExisting(
            Document("alpha", "beta"),
            Document("new", "alpha", "beta"),
            "alpha",
            "beta");
    }

    [Fact]
    public void Generate_TailInsertionDoesNotChangeExistingIdentifiers()
    {
        AssertStableExisting(
            Document("alpha", "beta"),
            Document("alpha", "beta", "new"),
            "alpha",
            "beta");
    }

    [Fact]
    public void Generate_DeletionDoesNotRenumberDifferentRemainingContent()
    {
        AssertStableExisting(
            Document("alpha", "delete", "beta"),
            Document("alpha", "beta"),
            "alpha",
            "beta");
    }

    [Fact]
    public void Generate_DuplicateContentUsesLocalEquivalentOccurrenceWithoutCollision()
    {
        var identifiers = generator.Generate(Document("same", "same", "same"));

        Assert.Equal(3, identifiers.Distinct(StringComparer.Ordinal).Count());
        Assert.All(identifiers, value => Assert.StartsWith(GeneratedBlockIdGenerator.ValuePrefix, value));
    }

    [Fact]
    public void Generate_BlockKindChangesIdentityFeature()
    {
        var document = new DocumentModel([
            new DocumentBlock(ParagraphBlock.FromText("same")),
            new DocumentBlock(new HeadingBlock(1, [new TextInline("same")])),
        ]);
        var identifiers = generator.Generate(document);

        Assert.NotEqual(identifiers[0], identifiers[1]);
    }

    [Fact]
    public void Generate_ParentHeadingContextChangesIdentifier()
    {
        var document = new DocumentModel([
            Heading("Parent A"),
            Paragraph("same"),
            Heading("Parent B"),
            Paragraph("same"),
        ]);
        var identifiers = generator.Generate(document);

        Assert.NotEqual(identifiers[1], identifiers[3]);
    }

    [Fact]
    public void Generate_AlgorithmVersionChangesPrefixAndIdentifier()
    {
        var document = Document("same");
        var versionOne = generator.Generate(document)[0];
        var versionTwo = new GeneratedBlockIdGenerator("2").Generate(document)[0];

        Assert.StartsWith("gid-v1:sha256:", versionOne);
        Assert.StartsWith("gid-v2:sha256:", versionTwo);
        Assert.NotEqual(versionOne, versionTwo);
    }

    [Fact]
    public void Generate_ReorderingDistinctBlocksPreservesTheirIdentifiers()
    {
        AssertStableExisting(
            Document("alpha", "beta", "gamma"),
            Document("gamma", "alpha", "beta"),
            "alpha",
            "beta",
            "gamma");
    }

    [Fact]
    public void Generate_ExplicitIdSuppressesGeneratedIdAndAnchorsChildren()
    {
        var document = new DocumentModel([
            new DocumentBlock(new HeadingBlock(1, [new TextInline("Parent")]), "parent"),
            Paragraph("child"),
        ]);

        var identifiers = generator.Generate(document);

        Assert.Null(identifiers[0]);
        Assert.NotNull(identifiers[1]);
    }

    private void AssertStableExisting(
        DocumentModel baseline,
        DocumentModel candidate,
        params string[] texts)
    {
        var baselineByText = ByText(baseline, generator.Generate(baseline));
        var candidateByText = ByText(candidate, generator.Generate(candidate));
        foreach (var text in texts)
        {
            Assert.Equal(baselineByText[text], candidateByText[text]);
        }
    }

    private static Dictionary<string, string?> ByText(
        DocumentModel document,
        IReadOnlyList<string?> identifiers) => document.Blocks
        .Select((block, index) => (Text: block.Inlines[0].Text, Id: identifiers[index]))
        .ToDictionary(item => item.Text, item => item.Id, StringComparer.Ordinal);

    private static DocumentModel Document(params string[] values) =>
        new(values.Select(Paragraph));

    private static DocumentBlock Paragraph(string value) =>
        new(ParagraphBlock.FromText(value));

    private static DocumentBlock Heading(string value) =>
        new(new HeadingBlock(1, [new TextInline(value)]));
}
