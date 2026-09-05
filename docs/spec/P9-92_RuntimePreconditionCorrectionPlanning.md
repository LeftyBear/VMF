# P9-92 - Residual-Process Evidence Runtime-Precondition Correction Planning

## Status

COMPLETE / docs-only runtime-precondition correction planning

## Purpose

Define the minimum evidence, correction boundary, and safety conditions needed
to address the P9-90 pre-Excel `Get-FileHash` command-availability failure
without changing or rematerializing the P9-79 fixed target, inferring an
unobserved cause, or authorizing another lifecycle execution.

P9-92 is documentation only. It does not inspect, rewrite, normalize,
rematerialize, or invoke the fixed target, run PowerShell or a runtime probe,
start Excel, open, save, or mutate a workbook or fixture, query, terminate, or
mutate a process, change or bypass a security control, change implementation,
tests, or tools, update package or `dist` artifacts, perform release or
publication work, access external services, stage, commit, push, or change
public APIs, persisted schemas, canonical formats, or Frozen specifications.

## Authoritative Problem Boundary

P9-90 retained a Windows PowerShell 5.1 child exit code `1`, empty stdout, and
stderr identifying a `Get-FileHash` `CommandNotFoundException` at line `23`,
character `73` during the first pre-operation identity calculation. No fixed
JSON event, including `PRE_OPERATION`, was emitted. Excel creation and every
workbook, PID-correlation, lifecycle, immediate-acceptance, diagnostic, and
final fixed-target observation were not reached.

This evidence proves only that the named command was unavailable to that
invocation at that source location. It does not prove why it was unavailable,
whether a module, command-discovery, environment, policy, or other runtime
condition was responsible, or whether a later invocation would behave the
same way. The mojibake explanatory text and the separate Avast event do not
close that causal gap.

The P9-79 target identity remains fixed at length `8264`, SHA-256
`80749CA24C4CF7A8BF27CF8D55633B526FF64FFBF6CE8216590C609060B1F353`,
UTF-8 with BOM, CRLF-only lines, and exactly one final CRLF. It is historical
execution input and must not be changed or rematerialized under P9-92.

## Minimum Correction Input

A later correction-input definition must provide one complete successor input
that removes `Get-FileHash` as a runtime prerequisite. The minimum acceptable
input must:

- retain the direct Windows PowerShell 5.1 `-File` transport, executable,
  ordered arguments, working directory, fixed paths, expected lengths,
  expected SHA-256 values, expected attributes, and fixture-count requirement;
- define a self-contained SHA-256 calculation using only runtime facilities
  available without command or module discovery, and define deterministic
  stream disposal and uppercase hexadecimal output equivalent to the existing
  expected 64-character SHA-256 values;
- use that single calculation path for every pre-operation and post-operation
  fixture identity calculation, without fallback to `Get-FileHash`, another
  executable, an external tool, or alternate workbook selection;
- preserve the P9-72 plus P9-74 lifecycle sequence, evidence-event names and
  fields, HWND-to-PID correlation, close-without-saving behavior, COM-release
  order, immediate acceptance point, diagnostic offsets and maximum window,
  classifications, final-state checks, and exit-code rules except for the
  minimum identity-calculation substitution;
- specify exact script bytes, encoding, newline form, path, length, and SHA-256
  so later reviews do not reconstruct or normalize the successor input; and
- provide line-by-line traceability showing that no other runtime or lifecycle
  semantic change is introduced.

This planning record does not select exact correction text. Exact code and
successor byte identity belong only to a later correction-input definition.

## Minimum Readiness Evidence

Before any later lifecycle execution can be considered, separate retained
evidence must establish all of the following for the exact successor input:

1. byte, encoding, newline, and fixed-path identity;
2. Windows PowerShell 5.1 `Parser.ParseFile` completion with structured token
   count, parse-error count, and parse-error details;
3. a non-lifecycle runtime-precondition verification that executes only the
   successor identity-calculation mechanism against the two authoritative
   fixtures and retains the executable identity, ordered arguments, working
   directory, PowerShell version information, stdout, stderr, exit code, both
   observed lengths and SHA-256 values, and fixture count; and
4. exact agreement with replacement-fixture length `8342` and SHA-256
   `220322FA5416DC7A10CD80BFBCEE0AE277D699175FF7A874E5DE3163D7FC301B`,
   historical-fixture length `3532` and SHA-256
   `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`,
   attributes `Archive`, and exactly two `.xlsm` fixtures.

The runtime-precondition verification must not dot-source or invoke the
lifecycle target, create Excel, open a workbook, perform PID correlation,
execute lifecycle or timing observations, mutate a fixture or process, or
terminate a process. A missing field, nonzero exit code, stderr, identity
mismatch, unexpected process observation, security intervention, or
incomplete structured evidence is a fail-closed result. Parser-only evidence
does not substitute for runtime-precondition evidence, and neither authorizes
a lifecycle execution.

## Authorization Sequence

The required later sequence remains separated:

1. docs-only correction-input definition;
2. docs-only parser-verification GO / NO-GO;
3. separately authorized parser-only verification and result review;
4. docs-only runtime-precondition verification GO / NO-GO;
5. separately authorized non-lifecycle runtime-precondition verification and
   result review;
6. docs-only lifecycle-execution GO / NO-GO; and
7. at most one separately authorized lifecycle execution.

No earlier step authorizes a later step. Any failure consumes only the exact
single invocation authorized for that step and does not authorize correction,
retry, substitution, fallback, or progression.

## Decision

Decision: `GO` for recording P9-92 as the docs-only runtime-precondition
correction plan.

Decision: `PASS` for defining the minimum successor-input, semantic-
equivalence, parser-readiness, isolated runtime-precondition evidence, and
authorization-separation requirements.

Decision: `UNPROVEN` remains authoritative for the cause of the P9-90 command
unavailability, successor parser and runtime readiness, Excel creation,
writable lifecycle success, and residual-process timing PASS.

Decision: `NO-GO` for defining or applying exact correction code,
rematerializing or invoking the P9-79 target, running a parser or runtime
probe, executing or retrying the lifecycle, Excel automation, workbook or
fixture operation, process query / mutation / termination, fallback, security-
control change or bypass, implementation or test changes, package / `dist`,
release / publication, external services, staging, commit, push, public API
change, persisted schema change, canonical format change, or Frozen
specification change from P9-92.

## Selected Next Candidate

**P9-93 - Residual-Process Evidence Runtime-Precondition Correction Input
Definition**

P9-93 should remain documentation only and define exactly one complete
successor input that replaces every `Get-FileHash` dependency with one self-
contained SHA-256 mechanism while preserving all other P9-79 transport and
P9-72 plus P9-74 lifecycle semantics. It must define exact bytes and a
parser-only verification method, but must not materialize or invoke the input,
run runtime-precondition verification, execute the lifecycle, or infer broader
authorization from P9-92.

## Preserved Invariants

P9-92 preserves the consumed P9-89 authorization, P9-90 operation-failure
result, P9-79 historical target identity, exact fixture identities and count,
P9-72 plus P9-74 lifecycle semantics, close without saving, immediate
residual-process HARD-STOP, no fallback, no process termination, causal
uncertainty, current security controls, and separation between planning,
input definition, parser evidence, runtime-precondition evidence, lifecycle
authorization, and any later execution.

## Verification

P9-92 verification is documentation-only: review P9-89 through P9-91 and the
synchronized current state; confirm that the plan defines evidence and safety
boundaries without defining or executing a correction; run `git diff
--check`; scan the four changed Markdown files for trailing whitespace; and
inspect Git branch and staged / unstaged state. No fixed-target inspection or
correction, parser or PowerShell invocation, Excel, workbook, fixture, process,
security-control, implementation test, build, package / `dist`, release,
publication, external-service, stage, commit, or push operation is run.
