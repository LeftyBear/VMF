# P9-258 R1 Stage Plan Disposition and Recreation Decision

## 1. Status and scope

- Work item: `P9-258`
- Basis: `P9-257a` and `P9-257`
- Work mode: `docs-only / stage-plan disposition decision`
- Final status: `COMPLETE / DISCARD AND RECREATE`
- Existing R1 plan draft: `DISCARDED AS A PLANNING BASIS`
- Selected next work item: `P9-259 R1 Limited Stage Plan Recreation`
- R1 stage authorization review: `DO NOT PROCEED`
- R1 actual stage: `NOT AUTHORIZED / NOT PERFORMED`
- Mainline continuation: `PAUSE / None`
- Technical execution: `NO-GO / SAFE-STOP`

This record decides only whether the R1 plan draft recorded by P9-257 should
be corrected in place or recreated under a fresh compliant procedure. It does
not validate that draft, inspect repository state, authorize a later review or
operation, or perform technical execution.

## 2. Controlling premises

P9-257a is `COMPLETE / docs-only procedural deviation recorded`. P9-257
remains `INCOMPLETE / SAFE-STOP`, and its R1 plan draft remains affected by
the recorded procedural deviation. All output from the prohibited
PowerShell-hosted occurrence remains isolated and unusable as evidence,
verification, `PASS`, repository-state proof, artifact verification,
approval, authorization, scope basis, or later-gate input.

The existing draft is not validated. R1 stage authorization review remains
ineligible, R1 actual stage remains unapproved and unperformed, mainline
remains paused, and technical execution remains fail-closed.

## 3. Alternatives considered

The two permitted alternatives were:

1. continue correction of P9-257 while retaining the existing R1 plan draft;
   or
2. discard the existing draft as a planning basis and recreate the R1 plan
   through a fresh compliant docs-only procedure.

Correction in place would retain a draft produced within the procedurally
noncompliant P9-257 run and would require a new rehabilitation judgment before
the draft could be relied upon. Recreation provides the clearer provenance:
the affected draft is not reused as planning authority, and a new record can
be prepared without relying on the prohibited output.

## 4. Decision

Decision: `DISCARD AND RECREATE`.

The current R1 plan draft in
`docs/spec/P9-257_R1LimitedStagePlanCreation.md` is discarded as a planning
basis. The file remains retained as historical evidence of the incomplete
P9-257 work and its recorded non-actions; this decision does not delete,
rewrite, validate, correct, adopt, or authorize its plan content.

P9-257 correction-in-place is not selected. The R1 limited stage plan must be
recreated in `P9-259` under a fresh, separately instructed, compliant
docs-only procedure. P9-259 is selected as the single next candidate but is
not started or authorized by this record.

## 5. P9-259 boundary

Any later P9-259 instruction must recreate the R1 limited stage plan without
using the P9-257 draft or prohibited PowerShell-host output as evidence,
verification, `PASS`, repository-state proof, artifact verification,
approval, authorization, or planning authority. P9-259 must preserve all
applicable safety and authorization separations and must not infer stage,
cached-verification, commit, push, or technical-execution authority.

The previously recorded proposed R1 scope is historical only until P9-259
recreates and independently records a compliant plan. P9-258 does not
validate that scope and performs no content, artifact, or repository-state
verification.

## 6. Continuing safety state

Do not proceed to R1 stage authorization review. R1 actual stage remains
`NOT AUTHORIZED / NOT PERFORMED`. Commit readiness remains `NOT ASSESSED /
NOT COMMIT-READY`. Mainline remains `PAUSE / None`. Technical execution
remains `NO-GO / SAFE-STOP`.

P9-258 authorizes no Git command or mutation, stage, cached verification,
commit, push, PowerShell, script, build, test, artifact verification,
technical execution, external operation, or repository-state inspection.

## 7. Final result and recorded non-actions

The final result is:

`COMPLETE / DISCARD CURRENT R1 PLAN DRAFT AS A PLANNING BASIS / RECREATE IN P9-259`

P9-258 performed no Git command, repository-state inspection, stage, cached
verification, commit, push, Git mutation, PowerShell, script, build, test,
artifact verification, technical execution, or external operation. It changed
only this decision record and the requested `CURRENT_STATUS`, `HANDOFF`, and
backlog synchronization content.
