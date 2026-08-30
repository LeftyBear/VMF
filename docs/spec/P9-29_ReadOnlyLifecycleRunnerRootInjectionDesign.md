# P9-29 - Read-Only Lifecycle Runner Root Injection Design

## Status

COMPLETE / docs-only read-only lifecycle runner root injection design

## Purpose

Define the minimum design boundary for a later read-only existing-workbook
lifecycle runner that can receive its repository root explicitly instead of
deriving the root from process state, current directory, active workbook,
recent files, or fallback search.

P9-29 is documentation only. It does not implement the runner, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, automate Excel, mutate the
fixture, mutate any workbook or VBProject, inject code, import or export
modules, update package or `dist` artifacts, perform release or publication
work, access external services, or change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Starting State

- P9-21 through P9-26 authorize, create, and verify exactly one test-owned
  repository workbook fixture:
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`.
- P9-26 verification records the fixture length as `3532` bytes and SHA-256 as
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`.
- P9-27 records GO for a later separate read-only existing-workbook lifecycle
  focused test implementation slice: exact path required, read-only open only,
  identity reconfirmation, lifecycle evidence, close without saving, and
  hard-stops for missing / mismatched fixture identity, writable mode, Save,
  SaveAs, mutation, or fallback workbook selection.
- This checkout contains no `docs/spec/P9-28_*` record. P9-29 therefore does
  not claim P9-28 completion and does not infer that the read-only lifecycle
  implementation start has occurred.
- P9 focused existing-workbook mutation implementation start remains NO-GO.

## Design Problem

A read-only lifecycle runner must locate the authorized fixture from an
explicitly supplied repository root. Root derivation from ambient process
state is not acceptable because it can silently change the target workbook.

The later runner must not use:

- the current working directory as an implicit repository root;
- the active workbook or active Excel application state;
- recent file lists, default document folders, environment-specific search
  roots, or recursive workbook discovery;
- fallback workbook selection when the authorized fixture path is missing;
- writable open mode, Save, SaveAs, repair, conversion, restore, backup,
  replacement, deletion, or fixture mutation.

## Root Injection Contract

The later implementation may introduce only an internal, focused runner input
shape for tests and local verification. It must not change any Frozen
specification, public API, persisted schema, canonical format, package
artifact, release contract, or external service behavior.

The runner input must carry:

| Input | Required boundary |
| --- | --- |
| Repository root | Explicit absolute local path supplied by the test or caller. Blank, relative, missing, or non-existent roots hard-stop before workbook operation. |
| Fixture relative path | Fixed as `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`. The later implementation must not accept alternate workbook names by fallback. |
| Expected file identity | P9-26 length `3532` bytes and SHA-256 `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`, or an equivalent exact identity check recorded by the later implementation-start task. |
| Open mode | Read-only only. Any writable, save-capable, repair, conversion, or SaveAs authorization hard-stops. |
| Lifecycle operations | Confirm root, resolve exact fixture path, verify identity, open read-only, reconfirm workbook identity, record evidence, and close without saving. |

Root injection must produce the authorized fixture path by joining the explicit
repository root with the fixed fixture relative path. It must then prove that
the resolved path is still within the supplied repository root and equals the
authorized fixture path for this checkout. If either condition fails, the
runner must hard-stop before Excel automation or workbook operation.

## Evidence Requirements

The later runner evidence should record, at minimum:

- explicit repository root supplied;
- resolved authorized fixture path;
- fixture identity check result before open;
- read-only open posture;
- workbook identity reconfirmation after open;
- absence of Save, SaveAs, writable open, mutation, VBProject access, code
  injection, module import / export, fallback selection, and fixture rewrite;
- close-without-saving attempt and result;
- fixture identity check result after close when feasible.

Evidence must distinguish `PASS`, `HardStop`, and operator-review-required
states. Partial lifecycle success must not be reported as complete.

## Safety Stops For Later Implementation

The later implementation-start task must stop before implementation or before
workbook operation if any of the following is true:

- the explicit repository root is absent, relative, unresolved, or outside the
  current VMF checkout;
- the exact fixture
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm` is missing or no longer
  matches the P9-26 identity evidence;
- the implementation would require workbook auto-discovery, active workbook
  selection, recent-file selection, fallback fixture selection, or production
  workbook operation;
- the implementation would require Save, SaveAs, writable open mode, fixture
  mutation, workbook repair, conversion, restore, backup, replacement, or
  deletion;
- the implementation would require VBProject mutation, code injection, module
  import / export, Trust Center changes, credentials, token stores, or external
  services;
- the implementation would require changing Frozen specifications, public
  APIs, persisted schemas, canonical formats, package / `dist`, release, or
  publication state;
- focused verification cannot prove that the fixture remains unchanged.

## Decision

Decision: `GO` for recording P9-29 as a docs-only read-only lifecycle runner
root injection design.

Decision: `GO` for a later separate GO / NO-GO decision to evaluate whether
this design is sufficient for implementation.

Decision: `NO-GO` for implementing the runner during P9-29.

Decision: `NO-GO` for treating P9-29 as evidence that P9-28 exists or is
complete in this checkout.

Decision: `NO-GO` for workbook open, workbook create, workbook save, workbook
SaveAs, workbook close, workbook discard, workbook restore, Excel automation,
fixture mutation, workbook mutation, or VBProject mutation during P9-29.

Decision: `NO-GO` for P9 focused existing-workbook mutation implementation
start. P9-29 only designs read-only lifecycle root injection.

Decision: `NO-GO` for package / `dist`, release, publication, external service
operations, public API changes, persisted schema changes, canonical format
changes, or Frozen specification changes during P9-29.

## Selected Next Candidate

Selected next candidate:

**P9-30 - Read-Only Lifecycle Runner Root Injection GO / NO-GO**

P9-30 should decide whether the P9-29 root-injection design is sufficient for
a later separate implementation-start task. P9-30 must not infer workbook
mutation, VBProject mutation, code injection, module import / export, Save,
SaveAs, release, package, or publication authorization from this design.

## Verification

P9-29 verification is documentation-only:

- reviewed P9-26 post-creation fixture verification;
- reviewed P9-27 read-only lifecycle focused test implementation GO / NO-GO;
- reviewed current `AppOutputWriteService.cls` and
  `AppOutputWriteBoundaryTests.bas` lifecycle boundaries as design context;
- confirmed no P9-28 spec record exists in this checkout;
- recorded the root-injection design without implementation, test execution,
  workbook operation, fixture mutation, VBProject mutation, package / `dist`,
  release, publication, external service, public API, schema, canonical format,
  or Frozen specification operation;
- required post-edit verification: `git diff --check` and Markdown trailing
  whitespace confirmation.

No implementation tests are required or run for P9-29.
