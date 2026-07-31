# Publisher Phase 3-10 Release Gate Checklist

Status  : BLOCKED / PENDING
Scope   : Post Phase 3-10 release gate approval checklist
Depends : docs/development/Publisher_Phase3-9_ReleaseApprovalPackage.md, docs/distribution/ReleaseChecklist.md, docs/releases/Publisher_Phase3-10_ReleaseNotes.md

This checklist controls the handoff from the Phase 3-9 release approval package
to any later release, tag, or publication action. It is local-only
documentation. It does not approve a release, create tags, publish artifacts,
create or update packages, execute Live E2E, mutate Google Docs or Google
Drive, re-run the Avast-flagged executable, change production behavior, change
public APIs, or modify Frozen specifications.

## 1. Gate Status

| Item | Status | Evidence |
| --- | --- | --- |
| Current release approval state | BLOCKED / PENDING | `docs/distribution/ReleaseChecklist.md` and `docs/development/Publisher_Phase3-9_ReleaseApprovalPackage.md` |
| Blocking condition | PENDING | Avast false positive classification pending |
| Release approval package baseline | RECORDED | Commit `22359dd` |
| Release execution | NOT AUTHORIZED | Requires explicit repository-owner approval after the blocker is resolved or accepted |

The Phase 3-9 release readiness evidence remains recorded, but the package must
not be treated as production-approved while the Avast false positive
classification remains pending unless the repository owner explicitly accepts
the antivirus exception posture as the release approval basis.

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
| Local-only verification | PASS |
| Working tree | Clean |
| `origin/main` | Synchronized with local `main` |
| Phase 3-9 release approval package | Reviewed |
| Avast blocker | Resolved or explicitly accepted by the repository owner |
| Release approval | Explicitly recorded by the repository owner |
| Frozen specifications | Unchanged |
| Production behavior | Unchanged unless separately approved as release scope |

If any precondition is not confirmed, stop before release execution.

## 4. Blocked Actions Before Approval

The following actions remain blocked until the required approvals and
preconditions are recorded:

- release execution;
- tag creation;
- publication;
- package creation or update;
- Live E2E;
- Google Docs or Google Drive mutation;
- release announcement;
- re-running the Avast-flagged executable.

Local-only documentation, consistency review, Markdown cleanup, and read-only
Git diff inspection may continue while the gate is blocked.

## 5. Allowed Actions After Approval

After explicit approval, the following actions may proceed only within the
separately authorized release scope:

- release execution;
- release tag creation;
- distribution artifact publication;
- Live E2E, if explicitly enabled for that run;
- release notes final confirmation;
- post-release verification and decision record update.

Each action still requires the normal release safeguards. Approval to release
does not automatically authorize Live E2E, Google Docs or Drive mutation, or a
package rebuild unless those actions are explicitly included.

## 6. Avast Resolution Condition

The Avast pending blocker can be closed only by recording one of these outcomes:

- Avast confirms the package or executable is not malicious, and the repository
  owner approves release continuation;
- Avast continues to classify the executable as unsafe, and the repository
  owner rejects release continuation or requires remediation;
- the repository owner explicitly accepts the antivirus exception posture
  without waiting further and records that decision as the release approval
  basis.

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

This checklist creation does not:

- approve the Phase 3-9 package release;
- resolve the Avast false positive classification;
- create a release tag;
- publish distribution artifacts;
- create, rebuild, replace, or update a package;
- execute Live E2E;
- mutate Google Docs or Google Drive;
- use or modify credentials, token stores, or live external resources;
- re-run the Avast-flagged executable;
- change Frozen specifications;
- change public APIs, persisted schemas, canonical formats, or production
  behavior;
- stage, commit, push, merge, rebase, reset, stash, or rewrite Git history.

