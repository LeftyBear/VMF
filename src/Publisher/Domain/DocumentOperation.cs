namespace Vmf.Publisher.Domain;

/// <summary>Identifies a target-neutral document operation.</summary>
public enum DocumentOperationKind
{
    /// <summary>Inserts text.</summary>
    InsertText,
    /// <summary>Applies a heading paragraph style.</summary>
    ApplyHeading,
    /// <summary>Creates list paragraphs.</summary>
    CreateBullet,
    /// <summary>Applies an inline text style.</summary>
    UpdateTextStyle,
    /// <summary>Applies a table column's paragraph alignment.</summary>
    UpdateParagraphAlignment,
    /// <summary>Applies fenced-code paragraph formatting.</summary>
    ApplyCodeBlockStyle,
    /// <summary>Applies block-quote paragraph formatting.</summary>
    ApplyQuoteBlockStyle,
}

/// <summary>Represents one target-neutral document operation.</summary>
public sealed class DocumentOperation
{
    /// <summary>Initializes a document operation.</summary>
    /// <param name="kind">The operation kind.</param>
    /// <param name="startIndex">The inclusive Google Docs text index.</param>
    /// <param name="endIndex">The exclusive Google Docs text index.</param>
    /// <param name="text">Text used by an insert operation.</param>
    /// <param name="level">A heading level used by a heading operation.</param>
    /// <param name="listKind">The marker kind used by a list operation.</param>
    /// <param name="inlineStyle">The style used by a text style operation.</param>
    /// <param name="url">The URL used by a link text style operation.</param>
    /// <param name="tableAlignment">The alignment used by a table-cell paragraph operation.</param>
    public DocumentOperation(
        DocumentOperationKind kind,
        int startIndex,
        int? endIndex = null,
        string? text = null,
        int? level = null,
        ListKind? listKind = null,
        InlineTextStyle? inlineStyle = null,
        Uri? url = null,
        TableAlignment? tableAlignment = null)
    {
        Kind = kind;
        StartIndex = startIndex;
        EndIndex = endIndex;
        Text = text;
        Level = level;
        ListKind = listKind;
        InlineStyle = inlineStyle;
        Url = url;
        TableAlignment = tableAlignment;
    }

    /// <summary>Gets the operation kind.</summary>
    public DocumentOperationKind Kind { get; }

    /// <summary>Gets the inclusive Google Docs text index.</summary>
    public int StartIndex { get; }

    /// <summary>Gets the exclusive Google Docs text index.</summary>
    public int? EndIndex { get; }

    /// <summary>Gets the text used by an insert operation.</summary>
    public string? Text { get; }

    /// <summary>Gets the heading level used by a heading operation.</summary>
    public int? Level { get; }

    /// <summary>Gets the marker kind used by a list operation.</summary>
    public ListKind? ListKind { get; }

    /// <summary>Gets the style used by a text style operation.</summary>
    public InlineTextStyle? InlineStyle { get; }

    /// <summary>Gets the URL used by a link text style operation.</summary>
    public Uri? Url { get; }

    /// <summary>Gets the paragraph alignment used by a table cell.</summary>
    public TableAlignment? TableAlignment { get; }
}
