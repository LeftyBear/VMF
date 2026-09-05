# P9-97 - Authoritative Security Disposition Intake Boundary

## Status

COMPLETE / docs-only intake-boundary definition

## Purpose

Define the minimum evidence and decision boundary for receiving a future
authoritative security disposition after the P9-96 continuation `NO-GO` review.
P9-97 does not supply, request, infer, or approve that disposition and does not
authorize continuation.

## Preserved Current State

- P9-95 remains `INCOMPLETE / SAFE-STOP`.
- The P9-95 materialization failure and the Avast `IDP.HELU.PSE90` detection /
  block remain separate `CONFIRMED` observations.
- Causation between those observations remains `UNPROVEN`.
- Parser invocation count remains `0`.
- The unused P9-94 invocation allowance is not reusable.
- P9 continuation remains `NO-GO / SAFE-STOP`, with no selected technical
  execution candidate.

## Acceptable Intake

A future disposition may enter review only when it is supplied as an
authoritative, attributable record that identifies:

1. the issuing authority and its authority for the security decision;
2. the affected detection, path, process, and artifact or input identity with
   enough specificity to correlate it to the P9-95 event;
3. the disposition outcome and its scope, limitations, conditions, and
   effective date;
4. the evidence or analysis basis for that outcome;
5. whether causation is established, rejected, or remains unresolved; and
6. any expiry, revocation, supersession, or required follow-up condition.

The intake record must preserve the supplied wording and provenance without
promoting an operator observation, absence of a later detection, local risk
acceptance, or an unrelated Publisher release record into vendor clearance or
safety certification.

## Fail-Closed Review Rules

Missing, ambiguous, unauthenticated, out-of-scope, expired, superseded, or
internally inconsistent material is insufficient. It must be recorded as
`NOT ACCEPTED` or `INCOMPLETE` for continuation purposes, without resolving
causation or changing the P9 safe-stop state.

Acceptance of a disposition establishes only that the security evidence is
suitable for a later continuation review. It does not reuse P9-94, authorize
materialization or parser invocation, select an alternate path, approve a
security-control change, establish runtime readiness, or supply the separate
task-specific continuation authorization required by P9-96.

## Decision

Decision: `GO` for recording this docs-only intake boundary.

Decision: no authoritative security disposition is supplied or accepted by
P9-97. The first P9-96 resumption condition therefore remains unsatisfied.

Decision: no task-specific continuation authorization is supplied by P9-97.
The second P9-96 resumption condition therefore remains unsatisfied.

Decision: P9 continuation remains `NO-GO / SAFE-STOP`. No new technical
execution candidate is selected.

## Prohibited Operations

P9-97 does not execute or re-execute materialization, a parser, PowerShell, a
runtime probe, the lifecycle, Excel, a workbook, a fixture, or a process. It
does not restore quarantine, change or pause Avast, add an exception,
exclusion, or allow-list entry, weaken or evade a security control, change
implementation, tests, tools, Frozen specifications, public APIs, canonical
formats, or persisted schemas, update package or `dist`, access an external
service, run a flagged executable, release, publish, tag, stage, commit, or
push.

## Next Boundary

There is no next technical execution candidate. After an authoritative
security disposition is actually supplied, a separately requested docs-only
intake review may evaluate it against this boundary. Only after an accepted
disposition and separate task-specific continuation authorization exist may a
later GO / NO-GO review consider a precisely scoped continuation candidate.

## Verification

Verification is documentation-only: compare P9-95 through P9-97 and the
synchronized current-state records, run `git diff --check`, inspect the four
Markdown files for whitespace defects, and inspect staged and unstaged Git
state. No prohibited operation is run.
