# Publisher Installation Guide

Status  : Phase 3-9D Operations Guide
Scope   : VMF Publisher ZIP installation
Depends : docs/development/Publisher_Phase3-9_DesignNotes.md, tools/publisher/package-publisher.ps1, tools/publisher/verify-package.ps1

This guide describes how to install a VMF Publisher release ZIP on Windows.
It is operational documentation only. It does not modify Frozen
specifications, public contracts, Google API contracts, identity/revision
contracts, or persisted state schemas.

## 1. Prerequisites

- Windows x64 host.
- .NET 8 Runtime available on the target machine.
- A verified VMF Publisher release ZIP for `win-x64`.
- A writable directory outside the ZIP for local configuration and token-store
  files.
- Google API credentials only when running commands that publish to Google
  Docs or Google Drive.

Confirm the runtime before installation:

```powershell
dotnet --list-runtimes
```

The runtime list must include a compatible `Microsoft.NETCore.App 8.x` entry.

## 2. Package Verification

Verify the ZIP before extracting it into the operational installation
directory:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip
```

Verification must pass before installation. The verifier checks required CLI
artifacts, `package-manifest.json`, file sizes, SHA-256 hashes, unmanifested
files, excluded configuration files, and secret-like filenames or content.

## 3. Installation

Create or choose an installation directory, then extract the verified ZIP:

```powershell
New-Item -ItemType Directory -Path C:\Tools\vmf-publisher -Force
Expand-Archive -LiteralPath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip -DestinationPath C:\Tools\vmf-publisher -Force
```

Run the CLI help command from the extracted directory:

```powershell
C:\Tools\vmf-publisher\vmf-publisher.exe --help
```

The command must print the VMF Publisher usage text and exit successfully.

## 4. Configuration

Release ZIP packages intentionally exclude:

- `appsettings.json`
- `appsettings.local.json`

Create configuration outside the release ZIP. Use
`src\Publisher.Cli\appsettings.Local.json.example` as the shape reference, but
do not place credentials, token stores, or local secrets under the repository
or inside the extracted release package.

Supported operational settings include:

- `Publisher:AllowTemporaryPublicImageHosting`
- `Publisher:ImageMaxWidthPoints`
- `Publisher:AllowImageUpscale`
- `Cli:OperationTimeoutSeconds`
- `Cli:HttpTimeoutSeconds`
- `GoogleApi:ApplicationName`
- `GoogleApi:AuthenticationMode`
- `GoogleApi:CredentialsPath`
- `GoogleApi:TokenStorePath`
- `GoogleApi:FolderId`
- `GoogleApi:TemporaryImageFolderId`

Configuration precedence is:

1. `appsettings.json`
2. `appsettings.local.json`
3. environment variables

For CLI timeout overrides, use:

- `VMF_PUBLISHER_OPERATION_TIMEOUT_SECONDS`
- `VMF_PUBLISHER_HTTP_TIMEOUT_SECONDS`

### OAuth Desktop Setup Checklist

ADR-0002 (`docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md`)
is the authentication decision source. This guide summarizes operator setup
only; it does not change OAuth scopes, adopt Google Picker, adopt
`drive.file`, or authorize OAuth consent.

Before selecting `OAuthDesktop`, confirm:

- the operation is for a personal Gmail / My Drive local operator workflow;
- a separate operation-specific authorization allows OAuth Desktop use for
  this run;
- the installed-application credential JSON is stored outside the repository,
  outside `dist`, and outside the extracted release package;
- the token-store directory is stored outside the repository, outside `dist`,
  and outside the extracted release package;
- local configuration records only paths needed by the operator and is not
  copied into repository docs, release packages, logs, or evidence bundles;
- the live publish, Live E2E, Google Docs / Drive operation, OAuth consent,
  token-store reuse, token-store creation, token-store deletion,
  token-store cleanup, and reauthorization gates have each been separately
  authorized when applicable.

### Authentication Mode Responsibilities

| Mode | Intended responsibility | Operator boundary |
| --- | --- | --- |
| `OAuthDesktop` | Preferred local operator mode for personal Gmail and My Drive workflows. | Requires installed-application OAuth client credentials and a persistent token-store path outside the repository and package. OAuth login, consent, reauthorization, and token-store lifecycle operations require separate authorization. |
| `ServiceAccount` | Automation or prepared Shared Drive workflows where the destination is explicitly accessible to the service identity. | Requires service-account credentials outside the repository and package. It is not the preferred path for ordinary personal Gmail / My Drive operation. |

The current OAuth Desktop scopes remain Documents and Drive as recorded in
ADR-0002. Google Picker and `drive.file` remain deferred and are not adopted
by this guide.

### Token-Store Lifecycle Authorization

| Token-store action | Authorization requirement | Documentation boundary |
| --- | --- | --- |
| Reuse existing token store | Requires explicit operation-specific authorization for credentialed reuse. | Do not inspect, copy, archive, log, or paste token-store contents. |
| Create token store through OAuth consent | Requires explicit authorization for OAuth consent and token-store creation. | Do not record OAuth codes, refresh tokens, account identifiers, or token contents. |
| Delete token store | Requires explicit authorization for token-store deletion. | Do not treat deletion as documentation cleanup or ordinary local verification. |
| Cleanup stale token-store files | Requires explicit authorization for credentialed cleanup. | Do not enumerate or quote token-store file contents in evidence. |
| Reauthorize / refresh consent | Requires explicit authorization for OAuth reauthorization. | Record only redacted status labels and approved outcome evidence. |

Credential files, service-account keys, token stores, OAuth tokens,
Authorization headers, credential-bearing local configuration, private URLs,
local sensitive paths, raw provider payloads, raw HTTP bodies, raw exceptions,
and stack traces must not be committed, packaged, copied into `dist`, written
to logs, or included in release or operation evidence.

## 5. Local Smoke Test

Run configuration validation without requiring Google publish settings:

```powershell
C:\Tools\vmf-publisher\vmf-publisher.exe verify
```

Run a local compile verification against a Markdown file:

```powershell
C:\Tools\vmf-publisher\vmf-publisher.exe verify .\sample.md
```

Run a local dry run before live publishing:

```powershell
C:\Tools\vmf-publisher\vmf-publisher.exe dry-run .\sample.md
```

These checks are local. They must not be reported as Google Docs API readback
or live E2E verification.

## 6. Live Publish Readiness

Before running `publish`, confirm:

- live Google Docs/Drive updates are explicitly authorized for the run;
- `GoogleApi:AuthenticationMode` is set to the intended mode;
- `GoogleApi:CredentialsPath` points to a credential file outside the
  repository and package;
- `GoogleApi:TokenStorePath` points to a token-store directory outside the
  repository and package when using OAuth Desktop;
- `GoogleApi:FolderId` identifies the intended destination folder;
- temporary public image hosting remains disabled unless explicitly approved
  for the operation.

Run live publish only after the above checks are complete:

```powershell
C:\Tools\vmf-publisher\vmf-publisher.exe publish .\sample.md
```

Record the publish session ID, exit code, classification, document ID, document
URL, and any readback evidence in the release or operation record. Do not
record secrets, credential paths containing sensitive names, token contents, or
private URLs.
