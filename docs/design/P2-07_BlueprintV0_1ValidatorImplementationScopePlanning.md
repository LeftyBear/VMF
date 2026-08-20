# P2-07 — Blueprint v0.1 Validator Implementation Scope Planning

## Status

COMPLETE / docs-only implementation scope planning

## Purpose

Define the minimum implementation scope for a future Blueprint v0.1 Validator.

This document uses P2-04 validation rules, P2-05 error classifications, and P2-06 validation error codes as the implementation planning baseline.

## Scope

This task is docs-only.

It defines:

- Validator responsibilities
- Validator non-responsibilities
- input and output model
- validation result categories
- diagnostic handling
- Manifest derivation eligibility judgment
- focused test plan
- implementation GO / NO-GO boundary

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Implementation Position

The Validator sits after Blueprint parsing and before Manifest derivation.

```text
Blueprint document
-> Blueprint parser
-> Blueprint Validator
-> Manifest derivation
-> Template
-> GenerateContext
-> Generator
-> VBA
```

## Validator Responsibilities

The Validator is responsible for:

- validating Blueprint v0.1 structure
- checking required fields
- checking enum values
- checking status / approval consistency
- checking target definitions
- checking module definitions
- checking procedure definitions
- checking Function / returnValue rules
- checking Sub / returnValue prohibition
- checking parameter definitions
- checking dependency definitions
- checking generationPolicy
- returning validation diagnostics
- returning Manifest derivation eligibility

## Validator Non-Responsibilities

The Validator must not:

- parse raw YAML
- generate Manifest files
- generate Template files
- create GenerateContext
- generate VBA
- edit Blueprint documents
- infer missing design intent
- approve Blueprint documents
- change approval state
- add modules
- add procedures
- add dependencies
- resolve ambiguous design decisions

Blueprint remains the Single Source of Truth.

## Input

The Validator input is a parsed Blueprint model.

Expected input boundary:

- raw YAML parsing is already complete
- syntax-level YAML errors are outside Validator scope
- the Validator receives a structured object model
- missing fields may be represented as absent/null values

## Output

The Validator output should contain:

- validation result category
- Manifest derivation eligibility
- diagnostics list

Conceptual shape:

```yaml
result: invalid
generatable: false
diagnostics:
  - code: BP421
    category: InvalidReturnValue
    severity: error
    field: modules[0].procedures[0].returnValue
```

This task does not implement the final object format.

## Validation Result Categories

The Validator should return one of:

| Result | Meaning | generatable |
|---|---|---:|
| `validGeneratable` | Blueprint is valid and approved | true |
| `validNotGeneratable` | Blueprint is structurally valid but not approved | false |
| `invalid` | Blueprint violates validation rules | false |

## Diagnostic Rules

Diagnostics should:

- use P2-06 error codes
- use P2-05 categories
- identify the affected field when possible
- preserve all detected validation failures where practical
- avoid changing Blueprint content
- avoid implementation-specific wording in stable fields

## Error Code Use

The future Validator should use P2-06 codes as follows:

- required field failures: `BP001`-`BP099`
- enum/value failures: `BP101`-`BP199`
- approval/generation eligibility: `BP201`-`BP204`
- target/module failures: `BP301`-`BP325`
- procedure/returnValue/parameter failures: `BP401`-`BP444`
- dependency failures: `BP501`-`BP503`
- generationPolicy failures: `BP601`-`BP606`
- Manifest boundary failures: `BP701`-`BP706`

## `BP204` Handling

`BP204` represents a structurally valid Blueprint that is not eligible for Manifest derivation.

Examples:

- `status = draft`
- `status = review`
- `status = rejected`
- `status = superseded`

Rules:

- `BP204` should not make the Blueprint structurally invalid
- `BP204` must set `generatable = false`
- `BP204` must prevent Manifest derivation
- `BP204` should be distinguishable from blocking structural errors

## Manifest Derivation Eligibility

Manifest derivation is allowed only when:

- validation result is `validGeneratable`
- `generatable = true`
- no `error` severity diagnostics exist
- `status = approved`
- `approval.state = approved`

Manifest derivation is not allowed when:

- validation result is `invalid`
- validation result is `validNotGeneratable`
- `generatable = false`
- any blocking validation error exists

## Minimum Implementation Slice

A future implementation should start with the smallest useful slice:

1. accept parsed Blueprint model
2. validate top-level required fields
3. validate status and approval consistency
4. validate modules minimum count
5. validate procedure kind and returnValue rules
6. validate generationPolicy required values
7. return result category
8. return diagnostics with P2-06 codes
9. return `generatable`

## Deferred Implementation Areas

The following should remain deferred unless separately approved:

- raw YAML parsing changes
- schema file generation
- auto-fix behavior
- AI Blueprint draft generation
- UI review workflow
- Manifest file writing
- Template generation
- GenerateContext generation
- VBA Generator changes
- Excel runtime execution

## Focused Test Plan

Future focused tests should cover:

| Test Area | Expected Coverage |
|---|---|
| valid minimal Blueprint | `validGeneratable`, no diagnostics |
| draft Blueprint | `validNotGeneratable`, `BP204` |
| rejected Blueprint | `validNotGeneratable`, `BP204` |
| missing top-level field | `invalid`, required field code |
| unsupported enum | `invalid`, enum code |
| approval conflict | `invalid`, `BP201` |
| Function without returnValue | `invalid`, `BP421` |
| Sub with returnValue | `invalid`, `BP423` |
| empty modules | `invalid`, `BP321` |
| invalid generationPolicy | `invalid`, `BP601`-`BP606` |

## Implementation GO / NO-GO Boundary

P2-07 itself is not implementation authorization.

Implementation remains NO-GO until a later task explicitly authorizes it.

A future GO task must specify:

- files to modify
- Validator entry point
- test files to add
- expected test command
- no-change boundary for Generator behavior
- no Manifest generation side effects

## Out of Scope

P2-07 does not define:

- final parser API
- final Validator class name
- final diagnostic object type
- final Manifest format
- implementation file paths
- production code changes
- test code changes

## Verification

Docs-only task.

Expected checks:

- Markdown added
- No VBA implementation changed
- No parser behavior changed
- No validator behavior changed
- No generator behavior changed
- No test execution required
