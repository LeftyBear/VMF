# Publisher P2-30 Candidate Selection

Status  : COMPLETE / docs-only candidate selection; implementation remains NO-GO until separately authorized
Scope   : Organize GO / CONDITIONAL GO / NO-GO decision inputs for the P2-29 `preview-update` implementation scope
Depends : docs/development/Publisher_P2-29_PreviewUpdateImplementationScopePlanning.md, docs/development/Publisher_P2-28_CandidateSelection.md, docs/development/Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only candidate-selection record. It does not
implement `preview-update`, change existing `dry-run`, change command syntax
in production code, change stdout, change structured stderr contracts, call
Google Docs or Google Drive APIs, perform OAuth or token-store operations, run
Live E2E, update packages or `dist`, perform release, tag, publication,
Avast, vendor, or flagged executable operations, stage, commit, or push.

## 1. Purpose

P2-30 follows the P2-29 implementation-scope planning record and organizes the
decision inputs needed before a later implementation task can be approved.

The goal is not to start implementation. The goal is to decide what evidence
would support GO, CONDITIONAL GO, or NO-GO for a future separately authorized
local-only first implementation slice.

## 2. P2-29 Baseline Confirmation

P2-29 fixed the only candidate implementation scope that may be considered
next: a first local-only `preview-update` command slice that operates from
approved local or synthetic inputs.

The P2-29 baseline remains:

- command name: `preview-update`;
- initial command shape: `publisher preview-update <markdown-file>`;
- structured event family: `UPDATE_PREVIEW_PLAN`,
  `UPDATE_PREVIEW_SUMMARY`, and `UPDATE_PREVIEW_FAILED`;
- evidence category: `physical-update-preview`;
- first-slice input boundary: local Markdown plus explicitly authorized local
  or synthetic Verified State and current snapshot inputs;
- no live Google snapshot acquisition in the first implementation slice;
- no adapter apply, Google Docs mutation, Google Drive mutation, readback
  verification, Verified State promotion, or Verified State save;
- existing `dry-run` syntax, stdout, stderr event meaning, exit codes,
  classifications, and `DRY_RUN_*` contracts remain unchanged.

P2-30 does not change that baseline. If later implementation needs additional
arguments, file formats, fixture shapes, or local input wiring, those details
must be fixed in the implementation authorization task before code edits begin.

## 3. Implementation Start Conditions

A future implementation task may be considered only if all of these conditions
are true:

- the task explicitly authorizes source and test changes for the first
  local-only `preview-update` slice;
- the authorized file scope names the candidate production and test areas;
- the local Verified State input shape and local or synthetic snapshot input
  shape are fixed before editing;
- the command can compute a preview without Google Docs, Google Drive, OAuth,
  token-store, Live E2E, package, release, Avast, vendor, flagged executable,
  or external state access;
- existing `dry-run` behavior remains protected by focused tests;
- the implementation can use existing Publisher parsing, compilation,
  Verified State, and physical planning boundaries without changing public
  APIs, persisted schemas, Frozen specifications, OAuth scopes, or
  authentication architecture;
- safe-value filtering can exclude raw content, provider identifiers,
  credentials, token-store paths, raw exception details, stacks, local
  sensitive paths, and release secrets from stdout, stderr, diagnostics,
  fixtures, and failures;
- focused local tests can be run before broader build / format verification.

## 4. Stop Conditions

A future implementation task must remain NO-GO or stop immediately if it
requires any of the following:

- live Google Docs or Google Drive API access;
- OAuth login, consent, reauthorization, scope change, token-store inspection,
  token-store reuse, token-store migration, or token-store cleanup;
- Google Picker or `drive.file` adoption;
- acquiring a current Google Docs snapshot as part of the command;
- applying a physical update through an adapter;
- performing post-apply readback verification;
- promoting or saving Verified State;
- changing existing `dry-run` stdout, stderr events, classifications, or exit
  codes;
- changing Frozen specifications, public APIs, persisted schemas, OAuth
  scopes, authentication architecture, package identity, release identity, or
  publication flow;
- adding a dependency;
- package generation, `dist` update, release, tag, publication, GitHub asset,
  Avast, vendor-clearance, or flagged executable operation;
- exposing prohibited sensitive values;
- weakening failure behavior, changing safe-stop semantics, skipping required
  tests, staging, committing, or pushing.

## 5. Technical Dependencies And Open Items

Known technical dependencies:

- existing CLI dispatch and structured summary patterns;
- existing Markdown parsing and compilation paths;
- existing Verified State loading / interpretation boundary, if suitable for
  local input without schema or public-contract changes;
- existing managed-document snapshot / physical planning types;
- existing safe diagnostic, classification, and stable error-code conventions;
- existing unit-test harnesses for CLI behavior and physical update planning.

Open items that must be closed before implementation:

- exact local Verified State argument or input mechanism;
- exact local or synthetic current snapshot argument or fixture mechanism;
- whether existing Application-layer orchestration is sufficient or a narrowly
  placed preview coordinator is required;
- exact stable error-code mapping for each P2-29 safe-stop status;
- exact success summary field names beyond the required P2-26 boundary
  booleans, if any;
- whether the first slice reports only aggregate operation-kind counts or also
  bounded lifecycle phase counts;
- whether any existing helper emits local paths or raw exception text that
  must be wrapped before use at the CLI boundary.

None of these open items authorizes implementation. If any item cannot be
closed without changing a protected boundary, the implementation decision is
NO-GO.

## 6. Local-Only Verification Boundary

Local-only implementation verification can cover:

- command parsing and dispatch for `preview-update`;
- unchanged `dry-run` command behavior;
- `UPDATE_PREVIEW_*` event isolation from `DRY_RUN_*`;
- missing / unsupported Verified State safe stops;
- document identity mismatch;
- revision conflict;
- managed-region mismatch;
- snapshot loading failure;
- no-change preview summaries with all non-mutation booleans `false`;
- planned operation counts and operation-kind counts from synthetic inputs;
- adapter apply not called;
- Google mutation flags remain `false`;
- readback verification not performed;
- Verified State promotion and save not called;
- sensitive-value exclusion from stdout, stderr, diagnostics, fixtures, and
  failure summaries.

Local-only verification cannot prove:

- Google Docs API compatibility;
- Google Drive API compatibility;
- OAuth consent, token-store, or scope behavior;
- selected-resource behavior from Google Picker or `drive.file`;
- Live E2E behavior;
- production document mutation safety;
- package, release, tag, publication, or GitHub asset readiness;
- Avast vendor clearance or Avast safety certification.

## 7. External / Gated Items

The following items remain separate from the first implementation slice:

- non-mutating Google Docs snapshot acquisition;
- Google Drive lookup or selected-resource access;
- OAuth Desktop login, reauthorization, token-store read/write/delete, scope
  migration, or credential handling;
- Live E2E verification;
- Google Docs or Google Drive mutation;
- package generation or `dist` update;
- release, tag, publication, or GitHub asset operation;
- Avast operation, vendor-clearance judgment, or flagged executable execution.

Each item requires its own scope, authorization, evidence plan, and cleanup /
redaction boundary before it can be performed.

## 8. Decision Criteria

GO for a future implementation task only if:

- all implementation start conditions are satisfied;
- all open input-shape and stable-error mapping questions are closed in the
  implementation task;
- the task remains local-only and synthetic / file-input bounded;
- the implementation can preserve Frozen specs, public APIs, persisted
  schemas, OAuth scope, authentication architecture, existing `dry-run`,
  stdout, exit codes, classifications, package / release state, and vendor
  boundaries;
- focused tests are authorized and sufficient to prove the local safety
  contract before broader verification.

CONDITIONAL GO if:

- the first slice is still local-only, but one or more input-shape or
  orchestration decisions must be fixed at task start;
- the condition is documentable without external services, dependency
  additions, public-contract changes, or protected-boundary changes;
- the task includes an explicit stop point before code edits if the condition
  cannot be resolved from existing repository patterns.

NO-GO if:

- implementation requires Google, OAuth, token-store, Live E2E, package,
  release, Avast, vendor, flagged executable, stage, commit, or push
  operations;
- implementation requires changing existing `dry-run` behavior or
  reinterpreting `DRY_RUN_*` events;
- implementation requires a Frozen specification, public API, persisted
  schema, OAuth scope, authentication architecture, package identity, release
  identity, or publication-flow change;
- implementation requires a new dependency;
- safe-stop statuses cannot be mapped without exposing sensitive values or
  weakening failure behavior;
- tests cannot protect unchanged `dry-run`, event isolation, non-mutation
  booleans, no adapter apply, no Verified State save, and sensitive-value
  exclusion.

## 9. P2-30 Selection Result

Decision: CONDITIONAL GO for a future separately authorized local-only
implementation task, provided the task first fixes the local Verified State
and local / synthetic snapshot input shapes and confirms that existing
Publisher boundaries can support the slice without public-contract,
persisted-schema, dependency, OAuth, Google, or `dry-run` changes.

Implementation remains NO-GO in P2-30.

NO-GO remains active for any broader physical update preview route that needs
live snapshot acquisition, OAuth or token-store access, Google Picker or
`drive.file` adoption, Google Docs / Drive mutation, Live E2E, package /
release operations, vendor-clearance judgment, Avast operation, flagged
executable execution, staging, committing, pushing, or weakened failure
semantics.

## 10. Local-Only Verification Plan

Required verification for this docs-only candidate-selection record:

```powershell
git diff -- docs/development/Publisher_P2-30_CandidateSelection.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

No build, unit test, integration test, format verification, Live E2E, package
verification, OAuth, Google Docs / Drive, Avast, release, tag, publication,
stage, commit, or push operation is required or authorized for this docs-only
selection.
