# Publisher 0.0.1-dev Release Execution Approval Wording Template

Status  : Template only; approval decision pending
Scope   : Exact wording requirements for future Publisher release execution approval
Date    : 2026-08-11
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md

This document is a documentation-only approval wording template. It is not
release execution approval, does not approve any future release operation, and
does not change the recorded Publisher release state.

## 1. Current Repository State

At template creation time:

- `origin/main` was `e8532d1197a196bfbfece375adf3a2da73b05f23`.
- `HEAD` was `e8532d1197a196bfbfece375adf3a2da73b05f23`.
- `HEAD` equaled `origin/main`.
- The working tree was clean before this template was created.
- Approval decision remains pending.

This template creation does not approve release execution, execute a release,
create or update tags, create or update a GitHub Release, publish artifacts,
run build or test commands, run `vmf-publisher.exe`, modify package/dist/ZIP
output, perform Live E2E, mutate Google Docs or Google Drive, operate on
OAuth/token-store/credentials, or perform any Avast operation.

## 2. Required Future Approval Wording

Future release execution approval for Publisher `0.0.1-dev` must be explicit.
The approval wording must include every field below. Any omitted, implied, or
ambiguous wording is insufficient.

```text
I approve release execution for target version Publisher 0.0.1-dev.

Authorized operation scope:
- [state the exact authorized operation scope]

Tag, release, and publication:
- Tag creation or update is [allowed/not allowed].
- GitHub Release creation or update is [allowed/not allowed].
- Artifact publication is [allowed/not allowed].

Build and test:
- Build execution is [allowed/not allowed].
- Test execution is [allowed/not allowed].

Package/dist/ZIP:
- Package creation or update is [allowed/not allowed].
- dist output mutation is [allowed/not allowed].
- ZIP creation, replacement, or mutation is [allowed/not allowed].

Executable execution:
- vmf-publisher.exe execution is [allowed/not allowed].

Live E2E and Google mutation:
- Live E2E execution is [allowed/not allowed].
- Google Docs mutation is [allowed/not allowed].
- Google Drive mutation is [allowed/not allowed].

OAuth, token store, and credentials:
- OAuth operation is [allowed/not allowed].
- Token-store read/write/delete operation is [allowed/not allowed].
- Credentials operation is [allowed/not allowed].

Avast:
- Avast operation is [allowed/not allowed].

Vendor-clearance acknowledgement:
- I acknowledge that Avast vendor clearance is not obtained.
- I acknowledge that Avast safety certification is not claimed.
```

## 3. Ambiguity Rule

Approval text is insufficient if it says only that release work may proceed,
that the release is approved, that publishing is approved, that the next step is
approved, or that Codex may continue, without explicitly answering each required
field in this template.

Authorization for one operation does not authorize any other operation. For
example, build/test approval does not authorize package mutation, package
approval does not authorize publication, publication approval does not authorize
Google mutation, and repository Git approval does not authorize release
execution.

## 4. Template Boundary

This template is intentionally limited to future approval wording. It records
the required wording shape and the approval boundary only. It does not create a
release decision record, update the current release status, change vendor
clearance, claim Avast safety certification, or replace the existing release
approval package.
