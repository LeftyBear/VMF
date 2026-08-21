# P3-01 — Validator Integration Planning

## Status

PLANNING / docs-only

## Purpose

Document the design boundary for integrating the Blueprint Validator into the
formal Blueprint to Generator flow.

P3-01 is a planning record only. It does not authorize implementation, tests,
VBA production changes, Manifest changes, Generator changes, or behavior
changes.

## Formal Validator Position

The Validator belongs after Blueprint parsing and before Manifest generation.

```text
Blueprint Parse
↓
Blueprint Validation
↓ PASS only
Manifest generation
↓
Template
↓
GenerateContext
↓
Generator
```

Only a validation PASS may proceed to Manifest generation.

## Parser Responsibility

The Parser converts Blueprint syntax and format into a Blueprint model.

The Parser is responsible for:

- reading Blueprint document structure
- parsing supported syntax and formatting
- constructing the parsed Blueprint model
- reporting parser errors when the input cannot be parsed into the model

The Parser is not responsible for semantic validity judgment.

The Parser must not decide whether a parsed Blueprint is approved,
generatable, semantically consistent, or safe to derive into a Manifest.

## Validator Responsibility

The Validator verifies the semantic validity of an already parsed Blueprint.

The Validator is responsible for:

- validating required Blueprint fields
- validating supported enum values
- validating approval and generation eligibility rules
- validating target, module, procedure, return value, and generation policy
  rules within the approved Validator scope
- returning validation diagnostics and validation result state

The Validator does not modify the Blueprint.

The Validator must not:

- repair invalid Blueprint content
- fill missing Blueprint fields
- normalize Blueprint values
- rewrite parser output
- generate Manifest content
- invoke Template, GenerateContext, or Generator behavior

## Validation PASS

Validation PASS means the parsed Blueprint is semantically valid and allowed to
advance to Manifest generation.

Future integration implementation may connect Manifest generation only after a
Validator result that explicitly allows generation.

## Validation FAIL

Validation FAIL is a hard stop.

When validation fails:

- Manifest generation must not run
- Template must not run
- GenerateContext must not run
- Generator must not run
- no partial generation should be treated as success

The failure must remain visible as a validation failure instead of being
converted into parser, Manifest, Template, GenerateContext, or Generator
behavior.

## Failure Categories

Parser errors and Validation errors are separate failure categories.

Parser error:

- occurs before a parsed Blueprint model exists
- indicates that syntax, format, structure, or parser-supported input rules
  prevented model construction
- stops before Validator execution

Validation error:

- occurs after Blueprint Parse succeeds
- indicates that the parsed Blueprint model is semantically invalid or not
  allowed to proceed
- stops before Manifest generation

Future integration implementation must preserve this distinction in control
flow and reporting.

## Future Focused Regression

Future integration implementation must keep `AppRunBlueprintValidatorTests` as
the focused Validator regression.

The focused regression should continue to verify the Validator contract before
or alongside integration-level checks.

## Existing Build Regression

Existing Build regression is in scope only when existing Build tests are present
in the VMF repository.

P3-01 does not require running Build tests. For later implementation work,
existing Build regression should be considered only if the repository already
contains applicable Build test coverage.

## P3-01 Out of Scope

P3-01 does not perform:

- Validator integration implementation
- production VBA code modification
- test code additions
- Manifest implementation changes
- Generator changes
- behavior changes
- Parser changes
- Template changes
- GenerateContext changes
- package or `dist` changes
- release operations
- external service operations

## Verification

Required for P3-01:

- `git diff --check`
- docs-only change confirmation

Not required for P3-01:

- tests/build
- focused Validator tests
- full Build regression
- release build

## Next Candidate

Next candidate:

P3-02 — Validator Integration Implementation

Expected scope:

Connect the Validator after Parser success and before Manifest generation as the
minimum integration slice.
