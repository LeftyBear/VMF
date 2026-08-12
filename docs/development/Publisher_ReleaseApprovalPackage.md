# Publisher Release Approval Package

Status  : Risk Accepted Go; responsible-owner release-gate approval recorded; Avast response pending
Scope   : Docs-only / local-only release approval package organization after explicit owner risk acceptance
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_Phase4-3-5_GoNoGoReview.md, docs/development/Publisher_PreflightHardening.md, docs/development/Publisher_TestClassification.md, docs/distribution/PublisherReleaseRunbook.md

This package summarizes the current VMF Publisher approval state for review. It
is documentation/status alignment only. It does not approve a new release,
create or update packages, create tags, publish artifacts, execute Live E2E,
mutate Google Docs or Google Drive, re-run flagged executables, change
production code, change public APIs, modify Frozen specifications, change
tests, write to `dist`, or perform release execution.

Post-authorization repository status update:

- Publisher `0.0.1-dev` release authorization record commit
  `57e71e240b9e42dbca03bae6dbf4d8a20216c58a` was pushed to `origin/main`.
- `docs/development/CURRENT_STATUS.md` commit
  `a04126ce24c7abd376bec943466c30cd565bb70e` was pushed to `origin/main`.

These repository synchronization facts record documentation/status alignment
only. They do not perform release execution, claim Avast vendor clearance,
claim Avast safety certification, create or update package/dist output, run
`vmf-publisher.exe`, mutate Google Docs or Google Drive, operate on
OAuth/token-store/credentials, or perform any Avast operation.

## 1. Current Release State

| Item | State |
| --- | --- |
| Formal release state | Risk Accepted Go / responsible-owner release-gate approval recorded / Avast response pending |
| Approval recommendation | Release gate PASS under evidence-based vendor-clearance criteria; subsequent release, tag, publication, distribution, package/dist, Live E2E, Google Docs / Drive, and flagged executable operations remain separately gated |
| Avast false-positive handling | Vendor response pending; latest authorized latest-definition rescan did not reproduce the detection; responsible-owner approval recorded for the current release-control assessment |
| Vendor clearance | Accepted for release-gate purposes under documented evidence-based criteria; Avast direct response remains pending |
| Avast safety certification | Not claimed |
| Release authorization record | Created and pushed in commit `57e71e240b9e42dbca03bae6dbf4d8a20216c58a`; repository synchronization only, not release execution by this docs update |
| Formal residual-risk release approval memo | `docs/development/Publisher_ResidualRiskReleaseAuthorizationApprovalMemo_2026-08-12.md` records Approved VMF-side residual risk acceptance plus release authorization for the fixed `0.0.1-dev` release identity; vendor clearance remains not obtained and Avast safety certification is not claimed |
| Current status update | `docs/development/CURRENT_STATUS.md` updated and pushed in commit `a04126ce24c7abd376bec943466c30cd565bb70e`; documentation/status alignment only |
| False Positive submission | Submitted 2026-07-25; unanswered as of 2026-08-12 |
| Avast standalone executable scan | No detection observed for `vmf-publisher.exe`; decision input only |
| Avast manual scan / CyberCapture result | `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`; release ZIP / repo Release exe match; Avast showed "このファイルは安全のようです"; no detection name; `IDP.HELU.PSD11` not reproduced; result `not reproduced`; local manual confirmation only |
| Avast setting-dependent observation | Message stopped after changing automatic suspicious-file submission to user-choice handling; decision input only |
| Release readiness | Completed for the `0.0.1-dev` GitHub prerelease; release completion evidence recorded docs-only |
| Release identity | `0.0.1-dev` / `vmf-publisher-v0.0.1-dev`; annotated tag object `a962e19ba2b0a494d1158011ae823d579e41711f`; peeled/package target commit `f08eef306ba82e3ea7f031ef652666178f2f0acf`; evidence docs commit `39df8bedd848da42a4de3cb9461ce4cc86b51197` |
| Final verification | Local checks passed on 2026-08-12; published artifact final verification is not complete because local `dist` ZIP identity does not match the recorded published identity |
| Live E2E | PASS after OAuth Desktop reauthorization refreshed the local authentication state; total 4 / passed 4 / failed 0 / skipped 0 |
| Result review | Recorded; initial Live E2E failure was attributed to stale, revoked, or inconsistent saved OAuth token state; rerun passed after token deletion and OAuth Desktop reauthorization |
| Google Docs / Google Drive mutation | Performed only as part of the authorized Live E2E run; no publication performed |
| Package creation or update by this docs update | Not performed; no `dist` write by this documentation-only update |
| Package path / size / SHA-256 | `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |
| Local `dist` ZIP package evidence | 2026-08-12 package evidence recorded for `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`: 983422 bytes; SHA-256 `0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76`; manifest `VMF Publisher` / `0.0.1-dev` / `win-x64` / `Release` / `selfContained=false` / 14 files; Static package verification PASS; Build/package PASS |
| Artifact identity reconciliation | GitHub Release asset metadata confirmed the recorded published identity: 983404 bytes and SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`; the local `dist` ZIP package evidence above is not the recorded published artifact and must not be treated as a replacement release asset |
| Package generation / verification | PASS / PASS for recorded package evidence; manifest files 14; secret/static package inspection PASS |
| GitHub Release | Published prerelease `true`: https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev; release name `VMF Publisher 0.0.1-dev` |
| Release asset | `vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; remote asset digest matched local verified package SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |
| Flagged executable re-run | Not performed |
| Release, tag, publication by this docs update | Not performed; this documentation/status alignment does not create tags, publish artifacts, update a GitHub Release, or execute a release operation |
| Post-release evidence capture | Mandatory after any Risk Accepted Go release-path execution; must preserve artifact identity, final verification evidence, publication evidence, post-release observations, and unresolved Avast/vendor-clearance state |

Local-only verification remains useful evidence for source quality inside its
approved boundary. It is not release readiness, package approval, Live E2E
readback, Google Drive cleanup evidence, publication approval, or antivirus
vendor clearance.

## 2. Approval Boundary

This approval package is limited to recording and indexing existing release
gate information. For future release-path work or a new release decision, it
may be used to decide what remains required before a later release approval
review, but it is not itself an approval record.

The following remain separate authorization gates for future release-path work
or a new decision:

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

## 2.1 Risk Accepted Go Basis

Approval Recommendation = Risk Accepted Go.

Basis:

- Avast has not responded to the false-positive submission;
- latest authorized reproducibility verification did not reproduce the Avast
  detection;
- the responsible owner gave an explicit Go decision;
- vendor clearance remains not obtained;
- Avast safety certification is not claimed;
- the decision advances only through explicit VMF-side risk acceptance.

Relationship to ADR-0003 and ADR-0008:

- ADR-0003 normal release-gate prerequisites are not fully satisfied because
  vendor clearance has not been obtained and Avast has not provided a response;
- ADR-0003 permits a formally accepted repository-owner risk decision as the
  alternative false-positive disposition path;
- ADR-0008 preflight hard-stop controls remain the default boundary while
  Avast handling is pending;
- this Risk Accepted Go record is a limited owner-risk-acceptance exception to
  proceed through the authorized release path; Avast pending remains in
  effect, vendor clearance remains not obtained, and Avast safety
  certification is not claimed.

Final verification remains mandatory before release-path completion. If final
verification fails, is incomplete, changes artifact identity, or produces
ambiguous evidence, the path returns to Hold until a new owner decision is
recorded.

Post-release evidence capture is mandatory after execution. It must record the
exact artifact identity, publication identity, final verification evidence,
post-release observations, and the continuing Avast pending / vendor clearance
not obtained state without retroactively converting local no-detection evidence
into vendor clearance.

Final verification local checks passed on 2026-08-12. The local `dist` ZIP
identity does not match the recorded published identity because it is a later
regenerated local ZIP. GitHub Release asset metadata confirms the recorded
published identity as 983404 bytes with SHA-256
`73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`. The
local regenerated ZIP must not be used as release evidence or replacement
artifact unless a separate approved cleanup, restore, or rebuild path is
recorded.

The initial build failure during this run was a transient local execution
issue caused by a Release-output DLL lock while build and tests overlapped.
The build passed after serial rerun with warnings 0 / errors 0.

## 3. Evidence Index

| Evidence | Status | Use |
| --- | --- | --- |
| `docs/development/CURRENT_STATUS.md` | Current status record | Source of the formal current state, published `0.0.1-dev` evidence, unresolved Avast vendor-clearance boundary, and future gated operations. |
| `docs/development/Publisher_ResidualRiskReleaseAuthorizationApprovalMemo_2026-08-12.md` | Approved formal record | Records VMF-side residual risk acceptance plus release authorization for `0.0.1-dev` while preserving Avast vendor clearance not obtained and Avast safety certification not claimed. |
| `docs/development/Publisher_ResponsibleOwnerApprovalReleaseGateReevaluationRecord_2026-08-12.md` | Approved release-gate record | Records responsible-owner approval and release-gate PASS under evidence-based vendor-clearance criteria after latest-definition Avast rescan evidence confirmed detection non-reproduction. |
| `docs/development/Publisher_Phase4-3-1_ReleaseReadinessChecklist.md` | Done / DEFERRED release readiness | Confirms local-only evidence does not establish release readiness. |
| `docs/development/Publisher_Phase4-3-2_ReleaseCandidateVerification.md` | Done / DEFERRED candidate verification | Records that no current candidate artifact verification was executed. |
| `docs/development/Publisher_Phase4-3-3_ReleaseArtifactAudit.md` | Done / DEFERRED artifact audit | Records missing current artifact audit evidence. |
| `docs/development/Publisher_Phase4-3-4_SecurityAndSupplyChainReview.md` | Done / DEFERRED security review | Records unresolved Avast and security review conditions. |
| `docs/development/Publisher_Phase4-3-5_GoNoGoReview.md` | Done / DEFERRED go/no-go | Records release go/no-go as deferred. |
| `docs/development/Publisher_PreflightHardening.md` | Done | Defines hard stops while Avast handling remains pending. |
| `docs/development/Publisher_AvastResponseIntakeTemplate.md` | Template only / no Avast response received | Defines safe Avast response intake; vendor clearance remains not obtained while ADR-0019 records VMF risk acceptance separately. |
| Avast manual confirmation note | Recorded / local evidence only | Manual Avast scan / CyberCapture result for `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f` showed "このファイルは安全のようです" and no `IDP.HELU.PSD11` detection; current result `not reproduced`. |
| `docs/development/Publisher_EvidenceBundleSpecification.md` | Done | Defines redacted evidence bundle structure without assembling a concrete bundle. |
| `docs/development/Publisher_TestClassification.md` | Done | Separates documentation, local, non-live, package, Live E2E, and publication checks. |
| ADR-0019 result review | Recorded in this package | Reviews final verification and Live E2E results; package generation, package verification, tag/release, GitHub prerelease publication, and asset upload are recorded complete for `0.0.1-dev`. |
| `docs/distribution/PublisherReleaseRunbook.md` | Draft | Defines release operation sequencing and authorization gates. |
| `docs/distribution/ReleaseChecklist.md` | Existing release checklist plus release completion record | Preserves completed `0.0.0-dev` evidence and records `0.0.1-dev` annotated tag object, peeled/package target commit, evidence docs commit, GitHub prerelease URL, asset identity, remote/local digest match, package generation PASS, and package verification PASS. |
| `docs/releases/Publisher_0.0.1-dev_ReleaseNotes.md` | Release completion notes | Records `0.0.1-dev` / `vmf-publisher-v0.0.1-dev`, tag object, peeled/package target commit, evidence docs commit, GitHub Release URL, asset name, size, SHA-256, package verification, remote digest match, and publication result. |

No new publication or vendor-clearance evidence is created by this approval
package. The recorded `PASS` package evidence is limited to the supplied
package generation, package verification, and static package inspection facts
for the explicit artifact identity being recorded. ADR-0019 records risk
acceptance and Hold lift only. Local package evidence with a different size or
SHA-256 does not replace the recorded GitHub Release asset identity unless a
separate approved release-asset replacement path is recorded.

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

The following future operations remain gated and must follow ADR-0019 order or
a later operation-specific authorization:

- final verification for a new decision;
- Live E2E for a new decision;
- result review for a new decision;
- package creation, replacement, update, or any new `dist` write;
- tag creation, GitHub Release creation or update, artifact replacement,
  publication, or release announcement;
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

## 6. Risk Accepted Go Execution Conditions

Release-path work under Risk Accepted Go may proceed only in this order, with
each step separately authorized and recorded:

1. Final verification.
2. Live E2E.
3. Result review.
4. Package/dist.
5. Tag/release.
6. Post-release evidence capture.

If any step fails, lacks authorization, produces ambiguous evidence, or changes
artifact identity, the sequence stops until a separate recorded decision
defines the next action.

## 7. Release Identity

Status: COMPLETE as published GitHub prerelease; completion evidence recorded
docs-only.

The next Publisher release identity is:

| Field | Value |
| --- | --- |
| Version | `0.0.1-dev` |
| Tag | `vmf-publisher-v0.0.1-dev` |
| Annotated tag object | `a962e19ba2b0a494d1158011ae823d579e41711f` |
| Peeled/package target commit | `f08eef306ba82e3ea7f031ef652666178f2f0acf` |
| Evidence docs commit | `39df8bedd848da42a4de3cb9461ce4cc86b51197` |
| Runtime | `win-x64` |
| Configuration | `Release` |
| Package type | Framework-dependent (`selfContained=false`) |
| Package path | `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip` |
| Asset name | `vmf-publisher-0.0.1-dev-win-x64.zip` |
| Package size | 983404 bytes |
| Package SHA-256 | `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |
| Package generation | PASS |
| Package verification | PASS |
| Manifest files | 14 |
| Secret/static package inspection | PASS |
| Tag state | Pushed; remote tag readback PASS |
| GitHub Release state | Published prerelease `true`; release name `VMF Publisher 0.0.1-dev`; https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev |
| Asset upload | Complete; remote asset digest matched local verified package SHA-256 |
| Publication state | GitHub prerelease publication complete |

The prior `0.0.0-dev` identity remains immutable historical release evidence:
existing tag, GitHub Release, and asset identities must not be retargeted,
replaced, or reused.

This record satisfies the ADR-0016 requirement to keep version, commit, tag,
package or artifact, evidence, and authorization identity fields explicit.
Version, peeled/package target commit, annotated tag object, verified package
identity, GitHub Release URL, asset identity, and evidence docs commit are now
fixed. Final verification `PASS`, Live E2E 4/4 `PASS`, result review complete,
package generation `PASS`, package verification `PASS`, tag push complete,
remote tag readback `PASS`, GitHub prerelease creation complete, asset upload
complete, and remote/local digest match are the recorded identity-chain
evidence.

## 8. ADR-0019 Result Review

Status: Recorded as docs-only result review evidence.

Final verification was completed before Live E2E and is recorded as `PASS`:

| Check | Result |
| --- | --- |
| Release build | PASS; warnings 0 / errors 0 |
| Unit tests | PASS; 492 passed / 0 failed / 0 skipped |
| Integration tests | PASS; 16 passed / 0 failed / 0 skipped |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS |

The initial Live E2E run produced total 4 / passed 3 / failed 1. The failed
test was `RevisionConflict_ReturnsConflictBeforeVerification`. The failure was
Google OAuth API HTTP 400 `invalid_grant`.

Result review conclusion: the failure was attributed to stale, revoked, or
inconsistent saved OAuth token state. The existing OAuth token was deleted,
OAuth Desktop reauthorization was performed, and the local authentication state
was refreshed. No OAuth token, refresh token, credential, client secret,
Authorization header, token-store content, private URL, or provider payload is
recorded in this package.

The Live E2E rerun passed:

| Check | Result |
| --- | --- |
| Live E2E rerun | PASS |
| Total | 4 |
| Passed | 4 |
| Failed | 0 |
| Skipped | 0 |

Passing Live E2E cases:

- `RevisionConflict_ReturnsConflictBeforeVerification`;
- `EmptyPlan_DoesNotCallGoogleDocsBatchUpdateAndStillVerifies`;
- `Success_AppliesReadsBackVerifiesAndAllowsVerifiedStateCommit`;
- `ReadbackMismatch_DisallowsVerifiedStateCommit`.

Execution-after checks:

| Item | State |
| --- | --- |
| `VMF_PUBLISHER_GOOGLE_E2E` | unset |
| Working tree at run completion | clean |
| `dist` | unchanged |
| Package/dist | not executed |
| Tag/release/publication | not executed |

This result review completes only the ADR-0019 result review record for the
facts above. Package generation and package verification are recorded
separately in this approval package for the fixed `0.0.1-dev` identity. This
docs-only update does not authorize or perform package/dist work, tag
creation, release publication, artifact publication, flagged executable smoke,
staging, commit, push, production code changes, test changes, Frozen
specification changes, public API changes, vendor clearance, or Avast safety
certification.

## 9. Approval Recommendation

Approval Recommendation = Risk Accepted Go; this docs-only update does not
perform release execution.

Basis:

- Avast vendor response remains pending and unanswered;
- latest authorized reproducibility verification did not reproduce the Avast
  detection;
- the responsible owner gave an explicit Go decision;
- vendor clearance has not been obtained;
- Avast safety certification is not claimed;
- ADR-0003 / ADR-0008 normal gate controls remain in force except for the
  limited owner-risk-acceptance path recorded here;
- final verification is mandatory before completion;
- post-release evidence capture is mandatory after any release-path execution;
- manual Avast scan / CyberCapture local evidence records `IDP.HELU.PSD11` as
  not reproduced for `vmf-publisher.exe` SHA-256
  `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`, without
  converting the state into vendor clearance or release authorization;
- final verification and Live E2E have passed and result review is recorded;
- package generation and package verification are recorded as `PASS` for the
  fixed `0.0.1-dev` package identity;
- tag push, remote tag readback, GitHub prerelease creation, asset upload, and
  remote/local digest match are recorded as complete for `0.0.1-dev`;
- the Publisher `0.0.1-dev` release authorization record was created and
  pushed in commit `57e71e240b9e42dbca03bae6dbf4d8a20216c58a`;
- `docs/development/CURRENT_STATUS.md` was updated and pushed in commit
  `a04126ce24c7abd376bec943466c30cd565bb70e`;
- no package creation, package update, package verification, `dist` write, or
  flagged executable smoke was performed by this docs-only update;
- this docs-only update did not modify tags, GitHub Release, assets,
  production code, tests, package, or `dist`;
- this docs-only update did not run build/test, execute `vmf-publisher.exe`,
  mutate Google Docs or Google Drive, operate on OAuth/token-store/credentials,
  or perform an Avast operation.
