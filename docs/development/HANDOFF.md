# VMF Publisher Handoff

Status  : Phase 4 local-only verification complete / release blocked
Scope   : Handoff for next worker, next chat, Codex, or Work Mode
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_Phase4_LocalVerificationEvidence.md, docs/development/Publisher_Phase4_LocalVerificationChecklist.md, docs/distribution/ReleaseChecklist.md

This document is the handoff boundary for continuing VMF Publisher work after
Phase 4 local-only verification. It is intended to prevent accidental release
gate movement while Avast false positive handling remains pending.

## 1. Starting State

Begin from this state:

- Phase 4 local-only verification safety scope is complete;
- current state is local verification complete / release blocked;
- Avast false positive handling is pending;
- release, tag creation, publication, package mutation, Live E2E, Google Docs
  mutation, and Google Drive mutation were not performed;
- Frozen specifications, public APIs, and production design remain unchanged.

Local verification completion is not release readiness.

## 2. Stop Line

Until the release gate is explicitly reopened, do not perform:

- Release;
- Git tag creation;
- Publication;
- New package creation;
- Package update;
- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- Re-running flagged artifacts before Avast false positive handling is
  resolved;
- Frozen specification changes;
- Public API changes;
- Production design changes.

If a requested task requires any blocked operation, stop and report the exact
blocked operation and required authorization. Do not infer authorization from
Phase 4 local-only verification.

## 3. Allowed Work

The following work may continue when it remains non-release, non-live, and
non-mutating:

- Build;
- Unit tests;
- Non-live integration tests;
- Mock-based verification;
- Dry-run verification;
- Existing package inspection;
- Format check;
- Documentation consistency check.

Existing package inspection must remain inspection only. It must not create,
replace, update, publish, or approve a package, and it must not re-run a flagged
artifact while Avast false positive handling is unresolved.

## 4. Next Actions

Recommended next actions are:

1. Track Avast false positive resolution.
2. Keep Phase 3-9 release approval pending until the release gate is explicitly
   reopened.
3. If release is reconsidered, record the repository-owner decision before any
   tag, publication, package mutation, Live E2E, or flagged artifact re-run.
4. If Live E2E is requested, require explicit per-run authorization, credentials
   scope, destination scope, cleanup expectations, and exact command.
5. Keep vNext hardening work separate as candidate work before adoption.

The vNext hardening backlog currently includes:

- signing;
- MSI / installer;
- distribution verification;
- security / trust workflow.

## 5. Required Reporting For Future Work

Future local-only work should report:

- files changed;
- commands executed;
- pass, fail, blocked, pending, N/A, or not-executed result;
- warning count and error count when available;
- whether Live E2E was enabled;
- whether Google Docs or Google Drive were mutated;
- whether package creation or package update occurred;
- whether release, tag creation, publication, or announcement occurred;
- whether any flagged artifact was re-run;
- Git branch, staged state, commit state, push state, and working-tree status.

Blocked operations must be reported as not executed, blocked, or pending with a
reason. They must not be omitted.

## 6. Related Commits

| Commit | Meaning |
| --- | --- |
| `fa4d6a6` | Phase 3-9 evidence |
| `6103003` | Phase 4 docs |
| `15cf77d` | Backlog boundary |
| `71bc23f` | LocalVerify boundary |
| `cf77964` | Checklist |
| `e59a7ec` | Execution order |

## 7. Handoff Summary

Treat the repository as ready for local-only follow-up and blocked for release
follow-up.

Proceed only inside the allowed scope unless the repository owner explicitly
reopens the relevant release gate or grants operation-specific authorization.
