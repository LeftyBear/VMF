namespace Vmf.Publisher.Application;

/// <summary>Defines stable errors for verified publish-state lifecycle failures.</summary>
public static class StateErrorCodes
{
    /// <summary>A required state record was not found.</summary>
    public const string NotFound = "STATE_NOT_FOUND";

    /// <summary>A persisted state record was malformed or internally inconsistent.</summary>
    public const string Corrupted = "STATE_CORRUPTED";

    /// <summary>The persisted state uses an unsupported schema version.</summary>
    public const string SchemaVersionUnsupported = "STATE_SCHEMA_VERSION_UNSUPPORTED";

    /// <summary>The persisted or verified state identifies a different document.</summary>
    public const string DocumentIdentityMismatch = "STATE_DOCUMENT_IDENTITY_MISMATCH";

    /// <summary>The requested persisted state transition is prohibited.</summary>
    public const string InvalidTransition = "STATE_INVALID_TRANSITION";

    /// <summary>Required application or readback verification did not complete.</summary>
    public const string VerificationRequired = "STATE_VERIFICATION_REQUIRED";

    /// <summary>Verified application output differs from the publish candidate.</summary>
    public const string VerificationMismatch = "STATE_VERIFICATION_MISMATCH";

    /// <summary>An atomic state save failed.</summary>
    public const string SaveFailed = "STATE_SAVE_FAILED";

    /// <summary>A persisted algorithm version is unsupported by this Publisher.</summary>
    public const string AlgorithmVersionUnsupported = "STATE_ALGORITHM_VERSION_UNSUPPORTED";
}

/// <summary>Represents a safe-fail error in the verified-state lifecycle.</summary>
public sealed class StateLifecycleException : Exception
{
    /// <summary>Initializes a state lifecycle exception.</summary>
    public StateLifecycleException(string code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    /// <summary>Gets the stable error code.</summary>
    public string Code { get; }
}
