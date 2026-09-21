# P9-222 Retrospective Technical Investigation Closeout

## 1. Closeout status and scope

- Work item: `P9-222`
- Investigation lineage: `P9-65` / `P9-68`, `P9-70` onward, and the proposed `P9-93` onward continuation
- Work mode: `docs-only`
- Final status: `COMPLETE / retrospective technical investigation closeout / ACCEPT`
- Assessment type: retrospective technical assessment
- Technical execution, security disposition, release, publication, and future-operation authority: `NOT GRANTED`

This record formally closes the residual-process / owned-resource causality investigation by reassessing the recorded evidence against the underlying VMF/Build safety requirements. It does not set a technical problem aside. It separates the VMF/Build `MUST` safety requirements from additional P9-specific proof requests and records that reopening the latter is not technically necessary.

This retrospective assessment is separate from, and does not rewrite, any contemporaneous P9 execution result or acceptance decision.

## 2. Evidence basis

The assessment relies on the existing P9-65 and P9-68 records and their preserved observations:

- Excel process count was `0` before each authorized lifecycle operation;
- only the exact test-owned replacement fixture was opened;
- the workbook was observed writable and clean;
- no workbook mutation was performed;
- the workbook was closed without saving;
- the replacement and historical fixture path/identity evidence, SHA-256 values, and fixture count remained unchanged;
- each immediate post-close check observed one Excel process and therefore retained its required `HARD-STOP` and exit code `1`; and
- each final check observed Excel process count `0` without targeted termination.

No operation is rerun and no missing evidence is reconstructed by this closeout.

## 3. Retrospective technical assessment

### 3.1 Target safety

The existing P9-65 and P9-68 evidence satisfies the underlying target-safety requirement. The authorized test-owned fixture was selected by exact path and identity, with no fallback selection or operation against the historical fixture.

### 3.2 Mutation safety

No persistent mutation was left in the test-owned fixture. The existing no-save close evidence together with the pre/post identity, hash, count, and related recorded checks satisfies the underlying mutation-safety requirement.

### 3.3 Owned-resource cleanup

Excel process count was `0` before execution and was finally `0`. The underlying final owned-resource cleanup requirement is therefore satisfied.

The evidence does not prove PID ownership, a strict causal relationship between `Quit` or COM-reference release and process termination, or the exact cleanup timing. Those non-findings remain explicit and unchanged.

## 4. Requirement classification

The remaining unproved matters are classified as follows:

| Matter | Classification |
| --- | --- |
| Exact correlation between the created `Application` and a PID | `P9-SPECIFIC` |
| Direct evidence that `Quit` completed | `SHOULD` |
| Direct evidence that COM-reference release completed | `SHOULD` |
| Causal relationship between COM release and process termination | `P9-SPECIFIC` |
| Exact cleanup timing | `P9-SPECIFIC` |

These matters are not VMF/Build product-safety `MUST` requirements. Their absence does not invalidate the established target safety, mutation safety, or final owned-resource cleanup safety.

## 5. Investigation closeout decision

Additional residual-process causality verification from P9-70 onward has no remaining technical necessity. In particular:

- P9-93 or any later continuation in that investigation line need not be executed;
- no temporary `.ps1` needs to be rematerialized;
- the same verification need not be reimplemented or rerun outside PowerShell; and
- no further PID-correlation, `Quit`, COM-release, process-exit causality, or exact-timing proof is required for VMF/Build technical completion.

This is a requirements-based closeout: the investigation traced the question back to its root safety requirements, separated required product-safety evidence from P9-specific additional proof, and found no technical need to resume the latter.

## 6. Historical decisions preserved

The contemporaneous P9-65 and P9-68 decisions remain authoritative for their original acceptance criteria:

- each immediate post-close residual-process observation remains recorded;
- each overall execution remains `HARD-STOP`;
- each command exit code remains `1`;
- the then-unproved matters remain unproved; and
- the decisions were correct under the P9 acceptance criteria then in force.

This closeout does not convert either historical execution to `PASS`, change its exit code, erase its hard stop, or retroactively alter its acceptance criteria. The `ACCEPT` in P9-222 applies only to this retrospective technical assessment and investigation closeout.

## 7. Avast and security boundary

The Avast detection remains a `confirmed unresolved security event`. Its causal relationship to the materialization failure remains `UNPROVEN`.

This closeout does not state or imply that the Avast event is resolved, safe, accepted, cleared by a vendor, or otherwise dispositioned. It does not authorize disabling Avast, creating an exception or allow-list entry, bypassing a detection, or using any other workaround.

Because additional residual-process causality proof is not a VMF/Build `MUST` requirement, the Avast issue is not treated as a mandatory blocker to VMF technical completion of this investigation. This limited technical-requirement conclusion is not a security disposition or clearance and does not remove any independently applicable Avast, security, execution, or release boundary.

## 8. Authorization boundaries

This closeout does not automatically clear, supersede, or weaken any existing `NO-GO / SAFE-STOP`, security-disposition requirement, release authorization, publication authorization, preservation hold, or future technical-execution authorization requirement.

Any future operation must obtain the independent authorization required for that operation, exact scope, executor, evidence boundary, and validity period at that time. P9-222 itself authorizes no PowerShell, Excel, test, build, package, `dist`, release, tag, external-service, flagged-executable, Avast-setting, P9-93-or-later verification, Git staging, commit, or push operation.

## 9. Final state and recorded non-actions

The final state is:

`COMPLETE / retrospective technical investigation closeout / ACCEPT`

P9-222 performed no PowerShell, Excel, test, build, package, `dist`, release, tag, external-service, flagged-executable, Avast-setting, or P9-93-or-later technical-verification operation. It did not rematerialize a temporary `.ps1`, substitute another execution mechanism, modify a workbook, change source or tests, alter Frozen specifications or public contracts, stage, commit, or push.
