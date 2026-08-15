# Publisher P2-21D Delivery-State CLI Exposure Evaluation

Status  : Complete / design evaluation; CLI exposure implementation remains NO-GO until separately authorized
Scope   : Re-evaluate delivery-state diagnostics exposure after P2-21C
Depends : docs/development/Publisher_P2-20_DeliveryStateDiagnosticsCliDecision.md, docs/development/Publisher_P2-21B_DeliveryStateCliDiagnosticsEvaluation.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, src/Publisher/Application/PublishResult.cs, src/Publisher.Cli/Program.cs, tests/unit/Publisher/CliApplicationTests.cs

This is a docs-only / local-only design evaluation. It does not expose
`deliveryState` in CLI output, add `httpStatus`, add a new delivery-state
classification, change retry behavior, mutate Google Docs or Google Drive,
perform OAuth or token-store operations, run Live E2E, update packages or
`dist`, publish releases, create tags, claim vendor clearance, or claim Avast
safety certification.

## 1. Purpose

P2-21D re-evaluates whether Publisher may expose retained delivery state in CLI
structured diagnostics / final summaries after P2-21C.

P2-20 made CLI exposure NO-GO until the Application boundary carried
`RequestDeliveryState?`. P2-21A implemented that Application carrier. P2-21B
then kept direct exposure NO-GO until a publish / CLI result bridge existed.
P2-21C implemented that bridge by carrying nullable
`RequestDeliveryState? DeliveryState` through `PublishError` and into
`CliResult` without emitting it.

## 2. Current State

The current code path now has an internal carrier from Application publish
failure results to the CLI result object:

- `PublishError` can retain nullable `RequestDeliveryState? DeliveryState`;
- `CliResult` can retain nullable `RequestDeliveryState? DeliveryState`;
- publish failures pass `result.Error?.DeliveryState` into `CliResult`;
- existing tests prove carrier propagation and omission from structured output;
- `StructuredPublisherLogger.Summary` and `SUPPORT_SUMMARY` still omit
  `deliveryState`.

The remaining question is not whether the value can be retained. It is whether
the retained value should become a structured diagnostics field.

## 3. Decision

Decision: P2-21D design evaluation is GO, but CLI exposure implementation is
still NO-GO in this task.

The P2-21C bridge satisfies the precondition for a later narrow implementation
decision. A future implementation may be acceptable only as a separately
authorized local-only CLI diagnostics change that emits `deliveryState` from
the existing `CliResult.DeliveryState` carrier under the constraints below.

## 4. Future Implementation Boundary

A future `deliveryState` CLI diagnostics implementation may be acceptable if
it:

- emits `deliveryState` only on final failure summaries;
- emits only when `CliResult.DeliveryState` is non-null;
- emits only the existing `RequestDeliveryState` names `NotSent`, `Sent`, and
  `Unknown`;
- omits `deliveryState` from success summaries;
- omits `deliveryState` from failures that have no Application-carried value;
- updates `SUPPORT_SUMMARY` only as the same bounded failure-summary-only
  value;
- preserves existing classification, exit code, stdout, command syntax, retry
  behavior, safe message, and readback reporting;
- avoids adding `httpStatus`, Google error reasons, raw exceptions, provider
  payloads, document identifiers, URLs, local paths, credentials, OAuth tokens,
  token-store paths, or account identifiers;
- includes focused `CliApplicationTests` coverage for allowed values, omission
  boundaries, `SUPPORT_SUMMARY` behavior, and sensitive-value exclusion.

## 5. Rejected Scope

P2-21D rejects any broader exposure that would:

- treat delivery state as a new failure classification;
- change retryability or retry budget behavior;
- infer delivery state when the carrier is null;
- expose raw transport details such as HTTP status;
- expose delivery state in success summaries, dry-run summaries, verify
  summaries, diff summaries, usage failures, configuration failures, input
  failures, or unrelated internal failures;
- imply publication authorization, release clearance, package approval, vendor
  clearance, Avast safety certification, or Verified State promotion.

## 6. Preserved Boundaries

P2-21D preserves:

- P2-21A Application carrier behavior;
- P2-21C publish / CLI result carrier behavior;
- existing `RequestDeliveryState` vocabulary;
- existing retry and failure classification;
- existing CLI stdout, exit codes, command syntax, and safe messages;
- existing structured stderr behavior in this task;
- Frozen specifications, public APIs, and persisted schemas;
- Google Docs / Drive, OAuth, token-store, and Live E2E gates;
- package, `dist`, release, tag, and publication gates;
- Avast pending, vendor-clearance not obtained, and Avast safety certification
  not claimed boundaries.

## 7. Decision Summary

P2-21D confirms that the P2-21A through P2-21C carrier path is complete enough
to support a future narrow `deliveryState` final failure summary field.

This evaluation does not implement that field. Actual CLI exposure remains a
later, separately authorized implementation decision.
