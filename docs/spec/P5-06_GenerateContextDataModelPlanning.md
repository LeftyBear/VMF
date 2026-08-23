# P5-06 - GenerateContext Data Model Planning

## Status

COMPLETE / docs-only data model planning

## Purpose

Define the future GenerateContext data model boundary after P5-05 Template
Derivation focused test design, while preserving the P5-04 and P5-05 failure
boundaries.

P5-06 is documentation only. It does not authorize implementation GO.

## Scope

P5-06 defines:

- the GenerateContext input boundary from complete, generatable Template
  Derivation output
- the future GenerateContext data model shape needed for Generator-ready input
- required and deferred data groups
- hard-stop conditions before Generator
- prohibited fallback, implicit Template selection, Template content inference,
  downstream repair, and Generator compensation

## Non-Scope

P5-06 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Template Derivation implementation
- GenerateContext implementation
- GenerateContext schema implementation
- Template file generation or changes
- Generator invocation
- Generator behavior changes
- runtime behavior changes
- public API changes
- persisted schema changes
- refactoring
- package, `dist`, build, release, or external service operations
- Frozen specification changes
- implementation GO

## Input Boundary

GenerateContext may begin only after Template Derivation has produced complete,
approved, generatable output under P5-03, P5-04, and P5-05.

Allowed input is limited to:

- complete P5-03 Template Derivation Model items with
  `isGeneratable = True`
- approved P4-derived Manifest facts carried from the upstream flow
- approved P5-02 Template inventory identities and deterministic selection
  rule results already normalized by Template Derivation
- deterministic ordering already present in the approved upstream data

GenerateContext must not use:

- raw Blueprint text
- unvalidated parsed Blueprint state
- Validator diagnostics
- Manifest Derivation diagnostics except as prior hard-stop state
- unsupported, non-generatable, ambiguous, incomplete, or unapproved Template
  Derivation output
- fallback or implicit Template selections
- Template file contents as a source of design intent
- Generator state, Generator behavior, or generated VBA output
- external runtime state

## Future Data Model

The future GenerateContext data model should contain one ordered generation
unit per generatable Template Derivation item.

Each generation unit should carry:

| Data group | Required for P5-06 planning | Source boundary |
| --- | --- | --- |
| `moduleName` | Yes | Approved Manifest-derived module fact. |
| `moduleType` | Yes | Approved Manifest-derived module fact. |
| `layerName` | Yes | Approved Manifest-derived module fact. |
| `templateKey` | Yes | Complete P5-03 Template Derivation Model item. |
| `templatePath` | Yes | Complete P5-03 Template Derivation Model item. |
| `templateRole` | Yes | Complete P5-03 Template Derivation Model item. |
| `selectionRuleId` | Yes | Complete P5-03 Template Derivation Model item. |
| `derivationReason` | Yes | Complete P5-03 Template Derivation Model item. |
| `bodySourcePath` | Deferred | Only if already present in approved Manifest data and accepted by a later implementation scope. |
| `sectionSourcePaths` | Deferred | Only if already present in approved Manifest data and accepted by a later implementation scope. |
| `memberSourcePath` / `memberSourceText` | Deferred | Only if already present in approved Manifest data and accepted by a later implementation scope. |
| procedure / parameter / return value facts | Deferred / unsupported from current Manifest surface | Not inferred by GenerateContext. |
| dependency facts | Deferred / unsupported from current Manifest surface | Not inferred by GenerateContext. |
| generation policy facts | Deferred / unsupported from current Manifest surface | Not inferred by GenerateContext. |

The model must also carry an overall success / failure state. A successful
GenerateContext result may be consumed by a future Generator boundary. A failed
GenerateContext result must provide diagnostic classification only and must not
produce partial Generator input.

## Hard-Stop Conditions

GenerateContext must hard-stop before Generator when:

- Template Derivation output is absent, failed, partial, unsupported,
  non-generatable, ambiguous, incomplete, unapproved, fallback-derived, or
  implicitly selected
- any required P5-03 Template Derivation Model field is missing or inconsistent
- required approved Manifest facts for the planned data model are missing or
  blank
- generation unit ordering cannot be preserved from approved upstream data
- Template Derivation output conflicts with approved Manifest-derived data
- construction requires Template content inference, fallback, normalization,
  repair, or downstream compensation
- construction requires procedure, parameter, return value, dependency, or
  generation-policy facts not carried by the approved Manifest surface
- construction would require changing Template files or Generator behavior
- construction would produce incomplete Generator input

GenerateContext failure remains distinct from parse failure, validation
failure, Manifest Derivation failure, Template Derivation failure, and
Generator failure.

## GenerateContext / Generator Boundary

GenerateContext packages only complete, approved upstream data into
Generator-ready input. It does not select Templates, repair Template
Derivation output, infer missing Manifest facts, invoke Generator, emit VBA, or
perform file writes.

Generator may consume only a complete and successful GenerateContext result
after a separate implementation GO.

## Scope Planning Decision

GO:

- P5-06 docs-only GenerateContext data model planning

NO-GO:

- local-only implementation
- production code changes
- test code additions or updates
- Template Derivation implementation
- GenerateContext construction or implementation
- Template file generation or changes
- Generator invocation or behavior changes
- runtime behavior changes
- package, `dist`, build, release, external service, or Frozen specification
  changes

## Deferred Items

Deferred items:

- concrete GenerateContext class, module, record, or entry point names
- exact future diagnostic code constants
- exact focused local test file names
- whether future Manifest Derivation will carry procedure, parameter, return
  value, dependency, or generation-policy facts
- exact treatment of optional body, section, and member source data
- Generator invocation contract implementation
- implementation GO / NO-GO decision

## Verification Performed

P5-06 verification is docs-only:

- reviewed P5-02 Template inventory and derivation table
- reviewed P5-03 Template Derivation output model planning
- reviewed P5-04 Template Derivation failure boundary planning
- reviewed P5-05 Template Derivation focused test design
- reviewed P4-06 GenerateContext responsibility boundary freeze
- reviewed P4-07 Generator Input Contract scope planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, Template file changes, GenerateContext
  implementation, Generator changes, package, `dist`, build, release, or
  external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
