# P7-02 - Real Workbook / Real VBProject Mutation Reauthorization Boundary

## Status

COMPLETE / docs-only implementation scope planning

## Purpose

Connect the P7-01 selected candidate to a future implementation decision by
fixing the reauthorization boundary for real workbook and real VBProject
mutation.

P7-02 is documentation only. It does not authorize implementation, production
code changes, test code changes, workbook open / save / close / SaveAs /
restore, real workbook mutation, real VBProject mutation, package or `dist`
operations, release operations, publication, external service operations, or
Frozen specification changes.

## Scope

P7-02 defines:

- the formal P7-02 title and docs-only scope
- the reauthorization boundary after P7-01 candidate selection
- the minimum information required before any future real workbook / real
  VBProject mutation implementation GO can be considered
- candidate implementation scope that may be authorized later
- non-scope, acceptance criteria, and safety stops for any later
  implementation decision
- preserved boundaries from P5-04 through P7-01

## Starting State

- P6 is COMPLETE.
- P7-01 selected `P7-02 - Real Workbook / Real VBProject Mutation
  Reauthorization Boundary` as the next candidate.
- The completed mutation boundary remains fake/local target `Modules`
  dictionary create-only mutation through
  `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget`.
- Deterministic local folder generated-output write through
  `AppOutputWriteService.AppWriteGeneratedOutput` remains complete.
- Real workbook mutation and real VBProject mutation remain NO-GO.
- The current request explicitly starts P7-02 as docs-only and does not grant
  implementation GO.

## Reauthorization Preconditions For Future Implementation GO

A future implementation task may be considered only after a separate
repository-owner decision explicitly authorizes all of these:

- exact editable production files
- exact editable test files
- the real workbook / real VBProject mutation entry boundary to implement
- workbook fixture ownership, location, lifetime, backup, restore, and cleanup
- workbook open, save, close, SaveAs, and restore behavior
- VBProject trust/access preflight requirements before mutation
- allowed VBProject component operation set
- existing-module conflict behavior
- overwrite, delete, rename, and creation behavior, including any operations
  that must remain prohibited
- no-partial-mutation and rollback behavior
- readback verification requirements
- focused local verification commands
- `git diff --check`

If any item is missing, ambiguous, unapproved, or inconsistent with the
current repository state at that time, implementation remains NO-GO.

## Candidate Implementation Scope For Later GO

A later explicit implementation GO may include only:

- focused local tests for authorized real workbook / real VBProject mutation
  success and blocking states
- a test-owned local workbook fixture, if explicitly authorized
- pre-mutation workbook ownership and restore checks
- VBProject trust/access preflight before any module operation
- explicitly authorized module operations against the test-owned fixture
- readback verification from the authorized test fixture
- minimal local helpers needed to pass complete approved generated-output units
  into the mutation boundary
- Build test runner registration only if required to execute the focused tests
- the narrow real workbook / real VBProject mutation entry boundary, only
  after the later GO names exact editable files and permitted operations

The later implementation must consume complete approved generated-output units
or a focused-test local equivalent that preserves the same `fileName` /
`generatedSource` identity. It must not re-derive Blueprint, Manifest,
Template, GenerateContext, or Generator facts.

## Candidate Non-Scope

A later implementation GO must not include:

- mutation of real user workbooks or production workbooks
- runtime-selected workbook targets
- package, `dist`, release, publication, or external service operations
- credential, token-store, Google Docs, or Google Drive access
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

## Acceptance Criteria For Later Implementation

If separately authorized, the future implementation is acceptable only when:

- focused local tests prove complete approved generated output is the only
  mutation input
- workbook open / save / close / SaveAs / restore behavior occurs only under
  explicit authorization
- workbook ownership, backup, restore, and cleanup behavior are verified before
  and after mutation
- VBProject trust/access preflight completes before any VBProject operation
- only explicitly authorized module operations occur on the test-owned fixture
- success is reported only after every required mutation and readback
  verification completes
- hard-stop tests prove no workbook or VBProject mutation occurs for missing,
  failed, partial, ambiguous, unsupported, unapproved, fallback-derived,
  implicitly selected, or incomplete upstream state
- hard-stop tests prove no workbook or VBProject mutation occurs when workbook
  ownership, workbook operation authorization, trust/access preflight, allowed
  operations, conflict behavior, restore behavior, rollback behavior, or
  reporting behavior is undefined
- hard-stop tests prove no mutation reads raw Blueprint, Parser output,
  Validator diagnostics, Manifest Derivation diagnostics, Template contents,
  GenerateContext diagnostics, Generator internals, workbook runtime state,
  target project runtime state, or external state as design input
- package, `dist`, release, publication, external service, and real user
  workbook mutation remain absent from the implementation and verification path
- required focused verification and `git diff --check` pass

## Safety Stops

The future implementation task must stop before edits if:

- the task does not explicitly grant implementation GO
- exact editable files are not named
- the current codebase cannot identify a narrow real workbook / real VBProject
  mutation entry boundary
- workbook open / save / close / SaveAs / restore behavior is not explicitly
  authorized
- the workbook fixture is not test-owned, local, temporary, restorable, and
  isolated from package, `dist`, release, publication, external services, and
  user data
- VBProject trust/access preflight cannot be defined before mutation
- allowed module operations are not explicitly named
- conflict, overwrite, delete, rename, creation, rollback,
  no-partial-mutation, restore, or reporting behavior is undefined
- implementation requires fallback Template selection, implicit Template
  selection, Template content inference, GenerateContext or Generator
  compensation, public API changes, persisted schema changes, canonical format
  changes, Frozen specification changes, package or `dist` operations, release
  operations, external services, credentials, token stores, live user data, or
  real user workbook mutation
- existing user changes conflict with the target files

## Scope Planning Decision

GO:

- P7-02 docs-only Real Workbook / Real VBProject Mutation reauthorization
  boundary planning
- backlog, current-status, and handoff updates recording P7-02 completion
- docs-only diff review
- `git diff --check`

NO-GO:

- implementation start
- production code changes
- test code additions or updates
- real workbook mutation
- real VBProject mutation
- workbook open, save, close, SaveAs, restore, or file-system mutation beyond
  this Markdown documentation update
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
- package, `dist`, release, publication, tag creation, push, or external
  service operations
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Preserved Boundaries

P7-02 preserves the P5-04 through P7-01 boundaries:

- Template Derivation must use only approved Manifest-derived facts and
  approved mapping rules.
- GenerateContext must consume only complete, approved, generatable Template
  Derivation output.
- Generator must consume only complete successful GenerateContext output.
- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- `AppOutputWriteService.AppWriteGeneratedOutput` writes approved units only to
  a deterministic local folder.
- `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget` mutates only a
  fake/local `Modules` dictionary after full preflight.
- fake/local target create-only mutation remains the completed mutation
  boundary.
- real workbook and real VBProject mutation remain NO-GO until a separate
  implementation GO authorizes exact editable files, workbook handling,
  trust/access preflight, mutation operations, safety stops, restore behavior,
  rollback behavior, and verification.

## Verification Performed

P7-02 verification is docs-only:

- reviewed P7-01 Candidate Selection / GO-NO-GO
- reviewed backlog, current-status, and handoff P7-01 records
- confirmed fake/local target `Modules` dictionary create-only mutation is the
  completed mutation boundary
- confirmed real workbook mutation and real VBProject mutation remain NO-GO
- confirmed this request grants docs-only P7-02 start and no implementation GO
- confirmed no implementation, tests, workbook operation, VBProject mutation,
  package, `dist`, release, publication, push, tag creation, or external
  operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
