namespace Vmf.Publisher.Domain;

/// <summary>Identifies a supported document block kind.</summary>
public enum DocumentBlockKind
{
    /// <summary>A heading block.</summary>
    Heading,
    /// <summary>A paragraph block.</summary>
    Paragraph,
    /// <summary>An unordered list item block.</summary>
    BulletListItem,
    /// <summary>A contiguous ordered, unordered, or mixed list block.</summary>
    List,
    /// <summary>A table block with a header row.</summary>
    Table,
    /// <summary>A fenced code block.</summary>
    Code,
    /// <summary>A block quote.</summary>
    Quote,
    /// <summary>A standalone image.</summary>
    Image,
}

/// <summary>Represents a fenced code block.</summary>
public sealed class CodeBlock
{
    /// <summary>Initializes a fenced code block.</summary>
    /// <param name="text">The literal code body without fences.</param>
    /// <param name="language">The optional language string from the opening fence.</param>
    public CodeBlock(string text, string language)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Language = language ?? throw new ArgumentNullException(nameof(language));
    }

    /// <summary>Gets the literal code body without fences.</summary>
    public string Text { get; }

    /// <summary>Gets the optional language string from the opening fence.</summary>
    public string Language { get; }
}

/// <summary>Represents contiguous block-quote lines at one normalized level.</summary>
public sealed class QuoteBlock
{
    /// <summary>Initializes a block quote.</summary>
    /// <param name="level">The quote level from one through six.</param>
    /// <param name="content">The parsed inline content; an empty sequence preserves an empty quote line.</param>
    public QuoteBlock(int level, IEnumerable<InlineContent> content)
    {
        if (level is < 1 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(level));
        }

        ArgumentNullException.ThrowIfNull(content);
        var items = content.ToArray();
        if (items.Any(item => item is null))
        {
            throw new ArgumentException("Quote content must not contain null items.", nameof(content));
        }

        Level = level;
        Content = Array.AsReadOnly(items);
    }

    /// <summary>Gets the normalized quote level.</summary>
    public int Level { get; }

    /// <summary>Gets the parsed inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }
}

/// <summary>Represents a paragraph and its inline content.</summary>
public sealed class ParagraphBlock
{
    /// <summary>Initializes a paragraph.</summary>
    /// <param name="content">The inline content.</param>
    public ParagraphBlock(IEnumerable<InlineContent> content)
    {
        Content = InlineContentCollection.Create(content, nameof(content));
    }

    /// <summary>Gets the inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }

    /// <summary>Creates a plain-text paragraph.</summary>
    /// <param name="text">The paragraph text.</param>
    /// <returns>The paragraph.</returns>
    public static ParagraphBlock FromText(string text) => new([new TextInline(text)]);
}

/// <summary>Represents a heading and its inline content.</summary>
public sealed class HeadingBlock
{
    /// <summary>Initializes a heading.</summary>
    /// <param name="level">The heading level from one through six.</param>
    /// <param name="content">The inline content.</param>
    public HeadingBlock(int level, IEnumerable<InlineContent> content)
    {
        if (level is < 1 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(level));
        }

        Level = level;
        Content = InlineContentCollection.Create(content, nameof(content));
    }

    /// <summary>Gets the heading level.</summary>
    public int Level { get; }

    /// <summary>Gets the inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }

    /// <summary>Creates a plain-text heading.</summary>
    /// <param name="level">The heading level from one through six.</param>
    /// <param name="text">The heading text.</param>
    /// <returns>The heading.</returns>
    public static HeadingBlock FromText(int level, string text) => new(level, [new TextInline(text)]);
}

/// <summary>Represents one block in a document.</summary>
public sealed class DocumentBlock
{
    /// <summary>Initializes a document block with inline content.</summary>
    /// <param name="kind">The block kind.</param>
    /// <param name="content">The inline content.</param>
    /// <param name="level">The heading level, or zero for non-headings.</param>
    public DocumentBlock(DocumentBlockKind kind, IEnumerable<InlineContent> content, int level = 0)
    {
        if (kind is DocumentBlockKind.List or DocumentBlockKind.Table or
            DocumentBlockKind.Code or DocumentBlockKind.Quote or DocumentBlockKind.Image)
        {
            throw new ArgumentException(
                "Use the strongly typed constructor for structured document blocks.",
                nameof(kind));
        }

        Kind = kind;
        Level = level;
        Content = InlineContentCollection.Create(content, nameof(content));
    }

    /// <summary>Initializes a document block.</summary>
    /// <param name="kind">The block kind.</param>
    /// <param name="inlines">The inline content.</param>
    /// <param name="level">The heading level, or zero for non-headings.</param>
    public DocumentBlock(DocumentBlockKind kind, IEnumerable<InlineElement> inlines, int level = 0)
        : this(kind, InlineContentCompatibility.Convert(inlines), level)
    {
    }

    /// <summary>Initializes a paragraph document block.</summary>
    /// <param name="paragraph">The paragraph content.</param>
    public DocumentBlock(ParagraphBlock paragraph)
    {
        ArgumentNullException.ThrowIfNull(paragraph);
        Kind = DocumentBlockKind.Paragraph;
        Content = paragraph.Content;
    }

    /// <summary>Initializes a heading document block.</summary>
    /// <param name="heading">The heading content.</param>
    public DocumentBlock(HeadingBlock heading)
    {
        ArgumentNullException.ThrowIfNull(heading);
        Kind = DocumentBlockKind.Heading;
        Level = heading.Level;
        Content = heading.Content;
    }

    /// <summary>Initializes a list document block.</summary>
    /// <param name="list">The list content.</param>
    public DocumentBlock(ListBlock list)
    {
        List = list ?? throw new ArgumentNullException(nameof(list));
        Kind = DocumentBlockKind.List;
        Content = Array.Empty<InlineContent>();
    }

    /// <summary>Initializes a table document block.</summary>
    /// <param name="table">The table content.</param>
    public DocumentBlock(TableBlock table)
    {
        Table = table ?? throw new ArgumentNullException(nameof(table));
        Kind = DocumentBlockKind.Table;
        Content = Array.Empty<InlineContent>();
    }

    /// <summary>Initializes a fenced-code document block.</summary>
    /// <param name="code">The fenced code content.</param>
    public DocumentBlock(CodeBlock code)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Kind = DocumentBlockKind.Code;
        Content = Array.Empty<InlineContent>();
    }

    /// <summary>Initializes a quote document block.</summary>
    /// <param name="quote">The quote content.</param>
    public DocumentBlock(QuoteBlock quote)
    {
        Quote = quote ?? throw new ArgumentNullException(nameof(quote));
        Kind = DocumentBlockKind.Quote;
        Level = quote.Level;
        Content = quote.Content;
    }

    /// <summary>Initializes an image document block.</summary>
    /// <param name="image">The image content.</param>
    public DocumentBlock(ImageBlock image)
    {
        Image = image ?? throw new ArgumentNullException(nameof(image));
        Kind = DocumentBlockKind.Image;
        Content = Array.Empty<InlineContent>();
    }

    /// <summary>Gets the block kind.</summary>
    public DocumentBlockKind Kind { get; }

    /// <summary>Gets the heading level, or zero for non-headings.</summary>
    public int Level { get; }

    /// <summary>Gets the inline content.</summary>
    public IReadOnlyList<InlineContent> Content { get; }

    /// <summary>Gets a flattened legacy plain-text view of the inline content.</summary>
    public IReadOnlyList<InlineElement> Inlines => InlineContentCompatibility.Flatten(Content);

    /// <summary>Gets the list content when <see cref="Kind"/> is <see cref="DocumentBlockKind.List"/>.</summary>
    public ListBlock? List { get; }

    /// <summary>Gets the table content when <see cref="Kind"/> is <see cref="DocumentBlockKind.Table"/>.</summary>
    public TableBlock? Table { get; }

    /// <summary>Gets the code content when <see cref="Kind"/> is <see cref="DocumentBlockKind.Code"/>.</summary>
    public CodeBlock? Code { get; }

    /// <summary>Gets the quote content when <see cref="Kind"/> is <see cref="DocumentBlockKind.Quote"/>.</summary>
    public QuoteBlock? Quote { get; }

    /// <summary>Gets the image content when <see cref="Kind"/> is <see cref="DocumentBlockKind.Image"/>.</summary>
    public ImageBlock? Image { get; }
}
