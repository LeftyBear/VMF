# ADR-0007: Error Handling and Failure Classification

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher CLI error classification, exit-code mapping, safe user-facing messages, stable error-code fallback, cancellation propagation, and relationship to retry and diagnostic logging decisions
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md

## Context

Publisher Phase 4-2-2 defined and implemented local CLI error handling for
Publisher command execution. The detailed specification and final review are
recorded in
`docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md`.

This ADR records the durable architecture decision for the accepted Publisher
error handling behavior. It fixes the existing specification and implemented
behavior as an Accepted ADR. It does not introduce new runtime behavior,
change production code, change tests, alter public APIs, modify Frozen
Specifications, update persisted schemas, create packages, write distribution
artifacts, or change release records beyond documentation status tracking.

ADR-0005 records retry policy and failure-time retry judgment. ADR-0006
records diagnostic logging and safe observability. ADR-0007 records the CLI
boundary behavior after Publisher failures are classified for process results
and user-facing output.

## Decision

Publisher CLI owns final failure classification, process exit-code conversion,
and classification-based user-facing summary messages.

The adopted CLI classifications are:

- `None`;
- `Input`;
- `Internal`;
- `Usage`;
- `Configuration`;
- `Verification`;
- `Transient`;
- `Canceled`.

Stable Publisher error codes and known exception categories are mapped to
those classifications at the CLI boundary. Unknown, blank, or missing stable
error codes are not treated as transient and do not leak raw failure details.
They fall back to `Internal` classification and the existing generic publish
failure exit behavior.

The CLI exit-code mapping is:

| Classification | Exit Constant | Exit Code |
| --- | --- | --- |
| `None` | `ExitSuccess` | `0` |
| `Input` | `ExitPublishFailed` | `1` |
| `Internal` | `ExitPublishFailed` | `1` |
| `Usage` | `ExitUsage` | `2` |
| `Configuration` | `ExitConfiguration` | `3` |
| `Verification` | `ExitVerification` | `4` |
| `Transient` | `ExitTransient` | `75` |
| `Canceled` | `ExitCanceled` | `130` |

Verification failure returns exit code `4`. This includes readback mismatch,
readback failure where the outcome cannot be treated as definitely not sent,
managed-region validation failure, revision conflict, missing verified-state
verification, and verified-state mismatch.

Transient failure returns exit code `75` only for retryable external-service or
network conditions observed without confirmed successful mutation and after
retry policy processing. Retry eligibility, delivery-state requirements,
idempotency boundaries, and bounded backoff are governed by ADR-0005.

Cancellation returns exit code `130`. `OperationCanceledException` must be
re-thrown through lower layers and handled only at the CLI boundary or an
owning test harness boundary that converts it into a process result.

## Safe Message Boundary

Publisher separates stable error codes from safe user-facing messages. Stable
codes are used for diagnostics, tests, evidence review, and operator
classification. User-facing summaries are fixed, classification-based safe
messages.

The CLI must not emit raw `PublishResult.Error.Message`, raw exception
messages, stack traces, provider response bodies, raw HTTP request or response
bodies, local paths, credential paths, token-store paths, private URLs,
temporary public URLs, tokens, secrets, authorization headers, cookies, private
keys, invalid input payloads, or implementation-specific exception details in
user-facing output or structured diagnostics.

Raw exception messages are not a user-facing output contract. Safe summaries
and stable codes are the contract.

Diagnostic logging and redaction requirements remain governed by ADR-0006.

## Layer Boundaries

Domain, Application, and Infrastructure layers may expose stable codes and
safe exception types, but they must not depend on CLI exit codes or final CLI
classifications.

Lower layers must preserve cancellation semantics. They must not catch and
convert `OperationCanceledException` into an internal failure before it reaches
the CLI boundary.

Infrastructure must preserve delivery-state information for physical update
failures so that retry policy and final CLI classification can distinguish
definitely-not-sent transient failures from sent or unknown-delivery
verification/recovery cases.

## Relationship To Other ADRs And Records

ADR-0001 governs how this ADR is recorded, indexed, accepted, and later
superseded.

ADR-0003 records the release gate and vendor-clearance boundary. ADR-0007 does
not reopen the release gate, resolve Avast false-positive handling, obtain
vendor clearance, accept antivirus risk, authorize release, authorize package
work, authorize Live E2E, or authorize Google Docs / Drive mutation.

ADR-0005 governs retry policy. ADR-0007 relies on ADR-0005 for retry
eligibility, delivery-state aware transient handling, idempotency limits, and
bounded backoff. ADR-0007 records the final CLI classification and exit-code
surface after that policy is applied.

ADR-0006 governs diagnostic logging and safe observability. ADR-0007 preserves
ADR-0006's structured diagnostic and redaction boundary while recording the CLI
classification and user-facing summary behavior.

`docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md` remains
the detailed error handling specification, implementation plan, final review
record, and local-only verification evidence. This ADR records the durable
architecture decision and points reviewers back to that document for
phase-specific detail.

## Consequences

Publisher CLI failure behavior remains stable, testable, and conservative.
Verification failures are separated from transient failures, cancellation is
preserved, unknown metadata falls back to internal handling, and user-facing
output remains safe.

Future error handling changes must preserve the stable error-code and safe
message separation unless a later ADR supersedes this decision. Any future
public exit-code change, including a separate input-specific exit code, must
be proposed separately because it changes the public CLI behavior surface.

This ADR preserves the current release boundary. Release remains blocked,
Avast false-positive handling remains pending, and vendor clearance has not
been obtained.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Accepted | Error handling and failure classification accepted as the durable CLI failure classification and exit-code decision. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0001-architecture-decision-record-process.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0005-retry-policy-and-failure-classification.md`
- `docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md`
- `docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md`
- `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`
- `docs/development/CURRENT_STATUS.md`

## Replacement

This ADR does not supersede an earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify implementation behavior.
- This ADR does not modify tests.
- This ADR does not modify stable error-code names or public exit-code
  constants.
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not replace implementation specifications, Phase 4-2-2
  development records, runbooks, release records, verification evidence, or
  current status records.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, vendor submission, or
  vendor clearance.
- This ADR does not claim release readiness, Avast false-positive resolution,
  risk acceptance, final release verification, or publication completion.
