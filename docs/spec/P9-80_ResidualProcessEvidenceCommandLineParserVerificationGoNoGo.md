# P9-80 - Residual-Process Evidence Command-Line Parser Verification GO / NO-GO

## Status

COMPLETE / docs-only command-line parser verification GO decision

## Purpose

Review the complete P9-79 corrected transport input, materialized script
definition, parser-only verification method, and semantic-equivalence account,
then decide whether one later separate parser-only verification may be
authorized.

P9-80 is documentation only. It does not materialize the script file, compute
its observed byte identity, invoke a PowerShell parser, dot-source, import, or
execute the script, execute or retry the lifecycle command, run Excel
automation, open / create / save / SaveAs / close either workbook, inspect,
mutate, or repair either fixture, query, terminate, or otherwise mutate any
Excel process, change implementation or test code, run implementation tests,
update package or `dist` release artifacts, perform release or publication
work, access external services, stage, commit, push, or change public APIs,
persisted schemas, canonical formats, or Frozen specifications.

## Reviewed Evidence

P9-80 reviewed P9-76 through P9-79 and the synchronized backlog,
current-status, and handoff records.

P9-76 hard-stopped before `PRE_OPERATION` because the multiline `-Command`
invocation failed at command-line parsing. P9-77 accepted that result without
inferring the exact cause from mojibake host text. P9-78 fixed the minimum
correction-input categories without authorizing a correction or retry.

P9-79 supplies one complete corrected input: direct process creation of
Windows PowerShell 5.1, an exact `-File` argument vector, a fixed temporary
script path, UTF-8 with BOM, CRLF-only lines with one final CRLF, and the fully
materialized P9-72 lifecycle script with only the P9-74 pre-operation identity
correction. The lifecycle script no longer crosses a command-line quoting
boundary.

P9-79 also fixes a parser-only verification method. It requires byte-level
BOM, strict UTF-8, newline, final-CRLF, length, and SHA-256 observations;
`[System.Management.Automation.Language.Parser]::ParseFile` against the exact
fixed path; token and parse-error collection; and structured UTF-8 JSON
evidence containing the exact file identity, counts, and every parse-error
extent and diagnostic field. PASS requires all byte and newline checks and a
parse-error count of zero.

## Parser-Verification Readiness Assessment

The P9-79 input is internally consistent and sufficiently bounded for exactly
one later parser-only verification. The target file content, path, transport,
encoding, newline form, parser host and API, required observations, evidence
fields, and PASS condition are fixed. The semantic-equivalence account limits
the content change to the P9-74 correction and preserves every P9-72 lifecycle
meaning.

The verification is fail-closed: any materialization mismatch, missing or
incorrect BOM, UTF-8 decoding failure, non-CRLF newline, missing or additional
final newline, parser invocation failure, or nonzero parse-error count denies
PASS. Structured UTF-8 JSON, rather than console rendering, is the
authoritative diagnostic evidence.

The parser-only boundary is also explicit. The later task may materialize only
the exact P9-79 file and parse it. It must not dot-source, import, invoke, or
execute the script; create Excel; open or inspect a workbook or fixture; query
or mutate an Excel process; or perform any lifecycle operation. Therefore the
parser verification cannot establish lifecycle execution readiness or the
complete writable lifecycle success path.

## Decision

Decision: `GO` for recording P9-80 as a docs-only command-line parser
verification GO / NO-GO decision.

Decision: `PASS` for the internal consistency, fail-closed behavior, and
parser-only safety boundary of the complete P9-79 input and verification
method.

Decision: `GO` for exactly one later separate P9-81 command-line parser
verification using only the exact P9-79 fixed path, materialized script bytes,
Windows PowerShell 5.1 parser API, byte and newline checks, structured evidence
fields, and PASS condition.

Decision: `NO-GO` for materializing or parsing the file from P9-80, changing
the P9-79 content or transport, a second parser invocation, correction during
verification, alternate path, encoding or newline normalization, dot-sourcing,
importing, invoking or executing the script, lifecycle execution or retry,
Excel automation, opening or inspecting either workbook or fixture, querying
or mutating an Excel process, process termination, Save, SaveAs, fixture repair
or mutation, implementation or test code change, acceptance-criterion change,
broader workbook / VBProject mutation, package / `dist`, release / publication,
external services, staging, commit, push, public API change, persisted schema
change, canonical format change, or Frozen specification change.

Decision: `NO-GO` for claiming the exact P9-76 parsing root cause, lifecycle
execution readiness, residual-process timing evidence, or complete writable
lifecycle success-path evidence from P9-80 or from a parser-only PASS.

## Selected Next Candidate

**P9-81 - Residual-Process Evidence Command-Line Parser Verification**

P9-81 may materialize the exact P9-79 script once at the fixed temporary path
with UTF-8 BOM, CRLF-only lines, and one final CRLF, then perform exactly one
Windows PowerShell 5.1 parser-only verification and preserve the structured
UTF-8 JSON evidence. It must verify the bytes and newlines before calling
`ParseFile` and apply the P9-79 PASS condition without correction or retry.

P9-81 must not dot-source, import, invoke, or execute the script; run the
lifecycle command; create Excel; open or inspect a workbook or fixture; query
or mutate an Excel process; terminate a process; substitute any input; or infer
lifecycle-execution, implementation, broader workbook / VBProject, package /
`dist`, release / publication, external-service, or Git-write authorization
from P9-80.

## Preserved Invariants

P9-80 preserves the P9-65 and P9-68 immediate residual-process hard stops and
exit code `1`, the P9-72 fixed acceptance point and timing semantics, the
P9-74 authoritative fixture identities and pre-operation checks, the
historical fixture as immutable evidence input, exact workbook paths, close
without saving, and the separation of parser-verification GO / NO-GO,
parser-only verification, parser result review, later lifecycle-execution GO /
NO-GO, and any separately authorized lifecycle execution.

## Verification

P9-80 verification is documentation-only: review P9-76 through P9-79 and the
synchronized current state; confirm that the P9-79 file identity, parser API,
evidence fields, PASS condition, semantic-equivalence account, and prohibited
execution boundaries are complete and internally consistent; then run
docs-only diff confirmation, `git diff --check`, trailing-whitespace scan, and
Git status confirmation. No implementation test, file materialization, parser
invocation, command execution, Excel automation, workbook or fixture
operation, identity recheck, or process query or mutation is required or run.
