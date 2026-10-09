# Excel Ownership ROT-Only Attachment and Cleanup-Topology Detailed Design

> **Current applicability:** The designed implementation and accepted **74/74**
> offline verification were subsequently completed. Current residual behavior
> is amended by
> `ExcelOwnershipResidualObservationGraceGovernanceDecision.md`. The real
> Experiment remains separately **NO-GO / SAFE-STOP**.

## 1. Record status and authority

- Route: `C`
- Work mode: `architecture design review only`
- Status: `DESIGN REVIEW COMPLETE`
- Governing decision:
  `docs/spec/ExcelOwnershipRotOnlyCleanupTopologyGovernanceDecision.md`
- Implementation authorization: `NOT GRANTED`
- Real-Experiment authorization: `NOT GRANTED`
- Current implementation state: `NO-GO / SAFE-STOP`
- Frozen change required: `NO`

This design implements the adopted distinction between application ownership
and cleanup topology. It does not alter the canonical `/x` activation, infer
ownership from the activation PID, authorize an Experiment, or clear the
unresolved security disposition.

## 2. Adopted architecture

The ownership path has five independent gates:

1. launch and observe `ActivationProcessIdentity`;
2. find the exact sentinel by non-activating ROT enumeration;
3. validate `ApplicationProcessIdentity` without requiring PID equality;
4. durably persist ownership and cleanup-topology evidence; and
5. permit `Workbook.Close` and `Application.Quit` only when cleanup topology
   is validated and its evidence persistence is confirmed.

The invariant is:

```text
CleanupEligible =
    OwnershipValidated
    AND IdentityEvidenceDurablyPersisted
    AND CleanupTopologyValidated
    AND CleanupTopologyEvidenceDurablyPersisted
```

PID equality participates only in `CleanupTopologyValidated`. It is not an
ownership predicate.

## 3. ROT-only sentinel attachment

### 3.1 Implementation shape

Add an internal `RotSentinelAttachment` helper in the Primitives assembly. It
uses the framework COM types in `System.Runtime.InteropServices.ComTypes`:

- `IRunningObjectTable`;
- `IEnumMoniker`;
- `IMoniker`; and
- `IBindCtx`.

It declares only these `ole32.dll` imports:

```csharp
[DllImport("ole32.dll")]
private static extern int GetRunningObjectTable(
    int reserved,
    out IRunningObjectTable runningObjectTable);

[DllImport("ole32.dll")]
private static extern int CreateBindCtx(
    int reserved,
    out IBindCtx bindContext);
```

Both return values are checked with `Marshal.ThrowExceptionForHR`. No custom
COM interface definition or external package is required.

For each polling observation the helper:

1. calls `GetRunningObjectTable(0, ...)`;
2. calls `IRunningObjectTable.EnumRunning`;
3. creates one bind context with `CreateBindCtx(0, ...)`;
4. calls `IEnumMoniker.Reset`, then `Next(1, ...)` until exhausted;
5. calls `IMoniker.GetDisplayName` for each entry;
6. applies the exact, syntactic path acceptance algorithm in section 3.2;
7. retains only the matching moniker for the duration of that observation;
8. fails closed if more than one ROT entry matches; and
9. calls `IRunningObjectTable.GetObject` exactly once, and only on the sole
   exact matching moniker.

`GetRunningObjectTable`, `EnumRunning`, `Next`, and `GetDisplayName` are
observation-only. `IRunningObjectTable.GetObject` obtains an interface to an
object already registered as running; it is not an activating moniker bind.
Subsequent workbook/Application property reads and cleanup methods are COM
calls into that already-running server. The design contains no operation that
activates Excel during attachment.

The future ownership path must not contain `Marshal.BindToMoniker`,
`GetActiveObject`, global active-Application lookup, workbook enumeration,
process recency selection, or fallback selection.

### 3.2 Exact moniker matching

The invocation supplies the expected sentinel path. Before launch it is
normalized once with `Path.GetFullPath`. The normalized path must be fully
qualified, use a drive-letter root, and identify the invocation-created local
sentinel. UNC paths, URI forms, device paths, relative paths, environment
variables, shell names, and display names containing moniker decorations are
not accepted.

An ROT display name is a candidate local file path only when all of the
following hold:

- it is non-empty and contains no NUL;
- it is not a URI and has no `file:` prefix;
- it is not UNC, device-prefixed, or root-relative;
- `Path.IsPathFullyQualified` is true;
- `Path.GetPathRoot` is exactly a drive-letter root; and
- `Path.GetFullPath` succeeds without any filesystem probe.

The candidate matches only when its normalized full path equals the normalized
expected sentinel under `StringComparer.OrdinalIgnoreCase`. There is no
filename-only, suffix, substring, short-name, symlink, final-handle, existence,
or network-based resolution. A display name that cannot be canonicalized by
these purely syntactic rules is a non-match. `book.FullName` is independently
normalized by the same rules and reread during validation; it does not replace
the ROT display-name match.

### 3.3 Polling window

- interval: `100 ms`;
- maximum attachment window: `20 seconds` measured by `Stopwatch`;
- cancellation: checked before each enumeration, after each enumeration, and
  by `Task.Delay(remaining-or-100ms, cancellationToken)`;
- timeout: fail closed without calling `GetObject` on any non-match.

Twenty seconds leaves 100 seconds of the controller's 120-second overall
deadline for identity evidence, cleanup-topology evidence, normal cleanup,
residual observation, and result persistence. A 100 ms interval is responsive
without a busy loop. The deadline is monotonic; wall-clock changes cannot
extend it.

`RotAttachmentWaiting` is recorded before the first enumeration. Exactly one
match leads to `RotAttachmentObserved`. Zero matches continue until the bound.
More than one matching ROT entry records `RotAttachmentAmbiguous` and stops
immediately. The implementation must not choose the first, newest, or any PID.

### 3.4 COM lifetime and apartment

The worker entry point must run the entire attachment, identity observation,
validated use, and cleanup sequence on one explicitly initialized STA thread.
The helper must check `Thread.CurrentThread.GetApartmentState() == STA` and fail
before enumeration otherwise. It must not marshal workbook/Application RCWs to
another thread.

Every COM reference is released deterministically in reverse acquisition
order. Per-entry monikers are released in a `finally`; the retained matching
moniker is released after `GetObject`; the enumerator, bind context, and ROT
are released after every observation. The workbook and Application RCWs remain
owned by `ExcelOwnershipSession` and are released there. Release uses
`Marshal.FinalReleaseComObject` only for a non-null COM object. A release
failure is retained as a failure and must not make cleanup successful. GC/finalizer
passes may remain defensive after deterministic release, but are not evidence
of cleanup.

Enumeration HRESULT failure, display-name failure, canonicalization failure,
`GetObject` failure, cancellation, timeout, invalid apartment, or unexpected
COM failure all fail closed. Per-entry display names that are merely unsafe to
canonicalize are non-matches; infrastructure failures are not silently
downgraded to non-matches.

## 4. Ownership validation

`ActivationProcessIdentity` and `ApplicationProcessIdentity` remain separate
public immutable records. `ApplicationIdentityValidation.Compare` retains all
current checks:

- exact sentinel ROT match and exact `book.FullName` match;
- exact invocation ID;
- non-zero HWND and valid HWND-owner PID;
- independently observed HWND-owner start time and executable;
- expected executable match;
- observation stability and PID-reuse rejection.

`ActivationPidMatchesApplicationPid` remains recorded but remains excluded
from `ApplicationIdentityValidation.AggregateMatch`. No existing identity DTO
property needs to be removed or redefined. `ApplicationProcessIdentity` must
stop carrying authoritative cleanup eligibility; its existing
`CleanupEligible` property is retained for source/serialization compatibility
but is populated only from the aggregate session gate defined in section 2.

## 5. Cleanup-topology data model

Add this public immutable DTO to Primitives:

```csharp
public sealed record CleanupTopologyValidation(
    int ActivationProcessId,
    int ApplicationProcessId,
    bool ProcessIdsMatch,
    bool ActivationStartTimeStable,
    bool ActivationExecutableStable,
    bool ApplicationStartTimeStable,
    bool ApplicationExecutableStable,
    bool SentinelRevalidated,
    bool InvocationRevalidated,
    DateTimeOffset ObservationStartedUtc,
    DateTimeOffset ObservationCompletedUtc,
    bool AggregateMatch);
```

`AggregateMatch` is true only when every Boolean component is true, both PIDs
are positive, timestamps are non-default and ordered, and the observations
were obtained in the same pre-cleanup pass. Construction is centralized in a
static `Compare` method so callers cannot supply an inconsistent aggregate.

Add read-only `CleanupTopologyValidation? CleanupTopologyValidation` and
`bool CleanupTopologyValidated` properties to `ExcelOwnershipLease`. These are
additive facade changes. Raw COM and `RotSentinelAttachment` remain internal;
the Experiment receives no friend access.

## 6. Exact pre-cleanup sequence and responsibility

The sequence is mandatory:

| Step | Operation | Responsibility |
|---:|---|---|
| 1 | ROT-only attach | `ExcelOwnershipSession` via `RotSentinelAttachment` |
| 2 | Observe HWND-owner application identity | `ExcelOwnershipSession` / `ProcessIdentityObserver` |
| 3 | Compute ownership validation | `ApplicationIdentityValidation.Compare` |
| 4 | Create ownership-validation evidence | Experiment `ExperimentWorker` and `ExperimentEvidence.WriteSingle` |
| 5 | Read back, validate, and create persistence confirmation | Experiment `ExperimentWorker` |
| 6 | Record `ApplicationProcessIdentityValidated` | `ExcelOwnershipSession` after confirmation |
| 7 | Re-observe activation identity | `ExcelOwnershipSession` / `ProcessIdentityObserver` |
| 8 | Re-observe application HWND-owner identity | `ExcelOwnershipSession` / `ProcessIdentityObserver` |
| 9 | Reread sentinel full path and invocation property | `ExcelOwnershipSession` |
| 10 | Compute `CleanupTopologyValidation` | its centralized `Compare` method |
| 11 | Create cleanup-topology evidence | Experiment `ExperimentWorker` |
| 12 | Read back, validate, and create persistence confirmation | Experiment `ExperimentWorker` |
| 13 | Record `CleanupTopologyValidated` | `ExcelOwnershipSession` after confirmation |
| 14 | Set aggregate `CleanupEligible = true` | `ExcelOwnershipSession` only |
| 15 | call `Close(false)` and `Quit()` | `ExcelOwnershipSession.Dispose` |

The facade therefore needs additive methods that cannot be confused with a
bare assertion:

```text
ConfirmOwnershipEvidencePersisted(OwnershipEvidenceConfirmation)
ObserveCleanupTopology()
ConfirmCleanupTopologyEvidencePersisted(CleanupTopologyEvidenceConfirmation)
```

Each confirmation contains invocation ID, artifact path, SHA-256 of the exact
persisted bytes, and the confirmation timestamp. The session checks the
invocation, expected artifact kind, non-empty SHA-256, and ordering. The caller
must first read back and validate the create-new artifact. The old parameterless
`ConfirmIdentityEvidencePersisted()` is retained only as an obsolete,
fail-closed compatibility member that throws; no successful path may call it.

Any failure through step 13 keeps `CleanupEligible` false. In particular, PID
mismatch produces `CleanupTopologyValidated = false`, records rejection,
performs no Close/Quit or activation-process manipulation, observes residuals,
and ends failed/SAFE-STOP.

## 7. Append-only evidence design

### 7.1 Artifacts

All pre-cleanup artifacts use `FileMode.CreateNew`, UTF-8 compact JSON, and
`FileOptions.WriteThrough`, followed by `Flush(true)`, close, readback,
correlation validation, and SHA-256 calculation. An existing filename is a
failure; no artifact is replaced or overwritten.

For invocation `<id>` under the already validated temporary root:

| Filename | Record | Meaning |
|---|---|---|
| `<id>.ownership-validation.json` | `OwnershipValidationEvidence` | original activation/application observations and ownership comparison |
| `<id>.ownership-persisted.json` | `EvidencePersistenceConfirmation` | readback hash and persistence confirmation for ownership evidence |
| `<id>.cleanup-topology.json` | `CleanupTopologyEvidence` | both original identities, both re-observations, sentinel/invocation rereads, and aggregate topology result |
| `<id>.cleanup-topology-persisted.json` | `EvidencePersistenceConfirmation` | readback hash and persistence confirmation for topology evidence |
| `<id>.cleanup-attempt.json` | `CleanupAttemptEvidence` | eligibility decision and whether Close/Quit were attempted and returned/threw |
| `<id>.residual-observation.json` | `ResidualObservationEvidence` | independent activation/application residual observations and final cleanup result |

The existing `<id>.identity-validation.json` name is retired for new runs; it
is not reused as a mutable status snapshot. Existing failure-result
serialization remains supported. The final `<id>.result.json` remains
create-new and carries the final identities/residuals as today, but it is not
used to prove pre-cleanup ordering.

Every record contains `SchemaVersion`, `InvocationId`, `RecordedUtc`, and an
artifact-kind discriminator. Identity/topology records contain both PIDs and
the complete relevant DTOs. A confirmation additionally contains the target
artifact filename, byte length, SHA-256, target record timestamp, and its own
later confirmation timestamp. Cleanup attempt and residual records correlate
to the hashes of both persistence confirmations. Timestamps mean completion
of the named observation or durable write; start/end fields are used where an
operation spans time.

Lifecycle JSONL remains append-only and correlates by invocation ID, sequence,
and timestamp. It may summarize state, but the create-new artifacts above are
the authoritative detailed evidence.

### 7.2 Schema and compatibility impact

These are new Schema-4 Experiment evidence record kinds, not a reinterpretation
of an existing record and not a Schema-5 change. `ExperimentContracts.SchemaVersion`
remains `4`. The existing public command line, `ExperimentResult`, failure
shape, and serialized `EvidencePaths` shape remain unchanged. Supplemental
paths are derived deterministically from the validated temporary root and
invocation ID and are validated by the controller.

Therefore:

- schema increment beyond 4 required: `NO`;
- separate governance approval for a schema increment: `NOT REQUIRED`;
- implementation authorization: still separately required.

If implementation proves that a Schema-4 consumer cannot accept the new
artifact set without changing an existing field's meaning, implementation
must stop and request governance approval for a version increment.

## 8. Lifecycle state machine

Only reachable transitions are listed. An error at any nonterminal state may
transition to `InvocationFailed` after any required safe residual observation;
that general failure edge never grants cleanup eligibility.

```text
InvocationStarted
  -> ExcelActivationAboutToBegin
  -> ActivationProcessIdentityObserved
  -> RotAttachmentWaiting

RotAttachmentWaiting
  -> RotAttachmentWaiting
  -> RotAttachmentObserved
  -> RotAttachmentAmbiguous
  -> RotAttachmentTimedOut

RotAttachmentObserved
  -> ApplicationProcessIdentityObserved

ApplicationProcessIdentityObserved
  -> ApplicationProcessIdentityRejected
  -> OwnershipEvidencePersisted

OwnershipEvidencePersisted
  -> OwnershipPersistenceConfirmed

OwnershipPersistenceConfirmed
  -> ApplicationProcessIdentityValidated

ApplicationProcessIdentityValidated
  -> CleanupTopologyObserved

CleanupTopologyObserved
  -> CleanupTopologyRejected
  -> CleanupTopologyEvidencePersisted

CleanupTopologyEvidencePersisted
  -> CleanupTopologyPersistenceConfirmed

CleanupTopologyPersistenceConfirmed
  -> CleanupTopologyValidated

CleanupTopologyValidated
  -> ExcelOperationStarted
  -> ExcelCleanupStarted

ExcelOperationStarted
  -> ExcelCleanupStarted

ExcelCleanupStarted
  -> ExcelCleanupActionsCompleted
  -> ExcelCleanupFailed

RotAttachmentAmbiguous
RotAttachmentTimedOut
ApplicationProcessIdentityRejected
CleanupTopologyRejected
  -> ExcelCleanupSkippedUnsafeTopology

ExcelCleanupSkippedUnsafeTopology
ExcelCleanupActionsCompleted
ExcelCleanupFailed
  -> ExcelResidualChecked

ExcelResidualChecked
  -> ExcelCleanupCompleted
  -> ExcelCleanupFailed

ExcelCleanupCompleted
  -> InvocationSucceeded
  -> InvocationFailed
```

`InvocationSucceeded` and `InvocationFailed` are terminal. A rejected or
missing topology cannot transition to `ExcelCleanupStarted`, directly or
through `ExcelOperationStarted`. `CleanupTopologyValidated` is unreachable
until both topology evidence records are durable and confirmed. Waiting is a
state recorded once plus bounded diagnostic observations; repeated lifecycle
entries must remain sequence-valid and are not required for every poll.

## 9. Residual semantics

Residual observation is independent for the two recorded identities. When the
PIDs are equal, the process is observed once but that observation is applied
to both identity roles because both records identify the same PID and stable
start time. When PIDs differ, each PID is observed separately. A PID lookup is
accepted only when its start time still matches the recorded identity; a
different start time is `CheckFailed`, not proof of either Clear or Residual.

| Case | Activation residual | Application residual | Cleanup action | PASS |
|---|---|---|---|---|
| A. same PID, process exits | `Clear` | `Clear` | attempted and completed | yes, if every earlier gate passed |
| B. same PID, process remains | `Residual` | `Residual` | attempted | no |
| C. different PIDs, topology rejected | independently observed | independently observed | not attempted | no; SAFE-STOP |
| D. application exits, activation remains | `Residual` | `Clear` | topology-dependent; mismatch means skipped | no |
| E. activation exits, application remains | `Clear` | `Residual` | topology-dependent; mismatch means skipped | no |
| F. both remain | `Residual` | `Residual` | topology-dependent | no |

`CheckFailed` is preserved independently for either identity. One remaining
physical PID does not make both roles `Residual` unless both identities record
that same stable PID. Exact PASS requires validated ownership, durably
confirmed ownership evidence, validated and durably confirmed cleanup
topology, successful cleanup calls, a completed residual check, and both
residual states `Clear`.

## 10. Build.Host and public compatibility

- Public facade change: `ADDITIVE`.
- Build.Host source change: `REQUIRED`.
- CLI/result contract change: `NO`.
- Frozen change required: `NO`.

`Build.Host` must replace its call to the obsolete parameterless confirmation
with the new confirmation sequence and persist equivalent create-new
pre-cleanup evidence under its existing invocation work directory. This is a
source change, but the public production CLI, request/result JSON, artifact
contract, and transaction ordering remain unchanged. The added ownership and
topology persistence occurs inside the existing ownership-start boundary,
before `Session.Workbook` is exposed; inventory still precedes ownership,
worksheet/import/save/metadata still follow validated ownership, cleanup still
precedes publication, and publication remains last.

No raw COM type becomes public. No Experiment friend access is added.

## 11. Expected implementation impact

Expected modified files:

- `src/Build.ExcelOwnership.Primitives/ExcelOwnershipSession.cs`
- `src/Build.ExcelOwnership.Primitives/ExcelOwnershipLease.cs`
- `src/Build.ExcelOwnership.Primitives/ApplicationIdentityValidation.cs`
- `src/Build.ExcelOwnership.Primitives/ApplicationProcessIdentity.cs`
- `src/Build.ExcelOwnership.Primitives/LifecycleJournal.cs`
- `src/Build.ExcelOwnership.Experiment/ExperimentEvidence.cs`
- `src/Build.ExcelOwnership.Experiment/ExperimentContracts.cs`
- `src/Build.ExcelOwnership.Experiment/ExperimentWorker.cs`
- `src/Build.ExcelOwnership.Experiment/ExperimentController.cs`
- `src/Build.Host/Program.cs`
- `tests/unit/Build.ExcelOwnership.Primitives/Program.cs`
- `tests/unit/Build.ExcelOwnership.Experiment/Program.cs`
- `tests/unit/Build.ExcelOwnership.Experiment/StructuralIsolationTests.cs`
- `tests/unit/Build.Host/Program.cs`

Expected added files:

- `src/Build.ExcelOwnership.Primitives/RotSentinelAttachment.cs`
- `src/Build.ExcelOwnership.Primitives/CleanupTopologyValidation.cs`
- `src/Build.ExcelOwnership.Primitives/EvidencePersistenceConfirmation.cs`

No Frozen specification, project dependency, package reference, public request,
or public result record is expected to change.

## 12. Implementation-test matrix

All tests use fakes for ROT enumeration, monikers, process observation,
workbook/Application access, persistence, and time. No real Excel is required.

### 12.1 ROT attachment

| Test | Expected result |
|---|---|
| no matching moniker through 20 seconds | bounded timeout; no `GetObject`; fail closed |
| exactly one exact path | `GetObject` once for that moniker; attach |
| two exact matching entries | ambiguity recorded; no `GetObject`; fail closed |
| unrelated workbook entries | ignored; never passed to `GetObject` |
| case-only path difference | match |
| filename-only/suffix/substring match | non-match |
| relative, URI, UNC, device, decorated, malformed display name | non-match without filesystem/network resolution |
| cancellation during wait | immediate fail closed; no later `GetObject` |
| non-STA caller | rejected before ROT enumeration |
| COM enumeration failure | fail closed and release acquired references |
| source structure | no `BindToMoniker`, `GetActiveObject`, active Application, or workbook enumeration |

### 12.2 Ownership and topology

| Test | Expected result |
|---|---|
| ownership valid, same stable PID | ownership true; topology may validate after all checks |
| ownership valid, different PIDs | ownership true; topology false |
| PID mismatch | no Close/Quit and skipped-unsafe lifecycle |
| activation start time changes | topology false |
| activation executable changes | topology false |
| application start time changes | topology false |
| application executable changes | topology false |
| sentinel reread changes/fails | topology false |
| invocation reread changes/fails | topology false |
| missing ownership persistence confirmation | cleanup impossible |
| missing topology persistence confirmation | cleanup impossible |

### 12.3 Persistence

| Test | Expected result |
|---|---|
| ownership artifact write/readback/hash succeeds | confirmation created after target record |
| topology artifact write/readback/hash succeeds | confirmation created after target record |
| any artifact already exists | fail closed; no overwrite |
| any write, flush, readback, parse, correlation, or hash failure | fail closed; no cleanup |
| later cleanup/residual recording | pre-cleanup artifact bytes unchanged |
| failure result | serializes with existing result/failure shape |
| HWND at platform boundaries | numeric JSON round-trip retained |

### 12.4 Lifecycle

Generate one test for every allowed edge in section 8 and one rejection test
for every unlisted edge from each state. Explicit tests must prove:

- `CleanupTopologyRejected -> ExcelCleanupStarted` is invalid;
- no equivalent path through `ExcelOperationStarted` exists;
- `CleanupTopologyValidated` requires both persistence events; and
- success requires cleanup actions and residual check.

### 12.5 Residuals

Test all six cases A-F in section 9, plus independent `CheckFailed` for each
identity and PID-reuse detection. Same-PID tests observe the process once;
different-PID tests observe twice and do not copy one result to the other.

### 12.6 Regression and isolation

- numeric HWND serialization;
- failed-result serialization;
- activation/application dual identities;
- no hard termination or process killing;
- Experiment has no Build.Host reference, `VBProject`, import, `SaveAs`,
  metadata, publication, or production request types;
- no new Experiment friend access;
- Build.Host CLI/result serialization unchanged;
- Build.Host successful transaction ordering unchanged; and
- Schema remains exactly 4 for Experiment evidence.

## 13. Implementation order

1. Add fakeable ROT-enumeration abstraction and path-matching unit tests.
2. Add `RotSentinelAttachment` and replace the binding path.
3. Add `CleanupTopologyValidation` and pre-cleanup re-observation tests.
4. Make cleanup eligibility depend on both durable confirmations.
5. Add create-new evidence DTOs, filenames, persistence/readback validation,
   and controller validation.
6. Replace lifecycle graph and mechanically test all transitions.
7. Update Experiment orchestration, then Build.Host evidence calls without
   changing transaction order or serialized contracts.
8. Add residual matrix and regression/static-isolation tests.
9. Perform separately authorized focused, full, build, format, and diff
   verification. No real Excel execution is implied.

## 14. Static governance conclusion

- ROT lookup non-activating by design: `YES`.
- `BindToMoniker` removed from the future ownership path: `YES`.
- ownership and cleanup topology separate: `YES`.
- PID equality cleanup-only: `YES`.
- PID mismatch can reach Close/Quit: `NO`.
- pre-cleanup evidence append-only: `YES`.
- hard termination introduced: `NO`.
- security bypass introduced: `NO`.
- current implementation real-Experiment GO: `NO`.
- retry authorized: `NO`.
- new Experiment authorization: `NO`.
- Frozen change required: `NO`.
- unresolved design issues: `NONE`.

The design decision is:

```text
A. DESIGN COMPLETE - READY FOR IMPLEMENTATION
```

This means ready only for a separately authorized implementation packet. The
repository's current implementation remains `NO-GO / SAFE-STOP`.
