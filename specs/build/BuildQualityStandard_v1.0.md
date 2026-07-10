# Build Quality Standard v1.0

Version : 1.0
Status  : Frozen
Scope   : Build.xlam Release Quality
Depends : Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md, BuildBlueprint_v1.0.1.md

---

# 1. Purpose

This document defines the Build.xlam release quality standard used by BuildReleaseProcedure_v1.0.md.

The purpose of this standard is to keep release audit results consistent, evidence-based, and repeatable.

---

# 2. Release Audit Structure

Build v1.0.1 release quality SHALL be verified through the 14 Step release audit defined by BuildReleaseProcedure_v1.0.md.

Each Step SHALL use the same nine items:

- Purpose
- Inspection Targets
- Verification Method
- Expected Result
- PASS Criteria
- FAIL Criteria
- Result Code
- Evidence
- Failure Handling

The term Inspection Targets SHALL be used for files, artifacts, behavior, logs, records, and generated output inspected during release audit.

The term Target Files SHALL NOT be used as the release audit field name.

---

# 3. Result Code Standard

Release audit results SHALL use the following result codes:

| Result Code | Meaning |
| --- | --- |
| PASS | The Step satisfied all PASS Criteria and no FAIL Criteria were found. |
| FAIL | The Step met one or more FAIL Criteria. |
| BLOCKED | The Step could not be completed because a required artifact, tool, or prerequisite was unavailable. |
| N/A | The Step is explicitly not applicable to the release and the reason is recorded in the Release Report. |

Only PASS and approved N/A results MAY contribute to release approval.

Any FAIL or BLOCKED result SHALL prevent release approval.

---

# 4. Evidence Standard

Release evidence SHALL be recorded in the applicable Release Report under `specs/releases`.

Evidence MAY include inspected documents, source modules, generated output, build logs, artifact paths, artifact timestamps, reviewer notes, issue records, and rebuild records.

Generate Summary SHALL be treated as release evidence whenever generation behavior or generated output is inspected.

Evidence SHALL be sufficient to trace the release decision back to the inspected artifacts.

---

# 5. Version Verification

Release audit SHALL include Version Verification.

Version Verification SHALL confirm that official documents, release artifacts, source metadata when available, generated output metadata when available, git tag, and release record are consistent with the approved release version.

Build v1.0.1 release artifacts SHALL NOT imply adoption of Build v1.1 Candidate behavior.

---

# 6. PowerShell Build Artifact Verification

Release audit SHALL include verification of the latest Build.xlam produced after the approved PowerShell build.

The latest PowerShell-built Build.xlam SHALL be identified by path and timestamp before it is treated as the audited release artifact.

An older Build.xlam artifact SHALL NOT be used for release approval when a newer PowerShell-built artifact exists.

---

# 7. Failure Handling

When a release audit Step fails, the issue SHALL be recorded.

The issue SHALL be corrected before release approval.

If the correction affects Build.xlam or generated artifacts, Build.xlam SHALL be rebuilt and generated artifacts SHALL be regenerated when applicable.

After correction and required rebuild, release audit SHALL restart from Step 1.
