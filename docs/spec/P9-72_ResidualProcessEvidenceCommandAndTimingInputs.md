# P9-72 - Residual-Process Evidence Command and Timing Inputs

## Status

COMPLETE / docs-only residual-process evidence command and timing input
definition

## Purpose

Supply the complete, internally consistent operational input set that P9-71
identified as missing before a later residual-process evidence GO / NO-GO can
be decided.

P9-72 is documentation only. It does not execute the command below, run Excel
automation, open / create / save / SaveAs / close any workbook, mutate or
repair either fixture, terminate or otherwise mutate any process, change
implementation or test code, run implementation tests, update package or
`dist` release artifacts, perform release or publication work, access external
services, stage, commit, push, or change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Fixed Command Input

The later operation, if separately authorized, must be run from repository
root `C:\Users\biz\Documents\Project\VMF` in Windows PowerShell 5.1 with this
exact outer command and the verbatim script block below as `<SCRIPT>`:

```text
C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command <SCRIPT>
```

`<SCRIPT>` is a documentation metavariable, not a literal argument. The exact
invocation is the listed executable and arguments followed by the entire
script block as the `-Command` value, without textual substitution inside the
block.

```powershell
$ErrorActionPreference = 'Stop'
$fixture = 'C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks\P9_WritableLifecycleFixture.xlsm'
$historical = 'C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm'
$fixtureDir = 'C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks'
$offsetsMs = @(250, 500, 1000, 2000)
$excel = $null; $books = $null; $workbook = $null; $excelPid = 0; $hardStop = $false
$classification = 'OPERATION FAILURE'

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
Write-Evidence 'PRE_OPERATION' @{ ExcelProcesses = $preProcesses; Fixture = $preFixture; Historical = $preHistorical; FixtureCount = $preCount }
if ($preProcesses.Count -ne 0 -or $preCount -ne 2) { exit 1 }

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

No path discovery, process-difference fallback, PID substitution, timing
substitution, argument omission, command correction during execution, or
second invocation is permitted. A quoting or binding failure is an operation
failure, not authorization to edit and retry the command.

## Fixed PID-Correlation Input

The command requires zero pre-existing Excel processes, obtains the created
application's `Hwnd`, resolves that handle with `GetWindowThreadProcessId`, and
validates that the resulting process is named `EXCEL` and started no earlier
than the pre-operation timestamp. A zero PID, missing PID, pre-existing PID,
unexpected process name, or start-time mismatch is an immediate hard stop. No
process-list difference, newest-process choice, or fallback is permitted.

## Fixed Acceptance Point and Timing Inputs

The immediate acceptance observation occurs synchronously after workbook
`Close($false)`, final release of the workbook and Workbooks RCWs, Excel
`Quit()`, final release of the Excel Application RCW, and two
`GC.Collect` / `GC.WaitForPendingFinalizers` drain cycles, in that order.

The stopwatch starts only after the final drain. The first process query
follows immediately at offset `0 ms`; it is not delayed to a timer boundary.
If the correlated PID is present or the total Excel process count is nonzero,
the result is immediately fixed as HARD-STOP with exit code `1`.

Only when the correlated PID is present at offset `0`, diagnostic observations
occur at `250 ms`, `500 ms`, `1000 ms`, and `2000 ms` from the same stopwatch
origin. The maximum observation window is `2000 ms`. Observations do not
extend or restart the window and do not alter the already fixed HARD-STOP.

The later classification is `NO INITIAL RESIDUAL` when the PID is absent and
the total count is zero at offset `0`; `DELAYED NATURAL EXIT` when it is present
at offset `0` but absent later without intervention; or `OBSERVATION WINDOW
EXCEEDED` when it remains present at `2000 ms`.

## Decision

Decision: `GO` for recording P9-72 as the docs-only command and timing input
definition.

Decision: `PASS` for fixing one exact command, no-fallback HWND-to-PID
correlation method, precise post-COM-release acceptance boundary, diagnostic
offsets `250 / 500 / 1000 / 2000 ms`, and maximum window `2000 ms`.

Decision: `NO-GO` for executing the command or claiming complete writable
lifecycle success-path evidence from P9-65 through P9-72.

Decision: `NO-GO` for command or timing adjustment during execution, another
ordinary retry, Excel automation, opening or mutating either fixture, Save,
SaveAs, process termination, implementation or test code change, acceptance-
criterion change, workbook / VBProject mutation, package / `dist`, release /
publication, external services, staging, commit, push, public API change,
persisted schema change, canonical format change, or Frozen specification
change from P9-72.

## Selected Next Candidate

**P9-73 - Residual-Process Evidence Execution GO / NO-GO**

P9-73 should remain docs-only and review this fixed input set for internal
consistency and safety before deciding whether one later separate focused
evidence execution may be authorized. P9-73 must not execute the command or
infer broader authorization from P9-72.

## Preserved Invariants

P9-72 preserves the P9-65 and P9-68 immediate residual-process hard stops and
exit code `1`, immediate acceptance before diagnostic follow-up, delayed
natural exit as diagnostic final-safe-state evidence only, the historical
fixture as immutable evidence input, exact paths and identities, close without
saving, and separation of input definition, later GO / NO-GO, and execution.

## Verification

P9-72 verification is documentation-only: review the P9-65 through P9-71
evidence chain and current state, confirm the command and timing fields are
complete, then run docs-only diff confirmation, `git diff --check`, trailing-
whitespace scan, and Git status confirmation. No implementation tests, Excel
automation, workbook operation, or process mutation are required or run.
