# P4-09 - Generator Focused Test Implementation Scope Planning

## Status

COMPLETE / docs-only implementation scope planning

## Purpose

Fix the implementation scope, non-scope, acceptance criteria, and safety
boundary needed to connect the P4-08 Generator focused test design to a future
implementation decision.

P4-09 is documentation only. It does not authorize production code or test code
implementation GO.

## Scope

P4-09 defines:

- the future implementation decision boundary after P4-08
- the candidate focused test implementation scope
- the candidate non-scope for the future implementation task
- acceptance criteria that a future implementation GO must satisfy
- safety-stop conditions before production or test code edits
- prohibited operations for the current docs-only task
- the Parser / Validator / Manifest Derivation / Template Mapping /
  GenerateContext / Generator separation that the future implementation task
  must preserve

## Non-Scope

P4-09 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Generator implementation changes
- GenerateContext implementation changes
- Template Mapping implementation changes
- Manifest Derivation implementation changes
- Parser changes
- Validator changes
- Template file changes
- runtime behavior changes
- public API changes
- persisted schema changes
- refactoring
- package, `dist`, build, or release operations
- external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Preconditions For Future Implementation GO

A future implementation task may be considered only after all of these are
true:

- P4-07 remains the accepted Generator input contract boundary.
- P4-08 remains the accepted focused test design.
- the exact Generator entry boundary to test is identified from the current
  codebase at that time.
- the exact focused test file or runner registration change is identified from
  the current codebase at that time.
- the future task explicitly authorizes test code implementation.
- any production Generator code change, if needed, is separately authorized and
  limited to the approved Generator entry boundary.

If the future implementation would require changing Parser, Validator,
Manifest Derivation, Template Mapping, GenerateContext, Template files, public
APIs, persisted schemas, canonical formats, or Frozen specifications, the
future task must stop before implementation.

## Candidate Implementation Target

The future implementation task should target focused local tests for the
Generator boundary fixed by P4-07 and designed by P4-08.

Candidate target areas are limited to:

- a focused Build unit test module for Generator input acceptance and
  hard-stop cases
- existing Build test runner registration only if required to execute the
  focused Generator tests
- minimal local test helpers or fixtures that construct a complete successful
  GenerateContext result, or the narrow local equivalent approved by the
  future task
- the Generator entry boundary only if a separately authorized implementation
  GO determines that a production boundary adjustment is required

P4-09 does not name exact source or test files as editable future targets,
because those must be re-confirmed against the current codebase at the time of
implementation.

## Candidate Test Cases

The future implementation scope should include focused local tests for:

- accepting a complete and successful GenerateContext result
- rejecting missing GenerateContext input before runtime generation
- rejecting partial GenerateContext input before runtime generation
- rejecting failed GenerateContext input before runtime generation
- rejecting GenerateContext input that lacks Template binding data
- rejecting GenerateContext input that lacks Manifest-derived module facts
- rejecting GenerateContext input that lacks Manifest-derived procedure facts
- rejecting GenerateContext input that lacks parameter, return value,
  visibility, dependency, or generation policy facts required by Generator
  consumption
- rejecting input when deterministic generation unit ordering is absent
- confirming Generator does not inspect Parser, Validator, Manifest
  Derivation, or Template Mapping diagnostics to continue
- confirming Generator does not add ad hoc arguments to compensate for
  incomplete GenerateContext data
- confirming upstream hard stops remain distinct from Generator hard stops

The future tests must prove hard-stop behavior before runtime generation or
output writes when the P4-07 input contract is not satisfied.

## Candidate Non-Scope For Future Implementation

A future focused test implementation task must not include:

- new Blueprint parsing behavior
- new Validator behavior
- new Manifest Derivation behavior
- new Template Mapping behavior
- new GenerateContext construction behavior except a separately approved
  narrow local test helper
- Template file content changes
- output formatting redesign
- overwrite policy redesign
- token replacement redesign
- new Generator parameters for upstream design facts
- fallback behavior that repairs, normalizes, infers, or completes upstream
  data
- integration with package, `dist`, release, publication, or external service
  workflows

## Acceptance Criteria For Future Implementation

If separately authorized, the future implementation is acceptable only when:

- focused local tests cover successful Generator input acceptance from a
  complete successful GenerateContext result
- focused local tests cover missing, partial, failed, inconsistent, and
  incomplete GenerateContext hard-stop cases
- focused local tests confirm Generator does not read raw Blueprint, Parser
  output, Validator diagnostics, Manifest Derivation diagnostics, Template
  Mapping diagnostics, or Template contents as design input
- focused local tests confirm Generator does not infer, repair, normalize, or
  complete upstream data
- focused local tests confirm Generator does not add ad hoc arguments for
  upstream design facts
- Parser, Validator, Manifest Derivation, Template Mapping, GenerateContext,
  and Generator failure classifications remain distinct
- hard-stop cases produce no runtime generation or output writes
- existing Build regression behavior remains preserved
- required focused verification and `git diff --check` pass

## Safety Boundary

The future implementation task must stop before edits if:

- the current codebase cannot identify a narrow Generator entry boundary
- the current codebase cannot construct a complete successful GenerateContext
  result or approved local equivalent without broad production changes
- test implementation requires Parser, Validator, Manifest Derivation,
  Template Mapping, Template file, public API, persisted schema, canonical
  format, or Frozen specification changes
- the implementation requires external services, package or `dist` artifacts,
  release operations, credentials, token stores, or live user data
- the implementation would weaken, delete, skip, or disable existing tests
- existing user changes conflict with the target files

## Scope Planning Decision

GO:

- P4-09 docs-only Generator focused test implementation scope planning

NO-GO:

- production code implementation
- test code implementation
- Generator behavior changes
- GenerateContext implementation changes
- Template Mapping implementation changes
- Manifest Derivation implementation changes
- Parser or Validator changes
- Template file changes
- runtime behavior changes
- package, `dist`, build, release, external service, staging, commit, or push
  operations

## Deferred Items

Deferred items:

- future implementation GO / NO-GO decision
- exact Generator entry point identification
- exact GenerateContext data model or approved local equivalent
- exact focused test file names
- exact test runner registration changes, if any
- exact local test helper or fixture shape
- token replacement interface review
- Template inventory review
- Generator output behavior review
- overwrite policy review

## Verification Performed

P4-09 verification is docs-only:

- reviewed P4-07 Generator Input Contract scope planning
- reviewed P4-08 Generator Focused Test Design
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
