# Publisher P2-19 Physical Update Dry-Run Integration Decision

Status  : Complete / NO-GO for integration into existing dry-run
Scope   : Decide whether existing `dry-run` should integrate physical update dry-run behavior
Depends : docs/development/Publisher_P2-18_DryRunContractShapeDecision.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only decision record. It does not implement
physical update dry-run behavior, change CLI command syntax, mutate Google
Docs or Google Drive, perform OAuth or token-store operations, run Live E2E,
update packages or `dist`, publish releases, create tags, claim vendor
clearance, or claim Avast safety certification.

## 1. Purpose

P2-19 decides whether the existing Publisher `dry-run <markdown-file>` command
should absorb physical update dry-run behavior.

The decision is needed because P2-18 fixed the structured local dry-run
contract as a CLI-only planning surface. Physical update dry-run behavior has
different inputs, authorization boundaries, evidence meaning, and failure
semantics than the existing local Markdown dry-run.

## 2. Decision

Decision: NO-GO for integrating physical update dry-run into the existing
`dry-run` command.

The existing `dry-run` command remains a local Markdown compilation and
planning command. It must not be reinterpreted as a physical update dry-run
bridge, Google verification surface, publication authorization step, or
release-clearance signal.

## 3. Required Future Shape

If physical update dry-run is adopted in the future, it must be introduced as:

- a separate command;
- a separate machine-readable contract;
- a separate authorization boundary;
- a separate evidence category from existing local `dry-run` output.

The future command must preserve the distinction between local planning,
physical update request construction, remote mutation, readback verification,
Verified State save, release authorization, package approval, vendor clearance,
and Avast safety certification.

## 4. Rationale

The existing `dry-run` command intentionally works without Google publish
settings and without remote mutation. Combining it with physical update
dry-run would make one command carry two different meanings:

- local Markdown planning evidence;
- revision-bound physical update preview evidence.

That ambiguity would increase the risk that operators or automation treat
local-only evidence as Google verification, publication readiness, or release
clearance.

## 5. Preserved Boundaries

P2-19 preserves:

- existing `dry-run <markdown-file>` syntax;
- existing `DRY_RUN_PLAN` and `DRY_RUN_SUMMARY` meanings;
- existing stdout behavior, exit codes, and CLI classification;
- existing failure taxonomy and safe diagnostics;
- Frozen specifications, public APIs, and persisted schemas;
- Google Docs / Drive, OAuth, token-store, and Live E2E gates;
- package, `dist`, release, tag, and publication gates;
- Avast pending, vendor-clearance not obtained, and Avast safety certification
  not claimed boundaries.

## 6. Non-Goals

P2-19 does not:

- implement a new command;
- design the future physical update dry-run contract;
- expose delivery-state diagnostics;
- modify Application or Infrastructure code;
- change tests;
- authorize Google Docs / Drive mutation, OAuth/token-store work, Live E2E,
  package generation, release, publication, tag, Avast, flagged-executable, or
  vendor-clearance operations.

## 7. Decision Summary

Existing `dry-run` remains local-only. Physical update dry-run integration into
that command is NO-GO. Any future adoption requires a separate command,
separate contract, and separate authorization boundary.
