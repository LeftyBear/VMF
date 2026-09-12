# P9-116 - Technical Restart GO/NO-GO Boundary Decision

## Status

COMPLETE / docs-only technical restart boundary decision / NO-GO TO EXECUTION

## Purpose

Decide whether technical restart may proceed beyond the docs-only planning
boundary reopened by P9-114 and defined by P9-115.

This is a docs-only decision. It does not select a technical execution
candidate, authorize technical execution, or change any security control.

## Reviewed State

| Decision element | Current state | Result |
| --- | --- | --- |
| P9 governance continuation | Reopened for docs-only planning | Preserved |
| Security disposition | Accepted under the P9-112 alternative-evidence basis | Accepted within its recorded limitation |
| Continuation authorization | Accepted only for docs-only planning and reopening-decision preparation | Does not authorize execution |
| Avast detection | Remains unresolved | Must not be treated as resolved |
| Block-time Avast definition/version | Unavailable and unproven | Limitation preserved |
| Technical execution candidate | `None` | Required execution input is absent |
| P9-94 allowance | Not reusable | Preserved |

The accepted alternative-evidence security disposition is not a finding that
the Avast detection is resolved, a safety certification, or technical
execution authorization. The detection remains unresolved, and the Avast
definition/version active at block time remains unavailable and unproven.

## Decision

Decision: `NO-GO TO EXECUTION`.

P9-116 is `COMPLETE / docs-only technical restart boundary decision / NO-GO
TO EXECUTION`.

Technical execution remains `NO-GO / SAFE-STOP`.

Technical execution candidate remains `None`.

P9-94 allowance remains not reusable.

Owner direction to proceed toward technical restart permits this boundary
decision and the next docs-only candidate-selection step. It does not supply
the missing named technical candidate or operation-specific execution
authorization. Therefore technical restart may not proceed beyond docs-only
planning.

## Requirements Before a Future Technical GO May Be Considered

A future GO may be considered only through later, separate docs-only decisions
and separately authorized execution. Before any technical execution GO, the
record must define all of the following without ambiguity:

1. one named minimal technical execution candidate with fixed identity and
   scope;
2. the exact commands allowed for that candidate;
3. the exact commands and operation categories prohibited;
4. pre-execution, in-execution, and post-execution stop criteria;
5. rollback criteria, including when rollback is permitted, required, or
   unsafe, and the required safe-stop state;
6. evidence collection requirements, including command identity, inputs,
   outputs, exit status, timestamps, artifacts, and failure evidence;
7. an absolute prohibition on any Avast workaround, exception, exclusion,
   bypass, allow-list entry, disablement, weakening, or other security-control
   change;
8. no rerun or invocation of any flagged executable unless that exact action is
   separately reviewed and explicitly authorized; and
9. no reuse, revival, extension, or reinterpretation of the P9-94 allowance.

Candidate selection alone will not be execution authorization. After a
candidate is selected, a separate later decision must review the complete
boundary and record an explicit operation-specific GO before any technical
command can run. Missing, incomplete, or conflicting information preserves
`NO-GO / SAFE-STOP`.

## Next Required Step

The next required step is P9-117 - Minimal Technical Candidate Selection,
docs-only only.

P9-117 may identify and compare a minimal technical candidate for later
decision work. It must not execute the candidate, authorize execution, reuse
P9-94, rerun a flagged executable, or permit an Avast workaround or control
change.

## Explicitly Prohibited Operations

P9-116 does not authorize or execute:

- parser or project PowerShell script execution;
- Excel, workbook, fixture, or process execution;
- tests or build;
- package or `dist` work;
- release, publication, or tag work;
- external-service access;
- a flagged executable rerun or invocation;
- an Avast workaround, exception, exclusion, bypass, allow-list entry,
  disablement, weakening, or other security-control change; or
- any technical command beyond the authorized static text and Git inspection.

It does not modify implementation, tests, Frozen specifications, public APIs,
canonical formats, persisted schemas, packages, or distribution artifacts. It
does not stage, commit, or push.

## Verification

Verification is limited to `git diff --check`, a trailing-whitespace scan of
the four touched Markdown files, and Git status inspection. No prohibited
operation is run.
