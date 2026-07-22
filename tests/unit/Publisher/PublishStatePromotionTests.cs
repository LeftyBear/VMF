using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class PublishStatePromotionTests
{
    private readonly PublishResultVerifier verifier = new();
    private readonly VerifiedPublishStatePromoter promoter = new();

    [Fact]
    public void VerifyAndPromote_CreatesActiveStateFromCompleteVerification()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);

        var result = verifier.Verify(candidate, plan, Verification(candidate, plan));
        var state = promoter.Promote(null, result);

        Assert.Equal(DocumentState.Active, state.Identity.State);
        Assert.Same(candidate.Fingerprint, state.Fingerprint);
        Assert.Same(candidate.Versions, state.Versions);
        Assert.Equal(candidate.Blocks, state.Blocks);
    }

    [Fact]
    public void VerifiedResultAndState_HaveNoPublicCandidatePromotionConstructor()
    {
        Assert.Empty(typeof(VerifiedPublishResult).GetConstructors());
        Assert.Empty(typeof(VerifiedPublishState).GetConstructors());
        Assert.DoesNotContain(
            typeof(IVerifiedPublishStatePromoter).GetMethods(),
            method => method.GetParameters().Any(parameter => parameter.ParameterType == typeof(PublishCandidate)));
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void Verify_RequiresLogicalApplicationAndReadback(bool applied, bool readback)
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var verification = Verification(candidate, plan, applied, readback);

        AssertCode(StateErrorCodes.VerificationRequired, () => verifier.Verify(candidate, plan, verification));
    }

    [Fact]
    public void Verify_RejectsFingerprintMismatch()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var verification = Verification(candidate, plan, fingerprint: Fingerprint('f').Value);

        AssertCode(StateErrorCodes.VerificationMismatch, () => verifier.Verify(candidate, plan, verification));
    }

    [Fact]
    public void Verify_RejectsEvidenceForDifferentLogicalPlan()
    {
        var candidate = Candidate();
        var expectedPlan = new DiffEngine().CreatePlan(null, candidate);
        var differentPlan = new DiffEngine().CreatePlan(null, candidate);
        var verification = Verification(candidate, differentPlan);

        AssertCode(
            StateErrorCodes.VerificationMismatch,
            () => verifier.Verify(candidate, expectedPlan, verification));
    }

    [Fact]
    public void Verify_RejectsBlockCountMismatch()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var verification = Verification(candidate, plan, blocks: candidate.Blocks.Take(1));

        AssertCode(StateErrorCodes.VerificationMismatch, () => verifier.Verify(candidate, plan, verification));
    }

    [Fact]
    public void Verify_RejectsBlockOrderMismatch()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var verification = Verification(candidate, plan, blocks: candidate.Blocks.Reverse());

        AssertCode(StateErrorCodes.VerificationMismatch, () => verifier.Verify(candidate, plan, verification));
    }

    [Theory]
    [InlineData("other", null, null)]
    [InlineData(null, "other", null)]
    [InlineData(null, null, "other")]
    public void Verify_RejectsDocumentIdentityMismatch(
        string? publicationId,
        string? documentId,
        string? googleDocumentId)
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var identity = new DocumentIdentity(
            publicationId ?? candidate.Identity.PublicationId,
            documentId ?? candidate.Identity.DocumentId,
            googleDocumentId ?? candidate.Identity.GoogleDocumentId,
            DocumentState.Active);
        var verification = Verification(candidate, plan, identity: identity);

        AssertCode(StateErrorCodes.DocumentIdentityMismatch, () => verifier.Verify(candidate, plan, verification));
    }

    [Fact]
    public void Verify_RejectsExplicitIdentityMismatch()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var blocks = candidate.Blocks.ToArray();
        blocks[0] = new BlockIdentity("other", null, blocks[0].ContentHash);

        AssertCode(
            StateErrorCodes.VerificationMismatch,
            () => verifier.Verify(candidate, plan, Verification(candidate, plan, blocks: blocks)));
    }

    [Fact]
    public void Verify_RejectsGeneratedIdentityMismatch()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var blocks = candidate.Blocks.ToArray();
        blocks[1] = new BlockIdentity(null, Generated('f'), blocks[1].ContentHash);

        AssertCode(
            StateErrorCodes.VerificationMismatch,
            () => verifier.Verify(candidate, plan, Verification(candidate, plan, blocks: blocks)));
    }

    [Fact]
    public void Verify_RejectsContentHashMismatch()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(null, candidate);
        var blocks = candidate.Blocks.ToArray();
        blocks[0] = new BlockIdentity(blocks[0].ExplicitId, null, Content('f'));

        AssertCode(
            StateErrorCodes.VerificationMismatch,
            () => verifier.Verify(candidate, plan, Verification(candidate, plan, blocks: blocks)));
    }

    [Theory]
    [InlineData(null, DocumentState.Active, true)]
    [InlineData(null, DocumentState.Missing, false)]
    [InlineData(null, DocumentState.Archived, false)]
    [InlineData(DocumentState.Active, DocumentState.Active, true)]
    [InlineData(DocumentState.Active, DocumentState.Missing, true)]
    [InlineData(DocumentState.Active, DocumentState.Archived, true)]
    [InlineData(DocumentState.Missing, DocumentState.Active, true)]
    [InlineData(DocumentState.Missing, DocumentState.Missing, true)]
    [InlineData(DocumentState.Missing, DocumentState.Archived, true)]
    [InlineData(DocumentState.Archived, DocumentState.Active, false)]
    [InlineData(DocumentState.Archived, DocumentState.Missing, false)]
    [InlineData(DocumentState.Archived, DocumentState.Archived, true)]
    public void TransitionRules_DefineCompletePersistentStateMatrix(
        DocumentState? current,
        DocumentState next,
        bool expected)
    {
        Assert.Equal(expected, DocumentStateTransitionRules.IsAllowed(current, next));
    }

    [Theory]
    [InlineData(DocumentState.Active, DocumentState.Active)]
    [InlineData(DocumentState.Active, DocumentState.Missing)]
    [InlineData(DocumentState.Active, DocumentState.Archived)]
    [InlineData(DocumentState.Missing, DocumentState.Active)]
    [InlineData(DocumentState.Missing, DocumentState.Missing)]
    [InlineData(DocumentState.Missing, DocumentState.Archived)]
    [InlineData(DocumentState.Archived, DocumentState.Archived)]
    public void Transition_CreatesAllowedStateAndPreservesVerifiedPayload(
        DocumentState current,
        DocumentState next)
    {
        var baseline = State(current);

        var result = new VerifiedPublishStateTransitioner().Transition(baseline, next);

        Assert.Equal(next, result.Identity.State);
        Assert.Same(baseline.Fingerprint, result.Fingerprint);
        Assert.Same(baseline.Versions, result.Versions);
        Assert.Equal(baseline.Blocks, result.Blocks);
    }

    [Theory]
    [InlineData(DocumentState.Archived, DocumentState.Active)]
    [InlineData(DocumentState.Archived, DocumentState.Missing)]
    public void Transition_RejectsArchivedRecoveryWithoutDedicatedWorkflow(
        DocumentState current,
        DocumentState next)
    {
        AssertCode(
            StateErrorCodes.InvalidTransition,
            () => new VerifiedPublishStateTransitioner().Transition(State(current), next));
    }

    [Fact]
    public void Promote_RejectsArchivedBaselineRecovery()
    {
        var candidate = Candidate();
        var plan = new DiffEngine().CreatePlan(State(DocumentState.Archived), candidate);
        var verified = verifier.Verify(candidate, plan, Verification(candidate, plan));

        AssertCode(
            StateErrorCodes.InvalidTransition,
            () => promoter.Promote(State(DocumentState.Archived), verified));
    }

    [Fact]
    public void Promote_AllowsMissingBaselineRecoveryAfterVerification()
    {
        var candidate = Candidate();
        var baseline = State(DocumentState.Missing);
        var plan = new DiffEngine().CreatePlan(baseline, candidate);
        var verified = verifier.Verify(candidate, plan, Verification(candidate, plan));

        var state = promoter.Promote(baseline, verified);

        Assert.Equal(DocumentState.Active, state.Identity.State);
    }

    private static PublishCandidate Candidate() => new(
        Identity(DocumentState.Active),
        Versions(),
        Fingerprint('a'),
        Blocks());

    private static VerifiedPublishState State(DocumentState state) => new(
        Identity(state),
        Versions(),
        Revision(),
        Fingerprint('b'),
        Blocks());

    private static DocumentIdentity Identity(DocumentState state) =>
        new("publication", "document", "google-document", state);

    private static PublishStateVersions Versions() => new("2", "1", "1", "1", "1.0", "1.0.0");

    private static DocumentRevision Revision() => new("revision-1", 1);

    private static PublishFingerprint Fingerprint(char digit) =>
        new("v1:sha256:" + new string(digit, 64));

    private static BlockIdentity[] Blocks() =>
    [
        new BlockIdentity("intro", null, Content('c')),
        new BlockIdentity(null, Generated('d'), Content('e')),
    ];

    private static string Content(char digit) => "ch-v1:sha256:" + new string(digit, 64);

    private static string Generated(char digit) => "gid-v1:sha256:" + new string(digit, 64);

    private static PublishApplicationVerification Verification(
        PublishCandidate candidate,
        DiffPlan plan,
        bool applied = true,
        bool readback = true,
        string? fingerprint = null,
        IEnumerable<BlockIdentity>? blocks = null,
        DocumentIdentity? identity = null) => new(
            identity ?? candidate.Identity,
            plan,
            applied,
            readback,
            fingerprint ?? candidate.Fingerprint.Value,
            blocks ?? candidate.Blocks,
            Revision());

    private static void AssertCode(string code, Action action)
    {
        var exception = Assert.Throws<StateLifecycleException>(action);
        Assert.Equal(code, exception.Code);
    }
}
