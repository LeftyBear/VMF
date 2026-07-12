# Build v1.1 Candidate Archive

Version : 1.1 Candidate
Status  : Archived After Build v1.1 Release
Scope   : Build.xlam
Depends : Canon_v2.0.md, BuildCanon_v1.0.md, BuildDocumentationStandard_v1.0.md

---

# 1. Policy

This document records Build v1.1 Candidate items and their post-release
disposition.

Build v1.1 has been formally released.

Candidate items marked as adopted are part of the Build v1.1 release record.

Candidate items not adopted by Build v1.1 remain future candidates or Build v2.0
planning material.

Build v1.1 adoption SHALL NOT modify frozen Build v1.0.x specifications.

Build v1.0.1 release audit rules, including the 14 Step release audit, Result Code Standard, Version Verification, PowerShell Build Artifact Verification, Generate Summary evidence handling, and FAIL re-audit handling, are release governance rules and SHALL NOT be treated as Build v1.1 Candidate behavior.

---

# 2. Build v1.1 Release Disposition

Build v1.1 official release record:

- `docs/releases/Build_v1.1_ReleaseReport.md`

## 2.1 Adopted In Build v1.1

The following candidates were adopted in Build v1.1:

- B001 Blueprint Parser
- B002 Manifest Auto Generation
- B004 Generate Preview
- B007 Template Validation
- B008 Manifest Validation
- B011 Custom Layer Manifest Generation

## 2.2 Not Adopted In Build v1.1

The following candidates were not adopted in Build v1.1:

- B003 Parallel Generation
- B005 Source Generator Architecture
- B006 Incremental Generate
- B009 Ribbon UI
- B010 Visual Designer

B005 remains reserved for Build v2.0 planning.

B003, B006, B009, and B010 remain future candidates.

---

# 3. Candidates

## 3.1 Recommended Adoption Order

The recommended Build v1.1 adoption order is:

1. B001 Blueprint Parser
2. B002 Manifest Auto Generation
3. B011 Custom Layer Manifest Generation
4. B008 Manifest Validation
5. B007 Template Validation
6. B004 Generate Preview

B005 Source Generator Architecture is not recommended for Build v1.1 adoption.
It is a Build v2.0 architecture candidate because it changes the generation target
from VBProject artifacts to source files and separates packaging from generation.

B003 Parallel Generation remains deferred.

B006 Incremental Generate, B009 Ribbon UI, and B010 Visual Designer remain future
improvement candidates and SHOULD NOT block Build v1.1 adoption.

## 3.2 Build v1.1 Scope Recommendation

Build v1.1 SHOULD focus on improving manifest-driven generation while preserving
the Build v1.0.x architecture.

The recommended Build v1.1 scope is:

- parse blueprint input into generation metadata;
- generate manifests from approved blueprint metadata;
- support manifest-defined layer names, including downstream layers such as Core;
- validate manifests before generation;
- validate template availability and required tokens before generation.

Build v1.1 SHOULD NOT:

- replace VBProject generation with source-file generation;
- introduce workbook or add-in packaging redesign;
- adopt Build v2.0 self-hosting architecture;
- modify frozen Build v1.0.x specifications.

## 3.3 Build v2.0 Candidate Separation

The following item is reserved for Build v2.0 planning:

- B005 Source Generator Architecture

Build v2.0 planning MAY use B005 as the starting point for source-first
generation, Git-first development, and packaging separation.

Build v1.1 adoption SHALL NOT require B005.

### B001 Blueprint Parser

Status : Adopted
Priority : High
Recommended Release : Build v1.1
Implementation Status : Completed
Adopted Release : Build v1.1

Scope : Parse approved blueprint data into generation metadata without changing
the frozen Build v1.0.x generation pipeline.

### B002 Manifest Auto Generation

Status : Adopted
Priority : High
Recommended Release : Build v1.1
Implementation Status : Completed
Adopted Release : Build v1.1

Scope : Generate manifest entries from parsed blueprint metadata.

Dependency : B001 Blueprint Parser

### B003 Parallel Generation

Status : Deferred
Priority : Low
Reason : VBAは単一スレッドであり現時点で効果が限定的

Recommended Release : Not recommended for Build v1.1
Disposition : Future Candidate

### B004 Generate Preview

Status : Adopted
Priority : Medium
Recommended Release : Build v1.1 Optional
Implementation Status : Completed
Adopted Release : Build v1.1

Scope : Provide a non-mutating preview of manifest-driven generation output.

Dependency : B001 Blueprint Parser, B002 Manifest Auto Generation, B008 Manifest Validation

### B005 Source Generator Architecture

Status : Deferred
Priority : High
Recommended Release : Build v2.0
Disposition : Build v2.0 Planning

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
Priority : Medium
Recommended Release : Future Candidate
Disposition : Future Candidate

Scope : Generate only changed artifacts when a reliable change detection contract
exists.

### B007 Template Validation

Status : Adopted
Priority : High
Recommended Release : Build v1.1
Implementation Status : Completed
Adopted Release : Build v1.1

Scope : Validate template existence and required token coverage before generation.

Dependency : B008 Manifest Validation

### B008 Manifest Validation

Status : Adopted
Priority : High
Recommended Release : Build v1.1
Implementation Status : Completed
Adopted Release : Build v1.1

Scope : Validate manifest structure, component type, layer name, template path,
module name, and required generation metadata before generation.

### B009 Ribbon UI

Status : Proposed
Priority : Low
Recommended Release : Future Candidate
Disposition : Future Candidate

Scope : Provide Excel Ribbon entry points for approved Build operations.

### B010 Visual Designer

Status : Proposed
Priority : Low
Recommended Release : Future Candidate
Disposition : Future Candidate

Scope : Provide visual editing of blueprint or manifest metadata after the
underlying generation contracts are stable.

### B011 Custom Layer Manifest Generation

Status : Adopted
Priority : High
Recommended Release : Build v1.1
Implementation Status : Completed
Adopted Release : Build v1.1

Scope : Support manifest-defined custom layer names without changing the frozen
Build v1.0.x layer contract.

Dependency : B008 Manifest Validation

Build v1.0.1 accepts only the frozen layer set defined by its current ManifestItem validation. A future Build version SHOULD support manifest-defined custom layer names so downstream VMF projects can generate layers such as Core without changing the Build v1.0.1 release contract.
