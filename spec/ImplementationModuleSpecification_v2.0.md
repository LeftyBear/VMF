# ModuleSpecification_v2.0

**Build.xlam Project**

* **Version:** 2.0
* **Status:** Frozen
* **Encoding:** UTF-8
* **Format:** Markdown

---

# 1. Purpose

This specification defines the design rules for **Standard Modules (.bas)** used in the Build.xlam Project.

Its objectives are to:

* Standardize module responsibilities
* Ensure consistent public APIs
* Improve implementation quality
* Enhance maintainability, readability, and extensibility
* Enforce architectural dependency rules

This specification shall conform to **Canon_v2.0.md**.

---

# 2. Scope

This specification applies to:

* Standard Modules (.bas)

This specification does **not** apply to:

* Class Modules
* Interface Modules
* UserForms
* Workbook Modules
* Worksheet Modules

These components are governed by their respective specifications.

---

# 3. Module Definition

A Standard Module is a static code container that may contain:

* Procedures
* Public APIs
* Helper functions
* Constants
* Enumerations
* User-defined Types

A Standard Module **MUST NOT** maintain runtime state.

---

# 4. Design Principles

## 4.1 Single Responsibility

A module **MUST** have exactly one responsibility.

Typical responsibilities include:

* Path operations
* File operations
* Logging
* Validation
* Factory functions
* Utility functions

Multiple unrelated responsibilities **MUST NOT** be combined.

---

## 4.2 Stateless Design

A Standard Module **MUST NOT** maintain runtime state.

The following are prohibited:

* Module-level variables
* Static variables
* Internal caches

If state management is required, it **MUST** be implemented using Classes.

---

## 4.3 Pure Function Preference

Functions without side effects **SHOULD** be implemented as pure functions whenever possible.

---

## 4.4 Layer Isolation

Modules **MUST** follow the dependency direction defined in LayerSpecification_v2.0.md.

Reverse dependencies are prohibited.

---

# 5. Responsibilities

A Standard Module may be responsible for:

* Utility functions
* Validation
* Factory methods
* Type conversion
* Formatting
* File I/O
* Path manipulation
* String processing
* Date and time processing
* Collection utilities
* Public API exposure
* Error creation

A Standard Module **MUST NOT** be responsible for:

* Runtime state management
* Business logic ownership
* Domain entities
* Repository implementation
* View control
* Persistent object management

---

# 6. Module Categories

Standard Modules are classified into the following categories.

## 6.1 API Module

Provides public Facade APIs.

Examples:

* Com_API
* App_API
* Dom_API

---

## 6.2 Utility Module

Provides reusable helper functions.

Examples:

* Com_String
* Com_Path
* Com_File

---

## 6.3 Validation Module

Provides validation logic.

Example:

* Com_Validate

---

## 6.4 Factory Module

Creates domain or application objects.

Example:

* Dom_Factory

---

## 6.5 Error Module

Creates and raises standardized errors.

Example:

* Com_Error

---

## 6.6 Constant Module

Defines shared constants.

Example:

* Com_Constant

---

## 6.7 Enumeration Module

Defines Enumerations.

Example:

* Com_Enum

---

# 7. Naming Rules

Module names **MUST** follow the format:

```
<Layer>_<Function>
```

Examples:

* Com_Path
* Com_File
* Dom_Factory
* App_API

The following names are prohibited:

* Module1
* Utility
* Common
* Test

---

# 8. Public API Rules

Only the minimum required procedures shall be exposed publicly.

External APIs **MUST** be declared as:

```
Public
```

Internal procedures **MUST** be declared as:

```
Private
```

`Friend` procedures are prohibited.

---

# 9. Visibility Rules

| Visibility | Allowed |
| ---------- | ------- |
| Public     | Yes     |
| Private    | Yes     |
| Friend     | No      |

---

# 10. Procedure Rules

Each procedure **MUST** have a single responsibility.

Procedures should be:

* Short
* Readable
* Minimized in side effects

Recommended size:

* Approximately 20–50 lines

Procedures exceeding 100 lines **SHOULD** be refactored.

---

# 11. Parameter Rules

Procedures **SHOULD** accept only the minimum required parameters.

Recommended maximum:

* 3–5 parameters

If additional information is required, it **SHOULD** be grouped into a Class or Type.

---

# 12. Return Value Rules

Return values **SHOULD** use meaningful data types.

The following should be avoided:

* Variant

Preferred types include:

* Boolean
* Long
* String
* Object
* Class
* Enumeration

---

# 13. Error Handling

Modules **MUST** comply with ErrorHandling_v2.0.md.

A module may:

* Raise errors
* Create Result objects

Error numbers **MUST** follow the standardized numbering rules.

---

# 14. Dependency Rules

Dependencies **MUST** follow a one-way direction:

```
Upper Layer
      ↓
Lower Layer
```

The following are prohibited:

* Circular dependencies
* Bidirectional dependencies
* Hidden dependencies

---

# 15. Constant Rules

Constants **MUST** be declared as:

* Public Const
* Private Const

Magic numbers are prohibited.

---

# 16. Enumeration Rules

Enumerations **SHOULD** represent a single conceptual domain.

Examples:

* ComColor
* ComResultType
* DomStatus

Unrelated values **MUST NOT** be combined.

---

# 17. Type Rules

User-defined Types are intended solely for data structures.

Behavior and business logic **MUST** be implemented using Classes.

---

# 18. Comments

Comments **SHOULD** explain *why*, not *what*.

Avoid comments that merely describe obvious code.

---

# 19. Option Statements

Every Standard Module **MUST** include:

```vb
Option Explicit
```

`Option Private Module` **MAY** be used when appropriate.

---

# 20. Module Layout

The recommended module structure is:

1. Option Statements
2. Constants
3. Enumerations
4. Types
5. Public APIs
6. Private Procedures
7. Helper Procedures

Modules **SHOULD** follow this order consistently.

---

# 21. Initialization

Standard Modules **MUST NOT** perform automatic initialization.

Implicit initialization such as `Auto_Open` is prohibited.

Initialization, when required, **MUST** be performed explicitly by the Composition Root or a Facade.

---

# 22. Testability

Modules **SHOULD** be independently testable.

They should:

* Minimize external dependencies
* Clearly define inputs and outputs
* Localize side effects
* Produce deterministic results

---

# 23. Performance

Optimization **MUST NOT** compromise maintainability.

The following priorities shall apply:

1. Correctness
2. Maintainability
3. Readability
4. Performance

Architectural principles shall never be sacrificed solely for performance.

---

# 24. Prohibited Practices

The following practices are prohibited:

* Module-level state
* Excessive use of Variant
* Circular dependencies
* Hidden dependencies
* Excessively long procedures
* Mixed responsibilities
* Magic numbers
* Public helper procedures
* Duplicate implementations
* Layer violations
* Implicit initialization
* Unused public procedures

---

# 25. Compliance

Every Standard Module **MUST** comply with:

* Canon_v2.0.md
* Architecture_v2.0.md
* DependencySpecification_v2.0.md
* LayerSpecification_v2.0.md
* NamingConvention_v2.0.md
* ApiSpecification_v2.0.md
* ErrorHandling_v2.0.md
* Configuration_v2.0.md

Modules that do not conform to this specification **MUST NOT** be incorporated into the Build.xlam Project.

---

# 26. Revision Policy

Changes to this specification shall follow the change management process defined in Canon_v2.0.md.

Version 2.0 is designated as **Frozen**. Breaking changes are permitted only in a future major version.
