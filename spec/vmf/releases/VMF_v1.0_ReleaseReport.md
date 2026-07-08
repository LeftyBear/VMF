# VMF v1.0 Release Report

Version : 1.0
Status  : Released
Scope   : VMF Project
Date    : 2026-07-08

---

# 1. Release Decision

VMF v1.0 project construction is released for the repository state generated from `spec/vmf/manifest.yaml`.

VMF v1.0 normative design was not changed.

Build.xlam v1.0.1 was used as the frozen official Build tool baseline. Build.xlam specification changes were not made.

---

# 2. Inputs

| Item | Path | Result |
| --- | --- | --- |
| VMF v1.0 Specification | `spec/vmf/VMF_v1.0.md` | Frozen |
| VMF Manifest | `spec/vmf/manifest.yaml` | Used |
| Build Blueprint | `spec/build/BuildBlueprint_v1.0.1.md` | Frozen |
| Build Candidates | `spec/build/BuildCandidates_v1.1.md` | Candidate separation maintained |
| VMF Candidates | `spec/vmf/VMFCandidates_v1.1.md` | Candidate separation maintained |

---

# 3. Generated Output

| Layer | Path | Result |
| --- | --- | --- |
| Common | `src/VMF/Common` | PASS |
| Core | `src/VMF/Core` | PASS |
| Domain | `src/VMF/Domain` | PASS |
| Application | `src/VMF/Application` | PASS |
| Infrastructure | `src/VMF/Infrastructure` | PASS |
| Presentation | `src/VMF/Presentation` | PASS |

---

# 4. Audit Evidence

| Gate | Evidence | Result |
| --- | --- | --- |
| Directory Reconfiguration | Build source, manifests, and templates organized under `src/Build`; VMF output under `src/VMF` | PASS |
| Manifest Verification | `spec/vmf/manifest.yaml` layer order and modules inspected by `tools/vmf/generate-vmf.ps1` | PASS |
| Generate Project | `powershell -ExecutionPolicy Bypass -File tools\vmf\generate-vmf.ps1` | PASS |
| Release Gate Audit | `powershell -ExecutionPolicy Bypass -File tools\vmf\audit-vmf.ps1` | PASS |
| Build Script Verification | `powershell -ExecutionPolicy Bypass -File tools\build\build.ps1` producing `dist/build/Build.xlam` | PASS |
| VBA Test Verification | `powershell -ExecutionPolicy Bypass -File tools\test\run-tests.ps1` | PASS |

---

# 5. Candidate Separation

The following improvements were separated from VMF v1.0 and Build v1.0.1:

- `spec/vmf/VMFCandidates_v1.1.md`: native Core layer generation and YAML manifest reader.
- `spec/build/BuildCandidates_v1.1.md`: custom layer manifest generation.

---

# 6. Release Declaration

VMF v1.0 Release is declared for the generated project artifacts under `src/VMF` based on `spec/vmf/manifest.yaml`.

The release is approved with candidate items separated and frozen VMF v1.0 / Build v1.0.1 specifications preserved.
