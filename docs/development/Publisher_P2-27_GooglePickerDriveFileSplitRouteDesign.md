# Publisher P2-27 Google Picker / drive.file Split-Route Design

Status  : COMPLETE / docs-only design; implementation NO-GO until adoption record and explicit authorization
Scope   : Define a future split-route least-privilege design boundary for Google Picker plus `drive.file`
Depends : docs/development/Publisher_P2-01_GooglePickerDriveFileEvaluation.md, docs/development/Publisher_P2-01_OAuthDesktopScopeBoundaryEvaluation.md, docs/development/Publisher_P2-01_LeastPrivilegeDesignReevaluation_2026-08-15.md, docs/development/Publisher_vNext_Backlog.md, docs/development/CURRENT_STATUS.md, docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md, docs/architecture/ADR-0010-vnext-backlog-and-deferred-scope-boundary.md, docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md

This is a docs-only / local-only design record. It does not adopt Google
Picker, adopt `drive.file`, change OAuth scopes, perform OAuth login or
consent, inspect or mutate token stores, call Google Docs or Google Drive APIs,
run Live E2E, change production code, change tests, update package or `dist`
artifacts, perform release, tag, publication, Avast, vendor, or flagged
executable operations, decide vendor clearance, stage, commit, or push.

## 1. Purpose

P2-27 closes the P2-01 future scoped design route selected by the P2-01
evaluations. The goal is to define a least-privilege split-route design
boundary without changing current behavior.

The design separates:

- the current ADR-0002 OAuth Desktop route using Documents plus full Drive;
- a possible future Picker-mediated route using Documents plus `drive.file`
  for explicitly selected-resource workflows.

Implementation remains NO-GO until a separate adoption record and explicit
implementation authorization define the exact supported workflows, token-store
lifecycle, configuration changes, tests, and any required Live E2E proof.

## 2. Decision

Decision: split-route design complete; implementation NO-GO.

The current route remains:

- OAuth Desktop with Documents plus full Drive scopes;
- configured folder IDs for destination and temporary image hosting;
- existing Docs readback and mutation behavior when separately authorized;
- existing Drive creation, parent assignment, temporary image hosting, public
  permission, and cleanup behavior when separately authorized.

The future least-privilege route is a separate route. It may be considered only
for explicitly selected-resource workflows and must not silently replace the
current route.

## 3. Route Model

### Route A: Current Full-Drive Route

Route A remains the only adopted behavior.

It continues to cover workflows that depend on:

- configured destination folder IDs;
- configured temporary image folder IDs;
- document creation in a configured folder;
- Shared Drive compatibility through existing Drive behavior;
- temporary image upload;
- temporary image public permission creation;
- temporary image cleanup;
- existing OAuth Desktop token-store reuse.

P2-27 does not change Route A.

### Route B: Future Picker / drive.file Route

Route B is a future candidate only.

Route B may cover a narrower workflow set:

- update an existing Google Doc explicitly selected by the operator;
- read back and mutate only the selected document through Docs APIs;
- optionally create a new document only if the destination-selection and
  parent-assignment model is proven safe;
- insert remote images that do not require Publisher Drive hosting;
- clean up only Publisher-created temporary files if ownership and access are
  proven under the selected-resource model.

Route B must not use configured folder IDs as implicit Picker grants.

## 4. Supported Workflow Matrix

| Workflow | Route A | Route B future eligibility | Decision |
| --- | --- | --- | --- |
| Existing selected document update | Supported when authorized | Eligible after design adoption and proof | Candidate Route B workflow |
| Docs readback for selected document | Supported when authorized | Eligible after design adoption and proof | Must preserve ADR-0004 |
| Docs batchUpdate for selected document | Supported when authorized | Eligible after design adoption and proof | Must preserve revision safety |
| New document in configured folder | Supported when authorized | Not eligible without separate folder-selection proof | Keep on Route A |
| Configured folder ID destination | Supported when authorized | Not eligible as an implicit grant | Keep on Route A |
| Shared Drive destination | Supported when authorized | Not eligible until proven with selected-resource semantics | Keep on Route A |
| Temporary image upload to configured folder | Supported when authorized | Not eligible without separate image-hosting design | Keep on Route A |
| Temporary public image permission | Supported when authorized | Not eligible without separate live proof and approval | Keep on Route A |
| Temporary image cleanup | Supported when authorized | Eligible only for Publisher-created files after proof | Candidate only |
| Remote public image URI insertion | Supported when authorized | Eligible if safe URI handling is preserved | Candidate Route B workflow |

## 5. Selected-Resource Representation

A future Route B adoption must define a representation for selected resources
that is useful for routing and tests without exposing private Google values.

Allowed design-level concepts:

- selected document present / absent;
- selected folder present / absent;
- selected-resource route enabled / disabled;
- selected-resource eligibility labels;
- bounded source labels such as `picker-selected`, `publisher-created`, and
  `configured-route`.

Prohibited in logs, diagnostics, fixtures, evidence, and release records:

- raw document IDs;
- raw folder IDs;
- private Google resource IDs;
- private URLs;
- temporary public URLs;
- account identifiers;
- OAuth tokens;
- credentials;
- credential paths;
- token-store paths;
- Authorization headers;
- cookies;
- provider payloads;
- raw HTTP bodies;
- raw exception messages;
- stack traces;
- local sensitive paths.

## 6. Token-Store And Scope Boundary

ADR-0002 remains current. OAuth Desktop continues to use Documents plus full
Drive until explicitly superseded.

A future adoption record must decide:

- whether Route B uses a separate auth mode, separate profile, or explicit
  capability flag;
- whether existing token stores can be reused when scopes change;
- whether Route B requires explicit reauthorization;
- how scope-version mismatch is detected and reported safely;
- how operators are warned without printing token-store paths or account
  identifiers;
- how Route A and Route B credentials remain distinguishable in local tests and
  operations.

P2-27 performs no token-store read, write, cleanup, reuse, or inspection.

## 7. Configuration Boundary

Route B cannot be implemented as a silent reinterpretation of existing
configuration keys.

Future implementation must not treat:

- `FolderId`;
- `TemporaryImageFolderId`;
- document IDs;
- raw Drive file IDs;
- existing token-store paths;

as Picker-selected grants unless a separate adopted design defines the mapping
and safe evidence behavior.

Any new configuration key, public API, persisted schema, CLI option,
dependency, or UI surface remains NO-GO until separately authorized.

## 8. Safety And Verification Boundary

Any future Route B implementation must preserve:

- ADR-0004 readback verification and Verified State promotion requirements;
- revision-conflict hard stops;
- delivery-state and HTTP-status diagnostic safety boundaries;
- ADR-0006 safe observability;
- ADR-0007 error classification and safe user-facing messages;
- release, package, Google, OAuth, Live E2E, vendor-clearance, and Avast gate
  separation.

Required future proof areas before adoption:

- selected existing document update;
- Docs readback on the selected document;
- Docs batchUpdate with revision safety;
- selected folder behavior if new-document creation is proposed;
- Shared Drive behavior if Route B claims Shared Drive support;
- temporary image hosting exclusion or separate authorization path;
- token-store scope mismatch and reauthorization behavior;
- redaction of selected-resource and OAuth-sensitive values.

Live Google proof, if required, remains separately authorized and is not part
of P2-27.

## 9. GO / NO-GO

GO:

- record P2-27 as the docs-only split-route design for P2-01;
- keep Route A as the only adopted behavior;
- define Route B as a future candidate route for selected-resource workflows;
- require a later adoption record before any implementation;
- preserve separate gates for OAuth, token-store, Google Docs / Drive, Live
  E2E, package, release, tag, publication, Avast, flagged executable, and
  vendor-clearance work.

NO-GO:

- no implementation in this task;
- no Google Picker adoption;
- no `drive.file` adoption for OAuth Desktop;
- no OAuth scope change;
- no token-store operation;
- no Google Docs or Google Drive operation;
- no Live E2E;
- no package, `dist`, release, tag, or publication operation;
- no Frozen specification, public API, persisted schema, CLI option,
  configuration key, dependency, or production-code change;
- no future implementation unless separately authorized after a scoped
  adoption record.

## 10. Local-Only Verification Plan

Required verification for this docs-only design:

```powershell
git diff -- docs/development/Publisher_P2-27_GooglePickerDriveFileSplitRouteDesign.md docs/development/Publisher_vNext_Backlog.md docs/development/CURRENT_STATUS.md
git diff --check
git status --short --branch
```

Implementation tests, Release build, format verification, OAuth login, browser
consent, token-store inspection, Live E2E, Google Docs / Drive verification,
package verification, Avast scanning, release publication, staging, commit, and
push are outside this docs-only scope.
