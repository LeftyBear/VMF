# Publisher Release Identity Reconciliation Record

Status  : NO-GO / release execution blocked by identity inconsistency
Date    : 2026-08-12
Scope   : Publisher `0.0.1-dev` release identity reconciliation before any further release execution
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/distribution/ReleaseChecklist.md, docs/distribution/PublisherReleaseRunbook.md, docs/releases/Publisher_0.0.1-dev_PrereleaseRecord_2026-08-12.md, docs/releases/Publisher_0.0.1-dev_FinalStatusFreeze_2026-08-12.md

This record reconciles the conflicting Publisher `0.0.1-dev` release identity
records before any further release operation. It is documentation-only. It does
not create or update packages, modify `dist`, create, move, retarget, delete,
or push tags, create or update a GitHub Release or prerelease, upload, replace,
or delete release assets, publish artifacts, execute Live E2E, mutate Google
Docs or Google Drive, operate on OAuth/token-store/credentials, operate on
Avast, run `vmf-publisher.exe`, re-run a flagged executable, change production
code, change tests, modify Frozen specifications, change public APIs, stage,
commit, or push.

## 1. Reconciliation Decision

Publisher `0.0.1-dev` is treated as an existing published GitHub prerelease.

The canonical published identity is the later `publisher-v0.0.1-dev` GitHub
prerelease identity recorded in
`docs/releases/Publisher_0.0.1-dev_PrereleaseRecord_2026-08-12.md` and
`docs/releases/Publisher_0.0.1-dev_FinalStatusFreeze_2026-08-12.md`, and
confirmed by read-only GitHub release metadata.

| Field | Canonical value |
| --- | --- |
| Product | VMF Publisher |
| Version | `0.0.1-dev` |
| Published state | Existing GitHub prerelease |
| GitHub Release URL | https://github.com/LeftyBear/VMF/releases/tag/publisher-v0.0.1-dev |
| Tag | `publisher-v0.0.1-dev` |
| Annotated tag object | `04a101729dbab431f9e67e1b7e43e6b9a94dd6e0` |
| Peeled / target commit | `382bd715d8307930d0aeb8bd48116dac3f57af5c` |
| Release title | `VMF Publisher 0.0.1-dev` |
| Prerelease | `true` |
| Draft | `false` |
| Published artifact name | `vmf-publisher-0.0.1-dev-win-x64.zip` |
| Published artifact size | 983422 bytes |
| Published artifact SHA-256 | `0174810d21c6072b8206acf2fed90b72c2e6be499c65b231d7d72d71fd69cb76` |
| Published artifact URL | https://github.com/LeftyBear/VMF/releases/download/publisher-v0.0.1-dev/vmf-publisher-0.0.1-dev-win-x64.zip |
| Assets count | 1 |

## 2. Evidence Used

Read-only evidence collected for this reconciliation:

| Evidence | Result |
| --- | --- |
| `git status --short --branch` | `main...origin/main`; working tree clean before this record was added |
| `git ls-remote --tags origin` | `publisher-v0.0.1-dev` tag object `04a101729dbab431f9e67e1b7e43e6b9a94dd6e0`; peeled commit `382bd715d8307930d0aeb8bd48116dac3f57af5c`; older `vmf-publisher-v0.0.1-dev` tag also exists |
| `gh release view publisher-v0.0.1-dev --repo LeftyBear/VMF --json ...` | Release URL, tag, target commit, prerelease/draft state, one asset, size, digest, and asset URL matched the canonical identity above |
| Local `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip` size | 983422 bytes |
| Local `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip` SHA-256 | `0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76` |

The local `dist` ZIP currently matches the canonical published artifact by
name, size, and SHA-256. It must not be regenerated, replaced, deleted, or
re-uploaded without separate package / `dist` and asset-operation
authorization.

## 3. Superseded Or Non-Canonical Identity Records

Several earlier records identify `0.0.1-dev` as:

| Field | Earlier / non-canonical value |
| --- | --- |
| Tag | `vmf-publisher-v0.0.1-dev` |
| Annotated tag object | `a962e19ba2b0a494d1158011ae823d579e41711f` |
| Peeled / package target commit | `f08eef306ba82e3ea7f031ef652666178f2f0acf` |
| GitHub Release URL | https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev |
| Artifact size | 983404 bytes |
| Artifact SHA-256 | `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |

Those values are retained only as historical or superseded release-identity
records. They are not the canonical current published prerelease identity for
Publisher `0.0.1-dev`.

Do not retarget, delete, replace, or reuse the older tag or any related release
record without separate explicit authorization. Do not use the older 983404
byte / `73582c...` identity as the current published artifact identity unless
a later approved correction record supersedes this reconciliation.

## 4. Execution State

The current execution state is:

`NO-GO / release execution blocked by identity inconsistency`.

This means:

- the release is recognized as an existing published GitHub prerelease under
  the canonical identity above;
- the identity inconsistency is documented, but broader status files have not
  yet been synchronized in this record;
- normal release execution must not proceed until downstream references are
  updated or explicitly reviewed against this canonical identity;
- package / `dist` updates remain blocked;
- tag operations remain blocked;
- GitHub Release or asset publication operations remain blocked;
- Live E2E remains blocked unless separately authorized for a later step;
- Google Docs and Google Drive mutation remain blocked;
- flagged executable re-run remains blocked.

## 5. Required Follow-Up Before Any Release Execution

Before any further release-path operation, reconcile downstream records that
still cite the non-canonical `vmf-publisher-v0.0.1-dev` / 983404 byte /
`73582c...` identity as current state.

At minimum, review and update or annotate:

- `docs/development/CURRENT_STATUS.md`;
- `docs/development/Publisher_ReleaseApprovalPackage.md`;
- `docs/distribution/ReleaseChecklist.md`;
- `docs/distribution/PublisherReleaseRunbook.md`;
- `docs/releases/Publisher_0.0.1-dev_ReleaseNotes.md`;
- `docs/development/Test_Traceability_Matrix.md`;
- `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`;
- `CHANGELOG.md`.

Any such update must preserve historical records without implying that this
reconciliation performed a release, tag, package, asset, Live E2E, Google,
Avast, executable, commit, or push operation.

## 6. Non-Actions

This reconciliation did not:

- create, regenerate, replace, delete, or verify a package;
- update, clean, or rewrite `dist`;
- create, move, retarget, delete, or push a tag;
- create, update, delete, or replace a GitHub Release or prerelease;
- upload, replace, or delete a release asset;
- publish artifacts or announce a release;
- execute Live E2E;
- mutate Google Docs or Google Drive;
- operate on OAuth, token-store, credentials, or private provider state;
- operate on Avast or process an Avast vendor response;
- run or re-run `vmf-publisher.exe`;
- claim Avast vendor clearance;
- claim Avast safety certification;
- change production code, tests, public APIs, persisted schemas, canonical
  formats, or Frozen specifications;
- stage, commit, or push.
