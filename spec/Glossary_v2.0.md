# Glossary v2.0

## 1. Purpose

This document defines the official terminology used throughout the Build.xlam Project.

The purpose of this glossary is to establish a Single Source of Truth (SSOT) for terminology so that all specifications, design documents, reviews, discussions, and future extensions use consistent definitions.

This glossary is normative unless explicitly stated otherwise.

---

## 2. Scope

This document applies to every normative document within the Build.xlam Project.

It defines common terminology used by, but not limited to, the following documents:

* Canon_v2.0.md
* Architecture_v2.0.md
* SpecificationHierarchy_v2.0.md

The glossary includes terminology related to:

* Project-wide concepts
* Architecture
* Specification hierarchy
* Document management
* Dependency management
* SSOT
* RFC2119 terminology
* Build.xlam-specific concepts

The following are explicitly outside the scope of this document:

* Implementation details
* VBA source code
* Class design
* API specifications
* File structure
* Naming conventions
* Development procedures

---

## 3. Normative References

The following documents are normative references for this glossary.

| Document                       | Description                                          |
| ------------------------------ | ---------------------------------------------------- |
| Canon_v2.0.md                  | Project constitution and highest-level specification |
| Architecture_v2.0.md           | System architecture specification                    |
| SpecificationHierarchy_v2.0.md | Specification hierarchy definition                   |

If any inconsistency is identified, the document precedence defined in SpecificationHierarchy_v2.0.md SHALL apply.

---

## 4. How to Read This Glossary

Unless otherwise stated, each glossary entry consists of the following sections.

### Definition

Provides the normative meaning of the term.

Definitions are intended to be unambiguous and stable across future revisions.

### Notes

Provides explanatory information, examples, relationships to other concepts, or implementation-independent clarification.

Notes are informative unless explicitly identified as normative.

---

The glossary is organized in alphabetical order.

When multiple terms appear to describe similar concepts, the definition provided by this document SHALL be considered the authoritative terminology for the Build.xlam Project.
## 5. Glossary

# A

## Architecture

### Definition

The logical structure of the Build.xlam Project that defines system layers, responsibilities, dependency relationships, and interaction rules.

### Notes

Architecture specifies structural principles rather than implementation details. The normative architecture is defined in *Architecture_v2.0.md*.

---

# B

## Build.xlam

### Definition

The development support add-in that serves as the build and project management environment for the Build.xlam Project.

### Notes

Build.xlam provides project-level capabilities while remaining independent from application-specific implementations. Its behavior and responsibilities are defined by the project specifications.

---

# C

## Canon

### Definition

The highest-level normative specification of the Build.xlam Project.

### Notes

The Canon defines fundamental design principles, architectural constraints, terminology policies, and governance rules. All subordinate specifications SHALL conform to the Canon.

---

## Compliance

### Definition

The state in which a document, design, implementation, or process conforms to all applicable normative specifications.

### Notes

Compliance is evaluated against the specification hierarchy defined by the project and is determined using the applicable normative requirements.

---

# D

## Dependency

### Definition

A relationship in which one component, specification, or artifact requires another to perform its defined responsibility.

### Notes

Dependencies SHALL follow the dependency rules defined by the architecture. Circular dependencies are prohibited unless explicitly permitted by a higher-level specification.

---

## Dependency Direction

### Definition

The permitted direction in which dependencies may exist between architectural elements.

### Notes

The Build.xlam Project adopts a one-way dependency model. Lower-level components SHALL NOT introduce dependencies that violate the architectural layering defined in *Architecture_v2.0.md*.

---

## Document

### Definition

A version-controlled specification, guideline, or reference maintained as part of the Build.xlam Project.

### Notes

Documents may be normative or informative. Their authority is determined by the specification hierarchy.

---

## Document Hierarchy

### Definition

The ordered relationship among project documents based on their normative authority.

### Notes

The hierarchy is defined by *SpecificationHierarchy_v2.0.md*. Higher-level documents take precedence over lower-level documents in the event of inconsistency.

---

## Document Owner

### Definition

The authority responsible for maintaining the accuracy, consistency, and lifecycle of a document.

### Notes

Ownership refers to governance responsibility and does not imply implementation responsibility.
## 5. Glossary (continued)

# E

## Extension

### Definition

A capability, specification, or component that adds functionality without modifying existing normative behavior.

### Notes

Extensions SHALL remain consistent with the Canon and SHALL NOT redefine existing normative concepts.

---

# G

## Glossary

### Definition

The authoritative collection of project terminology and definitions.

### Notes

This document serves as the Single Source of Truth (SSOT) for terminology used throughout the Build.xlam Project.

---

# H

## Hierarchy

### Definition

The ordered relationship between documents, architectural elements, or responsibilities based on defined precedence.

### Notes

Hierarchy establishes authority and determines which definition or requirement prevails when conflicts occur.

---

# I

## Informative

### Definition

Content that provides explanation, guidance, examples, or background information but does not define mandatory project requirements.

### Notes

Informative content supports understanding and interpretation. It does not override normative specifications.

---

# L

## Layer

### Definition

A logical architectural boundary that groups responsibilities of a similar abstraction level.

### Notes

Layers communicate only through defined interfaces and SHALL comply with the dependency rules defined by the architecture.

---

# N

## Normative

### Definition

Content that defines mandatory project requirements and contributes directly to specification compliance.

### Notes

Normative statements typically use RFC2119 keywords such as **MUST**, **SHOULD**, and **MAY**.

---

# P

## Project

### Definition

The complete Build.xlam Project, including all specifications, architectural definitions, implementations, and supporting documentation.

### Notes

Unless explicitly stated otherwise, the term "Project" refers to the entire Build.xlam ecosystem rather than an individual repository or artifact.

---

# R

## Requirement

### Definition

A mandatory or recommended rule defined by a normative specification.

### Notes

Requirements are expressed using RFC2119 terminology and SHALL be interpreted according to the rules defined in this glossary.

---

## RFC2119

### Definition

The terminology standard used to express normative requirement levels within the Build.xlam Project.

### Notes

The keywords **MUST**, **SHOULD**, and **MAY** have the meanings defined in Section 6 of this document and SHALL be interpreted consistently throughout all normative specifications.

---

# S

## Single Source of Truth (SSOT)

### Definition

The principle that every authoritative piece of project information has exactly one normative source.

### Notes

Duplicate definitions SHALL be avoided. When identical information is required in multiple documents, subordinate documents SHALL reference the authoritative source instead of redefining it.

---

## Specification

### Definition

A document that defines requirements, responsibilities, constraints, or rules governing the Build.xlam Project.

### Notes

Specifications may be normative or informative. Their authority is determined by the specification hierarchy.

---

## Specification Hierarchy

### Definition

The formal ordering of specifications according to their normative authority.

### Notes

The hierarchy ensures consistent governance and defines which specification prevails when multiple documents appear to conflict.

---

## SSOT

### Definition

Abbreviation for **Single Source of Truth**.

### Notes

See **Single Source of Truth (SSOT)**.
## 5. Glossary (continued)

# T

## Term

### Definition

A word or phrase that has a defined meaning within the Build.xlam Project.

### Notes

When a term is defined in this glossary, that definition SHALL take precedence over informal or external interpretations unless a higher-level normative document explicitly states otherwise.

---

## Traceability

### Definition

The ability to identify and follow relationships between specifications, requirements, architectural decisions, and related project artifacts.

### Notes

Traceability supports consistency, impact analysis, review activities, and long-term maintenance. Normative documents SHOULD preserve traceable relationships to their authoritative sources.

---

# V

## Version

### Definition

A unique identifier representing a specific released state of a document, specification, or project artifact.

### Notes

Version identifiers are used to distinguish revisions and SHALL be managed according to the project's document governance policy.

---

# W

## Work Product

### Definition

Any artifact formally produced as part of the Build.xlam Project.

### Notes

Examples include specifications, architecture documents, design documents, implementation deliverables, review records, and related documentation.

---

# 6. RFC2119 Keywords

The Build.xlam Project adopts the terminology defined by RFC2119 to express normative requirement levels.

| Keyword        | Definition                                                                                                                                     |
| -------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| **MUST**       | Indicates an absolute requirement that shall always be satisfied.                                                                              |
| **MUST NOT**   | Indicates an absolute prohibition.                                                                                                             |
| **SHOULD**     | Indicates a recommended practice. Valid reasons may exist to choose an alternative, but the implications should be understood before doing so. |
| **SHOULD NOT** | Indicates a practice that is generally discouraged except under exceptional circumstances.                                                     |
| **MAY**        | Indicates an optional capability or permissible behavior.                                                                                      |

Unless explicitly stated otherwise, all RFC2119 keywords appearing in Build.xlam Project specifications SHALL be interpreted according to the definitions above.

---

# 7. Revision Policy

This glossary is a normative document within the Build.xlam Project specification hierarchy.

New terminology SHALL be added only when it represents a stable concept that is expected to be reused across multiple specifications.

Existing definitions SHALL NOT be modified unless required to maintain consistency with higher-level normative specifications.

When a new term duplicates or overlaps an existing definition, the existing authoritative definition SHALL be reused to preserve the Single Source of Truth (SSOT).

All revisions SHALL maintain consistency with:

* Canon_v2.0.md
* Architecture_v2.0.md
* SpecificationHierarchy_v2.0.md

This document serves as the authoritative terminology reference for the Build.xlam Project.
