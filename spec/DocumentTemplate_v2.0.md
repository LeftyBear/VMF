# DocumentTemplate_v2.0

**Version:** 2.0  
**Status:** Frozen  
**Encoding:** UTF-8  
**Format:** Markdown (GitHub Flavored Markdown)  
**Normative Language:** RFC 2119  
**Classification:** Normative Specification

---

# 1. Purpose

## 1.1 Objective

This document defines the standard template used for all specification documents within the Build.xlam Project.

The objective of this template is to ensure that every specification document:

- follows a common structure;
- uses consistent terminology;
- provides uniform metadata;
- clearly distinguishes normative and informative content;
- complies with the Single Source of Truth (SSOT) principle; and
- remains maintainable over the long term.

This document specifies **how specifications are written**, not **what individual specifications define**.

## 1.2 Design Goals

The template SHALL provide:

- structural consistency;
- readability;
- maintainability;
- traceability;
- interoperability between specification documents; and
- predictable document organization.

Every specification SHALL conform to this template unless explicitly exempted by a higher-level normative specification.

## 1.3 Non-Goals

This document does **NOT** define:

- software architecture;
- implementation details;
- VBA source code;
- API specifications;
- module organization;
- file structure;
- naming conventions; or
- development workflow.

Those subjects are defined by their respective normative specifications.

---

# 2. Scope

This document applies to every normative specification contained within the Build.xlam Project, including but not limited to:

- architecture specifications;
- design specifications;
- interface specifications;
- behavioral specifications;
- data specifications;
- operational specifications; and
- development standards.

Informative documents MAY follow this template where appropriate but are not required to do so.

---

# 3. Normative References

The following documents constitute the normative foundation of this specification.

In the event of conflict, higher-level specifications SHALL take precedence according to the specification hierarchy.

1. Canon_v2.0.md
2. Architecture_v2.0.md
3. SpecificationHierarchy_v2.0.md
4. Glossary_v2.0.md

No requirement in this document SHALL contradict any higher-level normative specification.

---

# 4. Conformance

A specification document is considered conformant only if it satisfies all mandatory requirements defined in this document.

Conforming documents SHALL:

- follow the prescribed document structure;
- include the required document metadata;
- use the standardized section organization;
- comply with Markdown formatting rules;
- correctly apply RFC 2119 terminology;
- respect SSOT reference rules;
- distinguish normative and informative content; and
- maintain consistency with higher-level specifications.

Partial conformance SHALL NOT be claimed.

---

# 5. Terminology

Unless otherwise specified, all technical terms used in this document SHALL have the meanings defined in **Glossary_v2.0.md**.

Terms defined by higher-level specifications SHALL NOT be redefined within individual documents.

If a new term is required, it SHALL first be added to the project glossary before being referenced elsewhere.
# 6. Document Metadata

## 6.1 Required Metadata

Every normative specification SHALL begin with the following metadata.

| Field | Requirement | Description |
|-------|-------------|-------------|
| Title | MUST | Official document title |
| Version | MUST | Document version |
| Status | MUST | Draft / Review / Frozen / Deprecated |
| Encoding | MUST | UTF-8 |
| Format | MUST | Markdown (GitHub Flavored Markdown) |
| Normative Language | MUST | RFC 2119 |
| Classification | MUST | Normative or Informative |

Additional metadata MAY be added where appropriate, provided that it does not conflict with this specification.

---

## 6.2 Metadata Placement

Document metadata SHALL appear immediately after the document title.

No normative content SHALL precede the metadata section.

---

# 7. Standard Document Structure

Unless otherwise specified by a higher-level specification, a normative document SHOULD follow the structure below.

1. Purpose
2. Scope
3. Normative References
4. Conformance
5. Terminology
6. Main Body
7. Appendix (optional)
8. Revision History (optional)

Documents MAY omit sections that are not applicable.

Documents SHALL NOT reorder mandatory sections unless explicitly required by a higher-level specification.

---

# 8. Mandatory Sections

The following sections SHALL be included where applicable.

| Section | Requirement |
|----------|-------------|
| Purpose | MUST |
| Scope | MUST |
| Normative References | MUST |
| Conformance | SHOULD |
| Terminology | SHOULD |
| Main Body | MUST |

The Main Body SHALL contain the specification-specific content.

---

# 9. Optional Sections

The following sections MAY be included.

- Examples
- Notes
- Rationale
- Design Considerations
- Appendix
- Revision History
- References
- Change Log

Optional sections SHALL NOT introduce new normative requirements unless explicitly identified as normative.

---

# 10. Appendix Rules

Appendices MAY provide supplementary information that improves understanding of the specification.

Typical appendix content includes:

- examples;
- reference material;
- explanatory diagrams;
- implementation notes;
- historical information.

Appendices SHALL be considered informative unless explicitly stated otherwise.

Normative requirements SHALL appear within the main body of the specification and SHALL NOT be introduced solely within an appendix.
Code blocks SHALL be informative unless explicitly identified as normative.

---
# 11. Heading Level Rules

## 11.1 Heading Hierarchy

Specification documents SHALL use a consistent heading hierarchy.

| Level | Markdown Syntax | Usage |
|--------|-----------------|-------|
| Level 1 | `#` | Major section |
| Level 2 | `##` | Subsection |
| Level 3 | `###` | Topic |
| Level 4 | `####` | Detail |

Heading levels deeper than Level 4 SHOULD NOT be used.

---

## 11.2 Numbering

Headings SHOULD use hierarchical numbering.

Example:

```text
# 1. Purpose

## 1.1 Objective

### 1.1.1 Background
```

Numbering SHALL remain consistent throughout the document.

---

## 11.3 Heading Titles

Heading titles SHOULD:

- be concise;
- clearly describe the section;
- avoid abbreviations unless defined in the Glossary; and
- remain stable across document revisions whenever practical.

---

# 12. Markdown Rules

Specification documents SHALL conform to standard GitHub Flavored Markdown.

The following elements SHOULD be used consistently:

- headings;
- paragraphs;
- ordered lists;
- unordered lists;
- tables;
- fenced code blocks;
- inline code.

HTML embedded within Markdown SHOULD NOT be used unless Markdown cannot express the required structure.

---

# 13. Table Rules

Tables SHOULD be used when presenting structured information.

Typical use cases include:

- requirements;
- comparisons;
- classifications;
- metadata;
- parameter definitions.

Tables SHOULD:

- include a header row;
- use concise column names;
- maintain consistent formatting; and
- avoid excessively wide layouts.

---

# 14. List Rules

Lists SHOULD be used for sequential or grouped information.

Use:

- ordered lists for sequences, procedures, or priorities;
- unordered lists for unordered collections.

List nesting SHOULD remain shallow to preserve readability.

Deeply nested lists SHOULD be avoided.

---

# 15. Code Block Rules

Code blocks MAY be used to present:

- syntax examples;
- pseudocode;
- configuration examples;
- command examples;
- illustrative source code.

Code blocks SHALL be enclosed using fenced code block syntax.

Example:

````text
```vb
Public Sub Example()

End Sub
```
Code blocks SHALL be informative unless explicitly identified as normative.

---

# 16. Figure Rules

Figures and diagrams MAY be included where they improve comprehension.

Figures SHOULD:

- have a descriptive title or caption;
- be referenced from the surrounding text;
- remain consistent with the normative specification; and
- avoid duplicating information already defined elsewhere.

Normative requirements SHALL NOT be expressed exclusively by figures or diagrams.
# 17. RFC 2119 Usage Rules

## 17.1 Normative Keywords

Normative requirements SHALL be expressed using the keywords defined by RFC 2119.

The following keywords are permitted:

| Keyword | Meaning |
|----------|---------|
| MUST | Absolute requirement |
| MUST NOT | Absolute prohibition |
| SHOULD | Strong recommendation |
| SHOULD NOT | Recommendation against |
| MAY | Optional behavior |

Keywords other than those defined by RFC 2119 SHOULD NOT be used to express normative requirements.

---

## 17.2 Consistent Usage

RFC 2119 keywords SHALL:

- be written in uppercase;
- be used only to express normative requirements;
- not be used for explanatory or descriptive statements; and
- be applied consistently throughout the document.

---

# 18. Normative and Informative Content

## 18.1 Normative Content

Normative content defines mandatory project requirements.

Normative content SHALL:

- use RFC 2119 terminology where appropriate;
- define requirements, constraints, or obligations; and
- remain consistent with higher-level specifications.

---

## 18.2 Informative Content

Informative content provides explanatory or supplementary information.

Examples include:

- notes;
- rationale;
- examples;
- implementation guidance;
- historical background.

Informative content SHALL NOT introduce new normative requirements.

---

## 18.3 Separation

Normative and informative content SHOULD be clearly distinguishable.

Where ambiguity may arise, the document SHOULD explicitly identify informative sections.

---

# 19. SSOT Reference Rules

## 19.1 Single Source of Truth

Each requirement SHALL have a single authoritative source.

The same normative requirement SHALL NOT be duplicated across multiple specification documents.

---

## 19.2 Referencing Higher-Level Specifications

Lower-level specifications SHALL reference higher-level specifications instead of restating their requirements.

References SHOULD identify the authoritative document whenever practical.

---

## 19.3 Avoiding Duplication

Duplicated normative content SHOULD be replaced with references to the authoritative specification.

Editorial summaries MAY be provided for readability, provided they do not modify or reinterpret the original requirement.

---

# 20. Cross-Reference Rules

Cross-references SHOULD:

- reference the authoritative specification;
- remain stable across revisions whenever practical;
- avoid circular references; and
- clearly identify the referenced section when appropriate.

Broken or obsolete references SHALL be corrected as part of document maintenance.

---

# 21. Prohibited Descriptions

The following practices SHALL NOT be used in normative specifications:

- contradictory requirements;
- duplicated normative definitions;
- undefined terminology;
- ambiguous wording;
- implementation-specific instructions unrelated to the document scope;
- undocumented assumptions; and
- references to unofficial or non-authoritative sources as normative requirements.

Normative specifications SHOULD remain concise, precise, and internally consistent.
# 22. Revision Policy

## 22.1 Revision Principles

All revisions to specification documents SHALL preserve consistency with the project specification hierarchy.

Revisions SHALL:

- maintain consistency with higher-level specifications;
- preserve the Single Source of Truth (SSOT);
- avoid introducing conflicting requirements; and
- document significant normative changes.

Editorial improvements that do not alter normative meaning MAY be made without affecting document intent.

---

## 22.2 Change Classification

Document changes SHOULD be classified as one of the following:

| Change Type | Description |
|--------------|-------------|
| Editorial | Grammar, formatting, wording, or readability improvements without changing normative meaning |
| Clarification | Improves precision without modifying existing requirements |
| Functional | Adds, modifies, or removes normative requirements |
| Structural | Reorganizes document structure without changing normative intent |

Only Functional changes modify the normative behavior of the specification.

---

# 23. Versioning

Document versions SHOULD follow the project's versioning policy.

Major versions SHALL indicate normative or structural changes that may affect dependent specifications.

Minor versions SHOULD indicate backward-compatible improvements such as clarifications or editorial refinements.

Patch versions MAY be used for typo corrections and formatting improvements.

---

# 24. Backward Compatibility

Normative revisions SHOULD preserve backward compatibility whenever practical.

When backward compatibility cannot be maintained:

- the reason SHALL be documented;
- affected specifications SHALL be identified; and
- dependent documents SHALL be updated accordingly.

---

# 25. Review Checklist

Before publication, each specification SHOULD be reviewed to confirm that:

- all mandatory sections are present;
- metadata is complete and correct;
- RFC 2119 keywords are used appropriately;
- terminology is consistent with the Glossary;
- SSOT is preserved;
- cross-references are valid;
- Markdown renders correctly on GitHub;
- tables, lists, and code blocks follow this template; and
- no contradictory or duplicated normative requirements exist.

---

# 26. Authoring Checklist

Authors SHOULD verify that the document:

- conforms to this template;
- follows the specification hierarchy;
- references higher-level specifications where required;
- distinguishes normative and informative content;
- avoids implementation-specific descriptions outside its scope;
- remains internally consistent; and
- is suitable for long-term maintenance.

---

# 27. Document Completion

A specification document conforming to this template SHALL:

- satisfy all applicable requirements defined herein;
- remain consistent with all higher-level normative specifications;
- be suitable for publication in the Build.xlam Project repository; and
- serve as a stable, maintainable, and authoritative project specification.

Upon approval, the document status SHOULD be updated to **Frozen** until a formally approved revision is required.