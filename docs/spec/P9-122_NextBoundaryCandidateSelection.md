# P9-122 - Next Boundary Candidate Selection

## Status

COMPLETE / docs-only next boundary candidate selection

## Decision

| Decision element | P9-122 state |
| --- | --- |
| Selected candidate | Static command allowlist drafting |
| Execution authorization | No |
| Technical execution | `NO-GO / SAFE-STOP` |
| Avast detection | Unresolved |
| P9-94 allowance | Not reusable |

## Candidate Definition

Purpose: Draft a future command allowlist / prohibit list before any technical
restart.

Allowed in future planning only:

- read-only / static command classification;
- explicit allowlist / prohibit-list drafting; and
- no execution approval.

Prohibited:

- parser, script, or PowerShell execution;
- Excel, workbook, or process operations;
- tests, build, package, `dist`, release, publication, or tag operations;
- external services;
- a flagged executable; and
- any Avast workaround, exception, exclusion, or bypass.

Selection does not authorize candidate execution or any technical operation.
Missing, ambiguous, or conflicting authorization preserves `NO-GO /
SAFE-STOP`.

## Next Step

P9-123 - Static Command Allowlist Drafting, docs-only.
