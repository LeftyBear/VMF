using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Provides managed-snapshot and revision-bound operations for Google Docs.</summary>
public interface IGoogleDocsManagedDocumentGateway
{
    /// <summary>Reads one managed document snapshot reconstructed from Google Docs.</summary>
    Task<ManagedDocumentSnapshot> GetSnapshotAsync(
        DocumentIdentity identity,
        CancellationToken cancellationToken);

    /// <summary>Applies one revision-bound physical plan through Google Docs batchUpdate.</summary>
    Task<PhysicalApplyReceipt> ApplyAsync(
        PhysicalUpdatePlan plan,
        CancellationToken cancellationToken);
}

/// <summary>Adapts a Google Docs managed-document gateway without enabling live writes by default.</summary>
public sealed class GoogleDocsPhysicalUpdateAdapter : IManagedDocumentAdapter
{
    private readonly IGoogleDocsManagedDocumentGateway gateway;
    private readonly bool liveUpdatesEnabled;

    /// <summary>Initializes a read-capable adapter with live updates disabled.</summary>
    public GoogleDocsPhysicalUpdateAdapter(IGoogleDocsManagedDocumentGateway gateway)
        : this(gateway, liveUpdatesEnabled: false)
    {
    }

    /// <summary>Initializes an adapter with an explicit live-update decision.</summary>
    public GoogleDocsPhysicalUpdateAdapter(
        IGoogleDocsManagedDocumentGateway gateway,
        bool liveUpdatesEnabled)
    {
        this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        this.liveUpdatesEnabled = liveUpdatesEnabled;
    }

    /// <inheritdoc />
    public Task<ManagedDocumentSnapshot> GetSnapshotAsync(
        DocumentIdentity identity,
        CancellationToken cancellationToken) => gateway.GetSnapshotAsync(identity, cancellationToken);

    /// <inheritdoc />
    public Task<PhysicalApplyReceipt> ApplyAsync(
        PhysicalUpdatePlan plan,
        CancellationToken cancellationToken)
    {
        if (!liveUpdatesEnabled)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.ApplicationFailed,
                "Live Google Docs physical updates are disabled by default.");
        }

        return gateway.ApplyAsync(plan, cancellationToken);
    }
}
