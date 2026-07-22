using System.Text.RegularExpressions;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class PublishFingerprintGeneratorTests
{
    private readonly PublishFingerprintGenerator generator = new();

    [Fact]
    public void Generate_ProducesVersionedLowercaseSha256KnownVector()
    {
        var fingerprint = generator.Generate(BaseInput("日本語\r\ntext"));

        Assert.Matches(
            new Regex("^v1:sha256:[0-9a-f]{64}$", RegexOptions.CultureInvariant),
            fingerprint.Value);
        Assert.Equal(
            "v1:sha256:fb87ab10ce186175f068c0db56c1fec18e7251982471677e406a0ca3a0f4de9a",
            fingerprint.Value);
    }

    [Fact]
    public void Generate_SortsOutputSettingsByOrdinalName()
    {
        var first = BaseInput(
            "text",
            settings:
            [
                new PublishFingerprintSetting("z.setting", "last"),
                new PublishFingerprintSetting("A.setting", "first"),
            ]);
        var second = BaseInput(
            "text",
            settings:
            [
                new PublishFingerprintSetting("A.setting", "first"),
                new PublishFingerprintSetting("z.setting", "last"),
            ]);

        Assert.Equal(generator.Generate(first), generator.Generate(second));
    }

    [Fact]
    public void Generate_DistinguishesNullAndEmptySettingValues()
    {
        var nullValue = BaseInput(
            "text",
            settings: [new PublishFingerprintSetting("setting", null)]);
        var emptyValue = BaseInput(
            "text",
            settings: [new PublishFingerprintSetting("setting", string.Empty)]);

        Assert.NotEqual(generator.Generate(nullValue), generator.Generate(emptyValue));
    }

    [Fact]
    public void Generate_NormalizesAllLineEndingsToLfBeforeUtf8Hashing()
    {
        var lf = generator.Generate(BaseInput("line one\nline two"));
        var crlf = generator.Generate(BaseInput("line one\r\nline two"));
        var cr = generator.Generate(BaseInput("line one\rline two"));

        Assert.Equal(lf, crlf);
        Assert.Equal(lf, cr);
    }

    [Fact]
    public void Generate_IncludesPublicationAndDocumentIdentityWithoutGoogleDocumentId()
    {
        var baseline = generator.Generate(BaseInput("text"));
        var changedPublication = generator.Generate(BaseInput("text", publicationId: "other"));
        var changedDocument = generator.Generate(BaseInput("text", documentId: "other"));

        Assert.NotEqual(baseline, changedPublication);
        Assert.NotEqual(baseline, changedDocument);
        Assert.Null(typeof(PublishFingerprintInput).GetProperty("GoogleDocumentId"));
    }

    [Fact]
    public void Generate_IncludesOrderedCanonicalDocumentModel()
    {
        var firstBlock = new DocumentBlock(ParagraphBlock.FromText("first"));
        var secondBlock = new DocumentBlock(ParagraphBlock.FromText("second"));
        var firstIdentity = new BlockIdentity("first", null, "hash-first");
        var secondIdentity = new BlockIdentity("second", null, "hash-second");
        var first = Input(
            new DocumentModel([firstBlock, secondBlock]),
            [firstIdentity, secondIdentity]);
        var reversed = Input(
            new DocumentModel([secondBlock, firstBlock]),
            [secondIdentity, firstIdentity]);
        var changedText = Input(
            new DocumentModel([
                new DocumentBlock(ParagraphBlock.FromText("changed")),
                secondBlock,
            ]),
            [firstIdentity, secondIdentity]);

        Assert.NotEqual(generator.Generate(first), generator.Generate(reversed));
        Assert.NotEqual(generator.Generate(first), generator.Generate(changedText));
    }

    [Fact]
    public void Generate_IncludesEveryBlockIdentityTierAndContentHash()
    {
        var document = new DocumentModel([new DocumentBlock(ParagraphBlock.FromText("text"))]);
        var baseline = generator.Generate(Input(
            document,
            [new BlockIdentity("explicit", "generated", "hash")]));
        var changedExplicit = generator.Generate(Input(
            document,
            [new BlockIdentity("other", "generated", "hash")]));
        var changedGenerated = generator.Generate(Input(
            document,
            [new BlockIdentity("explicit", "other", "hash")]));
        var changedHash = generator.Generate(Input(
            document,
            [new BlockIdentity("explicit", "generated", "other")]));

        Assert.NotEqual(baseline, changedExplicit);
        Assert.NotEqual(baseline, changedGenerated);
        Assert.NotEqual(baseline, changedHash);
    }

    [Fact]
    public void Generate_IncludesEveryVersionAndOutputAffectingSetting()
    {
        var baseline = generator.Generate(BaseInput("text"));
        var publisherVersion = generator.Generate(BaseInput("text", publisherVersion: "2.0.0"));
        var specificationVersion = generator.Generate(BaseInput(
            "text",
            transformationSpecificationVersion: "2.0"));
        var schemaVersion = generator.Generate(BaseInput("text", publishStateSchemaVersion: "2"));
        var setting = generator.Generate(BaseInput(
            "text",
            settings: [new PublishFingerprintSetting("publisher.imageMaxWidthPoints", "451")]));

        Assert.NotEqual(baseline, publisherVersion);
        Assert.NotEqual(baseline, specificationVersion);
        Assert.NotEqual(baseline, schemaVersion);
        Assert.NotEqual(baseline, setting);
    }

    [Fact]
    public void Generate_SerializesEverySupportedDocumentAndInlineKind()
    {
        var document = new DocumentModel([
            new DocumentBlock(new HeadingBlock(2, [
                new BoldInline([
                    new ItalicInline([
                        new LinkInline([new CodeInline("code")], new Uri("https://example.com/path")),
                    ]),
                ]),
            ])),
            new DocumentBlock(ParagraphBlock.FromText("paragraph")),
            new DocumentBlock(DocumentBlockKind.BulletListItem, [new TextInline("legacy bullet")]),
            new DocumentBlock(new ListBlock([
                new ListItem(ListKind.Unordered, [new TextInline("root")], 0),
                new ListItem(ListKind.Ordered, [new TextInline("nested")], 1),
            ])),
            new DocumentBlock(new TableBlock(
                [new TableColumn(TableAlignment.Left), new TableColumn(TableAlignment.Right)],
                new TableRow([
                    new TableCell([new TextInline("header")]),
                    new TableCell([new TextInline("value")]),
                ]),
                [new TableRow([
                    TableCell.Empty(),
                    new TableCell([new CodeInline("cell")]),
                ])])),
            new DocumentBlock(new CodeBlock("line one\nline two", "csharp")),
            new DocumentBlock(new QuoteBlock(3, [new TextInline("quote")])),
            new DocumentBlock(new ImageBlock(
                "image",
                new RemoteImageSource(new Uri("https://example.com/image.png")),
                new ImageSize(120, 60))),
        ]);
        var blocks = Enumerable.Range(0, document.Blocks.Count)
            .Select(index => new BlockIdentity($"explicit-{index}", $"generated-{index}", $"hash-{index}"))
            .ToArray();

        var fingerprint = generator.Generate(Input(document, blocks));

        Assert.StartsWith(PublishFingerprintGenerator.ValuePrefix, fingerprint.Value, StringComparison.Ordinal);
    }

    [Fact]
    public void Input_RequiresOneIdentityPerDocumentBlock()
    {
        var document = new DocumentModel([new DocumentBlock(ParagraphBlock.FromText("text"))]);

        Assert.Throws<ArgumentException>(() => Input(document, []));
    }

    [Fact]
    public void Input_RejectsDuplicateSettingNamesUsingOrdinalComparison()
    {
        Assert.Throws<ArgumentException>(() => BaseInput(
            "text",
            settings:
            [
                new PublishFingerprintSetting("same", "one"),
                new PublishFingerprintSetting("same", "two"),
            ]));

        var differentCase = BaseInput(
            "text",
            settings:
            [
                new PublishFingerprintSetting("Setting", "one"),
                new PublishFingerprintSetting("setting", "two"),
            ]);
        Assert.Equal(PublishFingerprintSettingNames.Required.Count + 2, differentCase.OutputSettings.Count);
    }

    [Fact]
    public void Input_RequiresEveryCurrentOutputAffectingSetting()
    {
        var document = new DocumentModel([new DocumentBlock(ParagraphBlock.FromText("text"))]);

        Assert.Throws<ArgumentException>(() => new PublishFingerprintInput(
            "publication",
            "document",
            document,
            [new BlockIdentity("explicit", null, "hash")],
            "1.0.0",
            "1.0",
            "1",
            []));
    }

    private static PublishFingerprintInput BaseInput(
        string text,
        string publicationId = "publication",
        string documentId = "document",
        string publisherVersion = "1.0.0",
        string transformationSpecificationVersion = "1.0",
        string publishStateSchemaVersion = "1",
        IEnumerable<PublishFingerprintSetting>? settings = null)
    {
        var document = new DocumentModel([new DocumentBlock(ParagraphBlock.FromText(text))]);
        return Input(
            document,
            [new BlockIdentity("explicit", "generated", "content-hash")],
            publicationId,
            documentId,
            publisherVersion,
            transformationSpecificationVersion,
            publishStateSchemaVersion,
            settings);
    }

    private static PublishFingerprintInput Input(
        DocumentModel document,
        BlockIdentity[] blocks,
        string publicationId = "publication",
        string documentId = "document",
        string publisherVersion = "1.0.0",
        string transformationSpecificationVersion = "1.0",
        string publishStateSchemaVersion = "1",
        IEnumerable<PublishFingerprintSetting>? settings = null) =>
        new(
            publicationId,
            documentId,
            document,
            blocks,
            publisherVersion,
            transformationSpecificationVersion,
            publishStateSchemaVersion,
            CompleteSettings(settings));

    private static IReadOnlyList<PublishFingerprintSetting> CompleteSettings(
        IEnumerable<PublishFingerprintSetting>? settings)
    {
        var items = settings?.ToList() ?? [];
        AddDefaultIfMissing(items, PublishFingerprintSettingNames.MarkdownInlineMaxDepth, "8");
        AddDefaultIfMissing(items, PublishFingerprintSettingNames.MarkdownListIndentSize, "2");
        AddDefaultIfMissing(items, PublishFingerprintSettingNames.MarkdownListMaxDepth, "6");
        AddDefaultIfMissing(items, PublishFingerprintSettingNames.PublisherAllowImageUpscale, "false");
        AddDefaultIfMissing(items, PublishFingerprintSettingNames.PublisherImageMaxWidthPoints, "450");
        return items;
    }

    private static void AddDefaultIfMissing(
        ICollection<PublishFingerprintSetting> settings,
        string name,
        string value)
    {
        if (!settings.Any(setting => string.Equals(setting.Name, name, StringComparison.Ordinal)))
        {
            settings.Add(new PublishFingerprintSetting(name, value));
        }
    }
}
