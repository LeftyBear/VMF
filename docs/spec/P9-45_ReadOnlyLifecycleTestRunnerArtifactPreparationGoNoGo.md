# P9-45 - Read-Only Lifecycle Test Runner Artifact Preparation GO / NO-GO

## Status

COMPLETE / docs-only test runner artifact preparation GO / NO-GO decision

## Purpose

Decide whether the missing local test runner artifact from P9-44 can be
prepared in a later separate task through the established test-runner setup
process.

P9-45 is documentation only. It does not execute `setup-test-runner.ps1`,
create or copy `VMFTestRunner.xlam`, execute the P9-39 / P9-43 retry command,
start Excel automation, open / create / save / SaveAs / close / discard /
restore any workbook, mutate or replace the P9 fixture, repair or convert any
workbook, mutate any workbook or VBProject, inject code, import or export
modules, change implementation or test code, run full regression, update
package or `dist` artifacts, perform release or publication work, access
external services, stage, commit, push, or change public APIs, persisted
schemas, canonical formats, or Frozen specifications.

## Starting State

- P9-43 records GO for a later separate focused retry task limited to:
  `tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam`.
- P9-44 executed that exact retry command after rechecking the P9 fixture and
  prepared `Build.xlam` identities.
- P9-44 stopped before Excel workbook open because
  `tools\test\runner\VMFTestRunner.xlam` was missing.
- P9-44 records that the missing `VMFTestRunner.xlam` is a local test runner
  artifact precondition gap, not a P9 fixture failed-open result and not
  successful read-only lifecycle proof.
- P9-44 selects P9-45 to decide whether the missing local test runner artifact
  may be prepared through the established setup process before any later
  success-path evidence retry.

## Current Fixture And Artifact State

The current P9 fixture identity was rechecked during P9-45 without opening the
workbook:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

The prepared P9 success-path Build artifact still exists at the exact retry
command path:

- path: `tmp\p9-success\Build.xlam`;
- length: `515199` bytes;
- SHA-256:
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`.

The missing local test runner artifact precondition is still current:

- path: `tools\test\runner\VMFTestRunner.xlam`;
- exists during P9-45 decision: `No`;
- P9-45 did not create it.

## Preparation Command For Later Task

The only approved later preparation command is the established local test
runner setup script against the already prepared P9 Build artifact:

```text
tools\test\setup-test-runner.ps1 -BuildPath tmp\p9-success\Build.xlam
```

The later preparation task may use PowerShell execution-policy wrapping needed
to run that exact script locally, but it must preserve the same script and
`BuildPath` value.

The later preparation task must not copy a pre-existing
`VMFTestRunner.xlam`, use a fallback runner workbook, use a fallback
`Build.xlam`, use `dist\release\Build\v1.1\Build.xlam`, use an externally
supplied workbook, repair or replace the P9 fixture, or broaden into full
regression.

## Verification Boundary For Later Preparation

The later preparation task is limited to proving the local test runner
artifact precondition for the focused P9 retry:

- recheck the P9 fixture path, length, SHA-256, and fixture count before setup;
- recheck `tmp\p9-success\Build.xlam` length and SHA-256 before setup;
- execute only
  `tools\test\setup-test-runner.ps1 -BuildPath tmp\p9-success\Build.xlam`;
- verify `tools\test\runner\VMFTestRunner.xlam` exists after setup;
- record its length and SHA-256 as local evidence;
- run `git diff --check`;
- confirm Git status.

P9-45 does not authorize running:

```text
tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam
```

That remains a separate follow-up after the local test runner artifact exists
and has been recorded.

## Cleanup And Retention Boundary

The later preparation task may retain only the local test runner artifact at:

```text
tools\test\runner\VMFTestRunner.xlam
```

The artifact is temporary local verification evidence only. It is not a
release artifact, package artifact, `dist` artifact, publication artifact, or
replacement for the committed runner source `tools\test\runner\VMFTestRunner.bas`.

The later preparation task must not stage, commit, push, tag, publish, or
package the generated runner artifact. It must not delete, overwrite, repair,
or replace the P9 fixture.

## Safety Stops For Later Preparation

The later preparation task must stop if any of the following is true:

- the P9 fixture is missing, duplicated, or no longer matches length `3532`
  bytes and SHA-256
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- `tmp\p9-success\Build.xlam` is missing or no longer matches length `515199`
  bytes and SHA-256
  `71C4D9C3AD5D6F65607301468BE68B2676024A74640E5E759BA9DFE53C8195A4`;
- the setup would require copying a runner workbook, using a fallback
  `Build.xlam`, using `dist\release\Build\v1.1\Build.xlam`, using an external
  workbook, or using any source other than the established setup script and
  prepared P9 Build artifact;
- Excel automation cannot create the local test runner artifact;
- the operation would require running the P9 retry command, opening the P9
  fixture, running full regression, mutating any workbook, mutating VBProject,
  Save, SaveAs, fixture repair, fixture conversion, fixture replacement,
  package / `dist`, release / publication, external service operation, public
  API change, persisted schema change, canonical format change, or Frozen
  specification change.

## Decision

Decision: `GO` for recording P9-45 as a docs-only test runner artifact
preparation GO / NO-GO decision.

Decision: `GO` for a later separate test runner artifact preparation task
limited to:

```text
tools\test\setup-test-runner.ps1 -BuildPath tmp\p9-success\Build.xlam
```

Decision: `NO-GO` for creating or copying `VMFTestRunner.xlam` during P9-45.

Decision: `NO-GO` for retrying the P9-39 / P9-43 success-path evidence
command during P9-45.

Decision: `NO-GO` for claiming successful Excel read-only open, identity
reconfirmation, close without saving, or post-close unchanged-fixture
confirmation during P9-45.

Decision: `NO-GO` for full regression unless separately authorized.

Decision: `NO-GO` for fixture repair, fixture replacement, fixture creation,
fixture mutation, workbook mutation, VBProject mutation, writable lifecycle
operations, Save, SaveAs, code injection outside the established setup script,
module import / export outside the established setup script, package / `dist`,
release / publication, external service operation, staging, commit, push,
public API change, persisted schema change, canonical format change, or Frozen
specification change.

## Selected Next Candidate

Selected next candidate:

**P9-46 - Read-Only Lifecycle Test Runner Artifact Preparation**

P9-46 may execute only the P9-45 approved test runner artifact preparation
slice. It must not run the P9-39 / P9-43 success-path evidence retry unless a
later separate decision authorizes retry after `VMFTestRunner.xlam` exists.

## Verification

P9-45 verification is documentation-only:

- reviewed P9-43 and P9-44 records;
- reviewed the established `tools\test\setup-test-runner.ps1` setup script;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- confirmed `tmp\p9-success\Build.xlam` exists at the retry command path;
- recorded the prepared Build artifact length and SHA-256;
- confirmed `tools\test\runner\VMFTestRunner.xlam` is still absent before
  P9-45 test runner artifact preparation;
- recorded a GO decision only for a later separate test runner artifact
  preparation task;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, and Git status confirmation.

No implementation tests are required or run for P9-45. No workbook, Excel, or
VBProject operation is required or run for P9-45.
