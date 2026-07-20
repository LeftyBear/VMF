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
    public DocumentOperation(
        DocumentOperationKind kind,
        int startIndex,
        int? endIndex = null,
        string? text = null,
        int? level = null,
        ListKind? listKind = null)
    {
        Kind = kind;
        StartIndex = startIndex;
        EndIndex = endIndex;
        Text = text;
        Level = level;
        ListKind = listKind;
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
}
