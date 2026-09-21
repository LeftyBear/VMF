# P9-186 Corrected Limited Restaging Authorization Submission Draft

## 1. Draft status and boundary

- Work item: `P9-186`
- Activity: docs-only corrected authorization submission draft under P9-184 section 4
- Basis: task-provided P9-185 fail-closed result and P9-184 section 4
- Draft created: `2026-09-21T11:25:54+09:00`
- Draft result: `COMPLETE / CORRECTED DRAFT PREPARED / OWNER COMPLETION REQUIRED / NOT SUBMITTED`
- Scope phrase: `LIMITED RESTAGING ONLY`
- Restaging decision: `NOT AUTHORIZED`
- Index state: `0 STAGED PATHS`
- Cached verification state: `NOT PERFORMED`
- Commit state: `NOT COMMITTED / NOT AUTHORIZED`
- Push state: `NOT PUSHED / NOT AUTHORIZED`
- Technical execution state: `NO-GO / SAFE-STOP`

P9-186 corrects the owner-completable structure of P9-184 without supplying an owner decision. The P9-185 record is not present in the working tree; therefore this draft relies only on the explicit fail-closed correction requirements in the P9-186 task instruction and the mandatory fields and boundaries in P9-184 section 4. No unstated P9-185 finding is inferred.

This document is an unpopulated and unsubmitted draft. Its creation, review, owner completion, submission, receipt, or acceptance does not itself authorize `LIMITED RESTAGING ONLY`, cached verification, commit, or push. It performs no Git index mutation.

## 2. Exact 25 literal paths

The requested `LIMITED RESTAGING ONLY` scope is the following indivisible set of exactly 25 literal documentation paths:

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

The three governance paths are modified tracked files and the 22 P9 records are untracked at draft preparation. A qualifying owner approval must bind to the complete reviewed then-current content and Git status of each literal path. No subset, superset, substitution, inferred addition, rename, copy, deletion, replacement, directory, or glob qualifies.

## 3. Excluded scope

Every path not listed in section 2 is excluded. This expressly includes:

- `docs/spec/P9-183_FreshRestagingOwnerAuthorizationIntake.md`, `docs/spec/P9-184_FreshRestagingAuthorizationSubmissionDraft.md`, any P9-185 record, this P9-186 record, any `docs/spec/P9-179_*.md`, and every later-created record;
- `AGENTS.md`, `VMF_CODEX_PLAYBOOK.md`, Frozen specifications, and every file under `specs/`;
- all other documentation, source, tests, tools, templates, candidates, assets, applications, packages, `dist`, workbooks, generated artifacts, release records, secrets, credentials, local configuration, logs, and temporary files;
- unrelated tracked modifications, untracked files, user changes, Git metadata, branches, tags, and external-service state.

The historical 19-path scope is not inherited. P9-178 is consumed and cannot be reused, renewed, extended, corrected, or interpreted as authority for `LIMITED RESTAGING ONLY`. Broad, directory-wide, glob-based, or implicit staging, including `git add .`, `git add -A`, and equivalents, is prohibited. Correction, unstage, reset, restore, clean, retry, privilege elevation, ACL or ownership change, alternate credentials, Git configuration change, and `.git/index.lock` manipulation are outside scope.

## 4. Owner-completed `LIMITED RESTAGING ONLY` submission

Every bracketed field is mandatory owner-supplied content. Codex, an operator, reviewer, or intermediary must not infer, select, populate, sign, or approve any field by proxy.

### 4.1 Owner identity and authority

- Accountable owner legal/display name: `[OWNER REQUIRED]`
- Role/title: `[OWNER REQUIRED]`
- Organization or accountable group: `[OWNER REQUIRED]`
- Authority source: `[OWNER REQUIRED]`
- Authority scope and limits: `[OWNER REQUIRED]`
- Stable authorization/submission ID: `[OWNER REQUIRED]`
- Owner-controlled signature or equivalent attributable approval: `[OWNER REQUIRED]`

### 4.2 Exact owner decision

> I authorize `LIMITED RESTAGING ONLY` for all and only the 25 literal paths in P9-186 section 2, subject to every exclusion in section 3, the complete reviewed content and Git-status basis, the bounded validity window, the single-use limit, the cached verification conditions, and the fail-closed conditions in this document. I authorize `ONE EXACT-PATH RESTAGING ATTEMPT ONLY`. I understand that this approval does not reuse P9-178, does not authorize correction or retry, and does not authorize commit or push.

- Owner decision: `[APPROVE EXACT STATEMENT / DO NOT APPROVE — OWNER REQUIRED]`
- Owner qualification, limitation, or conflict declaration: `[OWNER REQUIRED; NONE must be stated explicitly if none]`

Any wording that replaces or varies `LIMITED RESTAGING ONLY`, or makes the decision partial, broader, conditional, path-incomplete, internally inconsistent, or ambiguous, is not a qualifying approval.

### 4.3 Decision and validity timestamps

- Owner decision timestamp with timezone: `[OWNER REQUIRED]`
- Authorization effective timestamp with timezone: `[OWNER REQUIRED]`
- Authorization expiry timestamp with timezone: `[OWNER REQUIRED]`
- Maximum use: `ONE EXACT-PATH RESTAGING ATTEMPT ONLY`

The attempt must begin and complete within the bounded window against the approved repository, `main` branch, exact paths, exclusions, complete content, and statuses. Authority is consumed when the first attempt begins, whether it succeeds or fails. Missing timezone, open-ended or reversed bounds, an expired or not-yet-effective window, or inability to establish current time is fail-closed.

## 5. Preconditions and cached verification conditions

Before any separately accepted `LIMITED RESTAGING ONLY` attempt, confirm the repository root and `main` branch; reconcile complete status; confirm all and only the 25 literal paths and expected statuses; review their complete current content; confirm all exclusions; confirm owner identity, role/title, organization, authority source and limits, owner-controlled approval, stable ID, and timestamp validity; and confirm that the approved basis has not changed.

Immediately after the one permitted attempt, all checks below must use the same unchanged index:

1. `git diff --cached --name-status` contains all and only the 25 section 2 paths with expected statuses and no missing, extra, renamed, copied, deleted, or unexpected entry.
2. Complete cached content for every target exactly matches the owner-approved content basis.
3. P9-183, P9-184, any P9-185 record, P9-186, and every other exclusion are absent from the index.
4. `git diff --cached --check` exits `0` with no whitespace-error output.
5. `git status --short` reconciles the cached snapshot and remaining working-tree state.
6. Index identity and content remain unchanged throughout verification and until any later commit-authorization decision.

Cached verification is evidence only for that unchanged index. It grants no correction, retry, commit, or push authority. Any later index mutation invalidates the evidence.

## 6. Fail-closed conditions

Retain `RESTAGING NOT AUTHORIZED / SAFE-STOP`, or stop an authorized attempt without correction or retry, when any required owner field is missing; attribution or authority is not established; approval is not owner-controlled; the decision is ambiguous, inferred, incomplete, outside authority, ineffective, expired, reused, or inconsistent; the scope phrase is not exactly `LIMITED RESTAGING ONLY`; the repository, branch, path set, exclusions, content/status basis, validity window, or one-use limit differs; any path or status drifts; an excluded path would enter the index; index-lock creation or an index update fails; an elevation, permission, lock, credential, configuration, or other workaround would be required; complete cached evidence cannot be established on one unchanged index; or the working tree or index changes after basis review, during verification, or after verification.

A failed attempt consumes the one-use authority. Failure, an empty index, or absence of contamination does not permit correction, retry, or another `LIMITED RESTAGING ONLY` attempt by inference. An invalid or unexpected index grants no corrective Git authority.

## 7. Independent authorization boundaries

1. **P9-186 draft:** unpopulated and unsubmitted; grants no Git authority.
2. **Owner completion and submission:** supplies a decision for later intake; completion or submission alone does not establish usable authority.
3. **`LIMITED RESTAGING ONLY` acceptance:** requires a separate completeness, attribution, authority, exact-scope, basis, and validity determination before one attempt.
4. **Cached verification:** establishes evidence for one unchanged index only and does not authorize commit.
5. **Commit:** requires separate explicit authorization bound to the exact verified unchanged index and does not authorize push.
6. **Push:** requires another separate explicit authorization bound to the repository, branch, and resulting commit.

No boundary implies a later permission. P9-174 remains `HOLD / NOT YET COMMIT-READY`.

## 8. Preserved state and non-actions

P9-186 performed no owner attribution, owner approval, submission, `LIMITED RESTAGING ONLY` attempt, stage, restage, retry, cached verification, index write or correction, permission elevation, ACL or ownership change, lock operation, unstage, commit, push, branch or tag operation, owner contact, external transmission, build, test, Excel or Avast operation, release, publication, or technical execution. All prior SAFE-STOP states remain effective.
