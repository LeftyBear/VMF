namespace Vmf.Publisher.Infrastructure;

/// <summary>Configures Markdown list parsing and normalization.</summary>
public sealed class MarkdownListParserOptions
{
    /// <summary>The default number of spaces represented by one list depth.</summary>
    public const int DefaultListIndentSize = 2;

    /// <summary>The default maximum number of list levels.</summary>
    public const int DefaultMaxListDepth = 6;

    /// <summary>Gets or sets the number of spaces represented by one list depth.</summary>
    public int ListIndentSize { get; set; } = DefaultListIndentSize;

    /// <summary>Gets or sets the maximum number of list levels.</summary>
    public int MaxListDepth { get; set; } = DefaultMaxListDepth;

    internal void Validate()
    {
        if (ListIndentSize <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ListIndentSize),
                ListIndentSize,
                "ListIndentSize must be greater than zero.");
        }

        if (MaxListDepth <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxListDepth),
                MaxListDepth,
                "MaxListDepth must be greater than zero.");
        }
    }
}
