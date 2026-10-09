namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed record EvidencePersistenceConfirmation(
    Guid InvocationId,
    string ArtifactKind,
    string ArtifactPath,
    long ByteLength,
    string Sha256,
    DateTimeOffset TargetRecordedUtc,
    DateTimeOffset ConfirmedUtc);
