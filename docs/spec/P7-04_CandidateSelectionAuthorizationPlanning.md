# P7-04 - Candidate Selection / Authorization Planning

## Status

COMPLETE / docs-only authorization candidate selection and planning

## Purpose

Resolve the P7-03 NO-GO factors at the planning level by selecting the
authorization candidate that must be completed before the minimum real workbook
/ real VBProject mutation implementation slice can be re-evaluated.

P7-04 is documentation only. It does not authorize implementation, production
code changes, test code changes, workbook open / save / close / SaveAs /
restore, real workbook mutation, real VBProject mutation, package or `dist`
operations, release operations, publication, external service operations, or
Frozen specification changes.

## Starting State

- P6 is COMPLETE.
- P7-01 selected P7-02 as the first P7 candidate.
- P7-02 fixed the required reauthorization boundary for any future real
  workbook / real VBProject mutation implementation GO.
- P7-03 applied the P7-02 boundary and recorded NO-GO for the minimum
  implementation slice.
- The completed mutation boundary remains fake/local target `Modules`
  dictionary create-only mutation through
  `AppOutputWriteService.AppApplyGeneratedOutputToLocalTarget`.
- Real workbook mutation and real VBProject mutation remain NO-GO.
- The current request explicitly starts P7-04 as docs-only and explicitly
  excludes implementation, production / test code, workbook / VBProject
  operations, package / `dist`, release / publication, and external service
  operations.

## P7-03 NO-GO Factors To Resolve

P7-03 recorded implementation NO-GO because all of the following remain
missing, unauthorized, or explicitly excluded:

- separate implementation GO
- exact editable production files
- exact editable test files
- real workbook / real VBProject mutation entry boundary
- workbook fixture ownership, location, lifetime, backup, restore, and cleanup
- workbook open, save, close, SaveAs, and restore behavior
- VBProject trust/access preflight before mutation
- allowed VBProject component operation set
- existing-module conflict behavior
- overwrite, delete, rename, and creation behavior
- no-partial-mutation and rollback behavior
- readback verification requirements
- focused local verification commands

P7-04 does not satisfy these items by itself. It selects the authorization
candidate that must later satisfy them before implementation can be
re-evaluated.

## Authorization Candidate Options

| Candidate | Scope | Decision |
| --- | --- | --- |
| Candidate A | Repository-owner authorization package for the minimum real workbook / real VBProject mutation slice | Selected |
| Candidate B | Direct implementation start for real workbook / real VBProject mutation | Rejected / NO-GO |
| Candidate C | Additional docs-only restatement of P7-02 and P7-03 without an owner authorization package | Deferred |

## Selected Candidate

Selected candidate: `P7-05 - Minimum Real Workbook / Real VBProject Mutation
Authorization Package`.

P7-05 must be a repository-owner authorization package, not implementation.
It must collect and fix the exact authorization facts needed to re-evaluate the
minimum implementation slice. Until P7-05 is complete and a later separate
implementation GO is recorded, the implementation start remains NO-GO.

## Required P7-05 Authorization Package Contents

P7-05 must explicitly record:

- exact editable production files
- exact editable test files
- the single real workbook / real VBProject mutation entry boundary to
  implement
- workbook fixture ownership, location, lifetime, backup, restore, and cleanup
- whether workbook open, save, close, SaveAs, and restore are authorized, and
  the exact limits for each operation
- VBProject trust/access preflight requirements before mutation
- allowed VBProject component operations
- existing-module conflict behavior
- overwrite, delete, rename, and creation behavior, including any prohibited
  operations
- no-partial-mutation and rollback behavior
- readback verification requirements
- focused local verification commands
- required docs-only and implementation diff checks, including
  `git diff --check`
- explicit GO / NO-GO decision for re-evaluating, not starting, the minimum
  implementation slice

If any item is missing, ambiguous, unapproved, or inconsistent with the current
repository state at the time P7-05 is evaluated, the minimum implementation
slice remains NO-GO.

## Minimum Implementation Slice Re-Evaluation Conditions

The minimum implementation slice may be re-evaluated only when all of these are
true:

- P7-05 exists and is complete.
- P7-05 records a repository-owner approval for every required authorization
  package item.
- The exact editable file list is still valid against the current repository
  state.
- The selected mutation entry boundary remains narrow and does not require
  Parser, Validator, Manifest Derivation, Template Derivation, GenerateContext,
  Generator, Output Write, or fake/local target behavior changes.
- The workbook fixture is test-owned, local, restorable, isolated from user
  data, and outside package, `dist`, release, publication, and external service
  paths.
- VBProject trust/access preflight is defined as a hard stop before any
  mutation.
- Allowed module operations and prohibited operations are explicit.
- Conflict, overwrite, delete, rename, creation, no-partial-mutation,
  rollback, restore, and reporting behavior are explicit.
- Readback verification and focused local verification commands are explicit.
- The later task separately grants implementation GO after reviewing P7-05.

These are re-evaluation conditions only. They are not implementation GO.

## Rejected / Deferred Candidates

Candidate B is rejected because P7-03 already recorded implementation NO-GO and
the current request does not grant implementation GO or authorize production
code, test code, workbook operations, or VBProject mutation.

Candidate C is deferred because restating P7-02 and P7-03 would not resolve the
missing authorization facts. The next useful step is a concrete owner
authorization package that can be checked against the current repository state.

## GO / NO-GO Decision

Decision: `GO` for docs-only P7-04 authorization candidate selection and
planning.

Decision: `GO` for selecting `P7-05 - Minimum Real Workbook / Real VBProject
Mutation Authorization Package` as the next docs-only candidate.

Decision: `NO-GO` for implementation start.

Decision: `NO-GO` for the minimum real workbook / real VBProject mutation
implementation slice until P7-05 is complete and a later separate
implementation GO is explicitly recorded.

## Preserved NO-GO Operations

The following remain NO-GO:

- implementation start
- production code changes
- test code additions or updates
- real workbook mutation
- real VBProject mutation
- workbook open, save, close, SaveAs, restore, or workbook fixture mutation
- VBProject module import, export, overwrite, delete, rename, or creation
- package, `dist`, release, publication, tag creation, push, or external
  service operations
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

P7-04 verification is docs-only:

- reviewed P7-03 Implementation GO / NO-GO Decision
- reviewed P7-02 Real Workbook / Real VBProject Mutation Reauthorization
  Boundary
- reviewed backlog, current-status, and handoff P7 records
- confirmed P7-03 NO-GO factors require an authorization package before
  implementation re-evaluation
- selected P7-05 as the next docs-only authorization package candidate
- confirmed this task explicitly excludes implementation, production / test
  code, workbook / VBProject operations, package / `dist`, release /
  publication, and external service operations

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
