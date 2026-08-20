# P2-09 — Blueprint Validator Candidate B Implementation Scope Definition

## Status

COMPLETE / docs-only implementation scope definition

## Purpose

Define the implementation scope for Blueprint Validator Candidate B — Minimal Generatable Validation.

This document fixes the exact boundary for a later implementation task so the Validator can be implemented without changing Parser, Manifest, Template, GenerateContext, or Generator behavior.

## Scope

This task is docs-only.

It defines:

- Candidate B implementation scope
- Validator entry point expectations
- input model expectations
- output model expectations
- diagnostic expectations
- generatable judgment rules
- target error code coverage
- focused test cases
- allowed implementation scope
- prohibited implementation scope

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Baseline

P2-09 assumes the following completed documents:

- P2-01 — Blueprint Specification v0.1 Scope Definition
- P2-02 — Blueprint Specification v0.1 Field Model Definition
- P2-03 — Blueprint Specification v0.1 Example Documents
- P2-04 — Blueprint v0.1 Validation Rule Definition
- P2-05 — Blueprint v0.1 Error Classification Definition
- P2-06 — Blueprint v0.1 Validation Error Code Definition
- P2-07 — Blueprint v0.1 Validator Implementation Scope Planning
- P2-08 — Blueprint Validator Minimal Implementation Candidate Selection

## Selected Candidate

P2-09 fixes the scope for:

**Candidate B — Minimal Generatable Validation**

Candidate B is the first implementation candidate selected by P2-08.

## Implementation Goal

The future implementation should provide a minimal Blueprint Validator that can classify a parsed Blueprint model as:

- `validGeneratable`
- `validNotGeneratable`
- `invalid`

It should also return:

- `generatable`
- diagnostics using P2-06 error codes

## Validator Position

The Validator sits after parsing and before Manifest derivation.

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

## Validator Responsibility

The Candidate B Validator is responsible for:

- validating top-level required fields
- validating supported `version`
- validating supported `status`
- validating `approval.state`
- validating status / approval consistency
- validating Manifest derivation eligibility
- validating `target`
- validating `modules`
- validating module required fields
- validating procedure required fields
- validating procedure kind
- validating procedure visibility
- validating Function / returnValue rules
- validating Sub / returnValue prohibition
- validating required generationPolicy values
- returning diagnostics
- returning result category
- returning `generatable`

## Validator Non-Responsibility

The Candidate B Validator must not:

- parse raw YAML
- rewrite parsed Blueprint models
- auto-fix invalid Blueprint documents
- generate Manifest files
- generate Template files
- create GenerateContext
- generate VBA
- execute Excel
- infer missing design intent
- approve Blueprint documents
- change approval state
- add modules
- add procedures
- add parameters
- add dependencies

Blueprint remains the Single Source of Truth.

## Input Boundary

Input is a parsed Blueprint model.

Expected assumptions:

- raw YAML parsing has already completed
- YAML syntax errors are outside Candidate B Validator scope
- missing fields may be represented as absent or null
- the Validator does not read files directly
- the Validator does not depend on Excel runtime state

## Output Boundary

Output should include:

- validation result
- generatable flag
- diagnostics list

Conceptual output shape:

```yaml
result: invalid
generatable: false
diagnostics:
  - code: BP421
    category: InvalidReturnValue
    severity: error
    field: modules[0].procedures[0].returnValue
```

Final implementation type names are not fixed by this document.

## Result Rules

| Result | Condition | generatable |
|---|---|---:|
| `validGeneratable` | Structurally valid and approved | true |
| `validNotGeneratable` | Structurally valid but not approved | false |
| `invalid` | One or more validation errors | false |

## Generatable Rule

`generatable = true` only when all conditions are met:

- validation result is `validGeneratable`
- no `error` severity diagnostics exist
- `status = approved`
- `approval.state = approved`
- required Candidate B fields are valid
- generationPolicy is valid

`generatable = false` when any condition is met:

- validation result is `invalid`
- validation result is `validNotGeneratable`
- any `error` severity diagnostic exists
- `status` is not `approved`
- `approval.state` is not `approved`

## Candidate B Error Code Coverage

The future implementation should cover the following P2-06 code ranges:

```text
BP001-BP008
BP101-BP107
BP201-BP204
BP301-BP325
BP401-BP405
BP421-BP423
BP601-BP606
```

## Included Error Codes

### Structural / Required Field Codes

- `BP001` — Missing `blueprintId`
- `BP002` — Missing `version`
- `BP003` — Missing `status`
- `BP004` — Missing `approval`
- `BP005` — Missing `target`
- `BP006` — Missing `modules`
- `BP007` — Missing `generationPolicy`
- `BP008` — Missing required child field

### Enum / Value Codes

- `BP101` — Unsupported `version`
- `BP102` — Unsupported `status`
- `BP103` — Unsupported `approval.state`
- `BP104` — Unsupported `target.kind`
- `BP105` — Unsupported `modules[].kind`
- `BP106` — Unsupported `procedures[].kind`
- `BP107` — Unsupported `procedures[].visibility`

### Approval / Generation Eligibility Codes

- `BP201` — `status` and `approval.state` conflict
- `BP202` — `approval.state = approved` without `approvedBy`
- `BP203` — `approval.state = approved` without `approvedAt`
- `BP204` — Blueprint is structurally valid but not approved for generation

### Target / Module Codes

- `BP301` — Missing `target.kind`
- `BP302` — Missing `target.name`
- `BP303` — Empty `target.name`
- `BP321` — `modules` is empty
- `BP322` — Missing module `name`
- `BP323` — Missing module `kind`
- `BP324` — Missing module `responsibility`
- `BP325` — Empty module `name`

### Procedure / Return Value Codes

- `BP401` — Missing procedure `name`
- `BP402` — Missing procedure `kind`
- `BP403` — Missing procedure `visibility`
- `BP404` — Missing procedure `responsibility`
- `BP405` — Empty procedure `name`
- `BP421` — `Function` without `returnValue`
- `BP422` — `Function` returnValue missing `type`
- `BP423` — `Sub` with `returnValue`

### Generation Policy Codes

- `BP601` — Missing `generationPolicy.allowOverwrite`
- `BP602` — `allowOverwrite` is not `true`
- `BP603` — Missing `generationPolicy.encoding`
- `BP604` — `encoding` is not `utf-8`
- `BP605` — Missing `generationPolicy.missingDirectoryPolicy`
- `BP606` — `missingDirectoryPolicy` is not `error`

## Deferred Error Codes

The following areas are deferred from Candidate B:

- parameter validation codes
- dependency validation codes
- Manifest boundary validation codes
- localization-related diagnostics
- warning diagnostics
- non-blocking advisory diagnostics

## `BP204` Rule

`BP204` is used when the Blueprint is structurally valid but not eligible for Manifest derivation.

Examples:

- `status = draft`
- `status = review`
- `status = rejected`
- `status = superseded`

Rules:

- `BP204` does not make the Blueprint invalid
- `BP204` sets `generatable = false`
- `BP204` prevents Manifest derivation
- `BP204` should result in `validNotGeneratable`

## Focused Test Cases

A later implementation task should add focused tests for:

| Test Case | Expected Result | Expected Code |
|---|---|---|
| minimal approved Blueprint | `validGeneratable` | none |
| Function with returnValue | `validGeneratable` | none |
| multiple modules | `validGeneratable` | none |
| draft Blueprint | `validNotGeneratable` | `BP204` |
| rejected Blueprint | `validNotGeneratable` | `BP204` |
| missing `blueprintId` | `invalid` | `BP001` |
| missing `version` | `invalid` | `BP002` |
| unsupported `version` | `invalid` | `BP101` |
| unsupported `status` | `invalid` | `BP102` |
| approval conflict | `invalid` | `BP201` |
| approved without `approvedBy` | `invalid` | `BP202` |
| approved without `approvedAt` | `invalid` | `BP203` |
| missing target kind | `invalid` | `BP301` |
| empty modules | `invalid` | `BP321` |
| missing module name | `invalid` | `BP322` |
| missing procedure kind | `invalid` | `BP402` |
| Function without returnValue | `invalid` | `BP421` |
| Function returnValue missing type | `invalid` | `BP422` |
| Sub with returnValue | `invalid` | `BP423` |
| invalid allowOverwrite | `invalid` | `BP602` |
| invalid encoding | `invalid` | `BP604` |
| invalid missingDirectoryPolicy | `invalid` | `BP606` |

## Allowed Future Implementation Scope

A later implementation task may:

- add a Validator entry point
- add a validation result model
- add a diagnostic model
- add Candidate B validation logic
- add focused Validator tests
- use P2-06 error codes
- classify generatable status
- keep implementation independent from Generator behavior

## Prohibited Future Implementation Scope Without Separate Approval

A later implementation task must not:

- rewrite the existing YAML parser
- change Generator behavior
- generate Manifest files
- write VBA output
- execute Excel
- add UI review workflow
- add AI Blueprint generation
- add automatic Blueprint repair
- introduce external service calls
- modify release or packaging behavior

## P2-10 Handoff

P2-10 should define the concrete Validator entry point and model design before implementation.

P2-10 should fix:

- class or module naming
- input model shape
- output model shape
- diagnostic type shape
- test file placement
- no-change boundary for existing Parser and Generator behavior

## P2-11 Handoff

P2-11 may implement Candidate B only after P2-10 is complete.

P2-11 should avoid any broader validation beyond this document unless separately approved.

## P2-12 Handoff

P2-12 should verify Candidate B implementation with focused tests and existing regression checks.

P2-12 should confirm:

- focused Validator tests pass
- existing Build VBA regression passes if available
- Generator behavior is unchanged
- docs are synchronized

## Out of Scope

P2-09 does not define:

- final class names
- final file paths
- final test file names
- implementation details
- production code changes
- test code changes
- final diagnostic message text

## Verification

Docs-only task.

Expected checks:

- Markdown added
- No VBA implementation changed
- No parser behavior changed
- No validator behavior changed
- No generator behavior changed
- No test execution required
