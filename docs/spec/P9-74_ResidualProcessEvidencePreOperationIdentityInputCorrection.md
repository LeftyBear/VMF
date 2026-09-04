# P9-74 - Residual-Process Evidence Pre-Operation Identity Input Correction

## Status

COMPLETE / docs-only pre-operation identity input correction

## Purpose

Correct only the pre-operation fixture-identity omission identified by P9-73
while preserving the P9-72 command, PID correlation, lifecycle operation,
acceptance point, diagnostic timing, and hard-stop semantics.

P9-74 is documentation only. It does not execute the corrected input, run
Excel automation, open / create / save / SaveAs / close any workbook, mutate
or repair either fixture, terminate or otherwise mutate any process, change
implementation or test code, run implementation tests, update package or
`dist` release artifacts, perform release or publication work, access external
services, stage, commit, push, or change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Corrected Fixed Input

The P9-72 outer command, complete script, paths, functions, PID-correlation
method, workbook-open arguments, lifecycle sequence, evidence events,
acceptance point, diagnostic offsets, maximum observation window, final-state
checks, classifications, and exit-code rules remain fixed. The only correction
is the following exact pre-operation identity input and check.

The authoritative expected values are fixed as:

| Input | Exact path | Length | SHA-256 | Attributes |
|---|---|---:|---|---|
| Replacement writable fixture | `C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks\P9_WritableLifecycleFixture.xlsm` | `8342` | `220322FA5416DC7A10CD80BFBCEE0AE277D699175FF7A874E5DE3163D7FC301B` | `Archive` |
| Historical immutable fixture | `C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` | `3532` | `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E` | `Archive` |

Immediately after the P9-72 path variables and before `Add-Type`, the corrected
script fixes these constants:

```powershell
$expectedFixture = [ordered]@{ Path = $fixture; Length = 8342; SHA256 = '220322FA5416DC7A10CD80BFBCEE0AE277D699175FF7A874E5DE3163D7FC301B'; Attributes = 'Archive' }
$expectedHistorical = [ordered]@{ Path = $historical; Length = 3532; SHA256 = 'BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E'; Attributes = 'Archive' }
$expectedFixtureCount = 2
```

The P9-72 pre-operation block from `$preTimestamp` through its existing
precondition `if` is replaced by this exact block:

```powershell
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
```

This block completes all path, length, SHA-256, attributes, and exact-count
comparisons before the P9-72 `try` block. Therefore any mismatch exits `1`
before `New-Object -ComObject Excel.Application` and before workbook open. It
also emits the actual and expected identity inputs and the individual and
aggregate comparison results in `PRE_OPERATION` evidence.

No other line of the P9-72 fixed script is changed. The P9-72 prohibition on
path discovery, process-difference fallback, PID substitution, timing
substitution, argument omission, command correction during execution, or a
second invocation remains authoritative.

## Decision

Decision: `GO` for recording P9-74 as the docs-only pre-operation identity
input correction.

Decision: `PASS` for correcting the fixed input so both fixtures and the exact
fixture count are compared with authoritative expected values before Excel
creation or workbook open.

Decision: `NO-GO` for executing the corrected input or claiming complete
writable lifecycle success-path evidence from P9-65 through P9-74.

Decision: `NO-GO` for any further command correction from P9-74, another
ordinary retry, Excel automation, opening or mutating either fixture, Save,
SaveAs, fallback workbook or process selection, process termination,
implementation or test code change, acceptance-criterion change, workbook /
VBProject mutation, package / `dist`, release / publication, external
services, staging, commit, push, public API change, persisted schema change,
canonical format change, or Frozen specification change.

## Selected Next Candidate

**P9-75 - Residual-Process Evidence Execution GO / NO-GO**

P9-75 should remain docs-only and review the P9-72 fixed input with the exact
P9-74 correction for internal consistency and safety before deciding whether
one later separate focused evidence execution may be authorized. P9-75 must
not execute the corrected input or infer broader authorization from P9-74.

## Preserved Invariants

P9-74 preserves the P9-65 and P9-68 immediate residual-process hard stops and
exit code `1`, immediate acceptance before diagnostic follow-up, delayed
natural exit as diagnostic final-safe-state evidence only, the historical
fixture as immutable evidence input, exact paths and identities, close without
saving, and separation of input correction, later GO / NO-GO, and execution.

## Verification

P9-74 verification is documentation-only: review P9-72 and P9-73, confirm the
corrected pre-operation block checks both exact fixture identities and the
exact count before Excel creation or workbook open, then run docs-only diff
confirmation, `git diff --check`, trailing-whitespace scan, and Git status
confirmation. No implementation tests, Excel automation, workbook operation,
fixture identity recheck, or process mutation are required or run.
