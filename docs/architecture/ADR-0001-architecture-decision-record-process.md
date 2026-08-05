# ADR-0001: Architecture Decision Record Process

Status  : Accepted
Date    : 2026-08-05
Scope   : ADR authoring, numbering, status tracking, update, replacement, and document responsibility boundaries
Depends : docs/architecture/ADR_INDEX.md, docs/architecture/adr-template.md, AGENTS.md, VMF_CODEX_PLAYBOOK.md

## Context

VMF uses Frozen Specifications, implementation specifications, development
records, release records, and operational guidance to protect architectural
consistency and release safety. Some architectural decisions need a durable
record that captures why a choice was made without changing the authoritative
specification hierarchy.

An Architecture Decision Record provides that durable decision record. The ADR
process must remain subordinate to the repository's governing specifications
and safety boundaries.

## Purpose

ADRs record architecture decisions that are important enough to preserve for
future maintenance, review, or replacement. An ADR captures:

- the decision context;
- the accepted decision;
- the expected consequences;
- the status history;
- related governing documents;
- replacement or deprecation relationships.

ADRs are not implementation tickets, release approvals, test evidence, or
substitutes for Frozen Specifications.

## Applicability

Create or update an ADR when a decision:

- affects architecture boundaries, layering, ownership, dependency direction,
  operational architecture, or durable repository practice;
- is expected to influence future implementation or review decisions;
- needs explicit traceability separate from code comments, voyage logs, and
  release records;
- clarifies a local development process without changing authoritative
  specifications.

Do not create or update an ADR for:

- routine implementation details that are obvious from local code;
- transient task notes;
- release go/no-go approval;
- verification evidence;
- external service mutation approval;
- decisions that require modifying Frozen Specifications before the
  specification process has approved that change.

## Decision

The VMF repository uses ADRs under `docs/architecture/` with the following
rules.

ADR numbering starts at `ADR-0001` and continues as a zero-padded sequence.
Numbers are assigned by `docs/architecture/ADR_INDEX.md` and are never reused.

ADR status values are limited to:

- Proposed;
- Accepted;
- Superseded;
- Deprecated.

Accepted ADR body content is treated as stable. After an ADR is Accepted, its
decision body should not be rewritten for new meaning. Corrections are limited
to obvious spelling, formatting, broken links, or metadata synchronization that
does not change the accepted decision. A meaningful change must be made by a
new ADR that supersedes the earlier ADR.

`docs/architecture/ADR_INDEX.md` is the tracking index for ADR number, title,
current status, successor ADR, and related documents.

`docs/architecture/adr-template.md` is the starting template for new ADRs.

## Status Transitions

The allowed status transitions are:

| From | To | Meaning |
| --- | --- | --- |
| Proposed | Accepted | The decision is approved for use under the current governing specifications and repository rules. |
| Proposed | Deprecated | The proposal is withdrawn without becoming the accepted decision. |
| Accepted | Superseded | A later Accepted ADR replaces this decision. |
| Accepted | Deprecated | The decision is no longer recommended, but no successor ADR replaces it. |
| Superseded | Deprecated | The superseded decision is also marked obsolete when retaining it as historical context is no longer useful for current guidance. |

Do not move an ADR from Superseded or Deprecated back to Accepted. Create a new
ADR instead.

## Update And Deprecation Method

Before an ADR is Accepted, edit it normally while keeping the index current.

After an ADR is Accepted:

1. Keep the accepted decision body stable.
2. Use a new ADR for meaningful replacement or extension.
3. Mark the old ADR as Superseded only after the successor ADR is Accepted.
4. Record the successor ADR in the old ADR and in the index.
5. Mark an ADR as Deprecated when it should no longer guide future work and no
   direct successor exists.

The index must be updated in the same documentation change that adds, accepts,
supersedes, or deprecates an ADR.

## Document Responsibility Boundaries

ADRs preserve decision rationale. They do not replace or override:

- Canon v2.0;
- VMF v1.0 Frozen Specification;
- Publisher v1.0 implementation specifications;
- public API contracts;
- persisted schemas;
- canonical formats;
- release checklists;
- runbooks;
- release notes;
- verification evidence;
- current status records.

When an ADR conflicts with a higher-priority specification or explicit task
instruction, the higher-priority source controls. The ADR must then be
superseded, deprecated, or corrected through the approved documentation process.

An ADR may reference Frozen Specifications and implementation specifications,
but it must not silently modify them. Changes that require specification
updates must follow the applicable specification process before implementation.

## Consequences

Architecture decisions become easier to audit without expanding the authority
of development notes or release records.

Reviewers can track decision state through the ADR index and can identify
whether a decision is current, replaced, or deprecated.

The repository keeps Frozen Specifications and public contracts protected
because ADRs record decisions without changing authoritative specifications.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Proposed | Initial ADR process drafted as docs-only / local-only documentation. |
| 2026-08-05 | Accepted | ADR process accepted as the initial repository ADR operating basis. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/adr-template.md`
- `AGENTS.md`
- `VMF_CODEX_PLAYBOOK.md`
- `docs/development/CURRENT_STATUS.md`

## Replacement

This ADR does not supersede an earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify implementation behavior.
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation, or
  flagged executable execution.
