# P9-08 - Existing Workbook Identity Authorization Input Package

## Status

COMPLETE / docs-only existing workbook identity authorization input package

## Purpose

Create the input package selected by P9-07 so a later docs-only GO / NO-GO
record can decide whether focused existing-workbook implementation may start.

P9-08 is documentation only. It does not start implementation, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, mutate any workbook or
VBProject, create or modify workbook fixtures, update package or `dist`
artifacts, perform release or publication work, access external services, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Starting State

- P8 is COMPLETE only for the narrow local-only test-owned workbook /
  create-only VBProject mutation flow.
- P9-01 through P9-06 are COMPLETE as docs-only planning, focused-test design,
  implementation-scope, implementation GO / NO-GO, and authorization package
  records for actual existing-workbook mutation expansion.
- P9-07 is COMPLETE as the docs-only authorization package GO / NO-GO decision.
  It records focused existing-workbook implementation start as NO-GO because
  exact workbook identity and operation-level lifecycle authorization remain
  missing.
- This P9-08 task input names no exact local test-owned existing workbook path,
  file hash, fixture source, owner-approved workbook identity, path-open mode,
  no-save close cleanup authorization, pre-existing dirty-state policy, target
  component-state policy, fixture retention policy, or focused implementation
  verification authorization.

## Authorization Input Package

| Required input | P9-08 package value | Authorization state |
| --- | --- | --- |
| Accepted predecessor records | P9-01, P9-02, P9-03, P9-04, P9-05, P9-06, and P9-07 remain the accepted P9 records. | Fixed for later re-evaluation |
| Exact editable production files | Candidate remains `src/Build/Application/AppOutputWriteService.cls` only for a later separately authorized implementation-start task. | Candidate fixed; not implementation GO |
| Exact editable test files | Candidate remains `tests/unit/Build/AppOutputWriteBoundaryTests.bas` only for a later separately authorized implementation-start task. | Candidate fixed; not implementation GO |
| Existing-workbook lifecycle entry boundary | Candidate remains a narrow path-open lifecycle boundary in `AppOutputWriteService.cls`, with handoff to `AppApplyGeneratedOutputToAuthorizedWorkbook`. | Candidate fixed; not implementation GO |
| Exact local test-owned existing workbook identity | Not supplied in P9-08. No exact absolute path, repository-approved fixture identity, file hash, temporary fixture-copy source, or owner-approved workbook identity is available. | Missing; implementation remains NO-GO |
| Workbook ownership and isolation | Required value remains local test-owned, isolated from user and production workbooks, excluded from package, `dist`, release, publication, and external service paths. P9-08 does not identify such a workbook. | Missing; implementation remains NO-GO |
| Workbook selection method | Required value remains exact identity only. Active workbook state, recent files, name-only matching, directory scans, nearest matches, default fixtures, and production workbook fallback remain denied. | Fixed denial; exact selection input missing |
| Existing workbook open mode | Not supplied in P9-08. Read-only or editable path-open mode is not authorized. | Missing; implementation remains NO-GO |
| Identity reconfirmation after open | Required for any later implementation before `VBProject` handoff, but no exact identity or open mode is available for application. | Required for later owner authorization |
| VBProject trust/access preflight | Required before any `VBProject` access or mutation. P9-08 does not authorize Trust Center, macro security, credential, token-store, or protected-view changes. | Required for later owner authorization |
| Macro-enabled format posture | Not supplied in P9-08 because no exact workbook format is authorized. | Missing; implementation remains NO-GO |
| Protected-view, repair, conversion, external-link, and credential posture | Required value remains hard-stop unless separately authorized. No exception is supplied in P9-08. | Fixed hard-stop posture |
| Pre-existing dirty-state policy | Not supplied in P9-08. Pre-existing dirty state remains a hard stop for any later implementation unless a later authorization record defines safe handling. | Missing; implementation remains NO-GO |
| Target component-state policy | Not supplied in P9-08. The authorized workbook must not already contain target components selected for create-only mutation, but no target workbook is named. | Missing; implementation remains NO-GO |
| Allowed VBProject component operations | Candidate remains only the existing create-only missing supported module mutation path. | Candidate fixed; not expansion GO |
| Denied VBProject component operations | Import, export, overwrite, delete, rename, arbitrary component creation, destructive component operations, and component rollback redesign remain denied. | Fixed denial |
| Save, SaveAs, restore, backup, replacement, deletion, repair, and conversion | Not authorized by P9-08. | Prohibited |
| No-save close cleanup | Not supplied in P9-08. No-save close cleanup is not authorized because no exact fixture, open mode, dirty-state posture, or retention policy is named. | Missing; implementation remains NO-GO |
| Fixture retention and operator review | Required for a later GO. Incomplete lifecycle state, incomplete component rollback, incomplete close / cleanup, or uncertain dirty state must deny success and require operator review. | Required for later owner authorization |
| Focused verification command | Not supplied in P9-08. Candidate remains focused Build VBA `AppRunOutputWriteBoundaryTests` plus `git diff --check`, but P9-08 does not authorize implementation test execution. | Missing; implementation remains NO-GO |

## Package Decision

Decision: `GO` for recording P9-08 as a docs-only existing workbook identity
authorization input package.

Decision: `NO-GO` for focused existing-workbook implementation start.

Decision: `NO-GO` for workbook open, close, discard, fixture creation,
fixture mutation, existing workbook path-open lifecycle implementation,
production code changes, test code changes, implementation test execution,
and VBProject mutation expansion during P9-08.

P9-08 records that the exact local test-owned existing workbook identity and
operation-level lifecycle authorization are still absent from the task input
and repository evidence. The missing values are not inferred from repository
paths, active Excel state, file names, historical fixtures, or default
workbook assumptions.

## Required Owner Inputs For Later Re-Evaluation

A later GO / NO-GO record may re-evaluate implementation only if the task
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

- implementation start by P9-08;
- production code changes by P9-08;
- test code additions or updates by P9-08;
- implementation test execution by P9-08;
- workbook open, creation, save, SaveAs, close, discard, restore, backup,
  replacement, deletion, repair, conversion, fixture mutation, or Excel
  instance control by P9-08;
- existing-workbook path-open lifecycle implementation by P9-08;
- workbook or VBProject mutation expansion by P9-08;
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

**P9-09 - Existing Workbook Identity Authorization Package GO / NO-GO**

Selection basis:

- P9-08 records the required identity and lifecycle authorization inputs, and
  records that the current task input does not supply them;
- the next smallest safe step is a docs-only GO / NO-GO record that either
  applies owner-supplied values to approve a later separate
  implementation-start task, or keeps implementation NO-GO with the remaining
  missing inputs;
- implementation remains premature until exact workbook identity,
  operation-level lifecycle authorization, cleanup policy, component-state
  policy, and focused verification authorization are complete.

P9-09 must remain docs-only unless a separate task explicitly changes that
scope. P9-08 does not grant implementation GO for P9-09.

## Verification

P9-08 verification is documentation-only:

- reviewed P9-02, P9-04, P9-06, and P9-07 records;
- reviewed backlog, current-status, and handoff state;
- confirmed no exact local test-owned existing workbook identity or
  operation-level lifecycle authorization is supplied by this task input;
- confirmed this task grants no implementation, test change, workbook
  operation, fixture mutation, or VBProject mutation GO in P9-08;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.

No implementation tests are required or run for P9-08.
