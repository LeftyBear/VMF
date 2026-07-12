# Vision

**Repository:** VMF
**Project:** Build.xlam / VMF
**Version:** 2.0
**Status:** Frozen

---

# 1. Purpose

The VMF repository exists to maintain the Build.xlam development platform, the VMF architecture, and applications that are built on top of VMF.

Build.xlam is the development platform for creating maintainable, extensible, and reusable VBA applications. VMF provides the frozen implementation contract and generated project foundation that apply the Build.xlam architectural principles to concrete VBA application structures.

The purpose of this repository is not only to build a single Excel add-in, but to establish a standardized application architecture, development workflow, release structure, and engineering practice that enable long-term maintenance and continuous evolution.

---

# 2. Vision

Build.xlam and VMF aim to provide a complete foundation for VBA application development by providing:

* A clear layered architecture.
* Strong separation of responsibilities.
* Stable public interfaces.
* Predictable dependency management.
* Consistent coding standards.
* Automated development support.
* Reproducible release artifacts.
* High maintainability.
* Long-term extensibility.

Every component should be understandable, replaceable, and testable with minimal impact on the rest of the system.

---

# 3. Design Philosophy

The project is based on the following principles:

* Simplicity over complexity.
* Explicit architecture over implicit behavior.
* Contract-based collaboration.
* One-way dependencies.
* Single responsibility.
* Configuration over hard coding.
* Stability before optimization.
* Readability before cleverness.
* Long-term maintainability over short-term convenience.

---

# 4. Objectives

The primary objectives are:

1. Build a reusable VBA application framework.
2. Maintain VMF as a stable implementation contract.
3. Standardize application architecture.
4. Standardize module design.
5. Standardize class design.
6. Standardize interface design.
7. Standardize dependency management.
8. Standardize naming conventions.
9. Standardize error handling.
10. Support automated building, generation, testing, and deployment.
11. Enable future expansion without redesign.

---

# 5. Scope

The repository contains:

* Build.xlam source and release artifacts.
* VMF specifications and generated VMF project structure.
* Common libraries.
* Infrastructure services.
* Domain support.
* Application services.
* Presentation support.
* Build automation.
* VMF generation and audit tooling.
* Development utilities.
* Configuration and manifest management.
* Candidate proposals for future versions.
* Applications that use VMF.

Business-specific implementations are intentionally outside the Build.xlam and VMF framework scope, except where they are placed under `applications/` as consumers of VMF.

---

# 6. Current Repository Structure

The current repository structure is organized as follows:

```text
specs/          Official Build and VMF specifications
    build/      Build canon, architecture, API, module, and release specifications
    vmf/        VMF specifications

src/            Source code and generated project structures
    Build/      Build.xlam source
    VMF/        VMF generated structure and layer placeholders

tests/          Unit and integration tests
    unit/
    integration/

tools/          Build, test, and VMF tools
    build/
    test/
    vmf/

candidates/     Future-version candidate proposals
docs/           Development and release documentation
templates/      Generation templates
prompts/        AI prompts
assets/         Static assets
applications/   Applications that use VMF
dist/           Generated distribution artifacts only
```

Released Build artifacts are stored under `dist/release/Build/`, such as `dist/release/Build/v1.0.2/Build.xlam`.

---

# 7. Quality Goals

The framework should prioritize:

* Maintainability.
* Extensibility.
* Readability.
* Reliability.
* Consistency.
* Testability.
* Reusability.
* Predictability.

Performance optimization should never compromise architectural integrity.

---

# 8. Target Architecture

The standard architecture follows the layered model defined by `specs/build/LayerSpecification_v2.0.md`:

* Presentation.
* Application.
* Domain.
* Infrastructure.
* Common.

Dependencies shall always flow in a single permitted direction.

Circular dependencies are prohibited.

Build.xlam is the development platform and tooling subsystem for generating, building, testing, and releasing VMF-based projects. It is not an additional standard application layer in the layer model.

VMF v1.0 is the frozen implementation contract beneath Canon v2.0 and the applicable Build specifications.

---

# 9. Governance

All specifications shall conform to the project canon and the applicable specification hierarchy.

The following documents define the authoritative design and implementation contract:

1. `specs/build/Canon_v2.0.md`
2. `specs/build/Architecture_v2.0.md`
3. `specs/build/SpecificationHierarchy_v2.0.md`
4. `specs/vmf/VMF_v1.0.md`

Lower-level specifications shall not contradict higher-level specifications.

Candidate proposals under `candidates/` shall not modify frozen specifications until formally adopted.

---

# 10. Success Criteria

The project is considered successful when:

* New modules can be implemented using only the published specifications.
* Public APIs remain stable across frozen versions.
* Internal implementations can evolve independently.
* New functionality can be added with minimal impact.
* Development practices remain consistent across the entire repository.
* Release artifacts are reproducible from their authoritative sources.

---

# 11. Future Evolution

Major architectural changes shall be introduced only through new major versions.

Backward-compatible improvements should be introduced through minor versions.

Experimental ideas shall be documented separately as candidate proposals and shall not modify frozen specifications.

VMF v1.0 is frozen. Enhancements, redesigns, incompatible corrections, or architectural improvements shall be proposed as VMF v1.1 Candidate or a later candidate version until formally adopted.

---

# 12. Document Relationship

```text
Vision
  -> Canon
      -> Architecture
          -> Specification Hierarchy
              -> Build Specifications
              -> VMF Specification
                  -> Source, Tests, Tools, Releases, and Applications
```

---

# 13. References

* `specs/build/Canon_v2.0.md`
* `specs/build/Architecture_v2.0.md`
* `specs/build/SpecificationHierarchy_v2.0.md`
* `specs/build/LayerSpecification_v2.0.md`
* `specs/build/Glossary_v2.0.md`
* `specs/build/Manifest_v2.0.md`
* `specs/vmf/VMF_v1.0.md`
* `AGENTS.md`
