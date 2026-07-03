# SpecificationHierarchy v2.0

**Build.xlam Project**

| Item | Value |
|------|-------|
| Version | 2.0 |
| Status | Frozen |
| Encoding | UTF-8 |
| Format | Markdown (GitHub Compatible) |

---

# Part 1. Purpose

## 1.1 Objective

This document defines the **Specification Hierarchy** of the Build.xlam Project.

Its purpose is to establish a single, unambiguous hierarchy of specifications so that every specification has a clearly defined responsibility, dependency direction, and authority.

This document **MUST NOT** define implementation details.

Implementation specifications belong to lower-level documents.

---

## 1.2 Scope

This specification applies to every specification document contained within the Build.xlam Project.

It defines:

- Specification hierarchy
- Document responsibilities
- Dependency relationships
- Document authority
- SSOT application
- Reference rules
- Change management principles
- Document application order

This specification **MUST NOT** define:

- Implementation
- Source code
- VBA
- API specifications
- Class design
- Directory structure
- Naming conventions
- Development workflow

---

## 1.3 Position in the Specification Hierarchy

This document is a lower-level specification of:

- `Canon_v2.0.md`
- `Architecture_v2.0.md`

This document supplements those specifications.

If any inconsistency exists, the higher-level specification **MUST** take precedence.

---

# Part 2. Design Principles

The Specification Hierarchy exists to achieve the following goals.

## 2.1 Single Source of Truth

Every rule **MUST** be defined in exactly one specification.

Duplicate definitions **MUST NOT** exist.

---

## 2.2 Clear Responsibility

Each specification **MUST** have exactly one primary responsibility.

Responsibilities **SHOULD NOT** overlap.

---

## 2.3 One-way Dependency

Specification dependencies **MUST** form a directed acyclic hierarchy.

Circular document dependencies **MUST NOT** exist.

---

## 2.4 Consistency

All lower-level specifications **MUST** remain consistent with their parent specifications.

Lower-level documents **MUST NOT** redefine higher-level rules.

---

## 2.5 Maintainability

The hierarchy **SHOULD** minimize the impact of future specification changes.

Changes **SHOULD** propagate only from higher-level specifications to lower-level specifications.
# Part 3. Specification Hierarchy

## 3.1 Hierarchy Overview

The Build.xlam Project specification hierarchy **MUST** be organized as a layered structure.

Each specification belongs to exactly one layer.

A lower layer **MUST** depend only on specifications located in higher layers.

A higher layer **MUST NOT** depend on lower-layer specifications.

---

## 3.2 Hierarchy Structure

| Layer | Specification | Responsibility |
|--------|---------------|----------------|
| 1 | `Canon_v2.0.md` | Defines the fundamental principles, constraints, terminology, and governing rules of the project. |
| 2 | `Architecture_v2.0.md` | Defines the overall system architecture based on the Canon. |
| 3 | `SpecificationHierarchy_v2.0.md` | Defines the specification system, document hierarchy, authority, and dependency relationships. |
| 4 | Domain Specifications | Define specifications for each architectural domain. |
| 5 | Component Specifications | Define the behavior and responsibilities of individual components. |
| 6 | Interface Specifications | Define contracts between components and layers. |
| 7 | Operational Specifications | Define operational rules, maintenance, deployment, and lifecycle management. |

---

## 3.3 Dependency Rules

Specifications **MUST** reference only specifications located in higher layers.

Reverse dependencies are prohibited.

Cross-layer references **SHOULD** be minimized.

Circular dependencies **MUST NOT** exist.

---

## 3.4 Layer Independence

Each layer **SHOULD** remain logically independent.

A specification **MUST NOT** duplicate the responsibilities of another specification.

Common definitions **MUST** be referenced from their authoritative specification instead of being copied.

---

## 3.5 Hierarchy Stability

The hierarchy itself **SHOULD** remain stable over time.

New specifications **MAY** be added as lower-level documents without requiring modifications to higher-level specifications, provided that they conform to the established hierarchy and do not violate existing responsibilities.
# Part 4. Responsibilities of Specifications

## 4.1 Responsibility Principle

Each specification **MUST** have one clearly defined primary responsibility.

A specification **MUST NOT** assume responsibilities assigned to another specification.

Each rule **MUST** have exactly one authoritative source.

---

## 4.2 Canon Specification

**Canonical Specification**

Document:

- `Canon_v2.0.md`

Responsibilities:

- Define the fundamental principles of the Build.xlam Project.
- Define project-wide terminology.
- Define mandatory design principles.
- Define project-wide constraints.
- Serve as the highest-level normative specification.

The Canon **MUST NOT** contain implementation-specific information.

---

## 4.3 Architecture Specification

Document:

- `Architecture_v2.0.md`

Responsibilities:

- Define the overall architecture of the system.
- Define architectural layers.
- Define architectural responsibilities.
- Define dependency directions.
- Define architectural constraints.

The Architecture specification **MUST** conform to the Canon.

---

## 4.4 Specification Hierarchy Specification

Document:

- `SpecificationHierarchy_v2.0.md`

Responsibilities:

- Define the specification hierarchy.
- Define document authority.
- Define dependency relationships among specifications.
- Define SSOT application.
- Define document reference rules.
- Define specification application order.
- Define specification change management principles.

This specification **MUST NOT** define implementation or architectural behavior.

---

## 4.5 Lower-level Specifications

Lower-level specifications **MAY** define detailed requirements within their assigned scope.

They **MUST** conform to all higher-level specifications.

They **MUST NOT** redefine:

- Canonical principles
- Architectural rules
- Specification hierarchy
- Document authority

---

## 4.6 Responsibility Boundaries

When a rule belongs to another specification, the current specification **MUST** reference the authoritative document instead of redefining the rule.

Responsibility duplication **MUST NOT** occur across specifications.

Each specification **SHOULD** remain cohesive, with a clearly defined and limited scope of responsibility.
# Part 5. Specification Dependencies

## 5.1 Dependency Principle

Specification dependencies **MUST** be unidirectional.

Every specification **MUST** depend only on specifications located above it in the hierarchy.

Reverse dependencies **MUST NOT** exist.

---

## 5.2 Dependency Graph

The specification hierarchy is defined as follows:

| Level | Specification |
|------:|---------------|
| 1 | `Canon_v2.0.md` |
| 2 | `Architecture_v2.0.md` |
| 3 | `SpecificationHierarchy_v2.0.md` |
| 4 | Domain Specifications |
| 5 | Component Specifications |
| 6 | Interface Specifications |
| 7 | Operational Specifications |

Each specification **MUST** depend only on specifications located above it in the hierarchy.

All dependency relationships **MUST** follow this direction.

---

## 5.3 Permitted Dependencies

A specification **MAY** reference:

- Its direct parent specification
- Any higher-level specification
- Shared normative specifications defined above it

Such references **MUST** preserve the established dependency direction.

---

## 5.4 Prohibited Dependencies

A specification **MUST NOT**:

- Depend on lower-level specifications
- Create circular dependencies
- Introduce bidirectional document relationships
- Require implementation documents to interpret higher-level specifications

These constraints ensure that higher-level specifications remain independent of implementation details.

---

## 5.5 Dependency Integrity

When introducing a new specification, its dependency relationships **MUST** be explicitly defined.

The addition of a new specification **MUST NOT** violate the existing dependency graph.

Any change that introduces ambiguity or cyclic dependencies **MUST NOT** be accepted.

---

## 5.6 Dependency Validation

The specification hierarchy **SHOULD** be periodically reviewed to verify that:

- Dependency directions remain correct.
- No circular references have been introduced.
- Responsibilities remain aligned with the hierarchy.
- Higher-level specifications remain independent of lower-level documents.

Maintaining dependency integrity is essential for long-term maintainability and consistent evolution of the Build.xlam Project specification system.
# Part 6. Single Source of Truth (SSOT)

## 6.1 Principle

The Build.xlam Project **MUST** adopt the **Single Source of Truth (SSOT)** principle for all specifications.

Every rule, definition, constraint, and normative statement **MUST** have exactly one authoritative specification.

---

## 6.2 Authoritative Source

Each specification **MUST** act as the authoritative source only within its defined responsibility.

A specification **MUST NOT** become the authoritative source for subjects assigned to another specification.

Authority is determined by the Specification Hierarchy.

---

## 6.3 Rule Ownership

Every normative rule **MUST** have a single owner.

For each rule:

- One specification **MUST** define the rule.
- Other specifications **MUST** reference the rule.
- Other specifications **MUST NOT** redefine the rule.
- Other specifications **MUST NOT** maintain independent copies of the rule.

---

## 6.4 Reference Instead of Duplication

When a specification requires a rule defined elsewhere, it **MUST** reference the authoritative specification.

Duplicating normative text across specifications **MUST NOT** occur.

Informative summaries **MAY** be included only when they do not replace or modify the authoritative definition.

---

## 6.5 Consistency

All specifications **MUST** remain consistent with the authoritative source.

If an authoritative specification changes, dependent specifications **MUST** be reviewed and updated as necessary to maintain consistency.

No dependent specification **MAY** override or reinterpret an authoritative rule.

---

## 6.6 Benefits of SSOT

Applying SSOT ensures that the specification system:

- Eliminates conflicting definitions.
- Prevents duplicated maintenance.
- Improves long-term maintainability.
- Simplifies impact analysis.
- Establishes clear document authority.
- Supports consistent evolution of the Build.xlam Project.

SSOT **SHOULD** be regarded as a mandatory governing principle for the entire specification hierarchy.
# Part 7. Specification Reference Rules

## 7.1 General Rule

Specifications **MUST** reference other specifications in accordance with the established Specification Hierarchy.

References **MUST NOT** introduce ambiguity regarding document authority or responsibility.

---

## 7.2 Upward References

A specification **MAY** reference:

- Its direct parent specification.
- Any higher-level specification.
- Other authoritative specifications that define shared rules.

Such references **MUST NOT** violate the dependency direction defined by this hierarchy.

---

## 7.3 Downward References

Higher-level specifications **MUST NOT** reference lower-level specifications as normative sources.

Lower-level documents represent applications of higher-level rules and therefore **MUST NOT** influence the interpretation of those rules.

Informative examples **MAY** reference lower-level documents when no normative dependency is introduced.

---

## 7.4 Cross References

Cross references between specifications at the same hierarchical level **SHOULD** be minimized.

Where such references are necessary, they **MUST** satisfy all of the following conditions:

- No circular dependency is created.
- Responsibility boundaries remain unchanged.
- SSOT is preserved.
- Document authority remains unambiguous.

---

## 7.5 Reference Method

References **SHOULD** identify the target specification explicitly.

Typical references include:

- Document name
- Section number
- Clause number (when applicable)

References **SHOULD** remain stable across document revisions whenever possible.

---

## 7.6 Normative and Informative References

References are classified as either:

### Normative References

Normative references define requirements that **MUST** be followed.

A normative reference becomes part of the applicable specification requirements.

### Informative References

Informative references provide additional context, background information, or explanatory material.

Informative references **MUST NOT** introduce new requirements or modify existing normative statements.

---

## 7.7 Reference Integrity

All references **SHOULD** remain valid throughout the lifecycle of the project.

Broken, ambiguous, or obsolete references **SHOULD** be corrected as part of specification maintenance.

Maintaining reference integrity supports consistency, traceability, and long-term maintainability across the Build.xlam Project specification system.
# Part 8. Specification Change Management

## 8.1 General Principle

All specification changes **MUST** preserve the integrity of the Specification Hierarchy.

Changes **MUST NOT** violate:

- Canonical principles
- Architectural principles
- SSOT
- Dependency rules
- Responsibility boundaries

---

## 8.2 Change Authority

Each specification **MUST** be modified only within its defined scope of responsibility.

A specification **MUST NOT** introduce changes that belong to another specification.

When a change affects multiple specifications, the authoritative specification **MUST** be updated first.

Dependent specifications **MUST** then be reviewed and updated as necessary.

---

## 8.3 Impact Analysis

Before modifying a specification, the potential impact on dependent specifications **SHOULD** be evaluated.

The review **SHOULD** include:

- Dependency relationships
- Responsibility boundaries
- SSOT compliance
- Reference integrity
- Consistency with higher-level specifications

---

## 8.4 Backward Consistency

Specification changes **SHOULD** minimize unnecessary disruption to existing lower-level specifications.

Where feasible, compatible evolution is preferred over structural redesign.

If an incompatible change is unavoidable, all affected specifications **MUST** be updated to restore consistency.

---

## 8.5 Version Management

Each specification **SHOULD** maintain an explicit version identifier.

Major version updates **SHOULD** indicate normative or structural changes.

Minor version updates **SHOULD** be limited to clarifications, editorial improvements, or other non-breaking revisions.

Version numbering **SHOULD** remain consistent across the Build.xlam Project.

---

## 8.6 Review Requirements

Every specification change **SHOULD** undergo a consistency review prior to publication.

The review **SHOULD** confirm:

- Compliance with the Canon.
- Compliance with the Architecture.
- Compliance with the Specification Hierarchy.
- Preservation of SSOT.
- Preservation of dependency integrity.
- Preservation of document responsibilities.

---

## 8.7 Frozen Specifications

A specification designated as **Frozen** **MUST NOT** be modified except through an approved version revision.

Any proposed enhancement or redesign **SHOULD** be introduced as a subsequent version rather than by altering the frozen specification directly.

This principle ensures long-term traceability, reproducibility, and stability of the Build.xlam Project specification system.
# Part 9. Specification Application Order

## 9.1 General Principle

Specifications **MUST** be interpreted and applied in accordance with the established Specification Hierarchy.

Higher-level specifications define the governing principles for all lower-level specifications.

Lower-level specifications **MUST** conform to all applicable higher-level specifications.

---

## 9.2 Application Order

Unless otherwise specified, specifications **MUST** be applied in the following order:

1. `Canon_v2.0.md`
2. `Architecture_v2.0.md`
3. `SpecificationHierarchy_v2.0.md`
4. Domain Specifications
5. Component Specifications
6. Interface Specifications
7. Operational Specifications

Each specification **MUST** inherit the constraints and requirements defined by all preceding specifications.

---

## 9.3 Conflict Resolution

If conflicting statements are identified, the specification with the higher authority **MUST** take precedence.

The order of precedence is determined by the Specification Hierarchy.

Lower-level specifications **MUST NOT** override or reinterpret higher-level requirements.

Any detected inconsistency **SHOULD** be resolved by correcting the lower-level specification rather than modifying the authoritative source.

---

## 9.4 Applicability

Each specification **MUST** be applied only within its defined scope.

Requirements outside the scope of a specification **MUST** be defined by the appropriate authoritative specification.

Specifications **MUST NOT** expand their responsibilities beyond those assigned by the hierarchy.

---

## 9.5 Traceability

Every normative requirement **SHOULD** be traceable to its authoritative specification.

Traceability **SHOULD** enable reviewers to determine:

- The origin of a requirement.
- The governing specification.
- The applicable dependency path.
- The responsible specification owner.

Maintaining traceability improves consistency, reviewability, and long-term maintenance.

---

# Part 10. Maintenance and Future Evolution

## 10.1 Stability

The Specification Hierarchy **SHOULD** remain stable throughout the lifecycle of the Build.xlam Project.

Structural modifications to the hierarchy **SHOULD** be minimized.

---

## 10.2 Extensibility

New specifications **MAY** be introduced when additional responsibilities are required.

New specifications **MUST**:

- Conform to the established hierarchy.
- Preserve SSOT.
- Preserve dependency integrity.
- Maintain clear responsibility boundaries.
- Avoid introducing circular dependencies.

---

## 10.3 Long-term Maintainability

The specification system **SHOULD** support long-term maintenance by ensuring:

- Clear document authority.
- Stable dependency relationships.
- Consistent terminology.
- Predictable change management.
- Complete traceability.
- Minimal duplication.

---

## 10.4 Compliance

All specifications within the Build.xlam Project **MUST** comply with:

- `Canon_v2.0.md`
- `Architecture_v2.0.md`
- `SpecificationHierarchy_v2.0.md`

Compliance with these specifications ensures that the project maintains a coherent, maintainable, and authoritative specification system throughout its lifecycle.
