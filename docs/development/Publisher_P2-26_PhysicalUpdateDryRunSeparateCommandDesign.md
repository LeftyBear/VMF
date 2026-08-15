# Publisher P2-26 Physical Update Dry-Run Separate Command Design

Status  : COMPLETE / docs-only design; separate-command implementation NO-GO until separately authorized
Scope   : Define the command name, contract shape, evidence category, and authorization boundary for a future physical update dry-run separate command
Depends : docs/development/Publisher_P2-23_PhysicalUpdateDryRunSeparateCommandEvaluation.md, docs/development/Publisher_P2-19_PhysicalUpdateDryRunIntegrationDecision.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only design record. It does not implement a new
command, change existing `dry-run` behavior, mutate Google Docs or Google
Drive, perform OAuth or token-store operations, run Live E2E, update packages
or `dist`, publish releases, create tags, claim vendor clearance, claim Avast
safety certification, stage, commit, or push.

## 1. Purpose

P2-26 follows the P2-23 decision that P2-03-E physical update dry-run may
proceed only as a future separate command. This record fixes the design
boundary before any implementation task can be considered.

The design answers four questions:

- command name;
- structured contract shape;
- evidence category;
- authorization boundary.

Implementation remains NO-GO until separately authorized with focused tests and
an explicit non-destructive contract.

## 2. Decision

Decision: design complete for a future `preview-update` command; implementation
NO-GO in this task.

The existing `dry-run <markdown-file>` command remains local Markdown
compilation and planning only. It must not be integrated with physical update
preview behavior and must not change its stdout, stderr event meaning, exit
codes, classifications, or `DRY_RUN_PLAN` / `DRY_RUN_SUMMARY` contracts.

## 3. Command Name

Selected future command name: `preview-update`.

Rationale:

- it names update-preview behavior without overloading existing `dry-run`;
- it avoids implying Google mutation, publication, release readiness, package
  approval, vendor clearance, or Avast safety certification;
- it leaves existing local dry-run evidence separate from revision-bound
  physical update preview evidence.

The future command may be documented as:

```text
publisher preview-update <markdown-file>
```

This record does not adopt final CLI syntax, options, or arguments beyond the
command name. Any implementation task must define required inputs explicitly
and preserve existing command behavior.

## 4. Contract Shape

The future structured contract is a new event family separate from existing
dry-run events.

Selected event names:

- `UPDATE_PREVIEW_PLAN`
- `UPDATE_PREVIEW_SUMMARY`
- `UPDATE_PREVIEW_FAILED`

`UPDATE_PREVIEW_PLAN` is the machine-readable preview event for a successfully
computed physical update preview. `UPDATE_PREVIEW_SUMMARY` is the final
success summary. `UPDATE_PREVIEW_FAILED` is the final failure summary.

Allowed field categories:

- command and mode labels;
- bounded lifecycle phase labels;
- bounded status labels;
- stable error codes and existing CLI classification;
- logical plan counts;
- physical operation counts;
- operation-kind counts;
- revision-precondition status labels;
- non-destructive boundary booleans.

Allowed status labels:

- `planned`
- `no-change`
- `blocked`
- `revision-conflict`
- `verified-state-missing`
- `verified-state-unsupported`
- `document-identity-mismatch`
- `snapshot-failed`
- `managed-region-mismatch`
- `not-authorized`

Required non-destructive boundary booleans:

- `googleDocsMutationPerformed: false`
- `googleDriveMutationPerformed: false`
- `adapterApplyPerformed: false`
- `readbackVerificationPerformed: false`
- `verifiedStateSaved: false`
- `publicationAuthorized: false`
- `releaseClearanceGranted: false`
- `packageApprovalGranted: false`
- `vendorClearanceGranted: false`
- `avastSafetyCertificationClaimed: false`

The contract must not reuse `DRY_RUN_PLAN`, `DRY_RUN_SUMMARY`, or
`DRY_RUN_FAILED` with changed meaning.

## 5. Evidence Category

Selected evidence category: `physical-update-preview`.

This evidence category means only that a future implementation computed a
non-mutating, revision-bound physical update preview from approved local and
snapshot inputs.

It is not:

- local Markdown dry-run evidence;
- Google Docs mutation evidence;
- Google Drive mutation evidence;
- readback verification;
- Verified State promotion or save;
- publication authorization;
- release clearance;
- package approval;
- vendor clearance;
- Avast safety certification.

If snapshot acquisition uses Google Docs or Google Drive APIs, that acquisition
must be recorded as a separate non-mutating operation gate and must not be
collapsed into the preview evidence category.

## 6. Authorization Boundary

P2-26 authorizes no implementation and no external operation.

Any future implementation must separately authorize:

- command implementation;
- focused local unit tests;
- any non-mutating snapshot acquisition mechanism;
- any Google Docs or Google Drive API access;
- any OAuth or token-store access;
- any Live E2E verification;
- any package, release, tag, publication, Avast, flagged-executable, or
  vendor-clearance operation.

The future command must guarantee:

- no adapter apply;
- no Google Docs mutation;
- no Google Drive mutation;
- no temporary image hosting or permission changes;
- no post-apply readback verification;
- no Verified State promotion;
- no Verified State save;
- no package, `dist`, tag, release, publication, GitHub asset, Avast,
  flagged-executable, or vendor operation.

## 7. Safety Stops

The future command must stop safely before computing or reporting a successful
preview when any of these conditions is present:

- missing Verified State;
- unsupported Verified State schema;
- mismatched target document identity;
- missing or unauthorized current snapshot;
- snapshot acquisition failure;
- revision conflict;
- managed-region mismatch;
- unsafe or unapproved snapshot source;
- unavailable required input.

Safe-stop output may include only stable error codes, existing CLI
classification, bounded status labels, lifecycle phase labels, and
non-sensitive counts.

## 8. Safe-Value Boundary

Allowed values are bounded status labels, lifecycle phase labels, stable error
codes, existing CLI classification labels, booleans, operation-kind labels, and
non-content counts.

Prohibited values include raw document content, block text, document IDs,
private Google resource IDs, private URLs, temporary public URLs, OAuth tokens,
credentials, token-store paths, Authorization headers, cookies, provider
payloads, raw HTTP bodies, raw exception messages, stack traces, local
sensitive paths, usernames, hostnames, account identifiers, and release
secrets.

## 9. Future Test Contract

Future implementation authorization must include focused local tests before
broader verification.

Required focused test areas:

- existing `dry-run` command syntax, stdout, stderr events, exit codes, and
  classifications are unchanged;
- `preview-update` emits only the new `UPDATE_PREVIEW_*` event family;
- no successful preview is reported for missing Verified State;
- no successful preview is reported for unsupported Verified State schema;
- no successful preview is reported for document identity mismatch;
- revision conflict is a safe stop;
- snapshot acquisition failure is a safe stop;
- no-change and empty physical plans remain non-mutating and do not save state;
- adapter apply is never called;
- Verified State promotion and save are never called;
- sensitive values are excluded from stdout, stderr, diagnostics, fixtures, and
  failure messages.

Google/OAuth-backed verification remains outside implementation unless
separately authorized.

## 10. GO / NO-GO

GO:

- record P2-26 as the docs-only design for the P2-03-E separate command;
- fix `preview-update` as the future command name;
- fix the new `UPDATE_PREVIEW_*` structured contract family;
- fix `physical-update-preview` as the evidence category;
- preserve separate authorization gates for implementation, snapshot
  acquisition, Google/OAuth, Live E2E, package, release, Avast, flagged
  executable, and vendor-clearance work.

NO-GO:

- no implementation in this task;
- no command syntax change;
- no existing `dry-run` behavior or contract change;
- no CLI output, stdout, stderr, diagnostic schema, classification, or
  exit-code change in code;
- no Frozen specification, public API, persisted schema, OAuth scope,
  authentication architecture, Google, package, release, vendor-clearance,
  Avast, or flagged-executable operation;
- no future implementation unless separately authorized with focused tests and
  an explicit non-destructive contract.

## 11. Local-Only Verification Plan

Required verification for this docs-only design:

```powershell
git diff -- docs/development/Publisher_P2-26_PhysicalUpdateDryRunSeparateCommandDesign.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

Implementation tests, Release build, format verification, Live E2E, Google
Docs / Drive verification, package verification, Avast scanning, release
publication, staging, commit, and push are outside this docs-only scope.
