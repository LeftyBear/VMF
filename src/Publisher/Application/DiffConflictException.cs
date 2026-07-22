namespace Vmf.Publisher.Application;

/// <summary>Defines stable differential-planning conflict codes.</summary>
public static class DiffErrorCodes
{
    /// <summary>The baseline and candidate identify different documents.</summary>
    public const string DocumentIdentityMismatch = "DIFF_DOCUMENT_IDENTITY_MISMATCH";

    /// <summary>Multiple identity tiers resolve one candidate to different baseline blocks.</summary>
    public const string IdentityConflict = "DIFF_IDENTITY_CONFLICT";

    /// <summary>An explicit or generated identity is duplicated within one state.</summary>
    public const string DuplicateIdentity = "DIFF_DUPLICATE_IDENTITY";

    /// <summary>Content-hash fallback has multiple viable baseline or candidate blocks.</summary>
    public const string ContentHashAmbiguous = "DIFF_CONTENT_HASH_AMBIGUOUS";
}

/// <summary>Represents a safe-fail conflict while constructing a differential plan.</summary>
public sealed class DiffConflictException : Exception
{
    /// <summary>Initializes a differential conflict.</summary>
    /// <param name="code">The stable conflict code.</param>
    /// <param name="message">The conflict description.</param>
    public DiffConflictException(string code, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    /// <summary>Gets the stable conflict code.</summary>
    public string Code { get; }
}
