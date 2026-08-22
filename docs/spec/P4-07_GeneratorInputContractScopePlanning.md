# P4-07 - Generator Input Contract Scope Planning

## Status

COMPLETE / docs-only scope planning

## Purpose

Fix the Generator input contract after P4-06 GenerateContext responsibility
boundary freeze and before any Generator implementation work.

P4-07 limits Generator input to a complete and successful GenerateContext
result. It records the required input items, responsibility boundary, failure
boundary, and upstream connection. It does not authorize implementation GO.

## Scope

P4-07 defines:

- the Generator position in the Build vNext generation flow
- the only allowed Generator input boundary
- required GenerateContext-provided input items
- Generator responsibilities after the input boundary
- responsibilities explicitly not owned by Generator
- hard-stop conditions before Generator behavior changes or runtime execution
- the connection to Parser, Validator, Manifest Derivation, Template Mapping,
  and GenerateContext

## Non-Scope

P4-07 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Parser changes
- Validator changes
- Manifest Derivation changes
- Template Mapping changes
- GenerateContext implementation changes
- Template file changes
- Generator implementation changes
- runtime behavior changes
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

Generator starts only after GenerateContext has completed successfully.

Generator must not be invoked from failed, partial, unresolved, unsupported, or
incomplete upstream state. Generator input is not raw Blueprint, parsed
Blueprint, Validator output, derived Manifest data, Template Mapping output, or
Template file contents independently. Generator input is the complete
GenerateContext result only.

## Input Contract

The only allowed Generator input is a successful GenerateContext result.

The GenerateContext result must be complete enough to provide all data required
for Generator consumption without adding extra Generator parameters for
upstream design facts.

Required input items are:

- selected Template identity or binding data fixed upstream by Template Mapping
- Manifest-derived module facts required for generation
- Manifest-derived procedure facts required for generation
- Manifest-derived parameter facts required for generation
- Manifest-derived return value facts required for generation
- Manifest-derived visibility facts required for generation
- Manifest-derived dependency facts required for generation
- Manifest-derived generation policy facts required for generation
- deterministic generation unit ordering when ordering is required
- deterministic technical metadata required by Generator consumption
- a success state that confirms GenerateContext construction completed

Generator must not accept additional arguments to compensate for missing
GenerateContext data. If required input is absent from GenerateContext, the
pipeline must hard-stop before Generator.

## Disallowed Input Sources

Generator must not read or depend on:

- raw Blueprint text
- unvalidated parsed Blueprint state
- Validator diagnostics
- Manifest Derivation diagnostics
- failed or partial Manifest Derivation output
- failed or partial Template Mapping output
- failed, partial, unresolved, unsupported, or incomplete GenerateContext state
- Template file contents as a source of design intent
- ad hoc parameters that bypass GenerateContext
- external runtime state as a source of generation design

Generator must not repair, normalize, infer, or complete upstream data.

## Generator Responsibility Boundary

Generator owns only the final generation step that consumes an already complete
GenerateContext result through the approved generation engine boundary.

Generator may be responsible for:

- consuming the complete GenerateContext result
- applying approved Template binding data supplied by GenerateContext
- using approved token replacement or equivalent template mediation already
  defined by the Build architecture
- reflecting generated output into the target VBA project through the approved
  Generator Engine responsibility
- failing when the GenerateContext input is absent, partial, inconsistent, or
  unsupported

Generator must preserve the Build Canon rule that variable generation data is
centralized in GenerateContext. Generator must not introduce design changes by
adding separate input arguments for variable data.

## Responsibilities Not Owned By Generator

Generator does not decide:

- whether Blueprint syntax is valid
- whether Blueprint semantics are valid
- whether `Result.Generatable = True`
- whether Blueprint approval state is acceptable
- how Manifest data is derived
- which Templates are selected
- whether Template Mapping output is complete
- whether GenerateContext construction is complete
- missing upstream module, procedure, parameter, dependency, visibility, return
  value, generation policy, or approval values
- Template fallback behavior
- Template file format changes
- new module, procedure, dependency, approval, or generation policy creation

Generator must not treat runtime output success as proof that upstream contract
state was valid.

## Failure Boundary

Generator must hard-stop before runtime generation when:

- GenerateContext did not complete successfully
- GenerateContext input is missing
- GenerateContext input is partial
- GenerateContext input is marked failed
- required Template binding data is absent from GenerateContext
- required Manifest-derived module, procedure, parameter, return value,
  visibility, dependency, or generation policy data is absent from
  GenerateContext
- required generation unit ordering cannot be read from GenerateContext
- Generator would need raw Blueprint content, Validator diagnostics, Manifest
  Derivation diagnostics, or Template Mapping diagnostics to continue
- Generator would need to re-run or reinterpret upstream decisions
- Generator would need to infer design intent not present in GenerateContext
- Generator would need extra arguments to compensate for incomplete
  GenerateContext data
- Generator would need to change Template files or Template Mapping decisions

Generator failure must remain distinct from:

- parse failure
- validation failure
- Manifest Derivation failure
- Template Mapping failure
- GenerateContext construction failure

A successful upstream pipeline through GenerateContext is required before
Generator execution. A Generator hard-stop must not convert upstream failures
into partial generated output.

## Upstream Connection

The upstream connection fixed by P4-07 is:

- Parser provides parse results only to the validated upstream flow.
- Validator determines semantic validity and generatable eligibility.
- Manifest Derivation consumes only Validator-passed generatable Blueprint
  input and produces derived Manifest data.
- Template Mapping consumes only successfully derived Manifest data and
  produces deterministic Template binding data.
- GenerateContext consumes only successful Template Mapping output and approved
  Manifest-derived data, then packages the complete Generator-ready context.
- Generator consumes only the complete successful GenerateContext result.

Each upstream failure remains a hard stop at its own boundary. Generator must
not inspect earlier pipeline state to recover from, reinterpret, or bypass that
failure.

## Scope Planning Decision

GO:

- P4-07 docs-only Generator input contract scope planning

NO-GO:

- Generator implementation
- Generator code changes
- runtime behavior changes
- GenerateContext implementation changes
- Template Mapping implementation changes
- Manifest Derivation implementation changes
- Parser or Validator changes
- test additions or updates
- package, `dist`, build, release, external service, staging, commit, or push
  operations

## Minimum Future Implementation Slice

A future implementation slice may be considered only after separate
authorization. The minimum slice should:

- introduce or adapt a Generator entry boundary that accepts only a successful
  GenerateContext result
- reject missing, partial, failed, or incomplete GenerateContext input before
  runtime generation
- preserve existing public contracts unless separately authorized
- preserve Template Mapping and GenerateContext responsibility boundaries
- preserve Build Canon Rule 4 by avoiding ad hoc Generator arguments for
  variable generation data
- include focused local verification for valid GenerateContext input and
  pre-Generator hard-stop cases

This section is planning only. It is not implementation authorization.

## Deferred Items

Deferred items:

- concrete Generator entry point design
- concrete GenerateContext data model
- token replacement interface review
- Template inventory review
- Generator output behavior review
- overwrite policy review
- focused implementation test list
- implementation GO / NO-GO decision

## Verification Performed

P4-07 verification is docs-only:

- reviewed P4-05 Template Mapping contract freeze
- reviewed P4-06 GenerateContext responsibility boundary freeze
- reviewed Build Canon v1.0 Context Driven and Generator Pipeline rules
- reviewed Build Blueprint v1.0.1 authoritative flow
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
