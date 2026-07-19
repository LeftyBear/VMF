namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Identifies the configured Google authentication flow.</summary>
public enum GoogleAuthenticationMode
{
    /// <summary>Authenticates as a Google service account.</summary>
    ServiceAccount,

    /// <summary>Authenticates a user through the installed-application browser flow.</summary>
    OAuthDesktop,
}

/// <summary>Contains operational settings for Google publication.</summary>
public sealed class GooglePublisherOptions
{
    /// <summary>Gets or sets the authentication flow.</summary>
    public GoogleAuthenticationMode AuthenticationMode { get; set; } = GoogleAuthenticationMode.ServiceAccount;

    /// <summary>Gets or sets the selected authentication flow's credential JSON path.</summary>
    public string CredentialsPath { get; set; } = string.Empty;

    /// <summary>Gets or sets the OAuth Desktop token-store path.</summary>
    public string TokenStorePath { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional destination Google Drive folder identifier.</summary>
    public string FolderId { get; set; } = string.Empty;

    /// <summary>Gets or sets the application name sent to Google services.</summary>
    public string ApplicationName { get; set; } = "VMF Studio Publisher PoC";
}
