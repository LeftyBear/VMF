# Publisher 0.0.1-dev Release Authorization Record

Status  : AUTHORIZED - release-path continuation record only / no release operation performed
Scope   : Docs-only release authorization record for `0.0.1-dev` continuation
Date    : 2026-08-11
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_0.0.1-dev_FinalVerificationOnlyRecord_2026-08-11.md, docs/architecture/ADR-0019-vmf-risk-acceptance-and-release-hold-lift.md

## 1. Authorization

| Item | Value |
| --- | --- |
| Authorizer | Repository owner |
| Timestamp | 2026-08-11 14:09:22 +09:00 |
| Target version | `0.0.1-dev` |
| Target HEAD | `9f3f6f193d65801d4748c8cb21dd32adb7dbf371` |
| Target `origin/main` | `9f3f6f193d65801d4748c8cb21dd32adb7dbf371` |
| Authorization type | Release-path continuation decision record |
| Authorized scope in this task | Create this release authorization record docs-only and commit it docs-only |
| Release execution authorized by this record | No |

Repository owner decision:

Authorize release-path continuation for VMF Publisher `0.0.1-dev` based on
ADR-0019 risk acceptance, Avast manual scan not reproduced, final verification
only `PASS`, vendor clearance not obtained, and Avast safety certification not
claimed.

This record authorizes only the creation and preservation of this release
authorization record. It does not authorize or perform package creation,
package replacement, `dist` updates, ZIP recreation, tag creation, GitHub
Release creation or update, publication, Live E2E, Google Docs or Google Drive
mutation, OAuth credential or token-store operations, Avast UI operations,
Avast setting changes, quarantine release, exclusion creation, or push.

## 2. Decision Basis

| Item | Value | Interpretation |
| --- | --- | --- |
| Target version | `0.0.1-dev` | Fixed target of this continuation record. |
| Package path | `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip` | Existing package identity only; no package or `dist` update authorized. |
| Package SHA-256 | `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` | Existing package hash accepted as the package identity for this decision record. |
| Executable SHA-256 | `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f` | Existing `vmf-publisher.exe` identity for the manual Avast observation. |
| Final verification only | PASS | Final verification evidence only; no release operation performed by that record. |
| Avast manual scan / CyberCapture result | `IDP.HELU.PSD11` not reproduced | Local manual confirmation only. |
| Avast vendor clearance | Not obtained | This record does not convert local evidence into vendor clearance. |
| Avast safety certification | Not claimed | This record does not claim Avast certified, approved, cleared, whitelisted, or safety-recognized the executable or package. |
| Risk-acceptance basis | ADR-0019 VMF-side residual risk acceptance | Continuation proceeds on VMF risk acceptance, not on Avast vendor clearance. |

## 3. Authorized Boundary

The authorized continuation in this task is limited to the release
authorization record itself. This record is a decision traceability artifact
for the release path. It is not a package artifact, distribution artifact,
Release Publication Record, post-release evidence replacement, tag, GitHub
Release, release asset, or vendor response.

The following operations remain outside this record's authorization:

- package creation, package replacement, package update, ZIP recreation, or
  any `dist` write;
- tag creation, tag push, GitHub Release creation or update, asset upload,
  publication, or release announcement;
- Live E2E or any credentialed Google operation;
- Google Docs mutation or Google Drive mutation;
- OAuth credential or token-store operation;
- Avast UI operation, Avast setting change, quarantine release, or exclusion
  creation;
- executable smoke run or flagged executable re-run;
- production code change, test change, Frozen specification change, public API
  change, persisted schema change, or canonical format change;
- push.

## 4. Non-Actions In This Docs-Only Record

This docs-only authorization record did not run build, tests, final
verification, Live E2E, package verification, package creation, package update,
ZIP creation, `vmf-publisher.exe`, tag creation, GitHub Release creation or
update, publication, Google Docs mutation, Google Drive mutation, OAuth
credential operation, token-store operation, Avast UI operation, Avast setting
change, quarantine release, exclusion creation, production code change, test
change, Frozen specification change, public API change, or push.
