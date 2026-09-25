# P9-298 Supplemental Git Inspection Authorization

## 1. Record status and boundary

- Work item: `P9-298`
- Activity: supplemental Git inspection authorization
- Work mode: `docs-only`
- Document status: `DRAFT / supplemental Git inspection authorization / NOT YET ACCEPTED`
- Basis: `P9-297 ACCEPT / REVIEW REQUIRED`
- Workflow state: `SAFE-STOP / metadata-hold`
- Metadata phase: `INCOMPLETE`
- Technical execution: `NO-GO / SAFE-STOP`

This record authorizes, if accepted, a limited supplemental read-only Git
inspection to resolve the inspection gaps identified by P9-297.

It does not authorize staging, commit, push, technical execution, parser
execution, PowerShell execution, tests, builds, Excel operations, package,
`dist`, release, tag, external-service access, flagged executable rerun, or
Avast setting change.

## 2. Accepted basis carried forward

- P9-285 through P9-296 are accepted docs-only governance records.
- P9-297 is accepted as a Git inspection execution record with `REVIEW REQUIRED`.
- P9-285-PRE remains accepted as the procedural-deviation disposition.
- PowerShell-derived output quarantined by P9-285-PRE remains unusable.

P9-297 identified the following unresolved inspection gaps:

- governance records are untracked, so `git diff -- <path>` did not show their content;
- `P9-297_GitInspectionExecutionRecord.md` was not included in the P9-296 governance-record diff command;
- status-bearing files were not inspected because the command used logical names rather than repository paths;
- `git diff --check` produced LF-to-CRLF warnings requiring review.

## 3. Authorized supplemental inspection commands

If this record is accepted, only the following supplemental read-only commands
are authorized.

### Command 1

```text
git diff -- docs/development/CURRENT_STATUS.md docs/development/HANDOFF.md docs/VMF_vNext_Backlog.md
```

Permitted purpose:

- inspect status-bearing file changes using repository-visible paths.

### Command 2

```text
git diff --no-index -- NUL docs/spec/P9-285_ResumeRouteDeterminationUnderMetadataHold.md
```

Permitted purpose:

- inspect the untracked P9-285 governance record content.

### Command 3

```text
git diff --no-index -- NUL docs/spec/P9-286_MetadataHoldGovernanceOptionsRecord.md
```

Permitted purpose:

- inspect the untracked P9-286 governance record content.

### Command 4

```text
git diff --no-index -- NUL docs/spec/P9-287_MetadataHoldGovernanceOptionSelection.md
```

Permitted purpose:

- inspect the untracked P9-287 governance record content.

### Command 5

```text
git diff --no-index -- NUL docs/spec/P9-288_MetadataHoldContinuationControlRecord.md
```

Permitted purpose:

- inspect the untracked P9-288 governance record content.

### Command 6

```text
git diff --no-index -- NUL docs/spec/P9-289_DocsOnlyStatusSynchronizationPlanning.md
```

Permitted purpose:

- inspect the untracked P9-289 governance record content.

### Command 7

```text
git diff --no-index -- NUL docs/spec/P9-290_StatusSynchronizationScopeDefinition.md
```

Permitted purpose:

- inspect the untracked P9-290 governance record content.

### Command 8

```text
git diff --no-index -- NUL docs/spec/P9-291_DocsOnlyStatusSynchronizationExecutionAuthorization.md
```

Permitted purpose:

- inspect the untracked P9-291 governance record content.

### Command 9

```text
git diff --no-index -- NUL docs/spec/P9-292_DocsOnlyStatusSynchronizationEditPlan.md
```

Permitted purpose:

- inspect the untracked P9-292 governance record content.

### Command 10

```text
git diff --no-index -- NUL docs/spec/P9-293_DocsOnlyStatusSynchronizationExecutionRecord.md
```

Permitted purpose:

- inspect the untracked P9-293 governance record content.

### Command 11

```text
git diff --no-index -- NUL docs/spec/P9-294_PostSynchronizationStateReview.md
```

Permitted purpose:

- inspect the untracked P9-294 governance record content.

### Command 12

```text
git diff --no-index -- NUL docs/spec/P9-295_GitInspectionGovernanceDecision.md
```

Permitted purpose:

- inspect the untracked P9-295 governance record content.

### Command 13

```text
git diff --no-index -- NUL docs/spec/P9-296_CommandByCommandGitInspectionAuthorization.md
```

Permitted purpose:

- inspect the untracked P9-296 governance record content.

### Command 14

```text
git diff --no-index -- NUL docs/spec/P9-297_GitInspectionExecutionRecord.md
```

Permitted purpose:

- inspect the untracked P9-297 governance record content.

### Command 15

```text
git diff --no-index -- NUL docs/spec/P9-298_SupplementalGitInspectionAuthorization.md
```

Permitted purpose:

- inspect the untracked P9-298 supplemental Git inspection authorization record content.

No other command is authorized by this record.

## 4. Expected result handling

The supplemental outputs may be used only to determine whether:

- the three status-bearing files contain only the authorized synchronization edits;
- untracked governance records contain docs-only P9-285 through P9-298 records;
- no inspected content creates metadata completion, U03 start, candidate selection, technical execution, staging, commit, push, or downstream authority.

Any unexpected output, missing file, path mismatch, non-docs change, unclear
result, or apparent boundary expansion must preserve `SAFE-STOP / metadata-hold`
and require further review.

## 5. LF-to-CRLF warning handling

The LF-to-CRLF warnings reported by P9-297 do not by themselves authorize file
normalization, line-ending conversion, staging, commit, or push.

Any line-ending remediation, if later needed, requires a separate governance
decision.

## 6. Execution constraints

The authorized supplemental inspection must:

- be read-only;
- execute only the commands listed in section 3;
- avoid PowerShell;
- avoid parser, Excel, tests, builds, package, `dist`, release, tag, external services;
- avoid flagged executable rerun;
- avoid Avast setting changes;
- not stage files;
- not commit;
- not push;
- not create technical authority;
- not rely on quarantined PowerShell-derived output.

## 7. Preserved state

The following state remains unchanged:

- workflow state remains `SAFE-STOP / metadata-hold`;
- metadata phase remains `INCOMPLETE`;
- `G-145-MD07` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- `G-145-MD17` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- U03 remains `NOT STARTED`;
- candidate selection remains `NOT AUTHORIZED`;
- technical execution remains `NO-GO / SAFE-STOP`;
- push remains `NOT PERFORMED / NOT AUTHORIZED`.

## 8. Non-authorizations

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
- line-ending remediation;
- full P9 closure.

## 9. Draft disposition

Status: `DRAFT / NOT YET ACCEPTED`.

If accepted, acceptance authorizes only the supplemental read-only Git
inspection commands listed in section 3.

It does not authorize staging, commit, push, technical execution, metadata
completion, U03 start, candidate selection, line-ending remediation, or any
downstream gate.

## 10. Verification and non-actions

This draft is based only on accepted P9-285 through P9-297, the P9-284
carried-forward state, and the accepted P9-285-PRE isolation rule.

Quarantined PowerShell-derived output was not used.

No Git inspection, staging, commit, push, parser, PowerShell, Excel operation,
test, build, package, `dist`, release, tag, external service, technical
execution, flagged executable rerun, Avast setting change, or line-ending
remediation was performed by this draft.

## 11. Review disposition

Status: `ACCEPT / supplemental Git inspection authorization`.

Accepted scope:

- supplemental read-only Git inspection is authorized only for the commands listed in section 3;
- Command 1 may inspect the repository-visible status-bearing file paths;
- Commands 2 through 15 may inspect the untracked governance records P9-285 through P9-298 using `git diff --no-index -- NUL`;
- no other command is authorized;
- LF-to-CRLF warnings remain review items and do not authorize line-ending remediation;
- workflow state remains `SAFE-STOP / metadata-hold`;
- metadata phase remains `INCOMPLETE`;
- `G-145-MD07` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- `G-145-MD17` remains `OPEN / UNRESOLVED / SAFE-STOP`;
- U03 remains `NOT STARTED`;
- candidate selection remains `NOT AUTHORIZED`;
- technical execution remains `NO-GO / SAFE-STOP`;
- push remains `NOT PERFORMED / NOT AUTHORIZED`;
- quarantined PowerShell-derived output remains unusable.

This acceptance does not authorize staging, commit, push, MD07 or MD17
resolution, alternative disposition acceptance, metadata completion,
metadata-hold boundary change, U03 start, candidate selection, evidence
acceptance, security disposition, technical execution, technical GO, execution
instruction, parser or PowerShell execution, Excel operation, tests, builds,
package, `dist`, release, tag operations, external-service access or
modification, flagged executable rerun, Avast setting change, line-ending
remediation, or full P9 closure.
