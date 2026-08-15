# Publisher P2-29 Preview Update Implementation Scope Planning

Status  : COMPLETE / docs-only implementation-scope planning; implementation remains NO-GO until separately authorized
Scope   : Define the first allowable local-only implementation slice for the future `preview-update` command
Depends : docs/development/Publisher_P2-28_CandidateSelection.md, docs/development/Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md, docs/development/Publisher_P2-23_PhysicalUpdateDryRunSeparateCommandEvaluation.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only implementation-scope planning record. It does
not implement `preview-update`, change existing `dry-run`, change command
syntax in production code, change stdout, change structured stderr contracts,
call Google Docs or Google Drive APIs, perform OAuth or token-store operations,
run Live E2E, update packages or `dist`, perform release, tag, publication,
Avast, vendor, or flagged executable operations, stage, commit, or push.

## 1. Purpose

P2-29 follows the P2-28 candidate selection and fixes the first implementation
slice that may be considered later for the P2-26 `preview-update` command.

The planning goal is to decide whether a later local-only implementation task
can be narrow enough to proceed without live Google snapshot acquisition,
OAuth access, token-store access, Live E2E, package / release operations, or
external state lookup.

Implementation remains NO-GO in this task.

## 2. Adopted Boundary

Future implementation GO is limited to a first slice that wires a local
`preview-update` command to approved local or synthetic inputs only.

The first slice may:

- add a CLI command branch for `preview-update` only after explicit
  implementation authorization;
- load Markdown from a local file using existing Publisher local parsing and
  compilation paths;
- load Verified State only from an explicitly supplied local test or operator
  input path, if the implementation task authorizes that path;
- use an explicitly supplied local or synthetic current snapshot fixture;
- compute a physical update plan using existing local planning components;
- emit the P2-26 `UPDATE_PREVIEW_PLAN`, `UPDATE_PREVIEW_SUMMARY`, and
  `UPDATE_PREVIEW_FAILED` event family;
- report only the `physical-update-preview` evidence category;
- include required non-destructive boundary booleans, all set to `false`;
- add focused local unit tests before any broader verification.

The first slice must not acquire a current Google Docs snapshot itself. Any
future snapshot acquisition through Google Docs or Google Drive APIs is a
separate non-mutating operation gate and is outside this first implementation
slice.

## 3. Candidate Change Areas

Candidate production change areas for a later authorized implementation:

- `src/Publisher.Cli/Program.cs` for command dispatch, structured event
  emission, safe failure summaries, and existing `dry-run` compatibility
  protection;
- `src/Publisher/Composition/PublisherCompositionRoot.cs` only if an existing
  local composition boundary is insufficient;
- `src/Publisher/Application/VerifiedPublishLifecycle.cs` or a narrowly placed
  Application-layer preview coordinator if command orchestration cannot be
  expressed through existing lifecycle methods without changing existing
  behavior;
- existing Publisher Application physical planning types, only through
  additive local orchestration that preserves current semantics.

Candidate test change areas:

- `tests/unit/Publisher/CliApplicationTests.cs` for command shape, structured
  event isolation, unchanged `dry-run`, safe failures, and sensitive-value
  exclusion;
- focused Publisher unit tests near existing physical update and Verified
  State tests if an Application-layer preview coordinator is added.

No Frozen specification, public API, persisted schema, OAuth scope,
authentication architecture, Google infrastructure client, package, release,
or distribution file is a candidate change area for the first slice.

## 4. Required Contract

The future command name remains:

```text
publisher preview-update <markdown-file>
```

This planning record does not finalize additional options. A later
implementation task must explicitly define any required local Verified State
and local snapshot input shape before editing code.

Successful preview output must use only the P2-26 event family:

- `UPDATE_PREVIEW_PLAN`;
- `UPDATE_PREVIEW_SUMMARY`.

Failure output must use:

- `UPDATE_PREVIEW_FAILED`.

The command must not reuse `DRY_RUN_PLAN`, `DRY_RUN_SUMMARY`, or
`DRY_RUN_FAILED` with changed meaning. Existing `dry-run <markdown-file>`
stdout, stderr event meaning, exit-code behavior, classifications, and
diagnostic fields must remain unchanged.

Required boundary fields for all successful preview summaries:

- `googleDocsMutationPerformed: false`;
- `googleDriveMutationPerformed: false`;
- `adapterApplyPerformed: false`;
- `readbackVerificationPerformed: false`;
- `verifiedStateSaved: false`;
- `publicationAuthorized: false`;
- `releaseClearanceGranted: false`;
- `packageApprovalGranted: false`;
- `vendorClearanceGranted: false`;
- `avastSafetyCertificationClaimed: false`.

Allowed preview statuses are the P2-26 labels:

- `planned`;
- `no-change`;
- `blocked`;
- `revision-conflict`;
- `verified-state-missing`;
- `verified-state-unsupported`;
- `document-identity-mismatch`;
- `snapshot-failed`;
- `managed-region-mismatch`;
- `not-authorized`.

## 5. Safe Stops

A later implementation must stop safely before reporting a successful preview
when any of these conditions is present:

- missing Verified State;
- unsupported Verified State schema;
- document identity mismatch between the Verified State and supplied snapshot;
- missing, unauthorized, or unsafe local snapshot source;
- snapshot loading failure;
- revision conflict;
- managed-region mismatch;
- missing required local input;
- unapproved attempt to use Google Docs, Google Drive, OAuth, token-store, or
  external state lookup;
- any requirement to apply a physical update, perform readback verification,
  promote or save Verified State, or mutate package / release state.

Safe-stop output may include only stable error codes, existing CLI
classification labels, bounded status labels, lifecycle phase labels,
operation-kind labels, booleans, and non-content counts.

## 6. Safe-Value Boundary

Allowed values:

- bounded command, mode, status, lifecycle phase, operation-kind, evidence
  category, and classification labels;
- existing stable error codes;
- booleans;
- non-content counts.

Prohibited values:

- raw document content;
- block text;
- document IDs;
- private Google resource IDs;
- private URLs;
- temporary public URLs;
- OAuth tokens;
- credentials;
- credential paths;
- token-store paths;
- Authorization headers;
- cookies;
- provider payloads;
- raw HTTP bodies;
- raw exception messages;
- stack traces;
- local sensitive paths;
- usernames;
- hostnames;
- account identifiers;
- release secrets.

## 7. Focused Test Plan

A later implementation task must add focused local tests before broader
verification. Required test areas:

- existing `dry-run` command syntax, stdout, stderr events, exit codes,
  classifications, `DRY_RUN_PLAN`, and `DRY_RUN_SUMMARY` remain unchanged;
- `preview-update` emits only `UPDATE_PREVIEW_*` structured events;
- `preview-update` does not emit `DRY_RUN_*` events;
- missing Verified State returns `verified-state-missing` and no successful
  preview;
- unsupported Verified State schema returns `verified-state-unsupported`;
- document identity mismatch returns `document-identity-mismatch`;
- revision conflict returns `revision-conflict`;
- snapshot loading failure returns `snapshot-failed`;
- no-change / empty physical plan returns `no-change` while preserving all
  non-mutation booleans as `false`;
- planned physical operations report only operation-kind counts and
  non-content counts;
- adapter apply is never called;
- Google Docs and Google Drive mutation flags remain `false`;
- readback verification is not performed;
- Verified State promotion and save are never called;
- sensitive values are excluded from stdout, stderr, diagnostics, fixtures,
  and failure messages.

Implementation verification, if later authorized, should start with:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~CliApplicationTests"
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~PhysicalUpdate|FullyQualifiedName~Verified"
git diff --check
git status --short --branch
```

Full Publisher unit tests, Release build, and format verification remain
required for implementation completion, but they are not run for this
docs-only planning task unless separately requested.

## 8. GO / NO-GO Decision

Planning result: GO for a later separately authorized local-only first
implementation slice if it stays within the boundary above.

Implementation remains NO-GO until a later task explicitly authorizes code and
test changes.

NO-GO remains active for any task requiring live Google snapshot acquisition,
Google Docs or Google Drive API access, OAuth login, token-store inspection or
mutation, Google Picker or `drive.file` adoption, public API changes, persisted
schema changes, new dependencies, existing `dry-run` contract changes, stdout
compatibility changes, classification or exit-code changes, package or `dist`
mutation, release / tag / publication work, Live E2E, vendor-clearance
judgment, Avast operation, flagged executable execution, stage, commit, push,
or weakened failure behavior.

## 9. Local-Only Verification Plan

Required verification for this docs-only planning record:

```powershell
git diff -- docs/development/Publisher_P2-29_PreviewUpdateImplementationScopePlanning.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

No build, unit test, integration test, format verification, Live E2E, package
verification, OAuth, Google Docs / Drive, Avast, release, tag, publication,
stage, commit, or push operation is required or authorized for this docs-only
planning task.
