# P9-85 - Parser Verification Evidence-Generation Correction Input Definition

## Status

COMPLETE / docs-only evidence-generation correction input definition

## Purpose

Define exactly one complete corrected parser-verification evidence-generation
input for later review, while preserving the P9-79 fixed lifecycle-script
identity and parser-only semantics. The correction closes the evidence contract
at the caller boundary: a child-process exit code is never authoritative by
itself, and success is available only after the caller has retained and
validated exactly one structured JSON result.

P9-85 is documentation only. It does not materialize or inspect either fixed
file, invoke a parser, run PowerShell, execute the lifecycle, start Excel,
access or mutate a workbook, fixture, or process, restore an Avast quarantine
item, add an Avast exception, exclusion, or allow-list entry, change Avast
settings, rerun the detected target, apply the correction at runtime, change
implementation, tests, or tools, update package or `dist` artifacts, perform
release or publication work, access external services, stage, commit, push, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Preserved Fixed Input

The parser target remains the P9-79 file without byte or semantic change:

| Property | Fixed value |
|---|---|
| Path | `C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1` |
| Observed length | `8264` bytes |
| Observed SHA-256 | `80749CA24C4CF7A8BF27CF8D55633B526FF64FFBF6CE8216590C609060B1F353` |
| Encoding | UTF-8 with BOM |
| Newlines | CRLF only, with exactly one final CRLF |
| Permitted operation | byte validation and `Parser.ParseFile` only |

The corrected input must not write, normalize, replace, dot-source, import,
invoke, or execute this target. The recorded identity values are inputs to a
later verification, not observations newly made by P9-85.

## Corrected Evidence-Generation Input

### Caller and process boundary

One later implementation may realize the following fixed caller contract. The
caller must use direct process creation with an executable path and ordered
argument vector supplied separately; no shell, command string, batch file, or
intermediate PowerShell process is permitted.

| Component | Fixed value |
|---|---|
| Caller identity | `P9-85 parser-evidence caller`, a future repository-controlled implementation of this contract; no implementation or executable is created by P9-85 |
| Caller working directory | `C:\Users\biz\Documents\Project\VMF` |
| Process-creation contract | direct child process; shell execution disabled; stdout and stderr redirected separately; no window |
| Executable | `C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe` |
| Argument 1 | `-NoLogo` |
| Argument 2 | `-NoProfile` |
| Argument 3 | `-NonInteractive` |
| Argument 4 | `-ExecutionPolicy` |
| Argument 5 | `Bypass` |
| Argument 6 | `-File` |
| Argument 7 | `C:\Users\biz\AppData\Local\Temp\VMF-P9-85-ParserEvidence.ps1` |
| Argument 8 | `-TargetPath` |
| Argument 9 | `C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1` |
| Child output encoding | UTF-8 without BOM for both redirected streams |
| Read sequence | begin draining both streams before waiting; wait for process exit; then wait for both streams to reach EOF |
| Completion limit | 30,000 milliseconds from successful process start through child exit and EOF on both streams; expiration is failure with no retry |
| Retention | retain exact stdout and stderr bytes, child exit code, start and completion timestamps, and child process ID |

The caller must inherit the existing environment without adding or changing
an encoding, host-presentation, execution-policy, or security-product value.

The caller must treat failure to start, interruption, timeout, stream-read
failure, missing EOF, or loss of any retained byte as failure. It must not
retry. The caller result is `0` only after all validation in the Caller
Acceptance section succeeds; every other caller result is nonzero.

### Parser-only verifier file

Argument 7 identifies one verifier file defined by the following exact source.
If a later task is separately authorized to materialize it, the file must use
UTF-8 with BOM, CRLF-only lines, and exactly one final CRLF. No part of this
source is a lifecycle-script correction.

```powershell
param(
    [Parameter(Mandatory = $true)]
    [string]$TargetPath
)

$ErrorActionPreference = 'Stop'
$expectedPath = 'C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1'
$expectedLength = 8264
$expectedHash = '80749CA24C4CF7A8BF27CF8D55633B526FF64FFBF6CE8216590C609060B1F353'

try {
    if ($TargetPath -cne $expectedPath) { throw 'Target path mismatch.' }
    $bytes = [System.IO.File]::ReadAllBytes($TargetPath)
    if ($bytes.Length -ne $expectedLength) { throw 'Target length mismatch.' }
    if ($bytes.Length -lt 3 -or $bytes[0] -ne 0xEF -or $bytes[1] -ne 0xBB -or $bytes[2] -ne 0xBF) { throw 'UTF-8 BOM mismatch.' }
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try { $hash = ([BitConverter]::ToString($sha.ComputeHash($bytes))).Replace('-', '') }
    finally { $sha.Dispose() }
    if ($hash -cne $expectedHash) { throw 'Target SHA-256 mismatch.' }
    $utf8 = New-Object System.Text.UTF8Encoding($true, $true)
    $text = $utf8.GetString($bytes, 3, $bytes.Length - 3)
    if ([regex]::IsMatch($text, '(?<!\r)\n|\r(?!\n)')) { throw 'Target newline mismatch.' }
    if (-not $text.EndsWith("`r`n") -or $text.EndsWith("`r`n`r`n")) { throw 'Target final CRLF mismatch.' }

    $tokens = $null
    $errors = $null
    [void][System.Management.Automation.Language.Parser]::ParseFile($TargetPath, [ref]$tokens, [ref]$errors)
    $parseErrors = @($errors | ForEach-Object {
        [ordered]@{
            ExtentText = $_.Extent.Text
            StartLineNumber = $_.Extent.StartLineNumber
            StartColumnNumber = $_.Extent.StartColumnNumber
            EndLineNumber = $_.Extent.EndLineNumber
            EndColumnNumber = $_.Extent.EndColumnNumber
            ErrorId = $_.ErrorId
            Message = $_.Message
        }
    })
    $result = [ordered]@{
        Schema = 'VMF.P9.ParserEvidence.v1'
        OutcomeId = [Guid]::NewGuid().ToString('D')
        FilePath = $TargetPath
        ByteLength = $bytes.Length
        SHA256 = $hash
        TokenCount = @($tokens).Count
        ParseErrorCount = $parseErrors.Count
        ParseErrors = $parseErrors
    }
    $json = $result | ConvertTo-Json -Compress -Depth 6
    $stdout = [Console]::OpenStandardOutput()
    $encoded = (New-Object System.Text.UTF8Encoding($false)).GetBytes($json + "`n")
    $stdout.Write($encoded, 0, $encoded.Length)
    $stdout.Flush()
    if ($parseErrors.Count -ne 0) { exit 21 }
    exit 0
}
catch {
    [Console]::Error.WriteLine(($_.Exception.GetType().FullName + ': ' + $_.Exception.Message))
    exit 20
}
```

The verifier has exactly three child exit outcomes: `0` after one JSON line is
written and flushed with zero parse errors, `21` after one JSON line is written
and flushed with one or more parse errors, and `20` for validation, parser,
serialization, or output-write failure. Abrupt termination or security
intervention may yield another observed exit value or no child exit value;
both are caller failures and must remain distinct from the defined outcomes.

## Structured Evidence Contract

Exactly one LF-terminated UTF-8 JSON object and no other stdout byte is
permitted. The required schema is `VMF.P9.ParserEvidence.v1`, with exactly
these fields: `Schema`, `OutcomeId`, `FilePath`, `ByteLength`, `SHA256`,
`TokenCount`, `ParseErrorCount`, and `ParseErrors`. Every `ParseErrors` item
must contain exactly `ExtentText`, `StartLineNumber`, `StartColumnNumber`,
`EndLineNumber`, `EndColumnNumber`, `ErrorId`, and `Message`.

The caller must decode stdout with strict UTF-8, reject a BOM, invalid bytes,
empty output, multiple lines or objects, leading or trailing non-JSON text,
duplicate properties, unknown properties, wrong types, negative counts,
invalid `OutcomeId`, path or identity mismatch, a count inconsistent with the
array, or a child exit code inconsistent with `ParseErrorCount`. Stderr is
retained as diagnostic evidence but is never parsed as the structured result.

## Caller Acceptance and Failure Closure

The caller may return `0` only when process creation completed, both redirected
streams reached EOF, the exact raw bytes were retained, the child returned
`0`, stderr is empty, exactly one valid result was decoded, its path, length,
and SHA-256 equal the fixed P9-79 identity, `ParseErrorCount` is `0`, and all
schema and consistency checks pass. Only then may parser PASS be considered.

Every other outcome returns a nonzero caller result and preserves the raw
streams and process observations. In particular, child exit `0` with absent,
invalid, truncated, or unretained JSON is failure. The caller must not repair,
reconstruct, normalize, or infer evidence and must not retry.

The fixed caller-result mapping is: `0` for accepted parser PASS; `31` for a
valid retained result with child exit `21` and one or more parse errors; `40`
for child exit `20`, another child exit value, or absent child exit; `41` for
process-start failure; `42` for timeout or interruption; `43` for stream-read,
EOF, or byte-retention failure; `44` for structured-evidence decoding, schema,
or consistency failure; and `45` for nonempty stderr when no earlier caller
failure applies. No other caller result is a success result.

The P9-83 Avast record remains separate correlated evidence. A later result may
record the caller and child timestamps, child process ID, and `OutcomeId`
alongside the confirmed Avast identifier
`be179406c22a/2026-09-04T14:56:48.897Z`, but temporal proximity alone does not
establish causation. Missing or additional Avast evidence remains unknown. No
security-control change or bypass is part of this input.

## Decision

Decision: `GO` for recording P9-85 as the docs-only parser-verification
evidence-generation correction input definition.

Decision: `PASS` for defining one complete caller, verifier, serialization,
stdout / stderr capture, exit propagation, validation, failure-closure, and
security-correlation contract while preserving the P9-79 fixed input.

Decision: `UNPROVEN` for the exact cause of the P9-81 missing structured JSON
evidence and for causation by the confirmed Avast event.

Decision: `INCOMPLETE / NO-GO` remains authoritative for P9-81. Its child
process exit code `0` is not parser PASS, and the P9-80 invocation remains
consumed.

Decision: `NO-GO` for materializing either file, implementing the caller,
invoking or retrying a parser, running PowerShell, executing the lifecycle,
Excel, workbook, fixture, or process operation, changing or bypassing Avast
controls, runtime correction, package / `dist`, release / publication,
external services, staging, commit, push, public API change, persisted schema
change, canonical format change, or Frozen specification change from P9-85.

## Selected Next Candidate

**P9-86 - Parser Verification Evidence-Generation Correction GO / NO-GO**

P9-86 should remain docs-only and review the complete P9-85 caller, verifier,
structured-evidence, output-capture, exit-propagation, fail-closed, and Avast
correlation boundaries before deciding whether one later separate
parser-verification execution may be authorized.

P9-86 must not materialize either file, implement the caller, invoke a parser,
run PowerShell, execute the lifecycle, start Excel, operate on a workbook,
fixture, or process, change or bypass Avast controls, or infer implementation,
package / `dist`, release / publication, external-service, or Git-write
authorization from P9-85.

## Preserved Invariants

P9-85 preserves the consumed P9-80 single-invocation authorization, the P9-79
fixed script identity and parser-only semantics, the requirement for retained
valid structured JSON and zero parse errors before caller success, the P9-82
causal uncertainty, the P9-83 confirmed security event without causal
promotion, security controls, and the separation between correction-input
definition, later GO / NO-GO, implementation or materialization, parser-only
execution, result review, and lifecycle execution.

## Verification

P9-85 verification is documentation-only: review P9-79 through P9-84 and the
synchronized current state; confirm that the fixed target is unchanged, every
evidence-generation boundary is fixed, success follows retained validated JSON,
all missing or invalid evidence fails closed, and Avast controls are neither
changed nor bypassed; run `git diff --check`; scan the four changed Markdown
files for trailing whitespace; and inspect Git branch and staged / unstaged
state. No parser, PowerShell, lifecycle, Excel, workbook, fixture, process,
Avast, implementation test, build, package / `dist`, stage, commit, or push
operation is run.
