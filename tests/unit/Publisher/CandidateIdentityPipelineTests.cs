using Vmf.Publisher.Application;
using Vmf.Publisher.Composition;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class CandidateIdentityPipelineTests
{
    [Fact]
    public void Create_FromMarkdownBuildsExplicitGeneratedAndContentHashIdentity()
    {
        var document = Parse(
            "<!-- vmf:block-id=intro -->\n# Heading\n\n" +
            "Paragraph.\n");

        var candidate = Builder().Create(Identity(), document, Options());

        Assert.Equal("intro", candidate.Blocks[0].ExplicitId);
        Assert.Null(candidate.Blocks[0].GeneratedId);
        Assert.Null(candidate.Blocks[1].ExplicitId);
        Assert.StartsWith(GeneratedBlockIdGenerator.ValuePrefix, candidate.Blocks[1].GeneratedId);
        Assert.All(candidate.Blocks, block =>
            Assert.StartsWith(BlockContentHashGenerator.ValuePrefix, block.ContentHash));
        Assert.StartsWith(PublishFingerprintGenerator.ValuePrefix, candidate.Fingerprint.Value);
        Assert.Equal("1", candidate.Versions.SchemaVersion);
        Assert.Equal(GeneratedBlockIdGenerator.AlgorithmVersion, candidate.Versions.GeneratedIdAlgorithmVersion);
        Assert.Equal(BlockContentHashGenerator.AlgorithmVersion, candidate.Versions.ContentHashAlgorithmVersion);
        Assert.Equal(PublishFingerprintGenerator.AlgorithmVersion, candidate.Versions.FingerprintAlgorithmVersion);
        Assert.Equal("1.0", candidate.Versions.TransformationSpecificationVersion);
        Assert.Equal("1.0.0", candidate.Versions.PublisherVersion);
    }

    [Fact]
    public void Create_ExplicitIdIsAuthoritativeAndGeneratedIdIsFallbackOnly()
    {
        var document = Parse(
            "<!-- vmf:block-id=explicit -->\nExplicit paragraph.\n\n" +
            "Generated paragraph.\n");

        var candidate = Builder().Create(Identity(), document, Options());

        Assert.Equal("explicit", candidate.Blocks[0].ExplicitId);
        Assert.Null(candidate.Blocks[0].GeneratedId);
        Assert.Null(candidate.Blocks[1].ExplicitId);
        Assert.NotNull(candidate.Blocks[1].GeneratedId);
    }

    [Fact]
    public void Create_RejectsDuplicateExplicitIdsInCanonicalModel()
    {
        var document = new DocumentModel([
            new DocumentBlock(ParagraphBlock.FromText("one"), "same"),
            new DocumentBlock(ParagraphBlock.FromText("two"), "same"),
        ]);

        var exception = Assert.Throws<PublishPipelineException>(() =>
            Builder().Create(Identity(), document, Options()));

        Assert.Equal(PublishErrorCodes.BlockExplicitIdDuplicate, exception.Code);
    }

    [Fact]
    public void Create_SameMarkdownAndInputsProduceSameCandidateState()
    {
        var document = Parse("# Heading\n\nParagraph.\n");
        var builder = Builder();

        var first = builder.Create(Identity(), document, Options());
        var second = builder.Create(Identity(), document, Options());

        Assert.Equal(first.Fingerprint, second.Fingerprint);
        Assert.Equal(
            first.Blocks.Select(Signature),
            second.Blocks.Select(Signature));
    }

    [Fact]
    public void Create_FrontInsertionProducesOneInsertAndExistingNoChange()
    {
        var builder = Builder();
        var identity = Identity();
        var baselineCandidate = builder.Create(
            identity,
            Parse("Alpha.\n\nBeta.\n"),
            Options());
        var candidate = builder.Create(
            identity,
            Parse("New.\n\nAlpha.\n\nBeta.\n"),
            Options());
        var baseline = new VerifiedPublishState(
            baselineCandidate.Identity,
            baselineCandidate.Versions,
            baselineCandidate.Fingerprint,
            baselineCandidate.Blocks);

        var plan = new DiffEngine().CreatePlan(baseline, candidate);

        Assert.Collection(
            plan.Operations,
            operation => Assert.Equal(DiffOperationKind.Insert, operation.Kind),
            operation => Assert.Equal(DiffOperationKind.NoChange, operation.Kind),
            operation => Assert.Equal(DiffOperationKind.NoChange, operation.Kind));
        Assert.Equal(
            baseline.Blocks.Select(block => block.GeneratedId),
            candidate.Blocks.Skip(1).Select(block => block.GeneratedId));
    }

    [Fact]
    public void CompositionRoot_RegistersCompleteCandidatePipeline()
    {
        var candidate = PublisherCompositionRoot.CreatePublishCandidateBuilder().Create(
            Identity(),
            Parse("Paragraph.\n"),
            Options());

        Assert.Single(candidate.Blocks);
    }

    private static string Signature(BlockIdentity identity) => string.Join(
        ":",
        identity.ExplicitId ?? "-",
        identity.GeneratedId ?? "-",
        identity.ContentHash);

    private static IPublishCandidateBuilder Builder() =>
        PublisherCompositionRoot.CreatePublishCandidateBuilder();

    private static DocumentModel Parse(string markdown) =>
        new SimpleMarkdownParser().Parse(markdown);

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private static PublishCandidateBuildOptions Options() => new(
        "1.0.0",
        "1.0",
        "1",
        [
            new(PublishFingerprintSettingNames.MarkdownInlineMaxDepth, "8"),
            new(PublishFingerprintSettingNames.MarkdownListIndentSize, "2"),
            new(PublishFingerprintSettingNames.MarkdownListMaxDepth, "6"),
            new(PublishFingerprintSettingNames.PublisherAllowImageUpscale, "false"),
            new(PublishFingerprintSettingNames.PublisherImageMaxWidthPoints, "450"),
        ]);
}
