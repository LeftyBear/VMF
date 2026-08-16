# Publisher vNext Backlog

## Status

- Current state: Phase 4 local-only verification complete.
- Release state: `0.0.1-dev` GitHub prerelease published after ADR-0019
  VMF-side residual risk acceptance; release completion is formally recorded
  for the existing canonical prerelease, and post-release closeout is complete.
- Next boundary: after closeout, the next version / next phase starts as a new
  scope rather than an extension of the `0.0.1-dev` release execution.
- Any new release-path work remains separately gated.
- External dependency: Avast false positive response pending; vendor clearance
  has not been obtained and Avast safety certification is not claimed.
- Owner re-evaluation on 2026-08-14 records that Avast response receipt is not
  a blocking condition for normal development continuation. This does not grant
  Avast vendor clearance, Avast safety certification, release authorization, or
  any future public/general release approval.
- This backlog is documentation-only and does not authorize release, publication, Live E2E, package generation, or Google Docs/Drive mutation.
- Live E2E has not been executed for this backlog.
- Google Docs / Drive mutation has not been executed for this backlog.
- Package and `dist/` artifacts have not been updated for this backlog.
- No new release, tag, or publication has been performed for this backlog.
- The flagged executable has not been re-run for this backlog.
- P1-05 was implemented as a new independent scope after `0.0.1-dev`
  closeout; it did not reopen the existing prerelease.

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

The next normal-development candidates may proceed under their own scoped tasks
when they preserve local-only, docs-only, source, test, package, release,
Google, OAuth, Avast, flagged-executable, public API, persisted schema, and
Frozen specification boundaries.

`P0-01` remains the first Avast-response intake item, but it is no longer a
blocker for normal development continuation. It remains relevant to future
vendor-clearance wording and future public/general release gate review if an
Avast response is received.

## P0 — Next Release-Path Gate Items

| ID | Item | Priority | Rationale | Prerequisite | Release safety impact | State |
| --- | --- | --- | --- | --- | --- | --- |
| P0-01 | Record Avast false positive outcome and owner decision if a response is received. | P0 | The response, if received, must be preserved and reviewed before it changes vendor-clearance wording or future release/security gate evidence. | Avast response received for the selected artifact identity. | Prevents treating pending, ambiguous, mismatched, or adverse vendor evidence as clearance. | Pending / not blocking normal development continuation. |
| P0-02 | Use intake first, then decision classification for any Avast response. | P0 | Keeps raw response capture separate from reviewed clearance, rejection, clarification, or residual-risk decisions. | P0-01 response receipt; `Publisher_AvastResponseIntakeTemplate.md`; `Publisher_AvastResponseDecisionTemplate.md`. | Preserves auditability and avoids accidental vendor-clearance or safety-certification claims. | Pending / only if response received. |
| P0-03 | Confirm the next future public/general release-path basis: Avast vendor clearance, continued VMF-side risk acceptance, or a new owner decision. | P0 | Future public/general release work needs a fresh artifact-specific basis and current security review. | Future public/general release scope or P0-02 decision classification and owner review. | Prevents implicit carryover from the `0.0.1-dev` prerelease authorization or normal-development continuation. | Blocked for future release authorization / not blocking normal development continuation. |
| P0-04 | Reconcile release-readiness records after Avast clearance, rejection, continued pending status, or future release scope is reviewed. | P0 | Readiness records must reflect the reviewed Avast outcome or fresh release/security gate without rewriting historical accepted-at-the-time evidence. | P0-03 owner basis decision for a future public/general release. | Keeps current status, evidence, and historical hold/block wording separated. | Blocked for future release authorization / not blocking normal development continuation. |
| P0-05 | Update go/no-go review only after future release blockers are resolved or explicitly waived by the owner. | P0 | Go/no-go status must follow completed or waived release-blocking dependencies for the selected future artifact. | P0-04 reconciliation and explicit owner waiver where applicable. | Prevents a planning label or normal-development decision from becoming release authorization. | Blocked for future release authorization / not blocking normal development continuation. |
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
| PF-06 | Post-release closeout starts | Confirm closeout records, current-state consistency, and the next-version / next-phase boundary before selecting new work. | `Publisher_PostReleaseCloseoutRecord_2026-08-13.md`; current status / backlog alignment records. | Docs-only closeout; not additional `0.0.1-dev` release execution. |
| PF-07 | vNext planning resumes | Choose from the P0/P1/P2 candidates below under a new scoped task; keep candidate labels separate from adopted behavior and release authorization. | Backlog update or scoped design record. | Docs-only unless a future implementation task is approved. |

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
| P1-04 | Review Live E2E setup documentation for clearer authorization, credential, and cleanup boundaries. | P1 | Documentation clarifies the gate without running Live E2E or touching OAuth state. | Existing Live E2E records and runbook text. | Reduces risk of unapproved live mutation or credential handling. | Complete / docs-only boundary and cross-link update. |
| P1-05 | Review package verification scripts for clearer output that distinguishes local verification from release clearance. | P1 | Script messaging can make evidence labels harder to misread. | New independent implementation scope after `0.0.1-dev` closeout; temporary-package script verification. | Reduces release-clearance overclaim risk without granting publication, vendor clearance, or release authorization. | Complete / script output boundary added. |
| P1-06 | Convert durable AV triage lessons into a repeatable release-safety checklist. | P1 | Checklist hardening can proceed without a vendor response if it preserves pending/clearance boundaries. | Existing AV triage notes and redaction rules. | Improves future AV triage consistency without claiming Avast clearance. | Complete / docs-only AV triage hardening added. |

## P2 — vNext Enhancements

| ID | Item | Priority | Rationale | Prerequisite | Release safety impact | State |
| --- | --- | --- | --- | --- | --- | --- |
| P2-01 | Evaluate whether Google Picker plus `drive.file` least-privilege routing should be adopted by a future scoped design task. | P2 | Least-privilege routing may reduce future operator risk, but it is not required for current release follow-up. | `Publisher_P2-01_GooglePickerDriveFileEvaluation.md`; `Publisher_P2-01_OAuthDesktopScopeBoundaryEvaluation.md`; `Publisher_P2-01_LeastPrivilegeDesignReevaluation_2026-08-15.md`; `Publisher_P2-27_GooglePickerDriveFileSplitRouteDesign.md`; future adoption record before implementation. | Potential future credential and Drive-access risk reduction. | Complete / split-route design complete by P2-27; implementation remains NO-GO until adoption record and explicit authorization. |
| P2-02 | Evaluate additional Publisher diagnostics that improve troubleshooting without changing published document semantics. | P2 | Diagnostics may improve supportability but must preserve published document behavior. | P2-02-A / P2-02-B narrow local-only implementation and closeout record; follow-on P2-09, P2-10, P2-14, P2-16, P2-24, and P2-25 completions. | Improves local troubleshooting with no release gate, publication, Google, OAuth, package, or vendor-clearance effect. | Complete / A, B, C, E, delivery-state diagnostics, HTTP-status diagnostics, and the narrow retry subset of D implemented. |
| P2-03 | Evaluate clearer dry-run output for Google Docs publication planning. | P2 | Better dry-run output may reduce operator error in future Google Docs workflows. | `Publisher_P2-03_ClearerDryRunOutputEvaluation.md`; P2-03-A / P2-03-B narrow local-only implementation and closeout record; P2-13 failure-boundary implementation; P2-18 contract shape decision; P2-03-C implementation commit `6fb29bb`; P2-23 separate-command evaluation; P2-26 separate-command design. | Potential future planning clarity; no current clearance effect. | Complete / A, B, C, and D implemented; E design complete by P2-26; separate-command implementation remains NO-GO. |
| P2-04 | Evaluate richer release-note generation from existing verification records. | P2 | Automation could reduce documentation drift, but it is not required for the next release-path gate. | `Publisher_P2-04_ReleaseNoteGenerationEvaluation.md`; P2-08 drift checker; P2-12 verification evidence extractor; P2-17 CHANGELOG draft helper. | Potential future traceability improvement. | Complete / A, B, C, D, and E implemented through follow-on scoped tasks. |
| P2-05 | Evaluate documentation improvements for OAuth Desktop setup and token-store handling. | P2 | Guidance may reduce authentication confusion, but token-store operations remain prohibited unless separately authorized. | `Publisher_P2-05_OAuthDesktopTokenStoreDocumentationEvaluation.md`; docs-only implementation in installation and Live E2E guidance. | Reduces future credential-handling ambiguity without changing OAuth scope, authentication architecture, Google, package, release, or vendor-clearance gates. | Complete / docs-only guidance synchronized. |
| P2-06 | Evaluate whether vNext should include more explicit managed-document readback reporting. | P2 | Readback reporting may improve operator confidence but needs scoped design. | `Publisher_P2-06_ManagedDocumentReadbackReportingEvaluation.md`; P2-07 narrow local-only implementation closeout. | Potential future verification clarity; no current release gate, publication, Google, OAuth, package, or vendor-clearance effect. | Design complete / implementation decision closed by P2-07. |
| P2-07 | Implement narrow managed-document readback reporting from the P2-06 design evaluation. | P2 | P2-06 found that readback state is safety-critical but under-reported in operator-facing diagnostics; a narrow implementation can improve clarity without changing readback semantics. | `Publisher_P2-06_ManagedDocumentReadbackReportingEvaluation.md`; explicit implementation task authorization; focused unit tests before broader verification. | Improves future verification clarity while preserving separation from publication success, release clearance, package approval, vendor clearance, and Avast safety certification. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-08 | Implement the release-note drift checker selected after P2-07 closeout. | P2 | Bounded local drift checking reduces release-note ambiguity without rewriting approved release notes or promoting gated decisions. | `Publisher_P2-08_CandidateSelection.md`; P2-04 allow-listed source-field boundary; commit `75be0fc`. | Keeps vNext release-note review local-only while preserving release, Google, OAuth, package, vendor-clearance, and Avast boundaries. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-09 | Implement allow-listed configuration failure summary classification. | P2 | The P2-02 deferred `configurationCategory` field improves local troubleshooting while preserving safe diagnostic boundaries. | Existing stable `CONFIG_*` codes; P2-02 allow-listed category boundary; commit `d7c761d`. | Adds bounded local failure summary context without exposing configuration values or changing release, Google, OAuth, package, or vendor-clearance gates. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-10 | Implement safe retry diagnostics for final failure summaries. | P2 | The P2-02-D deferred retry diagnostics subset improves local troubleshooting while preserving safe diagnostic boundaries. | Existing retry/failure classification; P2-02-D allow-listed retry metadata boundary; commit `871ece5`. | Adds bounded local failure summary context without changing retry behavior, release, Google, OAuth, package, or vendor-clearance gates. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-12 | Implement the P2-04-C verification evidence extractor. | P2 | Normalizing allow-listed verification evidence rows reduces release-note drift without inferring approval or gated operation state. | `Publisher_P2-04_ReleaseNoteGenerationEvaluation.md`; P2-04 allow-listed source-field boundary; commit `f6c7d08`. | Keeps release-note evidence extraction local-only while preserving release, Google, OAuth, package, vendor-clearance, Avast, and `deliveryState` deferred boundaries. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-13 | Implement dry-run failure boundary hints selected after P2-12 closeout. | P2 | Bounded failure-boundary hints can reduce dry-run troubleshooting ambiguity by reusing existing CLI classifications and safe diagnostic routing. | `Publisher_P2-13_CandidateSelection.md`; P2-03-D failure-boundary hint boundary; commit `91d3969`. | Keeps dry-run troubleshooting local-only while preserving dry-run semantics, stdout, exit codes, Google, OAuth, package, release, vendor-clearance, and Avast boundaries. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-14 | Implement maxAttempts retry diagnostics for final failure summaries. | P2 | The remaining low-risk P2-02-D retry budget field improves local troubleshooting while preserving safe diagnostic boundaries. | Existing retry diagnostics boundary; explicit narrow implementation authorization; commit `7df613d`. | Adds bounded local failure summary context without changing retry policy, release, Google, OAuth, package, or vendor-clearance gates. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-16 | Implement support summary diagnostics for final failure summaries. | P2 | The deferred P2-02-E `SUPPORT_SUMMARY` field can improve supportability as a compact reconstruction of existing CLI-safe final summary fields. | Existing CLI final summary fields; explicit narrow implementation authorization; commit `d61fd00`. | Adds bounded local failure summary context without changing release, Google, OAuth, package, or vendor-clearance gates. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-17 | Implement a draft-only CHANGELOG helper derived from P2-04-E. | P2 | A bounded helper reduces manual transcription from release-note draft fields, but `CHANGELOG.md` remains manually curated. | `Publisher_P2-17_CHANGELOGDraftHelperEvaluation.md`; `Publisher_P2-17_CHANGELOGDraftHelperImplementationScope.md`; existing P2-04 allow-listed release-note draft boundary. | Reduces future documentation-drift risk without changing release, package, Google, OAuth, vendor-clearance, or Avast gates. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-18 | Decide the structured dry-run contract shape for deferred P2-03-C. | P2 | A fixed contract shape is required before any machine-readable dry-run implementation can proceed safely. | `Publisher_P2-18_DryRunContractShapeDecision.md`; P2-03-C deferred candidate; ADR-0006 safe diagnostics; ADR-0007 classification boundary; implementation commit `6fb29bb`. | Keeps future dry-run automation local-only while preserving `DRY_RUN_PLAN` compatibility, stdout, exit codes, failure taxonomy, Google, OAuth, package, release, vendor-clearance, and Avast boundaries. | Complete / contract shape fixed and implemented by P2-03-C. |
| P2-19 | Decide whether physical update dry-run should be integrated into existing `dry-run`. | P2 | Existing local dry-run and physical update dry-run carry different evidence meaning and authorization boundaries. | `Publisher_P2-19_PhysicalUpdateDryRunIntegrationDecision.md`; P2-18 dry-run contract decision. | Prevents local-only dry-run evidence from being confused with Google verification, publication authorization, release clearance, or vendor clearance. | Complete / NO-GO for integration into existing dry-run. |
| P2-20 | Decide whether delivery-state diagnostics should be exposed by the CLI now. | P2 | CLI exposure needs a stable Application boundary carrier before public diagnostic output can be defined safely. | `Publisher_P2-20_DeliveryStateDiagnosticsCliDecision.md`; P2-02 deferred `deliveryState` diagnostics; ADR-0005 retry boundary. | Preserves retry classification and safe diagnostics while deferring CLI exposure until the carrier exists. | Complete / NO-GO for CLI exposure before Application carrier. |
| P2-21A | Implement the Application boundary delivery-state carrier. | P2 | P2-20 requires a stable nullable carrier before any future CLI exposure can be reconsidered. | P2-20 CLI NO-GO decision; existing `RequestDeliveryState` enum; implementation commit `bb09ec5`. | Carries existing `NotSent`, `Sent`, and `Unknown` values through physical update results without adding CLI output, new classifications, release, Google, OAuth, package, vendor-clearance, or Avast effects. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-21B | Re-evaluate delivery-state diagnostics CLI exposure after P2-21A. | P2 | P2-21A satisfied the Application carrier precondition, but CLI exposure also requires a safe publish / CLI result bridge. | `Publisher_P2-21B_DeliveryStateCliDiagnosticsEvaluation.md`; P2-20; P2-21A carrier implementation. | Prevents exposing delivery state as a new classification or unsupported CLI field before the value reaches final summaries through a tested boundary. | Complete / direct CLI exposure remains NO-GO until result bridge. |
| P2-21C | Implement the Application publish result to `CliResult` delivery-state carrier bridge. | P2 | P2-21B selected a result bridge before any CLI output exposure can be reconsidered. | `Publisher_P2-21B_DeliveryStateCliDiagnosticsEvaluation.md`; existing `RequestDeliveryState` enum; implementation commit `f6717a1`. | Carries nullable delivery state through `PublishError` and `CliResult` without changing CLI display, JSON schema, final summaries, stdout, exit codes, classifications, release, Google, OAuth, package, vendor-clearance, or Avast gates. | Complete / narrow local-only implementation; CLI exposure remains NO-GO. |
| P2-21D | Re-evaluate delivery-state structured diagnostics exposure after P2-21C. | P2 | P2-21C completed the carrier path through `CliResult`, so the remaining question is whether a bounded final failure summary field is acceptable. | `Publisher_P2-21D_DeliveryStateCliExposureEvaluation.md`; P2-20; P2-21B; P2-21C carrier implementation. | Keeps delivery state value-safe and classification-neutral while preserving separate authorization for any actual CLI output change. | Complete / docs-only evaluation; CLI exposure implementation remains NO-GO until separately authorized. |
| P2-22 | Evaluate HTTP status structured diagnostics exposure. | P2 | `httpStatus` remains the P2-02 deferred transport diagnostic after retry, support summary, readback, and delivery-state carrier work. | `Publisher_P2-22_HttpStatusCliExposureEvaluation.md`; P2-02 deferred `httpStatus`; P2-21D diagnostic-size boundary. | Keeps any future HTTP status field final-failure-only, sanitized, classification-neutral, and separate from Google / OAuth provider payloads, release, package, vendor-clearance, and Avast gates. | Complete / docs-only evaluation; CLI exposure implementation remains NO-GO until separately authorized. |
| P2-23 | Evaluate P2-03-E physical update dry-run as a separate command. | P2 | P2-19 made integration into existing `dry-run` NO-GO, so any remaining physical update dry-run path must first prove a separate command, contract, evidence category, and authorization boundary. | `Publisher_P2-23_PhysicalUpdateDryRunSeparateCommandEvaluation.md`; P2-03-E deferred candidate; P2-19 existing `dry-run` integration NO-GO decision; ADR-0004 / ADR-0006 / ADR-0007. | Prevents physical update preview evidence from being confused with local Markdown dry-run, Google verification, publication authorization, release clearance, package approval, vendor clearance, or Avast safety certification. | Complete / docs-only evaluation; separate-command implementation remains NO-GO until separately authorized. |
| P2-24 | Implement `deliveryState` final failure summary diagnostics. | P2 | P2-21D established the safe final-summary boundary after the `CliResult` carrier path was completed. | `Publisher_P2-21D_DeliveryStateCliExposureEvaluation.md`; existing `CliResult.DeliveryState`; explicit narrow implementation authorization. | Adds bounded local failure summary context without changing retry behavior, classification, stdout, exit codes, `httpStatus`, release, Google, OAuth, package, or vendor-clearance gates. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-25 | Implement bounded `httpStatus` final failure summary diagnostics. | P2 | P2-22 established that a future sanitized HTTP status field may be acceptable, and P2-24 completed the adjacent final-summary diagnostic pattern while preserving `httpStatus` omission. | `Publisher_P2-25_CandidateSelection.md`; `Publisher_P2-22_HttpStatusCliExposureEvaluation.md`; explicit narrow implementation authorization. | Adds bounded local failure summary context only if the status is safely carried to the CLI boundary, without exposing provider payloads, changing classification, retry behavior, stdout, exit codes, release, Google, OAuth, package, or vendor-clearance gates. | Complete / narrow local-only implementation with focused unit coverage. |
| P2-26 | Design the P2-03-E physical update dry-run separate command. | P2 | P2-23 confirmed GO for a future separate-command design while keeping implementation NO-GO. The design must fix command name, contract shape, evidence category, and authorization boundary before any implementation can be considered. | `Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md`; `Publisher_P2-23_PhysicalUpdateDryRunSeparateCommandEvaluation.md`; P2-19 existing `dry-run` integration NO-GO decision; ADR-0004 / ADR-0006 / ADR-0007. | Prevents future physical update preview work from being confused with local Markdown dry-run, Google mutation, readback verification, publication authorization, release clearance, package approval, vendor clearance, or Avast safety certification. | Complete / docs-only design; separate-command implementation remains NO-GO until separately authorized. |
| P2-28 | Select the next candidate after P2-27 and fix the `preview-update` implementation-scope boundary. | P2 | P2-27 leaves Route B adoption too broad for the next local step, while P2-26 has already fixed the separate-command design for P2-03-E. Candidate selection should compare remaining P2 routes and choose one next bounded task. | `Publisher_P2-28_CandidateSelection.md`; `Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md`; `Publisher_P2-27_GooglePickerDriveFileSplitRouteDesign.md`; explicit later implementation authorization. | Preserves release, package, Google, OAuth, Live E2E, vendor-clearance, Avast, flagged-executable, Frozen spec, public API, persisted schema, existing `dry-run`, stdout, exit-code, and classification boundaries while preparing the next local-only scope record. | Complete / docs-only candidate selection; implementation remains NO-GO until separately authorized. |
| P2-29 | Plan the first `preview-update` implementation slice selected by P2-28. | P2 | P2-28 selected the P2-26-derived `preview-update` route as the next bounded candidate, but implementation still needs a concrete local-only boundary, candidate change areas, safe-stop requirements, and focused test plan before any GO decision can be considered. | `Publisher_P2-29_PreviewUpdateImplementationScopePlanning.md`; `Publisher_P2-28_CandidateSelection.md`; `Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md`; explicit later implementation authorization. | Preserves release, package, Google, OAuth, Live E2E, vendor-clearance, Avast, flagged-executable, Frozen spec, public API, persisted schema, existing `dry-run`, stdout, exit-code, and classification boundaries while defining the first local-only implementation slice. | Complete / docs-only implementation-scope planning; implementation remains NO-GO until separately authorized. |
| P2-30 | Select the GO / CONDITIONAL GO / NO-GO basis for the P2-29 `preview-update` implementation scope. | P2 | P2-29 fixed the first local-only implementation slice, but implementation should not start until start conditions, stop conditions, technical dependencies, local-only verification limits, and external Google / OAuth / Live E2E gates are separated. | `Publisher_P2-30_CandidateSelection.md`; `Publisher_P2-29_PreviewUpdateImplementationScopePlanning.md`; `Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md`; explicit later implementation authorization. | Records CONDITIONAL GO only for a future separately authorized local-only implementation task after local Verified State and snapshot input shapes are fixed, while preserving release, package, Google, OAuth, Live E2E, vendor-clearance, Avast, flagged-executable, Frozen spec, public API, persisted schema, existing `dry-run`, stdout, exit-code, and classification boundaries. | Complete / docs-only candidate selection; implementation remains NO-GO until separately authorized. |

### P2-07 Managed-Document Readback Reporting Implementation

P2-07 is the completed narrow local-only implementation derived from
`Publisher_P2-06_ManagedDocumentReadbackReportingEvaluation.md`. It adds
value-safe readback status reporting to existing local structured diagnostics
and operator-facing managed-document update summaries with focused unit
coverage completed in commit `5e4b03f`.

This backlog synchronization is documentation-only. It does not implement new
`src/` or `tests/` changes, does not synchronize the Voyage Log, `CHANGELOG.md`,
release records, packages, `dist/`, tags, or external services, and does not
broaden the completed implementation scope.

Completed implementation scope:

- add compact readback status reporting to existing local structured
  diagnostics and existing operator-facing managed-document update summaries;
- use only the closed readback status vocabulary listed below;
- include value-safe lifecycle phase labels that distinguish pre-apply read,
  physical apply, post-apply readback, verification, promotion, and Verified
  State save;
- preserve existing stable error codes, CLI classification, exit-code
  behavior, Physical Update Plan ordering, and Verified State promotion/save
  semantics;
- add focused unit tests for the required cases listed below.

Non-goals:

- no actual Google readback, Google Docs mutation, Google Drive mutation,
  OAuth login, token-store read/write/delete/cleanup/reuse, Live E2E, package
  or `dist` update, release, tag, publication, GitHub asset operation, Avast
  operation, flagged executable re-run, vendor-clearance judgment, stage,
  commit, or push;
- no Frozen specification, public API, persisted schema, OAuth scope,
  authentication architecture, release-record, package-identity, or
  publication-flow change;
- no change that reports failure, mismatch, unknown delivery, skipped
  readback, dry-run, or non-applicable states as success;
- no richer mismatch diagnostics unless they remain bounded, synthetic-data
  tested, and value-safe.

Closed readback status vocabulary:

- `verified`
- `failed`
- `mismatch`
- `revision-conflict`
- `not-attempted`
- `not-applicable`
- `blocked`

Safe-value boundary:

- allowed values are bounded status labels, stable error codes, existing CLI
  classifications, boolean boundary fields, non-content counts, and lifecycle
  phase labels;
- prohibited values include raw document content, block text, document IDs,
  private Google resource IDs, private URLs, OAuth tokens, credentials,
  credential paths, token-store paths, Authorization headers, provider
  payloads, raw HTTP bodies, raw exception messages, stack traces, local
  sensitive paths, usernames, hostnames, and account identifiers;
- readback reporting remains evidence of managed-document verification only
  and must not be represented as publication approval, release clearance,
  package approval, vendor clearance, Avast safety certification, or future
  operation authorization.

Completed focused unit-test coverage:

- verified readback reports `verified` and cannot promote/save Verified State
  before successful readback verification;
- readback acquisition failure reports `failed` with the existing stable error
  classification;
- readback mismatch reports `mismatch` without exposing content or provider
  identifiers;
- revision conflict reports `revision-conflict` and remains a hard stop;
- no-change / empty-plan paths report either verified readback completion or
  `not-applicable` only where readback is genuinely not applicable;
- dry-run / local-only paths report `not-attempted` and do not imply Google
  mutation, publication success, release clearance, or vendor clearance;
- blocked preconditions report `blocked` without weakening existing failure
  behavior;
- sensitive-value exclusion tests cover document IDs, private URLs, OAuth
  tokens, Authorization headers, credential paths, token-store paths, provider
  payloads, raw exception details, stack traces, and local sensitive paths.

Implementation closeout after current-state synchronization review: COMPLETE
for the narrow local-only implementation and focused unit coverage. The NO-GO
boundary remains active for any follow-up that would require changing readback
semantics, weakening Verified State promotion requirements, changing public
contracts or persisted schemas, adding dependencies, exposing sensitive
document/provider values, running live or external operations, or treating
readback evidence as release or vendor clearance.

### P2-09 Configuration Failure Summary Classification

P2-09 is the completed narrow local-only implementation of the P2-02 deferred
`configurationCategory` summary field. Commit `d7c761d` adds the field only to
structured command-summary diagnostics whose classification is
`Configuration`.

Completed implementation scope:

- derive the summary value only from existing stable `CONFIG_*` error codes;
- emit only allow-listed category labels: `cli`, `googleApi`, `publisher`, and
  `unknown`;
- map unknown future `CONFIG_*` codes to `unknown` instead of exposing the raw
  configuration source;
- omit `configurationCategory` from non-configuration failures and successful
  summaries;
- add focused `CliApplicationTests` coverage for allow-list mapping,
  non-configuration omission, and configuration-summary-only emission.

Safe-value boundary:

- allowed values are bounded category labels and existing stable error-code
  routing inputs;
- prohibited values include raw configuration values, credentials, credential
  paths, token-store paths, OAuth tokens, Authorization headers, private Google
  resource IDs, private URLs, provider payloads, raw HTTP bodies, raw
  exception messages, stack traces, local sensitive paths, usernames,
  hostnames, and account identifiers;
- configuration failure summary classification remains local troubleshooting
  evidence only and must not be represented as release approval, package
  approval, publication approval, vendor clearance, Avast safety certification,
  or future operation authorization.

Non-goals:

- no retry/delivery metadata, `SUPPORT_SUMMARY`, richer diagnostic payload, new
  configuration source, public API change, persisted schema change, OAuth scope
  change, authentication architecture change, dependency addition, release
  record change, package identity change, or publication-flow change;
- no release, tag, publication, package or `dist` update, GitHub asset
  operation, Live E2E, Google Docs mutation, Google Drive mutation, OAuth login,
  token-store read/write/delete/cleanup/reuse, Avast operation, vendor
  clearance judgment, flagged executable re-run, stage, commit, or push.

Verification result recorded for P2-09: focused `CliApplicationTests` coverage
was added with the implementation in commit `d7c761d`. Current-state
synchronization is documentation-only and records no new source or test change.

### P2-10 Safe Retry Diagnostics

P2-10 is the completed narrow local-only implementation of the P2-02-D safe
retry diagnostics subset. Commit `871ece5` adds only `attemptCount` and
`retryable` to structured stderr final failure summaries when retry diagnostics
are safely known. P2-14 subsequently completed the narrow `maxAttempts`
diagnostic in commit `7df613d`.

Completed implementation scope:

- emit `attemptCount` as numeric allow-listed metadata;
- emit `retryable` as boolean allow-listed metadata;
- emit both fields only on final failure summaries when retry diagnostics are
  safely known;
- omit both fields from unknown retry diagnostics and success summaries;
- preserve classification, exit-code behavior, stdout compatibility, Frozen
  specifications, public APIs, and persisted schemas.

Subsequent P2-14 completed implementation scope:

- emit `maxAttempts` as numeric allow-listed metadata;
- emit the field only on final failure summaries when retry diagnostics are
  safely known;
- omit the field from success summaries, non-retry failures, and unknown retry
  diagnostics;
- preserve classification, exit-code behavior, stdout compatibility, retry
  behavior, Frozen specifications, public APIs, and persisted schemas.

Deferred scope:

- `deliveryState`;
- `httpStatus`.

Safe-value boundary:

- allowed values are bounded numeric and boolean retry diagnostic metadata;
- prohibited values include raw exception messages, stack traces, raw HTTP
  bodies, provider payloads, local paths, raw URLs, private URLs, credentials,
  token-store paths, OAuth tokens, cookies, Authorization headers, usernames,
  hostnames, and account identifiers;
- safe retry diagnostics remain local troubleshooting evidence only and must
  not be represented as release approval, package approval, publication
  approval, vendor clearance, Avast safety certification, or future operation
  authorization.

Non-goals:

- no `deliveryState`, `httpStatus`, richer diagnostic
  payload, public API change, persisted schema change, OAuth scope change,
  authentication architecture change, dependency addition, release record
  change, package identity change, publication-flow change, retry-policy
  behavior change, classification change, exit-code change, or stdout change;
- no release, tag, publication, package or `dist` update, GitHub asset
  operation, Live E2E, Google Docs mutation, Google Drive mutation, OAuth login,
  token-store read/write/delete/cleanup/reuse, Avast operation, vendor
  clearance judgment, or flagged executable re-run.

Verification result recorded for P2-10: focused `CliApplicationTests` coverage
passed 63 / 0 / 0, full Publisher unit coverage passed 553 / 0 / 0, Release
build passed with warnings 0 / errors 0, format passed, and `git diff --check`
passed. Current-state synchronization is documentation-only and records no new
source or test change.

Verification result recorded for P2-14: focused `CliApplicationTests` coverage
passed 72 / 0 / 0, full Publisher unit coverage passed 568 / 0 / 0, Release
build passed with warnings 0 / errors 0, format passed, and `git diff --check`
passed. Commit `7df613d` is pushed to `origin/main`. Current-state
synchronization is documentation-only and records no new source or test change.

### P2-16 Support Summary Diagnostics

P2-16 is the completed narrow local-only implementation of the P2-02-E
`SUPPORT_SUMMARY` support summary field. Commit `d61fd00` adds the nested
field only to structured stderr final failure summaries by reconstructing
already available CLI-safe final summary fields.

Completed implementation scope:

- emit `SUPPORT_SUMMARY` only on final failure summaries;
- omit `SUPPORT_SUMMARY` from success summaries;
- reuse existing CLI-safe fields only, including result code, classification,
  exit code, command, phase, operation, safe message, adopted P2-02 fields,
  retry diagnostics when already known, failure boundary when already known,
  and readback safe status fields;
- preserve classification, exit-code behavior, stdout compatibility, retry
  behavior, Frozen specifications, Application and Domain behavior, public
  APIs, interfaces, and persisted schemas.

Deferred scope:

- `deliveryState`;
- `httpStatus`.

Safe-value boundary:

- allowed values are existing bounded CLI-safe final summary values only;
- prohibited values include raw exception messages, stack traces, raw HTTP
  bodies, provider payloads, local paths, raw URLs, private URLs, credentials,
  token-store paths, OAuth tokens, cookies, Authorization headers, usernames,
  hostnames, and account identifiers;
- support summary diagnostics remain local troubleshooting evidence only and
  must not be represented as release approval, package approval, publication
  approval, vendor clearance, Avast safety certification, or future operation
  authorization.

Non-goals:

- no `deliveryState`, `httpStatus`, richer diagnostic payload, public API
  change, interface change, persisted schema change, OAuth scope change,
  authentication architecture change, dependency addition, release record
  change, package identity change, publication-flow change, retry-policy
  behavior change, classification change, exit-code change, stdout change, or
  Application / Domain behavior change;
- no release, tag, publication, package or `dist` update, GitHub asset
  operation, Live E2E, Google Docs mutation, Google Drive mutation, OAuth login,
  token-store read/write/delete/cleanup/reuse, Avast operation, vendor
  clearance judgment, or flagged executable re-run.

Verification result recorded for P2-16: focused `CliApplicationTests` coverage
passed 72 / 0 / 0, full Publisher unit coverage passed 568 / 0 / 0, Release
build passed with warnings 0 / errors 0, format passed, `git diff --check`
passed, commit `d61fd00` was created and pushed, and
`HEAD == origin/main == d61fd00`. This current-state synchronization is
documentation-only and records no new source or test change.

### P2-12 Verification Evidence Extractor

P2-12 is the completed narrow local-only implementation of the P2-04-C
verification evidence extractor. Commit `f6c7d08` adds an internal extractor
that normalizes verification evidence rows from allow-listed current-state
Markdown tables only.

Completed implementation scope:

- read only explicitly allow-listed source records supplied to the extractor;
- accept only current-state records and reject historical or template records;
- normalize verification table columns for command, result, warnings, errors,
  passed, failed, skipped, and source path;
- exclude sensitive command or evidence values before returning rows;
- treat conflicting rows for the same command as blocking drift using the full
  result/count fingerprint;
- keep the implementation internal to the Publisher application test surface.

Safe-value and non-inference boundary:

- allowed values are checked-in Markdown table cells that pass the allow-list,
  current-state, and sensitive-value filters;
- prohibited values include local absolute paths, tokens, secrets,
  Authorization headers, localhost/private URLs, raw provider payloads, raw
  exception details, stacks, credentials, token-store paths, and external
  resource identifiers;
- extracted verification rows are draft evidence inputs only and must not be
  represented as release approval, release authorization, publication
  authorization, risk acceptance, vendor clearance, Avast safety
  certification, Live E2E authorization, Google Docs / Drive authorization,
  OAuth/token-store authorization, package approval, or current state inferred
  from historical records.

Non-goals:

- no release-note rewrite, `CHANGELOG.md` edit, package or `dist` update,
  release, tag, publication, GitHub asset operation, Live E2E, Google Docs
  mutation, Google Drive mutation, OAuth login, token-store read/write/delete,
  Avast operation, vendor-clearance judgment, flagged executable re-run, or
  external lookup;
- no stdout, exit-code, CLI classification, Frozen specification, public API,
  persisted schema, retry behavior, `deliveryState`,
  OAuth scope, authentication architecture, or release-record change.

Verification result recorded for P2-12: focused ReleaseNote unit coverage
passed 32 / 0 / 0, format passed, `git diff --check` passed, commit `f6c7d08`
was created and pushed, and `HEAD == origin/main == f6c7d08`. This
current-state synchronization is documentation-only and records no new source
or test change.

## Dependency Split

Avast-response dependent for vendor-clearance wording, but not blocking normal
development continuation:

- P0-01 through P0-05.

Authorization or artifact-scope dependent, but not dependent on an Avast
response:

- P0-06, P0-07, and P0-08.
- P1-03.

Avast-independent docs-only / design-ready:

- P2-02 through P2-06.
- P2-17.

Completed Avast-independent hardening / enhancements:

- P1-01.
- P1-02.
- P1-04.
- P1-05.
- P1-06.
- P2-01 / P2-27 split-route design.
- P2-07.
- P2-08.
- P2-09.
- P2-10.
- P2-12.
- P2-13.
- P2-14.
- P2-16.
- P2-20.
- P2-21A.
- P2-21B.
- P2-21C.
- P2-21D.
- P2-22.
- P2-23.
- P2-24.
- P2-25.
- P2-26.
- P2-27.
- P2-28.
- P2-29.
- P2-30.
- P2-31.

Latest completed Avast-independent enhancement candidate:

- P2-31: authorization/readiness planning complete as a docs-only / local-only
  record. It closes the P2-30 implementation-start conditions and records GO
  for a separately executed first narrow local-only `preview-update`
  implementation slice. P2-31 itself performs no implementation.

## Vendor-Clearance Boundary

The Avast false positive response remains unavailable, vendor clearance has not
been obtained, and Avast safety certification is not claimed. The
2026-08-14 owner re-evaluation removes Avast response receipt as a blocking
condition for normal development continuation only.

The following work remains separately gated:

- Treating Avast vendor clearance as complete.
- Claiming Avast safety certification.
- Performing a future public/general release publication.
- Creating or updating release tags for future public/general release work.
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
the ADR-0019 VMF risk acceptance basis, the unresolved Avast vendor-clearance
boundary, and the 2026-08-14 normal-development continuation decision.

## Recommended Next Review Order

1. Confirm the scoped task is normal development or future public/general
   release work.
2. For normal development, apply the 2026-08-14 owner re-evaluation and keep
   release, package, Google, OAuth, Avast, and flagged-executable operations
   out of scope unless separately authorized.
3. If an Avast response arrives, record the response and any owner follow-up
   decision before changing vendor-clearance wording.
4. For future public/general release, establish the release-path basis from the
   selected artifact identity and security state current at that time: Avast
   clearance, continued VMF-side risk acceptance, or new owner decision.
5. Current release-readiness checklist state and go/no-go review status.
6. Live E2E and Google Docs / Drive authorization status for the next scoped
   release-path task.
7. Release artifact audit status after package-generation scope is authorized.
8. Security and supply-chain review status for the approved release artifact.
9. vNext enhancement candidates.
