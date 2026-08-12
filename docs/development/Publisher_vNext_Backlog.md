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

## P0 — Next Release-Path Gate Items

- Record Avast false positive outcome and owner decision once the vendor
  response is available.
- Use `Publisher_AvastResponseIntakeTemplate.md` first for any received Avast
  response, then use `Publisher_AvastResponseDecisionTemplate.md` to classify
  the reviewed response for the exact selected artifact identity.
- Confirm whether the next release-path decision proceeds on Avast vendor
  clearance, continued VMF-side risk acceptance, or a new owner decision.
- Reconcile release-readiness records after Avast clearance, rejection, or
  continued pending status is reviewed.
- Update go/no-go review only after all release-blocking dependencies are
  resolved or explicitly waived by the owner.
- Confirm whether Live E2E is authorized, not applicable, or remains deferred
  for the next release-path scope.
- Confirm whether Google Docs / Drive readback or mutation checks are
  authorized, not applicable, or remain deferred for the next release-path
  scope.
- Re-run release artifact audit only after package-generation scope is
  explicitly authorized.

## Post-Release Follow-Up Register

This register is the docs-only next-action list after the published
`0.0.1-dev` GitHub prerelease, one attached asset, and final status freeze. It
does not update the release/prerelease record, replace or upload assets,
update package or `dist` output, operate tags, run Live E2E, mutate Google
Docs / Drive, perform OAuth operations, rerun Avast, re-run a flagged
executable, claim vendor clearance, or claim Avast safety certification.

| ID | Trigger | Next action | Allowed records | Gate |
| --- | --- | --- | --- | --- |
| PF-01 | Avast response received | Record the exact received response, response date, artifact/version/sha identity, detection name, and redacted evidence. | `Publisher_AvastResponseIntakeTemplate.md`; follow-on decision template only after review. | Docs-only intake; no execution. |
| PF-02 | Avast response appears favorable | Confirm exact artifact identity match, explicit false-positive treatment, detection-removal / allowlist wording, and whether any additional submission is required before accepting vendor-clearance evidence. | `Publisher_AvastResponseDecisionTemplate.md`; minimal references in current status / release notes / evidence records if accepted. | Vendor clearance evidence only; not release, package, asset, tag, or Live E2E authorization. |
| PF-03 | Vendor clearance is accepted | Update current status, release notes, and evidence index to say vendor clearance was obtained for the exact artifact identity and date while preserving that Avast safety certification is not claimed unless the vendor explicitly provides that certification. | `CURRENT_STATUS.md`, applicable release notes, release approval/evidence index, Voyage Log, CHANGELOG. | Docs-only status update unless a separate release-path operation is authorized. |
| PF-04 | Avast response is adverse, ambiguous, artifact-mismatched, or requests additional action | Keep vendor clearance not obtained; stop release-path assumptions; classify whether remediation, resubmission, package rebuild, withdrawal/rollback consideration, owner risk decision, or abandonment requires a new task. | Intake/decision templates, current status, issue/failure record if authorized. | No package rebuild, asset replacement, withdrawal, or rerun without separate approval. |
| PF-05 | Live E2E or Google Docs / Drive checks are proposed after `0.0.1-dev` | Treat each Live E2E, Google Docs mutation, Google Drive mutation, OAuth/token-store operation, and cleanup check as a separate gate with its own authorization and evidence plan. | Authorization record and redacted evidence only after approval. | No implicit carryover from prerelease publication or vendor response. |
| PF-06 | vNext planning resumes | Choose from the P0/P1/P2 candidates below; keep candidate labels separate from adopted behavior and release authorization. | Backlog update or scoped design record. | Docs-only unless a future implementation task is approved. |

## Avast Response Decision Paths

- Favorable vendor response: verify it names or unambiguously covers the
  selected `0.0.1-dev` artifact identity, version, SHA-256, and detection
  context before any `vendor clearance obtained` wording is added. Record the
  vendor-clearance date and evidence reference, but do not claim Avast safety
  certification unless the response explicitly certifies safety.
- Unfavorable vendor response: keep `vendor clearance not obtained`; identify
  whether the next route is remediation, rebuild/repackage, resubmission,
  withdrawal/rollback review, continued residual-risk decision, or no action.
  Each route requires a separate owner decision and scoped verification plan.
- Ambiguous or partial response: classify as `clarification required` or
  `rejected / not sufficient`; do not promote it to clearance, release
  authorization, or safety certification.
- No response: maintain the current pending state. Do not rerun Avast,
  execute flagged artifacts, mutate Google resources, update packages, or
  alter release assets solely because the response remains pending.

## P1 — Evidence / Release-Safety Hardening Candidates

- Add a compact post-release evidence summary template for future Publisher
  release candidates.
- Improve release-readiness checklist cross-links so blocked, deferred,
  risk-accepted, and local-only evidence remain easy to audit.
- Re-check supply-chain and package evidence only after the approved release
  artifact exists.
- Review Live E2E setup documentation for clearer authorization, credential,
  and cleanup boundaries.
- Review package verification scripts for clearer output that distinguishes
  local verification from release clearance.
- Review AV triage notes and convert durable lessons into a repeatable
  release-safety checklist.

## P2 — vNext Enhancements

- Evaluate whether Google Picker plus `drive.file` least-privilege routing
  should be adopted by a future scoped design task.
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
2. Next release-path basis: Avast clearance, continued VMF-side risk
   acceptance, or new owner decision.
3. Current release-readiness checklist state and go/no-go review status.
4. Live E2E and Google Docs / Drive authorization status for the next scoped
   release-path task.
5. Release artifact audit status after package-generation scope is authorized.
6. Security and supply-chain review status for the approved release artifact.
7. Evidence-summary and checklist hardening items.
8. vNext enhancement candidates.
