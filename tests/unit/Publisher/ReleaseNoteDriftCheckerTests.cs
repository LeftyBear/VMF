using Vmf.Publisher.Application;

namespace Vmf.Publisher.UnitTests;

public sealed class ReleaseNoteDriftCheckerTests
{
    [Fact]
    public void Check_MatchingReleaseNoteFieldReportsMatchWithSourceReference()
    {
        var result = Check(
            Manifest(CurrentField("version")),
            ["version"],
            [ReleaseNoteField("version", "0.0.1-dev")],
            [Current("docs/development/CURRENT_STATUS.md", Field("version", "0.0.1-dev"))]);

        Assert.False(result.HasBlockingDrift);
        var field = AssertField(result, "version", ReleaseNoteDriftStatus.Match);
        Assert.Equal("0.0.1-dev", field.ExpectedValue);
        Assert.Equal("0.0.1-dev", field.ActualValue);
        Assert.Contains("docs/development/CURRENT_STATUS.md", field.SourcePaths);
    }

    [Fact]
    public void Check_MissingReleaseNoteFieldReportsMissingAndBlocks()
    {
        var result = Check(
            Manifest(CurrentField("tag")),
            ["tag"],
            [],
            [Current("docs/development/CURRENT_STATUS.md", Field("tag", "publisher-v0.0.1-dev"))]);

        Assert.True(result.HasBlockingDrift);
        var field = AssertField(result, "tag", ReleaseNoteDriftStatus.Missing);
        Assert.Equal("publisher-v0.0.1-dev", field.ExpectedValue);
        Assert.Equal("MISSING", field.ActualValue);
    }

    [Fact]
    public void Check_ConflictingReleaseNoteValueReportsConflictAndBlocks()
    {
        var result = Check(
            Manifest(CurrentField("sha-256")),
            ["sha-256"],
            [ReleaseNoteField("sha-256", "73582c")],
            [Current("docs/development/reconciliation.md", Field("sha-256", "0174810d"))]);

        Assert.True(result.HasBlockingDrift);
        var field = AssertField(result, "sha-256", ReleaseNoteDriftStatus.Conflict);
        Assert.Equal("0174810d", field.ExpectedValue);
        Assert.Equal("73582c", field.ActualValue);
    }

    [Fact]
    public void Check_DuplicateReleaseNoteFieldReportsConflictAndBlocks()
    {
        var result = Check(
            Manifest(CurrentField("tag")),
            ["tag"],
            [ReleaseNoteField("tag", "publisher-v0.0.1-dev"), ReleaseNoteField("tag", "publisher-v0.0.1-dev")],
            [Current("docs/development/CURRENT_STATUS.md", Field("tag", "publisher-v0.0.1-dev"))]);

        Assert.True(result.HasBlockingDrift);
        var field = AssertField(result, "tag", ReleaseNoteDriftStatus.Conflict);
        Assert.Equal("publisher-v0.0.1-dev", field.ExpectedValue);
        Assert.Equal("publisher-v0.0.1-dev | publisher-v0.0.1-dev", field.ActualValue);
    }

    [Fact]
    public void Check_ConflictingAllowListedSourcesReportsConflictAndBlocks()
    {
        var result = Check(
            Manifest(CurrentField("target commit")),
            ["target commit"],
            [ReleaseNoteField("target commit", "382bd")],
            [
                Current("docs/development/current-a.md", Field("target commit", "382bd")),
                Current("docs/development/current-b.md", Field("target commit", "different")),
            ]);

        Assert.True(result.HasBlockingDrift);
        var field = AssertField(result, "target commit", ReleaseNoteDriftStatus.Conflict);
        Assert.Equal("CONFLICT", field.ExpectedValue);
        Assert.Equal("CONFLICT", field.ActualValue);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.Conflict);
    }

    [Fact]
    public void Check_HistoricalOnlySourceDoesNotSatisfyCurrentStateAndBlocksAsMissing()
    {
        var result = Check(
            Manifest(CurrentField("package size")),
            ["package size"],
            [ReleaseNoteField("package size", "983404 bytes")],
            [Historical("docs/releases/old.md", Field("package size", "983404 bytes"))]);

        Assert.True(result.HasBlockingDrift);
        var field = AssertField(result, "package size", ReleaseNoteDriftStatus.Missing);
        Assert.Equal("NOT RECORDED", field.ExpectedValue);
        Assert.Equal("MISSING", field.ActualValue);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.HistoricalNotPromoted);
    }

    [Fact]
    public void Check_ManualOnlyGateFieldBlocksAsMissingInsteadOfInferringApproval()
    {
        var result = Check(
            Manifest(ManualOnlyField("vendor clearance")),
            ["vendor clearance"],
            [ReleaseNoteField("vendor clearance", "Accepted")],
            [Current("docs/development/current.md", Field("vendor clearance", "Accepted"))]);

        Assert.True(result.HasBlockingDrift);
        var field = AssertField(result, "vendor clearance", ReleaseNoteDriftStatus.Missing);
        Assert.Equal("MANUAL ONLY", field.ExpectedValue);
        Assert.Equal("MISSING", field.ActualValue);
    }

    [Fact]
    public void Check_SensitiveSourceValueIsExcludedAndBlocksAsMissing()
    {
        var result = Check(
            Manifest(CurrentField("artifact path")),
            ["artifact path"],
            [ReleaseNoteField("artifact path", "dist/release.zip")],
            [Current("docs/development/current.md", Field("artifact path", @"C:\Users\biz\private\release.zip"))]);

        Assert.True(result.HasBlockingDrift);
        var field = AssertField(result, "artifact path", ReleaseNoteDriftStatus.Missing);
        Assert.Equal("NOT RECORDED", field.ExpectedValue);
        Assert.Equal("MISSING", field.ActualValue);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.SensitiveValueExcluded);
    }

    private static ReleaseNoteDriftCheckResult Check(
        ReleaseNoteSourceFieldManifest manifest,
        IReadOnlyList<string> fields,
        IReadOnlyList<ReleaseNoteSourceField> releaseNoteFields,
        IReadOnlyList<ReleaseNoteSourceRecord> records) =>
        new ReleaseNoteDriftChecker().Check(new ReleaseNoteDriftCheckRequest(fields, releaseNoteFields, records)
        {
            Manifest = manifest,
        });

    private static ReleaseNoteSourceRecord Current(string path, params ReleaseNoteSourceField[] fields) =>
        new(path, ReleaseNoteSourceRecordKind.CurrentState, fields);

    private static ReleaseNoteSourceRecord Historical(string path, params ReleaseNoteSourceField[] fields) =>
        new(path, ReleaseNoteSourceRecordKind.Historical, fields);

    private static ReleaseNoteSourceField Field(string name, string value) => new(name, value);

    private static ReleaseNoteSourceField ReleaseNoteField(string name, string value) => new(name, value);

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

    private static ReleaseNoteDriftField AssertField(
        ReleaseNoteDriftCheckResult result,
        string name,
        ReleaseNoteDriftStatus status)
    {
        var field = Assert.Single(result.Fields, field => field.Name == name);
        Assert.Equal(status, field.Status);
        return field;
    }
}
