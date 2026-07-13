# VMF v1.1 Detailed Adoption Review

Version : 0.1
Status  : Working Review
Scope   : VMF v1.1 initial candidate adoption review
Date    : 2026-07-13
Depends : docs/development/VMF_v1.1_CandidateReadinessAudit.md, docs/development/VMF_v1.1_CandidateScopeReview.md, candidates/VMFCandidates_v1.1.md, candidates/VMF_v1.1_Candidate.md, candidates/TemplateContract_v1.1_Candidate.md

---

# 1. Purpose

This document records a detailed adoption review for the initial VMF v1.1
Candidate package.

This is not a normative specification. It does not modify VMF v1.0, Canon
v2.0, Build v1.1, or any frozen specification.

---

# 2. Review Result

Status : Adoption review opened

The initial VMF v1.1 package is suitable for detailed adoption review, but no
candidate is adopted by this document.

The review separates adoption-ready governance and architecture items from
items that require additional verification criteria.

---

# 3. Adoption Review Package

| Candidate | Review Decision | Adoption Readiness | Notes |
|-----------|-----------------|--------------------|-------|
| V005 VMF v1.x Design Charter | Advance | High | Suitable as the first adoption unit because it clarifies governance without changing implementation behavior. |
| V006 Contract-Driven Generation Architecture | Advance | High | Suitable as the architecture foundation when implementation notes are separated from normative contract language. |
| V001 Native Core Layer Generation | Advance with criteria | Medium | Build v1.1 supports `Core` as a custom layer name, but VMF-level PASS criteria must be defined. |
| V007 Template Body And Section Contract | Advance with criteria | Medium | Current implementation supports `{{BODY}}` and empty section omission, but VMF-level verification criteria are still needed. |
| V008 Canonical Template Contract | Advance with staging | Medium | Candidate templates exist, but component types should be staged to avoid adopting UserForm, Interface, and Enum behavior before validation. |

---

# 4. Proposed Adoption Units

## 4.1 Governance Unit

Recommended contents:

- V005 VMF v1.x Design Charter
- VMF v1.0 freeze preservation rule
- Candidate-before-adoption workflow
- Contract-first design principle

Adoption risk : Low

Reason:

This unit records governance and does not require Build behavior changes.

## 4.2 Architecture Unit

Recommended contents:

- V006 Contract-Driven Generation Architecture
- Blueprint Contract role
- Manifest Contract role
- Template Contract role
- Generator mediator role
- one-way contract dependency rule

Adoption risk : Medium

Reason:

The architecture is already expressed in candidate documentation and partially
supported by Build v1.1, but future normative language should avoid embedding
Build-specific implementation details.

## 4.3 Core Layer Unit

Recommended contents:

- V001 Native Core Layer Generation
- `Core` as a valid VMF generation layer
- VMF-level PASS criteria for generated Core artifacts
- audit treatment for Core layer presence

Adoption risk : Medium

Reason:

Build v1.1 accepts `Core` as a valid custom layer, but VMF v1.1 still needs to
define what native Core generation means beyond layer-name acceptance.

## 4.4 Template Body Unit

Recommended contents:

- V007 Template Body And Section Contract
- `{{BODY}}` as the canonical body insertion point
- optional `@section` / `@endsection` handling
- empty body and empty section omission
- compatibility treatment for legacy token forms

Adoption risk : Medium

Reason:

Current templates and token replacement provide implementation evidence, but
VMF-level verification criteria should be written before adoption.

## 4.5 Canonical Template Unit

Recommended contents:

- V008 Canonical Template Contract
- Class Module Template
- Predeclared Class Module Template
- Standard Module Template

Deferred from this unit unless separately approved:

- Interface Template
- Enum Template
- UserForm Template

Adoption risk : Medium

Reason:

The minimal class and standard module set aligns with existing Build behavior.
Interface, Enum, and UserForm templates should remain staged until generation
and validation contracts are explicit.

---

# 5. Recommended Adoption Order

The recommended adoption order is:

1. Governance Unit
2. Architecture Unit
3. Template Body Unit
4. Canonical Template Unit, limited to class and standard module templates
5. Core Layer Unit

Rationale:

- Governance should be stable before contract details are adopted.
- Contract-driven architecture should frame the template and Core decisions.
- Template body behavior is narrower than the full canonical template set.
- Core generation should be reviewed after the generation contracts are stable.

---

# 6. Required Verification Criteria

Before any adoption decision, the following criteria should be defined:

| Area | Required Criteria |
|------|-------------------|
| Governance | VMF v1.0 remains frozen; candidate material is not treated as adopted until a future VMF version decision. |
| Architecture | Blueprint, Manifest, Template, Generator, and Generated Source responsibilities are separable and one-way. |
| Core Layer | A manifest with `Core` produces or audits Core-layer artifacts without treating Core as a Build v1.0.x layer change. |
| Body Insertion | A canonical template receives body content only through `{{BODY}}`; empty body placeholders are removed. |
| Section Omission | Empty optional sections are omitted completely, including section markers. |
| Template Selection | Manifest-selected templates match component type, template contract, and predeclared attribute expectations. |

---

# 7. Deferred Decisions

The following decisions remain deferred:

- Native YAML manifest reader adoption.
- Native Enum generation adoption.
- Interface template generation adoption.
- UserForm code-behind generation adoption.
- UserForm designer/control generation.
- BodySource and SectionSource resolution.
- Build v2.0 source-generator architecture.

---

# 8. Recommendation

Prepare a VMF v1.1 adoption checklist for the five adoption units above.

The checklist should require:

1. candidate isolation confirmation;
2. frozen specification confirmation;
3. architecture boundary confirmation;
4. Build v1.1 evidence mapping;
5. explicit PASS criteria for every adopted unit;
6. explicit deferred-item list.

---

# 9. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 0.1 | Working Review | Opens detailed adoption review and defines adoption units for initial VMF v1.1 scope |
