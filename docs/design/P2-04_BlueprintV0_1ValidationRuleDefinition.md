# P2-04 — Blueprint v0.1 Validation Rule Definition

## Status

COMPLETE / docs-only validation specification

## Purpose

Define validation rules for Blueprint Specification v0.1.

This document specifies the rules that a future Blueprint Validator should apply before Manifest derivation and VBA generation.

## Scope

This task is docs-only.

It defines:

- structural validation rules
- required field validation
- enum value validation
- approval consistency validation
- procedure validation
- parameter validation
- dependency validation
- generation policy validation
- Manifest derivation eligibility

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Validation Result Categories

Blueprint validation should distinguish the following categories:

| Category | Meaning |
|---|---|
| Valid and generatable | Blueprint is valid and eligible for Manifest derivation |
| Valid but not generatable | Blueprint is structurally valid but not approved for generation |
| Invalid | Blueprint violates v0.1 validation rules |

## Rule Group 1 — Required Top-Level Fields

A Blueprint v0.1 document must contain:

- `blueprintId`
- `version`
- `status`
- `approval`
- `target`
- `modules`
- `generationPolicy`

Validation result:

- Missing required top-level field: Invalid

## Rule Group 2 — Blueprint Version

`version` must identify the Blueprint specification version.

For v0.1, expected value:

- `"0.1"`

Validation result:

- Missing version: Invalid
- Unsupported version: Invalid

## Rule Group 3 — Status

Allowed `status` values:

- `draft`
- `review`
- `approved`
- `rejected`
- `superseded`

Validation result:

- Missing status: Invalid
- Unknown status: Invalid

## Rule Group 4 — Approval

`approval` must contain:

- `state`

Allowed `approval.state` values:

- `notApproved`
- `approved`
- `rejected`

When `approval.state = approved`, the following fields are required:

- `approvedBy`
- `approvedAt`

Validation result:

- Missing `approval.state`: Invalid
- Unknown `approval.state`: Invalid
- `approval.state = approved` without `approvedBy`: Invalid
- `approval.state = approved` without `approvedAt`: Invalid

## Rule Group 5 — Status / Approval Consistency

`status` and `approval.state` must not conflict.

Required consistency rules:

| status | allowed approval.state |
|---|---|
| `draft` | `notApproved` |
| `review` | `notApproved` |
| `approved` | `approved` |
| `rejected` | `rejected` |
| `superseded` | `notApproved`, `approved`, `rejected` |

Validation result:

- Conflicting status and approval state: Invalid

## Rule Group 6 — Manifest Derivation Eligibility

Manifest derivation is allowed only when:

- `status = approved`
- `approval.state = approved`
- structural validation passes
- generation policy validation passes

Validation result:

- Approved and valid Blueprint: Valid and generatable
- Draft, review, rejected, or superseded Blueprint: Valid but not generatable, if structurally valid
- Invalid Blueprint: Invalid and not generatable

## Rule Group 7 — Target

`target` must contain:

- `kind`
- `name`

Allowed `target.kind` values:

- `workbook`
- `addin`

Validation result:

- Missing `target`: Invalid
- Missing `target.kind`: Invalid
- Unknown `target.kind`: Invalid
- Missing `target.name`: Invalid

## Rule Group 8 — Modules

`modules` must contain one or more module definitions.

Each module must contain:

- `name`
- `kind`
- `responsibility`

Allowed `modules[].kind` values:

- `standard`
- `class`
- `form`

Validation result:

- Missing `modules`: Invalid
- Empty `modules`: Invalid
- Missing module name: Invalid
- Missing module kind: Invalid
- Unknown module kind: Invalid
- Missing module responsibility: Invalid

## Rule Group 9 — Procedures

`procedures` is optional per module.

When present, each procedure must contain:

- `name`
- `kind`
- `visibility`
- `responsibility`

Allowed `procedures[].kind` values:

- `Sub`
- `Function`

Allowed `procedures[].visibility` values:

- `Public`
- `Private`

Validation result:

- Missing procedure name: Invalid
- Missing procedure kind: Invalid
- Unknown procedure kind: Invalid
- Missing procedure visibility: Invalid
- Unknown procedure visibility: Invalid
- Missing procedure responsibility: Invalid

## Rule Group 10 — Function Return Value

When `procedures[].kind = Function`, `returnValue` is required.

`returnValue` must contain:

- `type`

Validation result:

- Function without `returnValue`: Invalid
- Function with `returnValue` missing `type`: Invalid

## Rule Group 11 — Sub Return Value Prohibition

When `procedures[].kind = Sub`, `returnValue` must be omitted.

Validation result:

- Sub with `returnValue`: Invalid

## Rule Group 12 — Parameters

`parameters` is optional per procedure.

When present, each parameter must contain:

- `name`
- `type`

Allowed `parameters[].passing` values:

- `ByVal`
- `ByRef`

Rules:

- `passing` may be omitted
- `optional` may be omitted
- `defaultValue` is allowed only when `optional = true`

Validation result:

- Missing parameter name: Invalid
- Missing parameter type: Invalid
- Unknown passing value: Invalid
- `defaultValue` present when `optional` is not `true`: Invalid

## Rule Group 13 — Dependencies

`dependencies` is optional per procedure.

When present, each dependency must contain:

- `kind`
- `name`

Allowed `dependencies[].kind` values:

- `module`
- `procedure`
- `reference`
- `worksheet`
- `namedRange`

Validation result:

- Missing dependency kind: Invalid
- Unknown dependency kind: Invalid
- Missing dependency name: Invalid

## Rule Group 14 — Generation Policy

`generationPolicy` must contain:

- `allowOverwrite`
- `encoding`
- `missingDirectoryPolicy`

Required v0.1 values:

- `allowOverwrite = true`
- `encoding = utf-8`
- `missingDirectoryPolicy = error`

Allowed `lineEnding` values:

- `platformDefault`
- `crlf`
- `lf`

Validation result:

- Missing `generationPolicy`: Invalid
- Missing `allowOverwrite`: Invalid
- `allowOverwrite` other than `true`: Invalid
- Missing `encoding`: Invalid
- `encoding` other than `utf-8`: Invalid
- Missing `missingDirectoryPolicy`: Invalid
- `missingDirectoryPolicy` other than `error`: Invalid
- Unknown `lineEnding`: Invalid

## Rule Group 15 — Manifest Boundary

Validation must ensure that Manifest derivation does not require independent design decisions outside Blueprint.

Manifest derivation must not introduce:

- new modules
- new procedures
- new responsibilities
- new dependencies
- new approval state
- independent design intent

Validation result:

- Blueprint requiring Manifest-side design decisions: Invalid for Manifest derivation

## Example-Based Expectations

The examples from P2-03 should classify as follows:

| Example | Expected Validation Result |
|---|---|
| Minimal approved Blueprint | Valid and generatable |
| Function with returnValue | Valid and generatable |
| Multiple modules | Valid and generatable |
| Draft Blueprint | Valid but not generatable |
| Rejected Blueprint | Valid but not generatable |
| Function without returnValue | Invalid |
| Approved status without approved approval state | Invalid |
| Sub with returnValue | Invalid |

## Out of Scope

P2-04 does not define:

- validation error codes
- parser implementation
- validator implementation
- Manifest generator implementation
- Template behavior
- GenerateContext behavior
- Generator behavior
- AI draft generation behavior
- UI review behavior

## Verification

Docs-only task.

Expected checks:

- Markdown added
- No VBA implementation changed
- No parser behavior changed
- No validator behavior changed
- No generator behavior changed
- No test execution required
