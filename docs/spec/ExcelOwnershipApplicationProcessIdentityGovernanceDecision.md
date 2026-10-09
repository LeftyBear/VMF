# Excel Ownership Application Process Identity Governance Decision

## 1. Record status

- Route: `C`
- Work mode: `docs-only governance decision record`
- Status: `ADOPTED`
- Option C: `ADOPTED`
- Evidence completion review: `COMPLETE`
- Ownership architecture revision: `REQUIRED / ADOPTED`
- Experiment evidence schema: `VERSION 4 ADOPTED`
- Frozen modified: `NONE`
- Implementation authorization: `NOT GRANTED`
- Future Experiment authorization: `NOT GRANTED`
- Excel/COM execution authorization: `NOT GRANTED`
- Avast disposition: `UNRESOLVED`

This record adopts Option C from the supplied authoritative Exact Evidence
Completion Review. It separates activation-process identity from the identity
of the process that owns the bound workbook's `Application.Hwnd`.

This is a governance decision only. It does not authorize implementation,
design execution, restore, build, test, Excel/COM execution, another
Experiment, security-control changes, release activity, or Git mutation.

## 2. Basis and current Experiment result

The authoritative review established:

- activation PID: `34952`;
- `Application.Hwnd`-derived PID: `32976`;
- sentinel path correlation: `MATCH`;
- invocation correlation: `MATCH`;
- PID equality: `FAIL`;
- schema-3 evidence mixed HWND PID `32976` with start-time and executable data
  from activation PID `34952`; and
- PID `32976` start time and executable identity were not independently
  recorded.

The existing fail-closed response was correct. Experiment #2 remains
`SAFE-STOP / FAILED OWNERSHIP VALIDATION`. This adoption does not reclassify
that result, does not make either PID retrospectively cleanup-eligible, and
grants no retrospective cleanup or retry authorization.

## 3. Adopted identity model

### 3.1 ActivationProcessIdentity

`ActivationProcessIdentity` represents the process returned by launching:

```text
EXCEL.EXE /x <invocation-specific-sentinel>
```

Its required evidence is:

- PID;
- process start time;
- canonical executable path;
- launch timestamp and evidence;
- lifetime or exit observation; and
- residual state.

### 3.2 ApplicationProcessIdentity

`ApplicationProcessIdentity` represents the process that actually owns the
bound workbook's `Application.Hwnd`.

Its required evidence is:

- HWND;
- HWND-owner PID;
- process start time independently read from that PID;
- canonical executable path independently read from that PID;
- observation timestamp;
- exact sentinel canonical path;
- bound workbook canonical path;
- invocation correlation value;
- validation state;
- cleanup eligibility; and
- residual state.

These are distinct first-class identities even when their PIDs are equal.
Activation-PID equality with the HWND-owner PID may be recorded as evidence,
but it is retired as a mandatory ownership invariant and is not sufficient by
itself to establish ownership.

## 4. Application ownership invariant

`ApplicationProcessIdentity` may become validated only when every condition
below holds:

1. Experiment preflight established an Excel process count of exactly zero.
2. The exact invocation-specific sentinel was created.
3. Activation used the canonical resolved `EXCEL.EXE`.
4. Sentinel-specific moniker attachment returned the workbook.
5. The bound workbook canonical path equals the exact expected sentinel path.
6. The workbook invocation correlation equals the exact invocation ID.
7. `Application.Hwnd` was obtained from that bound workbook.
8. The HWND-owner PID was independently obtained.
9. The HWND-owner process start time was independently obtained from that PID.
10. The HWND-owner canonical executable was independently obtained from that
    PID.
11. That executable equals the canonical expected `EXCEL.EXE`.
12. Complete evidence was durably persisted before cleanup eligibility.

Missing, conflicting, inaccessible, incomplete, or ambiguous identity evidence
requires `SAFE-STOP / HUMAN DISPOSITION`.

## 5. Prohibited ownership inference

Ownership must not be inferred from:

- `GetActiveObject`;
- a global active Excel instance;
- newest-process selection;
- arbitrary process enumeration or guessing;
- process-count ordering;
- a parent/child relationship alone; or
- activation-PID equality as the sole ownership test.

Sentinel identity and invocation correlation remain mandatory. Unrelated-Excel
protection remains fail-closed, and broad Excel discovery does not establish
ownership.

## 6. Cleanup and residual-state rules

`Application.Quit` is permitted only after `ApplicationProcessIdentity` is
fully validated. `ActivationProcessIdentity` must not automatically be treated
as the Application to quit.

Residual state must be tracked separately for:

- `ActivationProcessIdentity`; and
- `ApplicationProcessIdentity`.

If either identity is incomplete, conflicting, inaccessible, or ambiguous,
the required outcome is `SAFE-STOP / HUMAN DISPOSITION`.

Hard termination is not authorized for the non-production Experiment. This
Experiment-specific prohibition does not expand, invoke, or reinterpret any
separate production cleanup authority recorded elsewhere.

## 7. Evidence schema version 4

Experiment evidence schema version 4 is adopted because schema 3 conflates
activation-process and application-process semantics. Schema 4 must make the
identities separate first-class structures and must not silently reinterpret
existing schema-3 fields.

The minimum `activationProcessIdentity` concepts are:

- `pid`;
- `startTimeUtc`;
- `canonicalExecutablePath`;
- launch evidence and timestamp;
- lifetime or exit state; and
- residual state.

The minimum `applicationProcessIdentity` concepts are:

- `hwnd`;
- `pid`;
- `independentlyObservedStartTimeUtc`;
- `independentlyObservedCanonicalExecutablePath`;
- observation timestamp;
- expected and actual sentinel path;
- `sentinelPathMatch`;
- expected and actual invocation correlation;
- `invocationCorrelationMatch`;
- executable match;
- validation state;
- cleanup eligibility; and
- residual state.

Schema 4 must also record acquisition provenance and timestamps, explicit
component comparisons, and an aggregate application-identity validation
result. Cross-identity reuse of fields is prohibited.

## 8. Retained Experiment preflight and timing controls

The following controls remain in force:

- Excel process count exactly zero for controlled experiments;
- exact pinned artifact verification;
- an isolated invocation-specific temporary root;
- endpoint-protection fail-closed handling;
- one invocation per explicit authorization;
- a 120-second controller deadline;
- a 30-second cooperative cleanup grace; and
- no hard termination.

Deadline or cleanup-grace expiration does not relax identity validation or
authorize termination.

## 9. Production compatibility and preserved boundaries

This governance adoption does not change:

- the `Build.Host` CLI;
- VBA import;
- `SaveAs(55)`;
- publication;
- `build.ps1`;
- successful production behavior; or
- any Frozen specification.

A later, separately authorized implementation and design review must determine
how shared ownership primitives affect `Build.Host` failure-path behavior while
preserving successful production behavior. This record does not authorize or
perform that review or any resulting implementation.

## 10. Security status

- Avast disposition: `UNRESOLVED`
- Security exception, allow-list, or exclusion: `NOT AUTHORIZED`
- Security disablement or bypass: `NOT AUTHORIZED`

An endpoint-protection detection or block remains fail-closed. This record does
not establish a false positive or grant retry, workaround, or security-change
authority.

## 11. Explicit non-effects and non-actions

This record does not authorize:

- production or test implementation;
- source, test, schema implementation, or tool changes;
- restore, build, test, or Experiment execution;
- `build.ps1` or `Build.Host` execution;
- Excel startup, COM access, or process manipulation;
- another Experiment invocation;
- network or external-service access;
- security-control changes;
- release, package, or `dist` operations; or
- Git staging, commit, push, or any other Git mutation.

In creating this record:

- only this governance record was created;
- no production source, test, tool, Frozen specification, or existing
  governance record was modified;
- no restore, build, test, Experiment, Excel/COM, `build.ps1`, or `Build.Host`
  execution occurred;
- no process manipulation, network access, or security-control change
  occurred;
- no Git mutation was performed; and
- `docs/development/U03_CAND-02_ConcreteCandidateDefinition.md` was not
  accessed.

## 12. Governance decision and next independent gate

Option C and Experiment evidence schema version 4 are `ADOPTED`.

The governance outcome is:

```text
A. GOVERNANCE ADOPTED - READY FOR SCHEMA-4 DESIGN REVIEW
```

The schema-4 design review is a separate gate. This record does not begin or
authorize that review, implementation, verification, cleanup, retry, or any
future Experiment.
