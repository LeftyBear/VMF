# Publisher Live E2E Authorization Credential Cleanup Boundary Review

Status  : Review checklist only / docs-only / local-only
Scope   : Live E2E authorization, credential handling, token-store handling, and cleanup evidence boundaries
Depends : docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md, docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md, docs/development/Publisher_EvidenceBundleSpecification.md

This review records the boundaries that must be checked before any future Live
E2E run. It does not authorize Live E2E, set `VMF_PUBLISHER_GOOGLE_E2E=1`,
mutate Google Docs or Google Drive, mutate token stores, run cleanup, re-run a
flagged executable, create or update packages, create tags, publish artifacts,
change production code, change tests, change public APIs, or modify Frozen
specifications.

## 1. Authorization Boundary

| Operation | Default State | Required Before Execution |
| --- | --- | --- |
| Live E2E | BLOCKED | Separate explicit Live E2E authorization. |
| Google Docs mutation | BLOCKED | Separate explicit Google Docs mutation authorization. |
| Google Drive mutation | BLOCKED | Separate explicit Google Drive mutation authorization. |
| Token-store mutation | BLOCKED | Separate explicit credential/token-store authorization. |
| Temporary public image hosting | BLOCKED | Separate explicit authorization and cleanup review. |
| Flagged executable rerun | BLOCKED | Separate explicit executable rerun authorization. |

Authorization for one operation does not authorize another operation.

## 2. Credential And Token-Store Boundary

OAuth credential files and token stores must remain outside the repository,
outside packages, outside logs, and outside evidence bundles.

Do not record:

- OAuth client JSON;
- service-account JSON;
- access tokens;
- refresh tokens;
- client secrets;
- private keys;
- token-store contents;
- credential paths;
- token-store paths;
- Authorization headers;
- personal account details.

Safe records may include authentication mode, whether reauthorization occurred,
sanitized failure class, sanitized Google error code, and whether secrets were
excluded from evidence.

## 3. Live E2E Evidence Boundary

Live E2E must be recorded as `PASS` only when it was:

- separately authorized;
- executed with the intended target configuration;
- directly observed;
- read back with the expected Google Docs and Google Drive evidence;
- reviewed for cleanup and redaction.

If Live E2E is not executed, record `NOT EXECUTED`, `BLOCKED`, or `DEFERRED`.
Do not infer Live E2E success from unit tests, non-live integration tests,
dry-run output, prior runs, or post-release observations.

## 4. Cleanup Boundary

Cleanup evidence is part of the Live E2E result review when Google Docs,
Google Drive, temporary public image hosting, or token-store mutation is in
scope.

| Cleanup Item | Status | Evidence |
| --- | --- | --- |
| Temporary Google Drive files removed | PENDING |  |
| Temporary public image access disabled or removed | PENDING |  |
| Test Google Docs artifacts handled as authorized | PENDING |  |
| Token-store mutation reviewed | PENDING |  |
| Cleanup failure recorded as failure/blocker | PENDING |  |
| Redaction review complete | PENDING |  |

Cleanup failure must not be treated as success, partial success, or harmless
post-run noise. If cleanup fails or cannot be verified, record the failure or
blocker and stop release-path promotion until a separate decision records the
next action.

## 5. Evidence Redaction Review

Before any Live E2E evidence is added to a release record, confirm:

| Check | Expected Result |
| --- | --- |
| Token and credential scan | No token, credential, private key, bearer value, or cookie. |
| Local path scan | No local absolute path, user profile path, credential path, or token-store path. |
| URL scan | No private Google Docs or Drive URL unless explicitly approved. |
| Provider payload scan | No raw HTTP body, raw exception body, stack trace, or provider payload. |
| Status scan | No `PASS` for work that was not directly authorized and executed. |

## 6. Explicit Non-Actions

This review does not execute Live E2E, mutate Google Docs or Google Drive,
mutate token stores, clean up live resources, re-run flagged executables,
create or update packages, write to `dist`, create tags, publish artifacts,
push commits, or change production code, tests, public APIs, persisted schemas,
or Frozen specifications.
