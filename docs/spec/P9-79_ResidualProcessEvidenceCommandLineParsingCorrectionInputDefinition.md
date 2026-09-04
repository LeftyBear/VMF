# P9-79 - Residual-Process Evidence Command-Line Parsing Correction Input Definition

## Status

COMPLETE / docs-only command-line parsing correction input definition

## Purpose

Define exactly one complete corrected transport input and one parser-only
Windows PowerShell 5.1 verification method for the P9-76 command-line parsing
failure, while preserving the P9-72 lifecycle script semantics with only the
P9-74 pre-operation identity correction.

P9-79 is documentation only. It does not materialize the input file, invoke a
PowerShell parser, execute or retry the lifecycle command, run Excel
automation, open / create / save / SaveAs / close either workbook, inspect,
mutate, or repair either fixture, terminate or otherwise mutate any process,
change implementation or test code, run implementation tests, update package
or `dist` release artifacts, perform release or publication work, access
external services, stage, commit, push, or change public APIs, persisted
schemas, canonical formats, or Frozen specifications.

## Corrected Transport Input

The sole corrected transport is an exact-path local script file passed through
the Windows PowerShell 5.1 `-File` parameter. The caller must start the
executable directly; `cmd.exe`, another PowerShell process, a batch file, a
shell command string, and any other intermediary command parser are prohibited.

The fixed input components are:

| Component | Fixed value |
|---|---|
| Caller | direct process creation with executable and ordered arguments supplied separately |
| Executable | `C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe` |
| Working directory | `C:\Users\biz\Documents\Project\VMF` |
| Argument 1 | `-NoLogo` |
| Argument 2 | `-NoProfile` |
| Argument 3 | `-NonInteractive` |
| Argument 4 | `-ExecutionPolicy` |
| Argument 5 | `Bypass` |
| Argument 6 | `-File` |
| Argument 7 | `C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1` |
| Script encoding | UTF-8 with BOM |
| Newline form | CRLF between every line, with one final CRLF after the last line |

The exact later lifecycle invocation, if separately authorized, is the
executable and seven argument-vector entries above. The following display form
is informative only and must not be submitted through a shell command string:

```text
C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -File C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1
```

Before any parser-only verification or later lifecycle execution, the exact
materialized script below must be written to the fixed path with the fixed
encoding and newline form. No character substitution, interpolation, line
omission, normalization after materialization, or alternate path is permitted.

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
    [ordered]@{ Path = $item.FullName; Length = $item.Length; SHA256 = (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash; Attributes = $item.Attributes.ToString() }
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

## Quoting Boundary

The corrected transport has no script-bearing command-line argument. The
script's here-string delimiters, embedded double quotes, and single-quoted
PowerShell strings exist only in the UTF-8 file bytes and cross no command-line
quoting or escaping layer. The fixed executable path, working directory, and
script path contain no spaces and require no surrounding quotes. Each argument
is a distinct process argument; joining the vector into a command string is
prohibited.

This transport choice corrects the under-specified multiline `-Command`
boundary without claiming which character or boundary caused P9-76. It does
not change any lifecycle meaning.

## Parser-Only Verification Method

A later, separately authorized parser-verification task must use Windows
PowerShell 5.1 to load the exact fixed-path file as bytes, confirm the UTF-8
BOM, decode it strictly as UTF-8, confirm CRLF-only line endings and one final
CRLF, and compute and record its length and SHA-256. It must then call
`[System.Management.Automation.Language.Parser]::ParseFile` on that exact
path, capture both the returned token collection and parse-error collection,
and emit an encoding-stable JSON result containing the file path, byte length,
SHA-256, token count, parse-error count, and every parse error's extent text,
start line, start column, end line, end column, error ID, and message.

Parser verification passes only when the byte and newline checks pass and the
parse-error count is zero. The verifier must not dot-source, invoke, or import
the script, create Excel, open a workbook, inspect a fixture, query or mutate
an Excel process, or perform any lifecycle operation. Parser diagnostics must
be captured as structured Unicode values and serialized to UTF-8 JSON; console
rendering is not authoritative evidence.

P9-79 defines this method but does not materialize the file, compute its final
byte identity, or invoke the parser. Those observations belong to a later
separately authorized parser-only verification task.

## Semantic Equivalence

The materialized script copies the P9-72 script in full and makes only the
P9-74 correction: the three expected-identity constants are inserted before
`Add-Type`, and the P9-72 pre-operation block is replaced with the exact P9-74
block. Paths, expected identities, fixture-count requirement, process
precondition, HWND-to-PID correlation, workbook-open arguments, close-without-
saving behavior, lifecycle and COM-release order, evidence events, immediate
acceptance point, diagnostic offsets and maximum window, classifications,
final-state checks, and exit-code rules are otherwise unchanged.

Switching from a multiline `-Command` argument to an exact-path `-File`
transport changes only delivery to the same Windows PowerShell 5.1 parser. It
does not authorize or introduce a semantic script correction beyond P9-74.

## Decision

Decision: `GO` for recording P9-79 as the docs-only command-line parsing
correction input definition.

Decision: `PASS` for defining one complete `-File` transport input, a fully
materialized P9-72 plus P9-74 script, fixed encoding and newline form, direct
process argument boundaries, an encoding-stable parser-only verification
method, and semantic-equivalence traceability.

Decision: `NO-GO` for claiming an exact P9-76 root cause, materializing the
script file, computing observed byte identity, invoking parser verification,
executing or retrying the lifecycle command, Excel automation, opening or
mutating either fixture, timing or path substitution, fallback workbook or
process selection, process termination, Save, SaveAs, fixture repair,
implementation or test code change, acceptance-criterion change, broader
workbook / VBProject mutation, package / `dist`, release / publication,
external services, staging, commit, push, public API change, persisted schema
change, canonical format change, or Frozen specification change from P9-79.

## Selected Next Candidate

**P9-80 - Residual-Process Evidence Command-Line Parser Verification GO / NO-GO**

P9-80 should remain docs-only and review the complete P9-79 transport,
materialized script definition, parser-only method, and semantic-equivalence
account before deciding whether one later separate parser-only verification
may be authorized. P9-80 must not materialize the file, invoke the parser,
execute the lifecycle command, or infer broader authorization from P9-79.

## Preserved Invariants

P9-79 preserves the P9-65 and P9-68 immediate residual-process hard stops and
exit code `1`, the P9-72 fixed acceptance point and timing semantics, the
P9-74 authoritative fixture identities and pre-operation checks, the
historical fixture as immutable evidence input, exact workbook paths, close
without saving, and the separation of correction-input definition,
parser-verification GO / NO-GO, parser-only verification, later lifecycle-
execution GO / NO-GO, and any separately authorized lifecycle execution.

## Verification

P9-79 verification is documentation-only: review P9-72, P9-74, P9-75, P9-76,
P9-77, P9-78, and the synchronized current state; compare the complete script
against P9-72 plus only P9-74; confirm that every transport, encoding, newline,
quoting, parser-evidence, and equivalence field is fixed; then run docs-only
diff confirmation, `git diff --check`, trailing-whitespace scan, and Git status
confirmation. No implementation test, parser invocation, command execution,
Excel automation, workbook operation, fixture identity recheck, or process
mutation is required or run.
