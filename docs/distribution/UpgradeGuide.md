# Publisher Upgrade Guide

Status  : Phase 3-9D Operations Guide
Scope   : VMF Publisher ZIP upgrade and rollback
Depends : docs/distribution/InstallationGuide.md, docs/distribution/ReleaseChecklist.md

This guide describes conservative upgrade operations for VMF Publisher ZIP
deployments. It does not authorize live Google Docs or Google Drive updates.

## 1. Upgrade Principles

- Treat release ZIPs as immutable artifacts.
- Keep local configuration outside the extracted release directory.
- Verify the new package before replacing the active installation.
- Preserve the previous release until the new release passes smoke tests.
- Do not copy credentials, token stores, or local configuration into a release
  package.
- Do not mutate Google Docs or Google Drive during upgrade validation unless
  the live operation is explicitly approved.

## 2. Pre-Upgrade Checklist

Record the current release:

```powershell
C:\Tools\vmf-publisher\vmf-publisher.exe --help
```

Confirm the target package is present and verified:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\publisher\verify-package.ps1 -PackagePath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip
```

Confirm the target host has a compatible .NET 8 Runtime:

```powershell
dotnet --list-runtimes
```

Back up only operational files that are outside the release package, such as
local configuration and token-store directories. Do not write those backups
under the repository or into the release ZIP.

## 3. Side-by-Side Upgrade

Extract the new package to a versioned directory:

```powershell
New-Item -ItemType Directory -Path C:\Tools\vmf-publisher-<version> -Force
Expand-Archive -LiteralPath dist\release\Publisher\vmf-publisher-<version>-win-x64.zip -DestinationPath C:\Tools\vmf-publisher-<version> -Force
```

Run local smoke tests from the new directory:

```powershell
C:\Tools\vmf-publisher-<version>\vmf-publisher.exe --help
C:\Tools\vmf-publisher-<version>\vmf-publisher.exe verify
C:\Tools\vmf-publisher-<version>\vmf-publisher.exe verify .\sample.md
C:\Tools\vmf-publisher-<version>\vmf-publisher.exe dry-run .\sample.md
```

If all checks pass, update the operational launcher, PATH entry, or scheduled
task to reference the new directory.

## 4. In-Place Upgrade

Use in-place replacement only when side-by-side operation is not available.

1. Stop scheduled or automated Publisher invocations.
2. Move the current installation directory to a versioned backup location.
3. Extract the verified new ZIP to the original installation path.
4. Reconnect external configuration by path or environment variables.
5. Run the local smoke tests from Section 3.
6. Restart scheduled or automated invocations only after smoke tests pass.

## 5. Rollback

Rollback is allowed when local smoke tests fail or when live operation evidence
shows a regression.

1. Stop Publisher invocations.
2. Restore the previous release directory or launcher target.
3. Keep the current external configuration and token-store paths unchanged
   unless the failure was caused by configuration.
4. Run `--help`, `verify`, and a local Markdown `dry-run`.
5. Record the rollback reason, failed command, exit code, and restored version.

Do not delete the failed release package until the failure record is complete.

## 6. Compatibility Notes

Phase 3-9 ZIP packages are framework-dependent and require the target runtime
to supply .NET 8. Configuration files remain excluded from release packages.
The `GoogleApi` section is preferred for current configuration; the legacy
`Google` section remains accepted for service-account compatibility.

Persisted publish state, document identity, revision preconditions, and
verified-state promotion rules are not changed by this upgrade procedure.

