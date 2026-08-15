# Publisher P2-23 Physical Update Dry-Run Separate Command Evaluation

Status  : COMPLETE / docs-only evaluation; separate-command implementation NO-GO until separately authorized
Scope   : Evaluate whether P2-03-E physical update dry-run can proceed as a future separate command
Depends : docs/development/Publisher_P2-03_ClearerDryRunOutputEvaluation.md, docs/development/Publisher_P2-19_PhysicalUpdateDryRunIntegrationDecision.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only evaluation record. It does not implement a
new command, change existing `dry-run` behavior, mutate Google Docs or Google
Drive, perform OAuth or token-store operations, run Live E2E, update packages
or `dist`, publish releases, create tags, claim vendor clearance, claim Avast
safety certification, stage, commit, or push.

## 1. Purpose

P2-23 evaluates the remaining P2-03-E physical update dry-run candidate after
P2-19 decided that the existing `dry-run <markdown-file>` command must not
absorb physical update dry-run behavior.

The question is whether a future physical update dry-run can be made safe only
as a separate command with a separate contract, separate evidence category, and
separate authorization boundary.

## 2. Decision

Decision: GO for future separate-command design; NO-GO for implementation in
this task.

The existing `dry-run` command remains local Markdown compilation and planning
only. It must not be reinterpreted as physical update dry-run evidence, Google
verification, publication authorization, release clearance, package approval,
vendor clearance, or Avast safety certification.

Any future implementation must first receive a separate implementation
authorization and must introduce physical update dry-run as a distinct command,
distinct structured diagnostic contract, and distinct evidence category.

## 3. Candidate Command Responsibility

A future command may preview physical update planning only when it can preserve
the Publisher update-safety lifecycle without performing remote mutation.

Candidate responsibility:

- load the current Verified State baseline;
- acquire or receive a current managed-document snapshot through an explicitly
  authorized non-mutating path;
- compute logical and physical update plans against the current snapshot;
- report safe counts, operation kinds, revision-precondition state, and stable
  conflict codes;
- avoid adapter apply, post-apply readback verification, Verified State
  promotion, and Verified State save;
- clearly report that no Google Docs mutation, Google Drive mutation,
  publication, release action, package action, or vendor-clearance action was
  performed.

The final command name is not adopted by this record. A future design should
choose an explicit name that cannot be confused with existing local
`dry-run <markdown-file>` behavior, such as a physical-update-specific preview
or plan command.

## 4. Required Inputs and State Dependencies

A future command cannot be equivalent to the current local `dry-run` command
because it depends on managed-document update state.

Required dependencies to define before implementation:

- Verified State baseline location and schema compatibility checks;
- current managed-document snapshot source;
- current document revision or revision-precondition input;
- target document identity matching rules;
- handling for missing, stale, unsupported, or mismatched Verified State;
- handling for no-change plans and empty physical plans;
- evidence wording that distinguishes local planning, non-mutating remote
  read/snapshot acquisition, live mutation, readback verification, and release
  clearance.

If current snapshot acquisition requires Google Docs or Google Drive API access,
that access remains a separate authorization gate. P2-23 does not approve
OAuth login, token-store access, Google readback, Google Docs mutation, Google
Drive mutation, Live E2E, cleanup, or credential use.

## 5. Non-Destructive Guarantee

A future physical update dry-run command must guarantee:

- no adapter apply;
- no Google Docs mutation;
- no Google Drive mutation;
- no temporary image hosting or permission changes;
- no Verified State promotion;
- no Verified State save;
- no package, `dist`, tag, release, publication, GitHub asset, Avast,
  flagged-executable, or vendor operation;
- no conversion of preview evidence into publication success, release
  authorization, package approval, vendor clearance, or Avast safety
  certification.

If these guarantees cannot be enforced and tested, implementation is NO-GO.

## 6. Revision Conflict and Safety Stops

ADR-0004 revision conflict handling remains a hard stop.

A future physical update dry-run command must not:

- continue past mismatched stored revision, observed revision, or expected
  candidate revision when that mismatch would be a revision conflict in the
  physical update lifecycle;
- compute speculative physical plans after unsafe preconditions fail;
- treat missing Verified State, unsupported Verified State schema, mismatched
  document identity, read failure, or managed-region mismatch as success;
- save a new Verified State after preview.

Safe-stop output may include only stable error codes, existing CLI
classification, bounded lifecycle phase labels, and value-safe summary fields.

## 7. Separation From Existing `dry-run`

The existing `dry-run <markdown-file>` command remains:

- local Markdown compilation and planning;
- usable without Google publish settings;
- non-mutating;
- not Verified State evidence;
- not current snapshot evidence;
- not physical update preview evidence;
- not readback verification;
- not publication authorization or release clearance.

A future physical update dry-run command would be a different surface because
it depends on Verified State and current managed-document snapshot evidence.
It must not reuse `DRY_RUN_PLAN` or `DRY_RUN_SUMMARY` with changed meaning.

## 8. Future Diagnostic Contract

A future structured diagnostic contract should be machine-readable, stable, and
separate from existing local dry-run events.

Allowed fields should be limited to:

- command and mode labels;
- lifecycle phase labels;
- bounded status labels such as `planned`, `blocked`, `revision-conflict`,
  `verified-state-missing`, `verified-state-unsupported`, `snapshot-failed`,
  `mismatch`, and `not-authorized`;
- logical and physical plan counts;
- bounded operation-kind counts;
- revision-precondition status without raw revision values unless a future
  design explicitly proves those values are safe;
- stable error codes and existing CLI classification;
- explicit boundary fields for Google Docs mutation, Google Drive mutation,
  adapter apply, readback verification, Verified State save, publication
  authorization, release clearance, package approval, vendor clearance, and
  Avast safety certification.

Prohibited values include raw document content, block text, document IDs,
private Google resource IDs, private URLs, temporary public URLs, OAuth tokens,
credentials, token-store paths, Authorization headers, cookies, provider
payloads, raw HTTP bodies, raw exception messages, stack traces, local
sensitive paths, usernames, hostnames, account identifiers, and release secrets.

## 9. Future Test Contract

Future implementation authorization must define focused local tests before any
broader verification.

Required test areas:

- command routing does not change existing `dry-run` syntax, stdout, stderr
  event meaning, exit codes, or classifications;
- success preview emits separate physical update dry-run diagnostics and no
  existing `DRY_RUN_PLAN` / `DRY_RUN_SUMMARY` semantic changes;
- missing Verified State is a safe stop;
- unsupported Verified State schema is a safe stop;
- revision conflict is a safe stop;
- snapshot acquisition failure is a safe stop;
- no-change and empty physical plans remain non-mutating and do not save state;
- adapter apply is never called;
- Verified State promotion and save are never called;
- sensitive values are excluded from stdout, stderr, diagnostics, fixtures, and
  failure messages.

Implementation verification must include focused unit coverage and any
Google/OAuth-backed verification only under separate explicit authorization.

## 10. GO / NO-GO

GO:

- record P2-23 as the docs-only separate-command evaluation for P2-03-E;
- keep existing `dry-run` unchanged;
- allow a future scoped design to define a physical update dry-run command only
  if it preserves separate command, contract, evidence, and authorization
  boundaries.

NO-GO:

- no implementation in this task;
- no command syntax change;
- no CLI output, stdout, stderr, diagnostic schema, classification, or exit-code
  change;
- no Frozen specification, public API, persisted schema, OAuth scope,
  authentication architecture, Google, package, release, vendor-clearance,
  Avast, or flagged-executable operation;
- no future implementation unless separately authorized with focused tests and
  an explicit non-destructive contract.

## 11. Local-Only Verification Plan

Required verification for this docs-only evaluation:

```powershell
git diff -- docs/development/Publisher_P2-23_PhysicalUpdateDryRunSeparateCommandEvaluation.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

Implementation tests, Release build, format verification, Live E2E, Google
Docs / Drive verification, package verification, Avast scanning, release
publication, staging, commit, and push are outside this docs-only scope.
