# Publisher P2-22 HTTP Status CLI Exposure Evaluation

Status  : Complete / docs-only evaluation; `httpStatus` CLI exposure implementation remains NO-GO until separately authorized
Scope   : Evaluate whether bounded HTTP status structured diagnostics exposure is acceptable
Depends : docs/development/Publisher_P2-02_AdditionalDiagnosticsEvaluation.md, docs/development/Publisher_P2-20_DeliveryStateDiagnosticsCliDecision.md, docs/development/Publisher_P2-21D_DeliveryStateCliExposureEvaluation.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, src/Publisher.Cli/Program.cs, tests/unit/Publisher/CliApplicationTests.cs

This is a docs-only / local-only design evaluation. It does not add
`httpStatus` to CLI output, expose `deliveryState`, add or change structured
diagnostics fields, change retry behavior, mutate Google Docs or Google Drive,
perform OAuth or token-store operations, run Live E2E, update packages or
`dist`, publish releases, create tags, claim vendor clearance, or claim Avast
safety certification.

## 1. Purpose

P2-22 evaluates whether Publisher should expose HTTP status information in CLI
structured diagnostics / final failure summaries.

P2-02 left `httpStatus` deferred. Later diagnostics work added bounded retry
metadata, support summaries, readback status reporting, and delivery-state
carriers. P2-21D confirmed that `deliveryState` may be considered for a future
narrow final failure summary field, but did not implement CLI exposure.

The question here is whether `httpStatus` has an acceptable bounded diagnostic
shape, not whether it should be implemented in this task.

## 2. Evaluation Axes

P2-22 evaluates `httpStatus` against these constraints:

- final failure summary only;
- safe normalization to a numeric value or tightly bounded non-value marker;
- no change to failure classification, exit code, or retry behavior;
- no exposure of Google error reasons, OAuth details, provider payloads, raw
  responses, raw HTTP bodies, exception text, stack traces, URLs, credentials,
  tokens, token-store paths, account identifiers, or document identifiers;
- no unnecessary breakage of stdout, existing structured schema behavior,
  public APIs, or persisted schemas;
- no diagnostic over-expansion when considered together with possible future
  `deliveryState` exposure.

## 3. Decision

Decision: P2-22 docs-only evaluation is GO, but `httpStatus` CLI exposure
implementation is NO-GO in this task.

A future implementation may be acceptable only if a separately authorized
local-only CLI diagnostics task can prove that HTTP status is already available
at the final failure summary boundary as a sanitized, non-sensitive value.
Absent that proven carrier, the value must remain omitted.

## 4. Future Implementation Boundary

A future `httpStatus` CLI diagnostics implementation may be acceptable if it:

- emits `httpStatus` only on final failure summaries;
- emits only when an HTTP status is safely known at the CLI final-summary
  boundary;
- emits either an integer HTTP status code or an explicitly bounded marker such
  as `unknown`, if a marker is adopted by the implementation task;
- omits `httpStatus` from success summaries;
- omits `httpStatus` from failures that have no safely carried status;
- updates `SUPPORT_SUMMARY` only with the same bounded failure-summary-only
  value, if included there at all;
- preserves existing classification, exit code, stdout, command syntax, retry
  behavior, safe message, readback reporting, and any future delivery-state
  boundary;
- avoids adding Google error reasons, OAuth error details, provider payloads,
  raw HTTP bodies, raw exception text, stack traces, document IDs, URLs, local
  paths, credentials, OAuth tokens, token-store paths, account identifiers, or
  new failure taxonomy;
- includes focused `CliApplicationTests` coverage for known status emission,
  unknown / absent omission or marker behavior, `SUPPORT_SUMMARY` behavior if
  applicable, coexistence with `deliveryState` if implemented, and
  sensitive-value exclusion.

## 5. Rejected Scope

P2-22 rejects any broader exposure that would:

- expose provider error payloads, response bodies, Google API reasons, OAuth
  details, raw exceptions, stack traces, URLs, document IDs, credentials,
  tokens, or token-store paths;
- infer HTTP status from message text or raw exception text;
- expose `httpStatus` outside final failure summaries;
- treat HTTP status as a new failure classification;
- change retryability, retry budget behavior, exit codes, stdout, command
  syntax, public APIs, persisted schemas, or Frozen specifications;
- combine `httpStatus` and `deliveryState` into an expanded transport
  diagnostics block without a separate bounded design;
- imply publication authorization, release clearance, package approval, vendor
  clearance, Avast safety certification, or Verified State promotion.

## 6. Preserved Boundaries

P2-22 preserves:

- existing CLI stdout, exit codes, command syntax, classifications, and retry
  behavior;
- existing structured stderr behavior in this task;
- existing `deliveryState` NO-GO implementation boundary;
- existing support summary, retry diagnostics, failure-boundary, and readback
  reporting semantics;
- Frozen specifications, public APIs, and persisted schemas;
- Google Docs / Drive, OAuth, token-store, and Live E2E gates;
- package, `dist`, release, tag, and publication gates;
- Avast pending, vendor-clearance not obtained, and Avast safety certification
  not claimed boundaries.

## 7. Decision Summary

P2-22 confirms that a future `httpStatus` final failure summary field is not
rejected in principle, but only if it remains bounded, sanitized,
failure-summary-only, classification-neutral, and independently authorized.

This evaluation does not implement that field. Actual CLI exposure remains a
later, separately authorized implementation decision.
