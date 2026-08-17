# Publisher v1.0 Definition of Done

Status: PROPOSED
Scope: Publisher v1.0 completion criteria
Depends: `Publisher_v1.0_ScopeFreeze.md`

## Purpose

This record defines the conditions that must be satisfied before Publisher
v1.0 can be declared complete.

The completion decision is evaluated only against the frozen v1.0 scope.
Deferred vNext enhancements do not block v1.0 completion.

## 1. Functional Completion

Publisher v1.0 is functionally complete when the frozen in-scope behavior is
implemented and operates as intended.

Required capabilities include:

- Markdown parsing and Google Docs publication/format conversion.
- Diff planning and physical update execution.
- Verified State handling.
- Revision-conflict detection and safe stop.
- Managed-document readback and verification.
- Dry-run planning and its frozen reporting behavior.
- OAuth Desktop authentication.
- Safe diagnostics and stable exit-code behavior.

No new feature is required solely because it exists as a vNext candidate.

## 2. Safety Completion

Publisher v1.0 must preserve its defined safety boundaries.

Completion requires evidence that:

- conflicting or invalid update state does not silently proceed;
- revision conflicts result in the defined safe-stop behavior;
- physical updates are followed by the required readback and verification;
- Verified State is promoted or saved only under its defined success
  conditions;
- dry-run does not perform Google Docs / Drive mutation;
- diagnostic output preserves the defined redaction and safe-value boundaries;
- failure paths return the defined stable classifications and exit codes.

A known defect that violates these boundaries blocks v1.0 completion.

## 3. Operational Completion

Publisher v1.0 must be operable using the documented supported workflow.

Completion requires:

- OAuth Desktop setup and authentication procedure is documented and usable;
- token-store handling boundaries are documented;
- installation and normal-operation procedures are documented;
- dry-run and publication workflows are understandable to an operator;
- failure diagnostics provide sufficient bounded information for
  troubleshooting;
- recovery or escalation guidance exists for supported failure cases.

Operational convenience improvements outside the frozen scope do not block
completion.

## 4. Verification Completion

The final v1.0 candidate must pass the verification set approved for the v1.0
completion decision.

At minimum, the completion evidence must record the applicable results for:

- Release build;
- Publisher unit tests;
- non-live integration tests;
- Live E2E;
- required Google Docs / Drive readback and verification behavior exercised by
  the authorized Live E2E scope;
- formatting verification;
- documentation consistency checks;
- any additional focused regression tests required by defects corrected after
  the scope freeze.

All required checks must pass, or an explicit responsible-owner decision must
record why a check is non-applicable.

Historical verification evidence may support the completion review, but the
v1.0 completion decision must identify which evidence applies to the selected
v1.0 candidate.

## 5. Documentation and Evidence Completion

The completion state must be reproducible and reviewable from repository
records.

Required documentation must:

- identify the selected v1.0 candidate;
- identify the frozen v1.0 scope;
- record final verification results;
- distinguish current evidence from historical `0.0.1-dev` evidence;
- record known limitations and deferred vNext work;
- preserve applicable OAuth, Google, release, security, and credential
  boundaries;
- avoid unsupported claims of vendor clearance or safety certification.

Material contradictions between current-state, completion, verification, and
operator documentation must be resolved before completion.

## 6. Remaining-Work Rule

An unresolved item blocks Publisher v1.0 completion only when it:

1. belongs to the frozen v1.0 in-scope behavior;
2. represents a defect against that behavior;
3. prevents a required verification condition from passing; or
4. prevents accurate operation or completion evidence.

Items explicitly deferred by `Publisher_v1.0_ScopeFreeze.md` do not block
v1.0 completion.

In particular, further vNext enhancement work is not required merely to
exhaust the Publisher backlog.

## 7. Completion Decision

Publisher v1.0 may be declared complete when:

- all applicable conditions in this Definition of Done are satisfied;
- no unresolved v1.0-blocking defect remains;
- required final verification is PASS or explicitly recorded as
  non-applicable;
- documentation and evidence are consistent with the selected candidate; and
- the responsible owner records the final completion decision.

The completion decision does not by itself grant vendor clearance or safety
certification.

Release or publication operations remain subject to any separately applicable
authorization and release/security gates.

## Version Boundary

Publisher v1.0 is a completion target after the historical published
`0.0.1-dev` prerelease.

Evidence from `0.0.1-dev` may be reused where still applicable, but v1.0
completion must not be inferred solely from the historical prerelease status.

## Next Step

Evaluate the current Publisher baseline against each condition in this
Definition of Done and extract only the unmet v1.0-blocking conditions.
