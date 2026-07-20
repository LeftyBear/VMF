using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Resolves and validates local and remote image sources.</summary>
public interface IImageSourceResolver
{
    /// <summary>Resolves an image source relative to its Markdown document.</summary>
    Task<ImageSource> ResolveAsync(
        ImageSource source,
        string markdownFilePath,
        CancellationToken cancellationToken);
}
