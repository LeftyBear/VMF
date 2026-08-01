# Publisher Phase 4-2-2 Error Handling Specification

Status  : Done
Scope   : Publisher CLI error classification, safe messages, exit code mapping, retry policy notes, and Task 3 implementation plan
Depends : docs/development/Publisher_Phase4_Planning.md, docs/development/Publisher_Phase4-1_DesignNotes.md, docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md, docs/development/Publisher_Phase4_LocalVerificationPlan.md

This document defines the Publisher Phase 4-2-2 error handling specification
and records the final Task 4 review result. The specification task was
documentation only. Task 3 then implemented the local Publisher error handling
changes, and Task 4 reviewed and verified them. No Frozen specifications,
public APIs, persisted schemas, canonical formats, release artifacts, Google
Docs, Google Drive, Live E2E behavior, token stores, temporary public hosting,
or the Avast-pending package state were changed.

Task 3 implementation preserved existing public contracts and implemented
classification, exit conversion, safe CLI output, and required safe
Application-layer exception conversion without changing retry policy.

## 1. Purpose

The purpose of this specification is to make Publisher CLI failures stable,
safe, and testable before Task 3 implementation.

The specification covers:

- error classification;
- exit code mapping;
- verification and readback mismatch exit behavior;
- safe message rules;
- stable error code mapping;
- retry policy boundaries;
- Application, Infrastructure, and CLI responsibility boundaries;
- Task 3 test matrix and implementation plan.

## 2. Scope Boundaries

Allowed scope for Task 3:

- modify `src/Publisher.Cli/Program.cs`;
- modify focused Publisher unit tests under `tests/unit/Publisher/`;
- add internal CLI-only helpers when needed;
- classify existing Publisher failures without changing public APIs;
- normalize CLI summary and structured diagnostic messages;
- keep verification local-only.

Excluded scope:

- Frozen specification changes;
- public API changes;
- production code outside narrowly required CLI-internal conversion;
- tests outside the focused Publisher unit scope unless Task 3 evidence shows
  they are directly affected;
- retry behavior changes in Google Docs clients, temporary image hosting, or
  physical update execution;
- release artifact generation, replacement, verification, or publication;
- Live E2E;
- Google Docs or Google Drive mutation;
- token-store, credential, or temporary public hosting mutation;
- Avast-pending executable or package execution;
- signing, installer, apphost, package-trust, or release-process changes.

## 3. Error Classification

Publisher CLI must use the following stable classifications.

| Classification | Meaning | Typical Source |
| --- | --- | --- |
| `None` | Command completed successfully. | Successful `help`, `verify`, `diff`, `dry-run`, or `publish`. |
| `Usage` | Command shape or argument count is invalid. | Unknown command, missing command argument, extra command argument. |
| `Configuration` | Local configuration or required setting is missing or invalid. | `CONFIG_*` codes, CLI settings validation, publish settings validation. |
| `Input` | User-provided Markdown or publish input is invalid before an external write is attempted. | `MARKDOWN_*`, image-source validation, duplicate block identifiers, unsupported input content. |
| `Verification` | Post-apply verification, readback, managed-region, revision, or verified-state consistency failed. | `UPDATE_READBACK_MISMATCH`, `UPDATE_READBACK_FAILED`, `STATE_VERIFICATION_MISMATCH`, `STATE_VERIFICATION_REQUIRED`. |
| `Transient` | Retryable external-service or network condition was observed without a confirmed successful mutation. | HTTP 429, 500, 502, 503, 504; retryable Google API failures; timeout-like stable codes. |
| `Canceled` | Caller cancellation or Ctrl+C canceled the operation. | `OperationCanceledException`. |
| `Internal` | Unexpected Publisher failure, unknown stable code, invariant violation, or implementation defect. | Unknown error code, unclassified exception, impossible state. |

Classification is a CLI concern. Domain, Application, and Infrastructure code
may expose stable codes or safe exceptions, but the CLI owns final
classification and exit conversion.

## 4. Exit Code Mapping

Exit codes must map one-to-one from classification, except that `Input` and
`Internal` intentionally share the generic publish failure code `1` because the
current public CLI exit surface does not allocate a separate input code.

| Classification | Exit Constant | Exit Code | CLI Summary Code Guidance |
| --- | --- | --- | --- |
| `None` | `ExitSuccess` | `0` | Success code such as `VERIFY_SUCCEEDED`, `DRY_RUN_SUCCEEDED`, `DIFF_SUCCEEDED`, or `PUBLISH_SUCCEEDED`. |
| `Input` | `ExitPublishFailed` | `1` | Existing stable input error code. |
| `Internal` | `ExitPublishFailed` | `1` | `PUBLISHER_ERROR` or existing stable internal error code. |
| `Usage` | `ExitUsage` | `2` | `USAGE_ERROR`. |
| `Configuration` | `ExitConfiguration` | `3` | Existing `CONFIG_*` code. |
| `Verification` | `ExitVerification` | `4` | Existing verification, readback, state, revision, or managed-region code. |
| `Transient` | `ExitTransient` | `75` | `TRANSIENT_ERROR` or existing transient stable code. |
| `Canceled` | `ExitCanceled` | `130` | `CANCELED`. |

Task 3 did not introduce new public exit codes. If a future task needs a
separate exit code for `Input`, that must be proposed separately because it
would change the public CLI behavior surface.

### Verification Exit Conditions

CLI must return `ExitVerification = 4` when the stable code or exception
represents a verification/readback failure after the requested operation has
progressed beyond local input validation.

Assign `ExitVerification = 4` when any of the following is true:

- post-apply readback differs from the candidate document;
- post-apply readback cannot be obtained and the operation outcome cannot be
  treated as a pure transient not-sent failure;
- managed-region validation fails during physical update verification;
- revision conflict is detected during update planning or physical execution;
- verified-state lifecycle requires verification that is missing;
- verified-state lifecycle compares actual state with expected state and finds
  a mismatch.

Do not assign `ExitVerification = 4` for:

- local usage errors before command execution;
- local configuration errors before publish setup;
- local Markdown parse or image input validation failures before external
  mutation;
- retryable transient failures whose delivery state is definitely `NotSent`;
- caller cancellation.

## 5. Safe Message Policy

CLI final summary messages must be classification-based fixed messages. The
CLI must not echo `PublishResult.Error.Message` directly to stdout, stderr, or
structured diagnostic `message` fields.

Required fixed summary messages:

| Classification | Summary Message |
| --- | --- |
| `None` | Existing stable success message. |
| `Usage` | Stable usage failure message that does not echo path-like or secret-like input. |
| `Configuration` | `Publisher configuration is invalid.` |
| `Input` | `Publisher input is invalid.` |
| `Verification` | `Publisher verification failed.` |
| `Transient` | `A transient external service error occurred.` |
| `Canceled` | `Operation was canceled.` |
| `Internal` | `An internal Publisher error occurred.` |

The following values must never be emitted in CLI summary messages or
structured diagnostic messages:

- raw `PublishResult.Error.Message`;
- raw `exception.Message`;
- HTTP request body or response body;
- URI, except the existing publish-success compatibility `documentUrl` field
  defined by Phase 4-2-1;
- local file path, temp path, credential path, token-store path, or package
  path;
- access token, refresh token, private key, authorization header, cookie,
  bearer value, client secret, or credential JSON;
- raw inner exception message;
- stack trace.

`exceptionType` may be emitted only as the simple runtime type name, for
example `HttpRequestException`. It must not include a namespace-qualified type,
stack trace, message text, provider payload, URI, path, token, or secret.

Task 3 should implement safe messages as an allow-list keyed by
classification. Pattern redaction may exist only as a defensive backup.

## 6. Stable Error Code Mapping

Stable code mapping must use the existing public code catalogs without
renaming or deleting codes.

### PublishErrorCodes

| Code Family | Classification |
| --- | --- |
| `MARKDOWN_*` | `Input` |
| `BLOCK_*_DUPLICATE` | `Input` |
| `IMAGE_SOURCE_EMPTY` | `Input` |
| `IMAGE_FILE_NOT_FOUND` | `Input` |
| `IMAGE_PATH_INVALID` | `Input` |
| `IMAGE_FORMAT_NOT_SUPPORTED` | `Input` |
| `IMAGE_REMOTE_URI_INVALID` | `Input` |
| `IMAGE_REMOTE_HOST_NOT_ALLOWED` | `Input` |
| `IMAGE_URI_RESOLUTION_FAILED` | `Transient` when caused by retryable network resolution failure; otherwise `Input`. |
| `IMAGE_METADATA_READ_FAILED` | `Input` for invalid or unreadable local input; `Transient` only when the wrapped failure is retryable external fetch failure. |
| `IMAGE_SIZE_INVALID` | `Input` |
| `IMAGE_UPLOAD_FAILED` | `Transient` when delivery is not confirmed; otherwise `Internal`. |
| `IMAGE_PUBLIC_ACCESS_DENIED` | `Configuration` when temporary public hosting is not allowed or cannot be configured safely; otherwise `Verification`. |
| `IMAGE_INSERT_FAILED` | `Verification` when apply/readback cannot prove the inserted image; `Transient` only for retryable not-sent failures. |
| `IMAGE_NOT_FOUND_AFTER_INSERT` | `Verification` |
| `IMAGE_ALT_TEXT_UPDATE_FAILED` | Warning only; if promoted to failure, `Verification`. |
| `IMAGE_FOLLOWING_INDEX_NOT_FOUND` | `Verification` |
| `IMAGE_TEMP_FILE_DELETE_FAILED` | Warning only; if promoted to failure, `Verification` unless definitely retryable and not sent. |
| `TABLE_NOT_FOUND_AFTER_INSERT` | `Verification` |
| `TABLE_DIMENSION_MISMATCH` | `Verification` |
| `TABLE_CELL_INDEX_MISSING` | `Verification` |
| `TABLE_CONTENT_UPDATE_FAILED` | `Verification` unless definitely retryable and not sent. |

### UpdateErrorCodes

| Code | Classification |
| --- | --- |
| `UPDATE_REVISION_CONFLICT` | `Verification` |
| `UPDATE_MANAGED_REGION_MISMATCH` | `Verification` |
| `UPDATE_PHYSICAL_PLAN_INVALID` | `Internal` unless caused by validated user input, in which case `Input`. |
| `UPDATE_APPLICATION_FAILED` | `Verification` unless wrapped failure is definitely retryable and not sent. |
| `UPDATE_OPERATION_EXECUTOR_NOT_REGISTERED` | `Internal` |
| `UPDATE_OPERATION_EXECUTOR_DUPLICATE` | `Internal` |
| `UPDATE_OPERATION_EXECUTOR_FAILED` | `Internal` unless wrapped failure maps to `Transient` or `Verification`. |
| `UPDATE_READBACK_FAILED` | `Verification` unless the failure is definitely retryable and not sent. |
| `UPDATE_READBACK_MISMATCH` | `Verification` |

### StateErrorCodes

| Code | Classification |
| --- | --- |
| `STATE_NOT_FOUND` | `Verification` when a required prior state is missing for an update; `Input` only for an explicitly local state-inspection command. |
| `STATE_CORRUPTED` | `Verification` |
| `STATE_SCHEMA_VERSION_UNSUPPORTED` | `Verification` |
| `STATE_DOCUMENT_IDENTITY_MISMATCH` | `Verification` |
| `STATE_INVALID_TRANSITION` | `Verification` |
| `STATE_VERIFICATION_REQUIRED` | `Verification` |
| `STATE_VERIFICATION_MISMATCH` | `Verification` |
| `STATE_SAVE_FAILED` | `Verification` unless caused by local configuration or filesystem setup, then `Configuration`. |
| `STATE_ALGORITHM_VERSION_UNSUPPORTED` | `Verification` |

Unknown or blank stable codes must fall back to `Internal` and use the fixed
internal summary message. The CLI must not invent a new classification from an
unknown prefix.

## 7. Retry Policy Note

Task 3 did not change retry behavior. It may classify final failures using
the existing retry and delivery-state outcomes.

### Ordinary Google Docs Client

The ordinary Google Docs client may retry transient Google API failures such as
HTTP 429, 500, 502, 503, and 504 when the operation can be safely retried under
the existing client contract. It must continue to honor cancellation and must
not retry authentication, permission, validation, malformed request, or
non-retryable 4xx failures.

### Temporary Image Host

The temporary image host may retry retryable upload, permission update, and
cleanup calls under the existing bounded retry policy. It must not expose
temporary public image URIs, Drive file identifiers, provider response bodies,
credential paths, or token details in CLI messages. Cleanup failure remains a
warning unless the calling workflow requires it to be promoted to a hard
verification failure.

### Physical Update Executor

The physical update executor must preserve delivery-state based retry:

- retry only when the failure is retryable and delivery state is definitely
  `NotSent`;
- do not retry when delivery state is `Sent` or `Unknown`;
- honor `Retry-After` where available;
- honor cancellation.

This policy is required because a batch update can be partially or fully
applied after the client loses certainty. Retrying an indeterminate request can
duplicate document mutations or corrupt the managed region. Delivery-state
classification therefore protects crash consistency and verified-state
promotion.

### Retryable, Indeterminate, And Rejected Handling

| Outcome | Handling | CLI Classification |
| --- | --- | --- |
| retryable and `NotSent` | Retry according to the existing bounded policy. If attempts are exhausted, classify as transient. | `Transient` |
| retryable but `Sent` or `Unknown` | Do not retry. Preserve the indeterminate result and require readback or recovery before success is claimed. | `Verification` |
| non-retryable rejected request | Do not retry. Preserve stable rejected/error code. | `Verification`, `Input`, `Configuration`, or `Internal` according to stable code. |
| revision conflict | Do not retry as transient. Require replan from a fresh revision. | `Verification` |
| cancellation | Rethrow or propagate cancellation; do not convert to retry. | `Canceled` |

## 8. Boundary Rules

Application layer:

- may throw or return existing safe exception types and stable codes;
- may convert implementation failures into `PublishPipelineException`,
  `PhysicalUpdateException`, or `StateLifecycleException` where those types
  already exist;
- must not depend on CLI exit codes or CLI classifications;
- must rethrow `OperationCanceledException`.

Infrastructure layer:

- may normalize provider failures into existing Application exceptions or
  infrastructure exceptions with stable safe codes;
- must not expose provider response bodies, URIs, credential values, token
  values, local paths, or raw exception messages through safe user-facing
  messages;
- must preserve delivery-state information for physical update failures;
- must rethrow `OperationCanceledException`.

CLI layer:

- owns final classification;
- owns final exit code conversion;
- owns classification-based summary messages;
- may emit simple `exceptionType`;
- must not emit raw `PublishResult.Error.Message`, raw exception messages,
  stack traces, provider bodies, URIs, local paths, tokens, or secrets;
- must preserve success compatibility fields defined by Phase 4-2-1.

`OperationCanceledException` must be rethrown through lower layers and handled
only at the CLI boundary or test harness boundary that owns the process result.

## 9. Test Matrix

Task 3 must add or update local unit tests. Tests must use fakes, stubs,
in-memory data, temporary files, or captured console output. They must not use
real Google credentials, enable `VMF_PUBLISHER_GOOGLE_E2E`, mutate Google Docs
or Drive, create packages, or execute the Avast-pending package.

| Viewpoint | Required Assertion |
| --- | --- |
| classification | Stable codes map to `None`, `Usage`, `Configuration`, `Input`, `Verification`, `Transient`, `Canceled`, and `Internal` as specified. |
| exit code | Each classification returns the specified exit code; `Input` and `Internal` both return `1`. |
| safe message | Final summary uses the fixed classification message and never raw `PublishResult.Error.Message`. |
| verification exit | Readback mismatch, readback failure, managed-region mismatch, revision conflict, and state verification mismatch return exit `4`. |
| transient | HTTP 429, 500, 502, 503, 504 or stable transient code returns exit `75` with the transient fixed message. |
| canceled | `OperationCanceledException` returns exit `130`, code `CANCELED`, and no raw exception message. |
| unknown/internal | Unknown, blank, or missing stable code falls back to `Internal`, exit `1`, and the internal fixed message. |
| raw message non-output | Captured stdout, stderr, and structured JSON do not contain seeded sensitive values such as URI, local path, token, secret, HTTP response body, or exception message. |
| exceptionType | When present, `exceptionType` is only the simple type name. |
| retry boundary | Existing physical update tests continue to prove retryable `NotSent`, indeterminate, rejected, revision conflict, and cancellation handling. |

## 10. Task 3 Implementation Plan

Task 3 should proceed in the following order:

1. Preflight
   - inspect `git status --short`;
   - confirm no unrelated user changes in target files;
   - keep Live E2E disabled;
   - do not touch `dist/` or release artifacts.
2. Classification helper
   - extend internal CLI classification logic to include `Verification`;
   - map existing stable error codes from `PublishErrorCodes`,
     `UpdateErrorCodes`, and `StateErrorCodes`;
   - make unknown or blank code return `Internal`.
3. Exit conversion
   - keep existing exit constants;
   - route `Verification` to `ExitVerification = 4`;
   - keep `Input` and `Internal` on `ExitPublishFailed = 1`.
4. Safe messages
   - replace publish failure summary message with classification-based fixed
     text;
   - prevent direct output of `PublishResult.Error.Message`;
   - preserve usage/help success compatibility where already safe.
5. Cancellation
   - ensure lower-level `OperationCanceledException` is not converted to
     internal failure before the CLI boundary;
   - keep CLI result `CANCELED`, exit `130`.
6. Focused tests
   - add CLI tests for classification, exit code, safe message, verification
     exit, transient, canceled, unknown/internal, and raw message exclusion.
7. Local verification
   - run only Task 3-authorized focused unit tests and local checks;
   - do not run Live E2E;
   - do not create, verify, or update release artifacts unless a later task
     explicitly authorizes it.

Expected change targets:

| Path | Expected Change |
| --- | --- |
| `src/Publisher.Cli/Program.cs` | Internal `ErrorClassification.Verification`, stable code classification, exit conversion, fixed safe messages, cancellation handling checks. |
| `tests/unit/Publisher/CliApplicationTests.cs` | Focused CLI classification, exit code, safe message, verification exit, transient, canceled, unknown/internal, and sensitive-value exclusion coverage. |

Do not change:

| Path Or Area | Reason |
| --- | --- |
| `specs/` | Frozen and authoritative specifications are out of scope. |
| `src/Publisher/Application/` public contracts | Public API preservation. |
| `src/Publisher/Domain/` | Error classification and exit conversion are CLI concerns. |
| `src/Publisher/Infrastructure/Google/` | Retry and provider behavior changes are out of scope for Task 3 unless separately authorized. |
| `tests/integration/Publisher/GoogleDocsEndToEndIntegrationTests.cs` | Live E2E is out of scope. |
| `dist/` | Release artifacts must not be generated, verified, or updated by this task. |
| `tools/publisher/package-publisher.ps1` | Package creation is out of scope. |
| `tools/publisher/verify-package.ps1` | Package verification behavior is out of scope. |
| `docs/distribution/ReleaseChecklist.md` | Release approval state is out of scope. |

## 11. Acceptance Criteria

This specification is complete when:

- the classification catalog is defined;
- the classification and exit code matrix is defined;
- verification/readback mismatch exit behavior is explicit;
- safe message rules prohibit raw Publisher errors, exceptions, provider
  bodies, URIs, local paths, tokens, and secrets;
- stable mappings for `PublishErrorCodes`, `UpdateErrorCodes`, and
  `StateErrorCodes` are recorded;
- unknown stable codes fall back to `Internal`;
- retry policy notes preserve ordinary Google Docs, temporary image host, and
  physical update delivery-state behavior;
- Application, Infrastructure, and CLI boundary rules are defined;
- `OperationCanceledException` rethrow behavior is explicit;
- Task 3 test matrix and implementation plan are listed;
- no production code, tests, Frozen specifications, public APIs, release
  artifacts, Google Docs, Google Drive, Live E2E state, or Avast-pending
  package state are changed by this Task 2 document.

## 12. Task 4 Final Review Result

Phase 4-2-2 is Done as local-only implementation and review.

Task 4 review confirmed:

- classification and exit code matrix is implemented;
- verification/readback failures map to `ExitVerification = 4`;
- unknown, blank, or missing stable codes fall back to `Internal`;
- CLI summaries use fixed safe messages and do not echo raw publish or
  exception messages;
- stable error-code constants are preserved;
- `OperationCanceledException` is rethrown below the CLI boundary and becomes
  `CANCELED` / exit `130` at the CLI boundary;
- retry policy was not changed;
- publish success stdout compatibility is preserved;
- help and usage behavior is preserved;
- JSON lifecycle and summary fields remain present;
- public APIs are unchanged.

Security review confirmed that failure summaries and structured diagnostic
messages do not emit raw exception messages, HTTP response bodies, local paths,
tokens, secrets, credential paths, token-store paths, stack traces, or invalid
input values. The existing publish-success compatibility field `documentUrl`
remains the only allowed URI output in a successful publish summary.

Automated evidence from Task 4:

| Check | Result |
| --- | --- |
| Focused Publisher Unit Tests | PASS - 33/33 |
| Publisher Unit Tests | PASS - 490/490 |
| Publisher Integration Tests | PASS - 12/12, non-live |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS - no whitespace errors; CRLF normalization warnings only |
| Frozen specification changes | None |
| Public API changes | None |
| Live E2E / Google Docs / Google Drive mutation | Not executed |
| Release package / dist / tag / publication | Not changed |

Phase 4-2-2 has no phase-specific release blocker.

Continuing blockers:

- release gate remains blocked pending release-owner authorization;
- Live E2E remains blocked without explicit per-run authorization;
- Google Docs and Google Drive mutation remain blocked;
- package creation, package update, tag, and publication remain blocked;
- Avast-pending package state remains unchanged.

vNext candidate:

- a separate input-specific public exit code may be considered only as a future
  public CLI behavior change proposal.
