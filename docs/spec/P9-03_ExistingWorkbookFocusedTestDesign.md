# P9-03 - Existing Workbook Focused Test Design

## Status

COMPLETE / docs-only existing workbook focused test design

## Purpose

Start from P8 COMPLETE, P9-01 actual workbook mutation expansion scope
planning, and P9-02 actual workbook identity authorization boundary, then fix
the focused test design for a later local-only existing-workbook mutation
implementation decision.

P9-03 is documentation only. It does not grant implementation GO, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, mutate any workbook or
VBProject, create or modify workbook fixtures, update package or `dist`
artifacts, perform release or publication work, access external services, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Starting State

P8 is COMPLETE only for the narrow local-only test-owned workbook /
create-only VBProject mutation flow.

P9-01 is COMPLETE and inventories actual workbook mutation expansion areas
without authorizing existing-workbook handling, production workbook handling,
Save / SaveAs / restore, destructive component operations, package / `dist`,
release / publication, external service, public API, persisted schema,
canonical format, or Frozen specification changes.

P9-02 is COMPLETE and fixes the required authorization inputs before any later
actual workbook mutation expansion can identify or operate on a workbook.
P9-02 selected P9-03 Existing Workbook Focused Test Design as the next minimum
docs-only candidate.

## Focused Test Target

A later implementation task may add focused local tests only after a separate
GO / NO-GO record names the exact editable files, exact test-owned workbook
identity, exact allowed lifecycle operations, expected VBProject posture, and
verification command.

The future focused test target must be:

- local-only;
- test-owned;
- a single exact existing workbook identity;
- selected without active-workbook state, recent files, name-only matching,
  directory scans, nearest matches, default fixture fallback, or production
  workbook fallback;
- isolated from package, `dist`, release, publication, and external service
  paths;
- accompanied by explicit ownership, pre-open state, read-only / editable
  mode, macro-enabled / VBProject posture, cleanup, and operator-review
  expectations.

## Future Successful-State Test Cases

A later implementation slice should include focused local tests proving that:

- an existing workbook is opened only when its exact identity, ownership,
  location, access, read-only / editable mode, and macro / VBProject posture
  are explicitly authorized;
- workbook identity is re-confirmed after open before any `VBProject` handoff;
- `VBProject` trust/access preflight succeeds before any create-only mutation
  path can run;
- the only mutation path remains create-only missing supported module
  mutation under the existing P7 / P8 boundary;
- lifecycle handoff evidence records workbook identity, open state,
  ownership, dirty / saved state, read-only / editable mode, macro / VBProject
  posture, and remaining authorized lifecycle operations;
- readback verifies the exact created component state before success;
- no-save close is attempted only for the exact authorized workbook when a
  later GO record explicitly authorizes that cleanup behavior;
- success evidence distinguishes workbook lifecycle history, mutation result,
  readback result, component rollback result, dirty / saved / open state, and
  operator-review requirement.

## Future Blocking-State Test Cases

A later implementation slice should include focused local hard-stop tests
proving no workbook-derived `VBProject` handoff and no mutation occurs when:

- workbook identity is missing, relative, ambiguous, mismatched, inaccessible,
  locked, or not proven test-owned;
- more than one workbook matches the authorization;
- selection would depend on active workbook, recent files, name-only match,
  directory scan fallback, nearest match, default fixture fallback, or a
  production workbook;
- open is requested without exact existing workbook identity, allowed mode,
  macro / VBProject posture, or pre-open state checks;
- the workbook is not macro-enabled when macro-enabled behavior is required by
  the later named scope;
- pre-existing dirty state is present for an existing workbook and the later
  GO record does not authorize that state;
- `VBProject` trust/access is unavailable;
- target component state is incompatible with create-only mutation;
- write units are invalid or request an unsupported component operation;
- readback cannot verify the exact created component state;
- cleanup would require Save, SaveAs, restore, backup, replacement, deletion,
  repair, conversion, or any other lifecycle operation not separately
  authorized.

Blocking tests must verify hard stops before workbook open when the missing
authorization can be detected before touching the workbook. When state can be
known only after an authorized open starts, tests must verify failure
reporting, no success claim, no fallback, no unauthorized lifecycle operation,
and explicit operator-review state where clean lifecycle state cannot be
proven.

## Failure, Rollback, And Evidence Expectations

Future focused tests must preserve the P7 mutation failure baseline, the P8
workbook lifecycle boundary, and the P9-02 identity authorization boundary:

- failure before workbook open must not touch a workbook;
- failure before `VBProject` handoff must not create a mutation target;
- failure before mutation must not create, replace, remove, rename, import, or
  export any component;
- component rollback remains limited to current-operation created components;
- workbook lifecycle cleanup remains separate from component rollback;
- Save, SaveAs, restore, backup restoration, replacement, deletion, repair,
  conversion, and fallback target selection require separate later
  authorization;
- incomplete lifecycle state or incomplete component rollback must deny
  success and require operator review;
- evidence must preserve the reason for hard stop or failure without repairing
  workbook identity, workbook state, target component state, Template
  selection, GenerateContext, or Generator output.

## Candidate Non-Scope

P9-03 does not design tests for:

- production workbook handling;
- real user workbook handling;
- live user Excel session control;
- arbitrary workbook discovery;
- workbook Save / SaveAs / restore / backup / replacement / deletion / repair
  / conversion implementation;
- destructive component replace, remove, overwrite, delete, rename, import,
  export, or arbitrary component creation;
- package / `dist`, release, publication, or external service operation;
- credential, token-store, Trust Center, macro security, or protected-view
  mutation;
- fallback Template selection, implicit Template selection, Template content
  inference, GenerateContext compensation, or Generator compensation;
- public API, persisted schema, canonical format, or Frozen specification
  change.

## Later Implementation GO Requirements

A later implementation GO / NO-GO record is required before any code or test
change. That record must name:

- exact editable production and test files;
- exact local test-owned existing workbook identity;
- exact allowed lifecycle operations, including whether open and no-save close
  cleanup are authorized;
- exact denied lifecycle operations;
- expected macro-enabled and `VBProject` trust/access posture;
- pre-existing dirty-state policy;
- component target-state requirements;
- readback, rollback, incomplete-rollback, and operator-review expectations;
- whether any cleanup, discard, restore, backup, replacement, deletion, repair,
  or conversion operation is authorized;
- required focused verification command.

If any item is absent, implementation remains NO-GO.

## Authorized And Unauthorized Operations

Authorized by P9-03:

- create this P9-03 docs-only focused test design record;
- synchronize backlog, current-status, and handoff documentation;
- inspect repository documentation for P8-10, P9-01, P9-02, and related state;
- run documentation diff checks such as `git diff --check` and Git status.

Not authorized by P9-03:

- implementation GO;
- production code change;
- test code change;
- implementation test execution;
- workbook open, creation, save, SaveAs, close, discard, restore, backup,
  replacement, deletion, repair, conversion, fixture mutation, or Excel
  instance control;
- workbook or `VBProject` mutation expansion;
- destructive component operation, import, export, overwrite, delete, rename,
  or arbitrary component creation;
- macro security, Trust Center, credential, protected-view, or external-link
  changes;
- package / `dist` creation, update, replacement, or inspection;
- release, tag, push, or publication operation;
- external service operation;
- credential or token-store access;
- public API, persisted schema, canonical format, or Frozen specification
  change.

## Next Minimum Candidate

Selected next minimum candidate:

**P9-04 - Existing Workbook Focused Test Implementation Scope Planning**

Selection basis:

- P9-03 fixes the focused test design but does not implement it;
- the next smallest safe step is a docs-only implementation-scope and GO /
  NO-GO planning record that can decide whether this focused test design is
  sufficiently authorized for a later implementation-start task;
- implementation remains premature until exact editable files, exact workbook
  identity, allowed lifecycle operations, failure behavior, cleanup behavior,
  and verification command are separately named.

P9-04 must remain docs-only unless a separate task explicitly changes that
scope. P9-03 does not grant implementation GO for P9-04.

## Verification

P9-03 verification is documentation-only:

- review P8-10, P9-01, and P9-02 records;
- review backlog, current-status, and handoff state;
- review docs-only diff;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P9-03.
