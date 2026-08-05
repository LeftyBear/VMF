# ADR-0006: Diagnostic Logging and Safe Observability

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher diagnostic logging, structured stderr diagnostics, safe observability, lifecycle events, safe messages, and redaction boundary
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md, docs/architecture/ADR_INDEX.md, docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md, docs/development/Publisher_v1.0_Implementation_Voyage_Log.md

## Context

Publisher local CLI runs need diagnostic evidence that is predictable,
machine-readable, and safe to retain in local review records. Phase 4-2-1
records the detailed Diagnostic Logging Specification and implementation
review. This ADR records the durable architecture decision for diagnostic
logging and safe observability.

Diagnostics must support troubleshooting and release-review evidence without
turning logs into a source of secrets, private document references, local
operator paths, raw provider payloads, or implementation internals. The
decision applies to Publisher structured diagnostic logs and related safe
message policy. It does not change existing implementation behavior,
production code, public APIs, persisted schemas, canonical formats, tests,
runbooks, release records, or verification evidence.

## Decision

Publisher diagnostic logs use structured JSON as the standard format.
User-facing command results are written to stdout. Structured diagnostic log
events are written to stderr as one JSON object per log line.

Every structured diagnostic event must treat these as basic fields:

- `sessionId`;
- stable event `code`;
- `level`;
- `timestampUtc`.

The Publisher structured diagnostic context also carries the command, phase,
operation, and safe message fields defined by the existing Phase 4-2-1
Diagnostic Logging Specification.

Publisher diagnostic logging records lifecycle events for:

- session;
- command;
- phase;
- operation;
- summary;
- warning.

Event codes must be stable, uppercase, and suitable for tests, diagnostics,
and local evidence review. Levels must remain bounded and meaningful; logs are
not a raw verbose transcript.

## Safe Observability Boundary

Publisher diagnostics require safe messages and redaction policy before data
is serialized. Logs must not include:

- raw exception messages;
- stack traces;
- OAuth tokens, access tokens, refresh tokens, cookies, secrets, private keys,
  credentials, credential JSON, credential paths, token-store paths, or
  Authorization headers;
- raw HTTP request or response bodies;
- local paths, including Markdown input paths, local image paths, temp paths,
  package paths, and user profile paths;
- private URLs, private Google Docs or Google Drive URLs, temporary public
  URLs, raw external URIs, or temporary resource identifiers;
- provider payloads or implementation-specific exception details.

Allowed diagnostics are intentionally small and stable: event code, level,
timestamp, session id, command, phase, operation, safe fixed message,
classification, exit code, elapsed milliseconds, numeric HTTP status when
safe, retry metadata, and other explicitly reviewed value-safe fields.

Safe messages must be allow-listed or otherwise reviewed as value-free before
they are logged. Pattern redaction may be used only as a defensive backup; it
must not be the primary protection for raw exception, provider, credential, or
path content.

## Rejected Alternatives

Plain text only logging is rejected because it is harder to test, harder to
parse, and weaker as local evidence for release review.

Raw exception logging is rejected because provider and platform exception
messages can contain paths, private URLs, response bodies, identifiers,
credential metadata, or other sensitive values.

Unbounded verbose logging is rejected because it increases evidence-handling
risk, makes stable tests brittle, and can expose implementation internals
without improving the supported diagnostic contract.

## Non-Adopted Scope

This ADR does not adopt or require:

- external log collection infrastructure;
- OpenTelemetry;
- distributed tracing;
- monitoring service integration.

Those capabilities may be reconsidered separately only through a later
authorized design and implementation task. They are not part of Phase 4-2-1
and are not implied by this ADR.

## Relationship To Other ADRs And Records

ADR-0001 governs how this ADR is recorded, indexed, accepted, and later
superseded.

ADR-0003 records the release gate and vendor-clearance boundary. ADR-0006 does
not reopen the release gate, resolve Avast false-positive handling, obtain
vendor clearance, accept antivirus risk, authorize release, authorize package
work, authorize Live E2E, or authorize Google Docs / Drive mutation.

Phase 4-2-1 remains the detailed Diagnostic Logging Specification and
implementation record. ADR-0006 records the durable observability decision and
points reviewers back to Phase 4-2-1 for detailed field catalogs, lifecycle
events, command and phase mappings, safe-message examples, and local-only test
evidence.

## Consequences

Publisher diagnostics remain suitable for local troubleshooting, automated
tests, and redacted evidence review. The CLI keeps stdout reserved for
user-facing command results while stderr carries structured diagnostics.

Future diagnostic changes must preserve structured JSON logs, stable event
codes, safe messages, bounded fields, and the redaction boundary. Any new field
must be explicitly safe before it is emitted.

Observability improvements cannot be used to bypass release controls. Release
remains blocked until the separately governed release conditions are satisfied.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Proposed | Initial diagnostic logging and safe observability ADR drafted as docs-only / local-only documentation. |
| 2026-08-05 | Accepted | Diagnostic logging and safe observability accepted as the durable observability decision. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0001-architecture-decision-record-process.md`
- `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md`
- `docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md`
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
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not replace implementation specifications, Phase 4-2-1
  development records, runbooks, release records, verification evidence, or
  current status records.
- This ADR does not introduce external log collection infrastructure,
  OpenTelemetry, distributed tracing, or monitoring service integration.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, flagged executable execution, vendor submission, or
  vendor clearance.
- This ADR does not claim release readiness, Avast false-positive resolution,
  risk acceptance, final release verification, or publication completion.
