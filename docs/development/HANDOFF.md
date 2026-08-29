# VMF Publisher Handoff

Status  : Publisher v1.0 COMPLETE / future enhancement work is post-v1.0
Scope   : Handoff for next worker, next chat, Codex, or Work Mode
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_Phase4_LocalVerificationEvidence.md, docs/development/Publisher_Phase4_LocalVerificationChecklist.md, docs/distribution/ReleaseChecklist.md

This document is the handoff boundary for continuing VMF Publisher work after
Phase 4 local-only verification, the completed `0.0.1-dev` GitHub prerelease
publication, and post-release closeout. It is intended to prevent accidental
new release gate movement while Avast false positive vendor clearance remains
separately gated.

Subsequent update: ADR-0019 records VMF-side residual risk acceptance and the
Release Hold lift. The `0.0.1-dev` tag, GitHub prerelease, and asset upload are
recorded complete in `CURRENT_STATUS.md` and the release records. Avast vendor
clearance remains not obtained, Avast safety certification is not claimed, and
this handoff does not authorize any new package, tag, release, publication,
Live E2E, Google Docs / Drive mutation, or flagged executable run.

Build vNext update: P6 is COMPLETE. P6-19 Phase Closeout / Current-State
Consistency was committed and pushed as
`7fa2362519bdeee967cde8c0716b369d5b310ffa`; P7-01 Candidate Selection /
GO-NO-GO is recorded docs-only. P7-02 Real Workbook / Real VBProject Mutation
Reauthorization Boundary is now recorded docs-only. P7-02 fixes the future
reauthorization requirements, but P7 implementation start, real workbook
mutation, and real VBProject mutation remain NO-GO until a separate
implementation GO. P7-03 Implementation GO / NO-GO Decision is now recorded
docs-only and keeps the minimum real workbook / real VBProject mutation
implementation slice as NO-GO because the P7-02 reauthorization conditions are
not satisfied. P7-04 Candidate Selection / Authorization Planning is now
recorded docs-only; it selects P7-05 Minimum Real Workbook / Real VBProject
Mutation Authorization Package as the next docs-only candidate and fixes the
authorization package contents and re-evaluation conditions needed before any
later implementation GO can be considered. P7-05 Minimum Real Workbook / Real
VBProject Mutation Authorization Package is now recorded docs-only; it fixes
candidate editable files, test-owned fixture requirements, create-only
missing-module mutation limits, readback / rollback expectations, and
verification requirements for later re-evaluation, but it is not
implementation GO. P7-06 Implementation Re-evaluation / GO-NO-GO is now
recorded docs-only; it applies P7-05 and records GO for a later separate
minimum implementation-start task limited to
`src/Build/Application/AppOutputWriteService.cls`,
`tests/unit/Build/AppOutputWriteBoundaryTests.bas`, and a local test-owned
workbook fixture with create-only missing-module mutation after trust/access
preflight. P7-06 itself performs and authorizes no implementation, production
/ test code change, workbook / VBProject operation, package / `dist`, release
/ publication, external service operation, or Frozen specification change.
P7-07 Minimum Real Workbook / VBProject Mutation Implementation Start is
complete and committed as `78d1ab2b456ffa9fd923d79aa481bac0c51ba065`; it
changed only `src/Build/Application/AppOutputWriteService.cls` and
`tests/unit/Build/AppOutputWriteBoundaryTests.bas`, implemented preflight
hard-stop, create-only missing-module mutation, readback verification, and
rollback inside the P7-05 / P7-06 boundary, and recorded Build PASS, setup
PASS, and all 22 Build VBA runners PASS. P7-08 Minimum Real Workbook /
VBProject Mutation Implementation Closeout is now recorded docs-only in
`docs/spec/P7-08_MinimumRealWorkbookAndVbProjectMutationImplementationCloseout.md`;
it adds no implementation and keeps package / `dist`, release, publication,
external services, and Frozen specification changes as NO-GO. P7-09
Post-Minimum Real Workbook Mutation Next Candidate Selection is now recorded
docs-only in
`docs/spec/P7-09_PostMinimumRealWorkbookMutationNextCandidateSelection.md`; it
selects P7-10 Real Workbook / Real VBProject Mutation Expansion Scope Planning
as the next docs-only candidate and keeps additional implementation, workbook /
VBProject mutation, package / `dist`, release, publication, external services,
and Frozen specification changes as NO-GO. P7-10 Real Workbook / Real
VBProject Mutation Expansion Scope Planning is now recorded docs-only in
`docs/spec/P7-10_RealWorkbookAndVbProjectMutationExpansionScopePlanning.md`;
it organizes future expansion candidates from the P7-07 minimum mutation
boundary, identifies preserve-create-only focused coverage expansion as the
lowest-risk future candidate if separately authorized, requires renewed
authorization for workbook open / close or save / restore, rejects overwrite /
delete / rename / import / export and production workbook operations, and
keeps implementation, workbook / VBProject mutation, package / `dist`, release,
publication, external services, and Frozen specification changes as NO-GO.
P7-11 Create-Only Missing-Module Focused Coverage Expansion Scope is now
recorded docs-only in
`docs/spec/P7-11_CreateOnlyMissingModuleFocusedCoverageExpansionScope.md`; it
concretes P7-10 Candidate A into focused coverage target cases, expected
results, failure / rollback / readback / verification conditions, and
candidate implementation scope while preserving the P7-07 create-only
missing-module mutation boundary, and keeps implementation, workbook /
VBProject mutation, package / `dist`, release, publication, external services,
and Frozen specification changes as NO-GO. P7-12 Create-Only Missing-Module
Implementation Slice Selection is now recorded docs-only in
`docs/spec/P7-12_CreateOnlyMissingModuleImplementationSliceSelection.md`; it
evaluates P7-11-A through P7-11-L, selects P7-11-A/B/C/D/L as the minimum
later implementation slice, defers P7-11-E through P7-11-K, preserves the
P7-07 create-only missing-module mutation boundary, and keeps implementation,
workbook / VBProject mutation, package / `dist`, release, publication,
external services, and Frozen specification changes as NO-GO.
P7-13 Create-Only Missing-Module Focused Coverage Implementation is complete
and committed as `62ebb8ebaf0c0deb591cc6a5f571cc8859ea21d0`; it changed only
`src/Build/Application/AppOutputWriteService.cls` and
`tests/unit/Build/AppOutputWriteBoundaryTests.bas`, implemented the P7-12
selected P7-11-A/B/C/D/L minimum slice, added module-kind readback
verification, and kept P7-11-E through P7-11-K deferred. Verification recorded
focused `AppRunOutputWriteBoundaryTests` PASS, all 22 Build VBA runners PASS,
`git diff --check` PASS, commit / push complete, `HEAD == origin/main`, and
working tree clean. P7-14 Create-Only Missing-Module Focused Coverage
Implementation Closeout is now recorded docs-only in
`docs/spec/P7-14_CreateOnlyMissingModuleFocusedCoverageImplementationCloseout.md`;
it adds no implementation and keeps package / `dist`, release, publication,
external services, and Frozen specification changes as NO-GO. P7-15 Deferred
Failure / Rollback / Readback Candidate Selection is now recorded docs-only in
`docs/spec/P7-15_DeferredFailureRollbackReadbackCandidateSelection.md`; it
evaluates P7-11-E through P7-11-K, prioritizes pre-mutation failure coverage
before readback and rollback fault coverage, selects P7-11-E/F as the next
smallest later candidate, and keeps implementation and workbook / VBProject
mutation as NO-GO. P7-16 Pre-Mutation Failure Coverage Implementation GO /
NO-GO is now recorded docs-only in
`docs/spec/P7-16_PreMutationFailureCoverageImplementationGoNoGo.md`; it records
GO for a later separate implementation-start task limited to P7-11-E/F
unsupported module kind and empty / missing generated source pre-mutation
failure coverage, while P7-16 itself keeps implementation and workbook /
VBProject mutation as NO-GO.

## 1. Starting State

- Publisher v1.0 is COMPLETE under the frozen v1.0 scope and Definition of
  Done;
- responsible-owner completion decision is GO;
- final v1.0 verification passed: Release build PASS, unit 597 / 597,
  non-live integration 16 / 16, and Google Docs Live E2E 4 / 4;
- remaining P2 / vNext work is post-v1.0 enhancement work and is not a
  Publisher v1.0 completion blocker;

Begin from this state:

- Phase 4 local-only verification safety scope is complete;
- current state is local verification complete / Release Hold lifted by VMF
  risk acceptance / `0.0.1-dev` GitHub prerelease published / post-release
  closeout complete;
- Avast false positive handling is pending;
- Avast vendor clearance has not been obtained and Avast safety certification
  is not claimed;
- next version or next phase work starts as a new scope, not as an extension
  of `0.0.1-dev`;
- `0.0.1-dev` release, tag, publication, package, Live E2E, Google Docs
  mutation, and Google Drive mutation are historical completed release-path
  records, not authorization for new operations;
- Frozen specifications, public APIs, and production design remain unchanged.
- Build vNext P6 is complete: deterministic local folder generated output
  write and fake/local target `Modules` dictionary create-only mutation are the
  completed output/mutation boundaries. Build vNext P7-07 completes the
  minimum local-only real workbook / real VBProject mutation slice inside the
  P7-05 / P7-06 authorization boundary. Additional implementation, package /
  `dist`, release, publication, external service operations, and Frozen
  specification changes remain NO-GO.

Local verification completion is not release readiness.

## 2. Stop Line

For any new release-path work after the published `0.0.1-dev` prerelease, do
not perform:

- Release;
- Git tag creation;
- Publication;
- New package creation;
- Package update;
- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- Re-running flagged artifacts without exact operation-specific authorization;
- Frozen specification changes;
- Public API changes;
- Production design changes.

If a requested task requires any gated operation, stop and report the exact
operation and required authorization. Do not infer authorization from Phase 4
local-only verification, ADR-0019, or the completed `0.0.1-dev` publication.

## 3. Allowed Work

The following work may continue when it remains non-release, non-live, and
non-mutating:

- Build;
- Unit tests;
- Non-live integration tests;
- Mock-based verification;
- Dry-run verification;
- Existing package inspection;
- Format check;
- Documentation consistency check.

Existing package inspection must remain inspection only unless a separate
operation-specific authorization names a broader action. Inspection must not
create, replace, update, publish, or approve a package, and it must not re-run a
flagged artifact without explicit authorization.

## 4. Next Actions

Recommended next actions are:

1. Track Avast false positive resolution without claiming vendor clearance.
2. Keep historical Phase 4-3 hold/block records as accepted-at-the-time
   evidence; use `CURRENT_STATUS.md` for the current state.
3. If a follow-up release is reconsidered, record the repository-owner decision
   before any tag, publication, package mutation, Live E2E, or flagged artifact
   re-run.
4. If Live E2E is requested, require explicit per-run authorization, credentials
   scope, destination scope, cleanup expectations, and exact command.
5. Keep vNext hardening work separate as candidate work before adoption.
6. For Build vNext, use
   docs/spec/P7-09_PostMinimumRealWorkbookMutationNextCandidateSelection.md as
   the current P7 next-candidate selection and GO / NO-GO record,
   docs/spec/P7-08_MinimumRealWorkbookAndVbProjectMutationImplementationCloseout.md
   as the current P7 implementation closeout and status-sync record,
   docs/spec/P7-06_ImplementationReevaluationGoNoGo.md as the P7 docs-only
   implementation re-evaluation GO / NO-GO record,
   docs/spec/P7-04_CandidateSelectionAuthorizationPlanning.md as the current
   P7 docs-only authorization candidate selection record,
   docs/spec/P7-05_MinimumRealWorkbookAndVbProjectMutationAuthorizationPackage.md
   as the current P7 docs-only authorization package,
   docs/spec/P7-03_ImplementationGoNoGoDecision.md as the current P7
   implementation NO-GO record, and
   docs/spec/P7-02_RealWorkbookAndVbProjectMutationReauthorizationBoundary.md
   as the reauthorization basis. P7-07 is the completed local-only
   implementation-start task and changed only the two P7-05 / P7-06 files.
   P7-08 closes out that implementation docs-only. P7-09 selects P7-10 Real
   Workbook / Real VBProject Mutation Expansion Scope Planning as the next
   docs-only candidate. P7-10 records the docs-only expansion scope planning
   boundary and keeps implementation and workbook / VBProject mutation as
   NO-GO. P7-11 records the docs-only focused coverage expansion scope for
   P7-10 Candidate A and keeps implementation and workbook / VBProject
   mutation as NO-GO. P7-12 records the docs-only implementation slice
   selection for P7-11-A through P7-11-L, selects P7-11-A/B/C/D/L as the
   minimum later implementation slice, and keeps implementation and workbook /
   VBProject mutation as NO-GO. P7-13 implements only that selected
   P7-11-A/B/C/D/L slice and is closed out by P7-14 with P7-11-E through
   P7-11-K still deferred. P7-15 evaluates those deferred items, selects
   P7-11-E/F pre-mutation invalid write-unit coverage as the next smallest
   later candidate, and keeps implementation and workbook / VBProject mutation
   as NO-GO. P7-16 applies that selection and records GO for a later separate
   implementation-start task limited to P7-11-E/F unsupported module kind and
   empty / missing generated source pre-mutation failure coverage, while P7-16
   itself keeps implementation and workbook / VBProject mutation as NO-GO. Any
   further implementation, workbook / VBProject expansion, package / `dist`,
   release, publication, or external service work requires a new named scope
   and separate GO / NO-GO decision.

The vNext hardening backlog currently includes:

- signing;
- MSI / installer;
- distribution verification;
- security / trust workflow.

### P2-32 Local-Only Preview-Update Follow-Up

P2-32 completed the first narrow local-only `preview-update <markdown-file>`
implementation as vNext follow-up work. It compiles Markdown locally and emits
`PREVIEW_UPDATE_PLAN` and `PREVIEW_UPDATE_SUMMARY` diagnostics without applying
any physical document update.

This handoff update does not authorize or perform Live E2E, Google Docs
mutation, Google Drive mutation, OAuth login, token-store access, package or
`dist` update, release, tag creation, publication, Avast operation, vendor
clearance judgment, or flagged executable re-run.

For continuation, treat P2-32 as local-only planning evidence. Do not treat it
as release clearance, publication approval, vendor clearance, Avast safety
certification, or authorization for future external operations.

## 5. Required Reporting For Future Work

Future local-only work should report:

- files changed;
- commands executed;
- pass, fail, blocked, pending, N/A, or not-executed result;
- warning count and error count when available;
- whether Live E2E was enabled;
- whether Google Docs or Google Drive were mutated;
- whether package creation or package update occurred;
- whether release, tag creation, publication, or announcement occurred;
- whether any flagged artifact was re-run;
- Git branch, staged state, commit state, push state, and working-tree status.

Blocked operations must be reported as not executed, blocked, or pending with a
reason. They must not be omitted.

## 6. Related Commits

| Commit | Meaning |
| --- | --- |
| `fa4d6a6` | Phase 3-9 evidence |
| `6103003` | Phase 4 docs |
| `15cf77d` | Backlog boundary |
| `71bc23f` | LocalVerify boundary |
| `cf77964` | Checklist |
| `e59a7ec` | Execution order |

## 7. Handoff Summary

Treat the repository as ready for local-only follow-up. Treat any new
release-path follow-up as separately gated, while preserving that `0.0.1-dev`
is already recorded as a published GitHub prerelease.

Proceed only inside the allowed scope unless the repository owner explicitly
reopens the relevant release gate or grants operation-specific authorization.
