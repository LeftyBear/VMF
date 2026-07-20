namespace Vmf.Publisher.Application;

/// <summary>Provides stable error codes for table publish failures.</summary>
public static class PublishErrorCodes
{
    /// <summary>The image source was empty.</summary>
    public const string ImageSourceEmpty = "IMAGE_SOURCE_EMPTY";
    /// <summary>The local image file was absent.</summary>
    public const string ImageFileNotFound = "IMAGE_FILE_NOT_FOUND";
    /// <summary>The local image path was invalid.</summary>
    public const string ImagePathInvalid = "IMAGE_PATH_INVALID";
    /// <summary>The image format was unsupported or inconsistent.</summary>
    public const string ImageFormatNotSupported = "IMAGE_FORMAT_NOT_SUPPORTED";
    /// <summary>The remote image URI was invalid.</summary>
    public const string ImageRemoteUriInvalid = "IMAGE_REMOTE_URI_INVALID";
    /// <summary>The remote image host was unsafe.</summary>
    public const string ImageRemoteHostNotAllowed = "IMAGE_REMOTE_HOST_NOT_ALLOWED";
    /// <summary>Image metadata could not be read.</summary>
    public const string ImageMetadataReadFailed = "IMAGE_METADATA_READ_FAILED";
    /// <summary>The calculated image size was invalid.</summary>
    public const string ImageSizeInvalid = "IMAGE_SIZE_INVALID";
    /// <summary>The temporary image upload failed.</summary>
    public const string ImageUploadFailed = "IMAGE_UPLOAD_FAILED";
    /// <summary>Temporary public image access was denied.</summary>
    public const string ImagePublicAccessDenied = "IMAGE_PUBLIC_ACCESS_DENIED";
    /// <summary>The remote image host could not be resolved.</summary>
    public const string ImageUriResolutionFailed = "IMAGE_URI_RESOLUTION_FAILED";
    /// <summary>The Google Docs image insertion failed.</summary>
    public const string ImageInsertFailed = "IMAGE_INSERT_FAILED";
    /// <summary>The inserted image was absent from the document readback.</summary>
    public const string ImageNotFoundAfterInsert = "IMAGE_NOT_FOUND_AFTER_INSERT";
    /// <summary>Image alternative text could not be reflected in Google Docs.</summary>
    public const string ImageAltTextUpdateFailed = "IMAGE_ALT_TEXT_UPDATE_FAILED";
    /// <summary>The image paragraph exposed no following index.</summary>
    public const string ImageFollowingIndexNotFound = "IMAGE_FOLLOWING_INDEX_NOT_FOUND";
    /// <summary>The temporary Drive image could not be deleted.</summary>
    public const string ImageTempFileDeleteFailed = "IMAGE_TEMP_FILE_DELETE_FAILED";

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
