# P2-05 — Blueprint v0.1 Error Classification Definition

## Status

COMPLETE / docs-only error classification specification

## Purpose

Define error classification categories for Blueprint Specification v0.1 validation.

This document maps the P2-04 validation rules to stable error categories that a future Blueprint Validator may use.

## Scope

This task is docs-only.

It defines:

- validation error categories
- meanings of each category
- representative detection conditions
- relationship to Manifest derivation eligibility
- future error-code readiness

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Error Classification Principles

Blueprint v0.1 error classification follows these principles:

- classify validation failures by cause
- keep categories stable enough for future error codes
- avoid implementation-specific details
- distinguish invalid Blueprints from valid but non-generatable Blueprints
- preserve Blueprint as the Single Source of Truth
- prevent Manifest-side design decisions

## Validation Outcome Relationship

| Validation outcome | Meaning | Manifest derivation |
|---|---|---|
| Valid and generatable | Blueprint is structurally valid and approved | Allowed |
| Valid but not generatable | Blueprint is structurally valid but not approved for generation | Not allowed |
| Invalid | Blueprint violates validation rules | Not allowed |

## Error Categories

| Category | Meaning |
|---|---|
| `MissingRequiredField` | A required field is absent |
| `UnsupportedEnumValue` | A field contains a value outside the allowed set |
| `ApprovalConflict` | `status` and `approval.state` conflict |
| `GenerationIneligible` | Blueprint is valid but not eligible for Manifest derivation |
| `InvalidTarget` | `target` is missing or malformed |
| `InvalidModule` | module definition is missing or malformed |
| `InvalidProcedure` | procedure definition is missing or malformed |
| `InvalidReturnValue` | Function/Sub return value rules are violated |
| `InvalidParameter` | parameter definition is missing or malformed |
| `InvalidDependency` | dependency definition is missing or malformed |
| `InvalidGenerationPolicy` | generation policy is missing or violates required policy |
| `ManifestBoundaryViolation` | Manifest derivation would require independent design decisions |

## `MissingRequiredField`

Meaning:

A field required by Blueprint v0.1 is missing.

Representative conditions:

- missing `blueprintId`
- missing `version`
- missing `status`
- missing `approval`
- missing `target`
- missing `modules`
- missing `generationPolicy`
- missing required child fields such as module `name` or procedure `kind`

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `UnsupportedEnumValue`

Meaning:

A field contains a value outside the allowed set.

Representative conditions:

- unknown `status`
- unknown `approval.state`
- unknown `target.kind`
- unknown `modules[].kind`
- unknown `procedures[].kind`
- unknown `procedures[].visibility`
- unknown `parameters[].passing`
- unknown `dependencies[].kind`
- unknown `generationPolicy.lineEnding`

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `ApprovalConflict`

Meaning:

Blueprint approval fields contradict each other.

Representative conditions:

- `status = approved` with `approval.state = notApproved`
- `status = approved` with `approval.state = rejected`
- `status = rejected` with `approval.state = approved`
- `status = draft` with `approval.state = approved`
- `approval.state = approved` without `approvedBy`
- `approval.state = approved` without `approvedAt`

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `GenerationIneligible`

Meaning:

The Blueprint is structurally valid but must not be used for Manifest derivation.

Representative conditions:

- `status = draft`
- `status = review`
- `status = rejected`
- `status = superseded`

Validation outcome:

- Valid but not generatable

Manifest derivation:

- Not allowed

## `InvalidTarget`

Meaning:

The `target` section is missing, malformed, or incomplete.

Representative conditions:

- missing `target.kind`
- missing `target.name`
- unsupported `target.kind`
- empty target name

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `InvalidModule`

Meaning:

A module definition is missing, malformed, or incomplete.

Representative conditions:

- `modules` is empty
- module missing `name`
- module missing `kind`
- module missing `responsibility`
- unsupported module kind
- empty module name

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `InvalidProcedure`

Meaning:

A procedure definition is missing, malformed, or incomplete.

Representative conditions:

- procedure missing `name`
- procedure missing `kind`
- procedure missing `visibility`
- procedure missing `responsibility`
- unsupported procedure kind
- unsupported visibility
- empty procedure name

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `InvalidReturnValue`

Meaning:

A procedure violates Function/Sub return value rules.

Representative conditions:

- `Function` without `returnValue`
- `Function` with `returnValue` missing `type`
- `Sub` with `returnValue`

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `InvalidParameter`

Meaning:

A parameter definition is missing, malformed, or inconsistent.

Representative conditions:

- parameter missing `name`
- parameter missing `type`
- unsupported `passing`
- `defaultValue` present when `optional` is not `true`
- empty parameter name

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `InvalidDependency`

Meaning:

A dependency definition is missing or malformed.

Representative conditions:

- dependency missing `kind`
- dependency missing `name`
- unsupported dependency kind
- empty dependency name

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `InvalidGenerationPolicy`

Meaning:

The generation policy is missing or violates required v0.1 policy.

Representative conditions:

- missing `generationPolicy`
- missing `allowOverwrite`
- `allowOverwrite` other than `true`
- missing `encoding`
- `encoding` other than `utf-8`
- missing `missingDirectoryPolicy`
- `missingDirectoryPolicy` other than `error`
- unsupported `lineEnding`

Validation outcome:

- Invalid

Manifest derivation:

- Not allowed

## `ManifestBoundaryViolation`

Meaning:

Manifest derivation would require information or design decisions not present in the approved Blueprint.

Representative conditions:

- Manifest would need to add a module not present in Blueprint
- Manifest would need to add a procedure not present in Blueprint
- Manifest would need to invent a responsibility
- Manifest would need to infer an undeclared dependency
- Manifest would need to change approval state
- Manifest would need to resolve design ambiguity independently

Validation outcome:

- Invalid for Manifest derivation

Manifest derivation:

- Not allowed

## Category to Rule Mapping

| P2-04 Rule Group | Primary Error Category |
|---|---|
| Required Top-Level Fields | `MissingRequiredField` |
| Blueprint Version | `MissingRequiredField`, `UnsupportedEnumValue` |
| Status | `MissingRequiredField`, `UnsupportedEnumValue` |
| Approval | `MissingRequiredField`, `UnsupportedEnumValue`, `ApprovalConflict` |
| Status / Approval Consistency | `ApprovalConflict` |
| Manifest Derivation Eligibility | `GenerationIneligible` |
| Target | `InvalidTarget` |
| Modules | `InvalidModule` |
| Procedures | `InvalidProcedure` |
| Function Return Value | `InvalidReturnValue` |
| Sub Return Value Prohibition | `InvalidReturnValue` |
| Parameters | `InvalidParameter` |
| Dependencies | `InvalidDependency` |
| Generation Policy | `InvalidGenerationPolicy` |
| Manifest Boundary | `ManifestBoundaryViolation` |

## Example-Based Classification

| P2-03 Example | Expected Category |
|---|---|
| Minimal approved Blueprint | none |
| Function with returnValue | none |
| Multiple modules | none |
| Draft Blueprint | `GenerationIneligible` |
| Rejected Blueprint | `GenerationIneligible` |
| Function without returnValue | `InvalidReturnValue` |
| Approved status without approved approval state | `ApprovalConflict` |
| Sub with returnValue | `InvalidReturnValue` |

## Future Error Code Guidance

P2-05 defines categories only.

A later task may define stable error codes using these categories.

Example future shape:

```text
BP001 MissingRequiredField
BP002 UnsupportedEnumValue
BP003 ApprovalConflict
```

This task does not reserve final error code names or numbers.

## Out of Scope

P2-05 does not define:

- final error code numbers
- diagnostic message text
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
