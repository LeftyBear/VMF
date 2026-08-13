using System.Text;
using System.Text.RegularExpressions;

namespace Vmf.Publisher.Application;

/// <summary>Builds draft-only Publisher release-note content from allow-listed source records.</summary>
public sealed class ReleaseNoteDraftAssembler
{
    private const string NotRecorded = "NOT RECORDED";
    private const string Conflict = "CONFLICT";

    private static readonly Regex PrivateUrlPattern = new(
        @"https?://(?:localhost|127\.0\.0\.1|10\.\d{1,3}\.\d{1,3}\.\d{1,3}|192\.168\.\d{1,3}\.\d{1,3}|172\.(?:1[6-9]|2\d|3[0-1])\.\d{1,3}\.\d{1,3}|[^\s/]+\.local)(?:[^\s)]*)?",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Regex TokenLikePattern = new(
        @"(?i)\b(?:authorization|bearer|token|credential|secret|password|client_secret)\b\s*[:=]\s*\S+");

    private static readonly Regex WindowsPathPattern = new(
        @"(?i)\b[A-Z]:\\[^\s|)]+");

    /// <summary>Assembles deterministic draft Markdown from the request.</summary>
    /// <param name="request">The draft assembly request.</param>
    /// <returns>The assembled draft result.</returns>
    public ReleaseNoteDraftResult Assemble(ReleaseNoteDraftRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var diagnostics = new List<ReleaseNoteDraftDiagnostic>();
        var fields = new List<ReleaseNoteDraftField>();

        foreach (var fieldName in request.FieldNames)
        {
            var manifestField = request.Manifest.Find(fieldName);
            if (manifestField is null)
            {
                fields.Add(new ReleaseNoteDraftField(
                    fieldName,
                    NotRecorded,
                    [],
                    ReleaseNoteDraftFieldStatus.NotRecorded));
                diagnostics.Add(ReleaseNoteDraftDiagnostic.UnmanifestedField(fieldName));
                continue;
            }

            if (manifestField.ManualOnly)
            {
                fields.Add(new ReleaseNoteDraftField(
                    manifestField.CanonicalFieldName,
                    "MANUAL ONLY",
                    [],
                    ReleaseNoteDraftFieldStatus.ManualOnly));
                continue;
            }

            var currentValues = request.SourceRecords
                .Where(record => manifestField.IsPermitted(record.Kind))
                .SelectMany(record => record.Fields
                    .Where(field => StringComparer.OrdinalIgnoreCase.Equals(field.Name, manifestField.CanonicalFieldName))
                    .Select(field => new FieldValue(record, field.Value)))
                .ToList();

            foreach (var rejectedSource in request.SourceRecords
                         .Where(record => !manifestField.IsPermitted(record.Kind))
                         .Where(record => record.Fields.Any(field =>
                             StringComparer.OrdinalIgnoreCase.Equals(field.Name, manifestField.CanonicalFieldName))))
            {
                diagnostics.Add(ReleaseNoteDraftDiagnostic.SourceKindNotPermitted(
                    manifestField.CanonicalFieldName,
                    rejectedSource.Kind,
                    rejectedSource.Path));
            }

            if (currentValues.Count == 0)
            {
                fields.Add(new ReleaseNoteDraftField(
                    manifestField.CanonicalFieldName,
                    NotRecorded,
                    [],
                    ReleaseNoteDraftFieldStatus.NotRecorded));
                diagnostics.Add(ReleaseNoteDraftDiagnostic.Missing(manifestField.CanonicalFieldName, manifestField.MissingBehavior));
                continue;
            }

            var safeValues = new List<FieldValue>();
            foreach (var currentValue in currentValues)
            {
                if (ContainsSensitiveValue(currentValue.Value))
                {
                    diagnostics.Add(ReleaseNoteDraftDiagnostic.SensitiveValueExcluded(manifestField.CanonicalFieldName, currentValue.Record.Path));
                    continue;
                }

                safeValues.Add(currentValue);
            }

            var distinctValues = safeValues
                .Select(value => value.Value)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (distinctValues.Count == 0)
            {
                fields.Add(new ReleaseNoteDraftField(
                    manifestField.CanonicalFieldName,
                    NotRecorded,
                    [],
                    ReleaseNoteDraftFieldStatus.NotRecorded));
                diagnostics.Add(ReleaseNoteDraftDiagnostic.Missing(manifestField.CanonicalFieldName, manifestField.MissingBehavior));
                continue;
            }

            if (distinctValues.Count > 1)
            {
                fields.Add(new ReleaseNoteDraftField(
                    manifestField.CanonicalFieldName,
                    Conflict,
                    safeValues.Select(value => value.Record.Path).Distinct(StringComparer.Ordinal).ToList(),
                    ReleaseNoteDraftFieldStatus.Conflict));
                diagnostics.Add(ReleaseNoteDraftDiagnostic.Conflict(manifestField.CanonicalFieldName, manifestField.ConflictBehavior));
                continue;
            }

            fields.Add(new ReleaseNoteDraftField(
                manifestField.CanonicalFieldName,
                distinctValues[0],
                safeValues.Select(value => value.Record.Path).Distinct(StringComparer.Ordinal).ToList(),
                ReleaseNoteDraftFieldStatus.Recorded));
        }

        foreach (var historicalField in request.SourceRecords
                     .Where(record => record.Kind == ReleaseNoteSourceRecordKind.Historical)
                     .SelectMany(record => record.Fields)
                     .Where(field => request.Manifest.Find(field.Name) is not null))
        {
            if (!fields.Any(field =>
                    StringComparer.OrdinalIgnoreCase.Equals(field.Name, historicalField.Name)
                    && field.Status == ReleaseNoteDraftFieldStatus.Recorded))
            {
                diagnostics.Add(ReleaseNoteDraftDiagnostic.HistoricalNotPromoted(historicalField.Name));
            }
        }

        var markdown = BuildMarkdown(request.Title, fields, request.SourceRecords, diagnostics);

        return new ReleaseNoteDraftResult(
            markdown,
            fields,
            diagnostics,
            diagnostics.Any(diagnostic => diagnostic.Kind == ReleaseNoteDraftDiagnosticKind.Conflict));
    }

    private static string BuildMarkdown(
        string title,
        IReadOnlyList<ReleaseNoteDraftField> fields,
        IReadOnlyList<ReleaseNoteSourceRecord> sourceRecords,
        IReadOnlyList<ReleaseNoteDraftDiagnostic> diagnostics)
    {
        var builder = new StringBuilder();

        builder.AppendLine("# " + Sanitize(title));
        builder.AppendLine();
        builder.AppendLine("> DRAFT ONLY: This generated document is derived documentation. It is not release approval, release authorization, publication authorization, risk acceptance, vendor clearance, Avast safety certification, publication, or proof that any gated operation occurred.");
        builder.AppendLine();
        builder.AppendLine("## Fields");
        builder.AppendLine();
        builder.AppendLine("| Field | Value | Source |");
        builder.AppendLine("| --- | --- | --- |");

        foreach (var field in fields)
        {
            var source = field.SourcePaths.Count == 0 ? NotRecorded : string.Join("<br>", field.SourcePaths.Select(Sanitize));
            builder.AppendLine($"| {EscapeCell(field.Name)} | {EscapeCell(field.Value)} | {EscapeCell(source)} |");
        }

        builder.AppendLine();
        builder.AppendLine("## Source Records");
        builder.AppendLine();

        foreach (var record in sourceRecords)
        {
            builder.AppendLine("- " + Sanitize(record.Path) + " (" + record.Kind + ")");
        }

        builder.AppendLine();
        builder.AppendLine("## Diagnostics");
        builder.AppendLine();

        if (diagnostics.Count == 0)
        {
            builder.AppendLine("- none");
        }
        else
        {
            foreach (var diagnostic in diagnostics)
            {
                builder.AppendLine("- " + diagnostic.Kind + ": " + Sanitize(diagnostic.Message));
            }
        }

        return builder.ToString();
    }

    private static bool ContainsSensitiveValue(string value) =>
        PrivateUrlPattern.IsMatch(value)
        || TokenLikePattern.IsMatch(value)
        || WindowsPathPattern.IsMatch(value);

    private static string EscapeCell(string value) => Sanitize(value).Replace("|", "\\|", StringComparison.Ordinal);

    private static string Sanitize(string value)
    {
        var sanitized = PrivateUrlPattern.Replace(value, "[REDACTED]");
        sanitized = TokenLikePattern.Replace(sanitized, "[REDACTED]");
        return WindowsPathPattern.Replace(sanitized, "[REDACTED]");
    }

    private sealed record FieldValue(ReleaseNoteSourceRecord Record, string Value);
}

/// <summary>Input for draft release-note assembly.</summary>
public sealed record ReleaseNoteDraftRequest(
    string Title,
    IReadOnlyList<string> FieldNames,
    IReadOnlyList<ReleaseNoteSourceRecord> SourceRecords)
{
    /// <summary>The explicit allow-list of field/source combinations available to the assembler.</summary>
    public ReleaseNoteSourceFieldManifest Manifest { get; init; } = ReleaseNoteSourceFieldManifest.Empty;
}

/// <summary>An explicit allow-list of release-note draft source fields.</summary>
public sealed record ReleaseNoteSourceFieldManifest(IReadOnlyList<ReleaseNoteSourceFieldManifestEntry> Fields)
{
    /// <summary>An empty manifest that fails closed for all fields.</summary>
    public static ReleaseNoteSourceFieldManifest Empty { get; } = new([]);

    /// <summary>Finds a manifest entry by canonical field name.</summary>
    public ReleaseNoteSourceFieldManifestEntry? Find(string fieldName) =>
        Fields.SingleOrDefault(field => StringComparer.OrdinalIgnoreCase.Equals(field.CanonicalFieldName, fieldName));
}

/// <summary>Defines one release-note draft field and its permitted source boundary.</summary>
public sealed record ReleaseNoteSourceFieldManifestEntry(
    string CanonicalFieldName,
    IReadOnlySet<ReleaseNoteSourceRecordKind> PermittedSourceKinds,
    ReleaseNoteSourceBoundary SourceBoundary,
    ReleaseNoteMissingBehavior MissingBehavior,
    ReleaseNoteConflictBehavior ConflictBehavior,
    bool ManualOnly)
{
    /// <summary>Returns whether the source kind is permitted for this field.</summary>
    public bool IsPermitted(ReleaseNoteSourceRecordKind sourceKind) =>
        PermittedSourceKinds.Contains(sourceKind)
        && SourceBoundary switch
        {
            ReleaseNoteSourceBoundary.CurrentState => sourceKind == ReleaseNoteSourceRecordKind.CurrentState,
            ReleaseNoteSourceBoundary.HistoricalOnly => sourceKind == ReleaseNoteSourceRecordKind.Historical,
            ReleaseNoteSourceBoundary.TemplateOnly => sourceKind == ReleaseNoteSourceRecordKind.Template,
            _ => false,
        };
}

/// <summary>The current-state or non-current boundary assigned to a manifest field.</summary>
public enum ReleaseNoteSourceBoundary
{
    /// <summary>The field may be copied only from an explicitly permitted current-state source.</summary>
    CurrentState,

    /// <summary>The field is historical evidence only and cannot become current state.</summary>
    HistoricalOnly,

    /// <summary>The field is template or draft structure only and cannot become current state.</summary>
    TemplateOnly,
}

/// <summary>The missing-value behavior for a manifest field.</summary>
public enum ReleaseNoteMissingBehavior
{
    /// <summary>Missing values are emitted as NOT RECORDED.</summary>
    NotRecorded,
}

/// <summary>The conflict behavior for a manifest field.</summary>
public enum ReleaseNoteConflictBehavior
{
    /// <summary>Conflicts are emitted as CONFLICT and block approval-ready output.</summary>
    BlockingConflict,
}

/// <summary>An allow-listed source record supplied to the draft assembler.</summary>
public sealed record ReleaseNoteSourceRecord(
    string Path,
    ReleaseNoteSourceRecordKind Kind,
    IReadOnlyList<ReleaseNoteSourceField> Fields);

/// <summary>A named value explicitly copied from an allow-listed source record.</summary>
public sealed record ReleaseNoteSourceField(string Name, string Value);

/// <summary>The source record role used by the draft assembler.</summary>
public enum ReleaseNoteSourceRecordKind
{
    /// <summary>The record explicitly represents current state.</summary>
    CurrentState,

    /// <summary>The record is historical evidence and cannot supply current-state values.</summary>
    Historical,

    /// <summary>The record is a template source and cannot supply current-state values.</summary>
    Template,
}

/// <summary>The assembled draft output and diagnostics.</summary>
public sealed record ReleaseNoteDraftResult(
    string Markdown,
    IReadOnlyList<ReleaseNoteDraftField> Fields,
    IReadOnlyList<ReleaseNoteDraftDiagnostic> Diagnostics,
    bool HasBlockingConflict);

/// <summary>A generated draft field value.</summary>
public sealed record ReleaseNoteDraftField(
    string Name,
    string Value,
    IReadOnlyList<string> SourcePaths,
    ReleaseNoteDraftFieldStatus Status);

/// <summary>The status assigned to a draft field.</summary>
public enum ReleaseNoteDraftFieldStatus
{
    /// <summary>The field was copied from one or more current-state records.</summary>
    Recorded,

    /// <summary>The field was not present in current-state records.</summary>
    NotRecorded,

    /// <summary>The field had conflicting current-state values.</summary>
    Conflict,

    /// <summary>The field is manual-only and non-inferable.</summary>
    ManualOnly,
}

/// <summary>A draft assembly diagnostic.</summary>
public sealed record ReleaseNoteDraftDiagnostic(ReleaseNoteDraftDiagnosticKind Kind, string FieldName, string Message)
{
    /// <summary>Creates a missing-field diagnostic.</summary>
    public static ReleaseNoteDraftDiagnostic Missing(string fieldName, ReleaseNoteMissingBehavior missingBehavior) =>
        new(ReleaseNoteDraftDiagnosticKind.Missing, fieldName, fieldName + " was not recorded in a permitted source; behavior=" + missingBehavior + ".");

    /// <summary>Creates a conflict diagnostic.</summary>
    public static ReleaseNoteDraftDiagnostic Conflict(string fieldName, ReleaseNoteConflictBehavior conflictBehavior) =>
        new(ReleaseNoteDraftDiagnosticKind.Conflict, fieldName, fieldName + " has conflicting permitted source values; behavior=" + conflictBehavior + ".");

    /// <summary>Creates a historical-not-promoted diagnostic.</summary>
    public static ReleaseNoteDraftDiagnostic HistoricalNotPromoted(string fieldName) =>
        new(ReleaseNoteDraftDiagnosticKind.HistoricalNotPromoted, fieldName, fieldName + " appeared only in historical records and was not promoted.");

    /// <summary>Creates a sensitive-value exclusion diagnostic.</summary>
    public static ReleaseNoteDraftDiagnostic SensitiveValueExcluded(string fieldName, string sourcePath) =>
        new(ReleaseNoteDraftDiagnosticKind.SensitiveValueExcluded, fieldName, fieldName + " from " + sourcePath + " was excluded as sensitive.");

    /// <summary>Creates an unmanifested-field diagnostic.</summary>
    public static ReleaseNoteDraftDiagnostic UnmanifestedField(string fieldName) =>
        new(ReleaseNoteDraftDiagnosticKind.UnmanifestedField, fieldName, fieldName + " is not present in the source field manifest.");

    /// <summary>Creates a source-kind rejection diagnostic.</summary>
    public static ReleaseNoteDraftDiagnostic SourceKindNotPermitted(
        string fieldName,
        ReleaseNoteSourceRecordKind sourceKind,
        string sourcePath) =>
        new(
            ReleaseNoteDraftDiagnosticKind.SourceKindNotPermitted,
            fieldName,
            fieldName + " from " + sourcePath + " was ignored because source kind " + sourceKind + " is not permitted.");
}

/// <summary>The kind of draft assembly diagnostic.</summary>
public enum ReleaseNoteDraftDiagnosticKind
{
    /// <summary>A requested field was absent from current-state sources.</summary>
    Missing,

    /// <summary>A requested field had inconsistent current-state values.</summary>
    Conflict,

    /// <summary>A historical field was intentionally not used as current state.</summary>
    HistoricalNotPromoted,

    /// <summary>A value was excluded because it looked sensitive.</summary>
    SensitiveValueExcluded,

    /// <summary>A requested field was not defined in the manifest.</summary>
    UnmanifestedField,

    /// <summary>A source field was ignored because the source kind is not permitted by the manifest.</summary>
    SourceKindNotPermitted,
}

/// <summary>Compares approved release-note fields with allow-listed source records without rewriting files.</summary>
internal sealed class ReleaseNoteDriftChecker
{
    /// <summary>Checks release-note field drift against the existing draft assembler boundary.</summary>
    /// <param name="request">The drift-check request.</param>
    /// <returns>The bounded drift-check result.</returns>
    public ReleaseNoteDriftCheckResult Check(ReleaseNoteDriftCheckRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var expected = new ReleaseNoteDraftAssembler().Assemble(new ReleaseNoteDraftRequest(
            "Publisher Release Note Drift Check",
            request.FieldNames,
            request.SourceRecords)
        {
            Manifest = request.Manifest,
        });

        var releaseNoteFields = request.ReleaseNoteFields
            .GroupBy(field => field.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Select(field => field.Value).ToList(),
                StringComparer.OrdinalIgnoreCase);

        var fields = new List<ReleaseNoteDriftField>();
        foreach (var expectedField in expected.Fields)
        {
            if (expectedField.Status is ReleaseNoteDraftFieldStatus.Conflict)
            {
                fields.Add(new ReleaseNoteDriftField(
                    expectedField.Name,
                    ReleaseNoteDriftStatus.Conflict,
                    expectedField.Value,
                    "CONFLICT",
                    expectedField.SourcePaths,
                    "Allow-listed source records conflict."));
                continue;
            }

            if (expectedField.Status is ReleaseNoteDraftFieldStatus.NotRecorded or ReleaseNoteDraftFieldStatus.ManualOnly)
            {
                fields.Add(new ReleaseNoteDriftField(
                    expectedField.Name,
                    ReleaseNoteDriftStatus.Missing,
                    expectedField.Value,
                    "MISSING",
                    expectedField.SourcePaths,
                    "Expected value is not approval-ready from allow-listed sources."));
                continue;
            }

            if (!releaseNoteFields.TryGetValue(expectedField.Name, out var actualValues) || actualValues.Count == 0)
            {
                fields.Add(new ReleaseNoteDriftField(
                    expectedField.Name,
                    ReleaseNoteDriftStatus.Missing,
                    expectedField.Value,
                    "MISSING",
                    expectedField.SourcePaths,
                    "Release note field is missing."));
                continue;
            }

            if (actualValues.Count > 1 || !StringComparer.Ordinal.Equals(actualValues[0], expectedField.Value))
            {
                fields.Add(new ReleaseNoteDriftField(
                    expectedField.Name,
                    ReleaseNoteDriftStatus.Conflict,
                    expectedField.Value,
                    string.Join(" | ", actualValues),
                    expectedField.SourcePaths,
                    "Release note field does not match the allow-listed source value."));
                continue;
            }

            fields.Add(new ReleaseNoteDriftField(
                expectedField.Name,
                ReleaseNoteDriftStatus.Match,
                expectedField.Value,
                actualValues[0],
                expectedField.SourcePaths,
                "Release note field matches the allow-listed source value."));
        }

        return new ReleaseNoteDriftCheckResult(
            fields,
            expected.Diagnostics,
            fields.Any(field => field.Status is ReleaseNoteDriftStatus.Missing or ReleaseNoteDriftStatus.Conflict)
                || expected.HasBlockingConflict);
    }
}

/// <summary>Input for local-only release-note drift checking.</summary>
internal sealed record ReleaseNoteDriftCheckRequest(
    IReadOnlyList<string> FieldNames,
    IReadOnlyList<ReleaseNoteSourceField> ReleaseNoteFields,
    IReadOnlyList<ReleaseNoteSourceRecord> SourceRecords)
{
    /// <summary>The explicit allow-list of field/source combinations available to the drift checker.</summary>
    public ReleaseNoteSourceFieldManifest Manifest { get; init; } = ReleaseNoteSourceFieldManifest.Empty;
}

/// <summary>The bounded status assigned to one release-note field comparison.</summary>
internal enum ReleaseNoteDriftStatus
{
    /// <summary>The release note matches the allow-listed source value.</summary>
    Match,

    /// <summary>The release note or source value is missing or not approval-ready.</summary>
    Missing,

    /// <summary>The release note conflicts with the allow-listed source value or source records conflict.</summary>
    Conflict,
}

/// <summary>One field-level release-note drift result.</summary>
internal sealed record ReleaseNoteDriftField(
    string Name,
    ReleaseNoteDriftStatus Status,
    string ExpectedValue,
    string ActualValue,
    IReadOnlyList<string> SourcePaths,
    string Message);

/// <summary>The complete release-note drift-check result.</summary>
internal sealed record ReleaseNoteDriftCheckResult(
    IReadOnlyList<ReleaseNoteDriftField> Fields,
    IReadOnlyList<ReleaseNoteDraftDiagnostic> Diagnostics,
    bool HasBlockingDrift);
