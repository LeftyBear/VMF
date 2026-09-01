# P9-46 - Read-Only Lifecycle Test Runner Artifact Preparation

## Status

COMPLETE / test runner artifact prepared

## Purpose

Execute only the P9-45 approved local test runner artifact preparation slice
and record the observed result.

P9-46 may execute only:

```text
tools\test\setup-test-runner.ps1 -BuildPath tmp\p9-success\Build.xlam
```

with local PowerShell execution-policy wrapping as needed. P9-46 does not run
the P9-39 / P9-43 success-path evidence retry command, run full regression,
open the P9 fixture through the evidence runner, mutate or replace the P9
fixture, repair or convert any workbook, perform writable lifecycle operations
on the P9 fixture, update package or `dist` artifacts, perform release or
publication work, access external services, stage, commit, push, or change
public APIs, persisted schemas, canonical formats, or Frozen specifications.

## Starting State

- P9-44 stopped before Excel workbook open because
  `tools\test\runner\VMFTestRunner.xlam` was missing.
- P9-45 records GO for a later separate test runner artifact preparation task
  limited to
  `tools\test\setup-test-runner.ps1 -BuildPath tmp\p9-success\Build.xlam`.
- P9-45 does not authorize retrying
  `tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam` during the
  artifact preparation task.

## Pre-Preparation Checks

The P9 fixture identity was rechecked before setup without opening the fixture:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

The prepared P9 Build artifact identity was rechecked before setup:

- path: `tmp\p9-success\Build.xlam`;
- length: `515199` bytes;
- SHA-256:
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`.

The local test runner artifact was absent before setup:

- path: `tools\test\runner\VMFTestRunner.xlam`;
- exists before preparation: `No`.

## Preparation Execution

P9-46 executed the P9-45 approved setup command with local PowerShell
execution-policy wrapping:

```text
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\test\setup-test-runner.ps1 -BuildPath tmp\p9-success\Build.xlam
```

Observed command result:

| Field | Observed value |
| --- | --- |
| Exit code | `0` |
| Trusted location state | `Excel trusted location already registered: C:\Users\biz\Documents\Project\VMF\tools\test\runner\` |
| Created artifact | `C:\Users\biz\Documents\Project\VMF\tools\test\runner\VMFTestRunner.xlam` |

## Prepared Artifact Evidence

The local test runner artifact exists after setup:

- path: `tools\test\runner\VMFTestRunner.xlam`;
- length: `718210` bytes;
- SHA-256:
  `7A1D1364601DBAC125EDAB9F825281B8140138C30582C8E1687C9BC1837F538C`.

The P9 fixture identity was rechecked after setup without opening the fixture:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`.

The prepared P9 Build artifact identity was rechecked after setup:

- path: `tmp\p9-success\Build.xlam`;
- length: `515199` bytes;
- SHA-256:
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`.

No residual Excel process was observed after setup.

## Evidence Judgment

P9-46 proves only that the local test runner artifact precondition identified
by P9-44 has been prepared through the P9-45 approved setup command.

P9-46 does not prove successful read-only lifecycle execution. The P9-39 /
P9-43 success-path evidence retry command was not run, the P9 fixture was not
opened through the evidence runner, and no successful read-only open,
update-links disabled evidence, add-to-MRU disabled evidence, identity
inspection after open, close without saving, Excel quit / residual-process
confirmation for the retry, or post-close unchanged-fixture confirmation was
collected by this task.

The generated `VMFTestRunner.xlam` is temporary local verification evidence
only. It is not a release artifact, package artifact, `dist` artifact,
publication artifact, or replacement for the committed runner source
`tools\test\runner\VMFTestRunner.bas`.

## Decision

Decision: `GO` for recording P9-46 as the completed local test runner artifact
preparation.

Decision: `NO-GO` for claiming read-only lifecycle success-path proof from
P9-46.

Decision: `NO-GO` for retrying the P9-39 / P9-43 success-path evidence command
during P9-46.

Decision: `NO-GO` for full regression unless separately authorized.

Decision: `NO-GO` for fixture repair, fixture replacement, fixture mutation,
workbook mutation, VBProject mutation outside the established setup script,
writable lifecycle operations on the P9 fixture, Save / SaveAs on the P9
fixture, package / `dist`, release / publication, external service operation,
staging, commit, push, public API change, persisted schema change, canonical
format change, or Frozen specification change.

## Selected Next Candidate

Selected next candidate:

**P9-47 - Read-Only Lifecycle Success-Path Evidence Retry GO / NO-GO**

P9-47 should decide whether the focused success-path evidence retry may be run
now that both local prerequisites exist:

- `tmp\p9-success\Build.xlam`;
- `tools\test\runner\VMFTestRunner.xlam`.

P9-47 must not infer permission to run the retry from artifact preparation
alone.

## Verification

P9-46 verification:

- reviewed P9-44 and P9-45 records;
- reviewed the established `tools\test\setup-test-runner.ps1` setup script;
- rechecked the P9 fixture length and SHA-256 without opening the fixture;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- rechecked `tmp\p9-success\Build.xlam` length and SHA-256;
- confirmed `tools\test\runner\VMFTestRunner.xlam` was absent before setup;
- executed only the P9-45 approved setup command;
- confirmed `tools\test\runner\VMFTestRunner.xlam` exists after setup;
- recorded the generated runner artifact length and SHA-256;
- rechecked the P9 fixture and prepared P9 Build artifact identities after
  setup;
- confirmed no residual Excel process was observed after setup;
- required post-edit verification: documentation diff review,
  `git diff --check`, trailing-whitespace check, and Git status confirmation.

No P9-39 / P9-43 success-path evidence retry, full regression, P9 fixture open
through the evidence runner, P9 fixture mutation, workbook repair, workbook
replacement, package / `dist`, release / publication, external service
operation, staging, commit, or push was performed by P9-46.
