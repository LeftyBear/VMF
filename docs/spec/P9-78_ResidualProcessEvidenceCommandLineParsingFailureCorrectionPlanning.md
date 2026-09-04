# P9-78 - Residual-Process Evidence Command-Line Parsing Failure Correction Planning

## Status

COMPLETE / docs-only command-line parsing failure correction planning

## Purpose

Identify the minimum authoritative input required to correct the P9-76
command-line parsing failure before any later corrected-input GO / NO-GO is
considered, without inferring the exact failure cause from mojibake host text.

P9-78 is documentation only. It does not define or apply a corrected command,
execute or retry any command, invoke a PowerShell parser, run Excel automation,
open / create / save / SaveAs / close either workbook, mutate or repair either
fixture, terminate or otherwise mutate any process, change implementation or
test code, run implementation tests, update package or `dist` release
artifacts, perform release or publication work, access external services,
stage, commit, push, or change public APIs, persisted schemas, canonical
formats, or Frozen specifications.

## Failure Boundary

P9-76 establishes only that the single authorized invocation exited `1`
during command-line parsing before `PRE_OPERATION` or any other fixed JSON
evidence event. It does not establish whether the failure originated in the
outer invocation, transport of the multiline script, a quoting boundary, or
Windows PowerShell 5.1 parsing of the received script.

The mojibake-encoded host error text is not authoritative evidence for an
exact character, token, quoting boundary, or host-binding cause. The P9-72
script and P9-74 correction remain the intended semantic input, but their
documentation text alone does not prove the exact character stream received
by the P9-76 host.

## Minimum Authoritative Correction Input

A later correction-input definition must fix and review all of the following
as one traceable input set:

1. **Outer invocation:** the exact Windows PowerShell 5.1 executable path and
   ordered argument vector, including an explicit account of which component
   starts the process and whether another shell parses the command first.
2. **Materialized script:** one complete character-for-character script formed
   from the P9-72 script plus only the P9-74 correction, with no documentation
   metavariable, omitted block, or implicit edit.
3. **Script transport:** the exact mechanism by which that materialized script
   becomes the PowerShell input, including encoding, newline form, and whether
   the script is supplied as a single argument, encoded payload, or exact-path
   local script file.
4. **Quoting boundary:** a character-level account of every quoting or escaping
   layer between the caller and Windows PowerShell 5.1, including preservation
   of the C# here-string delimiters and embedded single and double quotes.
5. **Parser evidence:** non-Excel evidence from Windows PowerShell 5.1 that the
   exact transported script parses as a complete input, with parser diagnostics
   preserved without relying on console-rendered mojibake text.
6. **Equivalence evidence:** a review showing that the proposed transport-only
   correction leaves the P9-72 and P9-74 paths, expected identities,
   preconditions, PID correlation, workbook-open arguments, lifecycle and COM
   release sequence, evidence events, acceptance point, diagnostic timing,
   classifications, and exit-code rules unchanged.

The authoritative correction input is incomplete if any layer is represented
only by a display command, reconstructed from the P9-76 mojibake text, or left
to caller-dependent quoting. Parser evidence may not create Excel, open a
workbook, inspect or mutate a fixture, or perform the lifecycle operation.

## Planned Decision Sequence

The next task should define exactly one corrected transport input and its
parser-only verification method from the requirements above. A subsequent
docs-only GO / NO-GO must then review that fixed input before any separately
authorized execution is considered.

No later lifecycle execution is authorized merely because parser-only evidence
passes. Any later execution must retain the P9-72 and P9-74 fail-closed rules,
use the exact reviewed input once, and stop without correction or retry on any
failure.

## Decision

Decision: `GO` for recording P9-78 as the docs-only command-line parsing
failure correction plan.

Decision: `PASS` for separating the outer invocation, materialized script,
script transport, quoting boundary, Windows PowerShell 5.1 parser evidence,
and semantic-equivalence review as the minimum authoritative correction input.

Decision: `NO-GO` for naming an exact P9-76 root cause from the available
mojibake host text.

Decision: `NO-GO` for defining or applying a correction, invoking a parser,
executing or retrying the lifecycle command, Excel automation, opening or
mutating either fixture, timing or path substitution, fallback workbook or
process selection, process termination, Save, SaveAs, fixture repair,
implementation or test code change, acceptance-criterion change, broader
workbook / VBProject mutation, package / `dist`, release / publication,
external services, staging, commit, push, public API change, persisted schema
change, canonical format change, or Frozen specification change from P9-78.

## Selected Next Candidate

**P9-79 - Residual-Process Evidence Command-Line Parsing Correction Input
Definition**

P9-79 should remain docs-only and define exactly one complete corrected
transport input plus a parser-only Windows PowerShell 5.1 verification method.
It must provide traceability to the P9-72 script and only the P9-74 semantic
correction, account for every outer-invocation and quoting layer, and preserve
parser diagnostics in an encoding-stable form.

P9-79 must not run parser verification or the lifecycle command, run Excel
automation, open either workbook, mutate a fixture or process, terminate a
process, or infer execution, implementation, broader workbook / VBProject,
package / `dist`, release / publication, external-service, or Git-write
authorization from P9-78.

## Preserved Invariants

P9-78 preserves the P9-65 and P9-68 immediate residual-process hard stops and
exit code `1`, the P9-72 fixed acceptance point and timing semantics, the P9-74
authoritative fixture identities and pre-operation checks, the historical
fixture as immutable evidence input, exact paths, close without saving, and
the separation of failure review, correction planning, correction-input
definition, parser-only verification, later GO / NO-GO, and any separately
authorized lifecycle execution.

## Verification

P9-78 verification is documentation-only: review P9-72, P9-74, P9-75, P9-76,
P9-77, and the synchronized current state; confirm the plan neither infers a
root cause nor defines or executes a correction; then run docs-only diff
confirmation, `git diff --check`, trailing-whitespace scan, and Git status
confirmation. No implementation tests, parser invocation, command execution,
Excel automation, workbook operation, fixture identity recheck, or process
mutation are required or run.
