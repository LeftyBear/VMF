using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Coordinates revision checks, physical application, and complete readback verification.</summary>
public sealed class PhysicalUpdateApplicationVerifier : IPublishPlanApplicationVerifier
{
    private readonly IManagedDocumentAdapter adapter;
    private readonly IPhysicalUpdatePlanner planner;

    /// <summary>Initializes a physical update verifier.</summary>
    public PhysicalUpdateApplicationVerifier(
        IManagedDocumentAdapter adapter,
        IPhysicalUpdatePlanner planner)
    {
        this.adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        this.planner = planner ?? throw new ArgumentNullException(nameof(planner));
    }

    /// <inheritdoc />
    public async Task<ManagedDocumentSnapshot> PrepareAsync(
        PublishCandidate candidate,
        VerifiedPublishState? baseline,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        try
        {
            var snapshot = await adapter.GetSnapshotAsync(candidate.Identity, cancellationToken)
                .ConfigureAwait(false);
            EnsureSameDocument(candidate.Identity, snapshot.Identity);
            planner.ValidateSnapshot(baseline, snapshot);
            return snapshot;
        }
        catch (PhysicalUpdateException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ReadbackFailed,
                "The current managed snapshot could not be read.",
                exception);
        }
    }

    /// <inheritdoc />
    public async Task<PublishApplicationVerification> ApplyAndVerifyAsync(
        PublishCandidate candidate,
        VerifiedPublishState? baseline,
        DiffPlan plan,
        ManagedDocumentSnapshot preparedSnapshot,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(preparedSnapshot);
        var physicalPlan = planner.CreatePlan(baseline, candidate, plan, preparedSnapshot);

        ManagedDocumentSnapshot preApply;
        try
        {
            preApply = await adapter.GetSnapshotAsync(candidate.Identity, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ReadbackFailed,
                "The pre-apply revision could not be read.",
                exception);
        }

        EnsurePreparedSnapshotUnchanged(preparedSnapshot, preApply);
        if (!physicalPlan.IsPublishRequired)
        {
            VerifyReadback(candidate, preparedSnapshot, preApply, preApply.Revision, requireAdvance: false);
            return Evidence(candidate, plan, preApply);
        }

        PhysicalApplyReceipt receipt;
        try
        {
            receipt = await adapter.ApplyAsync(physicalPlan, cancellationToken).ConfigureAwait(false);
        }
        catch (PhysicalUpdateException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ApplicationFailed,
                "The physical update could not be applied.",
                exception);
        }

        if (receipt.Revision.Sequence <= preparedSnapshot.Revision.Sequence)
        {
            throw RevisionConflict("The applied revision did not advance monotonically.");
        }

        ManagedDocumentSnapshot readback;
        try
        {
            readback = await adapter.GetSnapshotAsync(candidate.Identity, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ReadbackFailed,
                "The post-apply managed snapshot could not be read.",
                exception);
        }

        VerifyReadback(candidate, preparedSnapshot, readback, receipt.Revision, requireAdvance: true);
        return Evidence(candidate, plan, readback);
    }

    /// <inheritdoc />
    public PhysicalUpdateDryRunResult CreateDryRun(
        PublishCandidate candidate,
        VerifiedPublishState? baseline,
        DiffPlan plan,
        ManagedDocumentSnapshot preparedSnapshot)
    {
        try
        {
            return new PhysicalUpdateDryRunResult(
                plan,
                planner.CreatePlan(baseline, candidate, plan, preparedSnapshot),
                Array.Empty<string>());
        }
        catch (PhysicalUpdateException exception)
        {
            return new PhysicalUpdateDryRunResult(plan, null, [exception.Code]);
        }
    }

    private static PublishApplicationVerification Evidence(
        PublishCandidate candidate,
        DiffPlan plan,
        ManagedDocumentSnapshot readback) => new(
            readback.Identity,
            plan,
            isLogicalPlanApplied: true,
            isReadbackVerified: true,
            readback.PublishFingerprint,
            readback.Blocks.Select(block => block.Identity),
            readback.Revision);

    private static void VerifyReadback(
        PublishCandidate candidate,
        ManagedDocumentSnapshot prepared,
        ManagedDocumentSnapshot readback,
        DocumentRevision appliedRevision,
        bool requireAdvance)
    {
        if (!SameDocument(candidate.Identity, readback.Identity))
        {
            throw Mismatch("The readback identifies a different document.");
        }
        if (!readback.Revision.Equals(appliedRevision))
        {
            throw RevisionConflict("The readback revision differs from the applied revision.");
        }

        if (requireAdvance && readback.Revision.Sequence <= prepared.Revision.Sequence)
        {
            throw RevisionConflict("The readback revision did not advance monotonically.");
        }

        if (readback.ManagedRegion.StartIndex != prepared.ManagedRegion.StartIndex ||
            readback.ManagedRegion.EndIndex < readback.ManagedRegion.StartIndex)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ManagedRegionMismatch,
                "The readback managed-region boundary is invalid.");
        }

        if (!string.Equals(
                readback.PublishFingerprint,
                candidate.Fingerprint.Value,
                StringComparison.Ordinal) ||
            readback.Blocks.Count != candidate.Blocks.Count)
        {
            throw Mismatch("The readback fingerprint or block count differs from the candidate.");
        }

        ValidateReadbackRanges(readback);

        for (var index = 0; index < candidate.Blocks.Count; index++)
        {
            if (!SameBlock(candidate.Blocks[index], readback.Blocks[index].Identity))
            {
                throw Mismatch($"The readback block at index {index} differs from the candidate.");
            }
        }
    }

    private static void EnsurePreparedSnapshotUnchanged(
        ManagedDocumentSnapshot prepared,
        ManagedDocumentSnapshot current)
    {
        EnsureSameDocument(prepared.Identity, current.Identity);
        if (!prepared.Revision.Equals(current.Revision))
        {
            throw RevisionConflict("The document changed between planning and application.");
        }

        if (!prepared.ManagedRegion.Equals(current.ManagedRegion) ||
            !string.Equals(prepared.PublishFingerprint, current.PublishFingerprint, StringComparison.Ordinal) ||
            prepared.Blocks.Count != current.Blocks.Count)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ManagedRegionMismatch,
                "The managed region changed between planning and application.");
        }

        for (var index = 0; index < prepared.Blocks.Count; index++)
        {
            if (!prepared.Blocks[index].Range.Equals(current.Blocks[index].Range) ||
                !SameBlock(prepared.Blocks[index].Identity, current.Blocks[index].Identity))
            {
                throw new PhysicalUpdateException(
                    UpdateErrorCodes.ManagedRegionMismatch,
                    "The managed block topology changed between planning and application.");
            }
        }
    }

    private static void ValidateReadbackRanges(ManagedDocumentSnapshot snapshot)
    {
        var previousEnd = snapshot.ManagedRegion.StartIndex;
        foreach (var block in snapshot.Blocks)
        {
            if (block.Range.StartIndex != previousEnd ||
                block.Range.StartIndex < snapshot.ManagedRegion.StartIndex ||
                block.Range.EndIndex > snapshot.ManagedRegion.EndIndex)
            {
                throw new PhysicalUpdateException(
                    UpdateErrorCodes.ManagedRegionMismatch,
                    "Readback block ranges overlap, contain gaps, or leave the managed region.");
            }

            previousEnd = block.Range.EndIndex;
        }

        if (previousEnd != snapshot.ManagedRegion.EndIndex)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ManagedRegionMismatch,
                "Readback block ranges do not cover the managed region.");
        }
    }

    private static bool SameBlock(BlockIdentity expected, BlockIdentity actual) =>
        string.Equals(expected.ExplicitId, actual.ExplicitId, StringComparison.Ordinal) &&
        string.Equals(expected.GeneratedId, actual.GeneratedId, StringComparison.Ordinal) &&
        string.Equals(expected.ContentHash, actual.ContentHash, StringComparison.Ordinal);

    private static void EnsureSameDocument(DocumentIdentity expected, DocumentIdentity actual)
    {
        if (!SameDocument(expected, actual))
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ManagedRegionMismatch,
                "The managed snapshot identifies a different document.");
        }
    }

    private static bool SameDocument(DocumentIdentity expected, DocumentIdentity actual) =>
        string.Equals(expected.PublicationId, actual.PublicationId, StringComparison.Ordinal) &&
        string.Equals(expected.DocumentId, actual.DocumentId, StringComparison.Ordinal) &&
        string.Equals(expected.GoogleDocumentId, actual.GoogleDocumentId, StringComparison.Ordinal) &&
        expected.State == actual.State;

    private static PhysicalUpdateException RevisionConflict(string message) => new(
        UpdateErrorCodes.RevisionConflict,
        message);

    private static PhysicalUpdateException Mismatch(string message) => new(
        UpdateErrorCodes.ReadbackMismatch,
        message);
}
