# P7-20 - Target Component Access Failure Implementation GO / NO-GO

## Status

COMPLETE / docs-only implementation GO / NO-GO decision

## Purpose

Apply the P7-19 selection of P7-11-G and decide whether target VBProject
component access failure coverage can start later as the next minimum
pre-mutation hard-stop implementation slice.

P7-20 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-11 defined P7-11-G as target VBProject component access failure during
  preflight.
- P7-12 deferred P7-11-G through P7-11-K from the first minimum slice.
- P7-15 selected P7-11-E/F as the first deferred pre-mutation failure
  candidate and kept P7-11-G through P7-11-K deferred.
- P7-16 recorded GO for a later separate implementation-start task limited to
  P7-11-E/F.
- P7-17 implemented P7-11-E/F in
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas` only.
- P7-18 closed out P7-17 and kept P7-11-G through P7-11-K deferred.
- P7-19 selected P7-11-G as the next smallest later candidate because it is
  still pre-mutation and does not require successful mutation, readback fault
  injection, rollback execution, or rollback failure injection.
- This P7-20 task is explicitly docs-only and performs no implementation,
  test change, workbook operation, or VBProject mutation.

## Existing Boundary Evidence

The current repository evidence supports a narrow later implementation start:

- `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject` validates the
  output write plan, write units, and target VBProject before calling
  `PreflightRealVBProjectMutation`.
- `PreflightRealVBProjectMutation` obtains the target component collection via
  `RequireVBComponents` before calling any mutation routine.
- `RequireVBComponents` reads `TargetVBProject.VBComponents` and `Count`; a
  failure at that access point is raised before
  `ApplyRealVBProjectMutation`.
- `ApplyRealVBProjectMutation`, `VerifyRealVBProjectReadback`, and
  `RollbackCreatedComponents` occur after preflight and are not required to
  prove P7-11-G.
- Existing focused tests already exercise pre-mutation hard stops and no
  target component creation for duplicate names, existing target conflicts,
  unsupported module kinds, and missing / blank generated source.

This evidence means P7-11-G can be covered as a pre-mutation hard-stop
behavior without adding a new mutation operation, without readback fault
injection, and without rollback fault injection.

## Selected Implementation Slice

Decision: P7-11-G is `GO` for a later separate implementation start as the
next minimum pre-mutation hard-stop coverage slice.

The later implementation slice is limited to:

- controlled target VBProject component access failure during preflight;
- hard-stop before creating any target component;
- no rollback requirement because mutation must not start;
- no readback verification attempt as a success condition;
- no success result and no partial success;
- preservation of the P7-07 / P7-13 / P7-17 create-only missing-module
  boundary.

The later implementation task must name exact editable files, the fixture or
adapter mechanism used to produce controlled component-access failure, and the
verification commands before any production or test code changes begin.

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
record, external service, or workbook / VBProject mutation operation is
authorized by P7-20.

## Preserved Boundary

The later implementation slice must preserve the P7-07 / P7-13 / P7-17
create-only missing-module boundary:

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

P7-20 does not authorize readback fault injection, rollback fault injection,
component creation failure injection, workbook restore behavior, or any
mutation operation beyond create-only missing standard / class modules.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-20 as docs-only implementation GO / NO-GO
decision.

Decision: `GO` for a later separate implementation-start task limited to
P7-11-G target VBProject component access failure pre-mutation hard-stop
coverage.

Decision: `NO-GO` for implementation in P7-20.

Decision: `NO-GO` for production code or test code changes in P7-20.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-20.

Decision: `NO-GO` for readback fault injection, rollback fault injection, or
post-preflight mutation-failure implementation in P7-20.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 create-only
missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-20:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- overwrite, delete, rename, import, export, or arbitrary component creation;
- mutation of production workbooks or real user data;
- readback fault injection implementation;
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

P7-20 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-12 implementation slice selection;
- reviewed P7-16 pre-mutation failure coverage implementation GO / NO-GO;
- reviewed P7-18 implementation closeout;
- reviewed P7-19 remaining deferred candidate selection;
- reviewed current repository evidence for
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`,
  `PreflightRealVBProjectMutation`, `RequireVBComponents`,
  `ApplyRealVBProjectMutation`, `VerifyRealVBProjectReadback`, and
  `RollbackCreatedComponents`;
- determined P7-11-G can start later as the next minimum pre-mutation
  hard-stop coverage slice;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
