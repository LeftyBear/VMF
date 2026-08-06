# ADR-0016: Release Versioning / Tag / Artifact Identity

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher release versioning, tag identity, package and artifact identity, evidence bundle identity, approval record identity, and separation from release authorization, final verification, package work, tag creation, and vendor clearance
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md, docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md, docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md, docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md, docs/architecture/ADR-0015-release-withdrawal-rollback-record-and-incident-evidence-boundary.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/CURRENT_STATUS.md

## Context

ADR-0003 defines the Publisher release gate and vendor-clearance governance
basis. ADR-0008 defines the operational preflight hard stop and release
boundary. ADR-0009 defines the Evidence Bundle and Release Approval Package
boundary. Evidence bundles and approval packages support review, but they are
not release authorization, vendor clearance, tag approval, or permission to
publish.

ADR-0011 defines that release authorization must be a separate
release-governance record, not an ADR. ADR-0012 defines the release resumption
procedure and final verification order. ADR-0013 defines the Release Decision
Record and post-authorization traceability boundary. ADR-0014 defines the
Release Publication Record and Post-Release Evidence boundary. ADR-0015
defines withdrawal, rollback, and incident evidence boundaries.

The project also needs a durable boundary for release identity before any
future release resumes. Without this boundary, later records can use ambiguous
or mutable identities such as `latest`, local build folders, unverifiable
package names, ad hoc ZIP names, or artifacts that cannot be traced back to a
specific commit, evidence bundle, and approval or authorization record.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

## Decision

Future release records must identify these canonical release identity fields:

- release version;
- git commit;
- git tag;
- artifact or package identity;
- evidence bundle identity;
- approval or authorization record identity.

These fields are identity controls. They are not, by themselves, release
approval, release authorization, vendor clearance, final verification success,
publication approval, package approval, tag approval, or production readiness.

No tag or artifact is created by this ADR. No package identity is finalized by
this ADR. No release version is approved by this ADR. No evidence bundle,
approval record, authorization record, decision record, publication record, or
post-release evidence is created by this ADR.

Tag, package, and artifact identity must be derived only after vendor clearance
or required repository-owner risk acceptance is recorded, release gate re-entry
is authorized, final release authorization is recorded, final verification
succeeds in the authorized release scope, the exact commit and release scope
are fixed, and the package or distribution artifact is produced by the
authorized release process.

Release identity must not be derived from:

- `latest`;
- local build folders;
- mutable package names;
- unverified ZIP or executable names;
- unstaged or uncommitted working-tree state;
- unpublished local paths;
- temporary folders;
- private Google Docs / Drive URLs;
- package names or paths that cannot be tied to a specific commit and
  verification record;
- artifacts whose hash, size, version, source commit, or production procedure
  cannot be verified.

When a future release is authorized and completed, release traceability must be
preserved from:

- git tag;
- git commit;
- package or distribution artifact;
- evidence bundle;
- approval or authorization record.

A release record, publication record, archive record, withdrawal record,
rollback record, or incident record must not replace, rename, or reinterpret an
earlier release identity in a way that breaks tag to commit to package or
artifact to evidence bundle to approval record traceability.

If any identity field is missing, ambiguous, mutable, unverifiable, derived too
early, or inconsistent with the final verification result, the release state
remains Hold and release-path work must stop until the mismatch is resolved
under the release gate.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0016 does not weaken required verification, vendor clearance, Avast
false-positive disposition requirements, explicit release authorization, final
release verification, or owner approval.

ADR-0008 remains the operational preflight hard stop. ADR-0016 does not reopen
release-path work and does not authorize package/dist updates, release, tag
creation, publication, Live E2E, Google Docs / Drive mutation, or flagged
executable re-run.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
ADR-0016 requires future release identity to reference those records after
they exist, but identity traceability does not convert evidence or an approval
package into release authorization or vendor clearance.

ADR-0011 remains the explicit release authorization boundary. ADR-0016 may
identify a future approval or authorization record after one exists, but it
does not create or approve that record.

ADR-0012 remains the release resumption and final verification order. ADR-0016
requires tag, package, and artifact identity to be derived only after final
release authorization and final verification in that order.

ADR-0013 remains the Release Decision Record and post-authorization
traceability boundary. ADR-0016 supplies the identity fields that future
decision records and later release records must preserve when release resumes.

ADR-0014 remains the Release Publication Record and Post-Release Evidence
boundary. ADR-0016 does not create publication evidence and does not imply
publication occurred.

ADR-0015 remains the withdrawal, rollback, and incident evidence boundary. Any
future withdrawal, rollback, or incident record must preserve the canonical
release identity chain instead of replacing it with ambiguous or mutable
labels.

ADR-0016 fills the previously absent numbering slot. It does not change,
supersede, renumber, weaken, or reinterpret ADR-0017 or ADR-0018. Existing
ADR-0001 through ADR-0015, ADR-0017, and ADR-0018 meanings are unchanged.

## Consequences

Future release records must use stable, verifiable identity fields instead of
ambiguous labels or mutable paths.

Release traceability improves because tag, commit, package or artifact,
evidence bundle, and approval or authorization record identities must align
after release authorization and final verification.

While Avast false-positive handling remains pending and vendor clearance has
not been obtained, current records may only preserve the Hold state. They must
not imply approval granted, vendor clearance obtained, final verification
complete, package/dist update permitted, tag creation permitted, publication
permitted, or release complete.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Release versioning, tag, and artifact identity boundary accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

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
- `docs/architecture/ADR-0017-release-retention-archival-audit-trail.md`
- `docs/architecture/ADR-0018-emergency-release-exception-boundary.md`
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
- This ADR does not mutate token stores.
- This ADR does not update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not execute, re-run, package, publish, or distribute a flagged
  executable.
- This ADR does not create release evidence.
- This ADR does not create an Evidence Bundle.
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
  withdrawal/rollback completion, incident resolution, artifact creation,
  package creation, artifact identity finalization, version approval, normal
  release gate reopening, or release completion.
