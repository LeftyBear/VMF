using Vmf.Publisher.Application;

namespace Vmf.Publisher.UnitTests;

public sealed class ReleaseNoteChangelogDraftHelperTests
{
    [Fact]
    public void CreateBullet_UsesRecordedDraftFieldsAndSourceReferences()
    {
        var result = CreateBullet(
            Manifest(
                CurrentField("version"),
                CurrentField("tag"),
                CurrentField("release status"),
                CurrentField("verification")),
            ["version", "tag", "release status", "verification"],
            [
                Current(
                    "docs/development/CURRENT_STATUS.md",
                    Field("version", "0.0.1-dev"),
                    Field("tag", "publisher-v0.0.1-dev"),
                    Field("release status", "published prerelease"),
                    Field("verification", "PASS")),
            ]);

        Assert.True(result.IsDraftAvailable);
        Assert.Empty(result.BlockingFields);
        Assert.StartsWith("- DRAFT ONLY - derived documentation: 0.0.1-dev", result.Bullet, StringComparison.Ordinal);
        Assert.Contains("publisher-v0.0.1-dev", result.Bullet, StringComparison.Ordinal);
        Assert.Contains("published prerelease", result.Bullet, StringComparison.Ordinal);
        Assert.Contains("verification PASS", result.Bullet, StringComparison.Ordinal);
        Assert.Contains("docs/development/CURRENT_STATUS.md", result.Bullet, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateBullet_FailsClosedWhenRequiredFieldIsNotRecorded()
    {
        var result = CreateBullet(
            Manifest(CurrentField("version"), CurrentField("release status")),
            ["version", "release status"],
            [Current("docs/development/CURRENT_STATUS.md", Field("version", "0.0.1-dev"))]);

        Assert.False(result.IsDraftAvailable);
        Assert.Empty(result.Bullet);
        Assert.Contains(result.BlockingFields, field => field.Name == "release status" && field.Status == ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.Missing);
    }

    [Fact]
    public void CreateBullet_FailsClosedOnConflict()
    {
        var result = CreateBullet(
            Manifest(CurrentField("version"), CurrentField("release status")),
            ["version", "release status"],
            [
                Current("docs/development/current-a.md", Field("version", "0.0.1-dev"), Field("release status", "published")),
                Current("docs/development/current-b.md", Field("version", "0.0.1-dev"), Field("release status", "held")),
            ]);

        Assert.False(result.IsDraftAvailable);
        Assert.Empty(result.Bullet);
        Assert.Contains(result.BlockingFields, field => field.Name == "release status" && field.Status == ReleaseNoteDraftFieldStatus.Conflict);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.Conflict);
    }

    [Fact]
    public void CreateBullet_FailsClosedOnManualOnlyGateField()
    {
        var result = CreateBullet(
            Manifest(CurrentField("version"), ManualOnlyField("release status")),
            ["version", "release status"],
            [Current("docs/development/current.md", Field("version", "0.0.1-dev"), Field("release status", "approved"))]);

        Assert.False(result.IsDraftAvailable);
        Assert.Empty(result.Bullet);
        Assert.Contains(result.BlockingFields, field => field.Name == "release status" && field.Status == ReleaseNoteDraftFieldStatus.ManualOnly);
        Assert.DoesNotContain("approved", result.Bullet, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateBullet_FailsClosedOnSensitiveValue()
    {
        var result = CreateBullet(
            Manifest(CurrentField("version"), CurrentField("release status")),
            ["version", "release status"],
            [
                Current(
                    "docs/development/current.md",
                    Field("version", "0.0.1-dev"),
                    Field("release status", "Authorization: Bearer abc")),
            ]);

        Assert.False(result.IsDraftAvailable);
        Assert.Empty(result.Bullet);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.SensitiveValueExcluded);
    }

    [Fact]
    public void CreateBullet_FailsClosedOnHistoricalOnlyAndUnmanifestedFields()
    {
        var result = CreateBullet(
            Manifest(CurrentField("version")),
            ["version", "release status"],
            [Historical("docs/releases/old.md", Field("version", "0.0.1-dev"), Field("release status", "published"))]);

        Assert.False(result.IsDraftAvailable);
        Assert.Empty(result.Bullet);
        Assert.Contains(result.BlockingFields, field => field.Name == "version" && field.Status == ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.Contains(result.BlockingFields, field => field.Name == "release status" && field.Status == ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.HistoricalNotPromoted);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.UnmanifestedField);
    }

    [Fact]
    public void CreateBullet_FailsClosedWhenSourceKindIsNotPermitted()
    {
        var result = CreateBullet(
            Manifest(CurrentField("version"), CurrentField("release status")),
            ["version", "release status"],
            [Template("docs/development/template.md", Field("version", "0.0.1-dev"), Field("release status", "template"))]);

        Assert.False(result.IsDraftAvailable);
        Assert.Empty(result.Bullet);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.SourceKindNotPermitted);
    }

    private static ReleaseNoteChangelogDraftResult CreateBullet(
        ReleaseNoteSourceFieldManifest manifest,
        IReadOnlyList<string> fields,
        IReadOnlyList<ReleaseNoteSourceRecord> records)
    {
        var draft = new ReleaseNoteDraftAssembler().Assemble(new ReleaseNoteDraftRequest("Publisher Release Note Draft", fields, records)
        {
            Manifest = manifest,
        });

        return new ReleaseNoteChangelogDraftHelper().CreateBullet(new ReleaseNoteChangelogDraftRequest(draft));
    }

    private static ReleaseNoteSourceRecord Current(string path, params ReleaseNoteSourceField[] fields) =>
        new(path, ReleaseNoteSourceRecordKind.CurrentState, fields);

    private static ReleaseNoteSourceRecord Historical(string path, params ReleaseNoteSourceField[] fields) =>
        new(path, ReleaseNoteSourceRecordKind.Historical, fields);

    private static ReleaseNoteSourceRecord Template(string path, params ReleaseNoteSourceField[] fields) =>
        new(path, ReleaseNoteSourceRecordKind.Template, fields);

    private static ReleaseNoteSourceField Field(string name, string value) => new(name, value);

    private static ReleaseNoteSourceFieldManifest Manifest(params ReleaseNoteSourceFieldManifestEntry[] fields) =>
        new(fields);

    private static ReleaseNoteSourceFieldManifestEntry CurrentField(string name) =>
        new(
            name,
            new HashSet<ReleaseNoteSourceRecordKind> { ReleaseNoteSourceRecordKind.CurrentState },
            ReleaseNoteSourceBoundary.CurrentState,
            ReleaseNoteMissingBehavior.NotRecorded,
            ReleaseNoteConflictBehavior.BlockingConflict,
            ManualOnly: false);

    private static ReleaseNoteSourceFieldManifestEntry ManualOnlyField(string name) =>
        new(
            name,
            new HashSet<ReleaseNoteSourceRecordKind>(),
            ReleaseNoteSourceBoundary.CurrentState,
            ReleaseNoteMissingBehavior.NotRecorded,
            ReleaseNoteConflictBehavior.BlockingConflict,
            ManualOnly: true);
}
