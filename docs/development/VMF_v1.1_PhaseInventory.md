# VMF v1.1 Phase Inventory

Version : 0.1
Status  : Working Inventory
Scope   : Build.xlam implementation against VMF v1.1 Candidate
Depends : candidates/VMF_v1.1_Candidate.md, candidates/BuildCandidates_v1.1.md

---

# 1. Purpose

This document records the current implementation inventory for the VMF v1.1
Candidate phase model.

This is not a normative specification. It does not modify VMF v1.0, Build v1.0.x,
or any frozen specification.

---

# 2. Summary

The current Build.xlam implementation has source and tests for all VMF v1.1
Candidate phases:

| Phase | Candidate Layer | Current Status | Notes |
|-------|-----------------|----------------|-------|
| Phase 1 | Common | Present | Required artifacts and facade operations are present. |
| Phase 2 | Infrastructure | Present | Required provider artifacts and facade operations are present. |
| Phase 3 | VMF Core | Present | Domain facade, model classes, manifest class, and validator are present. |
| Phase 4 | Build | Present | Required facade and service classes are present. Generation services also exist. |
| Phase 5 | UI | Present | Required facade, composition root, ribbon boundary, and presenter are present. |

The next implementation work should focus on Build v1.1 candidate adoption:

1. B001 Blueprint Parser
2. B002 Manifest Auto Generation
3. B011 Custom Layer Manifest Generation
4. B008 Manifest Validation
5. B007 Template Validation

---

# 3. Phase Inventory

## 3.1 Phase 1: Common

Required candidate artifacts:

| Artifact | Status | Current File |
|----------|--------|--------------|
| ComError | Present | src/Build/Common/ComError.bas |
| ComErrorInfo | Present | src/Build/Common/ComErrorInfo.cls |
| ComResult | Present | src/Build/Common/ComResult.cls |
| ComFacade | Present | src/Build/Common/ComFacade.bas |
| ComCompositionRoot | Present | src/Build/Common/ComCompositionRoot.cls |

Required facade operations are present:

- ComCreateErrorInfo
- ComRaiseError
- ComCreateSuccess
- ComCreateFailure
- ComIsBlankText
- ComRequireText
- ComTrimText

Verification coverage:

- tests/unit/Build/Common/ComCommonPhase1Tests.bas

## 3.2 Phase 2: Infrastructure

Required candidate artifacts:

| Artifact | Status | Current File |
|----------|--------|--------------|
| InfFacade | Present | src/Build/Infrastructure/InfFacade.bas |
| InfCompositionRoot | Present | src/Build/Infrastructure/InfCompositionRoot.cls |
| InfFileSystemProvider | Present | src/Build/Infrastructure/InfFileSystemProvider.cls |
| InfWorkbookProvider | Present | src/Build/Infrastructure/InfWorkbookProvider.cls |

Required facade operations are present:

- InfFileExists
- InfFolderExists
- InfEnsureFolder
- InfReadText
- InfWriteText
- InfGetWorkbookPath

Additional infrastructure artifacts are present:

- InfGenerator
- InfManifestProvider
- InfTemplateProvider
- InfTokenReplacer
- InfVbaProjectProvider
- ManifestItem
- InfTemplate

Verification coverage:

- tests/unit/Build/Infrastructure/InfInfrastructurePhase2Tests.bas

## 3.3 Phase 3: VMF Core

Required candidate artifacts:

| Artifact | Status | Current File |
|----------|--------|--------------|
| DomFacade | Present | src/Build/Domain/DomFacade.bas |
| DomCompositionRoot | Present | src/Build/Domain/DomCompositionRoot.cls |
| DomProject | Present | src/Build/Domain/DomProject.cls |
| DomModuleInfo | Present | src/Build/Domain/DomModuleInfo.cls |
| DomManifest | Present | src/Build/Domain/DomManifest.cls |
| DomValidator | Present | src/Build/Domain/DomValidator.cls |

Required facade operations are present:

- DomCreateProject
- DomCreateModuleInfo
- DomCreateManifest
- DomValidateProject
- DomValidateModuleInfo
- DomValidateManifest

Verification coverage:

- tests/unit/Build/VMF/DomVmfCorePhase3Tests.bas

## 3.4 Phase 4: Build

Required candidate artifacts:

| Artifact | Status | Current File |
|----------|--------|--------------|
| AppFacade | Present | src/Build/Application/AppFacade.bas |
| AppCompositionRoot | Present | src/Build/Application/AppCompositionRoot.cls |
| AppBuildService | Present | src/Build/Application/AppBuildService.cls |
| AppExportService | Present | src/Build/Application/AppExportService.cls |
| AppImportService | Present | src/Build/Application/AppImportService.cls |
| AppValidationService | Present | src/Build/Application/AppValidationService.cls |

Required facade operations are present:

- AppInitialize
- AppShutdown
- AppValidateProject
- AppExportProject
- AppImportProject
- AppBuildProject

Additional Build artifacts are present:

- AppGeneratorService
- BuildGenerationEngine
- Build_ProjectManifest
- ProjectProvider
- ComponentFactory
- HeaderNormalizer
- AttributeWriter
- SourceWriter
- VerificationService

Verification coverage:

- tests/unit/Build/AppBuildPhase4Tests.bas
- tests/unit/Build/AppGeneratorPhase1Tests.bas
- tests/unit/Build/AppGeneratorPhase1NegativeTests.bas
- tests/unit/Build/AppGeneratorPhase2Tests.bas
- tests/unit/Build/AppProjectManifestParseTests.bas
- tests/unit/Build/AppGenerateCommonPhase3_1Tests.bas
- tests/unit/Build/AppGenerateManPhase3_2Tests.bas
- tests/unit/Build/AppGenerateInfraPhase3_3Tests.bas
- tests/unit/Build/AppGenerateDomainPhase3_4Tests.bas
- tests/unit/Build/AppGenerateAppPhase3_5Tests.bas
- tests/unit/Build/AppGeneratePrePhase3_6Tests.bas

## 3.5 Phase 5: UI

Required candidate artifacts:

| Artifact | Status | Current File |
|----------|--------|--------------|
| PreFacade | Present | src/Build/Presentation/PreFacade.bas |
| PreCompositionRoot | Present | src/Build/Presentation/PreCompositionRoot.cls |
| PreRibbon | Present | src/Build/Presentation/PreRibbon.bas |
| PreNotificationPresenter | Present | src/Build/Presentation/PreNotificationPresenter.cls |

Required facade operations are present:

- PreInitialize
- PreShutdown
- PreShowValidationResult
- PreShowBuildResult

Additional UI operations are present:

- PreShowGenerateModuleResult
- PreGenerateModule
- PreGenerateModuleMessage

Verification coverage:

- tests/unit/Build/UI/PreUiPhase5Tests.bas
- tests/unit/Build/UI/PreGenerateModulePhase6Tests.bas

---

# 4. Alignment Status And Follow-Up Tasks

## 4.1 Generation Manifest Alignment

Status : Completed

The layer manifest files under `src/Build/*.manifest` now match current
implemented source file names.

Aligned manifests:

- src/Build/Common.manifest
- src/Build/Manifest.manifest
- src/Build/Infrastructure.manifest
- src/Build/Domain.manifest
- src/Build/Application.manifest
- src/Build/Presentation.manifest

Notes:

- `Manifest.manifest` intentionally keeps the existing Manifest generation phase.
- `DomManifest` remains physically located under `src/Build/Domain`.
- `Domain.manifest` avoids duplicating `DomManifest` to prevent duplicate
  generation when layer manifests are executed together.

## 4.2 Build v1.1 Candidate Implementation Order

The recommended Build v1.1 candidate implementation set is complete.

Future candidate work should remain separate unless explicitly adopted.

Completed preparation:

- Build manifest files are aligned with current implemented source artifacts.
- Infrastructure tests cover the current Build manifest format across all
  `src/Build/*.manifest` files.
- B008 Manifest Validation is implemented through `InfValidateManifestFile` and
  manifest-driven Build layer generation validates before loading items.
- B007 Template Validation is implemented through `InfValidateTemplateFile` and
  generation validates template files before token replacement.
- B011 Custom Layer Manifest Generation is implemented by allowing valid custom
  layer identifiers in manifest items and token replacement, including Core.
- B001 Blueprint Parser is implemented through `Build_BlueprintParser`, which
  parses blueprint text into existing `ManifestItem` generation metadata.
- B002 Manifest Auto Generation is implemented through
  `Build_BlueprintParser.BuildGenerateManifestContent`, which emits
  manifest-format entries from approved blueprint metadata.
- B004 Generate Preview is implemented through
  `AppGeneratorService.AppPreviewBuildLayer` and `AppPreviewBuildLayer`, which
  return generated source previews without creating VBProject components.

## 4.3 Build v2.0 Separation

B005 Source Generator Architecture should remain outside Build v1.1. It should be
tracked as Build v2.0 planning material because it changes the fundamental
generation target and packaging boundary.

---

# 5. Current Decision

Build.xlam is ready to proceed with Build v1.1 candidate adoption planning and
RC1 preparation.

The immediate next task should be:

> Review Build v1.1 candidate implementation, confirm the RC1 scope, and follow
> the Candidate -> RC1 -> official release path recorded in
> `docs/development/Build_v1.1_ReleasePlan.md`.
