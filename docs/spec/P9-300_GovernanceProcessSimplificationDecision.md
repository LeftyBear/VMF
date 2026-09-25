# P9-300 Governance Process Simplification and Codex Delegation Decision

## 1. Record status and boundary

- Work item: `P9-300`
- Activity: governance process simplification and Codex delegation decision
- Work mode: `docs-only`
- Document status: `DRAFT / governance process simplification and Codex delegation decision / NOT YET ACCEPTED`
- Basis: `P9-299 ACCEPT / supplemental Git inspection execution record / COMPLETE WITH LINE-ENDING WARNING REVIEW ITEM`
- Workflow state: `SAFE-STOP / metadata-hold`
- Metadata phase: `INCOMPLETE`
- Technical execution: `NO-GO / SAFE-STOP`

This record responds to the process burden created by repeated Git inspection,
supplemental inspection, execution-record splitting, and manual command-result
copying.

It establishes a simplified governance rule and a Codex delegation rule for
future docs-only work under `SAFE-STOP / metadata-hold`.

It does not perform Git inspection, staging, commit, push, or technical
execution.

## 2. Current accepted basis

P9-299 has now been accepted.

Current state:

- P9-299 is `ACCEPT / supplemental Git inspection execution record`.
- P9-299 result is `COMPLETE WITH LINE-ENDING WARNING REVIEW ITEM`.
- The supplemental inspection was executed within the P9-298 authorization.
- Status-bearing files were inspected using repository-visible paths.
- Untracked governance records P9-285 through P9-298 were inspected.
- Inspected content remained limited to docs-only governance, synchronization,
  and Git inspection records.
- LF-to-CRLF warnings remain review items only and do not authorize
  line-ending remediation.
- No staging, commit, or push was performed.
- No technical execution was performed.
- `SAFE-STOP / metadata-hold` continues.
- Technical execution remains `NO-GO / SAFE-STOP`.

## 3. Process problem identified

The prior governance pattern created a new P9 record for each of the following:

- Git inspection authorization;
- Git inspection execution;
- supplemental inspection authorization;
- supplemental inspection execution;
- minor correction or re-review;
- manual command-output transfer from terminal to chat;
- review-result transcription back into documents.

This pattern is no longer proportionate for docs-only work under SAFE-STOP. It
creates excessive documentation overhead, increases operator fatigue, and
obstructs development flow.

## 4. Simplified governance rule

For future docs-only work under `SAFE-STOP / metadata-hold`, use the smallest
governance unit that preserves the active boundary.

For docs-only Git inspection, use one consolidated inspection record per
inspection cycle unless the boundary changes.

A consolidated inspection record may include:

1. authorization scope;
2. exact commands authorized;
3. executed command results;
4. output summaries;
5. review disposition;
6. non-authorizations;
7. preserved SAFE-STOP state;
8. any warning items that do not change the boundary.

Authorization, execution, supplemental review, and minor correction should not be
split into separate P9 records unless the boundary changes.

A separate new P9 record remains required when any of the following is proposed:

- boundary change;
- staging;
- commit;
- push;
- technical execution;
- use of a previously prohibited tool or operation;
- metadata completion;
- MD07 or MD17 resolution;
- alternative disposition acceptance;
- metadata-hold boundary change;
- U03 start;
- candidate selection;
- evidence acceptance;
- security disposition;
- technical GO;
- execution instruction;
- line-ending remediation;
- release, package, `dist`, or tag operation;
- external-service access or modification;
- full P9 closure.

## 5. Codex delegation rule

For future docs-only work under `SAFE-STOP / metadata-hold`, Codex may perform
docs-only file edits and read-only Git inspection within the active governance
scope.

Codex may perform:

- docs-only creation and revision of governance records;
- docs-only updates to status-bearing documents;
- read-only Git inspection;
- diff review;
- `git diff --check`;
- consolidation of authorization, execution, and review content into one record;
- correction of documentation-only review findings;
- preparation of review-ready text for owner judgment.

Codex must not perform:

- staging;
- commit;
- push;
- technical execution;
- PowerShell execution;
- parser execution;
- Excel operation;
- tests or builds;
- package, `dist`, release, or tag operations;
- external-service access or modification;
- flagged executable rerun;
- Avast setting change;
- metadata completion;
- MD07 or MD17 resolution;
- alternative disposition acceptance;
- metadata-hold boundary change;
- U03 start;
- candidate selection;
- evidence acceptance;
- security disposition;
- technical GO;
- execution instruction;
- line-ending remediation;
- full P9 closure.

Staging, commit, and push remain human-owner decisions and require separate
explicit authorization.

## 6. Human-owner decision boundary

The human owner retains authority for:

- `ACCEPT / NOT ACCEPTED` governance judgments;
- staging authorization;
- commit authorization;
- push-governance decision;
- metadata completion;
- MD07 or MD17 resolution;
- alternative disposition acceptance;
- metadata-hold boundary change;
- U03 start;
- candidate selection;
- evidence acceptance;
- security disposition;
- technical GO;
- execution instruction;
- line-ending remediation;
- full P9 closure.

Codex may prepare decision materials, but must not make these decisions
operative without explicit owner acceptance.

## 7. LF-to-CRLF warning handling

The LF-to-CRLF warnings identified during P9-297 and P9-299 remain review items
only.

They do not authorize:

- line-ending normalization;
- line-ending conversion;
- staging;
- commit;
- push;
- remediation edits;
- technical execution.

Any future line-ending remediation requires a separate explicit governance
decision.

## 8. Preserved state

The following state remains unchanged:

- workflow state remains `SAFE-STOP / metadata-hold`;
- metadata phase remains `INCOMPLETE`;
- `G-145-MD07` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- `G-145-MD17` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- U03 remains `NOT STARTED`;
- candidate selection remains `NOT AUTHORIZED`;
- technical execution remains `NO-GO / SAFE-STOP`;
- push remains `NOT PERFORMED / NOT AUTHORIZED`;
- quarantined PowerShell-derived output remains unusable.

## 9. Non-authorizations

This record does not authorize:

- staging;
- commit;
- push;
- technical execution;
- PowerShell execution;
- parser execution;
- Excel operation;
- tests or builds;
- package, `dist`, release, or tag operations;
- external-service access or modification;
- flagged executable rerun;
- Avast setting change;
- metadata completion;
- MD07 or MD17 resolution;
- alternative disposition acceptance;
- metadata-hold boundary change;
- U03 start;
- candidate selection;
- evidence acceptance;
- security disposition;
- technical GO;
- execution instruction;
- line-ending remediation;
- full P9 closure.

## 10. Draft disposition

Status: `DRAFT / NOT YET ACCEPTED`.

If accepted, acceptance establishes:

- the simplified governance rule for future docs-only work under
  `SAFE-STOP / metadata-hold`;
- one consolidated inspection record per docs-only Git inspection cycle unless
  the boundary changes;
- Codex delegation for docs-only file edits and read-only Git inspection within
  the active governance scope;
- continued human-owner control over staging, commit, push, technical
  execution, metadata completion, U03 start, candidate selection, and closure.

It does not authorize staging, commit, push, technical execution, metadata
completion, U03 start, candidate selection, line-ending remediation, or any
downstream gate.

## 11. Verification and non-actions

This draft is based only on the accepted P9-285 through P9-299 state and the
identified need to reduce governance overhead.

Quarantined PowerShell-derived output was not used.

No Git inspection, staging, commit, push, parser, PowerShell, Excel operation,
test, build, package, `dist`, release, tag, external service, technical
execution, flagged executable rerun, Avast setting change, or line-ending
remediation was performed by this draft.

## 12. Review disposition

Status: `ACCEPT / governance process simplification and Codex delegation decision`.

Accepted scope:

- P9-299 is accepted as `ACCEPT / supplemental Git inspection execution record / COMPLETE WITH LINE-ENDING WARNING REVIEW ITEM`;
- future docs-only work under `SAFE-STOP / metadata-hold` should use the smallest governance unit that preserves the active boundary;
- future docs-only Git inspection should use one consolidated inspection record per inspection cycle unless the boundary changes;
- authorization, execution, supplemental review, and minor correction should not be split into separate P9 records unless the boundary changes;
- Codex may perform docs-only file edits and read-only Git inspection within the active governance scope;
- Codex may prepare review-ready text for owner judgment;
- human-owner authority is retained for `ACCEPT / NOT ACCEPTED` judgments, staging, commit, push, metadata completion, MD07 or MD17 resolution, alternative disposition acceptance, metadata-hold boundary change, U03 start, candidate selection, evidence acceptance, security disposition, technical GO, execution instruction, line-ending remediation, and full P9 closure;
- LF-to-CRLF warnings remain review items only and do not authorize line-ending remediation;
- workflow state remains `SAFE-STOP / metadata-hold`;
- metadata phase remains `INCOMPLETE`;
- `G-145-MD07` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- `G-145-MD17` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- U03 remains `NOT STARTED`;
- candidate selection remains `NOT AUTHORIZED`;
- technical execution remains `NO-GO / SAFE-STOP`;
- push remains `NOT PERFORMED / NOT AUTHORIZED`;
- quarantined PowerShell-derived output remains unusable.

This acceptance does not authorize staging, commit, push, technical execution,
PowerShell execution, parser execution, Excel operation, tests, builds, package,
`dist`, release, tag operations, external-service access or modification,
flagged executable rerun, Avast setting change, metadata completion, MD07 or
MD17 resolution, alternative disposition acceptance, metadata-hold boundary
change, U03 start, candidate selection, evidence acceptance, security
disposition, technical GO, execution instruction, line-ending remediation, or
full P9 closure.
