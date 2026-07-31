# Publisher Phase 4-1 Design Notes

Status  : Planning
Scope   : Phase 4-1 local verification design, evidence rules, task breakdown, and Phase 4-2 transition gate
Depends : docs/development/Publisher_Phase4_Planning.md, docs/development/Publisher_Phase4_BacklogReview.md, docs/development/Publisher_Phase4_LocalVerificationPlan.md, docs/development/Publisher_Phase4_LocalVerificationChecklist.md, docs/distribution/ReleaseChecklist.md

This document defines the Phase 4-1 planning baseline for Publisher local
verification and follow-on implementation task slicing. It is documentation
only. It does not modify Frozen specifications, public APIs, persisted schemas,
canonical formats, production behavior, release tags, prerelease assets,
packages, Google Docs, Google Drive, signing, MSI, or installer behavior.

## 1. Purpose

Phase 4-1 converts the initial Phase 4 planning documents into an actionable
local verification and implementation planning baseline.

The purpose is to:

- define the standard local verification order for Phase 4 Publisher work;
- make evidence requirements and stop conditions explicit;
- break P0 through P2 work into reviewable implementation tasks;
- confirm the conditions required before Phase 4-2 starts;
- preserve the completed Phase 3-10 prerelease record without recreating,
  retargeting, replacing, or modifying published artifacts.

## 2. Scope

Phase 4-1 includes planning for local-only Publisher work that can be executed
without live external mutation or release-state changes.

Allowed scope:

- local verification order definition;
- evidence and reporting rules;
- safety stop conditions;
- P0, P1, and P2 implementation task decomposition;
- Phase 4-2 transition criteria;
- documentation under `docs/development/`.

Excluded scope:

- Frozen specification changes;
- public API changes;
- persisted schema changes;
- canonical format changes;
- production behavior changes;
- published prerelease, tag, package, or asset changes;
- package creation, replacement, or recreation;
- GitHub Release modification;
- Live Google Docs or Google Drive execution;
- token-store, credential, or temporary public hosting mutation;
- signing, MSI, installer, self-contained package, apphost trust posture, or
  automatic update adoption.

## 3. Standard Local Verification Order

Phase 4 work must use the following default verification order unless a
task-specific instruction narrows the required checks.

1. Preflight source state
   - `git branch --show-current`
   - `git status --short`
   - review changed files and confirm the change type.
2. Safety boundary check
   - confirm Live E2E is not enabled;
   - confirm no Google Docs or Google Drive mutation is planned;
   - confirm no package creation, replacement, or update is planned;
   - confirm no release, tag, publication, or announcement operation is
     planned;
   - confirm no published prerelease asset will be modified.
3. Focused verification
   - run the narrowest unit or integration filter that covers the changed
     behavior when source or test code changes.
4. Publisher unit tests
   - `dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore`
5. Non-live Publisher integration tests
   - `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore`
   - keep `VMF_PUBLISHER_GOOGLE_E2E` disabled unless explicitly authorized for
     that exact run.
6. Release build
   - `dotnet build VMF.Publisher.sln --configuration Release --no-restore`
7. Format verification
   - `dotnet format VMF.Publisher.sln --verify-no-changes --no-restore`
8. Diff whitespace verification
   - `git diff --check`
9. Final source state
   - `git status --short`
   - `git diff --stat`

For documentation-only changes, `git diff --check` plus documentation
consistency review is the minimum required verification. Build, tests, and
format checks may be reported as not executed when no source, project, test, or
generated behavior changed.

For packaging-script changes, package creation remains outside the default
verification order. Existing-package verification may be included only when the
task explicitly authorizes verifying an existing package and does not require
creating, replacing, or publishing a package.

## 4. Evidence Requirements

Every Phase 4 verification report must record:

- target branch;
- changed files;
- change type;
- exact commands executed;
- pass, fail, blocked, pending, not executed, or N/A result for each expected
  check;
- warning count when available;
- error count when available;
- passed, failed, and skipped test counts when available;
- whether `VMF_PUBLISHER_GOOGLE_E2E` was enabled;
- whether Google Docs or Google Drive were mutated;
- whether package creation, replacement, or update occurred;
- whether release tags, GitHub Releases, distribution publication, or release
  announcements were modified;
- whether published prerelease artifacts were changed;
- reason for every skipped, blocked, pending, not executed, or N/A check.

Evidence must be classified by boundary:

| Evidence Class | Allowed Meaning | Must Not Be Reported As |
| --- | --- | --- |
| Documentation review | Planning and operational consistency | Runtime behavior proof |
| Local build | Release-configuration source build | Release approval or package validation |
| Unit tests | Local behavior verification | Integration, live, or rendered Google Docs evidence |
| Non-live integration tests | Local or mock integration behavior | Google Docs API readback or Drive cleanup |
| Mock-backed checks | Fake, stub, in-memory, or snapshot provider behavior | Real provider verification |
| Dry-run checks | Non-mutating plan or diff behavior | Publish success |
| Existing-package verification | Static verification of an existing package | Package creation, publication, or antivirus clearance |
| Format and diff checks | Formatting and whitespace hygiene | Behavioral verification |

Evidence must not include credentials, OAuth tokens, private keys, token-store
content, secret-bearing configuration values, sensitive document URIs, or
private document content.

## 5. Stop Conditions

Stop and report before implementation or verification continues if any of the
following are required:

- modifying Frozen specifications;
- modifying public APIs;
- modifying persisted schemas;
- modifying canonical formats;
- changing production behavior or live-write defaults;
- creating, replacing, recreating, retargeting, or publishing a prerelease,
  tag, package, or release asset;
- modifying the existing GitHub prerelease;
- executing Live E2E without explicit per-run authorization;
- mutating Google Docs, Google Drive, token stores, credentials, temporary
  public hosting, or other live external resources;
- accessing or printing secret values;
- adding dependencies;
- adopting signing, MSI, installer, self-contained packaging, automatic update,
  or apphost trust posture changes;
- weakening, deleting, skipping, or relaxing tests to obtain a pass;
- overwriting unrelated user changes;
- staging, committing, pushing, merging, rebasing, resetting, stashing, or
  rewriting Git history without explicit authorization.

Stop and classify the check as an environment or precondition issue, not a
Publisher failure, when a `--no-restore` command cannot run because restored
packages are unavailable or NuGet access is blocked.

## 6. P0 Implementation Task Breakdown

P0 tasks are required before Phase 4-2 implementation begins.

| Task | Area | Scope | Acceptance Criteria | Default Verification |
| --- | --- | --- | --- | --- |
| P0-1 | Verification sequence | Adopt this Phase 4-1 standard order as the default for Phase 4 task reports. | Future task reports cite the order or record an explicit task-specific exception. | Documentation review, `git diff --check`. |
| P0-2 | Evidence boundary | Keep local, live, release, package, and candidate evidence separated. | Reports do not describe local checks as Live E2E, Google Docs readback, release approval, publication, or antivirus clearance. | Documentation review, `git diff --check`. |
| P0-3 | Release-state preservation | Preserve the Phase 3-10 prerelease, tag, package, and release asset state. | No package recreation, tag retargeting, GitHub Release modification, or release announcement is performed. | `git status --short`, documentation review. |
| P0-4 | Safety stops | Apply the stop conditions in this document before executing scoped work. | Blocked operations are reported explicitly with reasons. | Documentation review. |

## 7. P1 Implementation Task Breakdown

P1 tasks are normal Phase 4 local implementation candidates after P0 is
accepted.

| Task | Area | Scope | Acceptance Criteria | Default Verification |
| --- | --- | --- | --- | --- |
| P1-1 | Local verification reports | Produce or update local-only evidence records when Phase 4 implementation changes occur. | Evidence records include command results, counts, skipped checks, live boundary state, package state, and Git state. | Focused test when applicable, unit tests, non-live integration tests, build, format, `git diff --check`. |
| P1-2 | Mock-backed regression coverage | Add or refine tests for Publisher behavior that can be validated without live Google services. | Tests use fake, stub, in-memory, snapshot, or mock-backed providers and do not require credentials. | Focused test, unit or non-live integration project. |
| P1-3 | CLI non-mutating behavior | Maintain local `verify`, `diff`, and `dry-run` behavior without changing production publish semantics. | Behavior remains deterministic and dry-run does not mutate Google Docs or Drive. | Focused CLI/unit/integration coverage; no Live E2E unless separately authorized. |
| P1-4 | Documentation consistency | Keep Phase 4 planning and local evidence documents aligned with completed Phase 3-10 release records. | Documents distinguish prerelease completion from later production announcement or release-state changes. | Documentation review, `git diff --check`. |

## 8. P2 Implementation Task Breakdown

P2 tasks are optional hardening or candidate-preparation work. They must remain
local-only unless separately authorized.

| Task | Area | Scope | Acceptance Criteria | Default Verification |
| --- | --- | --- | --- | --- |
| P2-1 | Package verification maintenance | Review existing-package verification clarity without creating or replacing packages. | Any change preserves manifest, hash, path-safety, configuration exclusion, and secret-check boundaries. | Focused script tests if available, build when project files change, `git diff --check`. |
| P2-2 | Candidate outlines | Prepare candidate-only notes for signing, installer, apphost, package trust posture, or release process options. | Candidate records do not adopt implementation or alter current prerelease artifacts. | Documentation review, `git diff --check`. |
| P2-3 | Live E2E readiness checklist | Refine preauthorization questions and evidence fields for future Live E2E runs. | Checklist requires explicit destination, identity, cleanup, temporary hosting, and command scope before execution. | Documentation review, `git diff --check`. |
| P2-4 | Reporting ergonomics | Improve local evidence templates without changing quality gates or verification semantics. | Templates remain compatible with existing Phase 4 reporting fields. | Documentation review, `git diff --check`. |

## 9. Phase 4-2 Transition Conditions

Phase 4-2 may begin only when all of the following are true:

- this Phase 4-1 design note is reviewed and accepted as the local planning
  baseline;
- P0 tasks have an agreed owner decision or are recorded as complete;
- Phase 4-2 target task is classified as local-only, vNext candidate, or
  blocked external dependency before implementation starts;
- the exact Phase 4-2 change area and allowed files are identified;
- required verification commands for the Phase 4-2 change type are selected
  from the standard order;
- Live E2E is either explicitly out of scope or separately authorized with an
  exact command and cleanup boundary;
- no published prerelease, tag, package, GitHub Release, or release asset
  modification is required;
- Frozen specifications, public APIs, persisted schemas, canonical formats,
  and production behavior remain unchanged;
- signing, MSI, installer, self-contained packaging, automatic update, apphost
  trust posture, and production release-process changes remain outside Phase
  4-2 implementation unless first recorded and approved as candidates;
- stage, commit, and push remain separate explicit actions.

If any transition condition is not satisfied, Phase 4-2 must not start. Record
the blocking condition and the next required repository-owner decision.

## 10. Done Criteria

Phase 4-1 is complete when:

- the standard local verification order is documented;
- evidence requirements and stop conditions are explicit;
- P0 through P2 implementation tasks are decomposed;
- Phase 4-2 transition conditions are documented;
- documentation changes are limited to `docs/development/`;
- Frozen specifications, public APIs, persisted schemas, canonical formats,
  production behavior, published prerelease state, tags, packages, and release
  assets are unchanged;
- no Live Google Docs or Drive operation is performed;
- no signing, MSI, or installer work is adopted;
- `git diff --check` passes;
- changes remain unstaged and reviewable unless a later explicit Git operation
  is authorized.
