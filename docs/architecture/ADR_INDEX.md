# Architecture Decision Record Index

Index State : Active tracking document
Scope       : Architecture Decision Record tracking for VMF repository decisions
Depends     : docs/architecture/adr-template.md, docs/architecture/ADR-0001-architecture-decision-record-process.md

This index tracks Architecture Decision Records (ADRs) for the VMF repository.
It is documentation-only. It does not replace Frozen Specifications,
implementation specifications, public API contracts, release checklists, or
runbooks, or verification evidence.

## Status Values

ADR status values are limited to:

- Proposed;
- Accepted;
- Superseded;
- Deprecated.

## ADR Register

| Number | Title | Status | Successor ADR | Related Documents |
| --- | --- | --- | --- | --- |
| ADR-0001 | Architecture Decision Record Process | Accepted | - | `docs/architecture/adr-template.md`; `docs/architecture/ADR_INDEX.md`; `AGENTS.md`; `VMF_CODEX_PLAYBOOK.md`; `docs/development/CURRENT_STATUS.md` |
| ADR-0002 | OAuth 2.0 Desktop Authentication | Accepted | - | `docs/distribution/InstallationGuide.md`; `docs/distribution/LiveE2EOperations.md`; `src/Publisher/Infrastructure/Google/GooglePublisherOptions.cs`; `src/Publisher/Infrastructure/Google/OAuthDesktopGoogleCredentialProvider.cs` |
| ADR-0003 | Release Gate and Vendor Clearance | Accepted | - | `docs/development/CURRENT_STATUS.md`; `docs/development/Publisher_PreflightHardening.md`; `docs/development/Publisher_AvastResponseIntakeTemplate.md`; `docs/development/Publisher_ReleaseApprovalPackage.md`; `docs/distribution/PublisherReleaseRunbook.md` |
| ADR-0004 | Verified State and Differential Update Safety | Accepted | - | `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; `docs/development/CURRENT_STATUS.md`; `docs/architecture/ADR-0001-architecture-decision-record-process.md`; `docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md` |
| ADR-0005 | Retry Policy and Failure Classification | Accepted | - | `docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md`; `docs/development/Publisher_Phase4-2-3_RetryPolicySpecification.md`; `docs/development/CURRENT_STATUS.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md` |
| ADR-0006 | Diagnostic Logging and Safe Observability | Accepted | - | `docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md`; `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; `docs/development/CURRENT_STATUS.md`; `docs/architecture/ADR-0001-architecture-decision-record-process.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md` |
| ADR-0007 | Error Handling and Failure Classification | Accepted | - | `docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md`; `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; `docs/development/CURRENT_STATUS.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0005-retry-policy-and-failure-classification.md`; `docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md` |
| ADR-0008 | Preflight Hard Stop and Release Boundary | Accepted | - | `docs/development/Publisher_PreflightHardening.md`; `docs/distribution/PublisherReleaseRunbook.md`; `docs/development/Publisher_TestClassification.md`; `docs/development/CURRENT_STATUS.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0005-retry-policy-and-failure-classification.md`; `docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md`; `docs/architecture/ADR-0007-error-handling-and-failure-classification.md` |
| ADR-0009 | Evidence Bundle and Release Approval Package Boundary | Accepted | - | `docs/development/Publisher_EvidenceBundleSpecification.md`; `docs/development/Publisher_ReleaseApprovalPackage.md`; `docs/development/Publisher_AvastResponseIntakeTemplate.md`; `docs/development/Publisher_FailureReport_DiagnosticSummary.md`; `docs/distribution/PublisherReleaseRunbook.md`; `docs/development/CURRENT_STATUS.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0005-retry-policy-and-failure-classification.md`; `docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md`; `docs/architecture/ADR-0007-error-handling-and-failure-classification.md`; `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md` |
| ADR-0010 | vNext Backlog and Deferred Scope Boundary | Accepted | - | `docs/development/Publisher_vNext_Backlog.md`; `docs/development/CURRENT_STATUS.md`; `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`; `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md` |
| ADR-0011 | Release Authorization Record and Explicit Approval Boundary | Accepted | - | `docs/development/Publisher_ReleaseApprovalPackage.md`; `docs/development/Publisher_AvastResponseIntakeTemplate.md`; `docs/development/CURRENT_STATUS.md`; `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`; `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md` |
| ADR-0012 | Release Resumption Procedure and Final Verification Order | Accepted | - | `docs/development/Publisher_ReleaseApprovalPackage.md`; `docs/development/Publisher_AvastResponseIntakeTemplate.md`; `docs/development/CURRENT_STATUS.md`; `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`; `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`; `docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md` |
| ADR-0013 | Release Decision Record and Post-Authorization Traceability | Accepted | - | `docs/development/Publisher_ReleaseApprovalPackage.md`; `docs/development/Publisher_AvastResponseIntakeTemplate.md`; `docs/development/CURRENT_STATUS.md`; `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`; `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`; `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`; `docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md` |

## Numbering

ADR numbers are assigned as a zero-padded sequence beginning at `ADR-0001`.
The next ADR number is the next unused number in this index.

Numbers are never reused, even when an ADR is superseded or deprecated.

## Index Maintenance

When an ADR is added or its status changes, update the corresponding row in
this index. The row must retain enough information to identify:

- the ADR number;
- the ADR title;
- the current status;
- the successor ADR, when the decision is replaced;
- related documents that explain the governing specification, implementation
  boundary, release boundary, or operational context.

The index records tracking metadata only. It must not be used to change a
decision's accepted content, modify a Frozen Specification, replace a runbook,
or imply release approval.
