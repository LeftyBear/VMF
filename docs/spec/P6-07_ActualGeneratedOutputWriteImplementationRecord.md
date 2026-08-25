# P6-07 - Actual Generated Output Write Implementation Start

## Status

COMPLETE / local-only implementation verified

## Purpose

Start the downstream boundary selected by P6-06: actual generated output write
from approved `AppBuildOutputWritePlan` units.

P6-07 authorizes only deterministic local file output from an already
successful output-write plan. It does not authorize target VBA project mutation.

## Scope

P6-07 implements:

- `AppOutputWriteService.AppWriteGeneratedOutput`
- focused tests in `tests/unit/Build/AppOutputWriteBoundaryTests.bas`
- backlog and current-status synchronization

The implementation writes each approved write unit to a local output folder
using the planned `fileName` and `generatedSource`.

## GO / NO-GO Decision

GO:

- actual generated output write from an approved successful
  `AppBuildOutputWritePlan` result
- deterministic local folder write surface only
- focused verification for successful write, failed-plan no-write, and existing
  file hard-stop
- local temporary test folders only

NO-GO:

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
- public API, persisted schema, canonical format, or Frozen specification
  changes

## Minimal Boundary

The P6-07 write boundary accepts only a successful output-write plan with one or
more write units. Each unit must include:

- `fileName`
- `generatedSource`

Before writing, the service preflights all planned local file names. A file name
must not include a path, and an existing destination file hard-stops before any
new file is written.

On success, the service returns `Success`, classification `Success`, and the
local file paths written. On hard-stop, it returns no written files.

## Preserved Boundaries

P6-07 preserves:

- Output Write plan construction remains separate from actual output write.
- actual generated output write remains separate from target VBA project
  mutation.
- target VBA project mutation requires a separate explicit GO.
- upstream unsupported, ambiguous, incomplete, failed, fallback-derived,
  implicitly selected, or unapproved state must stop before write planning or
  actual write.

## Verification

Required verification:

- focused Build VBA test runner covering `AppRunOutputWriteBoundaryTests`
- related Build VBA regression
- `git diff --check`

No package, `dist`, release, publication, external-service, or target VBA
project mutation operation is required or authorized by P6-07.
