using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using System.Net;
using System.Text.Json;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Authenticates a Google user through the OAuth 2.0 desktop browser flow.</summary>
public sealed class OAuthDesktopGoogleCredentialProvider : IGoogleCredentialProvider
{
    private readonly GooglePublisherOptions options;
    private readonly IOAuthDesktopAuthorizationBroker authorizationBroker;
    private readonly SemaphoreSlim authorizationGate = new(1, 1);
    private GoogleCredential? cachedCredential;

    /// <summary>Initializes an OAuth Desktop credential provider.</summary>
    /// <param name="options">Google publisher settings.</param>
    public OAuthDesktopGoogleCredentialProvider(GooglePublisherOptions options)
        : this(options, new GoogleOAuthDesktopAuthorizationBroker())
    {
    }

    internal OAuthDesktopGoogleCredentialProvider(
        GooglePublisherOptions options,
        IOAuthDesktopAuthorizationBroker authorizationBroker)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.authorizationBroker = authorizationBroker ?? throw new ArgumentNullException(nameof(authorizationBroker));
    }

    /// <inheritdoc />
    public async Task<GoogleCredential> GetCredentialAsync(CancellationToken cancellationToken)
    {
        ValidateOptions();
        if (IsReusable(cachedCredential))
        {
            return cachedCredential!;
        }

        await authorizationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsReusable(cachedCredential))
            {
                return cachedCredential!;
            }

            cachedCredential = await authorizationBroker.AuthorizeAsync(options, cancellationToken)
                .ConfigureAwait(false);
            return cachedCredential;
        }
        finally
        {
            authorizationGate.Release();
        }
    }

    private static bool IsReusable(GoogleCredential? credential) =>
        credential is not null && credential.ExpiresAtUtc > DateTimeOffset.UtcNow.AddMinutes(5);

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(options.CredentialsPath))
        {
            throw new InvalidOperationException(
                "GoogleApi:CredentialsPath is required for OAuthDesktop authentication.");
        }

        if (string.IsNullOrWhiteSpace(options.TokenStorePath))
        {
            throw new InvalidOperationException(
                "GoogleApi:TokenStorePath is required for OAuthDesktop authentication.");
        }
    }
}

internal interface IOAuthDesktopAuthorizationBroker
{
    Task<GoogleCredential> AuthorizeAsync(
        GooglePublisherOptions options,
        CancellationToken cancellationToken);
}

internal sealed class GoogleOAuthDesktopAuthorizationBroker : IOAuthDesktopAuthorizationBroker
{
    private static readonly string[] Scopes =
    [
        "https://www.googleapis.com/auth/documents",
        "https://www.googleapis.com/auth/drive",
    ];

    private readonly ICodeReceiver codeReceiver;

    internal GoogleOAuthDesktopAuthorizationBroker()
        : this(new LocalServerCodeReceiver())
    {
    }

    internal GoogleOAuthDesktopAuthorizationBroker(ICodeReceiver codeReceiver)
    {
        this.codeReceiver = codeReceiver ?? throw new ArgumentNullException(nameof(codeReceiver));
    }

    public async Task<GoogleCredential> AuthorizeAsync(
        GooglePublisherOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            using var credentialStream = new FileStream(
                options.CredentialsPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            using (var clientDocument = await JsonDocument.ParseAsync(
                credentialStream,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                if (!clientDocument.RootElement.TryGetProperty("installed", out var installed) ||
                    installed.ValueKind != JsonValueKind.Object)
                {
                    throw new OAuthDesktopConfigurationException(
                        "GoogleApi:CredentialsPath must reference an OAuth client JSON of type Desktop app.");
                }
            }

            credentialStream.Position = 0;
            var clientSecrets = GoogleClientSecrets.FromStream(credentialStream).Secrets;
            var dataStore = new FileDataStore(options.TokenStorePath, fullPath: true);
            var userCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                Scopes,
                "vmf-publisher-user",
                cancellationToken,
                dataStore,
                codeReceiver).ConfigureAwait(false);
            var accessToken = await userCredential.GetAccessTokenForRequestAsync(
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidOperationException("Google OAuth Desktop authentication returned no access token.");
            }

            var expiresInSeconds = userCredential.Token.ExpiresInSeconds ?? 3600;
            var issuedUtc = new DateTimeOffset(userCredential.Token.IssuedUtc, TimeSpan.Zero);
            return new GoogleCredential(accessToken, issuedUtc.AddSeconds(expiresInSeconds));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (GoogleApiException)
        {
            throw;
        }
        catch (OAuthDesktopConfigurationException)
        {
            throw;
        }
        catch (TokenResponseException exception)
        {
            throw new GoogleApiException(
                "Google OAuth API",
                exception.StatusCode ?? HttpStatusCode.BadRequest,
                exception.Error.Error ?? "OAUTH_ERROR");
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                "Google OAuth Desktop authentication failed. Check the client configuration and token-store permissions.",
                exception);
        }
    }
}

internal sealed class OAuthDesktopConfigurationException : InvalidOperationException
{
    internal OAuthDesktopConfigurationException(string message)
        : base(message)
    {
    }
}
