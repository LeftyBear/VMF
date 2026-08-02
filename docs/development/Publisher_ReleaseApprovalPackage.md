# Publisher Release Approval Package

Status  : Hold
Scope   : Docs-only / local-only release approval package organization
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_Phase4-3-5_GoNoGoReview.md, docs/development/Publisher_PreflightHardening.md, docs/distribution/PublisherReleaseRunbook.md

This package summarizes the current VMF Publisher approval state for review. It
is documentation only. It does not approve a release, create or update packages,
create tags, publish artifacts, execute Live E2E, mutate Google Docs or Google
Drive, re-run flagged executables, change production code, change public APIs,
modify Frozen specifications, change tests, write to `dist`, or push commits.

## 1. Current Release State

| Item | State |
| --- | --- |
| Formal release state | Phase 4 local-only verification complete / release blocked |
| Approval recommendation | Hold |
| Avast false-positive handling | Pending |
| Vendor clearance | Not obtained |
| Release readiness | Not established |
| Live E2E | Not executed in this package; blocked without explicit authorization |
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
| `docs/development/Publisher_EvidenceBundleSpecification.md` | Done | Defines redacted evidence bundle structure without assembling a concrete bundle. |
| `docs/development/Publisher_TestClassification.md` | Done | Separates documentation, local, non-live, package, Live E2E, and publication checks. |
| `docs/distribution/PublisherReleaseRunbook.md` | Draft | Defines release operation sequencing and authorization gates. |
| `docs/distribution/ReleaseChecklist.md` | Existing release checklist | Release checklist reference only; this package does not update checklist results. |

No new `PASS` release, package, Live E2E, Google Docs/Drive, publication, or
vendor-clearance evidence is created by this approval package.

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

The following operations remain blocked:

- release approval or rejection;
- tag creation;
- GitHub Release creation or update;
- artifact publication;
- package creation, replacement, or update;
- writing release or package artifacts under `dist`;
- packaged executable smoke for the Avast-pending flagged executable;
- Live E2E;
- setting `VMF_PUBLISHER_GOOGLE_E2E=1`;
- Google Docs mutation;
- Google Drive mutation;
- token-store mutation;
- temporary public image hosting;
- treating VirusTotal no-detection, a local antivirus exception, or a
  false-positive submission as vendor clearance;
- changing production code, tests, public APIs, persisted schemas, canonical
  formats, or Frozen specifications.

## 6. Resume Conditions

Release-path work may resume only after all applicable conditions are recorded:

1. Avast response is received, dated, and tied to the exact selected artifact
   path and SHA-256.
2. Vendor clearance, confirmed detection, inconclusive response, or
   repository-owner exception decision is recorded without ambiguity.
3. The repository owner explicitly reopens only the required next gate.
4. Package creation or update, packaged executable smoke, Live E2E, and
   publication each receive separate operation-specific authorization before
   execution.
5. Local source verification is rerun before release readiness is
   reconsidered.
6. Package identity and package verification are recorded for the selected
   artifact.
7. Security and supply-chain review is completed for the selected artifact.
8. Live E2E is either explicitly authorized and executed with readback, or
   recorded as owner-approved N/A.
9. Go/no-go is recorded before publication.

If Avast confirms the detection, is inconclusive, or does not match the
selected artifact identity, release remains blocked until a separate
remediation, rebuild, repackage, abandon-candidate, or owner-exception decision
is recorded.

## 7. Approval Recommendation

Approval Recommendation = Hold.

Basis:

- release gate remains blocked;
- Avast false-positive handling remains pending;
- vendor clearance has not been obtained;
- Live E2E has not been authorized or executed for this approval package;
- no current package creation, package update, package verification, or
  flagged executable smoke was performed;
- repository-owner go/no-go approval has not been recorded;
- publication, tag creation, and push were not authorized.
