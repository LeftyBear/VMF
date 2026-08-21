# P3-03 - Validator Failure Contract / Diagnostics Planning

## Status

PLANNING / docs-only

## Purpose

Fix the failure contract and diagnostics boundary for Validator integration
after P3-02.

P3-03 is a planning record only. It does not authorize production VBA code
changes, Validator implementation changes, new diagnostic APIs, logging
framework changes, tests, Manifest changes, Template changes, GenerateContext
changes, or Generator changes.

## Failure Categories

The generation flow must keep these failure categories separate:

| Category | Position | Meaning | Required Stop |
| --- | --- | --- | --- |
| parse failure | before Validator | Blueprint content cannot be parsed into the supported parser model | stop before Validator |
| validation failure | after parse, before Manifest generation | Parsed Blueprint is semantically invalid or not generatable | stop before Manifest / Template / GenerateContext / Generator |
| manifest generation failure | after validation PASS | Manifest derivation or Manifest output cannot be completed | stop as Manifest generation failure |

Validation failure must not be converted into parse failure.

Validation failure must not be converted into manifest generation failure.

Parse failure must not run Validator. Manifest generation failure must not be
used to hide a failed validation result.

## Validator Responsibility

The Validator is responsible for:

- judging the semantic validity of an already parsed Blueprint
- returning a validation result
- returning validation diagnostics through the existing result contract

The Validator must not:

- modify the Blueprint
- repair invalid Blueprint content
- fill missing Blueprint fields
- normalize Blueprint values for the caller
- generate Manifest content
- invoke Template, GenerateContext, or Generator behavior

## Caller Responsibility

The caller is responsible for:

- treating `Result.Generatable = False` as a hard stop
- not advancing to Manifest, Template, GenerateContext, or Generator when
  `Result.Generatable = False`
- preserving the failure meaning when returning the failure upward
- keeping validation failure distinct from parse failure
- keeping validation failure distinct from manifest generation failure

The caller may wrap the failure for its own return type only if the wrapper
preserves that the source category was validation failure.

## Diagnostics Boundary

Diagnostics must make validation failure identifiable.

Diagnostics must make validation failure distinguishable from:

- parser failure
- manifest generation failure

Diagnostics must not:

- rewrite Blueprint content
- expose unnecessary internal implementation details
- expose raw exceptions
- expose stack traces
- expose private runtime state that is not part of the public failure contract

Diagnostics may use the existing validation result and diagnostic fields:

- `ResultKind`
- `Generatable`
- `Diagnostics`
- diagnostic `Code`
- diagnostic `Category`
- diagnostic `Severity`
- diagnostic `FieldPath`
- diagnostic `Message`

If an existing message, issue, code, category, severity, or result-kind contract
already applies, that existing contract takes priority. P3-03 does not invent a
new diagnostics schema.

## Existing Contract Assessment

The current Validator result contract is sufficient for the Validator's own
boundary because it already carries:

- a result kind such as `validGeneratable`, `validNotGeneratable`, or `invalid`
- `Generatable` as the generation hard-stop flag
- diagnostics with code, category, severity, optional field path, and optional
  message

The current integration boundary is sufficient to stop Manifest generation
when `Result.Generatable = False`.

The remaining limitation is at the caller-facing reporting boundary: a caller
that converts the validation stop into a generic failure message may preserve
the hard stop but provide limited structured detail about the validation
diagnostics. That limitation should be recorded as a future minimal
implementation candidate only if an upper layer needs structured failure
details beyond the current generic stop message.

## Known Gaps

Current known gaps are limited to reporting shape, not Validator semantics:

- upper-layer failure reporting may not expose structured validation
  diagnostics
- caller-facing failure text may identify that validation failed without
  carrying each diagnostic code/category/severity upward
- no separate public diagnostics API is fixed by P3-03

These gaps do not require implementation if existing tests and caller behavior
only require a hard stop before Manifest generation while preserving failure
category at a coarse level.

## Future Minimal Implementation Candidates

If implementation is needed later, the smallest candidate should be limited to
one or both of these:

- preserve validation failure category in the caller's existing failure return
  path
- expose existing Validator diagnostics through an existing return or reporting
  boundary without inventing a new schema

Future implementation must not change Validator semantics, Blueprint parsing,
Manifest generation, Template, GenerateContext, Generator, or production
logging architecture unless separately authorized.

## P3-03 Out of Scope

P3-03 does not perform:

- production VBA code modification
- Validator implementation change
- new diagnostic API implementation
- logging framework implementation
- test code additions
- broad error message wording changes
- Manifest changes
- Template changes
- GenerateContext changes
- Generator changes
- package or `dist` changes
- release operations
- external service operations
- Git staging, commit, or push

## Verification

Required for P3-03:

- `git diff --check`
- docs-only change confirmation

Not required for P3-03:

- tests
- builds
- focused Validator tests
- full Build regression
- release build

## Next Candidate

P3-04 - Validator Failure Contract Implementation

Recommendation:

P3-04 should be GO only if review determines that upper-layer callers need
structured validation diagnostics beyond the existing hard-stop behavior and
existing Validator result contract.

If the existing result/error contract is sufficient for the integration need,
P3-04 should be NO-GO and the next integration candidate should be selected.
