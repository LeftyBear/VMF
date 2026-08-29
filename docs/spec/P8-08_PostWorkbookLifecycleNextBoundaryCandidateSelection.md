# P8-08 - Post-Workbook Lifecycle Next Boundary Candidate Selection

## Status

COMPLETE / docs-only next boundary candidate selection

## Purpose

Select the minimum next boundary candidate after P8-07 and clarify what remains
before the Real Workbook / VBProject Mutation flow can be judged complete.

P8-08 is documentation only. It does not grant implementation GO, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, mutate any workbook or
VBProject, update package or `dist` artifacts, perform release or publication
work, access external services, or change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Starting State

P8-07 is COMPLETE and committed / pushed as
`c3e9137840830a1340f218027e07e2750513df8c`.

P8-07 closes out P8-06 after commit
`fe3edf29774b8f73e419759ca1ea411eda57181c`. P8-06 added the narrow workbook
lifecycle authorization / handoff helper and focused tests in:

- `src/Build/Application/AppOutputWriteService.cls`
- `tests/unit/Build/AppOutputWriteBoundaryTests.bas`

The completed P8 workbook lifecycle implementation proves:

- exact test-owned workbook object identity is required;
- lifecycle authorization is explicit;
- unauthorized Save and SaveAs remain denied;
- workbook lifecycle evidence is attached to the mutation result;
- `VBProject` handoff occurs only after lifecycle authorization;
- no-save close remains the only lifecycle operation retained by the focused
  fixture cleanup;
- the existing create-only real VBProject mutation boundary remains the
  mutation executor.

## Remaining Boundary Review

P8-07 completes the first workbook lifecycle authorization and handoff slice.
It does not complete every boundary needed to judge the Real Workbook /
VBProject Mutation flow complete.

The remaining boundary areas are:

| Boundary area | Current state after P8-07 | Remaining decision need |
| --- | --- | --- |
| Workbook lifecycle authorization | Implemented only for a test-owned newly-created fixture and no-save close cleanup. | Decide whether P8 completion requires broader existing-workbook, save, close, restore, or production-workbook lifecycle handling, or explicitly defers them. |
| VBProject mutation boundary | Existing P7 create-only missing standard / class module mutation remains the executor. | Decide whether create-only missing-module mutation is sufficient for P8 completion or whether add / replace / remove / rename / import / export require separate later phases. |
| Component mutation responsibility | Create-only addition of supported missing modules is implemented and covered. | Fix whether component replace / remove / overwrite responsibilities remain NO-GO and outside P8 completion. |
| Component rollback | Current-operation created components are rolled back after post-preflight mutation or readback failure; incomplete rollback is operator-review-required. | Confirm whether this rollback model is sufficient for P8 completion when workbook lifecycle evidence is present. |
| Workbook lifecycle rollback | Lifecycle rollback remains separate from component rollback and is not broadened beyond authorized no-save fixture cleanup. | Decide whether workbook save / restore / discard rollback is needed for P8 completion or must be deferred behind a later GO gate. |
| Readback / verification | Mutation readback is mandatory before success; P8-06 attaches lifecycle evidence around the existing mutation result. | Confirm final success / failure criteria across lifecycle authorization, mutation, readback, rollback, and operator-review evidence. |
| Actual workbook mutation GO gate | P8-06 uses a local test-owned workbook fixture and existing real VBProject mutation path. | Fix whether any future actual workbook mutation expansion needs a new GO / NO-GO gate, exact editable files, exact workbook identity, and verification commands. |
| P8 completion criteria | Not yet fixed in a single record after P8-07. | Define the exact criteria for judging P8 COMPLETE versus selecting another implementation or docs-only boundary. |

## Candidate Options Considered

### Candidate A - Real Workbook / VBProject Mutation Flow Completion Criteria Planning

Docs-only record that fixes the P8 completion criteria after P8-07. It would
classify remaining lifecycle, mutation, rollback, readback, final-status, and
actual workbook mutation GO-gate boundaries as either satisfied by the current
P7 / P8 implementation or deferred to named later candidates.

This candidate performs no implementation and grants no actual workbook /
VBProject mutation expansion.

### Candidate B - Component Operation Expansion Boundary

Docs-only record for replace / remove / overwrite / rename / import / export
responsibility. This is higher risk because those operations can destroy or
rewrite existing workbook state and depend on completion criteria and renewed
authorization.

### Candidate C - Workbook Save / Restore Lifecycle Boundary

Docs-only record for save, SaveAs, discard, restore, backup, and recovery
semantics. This is higher risk because it changes persisted-state reasoning and
must not be inferred from the test-owned no-save fixture path.

### Candidate D - Actual Workbook Mutation Implementation GO / NO-GO

Docs-only GO / NO-GO decision for a later implementation slice. This is
premature before P8 completion criteria and deferred boundaries are fixed.

## Selected Next Minimum Candidate

Selected next minimum candidate:

**P8-09 - Real Workbook / VBProject Mutation Flow Completion Criteria Planning**

Selection basis:

- P8-07 completes the workbook lifecycle authorization / handoff focused slice,
  but no single record now fixes the P8 COMPLETE criteria;
- the next smallest safe step is to decide, docs-only, whether the current
  lifecycle authorization, create-only mutation, component rollback, readback,
  and final failure evidence are sufficient to close P8 or whether additional
  named boundaries are required;
- component replace / remove / overwrite / rename / import / export and
  workbook save / restore are destructive or persistence-affecting areas and
  should not be selected before completion criteria are fixed;
- actual workbook mutation expansion requires a later explicit GO / NO-GO gate
  with exact workbook identity, editable files, safety stops, and verification
  commands.

## Required Scope For P8-09

P8-09 must remain docs-only unless a later task explicitly changes that scope.

P8-09 should:

- review P7 completion records and P8-01 through P8-08;
- inventory which Real Workbook / VBProject Mutation flow boundaries are
  already satisfied by P7 and P8-06 / P8-07;
- define P8 COMPLETE criteria for lifecycle authorization, VBProject mutation,
  component rollback, workbook lifecycle rollback separation, readback /
  verification, final success / failure status, and operator-review evidence;
- decide whether P8 can close after criteria review or whether another named
  P8 candidate is required;
- preserve the rule that any actual workbook mutation expansion requires a
  separate GO / NO-GO record;
- keep fallback workbook selection, implicit target selection, implicit
  authorization, package / `dist`, release / publication, external services,
  public API changes, persisted schema changes, canonical format changes, and
  Frozen specification changes as NO-GO.

## GO / NO-GO Decisions

Decision: `GO` for recording P8-08 as a docs-only next boundary candidate
selection.

Decision: `GO` for selecting P8-09 as the next minimum docs-only candidate.

Decision: `NO-GO` for implementation in P8-08.

Decision: `NO-GO` for production code or test code changes in P8-08.

Decision: `NO-GO` for actual Workbook / VBProject mutation expansion in
P8-08.

Decision: `NO-GO` for component replace, remove, overwrite, delete, rename,
import, export, arbitrary component creation, workbook save, SaveAs, restore,
replacement, deletion, repair, conversion, package / `dist`, release,
publication, external service, public API, persisted schema, canonical format,
or Frozen specification changes.

## Verification

P8-08 verification is documentation-only:

- reviewed P8-01, P8-02, and P8-07 records;
- reviewed current `AppOutputWriteService.AppApplyGeneratedOutputToAuthorizedWorkbook`;
- reviewed current `AppOutputWriteBoundaryTests` lifecycle coverage;
- reviewed backlog, current-status, and handoff state;
- docs-only diff review;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P8-08.
