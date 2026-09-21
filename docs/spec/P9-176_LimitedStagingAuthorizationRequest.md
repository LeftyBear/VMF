# P9-176 Limited Staging Authorization Request

## 1. Request status and boundary

- Work item: `P9-176`
- Activity: docs-only limited staging authorization request based on P9-175
- Request result: `COMPLETE / REQUEST PREPARED / AUTHORIZATION NOT GRANTED`
- Requested authority: `LIMITED STAGING ONLY`
- Stage state: `NOT STAGED / NOT AUTHORIZED`
- Cached verification state: `NOT PERFORMED`
- Commit state: `NOT COMMITTED / NOT AUTHORIZED`
- Push state: `NOT PUSHED / NOT AUTHORIZED`
- Technical execution state: `NO-GO / SAFE-STOP`

P9-176 requests a later explicit authorization to stage only the exact 19-path snapshot defined by P9-175. Creating or accepting this request does not grant that authorization and does not authorize any Git mutation. The request is limited to creation of a reviewable cached snapshot; it does not authorize commit, push, amendment, branch or tag creation, publication, release, transmission, external-service activity, or technical execution.

## 2. Exact requested staging target

The requested limited stage contains exactly these 19 paths and no others:

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

This is an indivisible requested path set. `docs/spec/P9-176_LimitedStagingAuthorizationRequest.md` is not part of the P9-175 19-path target and must not be inferred into that target. Any decision to stage P9-176 itself would require a new, separately explicit scope decision and authorization.

## 3. Excluded scope

Every path not listed in section 2 is excluded, including P9-176 itself and any later-created record. The request also excludes:

- `AGENTS.md`, `VMF_CODEX_PLAYBOOK.md`, Frozen specifications, and every other file under `specs/`;
- source, tests, tools, templates, candidates, assets, applications, packages, and `dist`;
- workbooks, generated artifacts, release records, secrets, credentials, local configuration, logs, and temporary files;
- unrelated tracked modifications, untracked files, user changes, Git metadata, branches, tags, and external-service state.

Repository-wide, directory-wide, glob-based, or implicit staging is prohibited. `git add .`, `git add -A`, and any equivalent operation capable of collecting an unlisted path are outside this request.

## 4. Conditions for granting and exercising authority

Authorization may be treated as granted only by a new explicit instruction that identifies P9-176, states `LIMITED STAGING ONLY`, and approves exactly the 19 literal paths in section 2. Before any stage, the authorized operator must:

1. confirm the repository root and current branch;
2. capture `git status --short` and reconcile every entry to the approved target and exclusions;
3. confirm that all 19 paths exist exactly and that none was renamed, replaced, or made ambiguous;
4. review the then-current content of every target, including the complete untracked P9 records and all three governance synchronizations;
5. confirm that user changes are separable and that no content changed after the review basis; and
6. stage only the 19 literal named paths through an exact-path operation.

An instruction that merely accepts this document, asks to continue, approves commit planning, or refers generally to Git work is insufficient. Authority that is absent, expired, ambiguous, path-incomplete, or broader only by inference must be treated as not granted.

## 5. Mandatory cached names and content checks

Immediately after an authorized limited stage, before any commit request or decision, the operator must:

1. obtain `git diff --cached --name-status`;
2. compare the cached names and statuses exactly with the 19-path set, permitting no missing, extra, renamed, copied, deleted, or unexpected-status entry;
3. inspect the complete cached content using `git diff --cached --` followed by all 19 literal paths;
4. confirm that the cached snapshot contains only the reviewed P9-160 through P9-175 records and the reviewed content of the three governance files;
5. run `git diff --cached --check` and require exit code `0` with no whitespace-error output; and
6. rerun `git status --short` and reconcile remaining working-tree and untracked state.

The cached names check, complete cached-content review, and cached diff check must refer to the same unchanged index. Any later index mutation invalidates all cached verification evidence and requires the full sequence to be repeated.

## 6. Fail-closed conditions

Stop without commit authorization and retain `HOLD / NOT YET COMMIT-READY` if:

- explicit P9-176 limited-staging authorization is absent or ambiguous;
- any target is missing, extra, renamed, copied, deleted, replaced, or has an unexpected status;
- any cached path or content is outside the exact target, including P9-176;
- cached content differs from the reviewed basis, is incomplete, or cannot be established exactly;
- source, test, Frozen specification, API, schema, package, `dist`, workbook, release, secret, local configuration, generated artifact, external-service, or unrelated user content enters the index;
- `git diff --cached --check` is nonzero or reports a whitespace error;
- working-tree state makes the intended snapshot ambiguous; or
- the index changes after verification or separate commit authorization is absent.

Finding an invalid cached entry does not authorize an unstage, reset, restore, cleanup, or other correction. Any correction is a separate Git mutation requiring applicable explicit authority, after which every cached check must restart.

## 7. Independent authorization boundaries

The boundaries remain independent and sequential:

1. **Request preparation or acceptance:** accepts P9-176 as a request only; no staging authority is granted.
2. **Limited staging authorization:** must be separately explicit and is limited to the 19 literal paths; it does not authorize commit.
3. **Cached snapshot verification:** records evidence for one unchanged index state; successful verification is not commit authorization.
4. **Commit authorization:** must be separately explicit and bound to the verified cached names and complete content; it does not authorize push.
5. **Push authorization:** must be separately explicit after a commit exists and identify the intended repository, branch, and commit.

No later permission may be inferred from an earlier boundary. P9-174 remains `HOLD / NOT YET COMMIT-READY` unless an authorized stage and all cached checks succeed and a separate commit authorization is later issued.

## 8. Preserved governance state

P9-176 changes no documentary, transmission, security, continuation, release, or technical-execution disposition. P9-170 remains `UNPOPULATED / OWNER COMPLETION REQUIRED / NOT SENT`; submission remains `NOT AUTHORIZED / NOT SUBMITTED`; transmission remains `WITHHOLD / SAFE-STOP`; the P9-156 notice remains `NOT SENT`; the execution instruction remains `NOT ISSUED`; the 105-versus-102 discrepancy and Avast detection remain unresolved; P9-94 remains non-reusable; P9-130/P9-135 remain non-rerunnable; and technical execution remains `NO-GO / SAFE-STOP`.

P9-176 performed no stage, cached verification, commit, push, unstage, reset, restore, branch or tag operation, owner contact, send action, external transmission, external-service access, build, test, project script or runner, Excel operation, Avast operation, release, publication, or technical execution.
