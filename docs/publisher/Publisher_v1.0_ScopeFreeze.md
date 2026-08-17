# Publisher v1.0 Scope Freeze

Status: FROZEN
Scope: Publisher v1.0 completion boundary

## Purpose

This record freezes the functional scope used to determine Publisher v1.0
completion.

The current implemented Publisher baseline is retained. Completion does not
require continued consumption of vNext enhancement candidates.

## In Scope — v1.0

- Markdown parsing and Google Docs publication/format conversion.
- Diff planning and physical update execution.
- Verified State handling and revision-conflict safe stop.
- Managed-document readback and verification.
- Dry-run planning and its currently implemented reporting contract.
- OAuth Desktop authentication and documented token-store handling.
- Safe diagnostics and stable exit-code behavior.
- Unit, integration, and Live E2E verification required by the v1.0
  completion gate.
- Installation, operation, failure-handling, and release-safety documentation
  required to operate Publisher.
- Publisher P2 functionality already implemented at the time of this freeze is
  retained as part of the baseline; further enhancement of those features is
  not required for v1.0 completion.

## Out of Scope — Deferred Beyond v1.0

- Adoption and production implementation of Google Picker / `drive.file`
  least-privilege routing.
- Further split-route implementation or rollout beyond the frozen baseline.
- Extension of local-only `preview-update` into Google Docs
  mutation/application behavior.
- Additional diagnostics, dry-run, readback, reporting, or support-output
  enhancements not required to correct a v1.0 defect.
- Additional release-note, CHANGELOG, evidence-generation, or documentation
  automation enhancements.
- Other convenience, automation, or feature-expansion work that is not
  necessary to satisfy the v1.0 Definition of Done.

## Freeze Rule

After this freeze, a change may enter the v1.0 completion path only when it is
one of the following:

1. A defect fix required for the frozen v1.0 behavior.
2. A change required to satisfy an agreed v1.0 Definition of Done condition.
3. A verification, evidence, or documentation correction required to prove
   v1.0 completion accurately.

New feature requests and nonessential enhancements are deferred to vNext or a
later version.

## Non-Effect

This scope freeze does not itself authorize Live E2E, Google Docs / Drive
mutation, OAuth or token-store operations, package or `dist` changes,
release/tag/publication, Avast operations, or flagged-executable execution.

Those operations remain subject to their applicable authorization and
verification gates.

This record does not claim Avast vendor clearance or Avast safety
certification.

## Version Boundary

Publisher v1.0 is a new completion target after the completed and published
`0.0.1-dev` prerelease.

This scope freeze does not modify, reopen, or reinterpret the historical
`0.0.1-dev` release record.

## Next Step

Define and approve the Publisher v1.0 Definition of Done against this frozen
scope.
