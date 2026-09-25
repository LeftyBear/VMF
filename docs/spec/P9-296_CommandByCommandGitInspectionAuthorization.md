# P9-296 Command-by-Command Git Inspection Authorization

## 1. Record status and boundary

- Work item: `P9-296`
- Activity: command-by-command Git inspection authorization
- Work mode: `docs-only`
- Document status: `DRAFT / docs-only Git inspection command authorization / NOT YET ACCEPTED`
- Basis: `P9-295 ACCEPT`
- Workflow state: `SAFE-STOP / metadata-hold`
- Metadata phase: `INCOMPLETE`
- Technical execution: `NO-GO / SAFE-STOP`

This record authorizes, if accepted, a limited read-only Git inspection using
only the explicitly listed commands in this record.

It does not authorize staging, commit, push, technical execution, parser
execution, PowerShell execution, tests, builds, Excel operations, package,
`dist`, release, tag, external-service access, flagged executable rerun, or
Avast setting change.

## 2. Accepted basis carried forward

- P9-285 is accepted as the resume-route determination.
- P9-286 is accepted as the metadata-hold governance options record.
- P9-287 is accepted as the metadata-hold governance option selection.
- P9-288 is accepted as the metadata-hold continuation control record.
- P9-289 is accepted as the status synchronization planning record.
- P9-290 is accepted as the status synchronization scope definition.
- P9-291 is accepted as the docs-only status synchronization execution authorization.
- P9-292 is accepted as the docs-only status synchronization edit plan.
- P9-293 is accepted as the docs-only status synchronization execution record.
- P9-294 is accepted as the post-synchronization state review.
- P9-295 is accepted as the Git inspection governance decision.
- P9-285-PRE remains accepted as the procedural-deviation disposition.
- PowerShell-derived output quarantined by P9-285-PRE remains unusable.

## 3. Authorized inspection commands

If this record is accepted, only the following commands are authorized.

### Command 1

```text
git status --short
```

Permitted purpose:

- confirm the repository-visible changed-file set.

### Command 2

```text
git diff -- docs/spec/P9-285_ResumeRouteDeterminationUnderMetadataHold.md docs/spec/P9-286_MetadataHoldGovernanceOptionsRecord.md docs/spec/P9-287_MetadataHoldGovernanceOptionSelection.md docs/spec/P9-288_MetadataHoldContinuationControlRecord.md docs/spec/P9-289_DocsOnlyStatusSynchronizationPlanning.md docs/spec/P9-290_StatusSynchronizationScopeDefinition.md docs/spec/P9-291_DocsOnlyStatusSynchronizationExecutionAuthorization.md docs/spec/P9-292_DocsOnlyStatusSynchronizationEditPlan.md docs/spec/P9-293_DocsOnlyStatusSynchronizationExecutionRecord.md docs/spec/P9-294_PostSynchronizationStateReview.md docs/spec/P9-295_GitInspectionGovernanceDecision.md docs/spec/P9-296_CommandByCommandGitInspectionAuthorization.md
```

Permitted purpose:

- inspect docs-only governance record changes from P9-285 through P9-296.

### Command 3

```text
git diff -- CURRENT_STATUS HANDOFF Backlog
```

Permitted purpose:

- inspect status synchronization changes to the three authorized status-bearing files.

### Command 4

```text
git diff --check
```

Permitted purpose:

- check whitespace and conflict-marker issues in the current diff.

No other command is authorized by this record.

## 4. Execution constraints

The authorized commands must be executed only as read-only Git inspection.

The inspection must not:

- use PowerShell;
- use parser execution;
- use Excel operations;
- run tests or builds;
- perform package, `dist`, release, or tag operations;
- access or modify external services;
- rerun a flagged executable;
- change Avast settings;
- stage files;
- commit;
- push;
- create technical authority;
- rely on quarantined PowerShell-derived output.

## 5. Expected result handling

The command outputs may be used only to determine whether the docs-only
synchronization and governance files are repository-visible and whether the diff
contains whitespace or conflict-marker issues.

Unexpected output, command failure, missing file paths, extra changed files,
conflicting changes, or unclear results must preserve `SAFE-STOP /
metadata-hold`.

## 6. Preserved state

The following state remains unchanged:

- workflow state remains `SAFE-STOP / metadata-hold`;
- metadata phase remains `INCOMPLETE`;
- `G-145-MD07` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- `G-145-MD17` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- U03 remains `NOT STARTED`;
- candidate selection remains `NOT AUTHORIZED`;
- technical execution remains `NO-GO / SAFE-STOP`;
- push remains `NOT PERFORMED / NOT AUTHORIZED`.

## 7. Non-authorizations

This record does not authorize:

- any command not listed in section 3;
- staging;
- commit;
- push;
- MD07 or MD17 resolution;
- alternative disposition acceptance;
- metadata completion;
- metadata-hold boundary change;
- U03 start;
- candidate selection;
- evidence acceptance;
- security disposition;
- technical execution;
- technical GO;
- execution instruction;
- parser or PowerShell execution;
- Excel operation, tests, or builds;
- package, `dist`, release, or tag operations;
- external-service access or modification;
- flagged executable rerun;
- Avast setting change;
- full P9 closure.

## 8. Draft disposition

Status: `DRAFT / NOT YET ACCEPTED`.

If accepted, acceptance authorizes only the four read-only Git inspection
commands listed in section 3.

It does not authorize staging, commit, push, technical execution, metadata
completion, U03 start, candidate selection, or any downstream gate.

## 9. Verification and non-actions

This draft is based only on accepted P9-285 through P9-295, the P9-284
carried-forward state, and the accepted P9-285-PRE isolation rule.

Quarantined PowerShell-derived output was not used.

No Git inspection, staging, commit, push, parser, PowerShell, Excel operation,
test, build, package, `dist`, release, tag, external service, technical
execution, flagged executable rerun, or Avast setting change was performed.

## 10. Review disposition

Status: `ACCEPT / docs-only command-by-command Git inspection authorization`.

Accepted scope:

- only the four read-only Git inspection commands listed in section 3 are authorized;
- no other command is authorized by this record;
- inspection output may be used only to confirm the repository-visible changed-file set, inspect docs-only governance and synchronization diffs, and check whitespace or conflict-marker issues;
- workflow state remains `SAFE-STOP / metadata-hold`;
- metadata phase remains `INCOMPLETE`;
- `G-145-MD07` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- `G-145-MD17` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- U03 remains `NOT STARTED`;
- candidate selection remains `NOT AUTHORIZED`;
- technical execution remains `NO-GO / SAFE-STOP`;
- push remains `NOT PERFORMED / NOT AUTHORIZED`;
- quarantined PowerShell-derived output remains unusable.

This acceptance does not authorize any command outside section 3, staging,
commit, push, MD07 or MD17 resolution, alternative disposition acceptance,
metadata completion, metadata-hold boundary change, U03 start, candidate
selection, evidence acceptance, security disposition, technical execution,
technical GO, execution instruction, parser or PowerShell execution, Excel
operation, tests, builds, package, `dist`, release, tag operations,
external-service access or modification, flagged executable rerun, Avast setting
change, or full P9 closure.
