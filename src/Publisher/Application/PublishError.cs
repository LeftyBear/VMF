namespace Vmf.Publisher.Application;

/// <summary>Represents a publication failure without exposing implementation details.</summary>
public sealed class PublishError
{
    /// <summary>Initializes a publication error.</summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The user-facing error message.</param>
    public PublishError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>Gets the stable error code.</summary>
    public string Code { get; }

    /// <summary>Gets the user-facing error message.</summary>
    public string Message { get; }
}
