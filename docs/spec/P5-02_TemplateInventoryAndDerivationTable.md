# P5-02 - Template Inventory Review And Derivation Table

## Status

COMPLETE / docs-only inventory and derivation table

## Purpose

Record the concrete Template inventory and the deterministic Manifest fact to
Template binding data table after P5-01 fixed the Manifest-only Template
Derivation boundary.

P5-02 is documentation only. It does not authorize implementation GO.

## Scope

P5-02 records:

- existing Template files and Template-related implementation surfaces reviewed
- Manifest facts relevant to Template Derivation
- deterministic Template binding mappings
- unsupported, ambiguous, missing, and deferred mapping cases
- mapping characteristics such as `1:1`, `1:N`, `N:1`, `derived`, and
  `unsupported`
- allowed and prohibited information sources
- concrete hard-stop conditions on the derivation table
- confirmation that GenerateContext and Generator responsibilities remain
  downstream
- open items needed before output model planning or implementation scope

## Non-Scope

P5-02 does not perform or authorize:

- production VBA code changes
- test code additions or updates
- Template file changes
- Template Mapping or Template Derivation implementation
- GenerateContext changes
- Generator changes
- runtime behavior changes
- public API changes
- persisted schema changes
- refactoring
- package, `dist`, build, release, or external service operations
- Git staging, commit, or push
- Frozen specification changes
- implementation GO

## Reviewed Inputs

| Source | Role in review |
| --- | --- |
| `docs/spec/P5-01_TemplateDerivationScopePlanning.md` | Authoritative P5 Template Derivation boundary for this task. |
| `docs/spec/P4-05_TemplateMappingContractFreeze.md` | Prior Manifest to Template binding contract. |
| `docs/spec/P4-06_GenerateContextResponsibilityBoundaryFreeze.md` | Downstream GenerateContext boundary. |
| `docs/spec/P4-07_GeneratorInputContractScopePlanning.md` | Generator input boundary. |
| `docs/VMF_vNext_Backlog.md` | Build vNext status and scope record. |
| `docs/development/CURRENT_STATUS.md` | Current status synchronization record. |
| `templates/ModuleTemplate.txt` | Existing standard module template file. |
| `templates/ClassTemplate.txt` | Existing non-Domain class template file. |
| `templates/DomainClassTemplate.txt` | Existing Domain class template file. |
| `templates/DomainModuleTemplate.txt` | Existing Domain module template file; not selected by current derivation rules. |
| `src/Build/Infrastructure/ManifestItem.cls` | Current Manifest item fields consumed by Generator. |
| `src/Build/Application/BlueprintManifestDeriver.cls` | Current P4 Manifest Derivation behavior and TemplatePath selection. |
| `src/Build/Application/Build_ProjectManifest.cls` | Current manifest parsing and legacy TemplatePath resolution. |
| `src/Build/Infrastructure/InfTemplateProvider.cls` | Current Template file loading and minimal token validation. |
| `src/Build/Infrastructure/InfGenerator.cls` | Current downstream template token and section consumption behavior. |

## Inventory Review Result

| Template file | Observed template kind | Required core tokens observed | Section tokens observed | Current derivation selection | Review result |
| --- | --- | --- | --- | --- | --- |
| `templates/ModuleTemplate.txt` | Standard module | `{{ModuleName}}`, `{{Layer}}`, `{{BODY}}` | `ModuleDeclaration`, `DeclarationSectionEnd`, `ProcedureGroupStart`, `ProcedureGroupEnd` | Selected for `ModuleType = StandardModule`, all supported layers. | Supported by deterministic `ModuleType` mapping. |
| `templates/ClassTemplate.txt` | Class module | `{{ModuleName}}`, `{{Layer}}`, `{{BODY}}` | `ModuleDeclaration`, `MemberBlock`, `DeclarationSectionEnd`, `InitializeBody`, `TerminateBody`, `PropertyGroupStart`, `Properties`, `PropertyGroupEnd`, `ProcedureGroupStart`, `ProcedureGroupEnd` | Selected for `ModuleType = ClassModule` when `LayerName <> Domain`. | Supported by deterministic `ModuleType + LayerName` mapping. |
| `templates/DomainClassTemplate.txt` | Domain class module | `{{ModuleName}}`, `{{Layer}}`, `{{BODY}}` | Same observed section tokens as `ClassTemplate.txt`. | Selected for `ModuleType = ClassModule` and `LayerName = Domain`. | Supported by deterministic Domain class specialization. |
| `templates/DomainModuleTemplate.txt` | Standard module shape | `{{ModuleName}}`, `{{Layer}}`, `{{BODY}}` | Same observed section tokens as `ModuleTemplate.txt`. | No current P4 / legacy derivation rule selects this file. | Existing but unsupported for Template Derivation until an approved rule exists. |

Inventory conclusions:

- The current Template inventory contains four text files.
- The current deterministic selection rules approve three Template identities:
  `ModuleTemplate.txt`, `ClassTemplate.txt`, and `DomainClassTemplate.txt`.
- `DomainModuleTemplate.txt` is a real Template file but has no approved
  Manifest-only selection rule in current P4 / P5 records.
- Template file contents are not an allowed source of design intent for
  Template Derivation. They are inventory evidence only.
- The current Template validation surface checks only usable content plus
  required `ModuleName`, `Layer`, and `BODY` tokens. It does not define a
  complete future Template Derivation output model.

## Allowed Information Sources

Template Derivation may use only:

- the P4-derived and verified Manifest
- approved Template inventory identities
- approved Manifest-to-Template derivation rules recorded by planning records

Template Derivation must not read or depend on:

- raw Blueprint text
- unvalidated parsed Blueprint state
- Validator diagnostics
- Manifest Derivation diagnostics except as prior hard-stop state
- generated VBA output
- Template file contents as a source of design intent
- GenerateContext state
- Generator behavior
- external runtime state as a source of design intent

## Concrete Template Derivation Table

| Manifest fact / field | Required? | Mapping characteristic | Template binding data | Deterministic result | Hard-stop condition |
| --- | --- | --- | --- | --- | --- |
| Manifest derivation success state | Yes | hard-stop gate | Eligibility to begin Template Derivation | Start only after successful P4-derived Manifest. | Missing, failed, partial, non-P4-derived, or diagnostic hard-stop state. |
| Manifest item order | Yes, when multiple items exist | `1:1` preserved order | Ordered generation-unit binding list | Preserve Manifest item order exactly. | Ordering absent, unstable, or conflicting. |
| `ModuleName` | Yes | `1:1` carry-through | Binding trace key and downstream module fact | Carry trimmed Manifest value without repair. | Missing, blank, object value, duplicate where uniqueness is required, or invalid upstream state. |
| `ModuleType = StandardModule` | Yes | `1:1` selection | `TemplateIdentity = ModuleTemplate.txt` | Select module Template for all supported layers. | Unsupported `ModuleType`, blank `ModuleType`, or multiple matching Templates. |
| `ModuleType = ClassModule` and `LayerName = Domain` | Yes | `N:1` selection from `ModuleType + LayerName` | `TemplateIdentity = DomainClassTemplate.txt` | Select Domain class Template. | Missing `LayerName`, unsupported layer, unsupported `ModuleType`, or more than one Domain class rule. |
| `ModuleType = ClassModule` and `LayerName <> Domain` | Yes | `N:1` selection from `ModuleType + LayerName` | `TemplateIdentity = ClassTemplate.txt` | Select standard class Template for Common, Core, Application, Infrastructure, and Presentation. | Missing `LayerName`, unsupported layer, unsupported `ModuleType`, or more than one class rule. |
| `LayerName` | Yes | `1:N` participation | Template selection plus downstream layer fact | Use only supported layer values already present in Manifest. | Missing, blank, unsupported, or requiring normalization / inference. |
| `TemplatePath` already present in Manifest | Yes for current Manifest consumer compatibility | `1:1` compatibility / consistency check | Template identity / path trace | Accept only when it matches the approved deterministic rule for `ModuleType + LayerName`. | Missing, blank, nonexistent under approved inventory, or conflicting with deterministic selection. |
| `BodySourcePath` | Optional current Manifest fact | `1:1` carry-through when present | Optional body source metadata for downstream context | Carry only if already present and supported by future GenerateContext planning. | Required by selected future output model but absent; present but unsupported by approved rules. |
| `SectionSourcePaths` | Optional current Manifest fact | `1:N` by section name | Optional section binding metadata | Carry only approved section names that match known insertion sections. | Unknown section, duplicate section, unsupported path source, or section data needed to repair Template output. |
| `MemberSourcePath` / `MemberSourceText` | Optional current Manifest facts | `1:N` derived section source for class member generation | Optional class member source metadata | Deferred to future output model planning; current Template Derivation must not synthesize member sections. | Needed for class binding but absent from approved Manifest data, or requires Generator behavior to derive meaning. |
| Procedure facts | Relevant by P5-01 but not present in current `ManifestItem` output | `unsupported` until carried by Manifest | No concrete binding data in P5-02 | Unsupported for concrete Template Derivation from current Manifest surface. | Any Template decision requires procedure kind, visibility, parameters, return value, or responsibility not present in Manifest. |
| Parameter facts | Relevant by P5-01 but not present in current `ManifestItem` output | `unsupported` until carried by Manifest | No concrete binding data in P5-02 | Unsupported for concrete Template Derivation from current Manifest surface. | Any Template decision requires parameter facts not present in Manifest. |
| Return value facts | Relevant by P5-01 but not present in current `ManifestItem` output | `unsupported` until carried by Manifest | No concrete binding data in P5-02 | Unsupported for concrete Template Derivation from current Manifest surface. | Any Template decision requires return value facts not present in Manifest. |
| Dependency facts | Relevant by P5-01 but not present in current `ManifestItem` output | `unsupported` until carried by Manifest | No concrete binding data in P5-02 | Unsupported for concrete Template Derivation from current Manifest surface. | Any Template decision requires dependency facts not present in Manifest. |
| Generation policy facts | Relevant by P5-01 but not present in current `ManifestItem` output | `unsupported` until carried by Manifest | No concrete binding data in P5-02 | Unsupported for concrete Template Derivation from current Manifest surface. | Any Template decision requires overwrite, encoding, directory policy, or other policy facts not present in Manifest. |
| `DomainModuleTemplate.txt` | No approved current selection | `unsupported` | No binding | Not selected. | Any attempt to select it without a separate approved Manifest-only rule. |
| Template fallback | No | `unsupported` | No binding | No fallback behavior is approved. | No exact approved Template match, more than one match, or selection requiring fallback. |

## Deterministic Mapping Set

| Condition | Template identity | Characteristic |
| --- | --- | --- |
| `ModuleType = StandardModule` | `ModuleTemplate.txt` | `1:1` from supported module type, independent of layer. |
| `ModuleType = ClassModule` and `LayerName = Domain` | `DomainClassTemplate.txt` | `N:1` from module type plus layer specialization. |
| `ModuleType = ClassModule` and `LayerName` is Common, Core, Application, Infrastructure, or Presentation | `ClassTemplate.txt` | `N:1` from module type plus non-Domain layer. |

No other Template selection is approved by P5-02.

## Unsupported / Ambiguous / Deferred Items

Unsupported items:

- `DomainModuleTemplate.txt` selection
- unsupported `ModuleType` values beyond `StandardModule` and `ClassModule`
- unsupported layer values beyond Common, Core, Domain, Application,
  Infrastructure, and Presentation
- procedure-level Template binding from the current Manifest surface
- parameter, return value, dependency, and generation-policy Template binding
  from the current Manifest surface
- Template fallback behavior

Ambiguous items:

- a Manifest `TemplatePath` that conflicts with the deterministic
  `ModuleType + LayerName` selection
- multiple approved Template rules matching the same Manifest item
- a Template decision that would require reading Template contents to infer
  intent

Deferred items:

- concrete Template Derivation output model
- concrete Template Derivation entry point
- whether future Manifest Derivation should carry procedure, parameter, return
  value, dependency, and generation policy facts into the Manifest surface used
  by Template Derivation
- whether `BodySourcePath`, `SectionSourcePaths`, `MemberSourcePath`, and
  `MemberSourceText` belong in the future Template Derivation output model or
  only in GenerateContext planning
- focused local test file names for a future implementation task
- any approved rule, if ever needed, for `DomainModuleTemplate.txt`

## Hard-Stop Confirmation

Template Derivation must hard-stop before GenerateContext when:

- any required Manifest item field is missing or blank
- `ModuleType`, `LayerName`, or `TemplatePath` cannot be matched to exactly one
  approved Template identity
- current Manifest data is insufficient for a requested procedure, parameter,
  return value, dependency, or generation-policy binding
- selection requires raw Blueprint, Validator diagnostics, Manifest Derivation
  diagnostics, Template content inference, GenerateContext state, Generator
  behavior, external runtime state, repair, normalization, or fallback
- `TemplatePath` conflicts with the approved deterministic mapping result
- ordering cannot be preserved
- downstream GenerateContext would need additional ad hoc facts to compensate
  for missing Template binding data

Template Derivation failure remains distinct from parse failure, validation
failure, Manifest Derivation failure, GenerateContext construction failure, and
Generator failure.

## GenerateContext / Generator Leakage Review

No GenerateContext or Generator responsibility is assigned to Template
Derivation by this table.

Template Derivation may select Template identities and carry supported
Manifest-derived binding data. It must not:

- construct GenerateContext
- decide GenerateContext schema
- synthesize member sections or procedure bodies
- invoke `InfGenerator`
- validate runtime Template file contents as proof of upstream design validity
- produce generated VBA text
- enforce write, overwrite, or file-system policy

GenerateContext remains responsible for packaging successful Template binding
output and approved Manifest-derived data into complete Generator-ready input.
Generator remains responsible only for consuming a complete and successful
GenerateContext result after that downstream boundary is separately approved.

## P5-01 Alignment

P5-02 preserves P5-01 by:

- using only the P4-derived Manifest as the Template Derivation input
- treating Template Derivation output as Template binding data, not Generator
  input
- preserving hard stops for missing, unsupported, ambiguous, non-unique, or
  incomplete binding state
- keeping GenerateContext and Generator downstream and unmodified
- recording unknowns as unsupported or deferred instead of inferring missing
  design intent
- keeping implementation, tests, Template files, package, `dist`, release,
  external service operations, staging, commit, and push out of scope

## Scope Planning Decision

GO:

- P5-02 docs-only Template inventory review and concrete derivation table

NO-GO:

- implementation
- test code implementation
- Template file changes
- GenerateContext changes
- Generator changes
- runtime behavior changes
- package, `dist`, build, release, external service, staging, commit, or push
  operations

## Next Task Candidates

Candidate next tasks:

- P5-03 - Template Derivation Output Model Planning
- P5-04 - Template Derivation Entry Point Scope Planning
- P5-05 - Template Derivation Focused Test Design
- P5-06 - GenerateContext Data Model Planning

P5-02 does not select or authorize any implementation task.

## Verification Performed

P5-02 verification is docs-only:

- reviewed P5-01 Template Derivation scope planning
- reviewed P4-05 Template Mapping contract freeze
- reviewed P4-06 GenerateContext responsibility boundary freeze
- reviewed P4-07 Generator Input Contract scope planning
- reviewed current Template files
- reviewed current `ManifestItem`, Manifest Derivation, Template Provider, and
  Generator surfaces as inventory evidence only
- confirmed no implementation, tests, Template file changes, GenerateContext
  changes, Generator changes, package, `dist`, build, release, staging, commit,
  push, or external operation is part of this task

Required post-edit verification:

- `git diff --check`
- docs-only diff confirmation
