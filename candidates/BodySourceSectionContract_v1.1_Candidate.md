# Body Source / Section Contract v1.1 Candidate

Version : 1.1 Candidate
Status  : Candidate
Scope   : VMF Body Source Contract / Build.xlam Generator Contract
Depends : Canon_v2.0.md, VMF_v1.0.md, VMF_v1.1_Candidate.md, TemplateContract_v1.1_Candidate.md

---

# 1. Purpose

This document defines the VMF v1.1 Candidate contract for supplying generated
body and section content to templates.

VMF v1.0 remains frozen. This candidate does not modify, supersede, or
reinterpret VMF v1.0. It records an additive contract that may be adopted by a
future VMF version.

---

# 2. Scope

This candidate covers:

- BodySource references;
- SectionSource references;
- manifest metadata for body and section generation;
- generator mediation between source metadata and template insertion;
- prohibition of implementation bodies directly embedded in manifest records.

This candidate does not define detailed generation algorithms, VBA procedure
implementation, domain behavior, UI behavior, packaging, or workbook import and
export mechanics.

---

# 3. Contract Role

BodySource and SectionSource artifacts describe generated source content inputs.

They are distinct from:

- Blueprint Contract, which describes project intent;
- Manifest Contract, which describes concrete generation targets;
- Template Contract, which describes source structure.

Generator SHALL mediate these contracts and emit source through the template
`{{BODY}}` insertion point and named section insertion points.

---

# 4. Manifest Responsibility

Manifest items SHALL describe where body and section content comes from.

Manifest items SHALL NOT contain source implementation bodies directly.

A manifest item MAY reference body and section sources by stable identifiers or
paths.

Example candidate shape:

```yaml
module_name: ExampleService
component_type: ClassModule
layer: Application
template_contract: ClassModule
template_path: candidates/templates/v1.1/ClassModuleTemplate.txt
body_source: candidates/body/v1.1/ExampleService.body.yaml
sections:
  PublicApi: candidates/body/v1.1/ExampleService.PublicApi.vba
  PrivateProcedures: candidates/body/v1.1/ExampleService.PrivateProcedures.vba
```

The exact storage format of body and section source artifacts MAY be defined by
a later candidate.

---

# 5. BodySource Responsibility

BodySource artifacts describe generated body content for one component or one
component family.

BodySource artifacts MAY contain:

- generated constants;
- generated declarations;
- generated public API members;
- generated private procedures;
- lifecycle procedures;
- generated implementation comments.

BodySource artifacts SHALL NOT:

- redefine module identity;
- select templates;
- override manifest target metadata;
- encode layer dependency exceptions;
- redefine VMF v1.0 behavior.

---

# 6. SectionSource Responsibility

SectionSource artifacts describe content for a named body section.

Section names SHALL match stable section contract names defined by the Template
Contract or a future adopted generation contract.

SectionSource artifacts SHALL be omitted from generated source when their content
is empty.

Generator SHALL NOT emit empty section headers solely because a section is known
by contract.

---

# 7. Generator Responsibility

Generator SHALL:

- resolve manifest body and section source references;
- validate that referenced sources exist when required;
- assemble body content in the canonical body section order;
- insert assembled body content through `{{BODY}}`;
- insert named section content through the Section Contract;
- omit empty body or section content;
- preserve template ownership of source structure.

Generator SHALL NOT:

- treat manifest records as source bodies;
- place behavioral defaults in templates;
- emit placeholder artifacts for missing body content;
- infer generation behavior from template comments.

---

# 8. Body Assembly Order

When sectioned body content is supplied, Generator SHOULD assemble content in the
order defined by the Template Contract:

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

Sections with no content SHALL be omitted.

---

# 9. Compatibility

This candidate is additive.

Existing VMF v1.0 behavior and Build v1.1 released behavior remain unchanged
until a future adoption decision explicitly adopts this contract.

Current generators MAY continue removing `{{BODY}}` when no body content is
supplied.

---

# 10. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 1.1 Candidate | Candidate | Defines BodySource and SectionSource references as the recommended source of generated body content. |
