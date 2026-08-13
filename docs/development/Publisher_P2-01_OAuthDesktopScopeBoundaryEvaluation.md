# Publisher P2-01 OAuth Desktop Scope Boundary Evaluation

Status  : Design complete / implementation decision pending
Scope   : Narrow design evaluation of current OAuth Desktop workflow boundaries after P2-01
Depends : docs/development/Publisher_P2-01_GooglePickerDriveFileEvaluation.md, docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/architecture/ADR-0010-vnext-backlog-and-deferred-scope-boundary.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, docs/distribution/LiveE2EOperations.md, specs/publisher/Publisher_v1.0_Architecture_Specification.md, src/Publisher/Infrastructure/Google/OAuthDesktopGoogleCredentialProvider.cs, src/Publisher/Infrastructure/Google/ServiceAccountGoogleCredentialProvider.cs, src/Publisher/Infrastructure/Google/GooglePublisherOptions.cs, src/Publisher/Infrastructure/Google/GoogleDocsPublisher.cs, src/Publisher/Infrastructure/Google/GoogleDriveClient.cs, src/Publisher/Infrastructure/Google/GoogleDocsClient.cs, src/Publisher/Infrastructure/Google/GoogleDocsBatchUpdateClient.cs, src/Publisher/Infrastructure/Google/GoogleDriveTemporaryImageHost.cs, src/Publisher/Infrastructure/Google/PublishPlanExecutor.cs

This is a design-only evaluation record. It does not implement Google Picker,
change OAuth scopes, update ADRs, perform OAuth login or consent, inspect or
mutate token stores, call Google Docs or Google Drive APIs, run Live E2E,
change production code, change tests, update package or `dist` artifacts,
stage, commit, or push.

## 1. Purpose

P2-01 concluded that Google Picker plus `drive.file` should not be adopted
immediately, but that a later narrow design may evaluate split
least-privilege routing.

This follow-on evaluation decomposes the current OAuth Desktop workflow into
operation boundaries and classifies which operations appear compatible with a
future `drive.file` route, which operations still require the current broader
Drive scope, and which operations need live proof before any adoption record.

## 2. Current OAuth Desktop Workflow

The current OAuth Desktop credential provider requests:

- `https://www.googleapis.com/auth/documents`;
- `https://www.googleapis.com/auth/drive`.

The current workflow uses those credentials for these operation groups:

| Step | Current component | API family | Current behavior |
| --- | --- | --- | --- |
| OAuth consent and token reuse | `OAuthDesktopGoogleCredentialProvider` | OAuth | Authorizes the local user with Documents plus full Drive and persists tokens through `FileDataStore`. |
| Destination configuration | `GooglePublisherOptions` | Configuration | Uses configured folder IDs for publish destination and temporary image hosting. No Picker-selected resource model exists. |
| Native document creation | `GoogleDocsPublisher`; `GoogleDriveClient` | Drive | Creates a Google Docs file through Drive and sets `parents` to the configured folder. |
| Initial content publication | `PublishPlanExecutor`; `GoogleDocsClient` | Docs | Applies batch updates, inserts tables, inserts inline images, and reads back Docs structure. |
| Differential physical update | `GoogleDocsBatchUpdateClient`; application update services | Docs | Applies batchUpdate with revision control and readback/Verified State promotion. |
| Table/image readback | `GoogleDocsClient`; `PublishPlanExecutor` | Docs | Reads document structure to verify inserted tables, images, indexes, and actual image size. |
| Temporary image upload | `GoogleDriveTemporaryImageHost` | Drive | Uploads local image bytes to a configured Drive folder with app properties. |
| Temporary image public access | `GoogleDriveTemporaryImageHost` | Drive | Creates an `anyone` reader permission so Google Docs can fetch the image URI. |
| Temporary image cleanup | `GoogleDriveTemporaryImageHost`; `PublishPlanExecutor` | Drive | Deletes Publisher-owned temporary image files after insertion or failed permission setup. |

Service Account mode already requests Documents plus `drive.file`, but that is
not sufficient evidence that OAuth Desktop can simply switch scopes. Service
Account and OAuth Desktop differ in identity, consent, target-folder access,
token lifecycle, and interactive resource-selection expectations.

## 3. Boundary Classification

| Operation | Future `drive.file` applicability | Boundary rationale |
| --- | --- | --- |
| OAuth Desktop authorization for selected Publisher-created files | Applicable only after redesign | `drive.file` could reduce Drive exposure for app-created or explicitly selected files, but the current token-store and consent lifecycle does not record selected resources or scope version. |
| Existing document update selected through Picker | Potentially applicable | A Picker-selected document may fit a least-privilege route if Docs access, Drive file visibility, revision readback, and evidence redaction are proven. |
| Google Docs `documents.get` readback for the selected document | Potentially applicable | This is a Docs API operation and still requires the Documents scope. It must remain bound to a selected or Publisher-created document and must preserve ADR-0004 readback guarantees. |
| Google Docs `documents.batchUpdate` for the selected document | Potentially applicable | This remains a Docs operation. It cannot be weakened because revision-conflict handling, delivery-state classification, and Verified State promotion depend on exact write/readback semantics. |
| Create a new native Google Doc without Picker-selected destination | Requires current broader Drive route or redesign | Current behavior creates the file in a configured folder. A pure `drive.file` route would need a new decision for creation location, Picker folder selection, or user-visible post-creation move. |
| Create a new native Google Doc in a Picker-selected folder | Needs live proof before adoption | This may be feasible only if the selected folder grant supports parent assignment under the intended OAuth consent model, including Shared Drive behavior. |
| Use configured `GoogleApi:FolderId` without user selection | Requires current broader Drive route | The current configured folder ID is not an interactive Picker grant. Treating it as sufficient for `drive.file` would silently change the authorization model. |
| Shared Drive destination compatibility | Requires current broader Drive route until proven otherwise | Current Drive calls use `supportsAllDrives=true`. A least-privilege route must prove selected-resource behavior for Shared Drives before adoption. |
| Temporary image upload into a configured folder | Requires current broader Drive route or a separate image-hosting route | The operation creates Drive files in a configured folder and is not naturally tied to a selected target document. |
| Temporary image public permission creation | Requires separate explicit design and live proof | Public permission mutation has a different risk profile from selected document editing and must stay separately gated. |
| Temporary image cleanup/delete | Potentially applicable only for Publisher-created temporary files | `drive.file` may be acceptable for files created by Publisher, but cleanup must prove ownership, failure behavior, and no unrelated file deletion. |
| Remote image insertion using an existing public URL | Applicable without Drive scope change | This path uses Docs insertion of a remote URI and does not require Drive file creation by Publisher. |
| Existing Google Drive image source by raw file ID/public URI | Requires separate design | Raw Drive file IDs and public URIs have logging and evidence risks; no Picker grant or redacted selected-resource model currently exists. |
| Dry-run publication planning | Not dependent on Drive scope | Dry-run must remain local-only and must not be represented as OAuth, Docs readback, Drive mutation, or Live E2E evidence. |

## 4. Scope Boundary

A future split design should treat `drive.file` as suitable only for a
resource-selected route, not as a blanket replacement for full Drive.

Candidate `drive.file` route:

- update an existing document explicitly selected by the operator;
- create or update only Publisher-created files when the creation location and
  user consent path are explicitly defined;
- read back and mutate only the selected or Publisher-created document through
  Google Docs APIs;
- insert remote images that do not require Publisher Drive hosting;
- clean up only Publisher-owned temporary files if ownership and app-created
  access are proven.

Current full-Drive route remains required unless superseded for:

- configured folder IDs that were not selected through a Picker flow;
- broad My Drive or Shared Drive destination compatibility;
- document creation with `parents` in an arbitrary configured folder;
- temporary image hosting in a configured Drive folder;
- public permission mutation for temporary image files;
- cleanup or discovery behavior that cannot be limited to app-created or
  selected resources.

## 5. ADR Impact

ADR-0002 remains unchanged. The current accepted decision is OAuth Desktop
with Documents plus full Drive, and Google Picker plus `drive.file` remains
deferred.

A future adoption would require a new ADR or explicit successor record because
it changes:

- OAuth Desktop scope selection;
- operator resource-selection workflow;
- token-store lifecycle and reauthorization expectations;
- configuration semantics for folder IDs, document IDs, and selected
  resources;
- Drive operation contracts for creation, parent assignment, public
  permissions, and cleanup.

ADR-0010 remains unchanged. P2-01 and this evaluation are planning records,
not implementation approval, Google mutation approval, Live E2E
authorization, release authorization, or vNext adoption.

## 6. Google Docs And Drive Contract Impact

Google Docs contracts must remain stable in any future split design:

- Documents scope remains required for document readback and mutation.
- Readback verification cannot be skipped or weakened.
- Revision-conflict and delivery-state handling must remain conservative.
- Verified State promotion must still occur only after successful readback.

Google Drive contracts would need explicit redesign before `drive.file`
adoption:

- `GooglePublisherOptions.FolderId` cannot silently become a Picker grant.
- `TemporaryImageFolderId` cannot silently become compatible with
  `drive.file`.
- raw file IDs, folder IDs, document IDs, web links, temporary public URLs,
  token-store paths, credential paths, and account identifiers must remain
  outside logs and evidence.
- any Picker-selected identity representation must be reviewable without
  exposing private Google resource values.

## 7. Evaluation Result

Implementation recommendation: NO-GO.

Design recommendation: GO only for a future scoped split-route design that
keeps the current full-Drive OAuth Desktop workflow available while defining a
separate `drive.file` route for selected-resource workflows.

The future design must answer these questions before implementation:

- Does the `drive.file` route support existing document update only, new
  document creation, Picker folder selection, or all three?
- Are local images excluded, routed through current full Drive, or handled by
  a separate image-hosting authorization path?
- How does reauthorization occur when an existing token store was created with
  different scopes?
- How are selected resources represented in configuration, diagnostics, tests,
  and evidence without leaking private identifiers?
- Which behavior can be proven with local-only unit tests, and which behavior
  requires separately authorized Live E2E?

## 8. Non-Changes

This evaluation does not change:

- ADR-0002, ADR-0010, or any other ADR;
- OAuth Desktop scopes;
- Service Account scopes;
- public APIs;
- persisted schemas;
- configuration keys;
- Google Docs or Google Drive implementation;
- tests;
- package, `dist`, release, tag, publication, or vendor-clearance state.

## 9. Local Verification

This docs-only evaluation can be verified with:

```powershell
git diff -- docs/development/Publisher_P2-01_OAuthDesktopScopeBoundaryEvaluation.md
git diff --check
git status --short --branch
```

No OAuth login, browser consent, token-store inspection, Google Docs operation,
Google Drive operation, Live E2E, package operation, release operation, stage,
commit, or push is required or authorized for this evaluation.
