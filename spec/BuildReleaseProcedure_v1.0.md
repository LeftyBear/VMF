# Build Release Procedure v1.0

Version : 1.0
Status  : Frozen
Scope   : Build.xlam
Depends : Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md, BuildBlueprint_v1.0.1.md, BuildReleaseChecklist_v1.0.md

---

# 1. Purpose

This document defines the official release verification procedure for Build.xlam.

This procedure makes BuildReleaseChecklist_v1.0.md executable by defining the purpose, target, confirmation method, expected result, and PASS/FAIL criteria for each checklist item.

Each procedure item SHALL correspond one-to-one with BuildReleaseChecklist_v1.0.md.

BuildReleaseChecklist_v1.0.md SHALL record only the final judgment for each item.

Release Reports under `spec/releases` SHALL record release-specific evidence and the completed release decision.

---

# 2. Procedure Rules

All release checks SHALL be performed according to this procedure before judging BuildReleaseChecklist_v1.0.md.

The checklist item SHALL be marked as passed only when the PASS criterion in this procedure is satisfied.

If any item meets the FAIL criterion, the release SHALL NOT be approved until the issue is resolved or explicitly deferred by an approved higher-priority document.

This procedure SHALL NOT redefine Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md, or BuildBlueprint_v1.0.1.md.

---

# 3. Blueprint

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| BuildCanon v1.0 と一致している | Confirm that the release follows the frozen Build canon. | Build.xlam design, implementation, and official documents | Compare release behavior and documents with BuildCanon_v1.0.md. | No contradiction with BuildCanon_v1.0.md exists. | All checked behavior and documents are consistent with BuildCanon_v1.0.md. | Any contradiction or unapproved extension of BuildCanon_v1.0.md exists. |
| BuildBlueprint v1.0.1 と一致している | Confirm that Build v1.0.1 follows the approved blueprint. | Build v1.0.1 design and generated output | Compare the release scope with BuildBlueprint_v1.0.1.md. | The release implements only the approved Build v1.0.1 blueprint. | The release matches BuildBlueprint_v1.0.1.md. | The release omits required blueprint behavior or includes unapproved behavior. |
| Blueprint は凍結済み | Confirm that the applicable blueprint is frozen. | BuildBlueprint_v1.0.1.md | Check the document metadata. | Status is Frozen. | BuildBlueprint_v1.0.1.md status is Frozen. | The blueprint is missing, not frozen, or replaced by an unapproved document. |
| v1.1 Candidate は分離済み | Confirm candidate behavior is not mixed into Build v1.0.1. | BuildCandidates_v1.1.md and Build v1.0.1 documents | Review official documents for candidate behavior. | Candidate items are documented only in BuildCandidates_v1.1.md. | v1.1 Candidate items are isolated from frozen v1.0.1 documents. | Candidate behavior appears as Build v1.0.1 official behavior. |

---

# 4. Architecture

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| Canon v2.0 に準拠 | Confirm project-level architectural compliance. | Build.xlam architecture and source code | Compare architecture with Canon_v2.0.md. | Canon v2.0 is not contradicted. | No Canon v2.0 violation is found. | Any Canon v2.0 violation is found. |
| Layer構成が正しい | Confirm that layer responsibilities are preserved. | Presentation, Application, Composition Root, Infrastructure | Inspect module placement and responsibility boundaries. | Layers follow BuildCanon_v1.0.md and BuildBlueprint_v1.0.1.md. | Each layer has the approved responsibility. | A layer is missing, misplaced, or contains another layer's responsibility. |
| Dependency Ruleを満たす | Confirm one-way dependency direction. | Source dependencies between layers | Inspect references and calls between modules. | Lower layers do not depend on higher layers. | Dependency direction follows the approved architecture. | A circular dependency or lower-to-higher dependency exists. |
| Composition Rootを利用している | Confirm boundary construction is centralized. | Composition Root and object wiring | Inspect initialization and dependency composition. | Composition Root defines architectural boundaries. | Composition Root is used for approved composition. | Composition is scattered across unrelated layers or bypasses the approved boundary. |

---

# 5. Generator

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| GenerateModule | Confirm module generation is available. | GenerateModule operation | Execute or inspect the module generation path. | A standard module can be generated through the approved generator flow. | GenerateModule works through the single Generator Engine. | GenerateModule is missing, fails, or bypasses the approved flow. |
| GenerateClass | Confirm class generation is available. | GenerateClass operation | Execute or inspect the class generation path. | A class module can be generated through the approved generator flow. | GenerateClass works through the single Generator Engine. | GenerateClass is missing, fails, or bypasses the approved flow. |
| GenerateEnum | Confirm enum generation is available. | GenerateEnum operation | Execute or inspect the enum generation path. | An enum can be generated through the approved generator flow. | GenerateEnum works through the single Generator Engine. | GenerateEnum is missing, fails, or bypasses the approved flow. |
| GenerateInterface | Confirm interface generation is available. | GenerateInterface operation | Execute or inspect the interface generation path. | An interface can be generated through the approved generator flow. | GenerateInterface works through the single Generator Engine. | GenerateInterface is missing, fails, or bypasses the approved flow. |
| GenerateLayer | Confirm layer generation composes component generation. | GenerateLayer operation | Execute or inspect the layer generation path. | A layer can be generated by composed generation steps. | GenerateLayer works through the approved composition model. | GenerateLayer is missing, fails, or duplicates generator responsibility. |
| GenerateProject | Confirm project generation composes layer generation. | GenerateProject operation | Execute or inspect the project generation path. | A project can be generated by composed generation steps. | GenerateProject works through the approved composition model. | GenerateProject is missing, fails, or bypasses approved composition. |

---

# 6. Pipeline

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| Manifest Driven | Confirm generation targets are manifest-defined. | Manifest and generator input | Inspect generator target resolution. | Generation targets come from Manifest. | No generation target is hard-coded in Generator. | Generator directly defines generation targets. |
| Template Driven | Confirm generated code forms are template-defined. | Templates and generator output | Inspect template usage and generated output. | Generated code structure comes from Template. | Generator does not know template content details. | Generator embeds generated code forms that belong in Template. |
| Context Driven | Confirm variable data is centralized. | GenerateContext and generator calls | Inspect variable generation data flow. | Variable data is held by GenerateContext. | Generation uses GenerateContext for variable data. | Variable data is scattered through additional generator arguments or globals. |
| Token Replace | Confirm template tokens are replaced by approved context values. | Token replacement process | Inspect or execute token replacement. | Tokens are replaced deterministically from context. | Token replacement succeeds with expected values. | Tokens remain unresolved or are replaced from unapproved sources. |
| Generator Engine | Confirm that generation uses a single engine. | Generator implementation | Inspect generation entry points. | All generation operations use the single Generator Engine. | No duplicate generator engine exists. | Multiple generator engines or bypass paths exist. |

---

# 7. Generated Layers

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| Common | Confirm Common layer generation. | Generated Common layer | Run or inspect project generation output. | Common layer is generated when defined by Manifest. | Common layer exists and matches approved templates. | Common layer is missing or structurally invalid. |
| Manifest | Confirm Manifest layer generation. | Generated Manifest layer | Run or inspect project generation output. | Manifest layer is generated when defined by Manifest. | Manifest layer exists and matches approved templates. | Manifest layer is missing or structurally invalid. |
| Infrastructure | Confirm Infrastructure layer generation. | Generated Infrastructure layer | Run or inspect project generation output. | Infrastructure layer is generated when defined by Manifest. | Infrastructure layer exists and matches approved templates. | Infrastructure layer is missing or structurally invalid. |
| Domain | Confirm Domain layer generation. | Generated Domain layer | Run or inspect project generation output. | Domain layer is generated when defined by Manifest. | Domain layer exists and matches approved templates. | Domain layer is missing or structurally invalid. |
| Application | Confirm Application layer generation. | Generated Application layer | Run or inspect project generation output. | Application layer is generated when defined by Manifest. | Application layer exists and matches approved templates. | Application layer is missing or structurally invalid. |
| Presentation | Confirm Presentation layer generation. | Generated Presentation layer | Run or inspect project generation output. | Presentation layer is generated when defined by Manifest. | Presentation layer exists and matches approved templates. | Presentation layer is missing or structurally invalid. |

---

# 8. Quality

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| Compile Error = 0 | Confirm the release compiles. | Build.xlam and generated VMF.xlam | Compile the VBA project after generation. | No compile errors occur. | Compile completes with zero errors. | Any compile error remains. |
| Option Explicit | Confirm explicit variable declaration. | VBA modules and templates | Inspect source and generated code. | Every applicable module contains Option Explicit. | Option Explicit is present where required. | Any applicable module lacks Option Explicit. |
| Header Comment | Confirm required headers are present. | VBA modules and templates | Inspect source and generated code headers. | Required header comments are present and consistent. | Headers satisfy the approved documentation/template rules. | Required headers are missing or inconsistent. |
| Naming Rule | Confirm naming consistency. | Source, templates, generated modules | Inspect names against the approved naming convention. | Names follow the approved rules. | No naming rule violation is found. | Any naming rule violation is found. |
| Template Review 完了 | Confirm templates were reviewed. | Template files | Review template content for approved structure and placeholders. | Templates are complete and consistent with the blueprint. | Template review is completed with no blocking issue. | Template review is incomplete or has unresolved blocking issues. |
| Manifest Review 完了 | Confirm manifests were reviewed. | Manifest definitions | Review manifest content for approved targets and layer definitions. | Manifests are complete and consistent with the blueprint. | Manifest review is completed with no blocking issue. | Manifest review is incomplete or has unresolved blocking issues. |

---

# 9. Validation

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| GenerateProject 成功 | Confirm end-to-end project generation. | GenerateProject operation | Execute GenerateProject with the release manifest. | GenerateProject completes successfully. | GenerateProject completes without blocking error. | GenerateProject fails or requires unapproved manual intervention. |
| VMF.xlam 生成成功 | Confirm VMF.xlam can be produced. | Generated VMF.xlam artifact | Run the approved generation flow and verify the output artifact. | VMF.xlam is generated. | VMF.xlam exists and is usable as the generated artifact. | VMF.xlam is not generated or is unusable. |
| 生成後コンパイル成功 | Confirm generated output compiles. | Generated VMF.xlam | Compile the generated project. | Generated project compiles successfully. | Generated project compiles with zero errors. | Any generated compile error remains. |
| 手修正不要 | Confirm generation is complete without manual fixes. | Generated output | Compare generated output with required release behavior. | Generated output requires no manual correction. | Release output is produced without manual source edits. | Manual edits are required after generation. |
| BuildCanonと一致 | Confirm validation output follows the Build canon. | Generated output and release behavior | Compare validation results with BuildCanon_v1.0.md. | Validation output is consistent with BuildCanon_v1.0.md. | No contradiction with BuildCanon_v1.0.md is found. | Generated output contradicts BuildCanon_v1.0.md. |

---

# 10. Self Hosting

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| Build が Build を生成できる設計 | Confirm the design supports future self-hosting. | Build.xlam design and blueprint | Review architecture against BuildCanon_v1.0.md Self Hosting rule. | Design does not prevent Build from generating Build. | The approved design supports the self-hosting direction. | The design introduces a contradiction to the self-hosting direction. |
| Self Hosting構想を満たす | Confirm consistency with the self-hosting concept. | Build.xlam official documents and architecture | Review documents and architecture for self-hosting consistency. | Self-hosting remains a valid design goal. | No release change blocks the self-hosting concept. | Release changes contradict or block the self-hosting concept. |

---

# 11. Documentation

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| README 更新 | Confirm the documentation index is current. | spec/README.md | Review README document list and release flow references. | README lists the official documents and release reference order. | README is current and does not redefine specifications. | README is missing required official documents or contradicts higher documents. |
| BuildCanon 更新 | Confirm BuildCanon references remain current. | BuildCanon_v1.0.md | Review metadata and documentation policy references. | BuildCanon remains frozen and consistent with the official set. | No required BuildCanon consistency update is missing. | BuildCanon requires a consistency update that has not been made. |
| BuildDocumentationStandard 更新 | Confirm documentation rules include release artifacts. | BuildDocumentationStandard_v1.0.md | Review official set, hierarchy, and release management rules. | Documentation standard defines Procedure, Checklist, and Report relationships. | Required documentation standard updates are present. | Required documentation standard updates are missing or contradictory. |
| BuildBlueprint 更新 | Confirm blueprint references remain current. | BuildBlueprint_v1.0.1.md | Review metadata, scope, and release criteria. | Blueprint remains frozen and consistent with release documents. | No required blueprint consistency update is missing. | Blueprint requires a consistency update that has not been made. |
| Candidate 更新 | Confirm candidate separation is current. | BuildCandidates_v1.1.md | Review candidate document for pending v1.1 items. | Candidate items remain separated from frozen v1.0.1 behavior. | Candidate document is current for known candidates. | Candidate items are missing or mixed into frozen documents. |
| CHANGELOG 更新 | Confirm documentation changes are recorded. | CHANGELOG.md | Review the changelog entry for the release documentation changes. | Changelog records the addition or update. | Changelog includes the documentation change. | Changelog omits required documentation changes. |
| Release Report created and saved | Confirm release evidence is preserved. | spec/releases/Build_v1.0.1_ReleaseReport.md | Verify the release report exists and references the checklist. | Release report records the completed release decision and evidence. | Release report exists, is saved, and is consistent with the checklist. | Release report is missing, unsaved, or redefines higher-priority documents. |

---

# 12. Git

| Checklist Item | Purpose | Target | Confirmation Method | Expected Result | PASS | FAIL |
| --- | --- | --- | --- | --- | --- | --- |
| Commit 完了 | Confirm release changes are committed. | Git repository | Check git status and release commit history. | Release changes are committed in a focused commit. | Required release changes are committed. | Required release changes are uncommitted or mixed with unrelated changes. |
| Tag 作成 | Confirm the release version is tagged. | Git repository tags | Check the release tag. | A release tag exists for the approved version. | The correct release tag exists. | The release tag is missing or points to the wrong commit. |
| Release 作成 | Confirm the official release is created. | Release publication target | Check the release record. | The official release record exists for the approved version. | Release is created with the approved version and artifacts. | Release is missing, incomplete, or inconsistent with the approved version. |

---

# 13. Release Decision

The release decision SHALL be recorded in BuildReleaseChecklist_v1.0.md and the applicable Release Report.

Release approval SHALL be granted only when all required checklist items have passed or an approved exception is recorded in the Release Report.
