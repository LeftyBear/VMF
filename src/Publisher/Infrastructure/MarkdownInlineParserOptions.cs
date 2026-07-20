namespace Vmf.Publisher.Infrastructure;

/// <summary>Defines Markdown inline parser settings.</summary>
public sealed class MarkdownInlineParserOptions
{
    /// <summary>The default maximum inline nesting depth.</summary>
    public const int DefaultMaxInlineDepth = 8;

    /// <summary>Gets or sets the maximum inline nesting depth.</summary>
    public int MaxInlineDepth { get; set; } = DefaultMaxInlineDepth;

    internal void Validate()
    {
        if (MaxInlineDepth < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxInlineDepth),
                "Maximum inline depth must be at least one.");
        }
    }
}
