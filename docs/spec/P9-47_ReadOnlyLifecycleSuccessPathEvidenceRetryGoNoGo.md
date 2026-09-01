# P9-47 - Read-Only Lifecycle Success-Path Evidence Retry GO / NO-GO

## Status

COMPLETE / docs-only success-path evidence retry GO / NO-GO decision

## Purpose

Decide whether the P9 read-only lifecycle success-path evidence retry may be
run after P9-46 prepared the local test runner artifact precondition.

P9-47 is documentation only. It does not execute the retry command, start Excel
automation, open / create / save / SaveAs / close / discard / restore any
workbook, mutate or replace the P9 fixture, repair or convert any workbook,
mutate any workbook or VBProject, inject code, import or export modules, change
implementation or test code, run full regression, update package or `dist`
artifacts, perform release or publication work, access external services,
stage, commit, push, or change public APIs, persisted schemas, canonical
formats, or Frozen specifications.

## Starting State

- P9-43 records GO for a later separate focused retry task limited to
  `tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam`.
- P9-44 executed that retry command and stopped before Excel workbook open
  because `tools\test\runner\VMFTestRunner.xlam` was missing.
- P9-45 records GO for a later separate local test runner artifact preparation
  task limited to
  `tools\test\setup-test-runner.ps1 -BuildPath tmp\p9-success\Build.xlam`.
- P9-46 executed that preparation command, created
  `tools\test\runner\VMFTestRunner.xlam`, and did not run the success-path
  evidence retry.

## Current Fixture And Artifact State

The current P9 fixture identity was rechecked during P9-47 without opening the
fixture:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

The prepared P9 Build artifact still exists at the exact retry command path:

- path: `tmp\p9-success\Build.xlam`;
- length: `515199` bytes;
- SHA-256:
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`.

The local test runner artifact prepared by P9-46 exists:

- path: `tools\test\runner\VMFTestRunner.xlam`;
- length: `718210` bytes;
- SHA-256:
  `7A1D1364601DBAC125EDAB9F825281B8140138C30582C8E1687C9BC1837F538C`.

These checks prove only that the P9-44 missing-runner precondition has been
removed. They do not prove successful read-only lifecycle execution.

## Approved Later Retry Slice

P9-47 records GO only for a later separate focused retry task limited to the
existing P9-43 retry command:

```text
tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam
```

The later retry task may use local PowerShell execution-policy wrapping needed
to run that exact script locally, but it must preserve the same script and
`BuildPath` value.

The later retry task may collect only textual / log evidence for:

- explicit-path read-only open of
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- update-links disabled;
- add-to-MRU disabled;
- identity inspection after open;
- close without saving;
- Excel quit and residual-process confirmation if possible;
- post-close unchanged fixture length and SHA-256.

The later retry task must preserve the focused
`AppRunOutputWriteBoundaryTests` scope and must not run full regression unless
separately authorized.

## Safety Stops For Later Retry

The later retry task must stop before or during evidence collection if any of
the following is true:

- `tmp\p9-success\Build.xlam` is missing or no longer matches length
  `515199` bytes and SHA-256
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`;
- `tools\test\runner\VMFTestRunner.xlam` is missing or no longer matches
  length `718210` bytes and SHA-256
  `7A1D1364601DBAC125EDAB9F825281B8140138C30582C8E1687C9BC1837F538C`;
- the P9 fixture is missing, duplicated, or no longer matches length `3532`
  bytes and SHA-256
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- any workbook other than
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` would be selected;
- workbook auto-discovery, active workbook selection, recent-file selection,
  fallback workbook selection, repair, conversion, or replacement would be
  required;
- Excel cannot open the fixture successfully as read-only;
- evidence cannot prove close without saving or post-close unchanged fixture
  identity;
- the operation would require Save, SaveAs, write operation, fixture mutation,
  workbook mutation, VBProject mutation, code injection, module import /
  export, business workbook operation, or production workbook operation;
- the operation would require package / `dist`, release / publication,
  external service operation, public API change, persisted schema change,
  canonical format change, or Frozen specification change;
- textual / log evidence would capture workbook content dumps, VBA source
  exports, tokens, credentials, or environment secrets.

If Excel cannot open the fixture successfully, the later retry task must record
that failed-open result as a success-path evidence gap / `NO-GO`. It must not
repair, convert, replace, or silently treat failed-open as success-path proof.

## Decision

Decision: `GO` for recording P9-47 as a docs-only read-only lifecycle
success-path evidence retry GO / NO-GO decision.

Decision: `GO` for a later separate focused retry task limited to the P9-43
command, P9-38 / P9-39 operation limits, P9-46 prepared local runner artifact,
P9-42 prepared Build artifact, exact P9 fixture identity, failed-open policy,
and textual / log evidence-retention boundary.

Decision: `NO-GO` for executing the retry during P9-47.

Decision: `NO-GO` for claiming successful Excel read-only open, identity
reconfirmation, close without saving, or post-close unchanged-fixture
confirmation during P9-47.

Decision: `NO-GO` for full regression unless separately authorized.

Decision: `NO-GO` for fixture repair, fixture replacement, fixture creation,
fixture mutation, workbook mutation, VBProject mutation, writable lifecycle
operations, Save, SaveAs, code injection, module import / export, package /
`dist`, release / publication, external service operation, staging, commit,
push, public API change, persisted schema change, canonical format change, or
Frozen specification change.

## Selected Next Candidate

Selected next candidate:

**P9-48 - Read-Only Lifecycle Success-Path Evidence Retry Execution**

P9-48 may execute only the P9-47 approved focused retry slice. It must preserve
the P9-38 / P9-39 failed-open, replacement-fixture, textual-evidence,
operator-review, and full-regression boundaries.

## Verification

P9-47 verification is documentation-only:

- reviewed P9-43, P9-44, P9-45, and P9-46 records;
- reviewed backlog, current-status, and handoff state;
- rechecked the P9 fixture length and SHA-256 without opening the fixture;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- confirmed `tmp\p9-success\Build.xlam` exists at the retry command path;
- recorded the prepared Build artifact length and SHA-256;
- confirmed `tools\test\runner\VMFTestRunner.xlam` exists;
- recorded the generated runner artifact length and SHA-256;
- recorded a GO decision only for a later separate focused retry task;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, trailing-whitespace check, and Git status confirmation.

No implementation tests are required or run for P9-47. No workbook, Excel, or
VBProject operation is required or run for P9-47.
