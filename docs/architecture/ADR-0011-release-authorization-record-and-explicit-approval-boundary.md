# ADR-0011: Release Authorization Record and Explicit Approval Boundary

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher release authorization record, explicit approval boundary, and separation from ADR acceptance
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/CURRENT_STATUS.md

## Context

VMF Publisher uses ADRs to record durable architectural and operational
decisions. ADR-0003 records the release gate and vendor-clearance governance
basis. ADR-0008 records the Avast-pending operational preflight hard stop.
ADR-0009 records the evidence bundle and Release Approval Package as review
records, not executable approval.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

Accepted ADRs can define release-control boundaries, but an Accepted ADR must
not be confused with the explicit governance act that authorizes a release.

## Decision

Release authorization MUST NOT be represented by an ADR.

Accepted ADRs document architectural and operational decisions only. Accepted
ADRs do not imply release approval, production readiness, vendor clearance, or
authorization to publish, tag, package, distribute, re-run a flagged
executable, perform Live E2E, or mutate live Google Docs / Drive resources.

A release authorization record is a separate release-governance artifact. It
must be created and approved outside the ADR set before release-blocked
operations may proceed.

At minimum, a release authorization record must include:

- vendor clearance;
- Avast response or explicit risk acceptance;
- final verification result;
- explicit approver identity;
- approval timestamp;
- approved release scope;
- release target/version;
- confirmation that the blocked operations named by the authorization are
  permitted.

The Release Approval Package is evidence for review, not approval itself. The
current Release Approval Package recommendation remains `Hold`.

A `Hold` recommendation cannot authorize release, package publication, tag
creation, Live E2E, Google Docs / Drive mutation, distribution, or flagged
executable re-run.

If vendor clearance or an Avast response arrives later, release remains
blocked until a separate explicit release authorization record is created and
approved.

Until that explicit approval exists, the following remain prohibited:

- release;
- production release tag creation;
- publication;
- package creation or package update;
- package publication;
- distribution artifact creation or update;
- distribution;
- Live E2E;
- live Google Docs mutation;
- live Google Drive mutation;
- token-store mutation for release-path execution;
- flagged executable re-run.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis. It
records the conditions that must be satisfied before release publication,
production release tag creation, production package publication, or live
Google Docs / Drive mutation may proceed.

ADR-0011 does not replace ADR-0003. ADR-0011 records that satisfying or
reviewing ADR-0003 gate conditions still does not itself create release
authorization unless a separate release-governance record explicitly approves
the release target, scope, and permitted operations.

ADR-0008 remains the operational preflight hard stop. ADR-0011 reinforces that
the preflight hard stop remains in force until a separate approved release
authorization record permits named blocked operations.

ADR-0009 remains the evidence bundle and Release Approval Package boundary.
ADR-0011 reinforces that evidence and review recommendations cannot substitute
for the separate release authorization record.

Existing ADR-0001 through ADR-0010 meanings are unchanged.

## Consequences

Release status records must keep Accepted ADRs separate from release approval,
production readiness, vendor clearance, and authorization to execute blocked
operations.

Future status summaries may say that ADR-0011 is Accepted as a docs-only /
local-only governance-boundary decision. They must not state or imply that
release authorization has been created, approval has been granted, release is
permitted, package publication is permitted, tagging is permitted, Live E2E is
permitted, Google Docs / Drive mutation is permitted, distribution is
permitted, or flagged executable re-run is permitted.

The release remains blocked while any required authorization element is absent,
including vendor clearance, Avast response or explicit risk acceptance, final
verification result, explicit approver identity, approval timestamp, approved
release scope, release target/version, or confirmation that named blocked
operations are permitted.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Release authorization record and explicit approval boundary accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/development/Publisher_ReleaseApprovalPackage.md`
- `docs/development/Publisher_AvastResponseIntakeTemplate.md`
- `docs/development/CURRENT_STATUS.md`
- `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`

## Replacement

This ADR does not supersede an earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify implementation behavior.
- This ADR does not modify production code.
- This ADR does not modify tests.
- This ADR does not create or update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not create or update release artifacts, publication artifacts,
  package artifacts, or generated artifacts.
- This ADR does not replace runbooks, release records, verification evidence,
  approval packages, current status records, or the Release Approval Package.
- This ADR does not create a release authorization record.
- This ADR does not approve release, tag, publication, package creation,
  package update, package publication, distribution, Live E2E, Google Docs
  mutation, Google Drive mutation, token-store mutation, flagged executable
  execution, vendor submission, or vendor clearance.
- This ADR does not claim release readiness, vendor clearance, Avast
  false-positive resolution, risk acceptance, final release verification, Live
  E2E authorization, approval granted, or publication completion.
