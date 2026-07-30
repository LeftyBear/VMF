# Publisher Phase 4 Planning

Status  : Planning
Scope   : Non-destructive Publisher Phase 4 planning
Depends : docs/distribution/ReleaseChecklist.md, docs/releases/Publisher_Phase3-10_ReleaseNotes.md, docs/development/Publisher_Phase4_BacklogReview.md, docs/development/Publisher_Phase4_LocalVerificationPlan.md

This document records the initial Phase 4 planning scope for VMF Publisher.
It is planning documentation only. It does not approve a release, create a tag,
publish distribution artifacts, execute Live E2E, mutate Google Docs or Google
Drive, change production defaults, change public APIs, or modify Frozen
specifications.

## 1. Purpose

Phase 4 organizes the next non-release Publisher work after Phase 3-10 release
gate finalization.

The purpose of Phase 4 is to:

- classify remaining Publisher work after Phase 3-8, Phase 3-9, and Phase 3-10;
- identify work that can proceed without depending on the Avast false positive
  response;
- define local-only verification expectations;
- separate vNext candidate items from Phase 4 implementation scope;
- preserve the Phase 3-9 release gate state while planning continues.

Phase 4 starts from the recorded release gate state. It does not reinterpret the
Phase 3-9 package as approved.

## 2. Entry Conditions

Phase 4 planning may proceed only while all of the following remain true:

- the Phase 3-9 package release approval remains blocked or pending until the
  Avast classification response is recorded or the repository owner explicitly
  accepts the antivirus exception posture;
- release tags are not created;
- distribution artifacts are not published;
- release announcements are not made;
- Live E2E is not executed without explicit approval for that specific run;
- Google Docs, Google Drive, token stores, temporary public hosting, and other
  external resources are not mutated;
- Frozen specifications, public APIs, persisted schemas, canonical formats, and
  production defaults remain unchanged;
- package trust posture, signing model, installer model, and production release
  process changes are recorded as vNext candidates before adoption.

## 3. Non-Goals

Phase 4 planning does not include:

- approving the Phase 3-9 package for release;
- closing the Avast pending risk without the required repository-owner decision;
- creating release tags;
- publishing ZIP packages or other distribution artifacts;
- announcing production availability;
- executing credentialed Live E2E;
- mutating Google Docs or Google Drive;
- enabling temporary public image hosting;
- changing production defaults;
- changing public APIs or persisted state schemas;
- modifying Frozen specifications;
- adopting code signing, MSI installers, self-contained packages, automatic
  updates, or apphost trust changes as Phase 4 implementation work.

## 4. Relationship To Release Gate

The Phase 3-10 release gate remains authoritative for the Phase 3-9 package.
Phase 4 planning is separate from release approval.

The Avast false positive submission response remains an external release gate
dependency. Phase 4 may continue only as non-release work while that dependency
is pending.

Release approval can resume only through the closure criteria recorded in
`docs/distribution/ReleaseChecklist.md`:

- Avast confirms the package or executable is not malicious and the repository
  owner approves release continuation;
- Avast continues to classify the executable as unsafe and the repository owner
  rejects release continuation or requires remediation;
- the repository owner explicitly accepts the antivirus exception posture
  without waiting further and records that decision as the release approval
  basis.

None of the Phase 4 planning documents may be used as release approval
evidence for the Phase 3-9 package.

## 5. Risk Register

| Risk | Classification | Impact | Phase 4 Handling |
| --- | --- | --- | --- |
| Avast classification remains pending | External dependency | Phase 3-9 package release approval remains blocked | Keep release gate pending; continue only non-release work. |
| Local checks are mistaken for Live E2E | Verification risk | Google Docs rendering or Drive cleanup may be overstated | Require local verification to be reported as local-only evidence. |
| Signing or installer work is pulled into implementation scope | Scope risk | Package trust posture or release process could change without adoption | Record as vNext candidate before implementation. |
| Release artifacts are changed during planning | Release integrity risk | Existing release evidence may no longer match artifacts | Do not rebuild, replace, publish, or mutate release artifacts without explicit approval. |
| Frozen specifications or public APIs are changed by planning follow-up | Governance risk | VMF v1.0 or compatibility boundary could be violated | Treat required changes as vNext candidates and exclude them from Phase 4 implementation. |
| Credentialed external operations are executed by assumption | External service risk | Real Google Docs, Drive, tokens, or temporary public files could be modified | Require explicit per-run authorization before Live E2E or live publish operations. |

## 6. Work Packages

### WP1: Backlog Classification

Classify carried-over Phase 3-8, Phase 3-9, and Phase 3-10 items into:

- local-only work;
- vNext candidate work;
- blocked external dependency work.

Output:

- `docs/development/Publisher_Phase4_BacklogReview.md`

Completion condition:

- each known carry-over item has a classification and rationale.

### WP2: Local Verification Plan

Define the local verification commands and evidence boundaries that can be used
without Live E2E or external mutations.

Output:

- `docs/development/Publisher_Phase4_LocalVerificationPlan.md`

Completion condition:

- build, unit tests, integration tests, mock or dry-run checks, package verify,
  and Live E2E boundaries are documented.

### WP3: Candidate Separation Review

Identify items that require candidate treatment before adoption.

Candidate examples:

- code signing;
- Authenticode trust posture;
- MSI installer;
- self-contained runtime package;
- automatic update mechanism;
- apphost distribution model changes;
- production release process changes.

Completion condition:

- candidate-required items remain outside Phase 4 implementation scope.

### WP4: Non-Destructive Documentation Maintenance

Maintain planning records that clarify scope and evidence without changing
Frozen specifications, public APIs, production defaults, or release artifacts.

Completion condition:

- documentation changes remain under `docs/development/` unless separately
  authorized.

## 7. Done Criteria

Phase 4 planning is complete when:

- Phase 4 purpose, entry conditions, non-goals, release gate relationship, risk
  register, work packages, and done criteria are documented;
- Phase 3 carry-over work is classified;
- local verification boundaries are documented;
- Avast pending response remains recorded as an external release gate
  dependency;
- release tags, distribution publication, release announcements, Live E2E, and
  Google Docs or Drive mutations have not been performed;
- Frozen specifications, public APIs, persisted schemas, canonical formats,
  production defaults, package artifacts, and release state remain unchanged;
- candidate-required changes are excluded from Phase 4 implementation scope.

