# P4-06 - GenerateContext Responsibility Boundary Freeze

## Status

COMPLETE / docs-only responsibility boundary freeze

## Purpose

Freeze the GenerateContext responsibility boundary after P4-05 Template Mapping
contract freeze and before any Generator implementation work.

P4-06 fixes GenerateContext as the boundary that converts successful Template
Mapping output and approved Manifest-derived data into a deterministic context
for Generator consumption. It does not authorize implementation GO.

## Scope

P4-06 defines:

- the GenerateContext position in the Build vNext generation flow
- the allowed GenerateContext input boundary
- the GenerateContext output boundary
- decisions owned by GenerateContext
- decisions explicitly not owned by GenerateContext
- hard-stop conditions before Generator behavior

## Non-Scope

P4-06 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Parser changes
- Validator changes
- Manifest Derivation changes
- Template Mapping changes
- Template file changes
- Generator implementation
- refactoring
- package, `dist`, build, or release operations
- external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Boundary Position

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

GenerateContext starts only after Template Mapping has completed successfully.

GenerateContext ends before Generator invocation. A successful GenerateContext
result may be consumed by a later Generator boundary, but GenerateContext does
not invoke Generator behavior and does not produce VBA output.

## Input Boundary

GenerateContext may consume only:

- a successful Template Mapping result
- approved Manifest-derived data required by the GenerateContext contract
- deterministic technical metadata produced by Template Mapping for downstream
  generation
- approved Template identity or binding information selected by Template
  Mapping

GenerateContext must not read or depend on:

- raw Blueprint text
- unvalidated parsed Blueprint state
- Validator diagnostics
- Manifest Derivation diagnostics except as a prior hard-stop condition
- failed or partial Template Mapping output
- Template file contents as a source of design intent
- Generator state or Generator output behavior

If GenerateContext construction cannot be completed from successful Template
Mapping output and approved Manifest-derived data, GenerateContext must
hard-stop before Generator.

## Output Boundary

GenerateContext output is a deterministic context object or equivalent
downstream input package for Generator consumption.

The output may contain only:

- selected Template identity or binding data already fixed by Template Mapping
- Manifest-derived module facts required for generation
- Manifest-derived procedure facts required for generation
- Manifest-derived parameter, return value, visibility, dependency, and
  generation policy facts required for generation
- ordered generation units when order is already represented by upstream
  approved data
- deterministic technical metadata required by Generator consumption
- diagnostics that explain GenerateContext construction failure without
  producing Generator input

The output must not introduce:

- new modules
- new procedures
- new parameters
- new dependencies
- new approval state
- inferred design intent
- Template selection changes
- generated VBA code
- Generator invocation behavior
- file write behavior
- overwrite policy enforcement

## Decisions Owned By GenerateContext

GenerateContext owns only decisions that package already-approved upstream data
into Generator-ready context.

GenerateContext decides:

- whether successful Template Mapping output is present and complete
- whether the required Manifest-derived facts are present for each mapped
  generation unit
- whether Template Mapping output and Manifest-derived data are internally
  consistent
- whether generation units can be ordered deterministically from approved
  upstream data
- whether the context is complete enough to pass to Generator
- whether construction failure must hard-stop before Generator

GenerateContext decisions must be deterministic. The same successful Template
Mapping output and the same approved Manifest-derived data must produce the same
GenerateContext result.

## Decisions Not Owned By GenerateContext

GenerateContext does not decide:

- whether Blueprint syntax is valid
- whether Blueprint semantics are valid
- whether `Result.Generatable = True`
- whether Blueprint approval state is acceptable
- how Manifest data is derived
- which Templates are selected
- Template fallback behavior
- Template file format changes
- missing upstream module, procedure, parameter, dependency, visibility, return
  value, generation policy, or approval values
- Generator invocation
- Generator output, formatting, write behavior, overwrite policy enforcement,
  or VBA emission

GenerateContext must not repair, normalize, infer, or complete upstream data.

## Hard-Stop Conditions

GenerateContext must hard-stop when:

- Template Mapping did not complete successfully
- Template Mapping output is missing
- Template Mapping output is partial or marked failed
- required Manifest-derived data is missing
- Template Mapping output conflicts with Manifest-derived data
- required generation unit ordering cannot be determined from approved upstream
  data
- construction requires reading raw Blueprint content or Validator diagnostics
- construction requires re-running Template Mapping decisions
- construction requires Template fallback behavior that has not been approved
- construction requires inferring design intent not present in Manifest-derived
  data
- construction requires changing Template files
- construction requires changing Generator behavior
- construction would produce incomplete Generator input

GenerateContext failure must remain distinct from:

- parse failure
- validation failure
- Manifest Derivation failure
- Template Mapping failure
- Generator failure

GenerateContext failure must not invoke Generator behavior and must not produce
partial Generator input.

## Generator Boundary

Generator may consume only a complete and successful GenerateContext result
produced by a later approved implementation boundary.

P4-06 does not define Generator invocation rules, output rules, formatting
rules, write behavior, overwrite policy, or VBA emission. It fixes only the
handoff rule: unresolved, inconsistent, unsupported, or incomplete
GenerateContext state stops before Generator.

## Responsibility Boundary Freeze

The frozen P4-06 boundary is:

- GenerateContext is after Template Mapping and before Generator.
- GenerateContext consumes only successful Template Mapping output and approved
  Manifest-derived data.
- GenerateContext packages upstream data into deterministic Generator-ready
  context.
- GenerateContext does not select Templates.
- GenerateContext does not infer upstream design intent.
- GenerateContext does not invoke Generator behavior.
- GenerateContext uncertainty, inconsistency, unsupported input, or incomplete
  input is a hard stop before downstream generation.

Any future implementation must preserve this boundary unless a separate
authorized planning record changes it.

## Deferred Items

Deferred items:

- concrete GenerateContext data model
- concrete GenerateContext entry point
- concrete Generator invocation contract
- focused implementation test list
- Template inventory review
- Template file format changes, if any
- Generator output behavior
- implementation GO / NO-GO decision

## Verification Performed

P4-06 verification is docs-only:

- reviewed P4-04 Template Mapping scope planning
- reviewed P4-05 Template Mapping contract freeze
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
