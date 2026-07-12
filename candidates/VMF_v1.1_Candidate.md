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

- the VMF v1.x design charter;
- contract-driven architecture as a candidate design policy;
- the distinction between Blueprint Contract, Manifest Contract, and Template Contract;
- the Generator role as a mediator between generation contracts;
- the one-way dependency architecture for generation contracts;
- the template policy that templates define structure only;
- the standard `{{BODY}}` insertion contract for class and standard module templates;
- the Section Contract using `@section`;
- the BodySource and SectionSource reference contract for generated body content;
- the policy that empty sections are not generated;
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

# 5. VMF v1.x Design Charter

VMF v1.x SHALL be governed by the following candidate design charter:

- Architecture is defined by contracts before implementation details.
- Blueprint, Manifest, and Template artifacts each own a distinct contract.
- Generator behavior is valid only when it preserves the contracts it mediates.
- Dependencies flow in one direction from higher coordination concerns toward lower contracts.
- Templates define structural shape only and SHALL NOT own domain behavior.
- Generated output SHALL omit empty optional sections.
- VMF v1.0 remains frozen; this charter is candidate guidance for VMF v1.1.

---

# 6. Contract-Driven Architecture

VMF v1.1 Candidate adopts Contract-Driven Architecture as a design candidate.

The generation pipeline SHALL be understood as a sequence of contracts:

```text
Blueprint Contract
        |
        v
Manifest Contract
        |
        v
Template Contract
        |
        v
Generated Source
```

The contracts SHALL remain separable.

Blueprints describe intended application structure.

Manifests describe concrete generation targets.

Templates describe source structure.

Generated source is the result of contract mediation and SHALL NOT redefine the upstream contracts.

---

# 7. Blueprint / Manifest / Template Contract

## 7.1 Blueprint Contract

The Blueprint Contract describes project intent.

It MAY define:

- application identity;
- canonical layers;
- module groups;
- domain entities;
- application services;
- infrastructure repositories;
- presentation modules or forms.

Blueprint data SHALL NOT depend on template syntax.

## 7.2 Manifest Contract

The Manifest Contract describes generation targets.

Each manifest item SHALL define:

- target module name;
- module type;
- layer name;
- template path.

Manifest data SHALL NOT contain source implementation bodies unless a future candidate explicitly adopts that extension.

Manifest data MAY reference BodySource and SectionSource artifacts when a future
candidate or adopted contract defines those references.

Manifest data SHALL NOT embed implementation bodies directly when a BodySource
or SectionSource contract is available.

## 7.3 Template Contract

The Template Contract describes source structure.

Class and standard module templates SHALL use:

- `{{ModuleName}}` for module identity;
- `{{Layer}}` for layer identity;
- `{{BODY}}` for body insertion.

The legacy `{ModuleName}` and `{Layer}` token forms MAY remain supported for compatibility.

Templates SHALL NOT encode domain behavior.

---

# 8. Generator As Contract Mediator

Generator SHALL be treated as a contract mediator.

Generator SHALL:

- validate template contract availability before generation;
- consume manifest-defined target metadata;
- apply template tokens without changing source contracts;
- omit empty optional sections;
- preserve one-way dependency boundaries.

Generator SHALL NOT:

- reinterpret VMF v1.0;
- introduce higher-layer dependencies into lower-layer contracts;
- place behavioral logic into templates;
- generate empty optional sections.

---

# 9. One-Way Generation Dependencies

Generation contracts SHALL follow this dependency direction:

```text
Blueprint -> Manifest -> Template -> Generated Source
```

Reverse dependencies are prohibited.

Templates SHALL NOT depend on Blueprint structure.

Blueprints SHALL NOT depend on Template internals.

Manifests SHALL bridge Blueprint intent to Template selection without owning generated source behavior.

---

# 10. Template Body Contract

Class and standard module templates SHALL reserve `{{BODY}}` as the only standard body insertion point.

If no body content is supplied, Generator SHALL remove the body placeholder and SHALL NOT emit a placeholder artifact.

The standard template shape is:

```text
Option Explicit

'=========================================================================
' Module or Class: {{ModuleName}}
' Layer: {{Layer}}
' Responsibility:
'=========================================================================

{{BODY}}
```

Domain class and domain module templates SHALL follow the same insertion contract.

---

# 11. Section Contract

The Section Contract defines optional generated sections.

Section syntax:

```text
' @section SectionName
{{SectionName}}
' @endsection
```

Generator SHALL include a section only when the corresponding section content is non-empty.

Generator SHALL remove both section markers and inner section structure when section content is empty.

Section names SHALL be stable contract names, not presentation labels.

---

# 12. BodySource / SectionSource Contract

BodySource and SectionSource artifacts SHALL describe generated body content
inputs without redefining module identity, layer identity, or template selection.

Manifest items SHOULD reference BodySource and SectionSource artifacts by stable
identifier or path.

Generator SHALL resolve those references, assemble body content in the canonical
body section order, and insert the result through `{{BODY}}` or named section
insertion points.

Templates SHALL remain structural and SHALL NOT contain default implementation
body skeletons.

If no body or section content is supplied, Generator SHALL remove the relevant
placeholder and SHALL NOT emit placeholder artifacts.

Detailed BodySource and SectionSource rules are defined by:

- `candidates/BodySourceSectionContract_v1.1_Candidate.md`

---

# 13. Empty Section Policy

Empty optional sections SHALL NOT be generated.

This policy applies to:

- body insertion;
- declaration sections;
- property sections;
- method sections;
- lifecycle sections;
- any future named section introduced by a VMF candidate.

Generated source SHOULD contain only meaningful structure and required VBA declarations.

---

# 14. Documentation Organization Policy

Candidate documentation SHALL be organized by contract:

- design charter and architectural rules;
- Blueprint Contract;
- Manifest Contract;
- Template Contract;
- Generator mediation behavior;
- verification policy.

Future candidate documents SHOULD avoid mixing formal contract rules with implementation notes.

---

# 15. Phase Model

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

# 16. Dependency Direction

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

# 17. Phase 1: Common Contract

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

# 18. Phase 2: Infrastructure Contract

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

# 19. Phase 3: VMF Core Contract

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

# 20. Phase 4: Build Contract

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

# 21. Phase 5: UI Contract

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

# 22. Implementation Rules

Each phase SHALL be completed before the next phase begins.

Each phase SHALL include:

- source files under `src`;
- focused verification tests under `tests`;
- a facade where the phase exposes services;
- a composition root where construction or lifecycle coordination is required;
- no changes to VMF v1.0 normative text.

Public contracts SHALL remain minimal and documented.

Implementation details SHALL remain private to the owning layer.

---

# 23. Verification Rules

Each phase SHOULD be verified by:

- static review of dependency direction;
- public API review;
- error behavior review;
- focused VBA tests where execution is available;
- import/compile validation in Excel/VBE where available.

Tests SHALL NOT depend on private implementation details.

---

# 24. Adoption Policy

This candidate SHALL NOT be treated as adopted VMF v1.0 behavior.

If approved for a future version, this candidate SHOULD become part of a new VMF version rather than modifying VMF v1.0 in place.

---

# 25. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 1.1 Candidate | Candidate | Defines phase implementation contracts for Build.xlam completion |
| 1.1 Candidate Update | Candidate | Adds contract-driven architecture, template body insertion, section contract, and empty-section policy |
| 1.1 Candidate Update | Candidate | Adds BodySource and SectionSource reference contract for generated body content |
