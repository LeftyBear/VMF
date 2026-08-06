# ADR-0018: Emergency Release Exception Boundary

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher emergency release exception boundary, required authority and evidence, risk acceptance, rollback planning, post-incident review, and separation from normal release gate reopening
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md, docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md, docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md, docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md, docs/architecture/ADR-0015-release-withdrawal-rollback-record-and-incident-evidence-boundary.md, docs/architecture/ADR-0017-release-retention-archival-audit-trail.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/CURRENT_STATUS.md

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

ADR-0017 defines retention, archival, and audit trail boundaries.

The project also needs a durable boundary for emergency release exception
consideration. Without this boundary, later readers can confuse emergency
exception discussion with normal release gate reopening, vendor clearance,
Avast false-positive resolution, release authorization, package approval,
publication approval, or permission to execute blocked operations.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

## Decision

An emergency release exception is not normal release gate reopening. It does
not clear Avast pending, does not obtain vendor clearance, does not change
`Approval Recommendation = Hold`, and does not convert a blocked release into
an approved release path.

Emergency release exception consideration is allowed only when all of these
inputs are explicitly recorded before any exception-dependent operation:

- emergency authority and decision owner;
- emergency condition or incident requiring consideration;
- exact exception scope, including version, commit, artifact, operation, and
  destination boundaries when applicable;
- risk acceptance that names the unresolved release-gate conditions;
- evidence supporting the emergency condition and proposed exception;
- rollback, withdrawal, containment, or restoration plan;
- communication and operator responsibility;
- post-incident review requirement;
- traceability to a future ADR or release decision record.

Missing, ambiguous, informal, or partial exception input means no exception is
approved.

Unless an emergency exception is explicitly approved and recorded with the
required authority, scope, risk acceptance, evidence, rollback plan, and
post-incident review requirement, the following remain prohibited:

- release;
- tag creation;
- publication;
- package creation or update;
- distribution artifact update;
- writing to `dist`;
- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- token-store mutation;
- flagged executable re-run;
- vendor submission or re-submission that requires executing or distributing
  the flagged executable.

An emergency exception, if approved in the future, must be narrow. It may
authorize only the named operation, target, artifact, destination, operator,
and time window. It must not authorize adjacent release-path work by
implication.

An emergency exception must not become a permanent precedent. After the
emergency is contained, the exception must be followed by a post-incident
review and tracked by a later ADR or release decision record that records the
actual facts, scope used, evidence, residual risk, rollback or withdrawal
result, and any required follow-up.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0018 does not weaken required verification, vendor clearance, Avast
false-positive disposition requirements, explicit release authorization, final
release verification, or owner approval.

ADR-0008 remains the operational preflight hard stop. ADR-0018 records an
exception boundary only; it does not reopen release-path work and does not
authorize package/dist updates, release, tag creation, publication, Live E2E,
Google Docs / Drive mutation, or flagged executable re-run.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
Emergency exception evidence cannot convert an Evidence Bundle or Release
Approval Package into release authorization or vendor clearance.

ADR-0011 remains the explicit release authorization boundary. Emergency
exception approval, if ever granted, must be separately recorded and must not
be inferred from an Accepted ADR.

ADR-0012 remains the release resumption and final verification order for
normal release gate reopening. ADR-0018 does not replace that order and does
not allow emergency exception use to satisfy normal release resumption.

ADR-0013 remains the Release Decision Record and post-authorization
traceability boundary. ADR-0018 requires a future emergency exception to be
traceable through a later ADR or release decision record, but it does not
create that record.

ADR-0014 remains the Release Publication Record and Post-Release Evidence
boundary. ADR-0018 does not create publication evidence and does not imply
publication occurred.

ADR-0015 remains the withdrawal, rollback, and incident evidence boundary.
Emergency exception rollback, withdrawal, containment, or incident evidence
must remain traceable to that boundary.

ADR-0017 remains the retention, archival, and audit trail boundary. Emergency
exception records, if created later, must be retained without rewriting Hold,
Pending, Blocked, Not Executed, or risk-accepted states into cleared or
approved states.

Existing ADR-0001 through ADR-0017 meanings are unchanged.

## Consequences

Emergency release exception handling becomes explicit and auditable without
weakening the normal release gate.

Future emergency records must distinguish exception consideration, exception
approval, risk acceptance, operation-specific authorization, rollback or
withdrawal planning, post-incident review, and normal release authorization.
They must not collapse those responsibilities into a single release approval
or readiness claim.

While Avast false-positive handling remains pending and vendor clearance has
not been obtained, current records may only preserve the Hold state unless a
separate emergency exception approval record is created with all required
inputs. Without that record, package/dist update, release, tag creation,
publication, Live E2E, Google Docs / Drive mutation, and flagged executable
re-run remain blocked.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Emergency release exception boundary accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

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
- This ADR does not create an emergency exception approval record.
- This ADR does not create release evidence.
- This ADR does not create a Release Approval Package.
- This ADR does not create a vendor clearance response.
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
  executable execution, vendor submission, vendor clearance, Avast resolution,
  or emergency exception use.
- This ADR does not claim release readiness, release authorization, vendor
  clearance, Avast false-positive resolution, risk acceptance, final release
  verification, Live E2E authorization, approval granted, package/dist update,
  tag creation, publication completion, republication permission,
  withdrawal/rollback completion, incident resolution, emergency exception
  approval, normal release gate reopening, or release completion.
