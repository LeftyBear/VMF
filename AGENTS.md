# AGENTS.md

## AI Development Guide for VMF

This document defines the operational rules for AI coding assistants (e.g. Codex) working on the VMF project.

This document governs **how to work**.
It does **not** define the software specification itself.

---

# 1. Purpose

The objective of this repository is to develop and maintain the VMF architecture and its implementations while preserving long-term maintainability, consistency, and architectural integrity.

AI assistants SHALL prioritize architectural consistency over implementation convenience.

---

# 2. Source of Truth

The following documents define the project.

Priority (highest first):

1. Canon v2.0
2. VMF v1.0
3. Module Specifications
4. API Specifications
5. Source Code

If multiple documents conflict, the higher-priority document SHALL take precedence.

---

# 3. Architecture Rules

The AI MUST follow Canon v2.0.

The AI MUST follow VMF v1.0.

The AI MUST NOT introduce architectural changes without explicit instruction.

If an improvement is identified, it SHALL be proposed separately as:

> v1.1 Candidate

The current implementation MUST remain compliant with VMF v1.0.

---

# 4. Repository Structure

```
spec/
    canon/
    vmf/
    architecture/
    api/
    modules/

src/

test/

tools/
```

The AI MUST place files in the appropriate directory.

---

# 5. Specification Rules

Specifications belong only under `spec`.

Source code belongs only under `src`.

Tests belong only under `test`.

Tools belong only under `tools`.

The AI MUST NOT mix specifications with implementation.

---

# 6. Coding Rules

The AI SHALL:

* preserve existing architecture
* minimize modifications
* avoid unrelated changes
* follow existing naming conventions
* maintain one-way dependencies
* preserve public APIs unless instructed otherwise

---

# 7. Dependency Rules

Circular dependencies are prohibited.

Higher layers MAY depend on lower-layer contracts.

Lower layers MUST NOT depend on higher layers.

Facade and CompositionRoot SHALL define architectural boundaries.

---

# 8. Implementation Policy

Unless explicitly instructed:

* do not redesign
* do not rename public APIs
* do not change file structure
* do not change specifications

Implement only the requested scope.

---

# 9. Documentation Policy

When documentation is requested:

* write in Markdown
* use UTF-8 encoding
* keep documents consistent with Canon

When VBA source is generated:

* use Shift_JIS encoding where required

---

# 10. Review Policy

When reviewing code:

* report architectural violations
* report dependency violations
* report specification inconsistencies
* do not rewrite code unless requested

---

# 11. Git Policy

Keep commits small.

Keep changes focused.

Avoid unrelated edits.

Provide a summary of modified files.

---

# 12. AI Behavior

The AI SHALL:

* think before editing
* prefer consistency
* preserve repository history
* avoid speculative modifications
* ask when requirements are ambiguous

The AI MUST NOT modify specifications without explicit approval.

---

# 13. Version Policy

Canon evolves.

VMF evolves only through new versions.

VMF v1.0 is frozen.

Changes SHALL be proposed as:

* VMF v1.1 Candidate

until officially adopted.

---

# 14. Project Philosophy

Architecture first.

Specification before implementation.

Implementation follows specification.

Maintainability over convenience.

Consistency over cleverness.

Single Source of Truth.

Long-term evolution over short-term optimization.
