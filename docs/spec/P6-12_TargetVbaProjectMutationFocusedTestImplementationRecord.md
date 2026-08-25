# P6-12 - Target VBA Project Mutation Focused Test Implementation Start

## Status

COMPLETE / local-only focused implementation

## Purpose

Implement the smallest local-only focused test slice for the P6-10 and P6-11
Target VBA Project Mutation boundary without mutating any real workbook or real
VBProject.

## Scope

P6-12 authorizes only:

- `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget`
- focused tests in `tests/unit/Build/AppOutputWriteBoundaryTests.bas`
- local fake target representation using an in-memory `Modules` dictionary
- create-only mutation of missing module identities inside that fake target
- preflight hard-stops before any fake-target mutation

## GO / NO-GO Decision

GO:

- local-only focused implementation for a test-controlled fake target surface
- create-only module mutation into the fake target `Modules` dictionary
- successful-state and blocking-state focused tests

NO-GO:

- real target VBA project mutation
- real workbook mutation
- VBProject module import, export, overwrite, delete, rename, or creation
- package, `dist`, release, publication, or external service operations
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, or actual generated-output write behavior changes
- Template file changes
- public API, persisted schema, canonical format, or Frozen specification
  changes

## Implemented Boundary

`AppApplyGeneratedOutputToLocalTarget` consumes an already successful
`AppBuildOutputWritePlan` result. It does not read raw Blueprint, Parser output,
Validator diagnostics, Manifest Derivation diagnostics, Template contents,
GenerateContext diagnostics, Generator internals, target runtime state, or
external state as design input.

The local fake target must provide a `Modules` dictionary. The implementation
preflights every write unit before applying any mutation. It hard-stops with no
fake-target mutation for failed plans, missing units, blank or path-bearing
file names, duplicate module identities, missing generated source, invalid
target shape, or an existing target module conflict.

The only allowed operation is create-only insertion of `moduleName` to
`generatedSource` in the fake target `Modules` dictionary.

## Verification Scope

Required verification for P6-12:

- focused Build VBA test runner including `AppRunOutputWriteBoundaryTests`
- existing Build VBA regression
- `git diff --check`

Package, `dist`, release, publication, external services, real workbook
mutation, and real target VBProject mutation are not part of P6-12.

## Verification Performed

- temporary local Build.xlam creation PASS:
  `tools\build\build.ps1 -OutputPath tmp\p6-12\Build.xlam`
- temporary test runner creation PASS:
  `tools\test\setup-test-runner.ps1 -BuildPath tmp\p6-12\Build.xlam`
- existing Build VBA regression PASS:
  `tools\test\run-tests.ps1`
- focused runner PASS:
  `AppRunOutputWriteBoundaryTests`
- `git diff --check` PASS with LF-to-CRLF warnings only
- generated runner, temporary Build.xlam, and debug test artifacts removed after
  verification
