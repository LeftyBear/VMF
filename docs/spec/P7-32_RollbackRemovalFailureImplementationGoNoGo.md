# P7-32 - Rollback Removal Failure Implementation GO / NO-GO

## Status

COMPLETE / docs-only implementation GO / NO-GO decision

## Purpose

Apply the P7-31 fixed selection of P7-11-K and decide whether rollback-removal
failure coverage can start later as the next minimum post-rollback failure
implementation slice.

P7-32 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-11 defined P7-11-K as rollback failure when rollback cannot remove one or
  more components created by the current operation.
- P7-25 implemented readback failure rollback coverage.
- P7-29 implemented mutation sequencing failure rollback coverage after at
  least one current-operation component is created.
- P7-30 closed out P7-29 and kept P7-11-K as the only remaining P7-11
  deferred item.
- P7-31 fixed P7-11-K as the next minimum later implementation candidate and
  identified controlled rollback-removal failure injection plus incomplete
  rollback evidence reporting as the needed scope.
- This P7-32 task is explicitly docs-only and performs no implementation,
  test change, workbook operation, or VBProject mutation.

## Existing Boundary Evidence

Current repository evidence supports a narrow later implementation start:

- `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject` completes
  preflight, calls `ApplyRealVBProjectMutation`, applies controlled readback
  fault injection, verifies readback, and returns success only after readback
  passes.
- `ApplyRealVBProjectMutation` creates only requested missing supported
  components and records each current-operation component in
  `CreatedComponents`.
- The shared error handler calls `RollbackCreatedComponents` when
  `CreatedComponents` exists and then returns a failed `HardStop` mutation
  result.
- `RollbackCreatedComponents` currently removes the captured current-operation
  components while suppressing removal errors.
- P7-25 and P7-29 already prove ordinary rollback can remove
  current-operation components and preserve unrelated pre-existing components.

This evidence means P7-11-K can be covered as a failure path after rollback is
already required, without changing the create-only missing-module operation set.

## Selected Implementation Slice

Decision: P7-11-K is `GO` for a later separate implementation-start task as
the next minimum rollback-removal failure coverage slice.

The later implementation slice is limited to:

- controlled rollback-removal failure injection at the rollback boundary;
- failure only after mutation has started and rollback has already been
  triggered by a post-preflight failure;
- reuse of an established rollback trigger, preferably the P7-29 controlled
  later component-creation failure path;
- incomplete rollback evidence reporting when one or more current-operation
  components cannot be removed;
- failure result with `Success = False`;
- classification remaining `HardStop`;
- no partial success;
- `MutatedModules = 0` in the failed result;
- confirmation that at least one current-operation component remains when
  rollback removal is injected to fail;
- confirmation that unrelated pre-existing components remain present and
  unchanged.

The later implementation task must name exact editable files, the
fault-injection key/value used to produce controlled rollback-removal failure,
the expected result message wording, and the verification commands before any
production or test code changes begin.

## Candidate Editable Scope For Later GO

If separately authorized, the later implementation-start task should remain
limited to:

- production file:
  `src/Build/Application/AppOutputWriteService.cls`;
- test file:
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas`;
- entry boundary:
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`;
- rollback boundary:
  `RollbackCreatedComponents`;
- candidate focused verification:
  existing Build focused test command for `AppOutputWriteBoundaryTests`;
- diff verification:
  `git diff --check`.

No other production file, test file, workbook fixture path, Template,
GenerateContext, Generator, specification, package, `dist` artifact, release
record, external service, or production workbook operation is authorized by
P7-32.

## Preserved Boundary

The later implementation slice must preserve the P7-07 / P7-13 / P7-17 /
P7-21 / P7-25 / P7-29 create-only missing-module boundary:

- consume only an already successful output write plan;
- require an explicitly supplied target VBProject;
- complete trust/access and component preflight before mutation;
- permit only create-only missing-module mutation for supported standard and
  class modules;
- hard-stop before mutation for invalid input, inaccessible target state, or
  requested existing modules;
- require readback verification before any success result;
- roll back only components created by the current operation after a
  post-preflight mutation or readback failure;
- report no partial success.

Rollback-removal failure must be reported as failed / operator-review-required.
It must not be reported as successful cleanup, successful mutation, successful
readback, a safe retry-ready state, or a workbook restore operation.

P7-32 does not authorize readback failure expansion, workbook restore behavior,
SaveAs behavior, overwrite / delete / rename / import / export behavior, or any
mutation operation beyond create-only missing standard / class modules.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-32 as docs-only implementation GO / NO-GO
decision.

Decision: `GO` for a later separate implementation-start task limited to
P7-11-K rollback-removal failure coverage.

Decision: `NO-GO` for implementation in P7-32.

Decision: `NO-GO` for production code or test code changes in P7-32.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-32.

Decision: `NO-GO` for rollback-removal failure injection implementation in
P7-32.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 / P7-21 /
P7-25 / P7-29 create-only missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-32:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- rollback-removal failure injection implementation;
- incomplete rollback evidence reporting implementation;
- overwrite, delete, rename, import, export, or arbitrary component mutation;
- mutation of production workbooks or real user data;
- fallback Template selection;
- implicit Template selection;
- Template content inference;
- GenerateContext or Generator compensation;
- package / `dist` creation, update, replacement, or inspection beyond
  existing repository evidence;
- release, tag, push, or publication operations;
- external service operations;
- credential or token-store access;
- public API changes;
- persisted schema changes;
- canonical format changes;
- Frozen specification changes.

## Verification Performed

P7-32 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-28 mutation sequencing failure implementation GO / NO-GO;
- reviewed P7-31 rollback-removal failure candidate fix;
- reviewed current repository evidence for
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`,
  `ApplyRealVBProjectMutation`, `ApplyControlledCreationFault`,
  `ApplyControlledReadbackFault`, `VerifyRealVBProjectReadback`, and
  `RollbackCreatedComponents`;
- determined P7-11-K can start later as the minimum rollback-removal failure
  coverage slice;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
