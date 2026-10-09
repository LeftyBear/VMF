# Excel Ownership Experiment Architecture Governance Decision

## 1. Record status

- Route: `C`
- Work mode: `docs-only governance decision record`
- Status: `ADOPTED`
- Design review result: `COMPLETE`
- Design review decision: `C. PRODUCTION REFACTOR REQUIRED BEFORE EXPERIMENT PATH`
- Frozen specification change required: `NO`
- Frozen modified: `NONE`
- Production public invocation changed: `NO`
- Thin-PowerShell interpretation changed: `NO`
- Production artifact contract changed: `NO`
- Experiment and shared-library architectural boundary: `ADOPTED`
- Implementation authorization: `NOT GRANTED`
- Excel/COM execution authorization: `NOT GRANTED`
- `build.ps1` execution authorization: `NOT GRANTED`
- Avast disposition: `UNRESOLVED`

This record adopts the architecture established by the supplied authoritative
Non-Production Experiment Entry-Point Design Review result. The review was
performed outside the repository documentation set; its supplied result is the
authoritative evidence for this decision, and no separate repository review
document is required.

This record is a governance decision only. It does not authorize
implementation, verification execution, Excel/COM execution, production build
execution, security-control changes, release activity, or Git mutation.

## 2. Adopted architectural boundary

The adopted architecture consists of:

1. a separate permanent non-production executable named
   `Vmf.Build.ExcelOwnership.Experiment`;
2. a separate, narrowly scoped ownership-primitives library shared by the
   production `Build.Host` and `ExcelOwnership.Experiment`;
3. no reference from the experiment project to the `Build.Host` executable
   project;
4. extraction of ownership-only functionality from the current
   `ExcelSession`; and
5. ownership startup that does not access `book.VBProject`.

The shared ownership-primitives library exists only to avoid duplicating the
ownership algorithm while maintaining a strict dependency boundary. It must
not expand into a shared production-build surface.

Production-only functionality remains owned by `Build.Host`, including:

- `VBProject` access;
- worksheet preparation;
- repository and source inventory;
- VBA import and mutation;
- format-55 `SaveAs`;
- metadata update; and
- artifact publication.

Production behavior, production invocation, and the adopted thin-PowerShell
production architecture remain unchanged.

## 3. Structural isolation requirements

The experiment dependency graph must be structurally incapable of reaching or
exposing:

- `Transaction.Execute`;
- repository or source inventory;
- VBA import or mutation;
- `VBProject` or `VBComponents`;
- `SaveAs` or format 55;
- metadata update;
- artifact publication;
- production build request types; or
- `build.ps1`.

The experiment must not accept production output, repository, source, artifact,
release, or other production-build inputs. Structural isolation is required so
that the diagnostic binary cannot invoke production import, mutation, save,
metadata, transaction, or publication behavior.

## 4. Experiment interface and inputs

The adopted interface is:

```text
dotnet Vmf.Build.ExcelOwnership.Experiment.dll run
  --invocation-id <non-empty-guid>
  --temporary-root <absolute-isolated-empty-directory>
```

The only accepted inputs are:

- a non-empty invocation ID represented as a GUID; and
- an absolute, isolated, initially empty temporary root.

No repository, source, production output, artifact, release, publication, or
production build request argument is accepted.

## 5. Experiment capability boundary

The experiment validates only:

1. sentinel creation;
2. installed `EXCEL.EXE` resolution;
3. dedicated `/x` launch;
4. exact process identity;
5. sentinel-specific attachment;
6. `Application.Hwnd` to PID agreement;
7. lifecycle and ownership evidence;
8. normal cleanup; and
9. residual-process state.

Ownership must not be inferred from `ActiveObject`, newest-process selection,
before-and-after process-list comparison, PID alone, or another global Excel
selection mechanism.

Existing unrelated Excel instances must remain untouched. Broad
`EXCEL.EXE` termination is prohibited.

## 6. Timeout and cleanup architecture

The adopted experiment timeout architecture uses a non-PowerShell
controller/worker arrangement with:

1. a 120-second overall deadline;
2. an invocation-correlated cooperative cleanup request after deadline;
3. a 30-second cleanup grace; and
4. `SAFE-STOP` if cleanup remains unsuccessful after the grace period.

The initial experiment must not contain or exercise hard Excel termination.
Deadline expiration, cleanup failure, or residual-process observation does not
authorize hard termination.

## 7. Diagnostic status and independent gates

The experiment entry point is retained permanently as a diagnostic capability.
It remains:

- non-production;
- non-publishing;
- isolated from production artifact capabilities; and
- independently gated for any real Excel/COM execution.

Production extraction requires characterization, unit, and static verification
showing that existing production behavior is preserved. This requirement does
not authorize that implementation or verification.

Architecture implementation and controlled real Excel/COM execution are
separate future gates. Neither is authorized by this record.

## 8. Security status

The experiment structurally avoids the `build.ps1` Avast path, but this does
not establish antivirus safety or resolve the existing detection.

- Avast disposition: `UNRESOLVED`
- Avast exception or exclusion: `NOT AUTHORIZED`
- Avast disablement or bypass: `NOT AUTHORIZED`
- Retry or workaround after a future antivirus detection or block:
  `NOT AUTHORIZED`

Any future antivirus detection or block requires immediate `SAFE-STOP` without
retry, bypass, exception, exclusion, or disablement.

## 9. Known residual risk

The known concurrent partial-record JSONL risk remains
`PRESENT / FAIL-CLOSED / NOT RESOLVED`.

It is accepted only as a fail-closed residual observation. This decision does
not declare the risk fixed and does not authorize implementation intended to
change it.

## 10. Compatibility and explicit non-effects

This decision requires no Frozen change and modifies no Frozen specification.
It does not change:

- the production public invocation;
- the thin-PowerShell governance interpretation;
- the production artifact contract;
- production behavior;
- a public API;
- a persisted schema;
- a canonical format; or
- an error-code contract.

This record does not authorize:

- architecture implementation or production refactoring;
- source or test changes;
- restore, build, test, or verification execution;
- `build.ps1` or `Build.Host` execution;
- Excel startup or COM access;
- XLAM creation or modification;
- production source import, VBA mutation, `SaveAs`, metadata, or publication;
- network, certificate, or external-service operations;
- Avast or other security-control changes;
- release, package, or `dist` operations; or
- Git staging, commit, push, or any other Git mutation.

## 11. Documentary verification and non-actions

Static documentary review confirms that this record reflects the supplied
authoritative review result, makes structural isolation explicit, leaves the
production invocation unchanged, claims no Frozen change, prohibits hard
termination in the initial experiment, preserves separate implementation and
Excel/COM execution gates, and leaves Avast unresolved.

In creating this record:

- only this governance record was created;
- no production source or test file was modified;
- no restore, build, or test was run;
- neither `build.ps1` nor `Build.Host` was executed;
- no Excel, COM, or XLAM operation occurred;
- no network, certificate, or external-service operation occurred;
- no Avast setting or other security control was changed;
- no Git mutation was performed; and
- `docs/development/U03_CAND-02_ConcreteCandidateDefinition.md` was not
  accessed.

## 12. Next independent gate

The next possible step is separately authorized implementation planning or
review. This record does not begin or authorize that step, implementation, or
controlled real Excel/COM execution.
