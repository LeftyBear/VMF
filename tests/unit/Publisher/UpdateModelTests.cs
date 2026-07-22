using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class UpdateModelTests
{
    [Fact]
    public void PublishCandidate_PreservesIdentityFingerprintAndBlockOrder()
    {
        var source = new List<BlockIdentity>
        {
            Block("first", "generated-first", "hash-first"),
            Block("second", "generated-second", "hash-second"),
        };
        var identity = Identity(DocumentState.Active);
        var fingerprint = Fingerprint('a');

        var candidate = new PublishCandidate(identity, Versions(), fingerprint, source);
        source.Clear();

        Assert.Same(identity, candidate.Identity);
        Assert.Same(fingerprint, candidate.Fingerprint);
        Assert.Equal(["first", "second"], candidate.Blocks.Select(block => block.ExplicitId));
    }

    [Fact]
    public void VerifiedPublishState_CannotBeConstructedOrReplacedByExternalCandidate()
    {
        Assert.Empty(typeof(VerifiedPublishState).GetConstructors());
        Assert.False(typeof(VerifiedPublishState).IsAssignableFrom(typeof(PublishCandidate)));
    }

    [Fact]
    public void VerifiedPublishState_PreservesRestoredValues()
    {
        var identity = Identity(DocumentState.Active);
        var fingerprint = Fingerprint('b');
        var block = Block("explicit", "generated", "hash");

        var baseline = new VerifiedPublishState(identity, Versions(), fingerprint, [block]);

        Assert.Same(identity, baseline.Identity);
        Assert.Same(fingerprint, baseline.Fingerprint);
        Assert.Same(block, Assert.Single(baseline.Blocks));
    }

    [Fact]
    public void PublishFingerprint_UsesValueEquality()
    {
        var first = Fingerprint('a');
        var same = Fingerprint('a');
        var different = Fingerprint('b');

        Assert.Equal(first, same);
        Assert.NotEqual(first, different);
        Assert.Equal("v1:sha256:" + new string('a', 64), first.ToString());
    }

    [Fact]
    public void PublishFingerprint_CannotBeConstructedByExternalCaller()
    {
        Assert.Empty(typeof(PublishFingerprint).GetConstructors());
    }

    [Theory]
    [InlineData("fingerprint")]
    [InlineData("v1:sha256:ABCDEF")]
    [InlineData("v1:sha256:ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789")]
    public void PublishFingerprint_RejectsNonCanonicalValues(string value)
    {
        Assert.Throws<ArgumentException>(() => new PublishFingerprint(value));
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

    private static PublishFingerprint Fingerprint(char hexadecimalDigit) =>
        new("v1:sha256:" + new string(hexadecimalDigit, 64));

    private static PublishStateVersions Versions() => new("1", "1", "1", "1", "1.0", "test");
}
