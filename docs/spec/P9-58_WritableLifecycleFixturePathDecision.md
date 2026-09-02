# P9-58 - Writable Lifecycle Fixture Path Decision

## Status

COMPLETE / docs-only writable lifecycle fixture path decision

## Purpose

Decide the next minimum fixture path after the P9-56 writable lifecycle
attempt failed at workbook open and P9-57 preserved that result as failed-open
safety evidence only.

P9-58 is documentation only. It does not run Excel automation, open / create /
save / SaveAs / close / discard / restore any workbook, mutate or repair any
fixture, replace or recreate any fixture, mutate any workbook or VBProject,
inject code, import or export modules, change implementation or test code, run
implementation tests, update package or `dist` release artifacts, perform
release or publication work, access external services, stage, commit, push, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Reviewed Evidence

P9-58 reviewed the current writable lifecycle evidence chain:

- `docs/spec/P9-51_ExistingWorkbookWritableLifecycleAuthorizationBoundary.md`;
- `docs/spec/P9-54_WritableLifecycleAuthorizationInputs.md`;
- `docs/spec/P9-55_ExistingWorkbookWritableLifecycleEvidenceGoNoGo.md`;
- `docs/spec/P9-56_ExistingWorkbookWritableLifecycleEvidenceExecution.md`;
- `docs/spec/P9-57_WritableLifecycleFailedOpenResultReview.md`;
- current backlog, current-status, and handoff records.

Current fixture identity was rechecked during P9-58 without opening the
workbook:

- path: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- length: `3532` bytes;
- SHA-256:
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- fixture count under `tests\fixtures\workbooks`: exactly `1`.

## Fixture Path Assessment

P9-56 attempted the authorized writable lifecycle operation against the exact
P9 fixture with `UpdateLinks = 0`, `ReadOnly = False`, and `AddToMru = False`.
The attempt and corrected COM optional-argument retry both failed at
`Workbooks.Open`. P9-57 records that result as useful failed-open safety
evidence, not writable lifecycle success-path evidence.

The current fixture remains valid as historical fixture identity evidence and
read-only lifecycle evidence input. It must not be repaired, resaved,
converted, replaced in place, or used as the basis for an implicit writable
retry. Mutating the same file would destroy the stable identity chain used by
P9-33 through P9-57 and would convert a failed-open hard stop into an
unreviewed fixture operation.

The minimum safe path is a new, explicit, test-owned replacement fixture path
for future writable lifecycle success-path evidence. That path must be
authorized separately by the owner before any file creation, import,
replacement, conversion, Excel automation, writable open, or test update. The
current P9 fixture should remain unchanged unless a later explicit owner
authorization says otherwise.

## Decision

Decision: `GO` for recording P9-58 as a docs-only writable lifecycle fixture
path decision.

Decision: `PASS` for current unchanged-fixture identity confirmation during
P9-58.

Decision: `NO-GO` for claiming writable lifecycle success-path evidence from
P9-56 or P9-57.

Decision: `NO-GO` for repairing, replacing in place, recreating, converting,
resaving, or otherwise mutating
`tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` from P9-58.

Decision: `GO` only for a later separate docs-only owner-authorization-input
record that defines whether to introduce a new explicit replacement fixture
path for writable lifecycle success-path evidence.

Decision: `NO-GO` for Excel automation, workbook open, Save, SaveAs, fixture
creation, fixture mutation, fixture repair, fixture replacement, fixture
recreation, workbook / VBProject mutation, implementation change, test code
change, package / `dist`, release / publication, external service operation,
staging, commit, push, public API change, persisted schema change, canonical
format change, or Frozen specification change from P9-58.

## Selected Next Candidate

Selected next candidate:

**P9-59 - Replacement Writable Fixture Authorization Inputs**

P9-59 should remain docs-only unless separately authorized otherwise. It should
record the complete owner inputs required before any later replacement fixture
path can be created, selected, opened, or used for writable lifecycle
success-path evidence.

P9-59 must not infer authorization for Excel automation, workbook open, Save,
SaveAs, fixture creation, fixture mutation, fixture repair, fixture
replacement, fixture recreation, workbook / VBProject mutation, implementation
start, test code change, package / `dist`, release / publication, external
services, staging, commit, push, public API changes, persisted schema changes,
canonical format changes, or Frozen specification changes.

## Preserved Invariants

P9-58 preserves:

- current P9 fixture identity and no fallback workbook selection;
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

P9-58 verification is documentation-only:

- reviewed the P9 writable lifecycle evidence chain through P9-57;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- confirmed exactly one workbook fixture under `tests\fixtures\workbooks`;
- reviewed backlog, current-status, and handoff state;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, and Git status confirmation.

No implementation tests are required or run for P9-58. No workbook operation is
performed by P9-58.
