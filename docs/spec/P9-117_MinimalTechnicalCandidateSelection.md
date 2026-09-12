# P9-117 - Minimal Technical Candidate Selection

## Status

COMPLETE / docs-only minimal technical candidate selection

## Purpose

Identify and compare possible minimal technical restart candidates without
executing or authorizing any candidate.

This record remains inside the docs-only planning boundary established by
P9-114 through P9-116. Candidate selection is not technical execution
authorization.

## Preserved State

| Decision element | P9-117 state |
| --- | --- |
| Security disposition accepted | `Yes`, under the P9-112 alternative-evidence basis |
| Continuation authorization accepted | `Yes`, limited to docs-only planning |
| Avast detection | Unresolved and not treated as resolved |
| Block-time Avast definition/version | Unavailable and unproven |
| P9-94 allowance | Not reusable |
| Technical execution before P9-117 | `NO-GO / SAFE-STOP` |
| Execution authorization in P9-117 | `No` |

The accepted security disposition and limited continuation authorization are
not a safety certification, a resolution of the Avast detection, or technical
execution authorization.

## Candidate Constraints

An acceptable minimal candidate must:

- avoid rerunning or invoking the flagged executable;
- avoid every Avast workaround, exception, exclusion, bypass, allow-list
  entry, disablement, weakening, or other security-control change;
- avoid parser, project PowerShell script, Excel, workbook, fixture, and
  process operations;
- avoid tests, build, package, `dist`, release, publication, and tag work;
- avoid external services and external state changes;
- be read-only, deterministic, and limited to static repository text;
- create no artifact and modify no repository file; and
- remain unexecuted until a separate operation-specific GO is recorded.

## Candidates Considered

| Candidate | Description | Assessment |
| --- | --- | --- |
| Static P9 governance-record consistency inspection | Use one read-only `rg` invocation against the named P9 governance Markdown records to check that the selected candidate, execution prohibition, P9-94 non-reuse, unresolved Avast detection, and P9-118 gate are stated consistently. | Acceptable and preferred. It is the smallest technical inspection with no materialization, script execution, flagged executable, security-control interaction, generated artifact, or external side effect. |
| Manual documentation-only consistency review | Review the same statements without invoking a command. | Acceptable as documentation validation, but it supplies no command-level technical restart candidate and therefore is not selected. |
| Parser or lifecycle readiness probe | Invoke or probe a parser, PowerShell script, lifecycle runner, Excel, workbook, fixture, or process state. | Rejected. It crosses the current technical-execution boundary and could intersect the flagged execution path. |
| Avast-mediated retry or alternate-path execution | Change, evade, or work around Avast controls, or rerun through another path. | Rejected. Every workaround, exception, exclusion, bypass, or flagged-executable rerun remains prohibited. |

## Selected Candidate

Selected candidate: `Static P9 governance-record consistency inspection`.

The selected candidate is limited to a single future read-only static-text
inspection of these records:

- `docs/spec/P9-112_AlternativeEvidenceSecurityDispositionIntakeReview.md`;
- `docs/spec/P9-113_SeparateContinuationAuthorizationReview.md`;
- `docs/spec/P9-114_ExplicitNewGODecision.md`;
- `docs/spec/P9-115_ContinuationBoundaryPlanning.md`;
- `docs/spec/P9-116_TechnicalRestartGONOGOBoundaryDecision.md`;
- `docs/spec/P9-117_MinimalTechnicalCandidateSelection.md`;
- `docs/VMF_vNext_Backlog.md`;
- `docs/development/CURRENT_STATUS.md`; and
- `docs/development/HANDOFF.md`.

For future P9-118 review only, the proposed command is:

```powershell
rg -n "P9-117|selected candidate|execution authorization|NO-GO / SAFE-STOP|P9-94|Avast detection|P9-118" docs/spec/P9-112_AlternativeEvidenceSecurityDispositionIntakeReview.md docs/spec/P9-113_SeparateContinuationAuthorizationReview.md docs/spec/P9-114_ExplicitNewGODecision.md docs/spec/P9-115_ContinuationBoundaryPlanning.md docs/spec/P9-116_TechnicalRestartGONOGOBoundaryDecision.md docs/spec/P9-117_MinimalTechnicalCandidateSelection.md docs/VMF_vNext_Backlog.md docs/development/CURRENT_STATUS.md docs/development/HANDOFF.md
```

This proposed command is listed only for future review. It is not executed by
P9-117. No correction, second command, retry, substitution, output write, or
follow-on operation is included in the candidate.

## Decision

P9-117 is `COMPLETE / docs-only minimal technical candidate selection`.

Selected candidate is `Static P9 governance-record consistency inspection`.

Execution authorization is `No`.

Technical execution remains `NO-GO / SAFE-STOP`.

P9-94 allowance remains not reusable.

Avast detection remains unresolved and is not treated as resolved.

## P9-118 Gate

The selected candidate requires a separate P9-118 GO / NO-GO authorization
before execution. P9-118 must review the exact candidate identity, exact
command, input-file boundary, prohibited operations, stop criteria, evidence
requirements, and operation-specific authorization.

Unless P9-118 records an explicit GO for this exact candidate, the proposed
command must not run. Any ambiguity, scope change, command change, missing
record, or conflicting state preserves `NO-GO / SAFE-STOP` and requires a new
review. P9-118 authorization, if later granted, must not be interpreted as
authorization for any other technical command or operation.

## Explicitly Unexecuted and Unauthorized

P9-117 does not execute or authorize:

- the selected candidate or its proposed command;
- parser or project PowerShell script execution;
- Excel, workbook, fixture, or process operations;
- tests or build;
- package or `dist` work;
- release, publication, or tag work;
- external-service access;
- a flagged executable rerun or invocation;
- an Avast workaround, exception, exclusion, bypass, allow-list entry,
  disablement, weakening, or other security-control change; or
- staging, commit, or push.

It does not modify implementation, tests, Frozen specifications, public APIs,
canonical formats, persisted schemas, packages, or distribution artifacts.

## Verification

P9-117 verification is limited to `git diff --check`, a trailing-whitespace
scan of the four touched Markdown files, and Git status inspection. The
selected candidate and every prohibited operation remain unexecuted.
