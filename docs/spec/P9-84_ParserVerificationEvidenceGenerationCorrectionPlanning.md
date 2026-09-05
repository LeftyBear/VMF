# P9-84 - Parser Verification Evidence-Generation Correction Planning

## Status

COMPLETE / docs-only evidence-generation correction planning

## Purpose

Define the minimum authoritative diagnostic and correction inputs needed before
any corrected parser-verification input or later parser-verification GO / NO-GO
can be considered. This planning preserves the P9-81 missing-evidence result,
the P9-82 causal uncertainty, and the P9-83 confirmed Avast security event
without treating any one boundary as the established cause.

P9-84 is documentation only. It does not inspect or rematerialize the fixed
input, invoke or retry a parser, run PowerShell, execute the lifecycle, start
Excel, access or mutate a workbook, fixture, or process, restore an Avast
quarantine item, add an Avast exception, exclusion, or allow-list entry, change
Avast settings, rerun the detected target, define or apply a correction, change
implementation, tests, or tools, update package or `dist` artifacts, perform
release or publication work, access external services, stage, commit, push, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Established Evidence Boundary

P9-81 establishes that the fixed-input materialization checks passed and that
the Windows PowerShell 5.1 verification process exited `0`, but the required
structured JSON evidence was not generated. Token count, parse-error count,
and `ParseFile` completion therefore remain unproven, and parser PASS is not
recognized. The single P9-80-authorized invocation is consumed.

P9-83 additionally establishes a confirmed Avast
`IDP.HELU.PSE90 - コマンド ライン検出` block event associated with Windows
PowerShell. Its causal relationship to the absent structured JSON evidence is
`UNPROVEN`. Neither the process exit code nor the Avast event establishes
whether the missing evidence originated in the caller, parser invocation,
serialization, output capture, exit-code propagation, security intervention,
or another boundary.

## Minimum Authoritative Correction Inputs

A later correction-input definition must fix and review the following as one
traceable evidence-generation chain:

1. **Caller boundary:** the exact caller identity, process-creation method,
   executable path, working directory, ordered argument vector, environment
   values that affect encoding or output, and the caller's handling of process
   startup and completion.
2. **Fixed-input boundary:** the P9-79 fixed path and the already specified
   byte, SHA-256, UTF-8 BOM, CRLF-only, and final-CRLF checks. The lifecycle
   script bytes and parser-only boundary must not be changed to compensate for
   evidence-generation failure.
3. **Parser-invocation boundary:** the complete non-executing Windows
   PowerShell 5.1 verification body, including strict byte decoding,
   `[System.Management.Automation.Language.Parser]::ParseFile`, token and
   parse-error capture, and an explicit account of every path that can bypass
   or terminate before evidence construction.
4. **Structured-evidence serialization boundary:** the exact result model,
   field names, value sources, `ConvertTo-Json` invocation, depth, encoding,
   and serialization-failure behavior. Required fields remain file path, byte
   length, SHA-256, token count, parse-error count, and every parse error's
   extent text and positions.
5. **Output-capture boundary:** the exact stdout and stderr wiring, byte
   encoding, asynchronous or synchronous read sequence, stream completion
   condition, retained raw bytes, and the rule by which exactly one structured
   JSON result is distinguished from host or diagnostic text.
6. **Exit-code propagation boundary:** the mapping from input-validation,
   parser, serialization, output-write, capture, security-interruption, and
   caller failures to a nonzero process and caller result. Exit code `0` must
   be possible only after the caller has retained and validated the required
   structured JSON evidence.
7. **Security-event boundary:** the P9-83 Avast identifier, displayed process,
   detection name, feature, action, and timestamp, plus any authoritative
   security-product record that can establish ordering or intervention scope.
   Missing security evidence must remain unknown; no exception, bypass,
   control change, or causal inference is permitted.
8. **Cross-boundary correlation:** stable timestamps, process identity, stream
   completion, and outcome identifiers sufficient to correlate the caller,
   parser-verification process, captured output, exit result, and confirmed
   security event without relying on temporal proximity alone.

The input set is incomplete if any executable body, argument, output stream,
exit mapping, or security observation is reconstructed from an assumed result
or represented only by console-rendered or mojibake text.

## Planned Decision Sequence

The next task should define exactly one complete corrected
evidence-generation input from the boundaries above and prove, by docs-only
review, that it preserves the P9-79 fixed script and parser-only semantics,
fails closed when structured evidence is absent or invalid, and neither
bypasses nor weakens Avast controls.

A later separate docs-only GO / NO-GO may review that fixed corrected input.
Only that later decision could authorize one new parser-only verification.
Even a parser-only PASS would not authorize lifecycle execution or establish
complete writable lifecycle success-path or residual-process timing evidence.

## Decision

Decision: `GO` for recording P9-84 as the docs-only parser-verification
evidence-generation correction plan.

Decision: `PASS` for separating the caller, fixed input, parser invocation,
structured-evidence serialization, output capture, exit-code propagation,
security event, and cross-boundary correlation as the minimum authoritative
correction inputs.

Decision: `UNPROVEN` for the exact cause of the missing structured JSON
evidence and for causation by the confirmed Avast event.

Decision: `INCOMPLETE / NO-GO` remains authoritative for P9-81. Exit code `0`
is not parser PASS, and token count, parse-error count, and `ParseFile`
completion remain unproven.

Decision: `NO-GO` for defining or applying a correction, another parser
invocation or retry, PowerShell execution, lifecycle execution, Excel,
workbook, fixture, or process operation, Avast quarantine restoration,
exception / exclusion / allow-list addition, settings change, detected-target
rerun, security-control bypass, implementation / tests / tools change, package
/ `dist`, release / publication, external services, staging, commit, push,
public API change, persisted schema change, canonical format change, or Frozen
specification change from P9-84.

## Selected Next Candidate

**P9-85 - Parser Verification Evidence-Generation Correction Input
Definition**

P9-85 should remain docs-only and define exactly one complete corrected
evidence-generation input across the caller, Windows PowerShell 5.1
parser-only verification, serialization, stdout / stderr capture, and
exit-code propagation boundaries. It must preserve the P9-79 fixed script,
emit and retain the required structured JSON before any success result, fail
closed on absent or invalid evidence, and account for the P9-83 security event
without inferring causation or changing or bypassing Avast controls.

P9-85 must not materialize or inspect the fixed input, invoke a parser, run
PowerShell, execute the lifecycle, start Excel, operate on a workbook, fixture,
or process, change Avast controls, apply a correction at runtime, authorize a
retry, or infer implementation, package / `dist`, release / publication,
external-service, or Git-write authorization from P9-84.

## Preserved Invariants

P9-84 preserves the consumed P9-80 single-invocation authorization, the P9-79
fixed script identity and parser-only boundary, the requirement for retained
structured JSON evidence and zero parse errors before parser PASS, the P9-82
causal uncertainty, the P9-83 confirmed security event without causal
promotion, and the separation between correction planning, correction-input
definition, later parser-verification GO / NO-GO, parser execution, and
lifecycle execution.

## Verification

P9-84 verification is documentation-only: review P9-79 through P9-83 and the
synchronized current state; confirm that the plan neither infers a cause nor
defines, applies, or executes a correction; run `git diff --check`; scan the
four changed Markdown files for trailing whitespace; and inspect Git branch
and staged / unstaged state. No parser, PowerShell, lifecycle, Excel, workbook,
fixture, process, Avast, implementation test, build, package / `dist`, stage,
commit, or push operation is run.
