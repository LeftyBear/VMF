using Vmf.Publisher.Application;

namespace Vmf.Publisher.UnitTests;

public sealed class ReleaseNoteDraftAssemblerTests
{
    [Fact]
    public void Assemble_UsesOnlyCurrentStateAllowListedFields()
    {
        var result = Assemble(
            Manifest(CurrentField("version"), CurrentField("tag")),
            ["version", "tag"],
            [
                Current("docs/development/CURRENT_STATUS.md", Field("version", "0.0.1-dev"), Field("tag", "publisher-v0.0.1-dev")),
                Historical("docs/development/old.md", Field("version", "0.0.0-dev")),
            ]);

        Assert.False(result.HasBlockingConflict);
        AssertField(result, "version", "0.0.1-dev", ReleaseNoteDraftFieldStatus.Recorded);
        AssertField(result, "tag", "publisher-v0.0.1-dev", ReleaseNoteDraftFieldStatus.Recorded);
        Assert.Contains("DRAFT ONLY", result.Markdown);
        Assert.Contains("docs/development/CURRENT_STATUS.md", result.Markdown);
        Assert.DoesNotContain("0.0.0-dev", result.Markdown);
    }

    [Fact]
    public void Assemble_MissingCurrentStateFieldBecomesNotRecorded()
    {
        var result = Assemble(
            Manifest(CurrentField("sha-256")),
            ["sha-256"],
            [Current("docs/development/CURRENT_STATUS.md", Field("version", "0.0.1-dev"))]);

        AssertField(result, "sha-256", "NOT RECORDED", ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.Missing);
    }

    [Fact]
    public void Assemble_ConflictingCurrentStateFieldsBecomeConflictAndBlockApprovalReadyOutput()
    {
        var result = Assemble(
            Manifest(CurrentField("target commit")),
            ["target commit"],
            [
                Current("docs/development/current-a.md", Field("target commit", "abc123")),
                Current("docs/development/current-b.md", Field("target commit", "def456")),
            ]);

        Assert.True(result.HasBlockingConflict);
        AssertField(result, "target commit", "CONFLICT", ReleaseNoteDraftFieldStatus.Conflict);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.Conflict);
    }

    [Fact]
    public void Assemble_HistoricalFieldIsNotPromotedToCurrentState()
    {
        var result = Assemble(
            Manifest(CurrentField("release status")),
            ["release status"],
            [Historical("docs/development/historical-approval.md", Field("release status", "Risk Accepted Go"))]);

        AssertField(result, "release status", "NOT RECORDED", ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.HistoricalNotPromoted);
        Assert.DoesNotContain("Risk Accepted Go", result.Markdown);
    }

    [Theory]
    [InlineData("release authorization")]
    [InlineData("publication authorization")]
    [InlineData("risk acceptance")]
    [InlineData("vendor clearance")]
    [InlineData("Avast safety certification")]
    public void Assemble_NonInferableGateFieldsRemainManualOnly(string fieldName)
    {
        var result = Assemble(
            Manifest(ManualOnlyField(fieldName)),
            [fieldName],
            [Current("docs/development/current.md", Field(fieldName, "approved"))]);

        AssertField(result, fieldName, "MANUAL ONLY", ReleaseNoteDraftFieldStatus.ManualOnly);
        Assert.DoesNotContain("approved", result.Markdown);
    }

    [Fact]
    public void Assemble_SensitiveValuesAreExcluded()
    {
        var result = Assemble(
            Manifest(CurrentField("artifact path"), CurrentField("release url"), CurrentField("token")),
            ["artifact path", "release url", "token"],
            [
                Current(
                    "docs/development/current.md",
                    Field("artifact path", @"C:\Users\biz\Documents\Project\VMF\dist\release.zip"),
                    Field("release url", "http://localhost:8080/private"),
                    Field("token", "Authorization: Bearer abc")),
            ]);

        AssertField(result, "artifact path", "NOT RECORDED", ReleaseNoteDraftFieldStatus.NotRecorded);
        AssertField(result, "release url", "NOT RECORDED", ReleaseNoteDraftFieldStatus.NotRecorded);
        AssertField(result, "token", "NOT RECORDED", ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.DoesNotContain(@"C:\Users\biz", result.Markdown);
        Assert.DoesNotContain("localhost:8080", result.Markdown);
        Assert.DoesNotContain("Bearer abc", result.Markdown);
        Assert.Equal(3, result.Diagnostics.Count(diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.SensitiveValueExcluded));
    }

    [Fact]
    public void Assemble_UnmanifestedFieldFailsClosedAsNotRecorded()
    {
        var result = Assemble(
            Manifest(CurrentField("version")),
            ["version", "asset name"],
            [Current("docs/development/current.md", Field("version", "0.0.1-dev"), Field("asset name", "vmf-publisher.zip"))]);

        AssertField(result, "version", "0.0.1-dev", ReleaseNoteDraftFieldStatus.Recorded);
        AssertField(result, "asset name", "NOT RECORDED", ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.DoesNotContain("vmf-publisher.zip", result.Markdown);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.UnmanifestedField);
    }

    [Fact]
    public void Assemble_SourceKindNotPermittedByManifestIsNotRecorded()
    {
        var result = Assemble(
            Manifest(CurrentField("version")),
            ["version"],
            [Template("docs/development/template.md", Field("version", "template-version"))]);

        AssertField(result, "version", "NOT RECORDED", ReleaseNoteDraftFieldStatus.NotRecorded);
        Assert.DoesNotContain("template-version", result.Markdown);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.SourceKindNotPermitted);
    }

    [Theory]
    [InlineData("release approval")]
    [InlineData("release authorization")]
    [InlineData("publication authorization")]
    [InlineData("risk acceptance")]
    [InlineData("vendor clearance")]
    [InlineData("Avast safety certification")]
    public void Assemble_AuthorizationRiskAndVendorFieldsAreManifestedManualOnly(string fieldName)
    {
        var manifestField = ManualOnlyField(fieldName);

        Assert.True(manifestField.ManualOnly);
        Assert.Empty(manifestField.PermittedSourceKinds);
        Assert.Equal(ReleaseNoteSourceBoundary.CurrentState, manifestField.SourceBoundary);

        var result = Assemble(
            Manifest(manifestField),
            [fieldName],
            [Current("docs/development/current.md", Field(fieldName, "PASS"))]);

        AssertField(result, fieldName, "MANUAL ONLY", ReleaseNoteDraftFieldStatus.ManualOnly);
        Assert.DoesNotContain("PASS", result.Markdown);
    }

    private static ReleaseNoteDraftResult Assemble(
        ReleaseNoteSourceFieldManifest manifest,
        IReadOnlyList<string> fields,
        IReadOnlyList<ReleaseNoteSourceRecord> records) =>
        new ReleaseNoteDraftAssembler().Assemble(new ReleaseNoteDraftRequest("Publisher Release Note Draft", fields, records)
        {
            Manifest = manifest,
        });

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

    private static void AssertField(
        ReleaseNoteDraftResult result,
        string name,
        string value,
        ReleaseNoteDraftFieldStatus status)
    {
        var field = Assert.Single(result.Fields, field => field.Name == name);
        Assert.Equal(value, field.Value);
        Assert.Equal(status, field.Status);
    }
}
