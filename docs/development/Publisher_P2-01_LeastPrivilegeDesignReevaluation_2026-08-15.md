# Publisher P2-01 Least-Privilege Design Re-evaluation

Status  : Design re-evaluation complete / split-route design GO / implementation NO-GO
Scope   : Docs-only re-evaluation after P2-06 closeout completion
Depends : docs/development/Publisher_P2-01_GooglePickerDriveFileEvaluation.md, docs/development/Publisher_P2-01_OAuthDesktopScopeBoundaryEvaluation.md, docs/development/Publisher_P2-06_ManagedDocumentReadbackReportingEvaluation.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/architecture/ADR-0010-vnext-backlog-and-deferred-scope-boundary.md, src/Publisher/Infrastructure/Google/OAuthDesktopGoogleCredentialProvider.cs, src/Publisher/Infrastructure/Google/GoogleDriveTemporaryImageHost.cs

This is a docs-only / local-only re-evaluation record. It does not implement
Google Picker, adopt `drive.file`, change OAuth scopes, perform OAuth login or
consent, inspect or mutate token stores, call Google Docs or Google Drive APIs,
run Live E2E, change production code, change tests, update package or `dist`
artifacts, perform release, tag, publication, Avast, vendor, or flagged
executable operations, decide vendor clearance, stage, commit, or push.

## 1. Purpose

P2-06 closeout is complete. The next natural vNext route is therefore P2-01,
but only as a design re-evaluation because Google Picker plus `drive.file`
affects OAuth scope, Drive authorization, token-store lifecycle, selected
resource semantics, temporary image hosting, cleanup, and Live E2E proof.

This record re-checks the existing P2-01 evaluations and confirms the next
safe boundary.

## 2. Re-evaluation Inputs

Authoritative current records agree on these points:

- ADR-0002 keeps OAuth Desktop on Google Documents plus full Google Drive.
- ADR-0002 and ADR-0010 keep Google Picker plus `drive.file` deferred to vNext.
- `Publisher_P2-01_GooglePickerDriveFileEvaluation.md` recommends a later
  split least-privilege design and rejects immediate implementation.
- `Publisher_P2-01_OAuthDesktopScopeBoundaryEvaluation.md` classifies
  selected-document workflows as potentially compatible, while configured
  folders, Shared Drives, temporary image hosting, public permission mutation,
  cleanup, and token-store reuse require redesign or proof.
- Current OAuth Desktop code still requests Documents plus full Drive.
- Current temporary image hosting still uploads Drive files into a configured
  folder, creates public reader permission, and deletes Publisher-owned
  temporary files.

No later closeout record changes the P2-01 authentication or Drive boundary.

## 3. Decision

Design re-evaluation result: GO for a future scoped split-route design task.

Implementation result: NO-GO.

The next P2-01 task should not be a scope swap from full Drive to `drive.file`.
It should be a design task that decides whether Publisher can add a separate
least-privilege route for explicitly selected resources while preserving the
current full-Drive route for workflows that still need it.

## 4. Required Design Questions

The future scoped design must answer at least:

- whether the Picker route supports existing document update only, new
  document creation, Picker folder selection, or a smaller subset;
- whether temporary image hosting is excluded, kept on the current full-Drive
  route, or moved behind a separate image-hosting authorization path;
- how selected document or folder identity is represented without exposing
  private Google resource IDs, private URLs, account identifiers, credential
  paths, token-store paths, tokens, provider payloads, or temporary public
  image URLs in logs or evidence;
- how OAuth Desktop token-store reuse behaves when scope or selected-resource
  assumptions change, including whether explicit reauthorization or
  token-store lifecycle handling is required;
- which behaviors can be verified with local-only tests and which behaviors
  require separately authorized Live E2E.

## 5. Non-Goals

This re-evaluation does not authorize:

- a new ADR adoption decision;
- Google Picker UI, OAuth scope changes, new configuration keys, new public
  APIs, persisted schema changes, or dependency additions;
- OAuth login, browser consent, token-store read/write/delete/cleanup/reuse,
  Google Docs mutation, Google Drive mutation, Drive readback, cleanup, or
  Live E2E;
- release, tag, publication, package or `dist` changes, GitHub asset
  operations, Avast operations, flagged executable re-run, vendor-clearance
  judgment, stage, commit, or push.

## 6. Future GO / NO-GO

Future design GO is limited to a docs-only or design-only task that preserves
ADR-0002 until explicitly superseded and keeps every external operation behind
its own approval gate.

Future implementation remains NO-GO until a scoped design and adoption record
defines supported workflows, selected-resource representation, token-store
lifecycle, safe diagnostics, local-only tests, and any separately authorized
Live E2E proof.

Immediate implementation is NO-GO if it requires a blanket OAuth Desktop
replacement with `drive.file`, treating configured folder IDs as Picker grants,
weakening readback or Verified State behavior, exposing sensitive Google or
OAuth values, changing public contracts, adding dependencies, or performing
external operations.

## 7. Local Verification

This docs-only re-evaluation can be verified with:

```powershell
git diff -- docs/development/Publisher_P2-01_LeastPrivilegeDesignReevaluation_2026-08-15.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

No build, unit test, integration test, Live E2E, OAuth, Google Docs / Drive,
package, `dist`, release, tag, publication, Avast, vendor, flagged executable,
stage, commit, or push operation is required or authorized for this docs-only
review.
