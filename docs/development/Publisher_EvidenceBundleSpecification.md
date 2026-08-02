# Publisher Evidence Bundle Specification

Status  : Done
Scope   : Documentation-only evidence bundle design for Publisher release, security review, false-positive appeal, and internal audit
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_Phase4_LocalVerificationEvidence.md, docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md, docs/development/Publisher_Phase4-2-2_ErrorHandlingSpecification.md, docs/development/Publisher_Phase4-2-3_RetryPolicySpecification.md, docs/distribution/PublisherReleaseRunbook.md, docs/distribution/ReleaseChecklist.md

This document defines the intended structure of a VMF Publisher evidence
bundle. It is documentation only. It does not create or update packages,
modify `dist/`, execute Live E2E, mutate Google Docs or Google Drive, re-run
flagged executables, submit files to antivirus vendors, approve a release,
change production code, change public APIs, or modify Frozen specifications.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. The bundle design must not be
used as antivirus vendor clearance, package approval, Live E2E evidence,
release readiness, or permission to resume the release path.

## 1. Purpose

The Evidence Bundle is a curated, redacted, reviewable collection of Publisher
release and security evidence. Its purpose is to make the evidence set
portable across release review, security review, antivirus false-positive
appeal, internal audit, and later regression investigation without copying
unsafe local details into review records.

The bundle records:

- what artifact, commit, and version a review concerns;
- which local verification commands passed, failed, or were not executed;
- which checks are mock-backed, non-live, static, or external;
- what diagnostic and error-handling behavior is expected;
- which retry behavior is safe, bounded, and non-mutating;
- which hold conditions still block release continuation;
- which evidence is intentionally excluded for security and privacy reasons.

The bundle is an evidence packaging convention. It does not replace the
release runbook, release checklist, go/no-go review, package verification
script, or security and supply-chain review.

## 2. Use Scenarios

### Avast False Positive Appeal

Use the bundle to provide a safe summary of the selected Publisher artifact,
build context, package manifest checks, source verification, and containment
status when preparing an antivirus false-positive appeal.

Required stance:

- include hashes, version, target commit, package type, and scanner status when
  those values were actually verified;
- include the current release hold and whether the flagged executable was
  re-run;
- exclude tokens, credentials, private URLs, personal account details, and raw
  provider exception bodies;
- do not claim Avast clearance until a vendor response or owner exception
  decision is separately recorded.

### Release Gate Review

Use the bundle to collect evidence for a release gate decision. The bundle may
support the gate, but it does not approve it.

Required stance:

- keep local verification, package verification, Live E2E, security review,
  owner go/no-go, tag creation, and publication as separate gates;
- record unexecuted gates as `PENDING`, `BLOCKED`, `SKIPPED`, `DEFERRED`, or
  `NOT EXECUTED` instead of inferring success;
- link to the release runbook and release checklist rather than duplicating
  the release procedure.

### Internal Audit

Use the bundle as an audit index for traceability from a release decision back
to commands, evidence files, artifact identity, known exclusions, and
remaining blockers.

Required stance:

- record the target branch, target commit, working-tree state, and staged
  state at the time evidence was collected;
- preserve exact command lines only when they do not contain local absolute
  paths, tokens, credentials, private URLs, or personal account details;
- record whether external mutation was authorized and whether it occurred.

### Regression Investigation

Use the bundle to compare a current failure against known good local evidence,
diagnostic log samples, error classification, retry behavior, and release
records.

Required stance:

- distinguish source/test regressions from release-operation holds;
- keep operational holds, such as pending antivirus classification, separate
  from product regressions;
- avoid copying raw exception bodies or private document links into the bundle.

## 3. Bundle Structure

An Evidence Bundle should contain the following logical sections. A concrete
bundle may be a directory, archive, or review record, but each section should
remain separately identifiable.

### 3.1 Metadata

| Field | Requirement |
| --- | --- |
| Bundle name | Required. Follow Section 6 naming convention. |
| Bundle status | Required: `Draft`, `Ready for Review`, `Deferred`, or `Archived`. |
| Evidence date | Required. Use local date plus UTC timestamp when command evidence is included. |
| Publisher version | Required when release-related. |
| Target branch | Required for release review. |
| Target commit | Required for release review or package evidence. |
| Artifact identity | Required only when a selected artifact exists and was authorized for inspection. |
| Release state | Required. Preserve current hold/deferred state exactly. |

### 3.2 Build Evidence

Build evidence records local source build status.

Include:

- command label or safe command line;
- solution or project name;
- configuration;
- restore mode when relevant;
- result;
- warning count;
- error count;
- timestamp;
- whether the check was local-only.

Do not include:

- local absolute paths;
- user profile paths;
- package paths containing personal directory information;
- environment variables containing secrets or credentials.

### 3.3 Unit Test Evidence

Unit test evidence records focused or full unit test outcomes.

Include:

- test project;
- filter, when used;
- passed, failed, skipped, and total counts;
- result;
- timestamp;
- warning/error summary if reported.

Do not call a test `PASS` unless the test was directly executed and the result
was observed for the same evidence collection.

### 3.4 Integration And Mock Evidence

Integration and mock evidence records non-live integration checks, fake-backed
gateway checks, in-memory adapter checks, dry-run checks, and local verify
checks.

Include:

- whether `VMF_PUBLISHER_GOOGLE_E2E` was disabled;
- whether Google Docs and Google Drive mutation was not performed;
- whether temporary public hosting was not performed;
- passed, failed, skipped, and total counts;
- relevant fixture or fake gateway identity when safe.

Do not represent mock-backed or non-live checks as Google Docs API readback,
Google Drive cleanup evidence, or Live E2E evidence.

### 3.5 Diagnostic Log Sample

Diagnostic log samples should demonstrate the safe structured logging shape
defined by `Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md`.

Include only sanitized samples with fields such as:

- `timestampUtc`;
- `level`;
- `sessionId`;
- `command`;
- `phase`;
- `operation`;
- `code`;
- `message`;
- `classification`;
- `exitCode`.

Samples must be synthetic or redacted unless captured output has been reviewed
for sensitive values. Do not include raw exception messages, document URLs,
temporary public image URIs, Google Drive file IDs, local paths, stack traces,
HTTP bodies, tokens, or credential values.

### 3.6 Error Handling Matrix

The bundle should summarize the current error-handling contract by reference
to `Publisher_Phase4-2-2_ErrorHandlingSpecification.md`.

Include:

- CLI classification names;
- exit code mapping;
- stable code families;
- safe summary message rule;
- verification/readback failure treatment;
- cancellation treatment.

Do not duplicate the full specification unless the bundle is intentionally
snapshotted for an external review packet.

### 3.7 Retry Policy Summary

The bundle should summarize retry behavior by reference to
`Publisher_Phase4-2-3_RetryPolicySpecification.md`.

Include:

- retryable `NotSent` boundary;
- `Sent` and `Unknown` delivery non-retry rule;
- non-retryable verification failure rule;
- bounded retry/backoff expectation;
- cancellation handling;
- safe structured logging expectations.

Do not imply that retry policy permits automatic re-execution of a flagged
package or live Google mutation outside an authorized run.

### 3.8 Release Runbook References

The bundle should link to current release-operation records rather than
copying the full procedure.

Required references:

- `docs/distribution/PublisherReleaseRunbook.md`;
- `docs/distribution/ReleaseChecklist.md`;
- `docs/development/CURRENT_STATUS.md`;
- `docs/development/Publisher_TestClassification.md`;
- relevant Phase 4-3 review records;
- release notes for the target release, when applicable.

### 3.9 Hold And Resume Conditions

The bundle must preserve the current release hold and resume conditions.

Current hold conditions include:

- Avast false-positive handling is pending;
- release gate reopening is not authorized;
- Live E2E is not authorized;
- Google Docs and Google Drive mutation is not authorized;
- package or distribution artifact creation/update is not authorized;
- flagged executable re-run is not authorized;
- tag, release, publication, and push are not authorized.

Resume may proceed only after the relevant owner approvals or vendor responses
are recorded in the applicable release records. Authorization for one gate does
not authorize another gate.

## 4. Prohibited Bundle Contents

The Evidence Bundle must not include:

- access tokens;
- refresh tokens;
- credential JSON;
- private keys;
- client secrets;
- authorization headers;
- cookies;
- token-store contents;
- credential file contents;
- local absolute paths;
- user profile paths;
- package extraction paths containing personal directory information;
- personal account details;
- Google Docs private URLs;
- private Google Drive URLs;
- raw exception bodies;
- raw HTTP request or response bodies;
- stack traces containing paths or provider payloads;
- temporary public image URLs;
- temporary Google Drive file IDs;
- unredacted environment variable dumps;
- screenshots or logs that expose private document contents unless separately
  approved for that review packet.

Compatibility note: a successful Publisher CLI run may emit `documentUrl` in
its normal output. The Evidence Bundle must treat that as sensitive and redact
or omit it unless the review packet has explicit approval to include it.

## 5. Folder Convention

Recommended future bundle root:

```text
docs/evidence/publisher/<version>/<YYYYMMDD>-<purpose>/
```

Allowed purpose values:

- `release-gate`;
- `security-review`;
- `false-positive-appeal`;
- `internal-audit`;
- `regression-investigation`.

The folder should contain only redacted text evidence and approved small
metadata files. Large generated artifacts, packages, crash dumps, or raw tool
logs should not be stored under `docs/`.

This task does not create `docs/evidence/` because no concrete evidence bundle
is being assembled.

## 6. Naming Convention

Use deterministic, reviewable names.

| Item | Convention |
| --- | --- |
| Bundle directory | `<YYYYMMDD>-publisher-<version>-<purpose>` |
| Bundle index | `EvidenceBundle_Index.md` |
| Build evidence | `BuildEvidence.md` |
| Unit test evidence | `UnitTestEvidence.md` |
| Integration evidence | `IntegrationMockEvidence.md` |
| Diagnostic sample | `DiagnosticLogSample.redacted.jsonl` |
| Error matrix | `ErrorHandlingMatrix.md` |
| Retry summary | `RetryPolicySummary.md` |
| Redaction record | `RedactionReview.md` |
| Hold/resume record | `HoldResumeConditions.md` |

For a bundle that is prepared before package selection, use
`publisher-vNext` instead of inventing a release version.

## 7. Redaction Policy

Redaction is mandatory before a bundle is shared, attached to a vendor appeal,
or used outside the local repository review context.

Rules:

- prefer omission over masking when a value is not needed for the review;
- replace sensitive scalar values with stable placeholders such as
  `<REDACTED_TOKEN>`, `<REDACTED_LOCAL_PATH>`, `<REDACTED_DOCUMENT_URL>`, or
  `<REDACTED_ACCOUNT>`;
- retain hashes, version strings, target commit SHAs, exit codes, test counts,
  warning counts, and error counts when directly verified and non-sensitive;
- keep diagnostic messages classification-based and safe;
- never redact by modifying the source evidence file in a way that destroys
  the original local review record unless the original is prohibited from
  being stored;
- record a redaction review result before external sharing.

Required redaction review checks:

| Check | Expected Result |
| --- | --- |
| Token and credential scan | No token, credential, private key, bearer value, or cookie. |
| Local path scan | No local absolute path or user profile path. |
| URL scan | No private Google Docs/Drive URL or temporary public image URL. |
| Exception-body scan | No raw provider body, raw HTTP body, or stack trace payload. |
| Account scan | No personal account email, display name, or OAuth identity detail unless explicitly approved. |
| Evidence status scan | No inferred `PASS`; unexecuted work remains `PENDING`, `BLOCKED`, `DEFERRED`, `SKIPPED`, or `NOT EXECUTED`. |

## 8. Verification Checklist

Before an Evidence Bundle is marked `Ready for Review`, verify:

| Item | Result |
| --- | --- |
| Bundle metadata identifies version, target branch, target commit, and release state where applicable. | PENDING |
| Build evidence is present or explicitly marked not executed. | PENDING |
| Unit test evidence is present or explicitly marked not executed. | PENDING |
| Integration/mock evidence is present or explicitly marked not executed. | PENDING |
| Live E2E is recorded as PASS only when explicitly authorized, executed, and read back. | PENDING |
| Package evidence is recorded only when package inspection or creation was authorized. | PENDING |
| Diagnostic log sample is synthetic or redacted. | PENDING |
| Error handling matrix references the current specification. | PENDING |
| Retry policy summary references the current specification. | PENDING |
| Release runbook and checklist references are current. | PENDING |
| Hold and resume conditions preserve the current release block. | PENDING |
| Redaction review passed. | PENDING |
| No prohibited contents are present. | PENDING |
| No Frozen specifications, production code, public APIs, or `dist/` artifacts were changed to assemble the bundle. | PENDING |

## 9. Future Automation Candidates

Future automation may be proposed separately. It must preserve the same gate
separation and redaction rules.

Candidates:

- generate a bundle index from approved evidence files;
- collect safe command result summaries from local verification output;
- validate that no prohibited strings or patterns appear in bundle files;
- compare bundle status values against `CURRENT_STATUS.md`;
- produce a sanitized diagnostic log sample from allow-listed fields;
- verify that local-only evidence is not labeled as Live E2E or release
  readiness;
- package the redacted bundle as an archive after approval;
- create a vendor-appeal packet that includes only approved artifact identity
  and redacted evidence summaries.

Automation must not:

- execute Live E2E;
- mutate Google Docs or Google Drive;
- create or update packages;
- re-run flagged executables;
- submit files to vendors;
- stage, commit, push, tag, or publish;
- upload evidence outside the repository;
- inspect or print token-store or credential contents.

## 10. Current Documentation Update Scope

This specification was added as a docs-only / local-only design record. The
only expected companion updates are minimal synchronization entries in:

- `docs/development/CURRENT_STATUS.md`;
- `docs/development/Publisher_v1.0_Implementation_Voyage_Log.md`;
- `CHANGELOG.md`.

Those updates must preserve the current release hold and must not mark any
pending release, security, Live E2E, package, vendor, or publication condition
as passed.
