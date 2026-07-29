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
| Release version recorded |  |  |  |  |  |
| Runtime identifier is `win-x64` |  |  |  |  |  |
| Package type is framework-dependent ZIP |  |  |  |  |  |
| Release notes prepared |  | `docs/releases/Publisher_Phase3-9_ReleaseNotes.md` |  |  |  |

## 2. Source Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Branch recorded |  | `git branch --show-current` |  |  |  |
| Working tree reviewed |  | `git status --short` |  |  |  |
| Frozen specs unchanged |  | `git diff -- specs` |  |  |  |
| No staged changes by automation |  | `git status --short` |  |  |  |

## 3. Build And Test Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Release solution build |  | `dotnet build VMF.Publisher.sln --configuration Release --no-restore` |  |  |  |
| Publisher unit tests |  | `dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore` |  |  |  |
| Publisher integration tests |  | `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore` |  |  |  |
| Format verification |  | `dotnet format VMF.Publisher.sln --verify-no-changes --no-restore` |  |  |  |
| Diff whitespace verification |  | `git diff --check` |  |  |  |

## 4. Package Creation

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Package command recorded |  | `powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\package-publisher.ps1 -Version <version>` |  |  |  |
| Package path recorded |  | `dist\release\Publisher\vmf-publisher-<version>-win-x64.zip` |  |  |  |
| Package manifest present |  | `package-manifest.json` in ZIP |  |  |  |
| Framework-dependent package recorded |  | `selfContained=false` in manifest |  |  |  |
| Configuration files excluded |  | `verify-package.ps1` output |  |  |  |

## 5. Package Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Required files present |  | `verify-package.ps1` output |  |  |  |
| Manifest paths are safe |  | `verify-package.ps1` output |  |  |  |
| File sizes match manifest |  | `verify-package.ps1` output |  |  |  |
| SHA-256 hashes match manifest |  | `verify-package.ps1` output |  |  |  |
| No unmanifested files |  | `verify-package.ps1` output |  |  |  |
| No secret-like filenames |  | `verify-package.ps1` output |  |  |  |
| No secret-like content |  | `verify-package.ps1` output |  |  |  |

## 6. Installation Smoke Test

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| ZIP extracted to temporary local directory |  | extraction path |  |  |  |
| Runtime verified on target host |  | `dotnet --list-runtimes` |  |  |  |
| CLI help succeeds |  | `vmf-publisher.exe --help` |  |  |  |
| Configuration validation succeeds |  | `vmf-publisher.exe verify` |  |  |  |
| Markdown compile verification succeeds |  | `vmf-publisher.exe verify <markdown-file>` |  |  |  |
| Dry run succeeds without Google writes |  | `vmf-publisher.exe dry-run <markdown-file>` |  |  |  |

## 7. Live E2E Verification

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| Live Google operation explicitly authorized |  | approval record |  |  |  |
| Credentials kept outside repository and package |  | path policy confirmation only |  |  |  |
| Token store kept outside repository and package |  | path policy confirmation only |  |  |  |
| Live E2E integration executed or marked N/A |  | `docs/distribution/LiveE2EOperations.md` |  |  |  |
| Temporary public image hosting reviewed |  | setting confirmation |  |  |  |
| Cleanup completed for temporary external resources |  | operation record |  |  |  |

## 8. Release Decision

| Item | Result | Evidence | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- |
| All required checklist items are PASS or approved N/A |  | this checklist |  |  |  |
| Release notes identify scope and exclusions |  | `Publisher_Phase3-9_ReleaseNotes.md` |  |  |  |
| Known issues recorded |  | release notes |  |  |  |
| Release approved or rejected |  | decision record |  |  |  |

Release approval requires all required items to be PASS or explicitly approved
as N/A. A failed package verification, missing release evidence, unapproved
live external update, secret exposure, or Frozen specification change blocks
the release.

