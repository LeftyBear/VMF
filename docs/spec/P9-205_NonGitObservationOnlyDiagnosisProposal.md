# P9-205 Non-Git Observation-Only Diagnosis Proposal

## 1. Proposal status and boundary

- Work item: `P9-205`
- Activity: docs-only proposal for a non-Git, observation-only diagnosis of the historical Git index-lock creation permission error
- Controlling predecessor: `docs/spec/P9-204_ReadOnlyDiagnosisPathDeviationReassessment.md`
- Controlling preservation closeout: `docs/spec/P9-200_PreservationHoldCloseout.md`
- Historical failure record: `docs/spec/P9-180_LimitedStagingAttemptFailureReview.md`
- Work mode: `docs-only`
- Proposal disposition: `COMPLETE / PROPOSAL RECORDED / EXECUTION NOT AUTHORIZED / OBSERVATIONS NOT PERFORMED`
- Preservation state: `HOLD MAINTAINED / REVIEWABLE UNCOMMITTED WORKING-TREE DOCUMENTATION`
- Diagnosis state: `NOT EXECUTED`
- Index and cached state: `NOT INSPECTED / NOT ESTABLISHED`
- Commit readiness: `HOLD / NOT YET COMMIT-READY`
- Technical execution state: `NO-GO / SAFE-STOP`

P9-205 supplies the new docs-only proposal required by P9-204. It does not authorize or perform an observation, select a cause, remediate the environment, reopen the P9-201/P9-202 path, or authorize any Git gate. P9-200 remains the controlling preservation hold, and all P9-203 deviation outputs remain rejected.

## 2. Diagnostic question and evidentiary limit

The only proposed question is whether current, safely observable non-Git facts support, contradict, or leave unresolved one or more hypotheses concerning the historical P9-180 failure to create the Git index lock because of a permission error.

The historical error identifies a failed boundary, not a unique cause. Observation-only evidence cannot prove effective create, rename, replace, or delete capability. No observation may be represented as a reproduction, write-capability proof, current Git/index state, verified snapshot, remediation approval, stage/restage authority, commit readiness, or technical-execution authority.

## 3. Cause candidates

The closed candidate set for this proposal is:

1. the effective Windows identity or token lacked a required filesystem right at the Git metadata directory;
2. an explicit or inherited access-control denial, ownership mismatch, integrity boundary, or other Windows authorization condition blocked creation;
3. endpoint security, ransomware protection, controlled-folder protection, organizational policy, or another filesystem filter denied the operation;
4. another process or tool held or interfered with the relevant path at the historical time;
5. the repository or its Git metadata directory was read-only, offline, redirected, synchronized, virtualized, or governed by a filesystem/provider restriction;
6. disk, quota, path-resolution, filesystem-health, or transient operating-system conditions surfaced as a permission failure;
7. environment or worktree metadata redirected the historical write boundary to a location with different access conditions; or
8. the condition was transient or the permitted evidence is insufficient to distinguish a cause.

These are hypotheses only. They are not ranked, and the observation set below may end with every candidate `UNRESOLVED`.

## 4. Closed observation list

Only the following observations may be considered for a later, separately authorized single run. Every item is non-Git and observation-only; omission from this list means prohibition.

| ID | Permitted observation | Purpose and boundary |
| --- | --- | --- |
| `NG-01` | Record the already-running operator process's Windows user name, session identity, token elevation state, integrity level, and group membership through native operating-system read interfaces. | Compare the current observation identity with the later authorization. Do not elevate, impersonate, acquire another token, or test a right by writing. |
| `NG-02` | Record the literal repository path `C:\Users\biz\Documents\Project\VMF`, its native resolved filesystem path, parent path, drive, volume type, free-space summary, and whether the repository path is local, redirected, synchronized, offline, compressed, encrypted, or read-only according to exposed filesystem metadata. | Evaluate path/provider/disk hypotheses without invoking Git or walking repository content. Do not relocate, normalize, repair, hydrate, mount, or change an attribute. |
| `NG-03` | Record existence, native resolved path, attributes, owner, and access-control metadata of the repository root and the `.git` directory object only. | Evaluate directory-level ownership and ACL hypotheses. Do not enumerate or open `.git` children; do not read, test, or reference the index or lock objects. Do not calculate effective rights through a write probe. |
| `NG-04` | Record inherited-versus-explicit access-control entries exposed for the repository root and `.git` directory object, including allow/deny type, principal, rights text, inheritance flags, and protection state. | Identify directly visible metadata that may support or contradict an access-control hypothesis. Do not change ACLs, ownership, inheritance, integrity, or attributes. |
| `NG-05` | Inventory already-running processes by process name, process ID, executable path when exposed, owning identity when exposed, and start time when exposed. | Identify possible contemporaneous tools without attachment or interference. Do not infer that a process owns a lock, and do not terminate, suspend, signal, debug, or inspect its handles. |
| `NG-06` | Read already-existing Windows operating-system event records that explicitly name the literal repository path or its `.git` directory and fall within a separately authorized time window. | Look for an existing access, filesystem, policy, quota, or provider denial. Do not enable logging, clear or acknowledge records, widen collection after execution begins, or inspect Avast. Empty or unavailable records remain `NO EVIDENCE`. |
| `NG-07` | Record current values of the native process environment variables `USERNAME`, `USERDOMAIN`, `USERPROFILE`, `TEMP`, and `TMP`, and Windows current-directory/path resolution for the literal repository path. | Detect identity or path-context mismatches without reading Git configuration or Git-derived variables. Do not change an environment variable or use an override. |
| `NG-08` | Compare only the authorized outputs of `NG-01` through `NG-07` with the literal P9-180 error record and classify each cause candidate as `SUPPORTED`, `CONTRADICTED`, or `UNRESOLVED`, with a direct evidence reference. | Keep historical and current facts separate. `SUPPORTED` is not root-cause proof; conflicting, indirect, missing, or stale evidence results in `UNRESOLVED`. |

No substitute observation, equivalent tool, extra field, recursive enumeration, follow-up query, or compensating check is implied. Exact tools or commands, if any, must be supplied and pre-classified in a later authorization record; this proposal does not authorize them.

## 5. Prohibitions and denylist

The later observation run, if separately authorized, must deny all of the following:

- every Git command and every Git-equivalent API, library, IDE source-control view, wrapper, alias, hook, or repository-status provider;
- stage, restage, retry staging, unstage, cached query, staged query, cached verification, index-derived inquiry, index refresh, index repair, index mutation, alternate index, or current Git-state derivation;
- listing, opening, parsing, hashing, copying, testing, creating, deleting, renaming, moving, replacing, editing, locking, unlocking, or otherwise operating on `.git/index`, `.git/index.lock`, or any lock object;
- recursive traversal or inventory of `.git`, including indirect traversal by search, antivirus, archive, backup, development, or diagnostic tooling initiated for this task;
- any write probe, temporary object, permission test that creates or changes an object, reproduction attempt, staging attempt, or effective-write-access test;
- permission elevation, alternate credentials, impersonation, ACL or ownership change, inheritance change, integrity change, attribute change, privilege assignment, policy change, or permission workaround;
- Git configuration, environment, repository location, filesystem/provider, security setting, controlled-folder setting, credential, or process-state changes;
- Avast access, query, scan, exclusion, acknowledgement, quarantine, restore, configuration, definition, service, process, log, or UI operation;
- process termination, suspension, signaling, attachment, debugging, handle inspection, or interference;
- lock operation of any kind, including attempted acquisition or release;
- build, test, project script, runner, Excel, release, publication, transmission, or external-service operation; and
- commit, push, pull, merge, rebase, reset, restore, clean, stash, amend, branch, tag, or any other Git operation.

If an exact proposed tool or command is absent from a later pre-approved allowlist, or if its implementation might cross any denylist boundary, it is prohibited.

## 6. Executor conditions

A later run is eligible only when all of these conditions are established before the first observation:

1. an accountable owner with stated identity, role, organization, authority source, and authority limits authorizes the exact `NG` IDs or a strict subset;
2. the authorization names one responsible operator and requires the same unelevated Windows identity, session, and repository path throughout the run;
3. every exact tool or command and every requested output field is listed and independently classified as non-Git, non-index-derived, non-cached, non-staged, non-refreshing, non-locking, and non-write-producing;
4. the authorization states the repository path, accepted observation IDs, output handling, single-run limit, start boundary, expiry boundary, and decision timestamp with numeric UTC offset;
5. the operator accepts P9-200 preservation, the P9-203 output rejection, this denylist, and the rule that diagnosis grants no remediation or downstream authority; and
6. a reviewer other than the operator confirms that the proposed run is all-and-only within the closed observation list and contains no mixed permitted/prohibited action.

Missing, implied, proxy-issued, open-ended, expired, or internally inconsistent authority is invalid. This proposal is not that authorization.

## 7. Validity and output boundaries

- Eligibility is limited to one uninterrupted observation run against the literal repository path and identity named in the later authorization.
- Authority begins no earlier than its stated effective time and expires at the first of the stated expiry, completion of the authorized list, first error, first mismatch, first unexpected fact, first denied or unavailable observation, first requested workaround, or any operator/session/path change.
- A partial run does not authorize resumption. A later run requires a new authorization and a freshly reviewed exact list.
- Outputs are diagnostic notes only. They must identify observation ID, timestamp with numeric UTC offset, operator identity, direct source, literal result, and `AVAILABLE`, `EMPTY`, `DENIED`, or `UNAVAILABLE` status without normalization.
- No output may be stored inside `.git`, fed to Git or Git-equivalent tooling, or used to infer current index, cached, staged, branch, working-tree, or commit state.
- Silence, empty output, denied access, unavailable fields, stale records, and unsupported fields are not positive findings.
- The P9-198 and P9-203 deviation outputs remain excluded and must not be quoted, compared, confirmed, refreshed, or reused.

## 8. Fail-closed conditions

Retain `NO-GO / SAFE-STOP` immediately, with no corrective or confirming action, if:

- fresh attributable authorization for the exact observation IDs, tools, fields, operator, path, and time boundary is missing, ambiguous, changed, or expired;
- an observation needs Git, repository-state derivation, `.git` traversal, index/lock access, a write, a lock, elevation, alternate credentials, ACL/ownership change, Avast interaction, process interference, or any workaround;
- a tool's behavior cannot be confidently classified before execution or differs from its approved classification;
- the identity, session, repository path, filesystem metadata, process context, authorization basis, or preserved documentation differs unexpectedly;
- any preserved P9-160 through P9-205 record or synchronization content is missing, conflicted, unexpectedly changed, narrowed, or incompletely accounted for;
- evidence is denied, inaccessible, incomplete, inconsistent, indirect, stale, broader than requested, or insufficient to distinguish observation from inference;
- any prohibited command or action occurs, including an ostensibly read-only Git or index-derived inquiry; or
- any result is presented as unique root cause, write-capability proof, remediation authority, Git-path reopening, cached evidence, verified snapshot, commit readiness, or technical-execution permission.

Any deviation requires exact documentary disclosure and rejection of all affected output for state promotion. No retry, correction, substitute observation, or compensating query follows automatically.

## 9. Post-observation choices

After a separately authorized run, the accountable owner may make one new, independent choice:

1. maintain the P9-200 hold without further action;
2. request a new docs-only proposal for a smaller or different observation-only evidence set;
3. close diagnosis as inconclusive because non-writing evidence cannot establish a unique cause or effective write capability;
4. prepare a separately governed environment-remediation proposal with its own operator, exact target, change, rollback, verification, and authorization boundaries; or
5. reconsider a future Git path only after all P9-200 minimum conditions and every still-applicable owner-control gate are independently satisfied.

Observation results do not select or authorize any choice. Remediation, remediation verification, owner reopening, intake acceptance, restaging authorization, a staging attempt, cached verification, commit authorization, and push authorization remain separate gates.

## 10. P9-200 hold maintenance and non-actions

The custody set is every existing P9-160 through P9-204 record, this P9-205 proposal, and the P9 synchronization content in `docs/VMF_vNext_Backlog.md`, `docs/development/CURRENT_STATUS.md`, and `docs/development/HANDOFF.md`. It remains reviewable uncommitted working-tree documentation, not a literal Git scope, cached snapshot, verified snapshot, commit candidate, or approved operation set.

Historically absent P9-179 and P9-185 remain absent. P9-174 remains `HOLD / NOT YET COMMIT-READY`; the restaging path remains held; no verified snapshot exists; and technical execution remains `NO-GO / SAFE-STOP`.

P9-205 performed no Git command, stage, restage, cached query, cached verification, index or lock operation, commit, push, diagnosis, observation run, build, test, script or runner, Excel or Avast operation, permission/ACL/ownership change, process interference, remediation, release, publication, transmission, external-service operation, or technical execution. Frozen specifications, public contracts, source, tests, tools, packages, `dist`, and workbooks were not changed.
