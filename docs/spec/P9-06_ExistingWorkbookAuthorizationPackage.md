# P9-06 - Existing Workbook Authorization Package

## Status

COMPLETE / docs-only existing workbook authorization package

## Purpose

Create the authorization package selected by P9-05 so focused
existing-workbook implementation can be re-evaluated in a later separate
GO / NO-GO record.

P9-06 is documentation only. It does not start implementation, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, mutate any workbook or
VBProject, create or modify workbook fixtures, update package or `dist`
artifacts, perform release or publication work, access external services, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Starting State

- P8 is COMPLETE only for the narrow local-only test-owned workbook /
  create-only VBProject mutation flow.
- P9-01 through P9-04 are COMPLETE as docs-only planning records for actual
  workbook mutation expansion, workbook identity authorization, focused test
  design, and focused test implementation scope planning.
- P9-05 is COMPLETE as a docs-only implementation GO / NO-GO decision and
  records focused existing-workbook implementation start as NO-GO.
- P9-05 selects P9-06 as the next minimum docs-only candidate because exact
  local test-owned existing workbook identity, existing-workbook path-open
  lifecycle boundary, operation-level lifecycle authorization,
  pre-existing dirty-state policy, target component-state policy, cleanup
  behavior, and focused implementation verification authorization are missing.
- Current repository inspection found no repository-owned `.xlsm`, `.xlsb`,
  `.xlsx`, or `.xlam` existing workbook fixture.

## Authorization Package

| Required item | P9-06 package value | Authorization state |
| --- | --- | --- |
| Accepted predecessor records | P9-01, P9-02, P9-03, P9-04, and P9-05 remain the accepted P9 records. | Fixed for later re-evaluation |
| Exact editable production files | `src/Build/Application/AppOutputWriteService.cls` only. | Candidate fixed for later implementation decision |
| Exact editable test files | `tests/unit/Build/AppOutputWriteBoundaryTests.bas` only. | Candidate fixed for later implementation decision |
| Existing-workbook lifecycle entry boundary | A later task may consider a new narrow path-open lifecycle boundary in `AppOutputWriteService.cls` that opens only an exact authorized existing workbook and hands off only confirmed lifecycle state to `AppApplyGeneratedOutputToAuthorizedWorkbook`. | Candidate boundary fixed; not implementation GO |
| Current in-memory workbook handoff boundary | `AppOutputWriteService.AppApplyGeneratedOutputToAuthorizedWorkbook` remains the existing explicit workbook-object handoff boundary. | Evidence only |
| Current VBProject mutation boundary | `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject` remains the existing create-only missing-module VBProject mutation boundary after preflight. | Evidence only |
| Exact local test-owned existing workbook identity | Not authorized in P9-06. No exact workbook path, file hash, repository fixture, temporary fixture-copy source, or owner-approved workbook identity is available in the task input or repository. | Missing; implementation remains NO-GO |
| Workbook ownership and isolation | Required value for a later GO: local test-owned existing workbook only, isolated from user and production workbooks, excluded from package, `dist`, release, publication, and external service paths. | Required for later owner authorization |
| Workbook selection method | Required value for a later GO: exact absolute path or repository-approved identity supplied by the authorization record; no active workbook, recent-file, name-only, directory-scan, nearest-match, default-fixture, or production-workbook fallback. | Required for later owner authorization |
| Existing workbook open | Not authorized in P9-06. A later GO must explicitly authorize path-open for only the exact test-owned existing workbook identity and must define read-only or editable mode. | Missing; implementation remains NO-GO |
| Identity reconfirmation after open | Required before VBProject handoff in any later implementation. Reconfirmation must compare the opened workbook to the exact authorized identity before mutation can start. | Required for later owner authorization |
| VBProject trust/access preflight | Required before any VBProject access or mutation. Preflight failure must hard-stop before mutation. P9-06 does not authorize Trust Center, macro security, or credential changes. | Required for later owner authorization |
| Macro-enabled format posture | Required value for a later GO: the exact authorized workbook format must support the requested VBProject access, and macro-enabled handling must be explicitly approved. | Required for later owner authorization |
| Protected-view, repair, conversion, external-link, and credential posture | Required value for a later GO: any protected-view, repair, conversion, external-link, credential, or token-store requirement must hard-stop unless separately authorized. | Required for later owner authorization |
| Pre-existing dirty-state policy | Required value for a later GO: pre-existing dirty state hard-stops before mutation unless the authorization record separately defines a safe handling policy. | Required for later owner authorization |
| Target component-state policy | Required value for a later GO: the authorized existing workbook must not already contain target components selected for create-only mutation; conflicts hard-stop before mutation. | Required for later owner authorization |
| Allowed VBProject component operations | Only the existing create-only missing supported module mutation path may be considered by a later GO. Import, export, overwrite, delete, rename, arbitrary component creation, and component rollback redesign remain prohibited. | Candidate fixed for later implementation decision |
| Save, SaveAs, restore, backup, replacement, deletion, repair, and conversion | Prohibited for the minimum P9-06 package unless a later authorization package separately names the operation. | Prohibited |
| No-save close cleanup | Not authorized in P9-06. A later GO must explicitly authorize whether no-save close cleanup is allowed, required, or prohibited for the exact fixture and observed dirty state. | Missing; implementation remains NO-GO |
| Fixture retention and operator review | Required value for a later GO: incomplete lifecycle state, incomplete component rollback, incomplete close / cleanup, or uncertain dirty state must deny success and require operator review. | Required for later owner authorization |
| Readback verification | Required after create-only mutation and before success; readback must verify the exact created component state from the authorized workbook-derived VBProject target. | Candidate fixed for later implementation decision |
| Focused verification command | Candidate command remains the focused Build VBA `AppRunOutputWriteBoundaryTests` path plus `git diff --check`; a later implementation GO must explicitly authorize the exact command and whether all Build VBA runners are required. | Required for later owner authorization |

## Re-Evaluation Decision

Decision: `GO` for recording P9-06 as a docs-only existing workbook
authorization package.

Decision: `NO-GO` for focused existing-workbook implementation start.

Decision: `NO-GO` for workbook open, close, discard, fixture creation,
fixture mutation, existing workbook path-open lifecycle implementation,
production code changes, test code changes, implementation test execution, and
VBProject mutation expansion during P9-06.

P9-06 fixes the package structure and candidate editable files, but it does
not invent or authorize the exact local test-owned existing workbook identity.
Because that identity and operation-level lifecycle authorization are still
missing, implementation remains NO-GO until a later owner-authorized record
names them and a separate GO / NO-GO decision approves an implementation-start
task.

## Minimum Future Implementation Slice

If a later separate implementation GO is granted, the minimum slice may
include only:

- edits to `src/Build/Application/AppOutputWriteService.cls`;
- edits to `tests/unit/Build/AppOutputWriteBoundaryTests.bas`;
- focused local tests for the exact authorized local test-owned existing
  workbook identity;
- minimal test helpers required to provide explicit existing-workbook identity
  and lifecycle authorization inputs;
- a narrow existing-workbook path-open lifecycle boundary only for the exact
  authorized workbook;
- identity reconfirmation after open and before `VBProject` handoff;
- `VBProject` trust/access preflight before mutation;
- handoff to the existing create-only missing supported module mutation path;
- readback verification from the authorized workbook-derived `VBProject`
  target;
- component rollback limited to current-operation created components;
- lifecycle cleanup evidence and operator-review evidence separate from
  component rollback evidence;
- hard-stop coverage for missing, ambiguous, mismatched, fallback-derived,
  dirty, protected-view, inaccessible, locked, production-workbook, or
  unauthorized lifecycle inputs.

The future implementation must not re-derive Blueprint, Manifest, Template,
Template Derivation, GenerateContext, Generator, or Output Write facts.

## Preserved NO-GO Operations

The following remain NO-GO:

- implementation start by P9-06;
- production code changes by P9-06;
- test code additions or updates by P9-06;
- implementation test execution by P9-06;
- workbook open, creation, save, SaveAs, close, discard, restore, backup,
  replacement, deletion, repair, conversion, fixture mutation, or Excel
  instance control by P9-06;
- existing-workbook path-open lifecycle implementation by P9-06;
- workbook or VBProject mutation expansion by P9-06;
- active workbook selection;
- recent-file selection;
- name-only workbook matching;
- directory scanning;
- nearest-match recovery;
- default fixture fallback;
- production workbook fallback;
- real user workbook or production workbook mutation;
- VBProject import, export, overwrite, delete, rename, arbitrary component
  creation, or component rollback redesign;
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

**P9-07 - Existing Workbook Authorization Package GO / NO-GO**

Selection basis:

- P9-06 records the package structure and candidate implementation boundary;
- exact local test-owned existing workbook identity and operation-level
  lifecycle authorization remain missing from the current task input and
  repository state;
- the next smallest safe step is a docs-only GO / NO-GO record that either
  applies owner-supplied workbook identity and lifecycle authorization values
  to approve a later implementation-start task, or keeps implementation
  NO-GO with the remaining missing inputs.

P9-07 must remain docs-only unless a separate task explicitly changes that
scope. P9-06 does not grant implementation GO for P9-07.

## Verification

P9-06 verification is documentation-only:

- reviewed P9-01, P9-02, P9-03, P9-04, and P9-05 records;
- reviewed backlog, current-status, and handoff state;
- reviewed current repository evidence for
  `AppOutputWriteService.AppApplyGeneratedOutputToAuthorizedWorkbook`,
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`,
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas`, and
  `AppRunOutputWriteBoundaryTests`;
- confirmed no repository-owned `.xlsm`, `.xlsb`, `.xlsx`, or `.xlam`
  existing workbook fixture is present;
- confirmed this task grants no implementation, test change, workbook
  operation, fixture mutation, or VBProject mutation GO in P9-06;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.

No implementation tests are required or run for P9-06.
