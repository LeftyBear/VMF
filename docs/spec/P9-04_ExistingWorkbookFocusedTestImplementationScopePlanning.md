# P9-04 - Existing Workbook Focused Test Implementation Scope Planning

## Status

COMPLETE / docs-only existing workbook focused test implementation scope
planning

## Purpose

Connect the P9-03 Existing Workbook Focused Test Design to a later
implementation GO / NO-GO decision by fixing candidate implementation scope,
required authorization inputs, acceptance criteria, non-scope, and safety
stops.

P9-04 is documentation only. It does not grant implementation GO, change
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
while keeping existing-workbook handling, production workbook handling, Save /
SaveAs / restore, destructive component operations, package / `dist`, release
/ publication, external services, public API changes, persisted schema
changes, canonical format changes, and Frozen specification changes as NO-GO.

P9-02 is COMPLETE and fixes the actual workbook identity authorization
boundary for exact local test-owned workbook identity, ownership, denied
fallback selection, lifecycle-operation boundary, safety stops, evidence, and
verification expectations.

P9-03 is COMPLETE and fixes the future focused local test design for an
explicitly named local test-owned existing workbook. P9-03 selected P9-04 as
the next minimum docs-only candidate. P9-03 did not grant implementation GO.

## Scope

P9-04 defines:

- the future implementation decision boundary after P9-03;
- candidate focused local test implementation scope for existing-workbook
  handling;
- required GO conditions before code, test, fixture, workbook, or VBProject
  operations;
- required NO-GO and safety-stop conditions;
- acceptance criteria for a later implementation-start task;
- preserved boundaries between workbook identity, workbook lifecycle handling,
  VBProject mutation, component rollback, readback / verification, package /
  `dist`, release / publication, external services, public APIs, persisted
  schemas, canonical formats, and Frozen specifications.

## Preconditions For Future Implementation GO

A future implementation task may be considered only after all of these are
true:

- P9-01 remains the accepted actual workbook mutation expansion scope planning
  record.
- P9-02 remains the accepted actual workbook identity authorization boundary.
- P9-03 remains the accepted existing workbook focused test design.
- The future task explicitly authorizes exact editable production and test
  files.
- The future task names the exact existing-workbook lifecycle entry boundary
  from the current codebase at that time.
- The future task names the exact local test-owned existing workbook identity.
- The workbook fixture is local, test-owned, isolated from user and production
  workbooks, and excluded from package, `dist`, release, publication, and
  external service paths.
- The future task states how the exact existing workbook is selected without
  active workbook state, recent files, name-only matching, directory scans,
  nearest matches, default fixture fallback, or production workbook fallback.
- Each allowed lifecycle operation is named separately: open, identity
  reconfirmation, `VBProject` handoff, no-save close cleanup, retention, or
  any other cleanup behavior.
- Each denied lifecycle operation is named separately.
- Macro-enabled format posture, `VBProject` trust/access posture, protected
  view posture, and external-link posture are explicitly stated.
- Pre-existing dirty-state policy is explicitly stated.
- Target component-state requirements are explicitly stated.
- Failure reporting, readback, component rollback, incomplete rollback,
  lifecycle cleanup evidence, and operator-review expectations are explicitly
  defined.
- The required focused verification command is explicitly named.

If any of these cannot be confirmed, implementation remains NO-GO.

## Candidate Implementation Scope

A future implementation GO may include only:

- focused local tests for the P9-03 successful-state and blocking-state
  existing-workbook cases selected by the future GO / NO-GO record;
- minimal test helpers required to construct explicit existing-workbook
  identity and lifecycle authorization inputs;
- explicitly authorized use of the exact local test-owned existing workbook
  fixture;
- explicitly authorized lifecycle operations against only that exact fixture;
- hard-stop tests for missing, ambiguous, mismatched, fallback-derived,
  active-workbook, recent-file, name-only, directory-scan, default-fixture,
  nearest-match, production-workbook, or unauthorized lifecycle inputs;
- evidence assertions for workbook identity, open state, ownership, dirty /
  saved state, read-only / editable mode, macro-enabled and `VBProject` access
  posture, authorized remaining lifecycle operations, mutation handoff,
  readback, rollback, failure reporting, cleanup state, and operator-review
  requirement;
- existing Build test runner registration only if required to execute the
  focused tests;
- a narrow existing-workbook lifecycle boundary only if separately authorized
  by the future implementation GO.

The implementation must not consume active workbook state, recent files,
directory scans, nearest matches, default fixtures, raw Blueprint state,
Template contents, GenerateContext diagnostics, Generator internals, target
project runtime state, production workbook state, or external state as
authorization input.

## Candidate Non-Scope

A future implementation GO must not include:

- mutation of real user workbooks or production workbooks;
- live user Excel session control;
- runtime-selected active workbook targets;
- arbitrary workbook discovery;
- workbook creation unless separately authorized by a later record;
- Save, SaveAs, backup, restore, replacement, deletion, repair, conversion, or
  cleanup beyond the exact authorized no-save close path;
- VBProject import, export, overwrite, delete, rename, arbitrary component
  creation, or component rollback redesign;
- Trust Center, macro security, credential, token-store, protected-view, or
  external-link mutation;
- package, `dist`, release, publication, or external service operation;
- fallback Template selection, implicit Template selection, Template content
  inference, GenerateContext compensation, or Generator compensation;
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, or fake/local target mutation behavior changes;
- public API changes;
- persisted schema changes;
- canonical format changes;
- Frozen specification changes.

## Acceptance Criteria For Future Implementation

If separately authorized, the future implementation is acceptable only when:

- focused local tests prove workbook operations occur only for the exact
  authorized existing workbook identity;
- focused local tests prove workbook identity is reconfirmed after open and
  before `VBProject` handoff;
- focused local tests prove selection never depends on active workbook state,
  recent files, name-only matching, directory scans, nearest matches, default
  fixture fallback, or production workbook fallback;
- focused local tests prove `VBProject` trust/access preflight succeeds before
  the existing create-only missing supported module mutation path can run;
- focused local tests prove denied, missing, ambiguous, mismatched,
  fallback-derived, production-workbook, dirty-state, locked, inaccessible,
  or unauthorized lifecycle paths hard-stop before mutation;
- focused local tests prove lifecycle state handoff includes workbook
  identity, open state, ownership, read-only or editable mode, dirty / saved
  state, macro-enabled and `VBProject` access posture, and authorized
  remaining lifecycle operations;
- focused local tests prove readback verifies the exact created component
  state before success;
- focused local tests prove component rollback remains limited to
  current-operation created components;
- focused local tests prove lifecycle cleanup evidence is separate from
  component rollback evidence;
- focused local tests prove incomplete lifecycle state, incomplete component
  rollback, or incomplete cleanup denies success and requires operator review;
- hard-stop tests prove existing-workbook lifecycle handling does not decide
  unsupported component operations or component rollback redesign;
- package, `dist`, release, publication, external service operation, public
  API change, persisted schema change, canonical format change, and Frozen
  specification change remain absent from the implementation and verification
  path;
- required focused verification and `git diff --check` pass.

## Safety Boundary

The future implementation task must stop before edits if:

- the current codebase cannot identify a narrow existing-workbook lifecycle
  entry boundary;
- exact editable files are not authorized;
- exact local test-owned existing workbook identity is not authorized;
- fixture ownership, isolation, pre-open state, retention, cleanup, or
  operator-review expectations are undefined;
- any requested lifecycle operation lacks operation-level authorization;
- macro-enabled format posture, `VBProject` access posture, Trust Center /
  access preflight, protected-view behavior, conversion behavior, repair
  behavior, external-link behavior, or credential handling is undefined;
- pre-existing dirty-state policy is undefined;
- target component-state policy is undefined;
- failure reporting, lifecycle state handoff, readback / verification
  evidence, component rollback, cleanup, restore, or discard behavior is
  undefined;
- implementation requires workbook fallback selection, active workbook
  selection, recent-file selection, name-only matching, directory scanning,
  nearest-match recovery, public API changes, persisted schema changes,
  canonical format changes, Frozen specification changes, package or `dist`
  operations, release operations, external services, credentials, token
  stores, or live user data;
- existing user changes conflict with the target files.

## Scope Planning Decision

GO:

- P9-04 docs-only Existing Workbook Focused Test Implementation Scope
  Planning;
- backlog, current-status, and handoff documentation updates recording P9-04
  completion;
- documentation diff review;
- `git diff --check`;
- Git status confirmation.

NO-GO:

- implementation GO;
- production code changes;
- test code additions or updates;
- implementation test execution;
- workbook open, creation, save, SaveAs, close, discard, restore, backup,
  replacement, deletion, repair, conversion, fixture mutation, or Excel
  instance control;
- workbook or VBProject mutation expansion;
- VBProject import, export, overwrite, delete, rename, arbitrary component
  creation, or component rollback redesign;
- macro security, Trust Center, credential, protected-view, or external-link
  changes;
- package, `dist`, release, tag, push, or publication operation;
- external service operation;
- credential or token-store access;
- public API changes;
- persisted schema changes;
- canonical format changes;
- Frozen specification changes.

## Preserved Boundaries

P9-04 preserves these boundaries:

- P8 remains complete only for the narrow local-only test-owned workbook /
  create-only VBProject mutation flow.
- P9-01 inventories actual workbook mutation expansion without implementation
  GO.
- P9-02 requires exact workbook identity, ownership, denied fallback
  selection, lifecycle-operation authorization, evidence, and verification
  expectations before any later actual workbook mutation expansion.
- P9-03 fixes focused existing-workbook test design but does not authorize
  implementation.
- Create-only missing supported module mutation remains the only completed
  VBProject operation.
- Trust/access, target-state, workbook-identity, lifecycle-authorization, and
  invalid-write-unit hard stops remain before mutation.
- Mandatory readback remains required before success.
- Component rollback remains limited to current-operation created components.
- Workbook lifecycle cleanup remains separate from component rollback.
- Incomplete rollback, incomplete cleanup, or incomplete lifecycle state must
  deny success and require operator review.
- Fallback / implicit Template selection, Template content inference,
  GenerateContext compensation, and Generator compensation remain prohibited.
- Real user workbook handling, production workbook handling, package / `dist`,
  release / publication, external services, public APIs, persisted schemas,
  canonical formats, and Frozen specifications remain outside P9-04.

## Next Minimum Candidate

Selected next minimum candidate:

**P9-05 - Existing Workbook Focused Test Implementation GO / NO-GO**

Selection basis:

- P9-04 fixes candidate implementation scope but does not authorize code or
  test changes;
- the next smallest safe step is a docs-only GO / NO-GO record that either
  names exact editable files, exact existing workbook identity, allowed
  lifecycle operations, denied lifecycle operations, failure behavior, cleanup
  behavior, and focused verification command, or records NO-GO with the
  missing authorization inputs;
- implementation remains premature until P9-05 is complete and explicitly
  authorizes a separate implementation-start task.

P9-05 must remain docs-only unless a separate task explicitly changes that
scope. P9-04 does not grant implementation GO for P9-05.

## Verification

P9-04 verification is documentation-only:

- review P9-01, P9-02, and P9-03 records;
- review backlog, current-status, and handoff state;
- review docs-only diff;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P9-04.
