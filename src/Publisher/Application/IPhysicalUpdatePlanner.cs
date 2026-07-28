using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Converts logical block differences into revision-bound physical operations.</summary>
public interface IPhysicalUpdatePlanner
{
    /// <summary>Validates a current managed snapshot against the verified baseline.</summary>
    void ValidateSnapshot(
        VerifiedPublishState? baseline,
        ManagedDocumentSnapshot snapshot);

    /// <summary>Creates a deterministic physical plan within an explicit managed region.</summary>
    PhysicalUpdatePlan CreatePlan(
        VerifiedPublishState? baseline,
        PublishCandidate candidate,
        DiffPlan logicalPlan,
        ManagedDocumentSnapshot snapshot);
}

/// <summary>Represents the revision returned after a physical application.</summary>
public sealed class PhysicalApplyReceipt
{
    /// <summary>Initializes an apply receipt.</summary>
    public PhysicalApplyReceipt(DocumentRevision revision)
        : this(revision, [])
    {
    }

    /// <summary>Initializes an apply receipt with applied operation identifiers.</summary>
    public PhysicalApplyReceipt(DocumentRevision revision, IEnumerable<string> operationIds)
    {
        Revision = revision ?? throw new ArgumentNullException(nameof(revision));
        ArgumentNullException.ThrowIfNull(operationIds);
        var items = operationIds.ToArray();
        if (items.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Operation IDs must not be empty.", nameof(operationIds));
        }

        OperationIds = Array.AsReadOnly(items);
    }

    /// <summary>Gets the revision immediately after application.</summary>
    public DocumentRevision Revision { get; }

    /// <summary>Gets operation identifiers accepted by the adapter, used for idempotent recovery.</summary>
    public IReadOnlyList<string> OperationIds { get; }
}

/// <summary>Reads and updates one managed document without exposing Google SDK types.</summary>
public interface IManagedDocumentAdapter
{
    /// <summary>Reads the current managed snapshot.</summary>
    Task<ManagedDocumentSnapshot> GetSnapshotAsync(
        DocumentIdentity identity,
        CancellationToken cancellationToken);

    /// <summary>Applies a physical plan only when its required revision still matches.</summary>
    Task<PhysicalApplyReceipt> ApplyAsync(
        PhysicalUpdatePlan plan,
        CancellationToken cancellationToken);
}
