# P6-06 - Output Write Mutation Boundary Planning

## Status

COMPLETE / docs-only boundary planning

## Purpose

Define the next boundary after the P6-05 Output Write focused test
implementation closeout.

P6-06 is docs-only. It separates actual generated output write from target VBA
project mutation, records GO / NO-GO boundaries for a future task, and
preserves the already implemented `AppOutputWriteService.AppBuildOutputWritePlan`
write-plan boundary.

## Scope

P6-06 records:

- P6-05 closeout commit:
  `3e4e9901070a3f71db1e7549191914e021ba9a38`
- existing Output Write plan entry boundary:
  `AppOutputWriteService.AppBuildOutputWritePlan`
- actual generated output write as the next downstream boundary to plan
- target VBA project mutation as a separate later downstream boundary
- GO / NO-GO boundaries required before any implementation

## Boundary Decision

The next candidate boundary is actual generated output write from an approved
write plan.

Actual generated output write means materializing already approved write-plan
units to a local, deterministic, non-target-project write surface. It must not
select Templates, infer from Template contents, repair upstream data, compensate
for GenerateContext or Generator state, or mutate a target VBA project.

Target VBA project mutation remains separate and later. Importing, exporting,
overwriting, deleting, renaming, or otherwise changing modules in a real target
VBA project is not part of the actual generated output write boundary.

## Future GO Requirements

A future implementation task may proceed only if it explicitly authorizes a
named boundary and exact editable files.

GO for a future actual generated output write task may include:

- focused local tests for writing approved `AppBuildOutputWritePlan` units to a
  deterministic local write surface
- minimal production code limited to the approved output-write boundary, if
  separately authorized
- local temporary or fake write targets only
- verification that failed, partial, fallback-derived, implicitly selected, or
  incomplete upstream state performs no write

GO for target VBA project mutation requires a separate later task. That later
task must explicitly authorize the target-mutation boundary, exact mutation
operations, overwrite policy, no-partial-write behavior, rollback or recovery
expectations, and focused verification.

## NO-GO

P6-06 does not authorize:

- production code changes
- test code additions or updates
- actual generated output write
- target VBA project mutation
- module import, export, overwrite, delete, rename, or creation in a real target
  VBA project
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  or Generator behavior changes
- Template file changes
- package, `dist`, release, publication, or external service operations
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Preserved Boundaries

P6-06 preserves the P5-04 through P6-05 boundaries:

- complete successful Generator output, or the approved focused-test local
  equivalent, is the only accepted input to Output Write planning.
- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- actual generated output write is downstream of write-plan construction.
- target VBA project mutation is downstream of actual generated output write
  and remains separately gated.
- fallback, implicit Template selection, Template content inference,
  GenerateContext compensation, and Generator compensation remain prohibited.
- unsupported, ambiguous, incomplete, unapproved, failed, fallback-derived, or
  implicitly selected upstream state must hard-stop before any write.

## Next Action

The next candidate may be a docs-only or implementation-scope planning task for
actual generated output write from `AppBuildOutputWritePlan` units.

That next task must keep target VBA project mutation NO-GO unless the user
separately authorizes a target-mutation task.

## Verification Plan

Required verification for P6-06:

- confirm P6-05 is recorded in the backlog and current status
- update P6-06 backlog and current-status records
- run `git diff --check`

Build, VBA, package, `dist`, release, and external-service verification are not
required because P6-06 performs no implementation, generated output write, or
target VBA project mutation.
