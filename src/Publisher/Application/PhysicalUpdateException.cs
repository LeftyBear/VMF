namespace Vmf.Publisher.Application;

/// <summary>Defines stable physical-update error codes.</summary>
public static class UpdateErrorCodes
{
    /// <summary>The current document revision differs from the required revision.</summary>
    public const string RevisionConflict = "UPDATE_REVISION_CONFLICT";

    /// <summary>The managed region or its block topology is invalid.</summary>
    public const string ManagedRegionMismatch = "UPDATE_MANAGED_REGION_MISMATCH";

    /// <summary>The logical plan cannot be converted into a safe physical plan.</summary>
    public const string PhysicalPlanInvalid = "UPDATE_PHYSICAL_PLAN_INVALID";

    /// <summary>Physical application failed.</summary>
    public const string ApplicationFailed = "UPDATE_APPLICATION_FAILED";

    /// <summary>Post-apply readback failed.</summary>
    public const string ReadbackFailed = "UPDATE_READBACK_FAILED";

    /// <summary>Post-apply readback differs from the candidate.</summary>
    public const string ReadbackMismatch = "UPDATE_READBACK_MISMATCH";
}

/// <summary>Represents a safe-fail physical update error.</summary>
public sealed class PhysicalUpdateException : Exception
{
    /// <summary>Initializes a physical update exception.</summary>
    public PhysicalUpdateException(string code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    /// <summary>Gets the stable public error code.</summary>
    public string Code { get; }
}
