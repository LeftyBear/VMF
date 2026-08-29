# P7-15 - Deferred Failure / Rollback / Readback Candidate Selection

## Status

COMPLETE / docs-only deferred candidate selection and GO / NO-GO record

## Purpose

Evaluate the P7-11-E through P7-11-K deferred focused coverage items after
the P7-13 implementation closeout and select the next smallest candidate
without granting implementation GO.

P7-15 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-11 defined focused coverage cases P7-11-A through P7-11-L.
- P7-12 selected P7-11-A, P7-11-B, P7-11-C, P7-11-D, and P7-11-L as the
  minimum implementation slice and deferred P7-11-E through P7-11-K.
- P7-13 implemented only the P7-12 selected P7-11-A/B/C/D/L slice.
- P7-14 closed out P7-13 docs-only and recorded P7-11-E through P7-11-K as
  still deferred.
- This P7-15 task is explicitly docs-only and provides no implementation,
  workbook operation, or VBProject mutation authorization.

## Preserved Boundary

Any later candidate selected from P7-11-E through P7-11-K must preserve the
P7-07 / P7-13 create-only missing-module boundary:

- consume only an already successful output write plan;
- require an explicitly supplied target VBProject;
- require local-only, test-owned focused verification;
- complete trust/access and component preflight before mutation;
- permit only create-only missing-module mutation for supported standard and
  class modules;
- hard-stop before mutation for invalid input or requested existing modules;
- require readback verification before success;
- roll back only components created by the current operation after a
  post-preflight mutation or readback failure;
- report no partial success.

## Priority Order

Decision priority for the remaining deferred items is:

1. Failure coverage before mutation.
2. Readback coverage after successful mutation.
3. Rollback coverage after post-preflight failure.

Failure coverage before mutation has the lowest operational risk because it
can prove fail-closed behavior without requiring a successful mutation,
readback fault injection, or rollback failure injection. Readback coverage is
next because it validates success-prevention after mutation but requires
controlled readback faults. Rollback coverage is highest risk because it
requires controlled failure after mutation and must prove that cleanup touches
only components created by the current operation.

## P7-11-E Through P7-11-K Evaluation

| Case | Selection result | Reason |
| --- | --- | --- |
| P7-11-E | Selected for next minimum candidate | Unsupported module kind is a pre-mutation failure case. It strengthens the supported-kind boundary without requiring mutation, readback fault injection, or rollback behavior. |
| P7-11-F | Selected for next minimum candidate | Empty or missing generated source is a pre-mutation failure case. It protects against invalid source writes without requiring mutation, readback fault injection, or rollback behavior. |
| P7-11-G | Deferred | Target VBProject component access failure is important, but it depends on controlled trust/access or adapter failure behavior and should be isolated from source-shape validation. |
| P7-11-H | Deferred | Component creation failure occurs after mutation starts and therefore requires rollback proof. It depends on the rollback boundary and is not the next smallest candidate. |
| P7-11-I | Deferred | Readback-missing failure requires controlled post-mutation readback fault injection and rollback proof. It should follow pre-mutation invalid-input coverage. |
| P7-11-J | Deferred | Readback mismatch or wrong-kind failure requires controlled readback fault injection after mutation. It belongs with a later readback-focused candidate. |
| P7-11-K | Deferred | Incomplete rollback failure requires controlled rollback failure behavior and must remain later than ordinary post-mutation failure coverage. |

## Selected Next Minimum Candidate

Decision: select P7-11-E and P7-11-F as the next smallest later candidate.

The selected candidate is limited to pre-mutation invalid write-unit coverage:

- unsupported module kind in an otherwise complete plan;
- empty or missing generated source in an otherwise complete plan;
- hard-stop before mutation;
- no target component creation;
- no rollback requirement because mutation must not start;
- no success result and no partial success.

This selection does not authorize implementation. A later implementation-start
task must separately name exact editable files, fixture assumptions,
verification commands, and GO / NO-GO boundaries before any production code,
test code, workbook operation, or VBProject mutation may begin.

## Dependency Order

The remaining deferred items should be handled in this order unless a later
authorization package records a different decision:

1. P7-11-E / P7-11-F pre-mutation invalid write-unit failures.
2. P7-11-G target VBProject component access failure.
3. P7-11-I / P7-11-J readback failure and mismatch coverage.
4. P7-11-H post-preflight creation failure rollback coverage.
5. P7-11-K incomplete rollback failure coverage.

P7-11-G remains separate because access failure may require a different
fixture or adapter control point than invalid write-unit validation. P7-11-I
and P7-11-J should precede rollback-failure coverage because readback failure
defines a normal rollback trigger. P7-11-K must remain last because it proves
the failure reporting behavior when cleanup itself cannot be completed.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-15 as docs-only deferred candidate selection.

Decision: `GO` for selecting P7-11-E and P7-11-F as the next smallest later
candidate.

Decision: `NO-GO` for implementation in P7-15.

Decision: `NO-GO` for production code or test code changes in P7-15.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-15.

Decision: `NO-GO` for readback fault injection, rollback fault injection, or
post-preflight mutation-failure implementation in P7-15.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 create-only
missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-15:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- overwrite, delete, rename, import, export, or arbitrary component creation;
- mutation of production workbooks or real user data;
- readback fault injection implementation;
- rollback fault injection implementation;
- post-preflight mutation-failure implementation;
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

## Verification Performed

P7-15 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-12 implementation slice selection;
- reviewed P7-14 implementation closeout;
- evaluated P7-11-E through P7-11-K;
- selected P7-11-E and P7-11-F as the next smallest later candidate;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
