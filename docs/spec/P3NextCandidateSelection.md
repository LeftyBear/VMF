# P3-08 - Next Candidate Selection

## Status

SELECTION / docs-only

## Purpose

Select one next VMF Build vNext candidate after completion of P3-01 through
P3-07 Validator Integration.

P3-08 is documentation only. It does not authorize or perform production VBA
code changes, test additions, refactoring, Parser changes, Validator changes,
Manifest changes, Template changes, GenerateContext changes, Generator changes,
package or `dist` updates, external service operations, release operations, Git
staging, commit, or push.

## Current State

The current VMF Build vNext flow state is:

```text
Blueprint
-> Parser
-> Validator
-> Manifest
-> Template
-> GenerateContext
-> Generator
```

P2 established Blueprint v0.1 scope, validation rules, error classification,
error codes, Validator implementation scope, and Candidate B Validator
implementation / closeout.

P3 established and completed Validator integration into the formal flow:

| Item | Current state | Evidence |
| --- | --- | --- |
| P3-01 | COMPLETE as Validator Integration Planning baseline | `docs/spec/ValidatorIntegrationPlanning.md` |
| P3-02 | COMPLETE as Validator Integration Implementation by subsequent completion review evidence | `docs/spec/ValidatorIntegrationCompletionReview.md` |
| P3-03 | COMPLETE as Validator Failure Contract / Diagnostics Planning baseline | `docs/spec/ValidatorFailureContractPlanning.md` |
| P3-04 | COMPLETE as failure-contract implementation need resolved by later caller-reporting implementation and completion review evidence | `docs/spec/ValidatorIntegrationCompletionReview.md` |
| P3-05 | COMPLETE as Validator Caller Reporting Evaluation | `docs/spec/ValidatorCallerReportingEvaluation.md` |
| P3-06 | COMPLETE as Validator Caller Reporting Minimal Implementation by subsequent completion review evidence | `docs/spec/ValidatorIntegrationCompletionReview.md` |
| P3-07 | COMPLETE as Validator Integration Completion Review | `docs/spec/ValidatorIntegrationCompletionReview.md` |

`docs/VMF_vNext_Backlog.md` currently records P3-07 as COMPLETE and records no
open P3 item after P3-07. `docs/development/CURRENT_STATUS.md` records VMF Build
P3-07 as COMPLETE with behavior PASS, Build PASS warnings 0 / errors 0,
existing Build regression 18 runners PASS, focused parser and Validator tests
PASS, `git diff --check` PASS, generated artifact cleanup, and no code-level
blocker.

The remaining forward movement is no longer Validator integration. The next
material boundary is how an approved, valid, generatable Blueprint should be
derived into Manifest data without broadening Template, GenerateContext, or
Generator behavior by assumption.

## Evaluation Criteria

Each candidate is evaluated against:

- contribution to the Blueprint -> Manifest -> Template -> GenerateContext ->
  Generator flow
- dependencies
- whether the work can proceed local-only
- implementation size
- regression risk
- existing test assets
- whether planning is required before implementation

## Candidate List

| Candidate | Flow contribution | Dependencies | Local-only | Implementation size | Regression risk | Existing test assets | Planning first |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Manifest derivation scope planning | Defines the next exact bridge from validated Blueprint to Manifest data | P2 Blueprint records, P2 Validator records, P3 integration completion | Yes | Docs-only | Low | Existing parser / Validator / Build regression evidence can inform later tests | Yes |
| Direct Manifest derivation implementation | Advances from validated Blueprint into Manifest production | Requires exact derivation contract, output shape, compatibility with existing Manifest generation | Yes, if later scoped | Medium to high | Medium to high because it touches generation flow | Existing parser tests, Validator tests, Build regression | Yes |
| Template mapping planning | Defines how derived Manifest data should feed Template behavior | Requires stable Manifest derivation contract first | Yes | Docs-only | Low | Existing Build regression only indirectly relevant | Yes, but after Manifest derivation planning |
| GenerateContext planning | Defines how generated Manifest / Template inputs become generation context | Requires Manifest derivation and Template mapping decisions first | Yes | Docs-only | Low | Existing Build regression only indirectly relevant | Yes, but later |
| Generator behavior planning or implementation | Moves final VBA output behavior forward | Requires upstream Manifest / Template / GenerateContext contracts | Local-only possible for planning; implementation is broader | High | High because Generator output is user-facing | Existing Build regression likely required | Yes, not now |
| AI Blueprint draft generation | Starts natural-language to Blueprint drafting | Requires product and authoring policy beyond current Validator integration | Potentially local-only only if fixture-bound | High | High; introduces new responsibility boundary | No direct current test assets identified in scoped records | Yes, later |
| Automatic Blueprint repair or normalization | Attempts to fix invalid Blueprint input | Conflicts with current Validator non-repair boundary unless redesigned | Potentially local-only | Medium to high | High; risks weakening validation semantics | No approved current test asset | Yes, later / likely NO-GO under current boundary |

## Selected Candidate

Selected candidate:

P4-01 - Manifest Derivation Scope Planning.

Decision:

GO for docs-only planning.

NO-GO for direct implementation in P3-08.

## Selection Rationale

Manifest derivation scope planning is the next best candidate because:

- P3-07 confirms the Parser -> Validator -> pre-Manifest hard-stop behavior is
  complete.
- The next flow step after a valid generatable Blueprint is Manifest derivation.
- Direct Manifest implementation is implementation-heavy and would affect the
  generation flow, so it needs a fixed scope before code changes.
- Planning can proceed local-only and docs-only.
- It preserves P3-08 boundaries by avoiding production code, tests, Template,
  GenerateContext, Generator, package, release, and external operations.
- Existing parser, Validator, and Build regression evidence can inform the
  later implementation test plan without rerunning tests in this selection task.

The selected next task should define the minimal Manifest derivation contract:

- source Blueprint fields allowed to feed Manifest derivation
- derived Manifest output responsibility
- prohibited derivations and assumptions
- compatibility with existing Manifest generation behavior
- focused test direction for a later implementation task
- safety-stop conditions before Template, GenerateContext, or Generator changes

## Rejected Or Deferred Candidates

Direct Manifest derivation implementation is deferred because it is
implementation-heavy and requires a planning record first.

Template mapping planning is deferred because Template behavior depends on a
stable Manifest derivation contract.

GenerateContext planning is deferred because GenerateContext inputs should not
be planned before Manifest derivation and Template boundaries are fixed.

Generator behavior planning or implementation is deferred because it is farther
downstream, has higher regression risk, and should not start before upstream
Manifest / Template / GenerateContext decisions are fixed.

AI Blueprint draft generation is deferred because it introduces a new upstream
authoring responsibility outside the current Validator integration sequence.

Automatic Blueprint repair or normalization is deferred because current P2 and
P3 boundaries preserve Validator non-repair semantics. Any repair behavior would
need a separate future candidate and must not be inferred from the current
vNext state.

## Next Task Name

P4-01 - Manifest Derivation Scope Planning

## GO / NO-GO

GO:

- create a future docs-only planning record for Manifest derivation scope
- keep the next task local-only
- define scope, dependencies, non-responsibilities, focused test direction, and
  implementation GO / NO-GO boundary

NO-GO:

- production VBA code changes
- test code additions
- Parser changes
- Validator changes
- Manifest implementation changes
- Template changes
- GenerateContext changes
- Generator changes
- refactoring
- package or `dist` operations
- external service operations
- release operations
- Git staging, commit, or push

## Scope Boundary

P3-08 only records next-candidate selection. It does not update the backlog or
current-status records, does not close or reopen any implementation item, and
does not authorize P4-01 execution by itself.

Tests and builds are intentionally not run for P3-08. The required verification
is limited to `git diff --check` and docs-only confirmation.
