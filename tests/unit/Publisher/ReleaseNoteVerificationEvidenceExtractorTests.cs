using Vmf.Publisher.Application;

namespace Vmf.Publisher.UnitTests;

public sealed class ReleaseNoteVerificationEvidenceExtractorTests
{
    [Fact]
    public void Extract_NormalizesRowsFromAllowListedCurrentStateTable()
    {
        var result = Extract(
            AllowList("docs/development/current.md"),
            Current(
                "docs/development/current.md",
                """
                # Verification

                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet test tests/unit/Publisher/Vmf.Publisher.UnitTests.csproj | PASS | 0 | 0 | 553 | 0 | 0 |
                """));

        Assert.False(result.HasBlockingConflict);
        Assert.Empty(result.Diagnostics);
        var row = Assert.Single(result.Rows);
        Assert.Equal("dotnet test tests/unit/Publisher/Vmf.Publisher.UnitTests.csproj", row.Command);
        Assert.Equal("PASS", row.Result);
        Assert.Equal("0", row.Warnings);
        Assert.Equal("553", row.Passed);
        Assert.Equal("docs/development/current.md", row.SourcePath);
    }

    [Fact]
    public void Extract_IgnoresNonAllowListedSource()
    {
        var result = Extract(
            AllowList("docs/development/current.md"),
            Current(
                "docs/development/unlisted.md",
                """
                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet build VMF.Publisher.sln | PASS | 0 | 0 | 0 | 0 | 0 |
                """));

        Assert.Empty(result.Rows);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteVerificationEvidenceDiagnosticKind.SourceNotAllowListed);
    }

    [Fact]
    public void Extract_DoesNotPromoteHistoricalVerificationEvidence()
    {
        var result = Extract(
            AllowList("docs/development/historical.md"),
            Historical(
                "docs/development/historical.md",
                """
                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet test VMF.Publisher.sln | PASS | 0 | 0 | 553 | 0 | 0 |
                """));

        Assert.Empty(result.Rows);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteVerificationEvidenceDiagnosticKind.SourceKindNotPermitted);
    }

    [Fact]
    public void Extract_ConflictingCommandResultsBlockApprovalReadyOutput()
    {
        var result = Extract(
            AllowList("docs/development/current-a.md", "docs/development/current-b.md"),
            Current(
                "docs/development/current-a.md",
                """
                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet test VMF.Publisher.sln | PASS | 0 | 0 | 553 | 0 | 0 |
                """),
            Current(
                "docs/development/current-b.md",
                """
                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet test VMF.Publisher.sln | FAIL | 0 | 1 | 552 | 1 | 0 |
                """));

        Assert.True(result.HasBlockingConflict);
        Assert.Equal(2, result.Rows.Count);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteVerificationEvidenceDiagnosticKind.Conflict);
    }

    [Fact]
    public void Extract_ConflictingCommandCountsBlockApprovalReadyOutput()
    {
        var result = Extract(
            AllowList("docs/development/current-a.md", "docs/development/current-b.md"),
            Current(
                "docs/development/current-a.md",
                """
                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet test VMF.Publisher.sln | PASS | 0 | 0 | 553 | 0 | 0 |
                """),
            Current(
                "docs/development/current-b.md",
                """
                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet test VMF.Publisher.sln | PASS | 0 | 0 | 552 | 0 | 0 |
                """));

        Assert.True(result.HasBlockingConflict);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteVerificationEvidenceDiagnosticKind.Conflict);
    }

    [Fact]
    public void Extract_SensitiveValuesAreExcluded()
    {
        var result = Extract(
            AllowList("docs/development/current.md"),
            Current(
                "docs/development/current.md",
                """
                | Command | Result | Warnings | Errors | Passed | Failed | Skipped |
                | --- | --- | --- | --- | --- | --- | --- |
                | dotnet test C:\Users\biz\secret-token\tests.csproj | PASS | 0 | 0 | 1 | 0 | 0 |
                """));

        Assert.Empty(result.Rows);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == ReleaseNoteVerificationEvidenceDiagnosticKind.SensitiveValueExcluded);
    }

    private static ReleaseNoteVerificationEvidenceResult Extract(
        IReadOnlySet<string> allowList,
        params ReleaseNoteVerificationEvidenceSourceRecord[] records) =>
        new ReleaseNoteVerificationEvidenceExtractor().Extract(new ReleaseNoteVerificationEvidenceRequest(records, allowList));

    private static HashSet<string> AllowList(params string[] paths) => new(paths, StringComparer.Ordinal);

    private static ReleaseNoteVerificationEvidenceSourceRecord Current(string path, string markdown) =>
        new(path, ReleaseNoteSourceRecordKind.CurrentState, markdown);

    private static ReleaseNoteVerificationEvidenceSourceRecord Historical(string path, string markdown) =>
        new(path, ReleaseNoteSourceRecordKind.Historical, markdown);
}
