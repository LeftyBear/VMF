# Publisher P2-04 Release-Note Generation Evaluation

Status  : COMPLETE / P2-04-A and P2-04-B implemented / P2-04-C-D-E deferred
Scope   : Evaluate safe release-note generation from existing verification and evidence records
Depends : CHANGELOG.md, docs/releases/README.md, docs/releases/Publisher_0.0.1-dev_ReleaseNotes.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_PostReleaseEvidenceSummaryTemplate.md, docs/development/Publisher_vNext_Backlog.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md, docs/architecture/ADR-0016-release-versioning-tag-and-artifact-identity.md

This record closes P2-04 after a narrow local-only implementation of P2-04-A
and P2-04-B. It does not generate release notes, update release notes, update
`CHANGELOG.md`, create or update packages or `dist`, execute Live E2E, mutate
Google Docs or Google Drive, operate on OAuth or token stores, operate on
Avast, re-run flagged executables, create or update tags, publish releases,
stage, commit, or push.

Generated release-note content, if implemented later, must remain derived
documentation. It must not be treated as release approval, release
authorization, publication authorization, risk acceptance, vendor clearance,
Avast safety certification, or proof that a gated operation occurred.

## 1. Purpose

The purpose of P2-04 is to evaluate whether release-note information can be
safely and consistently generated from existing Publisher verification,
evidence, release-control, and current-state records.

The target outcome is reduced documentation drift and fewer manual
transcription errors. The target is not a release-path operation and not a
state-promotion mechanism.

## 2. Scope

Allowed design scope:

- review current Publisher release notes, `CHANGELOG.md`, verification
  records, evidence records, and relevant runbook or template records;
- identify candidate source-of-truth records and fields;
- separate historical records from current-state records;
- define generated, derived, and manually approved content responsibilities;
- identify fields that must never be inferred automatically;
- prioritize safe generation design candidates;
- define acceptance criteria and a local-only verification plan;
- decide whether a later narrow implementation should proceed.

Allowed future implementation scope, if separately authorized:

- local-only tooling that reads repository Markdown records and emits a draft
  release-note artifact or dry-run diff;
- tests that use local fixtures only;
- documentation updates that record the generation boundary and output
  contract.

## 3. Non-Scope

P2-04 does not authorize:

- release approval, release authorization, publication authorization, risk
  acceptance, or vendor-clearance decisions;
- new release notes for an active publication path;
- edits to existing release records unless a separate docs task authorizes
  them;
- package generation, package verification, `dist` writes, tag operations,
  GitHub Release operations, or asset operations;
- Live E2E;
- Google Docs or Google Drive mutation;
- OAuth, credential, or token-store operation;
- Avast operation, flagged executable re-run, vendor-clearance judgment, or
  Avast safety-certification claim;
- production code, public API, persisted schema, canonical format, or Frozen
  specification changes;
- stage, commit, or push.

## 4. Current Records And Existing Generation

Current Publisher release-note records include:

- `docs/releases/Publisher_0.0.1-dev_ReleaseNotes.md`;
- older Publisher Phase 3 release-note records under `docs/releases/`;
- `docs/releases/README.md` as the release-report index;
- `CHANGELOG.md` as the repository-wide change log.

Current Publisher source records include:

- current-state records such as `docs/development/CURRENT_STATUS.md`;
- release-control records such as `Publisher_ReleaseApprovalPackage.md`,
  release execution gate records, operation-specific authorization records,
  release completion records, and release identity reconciliation records;
- evidence and verification records under `docs/development/`,
  `docs/distribution/`, and `docs/evidence/publisher/`;
- ADRs that define evidence, publication, identity, authorization, and
  vendor-clearance boundaries.

Existing generation tooling was found under `tools/release/`, but it generates
Build release inventory documents from VBA source declarations. No current
Publisher tool was found that generates release notes from Publisher
verification or evidence records.

## 5. Source-Of-Truth Candidates

Preferred current-state sources:

- `CURRENT_STATUS.md` for the formal current Publisher state and active
  external gate boundaries;
- the latest release identity reconciliation / completion records for version,
  tag, target commit, package identity, asset identity, digest, and publication
  identity after publication is complete;
- `Publisher_ReleaseApprovalPackage.md` for indexed evidence references and
  explicit approval-boundary wording;
- `docs/releases/README.md` for release-note index placement;
- `CHANGELOG.md` for manually curated repository change summaries.

Candidate generated fields:

- release version, tag, runtime, configuration, package type, asset name,
  package path, package size, and SHA-256 when explicitly recorded;
- target commit, annotated tag object, evidence docs commit, GitHub Release
  URL, prerelease flag, and remote/local digest match when explicitly
  recorded;
- directly recorded verification result rows, including command result,
  warnings, errors, passed, failed, and skipped counts;
- evidence-reference lists and document paths;
- explicit non-action boundary lists when copied from current records.

Fields that may be derived only mechanically:

- a table of evidence references from allow-listed record paths;
- a release-note draft section order from a fixed template;
- a missing-field checklist that reports `PENDING`, `NOT RECORDED`, or
  `NOT FOUND` instead of filling gaps.

## 6. Historical And Current-State Boundary

Generation must treat each source record as either historical evidence,
current-state evidence, template text, or generated draft input.

Rules:

- historical `Hold`, `blocked`, `pending`, `NO-GO`, superseded tag, old size,
  or old digest text must remain historical unless a current-state record
  explicitly promotes or supersedes it;
- current-state records must not erase historical records;
- generated notes must name the record that supplied each current value;
- when current and historical values conflict, generation must stop or mark the
  field `CONFLICT` rather than choosing silently;
- release-note drafts must preserve superseded identity facts as historical
  only when an explicit reconciliation record says so.

## 7. Responsibility Split

Generated information:

- deterministic formatting from an approved template;
- copied allow-listed fields from explicit records;
- evidence-reference inventory;
- missing-field and conflict report.

Derived information:

- draft section ordering;
- direct rollups such as `unit tests: 492 passed / 0 failed / 0 skipped` only
  when those numbers are present in the source record;
- status labels copied from current records, not inferred from nearby text.

Manually approved information:

- release approval;
- release authorization;
- publication authorization;
- risk acceptance;
- vendor-clearance acceptance;
- Avast safety certification;
- current release basis;
- decision to publish, replace, withdraw, rollback, or announce;
- any wording that changes external gate state.

## 8. Non-Inferable Items

A generator must never infer these from tests, local evidence, silence, or
successful draft generation:

- release approval;
- release authorization;
- publication authorization;
- package approval;
- tag approval;
- risk acceptance;
- vendor clearance;
- Avast false-positive resolution;
- Avast safety certification;
- Live E2E authorization;
- Google Docs / Drive authorization;
- OAuth/token-store authorization;
- package, `dist`, release, tag, or asset mutation permission;
- current state from historical accepted-at-the-time records alone.

## 9. Design Candidates

### P2-04-A: Draft-Only Release-Note Assembler

Create a local tool that reads an allow-listed set of Markdown records and
emits a draft release-note Markdown file to a temporary or explicitly named
draft path. The output must include source references and a non-authorization
boundary section.

Priority: P1.

Rationale: Highest drift-reduction value while keeping generated output clearly
separate from approved release notes.

### P2-04-B: Source Field Manifest

Define a small checked-in manifest that maps release-note fields to approved
source records and headings. Missing fields become `NOT RECORDED`; conflicting
fields become `CONFLICT`.

Priority: P1.

Rationale: Prevents broad Markdown scraping and makes source-of-truth choices
reviewable before any generation occurs.

### P2-04-C: Verification Evidence Extractor

Extract verification result tables from allow-listed records into a normalized
draft table.

Priority: P2.

Rationale: Useful, but table variation across old records requires careful
fixture coverage before relying on it.

### P2-04-D: Release-Note Drift Checker

Compare an existing release-note file against the allow-listed current-state
records and report mismatches without editing files.

Priority: P2.

Rationale: Safer than generation for existing published records and useful for
review. It should not rewrite release notes automatically.

### P2-04-E: CHANGELOG Draft Helper

Generate a candidate `CHANGELOG.md` bullet from approved release-note draft
fields.

Priority: P3 / defer.

Rationale: Changelog wording is broader and more editorial than release-note
identity fields. It should remain manual until the release-note generator
boundary is proven.

## 10. Recommended Design

Proceed later with P2-04-A and P2-04-B only.

The first implementation should be local-only, draft-only, and fail-closed:

1. read only a small allow-listed manifest of source records;
2. produce a draft release-note artifact or dry-run stdout report, not an
   approved release note;
3. copy only explicit values from source records;
4. mark missing data as `NOT RECORDED`;
5. mark inconsistent data as `CONFLICT` and return a non-zero exit code;
6. include source references for generated sections;
7. always include a boundary section stating that generation is not approval,
   authorization, publication, risk acceptance, vendor clearance, or Avast
   safety certification;
8. avoid credentials, token-store paths, private URLs, local absolute paths,
   raw exceptions, HTTP bodies, provider payloads, stack traces, and other
   sensitive values.

Defer P2-04-C until source table shapes are fixture-tested. Defer P2-04-D
until the draft assembler exists. Defer P2-04-E until manually curated
release-note generation is proven stable.

## 11. Acceptance Criteria For Future Implementation

Future P2-04 implementation is acceptable only when:

- it is local-only and does not mutate external services;
- it does not edit approved release notes unless a later docs task explicitly
  authorizes that edit;
- all source records are allow-listed and reviewable;
- every generated current-state field cites an explicit source record;
- historical records are not promoted to current state unless an explicit
  current-state or reconciliation record says so;
- missing values are reported as `NOT RECORDED`;
- conflicts are reported as `CONFLICT` and stop approval-ready output;
- generated output states that it is draft / derived documentation only;
- release approval, release authorization, publication authorization, risk
  acceptance, vendor clearance, and Avast safety certification remain
  manually approved information only;
- sensitive values and local absolute paths are excluded;
- focused tests cover normal generation, missing fields, conflicting current
  and historical identities, superseded identity handling, non-inferable gate
  fields, and sensitive-value exclusion;
- no Frozen specifications, public APIs, persisted schemas, package outputs,
  `dist/` contents, release assets, Google state, OAuth/token-store state,
  Avast state, staging, commit, or push are changed.

## 12. Local-Only Verification Plan

Required verification for a future implementation:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~ReleaseNote"
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
dotnet build VMF.Publisher.sln --configuration Release --no-restore
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
git status --short --branch
```

If the implementation is PowerShell-only, add focused Pester or script-level
fixture checks before the repository-wide checks. Do not run Live E2E, Google
Docs / Drive operations, OAuth/token-store operations, package generation,
package verification, Avast, flagged executables, release, tag, publication,
stage, commit, or push.

## 13. Implementation GO/NO-GO

Decision: COMPLETE for P2-04.

P2-04-A and P2-04-B were implemented as a narrow local-only release-note draft
assembler and source-field manifest boundary under `src/Publisher/Application`
with focused unit coverage under `tests/unit/Publisher`.

P2-04-C, P2-04-D, and P2-04-E remain deferred / unimplemented:

- P2-04-C remains deferred until verification evidence table shapes are
  fixture-tested.
- P2-04-D remains deferred until a separate drift-checker task is authorized.
- P2-04-E remains deferred because `CHANGELOG.md` wording remains manually
  curated.

The implementation did not generate or update release notes, update
`CHANGELOG.md`, reopen the existing `0.0.1-dev` release, update package or
`dist` output, perform release, tag, publication, GitHub asset, Live E2E,
Google Docs / Drive, OAuth/token-store, Avast, flagged executable, vendor
clearance, stage, commit, or push operations.

## 14. Verification Result

Recorded P2-04 closeout verification:

- Focused unit coverage: `ReleaseNoteDraftAssemblerTests` covers allow-listed
  current-state fields, missing fields, conflicting current-state values,
  historical non-promotion, manual-only gate fields, sensitive-value
  exclusion, unmanifested fields, and source-kind rejection.
- Local-only behavior: no external service, OAuth, token-store, package,
  release, tag, publication, Avast, stage, commit, or push operation was
  required for the P2-04-A/B implementation.
- Actual release-note and `CHANGELOG.md` generation remains not performed.

No P2-04-C, P2-04-D, or P2-04-E implementation verification is recorded
because those candidates remain deferred.

## 15. Previous Implementation GO Criteria

The implementation proceeded under the earlier GO criteria:

Conditions for GO:

- implementation is draft-only and allow-list driven;
- generated output cannot become approval-ready when required fields are
  missing or conflicting;
- current-state values require explicit source records;
- non-inferable release, publication, risk, and vendor-clearance fields remain
  manual-only;
- no external service, package, release, tag, publication, Avast, OAuth, or
  token-store operation is needed.

Former NO-GO items preserved as ongoing exclusions:

- do not generate or update release notes from this design task;
- do not proceed if generation requires broad Markdown scraping, implicit
  state promotion, external state lookup, release asset inspection, package or
  `dist` mutation, vendor-clearance judgment, or authorization inference.
