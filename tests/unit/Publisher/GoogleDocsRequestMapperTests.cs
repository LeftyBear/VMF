using System.Text.Json;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.UnitTests;

public sealed class GoogleDocsRequestMapperTests
{
    [Fact]
    public void MapBatchUpdate_MapsEverySupportedOperation()
    {
        DocumentOperation[] operations =
        [
            new(DocumentOperationKind.InsertText, 1, text: "Heading\n"),
            new(DocumentOperationKind.ApplyHeading, 1, 9, level: 1),
            new(DocumentOperationKind.CreateBullet, 9, 14, listKind: ListKind.Unordered),
            new(DocumentOperationKind.CreateBullet, 14, 20, listKind: ListKind.Ordered),
            new(DocumentOperationKind.UpdateTextStyle, 1, 4, inlineStyle: InlineTextStyle.Bold),
            new(DocumentOperationKind.UpdateTextStyle, 4, 7, inlineStyle: InlineTextStyle.Italic),
            new(
                DocumentOperationKind.UpdateTextStyle,
                7,
                11,
                inlineStyle: InlineTextStyle.Link,
                url: new Uri("https://example.com/")),
            new(
                DocumentOperationKind.UpdateParagraphAlignment,
                12,
                20,
                tableAlignment: TableAlignment.Right),
        ];

        var json = new GoogleDocsRequestMapper().MapBatchUpdate(operations);

        using var document = JsonDocument.Parse(json);
        var requests = document.RootElement.GetProperty("requests");
        Assert.Equal(8, requests.GetArrayLength());
        Assert.Equal("Heading\n", requests[0].GetProperty("insertText").GetProperty("text").GetString());
        Assert.Equal(
            "HEADING_1",
            requests[1].GetProperty("updateParagraphStyle").GetProperty("paragraphStyle")
                .GetProperty("namedStyleType").GetString());
        Assert.Equal(
            "BULLET_DISC_CIRCLE_SQUARE",
            requests[2].GetProperty("createParagraphBullets").GetProperty("bulletPreset").GetString());
        Assert.Equal(
            "NUMBERED_DECIMAL_ALPHA_ROMAN",
            requests[3].GetProperty("createParagraphBullets").GetProperty("bulletPreset").GetString());
        AssertTextStyle(requests[4], "bold", JsonValueKind.True, "bold");
        AssertTextStyle(requests[5], "italic", JsonValueKind.True, "italic");
        var linkRequest = requests[6].GetProperty("updateTextStyle");
        Assert.Equal("link", linkRequest.GetProperty("fields").GetString());
        Assert.Equal(
            "https://example.com/",
            linkRequest.GetProperty("textStyle").GetProperty("link").GetProperty("url").GetString());
        var alignmentRequest = requests[7].GetProperty("updateParagraphStyle");
        Assert.Equal("alignment", alignmentRequest.GetProperty("fields").GetString());
        Assert.Equal(
            "END",
            alignmentRequest.GetProperty("paragraphStyle").GetProperty("alignment").GetString());
    }

    private static void AssertTextStyle(
        JsonElement request,
        string property,
        JsonValueKind expectedKind,
        string fields)
    {
        var update = request.GetProperty("updateTextStyle");
        Assert.Equal(fields, update.GetProperty("fields").GetString());
        Assert.Equal(expectedKind, update.GetProperty("textStyle").GetProperty(property).ValueKind);
        Assert.Single(update.GetProperty("textStyle").EnumerateObject());
    }
}
