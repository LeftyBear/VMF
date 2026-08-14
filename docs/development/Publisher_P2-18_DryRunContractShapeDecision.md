# Publisher P2-18 Dry-Run Contract Shape Decision

Status  : Design complete / contract shape fixed / implementation not authorized
Scope   : Decide the structured dry-run contract shape for the deferred P2-03-C candidate
Depends : docs/development/Publisher_P2-03_ClearerDryRunOutputEvaluation.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, src/Publisher.Cli/Program.cs, tests/unit/Publisher/CliApplicationTests.cs

This record is a design-only decision gate. It fixes the contract shape that a
future P2-03-C implementation may use, but it does not authorize implementation.
No code, tests, public APIs, persisted schemas, packages, `dist` artifacts,
Google Docs, Google Drive, OAuth state, token stores, Live E2E, release state,
publication state, Avast state, vendor-clearance state, stage, commit, or push
operation is authorized by this record.

## 1. Purpose

P2-18 decides the contract shape for the deferred P2-03-C structured dry-run
output contract.

The goal is to make local dry-run planning easier for automation to consume
without changing what dry-run means. Dry-run remains local planning evidence
only. It is not Google verification, publication authorization, release
clearance, vendor clearance, Avast safety certification, package approval, or a
physical update dry-run bridge.

## 2. Current Surface

The current CLI command is `dry-run <markdown-file>`.

The current success path:

- validates one Markdown path argument;
- loads local settings without requiring Google publish settings;
- validates and compiles Markdown locally;
- emits a structured stderr `DRY_RUN_PLAN` event;
- emits the final `DRY_RUN_SUCCEEDED` command summary;
- returns exit code `0`.

The existing `DRY_RUN_PLAN` event already carries local-only planning fields
such as `mode`, `stepCount`, operation and content counts, `planningEvidence`,
`safePlanSummary`, mutation boundary fields, readback boundary fields, Verified
State boundary fields, and authorization / clearance boundary fields.

P2-13 also added `failureBoundary` only to dry-run final failure summaries. That
field is derived from existing CLI classification and safe routing context.

## 3. Decision

Future P2-03-C implementation should add a new structured stderr event:

- event code: `DRY_RUN_SUMMARY`;
- level: `info`;
- command: `dry-run`;
- phase: `planner`;
- operation: `summary`;
- shape: flat JSON fields;
- emission: success path only, after `DRY_RUN_PLAN` and before the final
  `DRY_RUN_SUCCEEDED` command summary.

The implementation must keep the existing `DRY_RUN_PLAN` event unchanged except
for any separately reviewed bug fix. `DRY_RUN_PLAN` remains the current local
planning event. `DRY_RUN_SUMMARY` becomes the new machine-readable contract
event for automation.

The contract should be flat, not nested. This matches the existing Publisher
diagnostic style for command events, minimizes parser changes, and avoids
making object topology part of the first public-like automation surface.

## 4. Contract Fields

Allowed `DRY_RUN_SUMMARY` fields:

- `contractVersion`: `1`;
- `mode`: `local-dry-run`;
- `planningResult`: `succeeded`;
- `markdownCompilation`: `succeeded`;
- `planningEvidence`: `local-only`;
- `stepCount`;
- `operationCount`;
- `batchUpdateStepCount`;
- `tableStepCount`;
- `imageStepCount`;
- `insertTextOperationCount`;
- `headingOperationCount`;
- `listOperationCount`;
- `textStyleOperationCount`;
- `paragraphAlignmentOperationCount`;
- `codeBlockOperationCount`;
- `quoteOperationCount`;
- `googleDocsMutation`: `not-attempted`;
- `googleDriveMutation`: `not-attempted`;
- `oauthOperation`: `not-attempted`;
- `tokenStoreOperation`: `not-attempted`;
- `physicalUpdatePlanApplied`: `false`;
- `readbackStatus`: `not-attempted`;
- `readbackVerified`: `false`;
- `verifiedStateSaved`: `false`;
- `publicationAuthorized`: `false`;
- `releaseClearance`: `false`;
- `packageApproval`: `false`;
- `vendorClearance`: `false`;
- `avastSafetyCertification`: `false`.

The first implementation may omit `safePlanSummary` from `DRY_RUN_SUMMARY` even
though it remains allowed on `DRY_RUN_PLAN`; automation should prefer bounded
scalar fields over prose. If `safePlanSummary` is included later, it must remain
fixed-template, value-free, and covered by sensitive-value exclusion tests.

## 5. Compatibility Rules

Future implementation must preserve:

- existing `dry-run <markdown-file>` command syntax;
- existing stdout behavior;
- existing exit codes and CLI classifications;
- existing `DRY_RUN_PLAN` event code and currently tested fields;
- existing `DRY_RUN_SUCCEEDED` final summary behavior;
- existing `failureBoundary` behavior for dry-run final failure summaries;
- ADR-0006 structured stderr and redaction requirements;
- ADR-0007 safe message, classification, and exit-code requirements.

Adding `DRY_RUN_SUMMARY` means existing parsers that consume `DRY_RUN_PLAN`
continue to work, while new automation can bind to the explicit summary
contract.

## 6. Failure Boundary

P2-18 does not add a new failure taxonomy.

Future implementation must not create a separate dry-run failure classification
system. Failure reporting remains governed by ADR-0007 classification and the
existing P2-13 `failureBoundary` field on dry-run final failure summaries.

`DRY_RUN_SUMMARY` is success-only for the first implementation. Failure cases
continue to use the final command summary, existing classification, safe
message, result code, and optional `failureBoundary`.

## 7. Value-Safe Allow-List

Future implementation must emit only bounded, deterministic, value-safe fields.

It must not emit:

- raw Markdown paths;
- raw Markdown content;
- raw local image paths;
- raw document IDs or document URLs;
- private URLs or temporary URLs;
- OAuth credentials, token-store paths, tokens, cookies, secrets, or
  Authorization headers;
- provider request or response bodies;
- raw exception messages;
- stack traces;
- package paths or user profile paths;
- release, vendor, or Avast claims beyond explicit `false` / `not-attempted`
  boundary values.

## 8. Implementation GO/NO-GO

Implementation readiness: GO after separate explicit implementation
authorization.

Implementation remains not authorized by this design record.

Implementation GO conditions:

- add only the `DRY_RUN_SUMMARY` structured stderr success event;
- keep the event flat and value-safe;
- preserve `DRY_RUN_PLAN` compatibility;
- preserve stdout, exit codes, CLI classification, safe messages, and
  `failureBoundary`;
- preserve dry-run semantics as local planning only;
- add focused unit coverage for event presence, field values, ordering,
  compatibility, and sensitive-value exclusion.

NO-GO conditions:

- changing public APIs or persisted schemas;
- changing command syntax, stdout, exit codes, or CLI classification;
- changing `DRY_RUN_PLAN` in a way that breaks existing parsers;
- adding nested contract shape for the first automation contract;
- adding or renaming failure taxonomy;
- requiring Google Docs or Google Drive mutation;
- requiring OAuth or token-store operation;
- requiring Live E2E;
- bridging to physical update dry-run semantics;
- touching release, package, `dist`, publication, vendor-clearance, or Avast
  state;
- adding dependencies;
- staging, committing, or pushing without separate authorization.

## 9. Verification Plan For Future Implementation

Required local-only verification for a future implementation:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~CliApplicationTests"
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
dotnet build VMF.Publisher.sln --configuration Release --no-restore
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
git status --short --branch
```

Prohibited verification:

- Live E2E;
- Google Docs or Google Drive mutation;
- OAuth or token-store operation;
- package generation or package verification;
- `dist` mutation;
- Avast or flagged executable execution;
- release, tag, publication, GitHub asset, stage, commit, or push.

## 10. Decision Summary

P2-18 fixes the contract shape as:

- new `DRY_RUN_SUMMARY`, not expanded `DRY_RUN_PLAN`;
- flat fields, not nested objects;
- existing parser compatibility preserved;
- value-safe allow-list only;
- no failure taxonomy expansion.

The next step is a separate implementation authorization decision. Until that
authorization exists, P2-03-C remains deferred for implementation.
