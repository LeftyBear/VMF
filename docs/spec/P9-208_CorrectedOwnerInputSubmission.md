# P9-208 Corrected Owner Input Submission

## 1. Submission status and boundary

- Work item: `P9-208`
- Corrected predecessor submission: `P9-206 Non-Git Observation Authorization Intake`
- Controlling review: `P9-207 Non-Git Observation Authorization Review`
- Controlling proposal and preservation hold: `P9-205` and `P9-200`
- Work mode: `docs-only`
- Submission status: `CORRECTED OWNER INPUT SUBMITTED / REVIEW NOT YET PERFORMED / EXECUTION NOT AUTHORIZED / OBSERVATIONS NOT PERFORMED / NO-GO / SAFE-STOP`
- Completion state: `INCOMPLETE / SAFE-STOP` while any required owner-controlled placeholder remains unfilled
- Observation and technical execution authority: `NOT GRANTED`
- Git recovery decision and cause determination: `NOT MADE`
- Next step: `P9-209 Corrected Owner Input Review`

This record corrects the P9-206 candidate/content mismatch and states the fields required for a narrowly bounded non-Git Windows Explorer property observation. It is a corrected submission only. It does not authorize or perform observation, Git recovery, cause determination, remediation, or technical execution. P9-209 must review this submission, and observation remains subject to a separate explicit single-execution authorization after that review.

## 2. Corrected observation candidate selection

The P9-206 selection of `NG-01` is withdrawn. The corrected selection is:

| Candidate | Corrected selection |
| --- | --- |
| `NG-01` | `not selected` |
| `NG-02` | `selected` |
| `NG-03` | `not selected` |
| `NG-04` | `not selected` |
| `NG-05` | `not selected` |
| `NG-06` | `not selected` |
| `NG-07` | `not selected` |
| `NG-08` | `not selected` |

Reason: for the historical P9-179 Git index-lock creation permission error, observe only externally visible filesystem properties of the target path without using Git. The proposed fields are a strict subset of the P9-205 `NG-02` filesystem-metadata boundary. No broader `NG-02` field, cause inference, or candidate is implied.

## 3. Observation target path

Observation target path:

`<責任者が明示する単一の対象パス>`

The target must be one literal path supplied by the accountable owner. While this value is unfilled, the submission is `INCOMPLETE / SAFE-STOP`, and no observation may begin.

## 4. Specific Windows observation method

The only submitted method is:

1. use Windows Explorer;
2. right-click the identified target path;
3. open **Properties**; and
4. inspect the **General** tab only.

The following are prohibited:

- Command Prompt;
- PowerShell;
- Git Bash;
- Git GUI;
- the VS Code Git view;
- any script, runner, build, or test;
- direct inquiry of `.git/index` or `.git/index.lock`; and
- inspection or modification of ACLs, owners, or the Security tab or other security details.

If Windows Explorer and the General tab alone cannot supply an authorized field, record no substitute and retain `SAFE-STOP`.

## 5. Output fields

The planned output is limited to:

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

No additional field, screen, tab, query, calculation, comparison, or derived result is authorized or implied.

## 6. Executor

Single executor:

`<責任者が明示する単一実行者名>`

While the executor is unfilled, the submission is `INCOMPLETE / SAFE-STOP`. The executor must not be inferred, substituted, or designated by proxy.

## 7. Approval timestamp and validity period

Approval timestamp:

`<責任者承認日時>`

While the approval timestamp is unfilled, the submission is `INCOMPLETE / SAFE-STOP` and no validity window exists.

If a later review and separate explicit single-execution authorization accept the completed values, validity is limited to 24 hours from the accountable owner's approval timestamp. The authority expires after that period. Re-observation, additional observation, or scope expansion requires separate approval. A duration statement alone does not approve observation or start a validity window.

## 8. Output destination and handling

Output destination:

`<責任者が明示する保存先>`

Observation results may be transcribed only into P9-208 or a later P9 document. If a screenshot is created under a later authorization, it may be stored only at the exact destination stated above. Unspecified storage, copying, sharing, or external transmission is prohibited.

While the output destination is unfilled, the submission is `INCOMPLETE / SAFE-STOP`, and no observation or screenshot creation may begin.

## 9. Single-execution condition

Any later observation is limited to one execution against the approved target path, by the approved executor, through the approved method, within the approved validity period, and using the approved output destination.

Each of the following requires separate approval:

- re-observation of the same item;
- an additional target path;
- an additional output field;
- a method change;
- an executor change; or
- an output-destination change.

P9-208 does not supply that single-execution authorization.

## 10. Fail-closed conditions

Do not observe and retain `SAFE-STOP` if:

- the selected `NG` candidate and observation content do not match;
- the target path is not identified;
- a method other than Windows Explorer is required;
- a tab other than General must be inspected;
- the executor is not identified;
- the approval timestamp is not identified;
- the validity period cannot be confirmed;
- the output destination is not identified;
- a Git command or Git-state check is required;
- `.git/index` or `.git/index.lock` inquiry, creation, or deletion is required;
- a stage, index, or lock operation is required;
- a build, test, or runner is required;
- an Avast or other security-product operation is required;
- permission, ACL, or ownership inspection or modification is required; or
- an unexpected warning, confirmation request, or error appears.

On any fail-closed condition, perform no correction, workaround, substitute, confirmation, retry, additional observation, or cause inference.

## 11. Preservation hold and exclusions

The P9-200 preservation hold continues unchanged. This submission does not release, modify, narrow, or create an exception to that hold.

This submission does not authorize:

- observation execution;
- technical execution;
- Git recovery;
- cause determination;
- any Git command or Git-state check;
- build, test, script, or runner execution;
- Avast or other security-product operation;
- permission, ACL, or ownership change;
- stage, index, or lock operation; or
- commit or push.

Preserve every existing P9-160 through P9-207 record, this P9-208 record, and the three P9 synchronization documents as reviewable uncommitted working-tree documentation. This custody description is not a Git scope, verified snapshot, staging set, commit candidate, or operation authority. Historically absent P9-179 and P9-185 remain absent. P9-174 remains `HOLD / NOT YET COMMIT-READY`; no verified snapshot exists; and technical execution remains `NO-GO / SAFE-STOP`.

## 12. Next step and recorded non-actions

The next step is `P9-209 Corrected Owner Input Review`. P9-209 must determine whether the completed corrected submission is sufficient to support an observation authorization decision. A P9-209 review result is not observation authority; observation is possible only under a later separate explicit single-execution authorization.

Recorded result: `CORRECTED OWNER INPUT SUBMITTED / REVIEW NOT YET PERFORMED / EXECUTION NOT AUTHORIZED / OBSERVATIONS NOT PERFORMED / NO-GO / SAFE-STOP`.

Because the target path, executor, approval timestamp, and output destination remain unfilled, the current submission is also `INCOMPLETE / SAFE-STOP`.

No observation, Git command, Git-state check, index inquiry, stage/index/lock operation, build, test, script, runner, Avast or other security-product operation, permission/ACL/ownership inspection or change, diagnosis, remediation, Git recovery, cause determination, release, publication, transmission, external-service operation, commit, push, or technical execution occurred. Frozen specifications, public contracts, source, tests, tools, packages, `dist`, and workbooks were unchanged.
