# Build Blueprint v1.0.1

Version : 1.0.1
Status  : Frozen
Scope   : Build.xlam
Depends : Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md

---

# 1. Purpose

This document defines the approved Build.xlam blueprint for Build v1.0.1.

Build v1.0.1 is a Manifest Driven Generator based on Blueprint, Manifest, Template, GenerateContext, Token Replace, and Generator responsibilities.

---

# 2. Scope

Build v1.0.1 includes:

- Manifest Driven generation
- Template Driven code output
- GenerateContext-based variable data
- Token replacement
- A single Generator Engine
- Project generation through composed generation steps

Build v1.0.1 does not include Build v1.1 Candidate behavior.

---

# 3. Authoritative Flow

The approved generation flow is:

1. Blueprint
2. Manifest
3. Template
4. GenerateContext
5. Token Replace
6. Generator
7. VBProject

Blueprint is the design source.

Manifest defines generation targets.

Template defines generated code forms.

GenerateContext holds variable generation data.

Generator reflects generated output into the VBA project.

---

# 4. Architectural Layers

Build v1.0.1 follows the layer direction defined by Canon v2.0 and BuildCanon_v1.0.md:

1. Presentation
2. Application
3. Composition Root
4. Infrastructure

Infrastructure includes:

- Manifest Provider
- Template Provider
- Token Replacer
- Generator

Lower layers SHALL NOT depend on higher layers.

---

# 5. Generator Composition

Large generation operations SHALL be composed from smaller generation operations.

The approved composition model is:

1. GenerateProject
2. GenerateLayer
3. GenerateComponent

All generation behavior SHALL use the single Generator Engine.

---

# 6. v1.1 Exclusions

Build v1.1 Candidate items are excluded from Build v1.0.1.

Candidate items SHALL be defined in BuildCandidates_v1.1.md and SHALL NOT be treated as Build v1.0.1 behavior until formally adopted.

---

# 7. Release Criteria

Build v1.0.1 release readiness SHALL be evaluated with BuildReleaseChecklist_v1.0.md.

Documentation consistency SHALL be evaluated with BuildDocumentationStandard_v1.0.md.
