# P6-11 - Target VBA Project Mutation Focused Test Implementation Scope Planning

## Status

COMPLETE / docs-only implementation scope planning

## Purpose

Connect the P6-10 Target VBA Project Mutation focused test design to a future
implementation decision by fixing candidate implementation scope, non-scope,
acceptance criteria, and safety stops.

P6-11 is documentation only. It does not authorize production code changes,
test code changes, target VBA project mutation, real workbook mutation, package
or `dist` operations, release operations, or external service operations.

## Scope

P6-11 defines:

- the formal P6-11 title and docs-only scope
- the future implementation decision boundary after P6-10
- candidate focused test implementation scope for target VBA project mutation
- required GO conditions before any code or test edits
- required NO-GO and safety-stop conditions
- preserved boundaries from deterministic local generated-output write through
  target mutation planning

## Preconditions For Future Implementation GO

A future implementation task may be considered only after all of these are
true:

- P6-09 remains the accepted target VBA project mutation boundary planning
  record.
- P6-10 remains the accepted Target VBA Project Mutation focused test design.
- `AppOutputWriteService.AppWriteGeneratedOutput` remains limited to
  deterministic local folder write.
- the future task explicitly authorizes exact editable production and test
  files.
- the future task names the exact target mutation entry boundary from the
  current codebase at that time.
- the target surface is local and test-controlled, or an equivalent fake target
  that cannot mutate a real user workbook.
- allowed mutation operations are explicitly named before implementation.
- conflict, overwrite, no-partial-mutation, rollback, recovery, and reporting
  behavior are explicitly defined before implementation.

If any of these cannot be confirmed, implementation remains NO-GO.

## Candidate Implementation Scope

A future implementation GO may include only:

- focused local tests for target VBA project mutation successful and blocking
  states
- local fake or test-controlled target project representation
- minimal local test helpers or fixtures needed to feed approved generated
  output identities into the mutation boundary
- existing Build test runner registration only if needed to execute the focused
  target mutation tests
- the narrow target mutation entry boundary, only if separately authorized by
  the future implementation GO

The implementation must consume only complete approved generated-output units
or a focused-test local equivalent preserving the same `fileName` /
`generatedSource` identity. It must not re-derive Template, GenerateContext, or
Generator facts.

## Candidate Non-Scope

A future implementation GO must not include:

- real workbook mutation
- production workbook mutation
- module import, export, overwrite, delete, rename, or creation in a real user
  target VBA project
- package, `dist`, release, publication, or external service operations
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, or Output Write behavior changes
- Template file changes
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes
- credentials, token stores, or live user data access

## Acceptance Criteria For Future Implementation

If separately authorized, the future implementation is acceptable only when:

- focused local tests prove complete approved generated output is the only
  mutation input
- focused local tests prove module identity and generated source content are
  carried unchanged into the target mutation request
- focused local tests prove only explicitly authorized mutation operations can
  occur on the local test-controlled target surface
- hard-stop tests prove no target mutation occurs for missing, failed, partial,
  ambiguous, unsupported, unapproved, fallback-derived, implicitly selected, or
  incomplete upstream state
- hard-stop tests prove no target mutation occurs for blank, path-bearing,
  duplicate, conflicting, or unsupported generated file names or module
  identities
- hard-stop tests prove no target mutation occurs when conflict, overwrite,
  no-partial-mutation, rollback, recovery, or reporting behavior is undefined
- the implementation does not read raw Blueprint, Parser output, Validator
  diagnostics, Manifest Derivation diagnostics, Template contents,
  GenerateContext diagnostics, Generator internals, target project runtime
  state, or external state as design input
- package, `dist`, release, external service, and real workbook mutation remain
  absent from the implementation and verification path
- required focused verification and `git diff --check` pass

## Safety Boundary

The future implementation task must stop before edits if:

- the current codebase cannot identify a narrow target mutation entry boundary
- the current codebase cannot provide a local test-controlled target surface
  without real workbook mutation
- allowed mutation operations are not explicitly named
- conflict, overwrite, no-partial-mutation, rollback, recovery, or reporting
  behavior is undefined
- implementation requires fallback Template selection, implicit Template
  selection, Template content inference, GenerateContext or Generator
  compensation, public API changes, persisted schema changes, canonical format
  changes, Frozen specification changes, package or `dist` operations, release
  operations, external services, credentials, token stores, or live user data
- existing user changes conflict with the target files

## Scope Planning Decision

GO:

- P6-11 docs-only Target VBA Project Mutation focused test implementation scope
  planning
- backlog and current-status updates recording P6-11 completion
- `git diff --check`
- commit and push of the P6-11 docs-only record, because this task explicitly
  authorizes commit and push after verification

NO-GO:

- production code changes
- test code additions or updates
- target VBA project mutation
- real workbook mutation
- generated output write beyond the existing deterministic local folder write
- module import, export, overwrite, delete, rename, or creation in a real target
  VBA project
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, or target mutation behavior changes
- Template file changes
- package, `dist`, release, publication, or external service operations
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Preserved Boundaries

P6-11 preserves the P5-04 through P6-10 boundaries:

- only complete approved downstream output may reach target mutation planning.
- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- `AppOutputWriteService.AppWriteGeneratedOutput` writes approved units only to
  a deterministic local folder.
- actual generated output write remains separate from target VBA project
  mutation.
- target VBA project mutation remains NO-GO until a separate implementation GO
  authorizes exact target surface, mutation operations, safety stops, editable
  files, and verification.
- target mutation must not select Templates by fallback, implicit selection,
  Template contents, GenerateContext behavior, Generator behavior, generated
  output, target project state, or runtime state.
- target mutation must not infer, repair, normalize, or complete missing
  upstream Template Derivation, GenerateContext, or Generator facts.

## Verification Performed

P6-11 verification is docs-only:

- reviewed P6-10 Target VBA Project Mutation Focused Test Design
- reviewed backlog and current-status P6-10 records
- confirmed actual generated output write remains limited to
  `AppOutputWriteService.AppWriteGeneratedOutput` deterministic local folder
  write only
- confirmed target VBA project mutation remains NO-GO
- confirmed no implementation, tests, generated output write, target mutation,
  real workbook mutation, package, `dist`, release, publication, or external
  operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
