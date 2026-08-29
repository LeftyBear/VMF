# P8-04 - Workbook Lifecycle Focused Test Implementation Scope Planning

## Status

COMPLETE / docs-only focused test implementation scope planning

## Purpose

Connect the P8-03 Workbook Lifecycle Focused Test Design to a later
implementation GO / NO-GO decision by fixing candidate implementation scope,
required authorization inputs, acceptance criteria, non-scope, and safety
stops.

P8-04 is documentation only. It does not add implementation, change production
code or test code, run implementation tests, open / create / save / SaveAs /
close / discard / restore any workbook, mutate any workbook or VBProject,
create or modify workbook fixtures, update package or `dist` artifacts,
perform release or publication work, access external services, or change
public APIs, persisted schemas, canonical formats, or Frozen specifications.

## Starting State

P7 is COMPLETE.

P8-01 is COMPLETE and fixed the post-P7 responsibility split between workbook
lifecycle handling, real VBProject mutation, and component rollback.

P8-02 is COMPLETE and fixed the workbook lifecycle authorization boundary for
explicit workbook identity, lifecycle operations, macro-enabled handling,
state confirmation, lifecycle rollback limits, and readback / verification
handoff.

P8-03 is COMPLETE and fixed the future focused local test design for workbook
lifecycle behavior. P8-03 selected P8-04 Workbook Lifecycle Focused Test
Implementation Scope Planning as the next minimum docs-only candidate. P8-03
did not grant implementation GO.

## Scope

P8-04 defines:

- the future implementation decision boundary after P8-03;
- candidate focused local test implementation scope for workbook lifecycle
  handling;
- required GO conditions before code, test, fixture, workbook, or VBProject
  operations;
- required NO-GO and safety-stop conditions;
- acceptance criteria for a later implementation-start task;
- preserved boundaries between workbook lifecycle handling, VBProject
  mutation, component rollback, readback / verification, package / `dist`,
  release / publication, external services, public APIs, persisted schemas,
  canonical formats, and Frozen specifications.

## Preconditions For Future Implementation GO

A future implementation task may be considered only after all of these are
true:

- P8-01 remains the accepted post-P7 real workbook / VBProject mutation scope
  planning record.
- P8-02 remains the accepted workbook lifecycle authorization-boundary record.
- P8-03 remains the accepted workbook lifecycle focused test design.
- The future task explicitly authorizes exact editable production and test
  files.
- The future task names the exact workbook lifecycle entry boundary from the
  current codebase at that time.
- The future task names the exact test-owned workbook fixture identity or
  exact temporary creation path.
- The fixture is local, test-owned, isolated from real user workbooks and
  production workbooks, and excluded from package, `dist`, release,
  publication, and external service paths.
- Each allowed lifecycle operation is named separately: open, create, save,
  SaveAs, close, discard / no-save, restore, cleanup, or retention.
- Each denied lifecycle operation is named separately.
- Macro-enabled format posture and VBProject access posture are explicitly
  stated.
- Existing-workbook dirty-state policy is explicitly stated.
- Lifecycle state handoff to VBProject mutation and readback / verification is
  explicitly defined.
- Failure reporting, operator-review evidence, and any permitted cleanup or
  restore behavior are explicitly defined.
- The required focused verification command is explicitly named.

If any of these cannot be confirmed, implementation remains NO-GO.

## Candidate Implementation Scope

A future implementation GO may include only:

- focused local tests for the P8-03 successful-state and blocking-state
  workbook lifecycle cases selected by the future GO / NO-GO record;
- minimal test helpers required to construct explicit workbook lifecycle
  authorization inputs;
- explicitly authorized creation or use of a test-owned local workbook fixture;
- explicitly authorized lifecycle operations against only that fixture;
- hard-stop tests for missing, ambiguous, mismatched, fallback-derived, or
  unauthorized workbook identity and lifecycle operations;
- evidence assertions for lifecycle operation history, workbook identity,
  open / dirty / saved state, macro-enabled and VBProject access posture,
  authorized remaining operations, failure reporting, and operator-review
  requirement;
- existing Build test runner registration only if required to execute the
  focused tests;
- the narrow workbook lifecycle entry boundary, only if separately authorized
  by the future implementation GO.

The implementation must not consume active workbook state, recent files,
directory scans, nearest matches, default fixtures, raw Blueprint state,
Template contents, GenerateContext diagnostics, Generator internals, target
project runtime state, or external state as authorization input.

## Candidate Non-Scope

A future implementation GO must not include:

- mutation of real user workbooks or production workbooks;
- live user Excel session control;
- runtime-selected active workbook targets;
- arbitrary workbook discovery;
- VBProject import, export, overwrite, delete, rename, arbitrary component
  creation, or component rollback redesign;
- workbook snapshot, backup, restore, replacement, deletion, cleanup, repair,
  or conversion unless explicitly named by the future GO / NO-GO record;
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

- focused local tests prove workbook lifecycle operations occur only for the
  exact authorized workbook identity or exact authorized creation path;
- focused local tests prove each lifecycle operation occurs only when that
  operation is explicitly authorized for the observed workbook state;
- focused local tests prove denied, missing, ambiguous, mismatched, fallback,
  active-workbook, recent-file, default-fixture, and nearest-match selection
  paths hard-stop before touching a workbook when the failure is knowable
  before open or creation;
- focused local tests prove lifecycle state handoff includes workbook identity,
  open / newly-created state, read-only or editable mode, dirty / saved state,
  macro-enabled and VBProject access posture, ownership class, and authorized
  remaining lifecycle operations;
- focused local tests prove readback / verification receives lifecycle state
  as evidence and does not repair, select, save, discard, convert, or
  reclassify workbook state;
- focused local tests prove failure evidence distinguishes lifecycle operation
  history, mutation result, readback result, component rollback result, dirty /
  saved / open state, and operator-review requirement;
- hard-stop tests prove workbook lifecycle handling does not decide VBProject
  component mutation or component rollback behavior;
- hard-stop tests prove VBProject mutation does not open, create, save, SaveAs,
  close, discard, restore, replace, or select workbooks;
- package, `dist`, release, publication, external service operation, public
  API change, persisted schema change, canonical format change, and Frozen
  specification change remain absent from the implementation and verification
  path;
- required focused verification and `git diff --check` pass.

## Safety Boundary

The future implementation task must stop before edits if:

- the current codebase cannot identify a narrow workbook lifecycle entry
  boundary;
- exact editable files are not authorized;
- exact test-owned workbook identity or exact temporary creation path is not
  authorized;
- fixture ownership, isolation, retention, cleanup, or operator-review
  expectations are undefined;
- any requested lifecycle operation lacks operation-level authorization;
- macro-enabled format posture, VBProject access posture, Trust Center /
  access preflight, protected-view behavior, conversion behavior, repair
  behavior, external-link behavior, or credential handling is undefined;
- pre-existing dirty-state policy is undefined;
- failure reporting, lifecycle state handoff, readback / verification evidence,
  cleanup, restore, or discard behavior is undefined;
- implementation requires workbook fallback selection, active workbook
  selection, recent-file selection, name-only matching, directory scanning,
  nearest-match recovery, public API changes, persisted schema changes,
  canonical format changes, Frozen specification changes, package or `dist`
  operations, release operations, external services, credentials, token stores,
  or live user data;
- existing user changes conflict with the target files.

## Scope Planning Decision

GO:

- P8-04 docs-only Workbook Lifecycle Focused Test Implementation Scope
  Planning;
- backlog, current-status, and handoff documentation updates recording P8-04
  completion;
- documentation diff review;
- `git diff --check`;
- Git status confirmation.

NO-GO:

- implementation GO;
- production code changes;
- test code additions or updates;
- implementation test execution;
- workbook open, creation, save, SaveAs, close, discard, restore, replacement,
  deletion, repair, conversion, fixture mutation, or Excel instance control;
- real workbook or real VBProject mutation;
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

P8-04 preserves these boundaries:

- P7 create-only missing-module mutation, readback failure handling, component
  rollback, incomplete rollback evidence, and operator-review reporting remain
  the completed mutation baseline.
- Workbook lifecycle handling is separate from VBProject mutation and
  component rollback.
- Workbook lifecycle authorization cannot be inferred from generated output,
  workbook runtime state, target project runtime state, active Excel state, or
  prior component rollback authority.
- Lifecycle rollback is limited to the explicitly authorized lifecycle
  operation that has already started.
- Workbook restore, backup restoration, replacement, deletion, cleanup, repair,
  format conversion, and fallback selection require separate explicit
  authorization before implementation.
- Readback / verification may consume lifecycle state as evidence but must not
  repair lifecycle state or convert incomplete lifecycle state into success.
- Real user workbook handling, production workbook handling, package / `dist`,
  release / publication, external services, public APIs, persisted schemas,
  canonical formats, and Frozen specifications remain outside P8-04.

## Next Minimum Candidate

Selected next minimum candidate:

**P8-05 - Workbook Lifecycle Focused Test Implementation GO / NO-GO**

Selection basis:

- P8-04 fixes candidate implementation scope but does not authorize code or
  test changes;
- the next smallest safe step is a docs-only GO / NO-GO record that either
  names exact editable files, fixture identity, lifecycle operations, failure
  behavior, and focused verification command, or records NO-GO with the
  missing authorization inputs;
- implementation remains premature until P8-05 is complete and explicitly
  authorizes a separate implementation-start task.

P8-05 must remain docs-only unless a separate task explicitly changes that
scope. P8-04 does not grant implementation GO for P8-05.

## Verification

P8-04 verification is documentation-only:

- review P8-01, P8-02, and P8-03;
- review backlog, current-status, and handoff state;
- review docs-only diff;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P8-04.
