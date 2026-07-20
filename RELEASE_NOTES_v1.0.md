# VMF Studio v2.0 Specification v1.0 Release Notes

- SpecificationName: VMF Studio v2.0 Specification
- SpecificationVersion: 1.0.0
- Edition: Frozen Edition
- Status: Frozen
- ReleaseDate: 2026-07-20
- ReleaseTag: vmf-studio-v2.0-spec-v1.0.0

---

## Overview

This release freezes VMF Studio v2.0 Specification v1.0 and establishes its
Markdown representation in this Git repository as the official
version-controlled artifact. The release closes the Specification Voyage and
opens the Implementation Voyage. A subsequent documentation update adds the
Publisher v1.0 Architecture Specification package in repository-native
Markdown form.

## Released Artifact

- `specs/studio/VMF-Studio-v2.0-Specification-v1.0-Frozen.md`
- SHA-256 (UTF-8/LF): `923801c7503c63c148e2bc553e01c374a65219703b1b4e72b82a6665a8e3d807`

## Publisher Architecture Documentation Update

The 2026-07-20 documentation update adds the Publisher v1.0 Architecture
Specification (Frozen Edition) package under `specs/publisher/`.

### Artifacts

- `specs/publisher/Publisher_v1.0_Architecture_Specification.md`
- `specs/publisher/volumes/P0_Overview.md`
- `specs/publisher/volumes/P1_Markdown_Engine.md`
- `specs/publisher/volumes/P2_Update_Engine.md`
- `specs/publisher/volumes/P3_Publish_Manifest.md`
- `specs/publisher/volumes/P4_Compilation_Engine.md`
- `specs/publisher/volumes/P5_VMF_Studio_Integration.md`
- `specs/publisher/volumes/P6_Extension_Points.md`
- `specs/publisher/schemas/publish-manifest.schema.yaml`

### Source Provenance and Verification

- Source document format: Microsoft Word (`.docx`).
- Repository document format: UTF-8 Markdown (`.md`).
- Preserved structure: titles, section headings, paragraphs, and numbered
  lists.
- Verified all 480 non-empty source paragraphs against the generated Markdown
  blocks with exact text matching.
- Removed the intermediate DOCX sources after successful verification.
- Retained the Publish Manifest schema as YAML under `specs/publisher/schemas/`.

## Source Provenance

- Source: Google Docs
- Source title: `VMF Studio v2.0 Specification v1.0 (Frozen Edition)`
- Source document ID: `1m1SEKRAhAOcq9HTABchTkt6eCk5Ebf-J5z9s013Mx5Q`
- Source revision: `ALtnJHzZ8bGlCAI8LZVaxq2B53cl2Uj5ibxDf2E6llLTtFtfewkOQS6c-uRXRvYOir2bB4tlr0Lo3xN-y4g4SkA7Ajcgzm1A8ezdHU_A2FUS`
- Export format: Markdown (`text/markdown`)

## Repository Authority

The frozen Markdown artifact committed and tagged by this release is the
official version-controlled baseline for implementation. The source Google Doc
records the frozen editorial source used for this export. Any normative change
requires a new specification version and the applicable governance process.

## Release Scope

- Exported the complete, single-tab frozen master specification to Markdown.
- Added the specification under `specs/studio/`.
- Updated `CHANGELOG.md`.
- Added these release notes.
- Committed and tagged the frozen baseline.
- Verified repository cleanliness, artifact integrity, and tag reachability.

## Milestone

- Specification Voyage ✓ Complete
- Implementation Voyage ▶ Begin

> From specification to implementation.

Fair winds and following seas.
