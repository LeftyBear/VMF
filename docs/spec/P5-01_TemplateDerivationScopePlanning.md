# P5-01 - Template Derivation Scope Planning

## Status

COMPLETE / docs-only planning

## Purpose

Fix the responsibility boundary for deriving Template-ready binding data from a
P4-derived and validated Manifest before any implementation task changes
Template, GenerateContext, Generator, tests, or runtime behavior.

P5-01 is documentation only. It does not authorize implementation GO.

## Scope

P5-01 defines:

- the Manifest-only input boundary for Template Derivation
- the Template Derivation responsibility boundary
- the Template Derivation output boundary
- the handoff boundary to downstream GenerateContext
- failure boundaries for missing required information, unsupported elements,
  non-unique conversion, and unsatisfied Generator preconditions
- the relationship to existing Template Mapping, GenerateContext, and
  Generator records
- the minimum future implementation slice that may be evaluated later

## Non-Scope

P5-01 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Parser changes
- Validator changes
- Manifest Derivation changes
- Template file changes
- Template Mapping implementation changes
- GenerateContext changes
- Generator changes
- runtime behavior changes
- public API changes
- persisted schema changes
- refactoring
- package, `dist`, build, or release operations
- external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Preconditions

Template Derivation may be considered only after all of these are true:

- Blueprint parsing has completed successfully.
- Validator checks have completed successfully.
- `Result.Generatable = True`.
- Manifest Derivation has completed successfully.
- The Manifest was derived from the P4-approved flow.
- The Manifest is complete for the existing Manifest consumer boundary.
- No parse, validation, or Manifest Derivation diagnostic requires a hard stop.

Parser, Validator, and Manifest Derivation failures stop before Template
Derivation.

Template Derivation failure stops before GenerateContext and Generator behavior.

## Input Boundary

The only input to Template Derivation is the P4-derived and verified Manifest.

Template Derivation must not read or depend on:

- raw Blueprint text
- unvalidated parsed Blueprint state
- Validator diagnostics
- Manifest Derivation diagnostics except as a prior hard-stop condition
- generated VBA output
- GenerateContext state
- Generator behavior
- external runtime state as a source of design intent

All Template decisions must be made from the Manifest and approved Template
derivation rules. If the Manifest does not contain enough information to derive
Template binding data, Template Derivation must hard-stop.

## Responsibility Boundary

Template Derivation is the boundary that converts derived Manifest facts into
deterministic Template binding data for downstream GenerateContext preparation.

Template Derivation is responsible for:

- consuming only the P4-derived Manifest
- identifying the approved Template identity or key for each supported Manifest
  generation unit
- binding Manifest modules to supported Templates
- binding Manifest procedures to supported Templates when procedure-level
  binding is supported by the approved flow
- preserving Manifest order where order is meaningful
- carrying only supported generation policy values needed by downstream
  GenerateContext construction
- rejecting missing, unsupported, ambiguous, or non-unique Template conversion
  conditions
- returning diagnostics without producing downstream input when derivation
  cannot complete safely

Template Derivation is not responsible for:

- parsing Blueprint content
- validating Blueprint semantics
- deciding `Result.Generatable`
- deriving Manifest data
- repairing, normalizing, or completing Manifest data
- changing Template file contents
- selecting fallback Templates without an approved rule
- constructing GenerateContext
- invoking Generator behavior
- producing VBA output
- enforcing overwrite or file-write policy
- changing Parser, Validator, Manifest Derivation, GenerateContext, or
  Generator result semantics

## Output Boundary

Template Derivation output is a successful Template binding result or a
hard-stop diagnostic result.

A successful Template binding result may contain only:

- selected Template identity or key
- module-to-template binding
- procedure-to-template binding when supported by the approved flow
- Template-relative metadata required by GenerateContext
- supported generation policy binding
- deterministic ordering information already represented by the Manifest
- trace information needed to relate the binding result back to Manifest items

The output must not introduce:

- new modules
- new procedures
- new parameters
- new dependencies
- new approval state
- inferred design intent
- Template file mutations
- GenerateContext construction behavior
- Generator invocation behavior
- generated VBA code
- file write behavior

Template Derivation output is not Generator input. Generator input remains a
complete and successful GenerateContext result only.

## GenerateContext Boundary

GenerateContext may consume Template Derivation output only when Template
Derivation completed successfully and the Manifest-derived facts required by
P4-06 remain available and consistent.

GenerateContext owns packaging the successful Template binding result and
approved Manifest-derived data into Generator-ready context. Template
Derivation does not decide GenerateContext schema, construct GenerateContext,
fill missing GenerateContext facts, or invoke Generator.

If Template Derivation output is missing, failed, partial, ambiguous,
unsupported, or inconsistent with Manifest-derived data, the pipeline must
hard-stop before GenerateContext.

## Failure Boundary / Hard-Stop Conditions

Template Derivation must hard-stop when:

- input is not the P4-derived Manifest
- required Manifest data is missing
- required Manifest data is blank where a concrete value is needed
- a Manifest target, module, procedure, parameter, return value, dependency, or
  generation policy value is unsupported by approved Template derivation rules
- a Manifest item maps to no approved Template
- a Manifest item maps to more than one approved Template
- Template selection depends on raw Blueprint content, Validator diagnostics, or
  Manifest Derivation diagnostics
- Template selection requires inferring design intent not present in the
  Manifest
- Template selection requires repairing, normalizing, or completing the
  Manifest
- Template selection requires Template fallback behavior that has not been
  approved
- Template selection requires changing Template files
- Template Derivation cannot preserve required Manifest ordering
- downstream GenerateContext cannot be satisfied from the Template binding
  result and approved Manifest-derived data
- Generator preconditions from P4-07 would not be satisfiable without adding
  ad hoc Generator inputs, reinterpreting upstream state, or changing Generator
  behavior

Template Derivation failure must remain distinct from:

- parse failure
- validation failure
- Manifest Derivation failure
- GenerateContext construction failure
- Generator failure

Template Derivation failure must not produce GenerateContext input and must not
invoke Generator behavior.

## Relationship To Existing Records

The Build vNext generation flow remains:

```text
Blueprint
-> Parser
-> Validator
-> Manifest Derivation
-> Template Derivation
-> GenerateContext
-> Generator
```

P5-01 preserves the P4-01 Manifest Derivation rule that Manifest is derived
generation data, not an independent design source.

P5-01 preserves the P4-05 Template Mapping contract by treating Template
Derivation as the Manifest -> Template binding responsibility. It does not
expand Template Mapping into GenerateContext construction or Generator
invocation.

P5-01 preserves the P4-06 GenerateContext boundary: GenerateContext consumes
only successful Template binding output and approved Manifest-derived data, then
packages Generator-ready context.

P5-01 preserves the P4-07 Generator input contract: Generator consumes only a
complete and successful GenerateContext result.

Existing implementation remains authoritative only as current behavior. It does
not override the docs-only responsibility boundary fixed here, and this record
does not require existing implementation changes.

## Minimum Future Implementation Slice

A future implementation task may be considered only after a separate GO
decision.

The minimum future slice should be limited to:

- one Template Derivation entry point after Manifest Derivation success
- one input contract that accepts only the P4-derived Manifest or a narrowly
  adapted equivalent
- deterministic Template binding for supported Manifest target, module,
  procedure, visibility, parameter, return value, dependency, and generation
  policy facts
- explicit hard-stop result for missing information, unsupported elements,
  non-unique conversion, and unsatisfied downstream preconditions
- focused local tests for successful binding, missing required Manifest data,
  unsupported Manifest element, ambiguous Template match, and no
  GenerateContext or Generator invocation on Template Derivation failure

The minimum slice must preserve:

- Parser behavior
- Validator behavior
- Manifest Derivation behavior
- Template file contents unless separately authorized
- GenerateContext behavior
- Generator behavior
- public API and persisted schema boundaries unless separately authorized
- existing Build regression behavior

## Scope Planning Decision

GO:

- P5-01 docs-only Template Derivation scope planning

NO-GO:

- implementation
- test code implementation
- Template file changes
- GenerateContext changes
- Generator changes
- runtime behavior changes
- package, `dist`, build, release, external service, staging, commit, or push
  operations

## Deferred Items

Deferred items:

- exact Template inventory review
- concrete Template derivation table
- concrete Template Derivation entry point
- concrete output model
- concrete GenerateContext data model
- focused implementation test file names
- Template fallback policy
- Template file format changes, if any
- Generator output behavior review
- implementation GO / NO-GO decision

## Verification Performed

P5-01 verification is docs-only:

- reviewed P4-01 Manifest Derivation scope planning
- reviewed P4-04 Template Mapping scope planning
- reviewed P4-05 Template Mapping contract freeze
- reviewed P4-06 GenerateContext responsibility boundary freeze
- reviewed P4-07 Generator Input Contract scope planning
- reviewed P4-09 Generator Focused Test Implementation Scope Planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
