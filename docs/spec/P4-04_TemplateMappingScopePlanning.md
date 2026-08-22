# P4-04 - Template Mapping Scope Planning

## Status

COMPLETE / docs-only planning

## Purpose

Fix the responsibility boundary for mapping derived Manifest data to Template
selection inputs after Manifest Derivation completion, while preserving the
P4-01 through P4-03 boundaries.

P4-04 is documentation only. It does not authorize implementation GO.

## Scope

P4-04 defines:

- the Template Mapping input boundary
- the Template Mapping output boundary
- the responsibility boundary between Manifest Derivation and GenerateContext
- minimum mapping rules that may be evaluated later
- hard-stop conditions before GenerateContext and Generator behavior
- the next candidate direction after Manifest Derivation verification

## Non-Scope

P4-04 does not perform or authorize:

- production VBA code changes
- test code additions
- Parser changes
- Validator changes
- Manifest Derivation changes
- Template implementation changes
- GenerateContext implementation or planning beyond boundary placement
- Generator implementation or planning
- package, `dist`, build, or release operations
- external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Preconditions

Template Mapping may be considered only after all of these are true:

- Blueprint parsing has completed successfully.
- The parsed Blueprint has passed Validator checks.
- `Result.Generatable = True`.
- Manifest Derivation has completed successfully.
- The derived Manifest data is complete for the existing Manifest consumer
  boundary.
- No parse, validation, or Manifest Derivation diagnostic requires a hard stop.

Parser, Validator, and Manifest Derivation failures stop before Template
Mapping.

Template Mapping failure stops before GenerateContext and Generator behavior.

## Input Boundary

The only input to Template Mapping is derived Manifest data produced after
Validator PASS and Manifest Derivation success.

Template Mapping must not read raw Blueprint text, unvalidated parsed Blueprint
state, or Validator diagnostics to make Template selection decisions. Those
decisions must already be represented in the derived Manifest data or must
hard-stop as unsupported for this boundary.

Template Mapping must not repair, normalize, or complete Manifest data for the
caller.

## Output Boundary

Template Mapping output is the deterministic Template selection and Template
binding data needed by the downstream GenerateContext boundary.

The output may contain only:

- selected Template identity or key
- module-to-template binding
- procedure-to-template binding when supported by the existing flow
- supported generation policy binding
- deterministic technical metadata required by the existing Template consumer

The output must not introduce:

- new modules
- new procedures
- new dependencies
- new generation behavior
- new approval state
- inferred design intent
- GenerateContext construction behavior
- Generator output behavior

## Responsibility Boundary

Template Mapping is an independent boundary after Manifest Derivation and
before GenerateContext.

The Template Mapping responsibility is:

- consume only successfully derived Manifest data
- select supported Templates deterministically
- bind Manifest module and procedure entries to supported Template identities
- preserve Manifest order where order is meaningful
- reject unsupported or ambiguous Template selection conditions
- return a mapping failure before GenerateContext when binding cannot be
  completed safely

The Template Mapping responsibility is not:

- parsing Blueprint content
- validating Blueprint semantics
- deriving Manifest data
- repairing Manifest content
- constructing GenerateContext
- invoking Generator behavior
- producing VBA output
- changing Parser, Validator, or Manifest Derivation result semantics

## Minimum Future Implementation Slice

A future implementation task may be considered only after a separate GO
decision.

The minimum future slice should be limited to:

- one Template Mapping entry point after Manifest Derivation success
- one input contract that accepts only derived Manifest data or a narrowly
  adapted equivalent
- deterministic mapping for supported module kinds, procedure kinds,
  visibility, return value, parameters, and generation policy values already
  represented in the Manifest data
- explicit hard-stop result for missing, ambiguous, or unsupported Template
  binding data
- focused tests for supported Template selection, unsupported Template
  hard-stop, Manifest Derivation failure hard-stop, and no GenerateContext or
  Generator invocation on Template Mapping failure

The minimum slice must preserve:

- Parser behavior
- Validator behavior
- Manifest Derivation behavior
- Template file contents unless separately authorized
- GenerateContext behavior
- Generator behavior
- public API and persisted schema boundaries unless separately authorized
- existing Build regression behavior

## Deferred Items

Deferred items:

- exact Template inventory review
- Template file format changes, if any
- GenerateContext construction rules
- Generator output behavior
- advanced Template selection policy
- Template fallback policy
- Blueprint repair or normalization
- dependency discovery
- implementation GO / NO-GO decision

## Verification Performed

P4-04 verification is docs-only:

- reviewed P4-01 Manifest Derivation scope planning
- reviewed P4-02 Manifest Derivation implementation record
- reviewed P4-03 focused test completion record
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
