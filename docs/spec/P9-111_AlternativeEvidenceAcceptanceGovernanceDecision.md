# P9-111 - Alternative Evidence Acceptance Governance Decision

## Status

COMPLETE / docs-only governance decision / Path B ADOPTED

## Purpose

Decide whether to adopt P9-110 Path B after the Avast definition/version active
at block time was determined to be unavailable and impossible to obtain. This
decision revises only the evidence basis that a later docs-only intake review
may assess. It does not accept the security disposition, reopen P9, authorize
continuation, or select or authorize technical execution.

## Governance Decision

Decision: P9-111 is `COMPLETE / docs-only governance decision / Path B
ADOPTED`.

Path B is adopted. The unavailable block-time definition/version is no longer
an absolute bar to a later evidence-sufficiency review. Alternative evidence
may be reviewed as sufficient when the evidence, its limitations, and explicit
owner/security-authority risk acceptance are documented together.

The Avast definition/version active at block time remains `Unavailable / cannot
be obtained`. The post-event definition/version must not be represented as the
block-time value, and no missing value may be inferred or reconstructed.

Adopting Path B changes only the permissible governance basis for a future
docs-only intake review. It does not itself accept the alternative evidence or
the security disposition and does not satisfy any continuation or execution
gate.

## Permissible Alternative Evidence Basis

A later separately requested docs-only intake review may consider the following
evidence together:

| Evidence field | Available value |
| --- | --- |
| Avast block record | Avast block notification screenshot |
| Detection name | `IDP.HELU.PSE90` |
| Block timestamp | `2026-09-05T02:53:26.209Z` |
| Target path | `C:\Users\biz\AppData\Local\Temp\VMF-P9-93-ResidualProcessEvidence.ps1` |
| Process path | `C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe` |
| Component | `挙動監視シールド` |
| Record ID-like value | `462fa489fa42/2026-09-05T02:53:26.209Z` |
| Post-event definition/version screenshot | `260905-2 / 2026年9月6日 00:16` |
| Required limitation | The definition/version is not proven at block time. |
| Required risk acceptance | Explicit owner/security-authority acceptance of the risk arising from the unavailable block-time definition/version and stated evidence limitations. |

This list is an allowed review basis, not an acceptance result. A later intake
review must verify that the evidence, limitation, attributable authority, and
explicit risk-acceptance statement are present and sufficiently documented. It
must fail closed if they are absent, ambiguous, or broader than the evidence
supports.

## Preserved State and Gates

Security disposition accepted remains `No`.

Continuation authorization accepted remains `No`.

P9 continuation remains closed in `NO-GO / indefinite SAFE-STOP`. Technical
execution candidate remains `None`, and the P9-94 allowance remains not
reusable.

Path B adoption does not reopen P9. Any future change requires a separately
requested docs-only intake review and acceptance of the security disposition,
a separate continuation authorization review, and an explicit new GO decision.
These gates remain independent and cumulative.

## Explicitly Prohibited Operations

P9-111 does not authorize or execute:

- parser, project PowerShell script, or Excel execution;
- tests or build;
- package, `dist`, release, publication, or tag work;
- external-service access;
- a flagged executable; or
- an Avast exception, exclusion, workaround, bypass, allow-list entry, or other
  security-control change.

It does not modify implementation, Frozen specifications, public APIs,
canonical formats, or persisted schemas, and it does not stage, commit, or
push.

## Verification

Verification is limited to `git diff --check`, a trailing-whitespace scan of
the four touched Markdown files, and Git status inspection. No prohibited
operation is run.
