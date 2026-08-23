# P5-07 - GenerateContext Focused Test Design

## Status

COMPLETE / docs-only focused test design

## Purpose

Fix the focused local test design for a future GenerateContext implementation
while preserving the P5-04 through P5-06 boundaries.

P5-07 is documentation only. It does not authorize implementation GO.

## Scope

P5-07 defines:

- the current docs-only update scope for P5-07
- future focused local test cases for successful GenerateContext construction
- future focused local test cases for GenerateContext hard-stop classifications
- acceptance criteria for a future local-only implementation slice
- prohibited operations for the current docs-only task
- the Parser / Validator / Manifest Derivation / Template Derivation /
  GenerateContext / Generator boundary separation future tests must preserve

## Non-Scope

P5-07 does not perform or authorize:

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

## Target Files

The current P5-07 docs-only task updates only:

- `docs/spec/P5-07_GenerateContextFocusedTestDesign.md`
- `docs/VMF_vNext_Backlog.md`
- `docs/development/CURRENT_STATUS.md`

A future implementation task may evaluate exact production and test file names
only after separate implementation GO. P5-07 does not pre-authorize those
edits.

Candidate future test areas are:

- focused Build unit tests for successful GenerateContext construction
- focused Build unit tests for GenerateContext hard-stop classifications
- a GenerateContext entry boundary only if separately authorized by an
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

Future GenerateContext tests must start after successful Template Derivation
from approved Manifest input. They must not accept raw Blueprint text,
unvalidated parsed Blueprint state, Validator diagnostics, failed or partial
Manifest Derivation output, unsupported Template Derivation output, Template
file contents, Generator behavior, generated VBA output, or external runtime
state as GenerateContext construction input.

The accepted future test input is complete, approved, generatable P5-03
Template Derivation Model output plus approved Manifest-derived facts allowed
by P5-06, or a narrow local equivalent approved by the future implementation
task.

## Future Successful-State Test Cases

A future implementation slice should include focused local tests for:

- constructing one ordered generation unit for each complete, approved,
  generatable Template Derivation item
- carrying required P5-06 data groups into the generation unit:
  `moduleName`, `moduleType`, `layerName`, `templateKey`, `templatePath`,
  `templateRole`, `selectionRuleId`, and `derivationReason`
- preserving Template Derivation item order when upstream ordering is already
  approved and deterministic
- returning an overall successful GenerateContext result only when every
  required generation unit is complete
- accepting optional body, section, or member source fields only when already
  present in approved Manifest-derived data and separately authorized by the
  future implementation scope
- keeping GenerateContext output as downstream Generator-ready input only, not
  Generator invocation or VBA output

## Future Blocking-State Test Cases

A future implementation slice should include focused local tests for hard-stop
classification when:

- Template Derivation output is absent, failed, partial, unsupported,
  non-generatable, ambiguous, incomplete, unapproved, fallback-derived, or
  implicitly selected
- any required P5-03 Template Derivation Model field is missing, blank, or
  internally inconsistent
- required approved Manifest facts such as `ModuleName`, `ModuleType`, or
  `LayerName` are missing, blank, or inconsistent with Template Derivation
  output
- generation unit ordering cannot be preserved from approved upstream data
- construction requires raw Blueprint content, unvalidated parsed Blueprint
  state, Validator diagnostics, Manifest Derivation diagnostics, Template file
  contents, Generator behavior, generated VBA output, or external runtime state
- construction requires fallback, implicit Template selection, Template content
  inference, downstream repair, normalization, or compensation
- construction requires procedure, parameter, return value, dependency, or
  generation-policy facts not carried by the approved Manifest surface
- construction would require changing Template files or Generator behavior
- construction would produce incomplete or partial Generator input

The tests must verify hard stops before Generator.

## Acceptance Criteria

P5-07 docs-only acceptance criteria are:

- the P5-07 purpose and docs-only scope are fixed
- future successful-state GenerateContext test cases are defined from P5-06
- future blocking-state GenerateContext test cases preserve the P5-04 through
  P5-06 hard-stop boundary before Generator
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

- GenerateContext consumes only complete, approved, generatable Template
  Derivation output and approved Manifest-derived facts
- successful output is complete under the P5-06 data model
- unsupported, non-generatable, ambiguous, incomplete, unapproved,
  fallback-derived, or implicitly selected Template candidates do not reach
  Generator
- focused local tests cover both successful construction and hard-stop
  classifications
- existing Parser, Validator, Manifest Derivation, Template Derivation,
  Template, and Generator behavior is preserved unless separately authorized

## Execution Prohibitions

The current P5-07 task must not:

- add or modify production code
- add or modify test code
- run implementation tests as evidence for new behavior
- change Parser, Validator, Manifest Derivation, Template Derivation,
  GenerateContext, Generator, or Template behavior
- choose Templates by fallback, implicit selection, Template contents,
  GenerateContext state, Generator behavior, or runtime state
- infer missing Template selection or GenerateContext facts from Template
  contents or downstream behavior
- alter public APIs, persisted schemas, canonical formats, or Frozen
  specifications
- create package, `dist`, build, release, or generated output artifacts
- access or modify external services

## Scope Planning Decision

GO:

- P5-07 docs-only GenerateContext focused test design

NO-GO:

- local-only implementation
- production code changes
- test code additions or updates
- Template file generation or changes
- Template Derivation implementation
- GenerateContext construction or implementation
- Generator invocation or behavior changes
- runtime behavior changes
- package, `dist`, build, release, external service, or Frozen specification
  changes

## Deferred Items

Deferred items:

- exact future diagnostic code constants
- exact future class, module, or record names
- concrete GenerateContext entry point
- exact focused test file names
- exact future test helper shape for approved Template Derivation output
- whether future Manifest Derivation will carry additional procedure,
  parameter, return value, dependency, or generation-policy facts
- exact treatment of optional body, section, and member source data
- Generator invocation contract implementation
- implementation GO / NO-GO decision

## Verification Performed

P5-07 verification is docs-only:

- reviewed P5-03 Template Derivation output model planning
- reviewed P5-04 Template Derivation failure boundary planning
- reviewed P5-05 Template Derivation focused test design
- reviewed P5-06 GenerateContext data model planning
- reviewed P4-06 GenerateContext responsibility boundary freeze
- reviewed P4-07 Generator Input Contract scope planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, Template file changes, GenerateContext
  implementation, Generator changes, package, `dist`, build, release, or
  external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
