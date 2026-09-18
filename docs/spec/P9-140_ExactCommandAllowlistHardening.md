# P9-140 Exact Command Allowlist Hardening

## 1. Record status

- Work item: `P9-140`
- Activity: Exact command allowlist hardening
- Basis: `P9-139` static risk-control matrix
- Work mode: `docs-only`
- Technical execution: `NO-GO / SAFE-STOP`
- Document status: `COMPLETE / docs-only exact command allowlist hardening / ACCEPT`
- Revised draft review: `ACCEPT`
- Evidence status: No technical evidence was generated or revalidated by this drafting activity.

This record defines a fail-closed candidate allowlist for a future, separately authorized documentation-state confirmation. It separates narrowly permitted read-only documentation checks from prohibited technical execution. It does not authorize any listed command, establish a technical execution candidate, or make a technical GO decision.

## 2. Controlling premises

- P9-139 is the accepted static risk-control matrix and is the direct governance basis for this draft.
- Technical execution remains `NO-GO / SAFE-STOP`.
- Avast detection remains unresolved.
- P9-94 remains non-reusable as a present decision basis or command authorization.
- P9-130 and P9-135 remain non-rerunnable and outside all rerun candidate sets.
- Security disposition, continuation authorization, command authorization, and technical execution authorization remain separate gates.
- A command appearing in this candidate allowlist is not authorized until a later authoritative record approves the exact command text, working directory, purpose, and allowed output handling.

## 3. Candidate authorization model

A future confirmation command may be considered only when all of the following are true:

1. The command text is an exact character-for-character match for one entry in section 4.
2. The working directory is exactly the repository root: `C:\Users\biz\Documents\Project\VMF`.
3. The stated purpose is limited to reviewing the P9-140 documentation changes and repository documentation state.
4. The command is issued directly, without a PowerShell, `cmd`, batch, script, macro, pipeline, redirection, command separator, subshell, alias, function, variable, wildcard, command substitution, or wrapper.
5. No argument, path, option, revision, environment setting, or output destination is added, removed, reordered, or substituted.
6. A separate authoritative authorization explicitly names the exact command before it is run.
7. Each authorization is single-use. Prior approval, prior output, or successful completion does not authorize repetition or another entry.

Failure to satisfy every condition means the command is not allowlisted and must not run.

## 4. Candidate exact-command allowlist

The following commands are candidates only. They are not authorized by this draft.

| ID | Exact candidate command | Narrow purpose | Permitted effect |
|---|---|---|---|
| AL-01 | `git branch --show-current` | Report the current branch name. | Read-only stdout. |
| AL-02 | `git status --short` | Report tracked and untracked working-tree entries. | Read-only stdout. |
| AL-03 | `git diff -- docs/VMF_vNext_Backlog.md docs/development/CURRENT_STATUS.md docs/development/HANDOFF.md` | Review unstaged changes only for the three tracked P9-140 synchronization paths. | Read-only stdout. |
| AL-04 | `git diff --no-index -- NUL docs/spec/P9-140_ExactCommandAllowlistHardening.md` | Review the new untracked P9-140 draft against an empty file. Exit code `1` means a difference was shown and is not, by itself, a failure. | Read-only stdout and exit status. |
| AL-05 | `git diff --stat -- docs/VMF_vNext_Backlog.md docs/development/CURRENT_STATUS.md docs/development/HANDOFF.md` | Summarize unstaged changes only for the three tracked P9-140 synchronization paths. | Read-only stdout. |
| AL-06 | `git diff --name-only -- docs/VMF_vNext_Backlog.md docs/development/CURRENT_STATUS.md docs/development/HANDOFF.md` | List changed names only within the three tracked P9-140 synchronization paths. | Read-only stdout. |
| AL-07 | `git diff --check -- docs/VMF_vNext_Backlog.md docs/development/CURRENT_STATUS.md docs/development/HANDOFF.md` | Check only the three tracked P9-140 synchronization paths for Git whitespace errors. | Read-only exit status and stdout/stderr. |
| AL-08 | `git status --short --untracked-files=all -- docs/spec/P9-140_ExactCommandAllowlistHardening.md` | Confirm that the new P9-140 draft is present as the single named untracked path. | Read-only stdout and exit status. |

This is a closed list. Similarity, equivalent intent, read-only appearance, a path alias, or a prior authorization does not add a command to the list. A future record may authorize only one or more exact entries; it must not authorize the list by category or implication.

### 4.1 Command outcome contracts

The following outcome contract applies independently to each candidate. An allowed exit code is not sufficient by itself: stdout and stderr must also match the same row. Output must be complete and attributable to that single invocation.

For AL-03, AL-05, AL-06, and AL-07, the only permitted stderr exception is zero or more Git line-ending warnings in exactly this form, with `<path>` replaced by one of the three paths named in that command:

`warning: in the working copy of '<path>', LF will be replaced by CRLF the next time Git touches it`

No other warning text or path is permitted. These line-ending warnings are informational only and do not convert a failed or ambiguous result into an allowed result.

| ID | Allowed exit code | Allowed stdout | Allowed stderr | Mandatory stop conditions |
|---|---|---|---|---|
| AL-01 | `0` only. | Exactly one non-empty line containing the current branch name and no other output. | Empty only. | Stop on a nonzero exit, empty or multiline stdout, detached or indeterminate branch state, or any stderr. |
| AL-02 | `0` only. | Empty, or one or more complete Git short-status records. No prose, diagnostic, or non-status text. | Empty only. | Stop on a nonzero exit, malformed or incomplete status record, unexpected text, truncation, or any stderr. |
| AL-03 | `0` only. | Empty, or a complete unified diff limited to the three named synchronization paths. | Empty, or only the path-limited line-ending warnings defined above. | Stop on a nonzero exit, any other path or output form, incomplete or truncated diff, or non-permitted stderr. |
| AL-04 | `1` only, meaning the new non-empty draft differs from the synthetic empty input. | One complete unified diff whose only substantive file content is `docs/spec/P9-140_ExactCommandAllowlistHardening.md` compared with `NUL`. | Empty only. | Stop on exit `0`, exit greater than `1`, absent or incomplete diff, any other substantive path, truncation, or any stderr. Exit `1` is allowed only together with the specified complete stdout and empty stderr. |
| AL-05 | `0` only. | Empty, or a complete diff-stat limited to the three named synchronization paths. | Empty, or only the path-limited line-ending warnings defined above. | Stop on a nonzero exit, any other path or output form, incomplete output, or non-permitted stderr. |
| AL-06 | `0` only. | Empty, or one or more lines drawn only from the three named synchronization paths. | Empty, or only the path-limited line-ending warnings defined above. | Stop on a nonzero exit, duplicate, malformed, or out-of-scope path, incomplete output, or non-permitted stderr. |
| AL-07 | `0` only. | Empty only. | Empty, or only the path-limited line-ending warnings defined above. | Stop on a nonzero exit, any stdout, or non-permitted stderr. A whitespace diagnostic is a failure, not an allowed result. |
| AL-08 | `0` only. | Exactly `?? docs/spec/P9-140_ExactCommandAllowlistHardening.md` followed only by the normal line terminator. | Empty only. | Stop on a nonzero exit, empty stdout, any different status or path, additional output, truncation, or any stderr. |

AL-04 is the sole `NUL` exception in this allowlist. In AL-04 only, `NUL` is a synthetic empty comparison input and not a repository or technical-artifact path. It may not be substituted, redirected, written, used as an output destination, or reused by another command. AL-08 contains no `--check` or `--no-index` combination, so it cannot conflate a content-difference exit with a whitespace-error exit.

## 5. Prohibited commands and operations

The following are outside the candidate allowlist and remain prohibited:

- every command not reproduced exactly in section 4;
- PowerShell, `cmd`, batch files, shell scripts, VBA, Python, or any other interpreter, wrapper, or script execution;
- pipelines, redirection, command separators, subshells, aliases, functions, variables, wildcards, command substitution, or output-file creation;
- tests, builds, formatters, linters, parsers, runners, macros, fixtures, or executable inspection;
- Excel, workbook, add-in, VBProject, or residual-process operations;
- Avast, antivirus, security-tool, quarantine, exclusion, submission, remediation, or configuration operations;
- external-service, network, credential, token-store, Google Docs, Google Drive, GitHub, or production-service access;
- package, `dist`, release, deployment, publication, or tag operations;
- Git mutation or remote operations, including `git add`, `commit`, `push`, `pull`, `fetch`, `merge`, `rebase`, `reset`, `restore`, `checkout`, `switch`, `stash`, `clean`, `tag`, or branch creation/deletion;
- any command that opens, hashes, scans, copies, moves, renames, deletes, generates, or modifies a technical artifact;
- any reproduction or rerun of P9-130 or P9-135, including a renamed, wrapped, partial, equivalent, or materially similar operation;
- any reuse of P9-94 as authority, evidence eligibility, or a basis to expand this list;
- any security disposition, continuation GO, technical execution GO, or inference that a successful documentation check establishes technical readiness.

## 6. Fail-closed conditions

Stop without running a command when any of the following applies:

- exact command text, working directory, purpose, or single-use authorization is absent, ambiguous, conditional, stale, or broader than one or more named entries;
- the proposed command differs from an allowlist entry in any token, option, argument, path, order, revision, wrapper, or execution context;
- the proposed action requires PowerShell, a script, a wrapper, shell syntax, a wildcard, a variable, a pipeline, redirection, or output persistence;
- the proposed action reads or affects a path outside the four named P9-140 documentation paths, except for the repository metadata inherently reported by AL-01 or AL-02 and the synthetic empty `NUL` input used only by the exact AL-04 command;
- the working tree contains an unexpected or unattributed change that prevents isolation of the P9-140 documentation review;
- the command would access or alter technical artifacts, Excel, Avast, an external service, package / `dist`, release state, tags, or Git history/index/remotes;
- P9-94 is offered as reusable authority or evidence;
- P9-130 or P9-135 is proposed directly, indirectly, partially, under another name, or as a rerun candidate;
- Avast detection remains unresolved and the requested action depends on treating that condition as cleared;
- exit code, stdout, or stderr does not satisfy every element of the applicable section 4.1 outcome contract;
- output is incomplete, truncated, unavailable, inconsistent, attributable to more than one invocation, or would require another unlisted command to interpret;
- the outcome is indeterminate, cannot be classified uniquely, or admits more than one interpretation, including any inability to distinguish a content difference from a whitespace error;
- any exit code, stdout, stderr, warning, diagnostic, status, path, or output combination is unexpected or not expressly allowed;
- authorization ownership or authority is unclear; or
- the result is being used to infer security acceptance, continuation authorization, technical execution authorization, or control effectiveness.

A fail-closed stop produces no technical result and must not be recorded as `PASS`. Every indeterminate or unexpected outcome is unconditionally fail-closed. The only permitted conclusion is that the proposed confirmation was not authorized or not completed under the exact allowlist boundary.

## 7. P9-139 correspondence

| P9-139 control | P9-140 hardening response |
|---|---|
| RC-01 — distinct execution authorization | Requires a separate, exact, single-use command authorization and states that this draft authorizes nothing. |
| RC-03 — missing or barred evidence | Rejects absent, truncated, unavailable, stale, ineligible, indeterminate, or unexpected evidence and forbids use of P9-94. |
| RC-04 — historical-evidence applicability | Prevents prior authorization or prior successful output from being reused as current command authority. |
| RC-05 — independent decision gates | Keeps security disposition, continuation authorization, command authorization, and technical execution authorization separate. |
| RC-06 — scope expansion and barred reruns | Uses a closed list, exact paths, direct invocation only, and an explicit prohibition on P9-130/P9-135 reruns or equivalents. |
| RC-07 — approval ownership | Fail-closes when the authorizing owner or authority basis is unclear. |
| RC-08 — narrow interpretation | Requires character-for-character command matching plus an exact per-command outcome contract, and forbids category, similarity, implied authorization, or ambiguous result interpretation. |
| RC-10 — documentation synchronization versus execution | Limits candidates to documentation-state review, keeps the AL-04 `NUL` exception synthetic and command-specific, and forbids treating results as execution or validation evidence. |
| RC-11 — future activity integrity | Does not define a future technical activity and excludes P9-130/P9-135 from every candidate route. |
| RC-12 — no silent boundary relaxation | Preserves `NO-GO / SAFE-STOP` and requires a later authoritative record for any changed condition or authorization. |

## 8. Revised draft review

The revised P9-140 draft review result is `ACCEPT`. The review confirmed that:

- AL-04 is the sole, explicit, command-limited use of `NUL` as a synthetic empty comparison input;
- AL-08 does not combine `--no-index` with `--check` and cannot conflate a content-difference exit with a whitespace-error exit;
- AL-01 through AL-08 each define an allowed exit code, allowed stdout, allowed stderr, and mandatory stop conditions;
- every indeterminate or unexpected outcome is unconditionally fail-closed;
- P9-94 remains non-reusable; and
- P9-130 and P9-135 remain non-rerunnable and outside all rerun candidates.

`ACCEPT` applies only to the content of this documentation record. It is not command authorization, control implementation, control-effectiveness confirmation, security disposition, continuation authorization, or technical execution authorization.

## 9. Closeout state

P9-140 is `COMPLETE / docs-only exact command allowlist hardening / ACCEPT`. It records eight narrowly scoped future command candidates and a broader explicit prohibition set. It does not authorize those commands or any other operation.

Technical execution remains `NO-GO / SAFE-STOP`. Avast detection remains unresolved. P9-94 remains non-reusable. P9-130 and P9-135 remain non-rerunnable and outside rerun candidates. No technical execution GO decision is made.
