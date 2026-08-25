# P6-14 - Real Workbook / Real VBProject Mutation Boundary Planning

## Status

COMPLETE / docs-only and local-only boundary planning

## Purpose

Define the next boundary after P6-13 Target VBA Project Mutation Focused Test
Implementation Closeout before any real workbook or real VBProject mutation is
authorized.

P6-14 is documentation only. It records the GO / NO-GO boundary for a future
real workbook / real VBProject mutation task and keeps the completed
fake/local target `Modules` dictionary create-only mutation boundary separate.

## Scope

P6-14 records:

- P6-13 closeout commit:
  `8d5d2660a0cc83731c16ee5271c078c68e3fb440`
- completed fake/local target mutation boundary:
  `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget`
- completed fake/local target surface:
  in-memory `Modules` dictionary only
- real workbook and real VBProject mutation as the next separate downstream
  boundary
- GO / NO-GO requirements required before any real workbook or real VBProject
  mutation implementation

## Boundary Decision

The next candidate boundary is real workbook / real VBProject mutation
planning.

P6-12 and P6-13 completed only the fake/local target boundary. The authorized
mutation surface is create-only insertion into a test-controlled in-memory
`Modules` dictionary after full preflight. That boundary does not open, save,
modify, import into, export from, overwrite, delete, rename, or create modules
in a real workbook or real VBProject.

Real workbook / real VBProject mutation means changing an actual workbook
project from already approved generated output. It is downstream of the
fake/local target focused implementation and remains NO-GO until a separate
explicit implementation task records exact target workbook ownership, allowed
VBProject surface, allowed module operations, trust/access requirements,
preflight checks, conflict handling, no-partial-mutation behavior, recovery or
restore expectations, and focused verification authorization.

## Future GO Requirements

A future real workbook / real VBProject mutation implementation may proceed
only if it explicitly authorizes a named boundary and exact editable files.

GO for real workbook / real VBProject mutation must define:

- the allowed workbook target surface
- the allowed VBProject target surface
- the allowed module operations
- whether workbook open/save/close behavior is authorized
- trust/access preflight checks required before any mutation
- conflict and existing-module handling
- no-partial-mutation behavior
- rollback, recovery, or restore expectations
- local-only focused test strategy before any real workbook operation
- proof that failed, partial, fallback-derived, implicitly selected,
  ambiguous, unsupported, unapproved, or incomplete upstream state performs no
  real workbook or real VBProject mutation

## NO-GO

P6-14 does not authorize:

- production code changes
- test code additions or updates
- real workbook mutation
- real VBProject mutation
- VBProject module import, export, overwrite, delete, rename, or creation
- workbook open, save, close, SaveAs, or file-system mutation
- generated output write beyond the completed deterministic local folder and
  fake/local target boundaries
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, or fake/local target mutation behavior changes
- Template file changes
- package, `dist`, release, publication, or external service operations
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Preserved Boundaries

P6-14 preserves the P5-04 through P6-13 boundaries:

- complete successful Generator output, or the approved focused-test local
  equivalent, remains the only accepted input to Output Write planning.
- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- `AppOutputWriteService.AppWriteGeneratedOutput` writes approved units only to
  a deterministic local folder.
- `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget` mutates only a
  fake/local `Modules` dictionary after full preflight.
- fake/local target mutation remains separate from real workbook and real
  VBProject mutation.
- real workbook and real VBProject mutation require a separate explicit GO.
- Output Write and target mutation must not select Templates by fallback,
  implicit selection, Template contents, GenerateContext behavior, Generator
  behavior, generated output, target project state, or runtime state.
- Output Write and target mutation must not infer, repair, normalize, or
  complete missing upstream Template Derivation, GenerateContext, or Generator
  facts.
- Failed, partial, ambiguous, unsupported, unapproved, fallback-derived, or
  implicitly selected upstream state hard-stops before write planning, actual
  output write, fake/local target mutation, real workbook mutation, or real
  VBProject mutation.

## Next Action

The next candidate may be real workbook / real VBProject mutation focused test
design or implementation-scope planning.

That next task must keep real workbook and real VBProject mutation
implementation NO-GO until the exact target surface, mutation operations,
safety stops, workbook handling, trust/access assumptions, recovery behavior,
and verification scope are explicitly authorized.

## Verification Plan

Required verification for P6-14:

- confirm P6-13 is recorded in the backlog and current status
- update P6-14 backlog and current-status records
- run `git diff --check`

Build, VBA, package, `dist`, release, and external-service verification are not
required because P6-14 performs no implementation, generated output write,
fake/local target mutation, real workbook mutation, or real VBProject mutation.
