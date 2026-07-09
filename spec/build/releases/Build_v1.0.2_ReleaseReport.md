# Build v1.0.2 Release Report

Version : 1.0.2
Status  : Official Release Report
Scope   : Build.xlam Patch Release
Depends : Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md, BuildQualityStandard_v1.0.md, BuildBlueprint_v1.0.1.md, BuildReleaseProcedure_v1.0.md, BuildReleaseChecklist_v1.0.md

---

# 1. Purpose

This document records the official release report for Build v1.0.2.

Build v1.0.2 is a patch release that fixes target VBProject resolution when Build.xlam commands are executed while Build.xlam is the active workbook.

This patch does not change BuildBlueprint_v1.0.1, BuildCanon_v1.0, BuildDocumentationStandard_v1.0, or BuildQualityStandard_v1.0.

---

# 2. Release Artifacts

The Build v1.0.2 patch release includes the following required release artifacts:

- `dist/build/Build.xlam`
- `src/Build/Infrastructure/InfVbaProjectProvider.cls`
- `tools/build/build.ps1`
- `test/build/Infrastructure/InfInfrastructurePhase2Tests.bas`
- `spec/build/releases/Build_v1.0.2_ReleaseReport.md`

---

# 3. Patch Scope

The patch fixes the following issue:

- `InfVbaProjectProvider.GetActiveVBProject()` depended on `Application.ActiveWorkbook`.
- When a Build.xlam command was executed with Build.xlam active, the provider resolved Build.xlam as the generation target.
- Existing Build modules were then detected in Build.xlam, causing generation to be skipped.

The correction keeps the public provider API unchanged and resolves a target workbook by:

1. Using the active workbook when it is not Build.xlam.
2. Preferring an open `VMF.xlam` when the active workbook is Build.xlam.
3. Falling back to another open workbook that is not Build.xlam.

Build.xlam itself is excluded as a generation target.

---

# 4. Audit Evidence

| Evidence Item | Reference / Location | Notes |
| --- | --- | --- |
| PowerShell build command | `powershell -ExecutionPolicy Bypass -File tools\build\build.ps1` | Completed successfully on 2026-07-09. |
| Latest Build.xlam path | `dist/build/Build.xlam` | Official Build v1.0.2 patch release artifact. |
| Latest Build.xlam timestamp | 2026-07-09 18:01:40 JST | Latest artifact produced by approved PowerShell build. |
| Unit and integration verification | `powershell -ExecutionPolicy Bypass -File tools\test\run-tests.ps1` | All 15 VBA test runners passed. |
| Target VBProject regression coverage | `InfRunInfrastructurePhase2Tests` | Confirms provider does not add generated modules to Build.xlam. |
| Version Verification evidence | `docProps/custom.xml` in `dist/build/Build.xlam` | Build Version = 1.0.2; Release Type = Release. |

---

# 5. Release Audit Results

| Step | Release Audit Item | Result Code | Evidence Reference | Reviewer | Date | Remarks |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Canon Compliance | PASS | Canon_v2.0.md, BuildCanon_v1.0.md, patch source | Codex | 2026-07-09 | Bug fix preserves the frozen Build v1.0 architecture. |
| 2 | Blueprint Compliance | PASS | BuildBlueprint_v1.0.1.md, source modules | Codex | 2026-07-09 | Manifest, template, context, token replacement, and generator responsibilities are unchanged. |
| 3 | Candidate Isolation | PASS | BuildCandidates_v1.1.md, patch source | Codex | 2026-07-09 | No v1.1 Candidate behavior was introduced. |
| 4 | Architecture Verification | PASS | `src/Build/Infrastructure/InfVbaProjectProvider.cls` | Codex | 2026-07-09 | Fix remains within Infrastructure provider responsibility. |
| 5 | Unit Verification | PASS | `tools\test\run-tests.ps1` | Codex | 2026-07-09 | All 15 VBA test runners passed. |
| 6 | Integration Verification | PASS | `tools\test\run-tests.ps1` | Codex | 2026-07-09 | Integrated generation and UI paths passed. |
| 7 | Generator Verification | PASS | Generator test runners | Codex | 2026-07-09 | Generation path continues to use the approved provider and generator flow. |
| 8 | Pipeline Verification | PASS | Build pipeline runners | Codex | 2026-07-09 | Manifest-driven and template-driven pipeline remains unchanged. |
| 9 | Generated Layer Verification | PASS | Layer generation runners | Codex | 2026-07-09 | Common, Manifest, Infrastructure, Domain, Application, and Presentation checks passed. |
| 10 | Documentation Verification | PASS | CHANGELOG.md, README.md, this report | Codex | 2026-07-09 | Patch release evidence is recorded without redefining frozen standards. |
| 11 | Version Verification | PASS | `dist/build/Build.xlam` `docProps/custom.xml` | Codex | 2026-07-09 | Build Version = 1.0.2 and Release Type = Release. |
| 12 | PowerShell Build Artifact Verification | PASS | `tools\build\build.ps1`, `dist/build/Build.xlam` | Codex | 2026-07-09 | Build completed and produced the audited artifact. |
| 13 | Release Evidence Verification | PASS | Section 4 evidence inventory | Codex | 2026-07-09 | Required release evidence is recorded. |
| 14 | Release Decision | PASS | Section 7 release decision | Codex | 2026-07-09 | Audit Decision is APPROVED. |

---

# 6. Issue And Rebuild Records

| Issue ID | Failed Step | Result Code | Issue Summary | Correction | Rebuild Required | Rebuild Evidence | Re-audit From Step 1 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BUILD-1.0.2-001 | N/A | PASS | Build.xlam was incorrectly resolved as the generation target when active. | Updated `InfVbaProjectProvider` target workbook resolution and added regression coverage. | Yes | `tools\build\build.ps1` completed successfully on 2026-07-09. | Completed; all Steps PASS. |

---

# 7. Release Decision

Release Status : Official Release

Audit Decision : APPROVED

Version : 1.0.2

Release Type : Patch Release

Reviewer : Codex

Date : 2026-07-09

Remarks : Build v1.0.2 is approved as the official patch release for target VBProject resolution. BuildBlueprint_v1.0.1, BuildCanon_v1.0, BuildDocumentationStandard_v1.0, and BuildQualityStandard_v1.0 remain unchanged.

---

# 8. Known Issues

No known issues are open for the Build v1.0.2 patch release.

---

# 9. Consistency Confirmation

This Release Report SHALL NOT redefine Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md, BuildQualityStandard_v1.0.md, or BuildBlueprint_v1.0.1.md.

This Release Report records only release evidence, issue and rebuild records, and release decision information for Build v1.0.2.
