using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Produces delete-descending and insert-reverse physical block plans.</summary>
public sealed class PhysicalUpdatePlanner : IPhysicalUpdatePlanner
{
    /// <inheritdoc />
    public void ValidateSnapshot(
        VerifiedPublishState? baseline,
        ManagedDocumentSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ValidateManagedSnapshot(baseline, snapshot);
    }

    /// <inheritdoc />
    public PhysicalUpdatePlan CreatePlan(
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        DiffPlan logicalPlan,
        ManagedDocumentSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(logicalPlan);
        ArgumentNullException.ThrowIfNull(snapshot);
        EnsureSameDocument(candidate.Identity, snapshot.Identity);
        ValidateManagedSnapshot(baseline, snapshot);

        if (!logicalPlan.CurrentFingerprint.Equals(candidate.Fingerprint))
        {
            throw Invalid("The logical plan fingerprint does not match the candidate.");
        }

        if (!logicalPlan.IsPublishRequired)
        {
            return new PhysicalUpdatePlan(
                candidate.Identity,
                snapshot.Revision,
                snapshot.ManagedRegion,
                logicalPlan,
                Array.Empty<PhysicalUpdateOperation>());
        }

        var document = candidate.Document ?? throw Invalid(
            "A physical update requires the candidate canonical document payload.");
        if (document.Blocks.Count != candidate.Blocks.Count)
        {
            throw Invalid("Candidate block payloads and identities are not aligned.");
        }

        var groups = GroupCurrentOperations(candidate, logicalPlan);
        var destructive = new List<PendingOperation>();
        var consumedPrevious = new HashSet<int>();

        foreach (var operation in logicalPlan.Operations.Where(item => item.Kind == DiffOperationKind.Delete))
        {
            var previousIndex = RequirePreviousIndex(operation, snapshot.Blocks.Count);
            ConsumePrevious(consumedPrevious, previousIndex);
            destructive.Add(new PendingOperation(
                PhysicalOperationKind.DeleteRange,
                PhysicalOperationReason.Delete,
                previousIndex,
                null,
                snapshot.Blocks[previousIndex].Range,
                snapshot.Blocks[previousIndex].Identity,
                null));
        }

        foreach (var group in groups.Values.Where(group => group.Reason is null))
        {
            var previousIndex = group.PreviousIndex ?? throw Invalid(
                $"NoChange block {group.CurrentIndex} has no previous index.");
            ConsumePrevious(consumedPrevious, previousIndex);
        }

        foreach (var group in groups.Values)
        {
            if (group.Reason is null || group.Reason == PhysicalOperationReason.Insert)
            {
                continue;
            }

            var previousIndex = group.PreviousIndex ?? throw Invalid(
                $"Candidate block {group.CurrentIndex} requires a previous index.");
            if (previousIndex < 0 || previousIndex >= snapshot.Blocks.Count)
            {
                throw Invalid($"Previous block index {previousIndex} is outside the snapshot.");
            }

            ConsumePrevious(consumedPrevious, previousIndex);
            destructive.Add(new PendingOperation(
                PhysicalOperationKind.DeleteRange,
                group.Reason.Value,
                previousIndex,
                group.CurrentIndex,
                snapshot.Blocks[previousIndex].Range,
                candidate.Blocks[group.CurrentIndex],
                null));
        }

        var destructiveOrdered = destructive
            .OrderByDescending(item => item.Range.StartIndex)
            .ThenByDescending(item => item.PreviousIndex)
            .ToArray();
        EnsureDistinctDeleteRanges(destructiveOrdered);

        var deletedRanges = destructiveOrdered.Select(item => item.Range).ToArray();
        var reducedRegionEnd = snapshot.ManagedRegion.EndIndex - deletedRanges.Sum(range => range.Length);
        var survivingRanges = BuildSurvivingRanges(groups, snapshot, deletedRanges);
        var constructive = groups.Values
            .Where(group => group.Reason is not null)
            .OrderByDescending(group => group.CurrentIndex)
            .Select(group =>
            {
                var insertionIndex = survivingRanges
                    .Where(item => item.Key > group.CurrentIndex)
                    .OrderBy(item => item.Key)
                    .Select(item => item.Value.StartIndex)
                    .FirstOrDefault(reducedRegionEnd);
                return new PendingOperation(
                    PhysicalOperationKind.InsertBlock,
                    group.Reason!.Value,
                    group.PreviousIndex,
                    group.CurrentIndex,
                    new DocumentTextRange(insertionIndex, insertionIndex),
                    candidate.Blocks[group.CurrentIndex],
                    document.Blocks[group.CurrentIndex]);
            })
            .ToArray();

        var pending = destructiveOrdered.Concat(constructive).ToArray();
        var operations = pending.Select((item, index) => new PhysicalUpdateOperation(
            index,
            item.Kind,
            item.Reason,
            item.PreviousIndex,
            item.CurrentIndex,
            item.Range,
            item.Identity,
            item.CandidateBlock)).ToArray();
        EnsureInsideManagedRegion(operations, snapshot.ManagedRegion);
        return new PhysicalUpdatePlan(
            candidate.Identity,
            snapshot.Revision,
            snapshot.ManagedRegion,
            logicalPlan,
            operations);
    }

    private static IReadOnlyDictionary<int, CurrentOperationGroup> GroupCurrentOperations(
        PublishCandidate candidate,
        DiffPlan plan)
    {
        var result = new SortedDictionary<int, CurrentOperationGroup>();
        foreach (var grouping in plan.Operations
                     .Where(operation => operation.CurrentIndex is not null)
                     .GroupBy(operation => operation.CurrentIndex!.Value))
        {
            if (grouping.Key < 0 || grouping.Key >= candidate.Blocks.Count)
            {
                throw Invalid($"Candidate block index {grouping.Key} is outside the candidate.");
            }

            var operations = grouping.ToArray();
            if (operations.Any(operation => operation.CurrentBlock is null ||
                !SameBlock(candidate.Blocks[grouping.Key], operation.CurrentBlock)))
            {
                throw Invalid($"Candidate block {grouping.Key} differs from its logical operation payload.");
            }

            var kinds = operations.Select(item => item.Kind).OrderBy(item => item).ToArray();
            var reason = kinds switch
            {
                [DiffOperationKind.NoChange] => (PhysicalOperationReason?)null,
                [DiffOperationKind.Insert] => PhysicalOperationReason.Insert,
                [DiffOperationKind.Update] => PhysicalOperationReason.Update,
                [DiffOperationKind.Move] => PhysicalOperationReason.Move,
                [DiffOperationKind.Update, DiffOperationKind.Move] => PhysicalOperationReason.MoveAndUpdate,
                _ => throw Invalid($"Candidate block {grouping.Key} has an invalid logical operation set."),
            };
            var previousIndexes = operations
                .Where(item => item.PreviousIndex is not null)
                .Select(item => item.PreviousIndex!.Value)
                .Distinct()
                .ToArray();
            if (previousIndexes.Length > 1)
            {
                throw Invalid($"Candidate block {grouping.Key} maps to multiple previous blocks.");
            }

            result.Add(grouping.Key, new CurrentOperationGroup(
                grouping.Key,
                previousIndexes.SingleOrDefault(-1) is var previous && previous >= 0 ? previous : null,
                reason));
        }

        if (result.Count != candidate.Blocks.Count)
        {
            throw Invalid("Every candidate block must have one logical operation group.");
        }

        return result;
    }

    private static IReadOnlyDictionary<int, DocumentTextRange> BuildSurvivingRanges(
        IReadOnlyDictionary<int, CurrentOperationGroup> groups,
        ManagedDocumentSnapshot snapshot,
        IReadOnlyList<DocumentTextRange> deletedRanges)
    {
        var result = new SortedDictionary<int, DocumentTextRange>();
        foreach (var group in groups.Values.Where(group => group.Reason is null))
        {
            var previousIndex = group.PreviousIndex ?? throw Invalid(
                $"NoChange block {group.CurrentIndex} has no previous index.");
            if (previousIndex < 0 || previousIndex >= snapshot.Blocks.Count)
            {
                throw Invalid($"Previous block index {previousIndex} is outside the snapshot.");
            }

            var range = snapshot.Blocks[previousIndex].Range;
            var shift = deletedRanges
                .Where(deleted => deleted.EndIndex <= range.StartIndex)
                .Sum(deleted => deleted.Length);
            result.Add(
                group.CurrentIndex,
                new DocumentTextRange(range.StartIndex - shift, range.EndIndex - shift));
        }

        return result;
    }

    private static void ValidateManagedSnapshot(
        VerifiedPublishState? baseline,
        ManagedDocumentSnapshot snapshot)
    {
        var previousEnd = snapshot.ManagedRegion.StartIndex;
        for (var index = 0; index < snapshot.Blocks.Count; index++)
        {
            var range = snapshot.Blocks[index].Range;
            if (range.StartIndex < snapshot.ManagedRegion.StartIndex ||
                range.EndIndex > snapshot.ManagedRegion.EndIndex ||
                range.StartIndex != previousEnd)
            {
                throw RegionMismatch("Managed block ranges overlap, contain gaps, or leave the managed boundary.");
            }

            previousEnd = range.EndIndex;
        }

        if (previousEnd != snapshot.ManagedRegion.EndIndex)
        {
            throw RegionMismatch("Managed block ranges do not cover the managed boundary.");
        }

        if (baseline is null)
        {
            if (snapshot.Blocks.Count != 0)
            {
                throw RegionMismatch("A publication without a baseline requires an empty managed region.");
            }

            return;
        }

        EnsureSameDocument(baseline.Identity, snapshot.Identity);
        if (!baseline.Revision.Equals(snapshot.Revision))
        {
            throw RevisionConflict("The current revision differs from the verified baseline revision.");
        }

        if (!string.Equals(snapshot.PublishFingerprint, baseline.Fingerprint.Value, StringComparison.Ordinal) ||
            snapshot.Blocks.Count != baseline.Blocks.Count)
        {
            throw RegionMismatch("The current managed snapshot differs from the verified baseline.");
        }

        for (var index = 0; index < baseline.Blocks.Count; index++)
        {
            EnsureSameBlock(baseline.Blocks[index], snapshot.Blocks[index].Identity, index);
        }
    }

    private static void EnsureSameDocument(DocumentIdentity expected, DocumentIdentity actual)
    {
        if (!string.Equals(expected.PublicationId, actual.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(expected.DocumentId, actual.DocumentId, StringComparison.Ordinal) ||
            !string.Equals(expected.GoogleDocumentId, actual.GoogleDocumentId, StringComparison.Ordinal) ||
            expected.State != actual.State)
        {
            throw RegionMismatch("The managed snapshot identifies a different document.");
        }
    }

    private static void EnsureSameBlock(BlockIdentity expected, BlockIdentity actual, int index)
    {
        if (!string.Equals(expected.ExplicitId, actual.ExplicitId, StringComparison.Ordinal) ||
            !string.Equals(expected.GeneratedId, actual.GeneratedId, StringComparison.Ordinal) ||
            !string.Equals(expected.ContentHash, actual.ContentHash, StringComparison.Ordinal))
        {
            throw RegionMismatch($"Managed block {index} differs from the verified baseline.");
        }
    }

    private static bool SameBlock(BlockIdentity expected, BlockIdentity actual) =>
        string.Equals(expected.ExplicitId, actual.ExplicitId, StringComparison.Ordinal) &&
        string.Equals(expected.GeneratedId, actual.GeneratedId, StringComparison.Ordinal) &&
        string.Equals(expected.ContentHash, actual.ContentHash, StringComparison.Ordinal);

    private static int RequirePreviousIndex(DiffOperation operation, int blockCount)
    {
        if (operation.PreviousIndex is not int index || index < 0 || index >= blockCount)
        {
            throw Invalid("A delete operation has an invalid previous index.");
        }

        return index;
    }

    private static void ConsumePrevious(HashSet<int> consumed, int index)
    {
        if (!consumed.Add(index))
        {
            throw Invalid($"Previous block {index} is consumed by multiple physical operations.");
        }
    }

    private static void EnsureDistinctDeleteRanges(IReadOnlyList<PendingOperation> operations)
    {
        for (var index = 1; index < operations.Count; index++)
        {
            if (operations[index - 1].Range.StartIndex < operations[index].Range.EndIndex)
            {
                throw RegionMismatch("Physical delete ranges overlap.");
            }
        }
    }

    private static void EnsureInsideManagedRegion(
        IEnumerable<PhysicalUpdateOperation> operations,
        DocumentTextRange region)
    {
        if (operations.Any(operation =>
            operation.AffectedRange.StartIndex < region.StartIndex ||
            operation.AffectedRange.EndIndex > region.EndIndex))
        {
            throw RegionMismatch("A physical operation extends outside the managed region.");
        }
    }

    private static PhysicalUpdateException Invalid(string message) => new(
        UpdateErrorCodes.PhysicalPlanInvalid,
        message);

    private static PhysicalUpdateException RegionMismatch(string message) => new(
        UpdateErrorCodes.ManagedRegionMismatch,
        message);

    private static PhysicalUpdateException RevisionConflict(string message) => new(
        UpdateErrorCodes.RevisionConflict,
        message);

    private sealed record CurrentOperationGroup(
        int CurrentIndex,
        int? PreviousIndex,
        PhysicalOperationReason? Reason);

    private sealed record PendingOperation(
        PhysicalOperationKind Kind,
        PhysicalOperationReason Reason,
        int? PreviousIndex,
        int? CurrentIndex,
        DocumentTextRange Range,
        BlockIdentity Identity,
        DocumentBlock? CandidateBlock);
}
