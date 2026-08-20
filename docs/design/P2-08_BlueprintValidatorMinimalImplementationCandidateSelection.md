# P2-08 — Blueprint Validator Minimal Implementation Candidate Selection

## Status

COMPLETE / docs-only candidate selection

## Purpose

Select the minimum implementation candidate for a future Blueprint v0.1 Validator.

This document follows P2-07 implementation scope planning and decides which validation slice should be implemented first.

## Scope

This task is docs-only.

It defines:

- candidate implementation slices
- comparison criteria
- selected candidate
- rationale
- implementation boundary
- deferred areas
- focused test direction

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Baseline

P2-08 assumes the following completed documents:

- P2-01 — Blueprint Specification v0.1 Scope Definition
- P2-02 — Blueprint Specification v0.1 Field Model Definition
- P2-03 — Blueprint Specification v0.1 Example Documents
- P2-04 — Blueprint v0.1 Validation Rule Definition
- P2-05 — Blueprint v0.1 Error Classification Definition
- P2-06 — Blueprint v0.1 Validation Error Code Definition
- P2-07 — Blueprint v0.1 Validator Implementation Scope Planning

## Candidate Selection Criteria

The first Validator implementation slice should:

- be small enough to implement safely
- validate meaningful Blueprint generation eligibility
- use stable P2-06 error codes
- avoid Generator behavior changes
- avoid Manifest generation side effects
- support focused tests
- preserve Blueprint as the Single Source of Truth

## Candidate A — Top-Level Validation Only

### Scope

Candidate A validates only top-level Blueprint fields:

- `blueprintId`
- `version`
- `status`
- `approval`
- `target`
- `modules`
- `generationPolicy`

### Included Rules

- required top-level fields
- supported `version`
- supported `status`
- supported `approval.state`
- basic presence of `target`
- basic presence of `modules`
- basic presence of `generationPolicy`

### Advantages

- smallest implementation
- lowest risk
- easy to test
- starts using P2-06 error codes

### Limitations

- does not validate procedure shape
- does not validate Function / returnValue rules
- does not validate Sub / returnValue prohibition
- cannot fully classify P2-03 example set
- provides limited Manifest derivation confidence

### Assessment

Candidate A is safe but too narrow for the first useful Validator slice.

## Candidate B — Minimal Generatable Validation

### Scope

Candidate B validates the minimum useful set needed to determine whether a Blueprint is structurally valid and eligible for Manifest derivation.

Included:

- top-level required fields
- `version`
- `status`
- `approval`
- status / approval consistency
- Manifest derivation eligibility
- `target`
- `modules` minimum count
- module required fields
- procedure required fields
- procedure kind
- procedure visibility
- Function / returnValue rule
- Sub / returnValue prohibition
- generationPolicy required values

### Included Error Code Areas

Candidate B should cover:

- `BP001`-`BP008`
- `BP101`-`BP107`
- `BP201`-`BP204`
- `BP301`-`BP325`
- `BP401`-`BP405`
- `BP421`-`BP423`
- `BP601`-`BP606`

### Advantages

- matches the P2-07 minimum implementation slice
- validates meaningful generation eligibility
- supports `validGeneratable`
- supports `validNotGeneratable`
- supports `invalid`
- covers the most important P2-03 examples
- avoids parameter and dependency complexity in the first implementation
- does not require Generator changes
- does not require Manifest file writing

### Limitations

- does not validate parameters
- does not validate dependencies
- does not validate Manifest boundary violations deeply
- does not cover the full P2-04 rule set

### Assessment

Candidate B is the best first implementation candidate because it is small but still useful.

## Candidate C — Full v0.1 Validation

### Scope

Candidate C implements all P2-04 validation rules and all P2-06 error codes.

Included:

- all Candidate B rules
- parameter validation
- dependency validation
- full Manifest boundary validation
- all remaining v0.1 validation codes

### Advantages

- closest to complete v0.1 specification
- reduces later follow-up work
- validates the full documented model

### Limitations

- larger initial implementation
- higher risk
- more tests required
- may force premature decisions about parameter and dependency behavior
- may create pressure to connect Validator to Manifest derivation too early

### Assessment

Candidate C is appropriate later, but too broad for the first implementation slice.

## Selected Candidate

Selected:

**Candidate B — Minimal Generatable Validation**

## Rationale

Candidate B is selected because it:

- matches P2-07 implementation planning
- validates the minimum meaningful Blueprint shape
- determines Manifest derivation eligibility
- uses stable error codes from P2-06
- covers the core approved / draft / rejected / invalid example cases from P2-03
- keeps implementation isolated from Generator behavior
- keeps Manifest generation out of scope
- preserves Blueprint as the Single Source of Truth

## Expected Result Categories

Candidate B should support:

| Result | Meaning |
|---|---|
| `validGeneratable` | Blueprint is structurally valid and approved |
| `validNotGeneratable` | Blueprint is structurally valid but not approved |
| `invalid` | Blueprint violates validation rules |

## Candidate B Example Coverage

| P2-03 Example | Expected Result | Expected Code |
|---|---|---|
| Minimal approved Blueprint | `validGeneratable` | none |
| Function with returnValue | `validGeneratable` | none |
| Multiple modules | `validGeneratable` | none |
| Draft Blueprint | `validNotGeneratable` | `BP204` |
| Rejected Blueprint | `validNotGeneratable` | `BP204` |
| Function without returnValue | `invalid` | `BP421` |
| Approved status without approved approval state | `invalid` | `BP201` |
| Sub with returnValue | `invalid` | `BP423` |

## Implementation Boundary for Future Task

A later implementation task may implement Candidate B only.

Allowed future implementation scope:

- create Validator entry point
- accept parsed Blueprint model
- return validation result category
- return `generatable`
- return diagnostics with P2-06 codes
- add focused tests for Candidate B rules

Not allowed without separate approval:

- raw YAML parser rewrite
- Manifest file generation
- Template changes
- GenerateContext changes
- VBA Generator changes
- Excel runtime execution
- AI Blueprint generation
- UI review workflow
- automatic Blueprint repair

## Deferred Areas

The following are deferred:

- parameter validation beyond initial omission tolerance
- dependency validation beyond initial omission tolerance
- full Manifest boundary validation
- final diagnostic message wording
- localization
- schema file generation
- auto-fix behavior

## Focused Test Direction

Future tests should prioritize:

- valid minimal approved Blueprint
- valid Function with returnValue
- valid multiple modules
- draft Blueprint returns not generatable
- rejected Blueprint returns not generatable
- missing required top-level field
- unsupported status
- approval conflict
- empty modules
- missing module fields
- missing procedure fields
- Function without returnValue
- Sub with returnValue
- invalid generationPolicy values

## Decision

P2-08 selects Candidate B as the next implementation candidate.

Implementation remains NO-GO until a later task explicitly authorizes implementation.

## Out of Scope

P2-08 does not define:

- final Validator class name
- final parser API
- final diagnostic object type
- implementation file paths
- production code changes
- test code changes
- Manifest format
- Generator behavior

## Verification

Docs-only task.

Expected checks:

- Markdown added
- No VBA implementation changed
- No parser behavior changed
- No validator behavior changed
- No generator behavior changed
- No test execution required
