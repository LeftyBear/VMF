# P9-42 - Read-Only Lifecycle Success-Path Runner Artifact Preparation

## Status

COMPLETE / runner artifact prepared

## Purpose

Execute only the P9-41 approved runner artifact preparation slice and record
the observed result.

P9-42 may create only the temporary local runner artifact
`tmp\p9-success\Build.xlam` from current repository Build source through the
existing Build script. P9-42 does not retry the P9-39 success-path evidence
command, open the P9 fixture, mutate or replace the P9 fixture, repair or
convert any workbook, mutate any workbook or VBProject, run full regression,
update package or `dist` artifacts, perform release or publication work,
access external services, stage, commit, push, or change public APIs,
persisted schemas, canonical formats, or Frozen specifications.

## Starting State

- P9-40 attempted the P9-39 authorized command
  `tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam` and stopped
  before workbook open because `tmp\p9-success\Build.xlam` was missing.
- P9-41 records GO for a later separate runner artifact preparation task
  limited to creating `tmp\p9-success\Build.xlam` from current repository
  Build source through:
  `tools\build\build.ps1 -OutputPath tmp\p9-success\Build.xlam -BuildVersion p9-success-local -ReleaseType LocalVerification`.
- P9-41 does not authorize retrying the P9-39 success-path evidence command
  during the artifact preparation task.

## Pre-Preparation Checks

The P9 fixture identity was rechecked before artifact creation without opening
the workbook:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

The runner artifact was absent before preparation:

- path: `tmp\p9-success\Build.xlam`;
- exists before preparation: `No`.

## Preparation Execution

P9-42 executed the P9-41 approved Build script command with local
PowerShell execution-policy wrapping:

```text
powershell.exe -ExecutionPolicy Bypass -File tools\build\build.ps1 -OutputPath tmp\p9-success\Build.xlam -BuildVersion p9-success-local -ReleaseType LocalVerification
```

Observed command result:

| Field | Observed value |
| --- | --- |
| Exit code | `0` |
| Source | Current repository Build source under `src\Build` |
| Output path | `tmp\p9-success\Build.xlam` |
| Build version | `p9-success-local` |
| Release type | `LocalVerification` |
| Product metadata | `VMF Studio 1.1.0` |
| Saved add-in | Yes |
| Release metadata recorded | Yes |

## Prepared Artifact Evidence

The prepared runner artifact exists after the build:

- path: `tmp\p9-success\Build.xlam`;
- length: `515199` bytes;
- SHA-256:
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`.

The P9 fixture was rechecked after artifact preparation without opening the
fixture:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

## Evidence Judgment

P9-42 proves only that the P9-40 missing runner-artifact precondition has been
prepared at the exact P9-39 command path.

P9-42 does not prove successful read-only lifecycle execution. The P9-39
success-path evidence command was not retried, the P9 fixture was not opened,
and no successful read-only open, identity reconfirmation, close without
saving, Excel quit / residual-process confirmation, or post-close unchanged
fixture confirmation was collected by this task.

The prepared `Build.xlam` is temporary local verification evidence only. It is
not a release artifact, package artifact, `dist` artifact, publication
artifact, or replacement for the committed P9 fixture.

## Decision

Decision: `GO` for recording P9-42 as the completed runner artifact
preparation.

Decision: `NO-GO` for claiming read-only lifecycle success-path proof from
P9-42.

Decision: `NO-GO` for retrying the P9-39 success-path evidence command during
P9-42.

Decision: `NO-GO` for fixture repair, fixture replacement, fixture mutation,
workbook mutation, VBProject mutation, writable lifecycle operations, Save,
SaveAs, full regression, package / `dist`, release / publication, external
service operation, public API change, persisted schema change, canonical
format change, or Frozen specification change.

## Selected Next Candidate

Selected next candidate:

**P9-43 - Read-Only Lifecycle Success-Path Evidence Retry GO / NO-GO**

P9-43 should decide whether the P9-39 success-path evidence command may be
retried now that `tmp\p9-success\Build.xlam` exists. P9-43 must not infer
permission to run the command from artifact preparation alone.

## Verification

P9-42 verification:

- reviewed P9-41 and P9-40 records;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- confirmed `tmp\p9-success\Build.xlam` was absent before preparation;
- executed only the P9-41 approved artifact preparation command;
- confirmed `tmp\p9-success\Build.xlam` exists after preparation;
- recorded the prepared artifact length and SHA-256;
- rechecked the P9 fixture length, SHA-256, and fixture count after
  preparation without opening the fixture;
- ran `git diff --check` with exit code `0`;
- confirmed Git status after verification.

No implementation tests, full regression, P9-39 success-path evidence retry,
P9 fixture open, workbook mutation, VBProject mutation, package / `dist`,
release / publication, external service operation, staging, commit, or push
was performed by P9-42.
