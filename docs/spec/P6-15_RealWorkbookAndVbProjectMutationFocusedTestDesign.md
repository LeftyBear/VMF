# P6-15 - Real Workbook / Real VBProject Mutation Focused Test Design

## Status

COMPLETE / docs-only focused test design

## Purpose

Fix the focused test design for a future real workbook / real VBProject
mutation implementation after P6-14 Real Workbook / Real VBProject Mutation
Boundary Planning.

P6-15 is documentation only. It does not authorize production code changes,
test code changes, real workbook mutation, real VBProject mutation, workbook
open/save/close behavior, package or `dist` operations, release operations, or
external service operations.

## Scope

P6-15 defines:

- the formal P6-15 title and docs-only scope
- the future real workbook / real VBProject focused test target
- successful-state and blocking-state focused test design
- required trust/access, ownership, mutation, and recovery boundaries
- GO / NO-GO conditions for a later implementation-scope decision

## Preconditions

P6-15 records that P6-14 is complete and pushed in commit:

`9a9e955e79719509f6c01aff9bc00e9591c74d8c`

The completed mutation boundary before P6-15 remains limited to
`AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget` create-only
insertion into a fake/local target `Modules` dictionary after full preflight.

Real workbook mutation and real VBProject mutation remain NO-GO.

## Future Test Target

A future implementation task may design focused tests only against an explicitly
authorized test workbook fixture or equivalent isolated workbook target. The
target must be local, temporary, test-owned, restorable, and excluded from
package, `dist`, release, publication, and external service paths.

The future test target must define before implementation:

- exact workbook fixture ownership
- whether workbook open, save, close, and restore are authorized
- exact VBProject surface allowed for mutation
- trust/access preflight requirements
- allowed module operation set
- existing-module and conflict policy
- no-partial-mutation and restore expectations
- verification readback method

No real user workbook, production workbook, package artifact, `dist` artifact,
external service, credential, token store, or runtime-selected target may be
used as the test target.

## Future Successful-State Test Cases

A future implementation slice should include focused local tests proving:

- complete approved generated output is the only mutation input
- module identity and generated source are carried unchanged from approved
  output-write units
- the authorized test workbook fixture is opened only if workbook open is
  explicitly authorized
- trust/access preflight succeeds before any VBProject mutation
- only explicitly authorized module operations occur
- success is reported only after every required module mutation and readback
  verification completes
- workbook restore or cleanup leaves no persistent user-data mutation
- no package, `dist`, release, publication, external service, fallback Template
  selection, implicit Template selection, Template content inference,
  GenerateContext compensation, or Generator compensation occurs

## Future Blocking-State Test Cases

A future implementation slice should include focused local tests proving no
workbook or VBProject mutation occurs when:

- upstream state is missing, failed, partial, ambiguous, unsupported,
  unapproved, fallback-derived, or implicitly selected
- generated output write did not complete successfully for every required unit
- generated file names are blank, path-bearing, duplicate, conflicting, or
  unsupported
- generated source content is blank, missing, conflicting, or incomplete
- workbook fixture ownership is absent, ambiguous, non-temporary, or not
  restorable
- workbook open/save/close behavior is not explicitly authorized
- VBProject access or trust preflight fails
- allowed module operations are not explicitly named
- existing-module conflict, overwrite, delete, rename, rollback, recovery,
  restore, or reporting behavior is undefined
- mutation would require reading raw Blueprint, Parser output, Validator
  diagnostics, Manifest Derivation diagnostics, Template contents,
  GenerateContext diagnostics, Generator internals, workbook runtime state,
  target project runtime state, or external state as design input

The tests must verify hard stops before workbook open when possible, and before
VBProject mutation in all blocking cases.

## Scope Planning Decision

GO:

- P6-15 docs-only Real Workbook / Real VBProject Mutation focused test design
- backlog and current-status updates recording P6-15 completion
- `git diff --check`
- commit and push of the P6-15 docs-only record, because this task explicitly
  authorizes commit and push after verification

NO-GO:

- production code changes
- test code additions or updates
- real workbook mutation
- real VBProject mutation
- workbook open, save, close, SaveAs, or file-system mutation
- VBProject module import, export, overwrite, delete, rename, or creation
- mutation of real user data, production workbooks, package artifacts, or
  `dist` artifacts
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, or fake/local target mutation behavior changes
- Template file changes
- package, `dist`, release, publication, or external service operations
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Preserved Boundaries

P6-15 preserves the P5-04 through P6-14 boundaries:

- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- `AppOutputWriteService.AppWriteGeneratedOutput` writes approved units only to
  a deterministic local folder.
- `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget` mutates only a
  fake/local `Modules` dictionary after full preflight.
- fake/local target create-only mutation remains the completed boundary.
- real workbook and real VBProject mutation require a separate implementation
  GO after this focused test design.
- Output Write and target mutation must not select Templates by fallback,
  implicit selection, Template contents, GenerateContext behavior, Generator
  behavior, generated output, target project state, workbook runtime state, or
  external state.
- Output Write and target mutation must not infer, repair, normalize, or
  complete missing upstream Template Derivation, GenerateContext, or Generator
  facts.

## Verification Performed

P6-15 verification is docs-only:

- reviewed P6-14 Real Workbook / Real VBProject Mutation Boundary Planning
- reviewed backlog and current-status P6-14 records
- confirmed fake/local target `Modules` dictionary create-only mutation is the
  completed boundary
- confirmed real workbook mutation and real VBProject mutation remain NO-GO
- confirmed no implementation, tests, workbook operation, VBProject mutation,
  package, `dist`, release, publication, or external operation is part of this
  task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
