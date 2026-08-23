# P5-08 - GenerateContext Focused Test Implementation Scope Planning

## Status

COMPLETE / docs-only implementation scope planning

## Purpose

Fix the implementation scope, non-scope, acceptance criteria, and safety
boundary needed to connect the P5-07 GenerateContext focused test design to a
future implementation decision.

P5-08 is documentation only. It does not authorize production code or test code
implementation GO.

## Scope

P5-08 defines:

- the future implementation decision boundary after P5-07
- the candidate focused test implementation scope
- the candidate non-scope for the future implementation task
- acceptance criteria that a future implementation GO must satisfy
- safety-stop conditions before production or test code edits
- prohibited operations for the current docs-only task
- the Parser / Validator / Manifest Derivation / Template Derivation /
  GenerateContext / Generator separation that the future implementation task
  must preserve

## Non-Scope

P5-08 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- GenerateContext implementation changes
- Template Derivation implementation changes
- Manifest Derivation implementation changes
- Parser changes
- Validator changes
- Template file changes
- Generator invocation or behavior changes
- runtime behavior changes
- public API changes
- persisted schema changes
- refactoring
- package, `dist`, build, or release operations
- external service operations
- Frozen specification changes
- implementation GO

## Preconditions For Future Implementation GO

A future implementation task may be considered only after all of these are
true:

- P5-06 remains the accepted GenerateContext data model boundary.
- P5-07 remains the accepted GenerateContext focused test design.
- the exact GenerateContext entry boundary to test is identified from the
  current codebase at that time.
- the exact focused test file or runner registration change is identified from
  the current codebase at that time.
- the future task explicitly authorizes test code implementation.
- any production GenerateContext code change, if needed, is separately
  authorized and limited to the approved GenerateContext entry boundary.

If the future implementation would require changing Parser, Validator,
Manifest Derivation, Template Derivation, Template files, Generator behavior,
public APIs, persisted schemas, canonical formats, or Frozen specifications,
the future task must stop before implementation.

## Candidate Implementation Target

The future implementation task should target focused local tests for the
GenerateContext boundary fixed by P5-06 and designed by P5-07.

Candidate target areas are limited to:

- a focused Build unit test module for GenerateContext successful construction
  and hard-stop cases
- existing Build test runner registration only if required to execute the
  focused GenerateContext tests
- minimal local test helpers or fixtures that construct complete, approved,
  generatable Template Derivation output, or the narrow local equivalent
  approved by the future task
- the GenerateContext entry boundary only if a separately authorized
  implementation GO determines that a production boundary adjustment is
  required

P5-08 does not name exact source or test files as editable future targets,
because those must be re-confirmed against the current codebase at the time of
implementation.

## Candidate Test Cases

The future implementation scope should include focused local tests for:

- constructing one ordered generation unit for each complete, approved,
  generatable Template Derivation item
- carrying required P5-06 data groups into each generation unit
- preserving approved upstream ordering
- returning overall success only when every required generation unit is
  complete
- rejecting absent, failed, partial, unsupported, non-generatable, ambiguous,
  incomplete, unapproved, fallback-derived, or implicitly selected Template
  Derivation output before Generator
- rejecting missing, blank, or internally inconsistent required P5-03 Template
  Derivation Model fields
- rejecting missing, blank, or inconsistent approved Manifest-derived
  `ModuleName`, `ModuleType`, or `LayerName`
- rejecting construction that requires raw Blueprint text, unvalidated parsed
  Blueprint state, Validator diagnostics, Manifest Derivation diagnostics,
  Template file contents, Generator behavior, generated VBA output, or
  external runtime state
- rejecting construction that requires fallback, implicit Template selection,
  Template content inference, downstream repair, normalization, or compensation
- rejecting procedure, parameter, return value, dependency, or
  generation-policy facts not carried by the approved Manifest surface

The future tests must prove hard-stop behavior before Generator invocation or
output writes.

## Candidate Non-Scope For Future Implementation

A future focused test implementation task must not include:

- new Blueprint parsing behavior
- new Validator behavior
- new Manifest Derivation behavior
- new Template Derivation behavior
- Template file content changes
- Generator invocation or output behavior changes
- fallback behavior that repairs, normalizes, infers, or completes upstream
  data
- implicit Template selection
- Template content inference
- new GenerateContext fields sourced from downstream Generator behavior
- integration with package, `dist`, release, publication, or external service
  workflows

## Acceptance Criteria For Future Implementation

If separately authorized, the future implementation is acceptable only when:

- focused local tests cover successful GenerateContext construction from
  complete, approved, generatable Template Derivation output and approved
  Manifest-derived facts
- focused local tests cover missing, partial, failed, unsupported,
  non-generatable, ambiguous, incomplete, unapproved, fallback-derived, and
  implicitly selected Template Derivation hard-stop cases
- focused local tests confirm GenerateContext does not read raw Blueprint,
  Parser output, Validator diagnostics, Manifest Derivation diagnostics,
  Template contents, Generator behavior, or generated output as design input
- focused local tests confirm GenerateContext does not infer, repair,
  normalize, or complete upstream data
- Parser, Validator, Manifest Derivation, Template Derivation,
  GenerateContext, and Generator failure classifications remain distinct
- hard-stop cases produce no Generator input, Generator invocation, runtime
  generation, or output writes
- existing Build regression behavior remains preserved
- required focused verification and `git diff --check` pass

## Safety Boundary

The future implementation task must stop before edits if:

- the current codebase cannot identify a narrow GenerateContext entry boundary
- the current codebase cannot construct complete, approved, generatable
  Template Derivation output or approved local equivalent without broad
  production changes
- test implementation requires Parser, Validator, Manifest Derivation,
  Template Derivation, Template file, Generator, public API, persisted schema,
  canonical format, or Frozen specification changes
- the implementation requires fallback, implicit Template selection, Template
  content inference, GenerateContext or Generator compensation, external
  services, package or `dist` artifacts, release operations, credentials,
  token stores, or live user data
- the implementation would weaken, delete, skip, or disable existing tests
- existing user changes conflict with the target files

## Scope Planning Decision

GO:

- P5-08 docs-only GenerateContext focused test implementation scope planning

NO-GO:

- local-only implementation
- production code implementation
- test code implementation
- GenerateContext implementation changes
- Template Derivation implementation changes
- Generator invocation or behavior changes
- Template file changes
- runtime behavior changes
- package, `dist`, build, release, external service, or Frozen specification
  changes

## Deferred Items

Deferred items:

- future implementation GO / NO-GO decision
- exact GenerateContext entry point identification
- exact focused test file names
- exact test runner registration changes, if any
- exact local test helper or fixture shape
- exact diagnostic code constants
- optional body, section, and member source data handling
- Generator invocation contract implementation

## Verification Performed

P5-08 verification is docs-only:

- reviewed P5-06 GenerateContext Data Model Planning
- reviewed P5-07 GenerateContext Focused Test Design
- reviewed P5-04 Template Derivation Failure Boundary Planning
- reviewed P5-05 Template Derivation Focused Test Design
- reviewed backlog and current-status records
- confirmed no implementation, tests, Template file changes, GenerateContext
  implementation, Generator changes, package, `dist`, build, release, or
  external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
