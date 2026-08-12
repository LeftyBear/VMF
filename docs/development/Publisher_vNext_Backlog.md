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

## First Docs-Only Completion After 0.0.1-dev

The first completed vNext backlog item after the completed `0.0.1-dev`
prerelease sequence is `P1-01`, the post-release evidence summary template.
It was completed as a docs-only template addition. It improves future release
traceability without processing an Avast response, touching release records,
operating assets, updating packages or `dist/`, running Live E2E, mutating
Google Docs / Drive, performing OAuth operations, rerunning Avast, or
re-running the flagged executable.

The next Avast-independent docs-only candidates are `P1-02`, `P1-04`, and
`P1-06`.

`P0-01` remains the first release-path item, but it is blocked until an Avast
response is received and explicitly reviewed.

## P0 — Next Release-Path Gate Items

| ID | Item | Priority | Rationale | Prerequisite | Release safety impact | State |
| --- | --- | --- | --- | --- | --- | --- |
| P0-01 | Record Avast false positive outcome and owner decision. | P0 | The next release-path basis cannot be re-evaluated until the vendor response is preserved and reviewed. | Avast response received for the selected artifact identity. | Prevents treating pending, ambiguous, mismatched, or adverse vendor evidence as clearance. | Blocked / Avast response pending. |
| P0-02 | Use intake first, then decision classification for any Avast response. | P0 | Keeps raw response capture separate from reviewed clearance, rejection, clarification, or residual-risk decisions. | P0-01 response receipt; `Publisher_AvastResponseIntakeTemplate.md`; `Publisher_AvastResponseDecisionTemplate.md`. | Preserves auditability and avoids accidental vendor-clearance or safety-certification claims. | Blocked / Avast response pending. |
| P0-03 | Confirm the next release-path basis: Avast vendor clearance, continued VMF-side risk acceptance, or a new owner decision. | P0 | Future release-path work needs an explicit basis after the published prerelease and unresolved vendor response. | P0-02 decision classification and owner review. | Prevents implicit carryover from the `0.0.1-dev` prerelease authorization. | Blocked / depends on P0-02. |
| P0-04 | Reconcile release-readiness records after Avast clearance, rejection, or continued pending status is reviewed. | P0 | Readiness records must reflect the reviewed Avast outcome without rewriting historical accepted-at-the-time evidence. | P0-03 owner basis decision. | Keeps current status, evidence, and historical hold/block wording separated. | Blocked / depends on P0-03. |
| P0-05 | Update go/no-go review only after blockers are resolved or explicitly waived by the owner. | P0 | Go/no-go status must follow completed or waived release-blocking dependencies. | P0-04 reconciliation and explicit owner waiver where applicable. | Prevents a planning label from becoming release authorization. | Blocked / depends on P0-04. |
| P0-06 | Confirm Live E2E status for the next release-path scope. | P0 | Live E2E remains a separate operation-specific gate even after `0.0.1-dev` publication. | Explicit authorization, non-applicability decision, or documented deferral. | Avoids unapproved live mutation and separates evidence from release clearance. | Blocked / requires separate authorization decision. |
| P0-07 | Confirm Google Docs / Drive readback or mutation status for the next release-path scope. | P0 | Google Docs / Drive operations remain independently gated and may require credentials or cleanup evidence. | Explicit authorization, non-applicability decision, or documented deferral. | Prevents unauthorized Google mutation, OAuth operation, or token-store side effect. | Blocked / requires separate authorization decision. |
| P0-08 | Re-run release artifact audit only after package-generation scope is explicitly authorized. | P0 | Artifact audit must match the approved package identity and cannot be refreshed from stale or local-only artifacts. | Approved package-generation or package-inspection scope. | Prevents package, `dist`, asset, or release identity drift. | Blocked / package scope not authorized. |

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

| ID | Item | Priority | Rationale | Prerequisite | Release safety impact | State |
| --- | --- | --- | --- | --- | --- | --- |
| P1-01 | Add a compact post-release evidence summary template for future Publisher release candidates. | P1 | This is the first completed docs-only item after `0.0.1-dev`; it improves traceability without depending on Avast. | Current published-prerelease records and final status freeze. | Improves future evidence review while preserving separate release authorization gates. | Complete / docs-only template added. |
| P1-02 | Improve release-readiness checklist cross-links for blocked, deferred, risk-accepted, and local-only evidence. | P1 | Cross-links reduce audit ambiguity between historical hold records and current status. | Current status and existing Phase 4-3 records. | Helps reviewers avoid mistaking historical blocked wording for current prerelease state or vendor clearance. | Complete / docs-only cross-links added. |
| P1-03 | Re-check supply-chain and package evidence only after the approved release artifact exists. | P1 | Evidence refresh must be tied to a specific approved artifact, not local or stale package output. | Future approved release artifact or explicitly authorized static inspection. | Prevents artifact identity drift and package evidence overclaiming. | Blocked / package scope not authorized. |
| P1-04 | Review Live E2E setup documentation for clearer authorization, credential, and cleanup boundaries. | P1 | Documentation can clarify the gate without running Live E2E or touching OAuth state. | Existing Live E2E records and runbook text. | Reduces risk of unapproved live mutation or credential handling. | Ready / Avast-independent docs-only review. |
| P1-05 | Review package verification scripts for clearer output that distinguishes local verification from release clearance. | P1 | Script messaging can make evidence labels harder to misread, but implementation requires a future code task. | Separate implementation authorization and tests. | Would reduce release-clearance overclaim risk when adopted. | Blocked / implementation scope not authorized. |
| P1-06 | Convert durable AV triage lessons into a repeatable release-safety checklist. | P1 | Checklist hardening can proceed without a vendor response if it preserves pending/clearance boundaries. | Existing AV triage notes and redaction rules. | Improves future AV triage consistency without claiming Avast clearance. | Ready / Avast-independent docs-only review. |

## P2 — vNext Enhancements

| ID | Item | Priority | Rationale | Prerequisite | Release safety impact | State |
| --- | --- | --- | --- | --- | --- | --- |
| P2-01 | Evaluate whether Google Picker plus `drive.file` least-privilege routing should be adopted by a future scoped design task. | P2 | Least-privilege routing may reduce future operator risk, but it is not required for current release follow-up. | Future scoped design task. | Potential future credential and Drive-access risk reduction. | Ready for design only / Avast-independent. |
| P2-02 | Evaluate additional Publisher diagnostics that improve troubleshooting without changing published document semantics. | P2 | Diagnostics may improve supportability but must preserve published document behavior. | Future scoped design and test plan. | Potential future observability improvement with no current release gate effect. | Ready for design only / Avast-independent. |
| P2-03 | Evaluate clearer dry-run output for Google Docs publication planning. | P2 | Better dry-run output may reduce operator error in future Google Docs workflows. | Future scoped design and non-live verification plan. | Potential future planning clarity; no current clearance effect. | Ready for design only / Avast-independent. |
| P2-04 | Evaluate richer release-note generation from existing verification records. | P2 | Automation could reduce documentation drift, but it is not required for the next release-path gate. | Future scoped design task and record-format review. | Potential future traceability improvement. | Ready for design only / Avast-independent. |
| P2-05 | Evaluate documentation improvements for OAuth Desktop setup and token-store handling. | P2 | Guidance may reduce authentication confusion, but token-store operations remain prohibited unless separately authorized. | Future docs task with secret-redaction review. | Potential future credential-handling risk reduction. | Ready for docs-only design / Avast-independent. |
| P2-06 | Evaluate whether vNext should include more explicit managed-document readback reporting. | P2 | Readback reporting may improve operator confidence but needs scoped design. | Future design task and non-live test plan. | Potential future verification clarity; no current release gate effect. | Ready for design only / Avast-independent. |

## Dependency Split

Avast-response dependent:

- P0-01 through P0-05.

Authorization or artifact-scope dependent, but not dependent on an Avast
response:

- P0-06, P0-07, P0-08, P1-03, and P1-05.

Avast-independent docs-only / design-ready:

- P1-02, P1-04, P1-06, and P2-01 through P2-06.

Completed Avast-independent docs-only:

- P1-01.

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
7. Next docs-only hardening candidates: P1-02, P1-04, and P1-06.
8. vNext enhancement candidates.
