# P3-05 - Validator Caller Reporting Evaluation

## Status

EVALUATION / docs-only

## Purpose

Evaluate whether upper callers need to preserve and return the existing
structured validation diagnostics from the Blueprint Validator.

P3-05 is documentation only. It does not authorize production VBA code
modification, test additions, Validator changes, logging framework additions,
diagnostics schema redesign, broad error wording changes, Manifest changes,
Template changes, GenerateContext changes, or Generator changes.

## Current Call and Reporting Flow

Current manifest-generation entry path:

```text
Build_BlueprintParser.BuildGenerateManifestContent(layerName)
-> EnsureInitialized
-> ValidateForManifestGeneration
-> BlueprintValidator.Validate(CreateValidationBlueprint())
-> If Result.Generatable = False:
     Err.Raise ComErrInvalidState,
       "Build_BlueprintParser",
       "Blueprint validation failed before Manifest generation."
-> layer existence check
-> layer item count check
-> manifest line formatting
-> manifest content string
```

`BuildInitializeFromContent` remains the parser initialization path. It parses
the blueprint content into the parser's current internal model and returns a
`ComResult`. Parser failures are wrapped through `ComCreateFailureFromErr`.
Validator execution is not part of `BuildInitializeFromContent`; it runs later
inside `BuildGenerateManifestContent` before manifest content is emitted.

## Validator Result Contract

`BlueprintValidator.Validate` currently returns a `BlueprintValidationResult`
with:

| Field | Current meaning |
| --- | --- |
| `ResultKind` | `validGeneratable`, `validNotGeneratable`, or `invalid` |
| `Generatable` | hard-stop flag; only `True` allows generation |
| `Diagnostics` | `Collection` of `BlueprintValidationDiagnostic` values |

Each `BlueprintValidationDiagnostic` currently carries:

| Field | Current meaning |
| --- | --- |
| `Code` | stable `BP###` validation code |
| `Category` | validation category such as `MissingRequiredField`, `UnsupportedEnumValue`, `ApprovalConflict`, or `GenerationIneligible` |
| `Severity` | `error` or `info` |
| `FieldPath` | field path, including nested/indexed paths where available |
| `Message` | optional message; currently initialized as blank by Validator-created diagnostics |

The Validator keeps structured details at its own boundary. Invalid semantic
input produces `ResultKind = invalid`, `Generatable = False`, and error
diagnostics. Structurally valid but generation-ineligible input produces
`ResultKind = validNotGeneratable`, `Generatable = False`, and the `BP204`
informational diagnostic.

## Boundary Information Retention

| Boundary | Preserved information | Lost or not exposed information |
| --- | --- | --- |
| Parser internal model to `CreateValidationBlueprint` | Parser-derived blueprint name, version, modules, target, approval, and generation-policy values used by Validator | The original parsed text shape is not passed to Validator; this is outside P3-05 because Validator consumes the existing parser-derived model |
| `BlueprintValidator.Validate` return | `ResultKind`, `Generatable`, full `Diagnostics`, and diagnostic `Code`, `Category`, `Severity`, `FieldPath`, `Message` | No loss at the Validator boundary |
| `ValidateForManifestGeneration` local caller | `Generatable` is read and enforced as a hard stop | `ResultKind` and `Diagnostics` are not carried into the raised caller-facing error |
| `BuildGenerateManifestContent` caller-facing failure | Failure source and message identify that blueprint validation failed before Manifest generation | Structured diagnostic collection is collapsed into a single fixed message |
| `BuildInitializeFromContent` caller-facing parse failure | Parse failures return a `ComResult` from `ComCreateFailureFromErr` | Not a validation boundary; parse errors and validation errors use different entry points/reporting shapes |

## Evaluation

### A. Failure Distinction

Current caller-facing reporting can distinguish validation failure from parser
failure at a coarse level:

- parser failures are returned by `BuildInitializeFromContent` as `ComResult`
  failures from the parser initialization path
- validation failures occur in `BuildGenerateManifestContent` and raise
  `ComErrInvalidState` with the explicit message
  `Blueprint validation failed before Manifest generation.`
- manifest generation failures after validation use separate checks and
  messages, such as undefined layer, empty generation layer, or invalid
  `ManifestItem`

This is sufficient to identify that the hard stop was a validation stop rather
than a parser stop or normal manifest derivation failure.

### B. Structured Diagnostics Retention

Structured diagnostics are retained by `BlueprintValidator.Validate` but are
lost at the immediate caller boundary. `ValidateForManifestGeneration` only
checks `Result.Generatable`. When `Generatable = False`, it raises a fixed
single-message error and does not expose:

- `ResultKind`
- diagnostic `Code`
- diagnostic `Category`
- diagnostic `Severity`
- diagnostic `FieldPath`
- diagnostic `Message`

The hard-stop meaning is preserved, but diagnostic detail is not available to
the caller of `BuildGenerateManifestContent`.

### C. Cause Identification

For a user or caller that only needs to know whether Manifest generation was
blocked before downstream generation, the current message is enough.

For a caller that needs to identify why validation failed, the current
caller-facing reporting is not enough. The Validator result already contains
the needed cause-identification fields, but `BuildGenerateManifestContent` does
not return or expose them. The caller can identify that validation failed, but
cannot obtain the specific code/category/severity/field path that explains the
failure.

## Decision

INSUFFICIENT.

Reason:

- validation failure is identifiable
- hard-stop behavior is preserved
- structured diagnostics are available at the Validator boundary
- structured diagnostics are not preserved across the
  `ValidateForManifestGeneration` / `BuildGenerateManifestContent`
  caller-facing boundary
- callers cannot retrieve the existing diagnostic cause fields from the final
  manifest-generation failure

Implementation recommendation:

GO for a later minimal implementation candidate.

This is not a Validator semantics gap. It is a caller-reporting gap.

## Minimal Implementation Slice

If implementation is authorized later, keep the slice limited to preserving the
existing validation result details through the current caller boundary.

Minimum acceptable scope:

- preserve the validation failure category in the caller-facing failure path
- expose or carry the existing `BlueprintValidationResult` information without
  inventing a new diagnostics schema
- keep `Result.Generatable = False` as the hard stop
- keep Manifest, Template, GenerateContext, Generator, and Validator semantics
  unchanged

Out of scope for the implementation slice:

- new diagnostics schema
- Validator rule changes
- Parser model redesign
- Manifest/Template/GenerateContext/Generator changes
- logging framework changes
- broad error wording changes

## Next Candidate

P3-06 - Validator Caller Reporting Minimal Implementation.

Recommended candidate boundary:

Implement the smallest caller-facing carrier or wrapper that preserves the
existing `ResultKind`, `Generatable`, and `Diagnostics` fields when
`BuildGenerateManifestContent` stops on validation failure.

## Verification

Required for P3-05:

- `git diff --check`
- docs-only change confirmation

Not run for P3-05:

- tests
- builds
- focused Validator tests
- full Build regression
- release build
