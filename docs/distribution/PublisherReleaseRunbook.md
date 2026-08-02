# Publisher Release Runbook

Status  : Draft
Scope   : VMF Publisher release operation procedure
Depends : docs/distribution/ReleaseChecklist.md, docs/distribution/LiveE2EOperations.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_TestClassification.md

This runbook defines the controlled operating procedure for a VMF Publisher
release. It is an operations guide only. It does not approve a release, create
or update packages, create tags, publish artifacts, execute Live E2E, mutate
Google Docs or Google Drive, change production design, change public APIs, or
modify Frozen specifications.

## 1. Purpose

Use this runbook when preparing, verifying, approving, or publishing a VMF
Publisher package.

The runbook exists to keep these activities separate:

- local source verification;
- package creation;
- package verification;
- Live E2E;
- security and supply-chain review;
- repository-owner go/no-go approval;
- tag creation and GitHub Release publication;
- post-release status recording.

Each gate must be completed or explicitly marked not applicable before the next
gate is treated as available.

## 2. Current Release Boundary

Before starting a release operation, read:

- `docs/development/CURRENT_STATUS.md`;
- `docs/development/Publisher_TestClassification.md`;
- `docs/distribution/ReleaseChecklist.md`;
- `docs/distribution/LiveE2EOperations.md`;
- the release-specific notes or readiness documents for the target version.

If the current status records the release gate as blocked or deferred, do not
proceed past local-only checks without explicit repository-owner authorization.

Local-only verification does not establish:

- release approval;
- package approval;
- Live E2E success;
- Google Docs API readback;
- Google Drive cleanup verification;
- antivirus vendor clearance;
- permission to create tags or publish artifacts.

## 3. Required Inputs

Record these inputs before any release command is executed:

| Input | Required Evidence |
| --- | --- |
| Release version | Exact version string. |
| Target branch | Current branch and expected release branch. |
| Target commit | Commit SHA intended for packaging, tag, and release record. |
| Package type | Framework-dependent or self-contained. |
| Runtime identifier | Expected RID, for example `win-x64`. |
| Release checklist | Path to the checklist being updated. |
| Release notes | Path to release notes for the target version. |
| Live E2E decision | Authorized, explicitly N/A, or blocked. |
| Security decision | Scanner evidence, vendor result, or owner exception basis. |
| Publication decision | Draft, prerelease, latest, or no publication. |

Stop if the version, target commit, artifact path, or release notes are
ambiguous.

## 4. Authorization Gates

The following operations require explicit authorization for the specific run:

- package creation or package update;
- execution of a packaged executable that is currently flagged by antivirus;
- Live E2E or any credentialed Google operation;
- temporary public image hosting;
- Google Docs or Google Drive mutation;
- release tag creation;
- GitHub Release creation or update;
- release announcement;
- staging, commit, push, merge, rebase, reset, stash, or history rewrite.

Authorization for one gate does not authorize another gate.

## 5. Preflight

Run preflight before build, package, or release operations.

```powershell
git branch --show-current
git status --short
git diff -- specs
git diff --cached --name-only
```

Expected result:

- branch matches the approved release branch;
- working tree state is understood and recorded;
- no unexpected staged changes exist;
- Frozen specifications are unchanged unless explicitly authorized.

Stop if unrelated changes are present and their ownership or release impact is
unclear.

## 6. Local Verification

Run local verification in this order unless the release-specific instructions
define a narrower approved set:

```powershell
dotnet build VMF.Publisher.sln --configuration Release --no-restore
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
```

Record:

- command executed;
- pass or failure result;
- passed, failed, and skipped test counts;
- warning and error counts when reported;
- whether `VMF_PUBLISHER_GOOGLE_E2E` was disabled;
- any unexecuted checks and the reason.

Do not continue after a failed local verification step unless the repository
owner records an explicit decision to stop, defer, or rerun after remediation.

## 7. Package Creation

Package creation is a separate authorization gate. When authorized, run the
approved packaging command for the target version:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\package-publisher.ps1 -Version <version>
```

Record:

- command executed;
- package path;
- package size;
- package SHA-256;
- manifest values including version, runtime identifier, configuration, and
  package type;
- whether configuration files and secrets are excluded.

The package path, hash, and target commit must remain stable for the rest of
the release operation. If the package is recreated, restart package
verification and release evidence recording for the new artifact.

## 8. Package Verification

Verify the selected package before any release publication:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip
```

Record:

- required files present;
- manifest paths safe;
- manifest sizes match package files;
- manifest SHA-256 hashes match package files;
- no unmanifested files;
- no secret-like filenames;
- no secret-like content;
- configuration files excluded.

A package verification failure blocks release publication.

## 9. Installation Smoke

When executable smoke testing is authorized, extract the package to a temporary
local directory and run the approved smoke commands:

```powershell
dotnet --list-runtimes
vmf-publisher.exe --help
vmf-publisher.exe verify
vmf-publisher.exe verify .\smoke-local.md
vmf-publisher.exe dry-run .\smoke-local.md
```

Record:

- extraction path;
- .NET runtime evidence;
- command results and exit codes;
- smoke input path;
- dry-run result and plan summary.

During an unresolved antivirus handling period, do not run flagged executables
unless the repository owner explicitly authorizes that exact run.

## 10. Live E2E

Live E2E is never implicit. Follow `docs/distribution/LiveE2EOperations.md`.

Before execution, record approval for:

- Google account or service identity;
- destination folder;
- template document copy or reset behavior;
- temporary public image hosting decision;
- cleanup expectation;
- exact command.

If Live E2E is not authorized, record it as not executed or explicitly N/A.
Do not substitute local verification for live readback evidence.

## 11. Security And Supply Chain

Before publication, record current security evidence for the selected package:

- package SHA-256;
- dependency restore and build source;
- package manifest verification result;
- antivirus scanner result when available;
- vendor classification or repository-owner exception decision when applicable;
- absence of credentials, token stores, local configuration, and secret-like
  content in the package.

Release remains blocked if a required scanner is disabled, inconclusive, or
reports an unresolved detection and no repository-owner exception decision has
been recorded.

## 12. Go / No-Go

Prepare a go/no-go record after verification and before publication.

The go decision requires:

- local verification PASS;
- selected package verification PASS;
- Live E2E PASS or owner-approved N/A;
- security and supply-chain review PASS or owner-approved exception basis;
- release notes prepared;
- target tag and target commit recorded;
- repository-owner release approval recorded.

If any required condition is missing, record `NO-GO` or `DEFERRED`. Do not
publish.

## 13. Publication

Publication requires explicit authorization after go approval.

Record before publication:

- tag name;
- target commit;
- GitHub Release title;
- draft state;
- prerelease state;
- latest-release state;
- asset path and SHA-256.

After publication, record:

- release URL;
- tag target;
- uploaded asset name;
- uploaded asset size;
- uploaded asset SHA-256;
- whether the release was draft, prerelease, and latest.

Do not retarget tags, replace uploaded assets, or modify a published release
without separate explicit authorization.

## 14. Post-Release Record

After publication or a deferred decision, update only the approved release
record files for the task.

Record:

- final status;
- executed commands;
- unexecuted operations;
- warnings and errors;
- artifact identity;
- release URL when applicable;
- remaining blockers;
- Git branch and working-tree state;
- stage, commit, and push status.

Do not record credential contents, token contents, private keys, private URLs,
or local secret-bearing paths.

## 15. Stop Conditions

Stop and report if any of these occur:

- Frozen specifications would need to change;
- public APIs, persisted schemas, canonical formats, or production defaults
  would need to change;
- package artifact identity is inconsistent;
- release notes do not match the selected package;
- package verification fails;
- tests fail without an approved remediation step;
- Live E2E requires credentials or mutation without explicit authorization;
- cleanup of temporary external resources fails;
- antivirus handling remains unresolved without an owner exception decision;
- the requested Git operation is not explicitly authorized;
- unrelated user changes make release evidence ambiguous.

## 16. Completion Checklist

| Item | Result | Evidence |
| --- | --- | --- |
| Preflight recorded | PENDING |  |
| Local verification complete | PENDING |  |
| Package creation authorized and complete, or explicitly not performed | PENDING |  |
| Package verification complete | PENDING |  |
| Installation smoke complete, authorized N/A, or blocked | PENDING |  |
| Live E2E complete, authorized N/A, or blocked | PENDING |  |
| Security and supply-chain review complete | PENDING |  |
| Repository-owner go/no-go decision recorded | PENDING |  |
| Publication complete or explicitly not performed | PENDING |  |
| Post-release records updated | PENDING |  |
| Git state recorded | PENDING |  |
