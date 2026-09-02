# AGENTS.md

## AI Development Guide for VMF

This document defines the operational rules for AI coding assistants, including Codex, working on the VMF repository.

This document governs **how AI assistants work**.
It does not define the software specification itself.

---

# 1. Required References

Before starting any task, the AI MUST read and follow:

1. `AGENTS.md`
2. `VMF_CODEX_PLAYBOOK.md`
3. The task-specific instructions provided by the user
4. The applicable project specifications and existing tests

The task-specific instructions define the scope of the current task.

If instructions conflict, apply the following priority:

1. Explicit task-specific instructions
2. Frozen specifications and authoritative project documents
3. `AGENTS.md`
4. `VMF_CODEX_PLAYBOOK.md`
5. Existing implementation patterns

The AI MUST NOT use a lower-priority instruction to override a higher-priority specification or safety constraint.

---

# 2. Most Important Rules

The AI MUST:

* preserve Frozen specifications
* preserve public APIs and existing contracts
* implement only the requested scope
* minimize changes
* avoid unrelated refactoring and formatting
* preserve existing user changes
* keep external updates disabled unless explicitly authorized
* execute the required verification steps
* leave reviewable uncommitted changes
* stop and report when safe implementation is not possible

The AI MUST NOT:

* change Frozen specifications
* redesign architecture without explicit instruction
* expand requirements by assumption
* add dependencies without explicit approval
* overwrite or discard user changes
* update live external services without explicit authorization
* stage, commit, push, merge, rebase, reset, stash, or rewrite Git history
* weaken, delete, skip, or disable tests to make the implementation pass
* treat partial success as completion

---

# 3. Purpose

The objective of this repository is to develop and maintain the VMF architecture and its implementations while preserving:

* long-term maintainability
* architectural consistency
* specification integrity
* testability
* deterministic behavior
* safe evolution

Architectural consistency SHALL take precedence over implementation convenience.

---

# 4. Source of Truth

The following documents define the project.

Priority, highest first:

1. Canon v2.0
   `specs/build/Canon_v2.0.md`
2. VMF v1.0
   `specs/vmf/VMF_v1.0.md`
3. Build and architecture specifications
   `specs/build/`
4. Module and API specifications
5. Applicable task-specific design documents
6. Existing tests
7. Source code
   `src/`

If documents conflict, the higher-priority source SHALL take precedence.

Source code does not override Frozen specifications.

---

# 5. Frozen Specification Policy

The AI MUST follow Canon v2.0 and VMF v1.0.

VMF v1.0 is frozen.

The AI MUST NOT modify:

* Frozen specifications
* public contracts
* persisted schemas
* canonical formats
* error-code contracts
* architectural boundaries

unless explicitly instructed and authorized.

Potential improvements SHALL be proposed separately as:

> VMF v1.1 Candidate

A candidate proposal MUST NOT be applied to the current VMF v1.0 implementation unless formally adopted.

---

# 6. Repository Structure

```text
specs/          Official Build and VMF specifications
    build/      Build canon, architecture, API, module, and release specifications
    vmf/        VMF specifications

src/            Source code
    Build/      Build source
    VMF/        VMF source

tests/          Unit and integration tests
    unit/
    integration/

tools/          Build, test, and VMF tools
    build/
    test/
    vmf/

candidates/     Future-version candidate proposals
docs/           Development and release documentation
templates/      Generation templates
prompts/        AI prompts
assets/         Static assets
applications/   Applications that use VMF
dist/           Generated distribution artifacts only
```

The AI MUST place files in the appropriate directory.

---

# 7. File Placement Rules

Specifications belong only under:

```text
specs/
```

Source code belongs only under:

```text
src/
```

Tests belong only under:

```text
tests/
```

Tools belong only under:

```text
tools/
```

Future-version proposals belong only under:

```text
candidates/
```

Generated distribution artifacts belong only under:

```text
dist/
```

The AI MUST NOT mix specifications, implementation, tests, tools, candidates, or generated artifacts.

---

# 8. Architecture Rules

The AI MUST:

* preserve the existing architecture
* preserve one-way dependencies
* preserve architectural boundaries
* follow existing naming conventions
* preserve public APIs unless explicitly instructed
* keep Facade and CompositionRoot responsibilities intact

Circular dependencies are prohibited.

Higher layers MAY depend on lower-layer contracts.

Lower layers MUST NOT depend on higher layers.

Facade and CompositionRoot SHALL define architectural boundaries.

---

# 9. Implementation Policy

Unless explicitly instructed, the AI MUST NOT:

* redesign architecture
* rename public APIs
* change repository structure
* change specifications
* introduce new abstractions
* add future-oriented functionality
* perform broad cleanup
* replace existing implementation patterns
* introduce new external packages

The AI SHALL implement only the requested scope.

Changes MUST be the smallest set required to satisfy the task acceptance conditions.

---

# 10. Scope Control

Each task SHOULD define:

* target repository
* target branch
* target Solution or Project
* target Phase
* task purpose
* allowed files or areas
* prohibited files or areas
* required behavior
* invariants
* safety-stop conditions
* acceptance criteria
* verification commands

The AI MUST begin with the declared scope.

Repository-wide exploration or modification is prohibited unless explicitly required.

The AI MUST inspect only files necessary for the declared task, preferring task-specific specifications, affected implementation files, and focused tests.

The AI MUST NOT repeatedly read authoritative files already inspected during the same task unless needed to resolve a concrete uncertainty.

The AI MUST avoid speculative investigation and follow established implementation patterns instead of searching for alternatives without a concrete need.

---

# 10A. Codex Usage and Reasoning Policy

Default Codex settings:

* model: GPT-5.6 Sol
* reasoning level: Low
* speed: Standard

The AI MUST start each task with the lowest reasoning level sufficient for the declared scope.

The AI MAY escalate reasoning only when necessary:

```text
Low -> Medium -> High
```

The AI MUST NOT start at High reasoning by default.

Low reasoning SHOULD be used for routine and well-scoped work, including:

* docs-only changes
* status, backlog, and handoff updates
* small or deterministic implementation changes
* focused test additions
* changes following an established implementation pattern
* minor bug fixes
* tasks where authoritative specifications and acceptance criteria are already clear

Medium reasoning SHOULD be used when the task requires material judgment, including:

* boundary design or interpretation
* comparison of multiple authoritative specifications
* GO / NO-GO decisions
* public-contract impact analysis
* implementation spanning multiple components
* non-trivial debugging
* ambiguity that cannot be safely resolved at Low reasoning

High reasoning SHOULD be used only for genuinely complex work, including:

* crash consistency
* persistence consistency
* retry or re-execution safety
* move detection
* complex diff algorithms
* complex state transitions
* partial-failure or conflict handling
* work involving several interacting safety boundaries

Highest or maximum reasoning levels MUST NOT be used routinely.

Reasoning escalation MUST NOT weaken:

* Frozen specification protection
* public-contract protection
* architectural boundaries
* external-service safety
* test requirements
* verification requirements
* safe-stop conditions
* Git restrictions

The AI MUST avoid unnecessary reasoning escalation when escalation would not materially improve correctness or safety.

---

# 11. Coding Rules

The AI SHALL:

* preserve existing architecture
* minimize modifications
* avoid unrelated changes
* follow existing naming conventions
* preserve deterministic behavior
* preserve public APIs
* preserve serialization and persistence contracts
* handle failure explicitly
* avoid partial success
* maintain testability
* prefer existing patterns over new patterns

The AI MUST NOT suppress warnings or exceptions merely to make verification pass.

---

# 12. Error and Safety Policy

The AI MUST:

* handle expected failures explicitly
* avoid swallowing exceptions
* preserve existing error-code conventions
* reject inconsistent or conflicting state
* fail safely when invariants are violated
* avoid partial writes
* avoid silent recovery that changes semantics
* preserve crash and retry safety where applicable

The AI MUST stop and report when implementation requires:

* modifying a Frozen specification
* modifying a public contract
* changing a persistence schema
* adding a dependency
* destructive Git operations
* access to credentials or secrets
* live external data modification
* unresolved architectural judgment
* weakening tests
* overwriting user changes

---

# 13. External Services

Unless explicitly authorized, the AI MUST NOT:

* update Google Docs
* update Google Drive
* modify external repositories
* access production services
* use credentials
* modify token stores
* write to real user data
* delete or move external resources

External integrations SHALL remain disabled by default.

Tests involving external services SHOULD use:

* Fake implementations
* Stub implementations
* In-memory implementations
* Dry-run
* Temporary directories
* Local fixtures
* Snapshots

---

# 14. Testing Policy

The AI MUST preserve existing tests.

The AI MUST NOT:

* delete tests
* disable tests
* add Skip markers to avoid failures
* weaken assertions
* change expected results merely to fit the implementation
* hide implementation defects in fixtures

Tests SHOULD cover, as applicable:

* normal cases
* boundary cases
* invalid input
* empty input
* null or missing values
* ordering
* deterministic output
* retries
* conflicts
* corrupted state
* partial failure
* compatibility with existing behavior

Focused tests SHOULD be run before the full test suite.

---

# 15. Verification Policy

The AI MUST execute the verification commands defined in the task-specific instructions.

Typical verification includes:

```powershell
dotnet test <Solution> --configuration Release --filter "<Filter>"
dotnet test <Solution> --configuration Release
dotnet build <Solution> --configuration Release
dotnet format <Solution> --verify-no-changes
git diff --check
```

The AI MUST report:

* commands executed
* pass or failure result
* warning count
* error count
* skipped or unexecuted checks
* reason for any incomplete verification

The AI MUST NOT claim completion when required verification has not passed.

The AI MUST run focused verification before broader verification when both are applicable.

---

# 16. Git Policy

The AI MAY inspect Git state using non-destructive commands such as:

```powershell
git status --short
git branch --show-current
git diff
git diff --stat
git diff --check
```

The AI MUST NOT execute:

* `git add`
* `git commit`
* `git push`
* `git pull`
* `git merge`
* `git rebase`
* `git reset`
* `git stash`
* `git clean`
* branch creation
* tag creation
* amend
* force push

The AI MUST preserve existing user changes.

The AI MUST leave reviewable uncommitted changes and stop.

---

# 17. Documentation Policy

When documentation changes are explicitly requested:

* use Markdown
* use UTF-8 encoding
* preserve the repository hierarchy
* remain consistent with Canon and applicable specifications
* avoid duplicating authoritative rules unnecessarily

When VBA source is generated:

* use Shift_JIS where explicitly required
* preserve established module conventions
* preserve VMF naming and section rules

Frozen documentation MUST NOT be modified without explicit approval.

---

# 18. Review Policy

When reviewing code, the AI SHALL report:

* architectural violations
* dependency violations
* specification inconsistencies
* public-contract changes
* persistence-contract changes
* unsafe external access
* missing or weakened tests
* unrelated changes
* possible user-change conflicts

The AI MUST NOT rewrite code during review unless implementation changes are explicitly requested.

---

# 19. AI Behavior

The AI SHALL:

* inspect before editing
* reason from authoritative sources
* prefer consistency over novelty
* preserve repository history
* avoid speculative changes
* keep the task narrow
* avoid repository-wide exploration unless required
* avoid unnecessary reasoning escalation
* report uncertainty
* distinguish facts from assumptions
* stop when safe implementation is not possible

The AI MUST NOT invent missing specifications.

When ambiguity cannot be resolved from authoritative sources, the AI SHALL choose the most conservative interpretation or stop and report.

---

# 20. Completion Conditions

A task is complete only when all applicable conditions are satisfied:

* required functionality is implemented
* requested scope is respected
* Frozen specifications are unchanged
* public contracts are preserved
* required tests are added or updated
* focused tests pass
* full tests pass
* Release build passes
* warning count is acceptable under task requirements
* error count is zero
* format verification passes
* diff verification passes
* no unrelated changes exist
* no secrets or local configuration changes exist
* no stage, commit, or push was performed
* reviewable uncommitted changes remain

If any condition is not satisfied, the AI MUST report the task as incomplete.

---

# 21. Final Report

The final report SHOULD be concise while still satisfying required reporting obligations.

The final report SHOULD include:

## Implementation Result

* target Phase
* implemented behavior
* added files
* modified files
* added or updated tests
* updated documentation

## Design Decisions

* adopted interpretation
* preserved invariants
* compatibility decisions
* safety-stop behavior
* excluded scope

## Verification Result

* focused tests
* full tests
* integration tests
* Release build
* warnings
* errors
* format check
* diff check

## Git State

* branch
* staged changes
* commit status
* push status
* working-tree status

## Remaining Issues

* incomplete items
* cause
* impact
* next required decision or action

---

# 22. Version Policy

Canon evolves through the approved project process.

VMF evolves only through formally adopted versions.

VMF v1.0 remains frozen.

Changes SHALL remain proposals under:

```text
candidates/
```

until officially adopted.

---

# 23. Project Philosophy

Architecture first.

Specification before implementation.

Implementation follows specification.

Maintainability over convenience.

Consistency over cleverness.

Safety over implicit recovery.

Minimal change over broad improvement.

Single Source of Truth.

Long-term evolution over short-term optimization.
