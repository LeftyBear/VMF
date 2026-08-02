# Publisher Phase 4-2-3 Retry Policy Specification

Status  : Done
Scope   : Publisher retry policy specification consolidation only
Depends : docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md, docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md, docs/distribution/PublisherReleaseRunbook.md, docs/development/Publisher_TestClassification.md, docs/development/CURRENT_STATUS.md

This document consolidates the VMF Publisher retry policy for Phase 4-2 review.
It is documentation only. It does not modify production code, Frozen
specifications, public APIs, persisted schemas, canonical formats, release
artifacts, packages, tags, publication state, Google Docs, Google Drive, Live
E2E behavior, token stores, credentials, temporary public hosting, or the
Avast-pending package state.

The existing `Phase 4-2-3 Local Verify Report Improvement` implementation
record remains unchanged. This file uses the requested Phase 4-2-3 retry-policy
filename as a specification consolidation record only; it does not renumber or
replace the existing Local Verify Report record.

## 1. Purpose

The purpose of this specification is to make retry behavior explicit,
testable, and consistent with the Phase 4-2 diagnostic logging and error
handling contracts.

The specification covers:

- retryable and non-retryable failure classification;
- relationship to CLI exit codes;
- relationship to stable error codes;
- transient failure handling;
- verification failure non-retry policy;
- idempotency and safe retry conditions;
- backoff and retry count policy;
- CLI behavior;
- structured logging requirements;
- test matrix;
- non-goals;
- release-hold continuation conditions.

## 2. Scope Boundaries

Allowed scope for a future implementation task:

- preserve existing public APIs and contracts;
- update internal retry classification only where existing internal boundaries
  already carry retry metadata;
- update focused Publisher unit tests using fakes, stubs, in-memory gateways,
  captured console output, or deterministic delay collaborators;
- keep verification local-only unless a later task explicitly authorizes a
  narrower external operation.

Excluded scope:

- Frozen specification changes;
- public API changes;
- production behavior changes outside an explicitly authorized retry task;
- persistence schema changes;
- canonical format changes;
- package creation, package update, artifact verification, or `dist/` changes;
- release tag creation, GitHub Release publication, or release announcement;
- Live E2E;
- Google Docs or Google Drive mutation;
- credential, token-store, or temporary public-hosting mutation;
- re-running the Avast-pending flagged executable;
- changing stable error-code names or existing exit-code constants.

## 3. Retry Classification

Publisher retry decisions must classify failures by both cause and delivery
certainty. A retryable cause is not sufficient by itself.

| Classification | Retry Decision | Typical Source |
| --- | --- | --- |
| RetryableNotSent | May retry under bounded policy. | HTTP 429, 500, 502, 503, 504, timeout-like transport failure, or retryable Google API failure where the request was definitely not sent. |
| RetryableSent | Must not retry automatically. | Retryable provider status after a request was sent and mutation outcome is uncertain. |
| RetryableUnknownDelivery | Must not retry automatically. | Timeout, connection loss, cancellation, or provider exception where delivery state cannot be proven. |
| NonRetryable | Must not retry. | Authentication failure, permission failure, validation failure, malformed request, unsupported operation, non-retryable 4xx, stable input/configuration error. |
| VerificationFailure | Must not retry as transient. | Readback mismatch, state verification mismatch, managed-region mismatch, revision conflict, post-apply verification failure. |
| Canceled | Must not retry. | Caller cancellation or Ctrl+C. |

Only `RetryableNotSent` may be retried. `RetryableSent` and
`RetryableUnknownDelivery` must move to readback, recovery, verification
failure, or safe stop according to the owning layer. This preserves document
mutation safety when a batch update may have been partially or fully applied.

## 4. Exit Code Relationship

Retry policy feeds the Phase 4-2-2 CLI classification after the final retry
attempt, not before it.

| Final Outcome | CLI Classification | Exit Code |
| --- | --- | --- |
| Operation succeeds within retry budget | `None` | `0` |
| Retryable `NotSent` attempts are exhausted without confirmed mutation | `Transient` | `75` |
| Retryable cause has `Sent` or `Unknown` delivery state | `Verification` | `4` |
| Post-apply readback or state verification fails | `Verification` | `4` |
| Revision conflict requires replanning | `Verification` | `4` |
| Authentication, permission, or local configuration failure | `Configuration` or existing mapped class | `3` or mapped exit |
| Invalid user input or unsupported content | `Input` | `1` |
| Caller cancellation | `Canceled` | `130` |
| Unknown retry metadata, missing stable code, or impossible state | `Internal` | `1` |

The retry policy must not introduce new public exit codes. Any future
input-specific exit code remains a separate public CLI behavior proposal.

## 5. Stable Error Code Relationship

Stable error codes remain the public diagnostic key. Retry policy may influence
the final classification only when the existing code and delivery state make
that classification safe.

| Stable Code Or Family | Retry Treatment |
| --- | --- |
| `TRANSIENT_ERROR` or retryable Google API / transport status | Retry only when delivery state is definitely `NotSent`; exhausted attempts become `Transient`. |
| HTTP 429, 500, 502, 503, 504 | Retryable only under safe idempotency and delivery-state rules. |
| `IMAGE_URI_RESOLUTION_FAILED` | Retryable only for retryable external fetch or resolution failures; otherwise `Input`. |
| `IMAGE_UPLOAD_FAILED` | Retryable only when upload delivery was not confirmed and retry is safe; otherwise `Internal` or verification-oriented failure. |
| `IMAGE_INSERT_FAILED` | `Verification` when insertion/readback cannot prove success; `Transient` only for retryable not-sent failures. |
| `TABLE_CONTENT_UPDATE_FAILED` | `Verification` unless definitely retryable and not sent. |
| `UPDATE_APPLICATION_FAILED` | `Verification` unless wrapped failure is definitely retryable and not sent. |
| `UPDATE_OPERATION_EXECUTOR_FAILED` | Maps to `Transient`, `Verification`, or `Internal` according to wrapped stable cause and delivery state. |
| `UPDATE_READBACK_FAILED` | `Verification` unless definitely retryable and not sent. |
| `UPDATE_READBACK_MISMATCH` | Never retry as transient; classify as `Verification`. |
| `UPDATE_REVISION_CONFLICT` | Never retry same request; require replan from fresh revision. |
| `STATE_VERIFICATION_REQUIRED`, `STATE_VERIFICATION_MISMATCH`, `STATE_INVALID_TRANSITION` | Never retry as transient; classify as `Verification`. |
| `CONFIG_*` | Do not retry. |
| `MARKDOWN_*`, duplicate block ids, invalid image input | Do not retry. |
| Unknown or blank stable code | Do not infer retryability; classify as `Internal`. |

Stable error-code names must not be renamed, removed, or weakened by retry
policy work.

## 6. Transient Failure Handling

Transient failure means a retryable external-service or network condition
occurred without confirmed successful mutation. It does not mean every
temporary-looking failure is safe to repeat.

Required handling:

- retry only when the operation is idempotent or protected by a safe request
  precondition and delivery state is definitely `NotSent`;
- honor cancellation before each attempt and before each retry delay;
- preserve `Retry-After` when supplied by the provider;
- stop after the configured retry budget;
- classify exhausted safe retries as `Transient`;
- classify indeterminate delivery as `Verification` or recovery-required
  rather than `Transient` success or automatic retry.

Retry exhaustion must not be reported as a verification pass, release
readiness, or Live E2E evidence.

## 7. Verification Failure Non-Retry Policy

Verification failures are not transient retry candidates.

Do not retry the same operation when:

- post-apply readback differs from the expected candidate;
- readback cannot establish whether a sent or unknown-delivery request applied;
- managed-region validation fails;
- revision conflict is detected;
- verified-state promotion or comparison fails;
- state schema, document identity, algorithm version, or transition checks
  fail;
- table or image readback proves the expected structure was not created.

The correct action is safe stop, readback/recovery where already designed,
fresh replan from current state, or operator decision. Retrying the same
request can duplicate Google Docs mutations or corrupt the managed region.

## 8. Idempotency And Safe Retry Conditions

An operation is safe to retry only when all applicable conditions are true:

- the previous attempt was definitely not sent, or the operation is explicitly
  idempotent under the existing contract;
- request payload and preconditions are identical across attempts;
- required revision, managed-region boundary, and operation order are
  preserved;
- no durable state has been promoted for the failed attempt;
- cleanup or compensation does not hide an earlier hard failure;
- retry does not require credential, token-store, Google Docs, Google Drive, or
  temporary public-host mutation outside an authorized run;
- cancellation has not been requested.

Physical update execution has the strictest rule: retry only retryable
`NotSent` failures. `Sent` and `Unknown` delivery states are indeterminate and
must not be resent by the executor.

## 9. Backoff And Retry Count Policy

Retry policy must be bounded and deterministic in tests.

Required policy:

- use a small finite maximum attempt count;
- use exponential backoff with a configured maximum delay;
- apply jitter only if tests can make it deterministic or inject the random
  source;
- honor provider `Retry-After` when available and safe;
- do not sleep in unit tests; use an injectable delay collaborator or fake
  clock where the existing design already supports it;
- do not retry indefinitely;
- do not retry authentication, permission, validation, malformed request,
  revision conflict, verification mismatch, configuration, input, internal
  invariant, or cancellation failures.

If existing production settings already define attempt counts or delays, a
future implementation task must preserve those values unless a separate
configuration change is explicitly approved.

## 10. CLI Behavior

The CLI reports only the final classified result after retry processing.

Required CLI behavior:

- successful retry ends with the normal success summary and exit `0`;
- exhausted safe transient retry ends with `TRANSIENT_ERROR` or the existing
  transient stable code, classification `Transient`, and exit `75`;
- indeterminate delivery ends as verification/recovery-required and exit `4`;
- verification mismatch ends with fixed safe verification summary and exit `4`;
- cancellation ends with `CANCELED` and exit `130`;
- raw exception messages, provider bodies, URIs, local paths, tokens, secrets,
  credential paths, token-store paths, stack traces, and temporary resource
  identifiers must not be emitted.

The CLI must not print every retry attempt as user-facing stdout. Structured
diagnostics may record safe attempt metadata to stderr.

## 11. Structured Logging Requirements

Retry diagnostics must follow Phase 4-2-1 safe structured logging rules.

Required safe fields for retry-related structured events:

| Field | Requirement |
| --- | --- |
| `timestampUtc` | Required standard field. |
| `level` | `warning` for retry attempt diagnostics; `error` for final failure summary. |
| `sessionId` | Required standard field; must not contain user input. |
| `command` | Normalized CLI command. |
| `phase` | Existing stable phase, such as `publish`, `executor`, `verification`, or `diagnostic`. |
| `operation` | Existing stable operation; do not invent misleading operation names. |
| `code` | Stable retry, warning, or final failure code. |
| `message` | Safe fixed message. |
| `classification` | Final summary only, or attempt classification when safe and stable. |
| `attempt` | Optional numeric attempt index. |
| `maxAttempts` | Optional numeric retry budget. |
| `retryAfterMilliseconds` | Optional safe numeric delay value. |
| `deliveryState` | Optional stable value such as `NotSent`, `Sent`, or `Unknown`. |

Structured logs must not contain:

- raw provider message or response body;
- document URL or temporary public URL, except existing publish-success
  compatibility fields;
- Google Drive file id for temporary images;
- local file paths;
- token, secret, credential, cookie, or authorization header;
- stack trace or namespace-qualified exception detail.

## 12. Test Matrix

A future implementation task must use local-only tests unless separately
authorized.

| Viewpoint | Required Assertion |
| --- | --- |
| retryable not-sent | HTTP 429/500/502/503/504 or equivalent retryable failure with `NotSent` delivery retries within the bounded policy. |
| exhausted transient | Exhausted retryable `NotSent` attempts return classification `Transient`, exit `75`, and a safe summary. |
| retry-after | Provider retry-after is honored without real sleeps in unit tests. |
| sent delivery | Retryable failure with `Sent` delivery is not retried and becomes verification/recovery-required. |
| unknown delivery | Retryable failure with unknown delivery is not retried and becomes verification/recovery-required. |
| non-retryable 4xx | Authentication, permission, validation, and malformed request failures are not retried. |
| verification failure | Readback mismatch, state verification mismatch, and revision conflict are not retried. |
| cancellation | Cancellation before attempt or during delay stops retry and returns `Canceled` where owned by CLI. |
| safe diagnostics | Retry logs contain stable fields and omit URIs, paths, tokens, secrets, provider bodies, and raw exception messages. |
| deterministic timing | Unit tests use fake delay or equivalent deterministic timing. |
| stable codes | Existing stable error codes and exit-code mappings are preserved. |

Tests must not enable `VMF_PUBLISHER_GOOGLE_E2E`, mutate Google Docs or Google
Drive, create or update packages, run flagged executables, create tags, publish
artifacts, or push.

## 13. Non-Goals

This specification does not:

- add or change public CLI exit codes;
- introduce a public retry configuration API;
- change stable error-code names;
- change Google Docs request mapping;
- change verified-state schema;
- change public logging contracts;
- authorize Live E2E;
- authorize Google Docs or Google Drive mutation;
- authorize package or release operations;
- authorize flagged executable smoke testing;
- resolve the Avast false-positive hold.

## 14. Release Hold Continuation Conditions

This retry-policy consolidation does not change the formal Publisher state:

`Phase 4 local-only verification complete / release blocked`.

The release hold continues until the approved release process records all
required decisions. At minimum, the following remain unresolved unless a later
authorized record says otherwise:

- Avast false-positive handling or owner-approved exception basis;
- release gate reopening;
- Live E2E authorization or owner-approved N/A decision;
- Google Docs and Google Drive mutation authorization;
- selected release candidate artifact identity and package audit;
- security and supply-chain review;
- repository-owner go/no-go approval;
- tag, release, publication, and push authorization.

This document must not be used as release approval, package approval, Live E2E
evidence, Google Docs readback evidence, Google Drive cleanup evidence,
antivirus vendor clearance, or permission to re-run the flagged executable.

## 15. Acceptance Criteria

This specification is complete when:

- retryable and non-retryable classifications are recorded;
- exit-code and stable-code relationships are explicit;
- transient failure handling is delivery-state aware;
- verification failures are explicitly non-retryable;
- idempotency and safe retry conditions are defined;
- backoff and retry-count policy is bounded;
- CLI behavior and structured logging requirements are safe-message aligned;
- local-only test matrix is listed;
- non-goals and release-hold continuation conditions are explicit;
- no production code, tests, Frozen specifications, public APIs, release
  artifacts, Google Docs, Google Drive, Live E2E state, package state, tagged
  release state, or Avast-pending executable state are changed by this
  documentation-only task.
