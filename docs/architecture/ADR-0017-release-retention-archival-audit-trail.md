# ADR-0017: Release Retention / Archival / Audit Trail

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher release evidence retention, archival responsibility, audit trail traceability, and separation from release authorization, publication, package work, and vendor clearance
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md, docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md, docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md, docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md, docs/architecture/ADR-0015-release-withdrawal-rollback-record-and-incident-evidence-boundary.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/CURRENT_STATUS.md

## Context

ADR-0003 defines the Publisher release gate and vendor-clearance governance
basis.

ADR-0008 defines the operational preflight hard stop and release boundary.

ADR-0009 defines the Evidence Bundle and Release Approval Package boundary.
Evidence bundles and approval packages support review, but they are not
release authorization, vendor clearance, or permission to publish.

ADR-0011 defines that release authorization must be a separate
release-governance record, not an ADR.

ADR-0012 defines the release resumption procedure and final verification
order.

ADR-0013 defines the Release Decision Record and post-authorization
traceability boundary.

ADR-0014 defines the Release Publication Record and Post-Release Evidence
boundary.

ADR-0015 defines withdrawal, rollback, and incident evidence boundaries.

The project also needs a durable boundary for retaining and archiving release
evidence after it is collected or prepared. Without this boundary, later
readers can confuse archival with release approval, package publication,
production readiness, vendor clearance, or permission to resume release-path
work.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

## Decision

Release evidence, approval packages, vendor clearance responses, final
verification records, release authorization records, release decision records,
publication records, post-release evidence, withdrawal records, rollback
records, and incident evidence bundles must be retained as immutable audit
evidence once finalized.

Archival is documentation and evidence preservation. It is not release
authorization, release approval, package approval, publication approval,
vendor clearance, Avast false-positive resolution, Live E2E authorization,
Google Docs / Drive mutation authorization, tag authorization, or production
readiness.

An archived release audit trail must preserve traceability from:

- release authorization or release decision record;
- final verification record;
- vendor clearance or Avast disposition evidence;
- Release Approval Package;
- Evidence Bundle;
- package, distribution artifact, tag, release, or publication identifiers
  when those identifiers exist and are authorized to be recorded.

Until vendor clearance is obtained and recorded, archival may record only the
current Hold state. It must preserve:

- release blocked;
- Avast pending;
- vendor clearance not obtained;
- `Approval Recommendation = Hold`.

No archive entry may state or imply release approval, publication,
distribution, package approval, production readiness, vendor clearance, Avast
resolution, final verification success, release completion, or permission to
resume release-path work unless the applicable release gate conditions have
already been satisfied and the corresponding source record exists.

Archived evidence must preserve record identity, version, date, owner or
operator when known, source record reference, and relationship to later release
records. It must not alter the meaning of the retained evidence or rewrite a
Hold, Pending, Deferred, Blocked, Failed, or Not Executed state into Pass,
Approved, Cleared, Ready, Published, Released, or Complete.

Archived evidence must follow safe evidence rules. It must not include:

- credentials;
- tokens;
- private keys;
- private URLs;
- raw local paths;
- raw exception bodies;
- raw HTTP bodies;
- stack traces;
- unredacted logs;
- sensitive Google Docs / Drive identifiers unless explicitly redacted or
  approved.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0017 does not weaken required verification, vendor clearance, explicit
release authorization, final release verification, or owner approval.

ADR-0008 remains the operational preflight hard stop. ADR-0017 does not reopen
release-path work and does not authorize package/dist updates, release, tag
creation, publication, Live E2E, Google Docs / Drive mutation, or flagged
executable re-run.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
ADR-0017 requires those records to be retained immutably after finalization,
but retention does not convert them into release authorization or vendor
clearance.

ADR-0011 remains the explicit release authorization boundary. ADR-0017 may
retain a release authorization record after it exists, but it does not create
one.

ADR-0012 remains the release resumption and final verification order. ADR-0017
may retain final verification records after they exist, but it does not permit
final release verification before the required prior gates are satisfied.

ADR-0013 remains the Release Decision Record and post-authorization
traceability boundary. ADR-0017 preserves the audit trail from a future
Release Decision Record to verification, vendor clearance, and package/release
identifiers when those records exist.

ADR-0014 remains the Release Publication Record and Post-Release Evidence
boundary. ADR-0017 may retain publication and post-release records after they
exist, but it does not create publication evidence or imply publication
occurred.

ADR-0015 remains the withdrawal, rollback, and incident evidence boundary.
ADR-0017 may retain those records after they exist, but retention does not
authorize republication or release restoration.

Existing ADR-0001 through ADR-0015 meanings are unchanged.

## Consequences

Release audit evidence remains traceable and reviewable without weakening the
release gate.

Future archive summaries must identify whether a record is evidence,
approval-package material, vendor response, final verification, authorization,
decision, publication, post-release evidence, withdrawal, rollback, or incident
evidence. They must not collapse those responsibilities into a single approval
or readiness claim.

While Avast false-positive handling remains pending and vendor clearance has
not been obtained, archive records may only preserve the Hold state. They must
not imply approval granted, vendor clearance obtained, final verification
complete, package/dist update permitted, tag creation permitted, publication
permitted, or release complete.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Release retention, archival, and audit trail boundary accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md`
- `docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md`
- `docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md`
- `docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md`
- `docs/architecture/ADR-0015-release-withdrawal-rollback-record-and-incident-evidence-boundary.md`
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
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not modify implementation behavior.
- This ADR does not modify production code.
- This ADR does not modify tests.
- This ADR does not perform release.
- This ADR does not create a tag.
- This ADR does not publish a package or artifact.
- This ADR does not execute Live E2E.
- This ADR does not mutate Google Docs or Google Drive.
- This ADR does not update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not execute, re-run, package, publish, or distribute a flagged
  executable.
- This ADR does not create release evidence.
- This ADR does not create a Release Approval Package.
- This ADR does not create a vendor clearance response.
- This ADR does not create a final verification record.
- This ADR does not create a release authorization record.
- This ADR does not create a Release Decision Record.
- This ADR does not create a Publication Record.
- This ADR does not create Post-Release Evidence.
- This ADR does not create a Withdrawal Record.
- This ADR does not create a Rollback Record.
- This ADR does not create an Incident Evidence Bundle.
- This ADR does not approve release, tag, publication, republication, package
  creation, package update, package publication, distribution, Live E2E,
  Google Docs mutation, Google Drive mutation, token-store mutation, flagged
  executable execution, vendor submission, vendor clearance, or Avast
  resolution.
- This ADR does not claim release readiness, release authorization, vendor
  clearance, Avast false-positive resolution, risk acceptance, final release
  verification, Live E2E authorization, approval granted, package/dist update,
  tag creation, publication completion, republication permission,
  withdrawal/rollback completion, incident resolution, or release completion.
