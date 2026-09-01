# P9-44 - Read-Only Lifecycle Success-Path Evidence Retry Execution

## Status

COMPLETE / evidence retry attempted and stopped before workbook open

## Purpose

Execute only the P9-43 approved focused retry slice and record the observed
result.

P9-44 may execute only:

```text
tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam
```

within the P9-38 / P9-39 / P9-43 boundaries. P9-44 does not set up or create a
test runner artifact, open the P9 fixture outside the approved command, mutate
or replace the P9 fixture, repair or convert any workbook, mutate any workbook
or VBProject, inject code, import or export modules, change implementation or
test code, run full regression, update package or `dist` artifacts, perform
release or publication work, access external services, stage, commit, push, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Starting State

- P9-43 records GO for a later separate focused retry task limited to
  `tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam`.
- The retry remains limited to focused `AppRunOutputWriteBoundaryTests` scope,
  the exact P9 fixture
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`, and textual / log
  evidence only.
- P9-43 requires a hard stop if the prepared `Build.xlam` or the P9 fixture no
  longer matches the recorded identity.

## Pre-Execution Checks

The P9 fixture identity was rechecked before retry execution without opening
the workbook:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

The prepared runner artifact identity was rechecked before retry execution:

- path: `tmp\p9-success\Build.xlam`;
- length: `515199` bytes;
- SHA-256:
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`.

These checks satisfy the P9-43 preconditions for attempting the focused retry.

## Retry Execution

P9-44 executed the P9-43 approved retry command with local PowerShell
execution-policy wrapping:

```text
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam
```

Observed command result:

| Field | Observed value |
| --- | --- |
| Exit code | `1` |
| Stopped before Excel workbook open | Yes |
| Stopped before P9 fixture open | Yes |
| Failure reason | `VMFTestRunner.xlam not found at C:\Users\biz\Documents\Project\VMF\tools\test\runner\VMFTestRunner.xlam. Run tools\test\setup-test-runner.ps1 first.` |

## Evidence Judgment

P9-44 proves only that the P9-43 retry command was attempted after the P9
fixture and prepared `Build.xlam` identities were rechecked and matched.

P9-44 does not prove successful read-only lifecycle execution. The retry
stopped before Excel workbook open because the local test runner artifact
`tools\test\runner\VMFTestRunner.xlam` was missing. Therefore no successful
read-only open, update-links disabled evidence, add-to-MRU disabled evidence,
identity inspection after open, close without saving, Excel quit /
residual-process confirmation, or post-close unchanged-fixture confirmation
was collected.

The observed failure is a runner precondition gap, not a fixture failed-open
result and not a successful lifecycle proof. P9-44 does not authorize running
`tools\test\setup-test-runner.ps1`; that requires a separate GO / NO-GO
decision.

## Decision

Decision: `GO` for recording P9-44 as the attempted focused retry execution.

Decision: `NO-GO` for claiming successful Excel read-only open, identity
reconfirmation, close without saving, or post-close unchanged-fixture
confirmation from P9-44.

Decision: `NO-GO` for treating the missing `VMFTestRunner.xlam` result as a
fixture failed-open result.

Decision: `NO-GO` for setting up or creating the test runner artifact during
P9-44.

Decision: `NO-GO` for full regression unless separately authorized.

Decision: `NO-GO` for fixture repair, fixture replacement, fixture mutation,
workbook mutation, VBProject mutation, writable lifecycle operations, Save,
SaveAs, code injection, module import / export, package / `dist`, release /
publication, external service operation, staging, commit, push, public API
change, persisted schema change, canonical format change, or Frozen
specification change.

## Selected Next Candidate

Selected next candidate:

**P9-45 - Read-Only Lifecycle Test Runner Artifact Preparation GO / NO-GO**

P9-45 should decide whether the missing local test runner artifact may be
prepared through the established test-runner setup process before any later
success-path evidence retry. P9-45 must not infer permission to run setup from
the P9-44 failure text alone.

## Verification

P9-44 verification:

- reviewed P9-43 retry GO / NO-GO boundaries;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- rechecked `tmp\p9-success\Build.xlam` length and SHA-256;
- executed only the P9-43 approved retry command;
- recorded the exit code and missing-runner precondition result;
- ran `git diff --check` with exit code `0`;
- confirmed Git status after verification.

No successful Excel workbook open, P9 fixture open, fixture mutation, workbook
mutation, VBProject mutation, full regression, package / `dist`, release /
publication, external service operation, staging, commit, or push was performed
by P9-44.
