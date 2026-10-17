using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vmf.Publisher.UnitTests;

[Trait("Privilege", "StandardUser")]
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

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    public void NativeLayoutsMatchWindowsAbiForX86AndX64(int pointerSize)
    {
        var owner = typeof(WindowsNativeFile);
        var unicodeString = Nested(owner, nameof(WindowsNativeFile.UnicodeString));
        var objectAttributes = Nested(owner, nameof(WindowsNativeFile.ObjectAttributes));
        var ioStatusBlock = Nested(owner, nameof(WindowsNativeFile.IoStatusBlock));

        AssertPortableLayout(unicodeString, pointerSize, pointerSize == 8 ? 16 : 8,
            ("Length", 0), ("MaximumLength", 2), ("Buffer", pointerSize == 8 ? 8 : 4));
        AssertPortableLayout(objectAttributes, pointerSize, pointerSize == 8 ? 48 : 24,
            ("Length", 0),
            ("RootDirectory", pointerSize == 8 ? 8 : 4),
            ("ObjectName", pointerSize == 8 ? 16 : 8),
            ("Attributes", pointerSize == 8 ? 24 : 12),
            ("SecurityDescriptor", pointerSize == 8 ? 32 : 16),
            ("SecurityQualityOfService", pointerSize == 8 ? 40 : 20));
        AssertPortableLayout(ioStatusBlock, pointerSize, pointerSize == 8 ? 16 : 8,
            ("Status", 0), ("Information", pointerSize));
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
    public void OpenRead_ExistingWriterIsRejectedEvenWhenItSharesReadAndWrite()
    {
        var path = Write("images/image.png", [1, 2, 3]);
        var parent = Path.GetDirectoryName(path)!;

        using (var writer = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        {
            writer.Position = 0;
            writer.Write([9, 9, 9]);
            writer.Flush(flushToDisk: true);

            Assert.Throws<IOException>(() => new WindowsHandleRelativeFileOpener().OpenRead(path));
        }

        Directory.Move(parent, Path.Combine(root, "moved"));
        Assert.Equal([9, 9, 9], File.ReadAllBytes(Path.Combine(root, "moved", "image.png")));
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
        Assert.DoesNotContain("missing.png", error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(parent, error.Message, StringComparison.OrdinalIgnoreCase);
        Directory.Move(parent, Path.Combine(root, "moved"));
    }

    [Fact]
    public void OpenRead_InjectedFileReparseAttributeIsRejectedWithoutPathDetails()
    {
        var path = Write("images/image.png", [1]);
        var relative = Path.GetFullPath(path)[Path.GetPathRoot(path)!.Length..];
        var expectedInspections = relative.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries).Length + 1;
        var inspections = 0;
        var opener = new WindowsHandleRelativeFileOpener(
            getAttributeTagInformation: _ =>
            {
                inspections++;
                return new WindowsNativeFile.FileAttributeTagInformation
                {
                    FileAttributes = inspections == expectedInspections
                        ? WindowsNativeFile.FileAttributeReparsePoint
                        : 0,
                };
            });

        var error = Assert.Throws<IOException>(() => opener.OpenRead(path));

        Assert.Equal(expectedInspections, inspections);
        Assert.Equal("A reparse point was rejected.", error.Message);
        Assert.DoesNotContain(path, error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void OpenRead_InjectedDirectoryReparseAttributeStopsBeforeFileOpen()
    {
        var path = Write("images/image.png", [1]);
        var relative = Path.GetFullPath(path)[Path.GetPathRoot(path)!.Length..];
        var fileInspection = relative.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries).Length + 1;
        var directoryInspection = fileInspection - 1;
        var inspections = 0;
        var opener = new WindowsHandleRelativeFileOpener(
            getAttributeTagInformation: _ =>
            {
                inspections++;
                return new WindowsNativeFile.FileAttributeTagInformation
                {
                    FileAttributes = inspections == directoryInspection
                        ? WindowsNativeFile.FileAttributeReparsePoint
                        : 0,
                };
            });

        var error = Assert.Throws<IOException>(() => opener.OpenRead(path));

        Assert.Equal(directoryInspection, inspections);
        Assert.Equal("A reparse point was rejected.", error.Message);
        Assert.DoesNotContain(path, error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void OpenRead_InjectedAttributeInspectionFailurePreservesGenericDiagnostic()
    {
        var path = Write("images/image.png", [1]);
        var failure = new IOException("A path element could not be inspected.",
            new Win32Exception(5, "secret-path-details"));
        var opener = new WindowsHandleRelativeFileOpener(
            getAttributeTagInformation: _ => throw failure);

        var error = Assert.Throws<IOException>(() => opener.OpenRead(path));

        Assert.Same(failure, error);
        Assert.Equal("A path element could not be inspected.", error.Message);
        Assert.DoesNotContain("secret", error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(path, error.Message, StringComparison.OrdinalIgnoreCase);
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
    public void OpenRead_OpenFileCannotBeModifiedUntilStreamIsDisposed()
    {
        var path = Write("images/image.png", [1, 2, 3]);

        using (var stream = new WindowsHandleRelativeFileOpener().OpenRead(path))
        {
            Assert.Throws<IOException>(() => File.Open(
                path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
            Assert.Equal([1, 2, 3], ReadAll(stream));
        }

        File.WriteAllBytes(path, [9, 9, 9]);
        Assert.Equal([9, 9, 9], File.ReadAllBytes(path));
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

    private static void AssertPortableLayout(Type type, int pointerSize, int size,
        params (string Name, int Offset)[] fields)
    {
        var actual = CalculateSequentialLayout(type, pointerSize);
        Assert.Equal(size, actual.Size);
        foreach (var (name, offset) in fields)
        {
            Assert.Equal(offset, actual.Offsets[name]);
        }

        if (pointerSize == IntPtr.Size)
        {
            Assert.Equal(size, Marshal.SizeOf(type));
            foreach (var (name, offset) in fields)
            {
                Assert.Equal(offset, Marshal.OffsetOf(type, name).ToInt32());
            }
        }
    }

    private static (int Size, Dictionary<string, int> Offsets) CalculateSequentialLayout(
        Type type, int pointerSize)
    {
        var offsets = new Dictionary<string, int>();
        var position = 0;
        var maximumAlignment = 1;
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                     .OrderBy(field => field.MetadataToken))
        {
            var size = field.FieldType == typeof(IntPtr) || field.FieldType == typeof(UIntPtr)
                ? pointerSize
                : Marshal.SizeOf(field.FieldType);
            var alignment = Math.Min(size, 8);
            maximumAlignment = Math.Max(maximumAlignment, alignment);
            position = Align(position, alignment);
            offsets.Add(field.Name, position);
            position += size;
        }

        return (Align(position, maximumAlignment), offsets);
    }

    private static int Align(int value, int alignment) =>
        checked((value + alignment - 1) / alignment * alignment);
}

[Trait("Privilege", "ReparsePointCreation")]
public sealed class WindowsHandleRelativeFileOpenerReparsePointTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "vmf-b3-link-" + Guid.NewGuid().ToString("N"));

    public WindowsHandleRelativeFileOpenerReparsePointTests() => Directory.CreateDirectory(root);

    public void Dispose()
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void OpenRead_RejectsJunction()
    {
        var target = CreateTarget("target/image.png");
        var junction = Path.Combine(root, "images");
        CreateJunction(junction, Path.GetDirectoryName(target)!);
        try
        {
            Assert.Throws<IOException>(() =>
                new WindowsHandleRelativeFileOpener().OpenRead(Path.Combine(junction, "image.png")));
        }
        finally
        {
            Directory.Delete(junction);
        }
    }

    [Fact]
    public void OpenRead_RejectsAncestorJunction()
    {
        CreateTarget("target/nested/image.png");
        var junction = Path.Combine(root, "ancestor");
        CreateJunction(junction, Path.Combine(root, "target"));
        try
        {
            Assert.Throws<IOException>(() => new WindowsHandleRelativeFileOpener().OpenRead(
                Path.Combine(junction, "nested", "image.png")));
        }
        finally
        {
            Directory.Delete(junction);
        }
    }

    [Fact(Skip = "Requires Windows symbolic-link creation privilege; run in the privileged reparse-point lane.")]
    public void OpenRead_RejectsFileSymbolicLink()
    {
        var target = CreateTarget("target/image.png");
        var link = Path.Combine(root, "image.png");
        File.CreateSymbolicLink(link, target);

        Assert.Throws<IOException>(() => new WindowsHandleRelativeFileOpener().OpenRead(link));
    }

    [Fact(Skip = "Requires Windows symbolic-link creation privilege; run in the privileged reparse-point lane.")]
    public void OpenRead_RejectsDirectorySymbolicLink()
    {
        var target = CreateTarget("target/image.png");
        var link = Path.Combine(root, "images");
        Directory.CreateSymbolicLink(link, Path.GetDirectoryName(target)!);

        Assert.Throws<IOException>(() =>
            new WindowsHandleRelativeFileOpener().OpenRead(Path.Combine(link, "image.png")));
    }

    private string CreateTarget(string relativePath)
    {
        var path = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllBytes(path, [1]);
        return path;
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
        Assert.True(process.ExitCode == 0,
            $"Junction creation failed (exit {process.ExitCode}): {process.StandardError.ReadToEnd()}");
    }
}
