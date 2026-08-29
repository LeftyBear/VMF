# P7-24 - Readback Failure Coverage Implementation GO / NO-GO

## Status

COMPLETE / docs-only implementation GO / NO-GO decision

## Purpose

Apply the P7-23 selection of P7-11-I and P7-11-J and decide whether readback
failure coverage can start later as the next minimum post-mutation rollback
implementation slice.

P7-24 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-11 defined P7-11-I as readback missing a component created by the current
  operation and P7-11-J as readback returning mismatched content or the wrong
  supported module kind.
- P7-12 deferred P7-11-E through P7-11-K from the first minimum slice.
- P7-13 implemented P7-11-A/B/C/D/L.
- P7-17 implemented P7-11-E/F pre-mutation invalid write-unit coverage.
- P7-21 implemented P7-11-G target VBProject component access failure
  pre-mutation coverage.
- P7-22 closed out P7-21 and kept P7-11-H/I/J/K deferred.
- P7-23 selected P7-11-I/J as the next smallest later candidate and kept
  P7-11-H/K deferred.
- This P7-24 task is explicitly docs-only and performs no implementation,
  test change, workbook operation, or VBProject mutation.

## Existing Boundary Evidence

The current repository evidence supports a narrow later implementation start:

- `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject` validates the
  output write plan and target VBProject, completes
  `PreflightRealVBProjectMutation`, calls `ApplyRealVBProjectMutation`, then
  calls `VerifyRealVBProjectReadback` before returning success.
- `ApplyRealVBProjectMutation` records the components created by the current
  operation in `CreatedComponents`.
- `VerifyRealVBProjectReadback` reads each requested component by module name,
  checks the supported component kind, and checks that the read-back source
  contains the expected generated source.
- The shared error handler calls `RollbackCreatedComponents` when
  `CreatedComponents` exists, then returns a failed `HardStop` mutation result.
- `RollbackCreatedComponents` removes only the component objects captured from
  the current operation.
- Existing focused tests already cover successful readback, pre-mutation hard
  stops, component-access hard stops, and unrelated existing-component
  preservation.

This evidence means P7-11-I/J can be covered as post-mutation readback failure
behavior while preserving the dependency boundary:

1. create-only missing-module mutation succeeds;
2. controlled readback failure is observed;
3. success is denied;
4. rollback is attempted only for components created by the current operation.

## Selected Implementation Slice

Decision: P7-11-I and P7-11-J are `GO` for a later separate implementation
start as the next minimum readback-failure rollback coverage slice.

The later implementation slice is limited to:

- controlled readback missing a current-operation component after successful
  create-only mutation;
- controlled readback returning mismatched generated source after successful
  create-only mutation;
- controlled readback returning the wrong supported component kind after
  successful create-only mutation, only if this can be produced without
  expanding the mutation operation set;
- failure result with `Success = False`;
- no partial success;
- rollback attempted only for components created by the current operation;
- unrelated pre-existing components preserved.

The later implementation task must name exact editable files, the fixture or
adapter mechanism used to produce controlled readback failure after mutation,
and the verification commands before any production or test code changes
begin.

## Candidate Editable Scope For Later GO

If separately authorized, the later implementation-start task should remain
limited to:

- production file:
  `src/Build/Application/AppOutputWriteService.cls`;
- test file:
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas`;
- entry boundary:
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`;
- candidate focused verification:
  existing Build focused test command for `AppOutputWriteBoundaryTests`;
- diff verification:
  `git diff --check`.

No other production file, test file, workbook fixture path, Template,
GenerateContext, Generator, specification, package, `dist` artifact, release
record, external service, or production workbook operation is authorized by
P7-24.

## Preserved Boundary

The later implementation slice must preserve the P7-07 / P7-13 / P7-17 /
P7-21 create-only missing-module boundary:

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

P7-24 does not authorize rollback fault injection, component creation failure
injection, workbook restore behavior, SaveAs behavior, overwrite / delete /
rename / import / export behavior, or any mutation operation beyond create-only
missing standard / class modules.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-24 as docs-only implementation GO / NO-GO
decision.

Decision: `GO` for a later separate implementation-start task limited to
P7-11-I/J readback failure rollback coverage.

Decision: `NO-GO` for implementation in P7-24.

Decision: `NO-GO` for production code or test code changes in P7-24.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-24.

Decision: `NO-GO` for rollback fault injection or post-preflight
mutation-failure implementation in P7-24.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 / P7-21
create-only missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-24:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- overwrite, delete, rename, import, export, or arbitrary component creation;
- mutation of production workbooks or real user data;
- rollback fault injection implementation;
- post-preflight mutation-failure implementation;
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

P7-24 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-16 pre-mutation failure coverage implementation GO / NO-GO;
- reviewed P7-20 target component access failure implementation GO / NO-GO;
- reviewed P7-22 target component access failure implementation closeout;
- reviewed P7-23 readback failure / rollback dependency candidate selection;
- reviewed current repository evidence for
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`,
  `ApplyRealVBProjectMutation`, `VerifyRealVBProjectReadback`, and
  `RollbackCreatedComponents`;
- determined P7-11-I/J can start later as the minimum readback-failure
  rollback coverage slice;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
