# Publisher P2-13 Candidate Selection

Status  : Selected / next scoped task candidate
Scope   : Compare remaining P2 candidates after P2-12 closeout and select the next vNext enhancement candidate
Depends : docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_P2-01_GooglePickerDriveFileEvaluation.md, docs/development/Publisher_P2-01_OAuthDesktopScopeBoundaryEvaluation.md, docs/development/Publisher_P2-02_AdditionalDiagnosticsEvaluation.md, docs/development/Publisher_P2-03_ClearerDryRunOutputEvaluation.md, docs/development/Publisher_P2-04_ReleaseNoteGenerationEvaluation.md, docs/development/Publisher_P2-06_ManagedDocumentReadbackReportingEvaluation.md, docs/development/Publisher_P2-08_CandidateSelection.md

This is a docs-only / local-only candidate-selection record. It does not
implement P2-13, change dry-run semantics, change stdout, change public APIs,
change persisted schemas, inspect or mutate token stores, run Live E2E, call
Google Docs or Google Drive APIs, update packages or `dist`, perform release,
tag, publication, Avast, vendor, or flagged executable operations, decide
vendor clearance, stage, commit, or push.

## 1. Purpose

P2-13 selects the next Publisher vNext enhancement candidate after P2-12
verification evidence extractor closeout. The selection compares remaining
deferred P2 work, existing evaluation records, and current status, then
identifies one next scoped task without treating that selection as
implementation approval.

## 2. Candidate Inventory

| Candidate | Current state | Remaining work | Gate / risk |
| --- | --- | --- | --- |
| P2-01 Google Picker plus `drive.file` split route | Design complete / implementation decision pending | Future split-route design before any implementation | High: OAuth scopes, token-store lifecycle, selected-resource semantics, Drive creation / folder behavior, temporary image hosting, and Live E2E proof. |
| P2-02 additional diagnostics deferred items | Complete for A/B; P2-09 completed C; P2-10 completed the safe subset of D; E and remaining D fields deferred | `maxAttempts`, `deliveryState`, `httpStatus`, or `SUPPORT_SUMMARY` only under a new scope | Medium to high: safe metadata may be useful, but delivery and support-summary wording can imply external operation state if not tightly bounded. |
| P2-03 dry-run deferred items | Complete for A/B; C/D/E deferred | Structured dry-run contract, failure boundary hints, or physical-update dry-run bridge only under a new scope | Medium: failure boundary hints can reuse existing classifications; structured contract and physical bridge need broader compatibility review. |
| P2-04 release-note generation deferred items | Complete for A/B; P2-08 completed D; P2-12 completed C; E deferred | CHANGELOG draft helper | Medium: local-only possible, but changelog wording remains editorial and manually curated. |
| P2-06 managed-document readback reporting | Design complete; P2-07 implementation complete | No immediate follow-on required from the existing design | Low immediate value: the selected narrow implementation is already complete. |

## 3. Selection Criteria

The next candidate should:

- remain local-only and avoid external mutation;
- avoid release, package, tag, publication, Avast, vendor, and flagged
  executable operations;
- preserve Frozen specifications, public APIs, persisted schemas, OAuth scope,
  authentication architecture, stdout compatibility, exit-code behavior, and
  release records;
- reduce operator ambiguity left after completed P2-08, P2-09, P2-10, and
  P2-12 diagnostics / release-note work;
- use existing classifications or allow-listed values instead of introducing a
  broad new taxonomy.

## 4. Comparison Result

P2-01 still has the largest future least-privilege upside, but it remains a
design task rather than the best immediate implementation candidate. The
existing records require a split-route design and later proof before any OAuth
Desktop `drive.file` adoption.

P2-02 remaining D fields and E support summary could improve diagnostics, but
`deliveryState`, `httpStatus`, and a new `SUPPORT_SUMMARY` event need a stricter
boundary review to avoid implying external delivery, publication, release, or
vendor-clearance state. P2-10 already completed the lowest-risk retry subset.

P2-04-E is intentionally lower priority because `CHANGELOG.md` remains manually
curated. A draft helper should wait until the release-note extractor and drift
checker are proven across future release records.

P2-03-D is the best next candidate. Failure boundary hints can be bounded to
existing CLI classifications and already reviewed safe diagnostic fields. It
can improve dry-run failure comprehension without changing dry-run planning
semantics, stdout, exit codes, Google Docs / Drive behavior, OAuth, package,
release, or vendor-clearance gates.

## 5. Selected Candidate

Selected next candidate: P2-13, derived from P2-03-D Failure Boundary Hints.

Recommended next scoped task:

- design and, only if separately authorized, implement local-only dry-run
  failure boundary hints for failure summaries;
- derive values only from existing CLI classifications, stable error-code
  routing, and already reviewed safe diagnostic context;
- emit only allow-listed boundary labels such as `usage`, `configuration`,
  `input`, `compile`, `planning`, `cancellation`, `internal`, or `unknown`;
- omit the field when the boundary cannot be determined safely;
- preserve the boundary that dry-run output is local planning evidence only,
  not Google verification, Live E2E evidence, publication authorization,
  release clearance, vendor clearance, or Avast safety certification.

## 6. Non-Goals

P2-13 selection does not authorize:

- implementing failure boundary hints;
- changing dry-run semantics, stdout, exit codes, CLI classification, stable
  error codes, Physical Update Plan behavior, Verified State semantics, or
  readback requirements;
- adding a structured dry-run output contract beyond the selected failure
  boundary field;
- introducing the P2-03-E physical update dry-run bridge;
- release, tag, publication, package, `dist`, GitHub asset, Live E2E, Google
  Docs / Drive, OAuth/token-store, Avast, vendor, or flagged executable
  operations;
- changing Frozen specifications, public APIs, persisted schemas, OAuth
  scopes, authentication architecture, production design, release identity, or
  publication state.

## 7. Future GO / NO-GO

Future implementation GO is limited to a separate local-only task if it:

- reuses existing CLI classification and stable error-code routing;
- emits only bounded allow-listed boundary labels;
- excludes raw argument values, raw paths, document content, document IDs,
  private URLs, credentials, token-store paths, OAuth tokens, Authorization
  headers, provider payloads, raw HTTP bodies, raw exception details, stacks,
  usernames, hostnames, and account identifiers;
- preserves stdout compatibility, exit-code behavior, dry-run semantics,
  Physical Update Plan meaning, Verified State meaning, and readback
  requirements;
- includes focused tests for usage, configuration, input, compile/planning,
  cancellation, internal/unknown, omission on unsafe inputs, and sensitive
  value exclusion.

NO-GO if the task requires a new dry-run command contract, broad taxonomy
redesign, public API changes, persisted schema changes, output that could be
read as Google mutation or release evidence, external state lookup, Live E2E,
Google / OAuth operations, package or `dist` mutation, release publication,
vendor-clearance judgment, new dependencies, or weakening existing failure
behavior.

## 8. Local Verification Plan

For this selection record:

```powershell
git diff -- docs/development/Publisher_P2-13_CandidateSelection.md docs/development/Publisher_vNext_Backlog.md
git diff --check
git status --short --branch
```

No build, unit test, integration test, Live E2E, package verification, OAuth,
Google Docs / Drive, Avast, release, tag, publication, stage, commit, or push
operation is required or authorized for this docs-only selection.
