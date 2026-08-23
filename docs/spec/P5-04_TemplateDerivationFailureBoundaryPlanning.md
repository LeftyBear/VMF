# P5-04 - Template Derivation Failure Boundary Planning

## Status

COMPLETE / docs-only failure boundary planning

## Purpose

Define the failure boundary for Template Derivation so only complete,
approved, generatable Template candidates may proceed toward GenerateContext.

P5-04 is documentation only. It does not authorize implementation GO.

## Scope

P5-04 defines:

- the Template Derivation input boundary
- the conditions under which a Template candidate may be passed downstream
- failure cases that must not be passed downstream
- treatment of unsupported, non-generatable, ambiguous, incomplete, and
  unapproved state
- the prohibition on fallback or implicit Template selection
- the hard-stop boundary before GenerateContext and Generator
- the relationship to the P5-03 Template Derivation Output Model
- unresolved items for P5-05 and later planning

## Non-Scope

P5-04 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Template file generation
- Template file changes
- Template Derivation implementation
- GenerateContext construction
- GenerateContext schema changes
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

Template Derivation may evaluate Template candidates only from upstream state
that has already satisfied all of these conditions:

- Validator completed with PASS.
- Manifest Derivation completed successfully.
- the Manifest is approved for generation by the upstream flow.
- the Manifest item is present, ordered, and complete enough to apply the
  approved P5-02 Template selection rules.
- the candidate can be represented by the P5-03 Template Derivation Output
  Model without adding inferred facts.

Template Derivation must not use:

- raw Blueprint text
- unvalidated parsed Blueprint state
- failed, partial, or non-generatable Validator output
- Manifest Derivation diagnostics except as prior hard-stop state
- Template file contents as a source of design intent
- GenerateContext state
- Generator behavior
- generated VBA output
- external runtime state

## Downstream Eligibility

A Template candidate may be passed downstream only when all of these are true:

- `isGeneratable = True` in the P5-03 Template Derivation Output Model.
- exactly one approved P5-02 selection rule matched the Manifest item.
- `templateKey`, `templatePath`, `templateRole`, `selectionRuleId`, and
  `derivationReason` are complete.
- `unsupportedReason` is blank.
- any Manifest-provided `TemplatePath` is consistent with the approved
  deterministic selection rule.
- the selected Template path is part of the approved Template inventory.
- Manifest item order can be preserved exactly.
- no downstream component would need to repair, infer, normalize, or compensate
  for missing Template Derivation facts.

Passing downstream means eligibility for future GenerateContext planning only.
It is not Generator input and does not authorize Generator invocation.

## Failure Cases Not Passed Downstream

Template Derivation must not pass a Template candidate downstream when any of
these states is present:

| Failure case | Required handling |
| --- | --- |
| Upstream parse, validation, or Manifest Derivation did not complete successfully | Hard-stop before Template Derivation output is consumed. |
| Manifest is not approved for generation | Classify as non-generatable and stop before GenerateContext. |
| Required Manifest fact is missing or blank | Classify as incomplete and stop before GenerateContext. |
| Manifest fact requires an unsupported Template decision | Classify as unsupported and stop before GenerateContext. |
| More than one approved rule matches | Classify as ambiguous and stop before GenerateContext. |
| No approved rule matches | Classify as unsupported or non-generatable and stop before GenerateContext. |
| Selected Template path is absent from the approved inventory | Classify as non-generatable and stop before GenerateContext. |
| Manifest `TemplatePath` conflicts with the approved deterministic rule | Classify as conflicting and stop before GenerateContext. |
| `DomainModuleTemplate.txt` is selected without a separate approved Manifest-only rule | Classify as unsupported and stop before GenerateContext. |
| Template selection requires fallback, defaulting, or implicit choice | Classify as unsupported and stop before GenerateContext. |
| Template selection requires Template content inference, Generator behavior, GenerateContext state, runtime state, or generated output | Classify as unsupported and stop before GenerateContext. |
| Output model fields are incomplete or internally inconsistent | Classify as incomplete or non-generatable and stop before GenerateContext. |

Failure state must remain observable as failure state. It must not be converted
into a best-effort Template candidate.

## Unsupported / Non-Generatable / Ambiguous / Incomplete / Unapproved State

Unsupported state:

- no approved P5-02 rule covers the Manifest facts
- the candidate depends on procedure, parameter, return value, dependency, or
  generation-policy binding not carried by the current approved Manifest
  surface
- the candidate targets a Template inventory item that has no approved
  Manifest-only selection rule

Non-generatable state:

- upstream state is not approved for generation
- `isGeneratable` cannot be `True` under the P5-03 rules
- the selected Template identity or path is not present in the approved
  inventory
- the candidate would require downstream repair or compensation

Ambiguous state:

- multiple approved rules match the same Manifest item
- Manifest `TemplatePath` and deterministic selection disagree
- the selected Template cannot be proven unique from approved Manifest facts
  alone

Incomplete state:

- required P5-03 fields are missing or blank
- required Manifest facts such as `ModuleName`, `ModuleType`, `LayerName`, or
  required compatible `TemplatePath` state are absent
- Manifest item order cannot be preserved

Unapproved state:

- Validator PASS, Manifest Derivation success, or Manifest generation approval
  is absent
- a candidate depends on a planning rule that has not been accepted by the
  current P5 record set

All five categories are blocking categories. None may proceed to
GenerateContext or Generator.

## Fallback And Implicit Selection Prohibition

Template Derivation must not:

- fall back from a missing Template to another Template
- default class modules to a Template when `LayerName` is absent
- choose a Template by reading Template file contents
- choose a Template by matching names heuristically
- choose a Template by asking Generator which tokens are usable
- choose `DomainModuleTemplate.txt` because it exists in the inventory
- repair a conflicting Manifest `TemplatePath`
- normalize unsupported Manifest facts into supported ones
- create a synthetic Template candidate to keep generation moving

The only approved successful selections remain the P5-02 deterministic mapping
set carried through the P5-03 output model.

## GenerateContext / Generator Failure Boundary

GenerateContext may consume only complete and generatable P5-03 Template
Derivation Model items.

GenerateContext must not receive:

- unsupported Template candidates
- non-generatable Template candidates
- ambiguous Template candidates
- incomplete Template candidates
- unapproved Template candidates
- Template candidates produced by fallback or implicit selection
- Template candidates that require downstream repair, inference, normalization,
  or compensation

Generator may consume only a complete and successful GenerateContext result
after the downstream GenerateContext boundary is separately approved.

Template Derivation failure remains distinct from parse failure, validation
failure, Manifest Derivation failure, GenerateContext construction failure, and
Generator failure.

## P5-03 Output Model Relationship

P5-04 does not replace the P5-03 Output Model. It fixes the pass / fail
boundary around that model.

P5-03 defines the fields:

- `templateKey`
- `templatePath`
- `templateRole`
- `selectionRuleId`
- `derivationReason`
- `isGeneratable`
- `unsupportedReason`

P5-04 requires downstream handoff to depend on the model state:

- complete fields plus `isGeneratable = True` and blank `unsupportedReason`
  may proceed toward GenerateContext planning.
- `isGeneratable = False`, populated `unsupportedReason`, missing required
  fields, inconsistent fields, or unapproved state must stop before
  GenerateContext.
- reason categories are classification evidence, not permission for downstream
  recovery.

## P5-02 / P5-03 Boundary Preservation

P5-04 preserves P5-02 by:

- using only the approved Template inventory and deterministic selection rules
- keeping `DomainModuleTemplate.txt` unsupported until a separate approved rule
  exists
- treating missing, conflicting, unsupported, ambiguous, and fallback-requiring
  states as hard stops
- forbidding Template content inference as a source of design intent

P5-04 preserves P5-03 by:

- treating the Template Derivation Output Model as the only handoff shape
- requiring complete generatable model items for downstream handoff
- keeping unsupported and non-generatable items out of GenerateContext and
  Generator
- preserving GenerateContext and Generator as downstream responsibilities

## Scope Planning Decision

GO:

- P5-04 docs-only Template Derivation failure boundary planning

NO-GO:

- implementation
- test code implementation
- Template generation
- Template file changes
- GenerateContext construction or changes
- Generator invocation or behavior changes
- runtime behavior changes
- package, `dist`, build, release, external service, or Frozen specification
  changes

## Deferred Items For P5-05 And Later

Deferred items:

- focused local test design for supported and blocking Template Derivation
  states
- exact future diagnostic code constants
- exact future class, module, or record names
- concrete Template Derivation entry point
- whether future Manifest Derivation will carry additional procedure,
  parameter, return value, dependency, or generation-policy facts
- GenerateContext data model details
- implementation GO / NO-GO decision

## Verification Performed

P5-04 verification is docs-only:

- reviewed P5-02 Template inventory and derivation table
- reviewed P5-03 Template Derivation output model planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, Template file changes, GenerateContext
  changes, Generator changes, package, `dist`, build, release, or external
  operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
