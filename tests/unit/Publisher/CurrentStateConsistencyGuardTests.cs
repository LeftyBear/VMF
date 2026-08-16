using Vmf.Publisher.Application;

namespace Vmf.Publisher.UnitTests;

public sealed class CurrentStateConsistencyGuardTests
{
    [Fact]
    public void Check_CurrentBlockedBoundaryReportsMatches()
    {
        var result = Check(
            Manifest(
                Claim("releaseState", "blocked", "blocked", "cleared"),
                Claim("avastState", "pending", "pending", "resolved"),
                Claim("vendorClearance", "notObtained", "notObtained", "obtained")),
            Allow("docs/development/CURRENT_STATUS.md"),
            Current(
                "docs/development/CURRENT_STATUS.md",
                Value("releaseState", "blocked"),
                Value("avastState", "pending"),
                Value("vendorClearance", "notObtained")));

        Assert.False(result.HasBlockingConflict);
        Assert.All(result.Claims, claim => Assert.Equal(CurrentStateConsistencyStatus.Match, claim.Status));
        Assert.Empty(result.Diagnostics);
    }

    [Theory]
    [InlineData("releaseState", "cleared")]
    [InlineData("vendorClearance", "obtained")]
    [InlineData("liveE2E", "complete")]
    public void Check_CurrentStateContradictionReportsConflict(string name, string value)
    {
        var result = Check(
            Manifest(
                Claim("releaseState", "blocked", "blocked", "cleared"),
                Claim("vendorClearance", "notObtained", "notObtained", "obtained"),
                Claim("liveE2E", "notComplete", "notComplete", "complete")),
            Allow("docs/development/CURRENT_STATUS.md"),
            Current("docs/development/CURRENT_STATUS.md", Value(name, value)));

        Assert.True(result.HasBlockingConflict);
        var claim = Assert.Single(result.Claims);
        Assert.Equal(CurrentStateConsistencyStatus.Conflict, claim.Status);
        Assert.Equal(value, claim.ActualValue);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == CurrentStateConsistencyDiagnosticKind.Conflict);
    }

    [Fact]
    public void Check_HistoricalCompletionClaimDoesNotPromoteCurrentClearance()
    {
        var result = Check(
            Manifest(Claim("releaseState", "blocked", "blocked", "cleared")),
            Allow("docs/development/CURRENT_STATUS.md", "docs/releases/historical.md"),
            Historical("docs/releases/historical.md", Value("releaseState", "cleared")));

        Assert.False(result.Claims.Any());
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Kind == CurrentStateConsistencyDiagnosticKind.SourceKindNotPermitted);
    }

    [Fact]
    public void Check_NonAllowListedClaimIsDiagnosticOnlyAndValueSafe()
    {
        var result = Check(
            Manifest(Claim("releaseState", "blocked", "blocked", "cleared")),
            Allow("docs/development/CURRENT_STATUS.md"),
            Current("docs/development/CURRENT_STATUS.md", Value("rawReleaseEvidence", "Authorization: Bearer token")));

        Assert.True(result.HasBlockingConflict);
        Assert.Empty(result.Claims);
        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal(CurrentStateConsistencyDiagnosticKind.ClaimNotAllowListed, diagnostic.Kind);
        Assert.Equal("Claim is not allow-listed.", diagnostic.Message);
        Assert.DoesNotContain("Bearer", diagnostic.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static CurrentStateConsistencyResult Check(
        IReadOnlyDictionary<string, CurrentStateConsistencyExpectedClaim> manifest,
        IReadOnlySet<string> allowList,
        params CurrentStateConsistencySource[] sources) =>
        new CurrentStateConsistencyGuard().Check(new CurrentStateConsistencyRequest(sources, manifest, allowList));

    private static Dictionary<string, CurrentStateConsistencyExpectedClaim> Manifest(params KeyValuePair<string, CurrentStateConsistencyExpectedClaim>[] claims) =>
        claims.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);

    private static KeyValuePair<string, CurrentStateConsistencyExpectedClaim> Claim(string name, string currentValue, params string[] allowedValues) =>
        new(name, new CurrentStateConsistencyExpectedClaim(currentValue, new HashSet<string>(allowedValues, StringComparer.Ordinal)));

    private static HashSet<string> Allow(params string[] paths) => new(paths, StringComparer.Ordinal);

    private static CurrentStateConsistencySource Current(string path, params CurrentStateConsistencyClaim[] claims) =>
        new(path, ReleaseNoteSourceRecordKind.CurrentState, claims);

    private static CurrentStateConsistencySource Historical(string path, params CurrentStateConsistencyClaim[] claims) =>
        new(path, ReleaseNoteSourceRecordKind.Historical, claims);

    private static CurrentStateConsistencyClaim Value(string name, string value) => new(name, value);
}
