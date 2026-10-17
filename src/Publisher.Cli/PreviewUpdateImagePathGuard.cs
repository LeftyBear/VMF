using Vmf.Publisher.Application;

internal sealed class PreviewUpdateImagePathGuard
{
    private readonly Func<string, FileAttributes> getAttributes;

    internal PreviewUpdateImagePathGuard()
        : this(File.GetAttributes)
    {
    }

    internal PreviewUpdateImagePathGuard(Func<string, FileAttributes> getAttributes)
    {
        this.getAttributes = getAttributes ?? throw new ArgumentNullException(nameof(getAttributes));
    }

    internal void Validate(string imagePath, string markdownPath)
    {
        try
        {
            if (Uri.TryCreate(imagePath, UriKind.Absolute, out var uri) && uri.IsFile)
            {
                throw InvalidPath();
            }

            if (Path.IsPathRooted(imagePath) || Path.IsPathFullyQualified(imagePath))
            {
                throw InvalidPath();
            }

            var markdownFullPath = Path.GetFullPath(markdownPath);
            var baseDirectory = Path.GetDirectoryName(markdownFullPath)
                ?? throw new InvalidOperationException("Markdown path has no parent directory.");
            var normalizedBase = Path.TrimEndingDirectorySeparator(Path.GetFullPath(baseDirectory));
            var normalizedImage = Path.GetFullPath(imagePath, normalizedBase);
            var basePrefix = normalizedBase + Path.DirectorySeparatorChar;
            if (!normalizedImage.StartsWith(basePrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw InvalidPath();
            }

            ValidatePathToRoot(normalizedBase, allowMissingLeaf: false);
            ValidatePathToRoot(normalizedImage, allowMissingLeaf: true);
        }
        catch (PublishPipelineException)
        {
            throw;
        }
        catch (Exception exception) when (IsPathInspectionException(exception))
        {
            throw InvalidPath(exception);
        }
    }

    internal void ValidateResolved(string imagePath, string markdownPath)
    {
        try
        {
            var markdownFullPath = Path.GetFullPath(markdownPath);
            var baseDirectory = Path.GetDirectoryName(markdownFullPath)
                ?? throw new InvalidOperationException("Markdown path has no parent directory.");
            ValidatePathToRoot(Path.GetFullPath(baseDirectory), allowMissingLeaf: false);
            ValidatePathToRoot(Path.GetFullPath(imagePath), allowMissingLeaf: false);
        }
        catch (PublishPipelineException)
        {
            throw;
        }
        catch (Exception exception) when (IsPathInspectionException(exception))
        {
            throw InvalidPath(exception);
        }
    }

    private void ValidatePathToRoot(string path, bool allowMissingLeaf)
    {
        var current = Path.TrimEndingDirectorySeparator(path);
        var missingAllowed = allowMissingLeaf;
        while (!string.IsNullOrEmpty(current))
        {
            try
            {
                if ((getAttributes(current) & FileAttributes.ReparsePoint) != 0)
                {
                    throw InvalidPath();
                }

                missingAllowed = false;
            }
            catch (Exception exception) when (
                missingAllowed && exception is FileNotFoundException or DirectoryNotFoundException)
            {
                // Preserve the resolver's established IMAGE_FILE_NOT_FOUND handling for an
                // ordinary missing image while still inspecting every existing ancestor.
            }

            var parent = Directory.GetParent(current)?.FullName;
            if (parent is null || string.Equals(parent, current, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            current = parent;
        }
    }

    private static bool IsPathInspectionException(Exception exception) =>
        exception is ArgumentException or NotSupportedException or PathTooLongException or
            IOException or UnauthorizedAccessException or InvalidOperationException;

    private static PublishPipelineException InvalidPath(Exception? innerException = null) =>
        new(
            PublishErrorCodes.ImagePathInvalid,
            "Local image path is invalid for preview-update.",
            innerException);
}
