# ADR-0003: Release Gate and Vendor Clearance

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher release gate, vendor clearance, antivirus false-positive handling, release authorization, and publication boundaries
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR_INDEX.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_PreflightHardening.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/distribution/PublisherReleaseRunbook.md

## Context

VMF Publisher has local-only verification records, release records, runbooks,
and approval-package documents that separate completed local evidence from
release readiness. The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling is pending, and vendor clearance has not been
obtained. Local verification and documentation updates are not enough to reopen
the release path, create production tags, publish packages, or mutate live
Google Docs or Google Drive resources.

Runbooks describe operational procedure. This ADR records the long-term
governance decision: Publisher release activities require explicit gate
completion, vendor-clearance treatment, and release authorization before any
production release action is allowed.

## Decision

Publisher release control uses a closed release gate. The release gate remains
blocked until every required release condition is satisfied and recorded in the
applicable release records.

The required release conditions are:

- required verification succeeds within the authorized verification scope;
- vendor clearance is obtained;
- the Avast false-positive review is resolved by vendor response or formally
  accepted by the repository owner as an explicit risk decision;
- explicit release authorization is recorded;
- final release verification succeeds after the selected release artifact,
  release scope, and release authorization are fixed.

Until all release conditions are satisfied, the following operations are
prohibited:

- release publication;
- production release tag creation;
- production package publication;
- unauthorized Live Google Docs mutation;
- unauthorized Live Google Drive mutation.

The current state is release blocked. Avast false-positive handling remains
pending. Vendor clearance has not been obtained.

This ADR does not grant release authorization, does not obtain vendor
clearance, does not resolve the Avast false-positive review, and does not
accept antivirus risk. Those outcomes must be recorded separately through the
applicable release records and repository-owner authorization path.

## Governance Boundary

Runbooks remain the operational source for step-by-step execution, command
order, evidence collection, and resume procedure. This ADR does not replace the
runbook and does not provide an executable release procedure.

The ADR records the durable governance rule that release, tag, package
publication, and live mutation boundaries stay closed until the required
release conditions are complete. Operational records may provide current
status, but they must not weaken this governance boundary.

Documentation-only work may update ADRs, status records, runbooks, and release
records without crossing the release gate when the task explicitly authorizes
that documentation scope. Such documentation work must continue to state when
release remains blocked, Avast handling remains pending, and vendor clearance
has not been obtained.

## Consequences

Local-only verification can be complete while release readiness remains
unestablished.

Vendor clearance and antivirus false-positive disposition are treated as
release-gate prerequisites, not as optional evidence notes.

Production release activities require both completed evidence and explicit
authorization. A passing local build, local test run, static package
inspection, documentation update, or approval-package draft cannot by itself
authorize release publication, production tagging, package publication, Live
E2E, or Google Docs / Drive mutation.

Release records, current-status documents, and voyage logs must continue to
separate completed documentation or local verification from release readiness,
vendor clearance, risk acceptance, and final publication approval.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Proposed | Initial release-gate and vendor-clearance governance ADR drafted as docs-only / local-only documentation. |
| 2026-08-05 | Accepted | Release gate and vendor clearance accepted as long-term Publisher release governance boundaries. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0001-architecture-decision-record-process.md`
- `docs/development/CURRENT_STATUS.md`
- `docs/development/Publisher_PreflightHardening.md`
- `docs/development/Publisher_AvastResponseIntakeTemplate.md`
- `docs/development/Publisher_ReleaseApprovalPackage.md`
- `docs/distribution/PublisherReleaseRunbook.md`

## Replacement

This ADR does not supersede an earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify implementation behavior.
- This ADR does not modify tests.
- This ADR does not create or update packages or distribution artifacts.
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not replace runbooks, release records, verification evidence,
  or current status records.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, or vendor submission.
- This ADR does not claim release readiness, vendor clearance, Avast
  false-positive resolution, risk acceptance, final release verification, or
  publication completion.
