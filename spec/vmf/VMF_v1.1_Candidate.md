# VMF v1.1 Candidate

**Document ID**: SPEC-VMF-v1.1-CANDIDATE

**Version**: 1.1 Candidate

**Status**: Candidate

**Type**: Implementation Candidate Specification

**Encoding**: UTF-8

**Format**: Markdown (GitHub Compatible)

---

# 1. Purpose

This document defines a candidate implementation contract for completing Build.xlam in phase units while preserving VMF v1.0 compatibility.

VMF v1.0 remains frozen. This candidate does not modify, supersede, or reinterpret VMF v1.0. It records additive implementation guidance that may be adopted as a future VMF version after review.

---

# 2. Scope

This candidate defines:

- phase names for implementation planning;
- the mapping between implementation phase names and canonical architectural layers;
- the minimum public contracts required to implement each phase;
- facade and composition root responsibilities for each phase;
- dependency direction for phase-by-phase implementation.

This candidate does not define detailed algorithms, UI design, storage formats, deployment procedures, or Excel workbook packaging.

---

# 3. Normative References

This candidate is intended to remain consistent with:

- Canon v2.0
- VMF v1.0
- Architecture v2.0
- LayerSpecification v2.0
- DependencySpecification v2.0
- ModuleSpecification v2.0
- ClassSpecification v2.0
- InterfaceSpecification v2.0
- FacadeSpecification v2.0
- CompositionRootSpecification v2.0
- ApiSpecification v2.0
- ErrorHandling v2.0
- NamingConvention v2.0

If this candidate conflicts with VMF v1.0 or any higher-priority specification, the higher-priority specification remains authoritative.

---

# 4. Candidate Status

This document is a VMF v1.1 Candidate.

Requirements in this document are not VMF v1.0 requirements until formally adopted by a future VMF version.

Implementation may use this candidate as guidance only when doing so does not violate VMF v1.0.

---

# 5. Phase Model

Implementation SHALL proceed in the following phases.

| Phase | Implementation Name | Canonical Layer | Layer Prefix |
|-------|---------------------|-----------------|--------------|
| Phase 1 | Common | Common | Com |
| Phase 2 | Infrastructure | Infrastructure | Inf |
| Phase 3 | VMF Core | Domain | Dom |
| Phase 4 | Build | Application | App |
| Phase 5 | UI | Presentation | Pre |

The implementation names describe repository planning phases. The canonical layer and prefix SHALL govern source identifiers.

The candidate mapping avoids introducing new architectural layers under VMF v1.0.

---

# 6. Dependency Direction

Phase dependencies SHALL follow the canonical layer dependency matrix.

| Phase | Permitted Dependencies |
|-------|------------------------|
| UI | Build, Common |
| Build | VMF Core, Infrastructure through contracts where applicable, Common |
| VMF Core | Common |
| Infrastructure | Common |
| Common | None |

Circular dependencies are prohibited.

Lower phases SHALL NOT depend on higher phases.

---

# 7. Phase 1: Common Contract

## 7.1 Responsibility

Common provides reusable technical utilities that are independent of business domains, Excel UI behavior, storage mechanisms, and infrastructure implementations.

## 7.2 Required Artifacts

| Artifact | Type | Responsibility |
|----------|------|----------------|
| ComError | Standard Module | Common error identifiers |
| ComErrorInfo | Class Module | Immutable project error information |
| ComResult | Class Module | Operation result value |
| ComFacade | Standard Module | Published Common entry point |
| ComCompositionRoot | Class Module | Common lifecycle composition boundary |

## 7.3 Public Contract

The Common facade SHALL expose:

- `ComCreateErrorInfo`
- `ComRaiseError`
- `ComCreateSuccess`
- `ComCreateFailure`
- `ComIsBlankText`
- `ComRequireText`
- `ComTrimText`

The Common composition root SHALL expose:

- `ComInitialize`
- `ComShutdown`

## 7.4 Error Contract

Common error identifiers SHALL be declared by `ComErrNum`.

Required members:

- `ComErrInvalidArgument`
- `ComErrInvalidState`

---

# 8. Phase 2: Infrastructure Contract

## 8.1 Responsibility

Infrastructure provides technical implementations for file system, workbook, and environment interaction while hiding implementation details behind stable contracts.

## 8.2 Required Artifacts

| Artifact | Type | Responsibility |
|----------|------|----------------|
| InfFacade | Standard Module | Published Infrastructure entry point |
| InfCompositionRoot | Class Module | Infrastructure composition boundary |
| InfFileSystemProvider | Class Module | File system operations |
| InfWorkbookProvider | Class Module | Workbook access operations |

## 8.3 Public Contract

The Infrastructure facade SHOULD expose only operations required by Build phase use cases.

Candidate public operations:

- `InfFileExists`
- `InfFolderExists`
- `InfEnsureFolder`
- `InfReadText`
- `InfWriteText`
- `InfGetWorkbookPath`

Infrastructure SHALL depend only on Common.

---

# 9. Phase 3: VMF Core Contract

## 9.1 Responsibility

VMF Core owns project model concepts, manifest concepts, module metadata concepts, and validation rules that are independent of Excel UI and infrastructure storage.

## 9.2 Required Artifacts

| Artifact | Type | Responsibility |
|----------|------|----------------|
| DomFacade | Standard Module | Published VMF Core entry point |
| DomCompositionRoot | Class Module | VMF Core composition boundary |
| DomProject | Class Module | Project identity and root metadata |
| DomModuleInfo | Class Module | Module metadata |
| DomManifest | Class Module | Manifest metadata |
| DomValidator | Class Module | Domain validation rules |

## 9.3 Public Contract

The VMF Core facade SHOULD expose:

- `DomCreateProject`
- `DomCreateModuleInfo`
- `DomCreateManifest`
- `DomValidateProject`
- `DomValidateModuleInfo`
- `DomValidateManifest`

VMF Core SHALL depend only on Common.

---

# 10. Phase 4: Build Contract

## 10.1 Responsibility

Build coordinates application use cases for generating, validating, importing, exporting, and packaging Build.xlam project artifacts.

## 10.2 Required Artifacts

| Artifact | Type | Responsibility |
|----------|------|----------------|
| AppFacade | Standard Module | Published Build entry point |
| AppCompositionRoot | Class Module | Build composition boundary |
| AppBuildService | Class Module | Build use case coordination |
| AppExportService | Class Module | Source export coordination |
| AppImportService | Class Module | Source import coordination |
| AppValidationService | Class Module | Application-level validation coordination |

## 10.3 Public Contract

The Build facade SHOULD expose:

- `AppInitialize`
- `AppShutdown`
- `AppValidateProject`
- `AppExportProject`
- `AppImportProject`
- `AppBuildProject`

Build MAY depend on VMF Core, Infrastructure through contracts where applicable, and Common.

Build SHALL NOT directly manipulate Excel UI.

---

# 11. Phase 5: UI Contract

## 11.1 Responsibility

UI provides presentation behavior for Excel users and delegates use cases to the Build facade.

## 11.2 Required Artifacts

| Artifact | Type | Responsibility |
|----------|------|----------------|
| PreFacade | Standard Module | Published Presentation entry point |
| PreCompositionRoot | Class Module | Presentation composition boundary |
| PreRibbon | Standard Module | Ribbon callback boundary |
| PreNotificationPresenter | Class Module | User notification behavior |

## 11.3 Public Contract

The UI facade SHOULD expose:

- `PreInitialize`
- `PreShutdown`
- `PreShowValidationResult`
- `PreShowBuildResult`

UI MAY depend on Build and Common.

UI SHALL NOT contain business rules.

---

# 12. Implementation Rules

Each phase SHALL be completed before the next phase begins.

Each phase SHALL include:

- source files under `src`;
- focused verification tests under `test`;
- a facade where the phase exposes services;
- a composition root where construction or lifecycle coordination is required;
- no changes to VMF v1.0 normative text.

Public contracts SHALL remain minimal and documented.

Implementation details SHALL remain private to the owning layer.

---

# 13. Verification Rules

Each phase SHOULD be verified by:

- static review of dependency direction;
- public API review;
- error behavior review;
- focused VBA tests where execution is available;
- import/compile validation in Excel/VBE where available.

Tests SHALL NOT depend on private implementation details.

---

# 14. Adoption Policy

This candidate SHALL NOT be treated as adopted VMF v1.0 behavior.

If approved for a future version, this candidate SHOULD become part of a new VMF version rather than modifying VMF v1.0 in place.

---

# 15. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 1.1 Candidate | Candidate | Defines phase implementation contracts for Build.xlam completion |
