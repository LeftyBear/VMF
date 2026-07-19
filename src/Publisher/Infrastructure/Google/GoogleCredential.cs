namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Represents a short-lived credential used by Google REST clients.</summary>
public sealed class GoogleCredential
{
    /// <summary>Initializes a Google credential.</summary>
    /// <param name="accessToken">The OAuth access token.</param>
    /// <param name="expiresAtUtc">The token expiration time.</param>
    public GoogleCredential(string accessToken, DateTimeOffset expiresAtUtc)
    {
        AccessToken = accessToken;
        ExpiresAtUtc = expiresAtUtc;
    }

    /// <summary>Gets the OAuth access token.</summary>
    public string AccessToken { get; }

    /// <summary>Gets the token expiration time.</summary>
    public DateTimeOffset ExpiresAtUtc { get; }
}
