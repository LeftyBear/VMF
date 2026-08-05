# ADR-0009: Evidence Bundle and Release Approval Package Boundary

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher evidence bundle, release approval package, hold recommendation, Avast response intake, and release-boundary separation
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_FailureReport_DiagnosticSummary.md, docs/distribution/PublisherReleaseRunbook.md, docs/development/CURRENT_STATUS.md

## Context

VMF Publisher has a redacted evidence-bundle specification, a release approval
package, a failure-report diagnostic summary, an Avast response-intake template,
and a release runbook. These records support review and future gate
reassessment, but the current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current release approval package records:

`Approval Recommendation = Hold`.

If an Avast response has not been received and recorded, the default decision is
`Hold continues`.

## Decision

The Publisher evidence bundle is a design, collection, validation, and
redaction boundary. It defines how evidence is identified, organized, checked,
and sanitized for release review, security review, Avast false-positive appeal,
internal audit, and regression investigation.

The evidence bundle is not itself:

- a release artifact;
- a publication artifact;
- a package artifact;
- a distribution artifact;
- release authorization;
- vendor clearance;
- Avast false-positive resolution.

The release approval package is a review record. It may summarize evidence,
blocked items, ahead commits, approval recommendation, and resume conditions,
but it is not executable approval. The current release approval package records
`Approval Recommendation = Hold`.

Vendor clearance not obtained means:

- no approval;
- no release authorization;
- no permission to publish.

Evidence bundle creation, evidence bundle validation, evidence redaction,
release approval package preparation, or approval package review does not imply
authorization for:

- package creation or package update;
- distribution artifact creation or update;
- release;
- tag creation;
- publication;
- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- token-store mutation;
- flagged executable re-run.

If the Avast response has not been received and recorded in
`docs/development/Publisher_AvastResponseIntakeTemplate.md`, the default
decision remains `Hold continues`.

## Relationship To Other ADRs

ADR-0003 records the release gate and vendor-clearance governance basis:
required verification, vendor clearance, Avast false-positive review resolution
or formal repository-owner risk acceptance, explicit release authorization, and
final release verification.

ADR-0009 does not replace ADR-0003. ADR-0009 fixes the boundary between
evidence or approval-package records and release authorization. Evidence
organization and approval-package preparation cannot satisfy vendor clearance,
resolve the Avast review, or reopen the release gate.

ADR-0008 records the operational preflight hard stop and release boundary:
Avast-pending prohibited operations, allowed local-only work, and resume-gate
requirements.

ADR-0009 does not replace ADR-0008. ADR-0009 records that evidence-bundle and
approval-package work must remain inside that preflight boundary until the
release gate is separately reopened and operation-specific authorization is
recorded.

ADR-0005 remains responsible for retry policy and failure classification.
ADR-0006 remains responsible for diagnostic logging and safe observability.
ADR-0007 remains responsible for CLI error handling and the stable failure
surface.

ADR-0009 is responsible for evidence-bundle and release-approval-package
boundary control. It does not change retry eligibility, diagnostic logging,
safe observability, CLI exit-code mapping, stable error classification, public
failure behavior, or redaction requirements already governed by those ADRs and
their source specifications.

## Consequences

Review records must keep the evidence bundle, release approval package, hold
recommendation, and Avast response intake separate from release execution.

An approval recommendation of `Hold` is not release approval. A future change
from `Hold` would still require the applicable release gate, vendor clearance
treatment, final verification, and operation-specific authorization before any
release-path operation may proceed.

Evidence bundle records must continue to redact sensitive material before they
are shared or attached to review workflows. Redacted evidence helps reviewers
evaluate state; it does not authorize release, tag, publication, package or
distribution artifact update, Live E2E, Google Docs or Drive mutation, or
flagged executable execution.

If no Avast response is recorded, release records must preserve `Hold
continues`. They must not infer vendor clearance from silence, local-only
verification, static review, evidence organization, or approval-package
preparation.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Accepted | Evidence bundle and release approval package boundary accepted as a durable release-control decision while Avast handling remains pending. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0005-retry-policy-and-failure-classification.md`
- `docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md`
- `docs/architecture/ADR-0007-error-handling-and-failure-classification.md`
- `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
- `docs/development/Publisher_EvidenceBundleSpecification.md`
- `docs/development/Publisher_ReleaseApprovalPackage.md`
- `docs/development/Publisher_AvastResponseIntakeTemplate.md`
- `docs/development/Publisher_FailureReport_DiagnosticSummary.md`
- `docs/distribution/PublisherReleaseRunbook.md`
- `docs/development/CURRENT_STATUS.md`
- `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`

## Replacement

This ADR does not supersede an earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify implementation behavior.
- This ADR does not modify tests.
- This ADR does not create or update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not create or update release artifacts, publication artifacts,
  package artifacts, or generated artifacts.
- This ADR does not replace runbooks, release records, verification evidence,
  approval packages, or current status records.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, vendor submission, or
  vendor clearance.
- This ADR does not claim release readiness, vendor clearance, Avast
  false-positive resolution, risk acceptance, final release verification, Live
  E2E authorization, approval granted, or publication completion.
