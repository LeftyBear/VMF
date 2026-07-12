# VMF v1.1 Candidates

Version : 1.1 Candidate
Status  : Candidate
Scope   : VMF
Depends : VMF_v1.0.md, Canon_v2.0.md

---

# 1. Policy

This document records VMF v1.1 Candidate items.

Candidate items are not part of VMF v1.0 until formally adopted.

VMF v1.0 is frozen and SHALL NOT be modified in place.

---

# 2. Candidates

## 2.1 Post Build v1.1 Note

Build.xlam v1.1 has been officially released.

Build v1.1 improves manifest-driven generation and supports valid custom layer
names, including `Core`, on the Build side.

VMF v1.0 remains frozen. The VMF candidate items below are not adopted VMF v1.1
requirements until a future VMF version is formally released.

---

### V001 Native Core Layer Generation

Status : Proposed
Priority : High
Build v1.1 Impact : Partially supported by Build custom layer manifest generation.

Build v1.0.1 validates generation layers against a fixed layer set and does not accept `Core` as a native generation layer. VMF v1.0 project construction records `Core` in `specs/vmf/manifest.yaml`, while future VMF tooling SHOULD support `Core` as a first-class generation target without changing VMF v1.0.

### V002 Manifest YAML Native Reader

Status : Proposed
Priority : Medium
Build v1.1 Impact : Not adopted as a native VMF manifest reader.

VMF project generation currently uses a PowerShell wrapper to read `specs/vmf/manifest.yaml`. A future VMF version SHOULD define an official manifest reader contract for YAML-based project manifests.

### V003 Native Enum Generation

Status : Proposed
Priority : Medium
Build v1.1 Impact : Not adopted.

Build v1.0.2 generates Class Modules and Standard Modules but does not provide a native template or generation pipeline for Enum definitions. VMF v1.0 therefore excludes Enum generation from the official generation scope while preserving Enum definitions in the design. A future VMF version SHOULD provide native Enum templates, generation support, and manifest validation for Enum components without changing VMF v1.0.

### V004 Manifest Generation Contract

Status : Proposed
Priority : Medium
Build v1.1 Impact : Partially related through Build blueprint-to-manifest generation, but not adopted as a VMF generation contract.

VMF v1.0 defines generated components under a `Modules` section in `specs/vmf/manifest.yaml`. This structure is sufficient for documentation and auditing but is not optimized as a direct generation contract. A future VMF version SHOULD define a native generation contract that explicitly describes generation targets, component types, layers, and generation options, allowing Build tooling to consume the manifest directly without changing VMF v1.0.

### V005 VMF v1.x Design Charter

Status : Proposed
Priority : High
Build v1.1 Impact : Implemented as candidate documentation only.

A future VMF version SHOULD adopt a v1.x design charter that states architecture is contract-driven, VMF v1.0 remains frozen, and future changes enter through candidate contracts before adoption.

### V006 Contract-Driven Generation Architecture

Status : Proposed
Priority : High
Build v1.1 Impact : Partially supported by the current Blueprint, Manifest, Template, and Generator implementation.

A future VMF version SHOULD define Blueprint Contract, Manifest Contract, and Template Contract as separate generation contracts. Generator SHOULD be defined as the mediator between those contracts rather than as the owner of the generated design.

### V007 Template Body And Section Contract

Status : Proposed
Priority : High
Build v1.1 Impact : Candidate implementation added to templates and template replacement.

A future VMF version SHOULD standardize `{{BODY}}` as the body insertion token for Class Module and Standard Module templates. It SHOULD also define `@section` / `@endsection` as the optional Section Contract and require empty sections to be omitted from generated source.

### V008 Canonical Template Contract

Status : Proposed
Priority : High
Build v1.1 Impact : Candidate documentation and template assets only.

A future VMF version SHOULD adopt canonical templates for Class Module, Predeclared Class Module, Standard Module, Interface, Enum, and UserForm code-behind generation. Template selection SHOULD be determined by the Manifest Contract, and module attributes such as `VB_PredeclaredId` SHOULD remain template-owned rather than rewritten by Generator.

Candidate contract:

- `candidates/TemplateContract_v1.1_Candidate.md`

Candidate template assets:

- `candidates/templates/v1.1/ClassModuleTemplate.txt`
- `candidates/templates/v1.1/PredeclaredClassModuleTemplate.txt`
- `candidates/templates/v1.1/StandardModuleTemplate.txt`
- `candidates/templates/v1.1/InterfaceTemplate.txt`
- `candidates/templates/v1.1/EnumTemplate.txt`
- `candidates/templates/v1.1/UserFormTemplate.frm.txt`

### V009 Body Source / Section Source Contract

Status : Proposed
Priority : High
Build v1.1 Impact : Not adopted.

A future VMF version SHOULD define BodySource and SectionSource contracts as the
source of generated implementation body content. Manifest items SHOULD reference
body and section sources rather than embedding implementation bodies directly.
Templates SHOULD remain structural and receive generated content only through
`{{BODY}}` and the Section Contract.

Candidate contract:

- `candidates/BodySourceSectionContract_v1.1_Candidate.md`
