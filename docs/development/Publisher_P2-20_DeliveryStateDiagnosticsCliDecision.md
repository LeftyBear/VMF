# Publisher P2-20 Delivery-State Diagnostics CLI Decision

Status  : Complete / NO-GO for CLI exposure before Application carrier
Scope   : Decide whether delivery-state diagnostics should be exposed by the CLI now
Depends : docs/development/Publisher_P2-02_AdditionalDiagnosticsEvaluation.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, src/Publisher/Application/PhysicalUpdateExecution.cs

This is a docs-only / local-only decision record. It does not expose
`deliveryState` in CLI output, add a new delivery-state classification, mutate
Google Docs or Google Drive, perform OAuth or token-store operations, run Live
E2E, update packages or `dist`, publish releases, create tags, claim vendor
clearance, or claim Avast safety certification.

## 1. Purpose

P2-20 decides whether Publisher should expose delivery-state diagnostics at the
CLI boundary immediately.

The decision follows earlier diagnostic work where `deliveryState` remained
deferred. Retry classification already depends on existing
`RequestDeliveryState` values, but CLI exposure requires a stable upstream
carrier before any operator-facing or machine-readable output can be safely
defined.

## 2. Decision

Decision: NO-GO for CLI exposure now.

Delivery-state diagnostics must not be added to CLI public output until the
Application boundary first carries `RequestDeliveryState?` through physical
update results without changing classification.

## 3. Required Precondition

Before any CLI exposure is reconsidered, Publisher must establish an
Application boundary carrier:

- use the existing `RequestDeliveryState` enum;
- add nullable `RequestDeliveryState?` carrier fields to physical update result
  types;
- propagate existing `NotSent`, `Sent`, and `Unknown` values without
  reclassification;
- avoid adding new delivery-state vocabulary or a new failure taxonomy;
- prove the carrier with focused unit tests.

This precondition is implemented separately by P2-21A.

## 4. Rationale

CLI output is harder to change than internal result transport. Exposing
delivery state before the Application boundary is stable would risk:

- treating delivery state as a new classification;
- exposing transport details without a durable carrier contract;
- confusing retry diagnostics with publication, readback, or verification
  status;
- expanding the public diagnostic surface before value-safe behavior is proven.

## 5. Preserved Boundaries

P2-20 preserves:

- existing CLI stdout, stderr event names, exit codes, and classifications;
- existing retry behavior for `NotSent`, `Sent`, and `Unknown`;
- existing `RequestDeliveryState` enum vocabulary;
- Frozen specifications, public APIs, and persisted schemas until a separate
  implementation scope explicitly changes them;
- Google Docs / Drive, OAuth, token-store, and Live E2E gates;
- package, `dist`, release, tag, and publication gates;
- Avast pending, vendor-clearance not obtained, and Avast safety certification
  not claimed boundaries.

## 6. Non-Goals

P2-20 does not:

- add `deliveryState` to CLI output;
- add `httpStatus` to CLI output;
- create new delivery-state classifications;
- change retry policy or failure classification;
- change command syntax or stdout;
- authorize Google Docs / Drive mutation, OAuth/token-store work, Live E2E,
  package generation, release, publication, tag, Avast, flagged-executable, or
  vendor-clearance operations.

## 7. Decision Summary

Delivery-state diagnostics CLI exposure is still NO-GO. The next accepted step
is to establish a nullable `RequestDeliveryState?` carrier at the Application
boundary first, then evaluate any future CLI exposure under a separate scoped
task.
