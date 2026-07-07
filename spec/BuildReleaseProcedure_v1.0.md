# Build Release Procedure v1.0

Version : 1.0
Status  : Frozen
Scope   : Build.xlam
Depends : Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md, BuildBlueprint_v1.0.1.md, BuildReleaseChecklist_v1.0.md

---

# 1. Purpose

This document defines the official release verification procedure for Build.xlam.

This procedure makes BuildReleaseChecklist_v1.0.md executable by defining the Purpose, Target, Verification Method, Expected Result, PASS Criteria, FAIL Criteria, and Evidence for each checklist item.

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

## Step 1 : BuildCanon v1.0 と一致している

### Purpose
Confirm that the release follows the frozen Build canon.

### Target
BuildCanon_v1.0.md, BuildBlueprint_v1.0.1.md, BuildDocumentationStandard_v1.0.md, BuildReleaseProcedure_v1.0.md, BuildReleaseChecklist_v1.0.md, release behavior, and generated output.

### Verification Method
Review BuildCanon_v1.0.md rules for Blueprint First, Manifest Driven, Template Driven, Context Driven, Single Generate Engine, Composition, Single Source of Truth, Self Hosting, Version Policy, and Documentation Policy. Then compare BuildBlueprint_v1.0.1.md, this procedure, the checklist, release behavior, and generated output against those rules. Confirm that no release document or behavior redefines, omits, or extends the frozen BuildCanon_v1.0.md rules.

### Expected Result
The release documents, behavior, and generated output follow BuildCanon_v1.0.md without contradiction, omission of required canon behavior, or unapproved extension.

### PASS Criteria
Each applicable BuildCanon_v1.0.md rule is satisfied by the checked documents and behavior, and no checked item treats non-canon behavior as approved Build v1.0.1 behavior.

### FAIL Criteria
Any checked document, behavior, or generated output contradicts BuildCanon_v1.0.md, omits required canon behavior, or introduces an unapproved extension as Build v1.0.1 behavior.

### Evidence
Record the checked BuildCanon_v1.0.md rule list, compared files/artifacts, any detected differences, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 2 : BuildBlueprint v1.0.1 と一致している

### Purpose
Confirm that Build v1.0.1 follows the approved blueprint.

### Target
Build v1.0.1 design and generated output

### Verification Method
Compare the release scope with BuildBlueprint_v1.0.1.md.

### Expected Result
The release implements only the approved Build v1.0.1 blueprint.

### PASS Criteria
The release matches BuildBlueprint_v1.0.1.md.

### FAIL Criteria
The release omits required blueprint behavior or includes unapproved behavior.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 3 : Blueprint は凍結済み

### Purpose
Confirm that the applicable blueprint is frozen.

### Target
BuildBlueprint_v1.0.1.md

### Verification Method
Check the document metadata.

### Expected Result
Status is Frozen.

### PASS Criteria
BuildBlueprint_v1.0.1.md status is Frozen.

### FAIL Criteria
The blueprint is missing, not frozen, or replaced by an unapproved document.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 4 : v1.1 Candidate は分離済み

### Purpose
Confirm candidate behavior is not mixed into Build v1.0.1.

### Target
BuildCandidates_v1.1.md, BuildBlueprint_v1.0.1.md, BuildReleaseProcedure_v1.0.md, BuildReleaseChecklist_v1.0.md, release report, source behavior, templates, manifests, and generated output.

### Verification Method
Identify all items documented in BuildCandidates_v1.1.md. Search the frozen Build v1.0.1 documents, release artifacts, source behavior, templates, manifests, and generated output for those candidate items. Confirm that candidate items are referenced only as excluded or future behavior and are not required, implemented, or judged as Build v1.0.1 release behavior.

### Expected Result
Build v1.1 Candidate behavior remains isolated in BuildCandidates_v1.1.md and is not mixed into frozen Build v1.0.1 requirements, procedures, checklist judgments, implementation, templates, manifests, or generated output.

### PASS Criteria
Every known v1.1 Candidate item is either absent from Build v1.0.1 artifacts or explicitly identified as excluded/future behavior, with no release PASS depending on candidate behavior.

### FAIL Criteria
Any v1.1 Candidate item appears as required, implemented, validated, or release-approved Build v1.0.1 behavior outside BuildCandidates_v1.1.md.

### Evidence
Record the checked candidate items, searched files/artifacts, any candidate references found, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 4. Architecture

## Step 5 : Canon v2.0 に準拠

### Purpose
Confirm project-level architectural compliance.

### Target
Build.xlam architecture and source code

### Verification Method
Compare architecture with Canon_v2.0.md.

### Expected Result
Canon v2.0 is not contradicted.

### PASS Criteria
No Canon v2.0 violation is found.

### FAIL Criteria
Any Canon v2.0 violation is found.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 6 : Layer構成が正しい

### Purpose
Confirm that layer responsibilities are preserved.

### Target
Presentation, Application, Composition Root, Infrastructure

### Verification Method
Inspect module placement and responsibility boundaries.

### Expected Result
Layers follow BuildCanon_v1.0.md and BuildBlueprint_v1.0.1.md.

### PASS Criteria
Each layer has the approved responsibility.

### FAIL Criteria
A layer is missing, misplaced, or contains another layer's responsibility.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 7 : Dependency Ruleを満たす

### Purpose
Confirm one-way dependency direction.

### Target
Source dependencies between layers

### Verification Method
Inspect references and calls between modules.

### Expected Result
Lower layers do not depend on higher layers.

### PASS Criteria
Dependency direction follows the approved architecture.

### FAIL Criteria
A circular dependency or lower-to-higher dependency exists.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 8 : Composition Rootを利用している

### Purpose
Confirm boundary construction is centralized.

### Target
Composition Root and object wiring

### Verification Method
Inspect initialization and dependency composition.

### Expected Result
Composition Root defines architectural boundaries.

### PASS Criteria
Composition Root is used for approved composition.

### FAIL Criteria
Composition is scattered across unrelated layers or bypasses the approved boundary.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 5. Generator

## Step 9 : GenerateModule

### Purpose
Confirm module generation is available.

### Target
GenerateModule operation

### Verification Method
Execute or inspect the module generation path.

### Expected Result
A standard module can be generated through the approved generator flow.

### PASS Criteria
GenerateModule works through the single Generator Engine.

### FAIL Criteria
GenerateModule is missing, fails, or bypasses the approved flow.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 10 : GenerateClass

### Purpose
Confirm class generation is available.

### Target
GenerateClass operation

### Verification Method
Execute or inspect the class generation path.

### Expected Result
A class module can be generated through the approved generator flow.

### PASS Criteria
GenerateClass works through the single Generator Engine.

### FAIL Criteria
GenerateClass is missing, fails, or bypasses the approved flow.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 11 : GenerateEnum

### Purpose
Confirm enum generation is available.

### Target
GenerateEnum operation

### Verification Method
Execute or inspect the enum generation path.

### Expected Result
An enum can be generated through the approved generator flow.

### PASS Criteria
GenerateEnum works through the single Generator Engine.

### FAIL Criteria
GenerateEnum is missing, fails, or bypasses the approved flow.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 12 : GenerateInterface

### Purpose
Confirm interface generation is available.

### Target
GenerateInterface operation

### Verification Method
Execute or inspect the interface generation path.

### Expected Result
An interface can be generated through the approved generator flow.

### PASS Criteria
GenerateInterface works through the single Generator Engine.

### FAIL Criteria
GenerateInterface is missing, fails, or bypasses the approved flow.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 13 : GenerateLayer

### Purpose
Confirm layer generation composes component generation.

### Target
GenerateLayer operation

### Verification Method
Execute or inspect the layer generation path.

### Expected Result
A layer can be generated by composed generation steps.

### PASS Criteria
GenerateLayer works through the approved composition model.

### FAIL Criteria
GenerateLayer is missing, fails, or duplicates generator responsibility.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 14 : GenerateProject

### Purpose
Confirm project generation composes layer generation.

### Target
GenerateProject operation

### Verification Method
Execute or inspect the project generation path.

### Expected Result
A project can be generated by composed generation steps.

### PASS Criteria
GenerateProject works through the approved composition model.

### FAIL Criteria
GenerateProject is missing, fails, or bypasses approved composition.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 6. Pipeline

## Step 15 : Manifest Driven

### Purpose
Confirm generation targets are manifest-defined.

### Target
Manifest and generator input

### Verification Method
Inspect generator target resolution.

### Expected Result
Generation targets come from Manifest.

### PASS Criteria
No generation target is hard-coded in Generator.

### FAIL Criteria
Generator directly defines generation targets.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 16 : Template Driven

### Purpose
Confirm generated code forms are template-defined.

### Target
Templates and generator output

### Verification Method
Inspect template usage and generated output.

### Expected Result
Generated code structure comes from Template.

### PASS Criteria
Generator does not know template content details.

### FAIL Criteria
Generator embeds generated code forms that belong in Template.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 17 : Context Driven

### Purpose
Confirm variable data is centralized.

### Target
GenerateContext and generator calls

### Verification Method
Inspect variable generation data flow.

### Expected Result
Variable data is held by GenerateContext.

### PASS Criteria
Generation uses GenerateContext for variable data.

### FAIL Criteria
Variable data is scattered through additional generator arguments or globals.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 18 : Token Replace

### Purpose
Confirm template tokens are replaced by approved context values.

### Target
Token replacement process

### Verification Method
Inspect or execute token replacement.

### Expected Result
Tokens are replaced deterministically from context.

### PASS Criteria
Token replacement succeeds with expected values.

### FAIL Criteria
Tokens remain unresolved or are replaced from unapproved sources.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 19 : Generator Engine

### Purpose
Confirm that generation uses a single engine.

### Target
Generator implementation

### Verification Method
Inspect generation entry points.

### Expected Result
All generation operations use the single Generator Engine.

### PASS Criteria
No duplicate generator engine exists.

### FAIL Criteria
Multiple generator engines or bypass paths exist.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 7. Generated Layers

## Step 20 : Common

### Purpose
Confirm Common layer generation.

### Target
Generated Common layer

### Verification Method
Run or inspect project generation output.

### Expected Result
Common layer is generated when defined by Manifest.

### PASS Criteria
Common layer exists and matches approved templates.

### FAIL Criteria
Common layer is missing or structurally invalid.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 21 : Manifest

### Purpose
Confirm Manifest layer generation.

### Target
Generated Manifest layer

### Verification Method
Run or inspect project generation output.

### Expected Result
Manifest layer is generated when defined by Manifest.

### PASS Criteria
Manifest layer exists and matches approved templates.

### FAIL Criteria
Manifest layer is missing or structurally invalid.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 22 : Infrastructure

### Purpose
Confirm Infrastructure layer generation.

### Target
Generated Infrastructure layer

### Verification Method
Run or inspect project generation output.

### Expected Result
Infrastructure layer is generated when defined by Manifest.

### PASS Criteria
Infrastructure layer exists and matches approved templates.

### FAIL Criteria
Infrastructure layer is missing or structurally invalid.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 23 : Domain

### Purpose
Confirm Domain layer generation.

### Target
Generated Domain layer

### Verification Method
Run or inspect project generation output.

### Expected Result
Domain layer is generated when defined by Manifest.

### PASS Criteria
Domain layer exists and matches approved templates.

### FAIL Criteria
Domain layer is missing or structurally invalid.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 24 : Application

### Purpose
Confirm Application layer generation.

### Target
Generated Application layer

### Verification Method
Run or inspect project generation output.

### Expected Result
Application layer is generated when defined by Manifest.

### PASS Criteria
Application layer exists and matches approved templates.

### FAIL Criteria
Application layer is missing or structurally invalid.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 25 : Presentation

### Purpose
Confirm Presentation layer generation.

### Target
Generated Presentation layer

### Verification Method
Run or inspect project generation output.

### Expected Result
Presentation layer is generated when defined by Manifest.

### PASS Criteria
Presentation layer exists and matches approved templates.

### FAIL Criteria
Presentation layer is missing or structurally invalid.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 8. Quality

## Step 26 : Compile Error = 0

### Purpose
Confirm the release compiles.

### Target
Build.xlam and generated VMF.xlam

### Verification Method
Compile the VBA project after generation.

### Expected Result
No compile errors occur.

### PASS Criteria
Compile completes with zero errors.

### FAIL Criteria
Any compile error remains.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 27 : Option Explicit

### Purpose
Confirm explicit variable declaration.

### Target
VBA modules and templates

### Verification Method
Inspect source and generated code.

### Expected Result
Every applicable module contains Option Explicit.

### PASS Criteria
Option Explicit is present where required.

### FAIL Criteria
Any applicable module lacks Option Explicit.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 28 : Header Comment

### Purpose
Confirm required headers are present.

### Target
VBA modules and templates

### Verification Method
Inspect source and generated code headers.

### Expected Result
Required header comments are present and consistent.

### PASS Criteria
Headers satisfy the approved documentation/template rules.

### FAIL Criteria
Required headers are missing or inconsistent.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 29 : Naming Rule

### Purpose
Confirm naming consistency.

### Target
Source, templates, generated modules

### Verification Method
Inspect names against the approved naming convention.

### Expected Result
Names follow the approved rules.

### PASS Criteria
No naming rule violation is found.

### FAIL Criteria
Any naming rule violation is found.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 30 : Template Review 完了

### Purpose
Confirm templates were reviewed.

### Target
Template files

### Verification Method
Review template content for approved structure and placeholders.

### Expected Result
Templates are complete and consistent with the blueprint.

### PASS Criteria
Template review is completed with no blocking issue.

### FAIL Criteria
Template review is incomplete or has unresolved blocking issues.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 31 : Manifest Review 完了

### Purpose
Confirm manifests were reviewed.

### Target
Manifest definitions

### Verification Method
Review manifest content for approved targets and layer definitions.

### Expected Result
Manifests are complete and consistent with the blueprint.

### PASS Criteria
Manifest review is completed with no blocking issue.

### FAIL Criteria
Manifest review is incomplete or has unresolved blocking issues.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 9. Validation

## Step 32 : GenerateProject 成功

### Purpose
Confirm end-to-end project generation.

### Target
GenerateProject operation

### Verification Method
Execute GenerateProject with the release manifest.

### Expected Result
GenerateProject completes successfully.

### PASS Criteria
GenerateProject completes without blocking error.

### FAIL Criteria
GenerateProject fails or requires unapproved manual intervention.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 33 : VMF.xlam 生成成功

### Purpose
Confirm VMF.xlam can be produced.

### Target
Generated VMF.xlam artifact

### Verification Method
Run the approved generation flow and verify the output artifact.

### Expected Result
VMF.xlam is generated.

### PASS Criteria
VMF.xlam exists and is usable as the generated artifact.

### FAIL Criteria
VMF.xlam is not generated or is unusable.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 34 : 生成後コンパイル成功

### Purpose
Confirm generated output compiles.

### Target
Generated VMF.xlam

### Verification Method
Compile the generated project.

### Expected Result
Generated project compiles successfully.

### PASS Criteria
Generated project compiles with zero errors.

### FAIL Criteria
Any generated compile error remains.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 35 : 手修正不要

### Purpose
Confirm generation is complete without manual fixes.

### Target
Generated output

### Verification Method
Compare generated output with required release behavior.

### Expected Result
Generated output requires no manual correction.

### PASS Criteria
Release output is produced without manual source edits.

### FAIL Criteria
Manual edits are required after generation.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 36 : BuildCanonと一致

### Purpose
Confirm validation output follows the Build canon.

### Target
Generated output and release behavior

### Verification Method
Compare validation results with BuildCanon_v1.0.md.

### Expected Result
Validation output is consistent with BuildCanon_v1.0.md.

### PASS Criteria
No contradiction with BuildCanon_v1.0.md is found.

### FAIL Criteria
Generated output contradicts BuildCanon_v1.0.md.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 10. Self Hosting

## Step 37 : Build が Build を生成できる設計

### Purpose
Confirm the design supports future self-hosting.

### Target
Build.xlam design and blueprint

### Verification Method
Review architecture against BuildCanon_v1.0.md Self Hosting rule.

### Expected Result
Design does not prevent Build from generating Build.

### PASS Criteria
The approved design supports the self-hosting direction.

### FAIL Criteria
The design introduces a contradiction to the self-hosting direction.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 38 : Self Hosting構想を満たす

### Purpose
Confirm consistency with the self-hosting concept.

### Target
Build.xlam official documents and architecture

### Verification Method
Review documents and architecture for self-hosting consistency.

### Expected Result
Self-hosting remains a valid design goal.

### PASS Criteria
No release change blocks the self-hosting concept.

### FAIL Criteria
Release changes contradict or block the self-hosting concept.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 11. Documentation

## Step 39 : README 更新

### Purpose
Confirm the documentation index is current.

### Target
spec/README.md

### Verification Method
Review README document list and release flow references.

### Expected Result
README lists the official documents and release reference order.

### PASS Criteria
README is current and does not redefine specifications.

### FAIL Criteria
README is missing required official documents or contradicts higher documents.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 40 : BuildCanon 更新

### Purpose
Confirm BuildCanon references remain current.

### Target
BuildCanon_v1.0.md

### Verification Method
Review metadata and documentation policy references.

### Expected Result
BuildCanon remains frozen and consistent with the official set.

### PASS Criteria
No required BuildCanon consistency update is missing.

### FAIL Criteria
BuildCanon requires a consistency update that has not been made.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 41 : BuildDocumentationStandard 更新

### Purpose
Confirm documentation rules include release artifacts.

### Target
BuildDocumentationStandard_v1.0.md

### Verification Method
Review official set, hierarchy, and release management rules.

### Expected Result
Documentation standard defines Procedure, Checklist, and Report relationships.

### PASS Criteria
Required documentation standard updates are present.

### FAIL Criteria
Required documentation standard updates are missing or contradictory.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 42 : BuildBlueprint 更新

### Purpose
Confirm blueprint references remain current.

### Target
BuildBlueprint_v1.0.1.md

### Verification Method
Review metadata, scope, and release criteria.

### Expected Result
Blueprint remains frozen and consistent with release documents.

### PASS Criteria
No required blueprint consistency update is missing.

### FAIL Criteria
Blueprint requires a consistency update that has not been made.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 43 : Candidate 更新

### Purpose
Confirm candidate separation is current.

### Target
BuildCandidates_v1.1.md

### Verification Method
Review candidate document for pending v1.1 items.

### Expected Result
Candidate items remain separated from frozen v1.0.1 behavior.

### PASS Criteria
Candidate document is current for known candidates.

### FAIL Criteria
Candidate items are missing or mixed into frozen documents.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 44 : CHANGELOG 更新

### Purpose
Confirm documentation changes are recorded.

### Target
CHANGELOG.md

### Verification Method
Review the changelog entry for the release documentation changes.

### Expected Result
Changelog records the addition or update.

### PASS Criteria
Changelog includes the documentation change.

### FAIL Criteria
Changelog omits required documentation changes.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 45 : Release Report created and saved

### Purpose
Confirm release evidence is preserved.

### Target
spec/releases/Build_v1.0.1_ReleaseReport.md

### Verification Method
Verify the release report exists and references the checklist.

### Expected Result
Release report records the completed release decision and evidence.

### PASS Criteria
Release report exists, is saved, and is consistent with the checklist.

### FAIL Criteria
Release report is missing, unsaved, or redefines higher-priority documents.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 12. Git

## Step 46 : Commit 完了

### Purpose
Confirm release changes are committed.

### Target
Git repository

### Verification Method
Check git status and release commit history.

### Expected Result
Release changes are committed in a focused commit.

### PASS Criteria
Required release changes are committed.

### FAIL Criteria
Required release changes are uncommitted or mixed with unrelated changes.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 47 : Tag 作成

### Purpose
Confirm the release version is tagged.

### Target
Git repository tags

### Verification Method
Check the release tag.

### Expected Result
A release tag exists for the approved version.

### PASS Criteria
The correct release tag exists.

### FAIL Criteria
The release tag is missing or points to the wrong commit.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

## Step 48 : Release 作成

### Purpose
Confirm the official release is created.

### Target
Release publication target

### Verification Method
Check the release record.

### Expected Result
The official release record exists for the approved version.

### PASS Criteria
Release is created with the approved version and artifacts.

### FAIL Criteria
Release is missing, incomplete, or inconsistent with the approved version.

### Evidence
Record the checked target files or artifacts, verification notes, reviewer, date, and PASS/FAIL result in the applicable Release Report.

---

# 13. Release Decision

The release decision SHALL be recorded in BuildReleaseChecklist_v1.0.md and the applicable Release Report.

Release approval SHALL be granted only when all required checklist items have passed or an approved exception is recorded in the Release Report.
