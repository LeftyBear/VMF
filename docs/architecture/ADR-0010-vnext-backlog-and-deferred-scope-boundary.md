# ADR-0010: vNext Backlog and Deferred Scope Boundary

Status  : Accepted
Date    : 2026-08-06
Scope   : Publisher vNext backlog classification, deferred scope boundary, and separation from v1.0 release authorization
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md

## Context

VMF Publisher maintains `docs/development/Publisher_vNext_Backlog.md` while the
current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. The v1.0 release boundary remains governed by the release records,
runbook, ADR-0003, ADR-0008, and ADR-0009.

The vNext backlog records candidate work, deferred scope, and review ordering
while the release is blocked. It exists so that future work can be triaged
without converting a candidate into adopted v1.0 behavior, release approval,
vendor clearance, or permission to cross the release boundary.

## Decision

`docs/development/Publisher_vNext_Backlog.md` is a planning, triage, sequencing,
and traceability record only.

The backlog classification values are defined as follows:

- P0: release-resume or release-safety items that must be resolved before
  release activity may resume or a blocked release decision can be
  re-evaluated;
- P1: hardening candidates after release-blocking items are resolved;
- P2: enhancement candidates for vNext planning;
- Blocked: work that cannot proceed until its named external dependency or
  release gate condition is satisfied;
- Deferred: work explicitly excluded from the current docs-only / local-only
  backlog scope.

These classifications do not authorize implementation. They do not approve a
vNext feature, reopen the v1.0 release gate, approve release execution, create
vendor clearance, resolve Avast false-positive handling, authorize Live E2E,
authorize Google Docs or Google Drive mutation, authorize package or
distribution artifact updates, authorize tag creation, or authorize
publication.

vNext candidates are not v1.0 release authorization.

Google Picker plus `drive.file` least-privilege routing remains a vNext
reconsideration candidate. It is not an adopted design decision for the current
v1.0 release boundary, and it must not be treated as implemented or approved
behavior unless a later scoped design and implementation task explicitly adopts
it.

Backlog organization is allowed during the current hold only as docs-only /
local-only work. Such organization may clarify deferred scope and future
sequence, but it must preserve Frozen Specifications, public APIs, production
code, tests, package artifacts, distribution artifacts, release records, and
the release-blocked state.

## Relationship To Other ADRs

ADR-0003 records the release gate and vendor-clearance governance basis:
required verification, vendor clearance, Avast false-positive review resolution
or formal repository-owner risk acceptance, explicit release authorization, and
successful final release verification.

ADR-0010 does not replace ADR-0003. ADR-0010 records that backlog
classification and vNext sequencing cannot satisfy the release gate or provide
release authorization.

ADR-0008 records the operational preflight hard stop while Avast handling
remains pending. ADR-0010 stays inside that preflight boundary by allowing only
docs-only / local-only backlog organization during the hold.

ADR-0009 records that evidence bundles and release approval packages are review
records rather than executable release approval. ADR-0010 applies the same
boundary principle to backlog records: planning and triage records do not
authorize release-path operations.

## Consequences

Backlog records must continue to distinguish planning priority from
implementation approval and release authorization.

P0, P1, P2, Blocked, and Deferred labels may be used to guide future review
order. They must not be used as approval labels. Each future implementation,
release, package, Live E2E, Google Docs / Drive, tag, publication, or
vendor-clearance action still requires its own explicit gate and verification
scope.

Current status records and voyage-log entries that mention the vNext backlog
must preserve:

- `Phase 4 local-only verification complete / release blocked`;
- Avast false-positive handling pending;
- vendor clearance not obtained;
- vNext candidates not adopted for v1.0 release authorization;
- Frozen Specification and v1.0 release boundary separation.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-06 | Accepted | vNext backlog and deferred scope boundary accepted as a docs-only / local-only planning-boundary decision while Publisher v1.0 release remains blocked. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/development/Publisher_vNext_Backlog.md`
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
- This ADR does not adopt vNext features.
- This ADR does not adopt Google Picker plus `drive.file` routing.
- This ADR does not create or update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not replace runbooks, release records, verification evidence,
  approval packages, backlog records, or current status records.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, vendor submission, or
  vendor clearance.
- This ADR does not claim release readiness, vendor clearance, Avast
  false-positive resolution, risk acceptance, final release verification, Live
  E2E authorization, approval granted, or publication completion.
