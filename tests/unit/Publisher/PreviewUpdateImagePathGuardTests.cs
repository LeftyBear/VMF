using System.Diagnostics;
using Vmf.Publisher.Application;

namespace Vmf.Publisher.UnitTests;

public sealed class PreviewUpdateImagePathGuardTests
{
    [Fact]
    public void Validate_AllowsNormalFile()
    {
        using var fixture = new PathFixture();
        var imagePath = fixture.CreateFile("images/image.png");

        new PreviewUpdateImagePathGuard().Validate("images/image.png", fixture.MarkdownPath);

        Assert.True(File.Exists(imagePath));
    }

    [WindowsSymbolicLinkFact(SymbolicLinkKind.File)]
    [Trait("Privilege", "ReparsePointCreation")]
    public void Validate_RejectsFileSymbolicLink()
    {
        using var fixture = new PathFixture();
        var target = fixture.CreateFile("target.png");
        var link = Path.Combine(fixture.MarkdownDirectory, "image.png");
        File.CreateSymbolicLink(link, target);

        AssertInvalid(() => new PreviewUpdateImagePathGuard().Validate("image.png", fixture.MarkdownPath));
    }

    [WindowsSymbolicLinkFact(SymbolicLinkKind.Directory)]
    [Trait("Privilege", "ReparsePointCreation")]
    public void Validate_RejectsDirectorySymbolicLink()
    {
        using var fixture = new PathFixture();
        var target = fixture.CreateDirectory("target");
        fixture.CreateFile("target/image.png");
        Directory.CreateSymbolicLink(Path.Combine(fixture.MarkdownDirectory, "images"), target);

        AssertInvalid(() => new PreviewUpdateImagePathGuard().Validate("images/image.png", fixture.MarkdownPath));
    }

    [Fact]
    [Trait("Privilege", "ReparsePointCreation")]
    public void Validate_RejectsJunction()
    {
        using var fixture = new PathFixture();
        var target = fixture.CreateDirectory("target");
        fixture.CreateFile("target/image.png");
        var junction = Path.Combine(fixture.MarkdownDirectory, "images");
        CreateJunction(junction, target);
        fixture.TrackLink(junction);

        AssertInvalid(() => new PreviewUpdateImagePathGuard().Validate("images/image.png", fixture.MarkdownPath));
    }

    [Fact]
    [Trait("Privilege", "ReparsePointCreation")]
    public void Validate_RejectsMarkdownDirectoryThatIsJunction()
    {
        using var fixture = new PathFixture(createMarkdownDirectory: false);
        var target = fixture.CreateDirectory("markdown-target");
        File.WriteAllText(Path.Combine(target, "image.png"), "image");
        CreateJunction(fixture.MarkdownDirectory, target);
        fixture.TrackLink(fixture.MarkdownDirectory);

        AssertInvalid(() => new PreviewUpdateImagePathGuard().Validate("image.png", fixture.MarkdownPath));
    }

    [Fact]
    [Trait("Privilege", "ReparsePointCreation")]
    public void Validate_RejectsMarkdownAncestorThatIsJunction()
    {
        using var fixture = new PathFixture(createMarkdownDirectory: false);
        var target = fixture.CreateDirectory("ancestor-target");
        var markdownTarget = Path.Combine(target, "markdown");
        Directory.CreateDirectory(markdownTarget);
        File.WriteAllText(Path.Combine(markdownTarget, "image.png"), "image");
        var ancestor = Path.Combine(fixture.Root, "ancestor");
        CreateJunction(ancestor, target);
        fixture.TrackLink(ancestor);
        fixture.SetMarkdownDirectory(Path.Combine(ancestor, "markdown"));

        AssertInvalid(() => new PreviewUpdateImagePathGuard().Validate("image.png", fixture.MarkdownPath));
    }

    [WindowsSymbolicLinkFact(SymbolicLinkKind.File)]
    [Trait("Privilege", "ReparsePointCreation")]
    public void Validate_RejectsDanglingFileSymbolicLink()
    {
        using var fixture = new PathFixture();
        var link = Path.Combine(fixture.MarkdownDirectory, "image.png");
        File.CreateSymbolicLink(link, Path.Combine(fixture.Root, "missing-target.png"));

        Assert.False(File.Exists(link));
        Assert.True((File.GetAttributes(link) & FileAttributes.ReparsePoint) != 0);
        AssertInvalid(() => new PreviewUpdateImagePathGuard().Validate("image.png", fixture.MarkdownPath));
    }

    [Fact]
    public void Validate_MapsAttributeReadFailureToImagePathInvalidWithoutPathDetails()
    {
        using var fixture = new PathFixture();
        var image = fixture.CreateFile("image.png");
        var guard = new PreviewUpdateImagePathGuard(path =>
            string.Equals(path, image, StringComparison.OrdinalIgnoreCase)
                ? throw new UnauthorizedAccessException("secret-path-details")
                : File.GetAttributes(path));

        var exception = Assert.Throws<PublishPipelineException>(() =>
            guard.Validate("image.png", fixture.MarkdownPath));

        Assert.Equal(PublishErrorCodes.ImagePathInvalid, exception.Code);
        Assert.DoesNotContain("secret", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(image, exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_AllowsOrdinaryMissingImageForResolverHandling()
    {
        using var fixture = new PathFixture();

        new PreviewUpdateImagePathGuard().Validate("missing/ordinary.png", fixture.MarkdownPath);
    }

    [Fact]
    public void Validate_RejectsReparsePointEvenWhenTargetDoesNotExist()
    {
        using var fixture = new PathFixture();
        var image = Path.Combine(fixture.MarkdownDirectory, "dangling.png");
        var guard = new PreviewUpdateImagePathGuard(path =>
            string.Equals(path, image, StringComparison.OrdinalIgnoreCase)
                ? FileAttributes.ReparsePoint
                : File.GetAttributes(path));

        Assert.False(File.Exists(image));
        AssertInvalid(() => guard.Validate("dangling.png", fixture.MarkdownPath));
    }

    [Fact]
    public void Validate_InjectedFileSymlinkAttributeRejectsExistingLeaf()
    {
        using var fixture = new PathFixture();
        var image = fixture.CreateFile("image.png");
        var guard = new PreviewUpdateImagePathGuard(path =>
            string.Equals(path, image, StringComparison.OrdinalIgnoreCase)
                ? FileAttributes.Normal | FileAttributes.ReparsePoint
                : File.GetAttributes(path));

        AssertInvalid(() => guard.Validate("image.png", fixture.MarkdownPath));
    }

    [Fact]
    public void Validate_InjectedDirectorySymlinkAttributeRejectsAncestor()
    {
        using var fixture = new PathFixture();
        var directory = Path.Combine(fixture.MarkdownDirectory, "images");
        Directory.CreateDirectory(directory);
        fixture.CreateFile("images/image.png");
        var guard = new PreviewUpdateImagePathGuard(path =>
            string.Equals(path, directory, StringComparison.OrdinalIgnoreCase)
                ? FileAttributes.Directory | FileAttributes.ReparsePoint
                : File.GetAttributes(path));

        AssertInvalid(() => guard.Validate("images/image.png", fixture.MarkdownPath));
    }

    private static void AssertInvalid(Action action)
    {
        var exception = Assert.Throws<PublishPipelineException>(action);
        Assert.Equal(PublishErrorCodes.ImagePathInvalid, exception.Code);
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
        Assert.True(
            process.ExitCode == 0,
            $"Junction creation was not verified (exit {process.ExitCode}).");
    }

    private sealed class PathFixture : IDisposable
    {
        private readonly List<string> links = [];

        internal PathFixture(bool createMarkdownDirectory = true)
        {
            Root = Path.Combine(Path.GetTempPath(), $"vmf-p2-36a-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Root);
            MarkdownDirectory = Path.Combine(Root, "markdown");
            if (createMarkdownDirectory)
            {
                Directory.CreateDirectory(MarkdownDirectory);
            }
        }

        internal string Root { get; }
        internal string MarkdownDirectory { get; private set; }
        internal string MarkdownPath => Path.Combine(MarkdownDirectory, "document.md");

        internal void SetMarkdownDirectory(string path) => MarkdownDirectory = path;

        internal void TrackLink(string path) => links.Add(path);

        internal string CreateDirectory(string relativePath)
        {
            var path = Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(path);
            return path;
        }

        internal string CreateFile(string relativePath)
        {
            var path = Path.Combine(MarkdownDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, "image");
            return path;
        }

        public void Dispose()
        {
            foreach (var link in links.OrderByDescending(path => path.Length))
            {
                if ((File.GetAttributes(link) & FileAttributes.Directory) != 0)
                {
                    Directory.Delete(link);
                }
                else
                {
                    File.Delete(link);
                }
            }

            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }
    }
}
