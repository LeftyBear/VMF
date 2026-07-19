using System.Net;
using System.Text.Json;

namespace Vmf.Publisher.Infrastructure.Google;

internal sealed class GoogleApiException : HttpRequestException
{
    internal GoogleApiException(string apiName, HttpStatusCode statusCode, string errorCode)
        : base(
            $"{apiName} failed: HTTP {(int)statusCode} ({Sanitize(errorCode)}).",
            null,
            statusCode)
    {
        ApiName = apiName;
        ErrorCode = Sanitize(errorCode);
    }

    internal string ApiName { get; }

    internal string ErrorCode { get; }

    private static string Sanitize(string value)
    {
        var safeValue = new string(value
            .Where(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.')
            .Take(80)
            .ToArray());
        return string.IsNullOrWhiteSpace(safeValue) ? "UNKNOWN" : safeValue;
    }
}

internal static class GoogleApiError
{
    internal static GoogleApiException Create(
        string apiName,
        HttpStatusCode statusCode,
        string responseBody)
    {
        var errorCode = TryReadErrorCode(responseBody) ?? $"HTTP_{(int)statusCode}";
        return new GoogleApiException(apiName, statusCode, errorCode);
    }

    private static string? TryReadErrorCode(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            if (!document.RootElement.TryGetProperty("error", out var error))
            {
                return null;
            }

            if (error.ValueKind == JsonValueKind.String)
            {
                return error.GetString();
            }

            if (error.ValueKind != JsonValueKind.Object)
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
                errors[0].TryGetProperty("reason", out var reason))
            {
                return reason.GetString();
            }
        }
        catch (JsonException)
        {
            // A non-JSON response is represented by its HTTP status only.
        }

        return null;
    }
}
