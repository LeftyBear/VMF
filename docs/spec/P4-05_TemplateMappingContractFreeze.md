# P4-05 - Template Mapping Contract Freeze

## Status

COMPLETE / docs-only contract freeze

## Purpose

Freeze the Template Mapping contract after P4-04 scope planning and before any
GenerateContext or Generator implementation work.

P4-05 fixes Template Mapping as the boundary that converts successfully derived
Manifest data into deterministic Template binding data. It does not authorize
implementation GO.

## Scope

P4-05 defines:

- the Template Mapping contract
- the Manifest -> Template Mapping -> GenerateContext / Generator connection
  boundary
- decisions owned by Template Mapping
- decisions explicitly not owned by Template Mapping
- hard-stop conditions for unresolved, ambiguous, unsupported, or unapproved
  mapping state

## Non-Scope

P4-05 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Parser changes
- Validator changes
- Manifest Derivation changes
- Template file changes
- GenerateContext implementation
- Generator implementation
- refactoring
- package, `dist`, build, or release operations
- external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Contract Position

The Build vNext generation flow remains:

```text
Blueprint
-> Parser
-> Validator
-> Manifest Derivation
-> Template Mapping
-> GenerateContext
-> Generator
```

Template Mapping starts only after Manifest Derivation has succeeded.

Template Mapping ends before GenerateContext construction. A successful mapping
result may be consumed by a later GenerateContext boundary, but Template Mapping
does not construct GenerateContext and does not invoke Generator behavior.

## Input Contract

The only allowed Template Mapping input is successfully derived Manifest data.

The input must already contain the supported generation facts needed to select
and bind Templates. Template Mapping must not consult earlier pipeline state to
repair or reinterpret the Manifest.

Template Mapping must not read or depend on:

- raw Blueprint text
- unvalidated parsed Blueprint state
- Validator diagnostics
- Manifest Derivation diagnostics except as a prior hard-stop condition
- Template file contents as a source of design intent
- GenerateContext state
- Generator behavior

If a Template decision cannot be made from the derived Manifest data and the
approved Template inventory / mapping rules, Template Mapping must hard-stop.

## Output Contract

Template Mapping output is a deterministic mapping result for downstream
GenerateContext preparation.

The output may contain only:

- selected Template identity or key
- module-to-template binding
- procedure-to-template binding where supported by the current flow
- supported generation policy binding
- Template-relative technical metadata required by the downstream
  GenerateContext boundary
- diagnostics that explain mapping failure without producing downstream input

The output must not introduce:

- new modules
- new procedures
- new parameters
- new dependencies
- new approval state
- inferred design intent
- generated VBA code
- GenerateContext construction behavior
- Generator execution behavior

## Decisions Owned By Template Mapping

Template Mapping owns only decisions that bind already-derived Manifest facts to
approved Template identities.

Template Mapping decides:

- whether the Manifest module kind maps to exactly one approved Template
- whether the Manifest procedure kind maps to exactly one approved Template
  binding when procedure-level binding is supported
- whether visibility, return value, parameters, and generation policy values
  are supported by an approved Template mapping rule
- whether ordering from Manifest data can be preserved without conflict
- whether the mapping result is complete enough to pass to GenerateContext
- whether a mapping failure must hard-stop before GenerateContext and Generator

Template Mapping decisions must be deterministic. The same derived Manifest data
and the same approved Template mapping rules must produce the same result.

## Decisions Not Owned By Template Mapping

Template Mapping does not decide:

- whether Blueprint syntax is valid
- whether Blueprint semantics are valid
- whether `Result.Generatable = True`
- whether Blueprint approval state is acceptable
- how Manifest data is derived
- missing `LayerName`, module, procedure, parameter, dependency, or generation
  policy values
- Template file format changes
- GenerateContext schema or construction rules
- Generator invocation, output, write behavior, overwrite policy enforcement, or
  VBA formatting
- fallback behavior for unsupported Template selections unless a separate
  approved mapping rule exists

Template Mapping must not repair, normalize, infer, or complete upstream data.

## Hard-Stop Conditions

Template Mapping must hard-stop when:

- Manifest Derivation did not complete successfully
- derived Manifest data is missing
- a required mapping input field is missing
- a required mapping input field is blank where a concrete value is required
- a Manifest module kind has no approved Template mapping
- a Manifest procedure kind has no approved Template mapping where
  procedure-level mapping is required
- more than one Template mapping applies to the same Manifest item
- mapping depends on raw Blueprint content or Validator diagnostics
- mapping requires Template fallback behavior that has not been approved
- mapping requires inferring design intent not present in Manifest data
- mapping requires changing Template files
- mapping requires changing GenerateContext or Generator behavior
- mapping requires accepting unsupported generation policy values
- mapping produces incomplete downstream binding data

Template Mapping failure must remain distinct from:

- parse failure
- validation failure
- Manifest Derivation failure
- GenerateContext construction failure
- Generator failure

Template Mapping failure must not produce GenerateContext input and must not
invoke Generator behavior.

## GenerateContext / Generator Boundary

GenerateContext may consume only a successful Template Mapping result and the
approved Manifest-derived data required by its own future contract.

P4-05 does not define GenerateContext construction rules. It fixes only the
handoff rule: unresolved, ambiguous, unsupported, or unapproved Template Mapping
state stops before GenerateContext.

Generator may consume only a valid GenerateContext produced by a later approved
boundary. Template Mapping does not call Generator and does not expose partial
Generator input.

## Contract Freeze

The frozen P4-05 contract is:

- Template Mapping is after Manifest Derivation and before GenerateContext.
- Template Mapping consumes only successfully derived Manifest data.
- Template Mapping selects and binds approved Templates deterministically.
- Template Mapping does not infer upstream design intent.
- Template Mapping does not construct GenerateContext.
- Template Mapping does not invoke Generator behavior.
- Mapping uncertainty, ambiguity, unsupported input, or missing approval is a
  hard stop before downstream generation.

Any future implementation must preserve this contract unless a separate
authorized planning record changes it.

## Deferred Items

Deferred items:

- exact Template inventory review
- concrete Template mapping table
- GenerateContext construction contract
- Generator invocation contract
- Template fallback policy
- Template file format changes, if any
- focused implementation test list
- implementation GO / NO-GO decision

## Verification Performed

P4-05 verification is docs-only:

- reviewed P4-01 Manifest Derivation scope planning
- reviewed P4-04 Template Mapping scope planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
