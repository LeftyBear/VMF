# P7-23 - Readback Failure / Rollback Dependency Candidate Selection

## Status

COMPLETE / docs-only remaining deferred candidate selection and GO / NO-GO record

## Purpose

Re-evaluate the P7-11-H, P7-11-I, P7-11-J, and P7-11-K remaining deferred
items after P7-21 / P7-22, considering the dependency order:

1. create-only mutation;
2. readback failure;
3. rollback;
4. rollback failure.

P7-23 is documentation only. It does not start implementation, does not change
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
- P7-15 selected P7-11-E/F as the next deferred pre-mutation failure
  candidate.
- P7-17 implemented P7-11-E/F and P7-18 closed that work out.
- P7-19 selected P7-11-G as the next remaining deferred candidate.
- P7-21 implemented P7-11-G and P7-22 closed that work out.
- P7-11-H, P7-11-I, P7-11-J, and P7-11-K remain deferred.
- This P7-23 task is explicitly docs-only and provides no implementation,
  workbook operation, or VBProject mutation authorization.

## Preserved Boundary

Any later candidate selected from P7-11-H, P7-11-I, P7-11-J, and P7-11-K must
preserve the P7-07 / P7-13 / P7-17 / P7-21 create-only missing-module
boundary:

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

## Dependency And Risk Re-evaluation

The remaining deferred items now all require mutation to start or a rollback
trigger to exist. Their risk differs by where the injected failure occurs.

| Case | Dependency profile | Risk |
| --- | --- | --- |
| P7-11-I | Successful create-only mutation occurs first; readback then misses a current-operation component; rollback must remove created components. | Lowest remaining risk because readback is already a success gate, the failure trigger is after mutation rather than inside creation sequencing, and rollback is the normal cleanup path. |
| P7-11-J | Successful create-only mutation occurs first; readback then returns mismatched content or the wrong supported kind; rollback must remove created components. | Similar to P7-11-I and should remain in the same readback-focused slice because it exercises the same success-gate and rollback-trigger boundary with a different readback mismatch condition. |
| P7-11-H | Mutation sequencing fails after at least one current-operation component was created; rollback must remove only components created by the current operation. | Higher than readback failure because the failure occurs inside creation sequencing and requires controlled post-preflight creation-failure injection. |
| P7-11-K | A prior rollback-triggering failure occurs, then rollback cannot remove one or more current-operation components. | Highest remaining risk because it requires both a rollback trigger and controlled rollback failure behavior, and it validates incomplete cleanup reporting. |

Readback failure coverage should precede mutation-sequencing failure and
rollback-failure coverage. P7-11-I/J prove that success remains gated on
complete readback and that a normal post-mutation readback failure triggers
rollback. P7-11-H should remain separate because it validates failure during
component creation rather than after creation during verification. P7-11-K
must remain last because it depends on a rollback trigger and then a separate
cleanup failure.

## P7-11-H Through P7-11-K Evaluation

| Case | Selection result | Reason |
| --- | --- | --- |
| P7-11-H | Deferred | It requires controlled failure during component creation after mutation has already started. That should follow ordinary readback-failure rollback trigger coverage. |
| P7-11-I | Selected for next minimum candidate | It is the smallest remaining rollback-triggering case: mutation succeeds, readback misses a current-operation component, success is denied, and rollback is attempted. |
| P7-11-J | Selected for next minimum candidate | It shares the same readback-gate and rollback-trigger boundary as P7-11-I, using mismatch or wrong-kind readback rather than missing readback. Keeping I/J together avoids splitting equivalent readback failure semantics. |
| P7-11-K | Deferred | It requires an established rollback-triggering failure and controlled rollback failure behavior. It must remain later than normal readback-triggered rollback coverage. |

## Selected Next Minimum Candidate

Decision: select P7-11-I and P7-11-J as the next smallest later candidate.

The selected candidate is limited to readback failure after successful
create-only missing-module mutation:

- all preflight checks pass before mutation;
- create-only mutation is limited to supported missing standard and class
  modules;
- controlled readback reports a missing current-operation component, mismatched
  generated source, or wrong supported module kind;
- the operation reports failure and never reports partial success;
- rollback attempts to remove only components created by the current operation;
- unrelated pre-existing components are not removed, rewritten, renamed, or
  counted as mutations.

This selection does not authorize implementation. A later implementation GO /
NO-GO task must separately name exact editable files, fixture or adapter
control assumptions, verification commands, and safety stops before any
production code, test code, workbook operation, or VBProject mutation may
begin.

## Remaining Deferred Order

After P7-11-I/J, the remaining deferred items should be handled in this order
unless a later authorization package records a different decision:

1. P7-11-H post-preflight component creation failure rollback coverage.
2. P7-11-K incomplete rollback failure coverage.

P7-11-H remains separate because it validates rollback after failure during
mutation sequencing. P7-11-K remains last because it validates failure
reporting when rollback itself cannot fully clean up.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-23 as docs-only remaining deferred candidate
selection.

Decision: `GO` for selecting P7-11-I and P7-11-J as the next smallest later
candidate.

Decision: `NO-GO` for implementation in P7-23.

Decision: `NO-GO` for production code or test code changes in P7-23.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-23.

Decision: `NO-GO` for readback fault injection, rollback fault injection, or
post-preflight mutation-failure implementation in P7-23.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 / P7-21
create-only missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-23:

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

P7-23 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-19 remaining deferred candidate selection;
- reviewed P7-22 target component access failure implementation closeout;
- re-evaluated P7-11-H, P7-11-I, P7-11-J, and P7-11-K after P7-21 / P7-22;
- selected P7-11-I/J as the next smallest later candidate;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
