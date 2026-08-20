# P2-06 — Blueprint v0.1 Validation Error Code Definition

## Status

COMPLETE / docs-only error code specification

## Purpose

Define stable validation error codes for Blueprint Specification v0.1.

This document maps P2-05 error classification categories to concrete error codes that a future Blueprint Validator may return.

## Scope

This task is docs-only.

It defines:

- error code naming rules
- error code ranges
- code-to-category mapping
- severity levels
- Manifest derivation eligibility impact
- example-based expected codes

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Error Code Principles

Blueprint v0.1 error codes follow these principles:

- use stable identifiers
- map to P2-05 error categories
- avoid implementation-specific wording
- support future diagnostics
- distinguish invalid Blueprints from valid but non-generatable Blueprints
- protect the Blueprint / Manifest boundary

## Error Code Format

Error codes use the following format:

```text
BP###
```

Where:

- `BP` means Blueprint
- `###` is a three-digit numeric code

## Error Code Ranges

| Range | Area |
|---|---|
| `BP001`-`BP099` | structural / required fields |
| `BP100`-`BP199` | enum / value validation |
| `BP200`-`BP299` | approval / generation eligibility |
| `BP300`-`BP399` | target / module validation |
| `BP400`-`BP499` | procedure / returnValue / parameter validation |
| `BP500`-`BP599` | dependency validation |
| `BP600`-`BP699` | generation policy validation |
| `BP700`-`BP799` | Manifest boundary validation |

## Severity Levels

| Severity | Meaning |
|---|---|
| `error` | Blueprint is invalid |
| `warning` | Blueprint is valid but has non-blocking concern |
| `info` | Informational diagnostic |

Blueprint v0.1 defines only blocking `error` codes and one non-generatable `info` code family.

## Manifest Derivation Impact

| Validation result | Manifest derivation |
|---|---|
| no error and approved | Allowed |
| non-generatable status | Not allowed |
| any error code | Not allowed |

## Structural / Required Field Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP001` | `MissingRequiredField` | error | Missing `blueprintId` |
| `BP002` | `MissingRequiredField` | error | Missing `version` |
| `BP003` | `MissingRequiredField` | error | Missing `status` |
| `BP004` | `MissingRequiredField` | error | Missing `approval` |
| `BP005` | `MissingRequiredField` | error | Missing `target` |
| `BP006` | `MissingRequiredField` | error | Missing `modules` |
| `BP007` | `MissingRequiredField` | error | Missing `generationPolicy` |
| `BP008` | `MissingRequiredField` | error | Missing required child field |

## Enum / Value Validation Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP101` | `UnsupportedEnumValue` | error | Unsupported `version` |
| `BP102` | `UnsupportedEnumValue` | error | Unsupported `status` |
| `BP103` | `UnsupportedEnumValue` | error | Unsupported `approval.state` |
| `BP104` | `UnsupportedEnumValue` | error | Unsupported `target.kind` |
| `BP105` | `UnsupportedEnumValue` | error | Unsupported `modules[].kind` |
| `BP106` | `UnsupportedEnumValue` | error | Unsupported `procedures[].kind` |
| `BP107` | `UnsupportedEnumValue` | error | Unsupported `procedures[].visibility` |
| `BP108` | `UnsupportedEnumValue` | error | Unsupported `parameters[].passing` |
| `BP109` | `UnsupportedEnumValue` | error | Unsupported `dependencies[].kind` |
| `BP110` | `UnsupportedEnumValue` | error | Unsupported `generationPolicy.lineEnding` |

## Approval / Generation Eligibility Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP201` | `ApprovalConflict` | error | `status` and `approval.state` conflict |
| `BP202` | `ApprovalConflict` | error | `approval.state = approved` without `approvedBy` |
| `BP203` | `ApprovalConflict` | error | `approval.state = approved` without `approvedAt` |
| `BP204` | `GenerationIneligible` | info | Blueprint is structurally valid but not approved for generation |

## Target / Module Validation Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP301` | `InvalidTarget` | error | Missing `target.kind` |
| `BP302` | `InvalidTarget` | error | Missing `target.name` |
| `BP303` | `InvalidTarget` | error | Empty `target.name` |
| `BP321` | `InvalidModule` | error | `modules` is empty |
| `BP322` | `InvalidModule` | error | Missing module `name` |
| `BP323` | `InvalidModule` | error | Missing module `kind` |
| `BP324` | `InvalidModule` | error | Missing module `responsibility` |
| `BP325` | `InvalidModule` | error | Empty module `name` |

## Procedure / Return Value / Parameter Validation Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP401` | `InvalidProcedure` | error | Missing procedure `name` |
| `BP402` | `InvalidProcedure` | error | Missing procedure `kind` |
| `BP403` | `InvalidProcedure` | error | Missing procedure `visibility` |
| `BP404` | `InvalidProcedure` | error | Missing procedure `responsibility` |
| `BP405` | `InvalidProcedure` | error | Empty procedure `name` |
| `BP421` | `InvalidReturnValue` | error | `Function` without `returnValue` |
| `BP422` | `InvalidReturnValue` | error | `Function` returnValue missing `type` |
| `BP423` | `InvalidReturnValue` | error | `Sub` with `returnValue` |
| `BP441` | `InvalidParameter` | error | Missing parameter `name` |
| `BP442` | `InvalidParameter` | error | Missing parameter `type` |
| `BP443` | `InvalidParameter` | error | Empty parameter `name` |
| `BP444` | `InvalidParameter` | error | `defaultValue` present when `optional` is not `true` |

## Dependency Validation Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP501` | `InvalidDependency` | error | Missing dependency `kind` |
| `BP502` | `InvalidDependency` | error | Missing dependency `name` |
| `BP503` | `InvalidDependency` | error | Empty dependency `name` |

## Generation Policy Validation Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP601` | `InvalidGenerationPolicy` | error | Missing `generationPolicy.allowOverwrite` |
| `BP602` | `InvalidGenerationPolicy` | error | `allowOverwrite` is not `true` |
| `BP603` | `InvalidGenerationPolicy` | error | Missing `generationPolicy.encoding` |
| `BP604` | `InvalidGenerationPolicy` | error | `encoding` is not `utf-8` |
| `BP605` | `InvalidGenerationPolicy` | error | Missing `generationPolicy.missingDirectoryPolicy` |
| `BP606` | `InvalidGenerationPolicy` | error | `missingDirectoryPolicy` is not `error` |

## Manifest Boundary Validation Codes

| Code | Category | Severity | Meaning |
|---|---|---:|---|
| `BP701` | `ManifestBoundaryViolation` | error | Manifest derivation would add a module |
| `BP702` | `ManifestBoundaryViolation` | error | Manifest derivation would add a procedure |
| `BP703` | `ManifestBoundaryViolation` | error | Manifest derivation would invent a responsibility |
| `BP704` | `ManifestBoundaryViolation` | error | Manifest derivation would infer an undeclared dependency |
| `BP705` | `ManifestBoundaryViolation` | error | Manifest derivation would change approval state |
| `BP706` | `ManifestBoundaryViolation` | error | Manifest derivation requires independent design intent |

## Example-Based Expected Codes

| P2-03 Example | Expected Code |
|---|---|
| Minimal approved Blueprint | none |
| Function with returnValue | none |
| Multiple modules | none |
| Draft Blueprint | `BP204` |
| Rejected Blueprint | `BP204` |
| Function without returnValue | `BP421` |
| Approved status without approved approval state | `BP201` |
| Sub with returnValue | `BP423` |

## Diagnostic Shape Guidance

A later validator may return diagnostics using this conceptual shape:

```yaml
code: BP421
category: InvalidReturnValue
severity: error
field: modules[0].procedures[0].returnValue
message: "Function procedures require returnValue."
```

This task does not implement the diagnostic format.

## Stability Rule

Once implemented, error codes should remain stable within Blueprint Specification v0.1.

Future changes should prefer:

- adding new codes
- deprecating old codes explicitly
- avoiding silent meaning changes

## Out of Scope

P2-06 does not define:

- parser implementation
- validator implementation
- final diagnostic message wording
- localization
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
