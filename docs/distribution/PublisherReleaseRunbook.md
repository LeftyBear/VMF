# Publisher Release Runbook

Status  : Draft
Scope   : VMF Publisher release operation procedure
Depends : docs/distribution/ReleaseChecklist.md, docs/distribution/LiveE2EOperations.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_TestClassification.md

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
- `docs/development/Publisher_AvastResponseIntakeTemplate.md`;
- `docs/development/Publisher_ReleaseApprovalPackage.md`;
- `docs/development/Publisher_TestClassification.md`;
- `docs/distribution/ReleaseChecklist.md`;
- `docs/distribution/LiveE2EOperations.md`;
- the release-specific notes or readiness documents for the target version.

If the current status records the release gate as blocked or deferred, do not
proceed past local-only checks without explicit repository-owner authorization.

Current operating snapshot:

| Item | State |
| --- | --- |
| Formal state | Phase 4 local-only verification complete / Release Hold lifted by VMF risk acceptance |
| Release gate | Hold lifted by ADR-0019; release execution advanced through GitHub prerelease publication |
| Release identity | `0.0.1-dev` / `vmf-publisher-v0.0.1-dev`; annotated tag object `a962e19ba2b0a494d1158011ae823d579e41711f`; peeled/package target commit `f08eef306ba82e3ea7f031ef652666178f2f0acf`; evidence docs commit `39df8bedd848da42a4de3cb9461ce4cc86b51197` |
| Package identity | Fixed: `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |
| Package target commit | Fixed: `f08eef306ba82e3ea7f031ef652666178f2f0acf` |
| Avast false-positive handling | Vendor response pending; VMF risk acceptance recorded |
| Vendor clearance | Not obtained |
| Avast safety certification | Not claimed |
| Approval recommendation | Record release completion evidence; commit/push of this docs-only update remains pending separate authorization |
| Final verification / Live E2E / result review | PASS / Live E2E 4 of 4 PASS / result review complete |
| Package generation / verification | PASS / PASS; manifest files 14; secret/static package inspection PASS |
| Tag state | Pushed; remote tag readback PASS |
| GitHub Release / publication | Published prerelease `true`: https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev; release name `VMF Publisher 0.0.1-dev` |
| Release asset | `vmf-publisher-0.0.1-dev-win-x64.zip`; 983404 bytes; remote asset digest matched local verified package SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6` |

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

### 5.1 Post-Hold Hard Stops

After ADR-0019 VMF risk acceptance, also read
`docs/development/Publisher_PreflightHardening.md` before proceeding into
final verification or later release-path checks.

Stop before the next command if preflight would cross any of these boundaries
without separate operation-specific authorization:

- final verification;
- Live E2E or setting `VMF_PUBLISHER_GOOGLE_E2E=1`;
- result review;
- package creation, replacement, update, or any write under `dist/`;
- release tag creation, GitHub Release creation or update, artifact
  publication, or release announcement;
- Google Docs mutation, Google Drive mutation, token-store mutation, or
  temporary public image hosting;
- re-running a previously flagged executable;
- treating local-only evidence as release readiness, Live E2E readback,
  package approval, publication approval, or antivirus vendor clearance.

If any hard stop is reached, record the operation as `BLOCKED`, `PENDING`,
`NOT EXECUTED`, or `DEFERRED`. Do not continue toward the next release step
until ADR-0019 risk acceptance, selected artifact identity, and the
operation-specific repository-owner decision are recorded.

Use `docs/development/Publisher_AvastResponseIntakeTemplate.md` to record a
future Avast response safely. The template addition itself does not mean a
response was received, does not create vendor clearance, and does not change
ADR-0019 into Avast safety certification.

### 5.2 Workstream Separation After Hold Lift

Keep each workstream separate after Release Hold lift:

| Workstream | Current Handling |
| --- | --- |
| Allowed local-only work | Documentation updates, read-only investigation, source build, unit tests, non-live integration tests with Live E2E disabled, mock-backed verification, dry-run checks that do not publish or execute the flagged package, and static existing-package inspection when explicitly in scope. |
| Gated release/live/mutation work | Final verification, Live E2E, result review, package or `dist` writes, tagged release work, publication, Google Docs or Google Drive mutation, token-store mutation, temporary public image hosting, and flagged executable re-run require the fixed ADR-0019 order and separate authorization. |
| Avast-response intake work | Record only a received vendor response, artifact identity, SHA-256, classification, redaction review, and decision in `Publisher_AvastResponseIntakeTemplate.md`; vendor clearance remains not obtained until a future response explicitly supports it. |
| Vendor-clearance-dependent work | Do not treat vendor clearance as obtained. The release path relies on ADR-0019 VMF residual risk acceptance unless a future Avast response changes the vendor-clearance record. |
| Final release-resume work | Proceed in fixed order: final verification, Live E2E, result review, package/dist, tag/release. |

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

Release execution stops if a required scanner is disabled, inconclusive, or
reports an unresolved detection and no repository-owner risk acceptance or
exception decision has been recorded.

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
