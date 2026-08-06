# ADR-0012: Release Resumption Procedure and Final Verification Order

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher release resumption procedure, final verification order, and post-clearance release boundary
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/CURRENT_STATUS.md

## Context

VMF Publisher remains in the formal state:

`Phase 4 local-only verification complete / release blocked`.

The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

ADR-0003 records the release gate and vendor-clearance governance basis.
ADR-0008 records the operational preflight hard stop. ADR-0009 records the
Evidence Bundle and Release Approval Package as evidence and review records,
not release authorization. ADR-0011 records that release authorization must be
a separate release-governance record, not an ADR.

ADR-0012 defines the order for resuming release consideration after the
external security-classification inputs arrive. It applies only after both of
the following are true:

- vendor clearance is obtained; and
- Avast response / false-positive disposition is received and reviewed.

Vendor clearance alone is not release authorization. Avast response or
false-positive disposition alone is not release authorization. A Release
Approval Package is not approval by itself.

## Decision

After vendor clearance is obtained and Avast response / false-positive
disposition is received and reviewed, release resumption must follow this
order:

1. Intake and preserve vendor / Avast response evidence.
2. Validate evidence authenticity and scope.
3. Confirm no remaining release blockers.
4. Re-run approved local verification only.
5. Review Evidence Bundle completeness and redaction.
6. Prepare Release Approval Package.
7. Record explicit release authorization decision.
8. Only after authorization, permit final release verification.
9. Only after final verification success, permit package/dist update, tag,
   publication, and release.

The recommendation remains `Hold` until an explicit release authorization
decision is recorded.

Release resumption must stop and return to `Hold` if any ambiguity, mismatch,
missing evidence, remaining blocker, or failed final verification is found.
Returning to `Hold` preserves the ADR-0008 preflight hard stop and prevents
promotion of incomplete evidence into release-path authorization.

Final release verification is not permitted before explicit release
authorization is recorded. Package/dist update, production tag creation,
publication, and release are not permitted before final verification succeeds.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0012 does not weaken or replace the vendor-clearance gate; it records the
post-clearance order that must still be followed before release-path work may
resume.

ADR-0008 remains the operational preflight hard stop. ADR-0012 preserves the
hard stop until the required external evidence has been received, reviewed,
validated, and followed by explicit release authorization.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
ADR-0012 preserves the rule that evidence completeness, redaction, and approval
package preparation support review but do not themselves approve release.

ADR-0011 remains the explicit release authorization boundary. ADR-0012 uses
that boundary by requiring the release authorization decision before final
release verification and before any package/dist update, tag, publication, or
release.

Existing ADR-0001 through ADR-0011 meanings are unchanged.

## Consequences

Status records must keep the release state at `Hold` until the explicit
release authorization decision is recorded.

Future summaries may say that ADR-0012 is Accepted as a docs-only / local-only
release-resumption-order decision. They must not state or imply that vendor
clearance has been obtained, Avast disposition has been accepted, release
authorization has been recorded, final release verification is permitted,
package/dist update is permitted, tag creation is permitted, publication is
permitted, or release is permitted.

Any ambiguity in vendor or Avast evidence, mismatch between evidence and the
release target, missing evidence, unresolved blocker, incomplete redaction,
missing approval decision, or failed final verification returns the state to
`Hold`.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Release resumption procedure and final verification order accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md`
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
  package artifacts, distribution artifacts, or generated artifacts.
- This ADR does not mutate Google Docs or Google Drive.
- This ADR does not execute Live E2E.
- This ADR does not execute, re-run, package, publish, or distribute a flagged
  executable.
- This ADR does not create a release authorization record.
- This ADR does not approve release, tag, publication, package creation,
  package update, package publication, distribution, Live E2E, Google Docs
  mutation, Google Drive mutation, token-store mutation, flagged executable
  execution, vendor submission, or vendor clearance.
- This ADR does not claim release readiness, vendor clearance, Avast
  false-positive resolution, risk acceptance, final release verification, Live
  E2E authorization, approval granted, package/dist update, tag creation,
  publication completion, or release completion.
