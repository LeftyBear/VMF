# Publisher Phase 4 Local Verification Plan

Status  : Planning
Scope   : Local-only Publisher verification boundaries
Depends : VMF.Publisher.sln, docs/distribution/LiveE2EOperations.md, docs/distribution/ReleaseChecklist.md

This document defines local-only verification for Publisher Phase 4 work.
It does not authorize Live E2E, Google Docs or Google Drive mutation, release
approval, tag creation, distribution publication, or production default changes.

## 1. Verification Principle

Phase 4 verification must distinguish local evidence from live external
evidence.

Local checks may verify build correctness, unit behavior, mock integration
behavior, package structure, package integrity, local CLI behavior, dry-run
planning, and static formatting.

Local checks must not be reported as:

- Google Docs rendered output verification;
- Google Docs API readback;
- Google Drive cleanup verification;
- live publish success;
- release approval;
- antivirus vendor clearance.

## 2. Build Verification

Use the Publisher solution for release configuration build checks:

```powershell
dotnet build VMF.Publisher.sln --configuration Release --no-restore
```

Expected evidence:

- command executed;
- exit result;
- warning count;
- error count.

Boundary:

- this check does not publish a package;
- this check does not approve a release;
- this check does not prove Live E2E behavior.

## 3. Unit Tests

Run Publisher unit tests when implementation changes affect Publisher behavior:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
```

Expected evidence:

- passed count;
- failed count;
- skipped count;
- warning count when reported;
- error count.

Boundary:

- unit tests must not use live Google credentials;
- unit tests must not mutate Google Docs, Google Drive, token stores, or
  temporary public hosting.

## 4. Integration Tests

Run non-live Publisher integration tests when implementation changes affect the
publish lifecycle, package-independent pipeline behavior, or mock integration
contracts:

```powershell
dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore
```

Expected evidence:

- passed count;
- failed count;
- skipped count;
- Live E2E enablement state.

Boundary:

- `VMF_PUBLISHER_GOOGLE_E2E` must not be enabled without explicit approval;
- local integration tests must be reported as local or mock integration
  evidence only;
- skipped or disabled Live E2E must not be described as Google Docs readback.

## 5. Mock And Dry-Run Verification

Use mock-backed integration tests, CLI `verify`, CLI `diff`, and CLI `dry-run`
for non-mutating verification.

Examples from an extracted, approved local test package:

```powershell
vmf-publisher.exe verify
vmf-publisher.exe verify .\sample.md
vmf-publisher.exe diff .\sample.md
vmf-publisher.exe dry-run .\sample.md
```

Expected evidence:

- command executed;
- exit code;
- result classification;
- local publish plan or diff summary when applicable.

Boundary:

- dry-run does not create or update Google Docs;
- dry-run does not verify rendered document placement;
- dry-run does not verify Drive file cleanup;
- dry-run does not approve release publication.

## 6. Package Verification

Use package verification only against an existing local package when explicitly
included in the task scope:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip
```

Expected evidence:

- package path;
- required file check result;
- manifest path safety result;
- size and SHA-256 verification result;
- unmanifested file check result;
- configuration exclusion result;
- secret-like filename and content check result.

Boundary:

- package verification does not create a release;
- package verification does not publish artifacts;
- package verification does not override antivirus classification;
- package verification does not close the Avast pending external dependency.

## 7. Format And Diff Checks

Use formatting and whitespace checks after implementation or documentation
changes:

```powershell
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
```

Expected evidence:

- command executed;
- exit result;
- files requiring formatting, if any;
- whitespace errors, if any.

Boundary:

- format verification must not be used to justify unrelated formatting churn;
- diff checks do not replace focused tests.

## 8. Live E2E Boundary

Live E2E is not part of default Phase 4 verification.

Live E2E requires explicit approval for the specific run, including:

- Google account or service identity;
- destination folder;
- template copy or reset behavior;
- temporary public image hosting permission;
- cleanup expectation;
- exact command.

Do not set:

```powershell
$env:VMF_PUBLISHER_GOOGLE_E2E = "1"
```

unless the run is explicitly authorized.

Live E2E evidence must remain separate from local verification evidence.
Local build, unit tests, integration tests, package verification, and dry-run
checks do not prove Google Docs rendering, Google Docs API readback, or Google
Drive cleanup.

## 9. Minimum Verification By Change Type

| Change Type | Minimum Local Verification | Notes |
| --- | --- | --- |
| Planning documentation only | `git diff --check` | Build and tests are not required unless documentation references generated behavior that needs validation. |
| Publisher source behavior | Focused unit tests, then Publisher unit tests | Add integration tests when lifecycle or pipeline behavior changes. |
| Publisher integration behavior | Focused integration filter, then integration project | Keep Live E2E disabled unless explicitly authorized. |
| Packaging scripts | Package creation only if explicitly authorized, then package verification | Do not publish artifacts or treat the result as release approval. |
| Release documentation | `git diff --check` and release evidence consistency review | Do not change release approval state without owner decision. |
| Candidate planning | `git diff --check` | Candidate documentation must not modify Frozen specifications. |

## 10. Reporting Requirements

Every Phase 4 verification report must state:

- commands executed;
- pass or failure result;
- warning count when available;
- error count when available;
- skipped or unexecuted checks;
- whether Live E2E was enabled;
- whether Google Docs or Google Drive were mutated;
- whether release tags, distribution publication, or release announcements were
  performed.

If Live E2E was not authorized, report it as not executed. Do not substitute
local verification for live readback.

