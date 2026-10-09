# Excel Ownership ROT-Only Attachment and Cleanup-Topology Governance Decision

## 1. Record status

- Route: `C`
- Work mode: `architecture/governance documentation only`
- Status: `ADOPTED`
- ROT-only sentinel attachment: `ADOPTED`
- Separate cleanup-topology validation: `ADOPTED`
- Design review: `REQUIRED / NOT PERFORMED`
- Implementation authorization: `NOT GRANTED`
- Future Experiment authorization: `NOT GRANTED`
- Excel/COM execution authorization: `NOT GRANTED`
- Frozen modified: `NONE`
- Security disposition: `UNRESOLVED`

The User explicitly adopts ROT-only sentinel attachment together with separate
cleanup-topology validation. This decision responds to repeated real-Excel
failures in which canonical `/x` activation, the workbook `Application` HWND
owner, sentinel file locking, and cleanup did not converge on one Excel
process. Unmodified re-execution of the current architecture is
`NO-GO / SAFE-STOP`.

This is a governance decision only. It does not authorize design completion,
implementation, verification execution, Excel/COM execution, another
Experiment, security-control changes, release activity, or Git mutation.

## 2. Activation

Canonical activation remains:

```text
EXCEL.EXE /x <sentinel>
```

`ActivationProcessIdentity` remains independently observed. Ownership must not
be inferred merely from the activation PID.

## 3. ROT-only attachment

For this ownership path, activating attachment through
`Marshal.BindToMoniker(sentinelPath)` is retired. Attachment must instead use a
ROT-only lookup that:

1. inspects the Running Object Table without activating another Excel server;
2. accepts only an already-registered workbook moniker corresponding to the
   exact sentinel;
3. does not itself create or open a second workbook or Excel instance;
4. waits and retries observation only within a bounded, design-defined
   attachment window when the exact sentinel is not yet registered; and
5. fails closed if the exact sentinel never appears.

The following are explicitly prohibited as ownership attachment mechanisms:

- `Marshal.BindToMoniker`;
- `GetActiveObject`;
- a global active `Application`;
- newest-process inference; and
- arbitrary workbook enumeration across unrelated Excel instances.

The exact non-activating ROT enumeration mechanism and canonical sentinel-path
comparison are unresolved design-review items. This decision does not select
their implementation mechanics.

## 4. Ownership validation

`ActivationProcessIdentity` and `ApplicationProcessIdentity` remain separate
first-class identities.

Application identity validation continues to require:

- exact sentinel correlation;
- exact invocation correlation;
- HWND-owner PID observation;
- independent process start-time observation;
- independent executable-path observation; and
- stability and reread checks.

Activation-PID equality is not restored as a general ownership-validity
predicate. An `ApplicationProcessIdentity` may be valid even when:

```text
Activation PID != Application HWND-owner PID
```

## 5. Cleanup-topology validation

`CleanupTopologyValidated` is a distinct result. It is separate from ownership
validation and `ApplicationProcessIdentity` validation.

Close/Quit eligibility requires every condition below:

1. `ApplicationProcessIdentity` is valid.
2. Identity evidence is durably persisted.
3. `ActivationProcessIdentity` is stable and revalidated.
4. `ApplicationProcessIdentity` is stable and revalidated.
5. Activation PID equals the Application HWND-owner PID.
6. Process start time remains consistent.
7. Executable identity remains consistent.
8. Exact sentinel and invocation correlation remain valid.

Only after all conditions pass:

```text
CleanupTopologyValidated = true
CleanupEligible = true
```

PID equality is therefore a cleanup-safety invariant, not a general
ownership-inference or ownership-validity invariant.

## 6. PID mismatch

If the activation PID differs from the Application HWND-owner PID:

- ownership evidence may still be recorded;
- `CleanupTopologyValidated = false`;
- `CleanupEligible = false`;
- `Workbook.Close` must not be called;
- `Application.Quit` must not be called;
- the activation process must not be manipulated;
- residual state must be recorded; and
- processing must fail closed as `SAFE-STOP`.

Hard termination is not authorized.

## 7. Durable pre-cleanup evidence

Append-only evidence must be durably persisted before cleanup and must be
sufficient to establish:

- `ActivationProcessIdentity`;
- `ApplicationProcessIdentity`;
- the ownership-validation result;
- the cleanup-topology-validation result;
- persistence confirmation; and
- the cleanup-eligibility decision.

Evidence must distinguish `OwnershipValidated` from
`CleanupTopologyValidated`. An identity artifact that is later overwritten
must not be relied upon to prove the original pre-cleanup ordering.
Lifecycle and evidence records must identify both the activation PID and the
Application PID wherever relevant.

## 8. Cleanup completion and residual validation

Close/Quit may operate only on the validated `Application` object and only
after `CleanupTopologyValidated` is true.

A successful COM method return does not by itself establish successful
cleanup. Success additionally requires:

1. cleanup calls completed;
2. residual observation was performed;
3. activation residual is `Clear`;
4. Application residual is `Clear`; and
5. the final expected Excel state is satisfied.

## 9. Retained safety boundaries

The following remain mandatory:

- no hard termination;
- no Avast or other security bypass;
- no process killing;
- fail closed on incomplete identity or topology; and
- the zero-Excel baseline remains specific to controlled Experiments.

The security disposition remains `UNRESOLVED`.

## 10. Relationship to prior governance

This decision must be read with
`docs/spec/ExcelOwnershipApplicationProcessIdentityGovernanceDecision.md`.
Historical governance records are not erased or rewritten.

The prior decision remains valid that activation-PID equality is not a
universal ownership-validity invariant. This decision supersedes any
interpretation of prior governance under which a validated
`ApplicationProcessIdentity` alone is sufficient for cleanup eligibility.
PID equality is required specifically for cleanup-topology validation.

This decision changes no Frozen specification, public API, persisted schema,
canonical format, or error-code contract. Any future discovery that the
adopted architecture requires a Frozen change must result in `SAFE-STOP` and a
separate report rather than modification under this authority.

## 11. Current execution state

The previous invocation:

```text
806af2b0-6c6c-4647-8a4b-6074f8fa40fc
```

failed. Its one-use authorization is `CONSUMED / NON-REUSABLE`. No retry is
authorized. The current implementation is not authorized for another real
Experiment.

New design review and implementation verification are required before any
future candidate, preflight, or execution sequence. Neither this adoption nor
future successful design or verification would itself authorize an
Experiment; every later gate remains independent.

## 12. Unresolved design-review items

The next phase must resolve, without treating this record as implementation
authority:

1. the exact non-activating ROT enumeration mechanism;
2. exact canonical comparison for the sentinel moniker and path;
3. ROT polling interval and attachment timeout;
4. behavior if more than one matching ROT entry appears;
5. exact revalidation timing before cleanup;
6. the new cleanup-topology DTO or state representation;
7. lifecycle event names and transitions;
8. append-only evidence artifact structure;
9. residual accounting when ownership is valid but topology is invalid;
10. required `Build.Host` compatibility changes;
11. required Experiment and test updates; and
12. whether the existing public facade can remain unchanged.

No implementation detail in this list is decided here.

## 13. Explicit non-effects and non-actions

This record does not authorize or perform:

- source, test, tool, or implementation changes;
- restore, build, test, or Experiment execution;
- Excel startup, COM access, or process manipulation;
- `Build.Host` or `build.ps1` execution;
- network or external-service access;
- security-control changes;
- access to the prohibited U03 record;
- release, package, or `dist` operations; or
- Git staging, commit, push, or any other Git mutation.

No new Experiment authorization is introduced. No Frozen specification is
modified.

## 14. Governance outcome and next independent gate

The governance outcome is:

```text
A. ARCHITECTURE ADOPTED - READY FOR DESIGN REVIEW
```

The next possible step is a separately authorized design review. This record
does not begin or authorize that review, implementation, verification,
candidate preparation, preflight, cleanup, retry, or future Experiment.
