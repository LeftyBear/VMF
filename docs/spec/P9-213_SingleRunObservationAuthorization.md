# P9-213 Single-Run Observation Authorization

## 1. Authorization status and boundary

- Work item: `P9-213`
- Authorization targets: `P9-211 Completed Owner Input Submission` and `P9-212 Observation Authorization Review`
- Controlling preservation closeout: `docs/spec/P9-200_PreservationHoldCloseout.md`
- Work mode: `docs-only`
- Authorization result: `AUTHORIZED FOR SINGLE-RUN OBSERVATION ONLY / NO GIT / NO INDEX / NO LOCK / NO PERMISSION CHANGE`
- Observation authority: `ONE NON-GIT OBSERVATION RUN ONLY`
- Git recovery, cause determination, technical execution, and permission-change authority: `NOT GRANTED`
- Preservation state: `HOLD MAINTAINED / REVIEWABLE UNCOMMITTED WORKING-TREE DOCUMENTATION`
- Next step after observation: `P9-214 Non-Git Observation Result Record`

This record authorizes exactly one non-Git observation run within the all-and-only scope fixed by P9-211 and accepted by P9-212. It does not perform the observation and does not authorize Git recovery, cause determination, technical execution, remediation, or any permission, ACL, or ownership inspection or change.

## 2. Authorization decision

The controlling result is:

`AUTHORIZED FOR SINGLE-RUN OBSERVATION ONLY / NO GIT / NO INDEX / NO LOCK / NO PERMISSION CHANGE`

This authority is single-use. It is valid only while every condition below remains satisfied and only during the stated validity period. It authorizes no retry, repeat, substitute, additional target, additional field, or expanded observation.

## 3. Authorized scope

- observation candidate: `NG-02` only;
- observation target path: `C:\Users\biz\Documents\Project\VMF`;
- executor: `Hirokazu Sakuma`;
- validity period: from `2026-09-21T13:44:00+09:00` through `2026-09-22T13:44:00+09:00`;
- means: Windows Explorer only;
- operation: right-click the authorized target path and open **Properties**;
- inspection location: **General** tab only;
- screenshot creation: `作成しない`;
- output destination: `C:\Users\biz\Documents\Project\VMF\docs\spec\P9-214_NonGitObservationResultRecord.md`; and
- observation-result record location: `C:\Users\biz\Documents\Project\VMF\docs\spec\P9-214_NonGitObservationResultRecord.md`.

No other target, executor, time window, means, operation, screen, tab, artifact, destination, or record location is authorized.

## 4. Authorized output fields

Only the following fields may be recorded:

- observation item ID;
- observation target path label;
- existence;
- item type;
- read-only attribute;
- last-modified timestamp;
- size, only if visible on the General tab;
- observer;
- observation timestamp; and
- notes.

No additional field, query, calculation, comparison, derived conclusion, or cause determination is authorized.

## 5. Explicit prohibitions

- every Git command;
- every Git-state check;
- inquiry of `.git/index`;
- creation, deletion, or inquiry of `.git/index.lock`;
- stage, index, or lock operations;
- build, test, script, or runner execution;
- Avast access or operation;
- permission, ACL, or ownership inspection or change;
- inspection of any security-details tab;
- use of Command Prompt, PowerShell, Git Bash, or the VS Code Git view;
- commit or push;
- addition of another target path;
- repeat observation of the same item; and
- cause determination inferred from anything beyond the recorded observation result.

This authorization also grants no diagnosis, remediation, Git recovery, retry, workaround, release, publication, external transmission, or other technical execution.

## 6. Fail-closed conditions

Do not perform the observation and retain `SAFE-STOP` if:

- the validity period has expired;
- the executor does not exactly match P9-211;
- the target path does not exactly match P9-211;
- a means other than Windows Explorer is required;
- inspection outside the General tab is required;
- any Git, index, lock, permission, ACL, ownership, or Avast check is required;
- any unexpected warning, confirmation request, or error appears;
- the output destination or observation-result record location must change;
- screenshot creation is required; or
- the observation scope must expand.

On any fail-closed condition, do not correct, supplement, retry, work around, substitute, observe, or infer a cause. This authority is not reusable outside its exact scope or validity period.

## 7. Preservation hold

The P9-200 preservation hold continues unchanged. This authorization does not release, modify, narrow, or create an exception to that hold.

Preserve every existing P9-160 through P9-212 record, this P9-213 record, and the P9 synchronization content in `docs/VMF_vNext_Backlog.md`, `docs/development/CURRENT_STATUS.md`, and `docs/development/HANDOFF.md` as reviewable uncommitted working-tree documentation. This custody description is not a Git scope, verified snapshot, staging set, commit candidate, or operation authority.

Historically absent P9-179 and P9-185 remain absent. P9-174 remains `HOLD / NOT YET COMMIT-READY`; no verified snapshot exists; and technical execution remains `NO-GO / SAFE-STOP` apart from the single non-Git observation expressly authorized here.

## 8. Post-observation requirement and recorded non-actions

If the authorized observation is performed, the next step is `P9-214 Non-Git Observation Result Record`. P9-214 may record only the authorized observation result. It must not determine a cause, perform Git recovery, conduct another observation, or perform technical execution.

P9-213 itself performed no observation, Git command, Git-state check, index inquiry, index or lock operation, stage or restage, cached query or verification, build, test, script, runner, Excel, Avast or other security-product operation, permission/ACL/ownership inspection or change, diagnosis, remediation, Git recovery, cause determination, release, publication, transmission, external-service operation, commit, push, or technical execution.

Frozen specifications, public contracts, source, tests, tools, packages, `dist`, and workbooks were not changed.
