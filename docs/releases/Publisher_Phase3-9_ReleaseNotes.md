# Publisher Phase 3-9 Release Notes

Date: 2026-07-29

## Scope

Phase 3-9 completes Publisher packaging and release operations. Phase 3-9D
adds operational documentation for installation, upgrade, release checklist,
and controlled Live E2E verification.

This phase does not change Frozen specifications, Google API contracts,
identity/revision contracts, Application/Domain public contracts, persisted
state schemas, or live external service defaults.

## Distribution Model

- Publisher is distributed as a `win-x64` framework-dependent ZIP package.
- The target host must provide a compatible .NET 8 Runtime.
- Release packages include `package-manifest.json` with SHA-256 inventory.
- `appsettings.json` and `appsettings.local.json` are intentionally excluded
  from release packages.
- MSI installers, code signing, automatic updates, and self-contained runtime
  packages remain out of scope for Phase 3-9.

## Operations Documentation

Added:

- `docs/distribution/InstallationGuide.md`
- `docs/distribution/UpgradeGuide.md`
- `docs/distribution/ReleaseChecklist.md`
- `docs/distribution/LiveE2EOperations.md`

These documents define package verification, install smoke tests, upgrade and
rollback flow, release evidence, explicit Live E2E authorization, secret
handling, and cleanup expectations.

## Safety And Compatibility

- Live Google Docs and Google Drive updates remain disabled unless explicitly
  authorized for a specific operation.
- Credentials, token stores, local configuration, and secret-bearing files must
  remain outside the repository and outside release ZIP packages.
- Package verification rejects missing required files, unsafe manifest paths,
  size or SHA-256 mismatches, unmanifested files, bundled configuration files,
  secret-like filenames, and secret-like content.
- Local dry-run, package verification, and non-live tests must not be reported
  as live Google Docs readback.

## Verification Checklist

- Release build: `dotnet build VMF.Publisher.sln --configuration Release --no-restore`
- Unit tests: `dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore`
- Integration tests: `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore`
- Package creation: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\package-publisher.ps1 -Version <version>`
- Package verification: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip`
- Format/static: `dotnet format VMF.Publisher.sln --verify-no-changes --no-restore`
  and `git diff --check`
- Live E2E: execute only with explicit authorization according to
  `docs/distribution/LiveE2EOperations.md`.

## Release Status

Phase 3-9D documentation is ready for review as release operations
documentation. Official release approval still requires completing
`docs/distribution/ReleaseChecklist.md` with concrete evidence for the selected
release artifact.

