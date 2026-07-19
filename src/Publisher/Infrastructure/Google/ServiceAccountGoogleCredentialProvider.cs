using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Obtains OAuth access tokens from a Google service-account JSON credential.</summary>
public sealed class ServiceAccountGoogleCredentialProvider : IGoogleCredentialProvider
{
    private const string Scope =
        "https://www.googleapis.com/auth/documents https://www.googleapis.com/auth/drive.file";

    private readonly GooglePublisherOptions options;
    private readonly HttpClient httpClient;
    private readonly SemaphoreSlim refreshGate = new(1, 1);
    private GoogleCredential? cachedCredential;

    /// <summary>Initializes the credential provider.</summary>
    /// <param name="options">Google publisher settings.</param>
    /// <param name="httpClient">The HTTP transport.</param>
    public ServiceAccountGoogleCredentialProvider(GooglePublisherOptions options, HttpClient httpClient)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<GoogleCredential> GetCredentialAsync(CancellationToken cancellationToken)
    {
        if (cachedCredential is not null && cachedCredential.ExpiresAtUtc > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            return cachedCredential;
        }

        await refreshGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (cachedCredential is not null &&
                cachedCredential.ExpiresAtUtc > DateTimeOffset.UtcNow.AddMinutes(5))
            {
                return cachedCredential;
            }

            if (string.IsNullOrWhiteSpace(options.CredentialsPath))
            {
                throw new InvalidOperationException(
                    "Google:CredentialsPath is required. Keep the credential file outside the repository.");
            }

            var json = await File.ReadAllTextAsync(options.CredentialsPath, cancellationToken)
                .ConfigureAwait(false);
            using var credentialDocument = JsonDocument.Parse(json);
            var root = credentialDocument.RootElement;
            var clientEmail = GetRequiredString(root, "client_email");
            var privateKey = GetRequiredString(root, "private_key");
            var tokenUri = root.TryGetProperty("token_uri", out var tokenUriProperty)
                ? tokenUriProperty.GetString()
                : "https://oauth2.googleapis.com/token";

            if (string.IsNullOrWhiteSpace(tokenUri))
            {
                throw new InvalidOperationException("The service-account token_uri is invalid.");
            }

            var assertion = CreateAssertion(clientEmail, privateKey, tokenUri);
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                ["assertion"] = assertion,
            });
            using var response = await httpClient.PostAsync(tokenUri, content, cancellationToken)
                .ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Google OAuth token request failed with HTTP {(int)response.StatusCode}.");
            }

            using var tokenDocument = JsonDocument.Parse(responseBody);
            var tokenRoot = tokenDocument.RootElement;
            var accessToken = GetRequiredString(tokenRoot, "access_token");
            var expiresIn = tokenRoot.TryGetProperty("expires_in", out var expiresProperty)
                ? expiresProperty.GetInt32()
                : 3600;
            cachedCredential = new GoogleCredential(
                accessToken,
                DateTimeOffset.UtcNow.AddSeconds(expiresIn));
            return cachedCredential;
        }
        finally
        {
            refreshGate.Release();
        }
    }

    private static string CreateAssertion(string clientEmail, string privateKey, string tokenUri)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var header = Base64UrlEncode(Encoding.UTF8.GetBytes("{\"alg\":\"RS256\",\"typ\":\"JWT\"}"));
        var payload = JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object>
        {
            ["iss"] = clientEmail,
            ["scope"] = Scope,
            ["aud"] = tokenUri,
            ["iat"] = now,
            ["exp"] = now + 3600,
        });
        var unsignedToken = $"{header}.{Base64UrlEncode(payload)}";

        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);
        var signature = rsa.SignData(
            Encoding.ASCII.GetBytes(unsignedToken),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        return $"{unsignedToken}.{Base64UrlEncode(signature)}";
    }

    private static string GetRequiredString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) ||
            string.IsNullOrWhiteSpace(property.GetString()))
        {
            throw new InvalidOperationException($"The service-account credential is missing '{propertyName}'.");
        }

        return property.GetString()!;
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
