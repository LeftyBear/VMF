# P7-05 - Minimum Real Workbook / Real VBProject Mutation Authorization Package

## Status

COMPLETE / docs-only authorization package

## Purpose

Create the authorization package selected by P7-04 so the minimum real
workbook / real VBProject mutation implementation slice can be re-evaluated in
a later separate task.

P7-05 is documentation only. It is not implementation GO. It does not
authorize production code changes, test code changes, workbook open / save /
close / SaveAs / restore, real workbook mutation, real VBProject mutation,
package or `dist` operations, release operations, publication, external
service operations, or Frozen specification changes.

## Starting State

- P6 is COMPLETE.
- P7-02 fixed the reauthorization boundary for any future real workbook / real
  VBProject mutation implementation GO.
- P7-03 recorded NO-GO for the minimum real workbook / real VBProject mutation
  implementation slice.
- P7-04 selected this P7-05 authorization package as the next docs-only
  candidate.
- P7-04 was committed and pushed as
  `5447576ac88239523bf344cc17397fe1ea857eb2`.
- The completed mutation boundary remains fake/local target `Modules`
  dictionary create-only mutation through
  `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget`.
- Real workbook mutation and real VBProject mutation remain NO-GO.

## Authorization Package

| Required item | P7-05 package value | Authorization state |
| --- | --- | --- |
| Exact editable production files | `src/Build/Application/AppOutputWriteService.cls` only | Candidate fixed for later implementation decision |
| Exact editable test files | `tests/unit/Build/AppOutputWriteBoundaryTests.bas` only | Candidate fixed for later implementation decision |
| Optional test runner registration | Existing Build test runner registration only if required by the later implementation task | Deferred to later implementation GO |
| Real workbook / real VBProject mutation entry boundary | Add a new narrow entry point in `AppOutputWriteService.cls` that consumes an already successful output write plan and an explicitly authorized test-owned workbook target | Candidate fixed for later implementation decision |
| Workbook fixture ownership | A local test-owned workbook fixture only; no user workbook, production workbook, release artifact, package artifact, or `dist` artifact | Candidate fixed for later implementation decision |
| Workbook fixture location | A test-controlled temporary path outside package, `dist`, release, publication, and external service paths | Candidate fixed for later implementation decision |
| Workbook lifetime | Created or copied by the focused local test setup and removed or restored by test cleanup | Candidate fixed for later implementation decision |
| Backup / restore / cleanup | Required before success reporting; cleanup failure must be reported and must not be hidden | Candidate fixed for later implementation decision |
| Workbook open | Allowed only for the explicitly authorized test-owned fixture during the later focused verification | Candidate fixed for later implementation decision |
| Workbook save | Allowed only for the explicitly authorized test-owned fixture after all preflight checks pass | Candidate fixed for later implementation decision |
| Workbook close | Allowed only for the explicitly authorized test-owned fixture during cleanup | Candidate fixed for later implementation decision |
| Workbook SaveAs | Prohibited for the minimum slice unless a later implementation GO explicitly reauthorizes it | Prohibited by this package |
| Workbook restore | Required for test-owned fixture cleanup or failure recovery | Candidate fixed for later implementation decision |
| VBProject trust/access preflight | Required before any VBProject component access or mutation; preflight failure hard-stops with no mutation | Candidate fixed for later implementation decision |
| Allowed VBProject component operations | Create/import a missing standard or class module from complete approved generated output only | Candidate fixed for later implementation decision |
| Existing-module conflict behavior | Existing target module hard-stops before any mutation | Candidate fixed for later implementation decision |
| Overwrite behavior | Prohibited for the minimum slice | Prohibited by this package |
| Delete behavior | Prohibited for the minimum slice except cleanup of the test-owned fixture copy itself | Prohibited by this package |
| Rename behavior | Prohibited for the minimum slice | Prohibited by this package |
| Creation behavior | Create missing modules only after complete preflight and only in the test-owned fixture | Candidate fixed for later implementation decision |
| No-partial-mutation behavior | Required; all target names, source text, fixture access, trust/access, and conflicts must be checked before any mutation | Candidate fixed for later implementation decision |
| Rollback behavior | Required for any mutation failure after preflight; success must not be reported unless readback and cleanup expectations are satisfied | Candidate fixed for later implementation decision |
| Readback verification | Required from the authorized test-owned fixture after mutation | Candidate fixed for later implementation decision |
| Focused local verification commands | Existing Build focused test command for `AppOutputWriteBoundaryTests`, existing Build regression if required by the later implementation GO, and `git diff --check` | Deferred to later implementation GO |

## Minimum Slice Re-Evaluation Decision

Decision: `GO` for recording the P7-05 docs-only authorization package.

Decision: `GO` for later re-evaluation of the minimum implementation slice
against this package.

Decision: `NO-GO` for implementation start.

Decision: `NO-GO` for production code changes, test code changes, workbook
operations, and VBProject mutation during P7-05.

The later re-evaluation may consider only the package values above. It must
still record a separate implementation GO before any production file, test
file, workbook fixture, or VBProject mutation work starts.

## Minimum Future Implementation Slice

If a later separate implementation GO is granted, the minimum slice may include
only:

- edits to `src/Build/Application/AppOutputWriteService.cls`
- edits to `tests/unit/Build/AppOutputWriteBoundaryTests.bas`
- optional existing Build test runner registration only if the later task names
  it as required
- a local test-owned workbook fixture or fixture copy only if the later task
  names its path, lifetime, backup, restore, and cleanup behavior
- a narrow real workbook / real VBProject mutation entry point that consumes an
  already successful output write plan
- trust/access preflight before any VBProject operation
- create-only missing module mutation in the test-owned fixture
- readback verification from the test-owned fixture
- hard-stop verification for failed, partial, fallback-derived, implicitly
  selected, ambiguous, unsupported, unapproved, incomplete, conflicting, or
  preflight-failed states

The later implementation must not re-derive Blueprint, Manifest, Template,
GenerateContext, Generator, or Output Write facts.

## Preserved NO-GO Operations

The following remain NO-GO:

- implementation start by P7-05
- production code changes by P7-05
- test code additions or updates by P7-05
- real workbook mutation by P7-05
- real VBProject mutation by P7-05
- workbook open, save, close, SaveAs, restore, or workbook fixture mutation by
  P7-05
- VBProject module import, export, overwrite, delete, rename, or creation by
  P7-05
- package, `dist`, release, publication, tag creation, push, or external
  service operations by P7-05
- credential, token-store, Google Docs, or Google Drive access
- mutation of real user data or production workbooks
- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator compensation
- Parser, Validator, Manifest Derivation, Template Derivation,
  GenerateContext, Generator, Output Write, or fake/local target mutation
  behavior changes
- Template file changes
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes

## Verification Performed

P7-05 verification is docs-only:

- reviewed P7-04 Candidate Selection / Authorization Planning
- reviewed P7-03 Implementation GO / NO-GO Decision
- reviewed P7-02 Real Workbook / Real VBProject Mutation Reauthorization
  Boundary
- reviewed the current `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget`
  fake/local target mutation boundary as repository evidence only
- reviewed the current focused `AppOutputWriteBoundaryTests` test surface as
  repository evidence only
- confirmed P7-05 is an authorization package and not implementation GO
- confirmed workbook / VBProject mutation and production / test code changes
  remain unperformed and unauthorized by P7-05

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
