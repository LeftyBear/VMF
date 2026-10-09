# Schema-4 ROT-Only — Option B0 Bootstrap PowerShell Exception Decision

> **Current applicability:** All B0 authorizations recorded here are
> **CONSUMED / NON-REUSABLE**. No general reusable PowerShell authorization
> exists, and this historical record grants no current execution authority.

## Status

**ADOPTED**

## Decision

The User re-authorized **Option B0** for the **next Fresh Full Offline Verification** only.

The prior Option B0 authorization was consumed by the prior attempted verification and is non-reusable. This re-authorization is a new, single-use authorization and does not revive or reuse the prior authorization.

Option B0 permits VS Code Codex LOCAL to use read-only PowerShell solely during initial bootstrap to obtain and understand:

- `AGENTS.md`
- `VMF_CODEX_PLAYBOOK.md`
- the current authorization/instruction packet, only if local file reading is technically required

The permitted effect is **read only**. No repository modification is permitted through this bootstrap exception.

The exception ends as soon as those instructions are available and understood. It ends before substantive verification begins.

## Duration

This exception applies only to the **next Fresh Full Offline Verification**.

It does not carry forward to any later verification run, candidate preparation, preflight, real Experiment, or other VMF task. Any later PowerShell use requires fresh authorization.

## Prohibited PowerShell Use

After bootstrap, PowerShell remains prohibited for all substantive verification operations, including:

- source or evidence inspection
- manifest generation or computation
- hashing or file enumeration
- process enumeration or Excel observation
- restore, build, tests, or Git operations
- candidate creation or Experiment execution
- security inspection or change

`tools\build\build.ps1` and every other repository PowerShell script remain prohibited. PowerShell does not authorize Build.Host execution, downloading or network access, Excel/COM, process manipulation, security exclusions, bypasses, or settings changes.

## Preserved Governance Boundaries

- The security disposition remains **UNRESOLVED**.
- Option C0 remains **ADOPTED**.
- Before the first `dotnet` invocation in the next Fresh Full Offline Verification, a task-specific `DOTNET_CLI_HOME` must be used and `DOTNET_GENERATE_ASPNET_CERTIFICATE=false` must be set.
- No certificate removal or change is authorized.
- The real Experiment remains **NO-GO / SAFE-STOP**.
- No candidate authorization is introduced.
- No Git mutation authorization is introduced.
- No restore, build, test, manifest computation, Excel observation, candidate creation, Experiment execution, Build.Host execution, network access, security change, or U03 access is authorized.
- Frozen specifications and contracts remain unchanged.

## Effect

Option B0 removes only the initial instruction-reading bootstrap obstacle for the next Fresh Full Offline Verification. It does not itself authorize that verification run, clear any security or execution gate, or authorize any follow-on operation. Substantive verification PowerShell remains prohibited, including `tools\build\build.ps1`.

---

# Candidate Preparation B0 Bootstrap Revision — 2026-10-09

## Status

**ADOPTED — NEXT CANDIDATE PREPARATION ONLY**

## Decision

The User revised **Candidate Preparation Option B0** solely for the **next Candidate Preparation** task.

The prior Candidate Preparation B0 attempt ended before substantive work. It created no candidate, invoked no `dotnet` command, and changed no repository file. That prior attempt and its B0 authorization are consumed and non-reusable. This revised B0 applies only to the next Candidate Preparation task.

During initial bootstrap only, Option B0 permits VS Code Codex LOCAL to use read-only PowerShell solely to read:

- the supplied Candidate Preparation authorization packet
- `AGENTS.md`
- `VMF_CODEX_PLAYBOOK.md`
- `C:\Users\biz\.codex\memories\MEMORY.md`, if it is consulted automatically by the local Codex bootstrap

`MEMORY.md` is non-authoritative. It must not grant authorization, override the current packet or any Frozen specification or governance record, expand task scope, or serve as evidence of current repository state.

The permitted effect is **read only**. No repository modification is permitted through this bootstrap exception. More than one read-only PowerShell invocation is permitted during bootstrap, provided every invocation occurs before `BOOTSTRAP COMPLETE` and is limited to the bootstrap targets listed above.

Bootstrap does not end merely when the packet is first read. It ends immediately before the first substantive task action, including repository source inspection beyond `AGENTS.md` and `VMF_CODEX_PLAYBOOK.md`, manifest enumeration or hashing, Excel-process observation, candidate-root creation, any `dotnet` invocation, restore/build/test, artifact copying, provenance generation, or Git inspection. At that boundary, Codex must record internally:

`BOOTSTRAP COMPLETE`

Any PowerShell use after `BOOTSTRAP COMPLETE` requires **SAFE-STOP**.

## Substantive PowerShell Prohibition

After bootstrap, PowerShell is prohibited. It must not be used for:

- source inspection
- manifest computation
- hashing or file enumeration
- restore, build, or tests
- artifact copying
- provenance generation
- candidate integrity checks
- Excel or process observation
- Git inspection
- security inspection

`tools\build\build.ps1` and all other repository PowerShell scripts remain prohibited. PowerShell does not authorize network download, Excel/COM, process manipulation, or any security bypass, setting, or exclusion.

## Option C0 Controls

Option C0 remains **ADOPTED**. Before the first `dotnet` invocation in the separately authorized next Candidate Preparation task:

- `DOTNET_GENERATE_ASPNET_CERTIFICATE=false` must be established.
- A fresh, task-specific `DOTNET_CLI_HOME` must be established.
- SDK `8.0.425` must be selected.

No certificate removal, trust change, or certificate-store modification is authorized.

## Preserved Governance Boundaries

- The security disposition remains **UNRESOLVED**.
- The real Experiment remains **NO-GO / SAFE-STOP**.
- This governance task does **not** authorize Candidate Preparation or create a candidate.
- Restore, build, tests, manifest computation, artifact copying, provenance generation, Excel observation, Excel/COM, Experiment execution, Build.Host execution, network access, security changes, Git mutation, and U03 access remain unauthorized.
- Frozen specifications and contracts remain unchanged.

## Effect

This revised B0 records only the bootstrap boundary for the next Candidate Preparation task. It does not authorize Candidate Preparation itself, candidate creation, technical execution, or any follow-on operation. The next Candidate Preparation requires its own separate authorization and must apply the Option C0 controls before its first `dotnet` invocation.
