# Publisher P2-21B Delivery-State CLI Diagnostics Evaluation

Status  : Complete / design evaluation; direct CLI exposure remains NO-GO until result bridge
Scope   : Re-evaluate delivery-state diagnostics CLI exposure after P2-21A
Depends : docs/development/Publisher_P2-20_DeliveryStateDiagnosticsCliDecision.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, src/Publisher/Application/PhysicalUpdateExecution.cs, src/Publisher/Application/PublishResult.cs, src/Publisher.Cli/Program.cs

This is a docs-only / local-only design evaluation. It does not expose
`deliveryState` in CLI output, add `httpStatus`, add a new delivery-state
classification, change retry behavior, mutate Google Docs or Google Drive,
perform OAuth or token-store operations, run Live E2E, update packages or
`dist`, publish releases, create tags, claim vendor clearance, or claim Avast
safety certification.

## 1. Purpose

P2-21B re-evaluates whether delivery-state diagnostics may now be exposed at
the CLI boundary.

P2-20 made CLI exposure NO-GO until Publisher first established a stable
Application boundary carrier. P2-21A completed that precondition by adding
nullable `RequestDeliveryState? DeliveryState` carrier fields to
`ApplyResult` and `PhysicalUpdateExecutionResult` and by propagating existing
`NotSent`, `Sent`, and `Unknown` values without changing classification.

## 2. Current State

P2-21A establishes delivery-state transport at the physical update result
boundary only.

Current CLI publish flow still uses:

- `PublishResult`, which exposes success document fields or `PublishError`;
- `PublishError`, which exposes stable error code and safe message only;
- `CliResult`, which drives structured stderr final summaries;
- `StructuredPublisherLogger.Summary`, which currently emits safe retry,
  failure-boundary, readback, support-summary, and clearance-boundary fields.

There is no current carrier from `PhysicalUpdateExecutionResult.DeliveryState`
through `PhysicalUpdateApplicationResult`, transaction / publish result
conversion, `PublishResult`, `PublishError`, or `CliResult` into the CLI final
summary.

## 3. Decision

Decision: direct CLI exposure remains NO-GO for immediate implementation.

The P2-20 precondition is satisfied, but the current result path does not yet
provide a value-safe, classification-neutral bridge from Application physical
update results to CLI summaries.

The next acceptable implementation scope is not direct CLI output. It is a
narrow result-bridge implementation that carries `RequestDeliveryState?` from
the Application publish boundary to `CliResult` without emitting it. Only after
that bridge exists and is tested should CLI output exposure be reconsidered.

## 4. Required Future Bridge

A future P2-21C-style bridge may be acceptable if it:

- reuses the existing `RequestDeliveryState` enum;
- carries nullable `RequestDeliveryState?` without changing classification,
  exit code, stdout, retry behavior, or safe message;
- keeps success summaries, dry-run summaries, verify summaries, diff summaries,
  usage failures, configuration failures, input failures, and unrelated
  internal failures from inventing delivery-state values;
- preserves omission when delivery state is unknown to the CLI path;
- avoids adding `httpStatus`;
- avoids adding new public command syntax;
- includes focused unit coverage proving omission and propagation boundaries.

The bridge should be implemented before any structured stderr field is added.

## 5. CLI Exposure Criteria

If a later task reconsiders actual CLI output exposure, it must satisfy all of
these criteria:

- emit `deliveryState` only on final failure summaries;
- emit only when the value came from the existing Application carrier;
- allow only `NotSent`, `Sent`, and `Unknown`;
- omit the field from success summaries and from failures without a known
  delivery-state carrier;
- preserve existing classification and exit code for `NotSent`, `Sent`, and
  `Unknown`;
- keep `deliveryState` separate from retryability, readback status,
  publication authorization, release clearance, vendor clearance, and Avast
  safety certification;
- update `SUPPORT_SUMMARY` only if it reuses the same bounded field and remains
  failure-summary-only;
- add focused `CliApplicationTests` coverage for allowed values, omission
  boundaries, support-summary behavior, and sensitive-value exclusion.

## 6. Safe-Value Boundary

Allowed value set for any future CLI field:

- `NotSent`;
- `Sent`;
- `Unknown`.

Prohibited values and payloads:

- raw HTTP status or `httpStatus`;
- Google error reason unless already exposed through existing safe stable code;
- raw exception messages;
- stack traces;
- HTTP request or response bodies;
- provider payloads;
- document IDs or URLs beyond already existing success-only compatibility
  fields;
- local paths, private URLs, temporary URLs, credentials, token-store paths,
  OAuth tokens, cookies, Authorization headers, usernames, hostnames, or
  account identifiers;
- release, package, vendor-clearance, Avast, or publication authorization
  claims.

## 7. Preserved Boundaries

P2-21B preserves:

- P2-21A Application carrier behavior;
- existing `RequestDeliveryState` vocabulary;
- existing retry and failure classification;
- existing CLI stdout, exit codes, command syntax, and safe messages;
- existing structured stderr behavior until a separate implementation task;
- Frozen specifications, public APIs, and persisted schemas;
- Google Docs / Drive, OAuth, token-store, and Live E2E gates;
- package, `dist`, release, tag, and publication gates;
- Avast pending, vendor-clearance not obtained, and Avast safety certification
  not claimed boundaries.

## 8. Decision Summary

P2-21B confirms that P2-21A satisfied the Application carrier precondition, but
direct CLI exposure is still NO-GO because the current publish / CLI result
path does not yet carry delivery state safely to final summaries.

The next narrow GO candidate is a carrier bridge from Application publish
results to `CliResult`, with no CLI output change. Actual CLI exposure remains
a later, separately authorized implementation decision.
