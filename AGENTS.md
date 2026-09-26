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

# 10A. AI Execution and Reasoning Policy

This section applies to Chat, Work, and Codex unless a role-specific rule states otherwise.

Chat, Work, and Codex MUST start each task with the lowest reasoning level sufficient for the declared scope.

Chat, Work, and Codex MAY escalate reasoning only when necessary:

```text
Low -> Medium -> High
```

Chat, Work, and Codex MUST NOT start at High reasoning by default.

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

## Work-specific policy

Work:

* performs only delegated investigation, cross-document review, document analysis, and other work explicitly included in the Chat-issued instruction packet
* remains within the Chat-issued instruction packet
* returns evidence, findings, uncertainties, and unresolved issues to Chat
* MUST NOT independently expand scope
* MUST NOT authorize subsequent execution
* MUST NOT hand off directly to Codex

## Codex-specific policy

Default Codex settings:

* model: GPT-5.6 Sol
* reasoning level: Low
* speed: Standard

Codex:

* performs only delegated repository work
* remains within the repository scope and execution boundaries in the Chat-issued instruction packet
* returns repository evidence, changes, verification results, uncertainties, and stop conditions to Chat
* MUST NOT treat repository access, working-tree write capability, or successful verification as authorization for Git mutation, authentication, push, release, or another gated operation

---

# 10B. Governance Decision Safety Policy

For VMF governance, release, security, GO / NO-GO, evidence, or authorization decisions, the AI MUST NOT remain fixed at Low reasoning.

The AI MUST use Medium reasoning or higher when evaluating:

* decisions or approvals
* evidence sufficiency or validity
* whether work may resume
* whether a NO-GO or SAFE-STOP boundary may be cleared

Before answering, the AI MUST confirm:

* the current Phase
* whether the scope is docs-only or technical execution
* the applicable NO-GO / SAFE-STOP boundary
* prohibited operations
* whether the required explicit authorization exists

Evidence that is missing, ambiguous, stale, expired, consumed, or otherwise non-reusable SHALL result in NO-GO / SAFE-STOP by default.

The AI MUST NOT infer, reconstruct, or substitute any missing approval.

VMF responses SHOULD normally state the following concisely:

* Decision
* Basis
* Next action
* Caution

Concise responses remain preferred, but the minimum basis needed to support the decision MUST NOT be omitted.

If work may involve release, package, `dist/`, tags, external services, flagged executables, Avast configuration changes, or technical execution, the AI MUST stop unless explicit authorization for the applicable operation and scope has been provided.

---

# 10C. Development Route and Role Policy

Each task SHALL use one of these routes:

* Route A — normal development
* Route B — large-scale investigation or cross-document review
* Route C — governance, authorization, or SAFE-STOP handling

Route selection changes the review depth, not any specification, approval, or safety boundary. In particular, it MUST NOT clear an existing P9 `NO-GO / SAFE-STOP`.

Responsibilities SHALL remain separate:

* Chat coordinates the route, prepares instructions, evaluates results, and compresses the current valid state.
* Work performs only the delegated investigation or document work and returns evidence and results to Chat.
* Codex performs only the delegated repository work and verification within scope.
* User owns decisions requiring human authority and performs Git mutation and authenticated Git operations.

Handoffs SHALL carry only the current valid state needed for the next step, not full historical transcripts. Work MUST NOT hand off directly to Codex; Chat SHALL evaluate and compress Work results before issuing any Codex instruction.

## Standard Instruction Packet

Chat SHALL issue a Standard Instruction Packet before delegating work to Work or Codex. The packet SHALL use the following normative structure:

```text
Task:
Route:
Target Executor:

Current Valid State:

Authorization Basis:

Execution Route:
  Chat:
  Work:
  Codex:
  User:

Execution Context:

Scope In:

Scope Out:

Requirements:

Authorized Actions:

Prohibited Actions:

Verification:

Stop Conditions:

Return Format:
```

The fields SHALL be interpreted as follows:

* `Task` identifies the delegated task.
* `Route` identifies Route A, B, or C.
* `Target Executor` identifies the immediate recipient as Work or Codex. For this rule, one execution step is one packet-authorized unit of work assigned to one recipient for one result handback. One execution step MUST NOT be delegated to both Work and Codex.
* `Current Valid State` contains only the current authoritative state required for the task. Superseded decisions, stale evidence, and historical intermediate states MUST NOT be represented as current state.
* `Authorization Basis` identifies the authorizer, exact authorized operation and scope, validity conditions, and whether the authorization is unused, consumed, expired, or otherwise non-reusable. It MUST NOT reconstruct or substitute missing authorization.
* `Execution Route` explicitly states the responsibility of Chat, Work, Codex, and User for the workflow. Unused roles SHALL be marked `Not used`. Execution-route assignment does not itself grant authorization.
* `Execution Context` identifies the required process identity, repository or profile context, and applicable capability boundaries. Each item SHALL be stated when required for the task and otherwise marked `Not applicable`. Capability does not itself grant authorization.
* `Scope In` defines included work.
* `Scope Out` defines excluded work and repeats applicable `NO-GO / SAFE-STOP` boundaries when relevant.
* `Requirements` defines required results and authoritative rules.
* `Authorized Actions` is interpreted narrowly. An unlisted operation MUST NOT be inferred merely because it is convenient or normally associated with the task.
* `Prohibited Actions` identifies operations that MUST NOT be performed.
* `Verification` defines required checks and evidence. Verification authority MUST NOT imply authorization for a prohibited operation.
* `Stop Conditions` defines conditions requiring SAFE-STOP or return to Chat. Work and Codex MUST NOT resolve authorization boundaries by assumption.
* `Return Format` defines the result returned to Chat.

Unless another format is required, the standard return format SHOULD be:

```text
Result:
Evidence:
Changes:
Verification:
Uncertainties:
Stop Conditions Triggered:
Recommended Next Step:
```

`Recommended Next Step` is advisory only. Work and Codex MUST NOT authorize or initiate the next gated operation through the return report.

Chat SHALL evaluate the returned result, discard superseded or irrelevant intermediate state, and prepare a new packet for subsequent delegated work.

The use of PowerShell, `cmd`, or another CLI is not by itself grounds for approval, rejection, or stopping. Decisions SHALL be based on design validity, procedural validity, impact scope, and verifiability. This general rule does not override a task-specific tool prohibition, an execution authorization gate, or an existing SAFE-STOP.

---

# 10D. Execution Context and Credential Boundary Policy

Capability and authorization SHALL be evaluated from the actual execution context, not from the name of an agent, tool, application, or operator. The AI MUST NOT define or assume a fixed privilege hierarchy such as Work being above or below Codex.

For each sensitive operation, the AI MUST consider the effective combination of:

* process token and security principal
* filesystem access
* repository access
* credential-store access
* network capability
* interactive UI or browser capability
* sandbox boundary
* explicit authorization for the exact operation and scope

Environment variables such as `USERNAME`, `USERPROFILE`, `APPDATA`, and `LOCALAPPDATA` describe environment or profile references; they do not prove the Windows security principal of the running process. When an operation depends on user-bound resources such as DPAPI or Windows Credential Manager, the AI MUST use the actual process-token identity as the authoritative identity. If that identity cannot be verified safely, the operation SHALL fail closed.

If the process token and referenced user profile indicate different identities or otherwise establish a split-context, the AI MUST SAFE-STOP before performing or continuing:

* credential creation
* credential update
* credential retrieval
* browser authentication
* credential-backed push

The AI MUST NOT work around a split-context by writing to another user's profile, changing a credential store, embedding a personal access token or other secret, or selecting an alternate account or profile.

## Known VMF Repository Execution Boundaries

The following boundaries apply to the VMF repository at `C:\Users\biz\Documents\Project\VMF`. They determine execution routing only and do not grant or imply authorization.

The governing principle is:

```text
Role != Capability != Authorization
```

### 1. Codex sandbox `.git` metadata boundary

The observed Codex sandbox process-token identity is `LAPTOP-96355HFT\codexsandboxonline`, while the referenced Windows profile is `C:\Users\biz`. This establishes a split execution context.

In the observed context:

* working-tree files are writable
* read-only Git inspection is available
* `.git` metadata mutation is restricted

The restriction was observed across the `.git` metadata area and is not merely an `.git\index` or stale-lock issue. At the time of diagnosis, `.git\index.lock` was absent. The evidence did not support stale lock contention, a Git layout or configuration defect, concurrent Git lock contention, or Avast as the cause.

Codex MUST NOT use a deliberately failing `.git` write as the standard probe for this known boundary. Working-tree write capability MUST NOT be treated as evidence that `.git` metadata mutation is available or authorized.

### 2. Codex sandbox authentication boundary

The Codex sandbox authentication boundary is separate from the `.git` metadata boundary. In the observed sandbox context, authenticated Git access failed with:

```text
SEC_E_NO_CREDENTIALS
```

The GitHub CLI account associated with `LeftyBear` also reported an invalid token in that sandbox context.

These sandbox authentication results MUST NOT be interpreted as evidence that the repository, remote, Git configuration, or the User's interactive credentials are invalid. Authenticated remote access using `git ls-remote` succeeded in a normal interactive PowerShell context running as `LAPTOP-96355HFT\biz`, and a separately authorized `git push origin main` subsequently succeeded in the appropriate interactive context.

Authentication capability MUST be evaluated from the actual process-token and credential context.

### 3. Required execution routing

The normal execution route is:

```text
Chat
-> judgment, authorization control, and instruction preparation

Work or Codex, as named by Target Executor
-> authorized non-Git work
-> docs-only or repository working-tree work as applicable
-> read-only Git inspection
-> static verification
-> result handback to Chat

User
-> authorized Git mutation
-> authenticated Git operations
-> result handback

Chat, Work, or Codex, as specified in the instruction packet
-> verification of the returned state within the assigned scope
-> next independent authorization gate
```

Git mutation and authenticated Git operations remain User responsibilities under Section 16. Work and Codex MUST NOT execute them. Packet-authorized read-only Git inspection MAY be performed by the named Target Executor. Execution boundaries determine execution routing but do not grant authorization.

### 4. No automatic boundary workaround

The AI MUST NOT attempt or propose an automatic boundary workaround, including:

* modifying `.git` ACLs
* removing a DENY ACE
* changing ownership
* using `takeown`
* changing sandbox policy
* using alternate accounts or profiles
* modifying a credential store
* embedding a personal access token or another secret
* embedding credentials in a remote URL
* switching from HTTPS to SSH as an authentication workaround
* weakening or disabling security software
* establishing a persistent elevated bypass

Such changes require separate diagnosis, risk review, and explicit owner authorization.

### 5. Manual Git handoff

When an authorized Git mutation or authenticated Git operation is required, Chat SHALL provide the User with an actionable manual handoff aligned with Section 16. The handoff SHOULD contain:

1. the exact minimal command
2. the repository and required interactive identity
3. the applicable preconditions
4. the expected result
5. the stop conditions
6. the exact result to return for verification, including the interactive identity, branch, HEAD, staged or cached paths, command exit code, and post-operation status when applicable

For the currently known VMF environment, the normal authorized interactive identity is `LAPTOP-96355HFT\biz`. A safe read-only identity check such as `whoami` MAY be used when identity verification is necessary.

A manual command does not itself constitute authorization. Repository file modification, staging, cached snapshot verification, commit, authentication, and push remain independent capabilities and authorization gates. In particular:

```text
commit succeeded != authentication available != push authorized
```

For a push to GitHub or another remote, the applicable gates remain separate: repository-state verification, identification of the exact commit or cached snapshot, authentication-readiness verification, explicit push authorization for the identified state, and push execution.

If authentication remediation occurs after push authorization, the prior push authorization MUST NOT automatically be reused. Repository state MUST be re-verified and fresh push authorization obtained.

Credential or authentication failure MUST fail closed. Without explicit authorization for the specific remediation, the AI MUST NOT create, delete, replace, or modify credential-store entries; generate or embed a personal access token or another secret; change authentication routing; use an alternate account or profile; or weaken security controls.

The handoff, the User's execution, and authentication remediation MUST NOT be treated as authorization for any subsequent operation. The next applicable gate remains independent.

The known VMF boundaries MUST NOT be generalized to unrelated repositories or execution environments. If the actual execution context or environment changes, the boundaries MAY be reevaluated using safe read-only evidence.

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

Only Work or Codex named as the Target Executor MAY inspect Git state, and only when the Standard Instruction Packet lists the specific read-only Git inspection under `Authorized Actions`. Permitted non-destructive commands include:

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

Git mutation and authenticated Git operations are User responsibilities. Work and Codex MUST NOT execute them. Work or Codex, when named as the Target Executor, MAY perform only the packet-authorized read-only Git inspection described above. When an authorized Git mutation or authenticated Git operation is needed, Chat SHALL provide an exact, minimal command for the identified repository state and literal target paths; User manually executes it in the required interactive context and returns the result for verification. `git add .` is not the standard staging command; staging instructions SHALL name each authorized file explicitly. Providing a command does not grant authorization, and stage, cached verification, commit, authentication, and push remain separate gates.

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
