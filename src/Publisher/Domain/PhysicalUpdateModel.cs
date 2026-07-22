namespace Vmf.Publisher.Domain;

/// <summary>Represents a UTF-16 Google Docs range with an exclusive end index.</summary>
public sealed class DocumentTextRange : IEquatable<DocumentTextRange>
{
    /// <summary>Initializes a document range.</summary>
    public DocumentTextRange(int startIndex, int endIndex)
    {
        if (startIndex < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (endIndex < startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(endIndex));
        }

        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    /// <summary>Gets the inclusive start index.</summary>
    public int StartIndex { get; }

    /// <summary>Gets the exclusive end index.</summary>
    public int EndIndex { get; }

    /// <summary>Gets the UTF-16 range length.</summary>
    public int Length => EndIndex - StartIndex;

    /// <inheritdoc />
    public bool Equals(DocumentTextRange? other) => other is not null &&
        StartIndex == other.StartIndex && EndIndex == other.EndIndex;

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as DocumentTextRange);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(StartIndex, EndIndex);
}

/// <summary>Identifies one monotonic document revision.</summary>
public sealed class DocumentRevision : IEquatable<DocumentRevision>
{
    /// <summary>Initializes a revision.</summary>
    public DocumentRevision(string revisionId, long sequence)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(revisionId);
        if (sequence < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence));
        }

        RevisionId = revisionId;
        Sequence = sequence;
    }

    /// <summary>Gets the provider revision identifier.</summary>
    public string RevisionId { get; }

    /// <summary>Gets the adapter-supplied monotonic sequence.</summary>
    public long Sequence { get; }

    /// <inheritdoc />
    public bool Equals(DocumentRevision? other) => other is not null &&
        Sequence == other.Sequence &&
        string.Equals(RevisionId, other.RevisionId, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as DocumentRevision);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(
        StringComparer.Ordinal.GetHashCode(RevisionId),
        Sequence);
}

/// <summary>Represents one managed block and its current document range.</summary>
public sealed class ManagedBlockSnapshot
{
    /// <summary>Initializes a managed block snapshot.</summary>
    public ManagedBlockSnapshot(BlockIdentity identity, DocumentTextRange range)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Range = range ?? throw new ArgumentNullException(nameof(range));
        if (range.Length == 0)
        {
            throw new ArgumentException("A managed block range must not be empty.", nameof(range));
        }
    }

    /// <summary>Gets the reconstructed block identity.</summary>
    public BlockIdentity Identity { get; }

    /// <summary>Gets the current block range.</summary>
    public DocumentTextRange Range { get; }
}

/// <summary>Represents a read-back managed document boundary and block topology.</summary>
public sealed class ManagedDocumentSnapshot
{
    /// <summary>Initializes a managed document snapshot.</summary>
    public ManagedDocumentSnapshot(
        DocumentIdentity identity,
        DocumentRevision revision,
        DocumentTextRange managedRegion,
        string publishFingerprint,
        IEnumerable<ManagedBlockSnapshot> blocks)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Revision = revision ?? throw new ArgumentNullException(nameof(revision));
        ManagedRegion = managedRegion ?? throw new ArgumentNullException(nameof(managedRegion));
        ArgumentException.ThrowIfNullOrWhiteSpace(publishFingerprint);
        ArgumentNullException.ThrowIfNull(blocks);

        var items = blocks.ToArray();
        if (items.Any(block => block is null))
        {
            throw new ArgumentException("Managed blocks must not contain null items.", nameof(blocks));
        }

        PublishFingerprint = publishFingerprint;
        Blocks = Array.AsReadOnly(items);
    }

    /// <summary>Gets the document identity.</summary>
    public DocumentIdentity Identity { get; }

    /// <summary>Gets the read revision.</summary>
    public DocumentRevision Revision { get; }

    /// <summary>Gets the explicit managed-region boundary.</summary>
    public DocumentTextRange ManagedRegion { get; }

    /// <summary>Gets the read-back publish fingerprint.</summary>
    public string PublishFingerprint { get; }

    /// <summary>Gets blocks in read-back order.</summary>
    public IReadOnlyList<ManagedBlockSnapshot> Blocks { get; }
}

/// <summary>Identifies an executable physical operation.</summary>
public enum PhysicalOperationKind
{
    /// <summary>Deletes one current managed block range.</summary>
    DeleteRange,

    /// <summary>Inserts one canonical candidate block.</summary>
    InsertBlock,
}

/// <summary>Explains why a physical operation exists.</summary>
public enum PhysicalOperationReason
{
    /// <summary>Logical insertion.</summary>
    Insert,

    /// <summary>Logical deletion.</summary>
    Delete,

    /// <summary>Logical content update.</summary>
    Update,

    /// <summary>Logical move without content change.</summary>
    Move,

    /// <summary>Combined logical move and content update.</summary>
    MoveAndUpdate,
}

/// <summary>Represents one deterministic physical block operation.</summary>
public sealed class PhysicalUpdateOperation
{
    internal PhysicalUpdateOperation(
        int sequence,
        PhysicalOperationKind kind,
        PhysicalOperationReason reason,
        int? previousIndex,
        int? currentIndex,
        DocumentTextRange affectedRange,
        BlockIdentity traceIdentity,
        DocumentBlock? candidateBlock)
    {
        Sequence = sequence;
        Kind = kind;
        Reason = reason;
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        AffectedRange = affectedRange;
        TraceIdentity = traceIdentity;
        CandidateBlock = candidateBlock;
    }

    /// <summary>Gets the zero-based physical execution sequence.</summary>
    public int Sequence { get; }

    /// <summary>Gets the physical operation kind.</summary>
    public PhysicalOperationKind Kind { get; }

    /// <summary>Gets the logical reason for the operation.</summary>
    public PhysicalOperationReason Reason { get; }

    /// <summary>Gets the prior block index, when applicable.</summary>
    public int? PreviousIndex { get; }

    /// <summary>Gets the candidate block index, when applicable.</summary>
    public int? CurrentIndex { get; }

    /// <summary>Gets the deleted range or zero-length insertion point.</summary>
    public DocumentTextRange AffectedRange { get; }

    /// <summary>Gets the logical block identity traced through this operation.</summary>
    public BlockIdentity TraceIdentity { get; }

    /// <summary>Gets the canonical candidate payload for insertion.</summary>
    public DocumentBlock? CandidateBlock { get; }
}

/// <summary>Represents a revision-bound physical update plan.</summary>
public sealed class PhysicalUpdatePlan
{
    internal PhysicalUpdatePlan(
        DocumentIdentity identity,
        DocumentRevision requiredRevision,
        DocumentTextRange managedRegion,
        DiffPlan logicalPlan,
        IEnumerable<PhysicalUpdateOperation> operations)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        RequiredRevision = requiredRevision ?? throw new ArgumentNullException(nameof(requiredRevision));
        ManagedRegion = managedRegion ?? throw new ArgumentNullException(nameof(managedRegion));
        LogicalPlan = logicalPlan ?? throw new ArgumentNullException(nameof(logicalPlan));
        ArgumentNullException.ThrowIfNull(operations);
        Operations = Array.AsReadOnly(operations.ToArray());
    }

    /// <summary>Gets the target document identity.</summary>
    public DocumentIdentity Identity { get; }

    /// <summary>Gets the optimistic-concurrency precondition.</summary>
    public DocumentRevision RequiredRevision { get; }

    /// <summary>Gets the validated pre-apply managed region.</summary>
    public DocumentTextRange ManagedRegion { get; }

    /// <summary>Gets the source logical plan.</summary>
    public DiffPlan LogicalPlan { get; }

    /// <summary>Gets physical operations in execution order.</summary>
    public IReadOnlyList<PhysicalUpdateOperation> Operations { get; }

    /// <summary>Gets whether an external mutation is required.</summary>
    public bool IsPublishRequired => Operations.Count > 0;
}

/// <summary>Summarizes a non-mutating physical-plan preview.</summary>
public sealed class PhysicalUpdateDryRunResult
{
    /// <summary>Initializes a dry-run result.</summary>
    public PhysicalUpdateDryRunResult(
        DiffPlan? logicalPlan,
        PhysicalUpdatePlan? physicalPlan,
        IEnumerable<string> conflicts,
        IEnumerable<string>? warnings = null)
    {
        ArgumentNullException.ThrowIfNull(conflicts);
        LogicalPlan = logicalPlan;
        PhysicalPlan = physicalPlan;
        Conflicts = Array.AsReadOnly(conflicts.ToArray());
        Warnings = Array.AsReadOnly((warnings ?? []).ToArray());
    }

    /// <summary>Gets the logical plan.</summary>
    public DiffPlan? LogicalPlan { get; }

    /// <summary>Gets the physical plan when validation succeeded.</summary>
    public PhysicalUpdatePlan? PhysicalPlan { get; }

    /// <summary>Gets stable conflict codes produced without applying changes.</summary>
    public IReadOnlyList<string> Conflicts { get; }

    /// <summary>Gets non-blocking preview warnings.</summary>
    public IReadOnlyList<string> Warnings { get; }

    /// <summary>Gets the logical operation count.</summary>
    public int LogicalOperationCount => LogicalPlan?.Operations.Count ?? 0;

    /// <summary>Gets the physical operation count.</summary>
    public int PhysicalOperationCount => PhysicalPlan?.Operations.Count ?? 0;

    /// <summary>Gets whether publication would be required.</summary>
    public bool IsPublishRequired => LogicalPlan?.IsPublishRequired ?? false;

    /// <summary>Counts logical operations of one kind.</summary>
    public int Count(DiffOperationKind kind) =>
        LogicalPlan?.Operations.Count(item => item.Kind == kind) ?? 0;
}
