# Publisher Release Checklist

Status  : Phase 3-10 Release COMPLETE; 0.0.1-dev GitHub prerelease published
Scope   : VMF Publisher package release readiness
Depends : VMF.Publisher.sln, docs/development/Publisher_Phase3-9_DesignNotes.md, tools/publisher/package-publisher.ps1, tools/publisher/verify-package.ps1

This checklist records release operations for Publisher packages. It is a
release-readiness checklist, not a Frozen specification.

Result codes:

| Code | Meaning |
| --- | --- |
| PASS | The item completed successfully and evidence is recorded. |
| FAIL | The item was executed and did not satisfy the release condition. |
| BLOCKED | The item could not be completed because a required condition is missing. |
| PENDING | The item is waiting on an external response or repository-owner decision. |
| N/A | The item is explicitly not applicable and the reason is recorded. |

## 0. Release Identity: 0.0.1-dev

Status: COMPLETE as published GitHub prerelease; completion evidence recorded
docs-only.

This section records the next Publisher release identity after the completed
`0.0.0-dev` release. It does not modify the completed Phase 3-10
`0.0.0-dev` evidence below.

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Next release version adopted | PASS | `0.0.1-dev` | Codex | 2026-08-09 | Adopted as the next release identity because `0.0.0-dev` is already used as tag, GitHub Release, and asset identity. |
| Git tag published | PASS | `vmf-publisher-v0.0.1-dev`; annotated tag object `a962e19ba2b0a494d1158011ae823d579e41711f`; peeled commit `f08eef306ba82e3ea7f031ef652666178f2f0acf` | Codex | 2026-08-09 | Tag push complete; remote tag readback PASS. |
| Runtime identifier recorded | PASS | `win-x64` | Codex | 2026-08-09 | Recorded from the fixed package identity. |
| Configuration recorded | PASS | `Release` | Codex | 2026-08-09 | Recorded from the fixed package identity. |
| Package type recorded | PASS | `selfContained=false` | Codex | 2026-08-09 | Framework-dependent package identity. |
| Package target commit fixed | PASS | `f08eef306ba82e3ea7f031ef652666178f2f0acf` | Codex | 2026-08-09 | Peeled commit used for package/release target. |
| Evidence docs commit recorded | PASS | `39df8bedd848da42a4de3cb9461ce4cc86b51197` | Codex | 2026-08-09 | Kept distinct from the package target commit. |
| Package artifact recorded | PASS | Asset `vmf-publisher-0.0.1-dev-win-x64.zip`; `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` | Codex | 2026-08-09 | Package identity fixed after package generation and verification. |
| Package generation recorded | PASS | `0.0.1-dev`; `win-x64`; `Release`; `selfContained=false`; manifest files 14 | Codex | 2026-08-09 | This checklist records supplied package-generation evidence; this docs update did not regenerate or modify the package. |
| Package verification recorded | PASS | Package verification PASS; secret/static package inspection PASS | Codex | 2026-08-09 | Verification applies to the fixed package path, size, and SHA-256 above. |
| Identity chain reviewed | PASS | Final verification PASS; Live E2E 4/4 PASS; result review complete; package generation PASS; package verification PASS; tag push complete; remote tag readback PASS; GitHub prerelease creation complete; asset upload complete | Codex | 2026-08-09 | ADR-0016 identity chain is version -> package target commit -> annotated tag / peeled commit -> verified package -> GitHub prerelease asset -> evidence docs commit. |
| GitHub Release published | PASS | https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev; release name `VMF Publisher 0.0.1-dev`; prerelease `true` | Codex | 2026-08-09 | GitHub prerelease creation complete. |
| Asset upload verified | PASS | `vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; remote asset digest matched local SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` | Codex | 2026-08-09 | Asset upload complete; remote asset digest / local verified package SHA-256 match. |
| Prior release identity unchanged | PASS | Existing `0.0.0-dev` tag, GitHub Release, and asset identity unchanged | Codex | 2026-08-09 | Historical identity was not retargeted, deleted, replaced, or reused. |
| Avast clearance boundary preserved | PASS | Vendor clearance not obtained; ADR-0019 VMF-side residual risk acceptance basis retained | Codex | 2026-08-09 | Publication record does not claim Avast vendor clearance or Avast safety certification. |
| Avast manual scan / CyberCapture recorded | PASS | `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`; release ZIP / repo Release exe match; Avast showed "このファイルは安全のようです"; no detection name; `IDP.HELU.PSD11` not reproduced | Codex | 2026-08-11 | Evidence-only local manual confirmation. It does not claim Avast vendor clearance, Avast safety certification, final release authorization, new package work, tag/release, or publication. |
| Documentation update boundary | PASS | Docs-only release completion evidence update | Codex | 2026-08-09 | This update did not modify production code, tests, package, `dist`, tags, GitHub Release, or assets; no staging, commit, or push performed. |

## 1. Release Identity

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Release version recorded | PASS | `0.0.0-dev` | Codex | 2026-07-31 | Phase 3-10 evidence run used `-Version 0.0.0-dev`. |
| Runtime identifier is `win-x64` | PASS | `package-manifest.json`: `runtimeIdentifier=win-x64` | Codex | 2026-07-31 | Package path: `dist\release\Publisher\vmf-publisher-0.0.0-dev-win-x64.zip`. |
| Package type is framework-dependent ZIP | PASS | `package-manifest.json`: `selfContained=false` | Codex | 2026-07-31 | ZIP package; .NET 8 runtime supplied by host. |
| Release notes prepared | PASS | `docs/releases/Publisher_Phase3-10_ReleaseNotes.md` | Codex | 2026-07-31 | Release notes identify Phase 3-10 completion evidence and remaining non-actions. |

## 2. Source Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Branch recorded | PASS | `git branch --show-current` -> `main` | Codex | 2026-07-31 | Checked before verification. |
| Working tree reviewed | PASS | `git status --short` -> no output | Codex | 2026-07-31 | Checked before verification and before documentation updates. |
| `origin/main` synchronized | PASS | `main...origin/main` ahead/behind `0 0` | Codex | 2026-07-31 | `HEAD` and `origin/main` both at `59116070f3258e4eb88201195994fb7267fdb9bb` after `git fetch origin main`. |
| Frozen specs unchanged | PASS | `git diff -- specs` -> no output | Codex | 2026-07-31 | No Frozen specification diff. |
| No staged changes by automation | PASS | `git diff --cached --name-only` -> no output | Codex | 2026-07-31 | No staging performed. |

## 3. Build And Test Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Clean | PASS | `dotnet clean VMF.Publisher.sln --configuration Release` | Codex | 2026-07-31 | Clean succeeded; 0 warnings, 0 errors. |
| Release solution build | PASS | `dotnet build VMF.Publisher.sln --configuration Release` | Codex | 2026-07-31 | Build succeeded; 0 warnings, 0 errors. |
| Publisher unit tests | PASS | `dotnet test VMF.Publisher.sln --configuration Release` | Codex | 2026-07-31 | 461 passed, 0 failed, 0 skipped. Initial sandbox run failed during NuGet signature retrieval; approved external rerun passed. |
| Publisher integration tests | PASS | `dotnet test VMF.Publisher.sln --configuration Release` | Codex | 2026-07-31 | 16 passed, 0 failed, 0 skipped. Initial sandbox run failed during NuGet signature retrieval; approved external rerun passed. |
| Format verification | PASS | `dotnet format VMF.Publisher.sln --verify-no-changes --no-restore` | Codex | 2026-07-29 | Command exited successfully with no changes. |
| Diff whitespace verification | PASS | `git diff --check` -> no output | Codex | 2026-07-29 | No whitespace errors reported. |

## 4. Package Creation

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Package command recorded | PASS | `powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\package-publisher.ps1 -Version 0.0.0-dev` | Codex | 2026-07-31 | Phase 3-10 release execution scope included package creation. |
| Package path recorded | PASS | `dist\release\Publisher\vmf-publisher-0.0.0-dev-win-x64.zip` | Codex | 2026-07-31 | ZIP size: 973097 bytes; SHA-256 `404F6D4B382132802CEF5F42A00A6B53E7C7177E3ABFC56C3DD518DE435C7742`. |
| Package manifest present | PASS | `package-manifest.json` in ZIP | Codex | 2026-07-31 | Manifest readback succeeded through package verification. |
| Framework-dependent package recorded | PASS | `package-manifest.json`: `selfContained=false` | Codex | 2026-07-31 | Runtime identifier: `win-x64`; configuration: `Release`. |
| Configuration files excluded | PASS | `verify-package.ps1` output and manifest readback | Codex | 2026-07-31 | `appsettings.json` and `appsettings.local.json` absent from ZIP. |

## 5. Package Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Required files present | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-31 | Required CLI files and manifest accepted. |
| Manifest paths are safe | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-31 | No unsafe manifest paths reported. |
| File sizes match manifest | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-31 | No size mismatches reported. |
| SHA-256 hashes match manifest | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-31 | No hash mismatches reported. |
| No unmanifested files | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-31 | No unmanifested files reported. |
| No secret-like filenames | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-31 | No secret-like filenames reported. |
| No secret-like content | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-31 | No secret-like content reported. |

## 6. Installation Smoke Test

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| ZIP extracted to temporary local directory | PASS | `C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev` | Codex | 2026-07-29 | Extracted package contained expected CLI files. |
| Runtime verified on target host | PASS | `dotnet --list-runtimes` includes `Microsoft.NETCore.App 8.0.29` | Codex | 2026-07-29 | Compatible .NET 8 runtime present. |
| CLI help succeeds | PASS | Prior Phase 3-9 smoke evidence plus Phase 3-10 Avast resolution instruction | Codex | 2026-07-31 | Repository owner instructed Codex to treat the Avast blocker as resolved for Phase 3-10 release execution. |
| Configuration validation succeeds | PASS | `C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev\vmf-publisher.exe verify` | Codex | 2026-07-29 | Exit 0; `VERIFY_SUCCEEDED`; no Google publish settings required. |
| Markdown compile verification succeeds | PASS | `vmf-publisher.exe verify C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev\smoke-local.md` | Codex | 2026-07-29 | Exit 0; local smoke Markdown compiled. |
| Dry run succeeds without Google writes | PASS | `vmf-publisher.exe dry-run C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev\smoke-local.md` | Codex | 2026-07-29 | Exit 0; `DRY_RUN_SUCCEEDED`; local publish plan had 2 steps. |

## 7. Live E2E Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Live Google operation explicitly authorized | PASS | User explicitly requested Google Live E2E for Phase 3-10 release execution | Codex | 2026-07-31 | Authorization scoped to Success, Revision Conflict, Readback Mismatch, and Empty Plan cases. |
| Credentials kept outside repository and package | PASS | Environment variables populated from local config paths; package verification passed | Codex | 2026-07-31 | Credential content, token-store content, and secret values were not printed or recorded. |
| Token store kept outside repository and package | PASS | OAuth token-store path used only as environment configuration; package verification passed | Codex | 2026-07-31 | Token-store content was not read or recorded. |
| Live E2E integration executed or marked N/A | PASS | `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore --filter FullyQualifiedName~GoogleDocsEndToEndIntegrationTests` | Codex | 2026-07-31 | 4 passed, 0 failed: Success, Revision Conflict, Readback Mismatch, Empty Plan. |
| Temporary public image hosting reviewed | N/A | No temporary public image hosting was enabled | Codex | 2026-07-31 | Live E2E scope did not authorize temporary public hosting. |
| Cleanup completed for temporary external resources | PASS | Live E2E completed successfully | Codex | 2026-07-31 | No cleanup failure was reported by the test run. |

## 8. Release Decision

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| All required checklist items are PASS or approved N/A | PASS | this checklist | Codex | 2026-07-31 | Phase 3-10 release completion evidence is recorded. Release announcement remains unexecuted. |
| Release notes identify scope and exclusions | PASS | `Publisher_Phase3-10_ReleaseNotes.md` | Codex | 2026-07-31 | Notes identify ZIP distribution, Live E2E result, artifact hash, and remaining non-actions. |
| Known issues recorded | PASS | this checklist | Codex | 2026-07-31 | Initial sandbox `dotnet test` failed on NuGet signature retrieval; approved external rerun passed. Initial LiveE2E command had PowerShell quoting failure before tests started; corrected command passed. |
| Release approved or rejected | PASS | Repository-owner Phase 3-10 Release Execution instruction | Codex | 2026-07-31 | Release completed as a GitHub prerelease. |

## 9. Phase 3-10 Release Gate Finalization

Phase 3-10 records the approved release execution and GitHub prerelease
completion evidence. It does not recreate packages, retarget tags, modify the
published GitHub Release, announce a release, or change production behavior.

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Avast blocker resolved for release execution | PASS | Repository-owner instruction on 2026-07-31 | Codex | 2026-07-31 | Codex was instructed to treat the Avast blocker as resolved for Phase 3-10 release execution. |
| Release status recorded | PASS | COMPLETE | Codex | 2026-07-31 | Phase 3-10 release is complete. |
| Release URL recorded | PASS | https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.0-dev | Codex | 2026-07-31 | GitHub Release publication record. |
| Tag recorded | PASS | `vmf-publisher-v0.0.0-dev` | Codex | 2026-07-31 | Target commit `44eafdb121da56c624ec53b8decdb21fb730df66`. |
| Release state recorded | PASS | Prerelease `true`; draft `false`; latest `false` | Codex | 2026-07-31 | GitHub Release state after publication. |
| Release asset recorded | PASS | `vmf-publisher-0.0.0-dev-win-x64.zip` | Codex | 2026-07-31 | Size 973097 bytes; SHA-256 `404F6D4B382132802CEF5F42A00A6B53E7C7177E3ABFC56C3DD518DE435C7742`. |
| Release approval separated from Phase 4 planning | PASS | this section | Codex | 2026-07-30 | Phase 4 planning may proceed only as non-release work. It must not imply release approval for the Phase 3-9 package. |
| Pending risk owner recorded | PASS | repository-owner completion record | Codex | 2026-07-31 | Repository owner accepted release continuation for this Phase 3-10 prerelease. |
| Pending risk impact recorded | PASS | Remaining non-actions recorded | Codex | 2026-07-31 | Do not recreate packages, retarget tags, modify the GitHub Release, or announce production release without separate authorization. |
| Phase 4 entry conditions recorded | PASS | this section | Codex | 2026-07-30 | Phase 4 may start only if Frozen specs, public APIs, production defaults, package artifacts, and release state remain unchanged unless separately authorized. |

### Phase 4 Entry Conditions

Phase 4 work may begin only when all of the following remain true:

- Work is scoped to planning, implementation, tests, or documentation that does
  not approve, publish, or announce the Phase 3-9 release.
- Frozen specifications, public APIs, persisted schemas, canonical formats,
  and existing production defaults remain unchanged.
- Live Google Docs, Google Drive, token stores, temporary public hosting,
  release tags, and distribution publication remain unchanged unless explicitly
  authorized for that specific operation.
- Any required improvement that would change a Frozen specification, public
  contract, package trust posture, signing model, installer model, or production
  release process is recorded separately as a vNext candidate before adoption.

### Pending Risk Closure Criteria

The Avast pending risk can be closed only by recording one of these outcomes:

- Vendor-clearance evidence is accepted for the selected artifact identity:
  latest-definition rescan evidence is recorded, detection removal or
  non-reproduction is evidenced, and the responsible owner approves the
  vendor-clearance determination. An Avast response can support this record,
  but Avast silence cannot.
- Avast continues to classify the executable as unsafe and the repository owner
  rejects release continuation or requires remediation.
- The repository owner explicitly accepts the AV exception posture without
  waiting further and records that decision as the release approval basis.

Release completion requires all required items to be PASS or explicitly
approved as N/A. A failed package verification, missing release evidence,
unapproved live external update, secret exposure, or Frozen specification
change blocks the release.
