# Publisher 0.0.1-dev Tag Creation Authorization

Status  : Approved for tag creation only
Date    : 2026-08-12
Scope   : Publisher `0.0.1-dev` tag creation authorization
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_ResidualRiskReleaseAuthorizationApprovalMemo_2026-08-12.md, docs/development/Publisher_0.0.1-dev_FinalScopeConfirmation_2026-08-12.md

This record documents explicit authorization to create one Git tag for
Publisher `0.0.1-dev`, limited to the confirmed release scope and recorded
package evidence.

## 1. Authorization Statement

Tag creation is explicitly authorized for Publisher `0.0.1-dev`, limited to
the confirmed release scope and recorded package evidence.

| Item | Authorized Value |
| --- | --- |
| Authorized tag | `publisher-v0.0.1-dev` |
| Reference commit | `382bd715d8307930d0aeb8bd48116dac3f57af5c` |

## 2. Authorization Boundary

This authorization is limited to creating the named Git tag at the named
reference commit.

This authorization does not include GitHub release or prerelease update, asset
operation, publication, Live E2E, Google Docs or Google Drive mutation, OAuth
operation, Avast rerun, flagged executable re-run, Avast vendor-clearance
claim, or Avast safety-certification claim.

This authorization also does not authorize package creation, package
replacement, `dist` mutation, ZIP mutation, production code change, test
change, Frozen specification change, public API change, staging, commit, push,
merge, rebase, reset, stash, or history rewrite.

## 3. Non-Actions In This Record

This documentation record did not create or update a GitHub Release or
prerelease, upload or modify assets, publish artifacts, execute Live E2E,
mutate Google Docs or Google Drive, operate on OAuth/token-store/credentials,
operate on Avast, run or rerun `vmf-publisher.exe`, create or replace
packages, modify `dist`, change production code, change tests, modify Frozen
specifications, change public APIs, or claim Avast vendor clearance or Avast
safety certification.
