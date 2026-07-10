# ModuleTemplate_v2.0

| Item | Value |
|------|-------|
| Document | ModuleTemplate_v2.0 |
| Version | 2.0 |
| Status | Frozen |
| Classification | Official Specification |
| Author | Build.xlam Project |
| Language | English |

---

# 1. Purpose

## 1.1 Objective

This specification defines the **single official template** for every Standard Module used in the Build.xlam Project.

The template establishes a common module structure that guarantees consistency across the entire code base.

All newly created Standard Modules shall be created by copying this template.

No alternative layouts are permitted.

---

## 1.2 Goals

This template has the following objectives.

- Standardize module structure
- Improve readability
- Improve maintainability
- Improve review efficiency
- Minimize implementation differences
- Reduce onboarding cost
- Enforce architectural consistency
- Support automated validation

---

## 1.3 Design Principles

This template follows the principles defined by Canon_v2.0.

- Single Responsibility Principle
- One-way Dependency Principle
- Contract-based Collaboration
- Changeability First
- Configuration Separation
- Development/Runtime Separation

---

# 2. Scope

This specification applies to every Standard Module contained in Build.xlam.

Examples include:

- Common Modules
- Infrastructure Modules
- Domain Modules
- Application Modules
- Utility Modules
- API Modules

This specification does **not** apply to:

- Class Modules
- UserForms
- Workbook Modules
- Worksheet Modules
- Ribbon Callback Modules
- Test Modules

Each of the above shall use its own dedicated template specification.

---

# 3. References

This specification shall conform to the following frozen specifications.

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
- DependencySpecification_v2.0.md
- ApiSpecification_v2.0.md
- EventSpecification_v2.0.md
- LayerSpecification_v2.0.md
- NamingConvention_v2.0.md

Whenever this specification conflicts with another specification, the precedence defined by SpecificationHierarchy_v2.0 shall apply.

---

# 4. Template Philosophy

## 4.1 Single Official Template

Exactly one template shall exist for Standard Modules.

Developers shall not create project-specific variations.

---

## 4.2 Copy-and-Implement

A new Standard Module shall be created by copying this template without modifying its structure.

Only implementation-specific content may be edited.

---

## 4.3 Structural Consistency

Every Standard Module shall use an identical section order.

This enables developers to understand an unfamiliar module immediately.

---

## 4.4 Readability First

The template is designed primarily for human readability rather than minimizing file size.

Consistent formatting shall always take precedence.

---

## 4.5 Self-Documenting Structure

The module layout itself shall communicate:

- responsibility
- dependencies
- public interface
- internal implementation
- error handling strategy

without requiring external explanation.

---

## 4.6 Maintainability

Every module shall be maintainable independently.

Implementation details shall remain localized.

Changes shall not propagate unnecessarily to unrelated modules.

---

## 4.7 Architecture Compliance

Every module created from this template shall satisfy:

- LayerSpecification_v2.0
- DependencySpecification_v2.0
- ModuleSpecification_v2.0

without requiring additional structural modifications.

---

## 4.8 Review Optimization

The template shall minimize review effort.

Reviewers shall always know where to find:

- constants
- public APIs
- private procedures
- helper functions
- initialization code
- cleanup code
- error handling

because every module uses the identical layout.

---

## 4.9 Long-Term Stability

This template is part of the frozen Build.xlam architecture.

Breaking changes shall only be introduced in a future major version.

Minor revisions shall remain backward compatible.

---
# 5. Standard Module Layout

## 5.1 Section Order

Every Standard Module shall follow the exact section order shown below.

```text
Module Header

Option Statements

Constants

Module Variables

Public API

Internal Procedures

Private Helper Functions

Initialization

Cleanup

End of Module
```

The order shall not be changed.

---

## 5.2 Required Sections

The following sections shall always exist.

```vb
'===============================================================================
' Constants
'===============================================================================

'===============================================================================
' Module Variables
'===============================================================================

'===============================================================================
' Public API
'===============================================================================

'===============================================================================
' Internal Procedures
'===============================================================================
```

Even if a section is temporarily empty, the section header shall remain until implementation is complete.

---

## 5.3 Optional Sections

The following sections may be omitted when unnecessary.

```vb
Initialization

Cleanup

Private Helper Functions
```

Unused optional sections shall be removed completely.

Empty procedures shall not remain in production code.

---

# 6. File Header Template

Every Standard Module shall begin with the following header.

```vb
'===============================================================================
' Module
'===============================================================================
'
' Name        :
'
' Layer       :
'
' Purpose     :
'
' Dependencies:
'
' Author      :
'
' Created     :
'
' Updated     :
'
'===============================================================================
```

---

## 6.1 Name

The module name shall comply with NamingConvention_v2.0.

Example:

```text
Com_FileSystem

App_Settings

Dom_Schedule

Inf_FileLogger
```

---

## 6.2 Layer

The declared layer shall match LayerSpecification_v2.0.

Example:

```text
Common

Infrastructure

Application

Domain
```

---

## 6.3 Purpose

The purpose shall consist of a single concise sentence describing the module responsibility.

Example:

```text
Provides file system utility functions.
```

---

## 6.4 Dependencies

Only direct dependencies shall be listed.

Transitive dependencies shall not be included.

Example:

```text
Com_Path
Com_File
Inf_Logger
```

---

# 7. Option Statements

Option statements shall immediately follow the file header.

The required order is:

```vb
Option Explicit

Option Private Module
```

No additional compiler directives shall appear before these statements unless explicitly defined by project-wide policy.

---

## 7.1 Option Explicit

Every module shall declare:

```vb
Option Explicit
```

This requirement is mandatory.

---

## 7.2 Option Private Module

Modules that are not intended to expose public procedures outside the VBA project shall declare:

```vb
Option Private Module
```

If a module intentionally publishes procedures to Excel or external callers, this statement may be omitted in accordance with ApiSpecification_v2.0.

---

# 8. Constants

All module-level constants shall be declared immediately after the option statements.

Example:

```vb
Private Const MODULE_NAME As String = "Com_FileSystem"

Private Const DEFAULT_TIMEOUT As Long = 30
```

---

## 8.1 Constant Rules

Constants shall:

- be immutable
- be declared at module scope
- use explicit data types
- follow NamingConvention_v2.0

Magic numbers and literal strings shall not appear directly in implementation code when a named constant is appropriate.

---
# 9. Module Variables

## 9.1 Declaration Area

All module-level variables shall be declared immediately after the Constants section.

Example:

```vb
Private mInitialized As Boolean

Private mLogger As Inf_Logger

Private mConfiguration As App_Configuration
```

---

## 9.2 Variable Rules

Module variables shall:

- be declared only when shared state is required
- use the narrowest possible scope
- have explicit data types
- follow NamingConvention_v2.0

Unnecessary global state shall not be introduced.

---

## 9.3 Visibility

Module-level variables shall normally be declared as:

```vb
Private
```

Public module variables are prohibited unless explicitly required by the architecture.

---

## 9.4 Initialization

Every module variable shall have a clearly defined initialization strategy.

Initialization shall occur through:

- explicit initialization procedures, or
- lazy initialization

Implicit initialization dependencies are prohibited.

---

# 10. Public API

## 10.1 Purpose

The Public API section contains every procedure intentionally exposed by the module.

Only documented entry points shall be declared here.

---

## 10.2 Section Layout

```vb
'===============================================================================
' Public API
'===============================================================================
```

All Public procedures shall be grouped together.

Public procedures shall never appear after Private procedures.

---

## 10.3 Documentation

Every Public procedure shall include a documentation block.

Example:

```vb
'-------------------------------------------------------------------------------
' Purpose
'   Returns the configuration file path.
'
' Parameters
'   None.
'
' Returns
'   String
'
' Errors
'   ComErrInvalidPath
'-------------------------------------------------------------------------------
```

---

## 10.4 Procedure Order

Public procedures should be ordered by functional importance.

Recommended order:

1. Initialize
2. Execute
3. Query
4. Update
5. Dispose

Related procedures shall remain adjacent.

---

## 10.5 Public Procedure Template

```vb
Public Function GetValue( _
    ByVal Key As String _
) As String

    On Error GoTo ErrorHandler

    ' Implementation

    Exit Function

ErrorHandler:

    Err.Raise Err.Number

End Function
```

The actual error handling implementation shall comply with ErrorHandling_v2.0.

---

# 11. Internal Procedures

## 11.1 Purpose

Internal Procedures contain implementation details that are not part of the module contract.

These procedures shall always be declared as:

```vb
Private
```

---

## 11.2 Section Layout

```vb
'===============================================================================
' Internal Procedures
'===============================================================================
```

---

## 11.3 Visibility

Private procedures shall never be called from outside the module.

No external module shall depend upon them.

---

## 11.4 Organization

Private procedures should be grouped by responsibility.

Example:

```text
Validation

Conversion

Formatting

File Operations

Calculation

Utility
```

---

## 11.5 Private Function Template

```vb
Private Function NormalizePath( _
    ByVal Path As String _
) As String

    ' Implementation

End Function
```

Private procedures should remain concise and focused on a single responsibility.

---

# 12. Private Helper Functions

## 12.1 Purpose

Helper Functions provide reusable internal logic.

They shall not contain business logic that belongs in Public APIs.

---

## 12.2 Characteristics

Helper functions shall:

- be deterministic whenever possible
- avoid side effects
- remain independent
- be easily testable

---

## 12.3 Recommended Examples

Typical helper functions include:

- string normalization
- path combination
- argument validation
- collection traversal
- formatting
- conversion

Business workflows shall not be implemented as helper functions.

---
# 13. Initialization

## 13.1 Purpose

The Initialization section defines how module-level resources are prepared before use.

Modules that maintain state shall provide an explicit initialization procedure.

Stateless modules should omit this section.

---

## 13.2 Initialization Procedure

The recommended initialization procedure is:

```vb
Public Sub Initialize()

    On Error GoTo ErrorHandler

    If mInitialized Then Exit Sub

    ' Initialize module resources

    mInitialized = True

    Exit Sub

ErrorHandler:

    Err.Raise Err.Number

End Sub
```

Initialization shall be idempotent.

Calling the procedure multiple times shall not produce different results.

---

## 13.3 Lazy Initialization

Lazy initialization may be used when initialization cost is high.

Example:

```vb
If Not mInitialized Then
    Initialize
End If
```

The initialization state shall always be deterministic.

---

# 14. Cleanup

## 14.1 Purpose

The Cleanup section releases resources owned by the module.

Modules that do not own resources may omit this section.

---

## 14.2 Cleanup Procedure

Example:

```vb
Public Sub Dispose()

    On Error Resume Next

    Set mLogger = Nothing
    Set mConfiguration = Nothing

    mInitialized = False

End Sub
```

---

## 14.3 Cleanup Rules

Cleanup procedures shall:

- release owned objects
- clear module state
- avoid raising unexpected errors
- be safe to execute multiple times

---

# 15. Error Handling Template

## 15.1 General Rule

Every Public procedure shall implement structured error handling.

The implementation shall comply with ErrorHandling_v2.0.

---

## 15.2 Standard Layout

```vb
Public Sub Execute()

    On Error GoTo ErrorHandler

    ' Processing

    Exit Sub

ErrorHandler:

    Err.Raise Err.Number

End Sub
```

---

## 15.3 Local Validation

Input validation shall occur before business logic.

Example:

```vb
If Len(Path) = 0 Then

    Err.Raise ComErrInvalidArgument

End If
```

---

## 15.4 Error Propagation

Private procedures shall normally propagate errors to their caller.

They should not suppress unexpected errors.

---

# 16. Documentation Rules

## 16.1 Public Procedures

Every Public procedure shall include documentation describing:

- Purpose
- Parameters
- Returns
- Errors
- Remarks

---

## 16.2 Private Procedures

Private procedures may omit documentation when their purpose is immediately obvious.

Complex implementations should include explanatory comments.

---

## 16.3 Inline Comments

Inline comments shall explain:

- why an implementation exists
- assumptions
- architectural constraints

Comments shall not restate obvious code behavior.

---

## 16.4 Comment Style

Use complete English sentences.

Examples:

```vb
' Validate input parameters.

' Create a temporary working directory.

' Load the configuration file.
```

Avoid redundant comments such as:

```vb
' Increment i

i = i + 1
```

---

# 17. Naming Rules

Every identifier shall comply with NamingConvention_v2.0.

This includes:

- module names
- procedures
- variables
- constants
- parameters
- enumerations
- user-defined types

No local naming conventions are permitted.

---
# 18. Dependency Rules

## 18.1 General Rule

Every Standard Module shall comply with DependencySpecification_v2.0.

Dependencies shall remain explicit, minimal, and unidirectional.

---

## 18.2 Allowed Dependencies

A module may depend only on components permitted by LayerSpecification_v2.0.

Dependencies outside the permitted architectural boundaries are prohibited.

---

## 18.3 Circular Dependencies

Circular dependencies shall never exist.

The dependency graph shall form a directed acyclic graph (DAG).

---

## 18.4 Hidden Dependencies

A module shall not rely on:

- implicit global state
- undocumented initialization order
- side effects from unrelated modules

All dependencies shall be visible from the module interface or documented in the module header.

---

## 18.5 External Components

Direct interaction with external resources (such as the file system, network, registry, or Excel object model) shall be isolated within the appropriate infrastructure layer.

Domain and Application modules shall not access external resources directly.

---

# 19. Implementation Checklist

Before a Standard Module is committed, the following checklist shall be satisfied.

| Item | Required |
|------|----------|
| Module name follows NamingConvention_v2.0 | ✓ |
| File header completed | ✓ |
| Option Explicit declared | ✓ |
| Option Private Module evaluated | ✓ |
| Constants grouped correctly | ✓ |
| Module variables minimized | ✓ |
| Public API documented | ✓ |
| Private procedures grouped | ✓ |
| Error handling implemented | ✓ |
| No circular dependencies | ✓ |
| Layer dependency verified | ✓ |
| No unused variables | ✓ |
| No unused procedures | ✓ |
| No magic numbers | ✓ |
| No TODO comments | ✓ |
| Code formatted consistently | ✓ |

All checklist items shall pass before the module is considered complete.

---

# 20. Complete Module Skeleton

The following skeleton is the official template for every new Standard Module.

```vb
'===============================================================================
' Module
'===============================================================================
'
' Name        :
' Layer       :
' Purpose     :
' Dependencies:
' Author      :
' Created     :
' Updated     :
'
'===============================================================================

Option Explicit
Option Private Module

'===============================================================================
' Constants
'===============================================================================

Private Const MODULE_NAME As String = ""

'===============================================================================
' Module Variables
'===============================================================================

Private mInitialized As Boolean

'===============================================================================
' Public API
'===============================================================================

Public Sub Initialize()

    On Error GoTo ErrorHandler

    If mInitialized Then Exit Sub

    mInitialized = True

    Exit Sub

ErrorHandler:

    Err.Raise Err.Number

End Sub

'===============================================================================
' Internal Procedures
'===============================================================================

Private Sub Validate()

End Sub

Private Function NormalizeValue( _
    ByVal Value As String _
) As String

End Function
```

This skeleton shall be copied when creating a new Standard Module.

Only implementation-specific content shall be modified.

---

# 21. Anti-Patterns

The following practices are prohibited.

- Mixing Public and Private procedures.
- Declaring Public module variables.
- Using undocumented dependencies.
- Violating layer boundaries.
- Introducing circular dependencies.
- Embedding business logic in helper functions.
- Leaving empty placeholder procedures in production code.
- Using inconsistent section ordering.
- Omitting error handling from Public procedures.
- Using names that violate NamingConvention_v2.0.

---

# 22. Migration Notes

Projects migrating from earlier versions shall update Standard Modules to conform to this specification.

Recommended migration order:

1. Apply the official module layout.
2. Reorganize declarations.
3. Separate Public and Private procedures.
4. Update documentation.
5. Apply NamingConvention_v2.0.
6. Verify DependencySpecification_v2.0 compliance.
7. Verify LayerSpecification_v2.0 compliance.
8. Validate against the implementation checklist.

Backward compatibility of public interfaces should be preserved whenever practical.

---

# 23. References

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
- DependencySpecification_v2.0.md
- ApiSpecification_v2.0.md
- EventSpecification_v2.0.md
- LayerSpecification_v2.0.md
- NamingConvention_v2.0.md

---

**End of Document**
