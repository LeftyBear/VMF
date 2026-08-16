namespace Vmf.Publisher.Application;

/// <summary>Checks current-state claims against an explicit allow-listed boundary.</summary>
internal sealed class CurrentStateConsistencyGuard
{
    /// <summary>Compares checked-in documentation claims without inferring release or vendor clearance.</summary>
    public CurrentStateConsistencyResult Check(CurrentStateConsistencyRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var diagnostics = new List<CurrentStateConsistencyDiagnostic>();
        var claims = new List<CurrentStateConsistencyClaimResult>();

        foreach (var source in request.Sources)
        {
            if (!request.AllowListedSourcePaths.Contains(source.Path))
            {
                diagnostics.Add(CurrentStateConsistencyDiagnostic.SourceNotAllowListed(source.Path));
                continue;
            }

            if (source.Kind != ReleaseNoteSourceRecordKind.CurrentState)
            {
                diagnostics.Add(CurrentStateConsistencyDiagnostic.SourceKindNotPermitted(source.Path, source.Kind));
                continue;
            }

            foreach (var claim in source.Claims)
            {
                if (!request.Manifest.TryGetValue(claim.Name, out var expected))
                {
                    diagnostics.Add(CurrentStateConsistencyDiagnostic.ClaimNotAllowListed(claim.Name, source.Path));
                    continue;
                }

                if (!expected.AllowedValues.Contains(claim.Value))
                {
                    claims.Add(new CurrentStateConsistencyClaimResult(
                        claim.Name,
                        CurrentStateConsistencyStatus.Conflict,
                        expected.CurrentValue,
                        claim.Value,
                        source.Path));
                    diagnostics.Add(CurrentStateConsistencyDiagnostic.Conflict(claim.Name, source.Path));
                    continue;
                }

                var status = StringComparer.Ordinal.Equals(claim.Value, expected.CurrentValue)
                    ? CurrentStateConsistencyStatus.Match
                    : CurrentStateConsistencyStatus.Conflict;

                claims.Add(new CurrentStateConsistencyClaimResult(
                    claim.Name,
                    status,
                    expected.CurrentValue,
                    claim.Value,
                    source.Path));

                if (status == CurrentStateConsistencyStatus.Conflict)
                {
                    diagnostics.Add(CurrentStateConsistencyDiagnostic.Conflict(claim.Name, source.Path));
                }
            }
        }

        return new CurrentStateConsistencyResult(
            claims,
            diagnostics,
            claims.Any(claim => claim.Status == CurrentStateConsistencyStatus.Conflict)
                || diagnostics.Any(diagnostic => diagnostic.Kind != CurrentStateConsistencyDiagnosticKind.SourceKindNotPermitted));
    }
}

/// <summary>Input for local-only current-state consistency checking.</summary>
internal sealed record CurrentStateConsistencyRequest(
    IReadOnlyList<CurrentStateConsistencySource> Sources,
    IReadOnlyDictionary<string, CurrentStateConsistencyExpectedClaim> Manifest,
    IReadOnlySet<string> AllowListedSourcePaths);

/// <summary>One checked-in documentation source available to the guard.</summary>
internal sealed record CurrentStateConsistencySource(
    string Path,
    ReleaseNoteSourceRecordKind Kind,
    IReadOnlyList<CurrentStateConsistencyClaim> Claims);

/// <summary>One extracted documentation claim supplied by an allow-listed caller.</summary>
internal sealed record CurrentStateConsistencyClaim(string Name, string Value);

/// <summary>The current-state value and closed vocabulary for a claim.</summary>
internal sealed record CurrentStateConsistencyExpectedClaim(
    string CurrentValue,
    IReadOnlySet<string> AllowedValues);

/// <summary>One bounded claim comparison result.</summary>
internal sealed record CurrentStateConsistencyClaimResult(
    string Name,
    CurrentStateConsistencyStatus Status,
    string ExpectedValue,
    string ActualValue,
    string SourcePath);

/// <summary>Closed claim comparison vocabulary.</summary>
internal enum CurrentStateConsistencyStatus
{
    Match,
    Conflict,
}

/// <summary>The complete current-state consistency result.</summary>
internal sealed record CurrentStateConsistencyResult(
    IReadOnlyList<CurrentStateConsistencyClaimResult> Claims,
    IReadOnlyList<CurrentStateConsistencyDiagnostic> Diagnostics,
    bool HasBlockingConflict);

/// <summary>A bounded current-state consistency diagnostic.</summary>
internal sealed record CurrentStateConsistencyDiagnostic(
    CurrentStateConsistencyDiagnosticKind Kind,
    string Name,
    string SourcePath,
    string Message)
{
    public static CurrentStateConsistencyDiagnostic SourceNotAllowListed(string sourcePath) =>
        new(CurrentStateConsistencyDiagnosticKind.SourceNotAllowListed, "source", sourcePath, "Source record is not allow-listed.");

    public static CurrentStateConsistencyDiagnostic SourceKindNotPermitted(string sourcePath, ReleaseNoteSourceRecordKind sourceKind) =>
        new(CurrentStateConsistencyDiagnosticKind.SourceKindNotPermitted, "source", sourcePath, "Source kind " + sourceKind + " is not permitted.");

    public static CurrentStateConsistencyDiagnostic ClaimNotAllowListed(string name, string sourcePath) =>
        new(CurrentStateConsistencyDiagnosticKind.ClaimNotAllowListed, name, sourcePath, "Claim is not allow-listed.");

    public static CurrentStateConsistencyDiagnostic Conflict(string name, string sourcePath) =>
        new(CurrentStateConsistencyDiagnosticKind.Conflict, name, sourcePath, "Claim conflicts with current state.");
}

/// <summary>Closed diagnostic vocabulary for current-state consistency checking.</summary>
internal enum CurrentStateConsistencyDiagnosticKind
{
    SourceNotAllowListed,
    SourceKindNotPermitted,
    ClaimNotAllowListed,
    Conflict,
}
