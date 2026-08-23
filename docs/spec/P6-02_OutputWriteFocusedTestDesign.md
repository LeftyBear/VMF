# P6-02 - Output Write Focused Test Design

## Status

COMPLETE / docs-only focused test design

## Purpose

Fix the focused local test design for a future output-write boundary
implementation after P6-01 Generator Output Write Boundary Planning.

P6-02 is documentation only. It does not authorize local-only implementation,
output write, or target VBA project mutation.

## Scope

P6-02 defines:

- the formal P6-02 title and docs-only scope
- future focused local test cases for successful output-write planning
- future focused local test cases for output-write hard-stop classifications
- acceptance criteria for a future local-only output-write implementation slice
- prohibited operations for the current docs-only task
- the separation between Generator output construction, output write, and
  target VBA project mutation

## Non-Scope

P6-02 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- local-only output-write implementation
- generated output writes to disk, workbook, VBA project, package, or `dist`
- target VBA project mutation
- module creation, update, removal, import, export, or overwrite
- Template Derivation, GenerateContext, or Generator behavior changes
- Template file changes
- runtime behavior changes
- public API changes
- persisted schema changes
- canonical format changes
- package, `dist`, build, release, publication, or external service operations
- Frozen specification changes

## Target Files

The current P6-02 docs-only task updates only:

- `docs/spec/P6-02_OutputWriteFocusedTestDesign.md`
- `docs/VMF_vNext_Backlog.md`
- `docs/development/CURRENT_STATUS.md`

A future implementation task may evaluate exact production and test file names
only after separate implementation GO. P6-02 does not pre-authorize those
edits.

Candidate future test areas are:

- focused Build unit tests for output-write acceptance and hard-stop cases
- existing Build test runner registration only if required by the separately
  authorized focused tests
- a narrow output-write entry boundary only if separately authorized by an
  implementation GO
- local fake or temporary write targets that prove write-boundary behavior
  without mutating a real target VBA project

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
-> Output Write
-> Target VBA Project Mutation
```

Future output-write tests must start after successful Generator output
construction from complete, successful GenerateContext input. They must not use
raw Blueprint text, unvalidated parsed Blueprint state, Validator diagnostics,
failed or partial Manifest Derivation output, failed or partial Template
Derivation output, failed or partial GenerateContext state, Template file
contents, target project state, or runtime state as design input.

The accepted future test input is complete successful Generator output plus
the narrow local output-write request model approved by the future
implementation task.

Output write remains separate from target VBA project mutation. A future test
may prove a write request, write plan, or local temporary write result only if
that future task explicitly authorizes the local-only boundary. Mutating a real
target VBA project remains a separate downstream boundary.

## Future Successful-State Test Cases

A future implementation slice should include focused local tests for:

- accepting complete successful Generator output as the only generated-content
  input to the output-write boundary
- preserving generated unit order already fixed by approved upstream data
- carrying the required generated module identity, template identity, and
  generated source content into the write boundary without re-deriving them
- producing a deterministic write request, write plan, or local temporary write
  result only for complete generated units
- keeping output write separate from target VBA project mutation
- returning overall success only when every required generated unit has a
  complete write-boundary result
- proving that successful output-write handling does not invoke Parser,
  Validator, Manifest Derivation, Template Derivation, GenerateContext, or
  Generator compensation paths

## Future Blocking-State Test Cases

A future implementation slice should include focused local tests for hard-stop
classification when:

- GenerateContext is missing, failed, partial, ambiguous, unsupported, or
  unapproved
- Generator output is missing, failed, partial, empty where content is
  required, or contains unresolved required data
- generated module identity, template identity, generated source content, or
  deterministic ordering is missing, blank, conflicting, or incomplete
- Template selection is fallback-derived, implicit, ambiguous, unsupported, or
  inferred from Template contents
- output-write handling would need raw Blueprint content, Parser output,
  Validator diagnostics, Manifest Derivation diagnostics, Template Derivation
  diagnostics, GenerateContext diagnostics, Template file contents, Generator
  internals, target project state, or external runtime state as design input
- output-write handling would need to repair, normalize, infer, complete, or
  compensate for incomplete upstream data
- the requested behavior requires target VBA project mutation controls not
  explicitly authorized by the future task
- the requested behavior requires package, `dist`, release, publication,
  external service, public API, persisted schema, canonical format, or Frozen
  specification changes

The tests must verify hard stops before any output write or target VBA project
mutation.

## Acceptance Criteria

P6-02 docs-only acceptance criteria are:

- the P6-02 formal title and docs-only scope are fixed
- future successful-state output-write test cases are defined from P6-01
- future blocking-state output-write test cases preserve the P5-04 through
  P6-01 hard-stop boundaries
- the output-write boundary remains post-Generator
- target VBA project mutation remains a separate downstream boundary
- future implementation acceptance criteria are recorded without authorizing
  implementation
- prohibited operations are explicit
- fallback, implicit Template selection, Template content inference,
  GenerateContext / Generator compensation, output write, and target mutation
  remain prohibited for this task
- no production code, test code, Template file, GenerateContext, Generator,
  output-write, target-mutation, runtime, package, `dist`, release, external
  service, public API, persisted schema, canonical format, or Frozen
  specification change is performed

Future implementation acceptance criteria, if separately authorized, should
include:

- output write consumes only complete successful Generator output
- output write rejects missing, partial, failed, ambiguous, unsupported,
  unapproved, fallback-derived, or implicitly selected upstream state before
  writing
- output write does not infer, repair, normalize, or complete upstream data
- output write does not mutate a target VBA project unless a separate target
  mutation GO explicitly authorizes that boundary
- focused local tests cover both accepted input and hard-stop cases
- required focused verification and `git diff --check` pass

## Execution Prohibitions

The current P6-02 task must not:

- add or modify production code
- add or modify test code
- run implementation tests as evidence for new behavior
- change Parser, Validator, Manifest Derivation, Template Derivation,
  GenerateContext, Generator, Template, output-write, or target-mutation
  behavior
- write generated output to disk, workbook, VBA project, package, or `dist`
- mutate a target VBA project
- choose Templates by fallback, implicit selection, Template contents,
  GenerateContext state, Generator behavior, target project state, or runtime
  state
- infer missing upstream facts from Template contents, generated output,
  target project state, or downstream behavior
- alter public APIs, persisted schemas, canonical formats, or Frozen
  specifications
- create package, `dist`, build, release, publication, or generated output
  artifacts
- access or modify external services

## Scope Planning Decision

GO:

- P6-02 docs-only Output Write Focused Test Design
- backlog and current-status updates recording P6-02 completion
- `git diff --check`
- commit and push of the P6-02 docs-only record, because this task explicitly
  authorizes commit and push after verification

NO-GO:

- local-only implementation
- production code changes
- test code additions or updates
- Template file changes
- Template Derivation, GenerateContext, or Generator behavior changes
- fallback or implicit Template selection
- Template content inference
- GenerateContext or Generator-side compensation
- generated output writes
- target VBA project mutation
- package, `dist`, release, publication, external service, public API,
  persisted schema, canonical format, or Frozen specification changes

## Deferred Items

Deferred items:

- future implementation GO / NO-GO decision
- exact output-write entry point
- exact production and test file names
- exact test runner registration changes, if any
- exact local fake or temporary write-target shape
- exact write request, write plan, or write-result model
- overwrite policy
- rollback / no-partial-write behavior
- target VBA project mutation controls
- package and `dist` relationship

## Verification Performed

P6-02 verification is docs-only:

- reviewed P6-01 Generator Output Write Boundary Planning
- reviewed P5-04 through P5-15 backlog and current-status boundary records
- confirmed output write remains post-Generator
- confirmed target VBA project mutation remains a separate downstream boundary
- confirmed no implementation, tests, Template file changes, GenerateContext
  changes, Generator changes, output write, target mutation, package, `dist`,
  build, release, publication, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
