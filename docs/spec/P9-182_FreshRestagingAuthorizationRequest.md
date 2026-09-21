# P9-182 Fresh Restaging Authorization Request

## 1. Request status and boundary

- Work item: `P9-182`
- Activity: docs-only fresh restaging authorization request based on P9-181
- Repository: `C:\Users\biz\Documents\Project\VMF`
- Branch: `main`
- Request result: `COMPLETE / REQUEST PREPARED / FRESH OWNER AUTHORIZATION REQUIRED`
- Restaging decision: `NOT AUTHORIZED`
- Index state at preparation: `0 STAGED PATHS`
- Commit state: `NOT AUTHORIZED`
- Push state: `NOT AUTHORIZED`

P9-182 requests a new owner decision for `LIMITED RESTAGING ONLY`. It is not an authorization, does not reuse or extend the consumed P9-178 authority, and does not authorize or perform stage, restage, retry, index correction, permission elevation, ACL or ownership change, lock manipulation, commit, or push. P9-174 remains `HOLD / NOT YET COMMIT-READY` and SAFE-STOP remains effective.

## 2. Fresh all-and-only target

The requested target is the following indivisible set of exactly 25 literal documentation paths, selected from the current working-tree status and including this request and its three governance synchronizations:

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
17. `docs/spec/P9-176_LimitedStagingAuthorizationRequest.md`
18. `docs/spec/P9-177_LimitedStagingOwnerAuthorizationIntake.md`
19. `docs/spec/P9-178_LimitedStagingAuthorizationSubmissionDraft.md`
20. `docs/spec/P9-180_LimitedStagingAttemptFailureReview.md`
21. `docs/spec/P9-181_RestagingAuthorizationGapReview.md`
22. `docs/spec/P9-182_FreshRestagingAuthorizationRequest.md`
23. `docs/VMF_vNext_Backlog.md`
24. `docs/development/CURRENT_STATUS.md`
25. `docs/development/HANDOFF.md`

At preparation, the three governance paths are modified tracked files and the 22 P9 records are untracked files. A qualifying authorization must bind to the complete reviewed then-current content and Git status of all 25 paths. No subset, superset, substitution, inferred addition, rename, copy, deletion, or replacement is requested.

## 3. No automatic inheritance of the historical 19-path scope

The P9-175/P9-176/P9-178 scope of P9-160 through P9-175 plus the three governance files is a historical 19-path set only. P9-182 does not inherit that set automatically. It freshly enumerates the 25 paths in section 2 because the working-tree documentation set now also includes P9-176, P9-177, P9-178, P9-180, P9-181, and P9-182.

P9-179 is not a document path in the current working tree and is not included. The prior authority is `CONSUMED / NOT REUSABLE`; similarity between old and current paths, an empty index, or a prior owner decision cannot authorize the new set.

## 4. Explicit exclusions

Every path not listed in section 2 is excluded. This expressly includes:

- any `docs/spec/P9-179_*.md` path and every record created after P9-182;
- `AGENTS.md`, `VMF_CODEX_PLAYBOOK.md`, Frozen specifications, and every file under `specs/`;
- all other documentation, source, tests, tools, templates, candidates, assets, applications, packages, `dist`, workbooks, generated artifacts, release records, secrets, credentials, local configuration, logs, and temporary files;
- unrelated tracked modifications, untracked files, user changes, Git metadata, branches, tags, and external-service state.

Repository-wide, directory-wide, glob-based, or implicit staging is prohibited, including `git add .`, `git add -A`, and equivalents. An excluded or unexpected path must not be staged or corrected under this request.

## 5. Required fresh owner authorization

Before any restaging attempt, a new attributable owner authorization must supply all of the following:

- accountable owner identity, role, organization, authority source, authority scope and limits, and owner-controlled signature or equivalent approval;
- a stable authorization identifier and owner decision timestamp with timezone;
- bounded effective and expiry timestamps with timezone;
- the repository and branch stated in section 1;
- express approval of `LIMITED RESTAGING ONLY` for all and only the 25 literal paths in section 2, subject to section 4 exclusions;
- confirmation that the owner reviewed and approves the complete current content and Git-status basis; and
- maximum use of `ONE EXACT-PATH RESTAGING ATTEMPT ONLY`, consumed when the attempt begins whether or not the index update succeeds.

The authorization must be a fresh decision, not a correction, renewal, interpretation, reuse, or extension of P9-178. The request, its receipt, owner contact, draft completion, or generic approval language does not establish authority.

## 6. Privilege, ACL, and lock-operation prohibition

This request authorizes no elevated shell, privilege escalation, alternate credential, ACL or ownership change, Git configuration change, `.git/index.lock` deletion, creation, modification, takeover, or other lock manipulation or permission workaround. It also authorizes no unstage, reset, restore, clean, or index correction.

Any future qualifying restaging authority remains subject to the then-applicable execution and environment permissions. If the index cannot be updated without an elevation or environment change, the attempt must stop without retry. Any separately considered permission change requires its own explicit prior authority and cannot substitute for fresh restaging authorization.

## 7. Mandatory cached verification conditions

Immediately after one qualifying authorized exact-path restaging attempt, all checks below must be completed against the same unchanged index:

1. `git diff --cached --name-status` must contain all and only the 25 section 2 paths with the expected statuses and no missing, extra, renamed, copied, deleted, or unexpected entry.
2. The complete cached content of every target must be reviewed and must match the owner-approved content basis.
3. Every section 4 exclusion must be absent from the index.
4. `git diff --cached --check` must exit `0` with no whitespace-error output.
5. `git status --short` must be reconciled with the cached snapshot and the remaining working-tree state.
6. The index identity and content must remain unchanged throughout verification and until any later commit authorization decision.

Cached verification success is evidence for only that unchanged index. It does not authorize commit. Any index mutation invalidates the evidence and does not authorize correction or another attempt.

## 8. Fail-closed conditions

Restaging remains prohibited, or an authorized attempt must stop without retry, if any of the following applies:

- fresh owner authorization is absent, unattributable, ambiguous, incomplete, outside the owner's authority, not yet effective, expired, reused, or inconsistent;
- repository, branch, exact 25 paths, exclusions, content/status basis, timestamps, validity, or one-use limit is missing or differs from the reviewed basis;
- any target is missing, additional, renamed, copied, deleted, replaced, changed after review, or has an unexpected status;
- an unlisted or excluded path would enter the index;
- index-lock creation or any index update fails;
- privilege elevation, ACL or ownership change, lock manipulation, alternate credentials, Git configuration change, or another unapproved workaround would be required;
- cached names/statuses, complete cached content, exclusions, whitespace result, or reconciled status cannot be established on one unchanged index; or
- the index changes after restaging or during or after cached verification.

A failed attempt consumes the one-use authority under which it began. Failure, an empty index, or absence of contamination does not permit retry, correction, or restaging by inference.

## 9. Independent commit and push boundaries

Fresh restaging authorization, successful exact-path restaging, and complete cached verification do not authorize commit. Commit requires a separate explicit authorization bound to the exact verified unchanged index after all section 7 checks pass.

Commit authorization does not authorize push. Push requires another separate explicit authorization bound to the repository, branch, and resulting commit. Restaging, cached verification, commit, and push are independent sequential gates and cannot be collapsed by later or broader wording.

## 10. Preserved state and non-actions

P9-182 preserves every prior owner-completion, submission, transmission, security, continuation, release, and technical-execution disposition. It performed no stage or restage, retry, cached verification, index write or correction, permission elevation, ACL or ownership change, lock operation, unstage, commit, push, branch or tag operation, owner contact, external transmission, build, test, Excel or Avast operation, release, publication, or technical execution.
