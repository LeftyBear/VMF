# ADR-0013: Release Decision Record and Post-Authorization Traceability

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher release decision record, post-authorization traceability, and separation from release execution and publication records
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/CURRENT_STATUS.md

## Context

ADR-0003 defines the Publisher release gate and vendor-clearance governance
basis.

ADR-0009 defines the Evidence Bundle and Release Approval Package boundary.
Evidence bundles and approval packages support review, but they are not
release artifacts, package artifacts, publication records, or release
authorization by themselves.

ADR-0012 defines the release resumption procedure and final verification order.
It requires explicit release authorization before final release verification
and requires final release verification success before package/dist update,
tag, publication, or release.

After release authorization is granted, the project still needs a durable
decision record linking the approver, rationale, evidence, and resulting
authorized scope. Without that record, later readers can confuse the release
approval / authorization decision with actual release work, the release
artifact, or the publication record.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

## Decision

A Release Decision Record must be created only after release authorization is
granted.

The Release Decision Record must record:

- decision date/time;
- decision owner / authorizer;
- authorized release scope;
- evidence bundle reference;
- final verification reference;
- vendor clearance / Avast resolution reference;
- explicit authorization outcome;
- any accepted residual risk;
- next allowed operation boundary.

The Release Decision Record is not itself a release artifact, package,
publication, tag, deployment, or publication record.

The Release Decision Record must not be backdated. It must not be used to
imply authorization before ADR-0003, ADR-0009, ADR-0012, and any applicable
release-authorization prerequisites are satisfied.

The Release Decision Record must keep these concepts separate:

- release approval / authorization decision;
- actual release work;
- release artifact;
- publication record.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0013 does not weaken or replace the vendor-clearance gate, Avast
false-positive disposition requirement, required verification, explicit release
authorization, or final release verification requirement.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
ADR-0013 uses those records as decision-basis inputs after authorization is
granted, but does not turn evidence or a recommendation into authorization.

ADR-0012 remains the release resumption and final verification order.
ADR-0013 adds the durable post-authorization traceability record that must be
created after release authorization is granted and before downstream readers
can treat the release decision as auditably recorded.

Existing ADR-0001 through ADR-0012 meanings are unchanged.

## Consequences

Post-authorization traceability improves because the project records who
approved release readiness, what decision basis was used, which evidence was
referenced, what scope was authorized, and what boundary applies next.

Approval evidence remains separate from execution evidence. The Release
Decision Record records the authorization decision and its basis. It does not
record package creation, tag creation, publication execution, deployment, or
release completion.

The publication record remains separate and must be created only after actual
publication.

Release remains blocked until ADR-0003, ADR-0009, ADR-0012, and any applicable
authorization prerequisites are met.

Future summaries may say that ADR-0013 is Accepted as a docs-only / local-only
post-authorization traceability decision. They must not state or imply that
release authorization has been granted, vendor clearance has been obtained,
Avast disposition has been accepted, final release verification has succeeded,
package/dist update is permitted, tag creation is permitted, publication is
permitted, or release is permitted.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Release Decision Record and post-authorization traceability accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md`
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
- This ADR does not create a publication record.
- This ADR does not approve release, tag, publication, package creation,
  package update, package publication, distribution, Live E2E, Google Docs
  mutation, Google Drive mutation, token-store mutation, flagged executable
  execution, vendor submission, vendor clearance, or Avast resolution.
- This ADR does not claim release readiness, release authorization, vendor
  clearance, Avast false-positive resolution, risk acceptance, final release
  verification, Live E2E authorization, approval granted, package/dist update,
  tag creation, publication completion, or release completion.
