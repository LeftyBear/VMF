# Build.xlam Official Documentation Set

This directory contains the official documentation set for Build.xlam.

---

# Document Set

| Document | Purpose |
| --- | --- |
| Canon_v2.0.md | Highest-level project canon |
| BuildCanon_v1.0.md | Frozen Build.xlam development canon |
| BuildDocumentationStandard_v1.0.md | Documentation rules for the official documentation set |
| BuildQualityStandard_v1.0.md | Release quality, result code, evidence, version, build artifact, and failure handling rules |
| BuildBlueprint_v1.0.1.md | Frozen Build v1.0.1 blueprint |
| BuildReleaseProcedure_v1.0.md | 14 Step release audit procedure |
| BuildReleaseChecklist_v1.0.md | Release readiness checklist |
| releases/Build_v1.0.1_ReleaseReport.md | Official release report for Build v1.0.1 |
| releases/Build_v1.0.2_ReleaseReport.md | Official patch release report for Build v1.0.2 |
| BuildCandidates_v1.1.md | Build v1.1 Candidate items |
| CHANGELOG.md | Documentation change history |
| README.md | Index for the official documentation set |

---

# Rules

Canon_v2.0.md is the highest-priority document.

BuildCanon_v1.0.md defines the frozen Build.xlam v1.0 development canon.

BuildDocumentationStandard_v1.0.md defines how this documentation set is maintained.

BuildQualityStandard_v1.0.md defines release quality rules, result codes, evidence handling, version verification, PowerShell build artifact verification, and failure handling.

BuildReleaseProcedure_v1.0.md defines the 14 Step release audit and how each release checklist item SHALL be confirmed.

BuildReleaseChecklist_v1.0.md defines the release readiness checks that SHALL be completed before an official release decision.

Release Reports under `releases/` record the completed release decision, release evidence, Generate Summary evidence when applicable, issue records, and rebuild records for each official release.

For release review, use the following reference order:

1. BuildBlueprint_v1.0.1.md
2. BuildQualityStandard_v1.0.md
3. BuildReleaseProcedure_v1.0.md
4. BuildReleaseChecklist_v1.0.md
5. The applicable Release Report under `releases/`
6. Official Release

The release audit SHALL include 14 Steps.

Step 5 SHALL be Unit Verification.

Step 6 SHALL be Integration Verification.

The latest Build.xlam produced after the approved PowerShell build SHALL be the audited release artifact.

Any FAIL or BLOCKED result SHALL prevent release approval until the issue is recorded, corrected, rebuilt when required, and re-audited from Step 1.

Build v1.1 Candidate items are maintained only in BuildCandidates_v1.1.md until formally adopted.

README.md is an index and SHALL NOT redefine specifications.
