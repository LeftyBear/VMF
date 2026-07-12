# Build v1.1 Post-Release Verification

Version : 0.1
Status  : Completed Verification
Scope   : Build.xlam v1.1 post-release state
Date    : 2026-07-12
Depends : docs/releases/Build_v1.1_ReleaseReport.md, docs/releases/README.md, docs/build/VERSIONING.md

---

# 1. Purpose

This document records post-release verification for Build.xlam v1.1.

This is not a normative specification. It does not modify VMF v1.0, Build
v1.0.x, Build v1.1, or any frozen specification.

---

# 2. Verification Result

Status : PASS

Build.xlam v1.1 release references, artifact metadata, and automated VBA tests
are consistent.

---

# 3. Release Artifact Verification

Artifact:

```text
dist/release/Build/v1.1/Build.xlam
```

Filesystem evidence:

- Size: 178804 bytes
- Timestamp: 2026-07-12 11:37:27 JST

Metadata evidence:

- Build Version: 1.1.0
- Release Type: Release

The metadata was verified from `docProps/custom.xml` inside the release
artifact.

---

# 4. Release Reference Verification

The following documents reference the official Build v1.1 release:

- `README.md`
- `docs/releases/README.md`
- `docs/build/VERSIONING.md`
- `docs/releases/Build_v1.1_ReleaseReport.md`
- `CHANGELOG.md`

The current official Build artifact is consistently recorded as:

```text
dist/release/Build/v1.1/Build.xlam
```

---

# 5. Automated VBA Verification

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

# 6. VMF Audit Note

`tools/vmf/audit-vmf.ps1` expects `specs/vmf/manifest.yaml` by default.

That manifest is not present in the current repository state, so the VMF v1.0
generated-output audit was not executed as part of this Build v1.1 post-release
verification.

This does not block Build v1.1 verification because the Build v1.1 release
artifact and VBA test suite were verified independently.

---

# 7. Decision

Build.xlam v1.1 post-release verification is complete.

No post-release Build v1.1 issue was found.

---

# 8. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 0.1 | Completed Verification | Records Build v1.1 post-release verification evidence |
