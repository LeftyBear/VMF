# AGENTS.md

## AI Development Guide for VMF

This document defines the repository-wide operational rules for AI assistants working on VMF.

It governs **how AI assistants work**. It does not define the software specification itself.
Model-specific defaults, usage guidance, and routine operating procedures belong in `VMF_CODEX_PLAYBOOK.md` unless they establish a repository safety boundary.

---

# 1. Authority and Required References

Before starting a task, the AI MUST follow the applicable instructions and authoritative project sources.

Operational instruction priority is:

1. explicit task-specific instructions
2. `AGENTS.md`
3. `VMF_CODEX_PLAYBOOK.md`
4. applicable implementation patterns

Specification authority is defined separately by Section 3. Frozen specifications and authoritative project documents govern software behavior and contracts regardless of operational instruction priority.

The AI MUST inspect the authoritative materials necessary for the declared task before editing.
A task-specific instruction defines the immediate scope, but it MUST NOT override a Frozen specification, authorization boundary, or safety constraint unless that change is itself explicitly authorized through the applicable project process.
Existing tests and source code provide implementation evidence but do not outrank Frozen specifications.

---

# 2. Core Invariants

The following rules apply to every task. Ordinary task instructions do not override unconditional MUST / MUST NOT rules. Where a rule explicitly states that an operation may occur when authorized, only that stated exception may be granted through the applicable authorization process.

The AI MUST:

* preserve Frozen specifications and authoritative contracts
* preserve public APIs, persisted schemas, canonical formats, and error-code contracts
* preserve architectural boundaries and one-way dependencies
* implement only the requested scope
* make the smallest change sufficient to satisfy the acceptance conditions
* preserve existing user changes
* keep external updates disabled unless explicitly authorized
* preserve test integrity
* perform the applicable verification
* leave repository changes reviewable and uncommitted
* report uncertainty, incomplete verification, and stop conditions
* fail closed when authorization or required evidence is missing

The AI MUST NOT:

* infer or reconstruct missing authorization
* expand requirements by assumption
* redesign architecture without explicit instruction
* add dependencies without explicit approval
* overwrite or discard user changes
* weaken, delete, disable, or skip tests merely to obtain a passing result
* treat partial success as completion
* perform Git mutation or authenticated Git operations
* perform gated technical execution merely because the capability exists

These invariants take precedence over convenience, speed, or model capability.

---

# 3. Project Source of Truth

Project authority, highest first:

1. Canon v2.0 — `specs/build/Canon_v2.0.md`
2. VMF v1.0 — `specs/vmf/VMF_v1.0.md`
3. Build and architecture specifications — `specs/build/`
4. module and API specifications
5. applicable task-specific design documents
6. existing tests
7. source code — `src/`

If sources conflict, the higher-priority source SHALL take precedence.
Source code does not override Frozen specifications.

VMF v1.0 is frozen. Potential improvements SHALL be proposed separately as a **VMF v1.1 Candidate** and MUST NOT be applied to VMF v1.0 unless formally adopted.

---

# 4. Repository Boundaries

Repository areas have distinct responsibilities:

```text
specs/          authoritative specifications
src/            implementation source
tests/          unit and integration tests
tools/          build, test, and VMF tooling
candidates/     future-version proposals
docs/           development and release documentation
templates/      generation templates
prompts/        AI prompts
assets/         static assets
applications/   applications using VMF
dist/           generated distribution artifacts only
```

The AI MUST place files in the appropriate area and MUST NOT mix specifications, implementation, tests, tools, candidates, or generated artifacts.

Unless explicitly requested, the AI MUST NOT change repository structure.

---

# 5. Scope and Change Policy

Each task SHOULD identify, when applicable:

* target repository, branch, Solution or Project, and Phase
* task purpose
* Scope In and Scope Out
* required behavior and invariants
* allowed and prohibited actions
* acceptance criteria
* verification
* stop conditions

The AI MUST begin from the declared scope.

Repository-wide exploration or modification is prohibited unless the task requires it. The AI SHOULD inspect only the task-specific specifications, affected implementation, focused tests, and directly relevant supporting files.

The AI MUST avoid:

* unrelated refactoring
* broad formatting changes
* speculative cleanup
* future-oriented functionality
* unnecessary new abstractions
* replacement of established patterns without a concrete requirement
* repeated reading of authoritative files already inspected during the same task unless needed to resolve a specific uncertainty

When ambiguity cannot be resolved from authoritative sources, the AI SHALL use the most conservative interpretation that preserves existing contracts, or stop and report if implementation would require a material design decision.

---

# 6. Architecture and Implementation Rules

The AI SHALL:

* preserve existing architecture
* preserve one-way dependencies
* follow existing naming and implementation conventions
* preserve Facade and CompositionRoot responsibilities
* preserve deterministic behavior
* preserve serialization and persistence contracts
* handle expected failure explicitly
* reject inconsistent state
* avoid partial writes and partial success
* preserve crash and retry safety where applicable
* maintain testability
* prefer established patterns over novel ones

Circular dependencies are prohibited.
Lower layers MUST NOT depend on higher layers.

The AI MUST NOT suppress warnings or exceptions merely to make verification pass.

---

# 7. Reasoning and Review Depth

Reasoning depth is an execution-quality control, not an authorization mechanism.
A stronger model, deeper reasoning setting, or successful analysis MUST NOT weaken any specification, scope, safety, Git, credential, or authorization boundary.

The AI SHOULD use the lowest review depth sufficient for correctness and safety, and increase depth only when doing so materially improves the result.

Where the execution environment exposes Low / Medium / High reasoning controls, use the following guidance:

* **Low** — routine, well-scoped, deterministic work following established patterns
* **Medium** — material judgment, cross-specification interpretation, public-contract impact, non-trivial debugging, or multiple-component changes
* **High** — complex consistency, retries, state transitions, conflict handling, partial failure, complex algorithms, or several interacting safety boundaries

High or maximum reasoning SHOULD NOT be the routine default.

If the environment does not expose explicit reasoning controls, the AI MUST apply the equivalent review depth through the analysis and verification performed.

For governance, release, security, evidence sufficiency, authorization, GO / NO-GO, or SAFE-STOP decisions, the AI MUST apply at least **Medium-equivalent review depth**.

Before such a decision, confirm:

* current Phase
* docs-only versus technical-execution scope
* applicable NO-GO / SAFE-STOP boundary
* prohibited operations
* required authorization and whether it remains valid and reusable

Missing, ambiguous, stale, expired, consumed, or otherwise non-reusable evidence SHALL result in NO-GO / SAFE-STOP by default.

---

# 8. Development Routes and Roles

Each task SHALL use one route:

* **Route A** — normal development
* **Route B** — large-scale investigation or cross-document review
* **Route C** — governance, authorization, or SAFE-STOP handling

Route selection changes review depth and delegation structure. It does not change specifications, authorization, or an existing NO-GO / SAFE-STOP.

Execution selection SHALL be **purpose-first**. The purpose of the step determines the role before any product surface, model, or reasoning level is selected:

* **Control / decision / authorization coordination** -> **Chat**
* **Investigation / cross-document review / evidence reduction** -> **Work**
* **Repository execution / implementation / repository verification** -> **Codex**
* **Human-authority or privileged interactive operation** -> **User**

Responsibilities remain separate:

* **Chat** — coordinates the route, prepares instructions, evaluates returned results, and compresses the current valid state
* **Work** — performs only delegated investigation or document work and returns evidence and findings to Chat; it MUST NOT authorize subsequent execution
* **Codex** — performs only delegated repository work and verification within scope
* **User** — owns decisions requiring human authority and performs Git mutation and authenticated Git operations

Within this guide, **Codex** is the **logical VMF repository-execution role implemented through an OpenAI Codex agent**. It is **not a single product surface or execution location**. The role MAY be realized through Codex Cloud, a Codex IDE extension such as VS Code, Codex CLI/local execution, or another explicitly recognized Codex execution mode.

The following are separate dimensions and MUST NOT be conflated with the Codex role:

* product surface or execution mode
* underlying model
* reasoning level
* terminal or shell
* process identity or User identity
* capability
* authorization

The actual Codex execution mode and execution context SHALL be identified when filesystem access, repository state, process identity, credentials, network capability, or sandbox boundaries materially affect the task. Selecting Codex Cloud, VS Code, CLI, or another mode does not itself grant authorization or imply equivalent capabilities.

Work MUST NOT hand off directly to Codex. Chat SHALL evaluate and compress Work results before any subsequent Codex delegation.

A delegated execution step MUST have one named Target Executor. Capability does not imply authorization.

Handoffs SHALL carry only the current valid state required for the next step. Superseded decisions, stale evidence, and unnecessary transcript history MUST NOT be represented as current state.

The detailed delegation packet and return format are defined by `VMF_CODEX_PLAYBOOK.md`. Any packet MUST still make scope, authorization, prohibited actions, verification, and stop conditions unambiguous.

---

# 9. Authorization and Gated Operations

Authorization is operation-specific and scope-specific.
An authorization for one action MUST NOT be reused for another action unless explicitly stated.

The AI MUST distinguish:

```text
Role != Capability != Authorization
```

For gated operations, authorization MUST identify the exact operation and applicable state or scope.
Authorization MUST be interpreted narrowly: an unlisted operation is not authorized merely because it is convenient, technically available, or normally associated with the task.

For delegated work, the instruction packet MUST make the following unambiguous:

* the named Target Executor for the single execution step
* the current valid state, excluding superseded or stale state
* the authorization basis, including scope and whether it is unused, consumed, expired, or otherwise non-reusable
* execution context where capability or identity matters
* Scope In and Scope Out
* explicitly authorized and prohibited actions
* required verification and stop conditions

Verification authority does not authorize a prohibited or separately gated operation. A recommended next step returned by Work or Codex is advisory only and does not authorize that step.

The following require explicit authorization for the applicable operation and scope:

* technical execution that is currently under a NO-GO / SAFE-STOP
* release or publication
* package generation or modification
* `dist/` modification
* tag creation or release-related Git operations
* live external-service modification
* credential or authentication remediation
* security-control changes
* flagged executable re-execution
* other operations explicitly gated by the current project state

A successful prior step does not authorize a later step.
For example:

```text
verification succeeded != commit authorized != authentication available != push authorized
```

If authorization is missing or cannot be verified, stop and report.

---

# 10. Execution Context and Credential Safety

Capability and authorization SHALL be evaluated from the actual execution context, not from the name of an agent, tool, application, or operator.

For sensitive operations, consider as applicable:

* process token and security principal
* filesystem and repository access
* credential-store access
* network capability
* interactive UI or browser capability
* sandbox boundary
* explicit authorization for the exact operation and scope

Environment variables such as `USERNAME`, `USERPROFILE`, `APPDATA`, and `LOCALAPPDATA` do not prove the Windows security principal of the running process.

When an operation depends on user-bound resources such as DPAPI or Windows Credential Manager, the actual process-token identity is authoritative. If the required identity cannot be verified safely, fail closed.

If the process token and referenced user profile establish a split execution context, SAFE-STOP before:

* credential creation, update, or retrieval
* browser authentication
* credential-backed push

The AI MUST NOT work around a split context by:

* writing into another user's profile
* changing credential stores
* embedding secrets or personal access tokens
* selecting alternate accounts or profiles
* weakening security controls

---

# 11. Known VMF Execution Boundaries

These observations apply only to the VMF repository at:

```text
C:\Users\biz\Documents\Project\VMF
```

They determine execution routing only and do not grant authorization.

Observed **Codex sandbox execution context** (one specific Codex execution mode/context):

* process-token identity: `LAPTOP-96355HFT\codexsandboxonline`
* referenced profile: `C:\Users\biz`
* working-tree files writable
* read-only Git inspection available when authorized
* `.git` metadata mutation restricted across the metadata area; the observed evidence did not support a stale lock, Git layout/configuration defect, concurrent lock contention, or Avast as the cause
* authenticated Git access in the sandbox has failed with `SEC_E_NO_CREDENTIALS`

These observations MUST NOT be interpreted as proof that the repository, remote, Git configuration, or the User's interactive credentials are invalid.

The normal authorized interactive identity for manual VMF Git operations is currently:

```text
LAPTOP-96355HFT\biz
```

The AI MUST NOT use a deliberately failing `.git` write as the standard probe for the known metadata boundary.

The AI MUST NOT automatically work around the boundary by changing ACLs, ownership, sandbox policy, credential stores, authentication routing, accounts, protocols, or security software.

Known boundaries MUST NOT be generalized to unrelated repositories, to the Codex role as a whole, or to changed execution modes/environments. They MAY be reevaluated using safe read-only evidence when the environment materially changes.

---

# 12. External Services and Real Data

External integrations are disabled by default.

Unless explicitly authorized, the AI MUST NOT:

* update Google Docs or Google Drive
* modify external repositories
* access or modify production services
* use, create, delete, or modify credentials or token stores
* write to real user data
* delete or move external resources

Tests involving external services SHOULD use safe substitutes such as:

* Fake or Stub implementations
* in-memory implementations
* dry-run modes
* temporary directories
* local fixtures
* snapshots

---

# 13. Testing and Verification

Existing tests MUST be preserved.

The AI MUST NOT:

* delete or disable tests
* add Skip markers merely to bypass failures
* weaken assertions merely to fit the implementation
* change expected results merely to hide a defect
* conceal implementation defects in fixtures

Tests SHOULD cover relevant normal, boundary, invalid, empty, null or missing, ordering, retry, conflict, corrupted-state, partial-failure, determinism, and compatibility cases.

Verification MUST follow the task-specific requirements. When multiple verification levels apply, run focused verification before broader verification.

Typical verification may include:

```powershell
dotnet test <Solution> --configuration Release --filter "<Filter>"
dotnet test <Solution> --configuration Release
dotnet build <Solution> --configuration Release
dotnet format <Solution> --verify-no-changes
git diff --check
```

Only checks applicable to the task are mandatory. A docs-only task, for example, MUST NOT be declared incomplete solely because an irrelevant build was not run.

The AI MUST report:

* verification performed
* pass or failure result
* warnings and errors when applicable
* required checks not executed
* reason for any incomplete verification

Required verification authority does not authorize an otherwise prohibited operation.

The AI MUST NOT claim completion when a required check has failed or remains unjustifiably unperformed.

---

# 14. Git Policy

Git mutation and authenticated Git operations are User responsibilities.
Work and Codex MUST NOT execute them.

Work or Codex MAY perform only packet-authorized read-only Git inspection when named as the Target Executor.
Permitted examples include:

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

The AI MUST preserve existing user changes and leave reviewable uncommitted changes.

When an authorized Git mutation or authenticated Git operation is required, Chat SHALL provide an exact minimal manual handoff for the identified repository state. Commands SHALL name authorized files explicitly; `git add .` is not the standard staging command.

A manual command is not itself authorization.
Stage, cached verification, commit, authentication, and push remain independent gates.

For manual Git handoff, provide as applicable:

1. exact minimal command
2. repository and required interactive identity
3. preconditions
4. expected result
5. stop conditions
6. exact evidence to return for verification, including relevant identity, branch, HEAD, staged or cached paths, command exit code, and post-operation status

The User's returned result MUST be verified before the next independent authorization gate.

If authentication remediation occurs after push authorization, repository state MUST be re-verified and fresh push authorization obtained before push.

Credential or authentication failure SHALL fail closed unless the specific remediation is separately authorized.

---

# 15. Documentation and Review

Documentation changes MUST be explicitly within scope.

When editing documentation:

* use Markdown and UTF-8 unless another format is explicitly required
* preserve repository hierarchy
* remain consistent with Canon and applicable specifications
* avoid duplicating authoritative rules unnecessarily

When VBA source is generated, preserve established module conventions and use Shift_JIS where explicitly required.

Frozen documentation MUST NOT be modified without explicit approval.

When reviewing code, report applicable:

* architectural or dependency violations
* specification inconsistencies
* public-contract or persistence-contract changes
* unsafe external access
* missing or weakened tests
* unrelated changes
* possible conflicts with user changes

Review alone does not authorize rewriting. The AI MUST NOT modify code during a review unless implementation changes are explicitly requested.

---

# 16. Completion and Reporting

A task is complete only when all **applicable** conditions are satisfied:

* requested behavior or deliverable is complete
* scope is respected
* Frozen specifications and public contracts are preserved unless validly authorized otherwise
* required tests or documentation changes are complete
* required verification passes
* no unrelated changes were introduced
* no secrets or unauthorized local configuration changes were introduced
* no prohibited Git mutation or authenticated Git operation was performed
* repository changes remain reviewable

If an applicable condition is not satisfied, report the task as incomplete.

Final reports SHOULD be concise and include only the sections relevant to the task. For implementation work, normally report:

```text
Result:
Changes:
Verification:
Git State:
Remaining Issues:
```

Add design decisions, evidence, uncertainties, or stop conditions when they materially affect the result.

For VMF governance or authorization decisions, normally report:

```text
Decision:
Basis:
Next Action:
Caution:
```

The minimum basis required to support the decision MUST NOT be omitted for brevity.

---

# 17. Version and Evolution Policy

Canon evolves only through the approved project process.
VMF evolves only through formally adopted versions.
VMF v1.0 remains frozen.

Future-version changes SHALL remain proposals under:

```text
candidates/
```

until formally adopted.

Changes to AI tooling or model capability do not by themselves change VMF specifications, authorization boundaries, or safety policy.

---

# 18. Project Philosophy

Architecture first.

Specification before implementation.

Implementation follows specification.

Maintainability over convenience.

Consistency over cleverness.

Safety over implicit recovery.

Minimal change over broad improvement.

Single Source of Truth.

Long-term evolution over short-term optimization.
