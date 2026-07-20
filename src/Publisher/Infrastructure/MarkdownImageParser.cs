using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses standalone Markdown image lines.</summary>
public sealed class MarkdownImageParser
{
    /// <summary>Attempts to parse a line that consists entirely of one image.</summary>
    public bool TryParse(string line, out ImageBlock? image)
    {
        ArgumentNullException.ThrowIfNull(line);
        image = null;
        if (!line.StartsWith("![", StringComparison.Ordinal))
        {
            return false;
        }

        var altText = new StringBuilder();
        var index = 2;
        var closedAltText = false;
        while (index < line.Length)
        {
            if (line[index] == '\\' && index + 1 < line.Length && line[index + 1] == ']')
            {
                altText.Append(']');
                index += 2;
                continue;
            }

            if (line[index] == ']')
            {
                closedAltText = true;
                index++;
                break;
            }

            altText.Append(line[index]);
            index++;
        }

        if (!closedAltText || index >= line.Length || line[index] != '(' || line[^1] != ')')
        {
            return false;
        }

        var sourceText = line[(index + 1)..^1];
        ImageSource source = Uri.TryCreate(sourceText, UriKind.Absolute, out var uri) && !uri.IsFile
            ? new RemoteImageSource(uri)
            : new LocalImageSource(sourceText);
        image = new ImageBlock(altText.ToString(), source);
        return true;
    }
}
