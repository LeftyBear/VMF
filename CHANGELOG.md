# Changelog

All notable Build.xlam, VMF, documentation, release artifact, tooling, and test changes are recorded in this file.

---

# 2026-08-06

## Added

- Added
  `docs/architecture/ADR-0017-release-retention-archival-audit-trail.md`
  to record release retention, archival, and audit trail responsibilities,
  preserving the boundary that immutable audit evidence is documentation and
  evidence preservation, not release authorization, publication approval,
  vendor clearance, Avast resolution, or production readiness.

- Added
  `docs/architecture/ADR-0015-release-withdrawal-rollback-record-and-incident-evidence-boundary.md`
  to record the future Release Withdrawal Record, Rollback Record, and
  Incident Evidence Bundle boundaries, preserving separation from the Release
  Evidence Bundle, Release Approval Package, Release Authorization, Release
  Decision Record, Publication Record, and permission to republish.

- Added
  `docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md`
  to record the future Release Publication Record responsibility and
  Post-Release Evidence boundary, preserving separation between the Evidence
  Bundle, Release Approval Package, Release Authorization, Release Decision
  Record, Publication Record, and post-release observation or audit evidence.

- Added
  `docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md`
  to record the future Release Decision Record requirement and
  post-authorization traceability boundary, preserving separation between the
  release approval / authorization decision, actual release work, release
  artifact, and publication record.

- Added
  `docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md`
  to record the post-clearance release resumption procedure and final
  verification order, preserving the rule that vendor clearance alone, Avast
  response alone, and the Release Approval Package alone are not release
  authorization.

- Added
  `docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md`
  to record that release authorization must be a separate
  release-governance record, not an ADR, and that Accepted ADRs do not imply
  release approval, production readiness, vendor clearance, or authorization
  to publish, tag, package, distribute, run Live E2E, mutate Google Docs /
  Drive, or re-run flagged executables.
- Added
  `docs/architecture/ADR-0010-vnext-backlog-and-deferred-scope-boundary.md`
  to record Publisher vNext backlog classification and deferred scope as a
  docs-only / local-only planning boundary, not vNext feature adoption, v1.0
  release authorization, vendor clearance, Avast resolution, Live E2E
  authorization, Google Docs / Drive mutation approval, package approval, tag
  approval, or publication approval.

## Changed

- Updated the ADR index to track ADR-0017 and require archived evidence to
  preserve traceability from release decision to verification, vendor
  clearance, release approval evidence, and package/release identifiers when
  those records exist and are authorized to be recorded.

- Updated the ADR index to track ADR-0015 and require future withdrawal or
  rollback records to identify trigger, affected artifact or publication
  target, detection timestamp, containment action, rollback or withdrawal
  action, verification performed, evidence references, residual risk,
  follow-up owner, and final status.

- Updated the ADR index to track ADR-0014 and keep post-release evidence from
  retroactively satisfying pre-release approval, release authorization,
  required release gates, vendor clearance, Avast false-positive resolution,
  final release verification, or Release Decision Record completeness.

- Updated the ADR index to track ADR-0013 and require future Release Decision
  Records to link decision date/time, decision owner / authorizer, authorized
  release scope, evidence bundle reference, final verification reference,
  vendor clearance / Avast resolution reference, explicit authorization
  outcome, accepted residual risk, and the next allowed operation boundary.

- Updated the ADR index to track ADR-0012 and keep release resumption
  ordered behind vendor clearance, Avast disposition review, evidence
  validation, blocker review, approved local verification, Release Approval
  Package preparation, explicit release authorization, and final release
  verification success.

- Updated the ADR index to track ADR-0011 and keep ADR acceptance separate
  from release authorization, while preserving ADR-0003 as the release gate,
  ADR-0008 as the operational preflight hard stop, and ADR-0009 as the
  evidence and Release Approval Package review boundary.
- Updated the ADR index to track ADR-0010 and keep P0, P1, P2, Blocked, and
  Deferred backlog labels as planning, triage, and sequencing classifications
  rather than implementation approval or release authorization.
- Recorded that Google Picker plus `drive.file` remains a vNext
  reconsideration candidate and is not an adopted design decision for the
  current v1.0 release boundary.
- Maintained the existing release boundary: Avast false-positive handling
  remains pending, vendor clearance has not been obtained, release remains
  blocked, the current Release Approval Package recommendation remains Hold,
  ADR-0017 does not create archive artifacts and no archive entry may imply
  release approval, publication, production readiness, vendor clearance, or
  Avast resolution while the gate remains blocked,
  ADR-0012 does not authorize release resumption, ADR-0013 does not create a
  Release Decision Record, ADR-0014 does not create a Publication Record or
  Post-Release Evidence, ADR-0015 does not create a Withdrawal Record,
  Rollback Record, or Incident Evidence Bundle, and any ambiguity, mismatch,
  missing evidence, or failed final verification returns the state to Hold,
  and no Live E2E, Google
  Docs or Drive mutation, package or distribution artifact update, release,
  tag, publication, republication, flagged executable re-run, production code
  change, test change, Frozen specification change, or public API change was
  performed.

---

# 2026-08-05

## Added

- Added
  `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
  to record the Publisher evidence bundle as a design, collection,
  validation, and redaction boundary; the release approval package as a review
  record with `Approval Recommendation = Hold`; and the default `Hold
  continues` decision when no Avast response has been received.
- Added
  `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
  to record Publisher preflight hard stops, Avast-pending prohibited
  operations, local-only allowed work, release-resume conditions, and the
  separation between local-only verification and release authorization.
- Added `docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md`
  to record Verified State as the trusted baseline for Publisher differential
  update safety, revision-conflict abort behavior, safe physical update
  ordering, mandatory readback verification, and post-verification-only
  atomic state promotion.
- Added `docs/architecture/ADR-0005-retry-policy-and-failure-classification.md`
  to record Publisher retryable and non-retryable failure classification,
  transient handling, exit-code mapping, idempotency-limited retry, bounded
  backoff, safe message policy, and non-retry behavior for revision conflicts,
  verification failures, configuration errors, unknown or blank stable codes,
  and cancellation.
- Added
  `docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md`
  to record Publisher structured JSON diagnostic logging, stdout/stderr
  separation, stable diagnostic event fields, lifecycle events, safe message
  requirements, redaction policy, and rejected alternatives for unsafe or
  unbounded logging.
- Added
  `docs/architecture/ADR-0007-error-handling-and-failure-classification.md`
  to record Publisher CLI failure classification, exit-code mapping,
  verification exit `4`, transient exit `75`, cancellation exit `130`,
  internal fallback for unknown or blank stable codes, safe message separation,
  and `OperationCanceledException` propagation to the CLI boundary.
- Added `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md` to
  record Publisher release gate, vendor clearance, Avast false-positive review,
  release authorization, and final release verification as long-term release
  governance boundaries.
- Added `docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md` to
  record OAuth 2.0 Desktop as the Publisher Google API authentication decision
  for personal Gmail and local operator workflows.
- Added `docs/architecture/ADR_INDEX.md`,
  `docs/architecture/adr-template.md`, and
  `docs/architecture/ADR-0001-architecture-decision-record-process.md` to
  establish the VMF repository ADR operating basis as docs-only / local-only
  architecture decision tracking.

## Changed

- Updated the ADR index to track ADR-0009 and keep evidence bundle creation,
  evidence validation, evidence redaction, and approval package preparation
  separate from release authorization, vendor clearance, Avast resolution,
  package or distribution artifact updates, release, tag, publication, Live
  E2E, Google Docs or Drive mutation, and flagged executable re-run.
- Updated the ADR index to track ADR-0008 and keep ADR-0003 as the release
  gate and vendor-clearance governance basis while ADR-0008 records the
  operational preflight hard stop and release boundary.
- Updated the ADR index to track ADR-0004 and link it to ADR-0001 through
  ADR-0003 plus the existing Phase 3-2B / Phase 3-2C voyage-log records.
- Updated the ADR index to track ADR-0005 and keep ADR-0004 focused on update
  safety while ADR-0005 records failure-time retry judgment and failure
  classification.
- Updated the ADR index to track ADR-0006 and keep Phase 4-2-1 as the detailed
  Diagnostic Logging Specification while ADR-0006 records the durable
  safe-observability decision.
- Updated the ADR index to track ADR-0007 and keep Phase 4-2-2 as the detailed
  Error Handling Specification while ADR-0007 records the durable CLI error
  handling and failure classification decision.
- Updated the ADR index to track ADR-0003 and keep runbook procedure separate
  from ADR governance: the ADR records the long-term release-control decision
  and does not authorize release, clearance, risk acceptance, publication, tag
  creation, package publication, Live E2E, or Google Docs / Drive mutation.
- Updated the ADR index to track ADR-0002 and record Google Picker plus
  `drive.file` least-privilege routing as a vNext reconsideration item rather
  than an adopted behavior.
- Recorded that ADR numbering starts at `ADR-0001`, ADR statuses are limited
  to Proposed, Accepted, Superseded, and Deprecated, the ADR index tracks
  successor ADRs and related documents, and Accepted ADRs are replaced by later
  ADRs rather than rewritten for new meaning.
- Recorded that ADRs do not replace Frozen Specifications, implementation
  specifications, public API contracts, runbooks, release records,
  verification evidence, or current status records.
- Maintained the existing release boundary: Avast false-positive handling
  remains pending, vendor clearance has not been obtained, release remains
  blocked, and no Live E2E, Google Docs or Drive mutation, package or
  distribution artifact update, release, tag, publication, flagged executable
  re-run, production code change, test change, Frozen specification change,
  public API change, or push was performed.

---

# 2026-08-02

## Added

- Added `docs/development/Publisher_AvastResponseIntakeTemplate.md` to define
  a safe docs-only / local-only intake record for a future Avast
  false-positive response while preserving the current release hold.
- Added `docs/development/Publisher_vNext_Backlog.md` to record Publisher
  vNext resume-gate, release-safety, hardening, and enhancement candidates
  while preserving the current docs-only / local-only release hold.
- Added `docs/development/Publisher_ReleaseApprovalPackage.md` to summarize
  the current docs-only / local-only approval package, evidence index, ahead
  commits, blocked operations, resume conditions, and approval recommendation:
  Hold.

## Changed

- Recorded that the Avast response intake template does not assert a response
  was received, does not resolve Avast pending, and does not authorize release,
  tag, publication, Live E2E, Google Docs or Drive mutation, package or
  distribution artifact creation/update, flagged executable re-run, production
  code changes, Frozen specification changes, public API changes, or push.
- Recorded that vendor clearance has not been obtained and the release remains
  blocked; the approval package did not perform release, tag, publication,
  Live E2E, Google Docs or Drive mutation, package or distribution artifact
  creation/update, flagged executable re-run, production code/test changes,
  Frozen specification changes, public API changes, or push.
- Recorded that the Publisher vNext backlog does not change the release state:
  Avast false-positive handling remains pending, release remains blocked, and
  no Live E2E, Google Docs or Drive mutation, package or distribution artifact
  update, release, tag, publication, or flagged executable re-run was
  performed.

---

# 2026-08-01

## Added

- Added `docs/development/Publisher_PreflightHardening.md` to consolidate
  Avast-pending allowed work, blocked work, preflight hard stops, resume
  conditions, and reporting requirements for Publisher release-boundary
  control.
- Added `docs/development/Publisher_EvidenceBundleSpecification.md` to define
  a redacted Publisher evidence bundle structure for release review, security
  review, Avast false-positive appeal, internal audit, and regression
  investigation while preserving the current release hold.
- Added `docs/development/Publisher_FailureReport_DiagnosticSummary.md` to
  record the Avast-pending release gate hold, Phase 4 local-only verification
  completion, Diagnostic Logging and Error Handling state, resume conditions,
  recommended resume order, and current decision: Hold. Await Avast response.
- Added `docs/development/Publisher_OperatorGuidance_AvastHold.md` to record
  local-only operator guidance for the Avast-pending release hold, including
  allowed actions, blocked actions, decision rules, and the required resume
  order.
- Added Publisher Phase 4-3 release-readiness review records covering release
  readiness, release candidate verification, artifact audit, security and
  supply-chain review, and go/no-go state; Phase 4-3 is Done with an overall
  `DEFERRED` release decision.
- Added Publisher Phase 4-2-1 diagnostic logging specification, implementation
  review record, and local-only verification evidence.
- Added Publisher Phase 4-2-2 error handling specification, final review
  record, and local-only verification evidence.
- Added Publisher Phase 4-2-3 retry policy specification consolidation,
  including retry classification, exit-code and stable-code relationships,
  safe retry conditions, structured logging requirements, and local-only test
  matrix.
- Added Publisher Phase 4-2-3 Local Verify Report implementation record and
  local-only verification report tests.

## Changed

- Synchronized Publisher current status and voyage log to keep local-only
  verification completion separate from release readiness, with Avast handling,
  Live E2E, artifact generation/audit, and release approval recorded as
  unresolved conditions.
- Maintained the Avast pending gate and unchanged release boundary: no release,
  tag, publication, Live E2E, Google Docs or Drive mutation, package or
  distribution artifact creation/update, flagged executable re-run, or push.
- Hardened Publisher runbook and test classification preflight rules so local
  evidence cannot be promoted into release readiness, Live E2E readback,
  package approval, publication approval, or antivirus vendor clearance.
- Clarified that operators must treat the current Avast hold as operational,
  not a product regression, and must not proceed to the release path before the
  Avast response is recorded.
- Standardized Publisher CLI structured diagnostic logs with command, phase,
  operation, lifecycle events, safe summary messages, and normalized warning
  messages while preserving public APIs and publish result compatibility.
- Standardized Publisher CLI error classification, verification exit code
  handling, cancellation exit behavior, and fixed safe failure summaries while
  preserving public APIs and retry policy.
- Standardized Publisher CLI `verify` report output with stable PASS / FAIL /
  SKIPPED check rows, exit code, safe failure summaries, execution timestamp,
  configuration, environment metadata, and explicit local-only constraints.

---

# 2026-07-31

## Changed

- Updated Publisher Phase 3-10 release evidence after repository-owner release execution approval, including synchronized `main` / `origin/main`, Avast blocker resolution, Release build/test, Live E2E, package verification, and artifact hash records.
- Recorded Publisher Phase 3-10 release completion for GitHub prerelease `vmf-publisher-v0.0.0-dev`, including target commit, release URL, asset size, and SHA-256.

---

# 2026-07-20

## Added

- Added Publisher Phase 2-2 inline content models and Markdown parsing for
  asterisk/underscore bold, italic, bold-italic, and HTTP(S) links.
- Added Google Docs inline text-style rendering for headings, paragraphs, and
  list items, including overlapping styles and post-tab-removal range correction.
- Added inline parser, renderer, request-mapper, block-renderer, and publication
  pipeline coverage plus a styled live-validation sample.
- Added Publisher Phase 2-1 ordered, unordered, nested, and mixed Markdown list support.
- Added configurable list indentation and depth normalization with defaults of two spaces and six levels.
- Added Google Docs list rendering with bullet/number presets and post-list index correction.
- Added parser, renderer, and publish-pipeline coverage plus a nested-list live-validation sample.
- Started VMF Studio v2.0 Publisher PoC v0.1 with a .NET solution, layered Publisher library, CLI, unit tests, and integration tests.
- Added minimal Markdown publication support for headings, paragraphs, and unordered-list items through separated Google Drive and Google Docs REST clients.
- Added the Publisher live-validation Markdown sample and local OAuth Desktop configuration example.
- Added installed-application OAuth 2.0 browser authorization for personal Gmail users, including persistent token-store reuse.
- Added configuration-based selection between OAuth Desktop and the existing service-account authentication flow.
- Added OAuth credential validation, cached-token reuse, persisted-token reuse, and authentication-mode unit coverage.
- Added `specs/studio/VMF-Studio-v2.0-Specification-v1.0-Frozen.md`, exported from the frozen Google Docs master specification.
- Added `RELEASE_NOTES_v1.0.md` for the VMF Studio v2.0 Specification v1.0 Frozen Edition.
- Added the Publisher v1.0 Architecture Specification Markdown set under `specs/publisher/`, including the consolidated specification artifact and volumes P0 through P6.
- Added `specs/publisher/schemas/publish-manifest.schema.yaml` as the Publisher v1.0 Publish Manifest schema baseline.

## Changed

- Enabled Shared Drive document creation and safe Google API failure reporting using only the API name, HTTP status, and sanitized error code.
- Enabled OAuth-authenticated document creation in an existing My Drive folder while preserving service-account Shared Drive support.
- Documented OAuth Desktop setup, browser consent, token persistence, service-account compatibility, secret management, and live verification for Publisher PoC v0.1.
- Established the Git repository copy of the Frozen Edition as the official version-controlled specification artifact.
- Advanced the project milestone from Specification Voyage to Implementation Voyage.
- Standardized the Publisher v1.0 specification documents as UTF-8 Markdown, preserved headings and numbered lists, and removed the intermediate DOCX sources after paragraph-level text verification.
- Updated `RELEASE_NOTES_v1.0.md` with the Publisher v1.0 documentation artifacts, source provenance, and conversion verification record.

## Released

- Released VMF Studio v2.0 Specification v1.0.0 (Frozen Edition).

---

# 2026-07-16

## Added

- Added centralized VMF Studio v1.1 release metadata in `AppReleaseInfo`.
- Added generated public API and error code inventories for the v1.1 design freeze.
- Added VMF Studio v1.1 release notes, user guide, manifest compatibility notes, and template compatibility notes.
- Added release documentation generation tooling under `tools/release/`.
- Added versioned VMF Studio v1.1.0 distribution packaging.

## Changed

- Updated `tools/build/build.ps1` to read release metadata from source and record product, schema, minimum supported version, and build date metadata in the add-in.
- Updated README release references for the VMF Studio v1.1.0 distribution artifact.

## Verified

- Rebuilt the add-in with ProductVersion 1.1.0.
- Ran VMF Studio Self Check: 12 passed, 0 failed.
- Ran all VBA regression test runners: 17 passed, 0 failed.

## Released

- Froze VMF Studio v1.1 design at ProductVersion 1.1.0.

---

# 2026-07-12

## Added

- Added the application workspace structure under `applications/SchoolTimetable/`, including manifest, source layer placeholders, docs, and tests.
- Added `.vmf-root` and `src/Build/Common/BuildPathResolver.bas` to support repository-root based path resolution.
- Added project manifest parsing support in `src/Build/Application/Build_ProjectManifest.cls`.
- Added project manifest parse regression coverage in `tests/unit/Build/AppProjectManifestParseTests.bas`.
- Added Build blueprint manifest parsing and generation support through `src/Build/Application/Build_BlueprintParser.cls`.
- Added Build layer preview generation support.
- Added Build v1.1 candidate readiness documentation in `docs/development/Build_v1.1_CandidateReadinessAudit.md`.
- Added VMF v1.1 phase inventory documentation in `docs/development/VMF_v1.1_PhaseInventory.md`.
- Added Build v1.1 release planning documentation in `docs/development/Build_v1.1_ReleasePlan.md`.
- Added Build v1.1 RC1 and official release documentation.
- Added generated SchoolTimetable application class stubs from the application manifest.

## Changed

- Reorganized repository documentation and specifications under `docs/`, `specs/`, `candidates/`, and `templates/`.
- Updated `README.md` and `AGENTS.md` to describe the optimized repository layout and AI development rules.
- Updated `README.md`, `Vision.md`, and `docs/build/ROADMAP.md` for the Build v1.1 official release state.
- Updated Build v1.1 development records to point to the official release report.
- Added a release report index and updated versioning documentation with the current Build v1.1 release.
- Added Build v1.1 post-release verification evidence.
- Archived Build v1.1 candidate dispositions and noted Build v1.1 impacts on VMF v1.1 candidates.
- Corrected Build v1.1 post-release verification to reference the moved SchoolTimetable manifest and recorded the VMF audit result.
- Updated the SchoolTimetable manifest for Build v1.1 and `applications/SchoolTimetable/src` layer paths.
- Updated VMF generation and audit tooling to support the moved SchoolTimetable application manifest.
- Updated Build v1.1 post-release verification with the passing SchoolTimetable VMF audit result.
- Consolidated tests under `tests/` and updated `tools/test/run-tests.ps1` for the reorganized test layout.
- Moved Build templates to `templates/` and updated Build manifest template references.
- Moved Build release artifacts under `dist/release/Build/v1.0.2/`.
- Updated Build tooling and tests to resolve paths from the repository root.
- Updated Build manifest files to align with resolved template paths, project manifests, and layer manifests.
- Updated `AppGeneratorService` to use project manifest input during project generation.
- Updated the Build preview API to return `ComResult`.
- Updated Build v1.1 candidate notes, readiness audit, and phase inventory with RC1 readiness and next-step status.
- Updated `tools/build/build.ps1` to support explicit release metadata for RC and official release artifacts.
- Regenerated the Build v1.0.2 add-in artifact after path-resolution and manifest changes.
- Generated Build v1.1 RC1 and official release artifacts.

## Fixed

- Fixed Build workspace and release artifact path resolution when running from different current directories.
- Fixed manifest template path resolution in `InfManifestProvider` and related Build manifests.
- Fixed project manifest parser support for v1 YAML manifest format.
- Fixed project manifest template path resolution in `Build_ProjectManifest`.
- Hardened Build manifest validation for missing or invalid manifest and template values.
- Rejected empty blueprint generation layers and added regression coverage.
- Made missing-template-path infrastructure coverage runnable in the consolidated test layout.

## Removed

- Removed obsolete `prompt/`, `test/`, and top-level source placeholder directories after repository layout consolidation.
- Removed obsolete top-level `src/Common`, `src/Infrastructure`, and `src/UI` placeholders.
- Removed generated VMF source placeholders and generated sample source modules from `src/VMF/`, leaving layer placeholders for future generation.
- Removed obsolete `dist/release/VMF.xlam`.

## Released

- Released Build.xlam v1.1 as the official Build v1.1 release.

---

# 2026-07-09

## Fixed

- Fixed `InfVbaProjectProvider` target VBProject resolution so Build.xlam is not selected as the generation target when Build.xlam is active.
- Added regression coverage confirming generated modules are not added to Build.xlam.

## Changed

- Updated `tools/build/build.ps1` to record Build Version = 1.0.2 and Release Type = Release in Build.xlam.
- Added `docs/releases/Build_v1.0.2_ReleaseReport.md` and updated README.md to reference the patch release report.

## Released

- Released Build.xlam v1.0.2 as an official patch release.

---

# 2026-07-07

## Added

- Added BuildQualityStandard_v1.0.md as the official Build.xlam release quality standard.
- Added Build v1.0.1 official release records.
- Added Build.xlam release metadata for Build Version and Release Type.

## Changed

- Updated BuildReleaseProcedure_v1.0.md to define the 14 Step Build v1.0.1 release audit.
- Updated Release Procedure steps to use the nine required items, including Inspection Targets, Result Code, and Failure Handling.
- Updated BuildReleaseChecklist_v1.0.md to record final judgments for the 14 Step release audit.
- Updated BuildDocumentationStandard_v1.0.md, BuildBlueprint_v1.0.1.md, README.md, BuildCandidates_v1.1.md, and docs/releases/Build_v1.0.1_ReleaseReport.md for Result Code Standard, Generate Summary evidence, Version Verification, PowerShell-built Build.xlam audit target, and FAIL re-audit handling.
- Updated BuildReleaseChecklist_v1.0.md and docs/releases/Build_v1.0.1_ReleaseReport.md with final PASS results and APPROVED release decision.
- Updated tools/build/build.ps1 to record Build Version = 1.0.1 and Release Type = Release in Build.xlam.

## Released

- Released Build.xlam v1.0.1 as the official Build Release baseline.

---

# 2026-07-05

## Added

- Added BuildReleaseProcedure_v1.0.md as the official release verification procedure.
- Added Release Report as a required official release artifact.
- Added docs/releases/Build_v1.0.1_ReleaseReport.md as the official Build v1.0.1 release report.
- Added BuildDocumentationStandard_v1.0.md as the documentation standard for the Build.xlam official documentation set.
- Added BuildBlueprint_v1.0.1.md as the frozen Build v1.0.1 blueprint.
- Added README.md as the index for the official documentation set.
- Added CHANGELOG.md.

## Changed

- Updated BuildCanon_v1.0.md to reference BuildDocumentationStandard_v1.0.md.
- Updated BuildDocumentationStandard_v1.0.md to include official release requirements.
- Updated BuildDocumentationStandard_v1.0.md to define the Procedure, Checklist, and Report relationship.
- Updated README.md to describe Release Checklist and Release Report reference order.
- Updated README.md to include the Release Procedure and release reference order.
- Updated BuildReleaseChecklist_v1.0.md to require Release Report creation and storage.
- Updated BuildReleaseChecklist_v1.0.md to require checks to be performed according to BuildReleaseProcedure_v1.0.md.
- Separated Build v1.1 Candidate items from BuildCanon_v1.0.md into BuildCandidates_v1.1.md.
- Updated BuildReleaseChecklist_v1.0.md to include BuildBlueprint and BuildDocumentationStandard checks.
