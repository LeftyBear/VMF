using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Contains target-neutral evidence returned after plan application and readback.</summary>
public sealed class PublishApplicationVerification
{
    /// <summary>Initializes verification evidence supplied by a future physical application adapter.</summary>
    public PublishApplicationVerification(
        DocumentIdentity appliedIdentity,
        DiffPlan appliedPlan,
        bool isLogicalPlanApplied,
        bool isReadbackVerified,
        string appliedFingerprint,
        IEnumerable<BlockIdentity> appliedBlocks,
        DocumentRevision appliedRevision)
    {
        AppliedIdentity = appliedIdentity ?? throw new ArgumentNullException(nameof(appliedIdentity));
        AppliedPlan = appliedPlan ?? throw new ArgumentNullException(nameof(appliedPlan));
        AppliedFingerprint = appliedFingerprint
            ?? throw new ArgumentNullException(nameof(appliedFingerprint));
        AppliedRevision = appliedRevision ?? throw new ArgumentNullException(nameof(appliedRevision));
        ArgumentNullException.ThrowIfNull(appliedBlocks);

        var blocks = appliedBlocks.ToArray();
        if (blocks.Any(block => block is null))
        {
            throw new ArgumentException("Applied blocks must not contain null items.", nameof(appliedBlocks));
        }

        IsLogicalPlanApplied = isLogicalPlanApplied;
        IsReadbackVerified = isReadbackVerified;
        AppliedBlocks = Array.AsReadOnly(blocks);
    }

    /// <summary>Gets the read-back document identity.</summary>
    public DocumentIdentity AppliedIdentity { get; }

    /// <summary>Gets the logical plan reported as applied.</summary>
    public DiffPlan AppliedPlan { get; }

    /// <summary>Gets a value indicating whether logical plan application completed.</summary>
    public bool IsLogicalPlanApplied { get; }

    /// <summary>Gets a value indicating whether readback or equivalent verification completed.</summary>
    public bool IsReadbackVerified { get; }

    /// <summary>Gets the applied canonical fingerprint value.</summary>
    public string AppliedFingerprint { get; }

    /// <summary>Gets the applied blocks in read-back order.</summary>
    public IReadOnlyList<BlockIdentity> AppliedBlocks { get; }

    /// <summary>Gets the revision that was read back with the verified payload.</summary>
    public DocumentRevision AppliedRevision { get; }
}

/// <summary>Represents evidence accepted by the Publisher verification boundary.</summary>
public sealed class VerifiedPublishResult
{
    internal VerifiedPublishResult(PublishCandidate candidate, DocumentRevision revision)
    {
        Candidate = candidate;
        Revision = revision;
    }

    internal PublishCandidate Candidate { get; }

    internal DocumentRevision Revision { get; }
}

/// <summary>Validates application evidence against a candidate.</summary>
public interface IPublishResultVerifier
{
    /// <summary>Validates all identity, fingerprint, block, and order invariants.</summary>
    VerifiedPublishResult Verify(
        PublishCandidate candidate,
        DiffPlan expectedPlan,
        PublishApplicationVerification verification);
}

/// <summary>Promotes only an accepted verification result to persisted state.</summary>
public interface IVerifiedPublishStatePromoter
{
    /// <summary>Creates active verified state after checking the baseline transition.</summary>
    VerifiedPublishState Promote(
        VerifiedPublishState? baseline,
        VerifiedPublishResult verifiedResult);
}

/// <summary>Transitions an existing verified state without changing its verified payload.</summary>
public interface IVerifiedPublishStateTransitioner
{
    /// <summary>Creates a new verified state for an allowed persisted-state transition.</summary>
    VerifiedPublishState Transition(VerifiedPublishState current, DocumentState next);
}
