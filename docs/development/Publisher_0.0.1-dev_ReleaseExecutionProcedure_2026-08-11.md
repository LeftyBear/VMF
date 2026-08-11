# Publisher 0.0.1-dev Release Execution Procedure

Status  : Docs-only procedure decomposition
Date    : 2026-08-11
Scope   : Future Publisher `0.0.1-dev` release execution procedure
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_0.0.1-dev_GoNoGoDecisionMemo_2026-08-11.md, docs/development/Publisher_0.0.1-dev_ReleaseExecutionGateChecklist_2026-08-11.md, docs/development/Publisher_0.0.1-dev_ReleaseExecutionPreReview_2026-08-11.md, docs/architecture/ADR-0019-vmf-risk-acceptance-and-release-hold-lift.md

This document decomposes a future Publisher `0.0.1-dev` release execution
procedure into gated phases. It is a planning and procedure record only. It
does not execute a release, create or update packages, modify `dist`, recreate
or modify a ZIP, run `vmf-publisher.exe`, run build or tests, execute Live E2E,
mutate Google Docs or Google Drive, operate on OAuth/token-store/credentials,
operate on Avast, create or update tags, publish artifacts, or push.

This procedure does not claim Avast vendor clearance. It does not claim Avast
safety certification. Local scan observations, VMF-side residual risk
acceptance, and GitHub prerelease publication records must not be restated as
vendor clearance or Avast safety certification.

## 1. Preconditions Before Execution

Before any future release execution step begins, the operator must confirm and
record all of the following preconditions:

| Precondition | Required Evidence | Stop If |
| --- | --- | --- |
| Repository identity | Current branch, `HEAD`, `origin/main`, and working-tree state are captured. | `HEAD` mismatches the approved commit, the branch is unexpected, or unrelated changes are present. |
| Procedure scope | The exact requested execution phase is named. | The request is ambiguous or attempts to approve multiple independently gated phases by implication. |
| Approval source | Explicit operation-specific approval exists for the named phase. | Approval is missing, broad, stale, or does not name the exact operation. |
| Current status records | `CURRENT_STATUS.md`, `Publisher_ReleaseApprovalPackage.md`, the gate checklist, the pre-review, and the Go/No-Go memo are reviewed. | A current record contradicts the requested phase or introduces a new blocker. |
| Release identity | Version, tag, package path, package SHA-256, target commit, evidence docs commit, and GitHub Release identity are matched to the approved operation. | Any identity value is missing, ambiguous, or mismatched. |
| Vendor boundary | Vendor clearance remains treated as not obtained unless a future Avast response is recorded and reviewed. | Any step depends on claiming Avast vendor clearance or Avast safety certification without recorded evidence. |
| External boundary | Google Docs, Google Drive, OAuth/token-store/credentials, Avast, and GitHub Release operations are separately authorized if they are part of the requested phase. | The requested phase requires an external operation that is not explicitly authorized. |
| Artifact boundary | Package, ZIP, `dist`, and executable operations are separately authorized if they are part of the requested phase. | The requested phase would alter artifact identity without explicit approval. |

Precondition review is itself evidence collection. It is not release execution
and must not be reported as a completed release phase.

## 2. Required Explicit Approval Point

The release execution gate opens only after a recorded approval names the exact
next phase, scope, artifact identity, external-service boundary, and Git
boundary.

Approval must remain narrow:

- approval to create or edit this procedure does not approve release execution;
- approval to run local verification does not approve Live E2E;
- approval to run Live E2E does not approve package, ZIP, or `dist` changes;
- approval to inspect an existing package does not approve package
  replacement;
- approval to create or update a package does not approve tag creation,
  GitHub Release work, asset upload, publication, or announcement;
- approval to stage does not approve commit;
- approval to commit does not approve push;
- approval for a local operation does not approve Google Docs, Google Drive,
  OAuth/token-store/credentials, or Avast operations unless those operations
  are explicitly named.

If an approval request attempts to combine phases, the operator must either
split it into named approvals or stop for a new recorded decision.

## 3. Release Execution Phases

The future execution sequence must proceed in the ADR-0019 order unless a
later recorded decision supersedes it:

1. Final verification.
2. Live E2E.
3. Result review.
4. Package / `dist`.
5. Tag / release.

Each phase below is a procedure definition only. No phase is executed by this
document.

### Phase 1. Final Verification

Purpose: establish a current local verification basis before any external or
publication step.

Procedure:

1. Confirm Phase 1 approval names the solution, configuration, check list, and
   whether the check may write local build outputs.
2. Confirm no package, ZIP, `dist`, Live E2E, Google, OAuth/token-store,
   credential, Avast, tag, release, publication, commit, or push operation is
   included unless separately authorized.
3. Run only the approved final-verification checks.
4. Record pass/fail status, warning count, error count, skipped count, command
   boundaries, and repository state after the phase.

Evidence to collect:

- approval text or approval record identifier;
- branch, `HEAD`, `origin/main`, and working-tree state before and after;
- exact commands, if execution is authorized;
- pass/fail result for each check;
- warning count, error count, skipped count;
- confirmation that Live E2E, package/`dist`, `vmf-publisher.exe`, Google,
  OAuth/token-store/credentials, Avast, tag, release, publication, and push
  were not performed unless explicitly authorized.

### Phase 2. Live E2E

Purpose: verify the credentialed Google Docs integration only when explicitly
approved and only after final verification has passed or been accepted by a
recorded decision.

Procedure:

1. Confirm Phase 2 approval names Live E2E, target test scope, Google Docs /
   Google Drive mutation boundary, OAuth mode, credential boundary, and cleanup
   expectations.
2. Confirm final-verification evidence is available and acceptable for the
   current release identity.
3. Enable only the authorized Live E2E environment for the approved duration.
4. Run only the approved Live E2E checks.
5. Disable Live E2E environment state after execution and record cleanup.

Evidence to collect:

- approval text or approval record identifier;
- final-verification evidence reference;
- Live E2E command scope and result;
- total, passed, failed, and skipped counts;
- redacted Google Docs / Google Drive mutation evidence;
- confirmation that no token, refresh token, client secret, Authorization
  header, private URL, provider payload, or raw credential value was recorded;
- post-run confirmation that Live E2E environment settings were disabled;
- repository state after the phase.

### Phase 3. Result Review

Purpose: review final verification and Live E2E evidence before artifact or
publication work proceeds.

Procedure:

1. Confirm Phase 3 approval names result review only, unless another phase is
   explicitly approved.
2. Review Phase 1 and Phase 2 evidence for pass/fail status, identity match,
   redaction completeness, and unresolved anomalies.
3. Record the decision outcome as Go, No-Go, or Hold for the next phase.
4. If any failure or ambiguity exists, record the exact stop condition and do
   not proceed to package/`dist`.

Evidence to collect:

- reviewed evidence references;
- identity match for version, tag, package path, SHA-256, and target commit;
- explicit review conclusion;
- list of unresolved failures, warnings, ambiguous results, skipped checks, or
  redaction issues;
- next-phase recommendation and whether approval is still required;
- repository state after review.

### Phase 4. Package / Dist

Purpose: create, update, verify, or inspect package artifacts only when this
exact artifact operation is explicitly authorized.

Procedure:

1. Confirm Phase 4 approval names whether the operation is package creation,
   package update, package verification, package inspection, or `dist` write.
2. Confirm result review authorizes proceeding to package/`dist`.
3. Confirm expected artifact identity: version, runtime, package path, asset
   name, size expectation if applicable, SHA-256 expectation if applicable, and
   target commit.
4. Execute only the approved package operation.
5. Record generated or inspected artifact identity.
6. Stop before tag/release unless Phase 5 is separately authorized.

Evidence to collect:

- approval text or approval record identifier;
- result-review Go decision reference;
- exact package path and artifact operation performed;
- package size and SHA-256 after the phase;
- package manifest verification result, if authorized;
- static package inspection result, if authorized;
- confirmation whether `vmf-publisher.exe` was or was not executed;
- confirmation that tag/release/publication was not performed unless
  separately authorized;
- repository state after the phase.

### Phase 5. Tag / Release

Purpose: create or update Git tag / GitHub Release / asset publication only
when explicitly authorized after package evidence is accepted.

Procedure:

1. Confirm Phase 5 approval names the exact tag, release state, prerelease
   setting, asset name, asset source path, upload behavior, and push boundary.
2. Confirm package/`dist` evidence matches the approved artifact identity.
3. Confirm any Git staging, commit, tag, push, GitHub Release creation, asset
   upload, and publication action is separately named.
4. Execute only the approved Git or GitHub Release operation.
5. Read back the remote tag, release metadata, asset name, asset size, and
   digest when authorized.
6. Record publication evidence without claiming vendor clearance or Avast
   safety certification.

Evidence to collect:

- approval text or approval record identifier;
- package evidence reference;
- tag name, tag object, peeled commit, and remote tag readback;
- GitHub Release URL, prerelease flag, release name, and asset list;
- uploaded asset name, size, and digest;
- local/remote digest comparison;
- repository state after the phase;
- confirmation that Avast vendor clearance remains not obtained unless a
  future Avast response is recorded and reviewed.

## 4. Stop Conditions / Hard Stops

Execution must stop immediately if any of the following occurs:

- approval is missing, ambiguous, stale, or does not name the exact phase;
- the requested phase is out of ADR-0019 order and no later decision
  authorizes the deviation;
- branch, `HEAD`, `origin/main`, package identity, tag identity, or artifact
  SHA-256 mismatches the approved record;
- working tree contains unrelated or unexplained changes;
- a command would modify package, ZIP, `dist`, tag, release, asset, Google
  Docs, Google Drive, OAuth/token-store/credentials, Avast, or publication
  state without explicit approval;
- a step requires running `vmf-publisher.exe` without explicit executable-run
  approval;
- a step requires Live E2E without explicit Google and credential-boundary
  approval;
- any evidence contains secrets, tokens, Authorization headers, private URLs,
  raw provider payloads, local absolute paths not intended for publication, raw
  exception bodies, or stack traces requiring redaction;
- any check fails and no recorded decision authorizes continuing;
- a result is ambiguous, partially successful, or cannot be reproduced well
  enough for the requested gate;
- any wording would imply vendor clearance or Avast safety certification
  without a future recorded Avast response.

Hard-stop evidence must record the phase, attempted operation, stop reason,
repository state, artifact identity if relevant, and the next required
decision. A hard stop is not a partial release success.

## 5. Evidence Collection Rules

Evidence must be collected phase-by-phase and must distinguish direct
verification from copied or historical status.

Minimum evidence fields:

- phase name and date;
- approval source and scope;
- operator or automation identity, when available;
- branch, `HEAD`, `origin/main`, and working-tree state;
- artifact identity before and after the phase, when applicable;
- exact command or external operation, when execution is authorized;
- result status with pass/fail/warning/error/skipped counts where applicable;
- external mutation summary, when explicitly authorized;
- redaction statement;
- stop condition or next approved phase.

Use `PASS` only for directly verified evidence from the current authorized
phase. Historical evidence may be cited as prior evidence, but it must not be
converted into a new current `PASS` result unless it was actually re-verified.

## 6. Operations Still Prohibited Unless Explicitly Authorized

The following operations remain prohibited unless an approval names the exact
operation:

- release execution;
- tag creation, tag update, tag push, GitHub Release creation or update,
  asset upload, publication, or announcement;
- build or test execution;
- package creation, package update, package replacement, ZIP modification, or
  `dist` write;
- package verification or static package inspection;
- `vmf-publisher.exe` execution;
- packaged executable smoke testing;
- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- OAuth operation, token-store operation, credential operation, credential
  cleanup, or reauthorization;
- Avast operation, Avast UI interaction, Avast setting change, quarantine
  action, exclusion creation, vendor submission, or vendor-response handling;
- production code, test, public API, persisted schema, canonical format, or
  Frozen specification change;
- staging, commit, push, merge, rebase, reset, stash, or history rewrite,
  except where the user explicitly authorizes the exact Git operation.

Documentation approval for this procedure does not authorize any prohibited
operation above.

## 7. Rollback / Abort Boundary

Abort is the default response to an unapproved, failed, ambiguous, or
identity-mismatched phase. The operator must stop at the first failed boundary
and record the abort evidence before considering remediation.

Rollback authority is separate from abort authority:

- aborting before artifact or external mutation requires only recording the
  stop condition and repository state;
- removing or replacing a package, ZIP, tag, release, asset, Google resource,
  OAuth/token-store state, credential state, or Avast setting requires explicit
  rollback approval for that exact resource;
- Git rollback, history rewrite, tag deletion, release deletion, asset
  deletion, and Google Drive cleanup must not be performed by implication;
- incident, withdrawal, or rollback records must not imply that release
  execution succeeded unless execution actually occurred and was verified.

If a rollback would require destructive Git or external-service changes, stop
and request a new recorded decision before acting.

## 8. Final Post-Execution Verification Requirements

After any future authorized release execution phase, the operator must perform
post-execution verification appropriate to the completed phase and record the
result before requesting the next phase.

Required final verification after the full sequence:

| Area | Required Readback |
| --- | --- |
| Repository | Branch, `HEAD`, `origin/main`, working-tree state, staged state, and commit/push state. |
| Local evidence | Final verification results, warnings, errors, skipped tests, and `git diff --check` result if authorized. |
| Live E2E | Live E2E result, cleanup state, and redacted Google mutation evidence if authorized. |
| Package | Package path, size, SHA-256, manifest verification, static inspection, and executable-run boundary. |
| Tag | Tag name, annotated tag object if applicable, peeled commit, and remote tag readback. |
| GitHub Release | Release URL, prerelease flag, release name, asset name, asset size, digest, and local/remote digest match. |
| Vendor boundary | Explicit statement that Avast vendor clearance is either still not obtained or is supported by a newly recorded and reviewed Avast response. |
| Safety certification boundary | Explicit statement that Avast safety certification is not claimed unless a future Avast record directly supports that claim. |
| Prohibited operations | Confirmation that no unapproved Google, OAuth/token-store/credentials, Avast, package, `dist`, executable, tag, release, publication, Git, code, test, API, schema, canonical-format, or Frozen-specification operation occurred. |

The final post-execution record must distinguish:

- planned procedure from executed operations;
- approved operations from prohibited operations;
- direct current evidence from historical evidence;
- VMF-side risk acceptance from vendor clearance;
- GitHub publication from Avast safety certification.

This document is complete as a docs-only release execution procedure
decomposition. It does not execute or authorize any future Publisher
`0.0.1-dev` release step by itself.
