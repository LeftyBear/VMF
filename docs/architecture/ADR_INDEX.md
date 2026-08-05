# Architecture Decision Record Index

Index State : Active tracking document
Scope       : Architecture Decision Record tracking for VMF repository decisions
Depends     : docs/architecture/adr-template.md, docs/architecture/ADR-0001-architecture-decision-record-process.md

This index tracks Architecture Decision Records (ADRs) for the VMF repository.
It is documentation-only. It does not replace Frozen Specifications,
implementation specifications, public API contracts, release checklists, or
runbooks, or verification evidence.

## Status Values

ADR status values are limited to:

- Proposed;
- Accepted;
- Superseded;
- Deprecated.

## ADR Register

| Number | Title | Status | Successor ADR | Related Documents |
| --- | --- | --- | --- | --- |
| ADR-0001 | Architecture Decision Record Process | Accepted | - | `docs/architecture/adr-template.md`; `docs/architecture/ADR_INDEX.md`; `AGENTS.md`; `VMF_CODEX_PLAYBOOK.md`; `docs/development/CURRENT_STATUS.md` |

## Numbering

ADR numbers are assigned as a zero-padded sequence beginning at `ADR-0001`.
The next ADR number is the next unused number in this index.

Numbers are never reused, even when an ADR is superseded or deprecated.

## Index Maintenance

When an ADR is added or its status changes, update the corresponding row in
this index. The row must retain enough information to identify:

- the ADR number;
- the ADR title;
- the current status;
- the successor ADR, when the decision is replaced;
- related documents that explain the governing specification, implementation
  boundary, release boundary, or operational context.

The index records tracking metadata only. It must not be used to change a
decision's accepted content, modify a Frozen Specification, replace a runbook,
or imply release approval.
