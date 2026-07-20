using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Infrastructure.Google;

/// <summary>Applies document operations through the Google Docs REST API.</summary>
public sealed class GoogleDocsClient : IGoogleDocsClient
{
    private const int MaxAttempts = 3;
    private readonly IGoogleCredentialProvider credentialProvider;
    private readonly IGoogleDocsRequestMapper requestMapper;
    private readonly HttpClient httpClient;

    /// <summary>Initializes a Google Docs client.</summary>
    /// <param name="credentialProvider">The Google credential provider.</param>
    /// <param name="requestMapper">The Google Docs request mapper.</param>
    /// <param name="httpClient">The HTTP transport.</param>
    public GoogleDocsClient(
        IGoogleCredentialProvider credentialProvider,
        IGoogleDocsRequestMapper requestMapper,
        HttpClient httpClient)
    {
        this.credentialProvider = credentialProvider ?? throw new ArgumentNullException(nameof(credentialProvider));
        this.requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task ApplyOperationsAsync(
        string documentId,
        IReadOnlyList<DocumentOperation> operations,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        ArgumentNullException.ThrowIfNull(operations);
        if (operations.Count == 0)
        {
            return;
        }

        var requestBody = requestMapper.MapBatchUpdate(operations);
        await SendAsync(
            HttpMethod.Post,
            $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}:batchUpdate",
            requestBody,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task InsertTableAsync(
        string documentId,
        int rows,
        int columns,
        int index,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows));
        }

        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columns));
        }

        if (index < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var requestBody = JsonSerializer.Serialize(new
        {
            requests = new[]
            {
                new
                {
                    insertTable = new
                    {
                        rows,
                        columns,
                        location = new { index },
                    },
                },
            },
        });
        await SendAsync(
            HttpMethod.Post,
            $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}:batchUpdate",
            requestBody,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<GoogleDocumentSnapshot> GetDocumentAsync(
        string documentId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        var responseBody = await SendAsync(
            HttpMethod.Get,
            $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}",
            content: null,
            cancellationToken).ConfigureAwait(false);
        return ParseDocument(responseBody);
    }

    private async Task<string> SendAsync(
        HttpMethod method,
        string requestUri,
        string? content,
        CancellationToken cancellationToken)
    {
        var credential = await credentialProvider.GetCredentialAsync(cancellationToken).ConfigureAwait(false);
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            using var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credential.AccessToken);
            if (content is not null)
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return responseBody;
            }

            if (attempt == MaxAttempts || !IsRetryable(response.StatusCode))
            {
                throw GoogleApiError.Create("Google Docs API", response.StatusCode, responseBody);
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100 * (1 << (attempt - 1))), cancellationToken)
                .ConfigureAwait(false);
        }

        throw new InvalidOperationException("The Google Docs retry loop terminated unexpectedly.");
    }

    private static bool IsRetryable(System.Net.HttpStatusCode statusCode) => statusCode is
        System.Net.HttpStatusCode.TooManyRequests or
        System.Net.HttpStatusCode.InternalServerError or
        System.Net.HttpStatusCode.BadGateway or
        System.Net.HttpStatusCode.ServiceUnavailable or
        System.Net.HttpStatusCode.GatewayTimeout;

    private static GoogleDocumentSnapshot ParseDocument(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);
        var tables = new List<GoogleTableSnapshot>();
        if (!document.RootElement.TryGetProperty("body", out var body) ||
            !body.TryGetProperty("content", out var content))
        {
            return new GoogleDocumentSnapshot(tables);
        }

        foreach (var element in content.EnumerateArray())
        {
            if (!element.TryGetProperty("table", out var table))
            {
                continue;
            }

            var rows = new List<GoogleTableRowSnapshot>();
            if (table.TryGetProperty("tableRows", out var tableRows))
            {
                foreach (var tableRow in tableRows.EnumerateArray())
                {
                    var cells = new List<GoogleTableCellSnapshot>();
                    if (tableRow.TryGetProperty("tableCells", out var tableCells))
                    {
                        foreach (var cell in tableCells.EnumerateArray())
                        {
                            cells.Add(new GoogleTableCellSnapshot(
                                ReadContentIndex(cell, "startIndex", useFirstContentElement: true),
                                ReadContentIndex(cell, "endIndex", useFirstContentElement: false)));
                        }
                    }

                    rows.Add(new GoogleTableRowSnapshot(cells));
                }
            }

            tables.Add(new GoogleTableSnapshot(
                ReadNullableInt(element, "startIndex"),
                ReadNullableInt(element, "endIndex"),
                rows));
        }

        return new GoogleDocumentSnapshot(tables);
    }

    private static int? ReadContentIndex(
        JsonElement cell,
        string propertyName,
        bool useFirstContentElement)
    {
        if (!cell.TryGetProperty("content", out var content))
        {
            return null;
        }

        var elements = content.EnumerateArray();
        if (useFirstContentElement)
        {
            return elements.MoveNext() ? ReadNullableInt(elements.Current, propertyName) : null;
        }

        int? result = null;
        while (elements.MoveNext())
        {
            result = ReadNullableInt(elements.Current, propertyName) ?? result;
        }

        return result;
    }

    private static int? ReadNullableInt(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value)
            ? value
            : null;
}
