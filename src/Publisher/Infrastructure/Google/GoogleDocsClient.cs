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
    public async Task InsertInlineImageAsync(
        string documentId,
        Uri imageUri,
        ImageSize size,
        int index,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        ArgumentNullException.ThrowIfNull(imageUri);
        ArgumentNullException.ThrowIfNull(size);
        if (index < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var requestBody = JsonSerializer.Serialize(new
        {
            requests = new object[]
            {
                new
                {
                    insertInlineImage = new
                    {
                        uri = imageUri.AbsoluteUri,
                        objectSize = new
                        {
                            width = Dimension(size.WidthPoints),
                            height = Dimension(size.HeightPoints),
                        },
                        location = new { index },
                    },
                },
                new
                {
                    insertText = new
                    {
                        location = new { index = index + 1 },
                        text = "\n",
                    },
                },
                new
                {
                    updateParagraphStyle = new
                    {
                        range = new { startIndex = index, endIndex = index + 1 },
                        paragraphStyle = new { alignment = "START" },
                        fields = "alignment",
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
        var images = new List<GoogleInlineImageSnapshot>();
        var inlineObjects = ParseInlineObjects(document.RootElement);
        if (!document.RootElement.TryGetProperty("body", out var body) ||
            !body.TryGetProperty("content", out var content))
        {
            return new GoogleDocumentSnapshot(tables, images);
        }

        foreach (var element in content.EnumerateArray())
        {
            if (element.TryGetProperty("paragraph", out var paragraph) &&
                paragraph.TryGetProperty("elements", out var paragraphElements))
            {
                foreach (var paragraphElement in paragraphElements.EnumerateArray())
                {
                    if (!paragraphElement.TryGetProperty("inlineObjectElement", out var inlineElement) ||
                        !inlineElement.TryGetProperty("inlineObjectId", out var idProperty))
                    {
                        continue;
                    }

                    var inlineObjectId = idProperty.GetString();
                    inlineObjects.TryGetValue(
                        inlineObjectId ?? string.Empty,
                        out var objectProperties);
                    images.Add(new GoogleInlineImageSnapshot(
                        ReadNullableInt(element, "startIndex"),
                        ReadNullableInt(element, "endIndex"),
                        ReadNullableInt(paragraphElement, "startIndex"),
                        ReadNullableInt(paragraphElement, "endIndex"),
                        inlineObjectId,
                        objectProperties?.Size,
                        objectProperties?.Title,
                        objectProperties?.Description));
                }
            }

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

        return new GoogleDocumentSnapshot(tables, images);
    }

    private static Dictionary<string, EmbeddedObjectProperties> ParseInlineObjects(
        JsonElement root)
    {
        var result = new Dictionary<string, EmbeddedObjectProperties>(StringComparer.Ordinal);
        if (!root.TryGetProperty("inlineObjects", out var inlineObjects) ||
            inlineObjects.ValueKind != JsonValueKind.Object)
        {
            return result;
        }

        foreach (var property in inlineObjects.EnumerateObject())
        {
            if (!property.Value.TryGetProperty("inlineObjectProperties", out var inlineProperties) ||
                !inlineProperties.TryGetProperty("embeddedObject", out var embeddedObject))
            {
                continue;
            }

            ImageSize? size = null;
            if (embeddedObject.TryGetProperty("size", out var sizeElement) &&
                TryReadPoints(sizeElement, "width", out var width) &&
                TryReadPoints(sizeElement, "height", out var height))
            {
                try
                {
                    size = new ImageSize(width, height);
                }
                catch (ArgumentOutOfRangeException)
                {
                    size = null;
                }
            }

            result[property.Name] = new EmbeddedObjectProperties(
                size,
                ReadNullableString(embeddedObject, "title"),
                ReadNullableString(embeddedObject, "description"));
        }

        return result;
    }

    private static bool TryReadPoints(
        JsonElement size,
        string propertyName,
        out double points)
    {
        points = 0;
        return size.TryGetProperty(propertyName, out var dimension) &&
            dimension.TryGetProperty("magnitude", out var magnitude) &&
            magnitude.TryGetDouble(out points) &&
            (!dimension.TryGetProperty("unit", out var unit) ||
                unit.GetString() is "PT" or "UNIT_UNSPECIFIED");
    }

    private static string? ReadNullableString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) &&
        property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

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

    private static object Dimension(double magnitude) => new { magnitude, unit = "PT" };

    private sealed record EmbeddedObjectProperties(
        ImageSize? Size,
        string? Title,
        string? Description);
}
