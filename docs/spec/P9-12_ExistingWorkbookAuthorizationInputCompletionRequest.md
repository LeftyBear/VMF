# P9-12 - Existing Workbook Authorization Input Completion Request

## Status

COMPLETE / docs-only existing workbook authorization input completion request

## Purpose

Record the exact owner-provided authorization inputs that remain required
before a later focused existing-workbook implementation GO / NO-GO decision
can be meaningful.

P9-12 is documentation only. It does not start implementation, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, mutate any workbook or
VBProject, create or modify workbook fixtures, update package or `dist`
artifacts, perform release or publication work, access external services, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Starting State

- P8 is COMPLETE only for the narrow local-only test-owned workbook /
  create-only VBProject mutation flow.
- P9-01 through P9-11 are COMPLETE as docs-only planning, focused-test design,
  implementation-scope, authorization-package, input-package, GO / NO-GO,
  follow-up, and re-evaluation predecessor records for actual
  existing-workbook mutation expansion.
- P9-11 selects P9-12 as the next minimum docs-only candidate because the
  exact workbook identity and operation-level lifecycle authorization values
  remain absent.
- This P9-12 task input starts the named input-completion request but supplies
  no exact local test-owned existing workbook path, file hash,
  repository-approved fixture identity, owner-approved workbook identity,
  path-open mode, no-save close cleanup authorization, pre-existing
  dirty-state policy, target component-state policy, fixture retention policy,
  operator-review expectations, or focused implementation verification
  authorization.

## Completion Request

The following values must be supplied by the repository owner before a later
implementation GO / NO-GO task can re-evaluate focused existing-workbook
implementation start.

| Required owner input | Completion requirement |
| --- | --- |
| Accepted predecessor records | Confirm whether P9-01 through P9-12 remain the accepted P9 record sequence for the later decision. |
| Exact editable production files | Confirm whether the later implementation candidate remains limited to `src/Build/Application/AppOutputWriteService.cls`. |
| Exact editable test files | Confirm whether the later implementation candidate remains limited to `tests/unit/Build/AppOutputWriteBoundaryTests.bas`. |
| Existing-workbook lifecycle entry boundary | Confirm whether the later candidate remains a narrow path-open lifecycle boundary in `AppOutputWriteService.cls`, with handoff to `AppApplyGeneratedOutputToAuthorizedWorkbook`. |
| Exact local test-owned existing workbook identity | Provide an exact absolute path or repository-approved fixture identity, plus any required file hash or fixture-copy source. |
| Workbook ownership and isolation | Confirm the workbook is local, test-owned, isolated from production, and safe for the specified lifecycle operations. |
| Workbook selection method | Confirm exact-identity-only selection and denial of active workbook state, recent files, name-only matching, directory scans, nearest matches, default fixtures, and production workbook fallback. |
| Existing workbook open mode | Authorize exactly one path-open mode, either read-only or editable. |
| Identity reconfirmation after open | Define the post-open identity checks required before any `VBProject` handoff. |
| VBProject trust/access preflight | Confirm the required preflight state and that no Trust Center, macro security, credential, token-store, protected-view, or external-link setting may be changed by the implementation. |
| Macro-enabled format posture | Confirm the authorized workbook format and whether macro-enabled workbook handling is expected. |
| Protected-view, repair, conversion, external-link, and credential posture | Confirm that protected view, repair prompts, conversion prompts, external-link prompts, and credential prompts are hard stops unless an explicitly named exception is authorized. |
| Pre-existing dirty-state policy | Define whether a workbook already dirty before mutation is a hard stop, and how that state must be detected and reported. |
| Target component-state policy | Define the required precondition for create-only missing supported module mutation, including what happens if any target component already exists. |
| Allowed VBProject component operations | Confirm whether only the existing create-only missing supported module mutation path is allowed. |
| Denied VBProject component operations | Confirm denial of import, export, overwrite, delete, rename, arbitrary component creation, destructive component operations, and component rollback redesign. |
| Save, SaveAs, restore, backup, replacement, deletion, repair, and conversion | Confirm these operations remain denied unless separately and explicitly authorized. |
| No-save close cleanup | Define whether no-save close cleanup is authorized, and the exact failure reporting required when close / cleanup cannot complete. |
| Fixture retention and operator review | Define whether the fixture must be retained for inspection after success or failure, and when operator review is required. |
| Readback and rollback expectations | Define success readback requirements, rollback requirements after failure, incomplete rollback reporting, and evidence retention. |
| Focused verification command | Provide the exact focused verification command and state whether all Build VBA runners are required. |

## Completion Decision

Decision: `GO` for recording P9-12 as a docs-only existing workbook
authorization input completion request.

Decision: `NO-GO` for focused existing-workbook implementation start.

Decision: `NO-GO` for workbook open, close, discard, fixture creation,
fixture mutation, existing workbook path-open lifecycle implementation,
production code changes, test code changes, implementation test execution,
and VBProject mutation expansion during P9-12.

P9-12 keeps implementation NO-GO because this task input does not provide the
owner-supplied values listed in the completion request. The completion request
is a documentation record of missing inputs; it is not the missing input
values themselves.

No authorization value is inferred from repository paths, active Excel state,
file names, historical fixtures, default workbook assumptions, or local
environment state.

## Required Inputs For Later Re-Evaluation

A later implementation GO / NO-GO may re-evaluate this NO-GO only if the task
explicitly supplies:

- exact local test-owned existing workbook identity, with absolute path or
  repository-approved fixture identity and any required hash;
- workbook ownership, isolation, pre-open state, fixture retention, cleanup,
  and operator-review expectations;
- path-open mode, explicitly read-only or editable;
- allowed lifecycle operations, separately naming open, identity
  reconfirmation, `VBProject` handoff, no-save close cleanup, retention, and
  any other cleanup behavior;
- denied lifecycle operations;
- macro-enabled format posture and `VBProject` trust/access posture;
- protected-view, repair, conversion, external-link, credential, token-store,
  and Trust Center posture;
- pre-existing dirty-state policy;
- target component-state policy for create-only missing supported module
  mutation;
- readback, component rollback, incomplete rollback, lifecycle cleanup,
  incomplete cleanup, failure reporting, and operator-review expectations;
- exact focused verification command and whether all Build VBA runners are
  required.

Until those inputs are complete and a separate GO / NO-GO record approves a
specific implementation-start task, implementation remains NO-GO.

## Preserved NO-GO Operations

The following remain NO-GO:

- implementation start by P9-12;
- production code changes by P9-12;
- test code additions or updates by P9-12;
- implementation test execution by P9-12;
- workbook open, creation, save, SaveAs, close, discard, restore, backup,
  replacement, deletion, repair, conversion, fixture mutation, or Excel
  instance control by P9-12;
- existing-workbook path-open lifecycle implementation by P9-12;
- workbook or VBProject mutation expansion by P9-12;
- active workbook selection;
- recent-file selection;
- name-only workbook matching;
- directory scanning;
- nearest-match recovery;
- default fixture fallback;
- production workbook fallback;
- real user workbook or production workbook mutation;
- VBProject import, export, overwrite, delete, rename, arbitrary component
  creation, destructive component operation, or component rollback redesign;
- macro security, Trust Center, credential, protected-view, token-store, or
  external-link changes;
- fallback Template selection;
- implicit Template selection;
- Template content inference;
- GenerateContext or Generator compensation;
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, fake/local target mutation, real VBProject mutation,
  or workbook lifecycle behavior changes;
- package / `dist`, release, tag, push, or publication operation;
- external service operation;
- public API changes;
- persisted schema changes;
- canonical format changes;
- Frozen specification changes.

## Next Minimum Candidate

Selected next minimum candidate:

**P9-13 - Existing Workbook Authorization Input GO / NO-GO**

Selection basis:

- P9-12 records the exact owner input values required before implementation
  can be re-evaluated;
- this task input still does not provide those values;
- the next smallest safe step is a docs-only GO / NO-GO decision that applies
  any owner-supplied input values if they are provided by a later task.

P9-13 must remain docs-only unless a separate task explicitly changes that
scope. P9-12 does not grant implementation GO for P9-13.

## Verification

P9-12 verification is documentation-only:

- reviewed P9-11 record;
- reviewed backlog, current-status, and handoff state;
- confirmed no exact local test-owned existing workbook identity or
  operation-level lifecycle authorization is supplied by this task input;
- confirmed this task grants no implementation, test change, workbook
  operation, fixture mutation, or VBProject mutation GO in P9-12;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.

No implementation tests are required or run for P9-12.
