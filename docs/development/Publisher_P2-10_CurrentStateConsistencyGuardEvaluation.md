# Publisher P2-10 Current-State Consistency Guard Evaluation

Status  : Complete / narrow local-only implementation with focused unit coverage
Scope   : Evaluate and complete a docs/static/package-independent current-state consistency guard
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_vNext_Backlog.md, src/Publisher/Application/CurrentStateConsistencyGuard.cs, tests/unit/Publisher/CurrentStateConsistencyGuardTests.cs

This record uses the user-requested `P2-10 Current-State Consistency Guard`
label. The existing backlog already contains a completed `P2-10 Safe Retry
Diagnostics` item; this record does not rewrite or supersede that historical
item. Any backlog synchronization should preserve the existing safe-retry
record and separately identify this guard scope.

## 1. Decision

GO for a narrow local-only helper that compares explicit, caller-supplied
current-state claims against an allow-listed current-state manifest.

The helper is static, package-independent, and source/test local. It does not
scrape broad Markdown, infer current release clearance from historical
completion wording, inspect packages, run executables, call Google Docs or
Google Drive, use OAuth, read or write token stores, run Live E2E, update
`dist/`, operate releases/tags/publication, operate Avast, or decide vendor
clearance.

## 2. Current-State Boundary

The guarded boundary for this scope is:

- release remains blocked for the selected current-state basis supplied to the
  guard;
- Avast false-positive handling remains pending;
- Avast vendor clearance is not obtained;
- Live E2E completion must not be inferred unless an allow-listed current-state
  claim explicitly says so.

Historical completion, publication, closeout, owner-decision, local
verification, or risk-acceptance text must remain historical or scope-specific
unless the allow-listed current-state manifest accepts it as the current value.

## 3. Implemented Behavior

The guard:

- accepts only allow-listed source paths;
- accepts current-state source records only;
- accepts only manifest-listed claim names;
- compares claim values against a closed vocabulary and the expected current
  value;
- returns only bounded statuses: `Match` and `Conflict`;
- returns only bounded diagnostics:
  `SourceNotAllowListed`, `SourceKindNotPermitted`, `ClaimNotAllowListed`, and
  `Conflict`;
- omits raw claim values from diagnostics.

## 4. Conflict Conditions

The guard reports `Conflict` when an allow-listed current-state source claims a
state that contradicts the supplied current-state manifest, including
release-cleared, vendor-cleared, or Live-E2E-complete wording when the manifest
keeps those states blocked, not obtained, or not complete.

Historical sources are rejected as non-current and cannot promote current
release clearance.

## 5. Verification

Required verification for this implementation:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --filter "FullyQualifiedName~CurrentStateConsistencyGuard"
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release
dotnet build VMF.Publisher.sln --configuration Release
dotnet format VMF.Publisher.sln --verify-no-changes
git diff --check
```

No Live E2E, Google Docs / Drive, OAuth, token-store, package, `dist`, release,
tag, publication, Avast, or flagged-executable operation is part of this scope.
