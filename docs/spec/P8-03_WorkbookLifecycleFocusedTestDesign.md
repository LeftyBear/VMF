# P8-03 - Workbook Lifecycle Focused Test Design

## Status

COMPLETE / docs-only workbook lifecycle focused test design

## Purpose

Fix the focused test design for future workbook lifecycle handling after
P8-02 Workbook Lifecycle Authorization Boundary.

P8-03 is documentation only. It does not add implementation, change production
code or test code, run implementation tests, open / create / save / SaveAs /
close / discard / restore any workbook, mutate any workbook or VBProject,
create or modify workbook fixtures, update package or `dist` artifacts,
perform release or publication work, access external services, or change
public APIs, persisted schemas, canonical formats, or Frozen specifications.

## Starting State

P7 is COMPLETE.

P8-01 is COMPLETE and fixed the post-P7 responsibility split between workbook
lifecycle handling, real VBProject mutation, and component rollback.

P8-02 is COMPLETE and fixed explicit workbook identity and lifecycle-operation
authorization for open, create, save, SaveAs, close, discard / no-save,
macro-enabled handling, lifecycle state confirmation, lifecycle rollback
limits, and readback / verification handoff.

P8-02 selected P8-03 Workbook Lifecycle Focused Test Design as the next minimum
docs-only candidate. P8-02 did not grant implementation GO.

## Focused Test Target

A future implementation task may add focused local tests only for workbook
lifecycle behavior that is explicitly authorized by a later GO / NO-GO record.

The future test target must be:

- local;
- test-owned;
- isolated from package, `dist`, release, publication, and external service
  paths;
- named by exact workbook identity or exact creation path;
- accompanied by explicit ownership, retention, cleanup, and operator-review
  expectations;
- separated from real user workbooks and production workbooks.

Runtime-selected active workbooks, recent files, name-only matches, directory
scan results, nearest matches, default fixtures, and existing user Excel
sessions are not valid focused test targets.

## Future Successful-State Test Cases

A future implementation slice should include focused local tests proving that:

- an existing workbook is opened only when exact identity, ownership,
  read-only / editable mode, macro/VBProject posture, and pre-open checks are
  authorized;
- a new workbook is created only at the exact authorized path with the exact
  authorized format, macro-enabled setting, replacement rule, and retention /
  cleanup expectation;
- save occurs only for an editable authorized workbook at an authorized save
  point after the expected lifecycle and mutation-adjacent state is proven;
- SaveAs occurs only with exact destination path, format, overwrite policy,
  source identity, and identity handoff rule;
- close occurs only for the exact authorized workbook with explicit save /
  no-save behavior and expected dirty-state handling;
- discard / no-save occurs only when explicitly authorized for the exact
  workbook and observed state;
- macro-enabled handling proceeds only when the file format and authorization
  explicitly allow macro/VBProject access;
- lifecycle handoff to VBProject mutation includes proven workbook identity,
  open / newly-created state, read-only or editable mode, saved / dirty state,
  macro/VBProject posture, ownership class, and remaining authorized
  lifecycle operations;
- post-mutation lifecycle state confirmation records whether the workbook is
  still open, identical to the authorized target, dirty, saveable, closeable,
  discardable, restorable, or operator-review-required;
- readback / verification receives lifecycle state as evidence and does not
  repair, select, save, discard, convert, or reclassify workbook state.

## Future Blocking-State Test Cases

A future implementation slice should include focused local hard-stop tests
proving no workbook lifecycle operation and no workbook-derived VBProject
target handoff occurs when:

- workbook identity is missing, relative, ambiguous, unmatched, mismatched, or
  not proven test-owned;
- more than one workbook matches an authorization;
- an active workbook, recent file, name-only match, directory scan fallback,
  default fixture fallback, or nearest match would be required;
- open is requested without exact existing workbook identity, allowed mode,
  macro/VBProject posture, or pre-open state checks;
- create is requested without exact path, format, macro-enabled setting,
  replacement rule, or retention / cleanup expectation;
- save, SaveAs, close, or discard / no-save is requested without
  operation-level authorization for the exact workbook and observed state;
- pre-existing dirty state is present for an existing workbook;
- macro content or VBProject access is required but macro-enabled handling,
  Trust Center / VBProject access preflight, protected-view behavior,
  conversion behavior, repair behavior, external-link behavior, or credential
  handling is undefined;
- lifecycle state cannot prove workbook identity, open state, dirty state,
  saved state, ownership class, or authorized remaining operations;
- failed open, create, save, SaveAs, close, or discard would require fallback,
  retry at another path, cleanup, deletion, replacement, restore, conversion,
  or operator-state mutation not separately authorized;
- workbook lifecycle handling would decide VBProject component mutation or
  component rollback behavior;
- VBProject mutation would open, create, save, SaveAs, close, discard, restore,
  replace, or select workbooks.

Blocking tests must verify hard stops before workbook open or creation when
the missing authorization can be detected before touching the workbook. When
state can be known only after an authorized lifecycle operation starts, tests
must verify failure reporting, no success claim, no fallback, and explicit
operator-review state where clean lifecycle state cannot be proven.

## Failure, Rollback, And Evidence Expectations

Future focused tests must preserve the P7 mutation failure baseline and the
P8-02 workbook lifecycle boundary:

- failure before workbook lifecycle operation starts must not touch a workbook;
- failure before VBProject mutation handoff must not create a mutation target;
- lifecycle rollback is limited to the lifecycle operation explicitly
  authorized and already started;
- workbook restore, backup restoration, replacement, deletion, cleanup, repair,
  format conversion, and fallback target selection require separate later
  authorization;
- component rollback remains limited to current-operation created VBComponents
  under the P7 boundary unless a later mutation candidate changes it;
- incomplete lifecycle state or incomplete component rollback must deny success
  and require operator review;
- evidence must distinguish lifecycle operation history, mutation result,
  readback result, component rollback result, dirty / saved / open state, and
  operator-review requirement.

## Candidate Non-Scope

P8-03 does not design tests for:

- production workbook handling;
- real user workbook handling;
- live user Excel session control;
- arbitrary workbook discovery;
- workbook snapshot, backup, restore, replacement, deletion, cleanup, repair,
  or conversion implementation;
- VBProject import, export, overwrite, delete, rename, or arbitrary component
  creation;
- package / `dist`, release, publication, or external service operation;
- credential, token-store, Trust Center, or macro security mutation;
- fallback Template selection, implicit Template selection, Template content
  inference, GenerateContext compensation, or Generator compensation;
- public API, persisted schema, canonical format, or Frozen specification
  change.

## Later Implementation GO Requirements

A later implementation GO / NO-GO record is required before any code or test
change. That record must name:

- exact editable production and test files;
- exact focused test workbook fixture identity or exact creation path;
- exact allowed lifecycle operations;
- exact denied lifecycle operations;
- expected macro-enabled and VBProject access posture;
- pre-existing dirty-state policy;
- lifecycle failure reporting and operator-review expectations;
- whether any cleanup, restore, or discard operation is authorized;
- required focused verification command.

If any item is absent, implementation remains NO-GO.

## Authorized And Unauthorized Operations

Authorized by P8-03:

- create this P8-03 docs-only focused test design record;
- synchronize backlog, current-status, and handoff documentation;
- inspect repository documentation for P8-01, P8-02, and related state;
- run documentation diff checks such as `git diff --check` and Git status.

Not authorized by P8-03:

- implementation GO;
- production code change;
- test code change;
- implementation test execution;
- workbook open, creation, save, SaveAs, close, discard, restore, replacement,
  deletion, repair, conversion, fixture mutation, or Excel instance control;
- real workbook or real VBProject mutation;
- VBProject import, export, overwrite, delete, rename, or arbitrary component
  creation;
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

**P8-04 - Workbook Lifecycle Focused Test Implementation Scope Planning**

Selection basis:

- P8-03 fixes the focused test design but does not implement it;
- the next smallest safe step is a docs-only implementation-scope and GO /
  NO-GO planning record that can decide whether the focused test design is
  sufficiently authorized for a later implementation-start task;
- implementation remains premature until exact editable files, fixture
  identity, allowed lifecycle operations, failure behavior, and verification
  command are separately named.

P8-04 must remain docs-only unless a separate task explicitly changes that
scope. P8-03 does not grant implementation GO for P8-04.

## Verification

P8-03 verification is documentation-only:

- review P8-01 and P8-02;
- review backlog, current-status, and handoff state;
- review docs-only diff;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P8-03.
