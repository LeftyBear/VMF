# Publisher Release Approval Package

Status  : Hold lifted by VMF risk acceptance; release execution pending
Scope   : Docs-only / local-only release approval package organization after ADR-0019 risk acceptance
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_Phase4-3-5_GoNoGoReview.md, docs/development/Publisher_PreflightHardening.md, docs/development/Publisher_TestClassification.md, docs/distribution/PublisherReleaseRunbook.md

This package summarizes the current VMF Publisher approval state for review. It
is documentation only. It does not approve a release, create or update packages,
create tags, publish artifacts, execute Live E2E, mutate Google Docs or Google
Drive, re-run flagged executables, change production code, change public APIs,
modify Frozen specifications, change tests, write to `dist`, or push commits.

## 1. Current Release State

| Item | State |
| --- | --- |
| Formal release state | Phase 4 local-only verification complete / Release Hold lifted by VMF risk acceptance |
| Approval recommendation | Proceed to final verification sequence after explicit operation-specific authorization |
| Avast false-positive handling | Vendor response pending; VMF residual risk accepted |
| Vendor clearance | Not obtained |
| Avast safety certification | Not claimed |
| False Positive submission | Submitted 2026-07-25; unanswered as of 2026-08-09 |
| Avast standalone executable scan | No detection observed for `vmf-publisher.exe`; decision input only |
| Avast setting-dependent observation | Message stopped after changing automatic suspicious-file submission to user-choice handling; decision input only |
| Release readiness | Pending final verification, Live E2E, result review, package/dist, and tag/release sequence |
| Live E2E | Not executed in this package; requires explicit authorization after final verification |
| Google Docs / Google Drive mutation | Not performed |
| Package creation or update | Not performed |
| Flagged executable re-run | Not performed |
| Release, tag, publication, push | Not performed |

Local-only verification remains useful evidence for source quality inside its
approved boundary. It is not release readiness, package approval, Live E2E
readback, Google Drive cleanup evidence, publication approval, or antivirus
vendor clearance.

## 2. Approval Boundary

This approval package is limited to recording and indexing existing release
gate information. It may be used to decide what remains required before a later
release approval review, but it is not itself an approval record.

The following remain separate authorization gates:

- package creation or package update;
- package verification and packaged executable smoke;
- Live E2E and any credentialed Google operation;
- Google Docs mutation;
- Google Drive mutation;
- temporary public image hosting;
- antivirus vendor handling or repository-owner exception decision;
- repository-owner go/no-go approval;
- tag creation;
- GitHub Release creation or update;
- artifact publication;
- staging, commit, push, or other Git history changes.

Authorization for one gate must not be treated as authorization for any other
gate.

## 3. Evidence Index

| Evidence | Status | Use |
| --- | --- | --- |
| `docs/development/CURRENT_STATUS.md` | Current status record | Source of the formal release-blocked state and open release gates. |
| `docs/development/Publisher_Phase4-3-1_ReleaseReadinessChecklist.md` | Done / DEFERRED release readiness | Confirms local-only evidence does not establish release readiness. |
| `docs/development/Publisher_Phase4-3-2_ReleaseCandidateVerification.md` | Done / DEFERRED candidate verification | Records that no current candidate artifact verification was executed. |
| `docs/development/Publisher_Phase4-3-3_ReleaseArtifactAudit.md` | Done / DEFERRED artifact audit | Records missing current artifact audit evidence. |
| `docs/development/Publisher_Phase4-3-4_SecurityAndSupplyChainReview.md` | Done / DEFERRED security review | Records unresolved Avast and security review conditions. |
| `docs/development/Publisher_Phase4-3-5_GoNoGoReview.md` | Done / DEFERRED go/no-go | Records release go/no-go as deferred. |
| `docs/development/Publisher_PreflightHardening.md` | Done | Defines hard stops while Avast handling remains pending. |
| `docs/development/Publisher_AvastResponseIntakeTemplate.md` | Template only / no Avast response received | Defines safe Avast response intake; vendor clearance remains not obtained while ADR-0019 records VMF risk acceptance separately. |
| `docs/development/Publisher_EvidenceBundleSpecification.md` | Done | Defines redacted evidence bundle structure without assembling a concrete bundle. |
| `docs/development/Publisher_TestClassification.md` | Done | Separates documentation, local, non-live, package, Live E2E, and publication checks. |
| `docs/distribution/PublisherReleaseRunbook.md` | Draft | Defines release operation sequencing and authorization gates. |
| `docs/distribution/ReleaseChecklist.md` | Existing release checklist | Release checklist reference only; this package does not update checklist results. |

No new `PASS` release, package, Live E2E, Google Docs/Drive, publication, or
vendor-clearance evidence is created by this approval package. ADR-0019
records risk acceptance and Hold lift only.

## 4. Ahead Commits Summary

At package preparation time, the local branch was `main...origin/main [ahead 5]`.
The ahead commits were documentation commits only in the current local review
context:

| Commit | Summary |
| --- | --- |
| `e1fa49a` | `docs: harden Publisher preflight release gates` |
| `de8fb8d` | `docs: define publisher evidence bundle specification` |
| `8b46150` | `docs: add Publisher retry policy specification` |
| `f4541c3` | `docs: add Publisher Avast hold operator guidance` |
| `bb0dd2b` | `docs: add publisher test classification` |

These commits do not by themselves authorize push, release, tag creation,
publication, package work, Live E2E, Google Docs or Google Drive mutation, or
flagged executable re-run.

## 5. Blocked Operations

Allowed work remains limited to local-only documentation, read-only
investigation, source checks, non-live verification, mock-backed verification,
dry-run checks that do not publish or execute the flagged package, and static
existing-package inspection when explicitly in scope.

The following operations remain gated and must follow ADR-0019 order:

- final verification;
- Live E2E;
- result review;
- package creation, replacement, update, or any `dist` write;
- tag creation, GitHub Release creation or update, artifact publication, or
  release announcement;
- packaged executable smoke for any previously flagged executable;
- setting `VMF_PUBLISHER_GOOGLE_E2E=1`;
- Google Docs mutation;
- Google Drive mutation;
- token-store mutation;
- temporary public image hosting;
- treating standalone scanner no-detection, setting-dependent behavior,
  VirusTotal no-detection, a local antivirus exception, or a false-positive
  submission as vendor clearance;
- changing production code, tests, public APIs, persisted schemas, canonical
  formats, or Frozen specifications.

## 6. Post-Hold Execution Conditions

Release-path work may proceed only in this order, with each step separately
authorized and recorded:

1. Final verification.
2. Live E2E.
3. Result review.
4. Package/dist.
5. Tag/release.

If any step fails, lacks authorization, produces ambiguous evidence, or changes
artifact identity, the sequence stops until a separate recorded decision
defines the next action.

## 7. Approval Recommendation

Approval Recommendation = Proceed to final verification sequence after
explicit operation-specific authorization.

Basis:

- Release Hold is lifted by ADR-0019 VMF-side residual risk acceptance;
- Avast vendor response remains pending;
- vendor clearance has not been obtained;
- Avast safety certification is not claimed;
- Live E2E has not been authorized or executed for this approval package;
- no current package creation, package update, package verification, or
  flagged executable smoke was performed;
- repository-owner go/no-go approval has not been recorded;
- publication, tag creation, and push were not authorized.
