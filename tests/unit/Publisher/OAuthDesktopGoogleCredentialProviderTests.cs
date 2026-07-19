using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Vmf.Publisher.Infrastructure.Google;
using AuthorizationCodeRequestUrl = Google.Apis.Auth.OAuth2.Requests.AuthorizationCodeRequestUrl;
using ICodeReceiver = Google.Apis.Auth.OAuth2.ICodeReceiver;

namespace Vmf.Publisher.UnitTests;

public sealed class OAuthDesktopGoogleCredentialProviderTests
{
    [Fact]
    public async Task GetCredentialAsync_RequiresClientCredentialsPath()
    {
        var options = new GooglePublisherOptions
        {
            AuthenticationMode = GoogleAuthenticationMode.OAuthDesktop,
            TokenStorePath = "token-store",
        };
        var provider = new OAuthDesktopGoogleCredentialProvider(options, new StubAuthorizationBroker());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.GetCredentialAsync(CancellationToken.None));

        Assert.Contains("CredentialsPath", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetCredentialAsync_RequiresTokenStorePath()
    {
        var options = new GooglePublisherOptions
        {
            AuthenticationMode = GoogleAuthenticationMode.OAuthDesktop,
            CredentialsPath = "oauth-client.json",
        };
        var provider = new OAuthDesktopGoogleCredentialProvider(options, new StubAuthorizationBroker());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.GetCredentialAsync(CancellationToken.None));

        Assert.Contains("TokenStorePath", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetCredentialAsync_ReusesValidCredentialWithinProcess()
    {
        var broker = new StubAuthorizationBroker();
        var provider = new OAuthDesktopGoogleCredentialProvider(CreateOptions(), broker);

        var first = await provider.GetCredentialAsync(CancellationToken.None);
        var second = await provider.GetCredentialAsync(CancellationToken.None);

        Assert.Same(first, second);
        Assert.Equal(1, broker.CallCount);
    }

    [Fact]
    public async Task AuthorizationBroker_ReusesPersistedTokenWithoutBrowserAuthorization()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"vmf-oauth-test-{Guid.NewGuid():N}");
        var tokenStorePath = Path.Combine(tempRoot, "tokens");
        var credentialsPath = Path.Combine(tempRoot, "oauth-client.json");
        Directory.CreateDirectory(tempRoot);

        try
        {
            await File.WriteAllTextAsync(
                credentialsPath,
                "{\"installed\":{\"client_id\":\"test.apps.googleusercontent.com\",\"client_secret\":\"test-secret\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"redirect_uris\":[\"http://localhost\"]}}");
            var dataStore = new FileDataStore(tokenStorePath, fullPath: true);
            await dataStore.StoreAsync(
                "vmf-publisher-user",
                new TokenResponse
                {
                    AccessToken = "persisted-access-token",
                    RefreshToken = "persisted-refresh-token",
                    ExpiresInSeconds = 3600,
                    IssuedUtc = DateTime.UtcNow,
                });
            var options = CreateOptions();
            options.CredentialsPath = credentialsPath;
            options.TokenStorePath = tokenStorePath;

            var credential = await new GoogleOAuthDesktopAuthorizationBroker()
                .AuthorizeAsync(options, CancellationToken.None);

            Assert.Equal("persisted-access-token", credential.AccessToken);
            Assert.True(credential.ExpiresAtUtc > DateTimeOffset.UtcNow.AddMinutes(50));
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    public async Task AuthorizationBroker_OnFirstUse_StartsInstalledApplicationAuthorization()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"vmf-oauth-first-use-{Guid.NewGuid():N}");
        var credentialsPath = Path.Combine(tempRoot, "oauth-client.json");
        Directory.CreateDirectory(tempRoot);
        var codeReceiver = new RecordingCodeReceiver();

        try
        {
            await File.WriteAllTextAsync(
                credentialsPath,
                "{\"installed\":{\"client_id\":\"test.apps.googleusercontent.com\",\"client_secret\":\"test-secret\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"redirect_uris\":[\"http://localhost\"]}}");
            var options = CreateOptions();
            options.CredentialsPath = credentialsPath;
            options.TokenStorePath = Path.Combine(tempRoot, "tokens");

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                new GoogleOAuthDesktopAuthorizationBroker(codeReceiver)
                    .AuthorizeAsync(options, CancellationToken.None));

            Assert.True(codeReceiver.WasInvoked);
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    public async Task AuthorizationBroker_RejectsNonDesktopClientJson()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"vmf-oauth-web-client-{Guid.NewGuid():N}");
        var credentialsPath = Path.Combine(tempRoot, "oauth-client.json");
        Directory.CreateDirectory(tempRoot);

        try
        {
            await File.WriteAllTextAsync(
                credentialsPath,
                "{\"web\":{\"client_id\":\"test.apps.googleusercontent.com\",\"client_secret\":\"test-secret\"}}");
            var options = CreateOptions();
            options.CredentialsPath = credentialsPath;
            options.TokenStorePath = Path.Combine(tempRoot, "tokens");

            var exception = await Assert.ThrowsAsync<OAuthDesktopConfigurationException>(() =>
                new GoogleOAuthDesktopAuthorizationBroker(new RecordingCodeReceiver())
                    .AuthorizeAsync(options, CancellationToken.None));

            Assert.Contains("Desktop app", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Theory]
    [InlineData(GoogleAuthenticationMode.ServiceAccount, typeof(ServiceAccountGoogleCredentialProvider))]
    [InlineData(GoogleAuthenticationMode.OAuthDesktop, typeof(OAuthDesktopGoogleCredentialProvider))]
    public void CredentialProviderFactory_SelectsConfiguredMode(
        GoogleAuthenticationMode mode,
        Type expectedType)
    {
        var options = CreateOptions();
        options.AuthenticationMode = mode;
        using var httpClient = new HttpClient();

        var provider = GoogleCredentialProviderFactory.Create(options, httpClient);

        Assert.IsType(expectedType, provider);
    }

    private static GooglePublisherOptions CreateOptions() => new()
    {
        AuthenticationMode = GoogleAuthenticationMode.OAuthDesktop,
        CredentialsPath = "oauth-client.json",
        TokenStorePath = "token-store",
        FolderId = "folder-id",
    };

    private sealed class StubAuthorizationBroker : IOAuthDesktopAuthorizationBroker
    {
        internal int CallCount { get; private set; }

        public Task<GoogleCredential> AuthorizeAsync(
            GooglePublisherOptions options,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return Task.FromResult(new GoogleCredential(
                "test-access-token",
                DateTimeOffset.UtcNow.AddHours(1)));
        }
    }

    private sealed class RecordingCodeReceiver : ICodeReceiver
    {
        public string RedirectUri => "http://127.0.0.1:54321/authorize/";

        internal bool WasInvoked { get; private set; }

        public Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(
            AuthorizationCodeRequestUrl url,
            CancellationToken taskCancellationToken)
        {
            WasInvoked = true;
            throw new OperationCanceledException("Test stops before the token exchange.");
        }
    }
}
