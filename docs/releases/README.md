# Release Reports

This directory records official release reports for VMF and Build.xlam.

Release reports preserve release decisions, evidence, issue records, rebuild
records when applicable, and release artifact references.

---

## Official Releases

| Release | Report | Artifact |
|---------|--------|----------|
| VMF v1.0 | `VMF_v1.0_ReleaseReport.md` | N/A |
| Build.xlam v1.0.1 | `Build_v1.0.1_ReleaseReport.md` | Historical release baseline |
| Build.xlam v1.0.2 | `Build_v1.0.2_ReleaseReport.md` | `dist/release/Build/v1.0.2/Build.xlam` |
| Build.xlam v1.1 | `Build_v1.1_ReleaseReport.md` | `dist/release/Build/v1.1/Build.xlam` |
| VMF Studio v1.1.0 | `../../RELEASE_NOTES_v1.1.md` | `dist/release/VMFStudio_1.1.0/VMFStudio_1.1.0.xlam` |
| Publisher Phase 3-8 | `Publisher_Phase3-8_ReleaseNotes.md` | `VMF.Publisher.sln` |
| Publisher Phase 3-9 | `Publisher_Phase3-9_ReleaseNotes.md` | `dist/release/Publisher/vmf-publisher-<version>-win-x64.zip` |
| Publisher Phase 3-10 | `Publisher_Phase3-10_ReleaseNotes.md` | Release approval pending external Avast response |
| Publisher 0.0.1-dev | `Publisher_0.0.1-dev_ReleaseNotes.md` | GitHub prerelease published; Avast vendor clearance not obtained |

Post-release Publisher evidence is governed by
`../architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md`
and the P1 release-safety templates under `../development/`. Post-release
observations do not retroactively satisfy pre-release approval, release
authorization, vendor clearance, required gates, or final verification.

---

## Current Build Release

The current official VMF Studio release is:

```text
dist/release/VMFStudio_1.1.0/VMFStudio_1.1.0.xlam
```

Metadata:

- Product Name: `VMF Studio`
- Product Version: `1.1.0`
- Release Type: `Release`

---

## Candidate Separation

Future improvements SHALL be recorded under `candidates/` before adoption.

Build v2.0 planning remains separate from Build v1.1 maintenance.
