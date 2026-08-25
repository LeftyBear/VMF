# P6-04 - Output Write Focused Test Implementation Start

## Status

COMPLETE / local-only implementation verified

## Purpose

Start the narrow output-write focused implementation after P6-03 by adding a
local output-write boundary and focused tests without performing generated
output writes or target VBA project mutation.

## Scope

P6-04 implements:

- formal title:
  `Output Write Focused Test Implementation Start`
- implementation decision:
  `GO for minimal local output-write plan construction and focused tests only`
- production entry boundary:
  `AppOutputWriteService.AppBuildOutputWritePlan`
- focused test target:
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas`
- runner registration:
  `tools/test/runner/VMFTestRunner.bas`
- Application manifest registration:
  `src/Build/Application.manifest`

## GO / NO-GO Decision

GO:

- add the narrow `AppOutputWriteService` boundary
- accept only complete successful Generator output represented by a local
  structured model
- produce deterministic write-plan units only
- add focused local tests for successful write-plan construction
- add focused local tests for hard stops before output write
- update backlog and current-status records
- run focused Build verification and `git diff --check`

NO-GO:

- write generated output to disk, workbook, VBA project, package, or `dist`
- mutate a target VBA project
- create, update, remove, import, export, or overwrite target modules
- change Parser, Validator, Manifest Derivation, Template Derivation,
  GenerateContext, or Generator behavior
- infer missing facts from Template contents, generated output, target project
  state, or runtime state
- add fallback or implicit Template selection
- repair, normalize, complete, or compensate for incomplete upstream data
- change public APIs, persisted schemas, canonical formats, Frozen
  specifications, package, `dist`, release, publication, or external services

## Implemented Behavior

`AppOutputWriteService.AppBuildOutputWritePlan` consumes only a complete
successful Generator output object with `GeneratedUnits`.

For successful input, it returns:

- `Success = True`
- `Classification = Success`
- one planned write unit per generated unit
- carried `moduleName`, `moduleType`, `templateKey`, and `generatedSource`
- deterministic file name planning from `moduleName` and `moduleType`
- `writeStatus = Planned`

For blocking input, it returns a hard-stop result with no write units.

Blocking cases include:

- missing Generator output
- failed Generator output
- missing generated units
- missing deterministic order
- missing `moduleName`, `moduleType`, `templateKey`, or `generatedSource`
- fallback-derived Template selection
- implicit Template selection

## Preserved Boundaries

P6-04 preserves the P5-04 through P6-03 boundaries:

- output write remains post-Generator
- target VBA project mutation remains a separate downstream boundary
- no generated output write is performed
- no target VBA project mutation is performed
- no fallback Template selection is allowed
- no implicit Template selection is allowed
- no Template content inference is allowed
- no GenerateContext or Generator compensation is allowed
- hard-stop cases produce no write units
- package, `dist`, release, publication, external service, public API,
  persisted schema, canonical format, and Frozen specification changes remain
  NO-GO

## Verification Plan

Required verification for P6-04:

- build a temporary local Build.xlam from current source
- create a test runner from that temporary local Build.xlam
- run the Build VBA test runner
- run `git diff --check`

Generated verification artifacts are local-only and are not package, `dist`,
release, or publication artifacts.

## Verification Performed

| Check | Result | Notes |
| --- | --- | --- |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\build\build.ps1 -OutputPath tmp\p6-04\Build.xlam -BuildVersion p6-04-local -ReleaseType LocalVerification` | PASS | Temporary local Build output only. |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\test\setup-test-runner.ps1 -BuildPath tmp\p6-04\Build.xlam` | PASS | Test runner created from the temporary local Build. |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\test\run-tests.ps1` | PASS | All 22 VBA test runners passed, including `AppRunOutputWriteBoundaryTests`. |
| `git diff --check` | PASS | LF-to-CRLF warnings only. |

Generated verification artifacts were removed after verification:

- `tmp/p6-04/Build.xlam`
- `tools/test/runner/VMFTestRunner.xlam`
- `dist/debug/VMFTestRunner.log`
- `dist/debug/InfDiag.log`
- `dist/debug/test-target`
