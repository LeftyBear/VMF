# P9-175 Limited Git Staging Readiness Plan

## 1. Status and boundary

- Work item: `P9-175`
- Activity: docs-only limited Git staging readiness planning after P9-174
- Planning result: `COMPLETE / PLAN ONLY / STAGING NOT AUTHORIZED`
- Commit-readiness state: `HOLD / NOT YET COMMIT-READY`
- Stage state: `NOT STAGED`
- Commit state: `NOT COMMITTED / NOT AUTHORIZED`
- Push state: `NOT PUSHED / NOT AUTHORIZED`
- Technical execution state: `NO-GO / SAFE-STOP`

P9-175 defines the exact scope, checks, and fail-closed boundaries for a possible later limited staging workflow. It does not perform or authorize `git add`, staging, commit, push, publication, transmission, release, or technical execution. P9-174 remains controlling for the current `HOLD / NOT YET COMMIT-READY` disposition until a separately authorized workflow stages and verifies the proposed snapshot exactly as specified below.

## 2. Proposed limited staging target

The proposed target is exactly these 19 documentation files and no others:

1. `docs/spec/P9-160_TransmissionInputReviewRecord.md`
2. `docs/spec/P9-161_TransmissionExecutionEligibilityGapClosurePlan.md`
3. `docs/spec/P9-162_OwnerTransmissionPackageCompletionRequest.md`
4. `docs/spec/P9-163_OwnerTransmissionPackageResubmissionDraft.md`
5. `docs/spec/P9-164_OwnerRequiredFieldsPopulationAssistance.md`
6. `docs/spec/P9-165_OwnerFieldPopulationDecisionRequest.md`
7. `docs/spec/P9-166_OwnerDecisionSubmissionDraft.md`
8. `docs/spec/P9-167_OwnerDecisionPopulationAssistance.md`
9. `docs/spec/P9-168_OwnerDecisionIntakeForm.md`
10. `docs/spec/P9-169_OwnerDecisionIntakeReview.md`
11. `docs/spec/P9-170_OwnerDecisionCompletionPackageDraft.md`
12. `docs/spec/P9-171_OwnerDecisionCompletionPackagePopulationAssistance.md`
13. `docs/spec/P9-172_OwnerSafeCompletionGuidanceCloseoutReview.md`
14. `docs/spec/P9-173_AccumulatedDocsOnlyChangeReview.md`
15. `docs/spec/P9-174_CommitReadinessReview.md`
16. `docs/spec/P9-175_LimitedGitStagingReadinessPlan.md`
17. `docs/VMF_vNext_Backlog.md`
18. `docs/development/CURRENT_STATUS.md`
19. `docs/development/HANDOFF.md`

The list is an indivisible proposed snapshot. A missing target, an extra path, a renamed path, a path with unexpected content, or a later edit after verification invalidates readiness and returns the workflow to `HOLD / NOT YET COMMIT-READY`.

## 3. Explicit exclusions

Every path not listed in section 2 is excluded. In particular, the proposed snapshot excludes:

- `AGENTS.md`, `VMF_CODEX_PLAYBOOK.md`, all Frozen specifications, and all other files under `specs/`;
- source, tests, tools, templates, candidates, assets, applications, packages, and `dist`;
- workbooks, generated artifacts, release records, secrets, credentials, local configuration, logs, and temporary files;
- any unrelated tracked modification, untracked file, user change, or later-created P9 record;
- Git metadata, branches, tags, and external-service state.

No broad or implicit staging form is permitted. Repository-wide staging, directory-wide staging, glob-based staging, `git add .`, `git add -A`, and any equivalent operation that can collect an unlisted path are prohibited for this plan.

## 4. Conditions before limited staging

A later operator may stage the section 2 paths only after receiving explicit authorization that names P9-175 and authorizes **limited staging only**. Before staging, the operator must:

1. confirm the repository root and current branch;
2. capture `git status --short` and reconcile every entry against the 19-path target and section 3 exclusions;
3. confirm all 19 target files exist at the exact paths and that no intended target has been renamed or replaced;
4. review the unstaged content of the three tracked governance files and directly review every untracked P9 file;
5. stop if any target content changed after the review basis was established, if any user change cannot be separated safely, or if the exact target set is uncertain; and
6. stage only the 19 literal paths by an explicit named-path operation.

Limited staging authorization would authorize only creation of a reviewable cached snapshot. It would not authorize commit, push, amendment, branch or tag creation, publication, release, transmission, or technical execution.

## 5. Mandatory cached names and content verification

Immediately after the limited stage, and before any commit request, the operator must verify the actual cached snapshot:

1. obtain the cached name/status list with `git diff --cached --name-status`;
2. compare the cached names exactly against the 19 paths in section 2, with no missing, additional, renamed, copied, deleted, or unexpected-status entry;
3. inspect the complete cached content with `git diff --cached --` followed by the 19 literal target paths;
4. confirm that cached content contains only the reviewed P9-160 through P9-175 records and their three governance synchronizations;
5. run `git diff --cached --check` and require exit code `0` with no whitespace-error output; and
6. rerun `git status --short` to identify any unstaged changes, untracked files, or scope drift that could invalidate the proposed commit decision.

The cached names check, cached content review, and cached diff check apply to the same unchanged index state. Any subsequent staging or index mutation invalidates all three checks and requires them to be repeated from the beginning.

## 6. Fail-closed conditions

The workflow must stop without commit authorization and remain `HOLD / NOT YET COMMIT-READY` if any of the following occurs:

- any cached path is outside the 19-path target;
- any target path is absent, renamed, copied, deleted, or has an unexpected Git status;
- cached and reviewed content differ, content is incomplete, or review cannot establish the exact snapshot;
- source, test, Frozen specification, API, schema, package, `dist`, workbook, release, secret, local configuration, generated artifact, external-service, or unrelated user content is present;
- `git diff --cached --check` is nonzero or reports any whitespace error;
- the index changes after cached verification;
- working-tree changes create ambiguity about what would be committed;
- limited staging authority is absent, expired, ambiguous, or broader Git authority is merely inferred; or
- separate explicit commit authorization has not been issued for the verified cached snapshot.

If an out-of-scope path is found in the index, no commit may proceed. Removal or correction of cached content is a separate Git mutation and requires applicable explicit authorization; this plan does not authorize an unstage, reset, restore, cleanup, or destructive operation. After any authorized correction, the entire cached verification sequence must be repeated.

## 7. Independent authorization boundaries

The authorization sequence is strictly separated:

1. **Plan acceptance:** accepts this document only; it authorizes no Git mutation.
2. **Limited staging authorization:** may authorize staging only the 19 literal paths; it does not authorize commit.
3. **Cached snapshot verification:** establishes evidence about one exact index state; it is not commit authorization.
4. **Commit authorization:** must be separately explicit and bound to the verified cached names and content. It does not authorize push.
5. **Push authorization:** must be separately explicit after a commit exists and must identify the intended repository, branch, and commit. Commit permission, a successful commit, or prior push permission does not imply current push permission.

No later boundary may be inferred from an earlier one. Failure or absence at any boundary retains the applicable `NOT AUTHORIZED` state and requires safe stop.

## 8. Preserved governance state

P9-175 changes no documentary or technical disposition from P9-174. P9-170 remains `UNPOPULATED / OWNER COMPLETION REQUIRED / NOT SENT`; owner resubmission remains `NOT SUBMITTED`; package acceptance remains `NOT ASSESSED`; submission remains `NOT AUTHORIZED / NOT SUBMITTED`; transmission remains `WITHHOLD / SAFE-STOP`; the P9-156 notice remains `NOT SENT`; the execution instruction remains `NOT ISSUED`; the 105-versus-102 discrepancy remains unresolved; Avast remains unresolved; P9-94 remains non-reusable; P9-130/P9-135 remain non-rerunnable; and technical execution remains `NO-GO / SAFE-STOP`.

No owner contact, population, delivery, submission, send action, external transmission, external-service access, build, test, project script or runner, Excel operation, Avast operation, release, publication, stage, commit, or push was performed by P9-175.
