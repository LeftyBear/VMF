# P7-19 - Remaining Deferred Failure / Readback / Rollback Candidate Selection

## Status

COMPLETE / docs-only remaining deferred candidate selection and GO / NO-GO record

## Purpose

Re-evaluate P7-11-G through P7-11-K after P7-17 / P7-18 and select the next
smallest candidate while considering the dependency order between failure,
readback, and rollback behavior.

P7-19 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-11 defined focused coverage cases P7-11-A through P7-11-L.
- P7-12 selected P7-11-A/B/C/D/L as the first minimum implementation slice and
  deferred P7-11-E through P7-11-K.
- P7-13 implemented P7-11-A/B/C/D/L and P7-14 closed that work out.
- P7-15 selected P7-11-E/F as the next deferred failure candidate and kept
  P7-11-G through P7-11-K deferred.
- P7-16 recorded GO for a later separate implementation-start task limited to
  P7-11-E/F.
- P7-17 implemented P7-11-E/F pre-mutation failure coverage in
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas` only.
- P7-18 closed out P7-17 and kept P7-11-G through P7-11-K deferred.
- This P7-19 task is explicitly docs-only and provides no implementation,
  workbook operation, or VBProject mutation authorization.

## Preserved Boundary

Any later candidate selected from P7-11-G through P7-11-K must preserve the
P7-07 / P7-13 / P7-17 create-only missing-module boundary:

- consume only an already successful output write plan;
- require an explicitly supplied target VBProject;
- require local-only, test-owned focused verification;
- complete trust/access and component preflight before mutation;
- permit only create-only missing-module mutation for supported standard and
  class modules;
- hard-stop before mutation for invalid input, inaccessible target state, or
  requested existing modules;
- require readback verification before success;
- roll back only components created by the current operation after a
  post-preflight mutation or readback failure;
- report no partial success.

## Dependency Re-evaluation

The remaining deferred items have different dependency weight:

| Case | Dependency profile | Risk |
| --- | --- | --- |
| P7-11-G | Pre-mutation target component access failure; depends on controlled target-access failure or adapter behavior, but does not require mutation, readback fault injection, or rollback fault injection. | Lowest remaining risk because no component should be created and rollback is not required. |
| P7-11-I | Post-mutation readback misses a component; depends on successful create-only mutation, controlled readback fault injection, and rollback proof. | Higher than P7-11-G because mutation starts and rollback must be verified. |
| P7-11-J | Post-mutation readback returns mismatched content or wrong supported kind; depends on successful create-only mutation, controlled readback mismatch or kind fault injection, and rollback proof. | Higher than P7-11-G and similar to P7-11-I because mutation starts and rollback must be verified. |
| P7-11-H | Component creation fails after at least one current-operation component was created; depends on post-preflight creation-failure injection and rollback proof. | Higher than readback planning unless the rollback trigger behavior is already isolated, because the fault occurs during mutation sequencing. |
| P7-11-K | Rollback cannot remove one or more current-operation components; depends on a prior rollback-triggering failure and controlled rollback failure behavior. | Highest remaining risk because it verifies cleanup failure reporting after mutation and after a rollback trigger. |

P7-11-G should remain separate from P7-11-I/J/H/K because it proves a
pre-mutation hard stop at the target-access boundary. It is the only remaining
deferred case that can be selected without requiring successful mutation,
readback fault injection, rollback execution, or rollback failure injection.

Readback failure coverage should precede rollback-failure coverage because
readback failure is a normal rollback trigger. Rollback-incomplete behavior
should remain last because it depends on a rollback trigger and then a
separate cleanup failure.

## P7-11-G Through P7-11-K Evaluation

| Case | Selection result | Reason |
| --- | --- | --- |
| P7-11-G | Selected for next minimum candidate | It is the remaining pre-mutation failure boundary. It should hard-stop before mutation when target VBProject component access fails, creates no component, requires no readback fault injection, and requires no rollback. |
| P7-11-H | Deferred | It occurs after mutation starts and requires controlled creation-failure injection plus rollback proof. It is not the next smallest candidate after E/F. |
| P7-11-I | Deferred | It requires successful mutation, controlled readback-missing fault injection, and rollback proof. It should follow target-access failure coverage. |
| P7-11-J | Deferred | It requires successful mutation, controlled readback mismatch or wrong-kind fault injection, and rollback proof. It belongs with a later readback-focused candidate. |
| P7-11-K | Deferred | It requires controlled rollback failure after a rollback trigger and must remain later than ordinary access, readback, and creation-failure coverage. |

## Selected Next Minimum Candidate

Decision: select P7-11-G as the next smallest later candidate.

The selected candidate is limited to target VBProject component access failure
during preflight:

- component enumeration or equivalent target component access fails before
  mutation;
- the operation hard-stops before creating any target component;
- rollback is not required because mutation must not start;
- no readback verification is attempted as a success condition;
- no success result and no partial success are reported.

This selection does not authorize implementation. A later implementation GO /
NO-GO task must separately name exact editable files, fixture or adapter
control assumptions, verification commands, and safety stops before any
production code, test code, workbook operation, or VBProject mutation may
begin.

## Remaining Deferred Order

After P7-11-G, the remaining deferred items should be handled in this order
unless a later authorization package records a different decision:

1. P7-11-I / P7-11-J readback-missing and readback mismatch / wrong-kind
   failure coverage.
2. P7-11-H post-preflight component creation failure rollback coverage.
3. P7-11-K incomplete rollback failure coverage.

P7-11-I/J are grouped because both validate readback as a success gate and a
rollback trigger. P7-11-H remains separate because it validates failure during
mutation sequencing. P7-11-K remains last because it validates failure
reporting when rollback itself cannot fully clean up.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-19 as docs-only remaining deferred candidate
selection.

Decision: `GO` for selecting P7-11-G as the next smallest later candidate.

Decision: `NO-GO` for implementation in P7-19.

Decision: `NO-GO` for production code or test code changes in P7-19.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-19.

Decision: `NO-GO` for readback fault injection, rollback fault injection, or
post-preflight mutation-failure implementation in P7-19.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 create-only
missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-19:

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

P7-19 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-12 implementation slice selection;
- reviewed P7-15 deferred candidate selection;
- reviewed P7-18 implementation closeout;
- re-evaluated P7-11-G through P7-11-K after P7-17 / P7-18;
- selected P7-11-G as the next smallest later candidate;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
