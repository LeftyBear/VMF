# P9-86 - Parser Verification Evidence-Generation Correction GO / NO-GO

## Status

COMPLETE / docs-only evidence-generation correction GO decision

## Purpose

Review the complete P9-85 caller, Windows PowerShell 5.1 parser-only verifier,
structured-evidence, output-capture, exit-propagation, fail-closed, and Avast
correlation boundaries, then decide whether exactly one later separate
parser-verification evidence-generation correction execution may be authorized.

P9-86 is documentation only. It does not materialize or inspect either fixed
file, implement the caller, invoke a parser, run PowerShell, execute the
lifecycle, start Excel, access or mutate a workbook, fixture, or process,
restore an Avast quarantine item, add an Avast exception, exclusion, or
allow-list entry, change Avast settings, rerun the detected target, update
implementation, tests, tools, package or `dist` artifacts, perform release or
publication work, access external services, stage, commit, push, or change
public APIs, persisted schemas, canonical formats, or Frozen specifications.

## Reviewed Evidence

P9-86 reviewed P9-81 through P9-85 and the synchronized backlog,
current-status, and handoff records.

P9-81 remains `INCOMPLETE / NO-GO`. Its fixed-input materialization checks
passed and its Windows PowerShell 5.1 verification process exited `0`, but the
required structured JSON evidence was absent. Token count, parse-error count,
and `ParseFile` completion remain unproven, parser PASS is not recognized, and
the single P9-80-authorized invocation remains consumed.

P9-83 confirms one Avast `IDP.HELU.PSE90 - コマンド ライン検出` block event
associated with Windows PowerShell. Its causal relationship to the absent P9-81
evidence remains `UNPROVEN`; it is correlated security evidence, not proof of
the failure cause and not authority to change or bypass a security control.

P9-84 fixes the minimum correction-planning boundaries. P9-85 then defines one
complete corrected evidence-generation input across a direct-process caller,
the unchanged P9-79 target, a parser-only verifier file, strict structured JSON,
separately retained stdout and stderr, explicit child and caller results,
caller-side schema and consistency validation, failure closure, and
cross-boundary observations.

## Correction Readiness Assessment

The P9-85 input is internally consistent and sufficiently bounded for exactly
one later separate parser-only correction execution. The executable, ordered
argument vector, working directory, stream handling, completion limit, fixed
target path and identity, verifier source and file format, parser API, result
schema, exit mappings, acceptance condition, and no-retry rule are fixed.

The boundary is fail-closed. Caller success is available only after the child
exits `0`, both streams reach EOF, exact stdout and stderr bytes are retained,
stderr is empty, exactly one LF-terminated strict UTF-8 JSON object is decoded,
and every schema, identity, count, and consistency check passes. Missing,
invalid, truncated, inconsistent, or unretained evidence cannot be repaired,
reconstructed, normalized, inferred, or promoted to success. A child exit code
of `0` alone is not parser PASS.

The P9-79 lifecycle-script path, observed byte length, SHA-256, UTF-8 BOM,
CRLF-only form, final CRLF, and parser-only semantics remain unchanged. The
later execution may read and byte-validate that target and call
`Parser.ParseFile`; it must not dot-source, import, invoke, or execute it.

Security boundaries also remain explicit. The confirmed P9-83 Avast event may
be retained and correlated with process identity, timestamps, streams, child
result, caller result, and `OutcomeId`, but temporal proximity cannot establish
causation. The later execution must inherit current security controls and must
not restore, exempt, exclude, allow-list, disable, weaken, or bypass them. A
security intervention, incomplete observation, or missing additional security
record fails the parser result closed where applicable and otherwise remains
unknown.

The correction execution cannot establish lifecycle execution readiness,
residual-process timing evidence, or the complete writable lifecycle success
path. Those conclusions require later, separately authorized review and
lifecycle work even if the parser-only result is PASS.

## Decision

Decision: `GO` for recording P9-86 as the docs-only parser-verification
evidence-generation correction GO / NO-GO decision.

Decision: `PASS` for the internal consistency, determinism, fail-closed
behavior, parser-only boundary, and security-control preservation of the
complete P9-85 input.

Decision: `GO` for exactly one later separate P9-87 parser-verification
evidence-generation correction execution using only the complete P9-85
contract. That task may materialize the exact P9-85 verifier file, realize the
fixed direct-process caller, and perform exactly one parser-only child
invocation against the unchanged P9-79 target. It must retain the exact raw
streams and required process observations and apply the P9-85 child and caller
result mappings without correction or retry.

Decision: `UNPROVEN` for the exact cause of the P9-81 missing structured JSON
evidence and for causation by the confirmed P9-83 Avast event.

Decision: `INCOMPLETE / NO-GO` remains authoritative for P9-81. Its process
exit code `0` is not parser PASS, and the P9-80 invocation remains consumed.
P9-86 creates one new, distinct P9-87 authorization; it does not reopen or
reinterpret the consumed P9-80 authorization.

Decision: `NO-GO` for materializing either file, implementing the caller,
invoking a parser, running PowerShell, or executing the correction from P9-86
itself; for changing the P9-79 or P9-85 fixed inputs; for a second P9-87
invocation, correction or retry during P9-87, alternate target, evidence
reconstruction, or normalization; for dot-sourcing, importing, invoking, or
executing the lifecycle script; for lifecycle, Excel, workbook, fixture, or
process operations; for changing or bypassing Avast controls; and for package
/ `dist`, release / publication, external services, staging, commit, push,
public API change, persisted schema change, canonical format change, or Frozen
specification change.

Decision: `NO-GO` for claiming parser PASS before a valid accepted P9-85
caller result, or for claiming lifecycle readiness, residual-process timing
evidence, complete writable lifecycle success-path evidence, Avast causation,
vendor clearance, or safety certification from P9-86 or from a later
parser-only PASS.

## Selected Next Candidate

**P9-87 - Parser Verification Evidence-Generation Correction Execution**

P9-87 may materialize only the exact P9-85 verifier file, realize only the
fixed P9-85 direct-process caller contract, and perform exactly one
parser-only invocation against the unchanged P9-79 target. It must preserve
the exact stdout and stderr bytes, child process ID, start and completion
timestamps, child exit code, caller result, and any valid structured result,
then record the outcome without correction, retry, reconstruction, or causal
inference.

P9-87 must not dot-source, import, invoke, or execute the lifecycle script;
run the lifecycle command; start Excel; open, inspect, save, or mutate a
workbook or fixture; query, terminate, or mutate an Excel process; change or
bypass Avast controls; perform a second parser invocation; or infer lifecycle,
implementation beyond the fixed caller, package / `dist`, release /
publication, external-service, or Git-write authorization from P9-86.

## Preserved Invariants

P9-86 preserves the consumed P9-80 authorization, P9-81 `INCOMPLETE / NO-GO`,
the P9-79 fixed target identity and parser-only semantics, the P9-85 caller and
verifier contract, retained validated structured JSON and zero parse errors as
prerequisites for caller success, causal uncertainty, the confirmed P9-83
security event without causal promotion, unchanged security controls, and the
separation between docs-only GO / NO-GO, one correction execution, result
review, lifecycle GO / NO-GO, and lifecycle execution.

## Verification

P9-86 verification is documentation-only: review P9-81 through P9-85 and the
synchronized current state; confirm that the P9-85 contract is complete,
deterministic, fail-closed, parser-only, and security-control preserving; run
`git diff --check`; scan the four changed Markdown files for trailing
whitespace; and inspect Git branch and staged / unstaged state. No parser,
PowerShell, lifecycle, Excel, workbook, fixture, process, Avast,
implementation test, build, package / `dist`, stage, commit, or push operation
is run.
