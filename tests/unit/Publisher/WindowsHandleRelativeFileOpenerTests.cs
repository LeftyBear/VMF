using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vmf.Publisher.UnitTests;

public sealed class WindowsHandleRelativeFileOpenerTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "vmf-b3-" + Guid.NewGuid().ToString("N"));

    public WindowsHandleRelativeFileOpenerTests() => Directory.CreateDirectory(root);

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void OpenRead_ReadsOrdinaryFileAndReleasesHandle()
    {
        var path = Write("images/image.png", [1, 2, 3, 4]);

        using (var stream = new WindowsHandleRelativeFileOpener().OpenRead(path))
        {
            Assert.Equal([1, 2, 3, 4], ReadAll(stream));
        }

        File.Delete(path);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void NativeLayoutsMatchWindowsAbi()
    {
        var owner = typeof(WindowsHandleRelativeFileOpener);
        var unicodeString = Nested(owner, "UnicodeString");
        var objectAttributes = Nested(owner, "ObjectAttributes");
        var ioStatusBlock = Nested(owner, "IoStatusBlock");

        AssertLayout(unicodeString, IntPtr.Size == 8 ? 16 : 8,
            ("Length", 0), ("MaximumLength", 2), ("Buffer", IntPtr.Size == 8 ? 8 : 4));
        AssertLayout(objectAttributes, IntPtr.Size == 8 ? 48 : 24,
            ("Length", 0),
            ("RootDirectory", IntPtr.Size == 8 ? 8 : 4),
            ("ObjectName", IntPtr.Size == 8 ? 16 : 8),
            ("Attributes", IntPtr.Size == 8 ? 24 : 12),
            ("SecurityDescriptor", IntPtr.Size == 8 ? 32 : 16),
            ("SecurityQualityOfService", IntPtr.Size == 8 ? 40 : 20));
        AssertLayout(ioStatusBlock, IntPtr.Size == 8 ? 16 : 8,
            ("Status", 0), ("Information", IntPtr.Size));
    }

    [Fact]
    public void OpenRead_RejectsJunction()
    {
        var target = Path.Combine(root, "target");
        Directory.CreateDirectory(target);
        Write("target/image.png", [1]);
        var junction = Path.Combine(root, "images");
        CreateJunction(junction, target);

        Assert.Throws<IOException>(() =>
            new WindowsHandleRelativeFileOpener().OpenRead(Path.Combine(junction, "image.png")));

        Directory.Delete(junction);
    }

    [Fact]
    public void OpenRead_RejectsAncestorJunction()
    {
        var target = Path.Combine(root, "target");
        Directory.CreateDirectory(Path.Combine(target, "nested"));
        Write("target/nested/image.png", [1]);
        var junction = Path.Combine(root, "ancestor");
        CreateJunction(junction, target);

        Assert.Throws<IOException>(() => new WindowsHandleRelativeFileOpener().OpenRead(
            Path.Combine(junction, "nested", "image.png")));

        Directory.Delete(junction);
    }

    [Fact]
    public void OpenRead_ParentReplacementCannotRedirectFinalOpen()
    {
        var parent = Path.Combine(root, "images");
        Directory.CreateDirectory(parent);
        Write("images/image.png", [1, 2, 3]);
        var retained = Path.Combine(root, "retained");
        var opener = new WindowsHandleRelativeFileOpener(() =>
        {
            Directory.Move(parent, retained);
            Directory.CreateDirectory(parent);
            File.WriteAllBytes(Path.Combine(parent, "image.png"), [9, 9, 9]);
        });

        using var stream = opener.OpenRead(Path.Combine(parent, "image.png"));

        Assert.Equal([1, 2, 3], ReadAll(stream));
        Assert.Equal([9, 9, 9], File.ReadAllBytes(Path.Combine(parent, "image.png")));
    }

    [Fact]
    public void OpenRead_SharingViolationFailsAndDoesNotRetainParentHandles()
    {
        var path = Write("images/image.png", [1]);
        using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            Assert.Throws<IOException>(() => new WindowsHandleRelativeFileOpener().OpenRead(path));
        }

        Directory.Move(Path.GetDirectoryName(path)!, Path.Combine(root, "moved"));
        Assert.True(File.Exists(Path.Combine(root, "moved", "image.png")));
    }

    [Fact]
    public void OpenRead_MissingFileConvertsNtStatusAndReleasesParentHandles()
    {
        var parent = Path.Combine(root, "images");
        Directory.CreateDirectory(parent);

        var error = Assert.Throws<IOException>(() => new WindowsHandleRelativeFileOpener().OpenRead(
            Path.Combine(parent, "missing.png")));

        var native = Assert.IsType<Win32Exception>(error.InnerException);
        Assert.Equal(2, native.NativeErrorCode);
        Directory.Move(parent, Path.Combine(root, "moved"));
    }

    [Fact]
    public void OpenRead_CallbackCancellationReleasesParentHandles()
    {
        var path = Write("images/image.png", [1]);
        var parent = Path.GetDirectoryName(path)!;
        var opener = new WindowsHandleRelativeFileOpener(() => throw new OperationCanceledException());

        Assert.Throws<OperationCanceledException>(() => opener.OpenRead(path));

        Directory.Move(parent, Path.Combine(root, "moved"));
    }

    [Fact]
    public void OpenRead_OpenFileCannotBeReplacedUntilStreamIsDisposed()
    {
        var path = Write("images/image.png", [1]);

        using (var stream = new WindowsHandleRelativeFileOpener().OpenRead(path))
        {
            Assert.Throws<IOException>(() => File.Move(path, path + ".moved"));
            Assert.Equal(1, stream.ReadByte());
        }

        File.Move(path, path + ".moved");
        Assert.True(File.Exists(path + ".moved"));
    }

    [Fact]
    public void OpenRead_ConcurrentCallsUseIndependentOwnedHandles()
    {
        var path = Write("images/image.png", [1, 2, 3]);

        Parallel.For(0, 32, _ =>
        {
            using var stream = new WindowsHandleRelativeFileOpener().OpenRead(path);
            Assert.Equal([1, 2, 3], ReadAll(stream));
        });

        File.Delete(path);
        Assert.False(File.Exists(path));
    }

    private string Write(string relativePath, byte[] bytes)
    {
        var path = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    private static byte[] ReadAll(Stream stream)
    {
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }

    private static Type Nested(Type owner, string name) =>
        owner.GetNestedType(name, BindingFlags.NonPublic) ??
        throw new InvalidOperationException($"Missing nested type {name}.");

    private static void AssertLayout(Type type, int size, params (string Name, int Offset)[] fields)
    {
        Assert.Equal(size, Marshal.SizeOf(type));
        foreach (var (name, offset) in fields)
        {
            Assert.Equal(offset, Marshal.OffsetOf(type, name).ToInt32());
        }
    }

    private static void CreateJunction(string junction, string target)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            ArgumentList = { "/d", "/c", "mklink", "/J", junction, target },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        }) ?? throw new InvalidOperationException("Could not start mklink.");
        process.WaitForExit();
        Assert.True(process.ExitCode == 0, $"Junction creation failed (exit {process.ExitCode}).");
    }
}
