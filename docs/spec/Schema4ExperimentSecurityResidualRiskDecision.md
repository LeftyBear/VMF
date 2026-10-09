# Schema-4 Experiment Security Residual-Risk Governance Decision

> **Current applicability:** **CONSUMED / NON-REUSABLE**. This record applied
> only to the exact pinned candidate, Option B/B2 disposition, and one-use
> authorization stated below. It provides no current Experiment execution
> authority; the real Experiment remains **NO-GO / SAFE-STOP**.

## 1. Record status

- Route: `C`
- Work mode: `docs-only governance decision record`
- Status: `OPTION B2 SELECTED / SPECIFIC RESIDUAL RISK ACCEPTED`
- Technical readiness: `READY`
- Security disposition: `UNRESOLVED`
- Selected option: `OPTION B`
- Residual-risk acceptance: `GRANTED`
- Option B2 selected by User: `YES`
- Continuation authorization: `GRANTED`
- Technical GO: `GRANTED CONDITIONALLY`
- Technical-GO condition: `fresh Final Preflight`
- OS-command authorization: `NO`
- Experiment execution: `NOT AUTHORIZED`
- Invocation GUID: `NOT ASSIGNED`
- Maximum future invocations: `EXACTLY ONE`
- Frozen modified: `NONE`

The User selected Option B and Option B2 and granted bounded residual-risk acceptance for
exactly one future controlled Schema-4 ROT-only Excel/COM Experiment using the
exact pinned candidate identified in Section 3. This selection does not resolve
the security disposition and does not authorize an OS command or Experiment
execution.

## 2. Established technical state

The supplied authoritative evidence establishes:

- technical readiness: `READY`;
- restore/build: `6/6 PASS`;
- tests: `68/68 PASS`;
- execution-path contract: `PASS`;
- ROT-only architecture: `PASS`;
- cleanup topology: `PASS`;
- persistence, lifecycle, residual, and evidence checks: `PASS`;
- source and artifact integrity: `PASS`;
- zero Excel processes observed during pre-execution review; and
- no technical defect identified.

This established state supports Continuation Authorization and conditional
Technical GO. It does not establish endpoint-security safety and does not
replace the required fresh Final Preflight.

## 3. Exact pinned candidate

Option B and its residual-risk acceptance apply only to this candidate:

```text
C:\Users\biz\AppData\Local\Temp\vmf-schema4-rot-candidate-20261008T100753133Z-30524
```

The pinned hashes are:

- source manifest SHA-256:
  `6972b3f418a0371862d3251c1c52f6178c872d3e5d5c9e14c4c09f215c596511`;
- provenance SHA-256:
  `1be974e81bd2ca05d114f52c4e7069f555bfa04b55835f779cc5165b5116a603`;
- Experiment DLL SHA-256:
  `f3045b9239f3f5475d02c2d9c9917c5783ea4dd1f45784c089c4f045f5942fa3`;
- Primitives DLL SHA-256:
  `d0d11e34f5cfb8851d230c1bf75eaa2b7bb599d0841c0fd9e6ab97e39df64e1e`;
- `deps.json` SHA-256:
  `ea16913aece4a9e08eff2fdc9eb0a6522a228c65eceb34b4a8260baf54ae1159`;
  and
- `runtimeconfig.json` SHA-256:
  `28329cf08f6505e73806b17558b187c02f0c1c516fe47ebfb7a013d082aaa306`.

The only candidate Experiment DLL eligible for a future, separately authorized
invocation is:

```text
C:\Users\biz\AppData\Local\Temp\vmf-schema4-rot-candidate-20261008T100753133Z-30524\execution-artifacts\Vmf.Build.ExcelOwnership.Experiment.dll
```

## 4. Option B residual-risk decision

The User selected Option B for this exact pinned candidate. Residual-risk
acceptance is `GRANTED`, the security disposition remains `UNRESOLVED`,
Continuation Authorization is `GRANTED`, and Technical GO is `GRANTED
CONDITIONALLY`.

The acceptance is limited to exactly one future controlled invocation. It does
not establish that the candidate is safe, that endpoint protection is fully
active, or that endpoint-protection behavior is known. It does not authorize
an OS command or Experiment execution.

The User selected Option B2 for this exact pinned candidate. Option B2 accepts
only this specific endpoint-protection uncertainty established during Final
Preflight:

- Avast `detections.log` is current but locked by another process;
- its current contents cannot be inspected through standard authorized
  read-only access;
- readable `FileSystemShield.txt` and `BehaviorShield.txt` contain no
  candidate or artifact matches; and
- absence of a relevant detection or block therefore cannot be conclusively
  established.

This acceptance does not establish absence of an Avast detection,
endpoint-protection safety, or false-positive status. It does not authorize an
Avast exclusion, exception, allow-list change, privileged unlocking or copying
of protected logs, security bypass, or security-setting change.

All prior candidates, Option B decisions, Option B2 decisions, residual-risk
acceptances, and one-use authorizations are `NON-REUSABLE`. They must not be
inferred, reconstructed, extended, or carried forward to this candidate or any
future invocation.

## 5. Fresh Final Preflight condition

Technical GO is conditional on a fresh Final Preflight immediately before any
separately authorized command submission. The Final Preflight must freshly:

- establish the `EXCEL.EXE` process count as exactly `0`;
- verify the exact candidate path, authorized Experiment DLL path, and every
  hash in Section 3;
- perform the required endpoint-protection observation; and
- confirm that no new explicitly observable candidate or artifact
  endpoint-protection block or detection is present.

Any newly observable candidate or artifact endpoint-protection block or
detection before command submission requires immediate `SAFE-STOP`. Option B2
accepts only the present inability to inspect the locked `detections.log`; it
does not accept any new uncertainty or observable detection or block.

A successful Final Preflight will not itself authorize the OS command or
Experiment execution. Those remain separate gates. No retry is permitted after
a future command submission, and no hard termination is permitted.

## 6. Independent authorization gates

The current gates are:

```text
TECHNICAL READINESS: READY
SECURITY DISPOSITION: UNRESOLVED
OPTION B RESIDUAL-RISK ACCEPTANCE: GRANTED FOR THE EXACT PINNED CANDIDATE
OPTION B2: SELECTED BY USER FOR THE EXACT PINNED CANDIDATE
SPECIFIC LOCKED-DETECTIONS.LOG UNCERTAINTY: ACCEPTED
CONTINUATION AUTHORIZATION: GRANTED
TECHNICAL GO: GRANTED CONDITIONALLY ON A FRESH FINAL PREFLIGHT
OS-COMMAND AUTHORIZATION: NO
EXPERIMENT EXECUTION: NOT AUTHORIZED
INVOCATION GUID: NOT ASSIGNED
MAXIMUM FUTURE INVOCATIONS: EXACTLY ONE
```

Each gate is independent. Authorization or success at one gate does not grant
the next gate. A separate explicit one-use OS-command authorization remains
required after a successful fresh Final Preflight.

## 7. Explicit non-effects and non-actions

This governance update does not authorize or perform:

- run-directory creation;
- invocation GUID generation;
- Experiment execution;
- Excel or COM access;
- `Build.Host` or `build.ps1` execution;
- restore, build, or test execution;
- source or test changes;
- a security bypass, setting change, exception, exclusion, or allow-list;
- network access;
- Git mutation; or
- U03 access.

The run directory remains `NOT CREATED`. No invocation GUID has been generated
or assigned.

No Frozen specification, public API, persisted schema, canonical format,
error-code contract, or architectural boundary is modified.

## 8. Governance outcome

```text
A. OPTION B2 RECORDED - READY FOR FRESH FINAL PREFLIGHT
```

The next action is a separately authorized fresh Final Preflight. Experiment
execution remains `NOT AUTHORIZED`, OS-command authorization remains `NO`, and
the security disposition remains `UNRESOLVED`.
