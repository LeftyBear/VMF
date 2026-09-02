# P9-59 - Replacement Writable Fixture Authorization Inputs

## Status

COMPLETE / docs-only replacement writable fixture authorization input waiting state

## Purpose

Record the owner authorization inputs required before any later replacement
writable fixture path can be created, selected, opened, or used for writable
lifecycle success-path evidence after P9-58.

P9-59 is documentation only. It does not run Excel automation, open / create /
save / SaveAs / close / discard / restore any workbook, mutate or repair any
fixture, replace or recreate any fixture, mutate any workbook or VBProject,
inject code, import or export modules, change implementation or test code, run
implementation tests, update package or `dist` release artifacts, perform
release or publication work, access external services, stage, commit, push, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Reviewed Evidence

P9-59 reviewed the current writable lifecycle evidence chain:

- `docs/spec/P9-51_ExistingWorkbookWritableLifecycleAuthorizationBoundary.md`;
- `docs/spec/P9-54_WritableLifecycleAuthorizationInputs.md`;
- `docs/spec/P9-55_ExistingWorkbookWritableLifecycleEvidenceGoNoGo.md`;
- `docs/spec/P9-56_ExistingWorkbookWritableLifecycleEvidenceExecution.md`;
- `docs/spec/P9-57_WritableLifecycleFailedOpenResultReview.md`;
- `docs/spec/P9-58_WritableLifecycleFixturePathDecision.md`;
- current backlog, current-status, and handoff records.

Current fixture identity was rechecked during P9-59 without opening the
workbook:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

## Required Owner Authorization Inputs

P9-58 selected only a later docs-only owner-authorization-input record for a
new explicit replacement fixture path. P9-59 records that the following inputs
remain required before any later fixture operation or writable lifecycle
success-path evidence attempt:

- explicit owner authorization to introduce a replacement writable fixture
  path;
- exact replacement fixture path under the test fixture area;
- expected source of the replacement fixture, such as owner-supplied file,
  newly created test-owned workbook, or other explicitly approved source;
- expected replacement fixture file identity after creation or placement,
  including path, length, SHA-256, and fixture-count expectations;
- explicit confirmation that
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` remains immutable
  historical / read-only evidence input and is not repaired, resaved,
  converted, replaced in place, or used as an implicit writable retry target;
- explicit no-fallback selection rule for any future replacement fixture;
- permitted creation or placement operation, if any, and its verification
  command;
- permitted future writable-open arguments and lifecycle flags, if any;
- dirty-state observation and hard-stop policy for the replacement fixture;
- close-without-saving and no-save identity verification expectations;
- failed-open, failed-close, unexpected dirty-state, and residual-process
  handling;
- textual / log evidence retention requirements;
- operator-review acceptance criteria;
- separate execution authorization for any later fixture creation, Excel
  automation, workbook open, writable lifecycle evidence run, implementation
  change, or test code change.

The current task input supplies only the P9-59 title and does not supply a
replacement fixture path or any complete owner input set. P9-59 therefore
records the replacement fixture authorization inputs as `PENDING`.

## Decision

Decision: `GO` for recording P9-59 as a docs-only replacement writable fixture
authorization input waiting-state record.

Decision: `PASS` for current unchanged-fixture identity confirmation during
P9-59.

Decision: `PENDING` for replacement writable fixture owner authorization
inputs.

Decision: `NO-GO` for creating, copying, importing, selecting, opening, or
using any replacement writable fixture from P9-59.

Decision: `NO-GO` for repairing, replacing in place, recreating, converting,
resaving, or otherwise mutating
`tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` from P9-59.

Decision: `NO-GO` for claiming writable lifecycle success-path evidence from
P9-56, P9-57, P9-58, or P9-59.

Decision: `NO-GO` for Excel automation, workbook open, Save, SaveAs, fixture
creation, fixture mutation, fixture repair, fixture replacement, fixture
recreation, workbook / VBProject mutation, implementation change, test code
change, package / `dist`, release / publication, external service operation,
staging, commit, push, public API change, persisted schema change, canonical
format change, or Frozen specification change from P9-59.

## Selected Next Candidate

P9-59 intentionally selects no implementation or execution candidate. The next
candidate remains blocked until the repository owner supplies the complete
replacement writable fixture owner authorization input set or explicitly
selects a different docs-only planning task.

Any later continuation must remain docs-only unless separately authorized
otherwise. It must not infer authorization for Excel automation, workbook open,
Save, SaveAs, fixture creation, fixture mutation, fixture repair, fixture
replacement, fixture recreation, workbook / VBProject mutation,
implementation start, test code change, package / `dist`, release /
publication, external services, staging, commit, push, public API changes,
persisted schema changes, canonical format changes, or Frozen specification
changes.

## Preserved Invariants

P9-59 preserves:

- current P9 fixture identity and no fallback workbook selection;
- current P9 fixture as immutable historical / read-only evidence input;
- P9-56 as failed-open evidence only, not writable lifecycle success-path
  evidence;
- failed writable open as a hard stop;
- no mutation, repair, replacement, recreation, conversion, Save, or SaveAs of
  the current P9 fixture;
- mandatory separate owner authorization before any replacement fixture path,
  workbook operation, fixture operation, implementation change, or test code
  change;
- package / `dist`, release / publication, external service, public API,
  persisted schema, canonical format, and Frozen specification boundaries.

## Verification

P9-59 verification is documentation-only:

- reviewed the P9 writable lifecycle evidence chain through P9-58;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- reviewed backlog, current-status, and handoff state;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, and Git status confirmation.

No implementation tests are required or run for P9-59. No workbook operation is
performed by P9-59.
