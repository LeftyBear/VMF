# Schema-4 Experiment Execution Route Decision

> **Applicability after Schema-4 closeout:** This document is retained as historical and supporting governance evidence. Any pre-closeout operational state recorded here, including `NOT STARTED`, `NO-GO / SAFE-STOP`, unused authorization, future-route, or similar wording, is historical and does not describe the current Schema-4 execution status. This document grants no present execution authority and is not reusable authorization. For current operational status, use the committed Schema-4 closeout records in `docs/development/CURRENT_STATUS.md`, `docs/development/HANDOFF.md`, and `docs/VMF_vNext_Backlog.md`.

## Status

**ADOPTED — OPTION B SELECTED**

This Route C record is a docs-only execution-route decision. The User selected
Option B — `DEFERRED EXECUTION-ROUTE REQUALIFICATION`. This selection records
the deferred requalification state and grants no execution authority.

## 1. Current Valid State

- Candidate: `PINNED / VERIFIED / UNCHANGED`
- Candidate ID: `ca2321ce-feac-4d36-8039-8b7c2d349af9`
- Invocation GUID: `45d9efb9-86ee-4221-a30c-456a93ee9b22`
- One-Use Authorization: `ACTIVE / UNUSED`
- Invocation GUID status: `UNUSED`
- Experiment: `NOT STARTED`
- Current execution route: `VS Code Codex LOCAL`
- Current route disposition: `SAFE-STOP / EXECUTION-ENVIRONMENT BLOCKED`
- Abandoned temporary root: `ABANDONED / NON-REUSABLE`
- Security: `UNRESOLVED`
- Retry: `NONE OCCURRED`

The established pre-start facts are:

- all required preflight checks passed;
- temporary-root creation succeeded;
- the Experiment process never started;
- the execution environment rejected process creation;
- no endpoint-protection event was observed;
- the One-Use Authorization and Invocation GUID remain unused; and
- the candidate remains valid.

Because the Experiment process never started, the One-Use Authorization was
not consumed. This fact does not authorize a retry or any other technical
operation.

## 2. Option A — CONTINUE SAFE-STOP

If selected by the User, Option A means:

- no further Experiment execution attempt;
- the current One-Use Authorization remains `ACTIVE / UNUSED` but dormant;
- the candidate remains pinned; and
- no cleanup, retry, or command rebinding is implied.

## 3. Option B — DEFERRED EXECUTION-ROUTE REQUALIFICATION

As selected by the User, Option B means:

- do not retry through the currently blocked `VS Code Codex LOCAL` route;
- do not bypass, disable, evade, or work around the execution-environment
  restriction;
- preserve the candidate, One-Use Authorization, and Invocation GUID;
- require a future independently supported execution environment or formally
  changed execution policy;
- require a new route-specific preflight before any invocation;
- require a new temporary-root binding because the prior root is abandoned;
- require a new exact execution packet; and
- Option B does not itself authorize execution.

Future progression requires all of the following:

1. an independently supported execution environment or formally changed
   execution policy;
2. a separate route-specific preflight;
3. a new temporary-root binding;
4. a new exact execution packet;
5. no reuse of the abandoned temporary root; and
6. no automatic execution authorization.

Any future route-specific preflight, temporary-root binding, or exact execution
packet remains a separate, independently authorized step. Selection of Option
B alone does not authorize retry, cleanup, root modification, command
rebinding, alternate-executor invocation, or Experiment execution.

## 4. Prohibited Workarounds

Neither option permits any of the following to be treated as an acceptable
workaround:

- use of an alternate shell merely to bypass the block;
- manual execution solely to evade the blocked executor;
- wrapper or launcher substitution;
- candidate rebuild or republish, including use of a copied or rebuilt binary;
- a changed Candidate ID;
- a changed Invocation GUID;
- deletion or reuse of the abandoned temporary root; or
- a security exclusion, disablement, or allowlisting change.

These actions are not authorized by this decision or by selection of Option B.

## 5. Preserved Boundaries and Non-Effects

- Security remains `UNRESOLVED`.
- The candidate remains `PINNED / VERIFIED / UNCHANGED`.
- The One-Use Authorization remains `ACTIVE / UNUSED`.
- Invocation GUID `45d9efb9-86ee-4221-a30c-456a93ee9b22` remains `UNUSED`.
- The Experiment remains `NOT STARTED`.
- No retry has occurred.
- The abandoned temporary root remains `ABANDONED / NON-REUSABLE`.
- The blocked `VS Code Codex LOCAL` route remains `SAFE-STOP /
  EXECUTION-ENVIRONMENT BLOCKED`.

This decision does not authorize or perform Experiment execution, alternate
executor invocation, temporary-root creation or deletion, command rebinding,
candidate mutation, build, restore, publish, test, security changes, Git
operations, or changes to `CURRENT_STATUS.md` or `HANDOFF.md`.

## 6. User Decision

```text
A. CONTINUE SAFE-STOP
B. DEFERRED EXECUTION-ROUTE REQUALIFICATION

SELECTED OPTION: B — DEFERRED EXECUTION-ROUTE REQUALIFICATION
```

The User selected Option B. This decision must not be treated as completed
route requalification, an execution packet, automatic execution
authorization, or execution authority.
