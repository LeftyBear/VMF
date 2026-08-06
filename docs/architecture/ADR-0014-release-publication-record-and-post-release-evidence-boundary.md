# ADR-0014: Release Publication Record and Post-Release Evidence Boundary

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher release publication record, post-release evidence handling, and separation from approval, authorization, gates, and publication execution
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md, docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/CURRENT_STATUS.md

## Context

ADR-0009 defines the Evidence Bundle and Release Approval Package boundary.
Evidence bundles and approval packages support review, but they are not
release artifacts, package artifacts, publication records, or release
authorization by themselves.

ADR-0013 defines the Release Decision Record and post-authorization
traceability boundary. A Release Decision Record records the release
authorization decision and its basis after authorization is granted. It does
not record actual publication and is not a publication record.

After a release is actually published, the project needs a separate durable
Publication Record that records the fact of publication and the identifiers
needed for later audit. The project also needs a fixed boundary for
post-release evidence so later observations cannot be confused with
pre-release approval, release authorization, required gate completion, or the
Release Decision Record.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The current Release Approval Package recommendation remains:

`Approval Recommendation = Hold`.

## Decision

A Release Publication Record must be created only after actual publication has
occurred.

The Release Publication Record is the record of the facts actually published.
It must identify, as applicable:

- publication date/time;
- publication operator;
- published version;
- published commit, tag, or release identifier;
- published package or distribution artifact identity;
- publication destination;
- publication command or workflow reference;
- linked Release Decision Record or authorization reference;
- post-publication verification or observation references.

The Release Publication Record is not itself:

- pre-release evidence;
- a release approval package;
- release authorization;
- a required gate;
- vendor clearance;
- Avast false-positive resolution;
- permission to publish.

Post-Release Evidence is evidence collected after publication. It may include
post-publication observations, download or install checks, audit notes,
publication-system confirmations, external scanner results, or follow-up
verification logs.

Post-Release Evidence must be classified as post-release observation,
confirmation, or audit evidence. It must not be used to retroactively satisfy
or repair:

- pre-release approval;
- release authorization;
- required release gates;
- vendor clearance;
- Avast false-positive resolution;
- final release verification required before publication;
- Release Decision Record completeness.

Gate-missing or authorization-missing release work must not be justified by
later Post-Release Evidence. A release that occurred before a required gate,
approval, authorization, or final verification was satisfied remains a
governance exception or incident candidate; later observations may document
what happened, but they do not convert the earlier release into an approved or
authorized release.

The project must keep these records separate:

- Evidence Bundle;
- Release Approval Package;
- Release Authorization;
- Release Decision Record;
- Release Publication Record;
- Post-Release Evidence.

## Relationship To Other ADRs

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0014 does not weaken or replace required verification, vendor clearance,
Avast false-positive disposition requirements, explicit release authorization,
or final release verification.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
ADR-0014 adds the downstream boundary for records created after publication and
does not turn post-release observations into pre-release evidence or approval.

ADR-0012 remains the release resumption and final verification order.
ADR-0014 does not alter the order required before package/dist update, tag,
publication, or release.

ADR-0013 remains the Release Decision Record and post-authorization
traceability boundary. ADR-0014 follows ADR-0013 by defining the record created
only after actual publication and by fixing how evidence gathered after
publication must be treated.

Existing ADR-0001 through ADR-0013 meanings are unchanged.

## Consequences

Publication traceability improves because the project records what was
actually published, where it was published, who performed publication, and
which release authorization or decision record permitted that publication.

Post-release audit remains useful without weakening release governance.
Post-Release Evidence can confirm, inspect, or investigate a published release,
but it cannot replace missing approval, authorization, vendor clearance, final
verification, or required gate evidence.

Release remains blocked until ADR-0003, ADR-0009, ADR-0012, ADR-0013, and any
applicable authorization prerequisites are met.

Future summaries may say that ADR-0014 is Accepted as a docs-only / local-only
publication-record and post-release-evidence boundary decision. They must not
state or imply that release authorization has been granted, vendor clearance
has been obtained, Avast disposition has been accepted, final release
verification has succeeded, package/dist update is permitted, tag creation is
permitted, publication is permitted, publication has occurred, or release is
complete.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | Release Publication Record and Post-Release Evidence boundary accepted as a docs-only / local-only governance-boundary decision while Publisher v1.0 release remains blocked. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md`
- `docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md`
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
- This ADR does not approve release, tag, publication, package creation,
  package update, package publication, distribution, Live E2E, Google Docs
  mutation, Google Drive mutation, token-store mutation, flagged executable
  execution, vendor submission, vendor clearance, or Avast resolution.
- This ADR does not claim release readiness, release authorization, vendor
  clearance, Avast false-positive resolution, risk acceptance, final release
  verification, Live E2E authorization, approval granted, package/dist update,
  tag creation, publication completion, post-release verification, or release
  completion.
