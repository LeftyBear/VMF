# P7-31 - Rollback Removal Failure Candidate Fix

## Status

COMPLETE / docs-only rollback-removal failure candidate fix

## Purpose

Fix the remaining P7-11-K deferred item as the next minimum implementation
candidate after P7-29 / P7-30 completed P7-11-H mutation sequencing failure
rollback coverage.

P7-31 is documentation only. It does not grant implementation GO, does not
change production code or test code, does not open / save / close / SaveAs /
restore any workbook, does not mutate any workbook or VBProject, does not
create or modify workbook fixtures, does not update package or `dist`
artifacts, does not perform release or publication work, does not access
external services, and does not change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Starting State

- P7-11 defined P7-11-K as rollback failure when rollback cannot remove one or
  more components created by the current operation.
- P7-25 implemented controlled readback missing-component and mismatched-source
  failures that trigger the existing rollback path.
- P7-29 implemented controlled later component-creation failure after mutation
  starts and verified ordinary rollback of current-operation components.
- P7-30 closed out P7-29 and kept P7-11-K as the only remaining P7-11 deferred
  item.
- This P7-31 task is explicitly docs-only and provides no implementation,
  test change, workbook operation, or VBProject mutation authorization.

## Existing Rollback Path

Current repository evidence shows this real VBProject mutation sequence in
`AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`:

1. validate the output write plan and explicit target VBProject;
2. preflight target component access and requested module conflicts;
3. apply create-only mutation through `ApplyRealVBProjectMutation`;
4. optionally apply controlled readback fault injection;
5. verify readback for every requested component;
6. on any post-preflight error after `CreatedComponents` is available, call
   `RollbackCreatedComponents TargetVBProject, CreatedComponents`;
7. return `Success = False`, `Classification = HardStop`, and no partial
   mutation count.

`CreatedComponents` is the current-operation rollback scope. It is populated
only with components created by the operation. Existing P7-25 and P7-29 tests
verify that normal rollback removes current-operation components and preserves
unrelated pre-existing components.

The current `RollbackCreatedComponents` implementation suppresses removal
errors with `On Error Resume Next`. That behavior is acceptable for the
ordinary rollback coverage already closed out, but it does not satisfy
P7-11-K because it cannot report an incomplete rollback as failure evidence.

## Controlled Rollback Failure Injection

The minimum later implementation candidate should add controlled rollback
removal failure injection at the rollback boundary only.

Candidate injection properties:

- it is test-controlled and explicit, matching the existing
  `controlledCreationFault` and `controlledReadbackFault` pattern;
- it is consumed only after mutation has started and rollback is already
  required by an earlier controlled failure;
- it targets removal of one or more components in `CreatedComponents`;
- it must not create a new production workbook discovery path, implicit
  VBProject selection path, overwrite operation, delete operation, rename
  operation, import operation, export operation, or arbitrary component
  mutation operation;
- unsupported rollback fault values must fail closed.

The candidate trigger should reuse an already established rollback trigger,
preferably the P7-29 controlled later component-creation failure path, because
that path proves at least one current-operation component was created before
rollback begins. A readback-triggered rollback may be covered later, but it is
not required for the minimum P7-11-K slice.

## Failure-State Confirmation

The minimum later implementation candidate must verify the state after
rollback removal failure:

- the operation returns `Success = False`;
- classification remains `HardStop`;
- no partial success is reported;
- `MutatedModules = 0` remains the result-level mutation count;
- at least one current-operation component remains because rollback removal
  failed;
- unrelated pre-existing components remain present and unchanged;
- the result message identifies rollback failure or incomplete rollback without
  implying that the target is clean;
- the test does not rely on production workbook state and uses only a local
  test-owned workbook fixture when a later implementation GO authorizes fixture
  use.

An incomplete rollback leaves the target requiring operator review before any
later retry. It must not be reported as safe cleanup, successful mutation,
successful readback, or a retry-ready state.

## Safe-Stop / Readback Boundary

P7-11-K sits after mutation starts and after rollback is triggered. It must
preserve these boundaries:

- preflight failures still stop before mutation and require no rollback;
- successful mutation still requires complete readback before success;
- readback failure still denies success and enters rollback for
  current-operation components;
- rollback failure is a distinct hard-stop after rollback starts;
- rollback failure reporting must not change the readback success criteria;
- the implementation must not perform compensating readback, workbook restore,
  SaveAs, fixture replacement, package update, release work, external service
  work, or cleanup outside the current-operation created components.

If rollback removal failure prevents proving a clean target state, the safe
state is failed / operator-review-required, not successful / cleaned.

## Fixed Minimum Implementation Candidate

Decision: fix P7-11-K rollback-removal failure as the next minimum later
implementation candidate.

The candidate is limited to:

- candidate production file:
  `src/Build/Application/AppOutputWriteService.cls`;
- candidate test file:
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas`;
- candidate entry boundary:
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`;
- candidate behavior:
  controlled rollback-removal failure after a rollback-triggering
  post-preflight failure, incomplete rollback hard-stop reporting, no partial
  success, and explicit failure-state confirmation;
- candidate existing path:
  reuse the existing `CreatedComponents` rollback scope and the established
  controlled fault-injection style;
- candidate non-scope:
  no broader workbook lifecycle, restore, save, overwrite, delete, rename,
  import, export, production workbook mutation, package / `dist`, release,
  publication, external service, public API, persisted schema, canonical
  format, or Frozen specification change.

This candidate is fixed for a later GO / NO-GO task only. It is not
implementation authorization.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-31 as docs-only rollback-removal failure
candidate fixing.

Decision: `GO` for fixing P7-11-K as the next minimum later implementation
candidate.

Decision: `NO-GO` for implementation in P7-31.

Decision: `NO-GO` for production code or test code changes in P7-31.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-31.

Decision: `NO-GO` for rollback-removal failure injection implementation in
P7-31.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 / P7-21 /
P7-25 / P7-29 create-only missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-31:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- rollback-removal failure injection implementation;
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

P7-31 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-27 remaining mutation sequencing / rollback candidate selection;
- reviewed P7-30 mutation sequencing failure implementation closeout;
- reviewed current repository evidence for
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`,
  `ApplyRealVBProjectMutation`, `ApplyControlledCreationFault`,
  `ApplyControlledReadbackFault`, `VerifyRealVBProjectReadback`, and
  `RollbackCreatedComponents`;
- confirmed P7-11-K is the only remaining P7-11 deferred item;
- fixed P7-11-K as the next minimum later implementation candidate;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
