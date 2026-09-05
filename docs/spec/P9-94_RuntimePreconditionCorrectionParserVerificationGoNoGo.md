# P9-94 - Runtime-Precondition Correction Parser Verification GO / NO-GO

## Status

COMPLETE / docs-only runtime-precondition correction parser verification GO decision

## Purpose

Review the exact P9-93 successor identity, direct Windows PowerShell 5.1
`-File` transport, self-contained SHA-256 substitution, semantic-equivalence
account, parser-only verification method, failure closure, and prohibited-
operation boundaries, then decide whether one later separate parser-only
verification may be authorized.

P9-94 is documentation only. It does not materialize or inspect either the
historical P9-79 target or the P9-93 successor, invoke a parser or runtime
probe, run PowerShell, execute or retry the lifecycle, start Excel, access or
mutate a workbook, fixture, or process, change or bypass a security control,
change implementation, tests, or tools, update package or `dist` artifacts,
perform release or publication work, access external services, stage, commit,
push, or change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Reviewed Evidence

P9-94 reviewed P9-90 through P9-93 and the synchronized backlog,
current-status, and handoff records.

P9-90 remains `COMPLETE / HARD-STOP / OPERATION FAILURE`. Its one authorized
invocation exited `1` before `PRE_OPERATION` because `Get-FileHash` was
unavailable during the first pre-operation identity calculation. Excel and
all lifecycle and timing observations were not reached. P9-91 affirms that
result, the consumed P9-89 authorization, and the final local safe-state audit
without promoting the exact command-unavailability cause from `UNPROVEN`.

P9-92 fixes the minimum correction-planning boundaries. P9-93 then defines one
complete successor at
`C:\Users\biz\AppData\Local\Temp\VMF-P9-93-ResidualProcessEvidence.ps1`:
`8465` UTF-8-BOM bytes, CRLF-only lines with exactly one final CRLF, and
SHA-256 `805098C3BCA120E5FBBBF0B2FFC6511FDBB21A19FFE4BC6B629EF4416CF3B208`.
It preserves the direct Windows PowerShell 5.1 `-File` transport and replaces
only the `Get-FileHash` dependency with a read-only .NET file stream,
`SHA256.Create`, uppercase hexadecimal conversion, and deterministic disposal.

Every other P9-79 and P9-72 plus P9-74 lifecycle, evidence, timing, hard-stop,
and exit semantic remains unchanged. The historical P9-79 target remains
immutable and is neither a correction target nor an alternate input.

## Parser-Verification Readiness Assessment

The P9-93 successor is internally consistent and sufficiently bounded for
exactly one later parser-only verification. The path, definition-time byte
identity, encoding, newline form, transport, parser host and API, required
structured evidence, PASS condition, semantic-equivalence account, and
prohibited operations are fixed.

The verification is fail-closed. It must validate the exact path, length,
SHA-256, UTF-8 BOM, CRLF-only form, and one final CRLF before calling Windows
PowerShell 5.1
`[System.Management.Automation.Language.Parser]::ParseFile`. PASS requires
zero parse errors and retained, valid, encoding-stable structured JSON with
the exact identity, token count, parse-error count, and every parse-error
diagnostic field. An absent, malformed, truncated, inconsistent, or unretained
result is not PASS, and child exit code `0` alone is not PASS.

The parser-only boundary is explicit. The later task may materialize only the
exact P9-93 successor and parse it once. It must not dot-source, import, invoke,
or execute the successor; calculate fixture identities; perform a runtime-
precondition probe; create Excel; open a workbook; query an Excel process; or
perform any lifecycle operation. Parser PASS therefore cannot establish the
self-contained SHA-256 path's runtime readiness, lifecycle execution
readiness, residual-process timing PASS, or complete writable lifecycle
success-path evidence.

## Decision

Decision: `GO` for recording P9-94 as the docs-only runtime-precondition
correction parser verification GO / NO-GO decision.

Decision: `PASS` for the internal consistency, determinism, fail-closed
behavior, semantic-equivalence account, parser-only boundary, and security-
control preservation of the complete P9-93 successor and verification method.

Decision: `GO` for exactly one later separate P9-95 runtime-precondition
correction parser verification using only the exact P9-93 successor path and
bytes, Windows PowerShell 5.1 parser API, identity and newline checks,
structured evidence fields, and PASS condition. P9-95 may materialize the
exact successor once and perform exactly one parser-only invocation without
correction, retry, substitution, reconstruction, or normalization.

Decision: `NO-GO` for materializing or parsing either target from P9-94; for
changing or rematerializing the historical P9-79 target; for changing the
P9-93 successor; for a second P9-95 invocation, correction or retry, alternate
path, encoding or newline normalization, evidence reconstruction, dot-
sourcing, importing, invoking, or executing the successor; for a runtime
probe, fixture-identity calculation, lifecycle execution, Excel automation,
workbook or fixture operation, Excel-process query / mutation / termination,
fallback, or security-control change or bypass; and for implementation or test
changes, package / `dist`, release / publication, external services, staging,
commit, push, public API change, persisted schema change, canonical format
change, or Frozen specification change.

Decision: `UNPROVEN` remains authoritative for successor parser readiness
until accepted P9-95 evidence exists, successor runtime-precondition readiness,
the exact cause of the P9-90 command unavailability, Excel creation, writable
lifecycle success, and residual-process timing PASS.

## Selected Next Candidate

**P9-95 - Runtime-Precondition Correction Parser Verification**

P9-95 may materialize the exact P9-93 successor once at its fixed temporary
path using UTF-8 BOM, CRLF-only lines, and one final CRLF, validate its exact
byte identity, and perform exactly one Windows PowerShell 5.1 parser-only
verification. It must retain the required structured JSON and apply the P9-93
PASS condition without correction or retry.

P9-95 must not inspect or rematerialize the historical P9-79 target; dot-source,
import, invoke, or execute the successor; calculate fixture identities; run a
runtime-precondition probe or lifecycle command; create Excel; open, inspect,
save, or mutate a workbook or fixture; query, terminate, or mutate an Excel
process; change or bypass security controls; perform a second parser
invocation; or infer runtime, lifecycle, implementation, package / `dist`,
release / publication, external-service, or Git-write authorization from
P9-94.

## Preserved Invariants

P9-94 preserves the consumed P9-89 authorization, P9-90 operation-failure
result, immutable P9-79 historical identity, exact P9-93 successor definition,
exact fixture identities and count, P9-72 plus P9-74 lifecycle semantics,
close without saving, immediate residual-process HARD-STOP, no fallback, no
process termination, causal uncertainty, current security controls, and the
separation between parser GO / NO-GO, one parser-only verification, parser
result review, runtime-precondition GO / NO-GO, isolated runtime-precondition
evidence, lifecycle GO / NO-GO, and any later separately authorized lifecycle
execution.

## Verification

P9-94 verification is documentation-only: review P9-90 through P9-93 and the
synchronized current state; confirm that the P9-93 successor identity,
transport, SHA-256 substitution, semantic-equivalence account, parser method,
structured-evidence requirements, PASS condition, fail-closed behavior, and
prohibited-operation boundaries are complete and internally consistent; run
`git diff --check`; scan the four changed Markdown files for trailing
whitespace; and inspect Git branch and staged / unstaged state. No target is
materialized or inspected, and no parser, PowerShell child, runtime probe,
lifecycle, Excel, workbook, fixture, process, security-control, implementation
test, build, package / `dist`, release, publication, external-service, stage,
commit, or push operation is run.
