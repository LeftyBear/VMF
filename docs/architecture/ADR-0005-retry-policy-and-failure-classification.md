# ADR-0005: Retry Policy and Failure Classification

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher failure classification, retry eligibility, transient handling, exit-code relationship, backoff policy, and safe message boundary
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md, docs/development/Publisher_Phase4-2-3_RetryPolicySpecification.md

## Context

Publisher operations can fail before a request is sent, after a request is
sent, during post-apply readback, during verified-state comparison, or before
the operation is validly configured. The same provider status can therefore
have different safety consequences depending on delivery certainty and
idempotency.

Phase 4-2-2 records the CLI error handling and exit-code behavior. Phase
4-2-3 records the retry policy specification. This ADR records the adopted
architecture decision for failure classification and retry judgment. It does
not change existing specifications, production code, public APIs, persisted
schemas, canonical formats, tests, runbooks, release records, or verification
evidence.

ADR-0004 governs update safety: Verified State, revision conflicts,
differential-update ordering, readback verification, and state promotion.
ADR-0005 governs what happens after a failure is observed: whether the failure
may be retried, must stop, must be classified as verification/recovery
required, or must be reported as configuration, cancellation, input, transient,
or internal failure.

## Decision

Publisher retry decisions are delivery-state aware and idempotency bounded.
Only failures that are retryable by cause, definitely not sent, and safe under
the operation's idempotency rules may be retried automatically.

Retryable classification is limited to transient external-service or transport
conditions such as HTTP 429, 500, 502, 503, 504, retryable Google API failures,
and timeout-like transport failures when the request is definitely `NotSent`.
Retryable cause alone is insufficient. If a retryable cause has `Sent` or
`Unknown` delivery state, the same request must not be resent automatically.

Non-retryable failures include authentication failure, permission failure,
validation failure, malformed request, unsupported operation, non-retryable
4xx, stable input errors, configuration errors, internal invariant failures,
and caller cancellation. `OperationCanceled` and caller cancellation are not
retry candidates.

Verification failures are not transient retry candidates. Readback mismatch,
state verification mismatch, managed-region mismatch, revision conflict,
post-apply verification failure, and any failure that cannot prove whether a
sent or unknown-delivery mutation applied must stop for readback, recovery,
fresh replan, or operator decision according to the owning layer.

Revision conflict is never retried by resending the same request. The safe
path is to abort the current operation and replan from a fresh verified
snapshot only when a later authorized workflow reaches that step.

## Failure Classification

| Classification | Automatic Retry | Required Handling |
| --- | --- | --- |
| `RetryableNotSent` | Allowed within bounded policy. | Retry only when idempotency and authorization boundaries are satisfied. |
| `RetryableSent` | Not allowed. | Treat as verification or recovery required; do not resend the same request. |
| `RetryableUnknownDelivery` | Not allowed. | Treat as verification or recovery required; do not infer success or retry. |
| `NonRetryable` | Not allowed. | Return the mapped input, configuration, permission, authentication, or internal failure. |
| `VerificationFailure` | Not allowed. | Safe stop, readback/recovery, fresh replan, or operator decision. |
| `Canceled` | Not allowed. | Stop promptly and report cancellation. |

Transient failure means a retryable external-service or network condition
occurred without confirmed successful mutation and without confirmed delivery.
It does not include verification mismatch, revision conflict, configuration
error, cancellation, malformed input, authentication or permission failure, or
unknown delivery state.

## Exit-Code Relationship

Retry policy feeds the final Phase 4-2-2 CLI classification after retry
processing. It does not introduce new public exit codes.

| Final Outcome | CLI Classification | Exit Code |
| --- | --- | --- |
| Success within retry budget | `None` | `0` |
| Safe retryable `NotSent` attempts exhausted | `Transient` | `75` |
| Sent or unknown delivery after retryable cause | `Verification` | `4` |
| Readback, state verification, or revision conflict failure | `Verification` | `4` |
| Authentication, permission, or local configuration failure | `Configuration` or existing mapped class | `3` or mapped exit |
| Invalid user input or unsupported content | `Input` | `1` |
| Caller cancellation or `OperationCanceled` | `Canceled` | `130` |
| Unknown metadata, blank error code, or impossible state | `Internal` | `1` |

Unknown or blank stable error codes must not be treated as transient. They
remain aligned with the existing fallback policy: classify as `Internal` and
return the existing mapped internal failure behavior.

## Idempotency And Backoff

Automatic retry is limited to operations whose repeat execution is safe under
the existing contract. The request payload, operation order, preconditions,
revision assumptions, managed-region boundary, and durable state promotion
rules must remain unchanged across attempts. No durable Verified State may be
promoted for a failed attempt.

Backoff must be bounded. The adopted policy is finite retry attempts,
exponential backoff with a configured maximum delay, `Retry-After` honored when
provider-supplied and safe, deterministic timing in tests, and cancellation
checked before each attempt and before each retry delay. Unit tests must not
depend on real sleeps.

Retries must not cross release or external-service authorization boundaries.
A retry policy decision does not authorize Live E2E, Google Docs or Google
Drive mutation, token-store mutation, package work, release publication, tag
creation, flagged executable execution, or vendor submission.

## Safe Message Policy

Publisher retry and failure reporting must emit stable, safe summary messages.
User-facing output, structured diagnostics, evidence records, and release
records must not expose:

- raw exception text;
- stack traces;
- raw HTTP bodies or provider response bodies;
- tokens, secrets, cookies, authorization headers, credentials, private keys,
  credential paths, or token-store paths;
- private Google Docs or Google Drive URLs;
- temporary public URLs or temporary resource identifiers unless an existing
  publish-success compatibility field explicitly requires a safe value;
- local absolute paths;
- namespace-qualified exception details that reveal implementation internals.

Safe diagnostics may include stable error codes, safe fixed messages, numeric
HTTP status, normalized command, phase, operation, retry attempt count,
maximum attempts, retry delay, and stable delivery state.

## Relationship To Other ADRs

ADR-0001 governs how this ADR is recorded, indexed, accepted, and later
superseded. ADR-0005 follows that process and remains subordinate to higher
priority specifications.

ADR-0003 records the release gate and vendor-clearance boundary. ADR-0005 does
not reopen the release gate, resolve Avast false-positive handling, obtain
vendor clearance, authorize release, authorize Live E2E, or authorize Google
Docs / Drive mutation.

ADR-0004 records update safety, including Verified State, revision conflict
hard stops, differential-update ordering, readback verification, and
post-verification-only state promotion. ADR-0005 records failure-time retry
classification and does not weaken ADR-0004's safe-stop requirements.

Phase 4-2-2 and Phase 4-2-3 development specifications remain the detailed
records for error handling and retry policy. ADR-0005 records the durable
architecture decision and points reviewers back to those records for
phase-specific detail and evidence.

## Consequences

Publisher retry behavior is conservative by default. The system does not
repeat a request merely because the provider status is temporary-looking.
Delivery certainty and idempotency are required before automatic retry.

Verification failure, revision conflict, configuration error, unknown delivery,
unknown error metadata, blank stable codes, and cancellation all stop automatic
retry. This preserves document mutation safety and avoids converting uncertain
external state into a false success.

Future implementation, review, and recovery work must preserve safe summaries,
bounded backoff, cancellation behavior, existing exit-code mappings, and the
separation between retry policy and release authorization.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Proposed | Initial retry policy and failure classification ADR drafted as docs-only / local-only documentation. |
| 2026-08-05 | Accepted | Retry policy and failure classification accepted as the durable failure-time retry decision. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0001-architecture-decision-record-process.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md`
- `docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md`
- `docs/development/Publisher_Phase4-2-3_RetryPolicySpecification.md`
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
- This ADR does not replace implementation specifications, Phase 4-2-2 or
  Phase 4-2-3 development records, runbooks, release records, verification
  evidence, or current status records.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, vendor submission, or
  vendor clearance.
- This ADR does not claim release readiness, Avast false-positive resolution,
  risk acceptance, final release verification, or publication completion.
