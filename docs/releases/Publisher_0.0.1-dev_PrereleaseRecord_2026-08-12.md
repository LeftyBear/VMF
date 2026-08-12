# Publisher 0.0.1-dev Prerelease Record

Status  : GitHub prerelease record created; single asset attached
Scope   : Publisher `0.0.1-dev` GitHub prerelease record and single asset attach evidence only
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md

This record documents the Step 4 GitHub prerelease record creation and Step 4-b
single asset attach for Publisher `0.0.1-dev`. It records only the GitHub
Release metadata operation and single GitHub Release asset upload authorized for
these steps.

## Release Record

| Item | Value |
| --- | --- |
| Release URL | https://github.com/LeftyBear/VMF/releases/tag/publisher-v0.0.1-dev |
| Tag | `publisher-v0.0.1-dev` |
| Target commit | `382bd715d8307930d0aeb8bd48116dac3f57af5c` |
| Release title | `VMF Publisher 0.0.1-dev` |
| Release type | Prerelease |
| Prerelease | `true` |
| Draft | `false` |
| Assets | `vmf-publisher-0.0.1-dev-win-x64.zip` |

## Step 4-b Asset Attach Evidence

| Item | Value |
| --- | --- |
| Upload authorization | Explicitly authorized for Publisher `0.0.1-dev`, limited to one recorded artifact |
| Local path | `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip` |
| Local size | 983422 bytes |
| Local SHA-256 | `0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76` |
| Uploaded asset name | `vmf-publisher-0.0.1-dev-win-x64.zip` |
| Uploaded asset size | 983422 bytes |
| Uploaded asset URL | https://github.com/LeftyBear/VMF/releases/download/publisher-v0.0.1-dev/vmf-publisher-0.0.1-dev-win-x64.zip |
| Remote asset digest | `sha256:0174810d21c6072b8206acf2fed90b72c2e6be499c65b231d7d72d71fd69cb76` |
| Assets count after upload | 1 |
| Readback result | PASS: asset name, size, URL, digest, and assets count matched the authorized single-asset scope |

## Release Notes Boundary

The GitHub prerelease notes state:

- Avast vendor clearance has not been obtained.
- Avast safety certification is not claimed.
- This release proceeds under VMF-side residual risk acceptance and release
  authorization.
- Artifact evidence recorded separately with SHA-256.
- Release scope is limited to Publisher 0.0.1-dev.

## Prohibited Operations

No asset replacement or asset deletion was performed.

No package or `dist` update, Live E2E, Google Docs or Google Drive mutation,
OAuth operation, Avast rerun, flagged executable re-run, Avast vendor-clearance
claim, or Avast safety-certification claim was performed by these prerelease
record steps.
