# Excel Ownership Residual Observation Grace Governance Decision

> **Current applicability:** The amended implementation and accepted **74/74**
> offline verification were subsequently completed. That completion does not
> authorize real Experiment execution; the real Experiment remains **NO-GO /
> SAFE-STOP**.

## 1. Record status and authority

- Route: `C`
- Work mode: `architecture/governance documentation only`
- Status: `ADOPTED`
- Governing architecture:
  `docs/spec/ExcelOwnershipRotOnlyCleanupTopologyGovernanceDecision.md`
- Detailed design affected:
  `docs/spec/ExcelOwnershipRotOnlyCleanupTopologyDesign.md`
- Implementation authorization: `NOT GRANTED`
- Real-Experiment authorization: `NOT GRANTED`
- Current implementation state: `NO-GO / SAFE-STOP`
- Frozen change required: `NO`
- Security disposition: `UNRESOLVED`

This amendment adopts bounded residual observation after validated Close/Quit
cleanup. It supersedes only an interpretation of the governing architecture or
detailed design that permits immediate, single-shot residual classification.
It does not change ownership validity, cleanup-topology validity, the PID-
equality cleanup invariant, cleanup eligibility, or Close/Quit authorization.

## 2. Established forensic result

For invocation:

```text
9d6873af-12b4-4604-a123-c5cf3ef8454d
```

the established result is:

- ROT-only attachment defect: `NO`;
- double-open recurrence: `NO`;
- ownership-validation defect: `NO`;
- cleanup-topology defect: `NO`;
- persistence-order defect: `NO`;
- cleanup COM-call defect: `NO`;
- residual-accounting defect: `NO`;
- safety compromised: `NO`; and
- primary root cause: single-shot residual classification without bounded
  observation and recheck.

The current adopted design contains no bounded residual-wait requirement.
Therefore a design/governance amendment is required.

## 3. Bounded residual observation

After validated cleanup, residual determination must use bounded, read-only
polling and must not use a single process observation.

- polling interval: `100 ms`;
- maximum grace: `20 seconds`;
- observation-window start: after the cleanup COM actions have returned and
  the implementation-required COM/RCW release steps have completed; and
- time bound: measured monotonically so wall-clock changes cannot extend the
  grace period.

Return `Clear` immediately when the validated owned process identity no longer
exists. Return `Residual` only when that same validated process identity remains
present when the bounded 20-second grace expires.

Hard termination, signaling, or any other process manipulation is prohibited.

## 4. Allowed observation targets

Residual observation may target only process identities already established by
the validated ownership and cleanup-topology state. It must not:

- discover an arbitrary or new Excel process as cleanup ownership;
- infer ownership from the newest process;
- use a global active Excel instance; or
- terminate, signal, or otherwise manipulate a process.

When activation and Application identities resolve to the same PID, the
implementation observes that one physical process once per polling attempt and
maps the observation consistently to both identity roles.

## 5. Process-identity revalidation

Every residual observation must validate both:

- the PID; and
- the recorded process start time.

If the PID exists but the process start time differs, the new process must not
be treated as the owned Excel process. The original owned identity is classified
under the detailed design's non-matching or reused-PID semantics; the mismatch
must not be treated as proof that the owned process is `Residual`.

Executable identity may also be rechecked if the subsequent detailed design
determines that it is required. Start-time verification is mandatory.

## 6. Success semantics

Invocation success continues to require:

1. cleanup COM actions completed;
2. activation residual is `Clear`;
3. Application residual is `Clear`; and
4. every earlier ownership, topology, and persistence gate passed.

The bounded wait changes only residual-observation timing. It does not change:

- ownership validity;
- cleanup-topology validity;
- the PID-equality cleanup invariant;
- cleanup eligibility; or
- Close/Quit authorization rules.

A COM method return remains insufficient by itself to establish cleanup
success.

## 7. Durable evidence requirements

Residual evidence must additionally persist:

- observation start UTC;
- observation end UTC;
- polling interval;
- maximum grace;
- attempt count;
- target PID or PIDs;
- recorded process start time or times;
- final identity-validation result; and
- final residual result.

For same-PID topology, the evidence must state that one physical process was
observed for both identity roles.

The ROT attachment evidence must persist the exact integer exact-match count.
Successful attachment expects exactly `1`. The value must not later be inferred
solely from control flow. This is an audit and evidence-completeness requirement
and does not alter the ROT safety predicate.

Distinct timestamps must be persisted where practical for:

- `Workbook.Close` start;
- `Workbook.Close` end;
- `Application.Quit` start;
- `Application.Quit` end;
- residual observation start; and
- residual observation end.

Append-only evidence, the two pre-cleanup persistence gates, and independent
activation and Application residual roles remain mandatory.

## 8. Architecture and compatibility

The following remain unchanged:

- ROT-only, non-activating attachment;
- ownership/topology separation;
- PID equality as a cleanup-only invariant;
- dual persistence gates;
- append-only evidence;
- independent residual roles;
- no hard termination;
- Schema 4;
- `Build.Host` compatibility; and
- Experiment isolation.

No Frozen specification, public API, persisted schema, canonical format, or
error-code contract is changed. Schema increment beyond 4 is not required. If
implementation proves that the evidence additions cannot remain Schema-4
compatible without changing an existing field's meaning, implementation must
stop and request a separate governance decision.

## 9. Required subsequent design and implementation impact

A separately authorized design/implementation task must update at minimum:

- `ExcelOwnershipSession` residual observation;
- `RotSentinelAttachment` exact-match-count telemetry;
- Experiment contracts and evidence;
- Experiment worker evidence generation; and
- corresponding unit tests.

That later work must retire single-shot residual classification and cover the
100 ms / 20 second polling rule, same-PID observation, PID/start-time reuse,
evidence fields, timing telemetry, and the exact ROT match count.

This record does not implement or authorize those changes.

## 10. Current execution state

Invocation `9d6873af-12b4-4604-a123-c5cf3ef8454d` consumed its one-use
authorization. No retry is authorized, and the current candidate is not
reusable for another real Experiment.

Correction, full offline verification, a new candidate, and fresh governance
are required before any future real Experiment. Each remains an independent
authorization gate. The security disposition remains `UNRESOLVED`.

## 11. Explicit non-effects and non-actions

This amendment does not authorize or perform:

- source, test, tool, or implementation changes;
- restore, build, test, or Experiment execution;
- Excel startup, COM access, or process manipulation;
- candidate creation;
- `Build.Host` or `build.ps1` execution;
- network or external-service access;
- security-control changes;
- access to the prohibited U03 record;
- release, package, or `dist` operations; or
- Git staging, commit, push, or any other Git mutation.

No new execution authorization is introduced.

## 12. Governance outcome and next independent gate

The governance outcome is:

```text
A. AMENDMENT ADOPTED - READY FOR NARROW DESIGN/IMPLEMENTATION
```

This means ready only for a separately authorized narrow design/implementation
packet followed by full offline verification. The current implementation and
current candidate remain `NO-GO / SAFE-STOP` for a real Experiment.
