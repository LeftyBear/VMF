using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Vmf.Build.ExcelOwnership.Primitives;

internal enum RotAttachmentFailure { TimedOut, Ambiguous }

internal sealed class RotAttachmentException : InvalidOperationException
{
    internal RotAttachmentFailure Failure { get; }
    internal RotAttachmentException(RotAttachmentFailure failure, string message) : base(message) => Failure = failure;
}

internal sealed record RotSentinelAttachmentResult(object RunningObject, int ExactMatchCount);

internal static class RotSentinelAttachment
{
    internal static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);
    internal static readonly TimeSpan AttachmentTimeout = TimeSpan.FromSeconds(20);

    internal static RotSentinelAttachmentResult Attach(string sentinelPath, CancellationToken cancellationToken = default)
        => Attach(sentinelPath, Observe, AttachmentTimeout,
            (delay, token) => Task.Delay(delay, token).GetAwaiter().GetResult(), cancellationToken);

    internal static RotSentinelAttachmentResult Attach(string sentinelPath, Func<string, RotSentinelAttachmentResult?> observe, TimeSpan timeout,
        Action<TimeSpan, CancellationToken> delay, CancellationToken cancellationToken = default)
    {
        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            throw new InvalidOperationException("ROT attachment requires an STA thread.");
        if (!TryNormalizeLocalDrivePath(sentinelPath, out var expected))
            throw new ArgumentException("Sentinel must be a fully qualified local drive path.", nameof(sentinelPath));

        var stopwatch = Stopwatch.StartNew();
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var attached = observe(expected!);
            cancellationToken.ThrowIfCancellationRequested();
            if (attached is not null) return attached;
            var remaining = timeout - stopwatch.Elapsed;
            if (remaining <= TimeSpan.Zero)
                throw new RotAttachmentException(RotAttachmentFailure.TimedOut, "Sentinel ROT attachment timed out.");
            delay(remaining < PollInterval ? remaining : PollInterval, cancellationToken);
        }
    }

    internal static bool IsExactSentinelDisplayName(string? displayName, string expectedCanonicalPath) =>
        TryNormalizeLocalDrivePath(displayName, out var candidate) &&
        StringComparer.OrdinalIgnoreCase.Equals(candidate, expectedCanonicalPath);

    internal static bool TryNormalizeLocalDrivePath(string? value, out string? normalized)
    {
        normalized = null;
        if (string.IsNullOrWhiteSpace(value) || value.IndexOf('\0') >= 0 || value.StartsWith("file:", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("://", StringComparison.Ordinal) && Uri.TryCreate(value, UriKind.Absolute, out _) || value.StartsWith(@"\\", StringComparison.Ordinal) ||
            value.StartsWith(@"\\?\", StringComparison.Ordinal) || value.StartsWith(@"\\.\", StringComparison.Ordinal) ||
            !Path.IsPathFullyQualified(value)) return false;
        try
        {
            var root = Path.GetPathRoot(value);
            if (root is null || root.Length != 3 || !char.IsLetter(root[0]) || root[1] != ':' ||
                root[2] != Path.DirectorySeparatorChar && root[2] != Path.AltDirectorySeparatorChar) return false;
            normalized = Path.GetFullPath(value);
            return true;
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return false;
        }
    }

    private static RotSentinelAttachmentResult? Observe(string expectedCanonicalPath)
    {
        IRunningObjectTable? rot = null;
        IEnumMoniker? enumerator = null;
        IBindCtx? bindContext = null;
        IMoniker? match = null;
        try
        {
            Marshal.ThrowExceptionForHR(GetRunningObjectTable(0, out rot));
            rot.EnumRunning(out enumerator);
            Marshal.ThrowExceptionForHR(CreateBindCtx(0, out bindContext));
            enumerator.Reset();
            var monikers = new IMoniker[1];
            while (enumerator.Next(1, monikers, IntPtr.Zero) == 0)
            {
                var current = monikers[0];
                try
                {
                    current.GetDisplayName(bindContext, null, out var displayName);
                    if (!IsExactSentinelDisplayName(displayName, expectedCanonicalPath)) continue;
                    if (match is not null)
                        throw new RotAttachmentException(RotAttachmentFailure.Ambiguous, "More than one exact sentinel ROT entry was observed.");
                    match = current;
                    current = null!;
                }
                finally
                {
                    Release(current);
                    monikers[0] = null!;
                }
            }
            if (match is null) return null;
            rot.GetObject(match, out var runningObject);
            return new RotSentinelAttachmentResult(
                runningObject ?? throw new InvalidOperationException("The exact sentinel ROT entry returned no running object."), 1);
        }
        finally
        {
            Release(match);
            Release(enumerator);
            Release(bindContext);
            Release(rot);
        }
    }

    private static void Release(object? value)
    {
        if (value is not null && Marshal.IsComObject(value)) Marshal.FinalReleaseComObject(value);
    }

    [DllImport("ole32.dll")]
    private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable runningObjectTable);

    [DllImport("ole32.dll")]
    private static extern int CreateBindCtx(int reserved, out IBindCtx bindContext);
}
