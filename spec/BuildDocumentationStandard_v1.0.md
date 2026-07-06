# Build Documentation Standard v1.0

Version : 1.0
Status  : Frozen
Scope   : Build.xlam Official Documentation Set
Depends : Canon_v2.0.md, BuildCanon_v1.0.md

---

# 1. Purpose

This document defines the documentation standard for the Build.xlam official documentation set.

The purpose of this standard is to keep Build.xlam documentation consistent, traceable, and separated by responsibility.

---

# 2. Official Documentation Set

The Build.xlam official documentation set consists of the following files:

- Canon_v2.0.md
- BuildCanon_v1.0.md
- BuildDocumentationStandard_v1.0.md
- BuildBlueprint_v1.0.1.md
- BuildReleaseChecklist_v1.0.md
- releases/Build_v1.0.1_ReleaseReport.md
- BuildCandidates_v1.1.md
- CHANGELOG.md
- README.md

---

# 3. Document Hierarchy

When documents conflict, the following precedence SHALL apply:

1. Canon_v2.0.md
2. BuildCanon_v1.0.md
3. BuildDocumentationStandard_v1.0.md
4. BuildBlueprint_v1.0.1.md
5. BuildReleaseChecklist_v1.0.md
6. releases/Build_v1.0.1_ReleaseReport.md
7. BuildCandidates_v1.1.md
8. CHANGELOG.md
9. README.md

Lower-precedence documents MUST NOT contradict higher-precedence documents.

---

# 4. Separation Rules

Specifications SHALL remain under `spec`.

Implementation details SHALL NOT be mixed into official documentation unless they define approved behavior.

Build v1.1 Candidate items SHALL be documented only in BuildCandidates_v1.1.md until formally adopted.

BuildBlueprint_v1.0.1.md SHALL describe the approved Build v1.0.1 blueprint and SHALL NOT include v1.1 Candidate behavior.

BuildReleaseChecklist_v1.0.md SHALL define the release readiness checks required before an official release.

Release Reports under `spec/releases` SHALL record the completed release decision and evidence for a specific official release.

CHANGELOG.md SHALL record documentation and specification changes without redefining the specifications.

README.md SHALL act as an index and orientation document, not as a duplicate specification.

---

# 5. Writing Rules

Official documentation SHALL use Markdown.

Official documentation SHALL use UTF-8 encoding.

Each official document SHOULD include:

- Title
- Version
- Status
- Scope
- Dependencies, when applicable

Normative language SHOULD use MUST, MUST NOT, SHALL, SHALL NOT, SHOULD, SHOULD NOT, MAY, and OPTIONAL consistently with Canon v2.0.

---

# 6. Change Policy

Frozen documents SHALL receive only consistency, reference, and clarification updates unless an explicit version change is approved.

Candidate items SHALL remain separated from frozen documents.

New official documents SHALL be added to README.md and CHANGELOG.md.

---

# 7. Release Management

## Official Release Requirements

An official Build.xlam release SHALL include both of the following release artifacts:

- BuildReleaseChecklist_v1.0.md
- A release-specific Release Report under `spec/releases`

The Release Checklist SHALL be completed before the release decision is finalized.

The Release Report SHALL be created and saved before the release is considered complete.

The Release Report SHALL reference the applicable Release Checklist and SHALL NOT redefine BuildCanon_v1.0.md or BuildBlueprint_v1.0.1.md.

---

# 8. Review Checklist

Before release, the documentation set SHALL be checked for:

- Required files exist
- Version and status are present
- Canon v2.0 is not contradicted
- BuildCanon_v1.0.md is not contradicted
- v1.1 Candidate items are isolated in BuildCandidates_v1.1.md
- README.md lists the official documentation set
- CHANGELOG.md records the documentation change
- Release Checklist exists for the release
- Release Report exists for the release
