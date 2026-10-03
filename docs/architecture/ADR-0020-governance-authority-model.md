# ADR-0020: Governance Authority Model

Status  : Accepted
Date    : 2026-10-02
Scope   : Project-wide governance authority functions, human role assignment, gate separation, and bootstrap transition
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR_INDEX.md, AGENTS.md, VMF_CODEX_PLAYBOOK.md, docs/development/CURRENT_STATUS.md, docs/development/HANDOFF.md

## Context

VMF governance work requires durable attribution of human authority while
preserving the separation between review, acceptance, execution, capability,
and authorization. Repository access, tool capability, or successful
verification does not itself grant authority for a gated decision.

The User has adopted a bootstrap authority model solely to establish this
proposal. Under that bootstrap model, the User holds Bootstrap Authority,
Governance Owner, Requirements Authority, Acceptance Authority, and Execution
Authority. Chat holds Review Authority for review and evaluation only. Work and
Codex hold no independent governance authority.

This ADR is a governance-process proposal subordinate to Frozen
Specifications, authoritative project documents, and existing safety
boundaries. It does not reconstruct missing historical authority.

## Decision

VMF uses the following operative project-wide governance authority model.

### Authority Functions

| Function | Assignment | Boundary |
| --- | --- | --- |
| Governance Owner | User | Owns the governance framework and human governance decisions. |
| Requirements Authority | User | Authorizes applicable requirements and their exact scope. |
| Review Authority | Chat | Performs review and evaluation only; this is not human approval authority. |
| Acceptance Authority | User | Makes explicit human acceptance decisions for the exact submitted subject and scope. |
| Execution Authority | User | Authorizes the exact execution operation and scope; authorization is distinct from capability and performance. |

The User may hold multiple human-authority roles. Combining roles in one human
does not combine, bypass, or implicitly satisfy their authorization gates. Each
gated decision requires explicit authorization applicable to that exact
operation and scope.

Human role assignment, gate separation, capability, and authorization are
distinct:

- human role assignment identifies who may make a class of human decision;
- gate separation requires each decision to be made independently at its
  applicable boundary, even when one human holds multiple roles;
- capability describes what a person, process, tool, or execution context can
  do;
- authorization is an explicit, applicable decision permitting an exact
  operation and scope.

Capability does not grant authorization. Role assignment does not itself
authorize a particular operation. Review or evaluation does not constitute
human approval. Chat, Work, and Codex receive no independent human governance
authority and may act only within an applicable instruction and authorization
boundary.

### Delegation and Authority Lifecycle

Delegation, temporary substitution, succession, or reassignment is valid only
when supported by a separately recorded, valid authority record. That record
must identify, at minimum:

- the authorizer;
- the holder;
- the exact scope;
- the validity period; and
- the revocation condition.

Delegation, substitution, succession, or reassignment does not merge
authorization gates, bypass a required gate, or expand authority beyond the
exact recorded scope. Retrospective authority must not be inferred or created.

If an applicable authority record is missing, ambiguous, expired, revoked, or
conflicting, authority for the affected authority or action is `NOT ESTABLISHED`.

### Authority Conflict Handling

A conflict between this ADR and a higher-authority document continues to be
controlled by the existing authoritative hierarchy. This ADR does not redefine
that hierarchy.

The Governance Owner is the resolution authority for conflicts internal to the
ADR-0020 governance model involving role assignments, gate decisions, or
authority records. Governance Owner conflict resolution does not substitute
for or automatically satisfy the Requirements, Acceptance, or Execution gate
applicable to the underlying operation.

No precedence may be inferred between conflicting authority records. Until the
conflict resolution is explicitly recorded, the affected operation remains
`UNAUTHORIZED / NO-GO / SAFE-STOP`.

### Bootstrap Transition

Bootstrap Authority belonged to the User solely to establish ADR-0020. Upon
the User's explicit Acceptance decision, the authority model in this ADR became
operative and Bootstrap Authority terminated. Creating, proposing, reviewing,
or editing this ADR did not accept it; the separate explicit Acceptance
decision supplied the required acceptance authority.

### Explicit Non-Effects

Creation, proposal, review, or later acceptance of ADR-0020 alone does not:

- reconstruct or validate P9-339;
- retroactively authorize P9-343;
- supersede P9-339;
- complete or modify P9-345;
- modify U03_CAND-02;
- accept evidence;
- authorize technical execution; or
- release any `NO-GO / SAFE-STOP` boundary.

Missing historical authority must not be inferred, reconstructed, or
substituted through this ADR.

## Consequences

Governance decisions will have explicit human ownership while review and
repository execution remain bounded supporting functions. A single human may
hold several authority roles without collapsing the corresponding gates.

Future gated work must identify both the applicable authority function and the
explicit authorization for the exact operation and scope. Tool access,
repository access, verification success, or role labels remain insufficient on
their own.

Acceptance makes only this governance authority model operative. It does not
change P9 state, evidence status, technical execution authority, or safety
boundaries.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-10-02 | Proposed | Initial project-wide governance authority model drafted under the adopted bootstrap authority model; not accepted or operative. |
| 2026-10-03 | Accepted | User explicitly accepted ADR-0020; the governance authority model became operative and Bootstrap Authority terminated. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0001-architecture-decision-record-process.md`
- `AGENTS.md`
- `VMF_CODEX_PLAYBOOK.md`
- `docs/development/CURRENT_STATUS.md`
- `docs/development/HANDOFF.md`
- `docs/VMF_vNext_Backlog.md`

## Replacement

This ADR does not supersede P9-339 or any earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not replace implementation specifications, runbooks, release
  records, or verification evidence.
- This ADR does not accept itself or grant evidence, continuation, command,
  technical GO, execution, Git-mutation, or `SAFE-STOP`-clearance authority.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation, or
  flagged executable execution.
