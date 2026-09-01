# P9-48 - Read-Only Lifecycle Success-Path Evidence Retry Execution

## Status

COMPLETE / read-only lifecycle success-path evidence captured

## Purpose

Execute only the P9-47 approved focused retry slice and record the observed
read-only lifecycle evidence.

P9-48 may execute only:

```text
tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam
```

within the P9-38 / P9-39 / P9-47 boundaries. P9-48 does not create or replace
the P9 fixture, repair or convert any workbook, mutate any workbook or
VBProject, inject code, import or export modules, change implementation or
test code, update package or `dist` release artifacts, perform release or
publication work, access external services, stage, commit, push, or change
public APIs, persisted schemas, canonical formats, or Frozen specifications.

## Starting State

- P9-47 records GO for a later separate focused retry task limited to
  `tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam`.
- The retry remains limited to the exact P9 fixture
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`, the P9-38 / P9-39
  read-only lifecycle boundaries, the P9-46 generated runner artifact, and
  textual / log evidence only.
- P9-47 requires a hard stop if the prepared `Build.xlam`, generated test
  runner artifact, or P9 fixture no longer matches the recorded identity.

## Pre-Execution Checks

The P9 fixture identity was rechecked before retry execution without opening
the workbook:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

The prepared P9 Build artifact identity was rechecked before retry execution:

- path: `tmp\p9-success\Build.xlam`;
- length: `515199` bytes;
- SHA-256:
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`.

The generated local test runner artifact identity was rechecked before retry
execution:

- path: `tools\test\runner\VMFTestRunner.xlam`;
- length: `718210` bytes;
- SHA-256:
  `7A1D1364601DBAC125EDAB9F825281B8140138C30582C8E1687C9BC1837F538C`.

These checks satisfy the P9-47 preconditions for attempting the focused retry.

## Retry Execution

P9-48 executed the P9-47 approved retry command with local PowerShell
execution-policy wrapping:

```text
powershell.exe -ExecutionPolicy Bypass -File tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam
```

Observed command result:

| Field | Observed value |
| --- | --- |
| Exit code | `0` |
| `AppRunOutputWriteBoundaryTests` | `Passed` |
| Runner log | `dist\debug\VMFTestRunner.log` |
| Residual Excel process after completion | None observed |

The current `VMFRunAllTests` runner also executed the other registered Build
VBA runners. They all reported `Passed`. This is recorded as an observed runner
behavior, not as an authorization to broaden future P9 evidence tasks beyond
their explicitly approved scope.

## Evidence Judgment

P9-48 proves the P9 read-only lifecycle success path for the exact authorized
fixture through the approved runner path.

The passing `AppRunOutputWriteBoundaryTests` path exercised
`AppRunReadOnlyWorkbookLifecycle`, which performs:

- fixed repository fixture resolution for
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- pre-open fixture length and SHA-256 identity confirmation;
- explicit-path Excel open with `UpdateLinks:=0`, `ReadOnly:=True`, and
  `AddToMru:=False`;
- workbook `FullName` identity reconfirmation after open;
- read-only mode confirmation;
- close without saving through `Close False`;
- post-close fixture length and SHA-256 identity confirmation;
- `MutatedModules = 0`.

Post-run fixture identity was rechecked from the filesystem and remained:

- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

This is success-path lifecycle evidence, not workbook / VBProject mutation
authorization. It does not authorize writable lifecycle operations, fixture
mutation, fixture repair, fixture replacement, real workbook mutation,
VBProject mutation, full regression as a future default, package / `dist`,
release / publication, external service operation, public API change, persisted
schema change, canonical format change, or Frozen specification change.

## Decision

Decision: `GO` for recording P9-48 as the focused read-only lifecycle
success-path evidence retry execution.

Decision: `PASS` for the P9 read-only lifecycle success-path evidence collected
through `AppRunOutputWriteBoundaryTests`.

Decision: `PASS` for post-close unchanged-fixture evidence.

Decision: `NO-GO` for treating this evidence as writable lifecycle,
workbook / VBProject mutation, fixture mutation, fixture repair, fixture
replacement, full-regression authorization, package / `dist`, release /
publication, external service operation, staging, commit, push, public API
change, persisted schema change, canonical format change, or Frozen
specification change authorization.

## Selected Next Candidate

Selected next candidate:

**P9-49 - Read-Only Lifecycle Evidence Closeout / Next Boundary Selection**

P9-49 should close out the P9 read-only lifecycle evidence chain and select the
next minimum Build vNext boundary without inferring authorization for writable
lifecycle operations, workbook / VBProject mutation expansion, package /
`dist`, release / publication, external services, staging, commit, push, public
API changes, persisted schema changes, canonical format changes, or Frozen
specification changes.

## Verification

P9-48 verification:

- reviewed P9-47 retry GO / NO-GO boundaries;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- rechecked `tmp\p9-success\Build.xlam` length and SHA-256;
- rechecked `tools\test\runner\VMFTestRunner.xlam` length and SHA-256;
- executed only the P9-47 approved retry command;
- recorded exit code `0`;
- recorded `AppRunOutputWriteBoundaryTests: Passed`;
- reviewed `dist\debug\VMFTestRunner.log`;
- rechecked the P9 fixture length and SHA-256 after completion;
- confirmed no residual Excel process was observed after completion.

No fixture mutation, fixture repair, fixture replacement, workbook mutation,
VBProject mutation, implementation change, test code change, package / `dist`
release artifact update, release / publication, external service operation,
staging, commit, push, public API change, persisted schema change, canonical
format change, or Frozen specification change was performed by P9-48.
