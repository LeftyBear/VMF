namespace Vmf.Publisher.Application;

/// <summary>Represents a publication failure without exposing implementation details.</summary>
public sealed class PublishError
{
    /// <summary>Initializes a publication error.</summary>
    /// <param name="code">The stable error code.</param>
    /// <param name="message">The user-facing error message.</param>
    /// <param name="deliveryState">The request delivery state carried from physical update execution, when known.</param>
    public PublishError(string code, string message, RequestDeliveryState? deliveryState = null)
    {
        Code = code;
        Message = message;
        DeliveryState = deliveryState;
    }

    /// <summary>Gets the stable error code.</summary>
    public string Code { get; }

    /// <summary>Gets the user-facing error message.</summary>
    public string Message { get; }

    /// <summary>Gets the request delivery state carried from physical update execution, when known.</summary>
    public RequestDeliveryState? DeliveryState { get; }
}
