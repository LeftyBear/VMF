# Manifest v2.0

**Document ID**: SPEC-MANIFEST-v2.0

**Version**: 2.0

**Status**: Stable

**Type**: Normative Specification

---

# 1. Purpose

This specification defines the normative Manifest governing the specification set of the Build.xlam Project.

The Manifest serves as the authoritative registry of all normative specifications that constitute the project documentation.

Its primary objectives are to:

- establish a single authoritative catalog of specifications;
- define document identity and lifecycle;
- maintain hierarchical consistency across specifications;
- preserve traceability between specifications;
- support long-term maintenance of the specification system; and
- ensure that every normative specification can be uniquely identified, referenced, reviewed, approved, and maintained.

The Manifest SHALL function as the Single Source of Truth (SSOT) for specification registration within the Build.xlam Project.

---

# 2. Scope

This specification applies to all normative specifications published as part of the Build.xlam Project.

It governs:

- specification registration;
- document identification;
- document classification;
- document status;
- version management;
- document relationships;
- dependency management;
- publication management;
- approval management;
- revision management;
- maintenance governance; and
- specification registry consistency.

This specification does not define:

- implementation details;
- VBA source code;
- API specifications;
- project directory structures;
- naming conventions;
- development procedures; or
- implementation workflows.

Those subjects SHALL be defined by their respective specifications.

---

# 3. Normative References

The following specifications are normative references.

- Canon v2.0
- Architecture v2.0
- SpecificationHierarchy v2.0
- Glossary v2.0
- DocumentTemplate v2.0

If any conflict exists, the precedence SHALL follow SpecificationHierarchy v2.0.

---

# 4. Conformance

A specification conforms to this Manifest only if all of the following conditions are satisfied.

- It possesses a unique specification identifier.
- It is registered within the Manifest.
- Its document classification is defined.
- Its document status is defined.
- Its parent-child relationships comply with SpecificationHierarchy.
- Its normative references are declared where applicable.
- Its version follows the Version Rules defined by this specification.
- Its publication status has been approved according to the Approval Rules.
- Its revision history is maintained.
- Its lifecycle is managed according to this Manifest.

A specification that violates any mandatory requirement defined herein SHALL NOT be regarded as a conforming specification.
# 5. Manifest Overview

## 5.1 General

The Manifest defines the authoritative registry of all normative specifications within the Build.xlam Project.

It provides a unified view of the specification system and establishes the governance required to maintain consistency, traceability, and long-term maintainability.

The Manifest SHALL be the sole normative registry for specification management.

No other document SHALL supersede or duplicate the registry defined by this specification.

---

## 5.2 Objectives

The Manifest SHALL provide the following capabilities.

- Identification of every normative specification.
- Classification of specification roles.
- Definition of document status.
- Management of specification versions.
- Representation of parent-child relationships.
- Representation of normative dependencies.
- Control of normative references.
- Publication governance.
- Approval governance.
- Revision governance.
- Long-term maintenance governance.

---

## 5.3 Single Source of Truth

The Manifest SHALL operate as the Single Source of Truth (SSOT) for specification registration.

Every normative specification SHALL appear exactly once within the Manifest.

Duplicate registrations SHALL NOT exist.

Information maintained by the Manifest SHALL be regarded as authoritative whenever inconsistencies arise between specifications.

---

## 5.4 Registry Principles

The Manifest SHALL maintain a complete and internally consistent registry.

Each registered specification SHALL include sufficient metadata to uniquely identify the document throughout its lifecycle.

The Manifest SHALL preserve the integrity of specification relationships without redefining technical content contained in individual specifications.

---

## 5.5 Registry Scope

The Manifest SHALL register normative specifications only.

The Manifest SHOULD exclude informative documents unless another normative specification explicitly requires their registration.

Supporting materials, examples, implementation notes, working drafts, temporary documents, and development artifacts SHALL NOT be treated as registered specifications unless formally approved.

---

## 5.6 Registry Consistency

Every registered specification SHALL satisfy all applicable requirements defined by:

- Canon v2.0;
- Architecture v2.0;
- SpecificationHierarchy v2.0;
- Glossary v2.0;
- DocumentTemplate v2.0; and
- this Manifest.

Registration SHALL NOT alter the authority or scope of any specification.

The Manifest records specification metadata only.

---

## 5.7 Traceability

The Manifest SHALL preserve traceability throughout the specification hierarchy.

Traceability SHALL include, where applicable:

- specification identifier;
- parent specification;
- child specifications;
- normative references;
- document classification;
- document status;
- version information;
- approval status; and
- publication status.

Traceability SHALL remain stable across revisions unless formally modified through Revision Management.

---

## 5.8 Authority

The Manifest defines specification registration rules but SHALL NOT redefine technical requirements established by other normative specifications.

Where interpretation is required, authority SHALL remain with the governing specification identified by the Specification Hierarchy.

The Manifest SHALL therefore function as the authoritative registry, not as a replacement for normative technical specifications.
# 6. Specification Identifier

## 6.1 General

Every normative specification SHALL have a unique Specification Identifier (Specification ID).

The Specification ID provides a stable and unambiguous identity for a specification throughout its lifecycle.

A Specification ID SHALL remain unchanged regardless of document revisions unless the document is formally superseded by a new specification.

---

## 6.2 Purpose

The Specification Identifier SHALL support:

- unique identification;
- normative referencing;
- traceability;
- document lifecycle management;
- revision tracking;
- dependency management; and
- manifest registration.

The Specification Identifier SHALL NOT be reused for any different specification.

---

## 6.3 Uniqueness

Each Specification ID SHALL identify exactly one normative specification.

No two specifications SHALL share the same Specification ID.

Duplicate identifiers SHALL NOT exist within the Manifest.

---

## 6.4 Stability

Once assigned, a Specification ID SHALL remain stable for the lifetime of the specification.

Editorial revisions, corrections, formatting updates, and version increments SHALL NOT change the Specification ID.

A new Specification ID SHALL be assigned only when a specification is formally established as a distinct normative document.

---

## 6.5 Identification Rules

Every registered specification SHALL include a Specification ID.

The Specification ID SHALL:

- uniquely identify the specification;
- be suitable for normative references;
- remain human-readable;
- remain stable across revisions; and
- be recorded in the Manifest.

The Specification ID SHALL NOT encode implementation-specific information.

---

## 6.6 Manifest Registration

Each Specification ID SHALL appear exactly once in the Manifest.

The Manifest SHALL maintain a one-to-one relationship between a Specification ID and its registered specification.

Registration SHALL include the metadata necessary to distinguish the specification from all other registered specifications.

---

## 6.7 Reference Usage

Normative references SHOULD identify specifications by their Specification ID where unambiguous identification is required.

References using document titles MAY be used when ambiguity cannot occur.

Where both are provided, the Specification ID SHALL take precedence for identification purposes.

---

## 6.8 Lifecycle Continuity

The Specification Identifier SHALL preserve continuity across the specification lifecycle.

Version updates, status transitions, approvals, publications, and revisions SHALL be associated with the existing Specification ID.

Only formal replacement by a new normative specification MAY terminate the lifecycle associated with an existing Specification ID.
# 7. Document Classification

## 7.1 General

Every registered specification SHALL have a defined Document Classification.

The Document Classification identifies the role of a specification within the overall specification system and supports consistent governance, traceability, and lifecycle management.

Each specification SHALL belong to one primary classification only.

---

## 7.2 Purpose

Document Classification SHALL provide a consistent mechanism for:

- identifying the role of a specification;
- organizing the specification hierarchy;
- supporting dependency analysis;
- facilitating document discovery;
- enabling lifecycle management; and
- maintaining long-term consistency of the specification set.

Classification SHALL describe the purpose of a document rather than its implementation content.

---

## 7.3 Classification Principles

Document Classification SHALL be:

- unique for each specification;
- stable throughout the document lifecycle unless formally reclassified;
- independent of implementation details;
- compatible with the Specification Hierarchy; and
- recorded in the Manifest.

A classification SHALL NOT imply implementation ownership or development responsibility.

---

## 7.4 Classification Categories

The Manifest SHALL recognize document classifications appropriate to the specification system.

Typical classifications MAY include, where applicable:

- Governing Specification;
- Architecture Specification;
- Registry Specification;
- Vocabulary Specification;
- Template Specification;
- Technical Specification;
- Operational Specification; and
- Supporting Specification.

The exact classification assigned to each document SHALL be recorded in the Manifest.

---

## 7.5 Registration Requirements

Every registered specification SHALL include its Document Classification.

The recorded classification SHALL remain consistent with:

- the specification purpose;
- the specification hierarchy;
- normative references; and
- governing specifications.

Conflicting classifications SHALL NOT be assigned.

---

## 7.6 Relationship to Hierarchy

Document Classification SHALL complement, but SHALL NOT replace, the parent-child hierarchy defined by SpecificationHierarchy.

Hierarchy defines structural relationships.

Classification defines document roles.

The two concepts SHALL remain independent while maintaining consistency.

---

## 7.7 Classification Changes

A Document Classification SHOULD remain stable throughout the lifecycle of a specification.

If reclassification becomes necessary, it SHALL be performed through the formal revision process defined by this Manifest.

The Manifest SHALL record the updated classification while preserving revision traceability.

---

## 7.8 Conformance

A specification conforms to the Document Classification requirements only if:

- exactly one primary classification is assigned;
- the classification is registered in the Manifest;
- the classification is consistent with the specification purpose;
- the classification is compatible with the Specification Hierarchy; and
- no conflicting classifications are assigned.
# 8. Document Status

## 8.1 General

Every registered specification SHALL have a defined Document Status.

The Document Status indicates the current stage of the specification within its lifecycle and provides a consistent basis for publication, review, approval, and maintenance.

The current status of every registered specification SHALL be recorded in the Manifest.

---

## 8.2 Purpose

Document Status SHALL provide a consistent mechanism for:

- identifying the maturity of a specification;
- controlling publication;
- managing document reviews;
- supporting approval decisions;
- tracking lifecycle progression; and
- maintaining traceability across revisions.

Document Status SHALL represent lifecycle state only and SHALL NOT indicate implementation completeness.

---

## 8.3 Status Principles

A Document Status SHALL:

- be uniquely defined for each specification;
- accurately represent the current lifecycle stage;
- be maintained in the Manifest;
- change only through the defined governance process; and
- remain consistent with the associated revision history.

Only one active status SHALL apply to a specification at any given time.

---

## 8.4 Status Categories

The Manifest SHALL recognize the following lifecycle statuses.

| Status | Description |
|---------|-------------|
| Draft | Initial working document under preparation. |
| Review | Document undergoing formal technical review. |
| Approved | Document formally accepted but not yet published. |
| Stable | Official published normative specification. |
| Deprecated | Specification retained for reference but scheduled for replacement or retirement. |
| Obsolete | Specification permanently withdrawn from active use. |

Additional statuses SHOULD NOT be introduced unless formally approved through Manifest governance.

---

## 8.5 Status Transition

A Document Status SHALL change only through the formal approval and revision process.

Status transitions SHOULD follow an orderly lifecycle.

Typical progression is:

Draft → Review → Approved → Stable

When applicable, a Stable specification MAY subsequently transition to:

- Deprecated; or
- Obsolete.

Status transitions SHALL preserve revision traceability.

---

## 8.6 Registration Requirements

Every registered specification SHALL include its current Document Status.

The recorded status SHALL be consistent with:

- publication state;
- approval state;
- revision history; and
- lifecycle governance.

The Manifest SHALL maintain the authoritative record of the current status.

---

## 8.7 Status Consistency

A specification SHALL NOT simultaneously possess multiple active statuses.

Historical statuses MAY be preserved through revision records but SHALL NOT replace the current status.

Any inconsistency between revision history and current status SHALL be resolved in favor of the formally approved status recorded in the Manifest.

---

## 8.8 Conformance

A specification conforms to the Document Status requirements only if:

- exactly one active status is assigned;
- the status is recorded in the Manifest;
- the status accurately reflects the document lifecycle;
- all status transitions follow the defined governance process; and
- historical traceability is preserved through Revision Management.
# 9. Version Rules

## 9.1 General

Every registered specification SHALL have an assigned version.

The version identifies the published state of a specification and provides a stable basis for reference, maintenance, and revision management throughout the document lifecycle.

The current version of every specification SHALL be recorded in the Manifest.

---

## 9.2 Purpose

Version management SHALL support:

- unique identification of published revisions;
- controlled evolution of specifications;
- compatibility assessment;
- lifecycle traceability;
- publication management; and
- long-term maintenance.

Version numbers SHALL identify document revisions only and SHALL NOT represent implementation versions.

---

## 9.3 Version Format

A specification version SHALL use the following format:

```
Major.Minor
```

Examples include:

- 1.0
- 1.1
- 2.0

Additional version components SHOULD NOT be introduced unless governed by a future revision of this specification.

---

## 9.4 Major Version

The Major version SHALL be incremented when a revision introduces one or more of the following:

- normative incompatibility;
- structural reorganization affecting conformance;
- replacement of governing requirements;
- changes requiring coordinated updates across dependent specifications.

A Major version increment establishes a new normative baseline.

---

## 9.5 Minor Version

The Minor version SHALL be incremented when a revision preserves normative compatibility while introducing one or more of the following:

- clarifications;
- additional explanatory material;
- editorial improvements;
- non-breaking normative refinements;
- maintenance updates.

A Minor version increment SHALL NOT invalidate conforming specifications.

---

## 9.6 Version Stability

The published version recorded in the Manifest SHALL represent the current authoritative version of the specification.

Only one current version SHALL be designated as active.

Previous versions MAY be retained for historical purposes but SHALL NOT supersede the current published version.

---

## 9.7 Registration Requirements

The Manifest SHALL record, for every registered specification:

- current version;
- current document status;
- approval state;
- publication state; and
- revision history reference.

Version information SHALL remain consistent with Revision Management.

---

## 9.8 Version References

Normative references SHOULD identify the applicable version where version-specific interpretation is required.

When no version is specified, the reference SHALL be interpreted as referring to the current Stable version recorded in the Manifest.

Version references SHALL remain consistent with the Specification Identifier.

---

## 9.9 Conformance

A specification conforms to the Version Rules only if:

- a valid version is assigned;
- the version format complies with this specification;
- the current version is recorded in the Manifest;
- version progression follows the defined rules; and
- revision history preserves traceability across version changes.
# 10. Manifest Table

## 10.1 General

The Manifest Table is the authoritative registry of all normative specifications within the Build.xlam Project.

It SHALL contain one entry for every registered normative specification and SHALL serve as the primary reference for specification identification, classification, status, and governance.

The Manifest Table SHALL be maintained as the Single Source of Truth (SSOT) for specification registration.

---

## 10.2 Registry Requirements

Each normative specification SHALL appear exactly once in the Manifest Table.

Every entry SHALL be complete, internally consistent, and uniquely identifiable.

Duplicate entries SHALL NOT exist.

Entries SHALL be maintained in accordance with this Manifest and the governing specifications referenced herein.

---

## 10.3 Required Metadata

Each Manifest entry SHALL include, at a minimum, the following metadata:

| Field | Requirement |
|--------|-------------|
| Specification Identifier | SHALL be unique. |
| Document Title | SHALL identify the specification. |
| Version | SHALL comply with the Version Rules. |
| Document Classification | SHALL comply with the Document Classification requirements. |
| Document Status | SHALL comply with the Document Status requirements. |
| Parent Specification | SHALL comply with the Specification Hierarchy. |
| Governing Specification | SHALL identify the immediate governing specification where applicable. |

Additional metadata MAY be recorded provided that it does not conflict with this Manifest.

---

## 10.4 Manifest Registry

The Manifest SHALL register all normative specifications composing the Build.xlam Project.

The initial registry is defined as follows.

| Specification ID | Document | Classification | Status | Version | Parent |
|------------------|----------|----------------|--------|---------|--------|
| SPEC-CANON-v2.0 | Canon v2.0 | Governing Specification | Stable | 2.0 | — |
| SPEC-ARCH-v2.0 | Architecture v2.0 | Architecture Specification | Stable | 2.0 | Canon v2.0 |
| SPEC-HIERARCHY-v2.0 | SpecificationHierarchy v2.0 | Registry Specification | Stable | 2.0 | Canon v2.0 |
| SPEC-GLOSSARY-v2.0 | Glossary v2.0 | Vocabulary Specification | Stable | 2.0 | Canon v2.0 |
| SPEC-TEMPLATE-v2.0 | DocumentTemplate v2.0 | Template Specification | Stable | 2.0 | Canon v2.0 |
| SPEC-MANIFEST-v2.0 | Manifest v2.0 | Registry Specification | Stable | 2.0 | Canon v2.0 |

Future normative specifications SHALL be added through the governance process defined by this Manifest.

---

## 10.5 Registry Integrity

The Manifest Table SHALL remain internally consistent.

For every registered specification:

- the Specification Identifier SHALL be unique;
- the registered version SHALL match the published version;
- the document classification SHALL be valid;
- the document status SHALL be current;
- the parent relationship SHALL conform to the Specification Hierarchy; and
- governing references SHALL remain valid.

Any inconsistency SHALL be resolved through the Revision Management process.

---

## 10.6 Registry Authority

The Manifest Table SHALL be regarded as the authoritative registry of normative specifications.

If discrepancies exist between the Manifest Table and secondary references, the Manifest Table SHALL take precedence unless superseded by a formally approved revision.

The Manifest Table SHALL NOT redefine technical requirements contained within registered specifications.

---

## 10.7 Registry Maintenance

Updates to the Manifest Table SHALL occur only when one or more of the following events occur:

- registration of a new normative specification;
- approval of a new specification version;
- document status transition;
- formal reclassification;
- revision affecting registry metadata; or
- retirement of a registered specification.

Every update SHALL preserve registry integrity and traceability.

---

## 10.8 Conformance

A Manifest Table conforms to this specification only if:

- every normative specification is registered exactly once;
- all required metadata are present;
- registry entries are internally consistent;
- parent relationships comply with the Specification Hierarchy;
- version and status information are current; and
- the Manifest Table remains the authoritative specification registry.
# 11. Parent / Child Relationship

## 11.1 General

The Parent / Child Relationship defines the normative hierarchical structure of the specification system.

The hierarchy establishes governance relationships between specifications and ensures that every normative specification has a well-defined position within the specification architecture.

The hierarchy SHALL remain consistent with SpecificationHierarchy v2.0.

---

## 11.2 Purpose

The Parent / Child Relationship SHALL support:

- hierarchical governance;
- inheritance of governing principles;
- structural consistency;
- traceability;
- dependency analysis; and
- long-term maintainability.

Hierarchical relationships SHALL describe governance only and SHALL NOT imply implementation dependencies.

---

## 11.3 Hierarchy Principles

The specification hierarchy SHALL satisfy the following principles.

- Every specification SHALL occupy a unique position within the hierarchy.
- A Child specification SHALL have at most one direct Parent specification.
- Parent relationships SHALL be explicitly recorded in the Manifest.
- Cyclic parent-child relationships SHALL NOT exist.
- The hierarchy SHALL form a directed acyclic structure.

---

## 11.4 Parent Responsibilities

A Parent specification SHALL:

- define governing requirements applicable to its Children;
- establish normative constraints where appropriate;
- maintain consistency across subordinate specifications; and
- preserve architectural integrity.

A Parent specification SHALL NOT redefine responsibilities delegated to subordinate specifications unless explicitly revised.

---

## 11.5 Child Responsibilities

A Child specification SHALL:

- conform to all applicable governing specifications;
- remain consistent with its Parent specification;
- define only the scope delegated to it;
- avoid contradicting inherited normative requirements; and
- maintain compatibility with the overall specification hierarchy.

A Child specification MAY introduce additional requirements provided they do not conflict with governing specifications.

---

## 11.6 Hierarchical Consistency

Changes affecting Parent / Child relationships SHALL be performed only through formal Revision Management.

Any modification SHALL preserve:

- structural integrity;
- traceability;
- governance consistency; and
- compatibility with the Specification Hierarchy.

Orphan specifications SHALL NOT exist within the registered hierarchy unless explicitly designated as root specifications.

---

## 11.7 Root Specification

The governing root specification SHALL be defined by the Specification Hierarchy.

The root specification SHALL have no Parent specification.

All other registered normative specifications SHALL ultimately derive their governance from the root through the established hierarchy.

---

## 11.8 Manifest Registration

The Manifest SHALL record, for every registered specification:

- Parent specification;
- Child relationships, where applicable; and
- hierarchical position.

The Manifest SHALL maintain sufficient information to reconstruct the complete specification hierarchy.

---

## 11.9 Conformance

A specification hierarchy conforms to this Manifest only if:

- every non-root specification has exactly one direct Parent;
- no cyclic relationships exist;
- hierarchical relationships are recorded in the Manifest;
- governance remains consistent across the hierarchy; and
- all Parent / Child relationships comply with SpecificationHierarchy v2.0.
# 12. Dependency Rules

## 12.1 General

Dependency Rules define the normative relationships between specifications that rely upon one another for governance, interpretation, or conformance.

Dependencies SHALL preserve the integrity of the specification system while maintaining clear separation of responsibilities.

A dependency represents a normative relationship and SHALL NOT be interpreted as an implementation dependency.

---

## 12.2 Purpose

Dependency Rules SHALL support:

- consistent interpretation of specifications;
- controlled evolution of the specification system;
- impact analysis of revisions;
- preservation of architectural integrity;
- traceability across specifications; and
- long-term maintainability.

---

## 12.3 Dependency Principles

All specification dependencies SHALL satisfy the following principles.

- Dependencies SHALL be explicit.
- Dependencies SHALL be recorded in the Manifest where applicable.
- Dependencies SHALL be unidirectional.
- Cyclic dependencies SHALL NOT exist.
- Dependencies SHALL remain consistent with the Specification Hierarchy.

The dependency graph SHALL form a directed acyclic graph (DAG).

---

## 12.4 Governing Dependencies

A specification MAY depend upon one or more governing specifications.

Such dependencies SHALL indicate that:

- governing requirements apply;
- normative interpretation is inherited;
- conformance obligations are inherited; and
- conflicting requirements are resolved according to the Specification Hierarchy.

A governing dependency SHALL NOT transfer implementation responsibility.

---

## 12.5 Dependency Consistency

A specification SHALL NOT declare dependencies that contradict:

- Canon v2.0;
- Architecture v2.0;
- SpecificationHierarchy v2.0;
- this Manifest; or
- any governing specification applicable to the dependent specification.

Conflicting dependencies SHALL be resolved through Revision Management.

---

## 12.6 Dependency Registration

Where normative dependencies exist, the Manifest SHALL record sufficient metadata to identify the dependency relationship.

Recorded dependency information SHOULD enable:

- dependency tracing;
- impact assessment;
- conformance verification; and
- lifecycle management.

---

## 12.7 Dependency Changes

Dependencies SHOULD remain stable throughout the lifecycle of a specification.

If a dependency changes, the modification SHALL:

- follow the formal revision process;
- preserve traceability;
- maintain hierarchical consistency; and
- avoid introducing dependency cycles.

---

## 12.8 Dependency Resolution

Where multiple governing specifications are applicable, precedence SHALL follow the order defined by SpecificationHierarchy v2.0.

The Manifest SHALL NOT establish an alternative precedence order.

Interpretation SHALL always follow the governing hierarchy.

---

## 12.9 Conformance

A specification conforms to the Dependency Rules only if:

- all dependencies are explicitly defined where required;
- dependency relationships are unidirectional;
- cyclic dependencies do not exist;
- dependencies remain consistent with the Specification Hierarchy;
- dependency information is accurately maintained in the Manifest; and
- dependency changes follow the defined governance process.
# 13. Reference Rules

## 13.1 General

Reference Rules define how specifications reference one another within the Build.xlam Project.

References SHALL preserve consistency, traceability, and clarity while preventing ambiguity in the interpretation of normative requirements.

A reference identifies an authoritative specification and SHALL NOT duplicate or replace the referenced content.

---

## 13.2 Purpose

Reference Rules SHALL support:

- unambiguous specification identification;
- normative traceability;
- consistent interpretation of requirements;
- controlled document relationships;
- maintenance of the specification hierarchy; and
- long-term documentation integrity.

---

## 13.3 Reference Principles

All references SHALL satisfy the following principles.

- References SHALL identify the intended specification unambiguously.
- References SHALL remain consistent with the Manifest.
- References SHALL NOT create conflicting interpretations.
- References SHALL NOT introduce circular governance.
- References SHALL preserve the Single Source of Truth (SSOT).

Normative requirements SHALL exist in one authoritative location only.

---

## 13.4 Normative References

A normative reference indicates that conformance depends upon the referenced specification.

Normative references SHALL:

- identify the referenced specification;
- remain consistent with the Specification Hierarchy;
- reference the current applicable version unless another version is explicitly identified; and
- be maintained throughout the specification lifecycle.

---

## 13.5 Informative References

Informative references MAY be used to provide additional context or explanatory material.

Informative references SHALL NOT introduce normative requirements.

The absence of an informative reference SHALL NOT affect conformance.

---

## 13.6 Reference Integrity

Every normative reference SHALL resolve to a registered specification recorded in the Manifest.

Broken, ambiguous, or obsolete references SHALL NOT remain unresolved in a published Stable specification.

Reference integrity SHALL be verified during document review.

---

## 13.7 Version References

Where interpretation depends upon a specific revision, the referenced version SHALL be identified explicitly.

When no version is specified, the reference SHALL be interpreted as referring to the current Stable version recorded in the Manifest.

Version references SHALL remain consistent with the registered Specification Identifier.

---

## 13.8 Reference Maintenance

References SHALL be reviewed whenever:

- a referenced specification is revised;
- a specification changes status;
- a specification is reclassified;
- a specification is deprecated; or
- a specification becomes obsolete.

Necessary updates SHALL be performed through Revision Management.

---

## 13.9 Conformance

A specification conforms to the Reference Rules only if:

- all normative references are valid;
- references uniquely identify the intended specifications;
- reference integrity is maintained;
- references remain consistent with the Manifest and Specification Hierarchy; and
- no conflicting or unresolved normative references exist.
# 14. Publication Rules

## 14.1 General

Publication Rules define the normative requirements governing the publication of specifications within the Build.xlam Project.

Publication establishes a specification as an officially released document and makes it available as part of the authoritative specification set.

Only specifications satisfying the applicable governance requirements SHALL be published.

---

## 14.2 Purpose

Publication Rules SHALL support:

- controlled release of specifications;
- preservation of specification integrity;
- consistent public availability;
- traceability of published revisions;
- lifecycle governance; and
- long-term maintainability.

Publication SHALL represent an administrative process and SHALL NOT modify the normative content of a specification.

---

## 14.3 Publication Eligibility

A specification SHALL satisfy all of the following conditions before publication.

- The specification is registered in the Manifest.
- A unique Specification Identifier has been assigned.
- A valid version has been assigned.
- A Document Classification has been assigned.
- A current Document Status has been assigned.
- Applicable reviews have been completed.
- Required approvals have been obtained.
- All normative references are valid.
- Manifest metadata are complete and internally consistent.

A specification failing any mandatory requirement SHALL NOT be published.

---

## 14.4 Published State

A published specification SHALL:

- be uniquely identifiable;
- remain accessible within the specification set;
- preserve revision traceability;
- maintain reference integrity; and
- remain consistent with the Manifest.

Publication SHALL establish the specification as part of the official normative documentation.

---

## 14.5 Publication Updates

Publication of a revised specification SHALL:

- preserve the Specification Identifier;
- assign the appropriate version;
- update the Manifest;
- maintain revision history; and
- preserve traceability to previous published revisions.

Only the current published version SHALL be regarded as authoritative unless otherwise specified.

---

## 14.6 Publication Consistency

Publication SHALL NOT introduce inconsistencies between:

- the Manifest;
- the Specification Hierarchy;
- governing specifications;
- registered metadata; or
- normative references.

Any inconsistency SHALL be resolved before publication.

---

## 14.7 Publication Withdrawal

A published specification MAY be withdrawn only through the formal governance process.

Withdrawal SHALL:

- preserve historical traceability;
- update the Document Status appropriately;
- maintain Manifest integrity; and
- avoid invalidating the traceability of dependent specifications.

Withdrawn specifications SHALL NOT be removed from historical revision records.

---

## 14.8 Conformance

A publication conforms to these Publication Rules only if:

- all publication eligibility requirements are satisfied;
- publication metadata are complete;
- the Manifest has been updated;
- revision traceability is preserved;
- reference integrity is maintained; and
- the published specification is consistent with all governing specifications.
# 15. Approval Rules

## 15.1 General

Approval Rules define the normative requirements for formally accepting specifications within the Build.xlam Project.

Approval establishes that a specification has successfully completed the required governance process and is eligible for publication or continued maintenance.

Approval SHALL apply to the specification as a whole and SHALL NOT imply approval of any implementation.

---

## 15.2 Purpose

Approval Rules SHALL support:

- formal acceptance of normative specifications;
- consistency of the specification system;
- governance accountability;
- controlled lifecycle progression;
- publication readiness; and
- long-term maintainability.

---

## 15.3 Approval Eligibility

A specification SHALL satisfy all of the following conditions before approval.

- The specification is registered in the Manifest.
- The specification has a valid Specification Identifier.
- The assigned Document Classification is appropriate.
- The assigned Document Status reflects its current lifecycle stage.
- All mandatory reviews have been completed.
- Normative references have been verified.
- Manifest metadata are complete and consistent.
- Applicable governing specifications are satisfied.

A specification SHALL NOT be approved if any mandatory requirement remains unsatisfied.

---

## 15.4 Approval Scope

Approval SHALL confirm that:

- the specification is internally consistent;
- governance requirements are satisfied;
- normative references are valid;
- document metadata are correct;
- revision records are maintained; and
- the specification is suitable for its declared lifecycle stage.

Approval SHALL NOT certify implementation correctness, software quality, or operational behavior.

---

## 15.5 Approval Authority

Approval SHALL be performed according to the governance model established for the Build.xlam Project.

The Manifest defines approval requirements but SHALL NOT prescribe organizational roles, responsibilities, or decision-making procedures.

Such procedures MAY be defined by separate governing documentation where applicable.

---

## 15.6 Approval Record

The approval state of every registered specification SHALL be reflected through the Manifest and its associated lifecycle information.

Approval records SHALL preserve:

- specification identity;
- approved version;
- applicable lifecycle status; and
- revision traceability.

Historical approval information SHALL remain available through Revision Management.

---

## 15.7 Approval Changes

Any modification affecting an approved specification SHALL follow the formal revision process.

Where a revision changes normative requirements, renewed approval SHALL be obtained before the revised specification is published.

Editorial revisions that do not affect approval eligibility MAY follow the versioning rules defined by this Manifest.

---

## 15.8 Conformance

A specification conforms to the Approval Rules only if:

- approval eligibility requirements have been satisfied;
- approval has been completed through the defined governance process;
- approval information is reflected in the Manifest;
- revision traceability is preserved; and
- no approved specification conflicts with governing specifications.
# 16. Revision Management

## 16.1 General

Revision Management defines the normative process for controlling changes to registered specifications throughout their lifecycle.

All revisions SHALL preserve the integrity, traceability, and consistency of the specification system.

Every revision SHALL be reflected in the Manifest.

---

## 16.2 Purpose

Revision Management SHALL support:

- controlled evolution of specifications;
- preservation of historical traceability;
- consistency across the specification hierarchy;
- coordinated maintenance of dependent specifications;
- lifecycle governance; and
- long-term maintainability.

Revisions SHALL improve or maintain the quality of the specification set without compromising normative consistency.

---

## 16.3 Revision Principles

All revisions SHALL satisfy the following principles.

- Every revision SHALL be documented.
- Revision history SHALL be preserved.
- Traceability SHALL be maintained.
- Specification Identifiers SHALL remain stable.
- Version progression SHALL comply with the Version Rules.
- Revisions SHALL remain consistent with governing specifications.

Normative changes SHALL NOT be introduced outside the formal revision process.

---

## 16.4 Types of Revisions

Revisions MAY include:

- editorial corrections;
- clarifications;
- maintenance updates;
- normative refinements;
- structural reorganizations;
- compatibility-preserving improvements; and
- compatibility-affecting revisions.

The applicable version increment SHALL comply with the Version Rules.

---

## 16.5 Revision Impact

Before approval of a revision, the impact on the following SHALL be evaluated where applicable:

- parent specifications;
- child specifications;
- dependent specifications;
- normative references;
- document classification;
- document status;
- Manifest metadata; and
- overall specification consistency.

Identified impacts SHALL be addressed prior to publication.

---

## 16.6 Revision Traceability

Revision history SHALL preserve sufficient information to reconstruct the lifecycle of every registered specification.

Traceability SHALL include, where applicable:

- Specification Identifier;
- version;
- document status;
- approval state;
- publication state; and
- revision sequence.

Historical information SHALL remain available after subsequent revisions.

---

## 16.7 Manifest Updates

Every approved revision SHALL update the Manifest where necessary.

Manifest updates SHALL maintain:

- registry integrity;
- version consistency;
- reference integrity;
- hierarchical consistency; and
- lifecycle traceability.

The Manifest SHALL always reflect the current authoritative state of registered specifications.

---

## 16.8 Revision Consistency

A revision SHALL NOT introduce inconsistencies between:

- governing specifications;
- the Specification Hierarchy;
- registered metadata;
- normative references; or
- the Manifest.

Any inconsistency SHALL be resolved before the revised specification is approved or published.

---

## 16.9 Conformance

A specification conforms to the Revision Management requirements only if:

- revisions follow the defined governance process;
- revision history is preserved;
- version progression complies with the Version Rules;
- Manifest information is updated appropriately;
- traceability is maintained; and
- the revised specification remains consistent with all governing specifications.
# 17. Manifest Maintenance

## 17.1 General

Manifest Maintenance defines the normative requirements for maintaining the Manifest as the authoritative registry of specifications throughout the lifecycle of the Build.xlam Project.

The Manifest SHALL remain accurate, complete, internally consistent, and aligned with all governing specifications.

Maintenance activities SHALL preserve the Manifest as the Single Source of Truth (SSOT) for specification registration.

---

## 17.2 Purpose

Manifest Maintenance SHALL support:

- continuous accuracy of the specification registry;
- preservation of registry integrity;
- consistency across governing specifications;
- reliable traceability;
- controlled evolution of the specification set; and
- long-term sustainability of documentation.

Maintenance SHALL ensure that the Manifest remains authoritative at all times.

---

## 17.3 Maintenance Principles

Maintenance SHALL adhere to the following principles.

- The Manifest SHALL accurately reflect the current authoritative specification set.
- Registry information SHALL remain internally consistent.
- Historical traceability SHALL be preserved.
- Changes SHALL be governed through Revision Management.
- Duplicate registry entries SHALL NOT exist.
- Inactive specifications SHALL remain traceable where applicable.

Routine maintenance SHALL NOT alter normative intent unless performed through an approved revision.

---

## 17.4 Maintenance Events

The Manifest SHALL be reviewed and updated whenever one or more of the following events occur:

- registration of a new normative specification;
- publication of a new version;
- change of Document Status;
- modification of a parent-child relationship;
- addition or removal of a normative dependency;
- approval of a specification revision;
- deprecation of a specification; or
- retirement of a specification.

All updates SHALL preserve registry integrity.

---

## 17.5 Registry Validation

Maintenance activities SHOULD include validation of:

- Specification Identifiers;
- version information;
- document classifications;
- document statuses;
- parent-child relationships;
- dependency information;
- normative references; and
- revision traceability.

Detected inconsistencies SHALL be resolved before publication of Manifest updates.

---

## 17.6 Maintenance Records

Maintenance actions SHALL preserve sufficient records to demonstrate:

- what changed;
- when the change became effective;
- which specification was affected; and
- the resulting authoritative registry state.

Maintenance records SHALL support historical reconstruction of the Manifest where required.

---

## 17.7 Consistency Verification

The Manifest SHALL be periodically verified for consistency with:

- Canon v2.0;
- Architecture v2.0;
- SpecificationHierarchy v2.0;
- Glossary v2.0;
- DocumentTemplate v2.0; and
- all registered normative specifications.

No published Manifest SHALL knowingly contain unresolved registry inconsistencies.

---

## 17.8 Conformance

The Manifest conforms to the Maintenance requirements only if:

- registry information is current;
- maintenance activities preserve registry integrity;
- traceability is maintained;
- updates follow Revision Management;
- consistency with governing specifications is preserved; and
- the Manifest continues to function as the Single Source of Truth for specification registration.
# 18. Review Checklist

## 18.1 General

The Review Checklist defines the minimum verification activities required before approving or publishing the Manifest and any registered normative specification.

The checklist supports systematic quality assurance and ensures that the specification registry remains accurate, consistent, and maintainable.

Completion of the checklist SHALL be part of the formal review process.

---

## 18.2 Purpose

The Review Checklist SHALL verify that:

- the Manifest remains internally consistent;
- registered specifications satisfy applicable governance requirements;
- normative references are valid;
- registry metadata are complete;
- traceability is preserved; and
- publication readiness has been achieved.

The checklist SHALL evaluate specification quality rather than implementation quality.

---

## 18.3 Review Scope

The review SHOULD examine, where applicable:

- Specification Identifier;
- document title;
- version;
- document classification;
- document status;
- parent-child relationships;
- dependency information;
- normative references;
- revision history;
- Manifest consistency; and
- conformance with governing specifications.

Additional review items MAY be included provided they do not conflict with this Manifest.

---

## 18.4 Mandatory Checklist

Before approval or publication, reviewers SHALL confirm the following.

| Item | Verification |
|------|--------------|
| Specification Identifier | Unique and correctly registered |
| Manifest Registration | Present and complete |
| Version | Complies with Version Rules |
| Document Classification | Correctly assigned |
| Document Status | Correctly assigned |
| Parent Relationship | Consistent with SpecificationHierarchy |
| Dependency Information | Valid and consistent |
| Normative References | Valid and current |
| Revision History | Preserved and consistent |
| Manifest Consistency | No registry conflicts |
| SSOT Compliance | No duplicated authoritative information |
| RFC2119 Usage | Correct and consistent |
| Markdown Quality | GitHub-compatible and valid UTF-8 |

Every mandatory item SHALL be verified before publication.

---

## 18.5 Review Outcomes

A review MAY produce one of the following outcomes:

- Accepted;
- Accepted with Required Revisions;
- Re-review Required; or
- Rejected.

Specifications requiring further revisions SHALL NOT proceed to publication until all mandatory issues have been resolved.

---

## 18.6 Review Records

Review activities SHOULD preserve sufficient information to demonstrate:

- the specification reviewed;
- the reviewed version;
- the review outcome;
- identified issues; and
- completion of required corrective actions.

Review records SHALL support lifecycle traceability.

---

## 18.7 Review Consistency

The review SHALL verify consistency with:

- Canon v2.0;
- Architecture v2.0;
- SpecificationHierarchy v2.0;
- Glossary v2.0;
- DocumentTemplate v2.0; and
- this Manifest.

No known inconsistency SHALL remain unresolved before approval.

---

## 18.8 Conformance

A review conforms to this specification only if:

- all mandatory checklist items have been verified;
- review outcomes have been recorded;
- unresolved mandatory issues do not remain;
- registry consistency has been confirmed; and
- the reviewed specification satisfies all applicable governing specifications.
# 19. Manifest Governance

## 19.1 General

Manifest Governance defines the normative principles for governing the specification registry throughout the lifecycle of the Build.xlam Project.

The governance model ensures that the Manifest remains the authoritative registry of normative specifications and continues to operate as the Single Source of Truth (SSOT).

Governance SHALL preserve consistency, integrity, traceability, and long-term maintainability across the entire specification system.

---

## 19.2 Governance Objectives

Manifest Governance SHALL ensure:

- consistent administration of the specification registry;
- preservation of specification integrity;
- controlled evolution of the specification system;
- transparent lifecycle management;
- maintenance of normative traceability; and
- continued alignment with governing specifications.

Governance SHALL prioritize stability over unnecessary change.

---

## 19.3 Governance Principles

The following principles SHALL apply to all Manifest governance activities.

- The Manifest SHALL remain the authoritative specification registry.
- Every registered specification SHALL have one authoritative registry entry.
- Governance SHALL preserve the Specification Hierarchy.
- Governance SHALL preserve SSOT.
- Governance SHALL preserve revision traceability.
- Governance SHALL avoid unnecessary disruption to published specifications.

No governance activity SHALL introduce conflicting authoritative information.

---

## 19.4 Governance Authority

Governance authority SHALL derive from the governing specifications defined by the Specification Hierarchy.

This Manifest governs specification registration only.

Technical requirements SHALL remain governed by their respective normative specifications.

Where conflicts arise, precedence SHALL follow SpecificationHierarchy v2.0.

---

## 19.5 Governance Responsibilities

Manifest governance SHALL ensure that:

- registry metadata remain accurate;
- Specification Identifiers remain unique;
- document classifications remain consistent;
- document statuses remain current;
- version information remains accurate;
- parent-child relationships remain valid;
- normative references remain valid; and
- revision history remains complete.

Governance SHALL continuously preserve registry integrity.

---

## 19.6 Governance Changes

Changes affecting Manifest governance SHALL:

- follow Revision Management;
- preserve backward traceability where applicable;
- maintain compatibility with governing specifications; and
- avoid introducing registry inconsistencies.

Governance changes SHOULD be exceptional rather than routine.

---

## 19.7 Governance Verification

Governance activities SHOULD periodically verify that:

- the Manifest remains complete;
- all registered specifications remain valid;
- registry metadata remain internally consistent;
- no duplicate registrations exist;
- hierarchical consistency is preserved;
- dependency information remains accurate;
- normative references remain valid; and
- SSOT is preserved.

Detected inconsistencies SHALL be resolved through the defined governance process.

---

## 19.8 Conformance

The Manifest Governance requirements are satisfied only if:

- the Manifest remains the authoritative registry of normative specifications;
- governance activities preserve registry integrity;
- traceability is maintained;
- governing specifications remain authoritative;
- SSOT is preserved; and
- all governance changes follow the processes defined by this Manifest.

---

# Annex A (Informative) — Integrated Review

This annex is informative and does not define additional normative requirements.

The completed **Manifest v2.0** has been reviewed against the governing specifications and project requirements.

The review confirms that:

- the document conforms to **Canon v2.0**;
- the document is consistent with **Architecture v2.0**;
- the document complies with **SpecificationHierarchy v2.0**;
- terminology is consistent with **Glossary v2.0**;
- document structure complies with **DocumentTemplate v2.0**;
- RFC 2119 normative language is used consistently;
- the Manifest functions as the Single Source of Truth (SSOT) for specification registration;
- no implementation details, VBA code, API specifications, file structures, naming conventions, or development procedures are included;
- the document is suitable for long-term maintenance;
- the document is GitHub-compatible Markdown encoded in UTF-8.

Following this review, **Manifest v2.0** is considered complete and suitable for publication as a Stable normative specification within the Build.xlam Project.
