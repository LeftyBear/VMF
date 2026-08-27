# P7-12 - Create-Only Missing-Module Implementation Slice Selection

## Status

COMPLETE / docs-only implementation slice selection and GO / NO-GO record

## Purpose

Evaluate P7-11-A through P7-11-L and select the smallest later implementation
slice that preserves the P7-07 create-only missing-module real VBProject
mutation boundary.

P7-12 is documentation only. It does not start implementation, does not change
production code or test code, does not open / save / close / SaveAs / restore
any workbook, does not mutate any workbook or VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
and does not change public APIs, persisted schemas, canonical formats, or
Frozen specifications.

## Starting State

- P7-07 completed the minimum local-only real workbook / real VBProject
  mutation slice in commit `78d1ab2b456ffa9fd923d79aa481bac0c51ba065`.
- P7-10 selected preserve-create-only focused coverage expansion as the
  lowest-risk future expansion direction.
- P7-11 defined cases P7-11-A through P7-11-L as the candidate coverage set.
- P7-11 granted no implementation GO.
- This P7-12 task is explicitly docs-only and performs no implementation,
  test change, workbook operation, or VBProject mutation.

## Preserved Boundary

The selected later implementation slice must preserve the P7-07 boundary:

- consume only an already successful output write plan;
- require an explicitly supplied target VBProject;
- require local-only, test-owned focused verification;
- complete trust/access and complete component preflight before mutation;
- permit only create-only missing-module mutation for supported standard and
  class modules;
- treat any requested module that already exists as a preflight conflict;
- require readback verification before success;
- roll back only components created by the current operation after a
  post-preflight mutation or readback failure;
- report no partial success.

## P7-11-A Through P7-11-L Evaluation

| Case | Selection result | Reason |
| --- | --- | --- |
| P7-11-A | Selected for minimum slice | Confirms the positive multi-module create-only path while staying inside supported standard and class module creation. |
| P7-11-B | Selected for minimum slice | Adds deterministic input-order coverage without expanding the mutation operation set. |
| P7-11-C | Selected for minimum slice | Adds a pre-mutation duplicate-name hard stop and protects no-partial-mutation behavior. |
| P7-11-D | Selected for minimum slice | Adds a pre-mutation existing-target conflict hard stop and protects existing components. |
| P7-11-E | Deferred | Unsupported-kind preflight is important, but it broadens invalid-kind matrix coverage beyond the smallest slice. |
| P7-11-F | Deferred | Empty or missing source preflight remains required, but it is not needed to prove the first minimum expansion. |
| P7-11-G | Deferred | Trust/access failure coverage is required before broader mutation confidence, but may require separate fixture or adapter control. |
| P7-11-H | Deferred | Post-preflight creation failure exercises rollback and failure injection; it is higher risk than the first minimum slice. |
| P7-11-I | Deferred | Readback-missing failure exercises rollback and readback fault injection; defer to a later failure-focused slice. |
| P7-11-J | Deferred | Readback mismatch / wrong-kind failure is valuable but belongs with later readback failure expansion. |
| P7-11-K | Deferred | Incomplete rollback requires controlled rollback failure behavior and is not the minimum implementation slice. |
| P7-11-L | Selected for minimum slice | Confirms unrelated pre-existing components are preserved and not counted while requested modules remain missing and create-only. |

## Selected Minimum Implementation Slice

Decision: select P7-11-A, P7-11-B, P7-11-C, P7-11-D, and P7-11-L as the
minimum later implementation slice.

The selected slice is limited to focused coverage around successful
multi-module create-only apply, deterministic input ordering, duplicate-name
preflight rejection, requested existing-component preflight rejection, and
preservation of unrelated pre-existing components.

The selected slice may later be implemented only if a separate
implementation-start task authorizes exact editable files, fixture handling,
workbook handling, VBProject access assumptions, verification commands, and
rollback expectations. P7-12 itself does not provide that implementation
start.

## Deferred Cases

P7-11-E through P7-11-K are deferred from the minimum slice. They remain valid
future coverage candidates, but require a later named scope because they
exercise broader invalid-input, trust/access, post-preflight failure, readback
failure, and rollback-failure behavior.

Deferral does not weaken the boundary. Any later implementation that observes
unsupported kinds, empty sources, failed access, failed creation, failed
readback, readback mismatch, or incomplete rollback must still fail closed and
must not report partial success.

## Candidate Editable Scope For Later GO

If the selected slice is later authorized for implementation, the candidate
editable scope remains:

- production file:
  `src/Build/Application/AppOutputWriteService.cls`;
- test file:
  `tests/unit/Build/AppOutputWriteBoundaryTests.bas`;
- entry boundary:
  `AppOutputWriteService.AppApplyGeneratedOutputToRealVBProject`.

No other production file, test file, Template, GenerateContext, Generator,
specification, package, `dist` artifact, release record, external service, or
workbook fixture path is authorized by P7-12.

## Prohibited Operations

The following remain NO-GO in P7-12:

- implementation start;
- production code changes;
- test code changes;
- workbook open, save, close, SaveAs, restore, creation, replacement, or
  fixture mutation;
- workbook or VBProject mutation;
- overwrite, delete, rename, import, export, or arbitrary component creation;
- mutation of production workbooks or real user data;
- fallback Template selection;
- implicit Template selection;
- Template content inference;
- GenerateContext or Generator compensation;
- package / `dist` creation, update, replacement, or inspection beyond
  existing repository evidence;
- release, tag, push, or publication operations;
- external service operations;
- credential or token-store access;
- public API changes;
- persisted schema changes;
- canonical format changes;
- Frozen specification changes.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-12 as docs-only implementation slice
selection and GO / NO-GO record.

Decision: `GO` for selecting P7-11-A, P7-11-B, P7-11-C, P7-11-D, and P7-11-L
as the minimum later implementation slice.

Decision: `NO-GO` for implementation in P7-12.

Decision: `NO-GO` for production code or test code changes in P7-12.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore or any
workbook / VBProject mutation in P7-12.

Decision: `NO-GO` for expanding beyond the P7-07 create-only missing-module
operation set.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Verification Performed

P7-12 verification is docs-only:

- reviewed P7-11 focused coverage expansion scope;
- evaluated P7-11-A through P7-11-L;
- selected the minimum later implementation slice while preserving the P7-07
  create-only missing-module mutation boundary;
- confirmed this task grants no implementation, test change, workbook
  operation, or VBProject mutation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
