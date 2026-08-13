# Publisher P2-05 OAuth Desktop Token-Store Documentation Evaluation

Status  : Complete / docs-only implementation synchronized
Scope   : Evaluate documentation improvements for OAuth Desktop setup and token-store handling
Depends : docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/distribution/InstallationGuide.md, docs/distribution/LiveE2EOperations.md, docs/distribution/PublisherReleaseRunbook.md, docs/distribution/ReleaseChecklist.md, docs/architecture/ADR-0005-retry-policy-and-failure-classification.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, src/Publisher.Cli/appsettings.Local.json.example

This is a docs-only design and closeout record. It does not execute OAuth
login or consent, generate, read, inspect, delete, or modify credential files,
generate, read, inspect, delete, or modify token stores, operate Google Docs
or Google Drive, change OAuth scopes, change authentication architecture,
update packages or `dist`, reopen a release, create or update release notes,
stage, commit, or push.

## 1. Purpose

P2-05 evaluates where Publisher documentation can better explain OAuth Desktop
setup and token-store handling so operators are less likely to place
credentials in the repository, confuse OAuth Desktop and Service Account
responsibilities, or treat token-store operations as ordinary docs work.

The goal is operator-risk reduction through documentation only. It is not a
credentialed Google operation and not an authentication redesign.

## 2. Scope

Allowed scope:

- review ADR-0002, installation guidance, Live E2E guidance, release runbook,
  release checklist, safe observability, retry/failure classification, and the
  local appsettings example;
- identify setup steps that are correct but too implicit for a first-time
  operator;
- preserve the existing OAuth Desktop and Service Account responsibility
  boundary;
- preserve the existing Documents and Drive OAuth scope decision;
- preserve the requirement that credentials, token stores, local sensitive
  paths, private URLs, OAuth tokens, Authorization headers, provider payloads,
  raw exceptions, and credentialed evidence stay out of repository docs,
  packages, logs, and evidence bundles;
- define prioritized documentation changes, acceptance criteria, and a
  docs-only verification plan.

## 3. Non-Scope

P2-05 does not authorize:

- OAuth login, browser consent, reauthorization, token refresh, token-store
  creation, token-store reuse, token-store deletion, token-store cleanup, or
  token-store inspection;
- credential file generation, credential rotation, credential validation,
  credential content inspection, or service-account key inspection;
- Google Docs or Google Drive API calls, mutation, cleanup, readback, or Live
  E2E;
- OAuth scope changes, Google Picker adoption, `drive.file` adoption, or
  authentication architecture changes;
- public API, persisted schema, canonical format, Frozen specification, code,
  test, package, `dist`, release, tag, publication, GitHub asset, Avast,
  flagged executable, vendor-clearance, stage, commit, or push operations.

## 4. Current Documentation Findings

ADR-0002 already records the key policy:

- OAuth Desktop is preferred for personal Gmail and My Drive workflows.
- Service Account remains available for automation and prepared Shared Drive
  workflows.
- OAuth Desktop requires installed-application credential JSON and a
  persistent token-store path outside the repository and outside release
  packages.
- Credential files, token-store contents, OAuth tokens, and secret-bearing
  local configuration must not be committed, packaged, or copied into release
  records.
- Current OAuth Desktop scopes remain Documents and Drive.
- Google Picker plus `drive.file` remains deferred to vNext.

Installation guidance already tells operators to keep configuration outside
the ZIP and outside the repository, use `appsettings.Local.json.example` only
as a shape reference, set `GoogleApi:AuthenticationMode`,
`GoogleApi:CredentialsPath`, and `GoogleApi:TokenStorePath`, and avoid
recording secrets or sensitive paths.

Live E2E guidance already states that credentialed runs need explicit
operation-specific approval, including whether OAuth Desktop reauthorization,
token-store deletion, creation, or reuse is authorized for that run.

Safe observability and failure-classification ADRs already prohibit logs and
evidence from exposing OAuth tokens, credentials, credential paths,
token-store paths, private URLs, raw provider payloads, raw HTTP bodies, and
raw exception details.

## 5. Gaps And Ambiguities

The current documentation is directionally correct, but the setup path is
spread across several records. A first-time operator could miss which document
is authoritative for each setup phase.

Observed improvement areas:

- No single setup checklist separates "choose auth mode", "prepare local
  paths", "run only after authorization", and "record only redacted evidence".
- `appsettings.Local.json.example` shows concrete local path shapes, but the
  surrounding docs could more strongly say that example paths are placeholders
  and must not be copied into repository evidence.
- Token-store persistence is documented, but lifecycle operations need a
  clearer "requires explicit authorization" table.
- OAuth Desktop and Service Account responsibilities are described in ADR-0002
  but are not summarized near the operator setup steps.
- The current Documents and Drive scope decision is recorded in ADR-0002, but
  setup docs could make clearer that P2-05 does not adopt `drive.file`, Google
  Picker, or a new least-privilege flow.
- Redaction expectations are present, but docs could include a concise
  evidence-safe / not-evidence-safe checklist without adding any real paths or
  secrets.

## 6. Security Boundary

The P2-05 documentation boundary should preserve these rules:

- OAuth Desktop credential JSON is local sensitive state and must stay outside
  the repository, outside release packages, outside `dist`, outside evidence
  bundles, and outside logs.
- OAuth token stores are persistent local sensitive state. Their contents must
  not be read, copied, inspected, archived into the repository, packaged,
  logged, or pasted into evidence.
- Token-store creation, deletion, cleanup, reuse, refresh, or reauthorization
  is a credentialed Google operation and requires explicit operation-specific
  authorization.
- Service Account keys are credentials and follow the same no-repo, no-package,
  no-log, no-evidence-content rule.
- Documentation may use placeholder names only. It must not contain private
  URLs, real local absolute paths, real folder IDs, document IDs, account
  identifiers, token names, credential contents, raw provider payloads, raw
  exceptions, stack traces, or Authorization headers.
- Local verification can inspect repository text and Git diff only. It must
  not inspect credential files or token-store directories.

## 7. Recommended Documentation Design

Priority P1:

- Add an OAuth Desktop setup subsection to `docs/distribution/InstallationGuide.md`
  that gives a redacted checklist for choosing `OAuthDesktop`, placing
  credential and token-store paths outside repository/package locations, and
  confirming that live publish remains separately authorized.
- Add a concise OAuth Desktop vs Service Account responsibility table near the
  existing live publish readiness checklist, sourced from ADR-0002.
- Add a token-store lifecycle table that labels reuse, creation, deletion,
  cleanup, and reauthorization as credentialed operations requiring explicit
  authorization.

Priority P2:

- Add a redaction checklist to `docs/distribution/LiveE2EOperations.md` that
  distinguishes allowed evidence labels from prohibited sensitive values.
- Cross-link ADR-0002 from the installation and Live E2E setup sections as the
  authentication decision source.
- Clarify in `src/Publisher.Cli/appsettings.Local.json.example` comments only
  if JSON comments are not introduced; otherwise leave the example unchanged
  and keep guidance in Markdown.

Priority P3 / defer:

- Add a separate operator quick-reference page only if the InstallationGuide
  update becomes too long.
- Revisit least-privilege Google Picker plus `drive.file` only through a
  future ADR or vNext design task, not through P2-05 docs implementation.

## 8. Acceptance Criteria

A future docs implementation is acceptable only if:

- it changes documentation only;
- it preserves ADR-0002's existing OAuth Desktop / Service Account boundary;
- it does not change OAuth scopes, adopt Google Picker, adopt `drive.file`, or
  imply authentication architecture changes;
- it states that OAuth login, consent, token-store operations, Google Docs /
  Drive operations, and Live E2E require separate explicit authorization;
- it keeps credential and token-store handling outside repository, package,
  logs, and evidence;
- it uses placeholders only and introduces no real credential path, token-store
  path, private URL, Google resource ID, token, client secret, account
  identifier, Authorization header, raw exception, stack trace, provider
  payload, or local sensitive path;
- it does not update actual release notes, `CHANGELOG.md`, packages, `dist`,
  release records, tags, GitHub assets, vendor-clearance records, or Frozen
  specifications;
- docs verification confirms the changed files and absence of prohibited
  sensitive terms or local absolute paths.

## 9. Docs-Only Verification Plan

Required verification for a future docs implementation:

```powershell
git diff -- docs/distribution/InstallationGuide.md docs/distribution/LiveE2EOperations.md docs/development/Publisher_P2-05_OAuthDesktopTokenStoreDocumentationEvaluation.md
git diff --check
git status --short --branch
```

Also run a targeted sensitive-value scan over changed documentation for OAuth
tokens, Authorization headers, credential secret keys, token-store content,
credential JSON content, private localhost URLs, private Google resource
URLs, real local absolute user paths, raw exceptions, stack traces, and
provider payloads. The scan must not inspect credential files or token-store
directories.

Do not run OAuth login, consent, token-store inspection, token-store cleanup,
Google Docs or Google Drive operations, Live E2E, package or `dist` operations,
release, tag, publication, GitHub asset, Avast, flagged executable, stage,
commit, or push.

## 10. Implementation GO/NO-GO

Recommendation: COMPLETE for the narrow docs-only implementation.

Satisfied implementation conditions:

- edit only Markdown documentation unless a separate explicit task authorizes
  another file;
- make InstallationGuide the primary operator setup surface;
- keep ADR-0002 as the authentication decision source and avoid restating it
  as a new decision;
- include token-store lifecycle authorization labels;
- include redaction guidance without real sensitive values;
- preserve all release, Google, OAuth, token-store, package, `dist`, vendor,
  stage, commit, and push exclusions.

Closeout evidence:

- `docs/distribution/InstallationGuide.md` now includes the OAuth Desktop setup
  checklist, authentication-mode responsibility table, token-store lifecycle
  authorization table, unchanged-scope note, and sensitive-data boundary.
- `docs/distribution/LiveE2EOperations.md` now cross-links ADR-0002, replaces
  concrete local path examples with placeholders, adds redaction guidance, and
  separates Live E2E, OAuth Desktop, token-store, Google mutation, cleanup,
  package, release, and vendor-clearance approvals.
- The implementation remained Markdown-only and did not change OAuth scopes,
  authentication architecture, Google Picker / `drive.file` status, code,
  tests, packages, `dist`, release artifacts, vendor clearance, stage, commit,
  or push.

Verification recorded for closeout:

- `git diff -- docs/distribution/InstallationGuide.md docs/distribution/LiveE2EOperations.md docs/development/Publisher_P2-05_OAuthDesktopTokenStoreDocumentationEvaluation.md`
  reviewed the intended docs-only scope.
- `git diff --check` passed for the working-tree diff.
- Targeted sensitive-value scan over changed docs passed for OAuth tokens,
  Authorization headers, credential content, token-store content, raw provider
  payloads, private Google resource URLs, and real local absolute user paths.

No OAuth login, consent, token-store inspection, token-store cleanup, Google
Docs or Google Drive operation, Live E2E, package or `dist` operation,
release, tag, publication, GitHub asset, Avast, flagged executable, stage,
commit, or push was performed.
