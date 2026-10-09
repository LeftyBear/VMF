namespace Vmf.Build.ExcelOwnership.Primitives;

public sealed record Ownership(int ProcessId, DateTimeOffset ProcessStartedUtc, string ExecutablePath, Guid InvocationId, string SentinelPath);
