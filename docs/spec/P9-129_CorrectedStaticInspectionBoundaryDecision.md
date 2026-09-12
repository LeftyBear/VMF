# P9-129 — Corrected Static Inspection Boundary Decision

## Status

- P9-129: **COMPLETE / docs-only corrected static inspection boundary decision**
- Decision: **GO**
- Corrected boundary source: **P9-126 exact command sequence and file boundary**
- P9-127 result: **SAFE-STOP / not accepted as conforming completion**
- Future execution: **authorized only through a separate P9-130 instruction**
- Execution in P9-129: **not performed**
- Technical execution outside any future separately authorized exact static inspection: **NO-GO / SAFE-STOP**
- Avast detection: **unresolved**
- P9-94 allowance: **not reusable**

## Decision

**GO**. The corrected future static inspection boundary below exactly matches
the P9-126-authorized command sequence, order, and named file boundary. If a
future boundary differs from P9-126, the decision is **NO-GO** unless a new
explicit boundary is defined before execution.

The previous P9-127 attempt remains `SAFE-STOP` and is not accepted as
conforming completion. P9-129 is a docs-only boundary decision and does not
execute or rerun P9-127 or any command.

## Exact corrected future execution boundary

The later separate step may run each of these read-only commands exactly once, in this order:

```text
git status --short
git branch --show-current
git rev-parse HEAD
rg -n "P9-123|P9-124|P9-125|P9-126|COMPLETE|GO|execution|NO-GO / SAFE-STOP|P9-94|Avast detection" docs/spec/P9-123_StaticCommandAllowlistDrafting.md docs/spec/P9-124_StaticCommandAllowlistReviewGONOGO.md docs/spec/P9-125_StepLevelStaticInspectionCandidateSelection.md docs/spec/P9-126_StepLevelStaticInspectionGONOGODecision.md docs/VMF_vNext_Backlog.md docs/development/CURRENT_STATUS.md docs/development/HANDOFF.md
```

The step may emit ordinary output only to the invoking console. It must not change a command, change the named file set, redirect or persist output, retry, substitute another command, correct any finding, or perform a follow-on operation.

## Execution and stop boundary

The GO decision does not permit execution in P9-129. Execution requires a
separate P9-130 instruction and must use the exact corrected boundary above.
No retry, substitution, correction, output persistence, or follow-on operation
is authorized unless separately authorized.

Any mismatch from the exact P9-126 boundary, failure, missing input, scope
change, conflicting governance state, or need for retry or correction requires
immediate `SAFE-STOP` and grants no remediation authority.

## Preserved state and prohibited scope

Technical execution remains `NO-GO / SAFE-STOP` except for any future,
separately authorized exact static inspection. Avast detection remains
unresolved, and the P9-94 allowance remains not reusable.

P9-129 does not execute the P9-126 sequence or rerun P9-127. Parser, scripts,
PowerShell scripts, Excel, workbook, process, tests, build, package, `dist`,
release, publication, tag, external services, flagged executables, and Avast
changes, workarounds, exceptions, exclusions, or bypasses remain prohibited
and unexecuted. No staging, commit, or push is performed.

## Next step

P9-130 may execute the exact corrected static inspection only if separately
instructed. Without that instruction, no candidate or command may run.
