# P2-02 — Blueprint Specification v0.1 Field Model Definition

## Status

COMPLETE / docs-only schema design

## Purpose

Define the Blueprint Specification v0.1 field model.

This document fixes the minimum field structure required for Blueprint documents so later Blueprint validation, Manifest derivation, Template handling, GenerateContext creation, and VBA generation can share the same assumptions.

## Scope

This task is docs-only.

It defines:

- Blueprint-level fields
- target fields
- module fields
- procedure fields
- parameter fields
- return value fields
- dependency fields
- generation policy fields
- Manifest derivation boundary

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Field Requirement Levels

Blueprint v0.1 uses the following requirement levels:

| Level | Meaning |
|---|---|
| Required | Must be present |
| Optional | May be omitted |
| Conditional | Required only when a stated condition is met |
| Derived | Not maintained directly; derived from another field |

## Top-Level Fields

| Field | Level | Type | Meaning |
|---|---|---|---|
| `blueprintId` | Required | string | Stable Blueprint identity |
| `version` | Required | string | Blueprint specification version |
| `status` | Required | string | Lifecycle state of the Blueprint |
| `approval` | Required | object | Human approval information |
| `target` | Required | object | Workbook or add-in generation target |
| `modules` | Required | array | Module definitions |
| `generationPolicy` | Required | object | Generation constraints and policies |

## `status`

Allowed values:

- `draft`
- `review`
- `approved`
- `rejected`
- `superseded`

Only `approved` Blueprint documents may be used for Manifest generation.

## `approval`

| Field | Level | Type | Meaning |
|---|---|---|---|
| `state` | Required | string | Approval state |
| `approvedBy` | Conditional | string | Human approver |
| `approvedAt` | Conditional | string | Approval timestamp |
| `note` | Optional | string | Approval note |

Allowed `state` values:

- `notApproved`
- `approved`
- `rejected`

Rules:

- `approvedBy` is required when `state` is `approved`
- `approvedAt` is required when `state` is `approved`
- `status` and `approval.state` must not conflict
- Manifest generation requires `status = approved` and `approval.state = approved`

## `target`

| Field | Level | Type | Meaning |
|---|---|---|---|
| `kind` | Required | string | Target kind |
| `name` | Required | string | Target name |
| `description` | Optional | string | Target description |

Allowed `kind` values:

- `workbook`
- `addin`

## `modules`

`modules` must contain one or more module definitions.

| Field | Level | Type | Meaning |
|---|---|---|---|
| `name` | Required | string | VBA module name |
| `kind` | Required | string | VBA module kind |
| `responsibility` | Required | string | Module responsibility |
| `procedures` | Optional | array | Procedure definitions |

Allowed `kind` values:

- `standard`
- `class`
- `form`

## `procedures`

`procedures` may contain zero or more procedure definitions per module.

| Field | Level | Type | Meaning |
|---|---|---|---|
| `name` | Required | string | Procedure name |
| `kind` | Required | string | Procedure kind |
| `visibility` | Required | string | VBA visibility |
| `responsibility` | Required | string | Procedure responsibility |
| `parameters` | Optional | array | Parameter definitions |
| `returnValue` | Conditional | object | Function return value |
| `dependencies` | Optional | array | Procedure dependencies |

Allowed `kind` values:

- `Sub`
- `Function`

Allowed `visibility` values:

- `Public`
- `Private`

Rules:

- `returnValue` is required when `kind = Function`
- `returnValue` must be omitted when `kind = Sub`

## `parameters`

| Field | Level | Type | Meaning |
|---|---|---|---|
| `name` | Required | string | Parameter name |
| `type` | Required | string | VBA type name |
| `passing` | Optional | string | Passing convention |
| `optional` | Optional | boolean | Whether parameter is optional |
| `defaultValue` | Conditional | string | Default value |

Allowed `passing` values:

- `ByVal`
- `ByRef`

Rules:

- If `passing` is omitted, the downstream implementation may apply the VMF default policy.
- `defaultValue` is allowed only when `optional = true`.

## `returnValue`

| Field | Level | Type | Meaning |
|---|---|---|---|
| `type` | Required | string | VBA return type |
| `description` | Optional | string | Return value description |

## `dependencies`

| Field | Level | Type | Meaning |
|---|---|---|---|
| `kind` | Required | string | Dependency kind |
| `name` | Required | string | Dependency name |
| `reason` | Optional | string | Reason for dependency |

Allowed `kind` values:

- `module`
- `procedure`
- `reference`
- `worksheet`
- `namedRange`

## `generationPolicy`

| Field | Level | Type | Meaning |
|---|---|---|---|
| `allowOverwrite` | Required | boolean | Whether existing generated files may be overwritten |
| `encoding` | Required | string | Output text encoding |
| `lineEnding` | Optional | string | Output line ending policy |
| `missingDirectoryPolicy` | Required | string | Behavior when output directory is missing |

Required v0.1 policy values:

- `encoding = utf-8`
- `allowOverwrite = true`
- `missingDirectoryPolicy = error`

Allowed `lineEnding` values:

- `platformDefault`
- `crlf`
- `lf`

## Manifest Derivation Boundary

Manifest may derive:

- Blueprint identity
- Blueprint version
- target kind and name
- module list
- procedure list
- parameter list
- return value information
- dependency list
- generation policy

Manifest must not introduce:

- new modules
- new procedures
- new responsibilities
- new dependencies
- new approval state
- independent design intent

Blueprint remains the Single Source of Truth.

## Validation Rules for v0.1

A Blueprint v0.1 document is valid only when:

- required fields are present
- enum values are valid
- `modules` contains at least one module
- each module has a valid name and kind
- each procedure has a valid name, kind, visibility, and responsibility
- Function procedures define `returnValue`
- Sub procedures do not define `returnValue`
- approved generation requires both `status = approved` and `approval.state = approved`
- Manifest derivation does not add independent design information

## Out of Scope

P2-02 does not define:

- YAML syntax details
- parser implementation
- validator implementation
- Manifest file format
- Template file format
- GenerateContext structure
- Generator implementation
- AI Blueprint draft generation
- UI review workflow

## Verification

Docs-only task.

Expected checks:

- Markdown added
- No VBA implementation changed
- No parser behavior changed
- No generator behavior changed
- No test execution required
