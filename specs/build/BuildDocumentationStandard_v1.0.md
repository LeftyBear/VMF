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
- BuildQualityStandard_v1.0.md
- BuildBlueprint_v1.0.1.md
- BuildReleaseProcedure_v1.0.md
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
4. BuildQualityStandard_v1.0.md
5. BuildBlueprint_v1.0.1.md
6. BuildReleaseProcedure_v1.0.md
7. BuildReleaseChecklist_v1.0.md
8. releases/Build_v1.0.1_ReleaseReport.md
9. BuildCandidates_v1.1.md
10. CHANGELOG.md
11. README.md

Lower-precedence documents MUST NOT contradict higher-precedence documents.

---

# 4. Separation Rules

Specifications SHALL remain under `specs`.

Implementation details SHALL NOT be mixed into official documentation unless they define approved behavior.

Build v1.1 Candidate items SHALL be documented only in BuildCandidates_v1.1.md until formally adopted.

BuildBlueprint_v1.0.1.md SHALL describe the approved Build v1.0.1 blueprint and SHALL NOT include v1.1 Candidate behavior.

BuildQualityStandard_v1.0.md SHALL define release quality rules, result codes, evidence handling, version verification, PowerShell build artifact verification, and failure handling.

BuildReleaseProcedure_v1.0.md SHALL define the executable release verification procedure for each checklist item.

BuildReleaseChecklist_v1.0.md SHALL define the release readiness checks required before an official release.

Release Reports under `specs/releases` SHALL record the completed release decision and evidence for a specific official release.

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

# 6. Procedure Design Rules

Procedure documents SHALL define executable verification steps.

Each Procedure step SHALL consist of the following nine items:

- Purpose
- Inspection Targets
- Verification Method
- Expected Result
- PASS Criteria
- FAIL Criteria
- Result Code
- Evidence
- Failure Handling

Purpose SHALL define why the verification is performed.

Inspection Targets SHALL define the files, artifacts, behavior, logs, records, generated output, or release artifacts to be verified.

Verification Method SHALL define the concrete confirmation method.

Expected Result SHALL define the expected state after verification.

PASS Criteria SHALL define the condition required to judge the step as passed.

FAIL Criteria SHALL define the condition requiring the step to be judged as failed.

Result Code SHALL record PASS, FAIL, BLOCKED, or N/A according to BuildReleaseProcedure_v1.0.md and BuildQualityStandard_v1.0.md.

Evidence SHALL define the records to be preserved for traceability.

Failure Handling SHALL define the required issue recording, correction, rebuild when required, and re-audit from Step 1.

Procedure documents SHALL NOT record only final judgments.

Checklist documents SHALL record final judgments and SHALL NOT redefine Procedure steps.

Release Reports SHALL record release-specific evidence and completed release decisions.

---

# 7. Change Policy

Frozen documents SHALL receive only consistency, reference, and clarification updates unless an explicit version change is approved.

Candidate items SHALL remain separated from frozen documents.

New official documents SHALL be added to README.md and CHANGELOG.md.

---

# 8. Release Management

## Official Release Requirements

An official Build.xlam release SHALL include the following release artifacts:

- BuildQualityStandard_v1.0.md
- BuildReleaseProcedure_v1.0.md
- BuildReleaseChecklist_v1.0.md
- A release-specific Release Report under `specs/releases`

Every official checklist shall have a corresponding procedure.

The Release Procedure SHALL define the 14 Step release audit and how each Release Checklist item is confirmed.

Each Release Procedure step SHALL follow the Procedure Design Rules.

The Release Procedure SHALL include Result Code Standard.

The Release Procedure SHALL include Version Verification.

The Release Procedure SHALL require the latest Build.xlam produced after the approved PowerShell build to be audited.

The Release Checklist SHALL be completed according to the Release Procedure before the release decision is finalized.

The Release Checklist SHALL record the judgment for each item and SHALL NOT redefine the confirmation procedure.

The Release Report SHALL be created and saved before the release is considered complete.

The Release Report SHALL reference the applicable Release Procedure and Release Checklist and SHALL NOT redefine BuildCanon_v1.0.md or BuildBlueprint_v1.0.1.md.

Generate Summary SHALL be recorded as release evidence when generation behavior or generated output is inspected.

If any release audit Step fails, the issue SHALL be recorded, corrected, rebuilt when required, and re-audited from Step 1 before release approval.

The release documentation flow SHALL be:

1. BuildBlueprint_v1.0.1.md
2. BuildQualityStandard_v1.0.md
3. BuildReleaseProcedure_v1.0.md
4. BuildReleaseChecklist_v1.0.md
5. releases/Build_v1.0.1_ReleaseReport.md
6. Official Release

---

# 9. Review Checklist

Before release, the documentation set SHALL be checked for:

- Required files exist
- Version and status are present
- Canon v2.0 is not contradicted
- BuildCanon_v1.0.md is not contradicted
- v1.1 Candidate items are isolated in BuildCandidates_v1.1.md
- README.md lists the official documentation set
- CHANGELOG.md records the documentation change
- Release Procedure exists for the release checklist
- Release Procedure defines the 14 Step release audit
- Release Procedure steps follow the Procedure Design Rules
- Result Code Standard is defined
- Version Verification is included
- PowerShell Build Artifact Verification is included
- Generate Summary is handled as evidence
- FAIL handling requires issue recording, correction, rebuild when required, and re-audit from Step 1
- Release Checklist exists for the release
- Release Report exists for the release
