# P9-114 - Explicit New GO Decision

## Status

COMPLETE / docs-only reopening decision / GO

## Purpose

Decide whether to grant the explicit new GO required by P9-109 after P9-112
accepted the security disposition and P9-113 accepted separate continuation
authorization for a precisely limited docs-only scope.

This decision concerns only reopening P9 continuation governance. It does not
identify or authorize a technical execution candidate or any technical
operation.

## Reviewed Basis

| Decision element | Recorded basis | Result |
| --- | --- | --- |
| Security disposition | P9-112 accepted the alternative-evidence security disposition under P9-111 Path B. | Accepted |
| Continuation authorization | P9-113 accepted continuation only for docs-only next-step planning and reopening-decision preparation. | Accepted |
| Block-time definition/version | The Avast definition/version active at block time remains unavailable and unproven. | Limitation preserved |
| Technical candidate | No technical execution candidate is identified or selected. | `None` |
| Prior allowance | The P9-94 allowance cannot be reused. | Not reusable |

The prerequisites are accepted only within their recorded limits. The
unavailable block-time definition/version is not inferred, reconstructed, or
represented as proven.

## Decision

Decision: `GO`.

Security disposition accepted: `Yes`.

Continuation authorization accepted: `Yes`, limited to docs-only next-step
planning and reopening-decision preparation.

Explicit new GO decision: `Yes`.

Technical execution candidate: `None`.

P9-94 allowance: not reusable.

The GO reopens P9 continuation governance state only. It authorizes:

- docs-only next-step planning; and
- definition of future implementation or restart boundaries.

P9 is therefore reopened for docs-only continuation governance. Technical
execution remains `NO-GO / SAFE-STOP` unless a future, separate boundary and
authorization process expressly changes that state. This decision supplies no
such authorization.

## Next Required Step

The next required step is a separate docs-only boundary planning item,
P9-115 - P9 Continuation Boundary Planning. That item may define future
implementation or restart boundaries, prerequisites, safety stops,
verification expectations, and later decision gates.

P9-115 is planning, not execution. It must not identify or select a technical
execution candidate unless a later explicit instruction separately authorizes
that activity. Any implementation, restart, or verification operation remains
subject to a later separate decision and operation-specific authorization.

## Explicitly Prohibited Operations

P9-114 does not authorize or execute:

- parser, project PowerShell script, or Excel execution;
- tests or build;
- package, `dist`, release, publication, or tag work;
- external-service access;
- a flagged executable; or
- an Avast exception, exclusion, workaround, bypass, allow-list entry, or
  other security-control change.

It does not modify implementation, Frozen specifications, public APIs,
canonical formats, or persisted schemas, and it does not stage, commit, or
push.

## Verification

Verification is limited to `git diff --check`, a trailing-whitespace scan of
the four touched Markdown files, and Git status inspection. No prohibited
operation is run.
