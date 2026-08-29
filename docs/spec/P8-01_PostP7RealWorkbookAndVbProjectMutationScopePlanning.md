# P8-01 - Post-P7 Real Workbook / VBProject Mutation Scope Planning

## Status

COMPLETE / docs-only post-P7 scope planning and candidate fixation

## Purpose

Start P8 from the P7 COMPLETE state and fix the post-P7 real workbook / real
VBProject mutation planning boundary before any implementation GO / NO-GO
decision.

P8-01 is documentation only. It does not add implementation, change production
code or test code, run implementation tests, open / save / close / SaveAs /
restore any workbook, mutate any workbook or VBProject, create or modify
workbook fixtures, update package or `dist` artifacts, perform release or
publication work, access external services, or change public APIs, persisted
schemas, canonical formats, or Frozen specifications.

## Starting State

P7 is COMPLETE.

P7 completed the explicitly authorized minimum local-only real workbook / real
VBProject mutation path and the focused failure / rollback / readback coverage
set:

- create-only missing-module mutation for supported standard and class modules
  on an explicitly supplied test-owned real VBProject target;
- trust/access and target-state preflight before mutation;
- hard-stop before mutation for invalid input, inaccessible target state,
  duplicate requested modules, unsupported module kind, missing / blank
  generated source, or requested existing target modules;
- deterministic multi-module create-only application;
- readback verification before success;
- rollback of current-operation created components after post-preflight
  mutation or readback failure;
- incomplete rollback reporting as failed / `operator-review-required`;
- preservation of unrelated pre-existing components.

No remaining P7-11 deferred focused coverage item is recorded after P7-34, and
P7-35 selected P8-01 as the minimum next-phase docs-only candidate.

## Target Scope Fixed By P8-01

P8 post-P7 planning concerns only future expansion beyond the completed P7
create-only missing-module real VBProject boundary.

Candidate expansion areas are:

- real workbook lifecycle authority: open, close, save, SaveAs, restore, and
  fixture ownership rules;
- real VBProject target acquisition from an authorized workbook rather than an
  already supplied target object;
- module operation expansion beyond create-only missing standard / class
  modules;
- recovery semantics when workbook state, VBProject state, or current-operation
  component state cannot be proven clean after failure;
- readback evidence and operator-facing diagnostics for post-P7 workbook /
  VBProject operations.

P8-01 does not include Publisher, Google Docs / Drive, OAuth, release, package,
or distribution work.

## Responsibility Boundary

The post-P7 boundary is split as follows:

- Generator remains responsible only for generating output from approved
  Generator-ready context.
- Output Write remains responsible for constructing approved output write
  plans and deterministic local-folder writes.
- Real VBProject mutation remains responsible only for applying approved write
  units to an explicitly authorized VBProject target and proving readback
  before success.
- Workbook lifecycle handling is a separate responsibility that must not be
  smuggled into mutation behavior without explicit authorization.
- Restore behavior is a workbook-lifecycle responsibility unless a later
  candidate explicitly defines a narrower mutation-local rollback behavior.
- Operator review is required when rollback or restore cannot prove a clean
  state.

No downstream boundary may compensate for an upstream hard stop. Fallback or
implicit Template selection, Template content inference, and GenerateContext or
Generator compensation remain prohibited.

## Authorized And Unauthorized Operations

Authorized by P8-01:

- create this P8-01 docs-only planning record;
- synchronize backlog, current-status, and handoff documentation;
- inspect repository documentation and source for evidence;
- run documentation diff checks such as `git diff --check` and Git status.

Not authorized by P8-01:

- implementation GO;
- production code change;
- test code change;
- implementation test execution;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- real workbook or real VBProject mutation;
- VBProject import, export, overwrite, delete, rename, or arbitrary component
  creation;
- package / `dist` creation, update, replacement, or inspection;
- release, tag, push, or publication operation;
- external service operation;
- credential or token-store access;
- public API, persisted schema, canonical format, or Frozen specification
  change.

## Failure, Rollback, And Readback Boundary

The P7 failure model remains the baseline:

- failure before mutation must hard-stop before creating, rewriting, deleting,
  renaming, importing, exporting, saving, or restoring anything;
- failure after post-preflight mutation starts must deny success and must not
  report partial mutation;
- rollback is limited to components created by the current operation unless a
  later candidate separately authorizes workbook restore behavior;
- readback is mandatory before any success result;
- readback failure after mutation must trigger rollback of current-operation
  created components;
- incomplete rollback must be reported as failed /
  `operator-review-required`, preserving failure evidence and avoiding a safe
  retry-ready claim;
- unrelated pre-existing components must remain untouched and must not be
  counted as mutations.

Post-P7 workbook lifecycle expansion adds a separate risk boundary: save /
restore behavior can affect workbook identity and persisted state, so it must
not be inferred from P7 component rollback. Any future workbook restore
candidate must define ownership, snapshot / backup rules, restore trigger,
readback after restore, and operator-review behavior before implementation can
be considered.

## Next Minimum Candidate

Selected next minimum candidate:

**P8-02 - Workbook Lifecycle Authorization Boundary**

Selection basis:

- workbook lifecycle authority is the smallest missing prerequisite before any
  post-P7 expansion can safely acquire or persist real VBProject state through
  a workbook;
- P7 already covers create-only mutation, readback, component rollback, and
  incomplete rollback reporting for an explicitly supplied target VBProject;
- broader VBProject operations such as overwrite, delete, rename, import, or
  export depend on a separately fixed workbook lifecycle and restore boundary;
- package / `dist`, release / publication, and external service operations are
  unrelated to the next Build mutation boundary.

P8-02 is selected only as the next docs-only authorization-boundary candidate.
P8-01 does not grant implementation GO for P8-02 and does not authorize test
changes, workbook operations, VBProject mutation, package / `dist`, release /
publication, or external services.

## Verification

P8-01 verification is documentation-only:

- review P7-34 and P7-35 records;
- review backlog, current-status, and handoff state;
- review docs-only diff;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P8-01.
