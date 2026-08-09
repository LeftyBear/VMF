# Publisher vNext Backlog

## Status

- Current state: Phase 4 local-only verification complete.
- Release state: `0.0.1-dev` GitHub prerelease published after ADR-0019
  VMF-side residual risk acceptance; any new release-path work remains
  separately gated.
- External dependency: Avast false positive response pending; vendor clearance
  has not been obtained and Avast safety certification is not claimed.
- This backlog is documentation-only and does not authorize release, publication, Live E2E, package generation, or Google Docs/Drive mutation.
- Live E2E has not been executed for this backlog.
- Google Docs / Drive mutation has not been executed for this backlog.
- Package and `dist/` artifacts have not been updated for this backlog.
- No new release, tag, or publication has been performed for this backlog.
- The flagged executable has not been re-run for this backlog.

## Scope

This backlog records candidate work for Publisher vNext after the Phase 4
local-only verification state and the subsequent `0.0.1-dev` GitHub prerelease
publication. It is a planning and traceability document only.

Allowed scope:

- Identify resume-gate and release-safety follow-up items.
- Identify post-release hardening candidates.
- Identify vNext enhancement candidates.
- Preserve that Avast vendor clearance is not obtained and that any new
  release-path operation requires separate authorization.

Out of scope:

- Changing production behavior.
- Updating package or distribution artifacts.
- Executing Live E2E.
- Mutating Google Docs or Google Drive resources.
- Re-running the flagged executable.
- Creating a new release, tag, publication, or external repository update.

## Priority Definitions

- P0: Required before future release-path activity or vendor-clearance follow-up
  can be re-evaluated.
- P1: Hardening candidate after future release-path or vendor-clearance
  follow-up items are resolved.
- P2: Enhancement candidate for vNext planning; not required for release
  resumption.

Priority does not authorize implementation. Each item still requires a
separate scoped task, explicit verification plan, and any required approval
gate.

## P0 — Resume-Gate / Release-Safety Items

- Record Avast false positive outcome and owner decision once the vendor
  response is available.
- Reconcile release-readiness records after Avast clearance or rejection.
- Confirm whether Live E2E is authorized, not applicable, or remains deferred.
- Confirm whether Google Docs / Drive readback or mutation checks are
  authorized, not applicable, or remain deferred.
- Re-run release artifact audit only after package-generation scope is
  explicitly authorized.
- Re-check supply-chain and package evidence only after the approved release
  artifact exists.
- Update go/no-go review only after all release-blocking dependencies are
  resolved or explicitly waived by the owner.

## P1 — Post-Release Hardening Candidates

- Improve release-readiness checklist cross-links so blocked, deferred, and
  local-only evidence remain easy to audit.
- Add a compact post-release evidence summary template for future Publisher
  release candidates.
- Review AV triage notes and convert durable lessons into a repeatable
  release-safety checklist.
- Review Live E2E setup documentation for clearer authorization, credential,
  and cleanup boundaries.
- Review package verification scripts for clearer output that distinguishes
  local verification from release clearance.

## P2 — vNext Enhancements

- Evaluate additional Publisher diagnostics that improve troubleshooting
  without changing published document semantics.
- Evaluate clearer dry-run output for Google Docs publication planning.
- Evaluate richer release-note generation from existing verification records.
- Evaluate documentation improvements for OAuth Desktop setup and token-store
  handling.
- Evaluate whether vNext should include more explicit managed-document
  readback reporting.

## Vendor-Clearance Boundary

The following work remains gated because the Avast false positive response is
not available and vendor clearance has not been obtained:

- Treating Avast vendor clearance as complete.
- Claiming Avast safety certification.
- Performing a new release publication.
- Creating or updating release tags for new release work.
- Publishing or replacing package artifacts.
- Re-running the flagged executable without exact authorization.
- Treating local package or smoke evidence as AV/vendor clearance.

VirusTotal no-detection, local exception handling, or a submitted
false-positive report is not equivalent to vendor clearance.

## Deferred / Non-Goals

- No Live E2E execution is included in this backlog.
- No Google Docs / Drive mutation is included in this backlog.
- No package generation or `dist/` update is included in this backlog.
- No new release, tag, publication, or external repository update is included
  in this backlog.
- No flagged executable re-run is included in this backlog.
- No frozen specification, public API, persistence schema, or canonical format
  change is included in this backlog.

## Traceability

This backlog should remain consistent with:

- `CURRENT_STATUS.md`
- `CHANGELOG.md`
- Publisher Voyage Log
- Publisher Phase 4 release-readiness records
- Avast false-positive tracking record

The backlog does not replace any release-readiness record. It identifies
candidate next work while preserving the current published `0.0.1-dev` record,
the ADR-0019 VMF risk acceptance basis, and the unresolved Avast vendor
clearance boundary.

## Recommended Next Review Order

1. Avast false positive response and any owner follow-up decision.
2. Current release-readiness checklist state.
3. Release candidate verification record.
4. Release artifact audit status.
5. Security and supply-chain review status.
6. Go/no-go review status.
7. Live E2E and Google Docs / Drive authorization status.
8. vNext backlog reprioritization after release state changes.
