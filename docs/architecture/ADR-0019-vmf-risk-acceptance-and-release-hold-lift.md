# ADR-0019: VMF Risk Acceptance And Release Hold Lift

Status  : Accepted
Date    : 2026-08-09
Scope   : Publisher Avast false-positive disposition by VMF-side residual risk acceptance, Release Hold lift, and post-hold release execution order
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md, docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md, docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md, docs/architecture/ADR_INDEX.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_TestClassification.md, docs/distribution/PublisherReleaseRunbook.md

## Context

ADR-0003 defines Publisher release gate and vendor-clearance governance.

ADR-0008 defines the operational preflight hard stop used while Avast
false-positive handling is pending.

ADR-0009 defines Evidence Bundle and Release Approval Package boundaries.

ADR-0011 defines that release authorization must be recorded separately from
ADR acceptance.

ADR-0012 defines release resumption procedure and final verification order.

The prior Hold state waited for Avast vendor clearance or a formally recorded
owner risk acceptance. The project now records new repository-owner facts and
decision input:

- `vmf-publisher.exe` was scanned by Avast as a standalone executable, and no
  detection was observed in that standalone scan.
- Avast configuration was observed to automatically submit suspicious files to
  Avast for inspection.
- After changing that setting to a user-choice handling mode, the reported
  message no longer appeared.
- The False Positive submission sent to Avast on 2026-07-25 remains
  unanswered as of 2026-08-09.
- Vendor clearance has not been obtained.
- The absence of detection in the standalone scan and the setting-dependent
  observation are not Avast safety certification.
- The repository owner directs VMF to formally accept the residual risk and
  lift the Release Hold without treating the state as vendor clearance.

## Decision

VMF formally accepts the residual antivirus false-positive risk for the
Publisher release path described by the current records.

The Release Hold created by the Avast-pending state is lifted by VMF-side risk
acceptance, not by Avast vendor clearance and not by Avast safety
certification.

Avast vendor clearance remains not obtained. The 2026-07-25 False Positive
submission remains unanswered. Future records must not state or imply that
Avast certified, approved, cleared, whitelisted, or safety-recognized
`vmf-publisher.exe` or any package.

The standalone Avast no-detection result and the setting-dependent message
observation may be cited only as VMF decision input. They are not release
verification, package approval, Live E2E evidence, publication approval,
vendor clearance, or proof of safety.

After this ADR, the release execution order is fixed as:

1. final verification;
2. Live E2E;
3. result review;
4. package/dist;
5. tag/release.

Each step remains a separate authorization gate. Lifting the Release Hold does
not authorize any step by implication.

This ADR is itself the VMF risk-acceptance and Release Hold lift record. It
does not create a Release Decision Record, publication record, package
artifact, tag, release, evidence bundle, or test result.

## Relationship To Other ADRs

Existing Accepted ADR meanings are unchanged.

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0019 records the allowed non-vendor path through formal VMF risk
acceptance. It does not rewrite vendor-clearance requirements into completed
vendor clearance.

ADR-0008 remains the hard-stop reference for blocked or ambiguous release-path
work. Its Avast-pending Hold stop no longer blocks the release sequence after
this risk acceptance, but its operation-specific authorization, evidence, and
preflight stop conditions still apply.

ADR-0009 remains the Evidence Bundle and Release Approval Package boundary.
Evidence and approval-package records remain review records and must not be
treated as executable approval without the required next-step authorization.

ADR-0011 remains the explicit release authorization boundary. This ADR records
risk acceptance and Hold lift, but final verification, Live E2E, package/dist,
tag, and release still require the appropriate explicit operation-specific
authorization and result recording.

ADR-0012 remains the final verification order basis, as updated by this ADR's
fixed post-hold sequence.

ADR-0013 remains the Release Decision Record traceability boundary. A future
release decision must reference this ADR as the risk-acceptance basis if the
release proceeds without vendor clearance.

## Consequences

The current state moves from Release Hold to post-hold release sequencing.
Release execution is not complete and publication has not occurred.

Records must distinguish:

- Avast vendor clearance: not obtained;
- Avast safety certification: not claimed;
- False Positive submission from 2026-07-25: unanswered;
- standalone Avast no-detection and setting-dependent observation: decision
  input only;
- VMF residual risk acceptance: accepted by ADR-0019;
- Release Hold: lifted by VMF risk acceptance;
- release execution: pending the fixed sequence and required authorizations.

Final verification must run before Live E2E. Live E2E must run before result
review. Package/dist work must wait until after result review. Tag/release
work must wait until after package/dist work and its review.

Any failed final verification, failed Live E2E, failed result review,
inconsistent artifact identity, unexpected antivirus result, missing
authorization, or ambiguous evidence stops the sequence and requires a new
recorded decision before proceeding.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-09 | Accepted | VMF-side residual risk acceptance recorded; Avast vendor clearance remains not obtained; Release Hold lifted; release execution sequence fixed but not executed. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md`
- `docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
- `docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md`
- `docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md`
- `docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md`
- `docs/development/CURRENT_STATUS.md`
- `docs/development/Publisher_AvastResponseIntakeTemplate.md`
- `docs/development/Publisher_ReleaseApprovalPackage.md`
- `docs/development/Publisher_TestClassification.md`
- `docs/distribution/PublisherReleaseRunbook.md`
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
- This ADR does not create or update packages or distribution artifacts.
- This ADR does not write to `dist`.
- This ADR does not create a tag.
- This ADR does not publish a package or artifact.
- This ADR does not execute Live E2E.
- This ADR does not mutate Google Docs or Google Drive.
- This ADR does not mutate token stores.
- This ADR does not execute, re-run, package, publish, or distribute a flagged
  executable.
- This ADR does not create release evidence.
- This ADR does not create a Release Approval Package.
- This ADR does not create a vendor clearance response.
- This ADR does not claim Avast vendor clearance.
- This ADR does not claim Avast safety certification.
- This ADR does not create a Release Decision Record.
- This ADR does not create a Publication Record.
- This ADR does not approve tag creation, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation, or
  token-store mutation by implication.
