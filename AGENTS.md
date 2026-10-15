# AGENTS.md

## AI Development Guide for VMF

This document defines the repository-wide operating and safety rules for AI-assisted VMF development. `VMF_CODEX_PLAYBOOK.md` provides the routine workflow. If the two documents conflict, this document prevails.

## 1. Authority and source of truth

Operational instruction priority is:

1. explicit task-specific instructions
2. this `AGENTS.md`
3. `VMF_CODEX_PLAYBOOK.md`
4. applicable implementation patterns

Software and contract authority is:

1. Canon v2.0 — `specs/build/Canon_v2.0.md`
2. VMF v1.0 — `specs/vmf/VMF_v1.0.md`
3. build and architecture specifications — `specs/build/`
4. module and API specifications
5. applicable task-specific design documents
6. existing tests
7. source code — `src/`

Higher-priority sources prevail. Source code and tests do not override Frozen specifications. VMF v1.0 is frozen; improvements must be proposed separately as VMF v1.1 Candidates and must not be applied to v1.0 unless formally adopted.

Before editing, Work must inspect the authoritative material needed for the task. Missing information must not be invented.

## 2. Standard operating model

VMF uses the following standard division of responsibility:

* **Chat** — judgment, scope confirmation, design or governance decisions, and evaluation of results
* **Local Work** — investigation, design, implementation, testing, build, verification, and document editing in the local workspace
* **VS Code / User** — review of diffs and all Git-changing or authenticated Git operations

Codex is not part of the standard VMF operating model. Do not require a Codex product surface, Codex execution route, Target Executor, or Codex-specific handoff for ordinary work. If a future task expressly authorizes another execution surface, that is a task-specific exception and does not silently change this standard.

Chat may ask Local Work to perform a task directly. Work returns evidence, changes, verification results, and unresolved issues. A separate route classification, executor-selection packet, or per-step authorization-consumption record is not required for ordinary development.

## 3. Core invariants

For every task, Work must:

* preserve Frozen specifications and authoritative contracts
* preserve public APIs, persisted schemas, canonical formats, and error-code contracts unless a change is expressly authorized
* preserve architectural boundaries and one-way dependencies
* implement only the requested scope and make the smallest sufficient change
* preserve existing user changes and avoid unrelated formatting or cleanup
* preserve test integrity and perform verification appropriate to the change
* keep changes reviewable and uncommitted
* protect credentials, personal data, and external systems
* report uncertainty, incomplete verification, and stop conditions

Work must not:

* infer missing requirements or authorization
* redesign architecture or add dependencies without an explicit requirement
* overwrite or discard user changes
* weaken, delete, disable, skip, or rewrite tests merely to obtain a passing result
* treat partial success as completion
* perform Git-changing or authenticated Git operations
* bypass a NO-GO / SAFE-STOP condition
* expose, embed, copy, or relocate secrets or credentials

## 4. Scope and implementation

Each task should identify the purpose, Scope In, Scope Out, acceptance conditions, verification, and stop conditions when these are not obvious from the request.

Work must limit inspection and change to the task-relevant specifications, affected implementation, focused tests, and directly supporting files. Repository-wide exploration is allowed only when materially necessary to complete the task safely.

Work must avoid unrelated refactoring, broad formatting changes, speculative cleanup, unnecessary abstractions, and future functionality. When an ambiguity cannot be resolved from authoritative sources, use the most conservative interpretation that preserves existing contracts; stop and return to Chat when implementation would require a material product, architecture, governance, or security decision.

Implementation must preserve existing naming and architectural patterns, deterministic behavior, serialization and persistence contracts, explicit failure handling, rollback or atomicity where required, retry safety where applicable, and testability. Circular dependencies and suppression of errors merely to pass verification are prohibited.

## 5. Routine authorization

The user's instruction to perform ordinary VMF development grants comprehensive authorization for the requested scope to Local Work. Within that scope, no separate approval is required for:

* read-only investigation and repository inspection
* design and analysis
* source and documentation editing
* creation or update of tests and local fixtures
* focused and broader tests
* local build, formatting, linting, and other non-destructive verification
* review and reporting of the resulting workspace diff

This comprehensive authorization ends at the declared task boundary and does not override Frozen specifications, SAFE-STOP, external-service restrictions, credential safety, or the individual-approval operations below.

Work should not ask for repeated approval merely because it moves from investigation to editing, testing, build, or documentation within one ordinary task.

## 6. Operations requiring individual approval

Individual user approval is required before:

* any Git mutation, including stage, unstage, commit, amend, merge, rebase, reset, tag, branch deletion, or push
* any authenticated Git operation
* release, publication, deployment, or modification of `dist/` or release packages
* destructive or difficult-to-recover operations outside the ordinary edit scope
* modification of live external services, production data, shared remote documents, or external repositories
* credential, authentication, account, ACL, ownership, sandbox, or security-control changes
* execution of flagged or quarantined code
* any technical operation currently covered by NO-GO / SAFE-STOP

Approval must identify the operation and scope. Approval for one high-risk action does not authorize another. Routine Local Work actions do not need use-count or consumption tracking; bounded or single-use wording is recorded only when the approval itself expressly imposes that limit.

## 7. P9 security separation and SAFE-STOP

Unresolved P9 security targets remain isolated from normal development and remain **NO-GO / SAFE-STOP**.

Normal development outside the affected P9 target may continue under Section 5 when it neither executes, modifies, packages, publishes, nor relies on the unresolved security target. Ordinary development success, static review, document preparation, test success elsewhere, or general owner approval does not clear the P9 hold.

The following are separate decisions and must not be conflated:

* design or document acceptance
* evidence sufficiency
* security-risk disposition
* SAFE-STOP release
* authorization for a specific technical execution
* release or publication authorization

If the requested work touches the unresolved target or could weaken its containment, Work must stop before technical execution and return the issue to Chat. SAFE-STOP may be released only by an explicit user decision supported by the required evidence.

## 8. Execution context and credential safety

Capability is not authorization. For sensitive work, assess the actual filesystem, process identity, credential-store access, network access, interactive context, and sandbox boundary.

Environment variables alone do not prove the Windows security principal. If a user-bound credential operation depends on identity and the process token cannot be safely established, stop. Do not work around identity or credential boundaries by changing profiles, stores, accounts, protocols, ACLs, ownership, sandbox policy, or security software.

External integrations are disabled by default. Without individual approval, use local fixtures, fakes, stubs, in-memory implementations, dry runs, or temporary directories instead of real services and data.

## 9. Testing and verification

Verification must be proportional to risk and relevant to the task. Start with focused checks, then broaden when the affected surface or acceptance criteria require it. Typical levels are focused tests, unit tests, relevant integration tests, release build, format or lint checks, and diff checks.

Tests must cover relevant normal, boundary, invalid, empty, ordering, retry, conflict, corrupted-state, partial-failure, determinism, and compatibility cases. Do not weaken assertions or fixtures to conceal an implementation defect.

Documentation-only work does not require an unrelated build. Report every check performed, its result, material warnings or errors, and any required check not run with the reason.

## 10. Git policy

Local Work must not perform Git mutations or authenticated Git operations. The user performs them in VS Code after reviewing the diff.

Read-only Git inspection may be used by Local Work when needed to understand state or review changes. It does not authorize mutation. Do not use a deliberately failing write as a capability probe.

When Git action is requested, provide the user with a minimal VS Code handoff that identifies:

* repository and intended operation
* exact files or scope
* preconditions and expected result
* stop conditions

Prefer explicit file paths over broad staging such as `git add .`. Staging, cached-diff review, commit, authentication, push, tag, and release remain distinct user actions unless the user explicitly groups them in one approval.

## 11. Documentation, completion, and reporting

Documentation must match implemented behavior and current verified state. Do not present proposals, drafts, requested approvals, or partial evidence as accepted or complete. Frozen documentation must not be changed without explicit authorization.

A task is complete only when the requested scope is implemented, applicable acceptance conditions are satisfied, relevant verification is complete or transparently reported as incomplete, user changes are preserved, and no prohibited operation occurred.

The final report must state, concisely:

* result and files changed
* verification performed and results
* unresolved issues or stop conditions
* Git state relevant to handoff
* explicitly omitted high-risk actions when material

## 12. Evolution policy

Model or product capability changes may justify updating the Playbook, but do not by themselves weaken Frozen-specification protection, minimal-change discipline, test integrity, user-change protection, credential safety, Git boundaries, or P9 SAFE-STOP.

The goal is safe delivery with low procedural overhead: broad autonomy for ordinary local development, explicit human control for Git and high-risk actions, and fail-closed handling for unresolved security work.
