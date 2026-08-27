# P7-11 - Create-Only Missing-Module Focused Coverage Expansion Scope

## Status

COMPLETE / docs-only focused coverage expansion scope

## Purpose

Concrete the lowest-risk candidate identified by P7-10: preserve the
create-only missing-module real VBProject mutation boundary and broaden only
focused coverage for the completed P7-07 behavior.

P7-11 is documentation only. It does not grant implementation GO, does not
change production code or test code, does not open / save / close / SaveAs /
restore any workbook, does not mutate any real VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
does not change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Starting State

- P7-07 completed the minimum local-only real workbook / real VBProject
  mutation slice in commit `78d1ab2b456ffa9fd923d79aa481bac0c51ba065`.
- P7-08 closed out P7-07 docs-only and recorded the completed boundary.
- P7-09 selected P7-10 as the next docs-only planning candidate.
- P7-10 recorded Candidate A as the lowest-risk future candidate because it
  preserves create-only missing-module mutation and expands only focused
  coverage.
- Implementation GO has not been granted for P7-11.

## Preserved Mutation Boundary

The P7-11 candidate must preserve the P7-07 boundary exactly:

- consume only an already successful output write plan;
- require an explicitly supplied target VBProject;
- require local-only, test-owned focused verification;
- complete trust/access and component preflight before mutation;
- permit only create-only missing-module mutation for supported standard and
  class modules;
- treat any existing module as a conflict and hard-stop before mutation;
- require readback verification before success;
- roll back only components created by the current operation after a
  post-preflight mutation or readback failure;
- report no partial success.

## Focused Coverage Target Cases

| Case | Target condition | Expected result |
| --- | --- | --- |
| P7-11-A | Successful multi-module create-only apply with mixed supported standard and class modules | Success; all requested missing modules are created; mutated count equals requested unit count; readback confirms every module name, supported kind, and generated content |
| P7-11-B | Input write units arrive in a non-alphabetic order | Success; mutation and readback treat the complete set deterministically and do not depend on target enumeration order |
| P7-11-C | Duplicate module name appears in the same approved plan before mutation | Hard-stop before mutation; no target component is created; existing target state is unchanged |
| P7-11-D | Later write unit conflicts with an existing target component | Hard-stop before mutation; earlier missing modules are not created; the pre-existing component remains unchanged |
| P7-11-E | Unsupported module kind appears in an otherwise complete plan | Hard-stop before mutation; no target component is created |
| P7-11-F | Empty or missing generated source appears in an otherwise complete plan | Hard-stop before mutation; no target component is created |
| P7-11-G | Target VBProject component access fails during preflight | Hard-stop before mutation; no rollback is required because no component was created |
| P7-11-H | Component creation fails after preflight after at least one component was created by the current operation | Failure; rollback attempts to remove only components created by the current operation; success is not reported |
| P7-11-I | Readback misses a component created by the current operation | Failure; rollback attempts to remove created components; success is not reported |
| P7-11-J | Readback returns mismatched content or wrong supported module kind | Failure; rollback attempts to remove created components; success is not reported |
| P7-11-K | Rollback cannot remove one or more components created by the current operation | Failure; incomplete rollback is reported as failure; no partial success is reported |
| P7-11-L | Pre-existing component coexists with created components only when it is not part of the requested plan | Success only for requested missing modules; unrelated pre-existing components are preserved and not counted as mutations |

## Failure Conditions

The future focused implementation candidate must fail closed when any of the
following conditions is observed:

- missing or failed output write plan;
- missing, empty, duplicate, unsafe, unsupported, or incomplete write unit;
- implicit workbook discovery or implicit VBProject target selection;
- unavailable VBProject trust/access or component enumeration;
- existing target component for any requested module name;
- mutation request beyond create-only missing standard or class modules;
- readback missing, mismatched, incomplete, wrong-kind, or unavailable;
- rollback missing, incomplete, or unable to prove it touched only created
  components;
- request for overwrite, delete, rename, import, export, SaveAs, production
  workbook mutation, package / `dist`, release, publication, external service,
  credential, token-store, public API, persisted schema, canonical format, or
  Frozen specification change.

## Rollback Conditions

Rollback remains limited to the create-only operation:

- rollback is required only after mutation starts and the operation later
  fails during component creation or readback;
- rollback may remove only components created by the current operation;
- rollback must not remove, alter, rename, or rewrite pre-existing components;
- rollback failure is a failure result and cannot be converted to success;
- incomplete rollback leaves the operation in a failed state that requires
  operator review before any later retry.

Workbook restore is not in scope for P7-11. Any future workbook open / close,
save / restore, fixture replacement, or cleanup behavior requires a separate
authorization package before implementation GO.

## Readback Conditions

Readback is mandatory before any future success result:

- read back every requested module from the explicitly supplied target
  VBProject;
- confirm the read-back component name matches the requested module name;
- confirm the read-back module kind matches the requested supported module
  kind;
- confirm the read-back source text matches the expected generated source;
- confirm the complete requested set was verified, not only the first created
  module;
- treat any mismatch, missing component, wrong kind, empty source, or readback
  exception as failure requiring rollback for created components.

## Verification Conditions

P7-11 itself authorizes only documentation verification:

- `git diff --check`;
- docs-only diff confirmation.

If a later implementation GO is granted for this P7-11 candidate, the focused
verification candidate is:

- existing Build focused test command for `AppOutputWriteBoundaryTests`;
- existing Build VBA regression if named by the later implementation GO;
- `git diff --check`;
- docs-only confirmation that package / `dist`, release, publication, external
  service, Frozen specification, public API, persisted schema, and canonical
  format changes were not performed.

Full Build regression, Release build, format verification, and any workbook
fixture operation remain outside P7-11 unless a later implementation task
names and authorizes them.

## Candidate Implementation Scope For Later GO

P7-11 identifies only a candidate scope for a later implementation decision:

- candidate production file:
  `src/Build/Application/AppOutputWriteService.cls`;
- candidate test file:
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas`;
- candidate entry boundary:
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`;
- candidate behavior:
  focused coverage for existing create-only missing-module mutation,
  preflight, readback, and rollback behavior;
- candidate fixture surface:
  local test-owned fixture only, with exact path, lifetime, cleanup, and
  restore expectations to be named by a later implementation GO if fixture
  handling is required.

The later implementation candidate must not re-derive Blueprint, Manifest,
Template, GenerateContext, Generator, or Output Write facts. It must not expand
the mutation operation set.

## Prohibited Operations

The following remain NO-GO:

- implementation start by P7-11;
- production code changes by P7-11;
- test code changes by P7-11;
- workbook open, save, close, SaveAs, restore, or fixture mutation by P7-11;
- real VBProject mutation by P7-11;
- overwrite, delete, rename, import, export, or arbitrary component creation;
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

## GO / NO-GO Decisions

Decision: `GO` for recording P7-11 as docs-only focused coverage expansion
scope.

Decision: `NO-GO` for implementation in P7-11.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore in
P7-11.

Decision: `NO-GO` for real workbook mutation or real VBProject mutation in
P7-11.

Decision: `NO-GO` for expanding beyond the P7-07 create-only missing-module
operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Verification Performed

P7-11 verification is docs-only:

- reviewed P7-10 expansion scope planning;
- reviewed P7-05 authorization package and P7-08 implementation closeout;
- reviewed current repository evidence for
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject` and
  `AppOutputWriteBoundaryTests`;
- confirmed this request grants no implementation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
