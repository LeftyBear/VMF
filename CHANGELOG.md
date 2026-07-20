# Changelog

All notable Build.xlam, VMF, documentation, release artifact, tooling, and test changes are recorded in this file.

---

# 2026-07-20

## Added

- Added Publisher Phase 2-1 ordered, unordered, nested, and mixed Markdown list support.
- Added configurable list indentation and depth normalization with defaults of two spaces and six levels.
- Added Google Docs list rendering with bullet/number presets and post-list index correction.
- Added parser, renderer, and publish-pipeline coverage plus a nested-list live-validation sample.
- Started VMF Studio v2.0 Publisher PoC v0.1 with a .NET solution, layered Publisher library, CLI, unit tests, and integration tests.
- Added minimal Markdown publication support for headings, paragraphs, and unordered-list items through separated Google Drive and Google Docs REST clients.
- Added the Publisher live-validation Markdown sample and local OAuth Desktop configuration example.
- Added installed-application OAuth 2.0 browser authorization for personal Gmail users, including persistent token-store reuse.
- Added configuration-based selection between OAuth Desktop and the existing service-account authentication flow.
- Added OAuth credential validation, cached-token reuse, persisted-token reuse, and authentication-mode unit coverage.
- Added `specs/studio/VMF-Studio-v2.0-Specification-v1.0-Frozen.md`, exported from the frozen Google Docs master specification.
- Added `RELEASE_NOTES_v1.0.md` for the VMF Studio v2.0 Specification v1.0 Frozen Edition.
- Added the Publisher v1.0 Architecture Specification Markdown set under `specs/publisher/`, including the consolidated specification artifact and volumes P0 through P6.
- Added `specs/publisher/schemas/publish-manifest.schema.yaml` as the Publisher v1.0 Publish Manifest schema baseline.

## Changed

- Enabled Shared Drive document creation and safe Google API failure reporting using only the API name, HTTP status, and sanitized error code.
- Enabled OAuth-authenticated document creation in an existing My Drive folder while preserving service-account Shared Drive support.
- Documented OAuth Desktop setup, browser consent, token persistence, service-account compatibility, secret management, and live verification for Publisher PoC v0.1.
- Established the Git repository copy of the Frozen Edition as the official version-controlled specification artifact.
- Advanced the project milestone from Specification Voyage to Implementation Voyage.
- Standardized the Publisher v1.0 specification documents as UTF-8 Markdown, preserved headings and numbered lists, and removed the intermediate DOCX sources after paragraph-level text verification.
- Updated `RELEASE_NOTES_v1.0.md` with the Publisher v1.0 documentation artifacts, source provenance, and conversion verification record.

## Released

- Released VMF Studio v2.0 Specification v1.0.0 (Frozen Edition).

---

# 2026-07-16

## Added

- Added centralized VMF Studio v1.1 release metadata in `AppReleaseInfo`.
- Added generated public API and error code inventories for the v1.1 design freeze.
- Added VMF Studio v1.1 release notes, user guide, manifest compatibility notes, and template compatibility notes.
- Added release documentation generation tooling under `tools/release/`.
- Added versioned VMF Studio v1.1.0 distribution packaging.

## Changed

- Updated `tools/build/build.ps1` to read release metadata from source and record product, schema, minimum supported version, and build date metadata in the add-in.
- Updated README release references for the VMF Studio v1.1.0 distribution artifact.

## Verified

- Rebuilt the add-in with ProductVersion 1.1.0.
- Ran VMF Studio Self Check: 12 passed, 0 failed.
- Ran all VBA regression test runners: 17 passed, 0 failed.

## Released

- Froze VMF Studio v1.1 design at ProductVersion 1.1.0.

---

# 2026-07-12

## Added

- Added the application workspace structure under `applications/SchoolTimetable/`, including manifest, source layer placeholders, docs, and tests.
- Added `.vmf-root` and `src/Build/Common/BuildPathResolver.bas` to support repository-root based path resolution.
- Added project manifest parsing support in `src/Build/Application/Build_ProjectManifest.cls`.
- Added project manifest parse regression coverage in `tests/unit/Build/AppProjectManifestParseTests.bas`.
- Added Build blueprint manifest parsing and generation support through `src/Build/Application/Build_BlueprintParser.cls`.
- Added Build layer preview generation support.
- Added Build v1.1 candidate readiness documentation in `docs/development/Build_v1.1_CandidateReadinessAudit.md`.
- Added VMF v1.1 phase inventory documentation in `docs/development/VMF_v1.1_PhaseInventory.md`.
- Added Build v1.1 release planning documentation in `docs/development/Build_v1.1_ReleasePlan.md`.
- Added Build v1.1 RC1 and official release documentation.
- Added generated SchoolTimetable application class stubs from the application manifest.

## Changed

- Reorganized repository documentation and specifications under `docs/`, `specs/`, `candidates/`, and `templates/`.
- Updated `README.md` and `AGENTS.md` to describe the optimized repository layout and AI development rules.
- Updated `README.md`, `Vision.md`, and `docs/build/ROADMAP.md` for the Build v1.1 official release state.
- Updated Build v1.1 development records to point to the official release report.
- Added a release report index and updated versioning documentation with the current Build v1.1 release.
- Added Build v1.1 post-release verification evidence.
- Archived Build v1.1 candidate dispositions and noted Build v1.1 impacts on VMF v1.1 candidates.
- Corrected Build v1.1 post-release verification to reference the moved SchoolTimetable manifest and recorded the VMF audit result.
- Updated the SchoolTimetable manifest for Build v1.1 and `applications/SchoolTimetable/src` layer paths.
- Updated VMF generation and audit tooling to support the moved SchoolTimetable application manifest.
- Updated Build v1.1 post-release verification with the passing SchoolTimetable VMF audit result.
- Consolidated tests under `tests/` and updated `tools/test/run-tests.ps1` for the reorganized test layout.
- Moved Build templates to `templates/` and updated Build manifest template references.
- Moved Build release artifacts under `dist/release/Build/v1.0.2/`.
- Updated Build tooling and tests to resolve paths from the repository root.
- Updated Build manifest files to align with resolved template paths, project manifests, and layer manifests.
- Updated `AppGeneratorService` to use project manifest input during project generation.
- Updated the Build preview API to return `ComResult`.
- Updated Build v1.1 candidate notes, readiness audit, and phase inventory with RC1 readiness and next-step status.
- Updated `tools/build/build.ps1` to support explicit release metadata for RC and official release artifacts.
- Regenerated the Build v1.0.2 add-in artifact after path-resolution and manifest changes.
- Generated Build v1.1 RC1 and official release artifacts.

## Fixed

- Fixed Build workspace and release artifact path resolution when running from different current directories.
- Fixed manifest template path resolution in `InfManifestProvider` and related Build manifests.
- Fixed project manifest parser support for v1 YAML manifest format.
- Fixed project manifest template path resolution in `Build_ProjectManifest`.
- Hardened Build manifest validation for missing or invalid manifest and template values.
- Rejected empty blueprint generation layers and added regression coverage.
- Made missing-template-path infrastructure coverage runnable in the consolidated test layout.

## Removed

- Removed obsolete `prompt/`, `test/`, and top-level source placeholder directories after repository layout consolidation.
- Removed obsolete top-level `src/Common`, `src/Infrastructure`, and `src/UI` placeholders.
- Removed generated VMF source placeholders and generated sample source modules from `src/VMF/`, leaving layer placeholders for future generation.
- Removed obsolete `dist/release/VMF.xlam`.

## Released

- Released Build.xlam v1.1 as the official Build v1.1 release.

---

# 2026-07-09

## Fixed

- Fixed `InfVbaProjectProvider` target VBProject resolution so Build.xlam is not selected as the generation target when Build.xlam is active.
- Added regression coverage confirming generated modules are not added to Build.xlam.

## Changed

- Updated `tools/build/build.ps1` to record Build Version = 1.0.2 and Release Type = Release in Build.xlam.
- Added `docs/releases/Build_v1.0.2_ReleaseReport.md` and updated README.md to reference the patch release report.

## Released

- Released Build.xlam v1.0.2 as an official patch release.

---

# 2026-07-07

## Added

- Added BuildQualityStandard_v1.0.md as the official Build.xlam release quality standard.
- Added Build v1.0.1 official release records.
- Added Build.xlam release metadata for Build Version and Release Type.

## Changed

- Updated BuildReleaseProcedure_v1.0.md to define the 14 Step Build v1.0.1 release audit.
- Updated Release Procedure steps to use the nine required items, including Inspection Targets, Result Code, and Failure Handling.
- Updated BuildReleaseChecklist_v1.0.md to record final judgments for the 14 Step release audit.
- Updated BuildDocumentationStandard_v1.0.md, BuildBlueprint_v1.0.1.md, README.md, BuildCandidates_v1.1.md, and docs/releases/Build_v1.0.1_ReleaseReport.md for Result Code Standard, Generate Summary evidence, Version Verification, PowerShell-built Build.xlam audit target, and FAIL re-audit handling.
- Updated BuildReleaseChecklist_v1.0.md and docs/releases/Build_v1.0.1_ReleaseReport.md with final PASS results and APPROVED release decision.
- Updated tools/build/build.ps1 to record Build Version = 1.0.1 and Release Type = Release in Build.xlam.

## Released

- Released Build.xlam v1.0.1 as the official Build Release baseline.

---

# 2026-07-05

## Added

- Added BuildReleaseProcedure_v1.0.md as the official release verification procedure.
- Added Release Report as a required official release artifact.
- Added docs/releases/Build_v1.0.1_ReleaseReport.md as the official Build v1.0.1 release report.
- Added BuildDocumentationStandard_v1.0.md as the documentation standard for the Build.xlam official documentation set.
- Added BuildBlueprint_v1.0.1.md as the frozen Build v1.0.1 blueprint.
- Added README.md as the index for the official documentation set.
- Added CHANGELOG.md.

## Changed

- Updated BuildCanon_v1.0.md to reference BuildDocumentationStandard_v1.0.md.
- Updated BuildDocumentationStandard_v1.0.md to include official release requirements.
- Updated BuildDocumentationStandard_v1.0.md to define the Procedure, Checklist, and Report relationship.
- Updated README.md to describe Release Checklist and Release Report reference order.
- Updated README.md to include the Release Procedure and release reference order.
- Updated BuildReleaseChecklist_v1.0.md to require Release Report creation and storage.
- Updated BuildReleaseChecklist_v1.0.md to require checks to be performed according to BuildReleaseProcedure_v1.0.md.
- Separated Build v1.1 Candidate items from BuildCanon_v1.0.md into BuildCandidates_v1.1.md.
- Updated BuildReleaseChecklist_v1.0.md to include BuildBlueprint and BuildDocumentationStandard checks.
