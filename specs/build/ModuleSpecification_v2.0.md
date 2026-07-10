# Module Specification v2.0

| Item | Value |
|------|-------|
| Document | ModuleSpecification_v2.0.md |
| Version | 2.0 |
| Status | Frozen |
| Authority | Build.xlam Architecture |
| Scope | Standard Module Design Rules |
| Depends on | Canon, Architecture, LayerSpecification, DependencySpecification, ApiSpecification, NamingConvention |
| Applies to | All Standard Modules |

---

# 1. Purpose

## 1.1 Objective

This specification defines the design rules, responsibilities, implementation constraints, visibility rules, and structural requirements for every Standard Module used throughout Build.xlam.

The objective is to ensure that every Standard Module has a single, well-defined responsibility and remains maintainable, testable, reusable, and independent from unrelated implementation details.

---

## 1.2 Scope

This specification applies to every VBA Standard Module contained in every Build.xlam project.

This specification SHALL NOT apply to:

- Class Modules
- Interface Modules
- UserForms
- Workbook Modules
- Worksheet Modules

Those components are governed by their own specifications.

---

## 1.3 Relationship with Other Specifications

This document supplements the following specifications.

- Canon_v2.0.md
- Architecture_v2.0.md
- LayerSpecification_v2.0.md
- DependencySpecification_v2.0.md
- NamingConvention_v2.0.md
- ApiSpecification_v2.0.md
- ClassSpecification_v2.0.md
- InterfaceSpecification_v2.0.md

If any conflict exists, the higher-level specification SHALL take precedence.

---

# 2. Definition of Standard Module

A Standard Module is a VBA module (*.bas) that provides stateless procedures and functions shared across the application.

A Standard Module SHALL NOT represent domain state.

A Standard Module SHALL NOT own object lifetime.

A Standard Module SHALL NOT maintain mutable application state.

A Standard Module SHALL provide procedural behavior only.
# 3. Responsibilities

## 3.1 Primary Responsibilities

A Standard Module MAY provide one of the following responsibilities.

- Public API entry points
- Internal helper procedures
- Data conversion
- Validation
- Factory procedures
- Utility functions
- Layer Facade implementation
- Error construction helpers
- Configuration access helpers

A module SHALL have exactly one primary responsibility.

---

## 3.2 Single Responsibility Principle

Every Standard Module MUST satisfy the Single Responsibility Principle defined by Canon_v2.0.

A module SHALL have one reason to change.

The following examples violate this rule.

- Validation + File I/O
- JSON + Logging
- Domain Logic + UI
- Configuration + Network Access

Such responsibilities SHALL be separated into different modules.

---

## 3.3 Stateless Design

Standard Modules SHALL be stateless.

The following SHALL NOT exist.

- Public mutable variables
- Global state
- Hidden cache
- Shared mutable collections
- Singleton-like module variables

Read-only constants MAY exist.

Compile-time constants MAY exist.

---

## 3.4 Lifetime Management

A Standard Module SHALL NOT manage object lifetime.

The following SHALL be performed by Composition Root or Factory Classes.

- Object creation
- Dependency injection
- Lifetime management
- Instance ownership

---

## 3.5 Business Logic

Business logic SHALL belong to Domain Classes.

A Standard Module SHALL NOT become a replacement for object-oriented design.

Large procedural algorithms SHALL be decomposed into domain services or dedicated classes when appropriate.

---

# 4. Module Categories

Every Standard Module SHALL belong to exactly one category.

## 4.1 Public API Module

Responsibilities:

- expose public procedures
- expose public functions
- provide stable API surface

These modules SHALL be the only publicly accessible entry points unless explicitly documented otherwise.

---

## 4.2 Facade Module

Facade modules coordinate lower-level components.

They SHALL NOT implement business rules directly.

They SHALL delegate work to lower layers.

---

## 4.3 Factory Module

Factory modules create objects.

They SHALL NOT retain ownership of created objects.

They SHALL return fully initialized instances.

---

## 4.4 Utility Module

Utility modules provide reusable pure functions.

Typical examples include:

- string helpers
- path helpers
- collection helpers
- date helpers

Utility modules SHOULD remain side-effect free whenever practical.

---

## 4.5 Validation Module

Validation modules verify input values.

They SHALL NOT modify input.

They SHALL either:

- return validation results, or
- raise standardized errors defined by ErrorHandling_v2.0.

---

## 4.6 Conversion Module

Conversion modules convert between representations.

Examples include:

- DTO ↔ Domain
- Enum ↔ String
- String ↔ Date
- String ↔ Number

Conversion modules SHALL NOT perform unrelated business processing.
# 5. Module Structure

## 5.1 Standard Layout

Every Standard Module SHOULD follow the structure below.

```text
Option Explicit

'=========================================================================
' Constants
'=========================================================================

'=========================================================================
' Private Types
'=========================================================================

'=========================================================================
' Module Variables (only when explicitly permitted)
'=========================================================================

'=========================================================================
' Public API
'=========================================================================

'=========================================================================
' Internal Procedures
'=========================================================================

'=========================================================================
' Private Helper Functions
'=========================================================================
```

The ordering SHOULD remain consistent throughout the project.

---

## 5.2 Option Explicit

Every Standard Module MUST begin with:

```vb
Option Explicit
```

Implicit variable declaration SHALL NOT be permitted.

---

## 5.3 Module Variables

Module-level variables SHOULD NOT be used.

If unavoidable, they SHALL satisfy all of the following:

- Private visibility
- Clearly documented purpose
- Immutable whenever possible
- No hidden application state
- No dependency management

Public module variables MUST NOT exist.

---

## 5.4 Constants

Constants SHOULD be declared near the beginning of the module.

Constants SHALL use the naming convention defined by NamingConvention_v2.0.

Constants SHALL NOT duplicate values already defined elsewhere.

Shared constants SHOULD be centralized.

---

## 5.5 Public Procedures

Public procedures SHALL appear before private procedures.

Public procedures SHALL be documented.

Public procedures SHALL remain minimal.

Complex processing SHALL be delegated to internal procedures or lower layers.

---

## 5.6 Private Procedures

Private procedures SHALL implement internal details only.

They SHALL NOT be called from outside the module.

Private procedures MAY assume valid internal state established by the public entry point.

---

## 5.7 Function Size

Functions SHOULD remain small.

As guidance:

- approximately 30 lines is preferred
- over 50 lines SHOULD trigger review
- over 100 lines SHOULD normally be decomposed

These thresholds are recommendations rather than strict limits.

---

# 6. Visibility Rules

## 6.1 Public Members

Only members intended for external use SHALL be declared Public.

Every Public member SHALL form part of the documented API.

Undocumented Public procedures SHOULD NOT exist.

---

## 6.2 Private Members

Implementation details SHALL remain Private.

Private procedures SHALL NOT be exposed for convenience.

---

## 6.3 Friend Visibility

Friend visibility SHALL NOT be used.

Inter-module collaboration SHALL occur through documented Public APIs only.

---

## 6.4 Module Accessibility

Modules SHALL communicate through published interfaces.

Direct access to another module's internal implementation SHALL NOT occur.

Dependencies SHALL follow DependencySpecification_v2.0.
# 7. Implementation Rules

## 7.1 Error Handling

Every Standard Module SHALL comply with ErrorHandling_v2.0.

Errors SHALL be:

- predictable
- deterministic
- reproducible
- meaningful

Unexpected VBA runtime errors SHALL NOT be exposed directly to upper layers.

---

## 7.2 Dependency Direction

A Standard Module SHALL only depend upon permitted lower layers.

Circular dependencies SHALL NOT exist.

Dependency direction SHALL conform to:

- LayerSpecification_v2.0
- DependencySpecification_v2.0

---

## 7.3 Configuration Access

Configuration values SHALL be obtained through the Configuration subsystem.

Hard-coded configuration values SHALL NOT exist except for compile-time constants explicitly documented.

---

## 7.4 Object Creation

Object creation SHOULD be centralized.

Repeated `New` expressions scattered throughout procedures SHOULD be avoided.

Factories or Composition Root SHOULD own complex object construction.

---

## 7.5 Side Effects

Functions SHOULD minimize side effects.

Pure functions are preferred whenever practical.

If a procedure modifies:

- files
- worksheets
- workbook state
- external resources
- application settings

the side effects SHALL be explicit and documented.

---

## 7.6 Worksheet Access

Standard Modules SHALL NOT manipulate worksheets directly unless the module belongs to the Presentation layer.

Business logic SHALL remain independent of Excel UI objects.

---

## 7.7 Application Object

Direct use of global Excel objects such as:

- `Application`
- `ActiveWorkbook`
- `ActiveSheet`
- `Selection`

SHOULD be avoided.

Required objects SHOULD be supplied explicitly as parameters whenever practical.

---

# 8. Documentation Requirements

## 8.1 Header Comment

Each Standard Module SHOULD begin with a module header describing:

- purpose
- responsibility
- layer
- dependencies

---

## 8.2 Public Procedure Documentation

Every Public procedure SHOULD include documentation describing:

- purpose
- parameters
- return value
- raised errors
- remarks

---

## 8.3 Internal Documentation

Private procedures SHOULD be self-explanatory.

Comments SHOULD explain *why* rather than *what*.

Obvious implementation details SHOULD NOT be commented redundantly.

---

## 8.4 Examples

Examples MAY be included when they improve understanding.

Examples SHALL be informative only.

Examples SHALL NOT be treated as normative requirements.
# 9. Prohibited Practices

The following practices are prohibited unless explicitly permitted by a higher-level specification.

## 9.1 Global Mutable State

The following SHALL NOT exist.

- Public variables
- Shared mutable state
- Hidden caches
- Module-wide application state

---

## 9.2 Circular Dependencies

A Standard Module SHALL NOT introduce circular dependencies.

Dependency graphs SHALL remain acyclic.

---

## 9.3 Mixed Responsibilities

A module SHALL NOT combine unrelated responsibilities.

Examples include:

- UI + Domain Logic
- File I/O + Validation
- Configuration + Business Rules
- Logging + Object Construction

---

## 9.4 Hidden Side Effects

Procedures SHALL NOT unexpectedly modify:

- workbook state
- worksheet contents
- global settings
- configuration
- application environment

Such behavior SHALL always be explicit.

---

## 9.5 Direct Layer Violation

A Standard Module SHALL NOT bypass the architecture by directly invoking components in non-permitted layers.

All interactions SHALL comply with LayerSpecification_v2.0 and DependencySpecification_v2.0.

---

## 9.6 Duplicate Logic

Common processing SHALL NOT be copied across multiple modules.

Shared behavior SHOULD be extracted into a dedicated Utility, Factory, or Service module, as appropriate.

---

## 9.7 God Modules

A Standard Module SHALL NOT become excessively large or accumulate unrelated functionality.

If a module grows beyond its single responsibility, it SHALL be divided into multiple modules with clearly separated concerns.

---

# 10. Compliance

A Standard Module is considered compliant with this specification only if all of the following conditions are satisfied.

- It has exactly one primary responsibility.
- It complies with the dependency rules defined by DependencySpecification_v2.0.
- It resides within the appropriate architectural layer defined by LayerSpecification_v2.0.
- It exposes only documented Public members.
- It maintains no mutable global state.
- It follows the naming rules defined by NamingConvention_v2.0.
- It implements standardized error handling in accordance with ErrorHandling_v2.0.
- It accesses configuration exclusively through Configuration_v2.0.
- It conforms to the architectural principles defined by Canon_v2.0.
- It satisfies the document requirements defined by DocumentTemplate_v2.0.

Failure to satisfy any mandatory requirement defined by this specification SHALL constitute non-conformance.

---

# 11. References

## Normative References

- Canon_v2.0.md
- Architecture_v2.0.md
- SpecificationHierarchy_v2.0.md
- DocumentTemplate_v2.0.md
- Manifest_v2.0.md
- ErrorHandling_v2.0.md
- Configuration_v2.0.md
- ClassSpecification_v2.0.md
- InterfaceSpecification_v2.0.md
- ApiSpecification_v2.0.md
- DependencySpecification_v2.0.md
- LayerSpecification_v2.0.md
- NamingConvention_v2.0.md

---

## Informative References

- RFC 2119 — Key words for use in RFCs to Indicate Requirement Levels
- Microsoft VBA Language Reference
- SOLID Principles
- Clean Architecture
- Clean Code
