# Publisher Operator Guidance - Avast Risk Acceptance

Status  : Hold lifted by VMF risk acceptance; post-hold release sequence pending
Scope   : Local-only operator guidance after VMF accepts residual Avast false-positive risk
Depends : docs/distribution/PublisherReleaseRunbook.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_TestClassification.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_v1.0_Implementation_Voyage_Log.md

This document gives operators the current allowed and gated actions after VMF
accepts residual Avast false-positive risk through ADR-0019. It is
documentation only. It does not approve a release, create or update packages,
create tags, publish artifacts, execute Live E2E, mutate Google Docs or Google
Drive, re-run the flagged executable, push commits, change production code,
change public APIs, or modify Frozen specifications.

## 1. Current State

| Item | Current State |
| --- | --- |
| Formal state | Phase 4 local-only verification complete / Release Hold lifted by VMF risk acceptance |
| Release gate | Hold lifted; release execution sequence not started |
| Current decision | VMF residual risk accepted; proceed only by fixed post-hold sequence |
| Avast false positive | Vendor response pending; VMF risk acceptance recorded |
| Vendor clearance | Not obtained |
| Avast safety certification | Not claimed |
| False Positive submission | Submitted 2026-07-25; unanswered as of 2026-08-09 |
| Approval recommendation | Proceed to final verification sequence after explicit operation-specific authorization |
| Product regression | Not indicated by the current records |

The prior Hold was operational. ADR-0019 lifts that Hold by VMF-side residual
risk acceptance, not by Avast vendor clearance or Avast safety certification.
Phase 4 local-only verification may remain complete while release execution
still requires final verification, Live E2E, result review, package/dist, and
tag/release.

## 2. Allowed Actions Before Release Execution

Operators may perform only local, non-mutating, non-release work:

- build;
- unit tests;
- mock-backed verification;
- dry-run verification that does not publish, mutate Google resources, create
  packages, or re-run the flagged executable;
- documentation updates;
- existing package inspection only, when the inspection is static and does not
  create, update, publish, or execute the package.

Allowed results must be reported as local, non-live, mock-backed, dry-run, or
static evidence. They must not be promoted to release readiness, Live E2E
evidence, Google Docs readback evidence, Google Drive cleanup evidence, package
approval, publication approval, or antivirus vendor clearance.

## 3. Gated Actions After Hold Lift

The following actions remain gated until the ADR-0019 order reaches that step
and explicit authorization is recorded:

- final verification;
- Live E2E;
- result review;
- package or distribution artifact creation;
- package or distribution artifact update;
- release, tag, or publication;
- Google Docs mutation;
- Google Drive mutation;
- flagged executable re-run;
- push.

Authorization for one blocked action does not authorize any other blocked
action.

## 4. Decision Rules

Do not describe the current state as Avast-cleared, Avast-certified, or vendor
cleared. Avast vendor clearance remains not obtained, and the 2026-07-25 False
Positive submission remains unanswered.

Treat ADR-0019 as VMF-side residual risk acceptance only. Do not use it to
reopen completed local-only implementation work unless a separate source-level
defect is identified.

Proceed after Hold lift only in this order:

1. final verification;
2. Live E2E;
3. result review;
4. package/dist;
5. tag/release.

Each step requires explicit repository-owner authorization when applicable.

## 4.1 Common Recovery Procedures

Use these procedures when the hold affects an operator workflow:

| Situation | Recovery |
| --- | --- |
| A local command would cross the release boundary | Stop before execution, record the operation as `BLOCKED` or `NOT EXECUTED`, and return to local-only documentation or source checks. |
| An Avast response is received | Record it first in `Publisher_AvastResponseIntakeTemplate.md` with artifact path and SHA-256 redacted or summarized safely, then reassess the remaining vendor-clearance record without rewriting ADR-0019 as Avast certification. |
| Vendor response is inconclusive, mismatched, or asks for more information | Stop the release sequence, record the vendor-clearance state as unresolved, and do not run package, Live E2E, publication, or flagged executable work until a new decision is recorded. |
| Local-only verification evidence is being reused | Keep the evidence label local-only, non-live, mock-backed, dry-run, or static; do not relabel it as release readiness or vendor clearance. |
| Final release resume is requested | Follow ADR-0019 order: final verification, Live E2E, result review, package/dist, tag/release. |

## 5. Stop Conditions

Stop and report before proceeding if:

- the Avast response does not identify the affected artifact clearly;
- the selected artifact path, version, or SHA-256 is ambiguous;
- a command would create or update `dist`;
- a command would execute the flagged package before the exact run is
  explicitly authorized;
- `VMF_PUBLISHER_GOOGLE_E2E` would be enabled;
- Google Docs or Google Drive would be mutated;
- release, tag, publication, or push is requested without separate explicit
  authorization;
- Frozen specifications, public APIs, persisted schemas, canonical formats, or
  production defaults would need to change.

## 6. Operator Reporting

Operator reports after Hold lift must state whether each gated operation was
performed or not performed. Use `PASS` only for directly executed and directly
verified evidence. Keep `PENDING`, `BLOCKED`, `NOT EXECUTED`, and `DEFERRED`
when evidence has not been produced.
