# Publisher P2-17 CHANGELOG Draft Helper Evaluation

Status  : Design complete / implementation decision pending
Scope   : Evaluate whether a future local-only helper can generate a candidate `CHANGELOG.md` bullet from existing release-note draft fields without editing `CHANGELOG.md`
Depends : docs/development/Publisher_P2-04_ReleaseNoteGenerationEvaluation.md, docs/development/Publisher_P2-08_CandidateSelection.md, docs/development/Publisher_P2-13_CandidateSelection.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, src/Publisher/Application/ReleaseNoteDraftAssembler.cs, tests/unit/Publisher/ReleaseNoteDraftAssemblerTests.cs

This is a design-only / local-only evaluation record. It does not implement a
CHANGELOG helper, edit `CHANGELOG.md`, generate or update release notes, change
production code, change tests, update package or `dist` artifacts, execute Live
E2E, mutate Google Docs or Google Drive, operate on OAuth or token stores,
operate on Avast, re-run flagged executables, create or update tags, publish
releases, decide vendor clearance, stage, commit, or push.

## 1. Purpose

P2-17 evaluates the remaining P2-04-E candidate after P2-04-A/B, P2-08,
P2-12, and P2-16 closeout. The question is narrow:

Can a future helper produce a draft `CHANGELOG.md` bullet from already
allow-listed Publisher release-note draft fields without directly editing
`CHANGELOG.md`, inferring approval, or changing release state?

The target output, if later implemented, would be draft text only. It would
reduce manual transcription effort, not replace human editorial review.

## 2. Current Findings

P2-04 already established the release-note generation boundary:

- release-note information must come from allow-listed source records;
- missing values become `NOT RECORDED`;
- conflicts become `CONFLICT` and block approval-ready output;
- current-state facts must not be inferred from historical records;
- release approval, authorization, risk acceptance, vendor clearance, and Avast
  safety certification remain manual-only.

The current implementation in `ReleaseNoteDraftAssembler` supports this
boundary by assembling deterministic draft fields from a manifest, excluding
sensitive values, rejecting unpermitted source kinds, and preserving source
references. Later P2-08 and P2-12 work reused the same boundary for drift
checking and verification evidence extraction.

No current Publisher helper was found that creates `CHANGELOG.md` bullets.
`CHANGELOG.md` remains manually curated.

## 3. Candidate Helper Shape

A future P2-17 implementation may be acceptable if it is limited to a
draft-only helper that:

- takes an existing release-note draft result or the same allow-listed manifest
  and source records;
- emits a candidate bullet to stdout, a temporary path, or an explicitly named
  draft artifact outside `CHANGELOG.md`;
- includes source references or diagnostics sufficient for review;
- refuses approval-ready output when required fields are `NOT RECORDED`,
  `CONFLICT`, `MANUAL ONLY`, sensitive, historical-only, or unmanifested;
- uses a fixed, small template rather than broad Markdown scraping or editorial
  generation;
- labels the result as draft / derived documentation only.

The helper should not update `CHANGELOG.md` automatically. Any actual
`CHANGELOG.md` edit must remain a separate documentation task.

## 4. Safe-Value Boundary

Allowed values:

- explicit allow-listed release-note draft fields;
- bounded status values such as `NOT RECORDED`, `CONFLICT`, `MANUAL ONLY`, and
  draft diagnostics;
- checked-in source record paths and source references;
- non-sensitive version, tag, package identity, release URL, verification
  counts, and evidence references only when explicitly recorded by permitted
  current-state sources.

Prohibited values:

- credentials, tokens, Authorization headers, token-store paths, credential
  paths, private URLs, local absolute paths, provider payloads, raw HTTP
  bodies, raw exceptions, stack traces, account identifiers, hostnames, raw
  Google resource IDs, and document content;
- inferred approval, authorization, publication, risk acceptance, vendor
  clearance, Avast false-positive resolution, or Avast safety certification.

## 5. GO / NO-GO

Future implementation recommendation: GO only for a separate narrow local-only
implementation task.

GO conditions:

- reuse the existing P2-04 allow-list / manifest / draft-result boundary;
- emit draft output only and never edit `CHANGELOG.md`;
- fail closed on missing, conflicting, manual-only, sensitive, historical-only,
  or unmanifested fields;
- preserve source references and diagnostics;
- include focused unit coverage under the existing ReleaseNote test area;
- preserve Frozen specifications, public APIs, persisted schemas, release
  records, package identity, publication state, Google/OAuth state, vendor
  state, and Avast state.

NO-GO if the task requires:

- editing `CHANGELOG.md` as part of the helper;
- broad Markdown scraping or editorial summarization;
- inferring current state from historical records;
- inferring or changing release approval, release authorization, publication
  authorization, risk acceptance, vendor clearance, Avast false-positive
  disposition, or Avast safety certification;
- package or `dist` mutation, release, tag, publication, GitHub asset, Live
  E2E, Google Docs / Drive, OAuth/token-store, Avast, flagged-executable,
  stage, commit, or push operations;
- public API, persisted schema, dependency, stdout compatibility, or release
  record changes beyond a separately authorized scope.

## 6. Verification Plan For Future Implementation

Required local-only verification for a future implementation:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~ReleaseNote"
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
dotnet build VMF.Publisher.sln --configuration Release --no-restore
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
git status --short --branch
```

Do not run Live E2E, Google Docs or Google Drive operations, OAuth/token-store
operations, package generation, package verification, Avast, flagged
executables, release, tag, publication, stage, commit, or push for this design
evaluation or any future local-only helper implementation.

## 7. Design Decision

Decision: design complete.

P2-17 is a valid future implementation candidate only as a draft-only,
local-only helper. It should not be treated as part of P2 closeout, as release
evidence, or as authorization to edit `CHANGELOG.md`.

Recommended next step, if implementation is desired later: create a separately
authorized P2 implementation task that adds a small internal helper and focused
ReleaseNote unit tests while preserving the non-editing, non-authorization
boundary above.
