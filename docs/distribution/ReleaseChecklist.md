# Publisher Release Checklist

Status  : Phase 3-9D Operations Checklist
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
| N/A | The item is explicitly not applicable and the reason is recorded. |

## 1. Release Identity

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Release version recorded | PASS | `0.0.0-dev` | Codex | 2026-07-29 | Evidence run used `-Version 0.0.0-dev`. |
| Runtime identifier is `win-x64` | PASS | `package-manifest.json`: `runtimeIdentifier=win-x64` | Codex | 2026-07-29 | Package path: `dist\release\Publisher\vmf-publisher-0.0.0-dev-win-x64.zip`. |
| Package type is framework-dependent ZIP | PASS | `package-manifest.json`: `selfContained=false` | Codex | 2026-07-29 | ZIP package; .NET 8 runtime supplied by host. |
| Release notes prepared | PASS | `docs/releases/Publisher_Phase3-9_ReleaseNotes.md` | Codex | 2026-07-29 | Release notes identify Phase 3-9 scope and exclusions. |

## 2. Source Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Branch recorded | PASS | `git branch --show-current` -> `main` | Codex | 2026-07-29 | Checked before verification. |
| Working tree reviewed | PASS | `git status --short` -> no output | Codex | 2026-07-29 | Checked before verification and after package creation. |
| Frozen specs unchanged | PASS | `git diff -- specs` -> no output | Codex | 2026-07-29 | No Frozen specification diff. |
| No staged changes by automation | PASS | `git diff --cached --name-only` -> no output | Codex | 2026-07-29 | No staging performed. |

## 3. Build And Test Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Release solution build | PASS | `dotnet build VMF.Publisher.sln --configuration Release --no-restore` | Codex | 2026-07-29 | Build succeeded; 0 warnings, 0 errors. |
| Publisher unit tests | PASS | `dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore` | Codex | 2026-07-29 | 461 passed, 0 failed, 0 skipped. |
| Publisher integration tests | PASS | `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore` | Codex | 2026-07-29 | 16 passed, 0 failed, 0 skipped. Live E2E was not enabled. |
| Format verification | PASS | `dotnet format VMF.Publisher.sln --verify-no-changes --no-restore` | Codex | 2026-07-29 | Command exited successfully with no changes. |
| Diff whitespace verification | PASS | `git diff --check` -> no output | Codex | 2026-07-29 | No whitespace errors reported. |

## 4. Package Creation

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Package command recorded | PASS | `powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\package-publisher.ps1 -Version 0.0.0-dev` | Codex | 2026-07-29 | Initial run hit access denied replacing existing ZIP; targeted ZIP removal then rerun succeeded. |
| Package path recorded | PASS | `dist\release\Publisher\vmf-publisher-0.0.0-dev-win-x64.zip` | Codex | 2026-07-29 | ZIP size: 973120 bytes. |
| Package manifest present | PASS | `package-manifest.json` in ZIP | Codex | 2026-07-29 | Manifest readback succeeded; file inventory count: 14. |
| Framework-dependent package recorded | PASS | `package-manifest.json`: `selfContained=false` | Codex | 2026-07-29 | Runtime identifier: `win-x64`; configuration: `Release`. |
| Configuration files excluded | PASS | `verify-package.ps1` output and manifest readback | Codex | 2026-07-29 | `appsettings.json` and `appsettings.local.json` absent from ZIP. |

## 5. Package Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Required files present | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-29 | Required CLI files and manifest accepted. |
| Manifest paths are safe | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-29 | No unsafe manifest paths reported. |
| File sizes match manifest | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-29 | No size mismatches reported. |
| SHA-256 hashes match manifest | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-29 | No hash mismatches reported. |
| No unmanifested files | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-29 | No unmanifested files reported. |
| No secret-like filenames | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-29 | No secret-like filenames reported. |
| No secret-like content | PASS | `verify-package.ps1`: package verification passed | Codex | 2026-07-29 | No secret-like content reported. |

## 6. Installation Smoke Test

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| ZIP extracted to temporary local directory | PASS | `C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev` | Codex | 2026-07-29 | Extracted package contained expected CLI files. |
| Runtime verified on target host | PASS | `dotnet --list-runtimes` includes `Microsoft.NETCore.App 8.0.29` | Codex | 2026-07-29 | Compatible .NET 8 runtime present. |
| CLI help succeeds | BLOCKED | `C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev\vmf-publisher.exe --help` | Codex | 2026-07-29 | Re-run exited 0, but user later reported Avast blocked `vmf-publisher.exe` as `IDP.HELU.PSD11`. Static triage: exe SHA-256 matches manifest, ZIP verification passed, exe is not Authenticode signed, Defender scan could not run because the feature is disabled, VirusTotal reported no detection, and Avast exception handling was applied. |
| Configuration validation succeeds | PASS | `C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev\vmf-publisher.exe verify` | Codex | 2026-07-29 | Exit 0; `VERIFY_SUCCEEDED`; no Google publish settings required. |
| Markdown compile verification succeeds | PASS | `vmf-publisher.exe verify C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev\smoke-local.md` | Codex | 2026-07-29 | Exit 0; local smoke Markdown compiled. |
| Dry run succeeds without Google writes | PASS | `vmf-publisher.exe dry-run C:\Users\biz\AppData\Local\Temp\vmf-publisher-smoke-0.0.0-dev\smoke-local.md` | Codex | 2026-07-29 | Exit 0; `DRY_RUN_SUCCEEDED`; local publish plan had 2 steps. |

## 7. Live E2E Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Live Google operation explicitly authorized | N/A | User approved N/A for this release-readiness pass | Codex | 2026-07-29 | Live E2E is a separate gate and was not authorized for execution. |
| Credentials kept outside repository and package | N/A | No live credentials used; `verify-package.ps1` passed | Codex | 2026-07-29 | Package contains no bundled configuration files or secret-like content. |
| Token store kept outside repository and package | N/A | No live token store used; `verify-package.ps1` passed | Codex | 2026-07-29 | No token-store path or token content was read or recorded. |
| Live E2E integration executed or marked N/A | N/A | `docs/distribution/LiveE2EOperations.md`; user approved N/A | Codex | 2026-07-29 | Not executed; local checks must not be reported as Google Docs readback. |
| Temporary public image hosting reviewed | N/A | No live publish or temporary public image hosting executed | Codex | 2026-07-29 | Separate approval required before enabling temporary public hosting. |
| Cleanup completed for temporary external resources | N/A | No external resources created | Codex | 2026-07-29 | No Google Docs or Drive cleanup required for this pass. |

## 8. Release Decision

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| All required checklist items are PASS or approved N/A | BLOCKED | this checklist | Codex | 2026-07-29 | Avast reported `vmf-publisher.exe` as `IDP.HELU.PSD11`; VirusTotal reported no detection, Avast exception handling was applied, and false positive submission was reported completed. Final release approval remains blocked pending Avast classification response or repository-owner acceptance of the AV exception posture. |
| Release notes identify scope and exclusions | PASS | `Publisher_Phase3-9_ReleaseNotes.md` | Codex | 2026-07-29 | Notes identify ZIP distribution, configuration exclusion, and Live E2E gate. |
| Known issues recorded | PASS | this checklist | Codex | 2026-07-29 | Initial package replacement access denial and initial CLI help timeout were resolved by targeted retry. Avast later reported `vmf-publisher.exe` as `IDP.HELU.PSD11`; VirusTotal reported no detection, Avast exception handling was applied, and false positive submission was reported completed with Avast classification pending. Static triage recorded ZIP SHA-256 `D9C5C6E5269D20ED01447AB7738728ACD41CCF0E308688DF11D08143DC87C244` and exe SHA-256 `DF49E365A698A9C885C497DD5972B313708E92F127BE5E4CE786AFA88941FFCA`. |
| Release approved or rejected | BLOCKED | decision record pending | Codex | 2026-07-29 | Final release approval or rejection remains a repository-owner decision. |

Release approval requires all required items to be PASS or explicitly approved
as N/A. A failed package verification, missing release evidence, unapproved
live external update, secret exposure, or Frozen specification change blocks
the release.
