# ADR-0002: OAuth 2.0 Desktop Authentication

Status  : Accepted
Date    : 2026-08-05
Scope   : Publisher Google API authentication mode for personal Google accounts and local operator workflows
Depends : docs/architecture/ADR-0001-architecture-decision-record-process.md, docs/architecture/ADR_INDEX.md, docs/distribution/InstallationGuide.md, docs/distribution/LiveE2EOperations.md, src/Publisher/Infrastructure/Google/GooglePublisherOptions.cs, src/Publisher/Infrastructure/Google/OAuthDesktopGoogleCredentialProvider.cs

## Context

VMF Publisher publishes Markdown-derived content to Google Docs and uses
Google Drive for document creation and related Drive operations when an
authorized live publish or Live E2E operation is explicitly approved.

The Publisher implementation supports two Google authentication modes:

- Service Account;
- OAuthDesktop.

Service Account authentication remains useful for automation and controlled
Shared Drive workflows, but it depends on the destination being accessible to
the service identity. Personal Gmail and ordinary My Drive operator workflows
do not naturally grant a service account access to the user's target folders
without extra sharing or administrative setup.

OAuth 2.0 Desktop authentication lets a local operator authorize Publisher as
the interactive Google user through the installed-application browser flow. It
matches personal Gmail operation better because the resulting credential acts
as the consenting user, subject to that user's Google account permissions and
the configured OAuth scopes.

This ADR records the authentication decision only. It does not authorize a live
publish, Live E2E, Google Docs mutation, Google Drive mutation, token-store
mutation, release, package creation, tag, or publication.

## Decision

Publisher uses OAuth 2.0 Desktop authentication as the preferred local operator
authentication mode for personal Gmail and My Drive workflows.

The configured authentication mode remains explicit. Operators select
`OAuthDesktop` or `ServiceAccount` through the existing Google API
authentication configuration. This ADR does not remove Service Account support.

OAuth 2.0 Desktop requires an installed-application OAuth client credential
JSON and a token-store path outside the repository and outside release
packages. The token store is intentionally persistent so repeat local
operations can reuse the user's consent without requiring a browser
authorization flow every time. Credential files, token-store contents, OAuth
tokens, and secret-bearing local configuration must not be committed, packaged,
or copied into release records.

The current OAuth Desktop implementation uses these Google scopes:

- `https://www.googleapis.com/auth/documents`;
- `https://www.googleapis.com/auth/drive`.

The Documents scope is required for Google Docs content operations. The Drive
scope is used for Drive document creation and related Drive operations,
including workflows that must address folders, Shared Drive compatibility, or
temporary image-hosting behavior when separately authorized.

Service Account authentication remains available for automation and Shared
Drive scenarios where the target Drive location is explicitly shared with, or
otherwise accessible to, the service identity. Service Account mode is not the
preferred path for personal Gmail My Drive operation because access depends on
the target folder and account-sharing setup rather than the interactive user's
ordinary browser consent.

Google Picker plus `drive.file` least-privilege routing is not adopted by this
ADR. It is a vNext reconsideration item because it would require a different
operator selection flow, scope model, and possibly additional UI or
authorization behavior. Until such a later ADR or adopted specification changes
the decision, the current OAuth Desktop mode continues to use Documents and
Drive scopes.

## Consequences

Local operators can use personal Gmail accounts without requiring a service
account to be shared into each My Drive destination before every publish.

Token-store persistence improves repeat operation ergonomics, but it creates a
strict operational boundary: token stores are local sensitive state and must
remain outside the repository, release packages, logs, and evidence bundles.

Service Account remains the safer fit for non-interactive automation when the
Drive access model is explicitly prepared for the service identity.

The current scope choice is broader than a future Picker plus `drive.file`
model. That least-privilege improvement remains deferred to vNext and must not
be represented as already implemented or approved by this ADR.

## Status History

| Date | Status | Notes |
| --- | --- | --- |
| 2026-08-05 | Proposed | Initial ADR drafted as docs-only / local-only authentication decision record. |
| 2026-08-05 | Accepted | OAuth 2.0 Desktop accepted as the preferred local operator mode for personal Gmail and My Drive workflows. |

## Related Documents

- `docs/architecture/ADR_INDEX.md`
- `docs/architecture/ADR-0001-architecture-decision-record-process.md`
- `docs/distribution/InstallationGuide.md`
- `docs/distribution/LiveE2EOperations.md`
- `src/Publisher/Infrastructure/Google/GooglePublisherOptions.cs`
- `src/Publisher/Infrastructure/Google/OAuthDesktopGoogleCredentialProvider.cs`

## Replacement

This ADR does not supersede an earlier ADR.

No successor ADR is recorded.

## Non-Goals

- This ADR does not modify Frozen Specifications.
- This ADR does not modify public APIs.
- This ADR does not modify implementation behavior.
- This ADR does not modify persisted schemas or canonical formats.
- This ADR does not replace implementation specifications, runbooks, release
  records, or verification evidence.
- This ADR does not approve release, tag, publication, package creation,
  package update, Live E2E, Google Docs mutation, Google Drive mutation,
  token-store mutation, or flagged executable execution.
- This ADR does not claim release readiness, vendor clearance, publication
  completion, or Live E2E completion.
