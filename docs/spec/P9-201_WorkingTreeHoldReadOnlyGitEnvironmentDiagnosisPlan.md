# P9-201 Working-Tree Hold and Read-Only Git Environment Diagnosis Plan

## 1. Plan status and boundary

- Work item: `P9-201`
- Activity: docs-only planning for diagnosis of the P9-179 Git index-lock creation permission error
- Controlling predecessor: `docs/spec/P9-200_PreservationHoldCloseout.md`
- Historical failure record: `docs/spec/P9-180_LimitedStagingAttemptFailureReview.md`
- Work mode: `docs-only`
- Plan disposition: `COMPLETE / READ-ONLY DIAGNOSIS PLAN RECORDED / DIAGNOSIS NOT EXECUTED`
- Preservation state: `HOLD MAINTAINED / REVIEWABLE UNCOMMITTED WORKING-TREE DOCUMENTATION`
- Index and cached state: `NOT INSPECTED / NOT ESTABLISHED`
- Commit readiness: `HOLD / NOT YET COMMIT-READY`
- Technical execution state: `NO-GO / SAFE-STOP`

P9-201 records a future diagnostic boundary only. It does not execute diagnosis, determine root cause, reopen the restaging path, authorize a Git write or query, establish current index state, cure the historical failure, or promote any later gate. P9-200 remains the controlling preservation-hold closeout.

## 2. Preserved working-tree documentation

The custody set consists of every existing P9-160 through P9-200 record, this P9-201 plan, and the P9 synchronization content in `docs/VMF_vNext_Backlog.md`, `docs/development/CURRENT_STATUS.md`, and `docs/development/HANDOFF.md`.

This is a preservation description, not a literal Git scope, cached snapshot, commit candidate, or authority for any Git operation. The historically absent P9-179 and P9-185 records remain absent and must not be created, reconstructed, inferred, or normalized. Untracked coverage must not be inferred from an ordinary diff, and no preserved file may be deleted, overwritten, narrowed, regenerated, moved, or repaired under this plan.

## 3. Historical observation and evidentiary limit

P9-180 records that the P9-179 one-use staging attempt failed because Git could not create its index lock due to a permission error. The attempt was consumed, nothing was staged, and no retry was authorized. P9-201 accepts only that historical observation; it does not independently reproduce the error, inspect the lock or index, or confirm that the same environment condition still exists.

An index-lock creation error identifies the boundary at which the operation failed, not its unique cause. Without a separately authorized, non-mutating diagnostic run, each cause below remains a hypothesis. Even after read-only observations, effective create, rename, replace, and delete rights cannot be proven without a write-producing probe, which remains prohibited.

## 4. Read-only cause candidates

The following candidates may be considered without selecting among them by inference:

1. the effective user or token lacked create, write, delete, or rename rights required in the Git metadata directory;
2. an explicit deny, inherited access-control entry, ownership mismatch, integrity level, or other Windows access-control condition prevented lock creation;
3. security software, ransomware protection, controlled-folder protection, endpoint policy, or another filter driver denied the operation;
4. a pre-existing or concurrently held lock, another Git process, an editor, or another tool made the lock path unavailable;
5. the repository or Git metadata directory was read-only, offline, redirected, synchronized, virtualized, or otherwise subject to filesystem/provider restrictions;
6. disk, quota, path-resolution, filesystem-health, or transient operating-system conditions surfaced as an access or permission failure;
7. a Git environment, worktree, or metadata-path configuration redirected the attempted write to a location with different permissions; or
8. the historical error was transient and is no longer observable without an impermissible reproduction attempt.

The list is non-exhaustive and does not establish probability, causation, current presence, or a remediation recommendation. A read-only observation may eliminate a candidate only when the observation directly and unambiguously contradicts it; otherwise the candidate remains `UNRESOLVED`.

## 5. Permitted scope for a separately authorized diagnostic run

A future diagnostic run requires a separate explicit instruction identifying its exact read-only checks. Within that later authority, the maximum permissible scope is:

- confirm the repository root and current branch through non-cached, non-staged, non-index-derived read-only Git information;
- record the current process identity, group membership, elevation state, and relevant environment-path values without changing them;
- read filesystem existence, resolved paths, attributes, owner, and access-control metadata for the repository and Git metadata directory without opening, parsing, refreshing, or modifying the index;
- inspect already-existing, directly relevant operating-system or security-product denial records only when separately authorized and available read-only, without changing security settings or acknowledgements;
- inventory already-running processes by name and identity without terminating, suspending, attaching to, or changing them;
- record Git executable identity and version, and read configuration origins or metadata-path resolution only when the exact command has first been classified as neither cached, staged, index-derived, refreshing, locking, nor write-producing; and
- compare observations with the literal historical P9-180 error text while keeping historical and current observations separate.

The diagnostic command list must be written and reviewed before execution. Mixed commands must be split at the permitted/prohibited boundary. If a command's read-only behavior is uncertain, it is prohibited. No result may be treated as staged-state, cached-state, commit-readiness, or authorization evidence.

## 6. Prohibited actions and inquiries

P9-201 does not permit:

- stage, restage, retry staging, unstage, commit, push, merge, rebase, reset, restore, clean, stash, amend, branch, tag, release, publication, or transmission;
- cached verification, cached query, staged query, index-derived query, index inspection, index refresh, index repair, or any command that may acquire an index lock;
- reading, creating, deleting, renaming, moving, replacing, editing, taking ownership of, or otherwise manipulating `.git/index.lock`;
- writing a probe file or directory, using a permission test that creates or changes an object, or reproducing the failed staging operation;
- changing ACLs, ownership, inheritance, attributes, integrity level, Git configuration, environment variables, credentials, security-product configuration, controlled-folder settings, filesystem/provider settings, or repository location;
- elevation, alternate credentials, impersonation, compatibility modes, safe-directory workarounds, alternate index files, environment overrides, or any other workaround;
- terminating or interfering with another process; or
- using P9-178, the historical 19-path or 25-path sets, the preservation set, or a diagnostic result as current Git authority.

The prohibition on cached, staged, and index-derived inquiries includes purportedly read-only confirmation. The P9-198 cached name/status inquiry must not be repeated, reconstructed, or independently confirmed.

## 7. Fail-closed conditions

Retain `NO-GO / SAFE-STOP` without corrective Git action if:

- separate authority for the exact diagnostic checks is absent, ambiguous, expired, broader than read-only observation, or mixed with a prohibited action;
- any proposed command may refresh, lock, parse, derive state from, or mutate the index, or its behavior cannot be classified confidently before execution;
- preserved P9-160 through P9-201 content is missing, conflicted, unexpectedly changed, narrowed, or not fully accounted for, including untracked coverage uncertainty;
- the repository, branch, identity, resolved paths, permissions, owner, attributes, process state, security records, or environment differ unexpectedly from the approved diagnostic basis;
- observation requires elevation, alternate credentials, ownership or ACL change, lock manipulation, configuration change, a write probe, process interference, or another workaround;
- results are incomplete, inconsistent, indirect, stale, inaccessible, or insufficient to distinguish observation from inference;
- a cached, staged, index-derived, locking, or write-producing command is run inadvertently; or
- any diagnosis is presented as root cause, remediation approval, restaging authority, cached evidence, commit readiness, or technical-execution permission without its separate gate.

Any deviation must be disclosed, its output rejected for state promotion, and the hold maintained.

## 8. Post-diagnosis choices

After a separately authorized read-only diagnosis, the accountable owner may choose one of these independent paths:

1. **Maintain the hold:** preserve the documentation unchanged and take no further Git or environment action.
2. **Request additional read-only evidence:** authorize a new, exact, bounded list of non-mutating observations for unresolved candidates. The prior diagnostic authority does not carry forward.
3. **Authorize environment remediation separately:** define the exact responsible operator, target, change, rollback, and verification boundary outside P9-201. Diagnosis does not authorize remediation, and remediation does not authorize staging or retry.
4. **Reconsider the Git path separately:** only after the P9-200 minimum conditions and every still-applicable owner-control requirement are satisfied may a new current proposal be reviewed. Historical authority and diagnostic observations are not reusable staging authority.
5. **Close diagnosis as inconclusive:** retain the preservation hold and record that read-only evidence cannot prove effective write capability or a unique cause.

Every choice preserves the independence of environment remediation, owner reopening, intake acceptance, restaging authorization, the one exact-path attempt, cached verification, commit authorization, and push authorization.

## 9. Non-actions

P9-201 performed no diagnostic command beyond ordinary non-cached working-tree orientation used to prepare this docs-only plan; no cause was confirmed. It performed no stage, restage, retry, unstage, cached verification, cached query, staged query, index-derived query, index inspection, index mutation or correction, lock inspection or manipulation, write-permission probe, permission or configuration change, elevation, process interference, commit, push, merge, rebase, tag, branch operation, owner contact, build, test, project script or runner, Excel or Avast operation, release, publication, transmission, external-service operation, or technical execution. Frozen specifications, public contracts, source, tests, tools, packages, `dist`, and workbooks were not changed.
