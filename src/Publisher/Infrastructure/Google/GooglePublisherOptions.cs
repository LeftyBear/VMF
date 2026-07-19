namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Contains operational settings for Google publication.</summary>
public sealed class GooglePublisherOptions
{
    /// <summary>Gets or sets the service-account credential JSON path.</summary>
    public string CredentialsPath { get; set; } = string.Empty;

    /// <summary>Gets or sets the reserved local OAuth token-store path.</summary>
    public string TokenStorePath { get; set; } = string.Empty;

    /// <summary>Gets or sets the optional destination Google Drive folder identifier.</summary>
    public string FolderId { get; set; } = string.Empty;

    /// <summary>Gets or sets the application name sent to Google services.</summary>
    public string ApplicationName { get; set; } = "VMF Studio Publisher PoC";
}
