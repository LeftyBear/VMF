# Publisher Phase 4 Local Verification Checklist

Status  : Planning
Scope   : Local-only Publisher Phase 4 verification checklist and reporting template
Depends : docs/development/Publisher_Phase4_Planning.md, docs/development/Publisher_Phase4_BacklogReview.md, docs/development/Publisher_Phase4_LocalVerificationPlan.md, docs/distribution/ReleaseChecklist.md, docs/distribution/LiveE2EOperations.md

This document provides the checklist and report format for Phase 4 local-only
verification. It is documentation only. It does not authorize implementation,
release approval, tag creation, distribution publication, package creation or
update, Live E2E, Google Docs or Google Drive mutation, production default
changes, public API changes, or Frozen specification changes.

## 1. Purpose

Use this checklist to execute and report Phase 4 verification that is strictly
local-only.

The purpose is to:

- keep local evidence separate from release gate evidence;
- prevent local checks from being reported as Live E2E, Google Docs readback,
  Google Drive cleanup, antivirus vendor clearance, or release approval;
- provide a repeatable report template for build, tests, dry-run, package
  verification, formatting, whitespace, and documentation consistency review;
- record skipped or blocked checks explicitly instead of treating them as
  implicit success.

## 2. Scope

This checklist applies to Phase 4 Publisher work that can be verified without:

- live external service mutation;
- package creation or package update;
- release gate changes;
- tag creation;
- distribution publication;
- re-running a flagged executable while Avast classification remains pending,
  unless the repository owner explicitly approves that specific run.

The checklist may be used for:

- planning documentation changes;
- local Publisher source or test changes;
- non-live integration behavior;
- mock-backed verification;
- dry-run behavior;
- verification of an existing package only when the task scope allows it;
- local evidence reports that must remain separate from release operations.

## 3. Non-Goals

This checklist does not:

- approve or reject a release;
- close the Avast pending external dependency;
- create, rebuild, replace, or update distribution packages;
- create release tags;
- publish distribution artifacts;
- execute credentialed Live E2E;
- mutate Google Docs, Google Drive, token stores, temporary public hosting, or
  other live external resources;
- change production defaults;
- change public APIs, persisted schemas, canonical formats, or Frozen
  specifications;
- adopt package trust, signing, installer, or release-process changes.

## 4. Preconditions

Before running local-only verification, record:

- target branch;
- working-tree status;
- changed files under review;
- whether the task is documentation-only, source/test work, integration work,
  packaging-script work, or existing-package verification;
- whether Live E2E is explicitly authorized for this task.

Local-only verification may proceed only when:

- `VMF_PUBLISHER_GOOGLE_E2E` is not enabled;
- no Google Docs or Google Drive mutation is planned;
- no package creation or package update is planned;
- no release, tag, publication, or release announcement operation is planned;
- Frozen specifications, public APIs, production defaults, and existing package
  artifacts remain unchanged unless separately authorized;
- any executable run is confirmed not to re-run the flagged executable during
  Avast pending status, or that exact run has explicit approval.

If a precondition cannot be confirmed, mark the affected check `BLOCKED` and
record the reason.

## 5. Allowed Local-Only Checks

### Build

Allowed when source, test, or project-file changes require it:

```powershell
dotnet build VMF.Publisher.sln --configuration Release --no-restore
```

Record the command, result, warning count, and error count.

### Unit Tests

Allowed for Publisher behavior covered by local unit tests:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
```

Record passed, failed, skipped, warning count when available, and error count.

### Non-Live Integration Tests

Allowed when Live E2E is not enabled:

```powershell
dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore
```

Record passed, failed, skipped, and confirm `VMF_PUBLISHER_GOOGLE_E2E` was not
enabled.

### Mock-Backed Tests

Allowed when tests use fake, stub, in-memory, snapshot, or mock-backed
providers and do not mutate live external services.

Record the test project or filter, result counts, and the mock or fake provider
boundary.

### Dry-Run Checks

Allowed only when the command does not mutate Google Docs, Google Drive, token
stores, temporary public hosting, or package artifacts.

Record the command, exit code, dry-run result, and summary of the local plan or
diff. Do not report dry-run success as publish success or rendered document
verification.

### Existing-Package Verification Only

Allowed only against an existing local package when the task scope includes
package verification:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip
```

Record package path, required file checks, manifest path safety, file sizes,
SHA-256 checks, unmanifested file checks, configuration exclusion, and
secret-like filename or content checks.

This check must not create, replace, update, or publish a package.

### Format And Whitespace Checks

Allowed after source, test, or documentation changes:

```powershell
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
```

For documentation-only changes, `git diff --check` is sufficient unless the
task requires code formatting verification.

### Documentation Consistency Review

Allowed for Phase 4 planning and operations documentation.

Review that local-only reports:

- cite local checks as local evidence only;
- do not imply release approval;
- do not imply Live E2E execution;
- do not imply Google Docs API readback or rendered placement verification;
- do not change Frozen specifications, public APIs, production defaults, or
  release state.

## 6. Explicitly Blocked Checks

The following are blocked during local-only Phase 4 verification unless
separately and explicitly authorized for that exact operation:

- release approval or release rejection;
- release tag creation;
- distribution publication;
- release announcement;
- package creation;
- package replacement;
- package update;
- Live E2E;
- setting `VMF_PUBLISHER_GOOGLE_E2E=1`;
- Google Docs mutation;
- Google Drive mutation;
- token-store mutation;
- temporary public image hosting;
- cleanup of live external resources created outside the approved task scope;
- flagged executable re-run during Avast pending status without explicit
  approval;
- any operation that changes Frozen specifications, public APIs, persisted
  schemas, canonical formats, production defaults, package trust posture,
  signing model, installer model, or release process.

Blocked checks must be reported as `BLOCKED`, `PENDING`, `N/A`, or `Not
executed`, with the reason. They must not be omitted.

## 7. Evidence Rules

Every report must distinguish these evidence classes:

| Evidence Class | Allowed Meaning | Must Not Be Used As |
| --- | --- | --- |
| Local build | Source and project build check | Release approval, Live E2E, package validation |
| Unit tests | Local behavior verification | Integration or live service evidence |
| Non-live integration tests | Local or mock integration behavior | Google Docs readback or Drive cleanup |
| Mock-backed tests | Behavior against fake or stub providers | Real provider verification |
| Dry-run | Non-mutating plan or diff verification | Publish success |
| Existing-package verification | Static verification of an existing package | Package creation, publication, antivirus clearance |
| Format and whitespace | Formatting or diff hygiene | Behavioral verification |
| Documentation review | Consistency of recorded boundaries | Runtime verification |

Reports must include:

- exact command executed;
- pass, fail, blocked, pending, N/A, or not-executed result;
- warning count when available;
- error count when available;
- passed, failed, and skipped test counts when available;
- whether Live E2E was enabled;
- whether Google Docs or Google Drive were mutated;
- whether package creation or update occurred;
- whether release, tag, publication, or announcement operations occurred;
- whether the flagged executable was run while Avast classification remained
  pending;
- reason for each skipped, blocked, pending, or N/A check.

Do not record credentials, OAuth tokens, private keys, token-store content,
secret-bearing configuration values, or private document content.

## 8. Reporting Template

Use this template for Phase 4 local-only verification reports.

```markdown
## Phase 4 Local-Only Verification Report

### Scope

- Target branch:
- Change type:
- Changed files:
- Local-only boundary confirmed: Yes/No
- Live E2E authorized: No
- Release/tag/publication authorized: No
- Package creation/update authorized: No

### Preconditions

| Item | Result | Evidence / Notes |
| --- | --- | --- |
| Working tree reviewed |  |  |
| Live E2E disabled |  |  |
| Google Docs / Drive mutation not planned |  |  |
| Package creation / update not planned |  |  |
| Release / tag / publication not planned |  |  |
| Flagged executable re-run avoided or explicitly approved |  |  |

### Verification Results

| Check | Result | Command / Evidence | Warnings | Errors | Notes |
| --- | --- | --- | --- | --- | --- |
| Build |  |  |  |  |  |
| Unit tests |  |  |  |  |  |
| Non-live integration tests |  |  |  |  |  |
| Mock-backed tests |  |  |  |  |  |
| Dry-run checks |  |  |  |  |  |
| Existing-package verification only |  |  |  |  |  |
| Format check |  |  |  |  |  |
| Whitespace check |  |  |  |  |  |
| Documentation consistency review |  |  |  |  |  |

### Explicitly Not Executed

| Operation | Result | Reason |
| --- | --- | --- |
| Release approval / rejection | Not executed | Local-only verification scope. |
| Tag creation | Not executed | Local-only verification scope. |
| Distribution publication | Not executed | Local-only verification scope. |
| Package creation / update | Not executed | Local-only verification scope. |
| Live E2E | Not executed | No explicit authorization for this report. |
| Google Docs / Drive mutation | Not executed | Local-only verification scope. |
| Flagged executable re-run during Avast pending | Not executed | Explicit approval not recorded. |

### Evidence Classification

- Local evidence collected:
- Live evidence collected: None
- Release gate evidence changed: No
- Package artifact changed: No
- Frozen spec / public API / production default changed: No

### Git State

- Branch:
- Staged changes:
- Commit performed: No
- Push performed: No
- Working-tree status:

### Result

- Done criteria satisfied:
- Remaining issues:
- Next required decision or action:
```

## 9. Done Criteria

Phase 4 local-only verification reporting is done when:

- all checks in scope are executed and reported, or explicitly marked skipped,
  blocked, pending, N/A, or not executed with reasons;
- local evidence is not described as release approval, Live E2E, Google Docs
  readback, Google Drive cleanup, or antivirus vendor clearance;
- Live E2E remains disabled unless explicitly authorized for that specific run;
- Google Docs, Google Drive, token stores, temporary public hosting, and other
  live external resources are not mutated;
- no release, tag, distribution publication, or release announcement operation
  is performed;
- no package is created, replaced, or updated;
- existing-package verification, if run, is reported as static local evidence
  only;
- flagged executable re-run during Avast pending status is avoided unless
  explicitly authorized;
- Frozen specifications, public APIs, persisted schemas, canonical formats, and
  production defaults remain unchanged;
- `git diff --check` passes;
- stage, commit, and push are not performed;
- remaining issues and required approvals are recorded.
