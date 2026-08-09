# Publisher Test Traceability Matrix

Status  : Done
Scope   : Documentation-only Publisher ADR, implementation, test, and release-governance traceability
Depends : docs/architecture/ADR_INDEX.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_TestClassification.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_PreflightHardening.md, docs/distribution/PublisherReleaseRunbook.md

This document records repository-evidence traceability for VMF Publisher.
It does not approve a release, create or update packages, create tags, publish
artifacts, execute Live E2E, mutate Google Docs or Google Drive, re-run flagged
executables, change production code, change tests, change public APIs, modify
Frozen specifications, write to `dist/`, or push commits.

## 1. Purpose

The purpose of this matrix is to show how current Publisher requirements and
controls trace from governing records through implementation, tests, operational
verification, and evidence.

The matrix is intentionally conservative. It records coverage only when the
repository contains direct evidence. It keeps governance traceability separate
from executable test coverage when the control is a release decision, approval
boundary, runbook rule, or future-operation record.

## 2. Scope

In scope:

- ADR-0001 through ADR-0019, all currently `Accepted` in
  `docs/architecture/ADR_INDEX.md`;
- Publisher local source implementation and tests that can be identified from
  the repository;
- local-only verification, non-live integration tests, mock-backed tests,
  documentation checks, and release-governance evidence;
- release-boundary records that preserve the current formal state.

Out of scope:

- Live E2E execution;
- Google Docs or Google Drive mutation;
- package or `dist/` creation, replacement, or update;
- release tags, GitHub Releases, publication, or release announcements;
- flagged executable re-run;
- vendor submission, vendor response processing, or vendor clearance;
- release authorization, release decision, publication, rollback, archival, or
  emergency-release execution.

## 3. Traceability Policy

Status values:

| Status | Meaning |
| --- | --- |
| Covered | Repository evidence demonstrates the control through governing documents and applicable implementation, tests, or documented operational records. |
| Partial | Repository evidence exists for part of the control, but some required implementation, test, live, package, release, or operational evidence is missing or intentionally deferred. |
| Blocked | The control cannot be completed in the current state because the release gate, Avast pending state, Live E2E gate, package gate, publication gate, or operation-specific authorization blocks it. |
| N/A | Executable test coverage is not the right evidence type for this governance or documentation control. |
| Not Yet Executed | The repository defines the procedure or test target, but current evidence for an authorized execution is not present for this matrix scope. |

Rules:

- Use `Covered` only for evidence that can be demonstrated from repository
  files.
- Do not promote local-only evidence to release readiness, Live E2E readback,
  Google Drive cleanup, package approval, publication approval, or vendor
  clearance.
- Treat `Evidence Bundle` and `Release Approval Package` records as review and
  evidence boundaries only. They are not release authorization.
- Treat ADR-0001 governance traceability separately from executable Publisher
  test coverage.
- Treat ADR-0011 through ADR-0018 primarily as release-governance controls.
  They define future records and boundaries; they do not prove release,
  publication, rollback, archival, or emergency-release execution.
- Use `PASS` only when a repository evidence record directly reports a
  previously executed check. This document itself records status labels, not a
  new execution pass for source tests or release gates.

## 4. Test Traceability Matrix

| Requirement / Control | Specification or Governing Document | ADR | Implementation | Unit Test | Integration Test | E2E / Operational Verification | Evidence | Status |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| ADR process, numbering, status tracking, and accepted-body stability | `docs/architecture/ADR_INDEX.md`; `docs/architecture/adr-template.md`; `AGENTS.md`; `VMF_CODEX_PLAYBOOK.md` | ADR-0001 | Documentation process under `docs/architecture/` | N/A | N/A | ADR index maintenance and current status synchronization | `docs/architecture/ADR_INDEX.md`; ADR-0001; `docs/development/CURRENT_STATUS.md` | Covered for governance; N/A for executable tests |
| OAuth Desktop authentication for local operator workflows while retaining Service Account support | `docs/distribution/InstallationGuide.md`; `docs/distribution/LiveE2EOperations.md` | ADR-0002 | `src/Publisher/Infrastructure/Google/GooglePublisherOptions.cs`; `GoogleCredentialProviderFactory.cs`; `OAuthDesktopGoogleCredentialProvider.cs`; `ServiceAccountGoogleCredentialProvider.cs` | `tests/unit/Publisher/OAuthDesktopGoogleCredentialProviderTests.cs` | `tests/integration/Publisher/GoogleDocsEndToEndIntegrationTests.cs` exists but is gated | Live E2E blocked unless separately authorized with `VMF_PUBLISHER_GOOGLE_E2E=1` | ADR-0002; `Publisher_TestClassification.md`; `LiveE2EOperations.md` | Partial: local/unit coverage present; Live E2E not executed here and currently blocked |
| Release gate, vendor clearance, Avast handling, and publication boundary | `docs/development/CURRENT_STATUS.md`; `Publisher_PreflightHardening.md`; `Publisher_ReleaseApprovalPackage.md`; `docs/distribution/PublisherReleaseRunbook.md` | ADR-0003; ADR-0019 | Release governance documents and runbook controls | N/A | N/A | Release Hold lifted by VMF risk acceptance; final verification, Live E2E, result review, package/dist, and tag/release remain gated | Current status records VMF risk acceptance, vendor clearance not obtained, and no Avast safety certification | Covered for governance; release execution pending |
| Verified State baseline, differential update safety, revision conflict, readback verification, and state promotion boundary | `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; ADR-0004 | ADR-0004 | `src/Publisher/Application/VerifiedPublishLifecycle.cs`; `PublishStateVerifier.cs`; `PhysicalUpdatePlanner.cs`; `GoogleDocsApplyEngine.cs`; `PublishTransactionCoordinator.cs`; `src/Publisher/Domain/PhysicalUpdateModel.cs` | `PhysicalUpdatePlannerTests.cs`; `PhysicalUpdateLifecycleTests.cs`; `PublishStatePromotionTests.cs`; `VerifiedPublishStateStoreTests.cs`; `GoogleDocsApplyEngineTests.cs` | `PhysicalUpdateLifecycleIntegrationTests.cs`; `VerifiedStateLifecycleIntegrationTests.cs`; `PublishTransactionCoordinatorIntegrationTests.cs` | Live Google readback not executed in this task | ADR-0004; local verification evidence summarized by `CURRENT_STATUS.md` | Covered for local/source and non-live integration; Partial for Live E2E readback |
| Retry policy and failure classification are delivery-state aware and idempotency bounded | `Publisher_Phase4-2-2_ErrorHandlingSpecification.md`; `Publisher_Phase4-2-3_RetryPolicySpecification.md` | ADR-0005 | `src/Publisher/Application/PhysicalUpdateExecutor.cs`; `src/Publisher/Infrastructure/ImageMetadataReader.cs`; Google batch/update exception delivery-state handling | `PhysicalUpdateExecutorTests.cs`; `GoogleDocsClientTests.cs`; `GoogleDocsBatchUpdateClientTests.cs`; `ImageMetadataReaderTests.cs`; `CliApplicationTests.cs` | Non-live integration tests cover pipeline and transaction behavior | No Live E2E, package, or flagged executable execution | Phase 4-2-2 final review reports unit/integration/build local PASS; retry consolidation remains docs-only for future changes | Partial: implemented local behavior exists; retry-policy consolidation is partly specification/future-scope |
| Structured diagnostic logging, safe observability, lifecycle events, and redaction boundary | `Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md` | ADR-0006 | `src/Publisher.Cli/Program.cs` `StructuredPublisherLogger`; warning logging from Publisher execution paths | `tests/unit/Publisher/CliApplicationTests.cs`; `PublishPlanExecutorTests.cs` where warning behavior applies | N/A | Operational evidence limited to local CLI/test diagnostics; no Live E2E in this task | Phase 4-2-2 final review records safe structured diagnostics and local PASS counts | Covered for local CLI/unit evidence |
| CLI error classification, exit-code mapping, safe messages, cancellation, and verification failure treatment | `Publisher_Phase4-2-2_ErrorHandlingSpecification.md` | ADR-0007 | `src/Publisher.Cli/Program.cs` `ErrorClassification`, `Classify`, `ExitCodeFor`, `SafeMessage`; lower-layer cancellation propagation | `tests/unit/Publisher/CliApplicationTests.cs`; `PhysicalUpdateExecutorTests.cs` | Publisher non-live integration tests | No Live E2E in this task | Phase 4-2-2 final review records 33/33 focused, 490/490 unit, 12/12 non-live integration, build, format, and diff checks from that phase | Covered for local/source evidence |
| Avast-pending preflight hard stop and local-only allowed work | `Publisher_PreflightHardening.md`; `Publisher_TestClassification.md`; `PublisherReleaseRunbook.md`; `CURRENT_STATUS.md` | ADR-0008 | Documentation and runbook hard stops | N/A | N/A | Release-path operations remain blocked until separate authorization | Current status and preflight records list allowed and blocked operations | Covered for governance; Blocked for release-path execution |
| Evidence Bundle and Release Approval Package remain evidence/review boundaries, not authorization | `Publisher_EvidenceBundleSpecification.md`; `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md` | ADR-0009 | Documentation-only evidence and approval-package records | N/A | N/A | Evidence bundle assembly, approval, vendor response, and release authorization are separate future gates | Approval Recommendation = Hold; no Avast response received; vendor clearance not obtained | Covered for governance; Blocked for release authorization |
| vNext backlog and deferred scope boundary | `Publisher_vNext_Backlog.md`; `CURRENT_STATUS.md`; Voyage Log | ADR-0010 | Documentation-only backlog and planning classification | N/A | N/A | vNext implementation and release adoption are not authorized by backlog labels | Current status records backlog as docs-only/local-only and release unchanged | Covered for governance; N/A for executable tests |
| Release authorization must be a separate explicit release-governance record | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md`; Voyage Log | ADR-0011; ADR-0019 | Governance records only; no release authorization implementation required | N/A | N/A | ADR-0019 lifts the Release Hold but does not authorize final verification, Live E2E, package/dist, tag, or release by implication | Current status says vendor clearance is not obtained and release execution has not started | Covered for boundary; release authorization pending |
| Release resumption procedure and final verification order after clearance | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md`; `PublisherReleaseRunbook.md` | ADR-0012 | Runbook and release-approval-package sequencing | N/A | N/A | Not yet available because Avast response and vendor clearance are pending; final release verification not executed | Current status records ADR-0012 as docs-only/local-only and not release authorization | Covered for procedure; Blocked for execution |
| Release Decision Record after release authorization | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md` | ADR-0013 | Future release decision record requirement; no current record | N/A | N/A | No Release Decision Record has been created because release authorization has not been granted | Current status records no Release Decision Record and Hold remains | Blocked |
| Release Publication Record and Post-Release Evidence after publication | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md`; `PublisherReleaseRunbook.md` | ADR-0014 | Future publication/post-release record requirement; no current record | N/A | N/A | No publication occurred; no Publication Record or Post-Release Evidence exists | Current status records no publication and Hold remains | Blocked |
| Release withdrawal, rollback, and incident evidence boundaries | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md` | ADR-0015 | Future withdrawal, rollback, and incident evidence record requirements | N/A | N/A | No withdrawal, rollback, or incident evidence bundle exists; release has not been published | Current status records no Withdrawal Record, Rollback Record, or Incident Evidence Bundle | Blocked / N/A until a release or incident exists |
| Release versioning, tag, artifact identity, evidence identity, and approval/authorization identity | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md` | ADR-0016 | Future release identity fields in release records; package tooling exists but was not run | N/A | N/A | No tag, package, artifact, evidence bundle, approval record, authorization record, or release identity created or finalized | Current status records no release identity finalization and no `dist` write | Blocked |
| Release retention, archival, and audit trail | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md` | ADR-0017 | Future archive/audit requirements; current docs preserve boundaries | N/A | N/A | No archive artifact creation; no archive entry may imply release approval, publication, production readiness, vendor clearance, or Avast safety certification | Current status records ADR-0019 risk acceptance and no publication/authorization | Covered for boundary; release archive execution pending |
| Emergency release exception boundary | `Publisher_ReleaseApprovalPackage.md`; `Publisher_AvastResponseIntakeTemplate.md`; `CURRENT_STATUS.md` | ADR-0018 | Future emergency exception record requirements | N/A | N/A | No emergency exception approval, risk acceptance, normal release gate reopening, or publication occurred | Current status records no emergency exception approval and Hold remains | Blocked |
| Test classification and safe validation categories | `Publisher_TestClassification.md`; `Publisher_PreflightHardening.md` | ADR-0008; ADR-0009; ADR-0011 | Documentation-only classification of build, unit, integration, Live E2E, package, and publication checks | N/A | N/A | Live E2E, package, publication, and flagged executable rows are blocked while Avast is pending | Classification record requires `PASS` only for directly verified evidence | Covered for governance |
| Local source verification evidence from completed Phase 4 scope | `CURRENT_STATUS.md`; `Publisher_Phase4_LocalVerificationEvidence.md`; Phase 4-3 records | ADR-0003; ADR-0008; ADR-0009; ADR-0019 | Source implementation exists across Publisher projects | Unit test evidence summarized in Phase records | Non-live integration evidence summarized in Phase records | Local-only evidence only; not release readiness | `CURRENT_STATUS.md` records Phase 4 local-only verification complete / Release Hold lifted by VMF risk acceptance | Covered for current local-only state; Partial for release readiness |

## 5. ADR Coverage

| ADR | Title | Current Status | Coverage Type | Coverage Result |
| --- | --- | --- | --- | --- |
| ADR-0001 | Architecture Decision Record Process | Accepted | Governance traceability | Covered; executable Publisher tests are N/A |
| ADR-0002 | OAuth 2.0 Desktop Authentication | Accepted | Implementation and local/unit evidence, Live E2E gated | Partial |
| ADR-0003 | Release Gate and Vendor Clearance | Accepted | Release governance | Covered for boundary; release execution blocked |
| ADR-0004 | Verified State and Differential Update Safety | Accepted | Implementation, unit, non-live integration | Covered locally; Live E2E readback partial/not executed here |
| ADR-0005 | Retry Policy and Failure Classification | Accepted | Implementation, unit, docs-only consolidation | Partial |
| ADR-0006 | Diagnostic Logging and Safe Observability | Accepted | CLI implementation and unit tests | Covered locally |
| ADR-0007 | Error Handling and Failure Classification | Accepted | CLI implementation and unit tests | Covered locally |
| ADR-0008 | Preflight Hard Stop and Release Boundary | Accepted | Governance hard stop | Covered for boundary; blocked for release operations |
| ADR-0009 | Evidence Bundle and Release Approval Package Boundary | Accepted | Evidence/approval boundary | Covered for boundary; blocked for authorization |
| ADR-0010 | vNext Backlog and Deferred Scope Boundary | Accepted | Planning/governance | Covered; executable tests N/A |
| ADR-0011 | Release Authorization Record and Explicit Approval Boundary | Accepted | Release governance | Covered for boundary; release authorization blocked |
| ADR-0012 | Release Resumption Procedure and Final Verification Order | Accepted | Future release procedure | Covered for procedure; execution blocked |
| ADR-0013 | Release Decision Record and Post-Authorization Traceability | Accepted | Future release decision record | Blocked |
| ADR-0014 | Release Publication Record and Post-Release Evidence Boundary | Accepted | Future publication/post-release records | Blocked |
| ADR-0015 | Release Withdrawal / Rollback Record and Incident Evidence Boundary | Accepted | Future withdrawal/rollback/incident records | Blocked / N/A until applicable event |
| ADR-0016 | Release Versioning / Tag / Artifact Identity | Accepted | Future release identity records | Blocked |
| ADR-0017 | Release Retention / Archival / Audit Trail | Accepted | Future retention/archive/audit records | Covered for boundary; blocked for archive execution |
| ADR-0018 | Emergency Release Exception Boundary | Accepted | Future emergency exception records | Blocked |
| ADR-0019 | VMF Risk Acceptance And Release Hold Lift | Accepted | Risk acceptance and post-hold release sequence | Covered for governance; execution pending |

## 6. Coverage Summary

Covered local/source areas:

- ADR governance process and index traceability;
- OAuth Desktop implementation and unit coverage;
- Verified State, physical update planning, readback verification, revision
  conflict, and state-promotion local/non-live coverage;
- diagnostic logging and safe CLI observability local coverage;
- CLI error classification, safe messages, exit-code mapping, and cancellation
  local coverage;
- preflight, test classification, evidence bundle, release approval package,
  and release-governance boundary records.

Partial areas:

- OAuth Desktop is implemented and unit-tested, but credentialed Live E2E is
  blocked in the current release state and was not executed for this matrix.
- Verified State and update safety have local and non-live evidence, but this
  matrix does not add Live Google readback evidence.
- Retry policy has implementation evidence for existing physical update,
  Google client, image metadata, and CLI behavior, while
  `Publisher_Phase4-2-3_RetryPolicySpecification.md` is also a documentation
  consolidation record for future policy work.
- Local-only Phase 4 verification is complete, but release readiness remains
  unestablished.

Blocked or not-yet-executed areas:

- Avast vendor response processing and vendor clearance;
- final verification under ADR-0019;
- Live E2E under ADR-0019;
- result review under ADR-0019;
- package creation or update under ADR-0019;
- selected release artifact identity finalization;
- package verification for a current authorized release candidate;
- packaged executable smoke;
- Live E2E;
- Google Docs or Google Drive mutation;
- release authorization, Release Decision Record, tag, publication, Publication
  Record, Post-Release Evidence, withdrawal, rollback, incident evidence,
  archival artifact creation, and emergency release exception execution.

## 7. Current Release State

The current formal state remains:

`Phase 4 local-only verification complete / Release Hold lifted by VMF risk acceptance`.

Release boundary:

| Item | State |
| --- | --- |
| Local verification | Complete within the approved local-only safety boundary |
| Release readiness | Pending final verification, Live E2E, result review, package/dist, and tag/release sequence |
| Release gate | Hold lifted by ADR-0019 VMF risk acceptance; release execution not yet started |
| Avast false-positive handling | Vendor response pending; VMF risk acceptance recorded |
| Vendor clearance | Not obtained |
| Avast safety certification | Not claimed |
| Approval recommendation | Proceed to final verification sequence after explicit operation-specific authorization |
| Live E2E | Not executed by this task; requires explicit per-run authorization after final verification |
| Google Docs / Google Drive mutation | Not performed by this task; gated |
| Package creation or update | Not performed by this task; gated after result review |
| Package/dist update | Not performed by this task; gated after result review |
| Flagged executable re-run | Not performed by this task; gated by exact authorization |
| Release, tag, publication artifacts | Not created by this task; gated after package/dist |
| Evidence Bundle / Release Approval Package | Review/evidence boundaries only; not release authorization |

This matrix does not execute release work. It records traceability after
ADR-0019 lifts the Release Hold by VMF risk acceptance.

## 8. Maintenance Rules

Maintain this document when:

- an ADR is added, superseded, deprecated, or materially reinterpreted by a
  successor ADR;
- implementation or tests are added for a control currently marked `Partial`;
- a release gate is explicitly reopened or a blocked operation is separately
  authorized and executed;
- Live E2E, package verification, publication, rollback, archival, or
  emergency-release evidence is created under an approved operation;
- the formal release state changes in `CURRENT_STATUS.md`.

Maintenance requirements:

- preserve the distinction between governance coverage and executable test
  coverage;
- keep local-only verification separate from Live E2E and production/release
  operations;
- do not mark a release-governance future record as executed until the
  corresponding authorized operation actually occurs;
- keep Evidence Bundle and Release Approval Package records separate from
  release authorization;
- keep Avast vendor response pending and vendor clearance not obtained until a
  response is recorded in the approved intake/release record and reassessed;
- do not treat ADR-0019 risk acceptance as Avast safety certification;
- run documentation-safe validation, including `git diff --check`, after
  updating this file.
