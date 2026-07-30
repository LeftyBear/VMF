# Publisher Release Checklist

Status  : Phase 3-10 Release Gate Finalization
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

## 9. Phase 3-10 Release Gate Finalization

Phase 3-10 does not approve the release, create tags, publish artifacts,
execute Live E2E, or change production behavior. It records the final gate
state needed to continue planning while the release approval remains dependent
on the external Avast classification response.

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Avast false positive response tracked as external dependency | PENDING | user reported submission completed; Avast response pending | Codex | 2026-07-30 | Release approval is pending external vendor response. The package may not be treated as approved until Avast responds or the repository owner explicitly accepts the AV exception posture. |
| Release approval separated from Phase 4 planning | PASS | this section | Codex | 2026-07-30 | Phase 4 planning may proceed only as non-release work. It must not imply release approval for the Phase 3-9 package. |
| Pending risk owner recorded | PASS | repository owner decision required | Codex | 2026-07-30 | Repository owner owns the final accept/reject decision after Avast response or explicit exception acceptance. |
| Pending risk impact recorded | PASS | Phase 3-9 package release approval remains blocked | Codex | 2026-07-30 | Do not create release tags, publish distribution artifacts, announce production release, or mutate live external services while this item is pending. |
| Phase 4 entry conditions recorded | PASS | this section | Codex | 2026-07-30 | Phase 4 may start only if Frozen specs, public APIs, production defaults, package artifacts, and release state remain unchanged unless separately authorized. |

### Phase 4 Entry Conditions

Phase 4 work may begin while the Avast response is pending only when all of the
following remain true:

- Work is scoped to planning, implementation, tests, or documentation that does
  not approve, publish, or announce the Phase 3-9 release.
- Frozen specifications, public APIs, persisted schemas, canonical formats,
  and existing production defaults remain unchanged.
- Live Google Docs, Google Drive, token stores, temporary public hosting,
  release tags, and distribution publication remain disabled unless explicitly
  authorized for that specific operation.
- Any required improvement that would change a Frozen specification, public
  contract, package trust posture, signing model, installer model, or production
  release process is recorded separately as a vNext candidate before adoption.

### Pending Risk Closure Criteria

The Avast pending risk can be closed only by recording one of these outcomes:

- Avast confirms the package or executable is not malicious and the repository
  owner approves release continuation.
- Avast continues to classify the executable as unsafe and the repository owner
  rejects release continuation or requires remediation.
- The repository owner explicitly accepts the AV exception posture without
  waiting further and records that decision as the release approval basis.

Release approval requires all required items to be PASS or explicitly approved
as N/A. A failed package verification, missing release evidence, unapproved
live external update, secret exposure, or Frozen specification change blocks
the release.
