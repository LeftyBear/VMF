# Publisher P2-01 Google Picker Drive File Evaluation

Status  : Design complete / implementation decision pending
Scope   : Evaluate whether Google Picker plus `drive.file` least-privilege routing should be adopted by a future scoped design task
Depends : docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/architecture/ADR-0010-vnext-backlog-and-deferred-scope-boundary.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, docs/distribution/LiveE2EOperations.md, docs/distribution/PublisherReleaseRunbook.md, specs/publisher/Publisher_v1.0_Architecture_Specification.md, src/Publisher/Infrastructure/Google/OAuthDesktopGoogleCredentialProvider.cs, src/Publisher/Infrastructure/Google/ServiceAccountGoogleCredentialProvider.cs, src/Publisher/Infrastructure/Google/GoogleDriveClient.cs, src/Publisher/Infrastructure/Google/GoogleDriveTemporaryImageHost.cs, src/Publisher/Infrastructure/Google/GooglePublisherOptions.cs, src/Publisher.Cli/Program.cs

This is a design-only evaluation record. It does not adopt Google Picker,
change OAuth scopes, perform OAuth login or consent, inspect or mutate token
stores, call Google Docs or Google Drive APIs, run Live E2E, change
production code, change tests, update package or `dist` artifacts, reopen the
existing `0.0.1-dev` release, decide vendor clearance, stage, commit, or push.

## 1. Purpose

P2-01 evaluates whether a future Publisher design should replace or supplement
the current broad OAuth Desktop Drive scope with a Google Picker plus
`drive.file` least-privilege routing model.

The goal is future operator-risk reduction. The evaluation does not change the
current authentication decision recorded by ADR-0002 and does not approve an
implementation.

## 2. Scope

Allowed scope:

- review ADR-0002, ADR-0010, related safety ADRs, Live E2E guidance, release
  runbook boundaries, Publisher architecture requirements, and current Google
  authentication / Drive implementation;
- compare the current broad Drive scope model with a possible Picker-mediated
  `drive.file` model;
- identify implementation, authorization, verification, and documentation
  impacts for a later scoped design;
- define GO / NO-GO conditions for a future design task.

## 3. Non-Scope

P2-01 does not authorize:

- adopting Google Picker, `drive.file`, new OAuth scopes, or a new
  authentication architecture;
- OAuth login, browser consent, reauthorization, token refresh, token-store
  creation, token-store reuse, token-store deletion, token-store cleanup, or
  token-store inspection;
- Google Docs mutation, Google Drive mutation, Drive readback, cleanup, Live
  E2E, temporary public image hosting, package or `dist` update, release, tag,
  publication, GitHub asset operation, Avast operation, flagged executable
  re-run, vendor-clearance judgment, stage, commit, or push;
- changing Frozen specifications, public APIs, persisted schemas, canonical
  formats, release records, or existing tests.

## 4. Current Findings

ADR-0002 records OAuth Desktop as the preferred local operator mode for
personal Gmail and My Drive workflows. It explicitly keeps the current OAuth
Desktop scopes as Google Documents plus full Google Drive and defers Google
Picker plus `drive.file` to vNext.

ADR-0010 records the backlog boundary: P2 labels are planning labels, not
implementation approval, release authorization, Google mutation approval, or
adoption of vNext behavior. It also names Google Picker plus `drive.file` as a
vNext reconsideration candidate only.

Live E2E guidance preserves ADR-0002: OAuth Desktop remains preferred for
personal Gmail / My Drive workflows, Service Account remains available for
automation or prepared Shared Drive workflows, Documents and Drive scopes
remain unchanged, and Google Picker plus `drive.file` remains deferred.

Current implementation findings:

- `OAuthDesktopGoogleCredentialProvider` requests
  `https://www.googleapis.com/auth/documents` and
  `https://www.googleapis.com/auth/drive`.
- `ServiceAccountGoogleCredentialProvider` requests Documents plus
  `drive.file`.
- `GoogleDriveClient.CreateDocumentAsync` creates a native Google Doc through
  Drive and optionally sets a configured folder as `parents`.
- `GoogleDriveTemporaryImageHost` can upload temporary image files, grant
  public reader permission, and delete temporary files when temporary public
  image hosting is separately authorized.
- CLI configuration currently models authentication mode, credential path,
  token-store path, destination folder ID, and temporary image folder ID. It
  does not model Picker-selected resource grants, per-file consent, selected
  document identity, selected folder identity, or Picker UI state.

## 5. Design Implications

Google Picker plus `drive.file` is not a drop-in scope reduction for the
current OAuth Desktop implementation.

A future design would need to decide how operators select or create the target
document and destination folder, how the selected file or folder identity is
passed into the CLI, and how that selection remains reviewable without
recording private Google URLs, account identifiers, resource IDs, tokens, or
provider payloads in logs or evidence.

The current `drive.file` service-account scope does not prove that OAuth
Desktop can simply switch to `drive.file`. OAuth Desktop plus Picker would
change the operator selection flow and could change Drive-access semantics for
folder placement, document creation, Shared Drive behavior, temporary image
hosting, cleanup, and repeat runs using a persisted token store.

Temporary image hosting is the highest-friction current behavior for a pure
least-privilege model. It creates Drive files, changes permissions, exposes
temporary public URLs for Google Docs insertion, and later deletes those files.
Any future `drive.file` design must either prove this remains supported under
the selected grant model, split image hosting into a separate authorization
path, or keep temporary image hosting outside the Picker-based mode.

## 6. Security Boundary

The main benefit of a Picker plus `drive.file` route is potential reduction of
operator Drive-access blast radius for personal Gmail workflows. The
tradeoff is a more complex consent and resource-selection lifecycle that must
not weaken existing safety boundaries.

A future design must preserve:

- explicit operation-specific authorization for OAuth, token-store, Google
  Docs / Drive, Live E2E, cleanup, package, release, and vendor-clearance
  gates;
- ADR-0006 safe observability: no credentials, tokens, Authorization headers,
  credential paths, token-store paths, private URLs, raw Google resource IDs,
  provider payloads, raw HTTP bodies, raw exceptions, stack traces, local
  sensitive paths, account identifiers, or temporary public URLs in logs or
  evidence;
- ADR-0007 safe user-facing messages and stable classification behavior;
- ADR-0004 Verified State and readback safety semantics;
- a clear distinction between Drive access reduction, release authorization,
  publication success, package approval, vendor clearance, and Avast safety
  certification.

## 7. Design Options

Option A: keep ADR-0002 unchanged.

This is the current baseline. It keeps OAuth Desktop on Documents plus full
Drive and preserves current CLI and Live E2E behavior. It avoids new UI,
resource-selection, and cleanup complexity, but leaves the broader Drive scope
in place.

Option B: add a future Picker-mediated OAuth Desktop mode.

This would introduce a new explicit mode or capability where operators select
target resources through Picker and the OAuth flow uses Documents plus
`drive.file` where feasible. This option should require a new ADR or scoped
design because it changes the authentication and Drive-access model.

Option C: split least-privilege by workflow.

This would keep the current mode for workflows that require folder creation,
Shared Drive compatibility, or temporary image hosting, while adding a
Picker-based path for narrower workflows that operate only on explicitly
selected documents or files. This may reduce risk for some operations without
forcing unsupported behavior into all workflows.

Option D: defer adoption but improve documentation.

This would keep the current implementation unchanged and add only future
operator guidance that explains why Picker plus `drive.file` remains deferred,
what evidence is required to reconsider it, and which workflows are likely to
be affected.

## 8. Recommended Design Direction

Recommendation: GO for a later narrow design task, NO-GO for immediate
implementation.

The preferred future path is Option C: evaluate a split least-privilege design
that can support a Picker-based route for resource-selected workflows while
preserving the existing full-Drive route for workflows that still require it.

The later design task should answer these questions before implementation:

- whether the Picker route targets existing documents only, folder selection,
  document creation, or all three;
- whether temporary image hosting is supported, separately gated, or excluded
  from the Picker route;
- how selected resource identity is represented without exposing private
  values in logs, evidence, release records, or tests;
- whether OAuth Desktop token-store reuse remains valid after scope changes
  or requires explicit reauthorization and token-store lifecycle handling;
- whether Service Account behavior remains unchanged;
- which tests can be local-only and which, if any, require separately
  authorized Live E2E.

## 9. Acceptance Criteria For Future Design

A future scoped design is acceptable only if it:

- records the adoption decision in a new ADR or equivalent approved design
  record before implementation;
- preserves ADR-0002 until explicitly superseded;
- keeps current behavior unchanged unless a new implementation task is
  separately approved;
- defines exact supported workflows for Picker / `drive.file`, including
  document creation, existing document update, folder selection, Shared Drive,
  temporary image hosting, cleanup, and repeated token-store reuse;
- separates OAuth consent, token-store lifecycle, Google Docs mutation,
  Google Drive mutation, Live E2E, cleanup, release, package, and vendor gates;
- preserves Verified State, readback verification, revision-conflict, safe
  logging, and safe error-message contracts;
- introduces no dependency, UI surface, public API, persisted schema, or
  configuration change without explicit implementation approval;
- includes a local-only verification plan and identifies any unavoidable Live
  E2E as separately authorized work.

## 10. Local-Only Verification Plan

Required verification for a future design-only or docs-only follow-up:

```powershell
git diff -- docs/development/Publisher_P2-01_GooglePickerDriveFileEvaluation.md docs/development/Publisher_vNext_Backlog.md
git diff --check
git status --short --branch
```

Required verification for a future implementation must be defined in that
future task. At minimum it should include focused OAuth provider, Google Drive
client, temporary image hosting, CLI configuration, safe diagnostics, and
Live E2E gating tests, followed by the Publisher solution build/test/format
checks required by the repository playbook.

Do not run OAuth login, browser consent, token-store inspection, token-store
cleanup, Google Docs or Google Drive operations, Live E2E, package or `dist`
operations, release, tag, publication, GitHub asset operation, Avast,
flagged executable, stage, commit, or push for this design evaluation.

## 11. Implementation GO/NO-GO

Immediate implementation recommendation: NO-GO.

Reason: the least-privilege route affects authentication scopes, operator
resource selection, configuration, Drive creation / folder behavior,
temporary image hosting, token-store lifecycle, documentation, and Live E2E
authorization. Those impacts require a future scoped design and adoption
record before code changes.

Future design recommendation: GO, limited to a new scoped task that evaluates
Option C and explicitly preserves all current release, Google, OAuth,
token-store, package, `dist`, vendor, stage, commit, and push exclusions until
separately authorized.
