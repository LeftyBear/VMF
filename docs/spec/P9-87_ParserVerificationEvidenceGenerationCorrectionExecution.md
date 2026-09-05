# P9-87 - Parser Verification Evidence-Generation Correction Execution

## Status

COMPLETE / PASS for the single parser-only correction execution

## Purpose

Record the result of the one P9-86-authorized parser-verification
evidence-generation correction execution using only the fixed P9-85 caller and
verifier contract against the unchanged P9-79 target.

P9-87 materialized only the exact P9-85 verifier, realized only the fixed
direct-process caller, and performed exactly one Windows PowerShell 5.1
parser-only child invocation. It did not correct or retry the invocation,
dot-source, import, invoke, or execute the lifecycle script, start Excel,
access a workbook or fixture, query or mutate an Excel process, or change or
bypass an Avast control.

## Materialized Verifier

- Path: `C:\Users\biz\AppData\Local\Temp\VMF-P9-85-ParserEvidence.ps1`
- Length: `2607` bytes
- SHA-256: `F3D70467EEB8FA0E067E06086DB718B0FBB8A19C7BF38340F2BA898410146163`
- UTF-8 BOM: `PASS`
- CRLF-only: `PASS`
- Exactly one final CRLF: `PASS`
- Exact P9-85 source match: `PASS`

These verifier identity values describe the P9-85 source materialized for this
execution. They do not alter the P9-79 target identity.

## Retained Process Observations

- Caller: `P9-85 parser-evidence caller`
- Caller working directory: `C:\Users\biz\Documents\Project\VMF`
- Child executable: `C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe`
- Start timestamp UTC: `2026-09-05T02:10:05.3721388+00:00`
- Completion timestamp UTC: `2026-09-05T02:10:06.8971126+00:00`
- Child process ID: `9080`
- Child exit code: `0`
- Caller result: `0`
- stdout length: `327` bytes
- stdout SHA-256: `0D662B11CFB1430417BB1C95FDCFF80CF5F61FB122D87EDB5F0141E07A4DFEE1`
- stderr length: `0` bytes
- stderr SHA-256: `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855`
- Outcome ID: `753d7f8e-ae27-43c8-a1a8-fd48d65f78b0`
- Timeout, interruption, stream, EOF, or retention failure: none observed

The caller began draining both redirected streams before waiting, observed
child exit and EOF on both streams inside the 30,000 millisecond completion
limit, retained the exact raw bytes, and applied the P9-85 validation and exit
mapping. No second invocation occurred.

## Structured Parser Evidence

The retained stdout contained exactly one LF-terminated UTF-8 JSON object and
no other bytes. Caller validation accepted its schema, field set, types,
identity, counts, child-exit consistency, and empty stderr.

| Field | Retained value |
|---|---|
| Schema | `VMF.P9.ParserEvidence.v1` |
| OutcomeId | `753d7f8e-ae27-43c8-a1a8-fd48d65f78b0` |
| FilePath | `C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1` |
| ByteLength | `8264` |
| SHA256 | `80749CA24C4CF7A8BF27CF8D55633B526FF64FFBF6CE8216590C609060B1F353` |
| TokenCount | `1404` |
| ParseErrorCount | `0` |
| ParseErrors | empty array |

The valid structured result proves that `Parser.ParseFile` completed for the
fixed P9-79 target during this single invocation and returned zero parse
errors. The P9-85 acceptance condition is satisfied, so parser PASS is
recognized for P9-87.

## Decision

Decision: `COMPLETE / PASS` for P9-87. The one P9-86-authorized invocation was
performed once, and its retained evidence satisfies the P9-85 caller result
`0` acceptance contract.

Decision: `PASS` for fixed-target byte identity, strict UTF-8 structured
evidence, token count `1404`, parse-error count `0`, empty stderr, child exit
`0`, caller result `0`, and complete required process observations.

Decision: `UNPROVEN` remains authoritative for the exact cause of the P9-81
missing structured JSON evidence and for causation by the confirmed P9-83
Avast event. P9-87 does not reinterpret the consumed P9-80 invocation or
overwrite the historical P9-81 `INCOMPLETE / NO-GO` result.

Decision: `NO-GO` for correction, retry, a second parser invocation, lifecycle
execution, Excel, workbook, fixture, or process operation, changing or
bypassing Avast controls, lifecycle readiness, residual-process timing or
complete writable lifecycle success-path claims, implementation beyond the
fixed caller, package / `dist`, release / publication, external services,
staging, commit, push, public API change, persisted schema change, canonical
format change, or Frozen specification change.

Complete writable lifecycle success-path evidence remains unproven.

## Selected Next Candidate

**P9-88 - Parser Verification Evidence-Generation Correction Result Review**

P9-88 should remain docs-only and review the recorded P9-87 materialization,
single-invocation compliance, retained raw streams, process observations,
structured evidence, caller result, parser PASS, causal uncertainty, and
prohibited-operation boundaries before selecting any later lifecycle-related
candidate.

P9-88 must not rematerialize either file, invoke or retry a parser, execute the
lifecycle script, start Excel, operate on a workbook, fixture, or process,
change or bypass Avast controls, or infer lifecycle, package / `dist`, release
/ publication, external-service, or Git-write authorization from P9-87.

## Preserved Invariants

P9-87 preserves the consumed P9-80 authorization and historical P9-81 result,
the unchanged P9-79 target identity and parser-only semantics, the complete
P9-85 caller and verifier contract, the single P9-86-authorized invocation,
causal uncertainty, the confirmed P9-83 event without causal promotion,
unchanged security controls, and separation between parser-only PASS, result
review, lifecycle GO / NO-GO, and lifecycle execution.

## Verification

P9-87 verification consists of the recorded verifier-format checks, caller
build, exactly one parser-only invocation, retained-evidence inspection,
documentation consistency review, `git diff --check`, trailing-whitespace
scan, and Git branch and staged / unstaged inspection. No lifecycle, Excel,
workbook, fixture, Excel-process, Avast-control, package / `dist`, release,
publication, external-service, stage, commit, or push operation is run.
