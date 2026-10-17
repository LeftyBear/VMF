namespace Vmf.Publisher.UnitTests;

public enum SymbolicLinkKind
{
    File,
    Directory,
}

/// <summary>
/// Runs a real symbolic-link test only when the current process can create the
/// required link type. The probe neither elevates nor changes machine policy.
/// </summary>
public sealed class WindowsSymbolicLinkFactAttribute : FactAttribute
{
    private static readonly Lazy<string?> FileSkipReason =
        new(() => ProbeCreation(SymbolicLinkKind.File));
    private static readonly Lazy<string?> DirectorySkipReason =
        new(() => ProbeCreation(SymbolicLinkKind.Directory));

    public WindowsSymbolicLinkFactAttribute(SymbolicLinkKind kind)
    {
        Skip = kind switch
        {
            SymbolicLinkKind.File => FileSkipReason.Value,
            SymbolicLinkKind.Directory => DirectorySkipReason.Value,
            _ => "Unsupported symbolic-link probe kind.",
        };
    }

    private static string? ProbeCreation(SymbolicLinkKind kind)
    {
        if (!OperatingSystem.IsWindows())
        {
            return "Requires Windows symbolic-link semantics.";
        }

        var root = Path.Combine(Path.GetTempPath(), $"vmf-p2-41-probe-{Guid.NewGuid():N}");
        var link = Path.Combine(root, "link");
        try
        {
            Directory.CreateDirectory(root);
            if (kind == SymbolicLinkKind.Directory)
            {
                var target = Path.Combine(root, "target-directory");
                Directory.CreateDirectory(target);
                Directory.CreateSymbolicLink(link, target);
                Directory.Delete(link);
            }
            else
            {
                var target = Path.Combine(root, "target-file");
                File.WriteAllText(target, "probe");
                File.CreateSymbolicLink(link, target);
                File.Delete(link);
            }

            return null;
        }
        catch (Exception exception) when (
            exception is UnauthorizedAccessException or IOException or PlatformNotSupportedException)
        {
            return $"Real {kind.ToString().ToLowerInvariant()} symbolic-link creation is unavailable " +
                $"to this non-elevating test process ({exception.GetType().Name}, " +
                $"HRESULT 0x{exception.HResult:X8}).";
        }
        finally
        {
            if (File.Exists(link))
            {
                File.Delete(link);
            }
            else if (Directory.Exists(link))
            {
                Directory.Delete(link);
            }

            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }
}
