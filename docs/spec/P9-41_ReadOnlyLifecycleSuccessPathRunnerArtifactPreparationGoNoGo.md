# P9-41 - Read-Only Lifecycle Success-Path Runner Artifact Preparation GO / NO-GO

## Status

COMPLETE / docs-only runner artifact preparation GO / NO-GO decision

## Purpose

Decide whether the missing runner artifact precondition from P9-40 can be
prepared in a later separate task.

P9-41 is documentation only. It does not create or copy `Build.xlam`, start
Excel automation, open / create / save / SaveAs / close / discard / restore
any workbook, mutate or replace the P9 fixture, repair or convert any
workbook, mutate any workbook or VBProject, inject code, import or export
modules outside the existing build script behavior, change implementation or
test code, run implementation tests, update package or `dist` artifacts,
perform release or publication work, access external services, or change
public APIs, persisted schemas, canonical formats, or Frozen specifications.

## Starting State

- P9-39 records GO for a later separate focused read-only lifecycle
  success-path evidence execution using:
  `tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam`.
- P9-40 executed that exact command and stopped before workbook open because
  `tmp\p9-success\Build.xlam` was missing.
- P9-40 records that the missing `Build.xlam` is a runner-artifact
  precondition failure, not a failed-open result for the P9 fixture.
- P9-40 selects P9-41 to decide the source, creation command, verification,
  cleanup, and retention boundary for preparing that exact artifact.

## Current Fixture And Artifact State

The current P9 fixture identity was rechecked during P9-41 without opening the
workbook:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

The missing runner artifact precondition is still current:

- path: `tmp\p9-success\Build.xlam`;
- exists before P9-41 artifact preparation: `No`;
- P9-41 did not create it.

## Preparation Source And Command

The only approved later source for `tmp\p9-success\Build.xlam` is the current
repository Build source through the existing local Build script:

```text
tools\build\build.ps1 -OutputPath tmp\p9-success\Build.xlam -BuildVersion p9-success-local -ReleaseType LocalVerification
```

The later preparation task may use PowerShell execution-policy wrapping needed
to run that exact script locally, but it must preserve the same script,
output path, build version, and release type values.

The later preparation task must not copy from `dist`, copy from an older
temporary Build artifact, use a fallback `Build.xlam`, use an externally
supplied workbook, or create / repair / replace the P9 fixture.

## Verification Boundary For Later Preparation

The later preparation task is limited to proving the artifact precondition for
the P9-39 command:

- verify `tmp\p9-success\Build.xlam` exists after the build command;
- record its length and SHA-256 as local evidence;
- recheck the P9 fixture path, length, SHA-256, and fixture count before any
  later success-path evidence execution;
- run `git diff --check`;
- confirm Git status.

P9-41 does not authorize running
`tools\test\run-tests.ps1 -BuildPath tmp\p9-success\Build.xlam`. That remains
a separate follow-up after the artifact exists.

## Cleanup And Retention Boundary

The later preparation task may retain only the temporary local artifact at:

```text
tmp\p9-success\Build.xlam
```

The artifact is temporary local verification evidence only. It is not a
release artifact, package artifact, `dist` artifact, publication artifact, or
replacement for the committed P9 fixture.

The later preparation task must not stage, commit, push, tag, publish, or
package the temporary artifact. It must not delete, overwrite, repair, or
replace the P9 fixture.

## Safety Stops For Later Preparation

The later preparation task must stop if any of the following is true:

- the P9 fixture is missing, duplicated, or no longer matches length `3532`
  bytes and SHA-256
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- preparing `tmp\p9-success\Build.xlam` would require copying from `dist`,
  using a fallback artifact, using an external workbook, or using any source
  other than current repository Build source through `tools\build\build.ps1`;
- the build requires changing implementation code, test code, public APIs,
  persisted schemas, canonical formats, Frozen specifications, package /
  `dist`, release artifacts, external services, or Git history;
- Excel automation cannot create the temporary Build artifact;
- the operation would require opening the P9 fixture, running the P9-39
  success-path evidence command, running full regression, mutating any
  workbook, mutating VBProject, Save, SaveAs, code injection, module import /
  export outside the existing Build script, fixture repair, fixture
  conversion, or fixture replacement.

## Decision

Decision: `GO` for recording P9-41 as a docs-only runner artifact preparation
GO / NO-GO decision.

Decision: `GO` for a later separate runner artifact preparation task limited
to creating `tmp\p9-success\Build.xlam` from current repository Build source
through:

```text
tools\build\build.ps1 -OutputPath tmp\p9-success\Build.xlam -BuildVersion p9-success-local -ReleaseType LocalVerification
```

Decision: `NO-GO` for creating or copying `tmp\p9-success\Build.xlam` during
P9-41.

Decision: `NO-GO` for retrying the P9-39 success-path evidence command during
P9-41.

Decision: `NO-GO` for fixture repair, fixture replacement, fixture creation,
fixture mutation, workbook mutation, VBProject mutation, writable lifecycle
operations, Save, SaveAs, full regression, package / `dist`, release /
publication, external service operation, public API change, persisted schema
change, canonical format change, or Frozen specification change.

## Selected Next Candidate

Selected next candidate:

**P9-42 - Read-Only Lifecycle Success-Path Runner Artifact Preparation**

P9-42 may execute only the P9-41 approved artifact preparation slice. It must
not run the P9-39 success-path evidence command unless a later separate
decision authorizes retry after the artifact exists.

## Verification

P9-41 verification is documentation-only:

- reviewed P9-38, P9-39, and P9-40 records;
- reviewed the existing build and test runner scripts;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- confirmed `tmp\p9-success\Build.xlam` is still absent before P9-41 artifact
  preparation;
- recorded a GO decision only for a later separate runner artifact preparation
  task;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, and Git status confirmation.

No implementation tests are required or run for P9-41. No workbook, Excel, or
VBProject operation is required or run for P9-41.
