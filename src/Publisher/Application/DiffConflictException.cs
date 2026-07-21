namespace Vmf.Publisher.Application;

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
