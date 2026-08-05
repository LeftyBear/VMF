# ADR-0004: Verified State and Differential Update Safety

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher Verified State baseline, differential update safety, revision conflict handling, physical update ordering, readback verification, and state promotion boundary
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_v1.0_Implementation_Voyage_Log.md

## Context

Publisher differential update depends on a trusted baseline that represents
the last successfully verified document state. Phase 3-2B recorded the
Verified State lifecycle decision, and Phase 3-2C recorded physical update
planning, optimistic concurrency, readback verification, and lifecycle
connection decisions.

Differential update must not use an assumed document state as the basis for
mutation. If the stored baseline, current managed document snapshot, revision,
managed-region topology, or post-apply readback cannot be proven consistent,
the safe result is to stop rather than to publish from uncertain state.

This ADR records the durable architecture decision for Verified State as the
trusted baseline for differential update safety. It does not replace the
Phase 3-2C implementation record, implementation specifications, Frozen
Specifications, public APIs, persisted schema definitions, tests, runbooks, or
release records.

## Decision

Verified State is the trusted baseline state for Publisher differential
updates.

A differential update may be planned only after the stored Verified State is
loaded and matched to the current managed document snapshot. The snapshot must
prove the same document identity, revision baseline, managed-region boundary,
block order, block identity, and `ContentHash` values required by the existing
Phase 3-2B and Phase 3-2C lifecycle decisions.

Revision conflict is a hard stop. If the stored Verified State revision, the
planning snapshot revision, the immediate pre-apply snapshot revision, the
adapter apply precondition, the apply receipt revision, or the post-apply
readback revision violates the expected monotonic relationship, the update is
aborted. The system must not continue by applying a best-effort plan, saving a
new Verified State, or treating the conflict as a successful no-op.

Physical Update Plan application uses the safe ordering recorded in Phase
3-2C:

1. Load the Verified State baseline.
2. Read and validate the current managed document snapshot.
3. Generate the logical `DiffPlan`.
4. Generate the `PhysicalUpdatePlan` from the validated snapshot.
5. Re-read the managed document snapshot immediately before apply.
6. Abort if the pre-apply snapshot no longer matches the prepared snapshot.
7. Apply destructive operations first in descending current document indexes.
8. Rebase surviving anchors by exact deletion lengths.
9. Apply constructive operations in descending Candidate indexes.
10. Read back the managed document after apply.
11. Verify the readback against the Candidate identity, fingerprint, block
    count, block order, managed-region boundary, block ranges, Explicit ID,
    Generated ID, and `ContentHash`.
12. Promote and atomically save the new Verified State only after readback
    verification succeeds.

Readback Verification is mandatory after any mutating physical update. A new
Verified State may be saved only after the post-apply readback proves the
expected Candidate state. If readback fails, mismatches, cannot establish the
sent or unknown-delivery result, or proves a managed-region inconsistency, the
Verified State is not updated.

The Verified State save is the final durable promotion step. It must be
atomic with respect to the state store: the previous Verified State remains
the trusted baseline until the new verified state is completely saved. A
failure during physical planning, application, revision verification, readback
verification, promotion, or save prevents the new state from becoming durable.

## Managed Region And ContentHash Relationship

Managed Region validation protects the physical boundary in the target
document. A differential update must not plan or apply changes when the
managed-region start or exclusive end, block ranges, block order, or block
membership cannot be verified against the stored baseline and current
snapshot.

`ContentHash` protects block content identity within that managed region. It
is part of the block identity evidence used to determine whether a block is
unchanged, updated, inserted, deleted, or moved. A `ContentHash` mismatch is
not treated as harmless drift; it must either be represented by the logical
diff from a verified baseline to the Candidate or cause a safe stop when the
current snapshot no longer matches the baseline required for planning.

The managed-region boundary and `ContentHash` checks are complementary. The
boundary proves where Publisher is allowed to reason about content; the hash
proves whether the expected block content identity is still present inside
that boundary.

## Empty Plan And Safe Stop Relationship

An empty physical plan is safe only when it follows successful baseline,
snapshot, revision, managed-region, and block verification. Empty plan does
not mean verification can be skipped.

`NoChange` may complete without applying mutations and may retain the same
revision, but it still depends on proving that the current managed document
matches the Verified State and Candidate expectations. If that proof fails,
the result is a safe stop, not a successful empty update.

Safe stops include, but are not limited to:

- revision conflict;
- managed-region mismatch;
- physical plan invalidity;
- update application failure;
- readback failure;
- readback mismatch;
- state verification mismatch;
- unsupported Verified State schema.

Safe stop results must not save a new Verified State.

## Relationship To Earlier ADRs

ADR-0001 governs how this ADR is recorded, indexed, accepted, and later
superseded. ADR-0004 follows that process and remains subordinate to higher
priority specifications.

ADR-0002 records the local operator authentication decision for authorized
Google API workflows. ADR-0004 does not change authentication mode, OAuth
scope, token-store handling, or Service Account support.

ADR-0003 records the release gate and vendor-clearance boundary. ADR-0004 does
not reopen the release gate, resolve Avast false-positive handling, obtain
vendor clearance, authorize release, authorize Live E2E, or authorize Google
Docs / Drive mutation.

Phase 3-2B and Phase 3-2C voyage-log records remain the detailed development
records for Verified State lifecycle and physical update planning and
verification. ADR-0004 preserves the durable safety decision and points
reviewers back to those records for phase-specific implementation history and
evidence.

## Consequences

Publisher update safety is defined by verified evidence, not by intent to
publish or by the presence of a Candidate document.

Differential update cannot silently recover from revision or managed-region
uncertainty by applying a partial or best-effort plan.

Readback Verification becomes the required promotion gate between physical
mutation and Verified State persistence.

Future implementation, review, and recovery work must preserve the ordering
that prevents an unverified or failed update from becoming the durable
baseline for later differential updates.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Proposed | Initial Verified State and differential update safety ADR drafted as docs-only / local-only documentation. |
| 2026-08-05 | Accepted | Verified State accepted as the trusted baseline for differential update safety and state promotion. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0001-architecture-decision-record-process.md`
- `docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`
- `docs/development/CURRENT_STATUS.md`

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
- This ADR does not replace implementation specifications, Phase 3-2B or
  Phase 3-2C development records, runbooks, release records, verification
  evidence, or current status records.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, or vendor submission.
- This ADR does not claim release readiness, vendor clearance, Avast
  false-positive resolution, risk acceptance, final release verification, or
  publication completion.
