# ADR-0008: Preflight Hard Stop and Release Boundary

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher preflight hard stops, Avast-pending release boundary, local-only allowed work, and release-resume gate
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_PreflightHardening.md, docs/distribution/PublisherReleaseRunbook.md, docs/development/Publisher_TestClassification.md, docs/development/CURRENT_STATUS.md

## Context

VMF Publisher has completed the approved Phase 4 local-only verification
scope, but the current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Vendor clearance has not been
obtained. Local-only verification, documentation records, runbook updates,
ADR acceptance, static inspection, build results, unit tests, mock-backed
verification, and dry-run evidence must not be interpreted as release
authorization.

`docs/development/Publisher_PreflightHardening.md`,
`docs/distribution/PublisherReleaseRunbook.md`, and
`docs/development/Publisher_TestClassification.md` already define the
operational stop conditions used while Avast handling remains pending. This
ADR fixes that operating boundary as an Accepted architecture decision so that
future release work cannot accidentally treat local-only evidence as a release
gate opening.

## Decision

Publisher release control requires a preflight hard stop before any operation
that could cross from local-only verification into release-path work.

The preflight hard stop exists to:

- prevent accidental bypass of the release boundary;
- stop dangerous operations while Avast false-positive handling is pending and
  vendor clearance has not been obtained;
- keep local-only verification separate from release authorization.

While Avast false-positive handling remains pending and vendor clearance has
not been obtained, the following operations remain prohibited:

- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- package creation or package update;
- distribution artifact update;
- release;
- tag creation;
- publication;
- flagged executable re-run.

Allowed work during the current hold is limited to local-only, non-releasing
activity:

- documentation updates;
- source build when it does not create or update packages;
- unit tests;
- mock-backed verification;
- dry-run verification that does not publish and does not execute the flagged
  package executable;
- static inspection;
- non-changing confirmation of existing package contents when explicitly in
  scope and no executable is run.

Any document, ADR, status record, verification result, checklist, runbook, or
approval package created while vendor clearance is not obtained must preserve
the release-blocked state. It must not claim or imply release authorization,
vendor clearance, Avast resolution, Live E2E execution permission, package
approval, tag authorization, publication approval, or release readiness.

## Resume Gate

Release-path work may resume only after the resume gate is rechecked according
to the runbook and all required evidence is recorded.

The minimum resume conditions are:

- Avast false-positive handling is complete for the exact selected artifact;
- vendor clearance is explicitly obtained through the ADR-0003 evidence and
  responsible-owner approval criteria, or another repository-owner decision
  path recorded by ADR-0003 is explicitly completed;
- the runbook resume gate is reread and revalidated before release-path work
  continues;
- required re-verification completes within the newly authorized scope;
- each later operation-specific gate is separately authorized before package
  work, flagged executable smoke, Live E2E, tag creation, publication, or
  release execution.

Authorization for one gate does not authorize any other gate.

No Avast response, including no response at all, is sufficient by itself to
clear the hard stop. Until latest-definition rescan evidence, detection-removal
or non-reproduction evidence, and responsible-owner approval are recorded for
the selected artifact identity, the release boundary must continue to preserve
the blocked or hold-equivalent vendor-clearance state.

## Relationship To Other ADRs

ADR-0003 records the release gate and vendor-clearance governance basis:
required verification, vendor clearance, Avast false-positive disposition or
formal owner risk decision, explicit release authorization, and final release
verification before release publication, production tag creation, production
package publication, or unauthorized live mutation may proceed.

ADR-0008 does not replace ADR-0003. ADR-0008 fixes the operational boundary
that enforces ADR-0003 before release work begins: preflight hard stop,
local-only allowed scope, Avast-pending prohibited operations, and resume-gate
requirements.

ADR-0005 remains responsible for retry policy and failure classification.
ADR-0006 remains responsible for diagnostic logging and safe observability.
ADR-0007 remains responsible for CLI error handling and the stable failure
surface.

ADR-0008 is responsible for release boundary, preflight stop, and operational
gate enforcement. It does not change retry policy, diagnostic logging, safe
message rules, CLI classifications, public exit codes, or failure-surface
behavior.

## Consequences

Preflight review is mandatory before release-path work. If the current state
is `Phase 4 local-only verification complete / release blocked`, local-only
work may continue only within the allowed scope, and any release-path operation
must stop until the resume gate is satisfied and separately authorized.

Documentation and verification records must continue to distinguish directly
verified local evidence from blocked or pending release evidence. `PASS` may
be used only for directly executed and directly verified work. Missing release
evidence must remain `PENDING`, `BLOCKED`, `NOT EXECUTED`, or `DEFERRED`.

The accepted preflight boundary prevents a later document from converting
vendor-clearance absence into an implied approval. Vendor clearance not
obtained means no ADR, status record, checklist, voyage-log entry, changelog
entry, local test result, or approval package authorizes release.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Accepted | Preflight hard stop and release boundary accepted as the durable Avast-pending operational gate. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0005-retry-policy-and-failure-classification.md`
- `docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md`
- `docs/architecture/ADR-0007-error-handling-and-failure-classification.md`
- `docs/development/Publisher_PreflightHardening.md`
- `docs/distribution/PublisherReleaseRunbook.md`
- `docs/development/Publisher_TestClassification.md`
- `docs/development/CURRENT_STATUS.md`
- `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`

## Replacement

This ADR does not supersede an earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify implementation behavior.
- This ADR does not modify tests.
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not create or update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not replace runbooks, release records, verification evidence,
  approval packages, or current status records.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, vendor submission, or
  vendor clearance.
- This ADR does not claim release readiness, vendor clearance, Avast
  false-positive resolution, risk acceptance, final release verification, Live
  E2E authorization, or publication completion.
