using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsPhysicalUpdateAdapterTests
{
    [Fact]
    public async Task GetSnapshotAsync_DelegatesReadWithoutEnablingWrites()
    {
        var gateway = new RecordingGateway();
        var adapter = new GoogleDocsPhysicalUpdateAdapter(gateway);

        var snapshot = await adapter.GetSnapshotAsync(Identity(), default);

        Assert.Same(gateway.Snapshot, snapshot);
        Assert.Equal(1, gateway.ReadCount);
    }

    [Fact]
    public async Task ApplyAsync_IsDisabledByDefault()
    {
        var gateway = new RecordingGateway();
        var adapter = new GoogleDocsPhysicalUpdateAdapter(gateway);

        var exception = await Assert.ThrowsAsync<PhysicalUpdateException>(
            () => adapter.ApplyAsync(Plan(), default));

        Assert.Equal(UpdateErrorCodes.ApplicationFailed, exception.Code);
        Assert.Equal(0, gateway.ApplyCount);
    }

    [Fact]
    public async Task ApplyAsync_DelegatesOnlyWhenExplicitlyEnabled()
    {
        var gateway = new RecordingGateway();
        var adapter = new GoogleDocsPhysicalUpdateAdapter(gateway, liveUpdatesEnabled: true);

        var receipt = await adapter.ApplyAsync(Plan(), default);

        Assert.Equal(2, receipt.Revision.Sequence);
        Assert.Equal(1, gateway.ApplyCount);
    }

    private static PhysicalUpdatePlan Plan()
    {
        var fingerprint = new PublishFingerprint("v1:sha256:" + new string('a', 64));
        var logical = new DiffPlan(fingerprint, fingerprint, true, Array.Empty<DiffOperation>());
        return new PhysicalUpdatePlan(
            Identity(),
            new DocumentRevision("revision-1", 1),
            new DocumentTextRange(10, 10),
            logical,
            Array.Empty<PhysicalUpdateOperation>());
    }

    private static DocumentIdentity Identity() =>
        new("publication", "document", "google-document", DocumentState.Active);

    private sealed class RecordingGateway : IGoogleDocsManagedDocumentGateway
    {
        internal RecordingGateway()
        {
            Snapshot = new ManagedDocumentSnapshot(
                Identity(),
                new DocumentRevision("revision-1", 1),
                new DocumentTextRange(10, 10),
                "v1:sha256:" + new string('a', 64),
                Array.Empty<ManagedBlockSnapshot>());
        }

        internal ManagedDocumentSnapshot Snapshot { get; }

        internal int ReadCount { get; private set; }

        internal int ApplyCount { get; private set; }

        public Task<ManagedDocumentSnapshot> GetSnapshotAsync(
            DocumentIdentity identity,
            CancellationToken cancellationToken)
        {
            ReadCount++;
            return Task.FromResult(Snapshot);
        }

        public Task<PhysicalApplyReceipt> ApplyAsync(
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken)
        {
            ApplyCount++;
            return Task.FromResult(new PhysicalApplyReceipt(new DocumentRevision("revision-2", 2)));
        }
    }
}
