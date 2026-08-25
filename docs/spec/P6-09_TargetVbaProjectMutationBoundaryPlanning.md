# P6-09 - Target VBA Project Mutation Boundary Planning

## Status

COMPLETE / docs-only boundary planning

## Purpose

Define the target VBA project mutation boundary after the P6-08 actual
generated output write implementation closeout.

P6-09 is docs-only. It records the GO / NO-GO boundary for a future target VBA
project mutation task, keeps actual generated output write separate, and
preserves the implemented deterministic local folder write boundary.

## Scope

P6-09 records:

- P6-08 closeout commit:
  `76278e8d16b77afc8e5572d8e267395a2b068dfe`
- actual generated output write entry boundary:
  `AppOutputWriteService.AppWriteGeneratedOutput`
- actual generated output write implementation surface:
  deterministic local folder write only
- target VBA project mutation as the next separate downstream boundary
- GO / NO-GO requirements required before any target mutation implementation

## Boundary Decision

The next candidate boundary is target VBA project mutation planning.

Actual generated output write is already implemented only as deterministic
local folder write from approved `AppBuildOutputWritePlan` units. That boundary
does not import, export, overwrite, delete, rename, or create modules in a real
target VBA project.

Target VBA project mutation means changing a target VBA project from already
approved generated output. It is downstream of actual generated output write
and remains NO-GO until a separate explicit implementation task records exact
allowed mutation operations, target surface, overwrite policy, conflict
handling, no-partial-write behavior, rollback or recovery expectations, and
focused verification.

## Future GO Requirements

A future target VBA project mutation implementation may proceed only if it
explicitly authorizes a named boundary and exact editable files.

GO for target VBA project mutation must define:

- the allowed target surface
- the allowed module operations
- preflight checks before any mutation
- conflict and existing-module handling
- no-partial-write behavior
- rollback, recovery, or restore expectations
- local-only focused test strategy
- proof that failed, partial, fallback-derived, implicitly selected,
  ambiguous, unsupported, unapproved, or incomplete upstream state performs no
  mutation

## NO-GO

P6-09 does not authorize:

- production code changes
- test code additions or updates
- target VBA project mutation
- module import, export, overwrite, delete, rename, or creation in a real target
  VBA project
- generated output write beyond the already implemented deterministic local
  folder write boundary
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, or Output Write behavior changes
- Template file changes
- package, `dist`, release, publication, or external service operations
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Preserved Boundaries

P6-09 preserves the P5-04 through P6-08 boundaries:

- complete successful Generator output, or the approved focused-test local
  equivalent, remains the only accepted input to Output Write planning.
- `AppOutputWriteService.AppBuildOutputWritePlan` constructs write-plan units
  only.
- `AppOutputWriteService.AppWriteGeneratedOutput` writes approved units only to
  a deterministic local folder.
- actual generated output write remains separate from target VBA project
  mutation.
- target VBA project mutation requires a separate explicit GO.
- Output Write and target mutation must not select Templates by fallback,
  implicit selection, Template contents, GenerateContext behavior, Generator
  behavior, generated output, target project state, or runtime state.
- Output Write and target mutation must not infer, repair, normalize, or
  complete missing upstream Template Derivation, GenerateContext, or Generator
  facts.
- Failed, partial, ambiguous, unsupported, unapproved, fallback-derived, or
  implicitly selected upstream state hard-stops before write planning, actual
  output write, or target mutation.

## Next Action

The next candidate may be target VBA project mutation focused test design or
implementation-scope planning.

That next task must keep target VBA project mutation implementation NO-GO until
the exact target surface, mutation operations, safety stops, and verification
scope are explicitly authorized.

## Verification Plan

Required verification for P6-09:

- confirm P6-08 is recorded in the backlog and current status
- update P6-09 backlog and current-status records
- run `git diff --check`

Build, VBA, package, `dist`, release, and external-service verification are not
required because P6-09 performs no implementation, generated output write, or
target VBA project mutation.
