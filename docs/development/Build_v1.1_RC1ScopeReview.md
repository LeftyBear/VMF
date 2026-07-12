# Build v1.1 RC1 Scope Review

Version : 0.1
Status  : Working Review
Scope   : Build.xlam v1.1 RC1 preparation
Date    : 2026-07-12
Depends : candidates/BuildCandidates_v1.1.md, docs/development/Build_v1.1_ReleasePlan.md

---

# 1. Purpose

This document records the RC1 scope review for Build v1.1.

This is not a normative specification. It does not modify VMF v1.0, Build
v1.0.x, or any frozen specification.

---

# 2. Review Result

Status : RC1 scope confirmed

The reviewed implementation matches the recommended Build v1.1 RC1 scope.

No Build v2.0 planning item is required for RC1.

---

# 3. Accepted RC1 Scope

The following completed candidates are accepted for RC1 preparation:

| Candidate | RC1 Decision | Evidence |
|-----------|--------------|----------|
| B001 Blueprint Parser | Include | `Build_BlueprintParser.BuildInitializeFromContent` parses blueprint content into generation metadata. |
| B002 Manifest Auto Generation | Include | `Build_BlueprintParser.BuildGenerateManifestContent` emits manifest-format content. |
| B004 Generate Preview | Include | `AppPreviewBuildLayer` returns `ComResult` and generated preview text without creating VBProject components. |
| B007 Template Validation | Include | `InfValidateTemplateFile` validates template existence and required tokens. |
| B008 Manifest Validation | Include | `InfValidateManifestFile` validates manifest files before manifest-driven generation. |
| B011 Custom Layer Manifest Generation | Include | Manifest item and manifest provider validation accept valid custom layer names, including `Core`. |

---

# 4. Deferred Scope

The following items remain outside RC1:

| Candidate | RC1 Decision | Reason |
|-----------|--------------|--------|
| B003 Parallel Generation | Exclude | Deferred and not required for Build v1.1 adoption. |
| B005 Source Generator Architecture | Exclude | Reserved for Build v2.0 because it changes the generation target and packaging boundary. |
| B006 Incremental Generate | Exclude | Future candidate. |
| B009 Ribbon UI | Exclude | Future candidate. |
| B010 Visual Designer | Exclude | Future candidate. |

---

# 5. Public API Review

Result : Acceptable for RC1

The RC1 implementation adds focused APIs that correspond to accepted candidate
behavior:

- `Build_BlueprintParser.BuildInitializeFromContent`
- `Build_BlueprintParser.BuildHasLayer`
- `Build_BlueprintParser.BuildGetLayerItems`
- `Build_BlueprintParser.BuildGetBlueprintValue`
- `Build_BlueprintParser.BuildGenerateManifestContent`
- `AppPreviewBuildLayer`
- `InfValidateTemplateFile`
- `InfValidateManifestFile`

These APIs do not replace the existing Build v1.0.x generation entry points.

---

# 6. Architecture Boundary Review

Result : Pass

The reviewed implementation preserves the Build v1.0.x architecture:

- generation still targets VBProject components;
- manifest-driven generation remains under the existing Build application and
  Infrastructure providers;
- preview generation stops before VBProject mutation;
- source-file generation and packaging separation remain outside RC1.

---

# 7. Verification Evidence

Command:

```powershell
powershell -ExecutionPolicy Bypass -File tools\test\run-tests.ps1
```

Result:

- 16 test runners passed.
- 0 test runners failed.

Passed runners:

- ComRunCommonPhase1Tests
- InfRunInfrastructurePhase2Tests
- DomRunVmfCorePhase3Tests
- AppRunGeneratorPhase1Tests
- AppRunGeneratorPhase1NegativeTests
- AppRunGeneratorPhase2Tests
- AppRunProjectManifestParseTests
- AppRunGenerateCommonPhase3_1Tests
- AppRunGenerateManifestPhase3_2Tests
- AppRunGenerateInfrastructurePhase3_3Tests
- AppRunGenerateDomainPhase3_4Tests
- AppRunGenerateApplicationPhase3_5Tests
- AppRunGeneratePresentationPhase3_6Tests
- AppRunBuildPhase4Tests
- PreRunUiPhase5Tests
- PreRunGenerateModulePhase6Tests

---

# 8. RC1 Decision

Build v1.1 is ready to proceed from Candidate review into RC1 preparation.

Next required work:

1. Prepare the Build v1.1 RC1 release audit checklist.
2. Prepare the Build v1.1 RC1 release report draft.
3. Keep all RC1 material separate from frozen Build v1.0.x specifications until
   official adoption is approved.

---

# 9. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 0.1 | Working Review | Confirms Build v1.1 RC1 scope and verification evidence |
