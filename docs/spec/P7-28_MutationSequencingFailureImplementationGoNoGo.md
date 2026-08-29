# P7-28 - Mutation Sequencing Failure Implementation GO / NO-GO

## Status

COMPLETE / docs-only implementation GO / NO-GO decision

## Purpose

Apply the P7-27 selection of P7-11-H and decide whether mutation sequencing
failure coverage can start later as the next minimum post-preflight rollback
implementation slice.

P7-28 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-11 defined P7-11-H as component creation failure after preflight and
  after at least one current-operation component was created.
- P7-21 implemented P7-11-G target VBProject component access failure as a
  pre-mutation hard stop.
- P7-25 implemented P7-11-I/J readback failure rollback coverage after
  successful create-only mutation.
- P7-26 closed out P7-25 and kept P7-11-H/K deferred.
- P7-27 selected P7-11-H as the next smallest later candidate and kept
  P7-11-K rollback failure deferred.
- This P7-28 task is explicitly docs-only and performs no implementation,
  test change, workbook operation, or VBProject mutation.

## Existing Boundary Evidence

The current repository evidence supports a narrow later implementation start:

- `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject` validates the
  output write plan and target VBProject, completes
  `PreflightRealVBProjectMutation`, calls `ApplyRealVBProjectMutation`, then
  calls readback verification before returning success.
- `ApplyRealVBProjectMutation` performs the create-only sequence and records
  each component created by the current operation in `CreatedComponents`.
- The shared error handler calls `RollbackCreatedComponents` when
  `CreatedComponents` exists, then returns a failed `HardStop` mutation result.
- `RollbackCreatedComponents` removes only the component objects captured from
  the current operation.
- P7-25 already proves the existing rollback path can remove current-operation
  components after a post-mutation failure and preserve unrelated pre-existing
  components.

This evidence means P7-11-H can be covered as post-preflight mutation
sequencing failure behavior while preserving the dependency boundary:

1. preflight succeeds;
2. at least one create-only missing-module mutation succeeds;
3. a controlled later component-creation failure is observed;
4. success is denied;
5. the existing rollback path is used only for components created by the
   current operation.

## Selected Implementation Slice

Decision: P7-11-H is `GO` for a later separate implementation-start task as
the next minimum mutation sequencing failure rollback coverage slice.

The later implementation slice is limited to:

- controlled component-creation failure during the post-preflight create-only
  mutation sequence;
- failure only after at least one current-operation component was created;
- failure result with `Success = False`;
- no partial success;
- rollback through the existing `CreatedComponents` / `RollbackCreatedComponents`
  path;
- rollback attempted only for components created by the current operation;
- unrelated pre-existing components preserved;
- P7-11-K rollback-removal failure remaining deferred.

The later implementation task must name exact editable files, the
fault-injection mechanism used to produce controlled component-creation failure
during mutation sequencing, and the verification commands before any
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
- candidate focused verification:
  existing Build focused test command for `AppOutputWriteBoundaryTests`;
- diff verification:
  `git diff --check`.

No other production file, test file, workbook fixture path, Template,
GenerateContext, Generator, specification, package, `dist` artifact, release
record, external service, or production workbook operation is authorized by
P7-28.

## Preserved Boundary

The later implementation slice must preserve the P7-07 / P7-13 / P7-17 /
P7-21 / P7-25 create-only missing-module boundary:

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

P7-28 does not authorize rollback fault injection, readback failure expansion,
workbook restore behavior, SaveAs behavior, overwrite / delete / rename /
import / export behavior, or any mutation operation beyond create-only missing
standard / class modules.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-28 as docs-only implementation GO / NO-GO
decision.

Decision: `GO` for a later separate implementation-start task limited to
P7-11-H mutation sequencing failure rollback coverage.

Decision: `NO-GO` for implementation in P7-28.

Decision: `NO-GO` for production code or test code changes in P7-28.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-28.

Decision: `NO-GO` for rollback fault injection or P7-11-K incomplete rollback
failure implementation in P7-28.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 / P7-21 /
P7-25 create-only missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-28:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- overwrite, delete, rename, import, export, or arbitrary component creation;
- mutation of production workbooks or real user data;
- rollback fault injection implementation;
- incomplete rollback failure implementation;
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

P7-28 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-24 readback failure coverage implementation GO / NO-GO;
- reviewed P7-27 remaining mutation sequencing / rollback candidate
  selection;
- reviewed current repository evidence for
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`,
  `ApplyRealVBProjectMutation`, and `RollbackCreatedComponents`;
- determined P7-11-H can start later as the minimum mutation sequencing
  failure rollback coverage slice;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
