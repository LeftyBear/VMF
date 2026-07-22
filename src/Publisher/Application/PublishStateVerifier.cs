using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Validates target-neutral application evidence and controls state promotion.</summary>
public sealed class PublishResultVerifier : IPublishResultVerifier
{
    /// <inheritdoc />
    public VerifiedPublishResult Verify(
        PublishCandidate candidate,
        DiffPlan expectedPlan,
        PublishApplicationVerification verification)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(expectedPlan);
        ArgumentNullException.ThrowIfNull(verification);

        if (!verification.IsLogicalPlanApplied || !verification.IsReadbackVerified)
        {
            throw new StateLifecycleException(
                StateErrorCodes.VerificationRequired,
                "Logical application and readback verification must both succeed before promotion.");
        }

        if (!ReferenceEquals(expectedPlan, verification.AppliedPlan))
        {
            throw Mismatch("The verification evidence refers to a different logical plan.");
        }

        EnsureSameDocument(candidate.Identity, verification.AppliedIdentity);
        if (verification.AppliedIdentity.State != DocumentState.Active)
        {
            throw Mismatch("The verified destination must be active.");
        }

        if (!verification.AppliedPlan.CurrentFingerprint.Equals(candidate.Fingerprint) ||
            !string.Equals(
                verification.AppliedFingerprint,
                candidate.Fingerprint.Value,
                StringComparison.Ordinal))
        {
            throw Mismatch("The applied fingerprint does not match the candidate.");
        }

        if (verification.AppliedBlocks.Count != candidate.Blocks.Count)
        {
            throw Mismatch("The applied block count does not match the candidate.");
        }

        for (var index = 0; index < candidate.Blocks.Count; index++)
        {
            var expected = candidate.Blocks[index];
            var actual = verification.AppliedBlocks[index];
            if (!string.Equals(expected.ExplicitId, actual.ExplicitId, StringComparison.Ordinal) ||
                !string.Equals(expected.GeneratedId, actual.GeneratedId, StringComparison.Ordinal) ||
                !string.Equals(expected.ContentHash, actual.ContentHash, StringComparison.Ordinal))
            {
                throw Mismatch($"The applied block at index {index} does not match the candidate.");
            }
        }

        return new VerifiedPublishResult(candidate, verification.AppliedRevision);
    }

    private static void EnsureSameDocument(DocumentIdentity expected, DocumentIdentity actual)
    {
        if (!string.Equals(expected.PublicationId, actual.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(expected.DocumentId, actual.DocumentId, StringComparison.Ordinal) ||
            !string.Equals(expected.GoogleDocumentId, actual.GoogleDocumentId, StringComparison.Ordinal))
        {
            throw new StateLifecycleException(
                StateErrorCodes.DocumentIdentityMismatch,
                "The verified output identifies a different document.");
        }
    }

    private static StateLifecycleException Mismatch(string message) => new(
        StateErrorCodes.VerificationMismatch,
        message);
}

/// <summary>Creates active verified state only from accepted verification results.</summary>
public sealed class VerifiedPublishStatePromoter : IVerifiedPublishStatePromoter
{
    /// <inheritdoc />
    public VerifiedPublishState Promote(
        VerifiedPublishState? baseline,
        VerifiedPublishResult verifiedResult)
    {
        ArgumentNullException.ThrowIfNull(verifiedResult);
        var candidate = verifiedResult.Candidate;
        if (!DocumentStateTransitionRules.IsAllowed(baseline?.Identity.State, DocumentState.Active))
        {
            throw InvalidTransition(baseline?.Identity.State, DocumentState.Active);
        }

        if (baseline is not null)
        {
            EnsureSameDocument(baseline.Identity, candidate.Identity);
        }

        var identity = new DocumentIdentity(
            candidate.Identity.PublicationId,
            candidate.Identity.DocumentId,
            candidate.Identity.GoogleDocumentId,
            DocumentState.Active);
        return new VerifiedPublishState(
            identity,
            candidate.Versions,
            verifiedResult.Revision,
            candidate.Fingerprint,
            candidate.Blocks);
    }

    internal static StateLifecycleException InvalidTransition(
        DocumentState? current,
        DocumentState next) => new(
            StateErrorCodes.InvalidTransition,
            $"Document state transition from {current?.ToString() ?? "none"} to {next} is prohibited.");

    private static void EnsureSameDocument(DocumentIdentity baseline, DocumentIdentity candidate)
    {
        if (!string.Equals(baseline.PublicationId, candidate.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(baseline.DocumentId, candidate.DocumentId, StringComparison.Ordinal) ||
            !string.Equals(baseline.GoogleDocumentId, candidate.GoogleDocumentId, StringComparison.Ordinal))
        {
            throw new StateLifecycleException(
                StateErrorCodes.DocumentIdentityMismatch,
                "The baseline and candidate do not identify the same document.");
        }
    }
}

/// <summary>Applies persisted-state transition rules without changing verified content.</summary>
public sealed class VerifiedPublishStateTransitioner : IVerifiedPublishStateTransitioner
{
    /// <inheritdoc />
    public VerifiedPublishState Transition(VerifiedPublishState current, DocumentState next)
    {
        ArgumentNullException.ThrowIfNull(current);
        if (!DocumentStateTransitionRules.IsAllowed(current.Identity.State, next))
        {
            throw VerifiedPublishStatePromoter.InvalidTransition(current.Identity.State, next);
        }

        var identity = new DocumentIdentity(
            current.Identity.PublicationId,
            current.Identity.DocumentId,
            current.Identity.GoogleDocumentId,
            next);
        return new VerifiedPublishState(
            identity,
            current.Versions,
            current.Revision,
            current.Fingerprint,
            current.Blocks);
    }
}
