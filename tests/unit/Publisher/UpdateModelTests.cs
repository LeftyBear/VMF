using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class UpdateModelTests
{
    [Fact]
    public void PublishState_PreservesIdentityFingerprintAndBlockOrder()
    {
        var source = new List<BlockIdentity>
        {
            Block("first", "generated-first", "hash-first"),
            Block("second", "generated-second", "hash-second"),
        };
        var identity = Identity(DocumentState.Existing);
        var fingerprint = new PublishFingerprint("fingerprint");

        var state = new PublishState(identity, fingerprint, source);
        source.Clear();

        Assert.Same(identity, state.Identity);
        Assert.Same(fingerprint, state.Fingerprint);
        Assert.Equal(["first", "second"], state.Blocks.Select(block => block.ExplicitId));
    }

    [Fact]
    public void PublishState_RejectsDuplicateExplicitAndGeneratedIdentifiers()
    {
        Assert.Throws<ArgumentException>(() => new PublishState(
            Identity(DocumentState.Existing),
            new PublishFingerprint("fingerprint"),
            [Block("duplicate", "generated-one", "hash-one"),
             Block("duplicate", "generated-two", "hash-two")]));

        Assert.Throws<ArgumentException>(() => new PublishState(
            Identity(DocumentState.Existing),
            new PublishFingerprint("fingerprint"),
            [Block("one", "duplicate", "hash-one"),
             Block("two", "duplicate", "hash-two")]));
    }

    [Fact]
    public void PublishFingerprint_UsesOrdinalValueEquality()
    {
        var first = new PublishFingerprint("ABC");
        var same = new PublishFingerprint("ABC");
        var differentCase = new PublishFingerprint("abc");

        Assert.Equal(first, same);
        Assert.NotEqual(first, differentCase);
        Assert.Equal("ABC", first.ToString());
    }

    [Theory]
    [InlineData(null, "generated", "hash")]
    [InlineData("explicit", null, "hash")]
    [InlineData(null, null, "hash")]
    public void BlockIdentity_AllowsIdentityFallbacks(
        string? explicitId,
        string? generatedId,
        string contentHash)
    {
        var identity = new BlockIdentity(explicitId, generatedId, contentHash);

        Assert.Equal(explicitId, identity.ExplicitId);
        Assert.Equal(generatedId, identity.GeneratedId);
        Assert.Equal(contentHash, identity.ContentHash);
    }

    private static DocumentIdentity Identity(DocumentState state) =>
        new("publication", "document", "google-document", state);

    private static BlockIdentity Block(string? explicitId, string? generatedId, string hash) =>
        new(explicitId, generatedId, hash);
}
