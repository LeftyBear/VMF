# Publisher Phase 3-9 Design Notes

This document records implementation-level design decisions for Publisher
Phase 3-9 packaging and release preparation. It is non-normative and does not
modify Frozen specifications, Google API contracts, Identity/Revision
contracts, or Application/Domain public contracts.

## Phase 3-9A Packaging Foundation

### Decision: Framework-Dependent Distribution

Status: Accepted

Publisher Phase 3-9A release packages are distributed for `win-x64` as
framework-dependent ZIP packages. The package contains the Publisher CLI,
managed assemblies, runtime configuration, and dependency metadata, but does
not embed the .NET Runtime.

The deployment target must provide a compatible .NET Runtime before running
`publisher.exe`.

Rationale:

- Reduces distribution package size.
- Allows runtime security and servicing updates to be applied through the
  environment's installed .NET Runtime.
- Matches the current operating environment, where Publisher is built and
  verified as a .NET 8 CLI rather than as a self-contained product installer.

Consequences:

- The Installation Guide must list the required .NET Runtime as a prerequisite.
- The Installation Guide must include a runtime verification step, such as
  confirming the installed runtime before executing `publisher.exe --help`.
- Packaging remains ZIP-based for Phase 3-9A; MSI, code signing, and automatic
  update behavior remain out of scope.

### Decision: Configuration Files Excluded from Release Package

Status: Accepted

`appsettings.json` and `appsettings.local.json` are excluded from the release
ZIP. The package verification process treats the absence of these files as the
expected release-package state.

Rationale:

- Keeps environment-specific configuration separate from the executable
  release artifact.
- Prevents accidental distribution of local or incorrect configuration values.
- Reduces the risk of packaging credentials, token-store paths, folder IDs, or
  other sensitive operational details.

Consequences:

- First-time setup must include explicit instructions for creating the required
  configuration outside the release ZIP.
- `verify-package.ps1` must continue to treat non-bundled configuration files
  as normal and must fail only when secret-like files or secret-like content are
  present in the package.
- Release package verification must not require live Google API credentials or
  mutate Google Docs, Google Drive, token stores, or other external services.

## Phase 3-9B Manifest and Package Verification

### Decision: Local Package Manifest Verification

Status: Accepted

Publisher release ZIP packages include `package-manifest.json` generated at
package time. The manifest records package metadata and a SHA-256 inventory for
all packaged files except the manifest itself.

`tools/publisher/verify-package.ps1` validates the ZIP by extracting it to a
temporary local directory, checking required CLI artifacts, verifying manifest
paths, sizes, and hashes, rejecting unmanifested files, and confirming excluded
configuration files remain absent.

Rationale:

- Provides deterministic package verification without introducing runtime
  dependencies or changing Publisher Application/Domain contracts.
- Keeps release evidence local and repeatable.
- Separates distributable package integrity from Google Docs or Google Drive
  live verification.

Consequences:

- `appsettings.json` and `appsettings.local.json` remain excluded from release
  ZIP packages.
- Verification fails when secret-like filenames or secret-like content are
  present in the ZIP.
- Verification does not require credentials, token stores, live Google APIs, or
  external service mutation.
- MSI, signing, automatic updates, and native installer verification remain out
  of scope for Phase 3-9B.
