# Publisher Release Approval Package

Status  : Hold lifted by VMF risk acceptance; 0.0.1-dev GitHub prerelease published
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
| Formal release state | Phase 4 local-only verification complete / Release Hold lifted by VMF risk acceptance / `0.0.1-dev` GitHub prerelease published |
| Approval recommendation | Record release completion evidence; commit/push of this docs-only update remains pending separate authorization |
| Avast false-positive handling | Vendor response pending; VMF residual risk accepted |
| Vendor clearance | Not obtained |
| Avast safety certification | Not claimed |
| False Positive submission | Submitted 2026-07-25; unanswered as of 2026-08-09 |
| Avast standalone executable scan | No detection observed for `vmf-publisher.exe`; decision input only |
| Avast manual scan / CyberCapture result | `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`; release ZIP / repo Release exe match; Avast showed "このファイルは安全のようです"; no detection name; `IDP.HELU.PSD11` not reproduced; result `not reproduced`; local manual confirmation only |
| Avast setting-dependent observation | Message stopped after changing automatic suspicious-file submission to user-choice handling; decision input only |
| Release readiness | Completed for the `0.0.1-dev` GitHub prerelease; release completion evidence recorded docs-only |
| Release identity | `0.0.1-dev` / `vmf-publisher-v0.0.1-dev`; annotated tag object `a962e19ba2b0a494d1158011ae823d579e41711f`; peeled/package target commit `f08eef306ba82e3ea7f031ef652666178f2f0acf`; evidence docs commit `39df8bedd848da42a4de3cb9461ce4cc86b51197` |
| Final verification | PASS before Live E2E: Release build PASS warnings 0 / errors 0; Unit tests 492 passed / 0 failed / 0 skipped; Integration tests 16 passed / 0 failed / 0 skipped; `dotnet format --verify-no-changes` PASS; `git diff --check` PASS |
| Live E2E | PASS after OAuth Desktop reauthorization refreshed the local authentication state; total 4 / passed 4 / failed 0 / skipped 0 |
| Result review | Recorded; initial Live E2E failure was attributed to stale, revoked, or inconsistent saved OAuth token state; rerun passed after token deletion and OAuth Desktop reauthorization |
| Google Docs / Google Drive mutation | Performed only as part of the authorized Live E2E run; no publication performed |
| Package creation or update by this docs update | Not performed; no `dist` write by this documentation-only update |
| Package path / size / SHA-256 | `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |
| Package generation / verification | PASS / PASS; manifest files 14; secret/static package inspection PASS |
| GitHub Release | Published prerelease `true`: https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev; release name `VMF Publisher 0.0.1-dev` |
| Release asset | `vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; remote asset digest matched local verified package SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |
| Flagged executable re-run | Not performed |
| Release, tag, publication, push | Tag push, GitHub prerelease creation, and asset upload are complete; this docs-only update did not perform a new release operation or push |

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

## 3. Evidence Index

| Evidence | Status | Use |
| --- | --- | --- |
| `docs/development/CURRENT_STATUS.md` | Current status record | Source of the formal current state, published `0.0.1-dev` evidence, unresolved Avast vendor-clearance boundary, and future gated operations. |
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
for the fixed package identity. ADR-0019 records risk acceptance and Hold lift
only.

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

Approval Recommendation = Record release completion evidence; commit/push of
this docs-only update remains pending separate authorization.

Basis:

- Release Hold is lifted by ADR-0019 VMF-side residual risk acceptance;
- Avast vendor response remains pending;
- vendor clearance has not been obtained;
- Avast safety certification is not claimed;
- manual Avast scan / CyberCapture local evidence records `IDP.HELU.PSD11` as
  not reproduced for `vmf-publisher.exe` SHA-256
  `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`, without
  converting the state into vendor clearance or release authorization;
- final verification and Live E2E have passed and result review is recorded;
- package generation and package verification are recorded as `PASS` for the
  fixed `0.0.1-dev` package identity;
- tag push, remote tag readback, GitHub prerelease creation, asset upload, and
  remote/local digest match are recorded as complete for `0.0.1-dev`;
- no package creation, package update, package verification, `dist` write, or
  flagged executable smoke was performed by this docs-only update;
- this docs-only update did not modify tags, GitHub Release, assets,
  production code, tests, package, or `dist`;
- staging, commit, and push of this docs-only update were not authorized.
