# Build v1.1 Release Plan

Version : 0.1
Status  : Working Plan
Scope   : Build.xlam v1.1 Candidate to official release
Depends : candidates/BuildCandidates_v1.1.md, docs/development/Build_v1.1_CandidateReadinessAudit.md

---

# 1. Purpose

This document records the working release plan for promoting Build v1.1 from
Candidate review through RC1 and then to an official Build v1.1 release.

This is not a normative specification. It does not modify VMF v1.0, Build
v1.0.x, or any frozen specification.

---

# 2. Milestone Path

The Build v1.1 release path is:

```text
Build v1.1 Candidate
    -> Release Candidate 1 (RC1)
    -> Build v1.1 Official Release
```

## 2.1 Build v1.1 Candidate

Purpose : Candidate implementation review and scope confirmation.

Entry condition:

- Build v1.1 candidate items are documented in `candidates/BuildCandidates_v1.1.md`.
- Candidate implementation evidence is recorded in
  `docs/development/Build_v1.1_CandidateReadinessAudit.md`.
- Candidate behavior remains outside frozen Build v1.0.x specifications.

Exit condition:

- Public API changes are reviewed.
- Candidate scope is accepted or explicitly reduced.
- Deferred items remain separated from the v1.1 release scope.
- RC1 release audit preparation is approved.

## 2.2 Release Candidate 1

Purpose : Freeze the accepted Build v1.1 release candidate for final validation.

Entry condition:

- Build v1.1 Candidate exit conditions are satisfied.
- Accepted candidate items are fixed for RC1.
- No Build v2.0 planning items are included in the RC1 scope.

RC1 rules:

- RC1 is the official release candidate for Build v1.1.
- Scope changes after RC1 SHOULD be limited to release-blocking defects,
  documentation inconsistencies, or verification corrections.
- Any functional change after RC1 SHOULD require a new RC decision.

Exit condition:

- Release audit passes.
- Required release evidence is recorded.
- No release-blocking issues remain open.

## 2.3 Build v1.1 Official Release

Purpose : Adopt the approved RC1 as the official Build v1.1 release.

Entry condition:

- RC1 exit conditions are satisfied.
- Release artifacts are approved.
- Version, documentation, and release evidence are consistent.

Exit condition:

- Build v1.1 is recorded as the official release.
- Further changes are tracked as future candidates.

---

# 3. Candidate Scope For RC1

The recommended RC1 scope is the completed Build v1.1 implementation set:

- B001 Blueprint Parser
- B002 Manifest Auto Generation
- B004 Generate Preview
- B007 Template Validation
- B008 Manifest Validation
- B011 Custom Layer Manifest Generation

The following items remain outside RC1:

- B003 Parallel Generation
- B005 Source Generator Architecture
- B006 Incremental Generate
- B009 Ribbon UI
- B010 Visual Designer

B005 remains a Build v2.0 planning candidate and SHALL NOT block Build v1.1 RC1.

---

# 4. RC1 Preparation Tasks

1. Review the Build v1.1 candidate implementation diff.
2. Confirm public API acceptability for accepted candidate items.
3. Confirm that deferred and Build v2.0 items are not included in RC1 behavior.
4. Run the automated VBA test suite.
5. Record verification evidence for the RC1 decision.
6. Prepare the release audit checklist and release report for Build v1.1.

---

# 5. Release Control

Build v1.1 Candidate and RC1 materials SHALL remain separate from frozen Build
v1.0.x specifications until official adoption is approved.

After Build v1.1 official release, new improvements SHALL be tracked as future
candidate items rather than being added directly to the released baseline.

---

# 6. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 0.1 | Working Plan | Defines Candidate to RC1 to official release path |
