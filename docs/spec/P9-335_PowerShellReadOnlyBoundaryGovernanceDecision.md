# P9-335 — PowerShell Read-Only Boundary Governance Decision

## 1. Record status and decision boundary

- Work item: `P9-335`
- Route: `C`
- Work mode: `docs-only`
- Decision date: `2026-09-30`
- Decision owner: `VMF Owner (User)`
- Record result: `ACCEPT / bounded PowerShell read-only inspection permission`
- Security disposition: `HOLD / CONDITIONAL DOCUMENTARY CONTINUATION ONLY`
- Residual risk: `UNRESOLVED / NOT ACCEPTED`
- Technical execution: `NO-GO / SAFE-STOP`
- Broader workflow: `SAFE-STOP`

The VMF Owner changes the operative PowerShell governance boundary from a
categorical prohibition to an explicitly bounded permission for read-only
inspection. PowerShell may act only as an inspection host within the exact
scope below. All other PowerShell use remains prohibited unless separately
authorized.

This decision grants no technical-execution authority and clears no existing
`NO-GO / SAFE-STOP`.

## 2. Authoritative basis and Route C classification

`AGENTS.md` and `VMF_CODEX_PLAYBOOK.md` define Route C for governance,
authorization, or `SAFE-STOP` handling and state that route selection changes
review depth rather than specifications, approval boundaries, or an existing
`NO-GO / SAFE-STOP`.

P9-300 requires a separate P9 record when an operative boundary changes or a
previously prohibited tool or operation is proposed. It also retains operative
boundary-change judgment with the human owner. P9-335 is that separate Route C
record for the PowerShell read-only boundary only.

## 3. Permitted context

PowerShell read-only use is permitted only for:

- docs-only governance review;
- static repository inspection;
- evidence inspection;
- textual search;
- repository or file metadata inspection; and
- read-only Git information retrieval.

PowerShell must not be used as part of implementation, verification execution,
materialization, testing, build, parsing, artifact generation, security-tool
operation, or external access.

## 4. Permitted command classes

Permitted PowerShell examples are:

- `Get-Content`;
- `Select-String`;
- `Get-ChildItem`;
- `Get-Item`; and
- `Test-Path`.

An equivalent command is permitted only when its expected and actual effect is
strictly read-only and immediately auditable as retrieval, display, comparison,
or search of already-existing information. Permission attaches to the exact
effect, not merely to a command name.

## 5. Read-only Git child-process boundary

PowerShell may invoke Git only for these read-only information classes:

- branch identification;
- `HEAD` or revision identification;
- status inspection;
- diff inspection; and
- log or history inspection.

Examples include:

- `git branch --show-current`;
- `git rev-parse HEAD`;
- `git status`;
- `git diff`;
- `git diff --check`; and
- `git log`.

These examples authorize only their read-only inspection effects. Options,
aliases, configuration, hooks, shell composition, or surrounding commands that
write, mutate, execute project logic, access external resources, or introduce
ambiguous side effects remain prohibited.

PowerShell may act only as the host for the permitted inspection. No child
process other than the read-only Git inspection described here is authorized.

## 6. Prohibited PowerShell operations

The following remain prohibited:

- `Start-Process` and other unapproved child-process launch;
- execution of project or generated `.ps1` files;
- creation or materialization of scripts;
- `Set-Content`, `Add-Content`, `Out-File`, and file-output redirection;
- file creation, modification, or deletion;
- file copy or move that changes repository or evidence state;
- registry or environment changes;
- COM or Excel automation;
- test, build, parser, generator, runner, artifact, or executable execution;
- package, `dist`, release, publication, or tag activity;
- download, network, or external-service access, including
  `Invoke-WebRequest`;
- Avast interaction, modification, exception, exclusion, workaround, or
  bypass;
- P9-94 reuse; and
- P9-130 or P9-135 rerun, reconstruction, approximation, equivalence,
  wrapping, renaming, partial reuse, or materially similar routing.

Shell chaining that invokes an unapproved process and scripts whose behavior is
not immediately auditable as read-only are prohibited.

## 7. Fail-closed side-effect rule

A command is permitted only when its expected effect is retrieval, display,
comparison, or search of already-existing information.

If a command writes, creates, modifies, or deletes state; launches a
non-approved process; accesses an external resource; executes project logic; or
has ambiguous side effects, it is not permitted. Missing, incomplete, or
uncertain command-effect information requires `NO-GO / SAFE-STOP` for that
operation.

## 8. Evidence admissibility

Output from a permitted PowerShell read-only inspection may be used as
documentary evidence only when:

- the inspected source is itself admissible;
- the command does not transform the source in a way that obscures provenance;
- the result is traceable to the inspected source; and
- no prohibited execution occurred during acquisition.

PowerShell use does not increase evidentiary authority and cannot convert
unavailable, incomplete, non-authoritative, quarantined, or otherwise
inadmissible material into authoritative evidence. Historical output obtained
through a then-prohibited route is not retroactively validated by P9-335.

## 9. Historical security-state preservation

P9-335 does not revise prior Avast or security findings. Preserve:

- Avast detection and block: `CONFIRMED`;
- P9-95 materialization failure: `CONFIRMED`;
- causal relationship: `UNPROVEN`;
- block-time Avast definition/version: `Unavailable / cannot be obtained`;
- candidate-specific residual risk: `UNRESOLVED / NOT ACCEPTED`;
- security disposition: `HOLD / CONDITIONAL DOCUMENTARY CONTINUATION ONLY`;
- technical execution: `NO-GO / SAFE-STOP`; and
- broader workflow: `SAFE-STOP`.

This record does not conclude that PowerShell generally is safe. It authorizes
only the bounded read-only use defined here.

## 10. P9-334 boundary

P9-334 is not automatically accepted, cured, validated, or reclassified by
this decision. It remains `NOT ACCEPTED / separate reassessment required`.

A separately instructed reassessment may occur only after P9-335 acceptance and
must establish that the exact P9-334 operation:

1. was strictly read-only;
2. caused no file or repository mutation;
3. did not execute a project or generated `.ps1`;
4. did not invoke an unapproved child process;
5. did not perform technical verification;
6. did not access an external resource;
7. did not interact with Avast; and
8. fits the command and process classes authorized by P9-335.

If the exact operation cannot be established, compliance must not be inferred.

## 11. Authority boundary and non-authorizations

This decision grants no:

- evidence acceptance;
- control acceptance or control-effectiveness acceptance;
- residual-risk acceptance or security clearance;
- continuation authorization;
- command authorization for technical execution;
- technical GO or EV-09;
- implementation, test, build, parser, generator, runner, artifact, or
  executable authorization;
- package, `dist`, release, publication, or tag authority;
- external-service or Avast authority;
- Git mutation authority; or
- `SAFE-STOP` clearance.

Staging, commit, push, branch or tag mutation, reset, working-tree-changing
checkout or switch, and every other Git mutation remain prohibited.

## 12. Verification and non-actions

Static review confirms that:

- Route C is the controlling route;
- P9-300's separate-record and human-owner boundary-change requirements are
  preserved;
- PowerShell is limited to explicitly bounded read-only inspection;
- permitted child-process execution is limited to the defined read-only Git
  information classes;
- ambiguous side effects fail closed;
- historical Avast findings are unchanged;
- the P9-332 HOLD, residual-risk state, technical `NO-GO / SAFE-STOP`, and
  broader `SAFE-STOP` remain unchanged;
- P9-334 has not been automatically accepted; and
- no downstream gate has been resolved.

No PowerShell, project script, test, build, parser, Excel, artifact or
executable, Avast, external-service, package, `dist`, release, tag, staging,
commit, push, or other Git-mutation operation was performed in creating this
record.

## 13. Decision result and next boundary

Result:

`ACCEPT / POWERSHELL READ-ONLY INSPECTION PERMITTED WITHIN THE P9-335 BOUNDARY`

All PowerShell use outside this exact boundary remains prohibited unless a
later separate owner decision expressly authorizes it. P9-334 reassessment is a
separate possible next Route C task; it is not started or authorized by this
record.
