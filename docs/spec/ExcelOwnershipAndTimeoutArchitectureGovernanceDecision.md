# Excel Ownership and Timeout Architecture Governance Decision

## 1. Record status

- Route: `C`
- Work mode: `docs-only governance recording`
- Status: `ADOPTED`
- Ownership Architecture: `APPROVED`
- Mandatory SAFE-STOP Fallback: `APPROVED`
- Conditional Hard Termination: `APPROVED`
- Controlled Excel/COM Experiment: `NOT AUTHORIZED`
- Frozen specification change: `NONE REQUIRED / NONE AUTHORIZED`
- Technical implementation or execution authorization: `NOT GRANTED`

This record documents already-approved Excel ownership and timeout architecture
decisions. It does not introduce, reinterpret, or expand those decisions and
does not grant implementation, execution, evidence-acceptance, release, or Git
authorization.

## 2. Adopted ownership architecture

Excel ownership shall use explicit Excel process launch plus exact-instance COM
attachment:

1. Persist `ExcelActivationAboutToBegin`.
2. Resolve the installed `EXCEL.EXE` directly.
3. Launch a dedicated Excel process using `/x` and an invocation-unique
   sentinel workbook.
4. Capture the exact PID, process start or creation time, canonical executable
   identity, and invocation ID.
5. Durably persist `ExcelOwnershipEstablished`.
6. Bind through the invocation-specific sentinel workbook or moniker and obtain
   its `Application`.
7. Map `Application.Hwnd` to PID.
8. Require exact agreement with the launched process identity before build work
   begins.

Ownership must not be inferred from any of the following:

- global active Excel selection alone;
- newest-process selection;
- before-and-after process-list inference; or
- PID alone.

## 3. Mandatory SAFE-STOP fallback

If exact ownership cannot be established or revalidated:

- do not terminate Excel;
- do not infer ownership;
- do not publish an artifact; and
- return `SAFE-STOP / human inspection`.

Unrelated Excel processes must never be terminated.

## 4. Conditional hard termination

Hard termination is permitted only after all of the following have occurred:

1. the 120-second launcher deadline has expired;
2. cooperative cleanup has been requested;
3. the 30-second cleanup grace has expired; and
4. complete ownership evidence has been positively revalidated.

Immediately before termination, one stable process handle must be used to
validate:

- exact PID;
- exact process start or creation time;
- canonical `EXCEL.EXE` identity;
- invocation ID;
- sentinel identity and path;
- valid lifecycle state;
- supported lifecycle schema; and
- valid monotonic lifecycle transition.

PID alone is insufficient. Any missing, malformed, stale, inaccessible,
mismatched, ambiguous, superseded, already-exited, or otherwise unvalidated
evidence prohibits termination and requires `SAFE-STOP`.

Broad `EXCEL.EXE` termination is prohibited.

## 5. Timing decisions

- Launcher deadline: `120 seconds`.
- Cleanup grace: `30 seconds`.
- Deadline expiration does not itself authorize termination.

The required timeout progression is:

```text
LauncherDeadlineExpired
-> cooperative cleanup request
-> cleanup grace
-> exact identity revalidation
-> conditional hard termination OR SAFE-STOP
```

## 6. Evidence requirements

Future implementation must provide invocation-correlated, durable evidence
sufficient to distinguish ownership, cleanup, residual state, timeout state,
and final outcome.

Required lifecycle concepts are:

- `InvocationStarted`
- `ExcelActivationAboutToBegin`
- `ExcelOwnershipEstablished`
- `ExcelAttachmentValidated`
- `ExcelOperationStarted`
- `ExcelCleanupStarted`
- `ExcelCleanupCompleted`
- `ExcelResidualChecked`
- `InvocationSucceeded` or `InvocationFailed`

When applicable, the lifecycle must also include:

- `LauncherDeadlineExpired`
- `CleanupRequested`
- `HardTerminationCompleted`

The evidence must preserve:

- invocation ID;
- PID;
- process start time;
- executable identity;
- sentinel identity; and
- monotonic lifecycle state and correlation.

## 7. Associated correction set

Future implementation must also address:

- invocation-correlated UTF-8 JSONL build logging;
- deterministic legal worksheet handling;
- explicit residual-process states rather than unchecked Boolean `false`;
- a same-directory publication candidate;
- safe absent-target publication;
- safe existing-target replacement; and
- independent post-publication length, SHA-256, and timestamp verification.

These are implementation requirements recorded for a future, separately
authorized step. They are not implemented or authorized by this record.

## 8. Compatibility and preserved boundaries

- Thin PowerShell launcher governance interpretation: `PRESERVED`
- Official `tools/build/build.ps1` entry point: `PRESERVED`
- Public `build.ps1` parameters: `PRESERVED`
- PowerShell-controlled transaction: `PRESERVED`
- Framework-dependent `net8.0-windows` `Build.Host`: `PRESERVED`
- Late-bound Excel COM: `PRESERVED`
- New external or NuGet dependency: `NONE`
- Frozen specification change: `NONE REQUIRED / NONE AUTHORIZED`

The adopted
`docs/spec/ThinPowerShellLauncherGovernanceInterpretation.md` remains in force.
This record neither modifies that interpretation nor changes a Frozen
specification, public contract, persisted schema, canonical format, error-code
contract, or architectural boundary.

## 9. Controlled Excel/COM experiment and future verification

The Controlled Excel/COM Experiment is `NOT AUTHORIZED`.

The required sequence is:

```text
architecture implementation
-> Excel-independent restore/build/unit/static verification
-> separate readiness review
-> separate Controlled Excel/COM Experiment Execution Authorization
```

Each step requires its own applicable authorization. This governance record
does not start or authorize architecture implementation, restoration, build,
unit verification, static verification, readiness review, or an Excel/COM
experiment.

Future verification must confirm the implemented ownership, lifecycle,
timeout, cleanup, termination, evidence, and publication behavior against the
adopted decisions. Excel-independent verification must precede any separately
authorized Controlled Excel/COM Experiment.

## 10. Security status

- Avast disposition: `UNRESOLVED`
- False-positive status: `NOT ESTABLISHED`
- Safe-to-allow-list status: `NOT ESTABLISHED`
- Avast exception or exclusion: `NOT AUTHORIZED`
- Avast disablement or bypass: `NOT AUTHORIZED`

Any later Avast block requires `STOP` without retry or workaround unless the
specific action is separately authorized.

## 11. Expected future implementation scope

The currently expected future scope is descriptive only:

Modify:

- `src/Build.Host/Program.cs`
- `tools/build/build.ps1`
- `tests/unit/Build.Host/Program.cs`

Add:

- None currently identified.

This scope description does not authorize implementation or any source or test
change.

## 12. Explicit non-effects

This record does not claim or establish:

- implementation completion;
- Excel/COM verification;
- PG-01 establishment;
- an Avast false positive;
- release acceptance; or
- Git authorization.

It does not authorize source changes, restore, build, tests, `build.ps1` or
`Build.Host` execution, Excel startup, COM access, XLAM generation, network
access, Avast changes, publication, release operations, or Git mutation.

## 13. Next independent gate

The next possible step is a separate Execution-Readiness Correction
Implementation Authorization. This record does not begin or authorize that
step.
