using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class MarkdownExplicitIdTests
{
    [Fact]
    public void Parse_StoresValidExplicitIdOnFollowingCanonicalBlock()
    {
        var document = Parse("<!-- vmf:block-id=intro.section -->\n# Heading\n");

        Assert.Equal("intro.section", Assert.Single(document.Blocks).ExplicitId);
    }

    [Fact]
    public void Parse_LeavesExplicitIdNullWhenDirectiveIsAbsent()
    {
        var document = Parse("Paragraph.\n");

        Assert.Null(Assert.Single(document.Blocks).ExplicitId);
    }

    [Fact]
    public void Parse_RejectsEmptyExplicitId()
    {
        AssertError(
            PublishErrorCodes.MarkdownExplicitIdEmpty,
            "<!-- vmf:block-id= -->\nParagraph.\n");
    }

    [Fact]
    public void Parse_RejectsDuplicateExplicitIdWithinDocument()
    {
        AssertError(
            PublishErrorCodes.BlockExplicitIdDuplicate,
            "<!-- vmf:block-id=same -->\n# One\n\n" +
            "<!-- vmf:block-id=same -->\nTwo\n");
    }

    [Fact]
    public void Parse_UsesOrdinalCaseSensitiveExplicitIds()
    {
        var document = Parse(
            "<!-- vmf:block-id=Example -->\n# One\n\n" +
            "<!-- vmf:block-id=example -->\nTwo\n");

        Assert.Equal(["Example", "example"], document.Blocks.Select(block => block.ExplicitId));
    }

    [Fact]
    public void Parse_AcceptsAndNfcNormalizesUnicodeExplicitId()
    {
        var document = Parse("<!-- vmf:block-id=見出し.e\u0301 -->\n# Unicode\n");

        Assert.Equal("見出し.é", Assert.Single(document.Blocks).ExplicitId);
    }

    [Fact]
    public void Parse_PreservesExplicitIdAcrossEverySupportedBlockKind()
    {
        var document = Parse(
            "<!-- vmf:block-id=heading -->\n# Heading\n\n" +
            "<!-- vmf:block-id=paragraph -->\nParagraph.\n\n" +
            "<!-- vmf:block-id=list -->\n- Item\n\n" +
            "<!-- vmf:block-id=table -->\nA | B\n--- | ---\nC | D\n\n" +
            "<!-- vmf:block-id=code -->\n```text\ncode\n```\n\n" +
            "<!-- vmf:block-id=quote -->\n> Quote\n\n" +
            "<!-- vmf:block-id=image -->\n![Alt](https://example.com/image.png)\n");

        Assert.Equal(
            ["heading", "paragraph", "list", "table", "code", "quote", "image"],
            document.Blocks.Select(block => block.ExplicitId));
    }

    [Theory]
    [InlineData("1starts-with-digit")]
    [InlineData("contains space")]
    [InlineData("contains/slash")]
    [InlineData("-starts-with-hyphen")]
    public void Parse_RejectsInvalidExplicitIdFormat(string value)
    {
        AssertError(
            PublishErrorCodes.MarkdownExplicitIdInvalid,
            $"<!-- vmf:block-id={value} -->\nParagraph.\n");
    }

    [Fact]
    public void Parse_DetectsDuplicateAfterUnicodeNormalization()
    {
        AssertError(
            PublishErrorCodes.BlockExplicitIdDuplicate,
            "<!-- vmf:block-id=é -->\n# One\n\n" +
            "<!-- vmf:block-id=e\u0301 -->\nTwo\n");
    }

    [Fact]
    public void Parse_RejectsMalformedOrOrphanedDirective()
    {
        AssertError(
            PublishErrorCodes.MarkdownExplicitIdInvalid,
            "<!-- vmf:block-id=value\nParagraph.\n");
        AssertError(
            PublishErrorCodes.MarkdownExplicitIdOrphaned,
            "<!-- vmf:block-id=value -->\n");
    }

    [Fact]
    public void DocumentBlock_RejectsInvalidProgrammaticExplicitId()
    {
        Assert.Throws<ArgumentException>(() => new DocumentBlock(
            ParagraphBlock.FromText("text"),
            "invalid value"));
    }

    private static DocumentModel Parse(string markdown) => new SimpleMarkdownParser().Parse(markdown);

    private static void AssertError(string expectedCode, string markdown)
    {
        var exception = Assert.Throws<PublishPipelineException>(() => Parse(markdown));
        Assert.Equal(expectedCode, exception.Code);
    }
}
