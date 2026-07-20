namespace Vmf.Publisher.Application;

/// <summary>Provides stable error codes for table publish failures.</summary>
public static class PublishErrorCodes
{
    /// <summary>The inserted table was absent from the document readback.</summary>
    public const string TableNotFoundAfterInsert = "TABLE_NOT_FOUND_AFTER_INSERT";

    /// <summary>The read table dimensions differed from the requested dimensions.</summary>
    public const string TableDimensionMismatch = "TABLE_DIMENSION_MISMATCH";

    /// <summary>A table cell did not expose a usable text index.</summary>
    public const string TableCellIndexMissing = "TABLE_CELL_INDEX_MISSING";

    /// <summary>The table content batch update failed.</summary>
    public const string TableContentUpdateFailed = "TABLE_CONTENT_UPDATE_FAILED";
}

/// <summary>Represents a publish-plan failure with a stable public error code.</summary>
public sealed class PublishPipelineException : Exception
{
    /// <summary>Initializes a publish-plan exception.</summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The safe error message.</param>
    /// <param name="innerException">The underlying failure, when available.</param>
    public PublishPipelineException(string code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    /// <summary>Gets the stable error code.</summary>
    public string Code { get; }
}
