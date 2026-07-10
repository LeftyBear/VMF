# NamingConvention_v2.0

## 1. Purpose

This specification defines the normative naming conventions for all artifacts within the Build.xlam Project.

The objectives of this specification are:

- Ensure consistency.
- Improve readability.
- Support maintainability.
- Enable automated analysis.
- Provide clear layer identification.
- Standardize public APIs.

This specification applies to all source code and specification artifacts unless explicitly stated otherwise.

---

## 2. Scope

This specification defines naming conventions for:

- Modules
- Classes
- Interfaces
- Facades
- Composition Roots
- Public APIs
- Procedures
- Variables
- Constants
- Enumerations
- User Defined Types
- Files
- Folders
- Configuration Keys
- Manifest Identifiers
- Error Identifiers

UI captions, worksheet names, and end-user visible text are outside the scope of this specification.

---

## 3. References

This specification SHALL conform to the following normative documents:

- Canon_v2.0
- Architecture_v2.0
- ...

---

## 4. Naming Principles

### 4.1 General
All identifiers SHALL follow a consistent naming convention throughout the Build.xlam Project.

Naming SHALL emphasize:

- Readability
- Consistency
- Discoverability
- Maintainability
- Layer identification
- Contract clarity

Identifiers SHALL clearly express their responsibility without requiring implementation knowledge.

---

### 4.2 Consistency
The same concept SHALL always use the same identifier.
Different identifiers SHALL NOT be used for the same semantic meaning.
Likewise, the same identifier SHALL NOT represent different concepts.

---

### 4.3 Descriptiveness
Identifiers SHALL describe intent rather than implementation.
Names SHALL represent domain concepts whenever possible.
Implementation details SHALL NOT appear in public identifiers unless they are part of the public contract.

---

### 4.4 Stability
Public identifiers SHALL remain stable across compatible releases.
Renaming public APIs SHALL be treated as a breaking change unless compatibility is explicitly preserved.
Internal identifiers MAY be renamed when doing so improves maintainability.

---

### 4.5 Abbreviations
Abbreviations SHALL be minimized.
Only abbreviations defined by this specification or established by the project glossary MAY be used.
Project-wide abbreviations SHALL remain consistent throughout all layers.

---

### 4.6 Language
All identifiers SHALL use English.
User-facing text MAY use other languages.
Comments SHOULD follow project documentation policy.

---

# 5. General Rules

## 5.1 Character Set

Identifiers SHALL consist only of:

- Uppercase letters (A–Z)
- Lowercase letters (a–z)
- Digits (0–9)
- Underscore (_)

Whitespace SHALL NOT be used.

Hyphens SHALL NOT be used.

Special characters SHALL NOT be used.

---

## 5.2 Case Style

The following casing rules SHALL apply.

| Artifact | Style |
|----------|-------|
| Module | PascalCase |
| Class | PascalCase |
| Interface | PascalCase |
| Procedure | PascalCase |
| Function | PascalCase |
| Property | PascalCase |
| Variable | PascalCase |
| Constant | PascalCase |
| Enum | PascalCase |
| Enum Member | PascalCase |
| User Defined Type | PascalCase |
| Manifest Identifier | PascalCase |
| Configuration Key | PascalCase |

---

## 5.3 Prefixes

Prefixes SHALL indicate architectural responsibility only.

Hungarian notation SHALL NOT be used.

Type prefixes based solely on VBA primitive types SHALL NOT be used.

Layer prefixes SHALL conform to LayerSpecification_v2.0.

---

## 5.4 Suffixes

Suffixes SHALL be used only when they communicate architectural meaning.

Examples include:

- Facade
- Factory
- Builder
- Provider
- Repository
- Service
- Configuration
- CompositionRoot

Decorative or redundant suffixes SHALL NOT be used.

---

## 5.5 Reserved VBA Keywords

Identifiers SHALL NOT match VBA reserved keywords.

If a domain term conflicts with a reserved keyword, an alternative descriptive identifier SHALL be selected.

---

## 5.6 Identifier Length

Identifiers SHOULD be concise.

Identifiers SHALL remain sufficiently descriptive to avoid ambiguity.

Excessively abbreviated names SHALL NOT be used.
## 3. References

This specification SHALL conform to the following normative documents.

- Canon_v2.0
- Architecture_v2.0
- SpecificationHierarchy_v2.0
- Glossary_v2.0
- DocumentTemplate_v2.0
- Manifest_v2.0
- ErrorHandling_v2.0
- Configuration_v2.0
- ModuleSpecification_v2.0
- ClassSpecification_v2.0
- InterfaceSpecification_v2.0
- ApiSpecification_v2.0
- FacadeSpecification_v2.0
- CompositionRootSpecification_v2.0
- DependencySpecification_v2.0
- LayerSpecification_v2.0

If any conflict exists between this specification and a higher-level specification, the higher-level specification SHALL take precedence.
# 6. Layer Prefix Rules

## 6.1 General

Layer prefixes SHALL identify the architectural layer to which an artifact belongs.

Layer prefixes SHALL be applied consistently across all layers.

Layer prefixes SHALL conform to LayerSpecification_v2.0.

---

## 6.2 Standard Layer Prefixes

The following prefixes are reserved.

| Layer | Prefix | Example |
|--------|--------|---------|
| Common | Com | ComLogger |
| Domain | Dom | DomStudent |
| Application | App | AppImportService |
| Infrastructure | Inf | InfFileSystem |
| Presentation | Pre | PreMainForm |

Additional layer prefixes SHALL NOT be introduced unless approved by the project architecture.

---

## 6.3 Public API Prefixes

Public APIs SHALL retain the prefix of the layer that owns the contract.

Examples:

```text
ComInitialize
DomCreateStudent
AppExecuteImport
InfOpenWorkbook
PreShowDialog
```

Public APIs SHALL NOT expose implementation-specific prefixes.

---

## 6.4 Cross-Layer Consistency

An artifact SHALL NOT use a prefix belonging to another layer.

Incorrect examples:

```text
DomLogger
AppStudent
InfCalculationService
```

Correct examples:

```text
ComLogger
DomStudent
AppCalculationService
```

---

# 7. Module Naming

## 7.1 General

Standard Modules SHALL use PascalCase.

Module names SHALL be unique within the project.

---

## 7.2 Layer Prefix

Module names SHALL begin with the owning layer prefix.

Examples:

```text
ComUtility
ComError
DomCalculation
AppImport
InfWorkbook
PreRibbon
```

---

## 7.3 Responsibility

A module name SHALL clearly describe its primary responsibility.

Examples:

```text
ComString
ComFile
ComValidation
DomSchedule
AppExport
InfStorage
```

Names describing multiple unrelated responsibilities SHALL NOT be used.

---

## 7.4 Utility Modules

Utility modules SHALL describe the capability they provide.

Examples:

```text
ComCollection
ComDate
ComPath
ComEncoding
```

Generic names SHALL NOT be used.

Examples of prohibited names:

```text
Module1
Utilities
Helpers
Misc
General
```

---

## 7.5 Test Modules

If test modules exist, they SHOULD use a consistent suffix.

Example:

```text
DomStudentTests
ComPathTests
AppImportTests
```

Production modules SHALL NOT include the `Tests` suffix.
# 8. Class Naming

## 8.1 General

Class names SHALL use PascalCase.

Each class name SHALL uniquely identify a single responsibility as defined by ClassSpecification_v2.0.

Class names SHALL represent nouns or noun phrases.

---

## 8.2 Layer Prefix

Every class SHALL begin with the prefix of its owning layer.

Examples:

```text
ComResult
ComLogger
DomStudent
DomStudentId
AppImportService
InfWorkbookProvider
PreMainPresenter
```

The layer prefix SHALL accurately reflect the architectural ownership of the class.

---

## 8.3 Domain Classes

Domain classes SHALL represent domain concepts.

Examples:

```text
DomStudent
DomTeacher
DomClassHour
DomSubject
DomGrade
```

Domain classes SHALL NOT expose implementation concerns.

---

## 8.4 Service Classes

Service classes SHALL use the `Service` suffix only when they encapsulate service-oriented behavior.

Examples:

```text
AppImportService
AppExportService
InfStorageService
```

The `Service` suffix SHALL NOT be used merely to distinguish a class from another class.

---

## 8.5 Provider Classes

Classes responsible for supplying resources or external objects SHOULD use the `Provider` suffix.

Examples:

```text
InfWorkbookProvider
InfConfigurationProvider
InfPathProvider
```

---

## 8.6 Factory Classes

Classes responsible for object creation SHOULD use the `Factory` suffix.

Examples:

```text
DomStudentFactory
ComResultFactory
```

Factory classes SHALL encapsulate object creation logic.

---

## 8.7 Builder Classes

Classes responsible for incremental object construction SHOULD use the `Builder` suffix.

Examples:

```text
ManifestBuilder
ConfigurationBuilder
```

---

# 9. Interface Naming

## 9.1 General

Interface names SHALL use PascalCase.

Interfaces SHALL describe contracts rather than implementations.

---

## 9.2 Prefix

Interface names SHALL begin with the layer prefix.

Examples:

```text
ComLogger
AppImporter
InfStorage
```

Interface names SHALL NOT use the `I` prefix.

The following style SHALL NOT be used:

```text
ILogger
IRepository
IStorage
```

---

## 9.3 Responsibility

Interface names SHALL express capabilities.

Examples:

```text
ComSerializer
AppImporter
InfFileStorage
```

Implementation-specific terminology SHALL NOT appear in interface names.

---

## 9.4 Contract Stability

Published interface names SHALL remain stable across compatible releases.

Renaming an interface SHALL be treated as a breaking change unless compatibility is preserved.
# 10. Facade Naming

## 10.1 General

Facade names SHALL conform to FacadeSpecification_v2.0.

Each layer SHALL expose at most one public Facade unless explicitly permitted by its specification.

Facade names SHALL use PascalCase.

---

## 10.2 Naming Rules

Facade names SHALL begin with the owning layer prefix and end with the `Facade` suffix.

Examples:

```text
ComFacade
DomFacade
AppFacade
InfFacade
PreFacade
```

A Facade SHALL represent the public entry point of its layer.

---

## 10.3 Visibility

Only published Facades SHALL be referenced by higher layers.

Internal implementation classes SHALL NOT include the `Facade` suffix.

---

# 11. Composition Root Naming

## 11.1 General

Composition Root names SHALL conform to CompositionRootSpecification_v2.0.

Composition Root classes SHALL use PascalCase.

---

## 11.2 Naming Rules

Composition Root classes SHALL begin with the owning layer prefix and end with the `CompositionRoot` suffix.

Examples:

```text
ComCompositionRoot
AppCompositionRoot
```

Only classes responsible for dependency composition SHALL use this suffix.

---

## 11.3 Responsibility

A Composition Root SHALL be responsible only for object composition and dependency wiring.

Its name SHALL NOT imply business logic or infrastructure behavior.

---

# 12. Procedure Naming

## 12.1 General

Procedures SHALL use PascalCase.

Procedure names SHALL describe the performed action.

Procedure names SHALL begin with a verb.

---

## 12.2 Verb Usage

Recommended verbs include:

```text
Create
Load
Save
Read
Write
Find
Get
Set
Add
Remove
Update
Delete
Build
Initialize
Validate
Execute
Register
Resolve
Open
Close
Import
Export
```

The selected verb SHALL accurately describe the behavior.

---

## 12.3 Function Names

Function names SHALL describe the returned result.

Examples:

```text
GetConfiguration
FindStudent
CreateManifest
ResolveDependency
BuildContext
```

---

## 12.4 Property Procedures

Property procedures SHALL represent object attributes.

Examples:

```text
Name
Version
Path
Value
Configuration
```

Property names SHALL be nouns.

---

## 12.5 Event Handlers

Event handler names SHOULD follow the source event.

Examples:

```text
WorkbookOpen
WorkbookBeforeClose
RibbonLoaded
ButtonClicked
```

Event handlers SHALL clearly indicate the originating event.
# 13. Variable Naming

## 13.1 General

Variable names SHALL use PascalCase.

Variable names SHALL clearly express their purpose.

Single-character variable names SHALL NOT be used except as loop counters with a very limited scope.

---

## 13.2 Local Variables

Local variable names SHALL describe the value they hold.

Examples:

```text
Student
Configuration
Manifest
WorkbookPath
Result
ErrorNumber
```

Temporary variables SHOULD use meaningful names.

Names such as the following SHALL NOT be used unless their scope is trivial:

```text
Tmp
Temp
Value1
Data2
Object3
```

---

## 13.3 Parameters

Parameter names SHALL describe their semantic meaning.

Examples:

```text
StudentId
FilePath
Configuration
Manifest
ErrorNumber
```

Parameter names SHALL NOT encode their data type.

---

## 13.4 Boolean Variables

Boolean variables SHOULD indicate a true/false condition.

Recommended prefixes include:

```text
Is
Has
Can
Should
```

Examples:

```text
IsValid
HasChildren
CanExecute
ShouldUpdate
```

---

## 13.5 Collections

Collection variables SHOULD use plural nouns.

Examples:

```text
Students
Modules
Errors
Dependencies
Files
```

---

# 14. Constant Naming

## 14.1 General

Constant names SHALL use PascalCase.

Constant names SHALL describe immutable values.

---

## 14.2 Project Constants

Examples:

```text
DefaultTimeout
ManifestVersion
MaximumRetryCount
DefaultEncoding
```

Constant names SHALL NOT include implementation details.

---

## 14.3 Literal Replacement

Repeated literal values SHOULD be replaced by named constants whenever doing so improves readability or maintainability.

---

# 15. Enumeration Naming

## 15.1 General

Enumeration names SHALL use PascalCase.

Enumeration names SHALL represent a category of related values.

Examples:

```text
ComResultType
DomGrade
AppExecutionMode
```

---

## 15.2 Enumeration Members

Enumeration members SHALL use PascalCase.

Examples:

```text
Success
Failure
Warning
Information
```

Enumeration member names SHALL remain concise and unambiguous.

---

## 15.3 Uniqueness

Enumeration member names SHOULD be meaningful within the context of the enumeration.

The enumeration name SHALL provide the necessary context without requiring redundant prefixes on every member.

Example:

```text
Public Enum DomGrade
    First
    Second
    Third
End Enum
```

rather than:

```text
Public Enum DomGrade
    DomGradeFirst
    DomGradeSecond
    DomGradeThird
End Enum
```
# 16. User Defined Type Naming

## 16.1 General

User Defined Types (UDTs) SHALL use PascalCase.

A UDT name SHALL represent a cohesive data structure.

Examples:

```text
ComVersionInfo
InfFileMetadata
AppExecutionContext
```

UDTs SHALL NOT contain business behavior.

---

## 16.2 Member Naming

UDT member names SHALL use PascalCase.

Examples:

```text
Major
Minor
Patch
Build
```

Member names SHALL clearly describe the stored value.

---

# 17. File Naming

## 17.1 General

File names SHALL use PascalCase.

File names SHALL correspond to the primary artifact contained within the file.

Examples:

```text
ComLogger.cls
DomStudent.cls
AppImportService.cls
ComUtility.bas
```

---

## 17.2 Class Files

The file name of a class module SHALL exactly match the class name.

Example:

```text
DomStudent.cls
```

---

## 17.3 Standard Modules

The file name of a standard module SHALL exactly match the module name.

Example:

```text
ComValidation.bas
```

---

## 17.4 UserForms

UserForm names SHALL follow the same naming rules as classes.

Examples:

```text
PreMainForm
PreSettingsDialog
```

---

## 17.5 Specification Documents

Normative specification documents SHALL use the following format:

```text
<DocumentName>_v<Major>.<Minor>.md
```

Examples:

```text
Canon_v2.0.md
Architecture_v2.0.md
NamingConvention_v2.0.md
```

---

# 18. Folder Naming

## 18.1 General

Folder names SHALL use PascalCase.

Folder names SHALL describe their contents.

Examples:

```text
Specification
Source
Modules
Classes
Interfaces
Resources
Tests
```

---

## 18.2 Versioned Directories

Directories representing released artifacts SHOULD include the version identifier where appropriate.

Example:

```text
Build_v2.0
```

---

## 18.3 Temporary Directories

Temporary directories SHOULD clearly indicate their transient nature.

Examples:

```text
Temp
Cache
Working
```

Temporary directories SHALL NOT contain permanent project artifacts.
# 19. Configuration Key Naming

## 19.1 General

Configuration keys SHALL use PascalCase.

Each key SHALL represent a single configuration item.

Examples:

```text
ApplicationName
LogDirectory
UpdateServerUrl
ManifestVersion
DefaultLanguage
```

Configuration keys SHALL remain stable across compatible releases.

---

## 19.2 Hierarchical Configuration

Where hierarchical configuration is required by the configuration provider, each level SHALL use the same PascalCase naming convention.

Each level SHALL represent a meaningful logical grouping.

---

# 20. Manifest Identifier Naming

## 20.1 General

Manifest identifiers SHALL use PascalCase.

Identifiers SHALL clearly express the represented artifact.

Examples:

```text
Application
Module
Version
Dependencies
PublicApi
```

Manifest identifiers SHALL remain consistent throughout the project.

---

## 20.2 Identifier Stability

Manifest identifier names SHALL NOT be changed without corresponding versioning and compatibility review.

---

# 21. API Naming

## 21.1 General

Public API names SHALL use PascalCase.

API names SHALL begin with a verb that accurately represents the exposed operation.

Examples:

```text
Initialize
ExecuteImport
RegisterModule
ResolveDependency
BuildManifest
```

---

## 21.2 Public Contract

Public API names SHALL express behavior rather than implementation.

Implementation details SHALL NOT appear in published API names.

---

## 21.3 Compatibility

Renaming a published API SHALL be treated as a breaking change.

Backward compatibility SHALL be preserved unless a major version change explicitly permits otherwise.

---

# 22. Event Naming

## 22.1 General

Event names SHALL use PascalCase.

Event names SHALL describe the occurrence that triggers notification.

Examples:

```text
Initialized
Completed
Failed
Changed
Loaded
Disposed
```

---

## 22.2 Event Handlers

Event handler procedures SHOULD use the originating object followed by the event name.

Examples:

```text
WorkbookOpen
WorkbookBeforeClose
RibbonLoaded
ButtonClicked
```

The naming pattern SHALL be applied consistently throughout the project.

---

# 23. Error Identifier Naming

## 23.1 General

Error-related identifiers SHALL conform to ErrorHandling_v2.0.

Error enumeration names SHALL use PascalCase.

Examples:

```text
ComErrNum
DomErrNum
AppErrNum
InfErrNum
```

---

## 23.2 Error Members

Error enumeration members SHALL:

- Begin with the owning layer prefix.
- Include the `Err` element.
- Clearly describe the failure condition.

Examples:

```text
ComErrInvalidArgument
ComErrFileNotFound
DomErrOutOfRange
AppErrConfigurationMissing
InfErrAccessDenied
```

Error names SHALL describe the error condition rather than its cause or implementation.

---

# 24. Abbreviation Rules

## 24.1 General

Abbreviations SHALL be avoided unless they are well-established, unambiguous, and consistently used throughout the project.

An abbreviation SHALL NOT reduce readability.

---

## 24.2 Approved Abbreviations

| Abbreviation | Meaning |
|--------------|--------|
| Api          | Application Programming Interface |
| App          | Application |
| Com          | Common |
| Dom          | Domain |
| Inf          | Infrastructure |
| Pre          | Presentation |
| Id           | Identifier |
| Url          | Uniform Resource Locator |
| Uri          | Uniform Resource Identifier |
| Xml          | Extensible Markup Language |
| Csv          | Comma-Separated Values |
| Json         | JavaScript Object Notation |

* Json is reserved only for interoperability with external systems.
  Build.xlam Project SHALL NOT introduce JSON as an internal configuration or persistence mechanism.

---

## 24.3 Acronym Capitalization

Abbreviations SHALL follow PascalCase rules.

Examples:

```text
ApiClient
XmlReader
CsvImporter
UriBuilder
```

The following forms SHALL NOT be used:

```text
APIClient
XMLReader
CSVImporter
URIBuilder
```

---

## 24.4 Project-Specific Terms

Project-specific abbreviations SHALL be defined in `Glossary_v2.0` before they are used in source code or specifications.

Undefined abbreviations SHALL NOT be introduced.

---

# 25. Reserved Words

## 25.1 Reserved Project Names

The following architectural suffixes are reserved for their defined responsibilities:

- Facade
- CompositionRoot
- Factory
- Builder
- Provider
- Service
- Repository
- Configuration

These suffixes SHALL NOT be applied to artifacts that do not fulfill the corresponding responsibility.

---

## 25.2 Reserved Layer Prefixes

The following layer prefixes are reserved:

- Com
- Dom
- App
- Inf
- Pre

Additional layer prefixes SHALL NOT be introduced without an architectural revision.

---

## 25.3 VBA Reserved Keywords

Identifiers SHALL NOT conflict with VBA language keywords.

Where a domain term conflicts with a VBA keyword, an alternative descriptive identifier SHALL be selected.

---

# 26. Examples

## 26.1 Good Examples

```text
ComFacade
ComCompositionRoot
ComLogger
DomStudent
DomClassHour
DomSubject
AppImportService
InfWorkbookProvider
PreMainPresenter

Initialize
ExecuteImport
ResolveDependency
BuildManifest

StudentId
WorkbookPath
Configuration
ManifestVersion

IsValid
HasChildren
CanExecute
ShouldUpdate
```

---

## 26.2 Non-Conforming Examples

The following examples do **not** conform to this specification.

```text
module1
Helper
Utilities
Misc

APIClient
XMLParser

ILog
IRepository

tmp
obj
data1
value2

Student_Class
MyModule
GeneralStuff
```

The reasons include:

- Missing layer prefix
- Incorrect capitalization
- Non-descriptive naming
- Hungarian notation
- Undefined abbreviation
- Inconsistent architectural terminology

---

# 27. Conformance

Source code, specifications, and project artifacts SHALL conform to this specification.

Automated review tools SHOULD verify naming consistency where practical.

Any deviation SHALL be explicitly justified and documented in the corresponding specification or implementation artifact.

This document is a normative specification for the Build.xlam Project and SHALL be interpreted in conjunction with all higher-level specifications.
