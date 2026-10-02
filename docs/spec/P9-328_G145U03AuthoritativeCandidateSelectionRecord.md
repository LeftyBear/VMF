# P9-328 G-145-U03 Authoritative Candidate-Selection Record

## 1. Record status and scope

- Work item: `P9-328`
- Activity: authoritative docs-only candidate-selection record for `G-145-U03`
- Route: `C`
- Decision date: `2026-09-29`
- Decision owner: `VMF Owner`
- Source submission: `U03-SUB-20260929-01`
- Record result: `ACCEPT / authoritative docs-only candidate selection`
- Technical execution: `NO-GO / SAFE-STOP`
- Broader workflow: `SAFE-STOP`

This record accepts and records only the VMF Owner's explicit selection of one
bounded technical candidate for `G-145-U03`. It does not complete the selected
candidate, establish that existing implementation is correct or safe, accept
evidence, establish control implementation or effectiveness, provide a
security disposition, authorize continuation or a command, make a technical
GO decision, issue an execution instruction, or clear any `SAFE-STOP`.

## 2. Owner decision and authority

On `2026-09-29`, the `VMF Owner`, acting as the Decision Owner for this exact
candidate-selection decision, explicitly selected `U03-SUB-20260929-01` as the
formal technical candidate for `G-145-U03`.

The selected candidate is:

> Blueprint-to-VBA-source-file generation slice

The authority basis is the Owner's explicit `Yes` decision selecting this exact
submission for this exact gap. The decision is limited to candidate selection.
It supplies no authority for implementation, verification execution, evidence
acceptance, security disposition, control reliance, command authorization,
technical GO, execution, release, or external action.

## 3. Exact candidate boundary

### Start

Blueprint content parse boundary.

### Included responsibilities

```text
Blueprint content
-> Build_BlueprintParser
-> BlueprintValidator
-> BlueprintManifestDeriver
-> Manifest content / ManifestItem
-> Template Derivation responsibility
-> AppGenerateContextBuilder.AppBuildGenerateContext
-> AppGeneratorService.AppGenerateFromContext
-> InfGenerator.InfGenerateManifestItem
-> Generator output
-> AppOutputWriteService.AppBuildOutputWritePlan
-> AppOutputWriteService.AppWriteGeneratedOutput
-> deterministic .bas / .cls source-file output
|| SELECTED CANDIDATE ENDS ||
```

### End

Deterministic `.bas` / `.cls` source-file output through
`AppOutputWriteService.AppWriteGeneratedOutput`.

### Explicit exclusions

- workbook or VBProject mutation
- `AppApplyGeneratedOutputToRealVBProject`
- `AppApplyGeneratedOutputToAuthorizedWorkbook`
- all other workbook or VBProject mutation
- PowerShell and flagged historical execution routes
- P9-94 reuse
- P9-130 or P9-135 rerun, reuse, reconstruction, approximation, renaming,
  wrapping, partial execution, equivalent mapping, or materially similar route
- Avast modification, exception, exclusion, workaround, or bypass
- real user data or external services
- package, `dist`, release, publication, or tag operations
- public-contract, persisted-schema, canonical-format, or Frozen-specification
  change

This candidate is not the entire canonical Blueprint-to-VBProject pipeline.
Candidate selection does not authorize work beyond the source-file endpoint.

## 4. Candidate completion condition

Candidate completion requires a specified approved Blueprint to produce
specification-conforming `.bas` / `.cls` VBA source files deterministically,
with the result verifiable by appropriate tests under later independent
authorization.

This completion condition defines the selected candidate outcome only. It does
not state that the candidate is complete and does not authorize implementation,
tests, builds, parser execution, Excel, PowerShell, or any technical operation.

## 5. Known NOT ESTABLISHED candidate work

The following remain explicit unresolved candidate work:

1. exact Template Derivation production component;
2. Manifest Derivation -> Template Derivation exact production connection;
3. Generator output -> `AppBuildOutputWritePlan` production connection.

These items are not represented as implemented, connected, correct, safe, or
complete. They may not be filled by inference from naming, current source, a
template, a test, or this selection record. They require later independently
authorized specification review, implementation planning, implementation, and
verification as applicable.

## 6. G-145-U03 documentary transition

Before this decision, `G-145-U03` was `OPEN / UNRESOLVED / SAFE-STOP` because
no exact technical candidate had been selected by an attributable authority.

P9-145 defines `G-145-U03` as the technical-candidate-selection gap and requires
an attributable owner decision identifying one exact candidate, its purpose,
scope, exclusions, dependencies, and decision authority. P9-268 identifies a
fresh attributable owner decision followed by a separate docs-only
candidate-selection review as the permitted transition route.

This record supplies and accepts that exact candidate-selection decision.
Therefore, `G-145-U03` transitions to:

`RESOLVED / docs-only candidate selection / ACCEPT`

This resolution is limited to the documentary absence of an exact selected
candidate. It is not candidate completion, technical resolution, security
clearance, evidence acceptance, readiness, GO, execution authorization, or
broader workflow resumption.

## 7. Independent gates and retained safety state

- Technical execution remains `NO-GO / SAFE-STOP`.
- The broader workflow remains `SAFE-STOP`.
- Avast and the candidate-specific security disposition remain unresolved.
- `G-145-U01`, `G-145-U02`, and `G-145-U04` through `G-145-U08` are unchanged.
- Evidence eligibility, submission, review, and acceptance remain separate.
- Control implementation, operation, exception handling, and effectiveness
  remain separate and unestablished.
- Command authorization remains absent.
- Technical GO / NO-GO remains a separate future decision.
- Any execution instruction remains later, separate, exact, and single-use.
- Metadata gaps and the metadata phase are unchanged.
- P9-94 remains non-reusable.
- P9-130 and P9-135 remain prohibited from rerun, reconstruction,
  approximation, equivalence, or materially similar routing.
- U03-CAND-02 remains `HOLD UNCHANGED / DRAFT / PROPOSED / NOT ACCEPTED`,
  untracked, and non-authoritative.

## 8. Non-actions

This record performs no source-code or test change, implementation, technical
validation, parser run, test, build, PowerShell operation, Excel or workbook
operation, VBProject mutation, Avast operation, evidence acceptance, control
assessment, command authorization, technical GO, technical execution,
external-service access, package / `dist` / release / publication / tag action,
or Git mutation.

## 9. Result and next boundary

`U03-SUB-20260929-01` is the formally selected technical candidate for
`G-145-U03`. `G-145-U03` is resolved only as the docs-only candidate-selection
gap. The selected candidate remains incomplete, its three `NOT ESTABLISHED`
items remain unresolved, and technical execution remains `NO-GO / SAFE-STOP`.

Any next step requires a separate explicit Route C decision identifying the
single downstream governance gate under review. This record supplies no
automatic authority for that step.
