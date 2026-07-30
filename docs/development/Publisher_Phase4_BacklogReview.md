# Publisher Phase 4 Backlog Review

Status  : Planning
Scope   : Publisher Phase 3 carry-over classification
Depends : docs/releases/Publisher_Phase3-8_ReleaseNotes.md, docs/releases/Publisher_Phase3-9_ReleaseNotes.md, docs/releases/Publisher_Phase3-10_ReleaseNotes.md, docs/distribution/ReleaseChecklist.md

This document classifies Publisher carry-over work after Phase 3-8, Phase 3-9,
and Phase 3-10. It is planning documentation only. It does not approve a
release, create tags, publish artifacts, execute Live E2E, mutate Google Docs
or Google Drive, change production defaults, change public APIs, or modify
Frozen specifications.

## 1. Classification Model

Phase 4 backlog items use these classifications:

| Classification | Meaning |
| --- | --- |
| Local-only | May be planned or implemented without live external mutation, release approval, tag creation, or artifact publication. |
| vNext candidate | Requires a candidate record before adoption because it may change trust posture, release process, installer model, public contract, production behavior, or future architecture. |
| Blocked external dependency | Cannot proceed until an external response, repository-owner decision, or explicit per-run authorization is recorded. |

## 2. Phase 3-8 Carry-Over

Phase 3-8 completed Publisher productization operations including structured
JSON diagnostics, publish summary fields, CLI commands, exit-code mapping,
configuration validation, timeout settings, and retry documentation.

| Item | Classification | Rationale | Phase 4 Handling |
| --- | --- | --- | --- |
| Local verification of CLI command behavior | Local-only | Build, unit tests, integration tests, and dry-run checks can run without live Google updates. | Include in the local verification plan. |
| Configuration and timeout behavior regression checks | Local-only | Configuration validation and timeout parsing can be verified with local tests and mock integrations. | Use focused tests before broad verification when implementation changes are made. |
| Live Google Docs end-to-end verification | Blocked external dependency | Live E2E requires explicit authorization, credentials, destination folder, and cleanup expectations. | Do not run during Phase 4 planning. |
| Changes to production live-write defaults | vNext candidate | Changing defaults could alter live external service behavior. | Exclude from Phase 4 implementation unless separately adopted. |

## 3. Phase 3-9 Carry-Over

Phase 3-9 completed ZIP packaging, package manifest verification, installation
and upgrade documentation, release checklist structure, and Live E2E operations
documentation.

| Item | Classification | Rationale | Phase 4 Handling |
| --- | --- | --- | --- |
| Package verification procedure maintenance | Local-only | Manifest, hash, file inventory, unsafe path, and secret-like content checks are local. | May be refined through tests or documentation if behavior remains compatible. |
| Installation and upgrade documentation review | Local-only | Documentation review can proceed without publishing artifacts or mutating external services. | Keep changes non-normative and scoped to operations docs. |
| Local smoke and dry-run evidence model | Local-only | Local smoke checks do not require Google Docs or Drive updates. | Clarify evidence boundaries in the local verification plan. |
| MSI installer | vNext candidate | Installer adoption changes the distribution model. | Record as candidate before implementation. |
| Code signing or Authenticode trust posture | vNext candidate | Signing changes package trust posture and release process. | Record as candidate before adoption. |
| Self-contained runtime package | vNext candidate | Distribution model changes from framework-dependent ZIP. | Record as candidate before adoption. |
| Automatic update mechanism | vNext candidate | Introduces new release and operational behavior. | Record as candidate before implementation. |
| Temporary public image hosting operational default changes | vNext candidate | Changes live external exposure behavior. | Exclude from Phase 4 implementation. |
| Live E2E execution | Blocked external dependency | Requires explicit authorization and cleanup expectations. | Do not execute without approval. |

## 4. Phase 3-10 Carry-Over

Phase 3-10 finalized the release gate state without approving the release.
Avast false positive submission response remains pending as an external
release approval dependency.

| Item | Classification | Rationale | Phase 4 Handling |
| --- | --- | --- | --- |
| Avast false positive response tracking | Blocked external dependency | Release approval depends on vendor response or repository-owner risk decision. | Keep pending in the release gate; do not close in planning. |
| Repository-owner final release decision | Blocked external dependency | Final approval or rejection is an owner decision after Avast response or explicit exception acceptance. | Do not infer approval. |
| Release tag creation | Blocked external dependency | Tagging is a release operation and remains out of scope. | Do not perform. |
| Distribution publication | Blocked external dependency | Publication requires release approval. | Do not perform. |
| Release announcement | Blocked external dependency | Announcement implies production release approval. | Do not perform. |
| Planning documents for Phase 4 | Local-only | Planning can proceed without release approval or external mutation. | Create under `docs/development/`. |
| Candidate separation for trust and release process changes | vNext candidate | Trust posture and release process changes must not be adopted directly through Phase 4. | Keep outside implementation scope until candidate review. |

## 5. Initial Phase 4 Backlog

| Priority | Work Item | Classification | Acceptance Condition |
| --- | --- | --- | --- |
| A | Document Phase 4 scope and release gate boundary | Local-only | Phase 4 planning records state that release approval remains blocked by the Avast external dependency. |
| A | Document local verification plan | Local-only | Local checks are listed separately from Live E2E and live readback. |
| A | Review Phase 3 carried-over operational items | Local-only | Items are classified into local-only, vNext candidate, or blocked external dependency. |
| B | Evaluate package verification documentation gaps | Local-only | Any proposed documentation update remains non-normative and does not change package artifacts. |
| B | Prepare candidate outline for signing or installer strategy | vNext candidate | Candidate is recorded before implementation or release process adoption. |
| B | Prepare candidate outline for package trust posture alternatives | vNext candidate | Candidate distinguishes signed apphost, no-apphost distribution, installer, and owner exception posture. |
| C | Execute Live E2E | Blocked external dependency | Requires explicit per-run authorization and cleanup plan. |
| C | Resume release approval | Blocked external dependency | Requires Avast response or explicit repository-owner acceptance of the antivirus exception posture. |

## 6. Excluded From Phase 4 Implementation

The following items are not Phase 4 implementation work unless separately
authorized and, where required, recorded as candidates first:

- code signing;
- MSI installer;
- self-contained package;
- automatic update mechanism;
- apphost distribution model change;
- production release process change;
- release approval;
- release tag creation;
- artifact publication;
- release announcement;
- Live E2E execution;
- Google Docs or Google Drive mutation.

