# Manifest Source Generation

## Purpose

This document records the current Build generator manifest format for source
body insertion.

It is an implementation guide, not a Canon or VMF specification. VMF v1.0
remains frozen. The related future-version contract is tracked separately in
`candidates/BodySourceSectionContract_v1.1_Candidate.md`.

---

## Manifest Columns

Build manifest records are comma-separated.

| Column | Required | Meaning |
|--------|----------|---------|
| 1 | Yes | Module name |
| 2 | Yes | Module type |
| 3 | Yes | Layer name |
| 4 | Yes | Template path |
| 5 | No | BodySource path |
| 6+ | No | SectionSource entries in `SectionName=SectionSourcePath` form |

The four-column format remains valid:

```text
ExampleService,ClassModule,Application,..\..\templates\ClassTemplate.txt
```

---

## BodySource

Column 5 references a source file whose content is inserted into the template
`{{BODY}}` token.

```text
ExampleService,ClassModule,Application,..\..\templates\ClassTemplate.txt,body\ExampleService.body.cls
```

When BodySource is omitted, the generator keeps the existing behavior and
removes the `{{BODY}}` token.

---

## SectionSource

Columns 6 and later reference named template sections.

Each entry uses this form:

```text
SectionName=SectionSourcePath
```

The section name must match a template block declared with `@section` and
`@endsection`.

```text
ExampleService,ClassModule,Application,..\..\templates\ClassTemplate.txt,,PublicApi=body\ExampleService.PublicApi.cls
```

The empty fifth column is intentional when a record has section sources but no
BodySource.

Multiple sections may be provided by adding more columns:

```text
ExampleService,ClassModule,Application,..\..\templates\ClassTemplate.txt,,PublicApi=body\ExampleService.PublicApi.cls,PrivateProcedures=body\ExampleService.PrivateProcedures.cls
```

When a section source is omitted or empty, the matching section block is omitted
from generated output.

---

## Combined Body And Sections

BodySource and SectionSource entries may be used together.

```text
ExampleService,ClassModule,Application,..\..\templates\ClassTemplate.txt,body\ExampleService.body.cls,PublicApi=body\ExampleService.PublicApi.cls,PrivateProcedures=body\ExampleService.PrivateProcedures.cls
```

BodySource owns the `{{BODY}}` insertion point. SectionSource entries own only
their matching named sections.

---

## Path Rules

Relative source paths are resolved from the manifest file directory.

Absolute source paths are preserved.

The manifest reader uses comma-separated fields. Paths and section entries
therefore must not contain commas.

---

## Source File Layout

Build-owned body source files are placed under `src/Build/BodySource`.

Use this default layout:

```text
src/Build/BodySource/<Layer>/<ModuleName>.body.txt
src/Build/BodySource/<Layer>/<ModuleName>.<SectionName>.txt
```

Manifest records under `src/Build` reference those files relative to the
manifest directory:

```text
AppValidationService,ClassModule,Application,..\..\templates\ClassTemplate.txt,BodySource\Application\AppValidationService.body.txt
```

BodySource and SectionSource files are fragments. They do not include VBA export
metadata, `Option Explicit`, or the standard template header. Those remain
template-owned.

Use `.txt` for Build-owned source fragments so VBA test import tooling does not
treat them as complete exported `.bas` or `.cls` modules.

---

## Responsibility Boundaries

Manifest records reference source files. They do not embed implementation
bodies directly.

Templates own source structure. BodySource and SectionSource files supply
generated source content only.

Generator mediates the manifest, source files, and template insertion points.
