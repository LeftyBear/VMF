# P6-10 - Target VBA Project Mutation Focused Test Design

## Status

COMPLETE / docs-only focused test design

## Purpose

Fix the focused local test design for a future target VBA project mutation
implementation after P6-09 Target VBA Project Mutation Boundary Planning.

P6-10 is documentation only. It does not authorize production code changes,
test code changes, generated output write, or target VBA project mutation.

## Scope

P6-10 defines:

- the formal P6-10 title and docs-only scope
- the exact future target surface for target VBA project mutation tests
- the allowed and prohibited mutation operations for a future implementation
- safety stops before any target VBA project mutation
- focused verification expectations for a future local-only implementation
- GO / NO-GO boundaries for the next implementation decision

## Target Surface

The future target VBA project mutation surface is limited to a local,
test-controlled VBA project representation or equivalent fake target supplied
by a separately authorized implementation task.

A future implementation may only consume already approved output from
`AppOutputWriteService.AppWriteGeneratedOutput` deterministic local folder
write results, or a focused-test local equivalent that preserves the same
approved `fileName` / `generatedSource` unit identity.

The target surface must not include:

- a real user workbook
- a production workbook
- package or `dist` artifacts
- external services
- token stores or credentials
- runtime state used to choose Templates or complete missing upstream facts

## Mutation Operations

A future implementation GO must name the exact mutation operations before code
or test edits. Candidate operations are limited to module import or replacement
inside the approved local test-controlled target surface.

The future task must define:

- module identity matching rules
- existing-module conflict policy
- overwrite policy
- creation policy for missing modules
- deletion and rename policy
- ordering or grouping expectations, if any
- no-partial-mutation behavior
- rollback, restore, or recovery expectations

Unless separately authorized, delete, rename, broad overwrite, project
reference changes, workbook save behavior, signed project changes, package
updates, and real target VBA project mutation remain NO-GO.

## Future Successful-State Test Cases

A future implementation slice should include focused local tests that prove:

- complete approved generated output is the only mutation input
- deterministic local write output is consumed without re-deriving Template,
  GenerateContext, or Generator facts
- module identity and generated source content are carried unchanged into the
  target mutation request
- the approved local test-controlled target receives only the explicitly
  authorized mutation operations
- success is reported only after every required module mutation completes
- no package, `dist`, release, external service, or real workbook mutation is
  performed

## Future Blocking-State Test Cases

A future implementation slice should include focused local tests that prove no
target mutation occurs when:

- upstream state is missing, failed, partial, ambiguous, unsupported,
  unapproved, fallback-derived, or implicitly selected
- generated output write did not complete successfully for every required unit
- generated file names contain paths, blank names, duplicates, conflicts, or
  unsupported module identities
- generated source content is blank, missing, conflicting, or incomplete
- mutation would require Template fallback, implicit Template selection,
  Template content inference, GenerateContext compensation, or Generator
  compensation
- mutation would require reading raw Blueprint, Parser output, Validator
  diagnostics, Manifest Derivation diagnostics, Template contents,
  GenerateContext diagnostics, Generator internals, target project runtime
  state, or external state as design input
- conflict handling, overwrite behavior, rollback behavior, or recovery
  behavior is not explicitly authorized
- the requested target is a real user workbook, package, `dist`, release
  artifact, or external service

The tests must verify hard stops before any target mutation.

## Safety Stops

Future implementation must stop before edits or mutation if:

- the current codebase cannot identify a narrow target mutation entry boundary
- the implementation task does not explicitly authorize exact editable files
- the target surface is not local and test-controlled
- mutation operations are not explicitly named
- conflict, overwrite, no-partial-mutation, and recovery behavior are not
  defined
- the implementation requires fallback Template selection, implicit Template
  selection, Template content inference, GenerateContext or Generator
  compensation, public API changes, persisted schema changes, canonical format
  changes, Frozen specification changes, package or `dist` operations, release
  operations, external services, credentials, token stores, or live user data
- existing user changes conflict with the target files

## Scope Planning Decision

GO:

- P6-10 docs-only Target VBA Project Mutation Focused Test Design
- backlog and current-status updates recording P6-10 completion
- `git diff --check`
- commit and push of the P6-10 docs-only record, because this task explicitly
  authorizes commit and push after verification

NO-GO:

- production code changes
- test code additions or updates
- generated output write beyond the existing deterministic local folder write
- target VBA project mutation
- real workbook mutation
- module import, export, overwrite, delete, rename, or creation in a real
  target VBA project
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, or target mutation behavior changes
- Template file changes
- package, `dist`, release, publication, or external service operations
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Preserved Boundaries

P6-10 preserves the P5-04 through P6-09 boundaries:

- only complete approved downstream output may reach target mutation planning.
- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- `AppOutputWriteService.AppWriteGeneratedOutput` writes approved units only to
  a deterministic local folder.
- actual generated output write remains separate from target VBA project
  mutation.
- target VBA project mutation remains NO-GO until a separate implementation GO
  authorizes exact target surface, operations, safety stops, and verification.
- Output Write and target mutation must not select Templates by fallback,
  implicit selection, Template contents, GenerateContext behavior, Generator
  behavior, generated output, target project state, or runtime state.
- Output Write and target mutation must not infer, repair, normalize, or
  complete missing upstream Template Derivation, GenerateContext, or Generator
  facts.

## Verification Performed

P6-10 verification is docs-only:

- reviewed P6-09 Target VBA Project Mutation Boundary Planning
- reviewed backlog and current-status P6-09 records
- confirmed actual generated output write remains limited to
  `AppOutputWriteService.AppWriteGeneratedOutput` deterministic local folder
  write only
- confirmed target VBA project mutation remains NO-GO
- confirmed no implementation, tests, generated output write, target mutation,
  package, `dist`, release, publication, or external operation is part of this
  task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
