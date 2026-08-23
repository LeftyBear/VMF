# P5-05 - Template Derivation Focused Test Design

## Status

COMPLETE / docs-only focused test design

## Purpose

Fix the focused local test design for a future Template Derivation
implementation while preserving the P5-04 failure boundary.

P5-05 is documentation only. It does not authorize implementation GO.

## Scope

P5-05 defines:

- the current docs-only update scope for P5-05
- future focused local test cases for supported Template Derivation states
- future focused local test cases for blocking Template Derivation states
- acceptance criteria for a future local-only implementation slice
- prohibited operations for the current docs-only task
- the Parser / Validator / Manifest Derivation / Template Derivation /
  GenerateContext / Generator boundary separation future tests must preserve

## Non-Scope

P5-05 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Template Derivation implementation
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

## Target Files

The current P5-05 docs-only task updates only:

- `docs/spec/P5-05_TemplateDerivationFocusedTestDesign.md`
- `docs/VMF_vNext_Backlog.md`
- `docs/development/CURRENT_STATUS.md`

A future implementation task may evaluate exact production and test file names
only after separate implementation GO. P5-05 does not pre-authorize those
edits.

Candidate future test areas are:

- focused Build unit tests for Template Derivation supported selections
- focused Build unit tests for Template Derivation hard-stop classifications
- a Template Derivation entry boundary only if separately authorized by an
  implementation GO
- existing Build test runner registration only if required by the separately
  authorized focused tests

## Test Design Boundary

The Build vNext generation flow remains:

```text
Blueprint
-> Parser
-> Validator
-> Manifest Derivation
-> Template Derivation
-> GenerateContext
-> Generator
```

Future Template Derivation tests must start after successful Manifest
Derivation from Validator PASS input. They must not accept raw Blueprint text,
unvalidated parsed Blueprint state, Validator diagnostics, Template file
contents, GenerateContext state, Generator behavior, generated VBA output, or
external runtime state as Template selection input.

The accepted future test input is the approved P4-derived Manifest or a narrow
local equivalent approved by the future implementation task.

## Future Supported-State Test Cases

A future implementation slice should include focused local tests for:

- selecting `templates/ModuleTemplate.txt` when `ModuleType = StandardModule`
  and the Manifest item is otherwise complete and approved
- selecting `templates/DomainClassTemplate.txt` when
  `ModuleType = ClassModule` and `LayerName = Domain`
- selecting `templates/ClassTemplate.txt` when `ModuleType = ClassModule` and
  `LayerName` is Common, Core, Application, Infrastructure, or Presentation
- preserving Manifest item order in Template Derivation output
- returning complete P5-03 fields for generatable items:
  `templateKey`, `templatePath`, `templateRole`, `selectionRuleId`,
  `derivationReason`, `isGeneratable = True`, and blank `unsupportedReason`
- accepting a Manifest-provided `TemplatePath` only when it is consistent with
  the approved deterministic P5-02 rule
- keeping Template Derivation output as downstream GenerateContext planning
  input only, not Generator input

## Future Blocking-State Test Cases

A future implementation slice should include focused local tests for hard-stop
classification when:

- upstream parse, validation, or Manifest Derivation success is absent
- the Manifest is not approved for generation
- required Manifest facts such as `ModuleName`, `ModuleType`, or `LayerName`
  are missing or blank
- `ModuleType` is unsupported
- `LayerName` is unsupported for the approved P5-02 rules
- no approved Template rule matches
- more than one approved Template rule matches
- selected Template path is absent from the approved inventory
- Manifest `TemplatePath` conflicts with the approved deterministic rule
- `DomainModuleTemplate.txt` is selected without a separate approved
  Manifest-only rule
- procedure, parameter, return value, dependency, or generation-policy binding
  is required from facts not carried by the approved Manifest surface
- Template selection requires fallback, defaulting, implicit selection, name
  heuristics, Template content inference, GenerateContext state, Generator
  behavior, runtime state, repair, normalization, or compensation

The tests must verify hard stops before GenerateContext and Generator.

## Acceptance Criteria

P5-05 docs-only acceptance criteria are:

- the P5-05 purpose and docs-only scope are fixed
- future supported-state Template Derivation test cases are defined from P5-02,
  P5-03, and P5-04
- future blocking-state Template Derivation test cases preserve the P5-04
  failure boundary
- future implementation acceptance criteria are recorded without authorizing
  implementation
- prohibited operations are explicit
- fallback, implicit Template selection, Template content inference, and
  GenerateContext / Generator compensation remain prohibited
- no production code, test code, Template file, GenerateContext, Generator,
  runtime, package, `dist`, release, external service, or Frozen specification
  change is performed

Future implementation acceptance criteria, if separately authorized, should
include:

- Template Derivation consumes only approved P4-derived Manifest input
- supported selections match exactly one approved P5-02 rule
- successful output is complete under the P5-03 output model
- unsupported, non-generatable, ambiguous, incomplete, unapproved,
  fallback-derived, or implicitly selected candidates do not reach
  GenerateContext or Generator
- focused local tests cover both supported selections and hard-stop
  classifications
- existing Parser, Validator, Manifest Derivation, Template, GenerateContext,
  and Generator behavior is preserved unless separately authorized

## Execution Prohibitions

The current P5-05 task must not:

- add or modify production code
- add or modify test code
- run implementation tests as evidence for new behavior
- change Parser, Validator, Manifest Derivation, Template Derivation,
  GenerateContext, Generator, or Template behavior
- choose Templates by fallback, implicit selection, Template contents,
  GenerateContext state, Generator behavior, or runtime state
- infer missing Template selection facts from Template contents or downstream
  behavior
- alter public APIs, persisted schemas, canonical formats, or Frozen
  specifications
- create package, `dist`, build, release, or generated output artifacts
- access or modify external services

## Scope Planning Decision

GO:

- P5-05 docs-only Template Derivation focused test design

NO-GO:

- local-only implementation
- production code changes
- test code additions or updates
- Template file generation or changes
- GenerateContext construction or changes
- Generator invocation or behavior changes
- runtime behavior changes
- package, `dist`, build, release, external service, or Frozen specification
  changes

## Deferred Items

Deferred items:

- exact future diagnostic code constants
- exact future class, module, or record names
- concrete Template Derivation entry point
- exact focused test file names
- exact future test helper shape for approved Manifest input
- whether future Manifest Derivation will carry additional procedure,
  parameter, return value, dependency, or generation-policy facts
- GenerateContext data model details
- implementation GO / NO-GO decision

## Verification Performed

P5-05 verification is docs-only:

- reviewed P5-02 Template inventory and derivation table
- reviewed P5-03 Template Derivation output model planning
- reviewed P5-04 Template Derivation failure boundary planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, Template file changes, GenerateContext
  changes, Generator changes, package, `dist`, build, release, or external
  operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
