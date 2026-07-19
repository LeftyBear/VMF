using System.Text;
using Vmf.Publisher.Application;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Loads UTF-8 Markdown files from the local file system.</summary>
public sealed class MarkdownFileDocumentLoader : IMarkdownDocumentLoader
{
    /// <inheritdoc />
    public async Task<string> LoadAsync(string path, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var reader = new StreamReader(stream, new UTF8Encoding(false, true), true);
        return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }
}
