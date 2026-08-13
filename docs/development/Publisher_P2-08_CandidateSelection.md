# Publisher P2-08 Candidate Selection

Status  : Selected / next scoped task candidate
Scope   : Compare remaining unimplemented P2 candidates and select the next vNext enhancement candidate
Depends : docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_P2-01_GooglePickerDriveFileEvaluation.md, docs/development/Publisher_P2-01_OAuthDesktopScopeBoundaryEvaluation.md, docs/development/Publisher_P2-02_AdditionalDiagnosticsEvaluation.md, docs/development/Publisher_P2-03_ClearerDryRunOutputEvaluation.md, docs/development/Publisher_P2-04_ReleaseNoteGenerationEvaluation.md, docs/development/Publisher_P2-06_ManagedDocumentReadbackReportingEvaluation.md, docs/architecture/ADR-0010-vnext-backlog-and-deferred-scope-boundary.md

This is a docs-only / local-only candidate-selection record. It does not
implement P2-08, change OAuth scopes, inspect or mutate token stores, run Live
E2E, call Google Docs or Google Drive APIs, update packages or `dist`, create
or update release notes, perform release, tag, publication, Avast, or flagged
executable operations, decide vendor clearance, stage, commit, or push.

## 1. Purpose

P2-08 selects the next Publisher vNext enhancement candidate after P2-07
closeout. The selection compares existing backlog entries, evaluation records,
and current status, then identifies one next scoped task without treating that
selection as implementation approval.

## 2. Candidate Inventory

| Candidate | Current state | Remaining work | Gate / risk |
| --- | --- | --- | --- |
| P2-01 Google Picker plus `drive.file` split route | Design complete / implementation decision pending | Future split-route design before any implementation | High: OAuth scope, token-store lifecycle, operator resource selection, Drive semantics, temporary image hosting, and Live E2E proof. |
| P2-02 additional diagnostics deferred items | Complete for A/B; C/D/E deferred by design | Configuration category, retry/delivery metadata, or support summary only under a new scope | Medium: value-safe if bounded, but some items depend on CLI diagnostic boundary review. |
| P2-03 dry-run deferred items | Complete for A/B; C/D/E deferred by design | Structured dry-run contract, failure boundary hints, or physical-update dry-run bridge only under a new scope | Medium to high: output compatibility and physical-update scope need separate review. |
| P2-04 release-note generation deferred items | Complete for A/B; C/D/E deferred by design | Verification evidence extractor, drift checker, or changelog helper | Medium: local-only possible, but release-note semantics need strict current/historical source handling. |
| P2-06 managed-document readback reporting | Design complete; P2-07 implementation complete | No immediate follow-on required from the existing design | Low immediate value: the selected narrow implementation is already complete. |

## 3. Selection Criteria

The next candidate should:

- remain local-only and avoid external mutation;
- avoid release, package, tag, publication, Avast, and flagged-executable
  operations;
- preserve Frozen specifications, public APIs, persisted schemas, OAuth scope,
  authentication architecture, and release records;
- reduce operator or reviewer ambiguity that remains after completed P2-07
  readback reporting;
- have existing design evidence sufficient to start a later narrow scoped task
  without broad architecture redesign.

## 4. Comparison Result

P2-01 has the largest future security upside, but it is not the best immediate
next task. The existing evaluations state NO-GO for immediate implementation
because the work affects OAuth scopes, token-store lifecycle, Picker-selected
resource representation, Drive creation / folder behavior, temporary image
hosting, and Live E2E proof.

P2-06 does not need another immediate task because P2-07 already implemented
the recommended narrow local-only reporting scope.

P2-02 and P2-03 still contain deferred diagnostic and dry-run output items.
Those items are useful, but the highest-value deferred items either depend on
output compatibility review or overlap with already completed P2-07 readback
status reporting.

P2-04 has a remaining local-only candidate with clear value after release
completion: a release-note drift checker. It can compare existing release-note
records against allow-listed current-state and release identity sources, report
`MATCH`, `MISSING`, or `CONFLICT`, and avoid editing approved release notes.
It builds directly on the completed P2-04-A/B manifest and draft assembler
boundary while preserving manual approval for release, publication, risk, and
vendor-clearance decisions.

## 5. Selected Candidate

Selected next candidate: P2-08, derived from P2-04-D Release-Note Drift
Checker.

Recommended next scoped task:

- design and, only if separately authorized, implement a local-only
  release-note drift checker that reads the existing P2-04 source-field
  manifest and allow-listed repository Markdown records;
- compare approved release-note fields against explicit current-state and
  release identity records;
- report missing fields and conflicts without rewriting release notes;
- fail closed on ambiguous current/historical state, superseded identities, or
  non-inferable gate fields;
- preserve the boundary that drift checking is not release approval, release
  authorization, publication authorization, risk acceptance, vendor clearance,
  Avast safety certification, or evidence that a gated operation occurred.

## 6. Non-Goals

P2-08 selection does not authorize:

- implementing the drift checker;
- generating or editing approved release notes;
- editing `CHANGELOG.md`;
- release, tag, publication, package, `dist`, GitHub asset, Live E2E, Google
  Docs / Drive, OAuth/token-store, Avast, vendor, or flagged-executable
  operations;
- changing Frozen specifications, public APIs, persisted schemas, OAuth
  scopes, authentication architecture, production design, release identity, or
  publication state.

## 7. Future GO / NO-GO

Future implementation GO is limited to a separate local-only task if it:

- reuses the existing P2-04 allow-list / manifest boundary;
- reports only bounded drift status values and source references;
- treats missing or conflicting required fields as non-approval-ready output;
- preserves sensitive-value exclusion rules;
- includes focused fixture tests for matching fields, missing fields,
  conflicting current/historical identities, superseded identity handling,
  manual-only gate fields, and sensitive-value exclusion.

NO-GO if the task requires broad Markdown scraping, external state lookup,
release asset inspection beyond existing checked-in records, package or `dist`
mutation, release-note rewriting, vendor-clearance judgment, authorization
inference, new dependencies, public API changes, persisted schema changes, or
Live E2E / Google / OAuth operations.

## 8. Local Verification Plan

For this selection record:

```powershell
git diff -- docs/development/Publisher_P2-08_CandidateSelection.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

No build, unit test, integration test, Live E2E, package verification, OAuth,
Google Docs / Drive, Avast, release, tag, publication, stage, commit, or push
operation is required or authorized for this docs-only selection.
