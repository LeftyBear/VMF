# LayerSpecification_v2.0

Version: 2.0
Status: Frozen
Target: Build.xlam Project

---

# 1. Introduction

## 1.1 Purpose

This specification defines the architectural layer model adopted by Build.xlam.

The objective of this document is to establish:

- the responsibilities of each architectural layer,
- the permitted dependency directions,
- communication rules between layers,
- visibility boundaries,
- ownership of objects and state,
- layer isolation requirements, and
- implementation constraints necessary to preserve long-term maintainability.

This specification does not redefine modules, classes, interfaces, APIs, facades, or dependency rules. Those are defined by their dedicated specifications.

---

## 1.2 Scope

This specification applies to every executable component within the Build.xlam project, including but not limited to:

- standard modules,
- class modules,
- interface classes,
- Facades,
- Composition Roots,
- service implementations,
- repositories,
- domain models,
- infrastructure components,
- configuration providers, and
- utility components.

Every implementation SHALL belong to exactly one architectural layer.

No implementation SHALL exist outside the defined layer model.

---

## 1.3 Goals

The layer architecture SHALL ensure:

- clear separation of responsibilities,
- one-directional dependencies,
- implementation isolation,
- replaceable infrastructure,
- stable APIs,
- testability,
- maintainability,
- predictable control flow,
- minimal coupling, and
- long-term extensibility.

---

## 1.4 Relationship to Other Specifications

This specification SHALL be interpreted together with:

- Canon_v2.0.md
- Architecture_v2.0.md
- DependencySpecification_v2.0.md
- ModuleSpecification_v2.0.md
- ClassSpecification_v2.0.md
- InterfaceSpecification_v2.0.md
- ApiSpecification_v2.0.md
- FacadeSpecification_v2.0.md
- CompositionRootSpecification_v2.0.md

If any conflict exists, the higher-level specification SHALL take precedence.

---

## 1.5 Non-Goals

This specification does not define:

- business logic,
- module structures,
- class structures,
- API signatures,
- interface members,
- dependency injection implementation,
- configuration values,
- exception definitions, or
- application workflows.

Those concerns are governed by their respective specifications.

---
# 2. Layer Architecture

## 2.1 Layer Model

Build.xlam SHALL adopt a fixed layered architecture.

Each layer SHALL have a clearly defined responsibility.

Each implementation component SHALL belong to one and only one layer.

A layer SHALL NOT contain responsibilities assigned to another layer.

---

## 2.2 Standard Layer Structure

The standard architectural layers are:

1. Presentation Layer
2. Application Layer
3. Domain Layer
4. Infrastructure Layer
5. Common Layer

The actual physical folder structure MAY differ from the conceptual layer structure.

Layer classification SHALL be determined by responsibility rather than file location.

---

## 2.3 Presentation Layer

The Presentation Layer is responsible for interaction with Excel users.

Typical responsibilities include:

- Ribbon callbacks
- UserForms
- Dialogs
- Worksheet interaction
- User input
- Output formatting
- Command initiation

The Presentation Layer SHALL NOT contain business rules.

The Presentation Layer SHALL communicate with lower layers only through published APIs or Facades.

---

## 2.4 Application Layer

The Application Layer coordinates application use cases.

Responsibilities include:

- use case orchestration
- workflow coordination
- transaction boundaries
- command execution
- service coordination

The Application Layer SHALL NOT implement infrastructure-specific logic.

The Application Layer SHALL NOT directly manipulate Excel UI components.

---

## 2.5 Domain Layer

The Domain Layer contains the business model.

Responsibilities include:

- domain entities
- value objects
- domain services
- domain rules
- validation
- business calculations

The Domain Layer SHALL remain independent of UI and infrastructure technologies.

Business logic SHALL reside primarily within this layer.

---

## 2.6 Infrastructure Layer

The Infrastructure Layer provides technical implementations.

Responsibilities include:

- file access
- workbook access
- worksheet access
- external communication
- configuration loading
- persistence
- logging
- operating system interaction

Infrastructure components SHALL implement contracts defined by upper layers whenever practical.

---

## 2.7 Common Layer

The Common Layer provides reusable technical utilities shared across the project.

Typical components include:

- common helper classes
- shared abstractions
- utility functions
- framework support
- generic collections
- common contracts

The Common Layer SHALL remain independent from business-specific concerns.

---
# 3. Layer Responsibilities

## 3.1 Responsibility Assignment

Each architectural layer SHALL have a single primary responsibility.

Responsibilities SHALL NOT overlap between layers.

A responsibility SHALL be assigned to the highest appropriate layer that can own it without violating dependency rules.

Implementation details SHALL remain within the owning layer.

---

## 3.2 Presentation Layer Responsibilities

The Presentation Layer SHALL be responsible for:

- receiving user actions,
- validating presentation-level input,
- invoking application services,
- displaying execution results,
- presenting error messages,
- managing UserForm state, and
- controlling user interaction.

The Presentation Layer SHALL NOT:

- implement business rules,
- perform persistence,
- contain domain calculations, or
- access infrastructure implementations directly.

---

## 3.3 Application Layer Responsibilities

The Application Layer SHALL be responsible for:

- coordinating use cases,
- controlling execution flow,
- invoking domain operations,
- coordinating repositories,
- coordinating infrastructure services,
- translating between presentation and domain models where required, and
- determining application-level execution order.

The Application Layer SHALL remain independent of Excel-specific user interface behavior.

---

## 3.4 Domain Layer Responsibilities

The Domain Layer SHALL own all business knowledge.

Responsibilities include:

- enforcing business constraints,
- maintaining invariant consistency,
- implementing business calculations,
- validating domain state,
- creating domain objects,
- expressing business terminology, and
- protecting domain integrity.

The Domain Layer SHALL NOT depend upon Presentation or Infrastructure concerns.

---

## 3.5 Infrastructure Layer Responsibilities

The Infrastructure Layer SHALL provide concrete implementations for technical capabilities.

Responsibilities include:

- workbook operations,
- worksheet operations,
- file system access,
- configuration retrieval,
- serialization,
- logging,
- communication with Excel object models, and
- external resource access.

Infrastructure SHALL encapsulate implementation-specific details behind stable contracts whenever applicable.

---

## 3.6 Common Layer Responsibilities

The Common Layer SHALL provide reusable functionality that is independent of business domains.

Examples include:

- reusable utility classes,
- generic algorithms,
- framework abstractions,
- shared constants,
- helper methods,
- reusable interfaces, and
- common data structures.

The Common Layer SHALL avoid assumptions about specific business contexts.

---

## 3.7 Responsibility Isolation

Responsibilities SHALL remain isolated.

A layer SHALL NOT perform work assigned to another layer merely for implementation convenience.

Cross-layer duplication SHALL be avoided.

Where responsibility is unclear, ownership SHALL follow the principles defined in Canon_v2.0.md.

---
# 4. Layer Communication

## 4.1 General Principles

Communication between architectural layers SHALL follow the dependency rules defined in DependencySpecification_v2.0.md.

All interactions SHALL preserve layer isolation.

A layer SHALL communicate only through published contracts, APIs, Facades, or interfaces defined by the owning layer.

---

## 4.2 Allowed Communication Direction

Layer communication SHALL follow a top-down flow.

The standard communication path is:

```text
Presentation
    ↓
Application
    ↓
Domain
    ↓
Infrastructure
```

The Common Layer MAY be referenced by any layer, provided such references do not introduce circular dependencies or business-specific coupling.

No reverse communication SHALL occur through concrete implementations.

---

## 4.3 Communication Through Contracts

A layer SHALL NOT depend directly on the internal implementation details of another layer.

Communication SHALL occur through one or more of the following:

- public APIs,
- published Facades,
- interfaces,
- immutable value objects,
- domain models, or
- standardized result types.

Internal helper classes SHALL remain inaccessible outside their owning layer.

---

## 4.4 Data Transfer

Data exchanged between layers SHALL be explicit and well-defined.

Objects transferred across layer boundaries SHALL:

- represent stable contracts,
- avoid exposing internal mutable state,
- preserve ownership semantics, and
- minimize unnecessary coupling.

Temporary implementation-specific objects SHALL NOT cross layer boundaries.

---

## 4.5 Error Propagation

Errors SHALL propagate according to ErrorHandling_v2.0.md.

Lower layers SHALL report failures upward without introducing presentation-specific behavior.

Presentation-specific error handling SHALL be performed only within the Presentation Layer.

---

## 4.6 State Ownership

Each layer SHALL own the state that it creates and manages.

No layer SHALL directly modify the internal state owned by another layer except through published operations.

Ownership SHALL remain explicit throughout the object lifecycle.

---

## 4.7 Communication Restrictions

A layer SHALL NOT:

- bypass intermediate layers,
- invoke private implementation classes,
- manipulate another layer's internal state,
- depend upon implementation-specific modules,
- expose internal helper objects as public contracts, or
- create circular communication paths.

These restrictions SHALL be enforced throughout the project lifecycle.

---
# 5. Layer Dependency Rules

## 5.1 Dependency Principles

Layer dependencies SHALL conform to the one-directional dependency principle defined in Canon_v2.0.md and DependencySpecification_v2.0.md.

A dependency SHALL always point toward a lower architectural layer or to the Common Layer where permitted.

No dependency SHALL violate the established architectural hierarchy.

---

## 5.2 Permitted Dependencies

The following dependencies are permitted:

| Layer | Permitted Dependencies |
|--------|------------------------|
| Presentation | Application, Common |
| Application | Domain, Infrastructure (through contracts where applicable), Common |
| Domain | Common |
| Infrastructure | Common |
| Common | None |

The existence of a permitted dependency SHALL NOT imply that every implementation should use it.

Dependencies SHALL be introduced only when required.

---

## 5.3 Prohibited Dependencies

The following dependencies SHALL be prohibited:

- Domain → Presentation
- Domain → Application
- Infrastructure → Application
- Infrastructure → Presentation
- Common → Domain
- Common → Application
- Common → Presentation
- Circular dependencies of any kind

Any dependency not explicitly permitted SHALL be considered prohibited.

---

## 5.4 Dependency Visibility

A layer SHALL expose only those components intended for external use.

Internal implementation classes SHALL remain inaccessible outside the owning layer.

Visibility SHALL be minimized to reduce coupling.

---

## 5.5 Dependency Through Interfaces

Where implementation replacement is anticipated, dependencies SHOULD target interfaces rather than concrete classes.

Interface ownership SHALL remain within the appropriate architectural layer as defined by InterfaceSpecification_v2.0.md.

Concrete implementations SHALL remain replaceable without affecting dependent layers.

---

## 5.6 Dependency Validation

Architectural compliance SHALL be verified during development.

Validation SHOULD confirm:

- absence of circular dependencies,
- permitted dependency directions,
- correct layer ownership,
- appropriate visibility,
- interface usage where required, and
- consistency with DependencySpecification_v2.0.md.

Architectural violations SHALL be corrected before release.

---

## 5.7 Future Extensions

Additional layers SHALL NOT be introduced unless formally defined by a future revision of the architecture.

Extensions SHALL preserve:

- one-directional dependencies,
- responsibility isolation,
- contract-based communication,
- implementation encapsulation, and
- compatibility with existing specifications.

Any future layer SHALL receive its own formal specification before adoption.

---
# 6. Layer Design Guidelines

## 6.1 General Guidelines

Every layer SHALL remain cohesive and focused on its defined responsibility.

Implementations SHALL prioritize readability, maintainability, and replaceability over implementation convenience.

Layer boundaries SHALL remain explicit throughout the project.

---

## 6.2 Separation of Concerns

Business concerns, presentation concerns, and technical concerns SHALL remain separated.

A layer SHALL NOT assume responsibilities belonging to another layer.

Temporary implementation shortcuts SHALL NOT justify crossing architectural boundaries.

---

## 6.3 Encapsulation

Each layer SHALL encapsulate its internal implementation details.

Only documented public contracts SHALL be visible outside the layer.

Internal helper modules, private classes, and implementation-specific utilities SHALL remain hidden.

---

## 6.4 Cohesion

Components within the same layer SHOULD collaborate to achieve closely related objectives.

Responsibilities that naturally belong together SHOULD remain within the same layer.

Unrelated responsibilities SHALL NOT be grouped for convenience.

---

## 6.5 Coupling

Inter-layer coupling SHALL be minimized.

Dependencies SHOULD be introduced only when they provide clear architectural value.

Implementation-specific knowledge SHALL NOT leak across layer boundaries.

---

## 6.6 Replaceability

Where practical, implementations SHOULD be replaceable without requiring changes to higher layers.

Stable contracts SHALL be preferred over implementation-specific interactions.

Infrastructure implementations SHOULD be replaceable with minimal impact on application behavior.

---

## 6.7 Testability

Layer design SHOULD facilitate isolated testing.

Business logic SHOULD be testable independently from:

- Excel UI,
- workbook structure,
- worksheets,
- file system,
- external resources, and
- infrastructure implementations.

Layer boundaries SHALL support independent verification wherever practical.

---

## 6.8 Maintainability

Architectural decisions SHALL favor long-term maintainability.

When multiple implementation choices satisfy the specification, the alternative with lower coupling and higher clarity SHOULD be preferred.

Design consistency SHALL take precedence over local optimization.

---

## 7. Compliance

### 7.1 General Compliance

Every implementation within Build.xlam SHALL comply with this specification.

Architectural compliance SHALL be considered a mandatory quality requirement.

Non-compliant implementations SHALL be corrected before integration into the project.

---

### 7.2 Layer Verification

During design and implementation, verification SHALL confirm at minimum:

- every component belongs to exactly one architectural layer,
- responsibilities are correctly assigned,
- dependency directions are valid,
- layer boundaries are preserved,
- communication follows published contracts,
- implementation visibility is appropriate, and
- no prohibited dependencies exist.

---

### 7.3 Consistency with Other Specifications

This specification SHALL remain consistent with:

- Canon_v2.0.md
- Architecture_v2.0.md
- DependencySpecification_v2.0.md
- ModuleSpecification_v2.0.md
- ClassSpecification_v2.0.md
- InterfaceSpecification_v2.0.md
- ApiSpecification_v2.0.md
- FacadeSpecification_v2.0.md
- CompositionRootSpecification_v2.0.md

Where overlapping topics exist, this specification defines only the layer-specific requirements.

---

### 7.4 Exception Handling

No implementation MAY violate the layer model unless explicitly permitted by a higher-level specification.

Temporary implementation convenience SHALL NOT constitute a valid exception.

Architectural exceptions SHALL be documented, justified, reviewed, and approved before adoption.

---

### 7.5 Backward Compatibility

Future revisions of the layer model SHOULD preserve compatibility whenever practical.

Where incompatible architectural changes become necessary, they SHALL be introduced through a new specification version rather than modifying this frozen specification.

---

### 7.6 Normative References

The following specifications are normative for this document:

- Canon_v2.0.md
- Architecture_v2.0.md
- SpecificationHierarchy_v2.0.md
- Glossary_v2.0.md
- DocumentTemplate_v2.0.md
- Manifest_v2.0.md
- ErrorHandling_v2.0.md
- Configuration_v2.0.md
- ModuleSpecification_v2.0.md
- ClassSpecification_v2.0.md
- InterfaceSpecification_v2.0.md
- ApiSpecification_v2.0.md
- FacadeSpecification_v2.0.md
- CompositionRootSpecification_v2.0.md
- DependencySpecification_v2.0.md

---

### Appendix A. Layer Dependency Matrix (Normative)

| From | Presentation | Application | Domain | Infrastructure | Common |
|------|:------------:|:-----------:|:------:|:--------------:|:------:|
| Presentation | — | ✓ | ✗ | ✗ | ✓ |
| Application | ✗ | — | ✓ | ✓* | ✓ |
| Domain | ✗ | ✗ | — | ✗ | ✓ |
| Infrastructure | ✗ | ✗ | ✗ | — | ✓ |
| Common | ✗ | ✗ | ✗ | ✗ | — |

\* Infrastructure dependencies SHALL follow the contract-based dependency principles defined by DependencySpecification_v2.0.md.
