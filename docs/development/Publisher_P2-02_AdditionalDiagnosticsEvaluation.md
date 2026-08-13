# Publisher P2-02 Additional Diagnostics Evaluation

Status  : Complete / P2-02-A and P2-02-B implemented as narrow local-only scope
Scope   : Evaluate and close additional Publisher diagnostics for troubleshooting while preserving published document semantics and ADR-0006 safe observability
Depends : docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md, docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md, docs/development/Publisher_vNext_Backlog.md

This record started as a design-only record and now records completion of the
separately authorized narrow local-only implementation for P2-02-A and P2-02-B.
It does not modify published document semantics, Frozen specifications, public
APIs, persisted schemas, release artifacts, Google Docs, Google Drive, OAuth
token stores, Live E2E state, package or dist contents, tags, publication
state, Avast evidence, or vendor-clearance status.

P2-02 is an independent vNext design scope. It does not reopen the existing
Publisher `0.0.1-dev` prerelease.

## 1. Purpose

The purpose of P2-02 is to evaluate small additional Publisher diagnostics
that can improve local troubleshooting without changing published document
semantics.

The diagnostics considered here must remain compatible with ADR-0006
Diagnostic Logging and Safe Observability. They must preserve structured JSON
stderr diagnostics, stdout result compatibility, stable safe messages, bounded
diagnostic fields, and redaction before serialization.

## 2. Scope

Allowed design scope:

- review the current Publisher diagnostic logging implementation;
- review related diagnostic, error handling, redaction, and safe-message
  documentation;
- review focused local tests that cover structured diagnostics;
- identify gaps that reduce troubleshooting value;
- propose value-safe diagnostic improvements;
- define implementation acceptance criteria and local-only verification.

Allowed implementation scope used for P2-02-A and P2-02-B:

- internal CLI diagnostic changes in `src/Publisher.Cli/Program.cs`;
- focused Publisher unit-test updates under `tests/unit/Publisher/`;
- documentation updates that record the adopted diagnostic behavior.

## 3. Non-Scope

P2-02 does not authorize:

- changes to published document content, formatting, update semantics, or
  Google Docs mutation behavior;
- Frozen specification changes;
- public API changes, including `IPublisherLogger`;
- persisted schema or canonical format changes;
- release, tag, publication, GitHub asset, package, or dist updates;
- Live E2E;
- Google Docs or Google Drive mutation;
- OAuth, credential, or token-store operation;
- Avast or flagged executable execution;
- vendor-clearance judgment;
- stage, commit, or push.

P2-02 also does not authorize external log collection, OpenTelemetry,
distributed tracing, monitoring-service integration, or unbounded verbose
logging.

## 4. Current Diagnostic Surface

The current CLI diagnostic surface is centered on
`StructuredPublisherLogger` in `src/Publisher.Cli/Program.cs`.

Observed current behavior:

- structured diagnostic events are emitted as one JSON object per stderr line;
- stdout remains reserved for user-facing command output;
- each structured event carries `timestampUtc`, `level`, `sessionId`,
  `command`, `phase`, `operation`, `code`, and `message`;
- lifecycle events include `SESSION_STARTED`, `COMMAND_STARTED`,
  `COMMAND_COMPLETED`, `COMMAND_FAILED`, summaries, warnings, and selected
  command-specific diagnostics;
- final summaries include exit code, classification, elapsed milliseconds, and
  simple exception type when applicable;
- `verify` emits a `LOCAL_VERIFY_REPORT` diagnostic with local-only boundary
  checks;
- safe messages are classification-based and do not expose raw exception
  messages;
- warning diagnostics preserve stable warning codes and safe executor context;
- publish success compatibility still allows `documentId` and `documentUrl`
  in the existing success surface.

Related local tests currently cover structured fields, lifecycle events,
normalized command and phase values, safe messages, cancellation, raw exception
message exclusion, transient classification, warning context, and publish
success compatibility fields.

## 5. Identified Gaps

The current diagnostics are safe and stable, but troubleshooting can still be
slow in these areas:

| Gap | Impact | Boundary |
| --- | --- | --- |
| Failure location is coarse after a command moves through multiple internal operations. | Operators can see the final phase but may not know the last completed safe step before failure. | Any improvement must use stable operation names only, not paths, arguments, document ids, provider payloads, or raw exception text. |
| Retry and delivery-state outcomes are not surfaced in final CLI diagnostics except indirectly through classification and error code. | Transient versus verification failures can require source-level inspection to understand whether retry policy, delivery state, or readback uncertainty drove the result. | Only value-safe retry metadata may be logged, such as attempt count, max attempts, retryable flag, delivery-state enum, and numeric HTTP status when already safe. |
| Input-shape diagnostics intentionally avoid echoing raw paths and arguments, but do not provide a safe shape summary. | Troubleshooting usage, verify, dry-run, diff, or publish invocation mistakes can require reproduction. | Safe summaries may include argument count and normalized command only; never raw argument values. |
| Configuration validation diagnostics avoid credential and token-store details but do not identify which configuration category failed. | Operators may need to inspect broad configuration areas manually. | A category-only field such as `configurationCategory` may be allowed if drawn from an allow-list like `googleApi`, `publisher`, `cli`, or `temporaryImageHost`. |
| Local verify report is richer than other command summaries. | Dry-run, diff, and publish failure paths do not have an equivalent compact support summary. | Any support summary must remain command-local, safe, and stderr-only. It must not become release evidence or external reporting. |

No gap requires a published-document semantic change.

## 6. ADR-0006 And Error-Handling Alignment

The following constraints are mandatory for any future P2-02 implementation:

- keep structured JSON diagnostics on stderr;
- keep stdout result compatibility unchanged;
- keep event codes stable, uppercase, and testable;
- add only bounded, reviewed, value-safe fields;
- never log raw exception messages, stack traces, raw HTTP bodies, provider
  payloads, local paths, raw URIs, private URLs, temporary public URLs,
  document URLs outside existing publish-success compatibility, credentials,
  token-store paths, tokens, secrets, cookies, Authorization headers, or raw
  command-line argument values;
- keep safe messages classification-based or allow-listed;
- perform redaction and safe-value selection before JSON serialization;
- use pattern redaction only as a defensive backup, not as the primary safety
  mechanism;
- preserve ADR-0007 classification, exit-code, cancellation, retry, and safe
  summary boundaries;
- preserve lower-layer cancellation propagation and delivery-state semantics.

## 7. Candidate Improvements

### P2-02-A: Last Safe Operation Summary

Add value-safe fields to final summary diagnostics:

- `lastPhase`;
- `lastOperation`;
- `lastEventCode`.

These fields would describe only the last emitted stable diagnostic context,
not raw input, provider details, or document identifiers.

Priority: P1 for implementation.

Rationale: Highest troubleshooting value with low semantic risk. It helps
operators localize failures while preserving published document behavior.

### P2-02-B: Safe Invocation Shape

Add value-safe fields to command start or usage diagnostics:

- `argumentCount`;
- `recognizedCommand`;
- `expectedArgumentShape`.

`expectedArgumentShape` must be an allow-listed label, for example
`publish-markdown-path`, `verify-optional-markdown-path`, `diff-before-after`,
or `none`.

Priority: P1 for implementation.

Rationale: Improves support for usage and local setup failures without logging
raw command-line values.

### P2-02-C: Configuration Category

Add an allow-listed `configurationCategory` field to configuration failures
where the category can be determined without exposing values.

Allowed initial categories:

- `cli`;
- `googleApi`;
- `publisher`;
- `temporaryImageHost`;
- `unknown`.

Priority: P2 for implementation.

Rationale: Useful for troubleshooting, but requires careful mapping from
existing `CONFIG_*` codes and tests to prevent accidental value leakage.

### P2-02-D: Retry And Delivery Metadata

For failures that already carry safe retry or physical-update metadata, expose
bounded fields such as:

- `attemptCount`;
- `maxAttempts`;
- `retryable`;
- `deliveryState`;
- `httpStatus`.

Priority: P2 for implementation.

Rationale: Valuable for distinguishing transient, verification, and
indeterminate cases. This should proceed only after source review confirms
metadata is already available at the CLI diagnostic boundary without public API
changes.

### P2-02-E: Support Summary Event

Add a compact `SUPPORT_SUMMARY` stderr event after final summary for local
troubleshooting. It would contain only safe, already-reviewed fields:

- result code;
- classification;
- exit code;
- command;
- phase;
- operation;
- safe message;
- selected P2-02 fields adopted above.

Priority: P3 / defer.

Rationale: It may duplicate final summary and increase log volume. Prefer
improving final summary fields first.

## 8. Implemented Design

P2-02 proceeded with a narrow implementation for P2-02-A and P2-02-B only.

The implementation:

1. tracks the last safe diagnostic phase, operation, and event code inside the
   internal CLI structured logger;
2. includes `lastPhase`, `lastOperation`, and `lastEventCode` only in final
   summary events;
3. includes safe invocation-shape metadata only in command-start diagnostics;
4. keeps all new fields optional and present only when the value is known and
   allow-listed;
5. adds focused unit tests that verify both presence of safe fields and absence
   of seeded sensitive values;
6. avoids public API changes by keeping all state inside the CLI logger and
   existing internal helpers.

P2-02-C, P2-02-D, and P2-02-E remain deferred. The implementation did not add
`configurationCategory`, retry or delivery metadata, or a `SUPPORT_SUMMARY`
event.

## 9. Acceptance Criteria

P2-02 is acceptable only when:

- published document semantics are unchanged;
- stdout compatibility is unchanged;
- `IPublisherLogger` and other public APIs are unchanged;
- no Frozen specifications, persisted schemas, canonical formats, release
  artifacts, package outputs, or dist contents are changed;
- all new diagnostic fields are allow-listed, bounded, and value-safe;
- final summaries include last-safe-operation fields only when known;
- invocation-shape diagnostics include counts and allow-listed shape labels,
  never raw argument values;
- raw exception messages, stack traces, provider payloads, HTTP bodies, local
  paths, private URLs, temporary public URLs, document URLs outside existing
  success compatibility, credentials, token-store paths, tokens, secrets,
  cookies, and Authorization headers remain absent from stdout and stderr;
- ADR-0007 classification, exit-code mapping, cancellation, and safe-message
  behavior are preserved;
- focused unit tests cover success, usage failure, configuration failure, and
  seeded sensitive-value exclusion for the adopted fields while preserving
  existing transient/internal/cancellation diagnostic coverage;
- local verification completes without Live E2E, Google Docs or Drive
  mutation, OAuth or token-store operation, package or dist update, Avast
  execution, stage, commit, or push.

## 10. Verification Result

Local-only verification for the P2-02-A / P2-02-B implementation:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~CliApplicationTests"
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
dotnet build VMF.Publisher.sln --configuration Release --no-restore
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
git status --short --branch
```

Recorded result for this closeout sync:

- focused CLI unit tests: PASS, 38 passed / 0 failed / 0 skipped;
- documentation and Git diff checks: PASS;
- broader unit suite, Release build, format verification, and full solution
  verification were not rerun in this closeout task.

Prohibited verification:

- Live E2E;
- Google Docs or Google Drive mutation;
- OAuth or token-store operation;
- package creation or package verification;
- `dist/` mutation;
- Avast or flagged executable execution;
- release, tag, publication, GitHub asset, stage, commit, or push.

## 11. Implementation GO/NO-GO

Closeout decision: COMPLETE for P2-02 overall.

Basis:

- P2-02-A and P2-02-B were the recommended P1 implementation scope and are
  implemented;
- deferred P2-02-C, P2-02-D, and P2-02-E were explicitly non-required for the
  first implementation and remain unimplemented by design;
- implementation remained CLI-internal and local-only;
- published document semantics, stdout compatibility, public APIs, persisted
  schemas, package/dist outputs, release state, Google state, OAuth/token-store
  state, Avast state, vendor-clearance status, staging, commit, and push were
  not changed by this closeout.
