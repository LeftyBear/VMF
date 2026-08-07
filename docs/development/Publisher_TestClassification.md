# Publisher Existing Test Classification

Status  : Done
Scope   : Existing Publisher test classification and Avast-response resume procedure
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_Phase4_LocalVerificationPlan.md, docs/development/Test_Traceability_Matrix.md, docs/distribution/PublisherReleaseRunbook.md

This document classifies existing Publisher verification targets after the
release runbook work. It is documentation only. It does not approve a release,
create or update packages, create tags, publish artifacts, execute Live E2E,
mutate Google Docs or Google Drive, re-run flagged artifacts, change production
design, change public APIs, or modify Frozen specifications.

`Test_Traceability_Matrix.md` is the companion traceability index for mapping
Publisher requirements and ADRs to implementation, test, operational
verification, evidence, and current coverage status.

## 1. Current Boundary

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Until it is resolved, verification
may use read-only investigation, source build, unit tests, mock-backed
integration tests, local dry-run paths that do not re-run flagged artifacts, and
static documentation checks. Verification must not cross into release, package
mutation, live Google operations, publication, or flagged executable re-run.

## 2. Test Classification

| Test Category | Example Command / Target | Scope | External Mutation | Avast Pending Execution | Resume Phase |
| --- | --- | --- | --- | --- | --- |
| Documentation consistency | `git diff --check` | Markdown whitespace and patch hygiene for documentation-only changes. | No | Allowed | Current local-only work |
| Release build | `dotnet build VMF.Publisher.sln --configuration Release --no-restore` | Source compilation for Publisher projects in Release configuration. | No | Allowed when no package or flagged executable is run | Source verification |
| Publisher unit tests | `dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore` | Parser, compiler, diff, planning, CLI boundary, Google request mapping, state, and infrastructure behavior through local or fake dependencies. | No | Allowed | Source verification |
| Focused unit tests | `dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~CliApplicationTests"` | Narrow source-level regression checks for the affected area. | No | Allowed | Source verification |
| Non-live integration tests | `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore` with `VMF_PUBLISHER_GOOGLE_E2E` unset | Publish pipeline, verified state lifecycle, physical update lifecycle, image pipeline, and transaction behavior using local or in-memory collaborators. | No | Allowed only with Live E2E disabled | Source verification |
| Live Google Docs E2E tests | `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~GoogleDocsEndToEndIntegrationTests"` with `VMF_PUBLISHER_GOOGLE_E2E=1` | Credentialed Google Docs / Drive create, copy, batchUpdate, readback, conflict, mismatch, and cleanup behavior. | Yes | Blocked | Live E2E gate |
| CLI local commands through project output | `dotnet run --project src\Publisher.Cli -- verify <markdown-file>` or equivalent non-packaged build output | Local configuration validation, Markdown compilation, diff, and dry-run planning. | No for `verify`, `diff`, and `dry-run`; `publish` is excluded | Allowed only when the command does not use a flagged package executable and does not publish | Local CLI verification |
| Packaged executable smoke | `vmf-publisher.exe --help`, `vmf-publisher.exe verify`, `vmf-publisher.exe dry-run <markdown-file>` from an extracted package | Installation and package executable behavior for the selected artifact. | No by command intent, but executes flagged artifact | Blocked unless repository owner explicitly authorizes that exact run | Installation smoke |
| Existing-package static verification | `powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip` | Package manifest, paths, hashes, required files, secret-like filenames, and secret-like content for an existing artifact. | No | Allowed only when explicitly in scope and no package is created or executable is run | Artifact audit |
| Package creation or update | `powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\package-publisher.ps1 -Version <version>` | Builds or updates release distribution artifacts. | Writes `dist` artifacts | Blocked | Package gate |
| Release publication checks | Git tag, GitHub Release, release asset upload, announcement, or publication command | Release distribution and public repository state. | Yes | Blocked | Publication gate |

## 3. Allowed Policy During Avast Pending

Allowed work during Avast pending is limited to:

- read-only investigation;
- documentation updates;
- `git status`, `git diff`, and `git diff --check`;
- existing test-name and category inspection;
- source build when it does not create or update packages;
- Publisher unit tests;
- non-live integration tests with `VMF_PUBLISHER_GOOGLE_E2E` disabled;
- mock-backed verification;
- dry-run classification that does not re-run flagged package executables;
- static existing-package inspection only when explicitly in scope.

Allowed results must be reported as local, non-live, mock-backed, dry-run, or
static evidence. They must not be promoted to release readiness, Google Docs
readback, Google Drive cleanup, package approval, publication approval, or
antivirus vendor clearance.

## 4. Blocked Policy During Avast Pending

The following remain blocked:

- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- package creation or package update;
- release tag creation;
- GitHub Release creation or update;
- artifact publication;
- release announcement;
- re-running the Avast-pending flagged executable;
- treating VirusTotal no-detection, a local exception, or a false-positive
  submission as vendor clearance.

Authorization for one blocked operation does not authorize any other operation.
For example, approval to run Live E2E would not approve package creation,
publication, or flagged executable smoke testing.

## 5. Resume Order After Avast Response

When an Avast response is received, resume in this order:

1. Record the exact Avast response, date, affected artifact identity, and
   interpretation in the approved release record or security review document.
2. If Avast confirms the detection, stop release work and decide whether to
   remediate, rebuild, repackage, or abandon the candidate under a new task.
3. If Avast clears the artifact, verify that the cleared artifact identity
   matches the selected package path and SHA-256 before any executable smoke
   run.
4. Reopen only the necessary release gate with explicit repository-owner
   authorization.
5. Run local source verification first: build, focused tests as needed, unit
   tests, non-live integration tests with Live E2E disabled, format if source
   changed, and `git diff --check`.
6. Select or generate a release candidate artifact only under explicit package
   authorization. If a package is created or updated, restart artifact
   verification for the new path and SHA-256.
7. Run package static verification for the selected artifact.
8. Run packaged executable smoke only if the artifact is cleared or an explicit
   owner exception authorizes that exact executable run.
9. Run Live E2E only after separate per-run authorization records the account
   or service identity, destination folder, template decision, temporary public
   image hosting decision, cleanup expectation, and exact command.
10. Complete security and supply-chain review for the selected artifact.
11. Prepare go/no-go review and record repository-owner approval or rejection.
12. Publish only after a separate publication authorization records tag, target
   commit, release state, asset path, and SHA-256.

## 6. Preflight Hard Stops

`Publisher_PreflightHardening.md` is the local-only hard-stop reference while
Avast false-positive handling remains pending. In addition to the stop
conditions below, do not proceed if a proposed command would move from local
verification into package, release, Live E2E, Google Docs or Google Drive
mutation, publication, or flagged executable execution without separate
operation-specific authorization.

## 7. Stop Conditions

Stop and report if any of these occur:

- the Avast response does not match the selected artifact identity;
- the artifact path, version, or SHA-256 is ambiguous;
- a command would create or update `dist` without explicit package
  authorization;
- a command would execute the flagged package before Avast clearance or an
  explicit owner exception;
- `VMF_PUBLISHER_GOOGLE_E2E` would be enabled without per-run authorization;
- any Google Docs or Google Drive mutation would occur without explicit
  operation-specific authorization;
- a local check fails and no remediation task has been approved;
- release approval, tag creation, publication, or push is requested without
  explicit authorization;
- Frozen specifications, public APIs, persisted schemas, canonical formats, or
  production defaults would need to change.

## 8. Reporting Requirements

Every resumed verification report must state:

- exact commands executed;
- test project, filter, or target;
- pass, fail, and skipped counts when available;
- warning and error counts when reported;
- whether `VMF_PUBLISHER_GOOGLE_E2E` was enabled;
- whether Google Docs or Google Drive were mutated;
- whether a package was created or updated;
- whether a flagged executable was run;
- whether release, tag, publication, or push was performed.

Use `PASS` only for directly executed and directly verified evidence. Keep
`PENDING`, `BLOCKED`, `NOT EXECUTED`, and `DEFERRED` when the evidence has not
been produced.
