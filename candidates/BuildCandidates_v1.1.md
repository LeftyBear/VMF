# Build v1.1 Candidates

Version : 1.1 Candidate
Status  : Candidate
Scope   : Build.xlam
Depends : Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md

---

# 1. Policy

This document records Build v1.1 Candidate items.

Candidate items are not part of Build v1.0 or Build v1.0.1 until formally adopted.

Build v1.1 Candidate items SHALL NOT be mixed into BuildBlueprint_v1.0.1.md.

Build v1.0.1 release audit rules, including the 14 Step release audit, Result Code Standard, Version Verification, PowerShell Build Artifact Verification, Generate Summary evidence handling, and FAIL re-audit handling, are release governance rules and SHALL NOT be treated as Build v1.1 Candidate behavior.

---

# 2. Candidates

### B001 Blueprint Parser

Status : Accepted
Priority : High

### B002 Manifest Auto Generation

Status : Accepted
Priority : High

### B003 Parallel Generation

Status : Deferred
Priority : Low
Reason : VBAは単一スレッドであり現時点で効果が限定的

### B004 Generate Preview

Status : Proposed

### B005 Source Generator Architecture

Status : Candidate
Priority : High

#### Background

Build v1.0.2 generates VBA modules directly into a VBProject.

This architecture is suitable for the first official release, but long-term development has the following limitations.

- Source code cannot exist independently from Excel.
- Git-based source management is difficult.
- Code review is difficult.
- VSCode-first development is impossible.
- Build is tightly coupled to the Excel VBE.

#### Proposal

Redesign Build as a Source Generator instead of a VBProject Generator.

Build shall generate standard VBA source files.

Generated artifacts include:

- `.bas`
- `.cls`
- `.frm`
- `.frx`
- `manifest.yaml`
- project metadata

Build is responsible for automatically generating scaffolds for:

- Domain classes
- Application Services
- Repositories
- Presenters
- Dependency Injection (Composition Root)

The generated source files become the canonical development artifacts.

Packaging into Workbook (`.xlsm`) or Add-in (`.xlam`) shall be performed by a separate Build/Packaging process.

#### Architecture

```text
manifest.yaml
        │
        ▼
      Build
        │
        ▼
   Source Files
(.bas/.cls/.frm/.frx)
        │
        ▼
 Packaging
        │
        ▼
 Workbook / Add-in
```

#### Benefits

- Git-first development
- VSCode-first development
- Independent source management
- Easier code review
- Separation of generation and packaging
- Better compatibility with future tools (including twinBASIC)

#### Impact

No impact on Build v1.0.2.

This proposal targets the Build v2 architecture.

Do not modify any released Build v1.0.x specifications or implementation.

#### Notes

This Candidate was identified during the first real-world VMF v1.0 application development project and represents a long-term architectural evolution rather than a bug fix or enhancement.

### B006 Incremental Generate

Status : Proposed

### B007 Template Validation

Status : Proposed

### B008 Manifest Validation

Status : Proposed

### B009 Ribbon UI

Status : Proposed

### B010 Visual Designer

Status : Proposed

### B011 Custom Layer Manifest Generation

Status : Proposed
Priority : High

Build v1.0.1 accepts only the frozen layer set defined by its current ManifestItem validation. A future Build version SHOULD support manifest-defined custom layer names so downstream VMF projects can generate layers such as Core without changing the Build v1.0.1 release contract.
