# P2-10 — Blueprint Validator Entry Point and Model Design

## Status

COMPLETE / docs-only implementation design

## Purpose

Define the concrete entry point and model design for the future Blueprint Validator Candidate B implementation.

This document prepares P2-11 implementation by fixing the Validator-facing model, result shape, diagnostic shape, constants, and test placement assumptions.

## Scope

This task is docs-only.

It defines:

- Validator entry point design
- input model design
- output model design
- diagnostic model design
- result enum design
- error code constants design
- Candidate B validation boundary
- generatable judgment design
- focused test placement
- Parser / Manifest / Generator no-change boundary

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Baseline

P2-10 assumes the following completed documents:

- P2-01 — Blueprint Specification v0.1 Scope Definition
- P2-02 — Blueprint Specification v0.1 Field Model Definition
- P2-03 — Blueprint Specification v0.1 Example Documents
- P2-04 — Blueprint v0.1 Validation Rule Definition
- P2-05 — Blueprint v0.1 Error Classification Definition
- P2-06 — Blueprint v0.1 Validation Error Code Definition
- P2-07 — Blueprint v0.1 Validator Implementation Scope Planning
- P2-08 — Blueprint Validator Minimal Implementation Candidate Selection
- P2-09 — Blueprint Validator Candidate B Implementation Scope Definition

## Selected Implementation Candidate

P2-10 designs the entry point and model for:

**Candidate B — Minimal Generatable Validation**

## Validator Position

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

## Entry Point Design

The future implementation should expose a single Validator entry point.

Conceptual name:

```text
BlueprintValidator.Validate
```

Conceptual signature:

```text
Validate(parsedBlueprint) -> BlueprintValidationResult
```

Rules:

- input is a parsed Blueprint model
- output is a validation result object
- raw YAML parsing is outside Validator scope
- file I/O is outside Validator scope
- Manifest generation is outside Validator scope
- Generator behavior must not change

## Input Model Design

The Validator input should represent Blueprint v0.1 fields.

Conceptual model:

```text
BlueprintModel
  blueprintId
  version
  status
  approval
  target
  modules
  generationPolicy
```

## Approval Model

Conceptual model:

```text
BlueprintApproval
  state
  approvedBy
  approvedAt
  note
```

Required for Candidate B:

- `state`

Conditionally required:

- `approvedBy` when `state = approved`
- `approvedAt` when `state = approved`

## Target Model

Conceptual model:

```text
BlueprintTarget
  kind
  name
  description
```

Required for Candidate B:

- `kind`
- `name`

## Module Model

Conceptual model:

```text
BlueprintModule
  name
  kind
  responsibility
  procedures
```

Required for Candidate B:

- `name`
- `kind`
- `responsibility`

## Procedure Model

Conceptual model:

```text
BlueprintProcedure
  name
  kind
  visibility
  responsibility
  parameters
  returnValue
  dependencies
```

Required for Candidate B:

- `name`
- `kind`
- `visibility`
- `responsibility`

Conditional for Candidate B:

- `returnValue` is required when `kind = Function`
- `returnValue` must be omitted when `kind = Sub`

## Return Value Model

Conceptual model:

```text
BlueprintReturnValue
  type
  description
```

Required for Candidate B:

- `type`

## Generation Policy Model

Conceptual model:

```text
BlueprintGenerationPolicy
  allowOverwrite
  encoding
  lineEnding
  missingDirectoryPolicy
```

Required for Candidate B:

- `allowOverwrite`
- `encoding`
- `missingDirectoryPolicy`

Required values:

- `allowOverwrite = true`
- `encoding = utf-8`
- `missingDirectoryPolicy = error`

## Deferred Input Fields

The following fields may exist in parsed models but are not validated in Candidate B except by omission tolerance:

- parameters
- dependencies
- Manifest boundary details
- localization metadata
- UI review metadata
- AI draft metadata

## Output Model Design

The Validator output should contain:

```text
BlueprintValidationResult
  result
  generatable
  diagnostics
```

## Result Enum Design

Conceptual enum:

```text
BlueprintValidationResultKind
  validGeneratable
  validNotGeneratable
  invalid
```

Rules:

- `validGeneratable` means structurally valid and approved
- `validNotGeneratable` means structurally valid but not approved
- `invalid` means one or more validation errors exist

## Generatable Flag Design

`generatable` is a boolean.

Rules:

- `true` only when result is `validGeneratable`
- `false` when result is `validNotGeneratable`
- `false` when result is `invalid`
- `false` when any error diagnostic exists
- `false` when `status` is not `approved`
- `false` when `approval.state` is not `approved`

## Diagnostic Model Design

Conceptual model:

```text
BlueprintValidationDiagnostic
  code
  category
  severity
  field
  message
```

Required diagnostic fields:

- `code`
- `category`
- `severity`

Optional diagnostic fields:

- `field`
- `message`

Rules:

- `code` must use P2-06 codes
- `category` must use P2-05 categories
- `severity` should be `error` or `info`
- `field` should identify the affected field when practical
- `message` is not stable API in Candidate B

## Error Code Constants Design

The future implementation may define constants for Candidate B codes.

Candidate B code coverage:

```text
BP001-BP008
BP101-BP107
BP201-BP204
BP301-BP325
BP401-BP405
BP421-BP423
BP601-BP606
```

## Candidate B Validation Order

Recommended validation order:

1. top-level required fields
2. version
3. status
4. approval
5. status / approval consistency
6. target
7. modules
8. module fields
9. procedure fields
10. Function / returnValue rules
11. Sub / returnValue rules
12. generationPolicy
13. result and generatable calculation

The implementation may collect multiple diagnostics in one pass where practical.

## Result Calculation Rule

If any `error` diagnostic exists:

```text
result = invalid
generatable = false
```

If no `error` diagnostic exists and Blueprint is approved:

```text
result = validGeneratable
generatable = true
```

If no `error` diagnostic exists and Blueprint is not approved:

```text
result = validNotGeneratable
generatable = false
diagnostics includes BP204
```

## `BP204` Design

`BP204` is an `info` diagnostic.

Meaning:

```text
Blueprint is structurally valid but not approved for generation.
```

Rules:

- does not make result `invalid`
- sets result to `validNotGeneratable`
- sets `generatable = false`
- prevents Manifest derivation

## Parser No-Change Boundary

P2-11 should not rewrite the existing parser.

Allowed:

- consume the existing parsed Blueprint model if available
- add adapter code only if needed and explicitly scoped

Not allowed:

- raw YAML parser rewrite
- YAML syntax behavior changes
- parser error behavior changes unless separately approved

## Manifest No-Change Boundary

P2-11 must not generate Manifest files.

Not allowed:

- Manifest file writing
- Manifest normalization changes
- Manifest derivation behavior changes
- adding Manifest-side design decisions

## Generator No-Change Boundary

P2-11 must not change VBA Generator behavior.

Not allowed:

- changing generated VBA output
- changing Template behavior
- changing GenerateContext behavior
- changing output encoding behavior
- changing overwrite behavior

## Suggested File Placement

Final file paths are not fixed by this document.

Preferred future placement should follow existing repository structure.

Conceptual production areas:

```text
src/.../BlueprintValidator
src/.../BlueprintValidationResult
src/.../BlueprintValidationDiagnostic
src/.../BlueprintValidationErrorCodes
```

Conceptual test area:

```text
tests/.../BlueprintValidatorTests
```

P2-11 should inspect the actual repository structure before choosing final paths.

## Focused Test Design

P2-11 or P2-12 should include tests for:

| Test | Expected Result | Expected Code |
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
| empty target name | `invalid` | `BP303` |
| empty modules | `invalid` | `BP321` |
| missing module name | `invalid` | `BP322` |
| missing procedure kind | `invalid` | `BP402` |
| Function without returnValue | `invalid` | `BP421` |
| Function returnValue missing type | `invalid` | `BP422` |
| Sub with returnValue | `invalid` | `BP423` |
| invalid allowOverwrite | `invalid` | `BP602` |
| invalid encoding | `invalid` | `BP604` |
| invalid missingDirectoryPolicy | `invalid` | `BP606` |

## P2-11 Implementation Handoff

P2-11 may implement:

- Validator entry point
- validation result model
- diagnostic model
- Candidate B validation logic
- Candidate B error code constants
- focused tests

P2-11 must not implement:

- full Candidate C validation
- parameter validation
- dependency validation
- Manifest boundary validation
- Manifest generation
- Generator changes
- Parser rewrite
- Excel runtime execution

## P2-12 Verification Handoff

P2-12 should verify:

- focused Validator tests pass
- Candidate B expected results match P2-09 / P2-10
- existing Build VBA regression passes if available
- Parser behavior is unchanged
- Generator behavior is unchanged
- docs sync is complete

## Out of Scope

P2-10 does not define:

- final implementation file paths
- final class names
- final language-specific API signatures
- final parser API changes
- Manifest format
- Generator behavior
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
