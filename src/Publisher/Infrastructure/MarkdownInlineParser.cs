using System.Text;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure;

/// <summary>Parses supported Markdown emphasis and link syntax into inline content.</summary>
public sealed class MarkdownInlineParser : IMarkdownInlineParser
{
    private readonly int maxInlineDepth;

    /// <summary>Initializes a parser with default settings.</summary>
    public MarkdownInlineParser()
        : this(new MarkdownInlineParserOptions())
    {
    }

    /// <summary>Initializes a parser with explicit settings.</summary>
    /// <param name="options">The inline parser settings.</param>
    public MarkdownInlineParser(MarkdownInlineParserOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();
        maxInlineDepth = options.MaxInlineDepth;
    }

    /// <summary>Parses one Markdown inline source.</summary>
    /// <param name="markdown">The Markdown inline source.</param>
    /// <returns>Parsed, adjacent-text-normalized inline content.</returns>
    public IReadOnlyList<InlineContent> Parse(string markdown)
    {
        ArgumentNullException.ThrowIfNull(markdown);
        if (markdown.Length == 0)
        {
            return Array.Empty<InlineContent>();
        }

        var index = 0;
        var result = ParseSequence(markdown, ref index, closingDelimiter: null, depth: 0, out _);
        return Array.AsReadOnly(result.ToArray());
    }

    private List<InlineContent> ParseSequence(
        string markdown,
        ref int index,
        string? closingDelimiter,
        int depth,
        out bool closed)
    {
        var result = new List<InlineContent>();
        var text = new StringBuilder();

        while (index < markdown.Length)
        {
            if (closingDelimiter is not null && StartsWith(markdown, index, closingDelimiter))
            {
                index += closingDelimiter.Length;
                FlushText(result, text);
                closed = true;
                return result;
            }

            if (TryReadEscape(markdown, ref index, text))
            {
                continue;
            }

            if (markdown[index] == '`')
            {
                var codeStart = index;
                if (TryParseCode(markdown, ref index, out var code, out var codeFallbackEnd))
                {
                    FlushText(result, text);
                    result.Add(code);
                    continue;
                }

                text.Append(markdown.AsSpan(codeStart, codeFallbackEnd - codeStart));
                index = codeFallbackEnd;
                continue;
            }

            if (markdown[index] == '[')
            {
                var linkStart = index;
                if (TryParseLink(markdown, ref index, depth, out var link, out var linkFallbackEnd))
                {
                    FlushText(result, text);
                    result.Add(link);
                    continue;
                }

                if (linkFallbackEnd > linkStart)
                {
                    text.Append(Unescape(markdown[linkStart..linkFallbackEnd]));
                    index = linkFallbackEnd;
                    continue;
                }
            }

            if (markdown[index] == '*' || markdown[index] == '_')
            {
                var delimiterStart = index;
                if (TryParseDelimited(
                    markdown,
                    ref index,
                    depth,
                    out var styled,
                    out var delimiterFallbackEnd))
                {
                    FlushText(result, text);
                    result.Add(styled);
                    continue;
                }

                if (delimiterFallbackEnd > delimiterStart)
                {
                    text.Append(Unescape(markdown[delimiterStart..delimiterFallbackEnd]));
                    index = delimiterFallbackEnd;
                    continue;
                }
            }

            text.Append(markdown[index]);
            index++;
        }

        FlushText(result, text);
        closed = closingDelimiter is null;
        return result;
    }

    private static bool TryParseCode(
        string markdown,
        ref int index,
        out InlineContent code,
        out int fallbackEnd)
    {
        code = null!;
        var start = index;
        var delimiterLength = CountRun(markdown, start, '`');
        var searchIndex = start + delimiterLength;
        while (searchIndex < markdown.Length)
        {
            if (markdown[searchIndex] != '`')
            {
                searchIndex++;
                continue;
            }

            var candidateLength = CountRun(markdown, searchIndex, '`');
            if (candidateLength == delimiterLength)
            {
                fallbackEnd = searchIndex + candidateLength;
                var value = markdown[(start + delimiterLength)..searchIndex];
                if (value.Length == 0)
                {
                    return false;
                }

                index = fallbackEnd;
                code = new CodeInline(value);
                return true;
            }

            searchIndex += candidateLength;
        }

        fallbackEnd = markdown.Length;
        return false;
    }

    private bool TryParseDelimited(
        string markdown,
        ref int index,
        int depth,
        out InlineContent styled,
        out int fallbackEnd)
    {
        styled = null!;
        fallbackEnd = index;
        if (depth >= maxInlineDepth)
        {
            return false;
        }

        var marker = markdown[index];
        var available = CountRun(markdown, index, marker);
        var length = Math.Min(available, 3);
        var start = index;
        var delimiter = new string(marker, length);
        index += length;
        var content = ParseSequence(markdown, ref index, delimiter, depth + 1, out var closed);
        fallbackEnd = index;
        if (closed && HasVisibleContent(content))
        {
            styled = length switch
            {
                3 => new BoldInline([new ItalicInline(content)]),
                2 => new BoldInline(content),
                1 => new ItalicInline(content),
                _ => throw new InvalidOperationException("Unsupported inline delimiter length."),
            };
            return true;
        }

        index = start;
        return false;
    }

    private bool TryParseLink(
        string markdown,
        ref int index,
        int depth,
        out InlineContent link,
        out int fallbackEnd)
    {
        link = null!;
        fallbackEnd = index;
        if (depth >= maxInlineDepth)
        {
            return false;
        }

        var labelEnd = FindUnescaped(markdown, index + 1, ']');
        if (labelEnd <= index + 1 ||
            labelEnd + 1 >= markdown.Length ||
            markdown[labelEnd + 1] != '(')
        {
            return false;
        }

        var urlEnd = FindUrlEnd(markdown, labelEnd + 2);
        if (urlEnd < 0)
        {
            fallbackEnd = markdown.Length;
            return false;
        }

        fallbackEnd = urlEnd + 1;

        var urlText = Unescape(markdown[(labelEnd + 2)..urlEnd]);
        if (!TryCreateHttpUrl(urlText, out var url))
        {
            return false;
        }

        var labelText = markdown[(index + 1)..labelEnd];
        var labelIndex = 0;
        var content = ParseSequence(labelText, ref labelIndex, closingDelimiter: null, depth + 1, out _);
        if (!HasVisibleContent(content))
        {
            return false;
        }

        index = urlEnd + 1;
        link = new LinkInline(content, url);
        return true;
    }

    private static bool TryReadEscape(string markdown, ref int index, StringBuilder text)
    {
        if (markdown[index] != '\\' ||
            index + 1 >= markdown.Length ||
            !IsEscapable(markdown[index + 1]))
        {
            return false;
        }

        text.Append(markdown[index + 1]);
        index += 2;
        return true;
    }

    private static int FindUnescaped(string source, int startIndex, char value)
    {
        for (var index = startIndex; index < source.Length; index++)
        {
            if (source[index] == '\\' &&
                index + 1 < source.Length &&
                IsEscapable(source[index + 1]))
            {
                index++;
                continue;
            }

            if (source[index] == value)
            {
                return index;
            }
        }

        return -1;
    }

    private static int FindUrlEnd(string source, int startIndex)
    {
        var nestedParentheses = 0;
        for (var index = startIndex; index < source.Length; index++)
        {
            if (source[index] == '\\' &&
                index + 1 < source.Length &&
                IsEscapable(source[index + 1]))
            {
                index++;
                continue;
            }

            if (source[index] == '(')
            {
                nestedParentheses++;
            }
            else if (source[index] == ')' && nestedParentheses-- == 0)
            {
                return index;
            }
        }

        return -1;
    }

    private static string Unescape(string value)
    {
        var result = new StringBuilder(value.Length);
        for (var index = 0; index < value.Length; index++)
        {
            if (value[index] == '\\' &&
                index + 1 < value.Length &&
                IsEscapable(value[index + 1]))
            {
                index++;
            }

            result.Append(value[index]);
        }

        return result.ToString();
    }

    private static bool TryCreateHttpUrl(string value, out Uri url)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var candidate) &&
            (candidate.Scheme == Uri.UriSchemeHttp || candidate.Scheme == Uri.UriSchemeHttps) &&
            !string.IsNullOrWhiteSpace(candidate.Host))
        {
            url = candidate;
            return true;
        }

        url = null!;
        return false;
    }

    private static bool StartsWith(string source, int index, string value) =>
        source.AsSpan(index).StartsWith(value, StringComparison.Ordinal);

    private static bool IsEscapable(char value) => char.IsPunctuation(value) || value == '|';

    private static int CountRun(string source, int index, char marker)
    {
        var length = 0;
        while (index + length < source.Length && source[index + length] == marker)
        {
            length++;
        }

        return length;
    }

    private static bool HasVisibleContent(IEnumerable<InlineContent> content) =>
        content.Any(item => item switch
        {
            TextInline text => !string.IsNullOrWhiteSpace(text.Text),
            CodeInline code => code.Text.Length > 0,
            BoldInline bold => HasVisibleContent(bold.Content),
            ItalicInline italic => HasVisibleContent(italic.Content),
            LinkInline link => HasVisibleContent(link.Content),
            _ => false,
        });

    private static void FlushText(ICollection<InlineContent> result, StringBuilder text)
    {
        if (text.Length == 0)
        {
            return;
        }

        var value = text.ToString();
        text.Clear();
        if (result.LastOrDefault() is TextInline previous)
        {
            result.Remove(previous);
            result.Add(new TextInline(previous.Text + value));
        }
        else
        {
            result.Add(new TextInline(value));
        }
    }
}
