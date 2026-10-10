# Schema-4 Candidate One-Use Experiment Authorization

> **Applicability after Schema-4 closeout:** This document is retained as historical and supporting governance evidence. Any pre-closeout operational state recorded here, including `NOT STARTED`, `NO-GO / SAFE-STOP`, unused authorization, future-route, or similar wording, is historical and does not describe the current Schema-4 execution status. This document grants no present execution authority and is not reusable authorization. For current operational status, use the committed Schema-4 closeout records in `docs/development/CURRENT_STATUS.md`, `docs/development/HANDOFF.md`, and `docs/VMF_vNext_Backlog.md`.

## Status

**ADOPTED — ACTIVE / UNUSED**

The User explicitly adopted this exact one-use Experiment authorization by
selecting Option A in Section 11. The authorization is active and unused. It
grants no present authority for further Experiment execution, Excel/COM,
command execution, candidate execution, or other technical execution. A
one-shot execution packet reached pre-invocation preparation, but the execution
environment rejected process creation before `dotnet` started. A separate
future Execution Route Decision is now required before any further execution
packet may be issued.

## 1. Purpose and Authorization Basis

This document records the User-adopted, candidate-specific,
invocation-specific, one-use authorization. This synchronization is docs-only.
It does not authorize or perform the Experiment.

The current valid state is:

- Candidate Preparation: `COMPLETE`;
- Pin-Set Pre-Execution Review: `COMPLETE`;
- Security Disposition: `ADOPTED - OPTION B SELECTED`;
- GO/NO-GO: `ADOPTED - GO SELECTED`;
- security: `UNRESOLVED`;
- One-Use Experiment Authorization: `USER ADOPTED / ACTIVE / UNUSED`;
- Experiment invocation: `NOT ATTEMPTED`;
- Experiment execution: `NO`;
- Invocation GUID: `UNUSED`; and
- all previous Experiment authorizations: `CONSUMED / NON-REUSABLE`.

The adopted GO is bounded risk acceptance for this candidate. It is not
security resolution and is not execution authority.

## 2. Exact Candidate and Invocation Binding

This authorization is bound exclusively to the following exact identity:

- Candidate ID: `ca2321ce-feac-4d36-8039-8b7c2d349af9`
- Candidate root: `C:\Users\biz\AppData\Local\Temp\vmf-schema4-candidate-ca2321ce-feac-4d36-8039-8b7c2d349af9`
- Experiment invocation GUID: `45d9efb9-86ee-4221-a30c-456a93ee9b22`
- Source manifest:
  - file count: `32`
  - byte length: `4188`
  - SHA-256: `bbb5b0ca72d6411165e660246aeb8dae9fe3232741bfd1dae5d4350dcf489e35`
- Provenance:
  - byte length: `2366`
  - SHA-256: `2527cbc6ccef51b3409e9eab75affd3385afc367b2fadfe2dd2f95513fe956ef`

Only this exact pinned candidate and this fresh invocation GUID may be used.
Any candidate mutation, rename, addition, deletion, replacement, path change,
identity change, or contradictory evidence invalidates this authorization and
requires immediate `SAFE-STOP`.

## 3. Conditional One-Use Authorization

The User has explicitly adopted this exact authorization. It is `ACTIVE / UNUSED`
and may authorize exactly one real Experiment execution attempt, subject to
every boundary in this document and a later separate exact execution packet.

The authorization permits:

- only one invocation of the real Experiment process;
- only Candidate ID `ca2321ce-feac-4d36-8039-8b7c2d349af9` at the exact candidate
  root and identities in Section 2; and
- only invocation GUID `45d9efb9-86ee-4221-a30c-456a93ee9b22`.

The authorization permits no second invocation, retry, rebuild, republish,
candidate mutation, alternate candidate, alternate invocation GUID, or reuse of
any historical authorization or invocation GUID. A successful or failed
attempt does not create authority for another attempt.

## 4. Authorization Lifecycle and Consumption

The authorization lifecycle is exactly:

```text
DRAFT / USER ADOPTION REQUIRED
-> unused, inactive, and no Experiment execution authority

User explicitly adopts this exact authorization
-> ACTIVE but not yet consumed
-> no command may run until the separate command-binding preflight and exact
   execution packet are complete and valid

Actual Experiment process invocation begins
-> CONSUMED / NON-REUSABLE

Any success or failure after invocation begins
-> remains CONSUMED / NON-REUSABLE
-> no retry
```

Preparation, preflight, readiness review, command construction, or an execution
packet that has not invoked the Experiment process does not itself consume the
authorization. Those steps also do not broaden it. Once actual process
invocation begins, consumption is immediate and irreversible regardless of the
result.

## 5. Separate Command-Binding Preflight Required

User adoption of this authorization does not by itself supply or verify an
executable command. After adoption and before any execution packet may be
issued, a separate command-binding preflight must establish all of the
following without inference:

- exact Experiment DLL path;
- exact CLI syntax;
- exact `run` arguments;
- exact temporary-root path;
- exact environment and working directory;
- exact evidence and output locations; and
- exact one-shot command.

No unverified CLI command may be bound by inference. The preflight is a
separate gate and does not itself authorize Experiment execution, invoke
Excel/COM, create the Experiment temporary root, or consume this authorization.
Any ambiguity or mismatch requires `SAFE-STOP` and return to Chat.

## 6. Separate Exact Execution Packet Required

Even after User adoption and a successful command-binding preflight, technical
execution must be supplied through a separate exact execution packet for the
named Target Executor. That packet must carry only the then-current valid state
and must bind the verified one-shot command, exact execution context, exact
temporary root, evidence/output locations, stop conditions, and this active,
unused authorization.

This document is not that execution packet. The later one-shot execution packet
reached pre-invocation preparation but did not begin process invocation, as
recorded in Section 10. User adoption alone does not authorize an unbound
command or any technical operation. Following the execution-environment
rejection, a separate future Execution Route Decision is required before any
new exact execution packet may be issued.

## 7. Security and Immediate-Stop Rules

- Security remains `UNRESOLVED` before and after adoption.
- GO remains bounded risk acceptance, not security resolution.
- Any endpoint-protection detection or block requires immediate `SAFE-STOP`.
- No endpoint-protection exclusion, disablement, allowlisting, bypass, or
  workaround is authorized.
- A detection, block, or execution failure after process invocation begins does
  not restore or preserve the authorization; it remains
  `CONSUMED / NON-REUSABLE`, with no retry.

## 8. Preserved Boundaries

- The candidate remains immutable and pinned.
- All prior Experiment authorizations remain `CONSUMED / NON-REUSABLE`.
- Historical invocation GUIDs are non-reusable.
- `tools/build/build.ps1` remains unauthorized.
- Production Build.Host remains unauthorized.
- Rebuild, restore, publish, republish, test, and candidate mutation remain
  unauthorized.
- Security-control changes remain unauthorized.
- This authorization does not authorize Git operations or changes to
  `CURRENT_STATUS.md` or `HANDOFF.md`.

## 9. Non-Execution Attestation

Adoption and static review of this authorization must not and does not authorize:

- Experiment execution;
- Excel or COM invocation;
- command execution;
- Experiment temporary-root creation;
- candidate binary inspection or execution;
- build, restore, publish, republish, or test;
- candidate mutation; or
- inference of an unverified CLI command.

The one-use authorization is active and unused. The completed preflight and the
rejected pre-invocation packet do not authorize a retry. No further Experiment
execution is authorized unless a separate future Execution Route Decision is
made and a new exact execution packet is separately issued. The authorization
has not been consumed because no actual Experiment process invocation began.

## 10. Pre-Start Execution SAFE-STOP Synchronization

A one-shot execution packet reached pre-invocation preparation for the exact
candidate and invocation identity in Section 2. The following preflight checks
passed:

- .NET 8 runtime available;
- internal worker variables absent;
- no pre-existing Excel process;
- candidate pin-set unchanged;
- temporary root initially absent; and
- temporary root created empty.

The VS Code Codex LOCAL execution environment then rejected Experiment process
creation before `dotnet` started. Therefore:

- Experiment invocation: `NOT ATTEMPTED`;
- One-Use Experiment Authorization: `ACTIVE / UNUSED`;
- Invocation GUID `45d9efb9-86ee-4221-a30c-456a93ee9b22`: `UNUSED`;
- controller exit code: none exists;
- Experiment evidence: none exists;
- Excel/COM: none occurred;
- endpoint-protection event: `NONE OBSERVED`; and
- retry: none occurred.

The created temporary root is:

```text
C:\Users\biz\AppData\Local\Temp\vmf-schema4-experiment-45d9efb9-86ee-4221-a30c-456a93ee9b22
```

Its disposition is `ABANDONED / NON-REUSABLE`. It must not be deleted, emptied,
repaired, or reused as an Experiment root without a separate future
authorization.

The candidate remains `PINNED / VERIFIED / UNCHANGED`. No candidate mutation
occurred, and no rebuild or republish is required. Security remains
`UNRESOLVED`; GO remains candidate-specific; the authorization remains
`ACTIVE / UNUSED`; and the invocation GUID remains `UNUSED`.

The current execution-route disposition is:

```text
VS Code Codex LOCAL:
SAFE-STOP / EXECUTION-ENVIRONMENT BLOCKED
for the attempted Experiment process creation.
```

This disposition does not imply that a repeated submission to the same route
would succeed. A separate future Execution Route Decision is required before:

- choosing any new temporary root;
- rebinding the command; or
- issuing any further Experiment execution packet.

That future decision is a separate governance gate and does not itself
authorize execution, retry, cleanup, root modification, command rebinding, or
alternate-executor use.

## 11. User Adoption Decision

```text
A. ADOPT THIS EXACT ONE-USE EXPERIMENT AUTHORIZATION
B. DO NOT ADOPT / CONTINUE SAFE-STOP

SELECTED OPTION: A. ADOPT THIS EXACT ONE-USE EXPERIMENT AUTHORIZATION
SELECTED BY: User
```

The User explicitly selected Option A for this exact document. The authorization
is `ACTIVE / UNUSED` and has not been consumed. The Experiment has not been
executed. The later packet reached pre-invocation preparation but process
creation was rejected before `dotnet` started. Section 10 now governs the
required separate future Execution Route Decision and any later packet.
