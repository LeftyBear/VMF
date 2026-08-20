# P2-03 — Blueprint Specification v0.1 Example Documents

## Status

COMPLETE / docs-only examples

## Purpose

Provide example Blueprint Specification v0.1 documents based on the P2-01 scope definition and P2-02 field model.

These examples are reference material for later Blueprint parser, validator, Manifest derivation, Template, GenerateContext, and Generator work.

## Scope

This task is docs-only.

It provides:

- valid Blueprint examples
- invalid Blueprint examples
- Manifest derivation explanation examples
- approval boundary examples

It does not implement parser, validator, Manifest generator, Template, GenerateContext, or VBA Generator changes.

## Example 1 — Minimal Approved Blueprint

```yaml
blueprintId: BP-VMF-EX-001
version: "0.1"
status: approved
approval:
  state: approved
  approvedBy: "reviewer"
  approvedAt: "2026-08-20T00:00:00Z"
target:
  kind: workbook
  name: "SampleWorkbook"
modules:
  - name: "modHello"
    kind: standard
    responsibility: "Provide a simple greeting macro."
    procedures:
      - name: "SayHello"
        kind: Sub
        visibility: Public
        responsibility: "Display a greeting message."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Expected result:

- Valid Blueprint
- Manifest derivation allowed
- VBA generation may proceed after downstream validation

Reason:

- `status = approved`
- `approval.state = approved`
- required fields are present
- module list contains one module
- Sub procedure does not define `returnValue`

## Example 2 — Function with Return Value

```yaml
blueprintId: BP-VMF-EX-002
version: "0.1"
status: approved
approval:
  state: approved
  approvedBy: "reviewer"
  approvedAt: "2026-08-20T00:00:00Z"
target:
  kind: workbook
  name: "CalculationWorkbook"
modules:
  - name: "modMath"
    kind: standard
    responsibility: "Provide basic calculation procedures."
    procedures:
      - name: "AddNumbers"
        kind: Function
        visibility: Public
        responsibility: "Return the sum of two numbers."
        parameters:
          - name: "leftValue"
            type: Double
            passing: ByVal
          - name: "rightValue"
            type: Double
            passing: ByVal
        returnValue:
          type: Double
          description: "Sum of leftValue and rightValue."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Expected result:

- Valid Blueprint
- Manifest derivation allowed

Reason:

- Function procedure defines `returnValue`
- parameters are explicitly defined
- required approval state is satisfied

## Example 3 — Multiple Modules

```yaml
blueprintId: BP-VMF-EX-003
version: "0.1"
status: approved
approval:
  state: approved
  approvedBy: "reviewer"
  approvedAt: "2026-08-20T00:00:00Z"
target:
  kind: addin
  name: "SampleAddin"
modules:
  - name: "modEntryPoint"
    kind: standard
    responsibility: "Provide public entry points."
    procedures:
      - name: "RunMain"
        kind: Sub
        visibility: Public
        responsibility: "Run the main workflow."
        dependencies:
          - kind: module
            name: "modWorker"
            reason: "Delegates processing."
  - name: "modWorker"
    kind: standard
    responsibility: "Provide internal processing logic."
    procedures:
      - name: "ExecuteWork"
        kind: Sub
        visibility: Private
        responsibility: "Execute internal work steps."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  lineEnding: crlf
  missingDirectoryPolicy: error
```

Expected result:

- Valid Blueprint
- Manifest derivation allowed

Reason:

- multiple modules are allowed
- dependency is declared explicitly
- `lineEnding` uses an allowed value

## Example 4 — Draft Blueprint

```yaml
blueprintId: BP-VMF-EX-004
version: "0.1"
status: draft
approval:
  state: notApproved
target:
  kind: workbook
  name: "DraftWorkbook"
modules:
  - name: "modDraft"
    kind: standard
    responsibility: "Draft module for review."
    procedures:
      - name: "DraftMacro"
        kind: Sub
        visibility: Public
        responsibility: "Draft macro not yet approved."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Expected result:

- Structurally valid as a draft Blueprint
- Manifest derivation not allowed
- VBA generation not allowed

Reason:

- `status = draft`
- `approval.state = notApproved`
- only approved Blueprints may be used for Manifest generation

## Example 5 — Rejected Blueprint

```yaml
blueprintId: BP-VMF-EX-005
version: "0.1"
status: rejected
approval:
  state: rejected
  note: "Rejected during human review."
target:
  kind: workbook
  name: "RejectedWorkbook"
modules:
  - name: "modRejected"
    kind: standard
    responsibility: "Rejected design example."
    procedures:
      - name: "RejectedMacro"
        kind: Sub
        visibility: Public
        responsibility: "Rejected macro example."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Expected result:

- Structurally valid as a rejected Blueprint
- Manifest derivation not allowed
- VBA generation not allowed

Reason:

- rejected Blueprint is not eligible for generation
- approval state does not permit generation

## Example 6 — Invalid Function without Return Value

```yaml
blueprintId: BP-VMF-EX-006
version: "0.1"
status: approved
approval:
  state: approved
  approvedBy: "reviewer"
  approvedAt: "2026-08-20T00:00:00Z"
target:
  kind: workbook
  name: "InvalidFunctionWorkbook"
modules:
  - name: "modInvalidFunction"
    kind: standard
    responsibility: "Invalid Function example."
    procedures:
      - name: "CalculateValue"
        kind: Function
        visibility: Public
        responsibility: "Calculate and return a value."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Expected result:

- Invalid Blueprint
- Manifest derivation not allowed
- VBA generation not allowed

Reason:

- `kind = Function`
- `returnValue` is missing
- Function procedures require `returnValue`

## Example 7 — Invalid Approved Status without Approved Approval State

```yaml
blueprintId: BP-VMF-EX-007
version: "0.1"
status: approved
approval:
  state: notApproved
target:
  kind: workbook
  name: "ApprovalConflictWorkbook"
modules:
  - name: "modApprovalConflict"
    kind: standard
    responsibility: "Approval conflict example."
    procedures:
      - name: "Run"
        kind: Sub
        visibility: Public
        responsibility: "Run a macro."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Expected result:

- Invalid Blueprint
- Manifest derivation not allowed
- VBA generation not allowed

Reason:

- `status = approved`
- `approval.state = notApproved`
- status and approval state conflict
- Manifest generation requires both `status = approved` and `approval.state = approved`

## Example 8 — Invalid Sub with Return Value

```yaml
blueprintId: BP-VMF-EX-008
version: "0.1"
status: approved
approval:
  state: approved
  approvedBy: "reviewer"
  approvedAt: "2026-08-20T00:00:00Z"
target:
  kind: workbook
  name: "InvalidSubWorkbook"
modules:
  - name: "modInvalidSub"
    kind: standard
    responsibility: "Invalid Sub example."
    procedures:
      - name: "RunTask"
        kind: Sub
        visibility: Public
        responsibility: "Run a task."
        returnValue:
          type: Boolean
          description: "Invalid return value for Sub."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Expected result:

- Invalid Blueprint
- Manifest derivation not allowed
- VBA generation not allowed

Reason:

- `kind = Sub`
- Sub procedures must not define `returnValue`

## Example 9 — Manifest Derivation Example

Source Blueprint fields:

```yaml
blueprintId: BP-VMF-EX-009
version: "0.1"
status: approved
approval:
  state: approved
  approvedBy: "reviewer"
  approvedAt: "2026-08-20T00:00:00Z"
target:
  kind: workbook
  name: "ManifestExampleWorkbook"
modules:
  - name: "modExample"
    kind: standard
    responsibility: "Provide one generated macro."
    procedures:
      - name: "RunExample"
        kind: Sub
        visibility: Public
        responsibility: "Run the example macro."
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Derived Manifest may contain:

```yaml
sourceBlueprintId: BP-VMF-EX-009
sourceBlueprintVersion: "0.1"
target:
  kind: workbook
  name: "ManifestExampleWorkbook"
modules:
  - name: "modExample"
    kind: standard
    procedures:
      - name: "RunExample"
        kind: Sub
        visibility: Public
generationPolicy:
  allowOverwrite: true
  encoding: utf-8
  missingDirectoryPolicy: error
```

Derivation rule:

- Manifest may normalize fields required for generation
- Manifest may omit human-review-only fields
- Manifest must not add new design intent
- Manifest must not add new modules, procedures, dependencies, or responsibilities

## Example Classification Summary

| Example | Validity | Manifest Derivation | Reason |
|---|---:|---:|---|
| Minimal approved Blueprint | Valid | Allowed | Approved and structurally valid |
| Function with returnValue | Valid | Allowed | Function return value is defined |
| Multiple modules | Valid | Allowed | Explicit modules and dependency |
| Draft Blueprint | Structurally valid | Not allowed | Not approved |
| Rejected Blueprint | Structurally valid | Not allowed | Rejected |
| Function without returnValue | Invalid | Not allowed | Missing required Function return value |
| Approved status without approved approval state | Invalid | Not allowed | Approval conflict |
| Sub with returnValue | Invalid | Not allowed | Sub must not return a value |

## Validation Expectations

Later validators should use these examples to distinguish:

- structurally valid approved Blueprints
- structurally valid but non-generatable Blueprints
- invalid Blueprints
- approval conflicts
- Manifest derivation boundaries

## Out of Scope

P2-03 does not define:

- YAML parser implementation
- validation error codes
- Manifest file format as a final specification
- Template behavior
- GenerateContext behavior
- Generator behavior
- AI draft generation behavior
- UI review behavior

## Verification

Docs-only task.

Expected checks:

- Markdown added
- No VBA implementation changed
- No parser behavior changed
- No validator behavior changed
- No generator behavior changed
- No test execution required
