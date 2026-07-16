# VMF Studio v1.1 Release Notes

ProductName : VMF Studio
ProductVersion : 1.1.0
ManifestSchemaVersion : 1.0
TemplateSchemaVersion : 1.0
MinimumSupportedVersion : 1.0
BuildDate : 2026-07-16
Status : Design Frozen

---

## Overview

VMF Studio v1.1 adds an integrated Studio workflow on top of the existing
Build.xlam generation path. The release focuses on safer editing, validation,
preview, generation, backup/restore, settings management, and repeatable
self-checks without changing the frozen VMF v1.0 generation specification.

---

## New Features

- Manifest Editor integration for module and member editing.
- Code Preview generated from the unsaved in-memory manifest model.
- Manifest Validation with stable error codes and ordered results.
- Generate All and Generate Selected workflows with Build Log output.
- Project Explorer with layer/module status, validation state, dirty state,
  and generate target synchronization.
- Template Manager for viewing, editing, validating, saving, and previewing
  existing template files.
- Studio Settings for paths, generate defaults, editor options, and Studio UI
  state.
- Backup/Restore and structured change history for Manifest, Template, and
  Studio Settings files.
- Self Check and reproducible service-level tests for core Studio workflows.

---

## Changes From v1.0

- Existing generator and renderer paths are preserved.
- Preview and Generate use the same render path for equivalent input.
- Validation runs before save, preview, and generation.
- Settings are stored outside the read-only add-in path.
- Release metadata is centralized in `AppReleaseInfo` and recorded in the
  add-in custom document properties.

---

## Compatibility

- VMF v1.0 manifests remain readable by the existing manifest/provider path.
- v1.1 Studio manifests use the same module, layer, type, and template fields
  already consumed by the generator.
- Existing template placeholder and section syntax is preserved.
- Unknown settings keys are warnings, not load blockers.
- Frozen VMF v1.0 specifications are not modified by this release.

---

## Known Constraints

- Template editing does not provide diff/merge or version-control integration.
- Backup/Restore is file based and does not coordinate multiple simultaneous
  editors.
- UI tests cover service and form construction paths; they do not automate
  mouse/keyboard interaction.
- Dedicated `VMF-BAK-*` error code families are reserved for future expansion.

---

## Release Verification

- Build completed from `tools/build/build.ps1`.
- Add-in metadata includes Product Name, Product Version, Build Version,
  Release Type, Manifest Schema Version, Template Schema Version, Minimum
  Supported Version, and Build Date.
- Self Check completed with 12 passed, 0 failed, 0 warnings, and 0 skipped.
- VBA regression tests completed with 17 test runners passed and 0 failed.
- Preview/Generate consistency, Validation, Settings, and Backup/Restore are
  covered by Self Check and regression tests.

---

## Public API And Error Code Review

- Public API inventory is recorded in `PUBLIC_API.md`.
- Error code inventory is recorded in `ERROR_CODES.md`.
- No v1.0 public API removals were made.
- Potential cleanup candidates were not removed during release freeze:
  legacy facade generation helpers, duplicate `IssuesContainErrors` helpers,
  and old preview/generate convenience paths remain for compatibility review in
  a future v1.2 Candidate.

---

## Upgrade

1. Keep the existing v1.0/v1.1 Build.xlam artifact available for rollback.
2. Deploy `VMFStudio_1.1.0.xlam` or the official `Build.xlam` v1.1 artifact.
3. Open VMF Studio and review Settings paths for Manifest, Templates, Output,
   Logs, and Backups.
4. Run Self Check.
5. Validate the project manifest before generation.

---

## Rollback

1. Close Excel.
2. Remove or disable `VMFStudio_1.1.0.xlam`.
3. Re-enable the previous Build.xlam artifact.
4. Restore Manifest, Template, or Settings files from backup if needed.
5. Re-run the previous version's validation/generation workflow.

---

## Design Freeze

VMF Studio v1.1 is frozen at ProductVersion 1.1.0. Future functional changes
must be recorded as VMF Studio v1.2 Candidate work unless they are bug fixes,
security fixes, documentation fixes, or compatibility-preserving minor
improvements.
