# P4-01 - Manifest Derivation Scope Planning

## Status

COMPLETE / docs-only planning

## Purpose

Fix the responsibility boundary for deriving Manifest data from a Validated
Blueprint before any implementation task changes the Parser, Validator,
Manifest generation, Template mapping, GenerateContext, Generator, tests, or
release artifacts.

P4-01 is documentation only. It does not authorize implementation GO.

## Scope

P4-01 defines:

- the Validated Blueprint input boundary
- the Manifest output boundary
- the Manifest derivation responsibility boundary
- transformation rules from Blueprint fields to Manifest data
- failure boundary and hard-stop conditions
- relationship to the existing Parser, Validator, and pre-Manifest flow
- the minimum future implementation slice that may be evaluated later

## Non-Scope

P4-01 does not perform or authorize:

- production VBA code changes
- test code additions
- Parser changes
- Validator changes
- Manifest derivation implementation
- Template mapping implementation or planning
- GenerateContext implementation or planning
- Generator implementation or planning
- refactoring
- package, `dist`, build, or release operations
- external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Preconditions

Manifest derivation may be considered only after all of these are true:

- Blueprint parsing has completed successfully.
- The parsed Blueprint has been passed to the Validator.
- The Validator result is valid and generatable.
- `Result.Generatable = True`.
- The Blueprint is approved according to the P2 approval rules.
- No validation diagnostics require a hard stop.

Parser failure stops before Validator.

Validation failure stops before Manifest derivation.

Manifest derivation failure stops before Template, GenerateContext, and
Generator behavior.

## Input: Validated Blueprint

The only input to Manifest derivation is a Validated Blueprint.

A Validated Blueprint means a parsed Blueprint model that has already passed
the Validator as valid and generatable. A merely parsed Blueprint is not a
Manifest derivation input. A structurally valid but not generatable Blueprint is
not a Manifest derivation input.

Allowed source data is limited to Blueprint v0.1 fields already covered by P2:

- `blueprintId`
- `version`
- `target.kind`
- `target.name`
- `target.description`
- `modules[].name`
- `modules[].kind`
- `modules[].responsibility`
- `modules[].procedures[]`
- `procedures[].name`
- `procedures[].kind`
- `procedures[].visibility`
- `procedures[].responsibility`
- `procedures[].parameters[]`
- `parameters[].name`
- `parameters[].type`
- `parameters[].passing`
- `parameters[].optional`
- `parameters[].defaultValue`
- `procedures[].returnValue`
- `returnValue.type`
- `returnValue.description`
- `procedures[].dependencies[]`
- `dependencies[].kind`
- `dependencies[].name`
- `dependencies[].reason`
- `generationPolicy.allowOverwrite`
- `generationPolicy.encoding`
- `generationPolicy.lineEnding`
- `generationPolicy.missingDirectoryPolicy`

Manifest derivation must not read unvalidated raw Blueprint text to make design
decisions after Validator PASS.

## Output: Manifest

The Manifest output is derived generation data for the downstream generation
flow.

The Manifest may contain only data derived from the Validated Blueprint and any
deterministic technical representation required by the existing Manifest
consumer boundary.

The Manifest must not become a maintained design source. It must not introduce:

- new modules
- new procedures
- new responsibilities
- new dependencies
- new approval state
- new target intent
- new generation policy intent
- independent design decisions

If an existing Manifest consumer requires fields that cannot be derived from the
Validated Blueprint without guessing, derivation must hard-stop and record the
gap for a later planning or implementation task.

## Responsibility Boundary

Manifest derivation is an independent boundary between Validator and Template.

The derivation responsibility is:

- consume only a Validated Blueprint
- map supported Blueprint fields into Manifest data
- preserve Blueprint order where order is meaningful
- apply deterministic representation choices that do not change design intent
- reject unsupported, incomplete, ambiguous, or internally inconsistent input
- return a Manifest derivation failure when Manifest data cannot be completed

The derivation responsibility is not:

- parsing Blueprint syntax
- validating Blueprint semantics
- repairing Blueprint content
- normalizing Blueprint for the caller
- approving Blueprint content
- selecting templates
- constructing GenerateContext
- invoking Generator behavior
- producing VBA output
- changing existing parser or Validator result semantics

Parser and Validator do not convert Blueprint to Manifest.

Validator PASS only establishes eligibility to derive Manifest data. It does
not itself create Manifest data and does not imply downstream Template,
GenerateContext, or Generator success.

## Transformation Rules

Manifest derivation must use explicit, deterministic mapping rules:

| Blueprint source | Manifest derivation rule |
| --- | --- |
| `blueprintId` | Preserve as the source Blueprint identity or trace field if the Manifest representation supports it. |
| `version` | Preserve as the source Blueprint version or trace field if the Manifest representation supports it. |
| `target.kind` | Map only supported target kinds. Unsupported values hard-stop even if a future Blueprint version later allows them. |
| `target.name` | Preserve as the target name without inventing workbook, add-in, or project identity. |
| `target.description` | Preserve only as descriptive metadata when the Manifest representation has a compatible field. |
| `modules[]` | Derive one Manifest module entry per Blueprint module. Do not add, merge, split, or reorder modules by assumption. |
| `modules[].name` | Preserve as the module name. Missing, duplicate, or unsupported names hard-stop if not already stopped by validation. |
| `modules[].kind` | Map only supported module kinds. Unsupported kinds hard-stop. |
| `modules[].responsibility` | Preserve only as metadata or documentation input when a compatible Manifest field exists. Do not generate behavior from responsibility prose. |
| `procedures[]` | Derive procedure data only within the owning module. Do not move procedures across modules. |
| `procedures[].name` | Preserve as the procedure name. Missing, duplicate, or unsupported names hard-stop if not already stopped by validation. |
| `procedures[].kind` | Map only `Sub` and `Function` as defined by Blueprint v0.1. |
| `procedures[].visibility` | Preserve as the requested VBA visibility when the Manifest representation supports it. |
| `parameters[]` | Preserve parameter order and fields. Do not infer additional parameters. |
| `parameters[].passing` | Preserve explicit `ByVal` or `ByRef`; if omitted, apply only an already-approved VMF default policy. If no approved default exists at implementation time, hard-stop. |
| `parameters[].optional` and `defaultValue` | Preserve optional/default metadata only when compatible with the Manifest representation. Do not infer defaults. |
| `returnValue` | Required only for `Function`; preserve return type and compatible metadata. Do not infer return type. |
| `dependencies[]` | Preserve declared dependencies only. Do not discover or infer dependencies. |
| `generationPolicy` | Carry supported policy values needed by the downstream generation flow. Unsupported or unmapped policy requirements hard-stop. |

Where existing Manifest format or consumer behavior conflicts with these rules,
P4-01 records the issue as conflict/deferred. It does not rewrite existing
Frozen specifications or implementation behavior.

## Failure Boundary / Hard-Stop Conditions

Manifest derivation must hard-stop when:

- the input is not a Validated Blueprint
- `Result.Generatable` is not `True`
- the Blueprint is unapproved, rejected, draft, review, or superseded and not
  generatable
- required derivation source data is missing
- source data is ambiguous
- source data uses an unsupported Blueprint version
- source data uses unsupported target, module, procedure, parameter,
  dependency, or generation policy values
- deriving a Manifest would require adding design intent not present in the
  Blueprint
- deriving a Manifest would require repairing, rewriting, or normalizing the
  Blueprint for the caller
- existing Manifest output requirements cannot be satisfied without guessing
- Template, GenerateContext, or Generator behavior would need to be changed to
  complete the derivation task

Manifest derivation failure must remain distinct from:

- parse failure
- validation failure
- Template mapping failure
- GenerateContext construction failure
- Generator failure

It must not be used to hide failed validation.

## Relationship To Existing Parser / Validator / Pre-Manifest Flow

Current flow:

```text
Blueprint
-> Parser
-> Validator
-> Manifest derivation
-> Template
-> GenerateContext
-> Generator
```

The Parser is responsible for converting Blueprint syntax and format into a
parsed Blueprint model. It does not decide approval, does not validate
generatability, and does not own Manifest derivation.

The Validator is responsible for semantic validation and the generatable
decision for an already parsed Blueprint. It does not modify, repair, normalize,
or convert Blueprint content into Manifest data.

The pre-Manifest flow is complete only when the parsed Blueprint has passed
validation as valid and generatable. P3-07 confirmed the existing hard-stop
behavior before Manifest generation when validation fails.

Manifest derivation starts after that hard-stop boundary. It is the next
independent boundary before Template, GenerateContext, and Generator.

## Minimum Future Implementation Slice

A future implementation task may be considered only after a separate GO
decision.

The minimum future slice should be limited to:

- one Manifest derivation entry point after Validator PASS
- one input contract that accepts only the existing validated Blueprint model or
  a narrowly adapted equivalent
- deterministic mapping for target, modules, procedures, parameters,
  return value, dependencies, and generation policy fields that are already
  supported by Blueprint v0.1
- explicit hard-stop result when required Manifest data cannot be derived
- focused tests for approved valid Blueprint, non-generatable Blueprint hard
  stop, unsupported value hard stop, and no Template / GenerateContext /
  Generator invocation on derivation failure

The minimum slice must preserve:

- Parser behavior
- Validator behavior
- validation result contracts
- Template behavior
- GenerateContext behavior
- Generator behavior
- public API and persisted schema boundaries unless separately authorized
- existing Build regression behavior

## Deferred Items

Deferred items:

- exact Manifest file format changes, if any
- compatibility review against every existing Manifest consumer field
- Template mapping rules
- GenerateContext construction rules
- Generator output behavior
- AI Blueprint draft generation
- Blueprint repair or normalization
- advanced type inference
- dependency discovery
- cross-module ordering policy beyond preserving Blueprint order
- implementation GO / NO-GO decision

## Verification Performed

P4-01 verification is docs-only:

- reviewed P2 Blueprint scope, field model, and validation-rule records
- reviewed P3 Validator failure and completion records
- reviewed P3-08 next-candidate selection
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, or push operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
