using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;

namespace Vmf.Publisher.UnitTests;

public sealed class PublishCandidateFactoryTests
{
    private readonly PublishFingerprintGenerator fingerprintGenerator = new();

    [Fact]
    public void Create_BindsIdentityBlocksAndFingerprintFromOneCanonicalInput()
    {
        var identity = Identity("publication", "document", "google-document");
        var block = new BlockIdentity("explicit", "generated", "content-hash");
        var input = Input("publication", "document", block);
        var factory = new PublishCandidateFactory(fingerprintGenerator);

        var candidate = factory.Create(identity, input);

        Assert.Same(identity, candidate.Identity);
        Assert.Same(block, Assert.Single(candidate.Blocks));
        Assert.Equal(fingerprintGenerator.Generate(input), candidate.Fingerprint);
    }

    [Theory]
    [InlineData("other-publication", "document")]
    [InlineData("publication", "other-document")]
    [InlineData("Publication", "document")]
    public void Create_RejectsCanonicalInputForDifferentIdentity(
        string publicationId,
        string documentId)
    {
        var factory = new PublishCandidateFactory(fingerprintGenerator);

        Assert.Throws<ArgumentException>(() => factory.Create(
            Identity("publication", "document", "google-document"),
            Input(publicationId, documentId, new BlockIdentity("explicit", null, "hash"))));
    }

    [Fact]
    public void PublishCandidate_CannotBeConstructedByExternalCaller()
    {
        Assert.Empty(typeof(PublishCandidate).GetConstructors());
    }

    private static DocumentIdentity Identity(
        string publicationId,
        string documentId,
        string? googleDocumentId) =>
        new(publicationId, documentId, googleDocumentId, DocumentState.Existing);

    private static PublishFingerprintInput Input(
        string publicationId,
        string documentId,
        BlockIdentity block) =>
        new(
            publicationId,
            documentId,
            new DocumentModel([new DocumentBlock(ParagraphBlock.FromText("text"))]),
            [block],
            "1.0.0",
            "1.0",
            "1",
            RequiredSettings());

    private static IReadOnlyList<PublishFingerprintSetting> RequiredSettings() =>
    [
        new(PublishFingerprintSettingNames.MarkdownInlineMaxDepth, "8"),
        new(PublishFingerprintSettingNames.MarkdownListIndentSize, "2"),
        new(PublishFingerprintSettingNames.MarkdownListMaxDepth, "6"),
        new(PublishFingerprintSettingNames.PublisherAllowImageUpscale, "false"),
        new(PublishFingerprintSettingNames.PublisherImageMaxWidthPoints, "450"),
    ];
}
