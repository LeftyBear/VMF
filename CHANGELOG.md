# Changelog

All notable Build.xlam, VMF, documentation, release artifact, tooling, and test changes are recorded in this file.

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

## Changed

- Reorganized repository documentation and specifications under `docs/`, `specs/`, `candidates/`, and `templates/`.
- Updated `README.md` and `AGENTS.md` to describe the optimized repository layout and AI development rules.
- Updated `README.md`, `Vision.md`, and `docs/build/ROADMAP.md` for the Build v1.1 official release state.
- Updated Build v1.1 development records to point to the official release report.
- Added a release report index and updated versioning documentation with the current Build v1.1 release.
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
