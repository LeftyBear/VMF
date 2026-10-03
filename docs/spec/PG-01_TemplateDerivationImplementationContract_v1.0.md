# PG-01 Implementation Contract v1.0

## 1. Document Control

- ID: PG-01 Implementation Contract
- Version: 1.0
- Status: Adopted
- Requirements Authority: User under ADR-0020
- Adoption Date: 2026-10-03
- Scope: PG-01 Exact Template Derivation

## 2. Public Entry

Component:
AppTemplateDeriver

Path:
src/Build/Application/AppTemplateDeriver.cls

Single public entry:

Public Function AppDeriveTemplateBindings(
    ByVal ManifestItems As Collection
) As AppTemplateDerivationResult

PG-01 does not perform PG-02 production wiring.

## 3. AppTemplateDerivationResult

Class:
src/Build/Application/AppTemplateDerivationResult.cls

Public contract:

Public Sub AppInitialize(
    ByVal IsSuccess As Boolean,
    ByVal Items As Collection,
    ByVal Diagnostics As Collection)

Public Property Get IsSuccess() As Boolean
Public Property Get Items() As Collection
Public Property Get Diagnostics() As Collection
Public Function HasDiagnosticCode(ByVal Code As String) As Boolean
Public Function ErrorCount() As Long

Success invariants:
- IsSuccess = True
- Items.Count = input ManifestItems.Count
- input order is preserved
- Diagnostics.Count = 0

Failure invariants:
- IsSuccess = False
- Items.Count = 0
- Diagnostics.Count >= 1

Items and Diagnostics must never be Nothing.
Partial successful output is prohibited.
AppInitialize must reject inconsistent result state.

## 4. AppTemplateDerivationItem

Class:
src/Build/Application/AppTemplateDerivationItem.cls

Fields/properties:

- SourceIndex As Long
- ModuleName As String
- ModuleType As String
- LayerName As String
- TemplateKey As String
- TemplatePath As String
- TemplateRole As String
- SelectionRuleId As String
- DerivationReason As String
- IsGeneratable As Boolean
- UnsupportedReason As String

A successful returned item has:
- IsGeneratable = True
- UnsupportedReason = ""

Failed items are not returned as successful derivation items.
Their failures are represented by diagnostics.

## 5. AppTemplateDerivationDiagnostic

Class:
src/Build/Application/AppTemplateDerivationDiagnostic.cls

Fields/properties:

- Code As String
- Category As String
- ItemIndex As Long
- ModuleName As String
- FieldName As String
- Message As String
- SortOrder As Long
- Severity As String

Severity is always:
Error

### Diagnostic definitions

TD001
Category: InputRequired
Condition: ManifestItems is Nothing.

TD002
Category: InputEmpty
Condition: ManifestItems.Count = 0.

TD003
Category: InvalidItemType
Condition: collection item is Nothing or is not ManifestItem.

TD004
Category: InvalidItemState
Condition: ManifestItem.InfValidate = False or a required getter fails.

TD005
Category: MissingRequiredFact
Field: ModuleName
Condition: ModuleName is blank.

TD006
Category: MissingRequiredFact
Field: ModuleType
Condition: ModuleType is blank.

TD007
Category: UnsupportedManifestFact
Field: ModuleType
Condition: ModuleType is unsupported.

TD008
Category: MissingRequiredFact
Field: LayerName
Condition: LayerName is blank.

TD009
Category: UnsupportedManifestFact
Field: LayerName
Condition: LayerName is unsupported.

TD010
Category: MissingRequiredFact
Field: TemplatePath
Condition: TemplatePath is blank.

TD011
Category: InvalidTemplatePath
Field: TemplatePath
Condition: TemplatePath cannot be lexically normalized or lexical
resolution attempts to escape the path root.

TD012
Category: TemplatePathConflict
Field: TemplatePath
Condition: Manifest TemplatePath conflicts with the independently
derived canonical template identity/path.

TD013
Category: ManifestOnlyTemplateMisuse
Field: TemplatePath
Condition: DomainModuleTemplate is specified or selected by Manifest
data even though it is not an approved Template Derivation identity.

TD014
Category: TemplateRuleNotFound
Condition: no approved Template Derivation rule matches the item.

TD015
Category: TemplateRuleNotUnique
Condition: more than one approved Template Derivation rule matches.

TD016
Category: DuplicateModuleName
Field: ModuleName
Condition: duplicate module identity exists in the input collection.

TD017
Category: OutputInvariantFailure
Condition: the required result/output invariant cannot be satisfied.

Message must deterministically describe the corresponding condition.
Input values must not alter the diagnostic classification.

## 6. Approved Template Identity Table

Approved logical inventory:

TemplateKey: ModuleTemplate
TemplatePath: templates/ModuleTemplate.txt
TemplateRole: StandardModuleTemplate

TemplateKey: ClassTemplate
TemplatePath: templates/ClassTemplate.txt
TemplateRole: ClassModuleTemplate

TemplateKey: DomainClassTemplate
TemplatePath: templates/DomainClassTemplate.txt
TemplateRole: DomainClassModuleTemplate

DomainModuleTemplate is not approved and must not be selected,
regardless of physical filesystem presence.

Approved inventory existence means membership in this fixed logical
identity table.

Template Derivation must not check physical filesystem existence or
template content validity.

Those checks belong to a separate Template Provider gate.

## 7. Deterministic Mapping Rules

Rule 1:
ModuleType = StandardModule
LayerName = any approved layer

Result:
TemplateKey = ModuleTemplate
TemplatePath = templates/ModuleTemplate.txt
TemplateRole = StandardModuleTemplate
SelectionRuleId = TD-P5-02-STANDARD-MODULE

Rule 2:
ModuleType = ClassModule
LayerName = Domain

Result:
TemplateKey = DomainClassTemplate
TemplatePath = templates/DomainClassTemplate.txt
TemplateRole = DomainClassModuleTemplate
SelectionRuleId = TD-P5-02-DOMAIN-CLASS

Rule 3:
ModuleType = ClassModule
LayerName = Common, Core, Application, Infrastructure, or Presentation

Result:
TemplateKey = ClassTemplate
TemplatePath = templates/ClassTemplate.txt
TemplateRole = ClassModuleTemplate
SelectionRuleId = TD-P5-02-CLASS

DerivationReason is exactly structured as:

ModuleType=<value>; LayerName=<value>; RuleId=<selectionRuleId>

No corrective case folding, fallback, implicit selection, or input
repair is permitted.

## 8. TemplatePath Authority and Compatibility

Primary template identity:
templateKey

Canonical templatePath:
repository/project-relative templates/... path from the approved
identity table.

Absolute filesystem resolution is a Template Provider runtime
responsibility.

ManifestItem.TemplatePath remains present during PG-01 for migration
compatibility.

ManifestItem.TemplatePath:
- is not Template selection authority
- is not the source of Template identity
- is used only for consistency checking against the independently
  derived canonical identity/path

Absolute/canonical comparison requires explicit normalization.
Implicit equality is prohibited.

Removal or later lifecycle change of ManifestItem.TemplatePath is
deferred to a future versioned contract migration.

## 9. TemplatePath Normalization and Comparison

Canonical output always uses the exact repository/project-relative
TemplatePath from the approved identity table.

Manifest TemplatePath comparison procedure:

1. trim surrounding whitespace
2. convert "\" separators to "/"
3. collapse duplicate separators
4. remove "." path segments
5. lexically resolve ".." segments
6. reject root escape as TD011
7. remove trailing separator
8. comparison is case-insensitive only for Windows path compatibility
9. a relative path must completely match the canonical relative path
10. an absolute path must contain the complete canonical path as a
    suffix at a path-segment boundary
11. basename-only matching is prohibited
12. no filesystem existence, read, or content check is performed

## 10. Formal Input Contract

Formal input:

ByVal ManifestItems As Collection

Each collection element must be strictly validated as ManifestItem at
the component boundary.

Input order must be preserved.

Nothing collection, empty collection, invalid element type/state, and
missing required facts are hard-stop validation failures.

PG-01 does not introduce a dictionary adapter or new typed Manifest
input model.

ManifestItem exposure at this Application boundary is permitted for
PG-01 and does not prohibit a future typed migration.

Prohibited PG-01 inputs/dependencies include:
- raw Blueprint
- Template contents
- GenerateContext
- Generator runtime state

## 11. Validation Order

Collection-level order:

1. ManifestItems Nothing check
2. empty collection check
3. validate item 1 through item n
4. duplicate ModuleName check
5. finalize diagnostics
6. if any diagnostic exists, produce failure
7. if no diagnostic exists, build successful derivation items in input order
8. validate result invariant

Per-item validation order:

1. Nothing / ManifestItem type
2. InfValidate
3. ModuleName required
4. ModuleType required / exact / supported
5. LayerName required / exact / supported
6. TemplatePath required / lexical validity
7. approved rule count must equal exactly one
8. approved Template identity membership
9. DomainModuleTemplate prohibition
10. Manifest TemplatePath consistency
11. output completeness

Dependency rule:
A dependent check is skipped when its prerequisite fact is invalid or
unavailable.

Independent fields/checks continue to be evaluated so that all
independently evaluable deterministic diagnostics can be accumulated
without side effects.

## 12. Diagnostic Ordering and Deduplication

Diagnostics are deterministic.

Ordering:

1. collection-level diagnostics
2. ItemIndex ascending
3. fixed per-item validation-step order

For the same:
ItemIndex + Code + FieldName

only the first diagnostic is retained.

There is no cross-item deduplication.

After final ordering:
SortOrder is assigned sequentially starting at 1.

## 13. Failure Atomicity

Candidate successful items remain private until all validation passes.

If any diagnostic/failure exists:

- IsSuccess = False
- Items.Count = 0
- Diagnostics contains the complete deterministic diagnostic set
- no partial successful derivation collection is exposed

AppTemplateDeriver must not invoke:

- GenerateContext
- Generator
- Template Provider
- filesystem access
- Blueprint processing
- external/runtime services

Expected validation/derivation failures are represented by the typed
result contract rather than exception-only control flow.

Unexpected programming/runtime failures continue to follow the
existing VMF error boundary.

## 14. Adopted D1-D7 Decisions

D1 — Input Type
Formal input is ByVal ManifestItems As Collection.
Each element is strictly validated as ManifestItem.
Order is preserved.
No dictionary adapter/new typed input model is introduced in PG-01.

D2 — ManifestItem.TemplatePath Lifecycle
ManifestItem.TemplatePath is retained for migration compatibility.
It is not selection authority.
It is consistency-check evidence only.
Removal/post-derivation lifecycle is deferred to a future versioned
contract migration.

D3 — Public Entry
Component: AppTemplateDeriver
Path: src/Build/Application/AppTemplateDeriver.cls
Single public Function:
AppDeriveTemplateBindings(ByVal ManifestItems As Collection)
As AppTemplateDerivationResult.
PG-02 production wiring is excluded.

D4 — Result Contract
Use a dedicated Template Derivation typed result.
Do not change ComResult.
Expected hard-stop failure is represented as a normal typed result.
Failure returns zero successful downstream items.

D5 — Diagnostics
Use deterministic ordered accumulated diagnostics.
Inspect all independently evaluable input without side effects.
Any diagnostic causes atomic whole-operation failure.
Unexpected programming/runtime errors remain under the existing VMF
error boundary.

D6 — Inventory Existence
Approved inventory existence means membership in the fixed approved
logical identity table.
No physical filesystem existence/content validation occurs in PG-01.
Physical validation belongs to Template Provider.
DomainModuleTemplate remains unapproved even if physically present.

D7 — Canonical Identity and Path
Primary identity is templateKey.
Canonical templatePath is repository/project-relative templates/...
Absolute filesystem resolution belongs to Template Provider.

## 15. Implementation Scope

ADD:

src/Build/Application/AppTemplateDeriver.cls
src/Build/Application/AppTemplateDerivationResult.cls
src/Build/Application/AppTemplateDerivationItem.cls
src/Build/Application/AppTemplateDerivationDiagnostic.cls
tests/unit/Build/AppTemplateDeriverTests.bas

MODIFY only if required by established repository convention:

existing Build unit-test runner registration point

No other production/test modification is authorized by this contract.

## 16. Focused Unit-Test Contract

Focused tests must cover at least:

1. StandardModule approved mapping
2. Domain ClassModule mapping
3. non-Domain ClassModule mapping for Common
4. non-Domain ClassModule mapping for Core
5. non-Domain ClassModule mapping for Application
6. non-Domain ClassModule mapping for Infrastructure
7. non-Domain ClassModule mapping for Presentation
8. input/output order preservation
9. deterministic repeated result and ordering
10. canonical relative TemplatePath
11. slash normalization
12. dot-segment normalization
13. compatible absolute-path suffix
14. basename-only rejection
15. root-escape rejection
16. TemplatePath conflict rejection
17. Nothing collection
18. empty collection
19. Nothing item
20. non-ManifestItem
21. invalid ManifestItem state
22. required ModuleName failure
23. required ModuleType failure
24. required LayerName failure
25. required TemplatePath failure
26. unsupported ModuleType
27. unsupported LayerName
28. no matching Template rule
29. multiple matching Template rules
30. DomainModuleTemplate rejection
31. duplicate ModuleName
32. accumulated diagnostics
33. deterministic diagnostic ordering
34. same-item/code/field diagnostic deduplication
35. no cross-item diagnostic deduplication
36. earlier valid item followed by later failure yields Items.Count = 0
37. no filesystem dependency
38. no Template Provider dependency
39. no GenerateContext dependency
40. no Generator dependency
41. uninitialized result access behavior
42. uninitialized item access behavior
43. uninitialized diagnostic access behavior
44. result invariant rejection

Tests may be created under a separately granted PG-01 implementation
authorization, but this contract does not itself authorize their
execution.

## 17. Non-Responsibilities

PG-01 does not perform:

- Blueprint parsing
- Blueprint validation
- Manifest derivation
- Manifest repair
- Template content reading
- fallback Template selection
- GenerateContext construction
- Generator execution
- output file writing
- PG-02 production orchestration/wiring
- PG-03 production connection

## 18. Non-Effects

Adoption or persistence of this document does not mean or grant:

- PG-01 ESTABLISHED
- test/mock/dry-run execution authorization
- PG-02 authorization
- PG-03 authorization
- evidence sufficiency
- evidence acceptance
- production-gap resolution
- security clearance
- residual-risk acceptance
- broader technical GO
- execution authorization outside separately granted PG-01 scope
- SAFE-STOP release
- Git stage/commit/push authorization

Current broader state therefore remains:
Technical execution = NO-GO / SAFE-STOP.
