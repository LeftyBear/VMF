using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Application;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Executes Google Docs batchUpdate requests for physical updates.</summary>
public sealed class GoogleDocsBatchUpdateClient : IGoogleDocsBatchUpdateClient
{
    private readonly IGoogleCredentialProvider credentialProvider;
    private readonly HttpClient httpClient;

    /// <summary>Initializes the Google Docs batchUpdate client.</summary>
    public GoogleDocsBatchUpdateClient(
        IGoogleCredentialProvider credentialProvider,
        HttpClient httpClient)
    {
        this.credentialProvider = credentialProvider ?? throw new ArgumentNullException(nameof(credentialProvider));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<GoogleDocsBatchUpdateResponse> ExecuteAsync(
        PhysicalUpdateRequestBatch batch,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(batch);
        RequestDeliveryState deliveryState = RequestDeliveryState.NotSent;
        try
        {
            var credential = await credentialProvider.GetCredentialAsync(cancellationToken).ConfigureAwait(false);
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(batch.DocumentId)}:batchUpdate");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credential.AccessToken);
            request.Content = new StringContent(Serialize(batch), Encoding.UTF8, "application/json");
            deliveryState = RequestDeliveryState.Unknown;
            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            deliveryState = RequestDeliveryState.Sent;
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw Normalize(response, body, RequestDeliveryState.Sent);
            }

            return new GoogleDocsBatchUpdateResponse(ReadRevisionId(body) ?? batch.RequiredRevisionId);
        }
        catch (GoogleDocsBatchUpdateException)
        {
            throw;
        }
        catch (OperationCanceledException) when (deliveryState != RequestDeliveryState.NotSent)
        {
            throw new GoogleDocsBatchUpdateException(
                httpStatusCode: null,
                googleErrorReason: "CANCELED",
                retryAfter: null,
                RequestDeliveryState.Unknown,
                "The batchUpdate request was canceled after send began.");
        }
        catch (OperationCanceledException exception)
        {
            throw new GoogleDocsBatchUpdateException(
                httpStatusCode: null,
                googleErrorReason: "CANCELED",
                retryAfter: null,
                RequestDeliveryState.NotSent,
                "The batchUpdate request was canceled before send.",
                exception);
        }
        catch (HttpRequestException exception)
        {
            throw new GoogleDocsBatchUpdateException(
                exception.StatusCode,
                exception.StatusCode is null ? "TRANSPORT_FAILURE" : $"HTTP_{(int)exception.StatusCode.Value}",
                retryAfter: null,
                deliveryState,
                exception.Message,
                exception);
        }
    }

    private static string Serialize(PhysicalUpdateRequestBatch batch) => JsonSerializer.Serialize(new
    {
        requests = batch.Requests,
        writeControl = new
        {
            requiredRevisionId = batch.RequiredRevisionId,
        },
    });

    private static GoogleDocsBatchUpdateException Normalize(
        HttpResponseMessage response,
        string body,
        RequestDeliveryState deliveryState)
    {
        var reason = TryReadErrorReason(body) ?? $"HTTP_{(int)response.StatusCode}";
        return new GoogleDocsBatchUpdateException(
            response.StatusCode,
            reason,
            response.Headers.RetryAfter?.Delta,
            deliveryState,
            $"Google Docs batchUpdate failed: HTTP {(int)response.StatusCode} ({reason}).");
    }

    private static string? ReadRevisionId(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("writeControl", out var writeControl) &&
                writeControl.TryGetProperty("requiredRevisionId", out var required) &&
                required.ValueKind == JsonValueKind.String)
            {
                return required.GetString();
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    private static string? TryReadErrorReason(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            if (!document.RootElement.TryGetProperty("error", out var error))
            {
                return null;
            }

            if (error.TryGetProperty("status", out var status) && status.ValueKind == JsonValueKind.String)
            {
                return status.GetString();
            }

            if (error.TryGetProperty("errors", out var errors) &&
                errors.ValueKind == JsonValueKind.Array &&
                errors.GetArrayLength() > 0 &&
                errors[0].TryGetProperty("reason", out var reason) &&
                reason.ValueKind == JsonValueKind.String)
            {
                return reason.GetString();
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }
}
