# P9-93 - Residual-Process Evidence Runtime-Precondition Correction Input Definition

## Status

COMPLETE / docs-only runtime-precondition correction input definition

## Purpose

Define exactly one complete successor lifecycle input that removes
`Get-FileHash` as a runtime prerequisite while preserving the P9-79 direct
Windows PowerShell 5.1 `-File` transport and every other P9-72 plus P9-74
lifecycle semantic.

P9-93 is documentation only. It does not materialize, inspect, invoke, or
execute either the historical P9-79 target or the successor input, invoke a
parser or runtime probe, start Excel, open, save, or mutate a workbook or
fixture, query, terminate, or mutate a process, change or bypass a security
control, change implementation, tests, or tools, update package or `dist`
artifacts, perform release or publication work, access external services,
stage, commit, push, or change public APIs, persisted schemas, canonical
formats, or Frozen specifications.

## Preserved Historical Input

The P9-79 target remains historical evidence and is not a correction target:

| Property | Fixed value |
|---|---|
| Path | `C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1` |
| Observed length | `8264` bytes |
| Observed SHA-256 | `80749CA24C4CF7A8BF27CF8D55633B526FF64FFBF6CE8216590C609060B1F353` |
| Encoding | UTF-8 with BOM |
| Newlines | CRLF only, with exactly one final CRLF |
| State | unchanged and not rematerialized by P9-93 |

## Successor Transport and Identity

The sole successor input uses direct process creation with the executable and
ordered arguments supplied separately. A shell, command string, batch file,
`cmd.exe`, intermediary PowerShell process, and alternate parser are
prohibited.

| Component | Fixed value |
|---|---|
| Executable | `C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe` |
| Working directory | `C:\Users\biz\Documents\Project\VMF` |
| Argument 1 | `-NoLogo` |
| Argument 2 | `-NoProfile` |
| Argument 3 | `-NonInteractive` |
| Argument 4 | `-ExecutionPolicy` |
| Argument 5 | `Bypass` |
| Argument 6 | `-File` |
| Argument 7 | `C:\Users\biz\AppData\Local\Temp\VMF-P9-93-ResidualProcessEvidence.ps1` |
| Script encoding | UTF-8 with BOM |
| Newline form | CRLF between every line, with exactly one final CRLF |
| Defined length | `8465` bytes |
| Defined SHA-256 | `805098C3BCA120E5FBBBF0B2FFC6511FDBB21A19FFE4BC6B629EF4416CF3B208` |

The length and SHA-256 above are definition-time values calculated over the
UTF-8 BOM, exact source bytes below, CRLF line endings, and one final CRLF.
They are not an observation of a materialized file.

## Exact Successor Source

```powershell
$ErrorActionPreference = 'Stop'
$fixture = 'C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks\P9_WritableLifecycleFixture.xlsm'
$historical = 'C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm'
$fixtureDir = 'C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks'
$offsetsMs = @(250, 500, 1000, 2000)
$excel = $null; $books = $null; $workbook = $null; $excelPid = 0; $hardStop = $false
$classification = 'OPERATION FAILURE'
$expectedFixture = [ordered]@{ Path = $fixture; Length = 8342; SHA256 = '220322FA5416DC7A10CD80BFBCEE0AE277D699175FF7A874E5DE3163D7FC301B'; Attributes = 'Archive' }
$expectedHistorical = [ordered]@{ Path = $historical; Length = 3532; SHA256 = 'BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E'; Attributes = 'Archive' }
$expectedFixtureCount = 2

Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class P972NativeMethods {
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
}
'@

function Get-Identity([string]$Path) {
    $item = Get-Item -LiteralPath $Path
    $stream = [System.IO.File]::OpenRead($Path)
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try { $hash = ([BitConverter]::ToString($sha.ComputeHash($stream))).Replace('-', '') }
    finally { $sha.Dispose(); $stream.Dispose() }
    [ordered]@{ Path = $item.FullName; Length = $item.Length; SHA256 = $hash; Attributes = $item.Attributes.ToString() }
}

function Write-Evidence([string]$Event, [hashtable]$Fields) {
    $record = [ordered]@{ TimestampUtc = [DateTime]::UtcNow.ToString('o'); Event = $Event }
    foreach ($key in $Fields.Keys) { $record[$key] = $Fields[$key] }
    $record | ConvertTo-Json -Compress -Depth 6
}

$preTimestamp = [DateTime]::UtcNow
$preProcesses = @(Get-Process -Name EXCEL -ErrorAction SilentlyContinue | ForEach-Object { [ordered]@{ PID = $_.Id; StartTimeUtc = $_.StartTime.ToUniversalTime().ToString('o') } })
$preFixture = Get-Identity $fixture
$preHistorical = Get-Identity $historical
$preCount = @(Get-ChildItem -LiteralPath $fixtureDir -File | Where-Object Extension -eq '.xlsm').Count
$fixtureIdentityMatch = ($preFixture.Path -eq $expectedFixture.Path) -and ($preFixture.Length -eq $expectedFixture.Length) -and ($preFixture.SHA256 -eq $expectedFixture.SHA256) -and ($preFixture.Attributes -eq $expectedFixture.Attributes)
$historicalIdentityMatch = ($preHistorical.Path -eq $expectedHistorical.Path) -and ($preHistorical.Length -eq $expectedHistorical.Length) -and ($preHistorical.SHA256 -eq $expectedHistorical.SHA256) -and ($preHistorical.Attributes -eq $expectedHistorical.Attributes)
$preIdentityMatch = $fixtureIdentityMatch -and $historicalIdentityMatch -and ($preCount -eq $expectedFixtureCount)
Write-Evidence 'PRE_OPERATION' @{ ExcelProcesses = $preProcesses; Fixture = $preFixture; ExpectedFixture = $expectedFixture; FixtureIdentityMatch = $fixtureIdentityMatch; Historical = $preHistorical; ExpectedHistorical = $expectedHistorical; HistoricalIdentityMatch = $historicalIdentityMatch; FixtureCount = $preCount; ExpectedFixtureCount = $expectedFixtureCount; IdentityMatch = $preIdentityMatch }
if ($preProcesses.Count -ne 0 -or -not $preIdentityMatch) { exit 1 }

try {
    $excel = New-Object -ComObject Excel.Application
    $excel.Visible = $false
    $excel.DisplayAlerts = $false
    [uint32]$createdPid = 0
    [void][P972NativeMethods]::GetWindowThreadProcessId([IntPtr]$excel.Hwnd, [ref]$createdPid)
    if ($createdPid -eq 0) { throw 'Created Excel PID correlation failed.' }
    $createdProcess = Get-Process -Id $createdPid -ErrorAction Stop
    if ($createdProcess.ProcessName -ne 'EXCEL' -or $createdProcess.StartTime.ToUniversalTime() -lt $preTimestamp) { throw 'Created Excel PID identity is inconsistent with the pre-operation inventory.' }
    $excelPid = [int]$createdPid
    Write-Evidence 'PID_CORRELATED' @{ PID = $excelPid; StartTimeUtc = $createdProcess.StartTime.ToUniversalTime().ToString('o'); Hwnd = [int64]$excel.Hwnd }

    $books = $excel.Workbooks
    $workbook = $books.Open($fixture, 0, $false, [Type]::Missing, [Type]::Missing, [Type]::Missing, $true, [Type]::Missing, [Type]::Missing, $false, $false, [Type]::Missing, $false, $false, [Type]::Missing)
    Write-Evidence 'WORKBOOK_OPENED' @{ PID = $excelPid; FullName = $workbook.FullName; ReadOnly = $workbook.ReadOnly; Saved = $workbook.Saved }
    if ($workbook.FullName -ne $fixture -or $workbook.ReadOnly -or -not $workbook.Saved) { throw 'Workbook identity, mode, or clean-state check failed.' }

    $workbook.Close($false)
    Write-Evidence 'WORKBOOK_CLOSE_RETURNED' @{ PID = $excelPid }
    [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($workbook)
    $workbook = $null
    [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($books)
    $books = $null
    $excel.Quit()
    Write-Evidence 'EXCEL_QUIT_RETURNED' @{ PID = $excelPid }
    [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($excel)
    $excel = $null
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()

    $releaseBoundary = [Diagnostics.Stopwatch]::StartNew()
    $present = $null -ne (Get-Process -Id $excelPid -ErrorAction SilentlyContinue)
    $total = @(Get-Process -Name EXCEL -ErrorAction SilentlyContinue).Count
    Write-Evidence 'IMMEDIATE_POST_RELEASE' @{ PID = $excelPid; Present = $present; TotalExcelProcesses = $total; ElapsedMilliseconds = 0 }
    if ($present -or $total -ne 0) { $hardStop = $true }

    if (-not $present -and $total -eq 0) { $classification = 'NO INITIAL RESIDUAL' }
    elseif (-not $present) { $classification = 'UNEXPECTED EXCEL PROCESS' }

    if ($present) {
        foreach ($offset in $offsetsMs) {
            while ($releaseBoundary.ElapsedMilliseconds -lt $offset) { Start-Sleep -Milliseconds ([Math]::Min(25, $offset - $releaseBoundary.ElapsedMilliseconds)) }
            $present = $null -ne (Get-Process -Id $excelPid -ErrorAction SilentlyContinue)
            $total = @(Get-Process -Name EXCEL -ErrorAction SilentlyContinue).Count
            Write-Evidence 'DIAGNOSTIC_FOLLOW_UP' @{ PID = $excelPid; Present = $present; TotalExcelProcesses = $total; ElapsedMilliseconds = $offset }
        }
        if ($present) { $classification = 'OBSERVATION WINDOW EXCEEDED' }
        else { $classification = 'DELAYED NATURAL EXIT' }
    }
}
catch {
    $hardStop = $true
    Write-Evidence 'OPERATION_FAILURE' @{ PID = $excelPid; ErrorType = $_.Exception.GetType().FullName; Message = $_.Exception.Message }
}
finally {
    if ($null -ne $workbook) { try { $workbook.Close($false) } catch {}; try { [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($workbook) } catch {} }
    if ($null -ne $books) { try { [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($books) } catch {} }
    if ($null -ne $excel) { try { $excel.Quit() } catch {}; try { [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($excel) } catch {} }
    $workbook = $null; $books = $null; $excel = $null
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}

$postFixture = Get-Identity $fixture
$postHistorical = Get-Identity $historical
$postCount = @(Get-ChildItem -LiteralPath $fixtureDir -File | Where-Object Extension -eq '.xlsm').Count
$identityMatch = (($preFixture | ConvertTo-Json -Compress) -eq ($postFixture | ConvertTo-Json -Compress)) -and (($preHistorical | ConvertTo-Json -Compress) -eq ($postHistorical | ConvertTo-Json -Compress)) -and ($postCount -eq 2)
$finalPresent = if ($excelPid -eq 0) { $false } else { $null -ne (Get-Process -Id $excelPid -ErrorAction SilentlyContinue) }
$finalTotal = @(Get-Process -Name EXCEL -ErrorAction SilentlyContinue).Count
Write-Evidence 'FINAL_STATE' @{ PID = $excelPid; CorrelatedPIDPresent = $finalPresent; TotalExcelProcesses = $finalTotal; Fixture = $postFixture; Historical = $postHistorical; FixtureCount = $postCount; IdentityMatch = $identityMatch }
if (-not $identityMatch -or $finalPresent -or $finalTotal -ne 0) { $hardStop = $true }
$exitCode = if ($hardStop) { 1 } else { 0 }
Write-Evidence 'COMMAND_RESULT' @{ Classification = $classification; ExitCode = $exitCode }
exit $exitCode
```

## Identity-Calculation Semantics

`Get-Identity` opens the requested file read-only, creates a .NET SHA-256
instance directly, hashes the stream, renders uppercase hexadecimal through
`BitConverter`, removes only hyphen separators, and disposes both objects in a
`finally` block. It performs no command or module discovery and has no
`Get-FileHash`, alternate executable, external tool, fallback, or alternate
fixture path.

The same function remains the sole identity path for both fixtures before and
after the operation. A file-open, hash, or disposal failure continues through
the existing terminating-error and operation-failure boundaries; it is not
recovered, retried, or converted into partial success.

## Semantic-Equivalence Traceability

The successor is the complete P9-79 source with exactly one localized
substitution inside `Get-Identity`:

| P9-79 element | P9-93 successor | Semantic account |
|---|---|---|
| `$item = Get-Item -LiteralPath $Path` | unchanged | path, length, and attributes remain sourced identically |
| `(Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash` | direct `File.OpenRead`, `SHA256.Create`, `ComputeHash`, uppercase `BitConverter` text, deterministic disposal | removes only command-discovery dependency and preserves the 64-character uppercase SHA-256 value |
| returned ordered fields | unchanged | `Path`, `Length`, `SHA256`, and `Attributes` evidence schema is unchanged |
| every call site | unchanged | both pre-operation and post-operation fixture calculations use one path |
| all lines outside `Get-Identity` | byte-for-byte source-text equivalent to P9-79 | lifecycle, evidence, timing, safety, and exit semantics are unchanged |

The changed script path and definition-time byte identity distinguish the
successor from historical P9-79 evidence; they do not change lifecycle
semantics. The executable, ordered arguments 1 through 6, working directory,
fixture paths and identities, count requirement, process precondition,
HWND-to-PID correlation, workbook-open arguments, close without saving,
COM-release order, evidence events and fields, immediate acceptance point,
diagnostic offsets and maximum window, classifications, final checks, and
exit-code rules are unchanged.

## Parser-Only Verification Method

A later separately authorized parser-only verification must validate the exact
successor path, `8465` bytes, defined SHA-256, UTF-8 BOM, CRLF-only lines, and
one final CRLF before calling Windows PowerShell 5.1
`[System.Management.Automation.Language.Parser]::ParseFile` on that path. It
must retain encoding-stable structured JSON containing path, length, SHA-256,
token count, parse-error count, and every parse error's extent text, start and
end line and column, error ID, and message.

PASS requires exact byte identity, zero parse errors, retained valid structured
evidence, and a caller result that fails closed for absent, malformed,
truncated, inconsistent, or unretained evidence. Child exit code `0` alone is
not PASS. The verifier must not dot-source, import, invoke, or execute the
successor, calculate fixture identities, create Excel, open a workbook, query
an Excel process, or perform any lifecycle or runtime-precondition operation.

## Decision

Decision: `GO` for recording P9-93 as the docs-only runtime-precondition
correction input definition.

Decision: `PASS` for defining one complete fixed-identity successor input that
removes every `Get-FileHash` dependency through one self-contained SHA-256
mechanism while preserving all other transport and lifecycle semantics.

Decision: `UNPROVEN` remains authoritative for successor parser readiness,
successor runtime-precondition readiness, the cause of the P9-90 command
unavailability, Excel creation, writable lifecycle success, and residual-
process timing PASS.

Decision: `NO-GO` for materializing either target, invoking a parser or runtime
probe, executing or retrying the lifecycle, Excel automation, workbook or
fixture operation, process query / mutation / termination, fallback, security-
control change or bypass, implementation or test changes, package / `dist`,
release / publication, external services, staging, commit, push, public API
change, persisted schema change, canonical format change, or Frozen
specification change from P9-93.

## Selected Next Candidate

**P9-94 - Runtime-Precondition Correction Parser Verification GO / NO-GO**

P9-94 should remain documentation only and review the exact P9-93 successor
identity, direct-process transport, self-contained SHA-256 substitution,
semantic-equivalence account, parser-only method, failure closure, and
prohibited-operation boundaries before deciding whether one later separate
parser-only verification may be authorized.

P9-94 must not materialize either target, invoke a parser or runtime probe,
execute the lifecycle, start Excel, operate on a workbook, fixture, or process,
change or bypass security controls, or infer implementation, package / `dist`,
release / publication, external-service, or Git-write authorization from
P9-93.

## Preserved Invariants

P9-93 preserves the consumed P9-89 authorization, P9-90 operation-failure
result, immutable P9-79 historical identity, exact fixture identities and
count, P9-72 plus P9-74 lifecycle semantics, close without saving, immediate
residual-process HARD-STOP, no fallback, no process termination, causal
uncertainty, current security controls, and separation between input
definition, parser GO / NO-GO, parser-only evidence, runtime-precondition GO /
NO-GO, isolated runtime-precondition evidence, lifecycle GO / NO-GO, and any
later separately authorized lifecycle execution.

## Verification

P9-93 verification is documentation-only: compare the exact successor source
with P9-79 and confirm that only the path identity and `Get-Identity` hash
implementation differ; independently calculate the defined bytes in memory;
review P9-90 through P9-92 and synchronized current state; run `git diff
--check`; scan the four changed Markdown files for trailing whitespace; and
inspect Git branch and staged / unstaged state. No target is materialized or
inspected, and no parser, PowerShell child, runtime probe, lifecycle, Excel,
workbook, fixture, process, security-control, implementation test, build,
package / `dist`, release, publication, external-service, stage, commit, or
push operation is run.
