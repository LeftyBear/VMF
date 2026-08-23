# P5-03 - Template Derivation Output Model Planning

## Status

COMPLETE / docs-only output model planning

## Purpose

Define the Template Derivation output model that normalizes existing Template
selection results from a Validator-passed Manifest for later GenerateContext
and Generator boundaries.

P5-03 is documentation only. It does not authorize implementation GO.

## Scope

P5-03 defines:

- the input boundary from Validator PASS and successful Manifest state
- the Template Derivation Model output fields
- the responsibility boundary between Template Derivation, GenerateContext, and
  Generator
- supported, unsupported, and non-generatable output states
- failure boundaries that stop before GenerateContext and Generator
- the relationship to the P5-02 Template inventory and selection rules

## Non-Scope

P5-03 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Template file generation
- Template file changes
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

The input to Template Derivation is a Manifest after Validator PASS.

Template Derivation may begin only when:

- Validator completed with PASS.
- Manifest Derivation completed successfully.
- the Manifest is approved for generation by the upstream flow.
- the Manifest contains the Template-selection facts required by the approved
  P5-02 selection rules.

Template Derivation must not use:

- raw Blueprint text
- unvalidated parsed Blueprint state
- failed or partial Validator output
- Manifest Derivation diagnostics except as prior hard-stop state
- Template file contents as a source of design intent
- GenerateContext state
- Generator behavior
- generated VBA output
- external runtime state

## Output Boundary

The output is the Template Derivation Model.

Template Derivation Model is an intermediate model for downstream
GenerateContext planning. It is not Generator input.

Template Derivation is responsible only for normalizing the existing Template
selection result into a stable downstream model. It does not create new design
intent, generate Template files, construct GenerateContext, invoke Generator,
or produce generated VBA output.

## Output Fields

Each Template Derivation Model item contains these fields:

| Field | Required | Meaning |
| --- | --- | --- |
| `templateKey` | Yes when generatable | Stable Template identity selected by the approved P5-02 rule. |
| `templatePath` | Yes when generatable | Approved Template file path corresponding to `templateKey`. |
| `templateRole` | Yes when generatable | Role of the selected Template in the generation flow, such as standard module, class module, or Domain class module. |
| `selectionRuleId` | Yes | Stable identifier of the approved rule that produced the selection or the hard-stop classification. |
| `derivationReason` | Yes | Human-readable trace of the Manifest facts used for selection. |
| `isGeneratable` | Yes | `True` only when the item may continue toward GenerateContext; `False` for unsupported or non-generatable state. |
| `unsupportedReason` | Required when `isGeneratable = False`; blank otherwise | Reason the item must not continue to GenerateContext or Generator. |

The model must preserve Manifest item order when multiple items exist.

The model must not add:

- modules
- procedures
- parameters
- return values
- dependencies
- generation policy facts
- approval state
- fallback Template choices
- inferred design intent
- generated source text
- file-write decisions

## Approved Selection Rules

P5-03 uses the P5-02 deterministic mapping set:

| Rule ID | Manifest condition | `templateKey` | `templatePath` | `templateRole` |
| --- | --- | --- | --- | --- |
| `TD-P5-02-STANDARD-MODULE` | `ModuleType = StandardModule` | `ModuleTemplate` | `templates/ModuleTemplate.txt` | Standard module Template |
| `TD-P5-02-DOMAIN-CLASS` | `ModuleType = ClassModule` and `LayerName = Domain` | `DomainClassTemplate` | `templates/DomainClassTemplate.txt` | Domain class Template |
| `TD-P5-02-CLASS` | `ModuleType = ClassModule` and `LayerName` is Common, Core, Application, Infrastructure, or Presentation | `ClassTemplate` | `templates/ClassTemplate.txt` | Class module Template |

No other Template selection is approved by P5-03.

`DomainModuleTemplate.txt` remains an existing Template inventory item without
an approved Manifest-only selection rule. It must not be returned as
generatable by this model.

## Derivation Reason

`derivationReason` records why the selected Template was chosen.

It may reference only Manifest facts used by approved rules, such as:

- `ModuleType`
- `LayerName`
- the approved selection rule ID
- TemplatePath consistency with the approved rule

It must not contain raw Blueprint content, inferred design intent, Template file
content interpretation, Generator behavior, or external runtime observations.

## Generatable Rule

`isGeneratable = True` only when all of these are true:

- Validator PASS is confirmed.
- Manifest is approved.
- exactly one approved P5-02 rule matches the Manifest item.
- `templateKey`, `templatePath`, `templateRole`, `selectionRuleId`, and
  `derivationReason` are complete.
- the Template file path exists in the approved Template inventory.
- any Manifest-provided TemplatePath is consistent with the approved rule.
- no unsupported or non-generatable condition is present.

`isGeneratable = False` is required when Template Derivation can classify the
item as unsupported or non-generatable without producing downstream input.

Items with `isGeneratable = False` must not be passed to GenerateContext or
Generator.

## Unsupported Reason

`unsupportedReason` is required when `isGeneratable = False`.

Allowed reason categories:

| Reason category | Meaning |
| --- | --- |
| `ManifestNotApproved` | Manifest is not approved for Template Derivation. |
| `TemplateNotUnique` | More than one approved Template rule matches, or selection is ambiguous. |
| `TemplateNotFound` | The selected Template path does not exist in the approved inventory. |
| `ManifestOnlyTemplateMisuse` | A Template with no approved generation rule is being used as a generation target. |
| `UnsupportedManifestFact` | Manifest facts require a Template decision not approved by P5-02. |
| `MissingRequiredManifestFact` | Required Manifest facts for approved selection are absent or blank. |
| `TemplatePathConflict` | Manifest TemplatePath conflicts with the approved deterministic rule. |

The exact future diagnostic code names remain deferred to an implementation
scope task. P5-03 fixes only the output model responsibility and reason
categories.

## Failure Boundary

Template Derivation must hard-stop before GenerateContext when:

- Manifest is unapproved.
- Template cannot be decided uniquely.
- Template file identity or path does not exist in the approved inventory.
- a Manifest-only Template is misused as a generation target.
- unsupported or non-generatable state is detected.
- an item would require Template fallback behavior.
- Template selection requires raw Blueprint, Validator diagnostics, Template
  content inference, GenerateContext state, Generator behavior, external state,
  repair, normalization, or inferred design intent.

Unsupported or non-generatable output must not be passed to Generator.

Failure remains distinct from parse failure, validation failure, Manifest
Derivation failure, GenerateContext construction failure, and Generator
failure.

## GenerateContext / Generator Boundary

GenerateContext may consume only a complete and generatable Template Derivation
Model item.

GenerateContext remains responsible for packaging successful Template
Derivation output together with approved Manifest-derived data into
Generator-ready context.

Generator remains responsible only for consuming a complete and successful
GenerateContext result.

Template Derivation must not:

- generate Template files
- mutate Template files
- create GenerateContext
- fill missing GenerateContext facts
- invoke Generator
- produce generated VBA output
- perform file writes

## P5-02 Alignment

P5-03 preserves P5-02 by:

- using the existing Template inventory and selection rule results as input
  evidence
- normalizing only approved deterministic selections
- keeping `DomainModuleTemplate.txt` unsupported until a separate approved rule
  exists
- recording ambiguous and unsupported states as hard stops
- preserving the downstream GenerateContext and Generator boundaries
- keeping implementation, tests, Template files, package, `dist`, release,
  external service operations, and Frozen specifications out of scope

## Scope Planning Decision

GO:

- P5-03 docs-only Template Derivation output model planning

NO-GO:

- implementation
- test code implementation
- Template file generation or changes
- GenerateContext construction or changes
- Generator invocation or behavior changes
- runtime behavior changes
- package, `dist`, build, release, external service, or Frozen specification
  changes

## Deferred Items

Deferred items:

- concrete Template Derivation entry point
- exact future class, module, or record names
- exact diagnostic code constants
- focused local test file names
- GenerateContext data model details
- implementation GO / NO-GO decision

## Verification Performed

P5-03 verification is docs-only:

- reviewed P5-01 Template Derivation scope planning
- reviewed P5-02 Template inventory and derivation table
- reviewed P4-06 GenerateContext responsibility boundary freeze
- reviewed P4-07 Generator Input Contract scope planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, Template file changes, GenerateContext
  changes, Generator changes, package, `dist`, build, release, or external
  operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
