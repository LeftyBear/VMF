# Template Contract v1.1 Candidate

Version : 1.1 Candidate
Status  : Candidate
Scope   : VMF Template Contract / Build.xlam Generator Contract
Depends : Canon_v2.0.md, VMF_v1.0.md, VMF_v1.1_Candidate.md

---

# 1. Purpose

This document defines the VMF v1.1 Candidate canonical template contract.

VMF v1.0 remains frozen. This candidate does not modify, supersede, or reinterpret VMF v1.0. It records an additive template contract that may be adopted by a future VMF version.

---

# 2. Scope

This candidate covers:

- Class Module Template;
- Predeclared Class Module Template;
- Standard Module Template;
- Interface Template;
- Enum Template;
- UserForm Template when UserForm generation is adopted;
- Section Contract using `@section`;
- `{{BODY}}` body insertion;
- manifest-driven template selection;
- generator responsibilities.

The canonical candidate template files are stored under:

```text
candidates/templates/v1.1/
```

---

# 3. Template Role

Templates SHALL express structure only.

Templates SHALL NOT contain business behavior, generated implementation logic, domain defaults, validation algorithms, or layer-specific use cases.

Templates MAY contain required VBA export headers, module attributes, `Option Explicit`, and structural comments.

---

# 4. Generator Role

Generator SHALL place generated source into templates according to the Template Contract.

Generator SHALL NOT:

- rewrite module attributes owned by the selected template;
- infer `VB_PredeclaredId` from module name;
- reinterpret template comments;
- generate empty optional sections;
- choose templates from implementation convenience.

Generator MAY:

- replace required tokens;
- include or omit a `@section` block according to section content;
- validate that a manifest-selected template exists;
- validate required token availability before generation.

---

# 5. Required Tokens

All canonical templates SHALL support these tokens:

```text
{{ModuleName}}
{{Layer}}
{{BODY}}
```

`{{ModuleName}}` identifies the generated VBA component.

`{{Layer}}` identifies the VMF or application layer.

`{{BODY}}` is the only standard generated code insertion token.

Legacy token forms such as `{ModuleName}`, `{Layer}`, and `{BODY}` MAY be supported by Generator for compatibility, but canonical v1.1 Candidate templates SHALL use the double-brace form.

---

# 6. Section Contract

A template section SHALL use this form:

```vb
' @section SectionName
' Section content or a generation token.
' @endsection
```

Section names SHALL be stable contract identifiers.

Section names SHALL NOT be localized presentation labels.

Generator SHALL remove the complete section block when the corresponding section content is empty or disabled by the manifest contract.

Generator SHALL remove both `@section` and `@endsection` markers from emitted source.

---

# 7. Body Section Order

Generated module body content SHOULD use this order inside `{{BODY}}`:

```text
Constants
ModuleState
Types
Events
Properties
PublicApi
FriendApi
PrivateProcedures
Lifecycle
ErrorHandling
```

Sections with no generated content SHALL be omitted.

Templates SHALL NOT emit empty section headers.

Template-level static sections, such as `OptionPrivateModule`, MAY exist outside `{{BODY}}` when they represent structural declarations rather than generated implementation body content.

---

# 8. Comment Rules

Template comments SHALL describe structural identity only:

- component kind;
- module name token;
- layer token;
- responsibility field.

Generated implementation comments belong to generated body content, not to the template.

Empty placeholder comments such as `TODO`, `Implementation`, or sample procedure comments SHALL NOT appear in canonical templates.

---

# 9. Canonical Templates

## 9.1 Class Module

File:

```text
candidates/templates/v1.1/ClassModuleTemplate.txt
```

Contract:

- `VB_PredeclaredId=False`;
- no implementation body;
- `{{BODY}}` inside a `Body` section.

## 9.2 Predeclared Class Module

File:

```text
candidates/templates/v1.1/PredeclaredClassModuleTemplate.txt
```

Contract:

- `VB_PredeclaredId=True`;
- used only when the Manifest Contract explicitly selects a predeclared class;
- Generator SHALL NOT rewrite the predeclared attribute.

## 9.3 Standard Module

File:

```text
candidates/templates/v1.1/StandardModuleTemplate.txt
```

Contract:

- standard module attributes are template-owned;
- `Option Private Module` is an optional section;
- generated body is inserted through `{{BODY}}`.

## 9.4 Interface

File:

```text
candidates/templates/v1.1/InterfaceTemplate.txt
```

Contract:

- represented as a VBA class module contract;
- `VB_PredeclaredId=False`;
- body content SHALL define interface members only.

## 9.5 Enum

File:

```text
candidates/templates/v1.1/EnumTemplate.txt
```

Contract:

- represented as a standard module containing one public enum;
- enum members are inserted through `{{BODY}}`;
- empty enum bodies SHALL NOT be generated.

## 9.6 UserForm

File:

```text
candidates/templates/v1.1/UserFormTemplate.frm.txt
```

Contract:

- candidate placeholder for UserForm code-behind structure;
- designer/control generation is outside this candidate unless separately adopted;
- body content SHALL define form code-behind members only.

---

# 10. Manifest Template Selection

Template selection SHALL be determined by the Manifest Contract.

A manifest generation item SHOULD identify:

```yaml
module_name: ExampleModule
component_type: ClassModule
layer: Application
template_contract: ClassModule
template_path: candidates/templates/v1.1/ClassModuleTemplate.txt
predeclared_id: false
```

`template_contract` SHALL determine the expected template class.

`template_path` SHALL identify the physical template asset.

`predeclared_id` SHALL be consistent with the selected template contract.

When `predeclared_id: true`, the manifest item SHALL select `PredeclaredClassModuleTemplate.txt`.

Generator SHALL reject a manifest item when its component type, template contract, and template path are inconsistent.

---

# 11. Compatibility

This candidate is additive.

Existing VMF v1.0 and Build v1.1 released templates remain unchanged until a future adoption decision explicitly replaces them.

Candidate templates SHALL NOT be treated as released templates while stored under `candidates/`.

---

# 12. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 1.1 Candidate | Candidate | Defines canonical candidate templates, section contract, body insertion, and manifest-driven template selection. |
