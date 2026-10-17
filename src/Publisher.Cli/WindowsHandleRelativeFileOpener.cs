using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

// Opens every path element relative to an already-open directory handle. Reparse points are
// opened themselves and rejected, so neither validation nor the final file open follows them.
internal sealed class WindowsHandleRelativeFileOpener
{
    private const uint GenericRead = 0x80000000;
    private const uint FileReadAttributes = 0x00000080;
    private const uint Synchronize = 0x00100000;
    private const uint ObjCaseInsensitive = 0x00000040;
    private const uint FileAttributeReparsePoint = 0x00000400;
    private const uint FileFlagBackupSemantics = 0x02000000;
    private const uint FileFlagOpenReparsePoint = 0x00200000;
    private const uint OpenExisting = 3;
    private const uint FileOpen = 1;
    private const uint FileDirectoryFile = 0x00000001;
    private const uint FileNonDirectoryFile = 0x00000040;
    private const uint FileOpenReparsePoint = 0x00200000;
    private const uint FileSynchronousIoNonAlert = 0x00000020;
    private const int FileAttributeTagInfo = 9;

    private readonly Action? parentOpened;

    internal WindowsHandleRelativeFileOpener(Action? parentOpened = null) =>
        this.parentOpened = parentOpened;

    internal Stream OpenRead(string fullPath)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Handle-relative image access requires Windows.");
        }

        var normalized = Path.GetFullPath(fullPath);
        var root = Path.GetPathRoot(normalized);
        if (string.IsNullOrEmpty(root) || root.StartsWith(@"\\", StringComparison.Ordinal))
        {
            throw new IOException("The image path does not have a supported local volume root.");
        }

        var relative = normalized[root.Length..];
        var elements = relative.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries);
        if (elements.Length == 0)
        {
            throw new IOException("The image path does not name a file.");
        }

        SafeFileHandle? current = OpenRoot(root);
        try
        {
            RejectReparsePoint(current);
            for (var index = 0; index < elements.Length - 1; index++)
            {
                var next = OpenRelative(current, elements[index], isDirectory: true);
                current.Dispose();
                current = next;
                RejectReparsePoint(current);
            }

            parentOpened?.Invoke();
            var file = OpenRelative(current, elements[^1], isDirectory: false);
            try
            {
                RejectReparsePoint(file);
                return new FileStream(file, FileAccess.Read, bufferSize: 4096, isAsync: false);
            }
            catch
            {
                file.Dispose();
                throw;
            }
        }
        finally
        {
            current?.Dispose();
        }
    }

    private static SafeFileHandle OpenRoot(string root)
    {
        var extendedRoot = @"\\?\" + root;
        var handle = CreateFileW(
            extendedRoot,
            FileReadAttributes | Synchronize,
            FileShare.ReadWrite | FileShare.Delete,
            IntPtr.Zero,
            OpenExisting,
            FileFlagBackupSemantics | FileFlagOpenReparsePoint,
            IntPtr.Zero);
        if (handle.IsInvalid)
        {
            var error = Marshal.GetLastWin32Error();
            handle.Dispose();
            throw new IOException("The volume root could not be opened.", new Win32Exception(error));
        }

        return handle;
    }

    private static SafeFileHandle OpenRelative(SafeFileHandle parent, string name, bool isDirectory)
    {
        if (name is "." or ".." || name.Contains(':'))
        {
            throw new IOException("A path element is not supported.");
        }

        var nameBuffer = Marshal.StringToHGlobalUni(name);
        var unicodeStringPointer = IntPtr.Zero;
        try
        {
            var unicodeString = new UnicodeString
            {
                Length = checked((ushort)(name.Length * sizeof(char))),
                MaximumLength = checked((ushort)((name.Length + 1) * sizeof(char))),
                Buffer = nameBuffer,
            };
            unicodeStringPointer = Marshal.AllocHGlobal(Marshal.SizeOf<UnicodeString>());
            Marshal.StructureToPtr(unicodeString, unicodeStringPointer, fDeleteOld: false);
            var attributes = new ObjectAttributes
            {
                Length = Marshal.SizeOf<ObjectAttributes>(),
                RootDirectory = IntPtr.Zero,
                ObjectName = unicodeStringPointer,
                Attributes = ObjCaseInsensitive,
            };
            var options = FileOpenReparsePoint | FileSynchronousIoNonAlert |
                (isDirectory ? FileDirectoryFile : FileNonDirectoryFile);
            var parentAddRef = false;
            int status;
            SafeFileHandle handle;
            try
            {
                // RootDirectory is embedded as a raw handle in OBJECT_ATTRIBUTES, so the
                // SafeHandle marshaller cannot keep it alive for the native call.
                parent.DangerousAddRef(ref parentAddRef);
                attributes.RootDirectory = parent.DangerousGetHandle();
                status = NtCreateFile(
                    out handle,
                    (isDirectory ? FileReadAttributes : GenericRead) | Synchronize,
                    ref attributes,
                    out _,
                    IntPtr.Zero,
                    0,
                    isDirectory ? FileShare.ReadWrite | FileShare.Delete : FileShare.Read,
                    FileOpen,
                    options,
                    IntPtr.Zero,
                    0);
            }
            finally
            {
                if (parentAddRef)
                {
                    parent.DangerousRelease();
                }
            }

            if (status < 0)
            {
                handle?.Dispose();
                var error = unchecked((int)RtlNtStatusToDosError(status));
                throw new IOException("A path element could not be opened.", new Win32Exception(error));
            }

            return handle;
        }
        finally
        {
            if (unicodeStringPointer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unicodeStringPointer);
            }

            Marshal.FreeHGlobal(nameBuffer);
        }
    }

    private static void RejectReparsePoint(SafeFileHandle handle)
    {
        if (!GetFileInformationByHandleEx(
            handle,
            FileAttributeTagInfo,
            out var information,
            (uint)Marshal.SizeOf<FileAttributeTagInformation>()))
        {
            throw new IOException(
                "A path element could not be inspected.",
                new Win32Exception(Marshal.GetLastWin32Error()));
        }

        if ((information.FileAttributes & FileAttributeReparsePoint) != 0)
        {
            throw new IOException("A reparse point was rejected.");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UnicodeString
    {
        internal ushort Length;
        internal ushort MaximumLength;
        internal IntPtr Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ObjectAttributes
    {
        internal int Length;
        internal IntPtr RootDirectory;
        internal IntPtr ObjectName;
        internal uint Attributes;
        internal IntPtr SecurityDescriptor;
        internal IntPtr SecurityQualityOfService;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IoStatusBlock
    {
        internal IntPtr Status;
        internal nuint Information;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FileAttributeTagInformation
    {
        internal uint FileAttributes;
        internal uint ReparseTag;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFileW(
        string fileName,
        uint desiredAccess,
        FileShare shareMode,
        IntPtr securityAttributes,
        uint creationDisposition,
        uint flagsAndAttributes,
        IntPtr templateFile);

    [DllImport("ntdll.dll")]
    private static extern int NtCreateFile(
        out SafeFileHandle fileHandle,
        uint desiredAccess,
        ref ObjectAttributes objectAttributes,
        out IoStatusBlock ioStatusBlock,
        IntPtr allocationSize,
        uint fileAttributes,
        FileShare shareAccess,
        uint createDisposition,
        uint createOptions,
        IntPtr eaBuffer,
        uint eaLength);

    [DllImport("ntdll.dll")]
    private static extern uint RtlNtStatusToDosError(int status);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetFileInformationByHandleEx(
        SafeFileHandle fileHandle,
        int fileInformationClass,
        out FileAttributeTagInformation fileInformation,
        uint bufferSize);
}
