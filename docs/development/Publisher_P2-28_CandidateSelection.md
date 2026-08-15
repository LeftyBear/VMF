# Publisher P2-28 Candidate Selection

Status  : Selected / next scoped task boundary fixed
Scope   : Re-inventory remaining P2 candidates after P2-27 closeout and select one next vNext enhancement candidate
Depends : docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md, docs/development/Publisher_P2-27_GooglePickerDriveFileSplitRouteDesign.md, docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only candidate-selection record. It does not
implement P2-28, add a command, change CLI output, change OAuth scopes, adopt
Google Picker or `drive.file`, inspect or mutate token stores, call Google Docs
or Google Drive APIs, run Live E2E, update packages or `dist`, perform release,
tag, publication, Avast, vendor, or flagged executable operations, decide
vendor clearance, stage, commit, or push.

## 1. Purpose

P2-28 selects the next Publisher vNext enhancement candidate after P2-27
Google Picker / `drive.file` split-route design closeout. The selection uses
the latest repository backlog as the baseline, compares the remaining viable
P2 routes, selects one candidate, and fixes the P2-28 scope, non-scope, and
verification boundary without treating selection as implementation approval.

## 2. Candidate Inventory

| Candidate | Current state | Remaining work | Gate / risk |
| --- | --- | --- | --- |
| P2-01 / P2-27 Route B selected-resource workflow | Split-route design complete / implementation NO-GO | Adoption record before any Google Picker, `drive.file`, OAuth, token-store, selected-resource, or Live E2E work | High: OAuth scope lifecycle, token-store separation, selected-resource semantics, Drive folder behavior, image hosting exclusion, and live proof requirements. |
| P2-03-E / P2-26 `preview-update` | Separate-command design complete / implementation NO-GO | Fix a local-only implementation scope for the future command, non-destructive contract, focused tests, and verification boundary | Medium: larger than final-summary diagnostics, but command name, event family, evidence category, and safety stops are already fixed by P2-26. |
| P2-04 release-note generation follow-ons | Complete through P2-08, P2-12, and P2-17 | No immediate follow-on required from the current backlog | Low immediate value: draft, drift, evidence extraction, and CHANGELOG helper paths are complete. |
| P2-02 diagnostics follow-ons | Complete through P2-09, P2-10, P2-14, P2-16, P2-24, and P2-25 | No immediate follow-on required from the current backlog | Low immediate value: the remaining named deferred diagnostics are closed. |
| P2-06 / P2-07 readback reporting | Design and narrow implementation complete | No immediate follow-on required from the current backlog | Low immediate value: selected readback reporting scope is complete. |

## 3. Selection Criteria

The next candidate should:

- remain local-only unless a later task explicitly authorizes a separate
  external operation;
- avoid release, package, tag, publication, Avast, vendor, and flagged
  executable operations;
- preserve Frozen specifications, public APIs, persisted schemas, OAuth scope,
  authentication architecture, existing `dry-run` behavior, stdout
  compatibility, exit-code behavior, classification, retry behavior, Verified
  State semantics, and release records;
- build directly on a completed repository design record rather than reopen a
  completed candidate;
- reduce operator ambiguity about physical update preview evidence without
  confusing it with Google mutation, readback verification, publication,
  release clearance, package approval, vendor clearance, or Avast safety
  certification;
- have a smaller near-term decision surface than OAuth / Google Picker
  adoption.

## 4. Comparison Result

P2-01 / P2-27 remains valuable for least-privilege Google workflows, but the
next step would be an adoption record that touches OAuth scope, token-store
lifecycle, selected-resource semantics, and likely Live E2E proof. That is not
the best immediate P2-28 candidate because it would require wider approval
gates before a safe local implementation boundary can be fixed.

P2-03-E / P2-26 is now the strongest next candidate. P2-26 already fixed the
future command name `preview-update`, the separate `UPDATE_PREVIEW_*`
structured event family, the `physical-update-preview` evidence category, and
the non-destructive authorization boundary. The remaining next step can be a
local-only implementation-scope record that decides whether the first
implementation slice is narrow enough to proceed later without Google / OAuth
operations.

P2-04, P2-02, and P2-06 follow-ons are not selected because their currently
identified narrow backlog work is already complete or closed by later P2
records. Reopening them would risk duplicate planning without a clearer
backlog need.

## 5. Selected Candidate

Selected next candidate: P2-28, derived from P2-26 physical update dry-run
separate-command design.

P2-28 fixes the next scoped task boundary as a docs-only implementation-scope
record for a future `preview-update` local command slice.

The selected P2-28 scope is:

- define the first allowable implementation slice for `preview-update`;
- preserve the P2-26 command name, `UPDATE_PREVIEW_*` event family, and
  `physical-update-preview` evidence category;
- require a non-destructive local contract with `googleDocsMutationPerformed:
  false`, `googleDriveMutationPerformed: false`, `adapterApplyPerformed:
  false`, `readbackVerificationPerformed: false`, and `verifiedStateSaved:
  false`;
- require focused local tests before any broader verification;
- explicitly decide whether implementation may proceed later without live
  Google snapshot acquisition, OAuth access, token-store access, Live E2E, or
  package / release operations.

P2-28 selection does not itself authorize implementation. A later
implementation task must be separately authorized after this boundary is
accepted.

## 6. Non-Scope

P2-28 selection does not authorize:

- implementing `preview-update`;
- changing the existing `dry-run` command, `DRY_RUN_PLAN`,
  `DRY_RUN_SUMMARY`, stdout, stderr event meaning, exit codes, or
  classifications;
- changing command syntax in production code;
- acquiring a current Google Docs snapshot;
- calling Google Docs or Google Drive APIs;
- performing OAuth login, consent, scope change, token-store inspection,
  token-store reuse, token-store migration, token-store cleanup, or
  reauthorization;
- adopting Google Picker or `drive.file`;
- mutating Google Docs or Google Drive resources;
- applying a physical update, saving Verified State, promoting Verified State,
  or performing post-apply readback verification;
- exposing raw document content, block text, document IDs, private Google
  resource IDs, URLs, temporary public URLs, account identifiers, OAuth tokens,
  credentials, token-store paths, provider payloads, raw HTTP bodies, raw
  exception messages, stack traces, local sensitive paths, or release secrets;
- changing Frozen specifications, public APIs, persisted schemas, OAuth scope,
  authentication architecture, retry behavior, safe messages, release identity,
  package identity, publication state, vendor-clearance state, or Avast state;
- package generation, `dist` updates, release, tag, publication, GitHub asset,
  Live E2E, Avast, vendor, flagged executable, stage, commit, or push
  operations.

## 7. Future GO / NO-GO

Future implementation GO is limited to a later, separately authorized
local-only task if the P2-28 scope record proves that:

- the first slice can compute or report only from approved local / synthetic
  inputs;
- no adapter apply, Google Docs mutation, Google Drive mutation, post-apply
  readback verification, Verified State promotion, or Verified State save can
  occur;
- existing `dry-run` behavior and structured contracts remain unchanged;
- output uses only bounded labels, booleans, stable error codes, existing
  classifications, operation-kind labels, and non-content counts;
- focused tests cover unchanged `dry-run`, `UPDATE_PREVIEW_*` event isolation,
  missing / unsupported Verified State safe stops, document identity mismatch,
  revision conflict, snapshot failure, no-change behavior, non-mutation
  booleans, no adapter apply, no state save, and sensitive value exclusion.

NO-GO if the task requires live Google snapshot acquisition, Google / OAuth
credentials, token-store operations, new dependencies, public API changes,
persisted schema changes, existing `dry-run` contract changes, external state
lookup, package or `dist` mutation, release publication, vendor-clearance
judgment, Avast operation, flagged executable execution, or weakening existing
failure behavior.

## 8. Verification Boundary

For this candidate-selection record:

```powershell
git diff -- docs/development/Publisher_P2-28_CandidateSelection.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

No build, unit test, integration test, format verification, Live E2E, package
verification, OAuth, Google Docs / Drive, Avast, release, tag, publication,
stage, commit, or push operation is required or authorized for this docs-only
selection.
