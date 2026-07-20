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
        ];

        var json = new GoogleDocsRequestMapper().MapBatchUpdate(operations);

        using var document = JsonDocument.Parse(json);
        var requests = document.RootElement.GetProperty("requests");
        Assert.Equal(4, requests.GetArrayLength());
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
    }
}
