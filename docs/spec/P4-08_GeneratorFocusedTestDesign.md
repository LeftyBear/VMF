# P4-08 - Generator Focused Test Design

## Status

COMPLETE / docs-only test design

## Purpose

Fix the focused local test design for a future Generator boundary
implementation after P4-07 Generator Input Contract scope planning.

P4-08 defines the minimum test intent, target files, acceptance criteria, and
prohibited operations for a future implementation slice. It does not authorize
implementation GO.

## Scope

P4-08 defines:

- the one-sentence purpose for P4-08
- the docs-only files updated by this planning task
- the future test targets that may be touched only after separate
  implementation GO
- acceptance criteria for the future Generator boundary implementation
- prohibited operations for the current docs-only task
- the Parser / Validator / Manifest Derivation / Template Mapping /
  GenerateContext / Generator separation that future tests must preserve

## Non-Scope

P4-08 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Parser changes
- Validator changes
- Manifest Derivation changes
- Template Mapping changes
- GenerateContext implementation changes
- Template file changes
- Generator implementation changes
- runtime behavior changes
- refactoring
- package, `dist`, build, or release operations
- external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Target Files

The current P4-08 docs-only task updates only:

- `docs/spec/P4-08_GeneratorFocusedTestDesign.md`
- `docs/VMF_vNext_Backlog.md`
- `docs/development/CURRENT_STATUS.md`

A future implementation task may evaluate only after separate GO and may need
to identify the exact Generator production and test files at that time. P4-08
does not pre-authorize those edits.

Candidate future test areas are:

- focused Build unit tests for Generator input acceptance and hard-stop cases
- the existing Build test runner registration only if required for the focused
  tests
- the Generator entry boundary only if separately authorized by an
  implementation GO

## Test Design Boundary

The Build vNext generation flow remains:

```text
Blueprint
-> Parser
-> Validator
-> Manifest Derivation
-> Template Mapping
-> GenerateContext
-> Generator
```

Future Generator tests must start at the Generator input boundary fixed by
P4-07. They must not use raw Blueprint text, unvalidated parsed Blueprint
state, Validator diagnostics, Manifest Derivation diagnostics, partial Template
Mapping output, or partial GenerateContext state as accepted Generator input.

The accepted future test input is a complete and successful GenerateContext
result or the narrow local equivalent approved by the future implementation
task.

## Future Focused Test Cases

A future implementation slice should include focused local tests for:

- accepting a complete and successful GenerateContext result
- rejecting missing GenerateContext input before runtime generation
- rejecting partial GenerateContext input before runtime generation
- rejecting failed GenerateContext input before runtime generation
- rejecting GenerateContext input that lacks required Template binding data
- rejecting GenerateContext input that lacks required Manifest-derived module
  facts
- rejecting GenerateContext input that lacks required Manifest-derived
  procedure facts
- rejecting GenerateContext input that lacks required parameter, return value,
  visibility, dependency, or generation policy facts
- rejecting input when deterministic generation unit ordering is absent
- confirming Generator does not inspect Parser, Validator, Manifest
  Derivation, or Template Mapping diagnostics to continue
- confirming Generator does not add ad hoc parameters to compensate for
  incomplete GenerateContext data
- confirming upstream failures remain distinct from Generator failures

The tests must verify hard stops before runtime generation or output writes
when the P4-07 input contract is not satisfied.

## Acceptance Criteria

P4-08 docs-only acceptance criteria are:

- the P4-08 purpose is fixed in one sentence
- the current docs-only target files are identified
- future implementation candidate file areas are listed without authorizing
  edits
- future focused Generator test cases are defined from the P4-07 input
  contract
- acceptance criteria for the future implementation slice are recorded
- prohibited operations are explicit
- Parser / Validator / Manifest Derivation / Template Mapping /
  GenerateContext / Generator responsibility separation is preserved
- no implementation, test code, runtime behavior, package, `dist`, release,
  external service, staging, commit, push, or Frozen specification change is
  performed

Future implementation acceptance criteria, if separately authorized, should
include:

- Generator accepts only a complete and successful GenerateContext result
- Generator rejects missing, partial, failed, inconsistent, or incomplete
  GenerateContext input before runtime generation
- Generator does not read raw Blueprint, Parser output, Validator diagnostics,
  Manifest Derivation diagnostics, Template Mapping diagnostics, or Template
  file contents as design input
- Generator does not infer, repair, normalize, or complete upstream data
- failure classification remains distinct for parse, validation, Manifest
  Derivation, Template Mapping, GenerateContext, and Generator boundaries
- focused local tests cover both accepted input and pre-Generator hard-stop
  cases
- existing Build regression behavior is preserved

## Execution Prohibitions

The current P4-08 task must not:

- add or modify production code
- add or modify test code
- run implementation tests as evidence for new behavior
- change Parser, Validator, Manifest Derivation, Template Mapping,
  GenerateContext, Generator, or Template behavior
- alter public APIs, persisted schemas, canonical formats, or Frozen
  specifications
- create package, `dist`, build, release, or generated output artifacts
- access or modify external services
- stage, commit, push, tag, merge, rebase, reset, stash, or rewrite Git
  history

## Scope Planning Decision

GO:

- P4-08 docs-only Generator focused test design

NO-GO:

- Generator implementation
- Generator code changes
- test code additions or updates
- runtime behavior changes
- GenerateContext implementation changes
- Template Mapping implementation changes
- Manifest Derivation implementation changes
- Parser or Validator changes
- package, `dist`, build, release, external service, staging, commit, or push
  operations

## Deferred Items

Deferred items:

- concrete Generator entry point design
- concrete GenerateContext data model
- exact focused test file names for implementation
- exact test helper shape for successful GenerateContext input
- token replacement interface review
- Template inventory review
- Generator output behavior review
- overwrite policy review
- implementation GO / NO-GO decision

## Verification Performed

P4-08 verification is docs-only:

- reviewed P4-06 GenerateContext responsibility boundary freeze
- reviewed P4-07 Generator Input Contract scope planning
- reviewed backlog and current-status records
- confirmed no implementation, tests, package, `dist`, build, release,
  staging, commit, push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
