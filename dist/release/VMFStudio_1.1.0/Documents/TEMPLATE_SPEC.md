# VMF Studio Template Compatibility Notes

Product : VMF Studio
Version : 1.1.0
TemplateSchemaVersion : 1.0
Status : Compatibility note, not a replacement for frozen template behavior.

---

## Scope

VMF Studio v1.1 Template Manager edits the same template files used by the
existing generator. It does not introduce a Studio-only template format.

---

## Template Files

The release includes existing template files under `templates/`:

- `ClassTemplate.txt`
- `DomainClassTemplate.txt`
- `DomainModuleTemplate.txt`
- `ModuleTemplate.txt`

Candidate templates under `candidates/templates/v1.1/` remain candidate assets
unless explicitly selected by a manifest or future release process.

---

## Placeholder Syntax

Existing placeholder syntax is preserved:

```text
{{ModuleName}}
{{Layer}}
{{BODY}}
{{MemberDeclarations}}
{{Properties}}
```

Unknown placeholders are validation errors in Template Manager.

---

## Section Syntax

Existing section syntax is preserved:

```text
@section ModuleDeclaration
{{ModuleDeclaration}}
@endsection
```

Template validation checks missing names, duplicate sections, nested sections,
unmatched end markers, and unclosed sections.

---

## Save Safety

Template save validates before replacing the existing file. Save failures must
leave the original template intact.

---

## Preview

Template preview uses unsaved template content plus the selected module model.
Preview must not save the template or manifest and must use the same render
path as generation.
