# P6-16 - Real Workbook / Real VBProject Mutation Focused Test Implementation Scope Planning

## Status

COMPLETE / docs-only implementation scope planning

## Purpose

Connect the P6-15 Real Workbook / Real VBProject Mutation focused test design
to a future implementation decision by fixing candidate implementation scope,
non-scope, acceptance criteria, GO / NO-GO requirements, and safety stops.

P6-16 is documentation only. It does not authorize production code changes,
test code changes, real workbook mutation, real VBProject mutation, workbook
open/save/close behavior, package or `dist` operations, release operations, or
external service operations.

## Scope

P6-16 defines:

- the formal P6-16 title and docs-only scope
- the future implementation decision boundary after P6-15
- candidate focused test implementation scope for real workbook / real
  VBProject mutation
- required GO conditions before any code, test, workbook, or VBProject edits
- required NO-GO and safety-stop conditions
- preserved boundaries from fake/local target `Modules` dictionary mutation
  through real workbook / real VBProject planning

## Preconditions For Future Implementation GO

A future implementation task may be considered only after all of these are
true:

- P6-14 remains the accepted real workbook / real VBProject mutation boundary
  planning record.
- P6-15 remains the accepted focused test design.
- P6-15 is complete and pushed in commit
  `994aa336b529a90990c54ecd01d0b5ad374bcef1`.
- `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget` remains limited
  to create-only mutation of a fake/local target `Modules` dictionary after
  full preflight.
- the future task explicitly authorizes exact editable production and test
  files.
- the future task names the exact real workbook / real VBProject mutation entry
  boundary from the current codebase at that time.
- the workbook fixture is explicitly test-owned, local, temporary, restorable,
  and excluded from package, `dist`, release, publication, and external service
  paths.
- workbook open, save, close, SaveAs, and restore behavior are each explicitly
  authorized before implementation.
- VBProject trust/access preflight requirements are explicitly defined before
  implementation.
- allowed VBProject module operations are explicitly named before
  implementation.
- existing-module conflict, overwrite, delete, rename, rollback,
  no-partial-mutation, restore, and reporting behavior are explicitly defined
  before implementation.

If any of these cannot be confirmed, implementation remains NO-GO.

## Candidate Implementation Scope

A future implementation GO may include only:

- focused local tests for real workbook / real VBProject mutation successful
  and blocking states
- explicitly authorized test workbook fixture creation, open/save/close, and
  restore behavior
- VBProject trust/access preflight checks before any mutation
- explicitly authorized module operations on the test-owned workbook fixture
- readback verification from the authorized test fixture
- minimal local test helpers needed to feed complete approved generated output
  identities into the mutation boundary
- existing Build test runner registration only if needed to execute the focused
  tests
- the narrow real workbook / real VBProject mutation entry boundary, only if
  separately authorized by the future implementation GO

The implementation must consume only complete approved generated-output units
or a focused-test local equivalent preserving the same `fileName` /
`generatedSource` identity. It must not re-derive Template, GenerateContext, or
Generator facts.

## Candidate Non-Scope

A future implementation GO must not include:

- mutation of real user workbooks or production workbooks
- runtime-selected workbook targets
- package, `dist`, release, publication, or external service operations
- credential or token-store access
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, or fake/local target mutation behavior changes
- Template file changes
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Acceptance Criteria For Future Implementation

If separately authorized, the future implementation is acceptable only when:

- focused local tests prove complete approved generated output is the only
  mutation input
- focused local tests prove workbook open/save/close behavior occurs only when
  explicitly authorized
- focused local tests prove trust/access preflight completes before any
  VBProject mutation
- focused local tests prove only explicitly authorized module operations occur
  on the test-owned workbook fixture
- focused local tests prove success is reported only after every required
  mutation and readback verification completes
- hard-stop tests prove no workbook or VBProject mutation occurs for missing,
  failed, partial, ambiguous, unsupported, unapproved, fallback-derived,
  implicitly selected, or incomplete upstream state
- hard-stop tests prove no workbook or VBProject mutation occurs when workbook
  ownership, open/save/close authorization, trust/access preflight, allowed
  operations, conflict behavior, restore behavior, or reporting behavior is
  undefined
- hard-stop tests prove no mutation reads raw Blueprint, Parser output,
  Validator diagnostics, Manifest Derivation diagnostics, Template contents,
  GenerateContext diagnostics, Generator internals, workbook runtime state,
  target project runtime state, or external state as design input
- package, `dist`, release, external service, and real user workbook mutation
  remain absent from the implementation and verification path
- required focused verification and `git diff --check` pass

## Safety Boundary

The future implementation task must stop before edits if:

- the current codebase cannot identify a narrow real workbook / real VBProject
  mutation entry boundary
- the future task does not explicitly authorize workbook open/save/close,
  restore, and exact editable files
- the workbook fixture is not test-owned, local, temporary, restorable, and
  isolated from package, `dist`, release, publication, and external service
  paths
- VBProject trust/access preflight cannot be defined before mutation
- allowed module operations are not explicitly named
- conflict, overwrite, delete, rename, rollback, no-partial-mutation, restore,
  or reporting behavior is undefined
- implementation requires fallback Template selection, implicit Template
  selection, Template content inference, GenerateContext or Generator
  compensation, public API changes, persisted schema changes, canonical format
  changes, Frozen specification changes, package or `dist` operations, release
  operations, external services, credentials, token stores, or live user data
- existing user changes conflict with the target files

## Scope Planning Decision

GO:

- P6-16 docs-only Real Workbook / Real VBProject Mutation focused test
  implementation scope planning
- backlog and current-status updates recording P6-16 completion
- `git diff --check`
- commit and push of the P6-16 docs-only record, because this task explicitly
  authorizes commit and push after verification

NO-GO:

- production code changes
- test code additions or updates
- real workbook mutation
- real VBProject mutation
- workbook open, save, close, SaveAs, restore, or file-system mutation
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

P6-16 preserves the P5-04 through P6-15 boundaries:

- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- `AppOutputWriteService.AppWriteGeneratedOutput` writes approved units only to
  a deterministic local folder.
- `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget` mutates only a
  fake/local `Modules` dictionary after full preflight.
- fake/local target create-only mutation remains the completed boundary.
- real workbook and real VBProject mutation remain NO-GO until a separate
  implementation GO authorizes exact editable files, workbook handling,
  trust/access preflight, mutation operations, safety stops, restore behavior,
  and verification.
- real workbook / real VBProject mutation must not select Templates by
  fallback, implicit selection, Template contents, GenerateContext behavior,
  Generator behavior, generated output, target project state, workbook runtime
  state, or external state.
- real workbook / real VBProject mutation must not infer, repair, normalize,
  or complete missing upstream Template Derivation, GenerateContext, or
  Generator facts.

## Verification Performed

P6-16 verification is docs-only:

- reviewed P6-15 Real Workbook / Real VBProject Mutation Focused Test Design
- reviewed backlog and current-status P6-15 records
- confirmed fake/local target `Modules` dictionary create-only mutation is the
  completed boundary
- confirmed P6-15 is complete and pushed in commit
  `994aa336b529a90990c54ecd01d0b5ad374bcef1`
- confirmed real workbook mutation and real VBProject mutation remain NO-GO
- confirmed no implementation, tests, workbook operation, VBProject mutation,
  package, `dist`, release, publication, or external operation is part of this
  task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
