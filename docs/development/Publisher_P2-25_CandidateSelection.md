# Publisher P2-25 Candidate Selection

Status  : Selected / next scoped task candidate
Scope   : Re-inventory remaining P2 candidates after P2-24 closeout and select the next vNext enhancement candidate
Depends : docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_P2-22_HttpStatusCliExposureEvaluation.md, docs/development/Publisher_P2-23_PhysicalUpdateDryRunSeparateCommandEvaluation.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only candidate-selection record. It does not
implement P2-25, add `httpStatus` to CLI output, change structured diagnostics,
change retry behavior, inspect or mutate token stores, run Live E2E, call
Google Docs or Google Drive APIs, update packages or `dist`, perform release,
tag, publication, Avast, vendor, or flagged executable operations, decide
vendor clearance, stage, commit, or push.

## 1. Purpose

P2-25 selects the next Publisher vNext enhancement candidate after P2-24
`deliveryState` final failure summary diagnostics closeout. The selection
compares remaining P2 work, current backlog state, and existing evaluation
records, then identifies one next scoped task without treating that selection
as implementation approval.

## 2. Candidate Inventory

| Candidate | Current state | Remaining work | Gate / risk |
| --- | --- | --- | --- |
| P2-01 Google Picker plus `drive.file` split route | Design re-evaluation complete / split-route design GO / implementation NO-GO | Future scoped split-route design before any implementation | High: OAuth scopes, token-store lifecycle, selected-resource semantics, Drive creation / folder behavior, temporary image hosting, and Live E2E proof. |
| P2-02 additional diagnostics deferred item | Complete except `httpStatus` | Bounded `httpStatus` final failure summary diagnostics only under a new scope | Medium: useful troubleshooting value, but must prove a sanitized final-summary carrier and avoid provider payload or classification leakage. |
| P2-03-E physical update dry-run | Separate-command evaluation complete / implementation NO-GO until separately authorized | Future separate command, contract, evidence category, and authorization boundary | High: depends on Verified State, current snapshot acquisition, revision preconditions, and possibly Google / OAuth read authorization. |
| P2-04 release-note generation follow-ons | A, B, C, D, and E implemented through P2-08, P2-12, and P2-17 | No immediate P2-04 follow-on required from the current backlog | Low immediate value: the local draft, drift, evidence extraction, and CHANGELOG helper paths are already complete. |
| P2-06 / P2-07 readback reporting | Design and narrow implementation complete | No immediate follow-on required from the existing design | Low immediate value: the selected readback reporting scope is already complete. |

## 3. Selection Criteria

The next candidate should:

- remain local-only and avoid external mutation;
- avoid release, package, tag, publication, Avast, vendor, and flagged
  executable operations;
- preserve Frozen specifications, public APIs, persisted schemas, OAuth scope,
  authentication architecture, stdout compatibility, exit-code behavior,
  retry behavior, and release records;
- reduce operator ambiguity left after completed retry, support summary,
  readback, and delivery-state diagnostics;
- require a smaller implementation surface than a new command or OAuth /
  Google-backed workflow.

## 4. Comparison Result

P2-01 still has future security value, but it remains a design-track item. It
is not the best immediate local implementation candidate because scope changes
would touch OAuth and Google Drive selection semantics.

P2-23 is also valuable, but the evaluation records a separate command,
separate evidence category, and possible current-snapshot acquisition gate.
That makes it larger than the next small local diagnostics task.

P2-17 and the P2-04 release-note follow-ons are already complete in the
current backlog and current-status records. Any stale planning wording that
lists P2-17 as design-ready should not be used to reopen duplicate work.

P2-22 left a narrow remaining diagnostics opportunity: `httpStatus` may be
acceptable if it is emitted only on final failure summaries, only when already
safely available as a sanitized value at the CLI boundary, and without exposing
provider details or changing classification. After P2-24, the final-summary
and `SUPPORT_SUMMARY` pattern for a bounded transport-adjacent value has recent
focused coverage, while `httpStatus` remains explicitly omitted.

## 5. Selected Candidate

Selected next candidate: P2-25, derived from P2-22 HTTP Status CLI Exposure
Evaluation.

Recommended next scoped task:

- design and, only if separately authorized, implement bounded `httpStatus`
  final failure summary diagnostics;
- emit `httpStatus` only when an HTTP status is already safely carried to the
  CLI final-summary boundary;
- use only a sanitized integer status code, unless the implementation task
  separately adopts a bounded marker and tests its omission / marker behavior;
- update `SUPPORT_SUMMARY` only with the same final-failure-only value, if
  included there at all;
- preserve the current omission behavior for success summaries, unrelated
  failures, and failures without a safely known status.

## 6. Non-Goals

P2-25 selection does not authorize:

- implementing `httpStatus`;
- exposing provider payloads, response bodies, Google API reasons, OAuth
  details, raw exceptions, stack traces, URLs, document IDs, credentials,
  tokens, token-store paths, account identifiers, or document content;
- changing retryability, retry budget behavior, classification, exit codes,
  stdout, command syntax, safe messages, readback reporting, delivery-state
  semantics, public APIs, persisted schemas, OAuth scopes, authentication
  architecture, production design, release identity, or publication state;
- adding a new transport diagnostics block or combining `httpStatus` with
  `deliveryState` outside the existing final failure summary boundary;
- release, tag, publication, package, `dist`, GitHub asset, Live E2E, Google
  Docs / Drive, OAuth/token-store, Avast, vendor, or flagged executable
  operations.

## 7. Future GO / NO-GO

Future implementation GO is limited to a separate local-only task if it:

- proves the status is already available at the final-summary boundary without
  parsing raw messages or provider payloads;
- emits only final-failure-only, sanitized, classification-neutral values;
- preserves current omission behavior where status is unknown or unavailable;
- keeps `SUPPORT_SUMMARY` consistent with the outer final failure summary;
- includes focused `CliApplicationTests` coverage for known status emission,
  absent-status omission, `SUPPORT_SUMMARY` behavior, coexistence with
  `deliveryState`, success omission, unrelated failure omission, and sensitive
  value exclusion.

NO-GO if the task requires broad exception plumbing, raw HTTP body or provider
payload access, message parsing, new dependencies, public API changes,
persisted schema changes, retry-policy changes, classification changes,
external state lookup, Live E2E, Google / OAuth operations, package or `dist`
mutation, release publication, vendor-clearance judgment, or weakening
existing failure behavior.

## 8. Local Verification Plan

For this selection record:

```powershell
git diff -- docs/development/Publisher_P2-25_CandidateSelection.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

No build, unit test, integration test, Live E2E, package verification, OAuth,
Google Docs / Drive, Avast, release, tag, publication, stage, commit, or push
operation is required or authorized for this docs-only selection.
