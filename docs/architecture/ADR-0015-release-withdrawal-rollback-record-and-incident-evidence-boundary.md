# ADR-0015: Release Withdrawal / Rollback Record and Incident Evidence Boundary

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher release withdrawal records, rollback records, incident evidence, and separation from release evidence, approval, authorization, vendor clearance, and republication permission
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md, docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md, docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/CURRENT_STATUS.md

## Context

ADR-0003 defines the Publisher release gate and vendor-clearance governance
basis.

ADR-0008 defines the operational preflight hard stop and release boundary.

ADR-0009 defines the Evidence Bundle and Release Approval Package boundary.
Evidence bundles and approval packages support review, but they are not
release authorization, vendor clearance, or permission to publish.

ADR-0012 defines the release resumption procedure and final verification
order. It requires release gate re-entry and explicit release authorization
before final release verification, package/dist update, tag, publication, or
release.

ADR-0013 defines the Release Decision Record and post-authorization
traceability boundary.

ADR-0014 defines the Release Publication Record and Post-Release Evidence
boundary after publication has occurred.

The project also needs a durable boundary for emergency or corrective records
created when a publication must be withdrawn, a release must be rolled back, or
an incident must be investigated. Without that boundary, later readers can
confuse a withdrawal record, rollback record, or incident evidence bundle with
release evidence, release approval, release authorization, vendor clearance, or
permission to republish.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

## Decision

The project must keep these records separate:

- Release Withdrawal Record;
- Rollback Record;
- Incident Evidence Bundle;
- Release Evidence Bundle;
- Release Approval / Authorization.

A Release Withdrawal Record documents the decision and actions used to remove,
disable, unpublish, revoke, or otherwise withdraw an affected release,
publication, package, artifact, or distribution target.

A Rollback Record documents the decision and actions used to restore a prior
known state, revert a deployment or publication effect, replace an affected
target with an earlier approved target, or otherwise return operators or users
to a previous release state.

An Incident Evidence Bundle documents evidence gathered to investigate,
contain, explain, audit, or follow up on an incident, suspected incident,
publication exception, withdrawal, rollback, or release-governance exception.

A Release Evidence Bundle remains the pre-release evidence boundary defined by
ADR-0009. It supports release review and approval-package preparation. It is
not replaced by incident evidence.

Release Approval / Authorization remains the separate approval and
authorization boundary defined by ADR-0003, ADR-0009, ADR-0011, ADR-0012, and
ADR-0013. It is not created by withdrawal, rollback, or incident evidence.

Withdrawal or rollback records are not:

- release approval;
- release authorization;
- vendor clearance;
- Avast false-positive resolution;
- risk acceptance for a future release;
- permission to republish;
- permission to create or update packages or distribution artifacts;
- permission to create tags;
- permission to execute Live E2E;
- permission to mutate Google Docs or Google Drive;
- permission to re-run flagged executables.

Any re-release, re-publication, package replacement, tag replacement, or
publication restoration after withdrawal or rollback must re-enter the release
gate and verification order defined by prior ADRs. At minimum, it must satisfy
ADR-0003, ADR-0008, ADR-0009, ADR-0012, ADR-0013, and any applicable
release-authorization prerequisite before release-path work resumes.

Incident evidence must follow safe evidence rules. It must not include:

- credentials;
- tokens;
- private URLs;
- raw local paths;
- unredacted logs;
- sensitive Google Docs / Drive identifiers unless explicitly redacted or
  approved.

A Withdrawal or Rollback Record must include, at minimum:

- trigger;
- affected artifact or publication target;
- detection timestamp;
- containment action;
- rollback or withdrawal action;
- verification performed;
- evidence references;
- residual risk;
- follow-up owner;
- final status.

The record must identify whether the action was withdrawal, rollback, or both.
It must identify the affected artifact or publication target without exposing
unsafe local paths, private URLs, credentials, tokens, or unredacted external
identifiers.

The final status must distinguish between containment completion,
withdrawal/rollback completion, residual risk, follow-up required, and release
gate state. It must not use successful withdrawal or rollback as proof that a
future release is approved, authorized, cleared, verified, or publishable.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0015 does not weaken or replace required verification, vendor clearance,
Avast false-positive disposition requirements, explicit release authorization,
or final release verification.

ADR-0008 remains the operational preflight hard stop. ADR-0015 does not reopen
release-path work after a withdrawal or rollback. It requires re-entry through
the established gate and operation-specific authorization boundaries.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
ADR-0015 adds an Incident Evidence Bundle boundary for incident investigation,
withdrawal, rollback, and governance-exception evidence. Incident evidence
cannot replace release evidence or approval-package evidence required before a
future release.

ADR-0012 remains the release resumption and final verification order. ADR-0015
requires any re-release after withdrawal or rollback to follow that order.

ADR-0013 remains the Release Decision Record and post-authorization
traceability boundary. ADR-0015 does not create or replace a Release Decision
Record.

ADR-0014 remains the Release Publication Record and Post-Release Evidence
boundary. ADR-0015 may use a publication record as an input when a published
target is withdrawn or rolled back, but it does not alter the facts recorded
by ADR-0014 and does not turn incident evidence into post-release approval.

Existing ADR-0001 through ADR-0014 meanings are unchanged.

## Consequences

Withdrawal and rollback traceability improves because corrective actions record
the trigger, affected target, timing, containment, action taken, verification,
evidence references, residual risk, owner, and final status.

Incident investigation remains useful without weakening release governance.
Incident Evidence Bundles can document what happened, what was contained, and
what follow-up remains, but they cannot approve, authorize, clear, verify, or
publish a future release.

Re-release after withdrawal or rollback remains blocked until the established
release gate, evidence, approval, authorization, decision, final verification,
and publication boundaries are satisfied in order.

Future summaries may say that ADR-0015 is Accepted as a docs-only / local-only
withdrawal, rollback, and incident-evidence boundary decision. They must not
state or imply that release authorization has been granted, vendor clearance
has been obtained, Avast disposition has been accepted, final release
verification has succeeded, package/dist update is permitted, tag creation is
permitted, publication is permitted, republication is permitted, withdrawal or
rollback has occurred, incident evidence has been collected, or release is
complete.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Release Withdrawal / Rollback Record and Incident Evidence boundary accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md`
- `docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md`
- `docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md`
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
- This ADR does not perform release.
- This ADR does not create a tag.
- This ADR does not publish a package or artifact.
- This ADR does not execute Live E2E.
- This ADR does not mutate Google Docs or Google Drive.
- This ADR does not update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not execute, re-run, package, publish, or distribute a flagged
  executable.
- This ADR does not create a Release Decision Record.
- This ADR does not create a release authorization record.
- This ADR does not create a Publication Record.
- This ADR does not create Post-Release Evidence.
- This ADR does not create a Release Withdrawal Record.
- This ADR does not create a Rollback Record.
- This ADR does not create an Incident Evidence Bundle.
- This ADR does not withdraw a release, roll back a release, republish a
  release, restore a publication, replace a package, or resolve an incident.
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
