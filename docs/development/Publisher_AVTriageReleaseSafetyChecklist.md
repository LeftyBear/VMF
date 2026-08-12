# Publisher AV Triage Release-Safety Checklist

Status  : Checklist only / docs-only / local-only
Scope   : Antivirus triage and release-safety boundary before future Publisher release-path work
Depends : docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md, docs/architecture/ADR-0019-vmf-risk-acceptance-and-release-hold-lift.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_vNext_Backlog.md

This checklist is for safe AV triage review. It does not approve release,
resolve Avast false-positive handling, obtain vendor clearance, authorize
package or distribution work, execute Live E2E, mutate Google Docs or Google
Drive, re-run flagged executables, create tags, publish artifacts, change
production code, change tests, change public APIs, or modify Frozen
specifications.

If no vendor response has been received, redacted, reviewed, and recorded for
the exact selected artifact identity, the vendor-clearance decision remains
`Hold continues` / `vendor clearance not obtained`. ADR-0019 may record
VMF-side residual risk acceptance for a specific release identity, but that
does not convert Avast response pending into vendor clearance and does not
claim Avast safety certification.

The 2026-08-11 manual Avast scan / CyberCapture observation for
`vmf-publisher.exe` SHA-256
`892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f` is local
evidence only. It records that Avast showed "このファイルは安全のようです" and
`IDP.HELU.PSD11` was not reproduced, but it is not an Avast vendor response or
vendor clearance.

Use this checklist as the P1-06 AV triage release-safety hardening record in
the vNext backlog. It is a reference checklist only; backlog priority labels do
not authorize release-path work.

## 1. Artifact Identity

| Item | Status | Evidence |
| --- | --- | --- |
| Selected artifact version recorded | PENDING |  |
| Selected artifact path recorded without local secret-bearing path | PASS | `vmf-publisher.exe`; matched release ZIP / repo Release exe. |
| Selected artifact SHA-256 recorded | PASS | `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`. |
| Target commit recorded | PENDING |  |
| Package manifest or asset identity recorded when authorized | PENDING |  |
| Existing package inspection authorized before use | PENDING |  |

## 2. Vendor Response State

| Item | Status | Evidence |
| --- | --- | --- |
| Avast response received | PENDING |  |
| Response safely summarized without private URL or token material | PENDING |  |
| Response mapped in `Publisher_AvastResponseIntakeTemplate.md` | PENDING |  |
| Vendor clearance confirmed for selected artifact identity | PENDING |  |
| Avast safety certification claimed | NOT CLAIMED |  |
| Avast response pending boundary preserved | PASS | `CURRENT_STATUS.md` and `Publisher_ReleaseApprovalPackage.md` record Avast response pending, vendor clearance not obtained, and Avast safety certification not claimed. |
| VMF-side residual risk acceptance kept separate | PASS | ADR-0019 records VMF risk acceptance; it is not a vendor response or Avast clearance. |
| Manual scan / CyberCapture not reproduced | PASS | Avast showed "このファイルは安全のようです"; detection name none; `IDP.HELU.PSD11` not reproduced; threat result none / allowed equivalent. |
| Hold decision reviewed | PENDING | Local evidence supports gate reconsideration only; vendor clearance and final authorization remain separate. |

Do not treat submission acknowledgement, silence, scanner no-detection,
setting-dependent local behavior, local exclusions, or third-party scanner
results as Avast vendor clearance.

Do not treat local manual scan / CyberCapture no-detection as Avast vendor
clearance.

## 3. Release-Safety Gate Review

| Gate | Status | Required Boundary |
| --- | --- | --- |
| Final verification | PENDING | Requires separate authorization and direct evidence. |
| Live E2E | BLOCKED | Requires separate explicit authorization; do not record `PASS` unless executed. |
| Google Docs mutation | BLOCKED | Requires separate explicit authorization. |
| Google Drive mutation | BLOCKED | Requires separate explicit authorization. |
| Package / `dist` update | BLOCKED | Requires separate explicit authorization. |
| Flagged executable rerun | BLOCKED | Requires separate explicit authorization. |
| Tag / release / publication | BLOCKED | Requires separate explicit authorization. |
| Push | BLOCKED | Requires separate explicit authorization. |

Authorization for one gate does not authorize any other gate.

Release-safety review must cross-check `CURRENT_STATUS.md` and
`Publisher_ReleaseApprovalPackage.md` before interpreting older release-hold
or approval wording. Evidence, approval, release execution, publication,
vendor-clearance, and backlog records remain separate evidence classes.

## 4. Post-Release Evidence Boundary

Post-release evidence may support audit, follow-up review, external scanner
observation, or publication confirmation. It must not retroactively satisfy:

- pre-release approval;
- release authorization;
- required release gates;
- final release verification;
- vendor clearance;
- Avast false-positive resolution;
- Release Decision Record completeness.

Gate-missing or authorization-missing release work remains a governance
exception or incident candidate even if later observations are favorable.

Evidence references should remain traceable to
`Publisher_EvidenceBundleSpecification.md`,
`Publisher_PostReleaseEvidenceSummaryTemplate.md`, and the relevant release
record without copying sensitive raw evidence into this checklist.

## 5. Redaction And Secret Review

Before sharing AV triage evidence, confirm that it excludes:

- OAuth credentials and service-account keys;
- access tokens and refresh tokens;
- token-store contents;
- credential paths and token-store paths;
- Authorization headers and cookies;
- local absolute paths and user profile paths;
- private Google Docs or Drive URLs;
- raw exception bodies, HTTP bodies, provider payloads, and stack traces.

## 6. Decision

Select exactly one decision after the checklist is reviewed.

| Decision | Selected | Meaning |
| --- | --- | --- |
| Hold continues |  | Vendor response is missing, inconclusive, unfavorable, or not yet reviewed for the exact artifact identity. |
| Gate may be reconsidered | X | Manual scan / CyberCapture local evidence and owner risk decision exist, but the next gate still requires separate authorization. |
| Escalation required |  | Evidence conflicts, cannot be safely redacted, or requires repository-owner/security review. |

## 7. Explicit Non-Actions

This checklist does not create, update, inspect, execute, publish, upload, or
distribute any package or executable. It does not run Live E2E, mutate Google
Docs or Google Drive, alter token stores, create tags, create releases, push
commits, or change production code, tests, public APIs, persisted schemas, or
Frozen specifications.
