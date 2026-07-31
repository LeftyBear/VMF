# Publisher Phase 3-10 Release Gate Checklist

Status  : RELEASE EXECUTION EVIDENCE RECORDED
Scope   : Post Phase 3-10 release gate approval checklist
Depends : docs/development/Publisher_Phase3-9_ReleaseApprovalPackage.md, docs/distribution/ReleaseChecklist.md, docs/releases/Publisher_Phase3-10_ReleaseNotes.md

This checklist controls the handoff from the Phase 3-9 release approval package
to any later release, tag, or publication action. It is local-only
documentation. It records the approved Phase 3-10 release execution evidence.
It does not create tags, publish artifacts, announce a release, change
production behavior, change public APIs, or modify Frozen specifications.

## 1. Gate Status

| Item | Status | Evidence |
| --- | --- | --- |
| Current release approval state | EVIDENCE RECORDED | `docs/distribution/ReleaseChecklist.md` and repository-owner Phase 3-10 execution instruction |
| Blocking condition | RESOLVED FOR THIS RUN | Repository owner instructed Codex to treat the Avast blocker as resolved on 2026-07-31 |
| Release approval package baseline | RECORDED | Commit `22359dd` |
| Release execution | AUTHORIZED / EXECUTED LOCALLY | Release checks, Live E2E, package creation, package verification, and evidence updates executed on 2026-07-31 |

The Phase 3-9 release readiness evidence remains recorded. The Phase 3-10 run
adds current build, test, Live E2E, package, and artifact-hash evidence without
creating a tag or publishing a GitHub Release.

## 2. Required Approvals

Before any release, tag, or publication action, record both of these approvals:

- repository owner approval for release continuation;
- Avast resolution or explicit repository-owner acceptance of the antivirus
  exception posture.

Approval of this document alone is not release approval.

## 3. Preconditions

All preconditions must be confirmed immediately before release execution:

| Precondition | Required state |
| --- | --- |
| Local and Live E2E verification | PASS |
| Working tree before execution | Clean |
| `origin/main` | Synchronized with local `main` at `59116070f3258e4eb88201195994fb7267fdb9bb` |
| Phase 3-9 release approval package | Reviewed |
| Avast blocker | Resolved for this run by repository-owner instruction |
| Release approval | Explicitly recorded by repository-owner Phase 3-10 execution instruction |
| Frozen specifications | Unchanged |
| Production behavior | Unchanged unless separately approved as release scope |

If any precondition is not confirmed, stop before release execution.

## 4. Actions Still Blocked Without Separate Approval

The following actions remain blocked unless separately authorized:

- tag creation;
- publication;
- release announcement;
- production behavior changes;
- public API, schema, or canonical-format changes;
- Frozen specification changes.

Package creation and Google Live E2E were authorized only for the Phase 3-10
release execution evidence run.

## 5. Allowed Actions After Approval

The following actions were executed within the approved Phase 3-10 release
scope:

- `git fetch origin main` and `main` / `origin/main` synchronization check;
- `dotnet clean VMF.Publisher.sln --configuration Release`;
- `dotnet build VMF.Publisher.sln --configuration Release`;
- `dotnet test VMF.Publisher.sln --configuration Release`;
- Google Live E2E with `VMF_PUBLISHER_GOOGLE_E2E=1` for the four
  `GoogleDocsEndToEndIntegrationTests` cases;
- package creation for `0.0.0-dev`;
- package verification and artifact hash recording;
- release notes, changelog, and checklist evidence updates.

No tag, publication, GitHub Release, release announcement, Frozen-specification
change, public API change, schema change, canonical-format change, or
production-behavior change was executed.

## 6. Avast Resolution Condition

The Avast pending blocker was closed for this run by this recorded outcome:

- the repository owner instructed Codex to treat the Avast blocker as resolved
  for Phase 3-10 Release Execution on 2026-07-31.

VirusTotal no-detection, an Avast exception, local package verification, or a
false-positive submission alone does not close this blocker.

## 7. Release Execution Handoff

### 7.1 Execution Order After Approval

1. Record the repository-owner release decision and the Avast resolution or
   accepted exception posture.
2. Confirm the working tree is clean.
3. Confirm local `main` and `origin/main` are synchronized.
4. Reconfirm the Phase 3-9 release approval package and
   `docs/distribution/ReleaseChecklist.md`.
5. Confirm the exact package artifact and hashes match the recorded release
   evidence.
6. Confirm release notes are final for the selected artifact.
7. Execute the authorized release operation.
8. Create the release tag only after release execution is authorized and the
   pre-tag checks still pass.
9. Publish distribution artifacts only after release and tag authorization.
10. Run Live E2E only if explicitly enabled and scoped for that run.
11. Record post-release verification and final release notes confirmation.

Phase 3-10 executed steps 1 through 7 and step 10 for the explicitly scoped
Live E2E run. Tag creation, publication, and announcement were not executed.

### 7.2 Pre-Execution Confirmation

Before the first release command, verify:

- the release decision is explicit and current;
- the package artifact is the artifact approved for release;
- no package rebuild or replacement is being introduced implicitly;
- no Live E2E, Google Docs, or Google Drive mutation is required unless
  explicitly authorized;
- no Frozen specification or production behavior change is required.

### 7.3 Post-Execution Confirmation

After execution, record:

- release command outcome;
- tag name and target commit, if tag creation was authorized;
- publication target and artifact path, if publication was authorized;
- Live E2E result, if explicitly enabled;
- any deviation from the approved release scope;
- final working-tree and origin synchronization status.

### 7.4 Phase 3-10 Execution Evidence

| Item | Result | Evidence |
| --- | --- | --- |
| Source synchronization | PASS | `main` and `origin/main` both at `59116070f3258e4eb88201195994fb7267fdb9bb`; ahead/behind `0 0` |
| Avast blocker | PASS | Repository-owner instruction: treat Avast blocker as resolved |
| Clean | PASS | `dotnet clean VMF.Publisher.sln --configuration Release`; 0 warnings, 0 errors |
| Release build | PASS | `dotnet build VMF.Publisher.sln --configuration Release`; 0 warnings, 0 errors |
| Release tests | PASS | `dotnet test VMF.Publisher.sln --configuration Release`; integration 16/16 and unit 461/461 passed |
| Google Live E2E | PASS | 4/4 `GoogleDocsEndToEndIntegrationTests` passed: Success, Revision Conflict, Readback Mismatch, Empty Plan |
| Package creation | PASS | `dist\release\Publisher\vmf-publisher-0.0.0-dev-win-x64.zip`; size 973097 bytes |
| Package verification | PASS | `tools\publisher\verify-package.ps1`; package verification passed |
| Artifact SHA-256 | PASS | `404F6D4B382132802CEF5F42A00A6B53E7C7177E3ABFC56C3DD518DE435C7742` |

## 8. Rollback And Stop Conditions

Stop before proceeding, or halt the release handoff if already in progress, when
any of these conditions appears:

| Condition | Required response |
| --- | --- |
| Release approval is not explicit | Stop; request repository-owner decision. |
| Avast resolution or accepted exception posture is not recorded | Stop; keep the gate BLOCKED / PENDING. |
| Working tree is dirty | Stop; resolve or review changes before release execution. |
| `origin/main` diverges from local `main` | Stop; reconcile by an explicitly authorized Git workflow. |
| Local verification fails | Stop; record failure and do not release. |
| Package artifact or hash mismatches the approved evidence | Stop; do not publish or tag. |
| Release notes do not match the selected artifact | Stop; correct under explicit documentation scope before release. |
| Live E2E becomes required but is not explicitly authorized | Stop; request authorization or mark N/A by owner decision. |
| Unexpected Google Docs or Google Drive mutation is required | Stop; request explicit external-service authorization. |
| Frozen specification or production behavior change is required | Stop; separate the change from the release handoff. |
| Avast-flagged executable must be re-run while pending | Stop unless explicit authorization is recorded. |

## 9. Explicit Non-Actions For This Checklist

This Phase 3-10 evidence update does not:

- create a release tag;
- publish distribution artifacts;
- announce a release;
- publish a GitHub Release;
- mutate Google Docs or Google Drive beyond the explicitly authorized Live E2E
  test scope;
- use or modify credentials, token stores, or live external resources;
- change Frozen specifications;
- change public APIs, persisted schemas, canonical formats, or production
  behavior;
- stage, commit, push, merge, rebase, reset, stash, or rewrite Git history.
