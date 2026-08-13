# Publisher P2-06 Managed-Document Readback Reporting Evaluation

Status  : Design complete / implementation decision pending
Scope   : Evaluate safer, clearer operator-facing reporting for managed-document readback verification
Depends : docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, docs/development/Publisher_v1.0_Implementation_Voyage_Log.md, docs/development/Test_Traceability_Matrix.md, src/Publisher/Application/PhysicalUpdateApplicationVerifier.cs, src/Publisher/Application/PhysicalUpdateApplicationService.cs, src/Publisher.Cli/Program.cs, tests/unit/Publisher/PhysicalUpdateApplicationVerifierTests.cs, tests/unit/Publisher/PhysicalUpdateLifecycleTests.cs, tests/unit/Publisher/PublishStatePromotionTests.cs

This is a design-only record. It does not execute managed-document readback,
mutate Google Docs or Google Drive, perform OAuth or token-store operations,
run Live E2E, update package or `dist` artifacts, reopen the existing
`0.0.1-dev` release, create release notes, decide vendor clearance, stage,
commit, or push.

## 1. Purpose

P2-06 evaluates whether vNext should make managed-document readback reporting
clearer for operators without changing the Verified State or Readback
Verification safety semantics.

The goal is reporting clarity only: help an operator distinguish verified
readback, readback failure, readback mismatch, not-run states, local dry-run
non-applicability, and release/vendor authorization boundaries.

## 2. Scope

Allowed scope:

- review ADR-0004, ADR-0006, ADR-0007, Verified State, Readback Verification,
  Physical Update Plan ordering, implementation, tests, and current docs;
- summarize current diagnostics and operator-facing output;
- identify currently expressible and ambiguous states;
- define safe reporting boundaries and prioritized reporting improvements;
- define acceptance criteria and local-only verification for a future narrow
  implementation.

## 3. Non-Scope

P2-06 does not authorize:

- actual Google readback, Google Docs mutation, Google Drive mutation, OAuth
  login, token-store read/write/delete/cleanup/reuse, Live E2E, package or
  `dist` update, release, tag, publication, GitHub asset operation, Avast
  operation, flagged executable re-run, vendor-clearance judgment, stage,
  commit, or push;
- changing Physical Update Plan ordering or meaning;
- changing the requirement that Verified State promotion/save occurs only
  after successful readback verification;
- promoting failure, mismatch, unknown delivery, skipped readback, dry-run, or
  non-applicable states to success;
- exposing raw document content, private document IDs, private URLs, provider
  payloads, raw exceptions, stack traces, credentials, tokens, token-store
  paths, or local sensitive paths.

## 4. Current Findings

ADR-0004 fixes the safety contract: load Verified State, validate snapshot,
create the logical and physical plans, re-read before apply, apply destructive
operations before constructive operations, read back after apply, verify the
candidate identity, fingerprint, block count, block order, managed region,
block ranges, block identities, and hashes, then promote and atomically save
Verified State only after readback verification succeeds.

The implementation preserves that contract. `PhysicalUpdateApplicationVerifier`
wraps snapshot-read failures as `UPDATE_READBACK_FAILED`, rejects revision
conflicts, rejects mismatched document identity, fingerprint, block count,
block order, content hash, and managed-region topology, and returns
`IsReadbackVerified=true` only after complete verification. The application
service saves Verified State only after readback evidence is verified and
promoted. Tests cover successful readback, revision conflicts, readback
failure, readback mismatch, managed-region mismatch, no-change verification,
and failure paths that do not save state.

Current CLI error handling can express readback failures through stable error
codes and the `Verification` classification / exit code `4`. Current dry-run
diagnostics explicitly say Google Docs / Drive mutation, readback
verification, Verified State save, publication approval, release clearance,
and vendor clearance were not attempted.

## 5. Current State Vocabulary

Clearly expressible today:

- `verified`: readback matched candidate evidence and `IsReadbackVerified` is
  true before Verified State promotion.
- `failed`: readback could not be obtained and maps to
  `UPDATE_READBACK_FAILED`.
- `mismatch`: readback was obtained but did not match candidate or managed
  region evidence and maps to `UPDATE_READBACK_MISMATCH` or
  `UPDATE_MANAGED_REGION_MISMATCH`.
- `revision-conflict`: revision checks failed and map to
  `UPDATE_REVISION_CONFLICT`.
- `not-attempted`: dry-run and local-only diagnostics can state that readback
  was not attempted.

Ambiguous or under-reported today:

- whether a successful publish path had readback verified versus merely
  completed an operation, unless the reader traces implementation evidence;
- whether no-change / empty physical plan completed because verification
  proved the state or because no readback was applicable;
- whether failure happened before apply, during apply, during readback, or
  during Verified State promotion/save;
- whether readback evidence is local/non-live, Live E2E, or real publication
  evidence;
- whether readback evidence is being incorrectly interpreted as publication
  success, release authorization, package approval, vendor clearance, or Avast
  safety certification.

## 6. Safe Reporting Boundary

A future reporting improvement may emit only value-safe status fields:

- bounded status labels such as `verified`, `failed`, `mismatch`,
  `revision-conflict`, `not-attempted`, `not-applicable`, and `blocked`;
- stable error codes and CLI classification;
- boolean boundary fields such as `readbackVerified`,
  `verifiedStateSaved`, `googleDocsMutation`, `publicationAuthorized`,
  `releaseClearance`, and `vendorClearance`;
- counts that do not disclose content, such as expected and observed block
  count;
- named lifecycle phase labels such as `pre-apply-read`, `apply`,
  `post-apply-readback`, and `verified-state-save`.

It must not emit raw document content, block text, document IDs, private
Google resource IDs, private URLs, OAuth tokens, credential paths,
token-store paths, Authorization headers, provider payloads, raw HTTP bodies,
raw exception messages, stack traces, local sensitive paths, usernames,
hostnames, or account identifiers.

Readback reporting is evidence of managed-document verification only. It is
not publication approval, release clearance, package approval, vendor
clearance, Avast safety certification, or authorization for future Google,
OAuth, package, release, or vendor operations.

## 7. Recommended Design

Priority P1:

- Add a compact readback status payload to local structured diagnostics and
  any operator-facing summary that already reports managed-document update
  outcomes.
- Use a closed status vocabulary:
  `verified`, `failed`, `mismatch`, `revision-conflict`, `not-attempted`,
  `not-applicable`, and `blocked`.
- Include phase labels that separate pre-apply read, physical apply,
  post-apply readback, verification, promotion, and save without exposing
  document content or identifiers.
- Preserve existing stable error codes and exit-code classifications.

Priority P2:

- Add explicit no-change / empty-plan wording that says verification was still
  required and completed, or that readback was not attempted because the mode
  was local dry-run.
- Add docs guidance explaining that readback verification is not release
  clearance, publication authorization, vendor clearance, or Avast safety
  certification.

Priority P3 / defer:

- Add richer mismatch diagnostics only if they remain value-safe, bounded, and
  tested with synthetic data. Do not include block text, document IDs, private
  URLs, or provider payloads.

## 8. Acceptance Criteria

A future narrow implementation is acceptable only if:

- it preserves revision conflict hard stops and the existing Physical Update
  Plan order and meaning;
- it preserves the rule that Verified State promotion/save is possible only
  after successful readback verification;
- it does not change public APIs, persisted schemas, Frozen specifications,
  OAuth scopes, authentication architecture, or release records;
- failure, mismatch, unknown delivery, skipped readback, dry-run, and
  non-applicable states cannot be reported as success;
- readback status is separated from publication success, release clearance,
  package approval, vendor clearance, and Avast safety certification;
- all new fields are value-safe and covered by ADR-0006 / ADR-0007 safe
  diagnostics and safe-message rules;
- unit tests cover success, readback failure, readback mismatch,
  revision-conflict, no-change, dry-run not-attempted, and sensitive-value
  exclusion.

## 9. Local-Only Verification Plan

Required verification for a future implementation:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --filter "FullyQualifiedName~PhysicalUpdateApplicationVerifierTests|FullyQualifiedName~PhysicalUpdateLifecycleTests|FullyQualifiedName~PublishStatePromotionTests|FullyQualifiedName~CliApplicationTests"
dotnet test VMF.Publisher.sln --configuration Release
dotnet build VMF.Publisher.sln --configuration Release
dotnet format VMF.Publisher.sln --verify-no-changes
git diff --check
git status --short --branch
```

Also run a targeted changed-file scan for private document IDs, private URLs,
OAuth tokens, Authorization headers, credential paths, token-store paths, raw
provider payloads, raw exception details, stack traces, and local sensitive
paths. Do not inspect credential files or token-store directories.

## 10. Implementation GO/NO-GO

Recommendation: GO for a later narrow local-only implementation after this
design record is reviewed.

Conditions for GO:

- implement only reporting/diagnostic wording and tests;
- keep the status vocabulary closed and value-safe;
- preserve ADR-0004 readback and Verified State semantics;
- preserve ADR-0006 / ADR-0007 safe diagnostics and CLI classification;
- do not perform actual Google readback, Google Docs / Drive mutation,
  OAuth/token-store operation, Live E2E, package or `dist` update, release,
  tag, publication, GitHub asset operation, Avast operation, flagged
  executable re-run, vendor-clearance judgment, stage, commit, or push.

NO-GO if implementation would require changing readback semantics, weakening
Verified State promotion requirements, exposing sensitive document/provider
values, or treating readback evidence as release or vendor clearance.
