using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Applies physical plans in memory for deterministic lifecycle and integration tests.</summary>
public sealed class InMemoryManagedDocumentAdapter : IManagedDocumentAdapter
{
    private ManagedDocumentSnapshot snapshot;

    /// <summary>Initializes an in-memory managed document.</summary>
    public InMemoryManagedDocumentAdapter(ManagedDocumentSnapshot initialSnapshot)
    {
        snapshot = initialSnapshot ?? throw new ArgumentNullException(nameof(initialSnapshot));
    }

    /// <summary>Gets the number of physical apply calls.</summary>
    public int ApplyCount { get; private set; }

    /// <summary>Gets the last applied plan.</summary>
    public PhysicalUpdatePlan? LastAppliedPlan { get; private set; }

    /// <summary>Gets or sets whether the next apply fails before mutation.</summary>
    public bool FailNextApply { get; set; }

    /// <summary>Gets or sets whether an external revision appears immediately before apply.</summary>
    public bool AdvanceRevisionBeforeApply { get; set; }

    /// <summary>Gets or sets whether an external revision appears between apply and readback.</summary>
    public bool AdvanceRevisionAfterApply { get; set; }

    /// <summary>Gets or sets whether apply incorrectly leaves the revision unchanged.</summary>
    public bool KeepRevisionAfterApply { get; set; }

    /// <summary>Gets or sets whether apply incorrectly returns a regressed revision.</summary>
    public bool RegressRevisionAfterApply { get; set; }

    /// <summary>Advances the current revision without changing managed content.</summary>
    public void SimulateExternalEdit()
    {
        snapshot = CopyWithRevision(NextRevision(snapshot.Revision));
    }

    /// <inheritdoc />
    public Task<ManagedDocumentSnapshot> GetSnapshotAsync(
        DocumentIdentity identity,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(snapshot);
    }

    /// <inheritdoc />
    public Task<PhysicalApplyReceipt> ApplyAsync(
        PhysicalUpdatePlan plan,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(plan);
        cancellationToken.ThrowIfCancellationRequested();
        if (FailNextApply)
        {
            FailNextApply = false;
            throw new InvalidOperationException("Injected physical application failure.");
        }

        if (AdvanceRevisionBeforeApply)
        {
            AdvanceRevisionBeforeApply = false;
            SimulateExternalEdit();
        }

        if (!plan.RequiredRevision.Equals(snapshot.Revision))
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.RevisionConflict,
                "The in-memory document revision differs from the physical-plan precondition.");
        }

        ValidateOperationOrder(plan);
        ApplyCount++;
        LastAppliedPlan = plan;
        var blocks = BuildCandidateBlocks(plan);
        var ranges = BuildCandidateRanges(plan, blocks);
        var appliedRevision = RegressRevisionAfterApply
            ? new DocumentRevision(
                "revision-" + Math.Max(0, snapshot.Revision.Sequence - 1),
                Math.Max(0, snapshot.Revision.Sequence - 1))
            : KeepRevisionAfterApply
                ? snapshot.Revision
                : NextRevision(snapshot.Revision);
        RegressRevisionAfterApply = false;
        KeepRevisionAfterApply = false;
        snapshot = new ManagedDocumentSnapshot(
            plan.Identity,
            appliedRevision,
            new DocumentTextRange(
                plan.ManagedRegion.StartIndex,
                ranges.Count == 0 ? plan.ManagedRegion.StartIndex : ranges[^1].Range.EndIndex),
            plan.LogicalPlan.CurrentFingerprint.Value,
            ranges);
        var receipt = new PhysicalApplyReceipt(appliedRevision);
        if (AdvanceRevisionAfterApply)
        {
            AdvanceRevisionAfterApply = false;
            SimulateExternalEdit();
        }

        return Task.FromResult(receipt);
    }

    private static IReadOnlyList<BlockIdentity> BuildCandidateBlocks(PhysicalUpdatePlan plan) =>
        plan.LogicalPlan.Operations
            .Where(operation => operation.CurrentIndex is not null)
            .GroupBy(operation => operation.CurrentIndex!.Value)
            .OrderBy(group => group.Key)
            .Select(group => group.Select(operation => operation.CurrentBlock)
                .First(block => block is not null)!)
            .ToArray();

    private IReadOnlyList<ManagedBlockSnapshot> BuildCandidateRanges(
        PhysicalUpdatePlan plan,
        IReadOnlyList<BlockIdentity> blocks)
    {
        var result = new ManagedBlockSnapshot[blocks.Count];
        var currentIndex = plan.ManagedRegion.StartIndex;
        for (var index = 0; index < blocks.Count; index++)
        {
            var inserted = plan.Operations.FirstOrDefault(operation =>
                operation.Kind == PhysicalOperationKind.InsertBlock &&
                operation.CurrentIndex == index);
            var length = inserted?.CandidateBlock is not null
                ? EstimateLength(inserted.CandidateBlock)
                : ExistingLength(plan, index);
            var range = new DocumentTextRange(currentIndex, currentIndex + length);
            result[index] = new ManagedBlockSnapshot(blocks[index], range);
            currentIndex = range.EndIndex;
        }

        return result;
    }

    private int ExistingLength(PhysicalUpdatePlan plan, int currentIndex)
    {
        var operation = plan.LogicalPlan.Operations.First(item =>
            item.CurrentIndex == currentIndex && item.PreviousIndex is not null);
        return snapshot.Blocks[operation.PreviousIndex!.Value].Range.Length;
    }

    private static int EstimateLength(DocumentBlock block) => block.Kind switch
    {
        DocumentBlockKind.Code => Math.Max(1, (block.Code?.Text.Length ?? 0) + 1),
        DocumentBlockKind.Table => Math.Max(1, block.Table?.AllRows.Count ?? 1),
        DocumentBlockKind.Image => 1,
        DocumentBlockKind.List => Math.Max(1, block.List?.Items.Count ?? 1),
        _ => Math.Max(1, block.Content.Sum(item => InlineLength(item)) + 1),
    };

    private static int InlineLength(InlineContent content) => content switch
    {
        TextInline text => text.Text.Length,
        BoldInline bold => bold.Content.Sum(InlineLength),
        ItalicInline italic => italic.Content.Sum(InlineLength),
        CodeInline code => code.Text.Length,
        LinkInline link => link.Content.Sum(InlineLength),
        _ => 0,
    };

    private static void ValidateOperationOrder(PhysicalUpdatePlan plan)
    {
        var deletes = plan.Operations.Where(operation => operation.Kind == PhysicalOperationKind.DeleteRange)
            .ToArray();
        if (!deletes.Select(operation => operation.AffectedRange.StartIndex)
            .SequenceEqual(deletes.Select(operation => operation.AffectedRange.StartIndex)
                .OrderByDescending(index => index)))
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.PhysicalPlanInvalid,
                "Physical deletes are not ordered from the end of the document.");
        }

        if (plan.Operations.Select(operation => operation.Sequence)
            .Where((sequence, index) => sequence != index)
            .Any())
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.PhysicalPlanInvalid,
                "Physical operation sequence values are not contiguous.");
        }
    }

    private static DocumentRevision NextRevision(DocumentRevision current) => new(
        "revision-" + (current.Sequence + 1),
        current.Sequence + 1);

    private ManagedDocumentSnapshot CopyWithRevision(DocumentRevision revision) => new(
        snapshot.Identity,
        revision,
        snapshot.ManagedRegion,
        snapshot.PublishFingerprint,
        snapshot.Blocks);
}
