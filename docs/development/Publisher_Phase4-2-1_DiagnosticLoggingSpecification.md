# Publisher Phase 4-2-1 Diagnostic Logging Specification

Status  : Planning
Scope   : Publisher diagnostic logging specification for Task 3 implementation
Depends : docs/development/Publisher_Phase4_Planning.md, docs/development/Publisher_Phase4-1_DesignNotes.md, docs/development/Publisher_Phase4_LocalVerificationPlan.md

This document defines the diagnostic logging specification for Publisher
Phase 4-2-1. It is documentation only. It does not modify Frozen
specifications, public APIs, persisted schemas, canonical formats, production
code, release artifacts, Google Docs, Google Drive, Live E2E behavior, or the
Avast-pending package state.

Task 3 implementation must preserve the public `IPublisherLogger` contract.
Any richer diagnostic behavior must be implemented inside the CLI logging
adapter or existing internal helpers unless a later task explicitly authorizes
a public API change.

## 1. Purpose

The purpose of this specification is to standardize Publisher diagnostic
logging so local CLI runs produce predictable, safe, machine-readable evidence.

The specification covers:

- required structured log fields;
- command, phase, and operation naming;
- command lifecycle event names;
- summary fields;
- exception message safety;
- warning code and message conventions;
- compatibility for existing `documentId` and `documentUrl` output;
- Task 3 implementation and test scope.

## 2. Scope Boundaries

Allowed scope for Task 3:

- modify `src/Publisher.Cli/Program.cs`;
- modify focused Publisher unit tests under `tests/unit/Publisher/`;
- add internal CLI-only helper methods or records when needed;
- update diagnostic logging behavior without changing publish semantics;
- keep verification local-only.

Excluded scope:

- Frozen specification changes;
- public API changes, including `IPublisherLogger`;
- production behavior unrelated to diagnostics;
- release artifact generation, replacement, or publication;
- Live E2E;
- Google Docs or Google Drive mutation;
- token store, credential, or temporary public hosting mutation;
- Avast-pending executable or package execution;
- signing, installer, apphost, package-trust, or release-process changes.

## 3. Field Definitions

Every structured Publisher diagnostic log line emitted by
`StructuredPublisherLogger` must be a single JSON object written to stderr.

Required standard fields:

| Field | Type | Required | Definition |
| --- | --- | --- | --- |
| `timestampUtc` | string | Yes | UTC timestamp in round-trip ISO 8601 format. |
| `level` | string | Yes | Lowercase severity: `debug`, `info`, `warning`, or `error`. Task 3 should use only existing effective levels unless behavior requires otherwise. |
| `sessionId` | string | Yes | Per-process Publisher diagnostic session id. Must not contain user input. |
| `command` | string | Yes | Normalized command name. Use `none` when no command is supplied, `help` for help output, and `unknown` for unrecognized commands. |
| `phase` | string | Yes | Stable coarse processing phase. See Section 5. |
| `operation` | string | Yes | Stable operation within the phase. See Section 5. |
| `code` | string | Yes | Stable uppercase event, result, or warning code. |
| `message` | string | Yes | Human-readable safe message. Must pass the redaction rules in Section 8. |

Optional fields must be emitted only when the value is non-null and safe under
Section 8.

Recommended optional fields:

| Field | Type | Definition |
| --- | --- | --- |
| `exitCode` | number | CLI process exit code for summary events. |
| `classification` | string | Existing `ErrorClassification` value for summary events. |
| `elapsedMilliseconds` | number | Total elapsed milliseconds for summary events. |
| `stepCount` | number | Compiled publish step count for dry-run planning. |
| `beforeStepCount` | number | Compiled step count for the diff before document. |
| `afterStepCount` | number | Compiled step count for the diff after document. |
| `stepDelta` | number | `afterStepCount - beforeStepCount`. |
| `exceptionType` | string | Exception type name only, when allowed by Section 8. |
| `documentId` | string | Existing compatibility field. See Section 10. |
| `documentUrl` | string | Existing compatibility field. See Section 10. |

Fields that must not be added in Task 3:

- raw command-line arguments;
- Markdown file paths;
- credential paths;
- token-store paths;
- access tokens or refresh tokens;
- raw URIs other than the existing compatibility `documentUrl`;
- HTTP request or response bodies;
- local image paths;
- temporary image public URIs;
- Google Drive temporary file identifiers.

## 4. Event Naming Rules

Event codes must be uppercase snake case and stable across releases.

Lifecycle event names:

| Event Suffix | Meaning | Level |
| --- | --- | --- |
| `_STARTED` | A command or internal operation started. | `info` |
| `_COMPLETED` | A command or internal operation completed successfully before final summary. | `info` |
| `_FAILED` | A command or internal operation failed before final summary. | `error` |
| `_WARNING` | A recoverable diagnostic condition occurred. Prefer the existing stable warning code when one exists. | `warning` |
| `_SUMMARY` | A compact result summary. Existing result codes may continue to be used for final summary compatibility. | `info` or `error` |

Command lifecycle event names:

| Event | Meaning |
| --- | --- |
| `SESSION_STARTED` | Diagnostic session started. This event must be command independent. |
| `COMMAND_STARTED` | Normalized command dispatch started. |
| `COMMAND_COMPLETED` | Command returned a success result before final summary. |
| `COMMAND_FAILED` | Command returned or threw a failure result before final summary. |

`SESSION_STARTED` message must be command independent. The preferred message is
`Publisher diagnostic session started.` rather than `Publish session started.`

Final summary may keep existing result codes such as `HELP`,
`VERIFY_SUCCEEDED`, `DRY_RUN_SUCCEEDED`, `DIFF_SUCCEEDED`,
`PUBLISH_SUCCEEDED`, `USAGE_ERROR`, and stable Publisher error codes for
backward compatibility.

## 5. Phase And Operation Catalog

Phase names must be lowercase and stable.

| Command | Phase | Operation | Meaning |
| --- | --- | --- | --- |
| none | `session` | `initialize` | No command was supplied and help will be shown. |
| help | `cli` | `help` | Help output. |
| unknown | `cli` | `usage` | Unknown command or invalid command shape. |
| publish | `publish` | `validateArguments` | Publish argument validation. |
| publish | `publish` | `loadSettings` | Publish settings load and validation. |
| publish | `publish` | `compile` | Markdown load, parse, image preparation, and compile before Google write. |
| publish | `publish` | `execute` | Publish execution through the configured publisher. |
| publish | `publish` | `summary` | Publish result summary. |
| verify | `verify` | `validateArguments` | Verify argument validation. |
| verify | `verify` | `loadSettings` | Verify settings load and validation without requiring publish settings. |
| verify | `verify` | `compile` | Optional local Markdown compile. |
| verify | `verify` | `summary` | Verify result summary. |
| dry-run | `planner` | `validateArguments` | Dry-run argument validation. |
| dry-run | `planner` | `loadSettings` | Dry-run settings load and validation. |
| dry-run | `planner` | `compile` | Local Markdown compile. |
| dry-run | `planner` | `plan` | Local publish plan summary. |
| dry-run | `planner` | `summary` | Dry-run result summary. |
| diff | `verification` | `validateArguments` | Diff argument validation. |
| diff | `verification` | `loadSettings` | Diff settings load and validation. |
| diff | `verification` | `compileBefore` | Compile the before Markdown document. |
| diff | `verification` | `compileAfter` | Compile the after Markdown document. |
| diff | `verification` | `diff` | Local diff summary. |
| diff | `verification` | `summary` | Diff result summary. |
| any | `diagnostic` | `summary` | Cross-command final summary when a more specific phase cannot be determined safely. |
| executor | `executor` | `insertImage` | Image insertion warning context from publish plan execution. |
| executor | `executor` | `cleanupTemporaryImage` | Temporary image cleanup warning context. |

Task 3 may implement a conservative subset of operation transitions if it keeps
all emitted structured lines populated with the required fields. It must not
emit misleading phases for work that has not started.

## 6. Event Catalog

Required Task 3 event catalog:

| Code | Level | Command | Phase | Operation | Message |
| --- | --- | --- | --- | --- | --- |
| `SESSION_STARTED` | `info` | normalized command or `none` | `session` | `initialize` | `Publisher diagnostic session started.` |
| `COMMAND_STARTED` | `info` | normalized command | command phase | first command operation | `Publisher command started.` |
| `COMMAND_COMPLETED` | `info` | normalized command | command phase | `summary` | `Publisher command completed.` |
| `COMMAND_FAILED` | `error` | normalized command | command phase | `summary` | `Publisher command failed.` |
| `CONFIGURATION_VALID` | `info` | `verify` | `verify` | `summary` | `Configuration validation succeeded.` |
| `DRY_RUN_PLAN` | `info` | `dry-run` | `planner` | `plan` | `Local publish plan compiled.` |
| `DIFF_SUMMARY` | `info` | `diff` | `verification` | `diff` | `Local diff summary compiled.` |
| `HELP` | `info` | `help` or `none` | `cli` | `help` | `Help displayed.` |
| `USAGE_ERROR` | `error` | `unknown` or supplied command | `cli` | `usage` | Safe usage failure message. |
| `VERIFY_SUCCEEDED` | `info` | `verify` | `verify` | `summary` | `Verification succeeded.` |
| `DRY_RUN_SUCCEEDED` | `info` | `dry-run` | `planner` | `summary` | `Dry run completed.` |
| `DIFF_SUCCEEDED` | `info` | `diff` | `verification` | `summary` | `Diff completed.` |
| `PUBLISH_SUCCEEDED` | `info` | `publish` | `publish` | `summary` | `Publication succeeded.` |
| `CANCELED` | `error` | current command | current phase or `diagnostic` | `summary` | `Publication was canceled.` or a command-neutral equivalent. |
| `TRANSIENT_ERROR` | `error` | current command | current phase or `diagnostic` | `summary` | Stable classified transient failure message. |
| `PUBLISHER_ERROR` | `error` | current command | current phase or `diagnostic` | `summary` | Stable classified internal failure message. |
| existing Publisher error code | `error` | current command | current phase or `diagnostic` | `summary` | Stable safe failure message. |
| `IMAGE_ALT_TEXT_UPDATE_FAILED` | `warning` | `publish` | `executor` | `insertImage` | Google Docs image insertion cannot set alt text; model retained. |
| `IMAGE_TEMP_FILE_DELETE_FAILED` | `warning` | `publish` | `executor` | `cleanupTemporaryImage` | Temporary image cleanup failed. |

The exact stable code strings for image warnings must continue to come from
`PublishErrorCodes` to preserve existing behavior.

## 7. Summary Fields

Every final summary event must include:

- required standard fields from Section 3;
- `exitCode`;
- `classification`;
- `elapsedMilliseconds`.

Final summary should include `documentId` and `documentUrl` only under the
compatibility rule in Section 10.

Final summary must not include:

- raw exception message from an external provider;
- stack trace;
- local file path;
- URI except the existing compatibility `documentUrl`;
- token, secret, credential value, or token-store path;
- raw HTTP response body.

Final summary level:

- `info` when `exitCode` is `0`;
- `error` when `exitCode` is non-zero.

## 8. Redaction And SafeMessage Rules

`SafeMessage` must produce a stable, classified, consumer-safe message. It must
not directly expose external exception messages.

Rules:

- Do not output raw message text from `HttpRequestException`,
  Google API exceptions, OAuth exceptions, IO exceptions, or any exception that
  may contain a URI, path, token, credential name, response body, or provider
  payload.
- Do not output secrets, access tokens, refresh tokens, private keys,
  credential JSON, authorization headers, cookies, or bearer values.
- Do not output token-store paths, credential paths, Markdown file paths, local
  image paths, temp paths, user profile paths, or package paths.
- Do not output document URLs except through the existing compatibility
  `documentUrl` field described in Section 10.
- Do not output temporary public image URIs or Google Drive temporary file ids.
- Use stable messages by classification, for example:
  - transient: `A transient external service error occurred.`;
  - configuration: use existing configuration exception message only after it
    has been reviewed as value-free;
  - input: use existing input message only after path-like values are removed;
  - canceled: `Operation was canceled.`;
  - internal: `An internal Publisher error occurred.`;
  - usage: existing usage messages are allowed when they do not echo paths or
    secrets.
- `exceptionType` may be emitted as the simple CLR type name, such as
  `HttpRequestException`, when it contains no user-controlled text. It must not
  include namespace-qualified provider payloads, stack traces, or inner
  exception messages.
- Inner exception message text must not be emitted.
- Redaction must be applied before serialization, not by post-processing JSON.

Task 3 should prefer allow-listed safe messages over pattern-based cleanup.
Pattern-based redaction may be added as a defensive backup, but it must not be
the only safety mechanism for external exception messages.

## 9. Warning Code And Message Rules

Warning codes must be stable uppercase snake case. Existing warning codes from
`PublishErrorCodes` must be preserved.

Warning messages must:

- describe the recoverable condition;
- avoid document ids, URLs, temporary resource ids, local paths, raw provider
  messages, and credentials;
- avoid implying that Google Docs or Drive behavior was live-verified unless a
  live operation actually occurred under authorization;
- be deterministic and testable.

Recommended warning message normalization:

| Code | Message |
| --- | --- |
| `IMAGE_ALT_TEXT_UPDATE_FAILED` | `Google Docs image insertion cannot set alt text; alt text remains in the publish model only.` |
| `IMAGE_TEMP_FILE_DELETE_FAILED` | `Temporary image cleanup failed.` |

If Task 3 changes warning message text, tests must assert the stable warning
code and the absence of sensitive values. Tests should avoid brittle assertions
on every word unless the message is part of the acceptance contract.

## 10. documentId And documentUrl Compatibility

Current CLI behavior emits publish success `Document ID` and `Document URL` on
stdout and includes `documentId` and `documentUrl` in the structured final
summary when available.

Task 3 must preserve compatibility by default:

- keep stdout success lines unchanged unless a later task explicitly authorizes
  a breaking CLI output change;
- keep structured `documentId` and `documentUrl` summary fields when publish
  succeeds;
- do not add document identifiers or URLs to intermediate events;
- do not include document identifiers or URLs in error events unless already
  present through existing compatibility behavior.

Rationale:

- existing users may depend on stdout success output;
- existing tests or scripts may inspect the structured summary;
- removing these fields is safer as a separately reviewed compatibility change.

Security note:

- `documentId` and `documentUrl` are identifiable document references. They are
  allowed only for current compatibility and should be treated as candidates for
  a future opt-in or redacted-output mode.

## 11. Test Requirements

Task 3 must add or update local unit tests without Live E2E.

Required test viewpoints:

| Viewpoint | Expected Assertion |
| --- | --- |
| JSON required fields | Every structured JSON line contains `timestampUtc`, `level`, `sessionId`, `command`, `phase`, `operation`, `code`, and `message`. |
| command | Help, verify, dry-run, diff, unknown command, and publish failure paths have the expected normalized `command`. |
| phase | Verify uses `verify`; dry-run uses `planner`; diff uses `verification`; publish uses `publish`; executor warnings use `executor` when emitted through CLI logger. |
| operation | Summary events use `summary`; dry-run plan uses `plan`; diff summary uses `diff`; help uses `help`; usage failures use `usage`. |
| command lifecycle | `COMMAND_STARTED` appears before command work; `COMMAND_COMPLETED` or `COMMAND_FAILED` appears before final summary when applicable. |
| SESSION_STARTED | Message is command independent and does not say `Publish session started.` |
| sensitive value exclusion | Structured logs do not contain credential paths, token-store paths, Markdown file paths, local image paths, bearer tokens, raw URIs, or raw provider bodies. |
| safe exception | External exception messages are replaced with stable classified messages; optional `exceptionType` contains only the simple type name. |
| warning code | Existing image warning codes are preserved and warning messages do not expose image URIs or temporary resource ids. |
| document compatibility | Publish success still exposes `documentId` and `documentUrl` exactly where existing compatibility requires it. |

Recommended focused test file:

- `tests/unit/Publisher/CliApplicationTests.cs`
- `tests/unit/Publisher/PublishPlanExecutorTests.cs` only if warning message or
  structured warning behavior requires adjustment.

Tests must use fakes, stubs, in-memory services, temporary files, or captured
console output. Tests must not use real Google credentials, enable
`VMF_PUBLISHER_GOOGLE_E2E`, mutate Google Docs or Drive, create packages, or
execute the Avast-pending package.

## 12. Task 3 Implementation Plan

Task 3 should proceed in the following order:

1. Preflight
   - inspect `git status --short`;
   - confirm no unrelated user changes in target files;
   - keep Live E2E disabled;
   - do not touch `dist/` or release artifacts.
2. CLI logger context
   - extend internal `StructuredPublisherLogger` state or methods to carry
     `command`, `phase`, and `operation`;
   - keep `IPublisherLogger.Warning(string code, string message)` unchanged;
   - ensure every structured write includes all required standard fields.
3. Command normalization
   - normalize command before logging;
   - map empty arguments to `none`, help to `help`, recognized commands to
     their command names, and unknown commands to `unknown`.
4. Session and command lifecycle events
   - change `SESSION_STARTED` message to command-independent wording;
   - add `COMMAND_STARTED`;
   - add `COMMAND_COMPLETED` or `COMMAND_FAILED` without changing exit codes.
5. Summary behavior
   - keep existing result codes and exit behavior;
   - add required standard fields to summaries;
   - keep compatibility `documentId` and `documentUrl` only where already
     emitted.
6. Safe messages
   - replace external exception message exposure with classification-based safe
     messages;
   - optionally include simple `exceptionType`;
   - avoid logging raw path-like input values.
7. Warning messages
   - preserve existing warning codes;
   - normalize warning messages only where required by this specification;
   - keep sensitive context out of warning payloads.
8. Focused tests
   - update CLI console-capture JSON tests;
   - add coverage for required fields and safe messages;
   - add warning-code coverage when warning behavior changes.
9. Local verification
   - run only the focused unit tests and local checks authorized by the Task 3
     instruction;
   - do not run Live E2E;
   - do not create or update release artifacts.

## 13. Task 3 Change Targets

Expected change targets:

| Path | Expected Change |
| --- | --- |
| `src/Publisher.Cli/Program.cs` | Internal structured logging fields, lifecycle events, command/phase/operation mapping, safe message behavior. |
| `tests/unit/Publisher/CliApplicationTests.cs` | Required JSON fields, command/phase/operation, lifecycle, safe exception, and compatibility assertions. |
| `tests/unit/Publisher/PublishPlanExecutorTests.cs` | Warning code/message assertions only if warning behavior changes. |

Do not change:

| Path Or Area | Reason |
| --- | --- |
| `specs/` | Frozen and authoritative specifications are out of scope. |
| `src/Publisher/Application/ITemporaryImageHost.cs` | Contains public `IPublisherLogger`; public API change is prohibited. |
| `src/Publisher/Application/` public contracts | Public API and application contract preservation. |
| `src/Publisher/Domain/` | Diagnostic logging should not affect domain model. |
| `src/Publisher/Infrastructure/Google/` | Avoid production publish behavior changes unless warning text requires a narrowly scoped update. |
| `tests/integration/Publisher/GoogleDocsEndToEndIntegrationTests.cs` | Live E2E is out of scope. |
| `dist/` | Release artifacts must not be generated or updated. |
| `tools/publisher/package-publisher.ps1` | Package creation is out of scope. |
| `tools/publisher/verify-package.ps1` | Package verification behavior is out of scope. |
| `docs/distribution/ReleaseChecklist.md` | Release approval state is out of scope. |

## 14. Acceptance Criteria

This specification is complete when:

- standard fields are defined;
- `SESSION_STARTED` command-independent wording is specified;
- command start, completion, and failure naming is defined;
- Publish, Verify, DryRun, and Diff phase and operation names are defined;
- summary required and optional fields are defined;
- redaction and `SafeMessage` rules are defined;
- warning code and message rules are defined;
- `documentId` and `documentUrl` compatibility is documented;
- Task 3 test viewpoints are listed;
- Task 3 change targets and non-targets are explicit;
- no production code, tests, Frozen specifications, public APIs, release
  artifacts, Google Docs, Google Drive, Live E2E state, or Avast-pending
  package state are changed by this Task 2 document.
