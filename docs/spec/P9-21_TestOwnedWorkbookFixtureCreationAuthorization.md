# P9-21 - Test-Owned Workbook Fixture Creation Authorization

## Status

COMPLETE / docs-only test-owned workbook fixture creation authorization record

## Purpose

Record the owner authorization input that permits later creation of one
test-owned workbook fixture for P9 existing-workbook identity / lifecycle
focused verification.

P9-21 is documentation only. It does not create the fixture, start focused
existing-workbook implementation, change production code or test code, run
implementation tests, open / create / save / SaveAs / close / discard /
restore any workbook, mutate any workbook or VBProject, update package or
`dist` artifacts, perform release or publication work, access external
services, or change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P8 is COMPLETE only for the narrow local-only test-owned workbook /
  create-only VBProject mutation flow.
- P9-01 through P9-20 are COMPLETE as docs-only predecessor records for
  actual existing-workbook mutation expansion.
- P9-20 selected `WAIT - Owner Workbook Authorization Inputs` and suppressed
  further same-reason follow-up until the owner supplied workbook
  authorization inputs.
- The current owner input supplies a fixture-creation-only authorization for
  one exact repository test fixture path.
- The current owner input explicitly does not authorize P9 focused
  existing-workbook implementation start.

## Authorized Fixture Identity

Only the following fixture identity is authorized for later creation:

| Field | Authorized value |
| --- | --- |
| Fixture path | `C:\Users\biz\Documents\Project\VMF\tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` |
| Repository-relative path | `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` |
| Fixture type | `.xlsm` |
| Ownership | test-owned / owner-approved |
| Production workbook | No |
| Repository fixture | Yes |
| Purpose | P9 existing-workbook identity / lifecycle focused verification |

No other workbook path, workbook identity, directory scan result, active
workbook, recent file, fallback fixture, production workbook, or business
workbook is authorized.

## Authorized Later Creation Operation

A later fixture creation task may perform only the following operations if
separately approved by a fixture creation GO / NO-GO decision:

- create parent directory `tests\fixtures\workbooks` if missing;
- create a new blank macro-enabled workbook named
  `P9_TestOwnedWorkbook.xlsm`;
- add minimal visible marker sheet or metadata only if needed for fixture
  identity;
- save once at the exact authorized fixture path;
- close the workbook after creation.

The initial save of the newly created workbook is authorized only for fixture
creation. Any subsequent unintended dirty state is a failure state. Saving
after unintended mutation and `SaveAs` to any other path remain prohibited.

## Retention Policy

The created fixture is intended to be retained as a repository test fixture:

- retain created fixture: Yes;
- delete after creation: No;
- manual operator review before later mutation: Required;
- later write / mutation authorization: Not included in this approval.

## Prohibited Operations

The following remain prohibited by P9-21:

- fixture creation during P9-21;
- focused existing-workbook implementation start;
- existing business workbook open, save, or mutation;
- workbook auto-discovery;
- workbook fallback selection;
- non-authorized workbook creation;
- VBProject code injection;
- production module import / export;
- existing repository source code mutation;
- Build.xlam or `.xlam` package mutation;
- package / `dist` update;
- release or publication;
- external service access;
- Frozen specification, public API, persisted schema, or canonical format
  change.

## Verification Authorization

After later fixture creation, the authorized verification is limited to:

- file exists at the exact authorized path;
- file extension is `.xlsm`;
- workbook can be identified as test-owned fixture;
- no other workbook files were created;
- `git status --short`;
- `git diff --check` for docs / source changes if any.

Implementation tests, workbook mutation tests, VBProject mutation tests, and
full regression tests are not authorized by this fixture-creation-only input.

## Decision

Decision: `GO` for recording P9-21 as a docs-only owner authorization record
for future test-owned workbook fixture creation.

Decision: `NO-GO` for creating
`tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` during P9-21.

Decision: `NO-GO` for P9 focused existing-workbook implementation start.

Decision: `NO-GO` for existing workbook mutation implementation, VBProject
mutation implementation, focused verification implementation, implementation
tests, workbook mutation tests, VBProject mutation tests, full regression
tests, package / `dist`, release, publication, external service operations,
public API changes, persisted schema changes, canonical format changes, or
Frozen specification changes during P9-21.

This authorization resolves the missing exact fixture identity and
fixture-retention input only for a later fixture creation GO / NO-GO decision.
It does not satisfy or replace the separate implementation-start GO / NO-GO
boundary.

## Selected Next Candidate

Selected next candidate:

**P9-22 - Test-Owned Workbook Fixture Creation GO / NO-GO**

The next candidate should decide whether to execute only the authorized
fixture creation operation for
`tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`. It must not start P9
focused existing-workbook implementation or authorize later workbook /
VBProject mutation.

## Verification

P9-21 verification is documentation-only:

- reviewed P9-20 state and owner-input waiting boundary;
- recorded the exact owner-authorized fixture identity and fixture-only
  creation operation;
- confirmed fixture creation is not performed by P9-21;
- confirmed P9 focused existing-workbook implementation start remains NO-GO;
- required post-edit verification: `git diff --check` and Markdown trailing
  whitespace confirmation.

No workbook, Excel, VBProject, implementation test, package / `dist`, release,
publication, external service, public API, schema, canonical format, or Frozen
specification operation is required or run for P9-21.
