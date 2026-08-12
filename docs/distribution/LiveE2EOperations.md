# Publisher Live E2E Operations

Status  : Phase 3-9D Operations Guide
Scope   : Authorized Google Docs end-to-end verification
Depends : tests/integration/Publisher/GoogleDocsEndToEndIntegrationTests.cs, docs/distribution/ReleaseChecklist.md, docs/distribution/PublisherReleaseRunbook.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_PreflightHardening.md, docs/development/Publisher_TestClassification.md

This guide defines the controlled procedure for live Google Docs end-to-end
verification. Live E2E is never implicit. It requires explicit approval for the
specific run because it uses credentials and mutates Google Docs or Google
Drive resources.

This guide can be reviewed or cross-linked as documentation-only work. A
documentation-only review of Live E2E setup does not authorize a live run, set
`VMF_PUBLISHER_GOOGLE_E2E=1`, open OAuth consent, mutate token stores, create
or clean up Google Docs / Drive resources, run package or `dist` operations,
execute flagged artifacts, publish releases, claim Avast vendor clearance, or
claim Avast safety certification.

## 1. Authorization Gate

Before running Live E2E, record approval for:

- the Google account or service identity to use;
- the destination folder;
- whether a template document may be copied or reset;
- whether temporary public image hosting is allowed;
- the cleanup expectation for temporary documents and files;
- the exact verification command;
- whether OAuth Desktop reauthorization, token-store deletion, token-store
  creation, or token-store reuse is authorized for this run.

If approval is missing, do not run Live E2E. Use local build, unit tests,
integration tests, package verification, and dry-run checks instead.

Approval for Live E2E is operation-specific. It does not authorize package or
`dist` updates, tagged release work, publication, flagged executable smoke
testing, Avast operations, vendor-clearance wording, or Avast safety
certification wording. Read `docs/development/CURRENT_STATUS.md`,
`docs/development/Publisher_PreflightHardening.md`, and
`docs/development/Publisher_TestClassification.md` before treating a Live E2E
gate as available.

## 2. Required Environment Variables

Live E2E tests are enabled only when:

```powershell
$env:VMF_PUBLISHER_GOOGLE_E2E = "1"
```

Set the credential and destination variables for the approved run:

```powershell
$env:VMF_PUBLISHER_GOOGLE_AUTH_MODE = "OAuthDesktop"
$env:VMF_PUBLISHER_GOOGLE_CREDENTIALS_PATH = "C:\Secrets\vmf-publisher-oauth-client.json"
$env:VMF_PUBLISHER_GOOGLE_TOKEN_STORE_PATH = "C:\Secrets\vmf-publisher-token-store"
$env:VMF_PUBLISHER_GOOGLE_E2E_FOLDER_ID = "<approved-folder-id>"
```

Optional template routing:

```powershell
$env:VMF_PUBLISHER_GOOGLE_E2E_TEMPLATE_DOCUMENT_ID = "<approved-template-document-id>"
```

Do not print credential file content, token-store content, OAuth tokens,
private keys, or secret-bearing configuration values in release records.

Credentials and token stores must stay outside the repository and outside
release packages. Do not commit, stage, copy into `dist`, attach to evidence
bundles, or paste credential files, token-store files, OAuth codes, refresh
tokens, client secrets, service-account private keys, Authorization headers, or
provider payloads. If reauthorization or token-store cleanup is needed, it
must be authorized and recorded as a credentialed Google operation, not as a
documentation task.

## 3. Execution

Run Live E2E serially:

```powershell
dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~GoogleDocsEndToEndIntegrationTests" --logger "console;verbosity=normal"
```

The Live E2E test collection is non-parallelized. Do not run multiple live
Publisher verification processes against the same document or folder unless
the operation has been explicitly designed for that concurrency test.

GitHub Actions Live E2E is available only through manual dispatch of
`.github/workflows/live-e2e.yml`. The job must use the
`publisher-live-e2e` Environment so repository maintainers can require
environment approval before credentials are exposed to the run.

The GitHub workflow uses Service Account authentication and must receive live
configuration only from these Secrets:

- `VMF_PUBLISHER_GOOGLE_CREDENTIALS_JSON`
- `VMF_PUBLISHER_GOOGLE_E2E_FOLDER_ID`
- `VMF_PUBLISHER_GOOGLE_E2E_TEMPLATE_DOCUMENT_ID`

Normal CI and Release workflows must not set `VMF_PUBLISHER_GOOGLE_E2E=1`.
They run the mock integration subset and package verification only.

For a release-path task, Live E2E follows the order recorded in
`docs/distribution/PublisherReleaseRunbook.md`: final verification first, then
Live E2E, then result review, then package or `dist` work, then tag/release
work. A prior authorized Live E2E result may be cited only as recorded
evidence for its exact scope; it must not be reused as approval for a new
credentialed run or Google mutation.

## 4. Expected Evidence

Record:

- command executed;
- start and end time;
- result, warning count, and error count;
- publish session IDs when CLI publish is used;
- created or copied Google document IDs;
- readback verification result;
- revision conflict result when applicable;
- cleanup result for temporary resources.

Do not record:

- OAuth refresh tokens;
- service-account private keys;
- credential JSON content;
- token-store files;
- private document content beyond the minimal evidence needed for verification;
- secret-like URLs or local paths that expose usernames, tokens, or private
  infrastructure names.

## 5. Cleanup

After Live E2E, remove or archive temporary Google Docs and Google Drive files
according to the approved cleanup expectation. Temporary public image files
must be cleaned up even when the publish operation fails.

If cleanup fails, record:

- resource identifier;
- failed cleanup command or API operation;
- error classification;
- next manual cleanup owner.

A cleanup failure does not become a successful Live E2E result. It remains an
open release issue until resolved or explicitly deferred.

Cleanup evidence is scoped to the temporary resources from the approved run.
It must not delete, archive, move, or alter unrelated user Google Docs / Drive
resources. Cleanup authorization does not authorize publication cleanup,
release asset replacement, package regeneration, token-store mutation, or
credential rotation unless those actions are separately named and approved.

## 6. Failure Handling

Retry only transient Google API failures such as HTTP 429, 500, 502, 503, or
504. Do not retry authentication failures, permission failures, malformed
configuration, missing credentials, or safety-policy failures as transient
errors.

If a test reports revision conflict, readback mismatch, managed-region
mismatch, unsafe configuration, or unverified state promotion denial, stop the
release operation and record the failure in the release checklist.

## 7. Local Alternatives

When Live E2E is not approved, execute and record local checks only:

```powershell
dotnet build VMF.Publisher.sln --configuration Release --no-restore
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip
```

Local alternatives must be reported as local verification. They do not prove
Google Docs rendering, Docs API readback, or Drive cleanup behavior.
