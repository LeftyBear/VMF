# P7-27 - Remaining Mutation Sequencing / Rollback Candidate Selection

## Status

COMPLETE / docs-only remaining deferred candidate selection and GO / NO-GO record

## Purpose

Re-evaluate the remaining P7-11-H and P7-11-K deferred items after P7-25 /
P7-26 completed P7-11-I/J readback failure rollback coverage.

P7-27 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-11 defined P7-11-H as component creation failure after preflight and
  after at least one current-operation component was created.
- P7-11 defined P7-11-K as rollback failure after rollback cannot remove one
  or more components created by the current operation.
- P7-23 selected P7-11-I/J before P7-11-H/K because ordinary readback-triggered
  rollback had to be proven before mutation-sequencing or rollback-failure
  coverage.
- P7-25 implemented P7-11-I/J controlled readback missing-component and
  mismatched-source rollback coverage.
- P7-26 closed out P7-25 and kept P7-11-H/K deferred.
- This P7-27 task is explicitly docs-only and provides no implementation,
  test change, workbook operation, or VBProject mutation authorization.

## Preserved Boundary

Any later candidate selected from P7-11-H or P7-11-K must preserve the
P7-07 / P7-13 / P7-17 / P7-21 / P7-25 create-only missing-module boundary:

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
- preserve unrelated pre-existing components;
- report no partial success.

## Remaining Candidate Comparison

P7-11-H and P7-11-K both sit after mutation starts, but they have different
dependency and fault-injection requirements.

| Case | Dependency profile | Fault injection need | Mutation / rollback risk |
| --- | --- | --- | --- |
| P7-11-H | Requires successful preflight, creation of at least one current-operation component, then a controlled failure during later component creation. Normal rollback must then remove only current-operation components. | Needs controlled post-preflight component-creation failure injection inside mutation sequencing. It does not require rollback itself to fail. | Medium-high: mutation has already started, so the test must prove cleanup of created components without touching unrelated pre-existing components. The rollback path remains the already-normal cleanup path proven by P7-25. |
| P7-11-K | Requires a rollback-triggering failure first, then a separate controlled failure while removing one or more current-operation components. | Needs both a rollback trigger and controlled rollback-removal failure behavior. | Highest remaining risk: it intentionally leaves cleanup incomplete, so the result must report failure without partial success and without implying the target is safe for blind retry. |

P7-11-H should precede P7-11-K. It is the smaller remaining candidate because
it exercises failure during mutation sequencing while still relying on normal
rollback behavior. P7-11-K should remain last because it requires an already
established rollback trigger plus a second failure inside cleanup, and its
acceptance must cover incomplete rollback reporting and operator-review
semantics.

## Selected Next Minimum Candidate

Decision: select P7-11-H as the next smallest later candidate.

The selected candidate is limited to post-preflight mutation sequencing
failure after at least one current-operation component has been created:

- all preflight checks pass before mutation;
- create-only mutation remains limited to supported missing standard and class
  modules;
- a controlled later component-creation failure occurs after one or more
  current-operation components were created;
- the operation reports failure and never reports partial success;
- rollback attempts to remove only components created by the current
  operation;
- unrelated pre-existing components are not removed, rewritten, renamed, or
  counted as mutations.

This selection does not authorize implementation. A later implementation GO /
NO-GO task must separately name exact editable files, the fault-injection
mechanism, fixture or adapter assumptions, verification commands, and safety
stops before any production code, test code, workbook operation, or VBProject
mutation may begin.

## Remaining Deferred Order

After P7-11-H, the remaining deferred item should be handled last unless a
later authorization package records a different decision:

1. P7-11-K incomplete rollback failure coverage.

P7-11-K remains deferred because rollback-failure coverage depends on both a
rollback trigger and controlled rollback-removal failure behavior. It must not
be combined with P7-11-H unless a later GO / NO-GO record explicitly accepts
that broader risk.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-27 as docs-only remaining deferred candidate
selection.

Decision: `GO` for selecting P7-11-H as the next smallest later candidate.

Decision: `NO-GO` for implementation in P7-27.

Decision: `NO-GO` for production code or test code changes in P7-27.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-27.

Decision: `NO-GO` for component creation failure injection implementation or
rollback fault injection implementation in P7-27.

Decision: `NO-GO` for P7-11-K rollback failure implementation in P7-27.

Decision: `NO-GO` for expanding beyond the P7-07 / P7-13 / P7-17 / P7-21 /
P7-25 create-only missing-module operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Prohibited Operations

The following remain NO-GO in P7-27:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- overwrite, delete, rename, import, export, or arbitrary component creation;
- mutation of production workbooks or real user data;
- component creation failure injection implementation;
- rollback fault injection implementation;
- incomplete rollback failure implementation;
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

P7-27 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- reviewed P7-23 readback failure / rollback dependency candidate selection;
- reviewed P7-26 readback failure coverage implementation closeout;
- re-evaluated P7-11-H and P7-11-K after P7-25 / P7-26;
- selected P7-11-H as the next smallest later candidate;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
