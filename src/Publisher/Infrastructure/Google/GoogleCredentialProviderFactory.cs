namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Creates the configured Google credential provider.</summary>
public static class GoogleCredentialProviderFactory
{
    /// <summary>Creates a credential provider for the selected authentication mode.</summary>
    /// <param name="options">Google publisher settings.</param>
    /// <param name="httpClient">The HTTP transport used by service-account authentication.</param>
    /// <returns>The configured credential provider.</returns>
    public static IGoogleCredentialProvider Create(
        GooglePublisherOptions options,
        HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(httpClient);
        return options.AuthenticationMode switch
        {
            GoogleAuthenticationMode.ServiceAccount =>
                new ServiceAccountGoogleCredentialProvider(options, httpClient),
            GoogleAuthenticationMode.OAuthDesktop =>
                new OAuthDesktopGoogleCredentialProvider(options),
            _ => throw new InvalidOperationException(
                $"Unsupported Google authentication mode: {options.AuthenticationMode}"),
        };
    }
}
