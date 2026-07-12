# Build v1.1 RC1 Release Audit Checklist

Version : 0.1
Status  : Working Checklist
Scope   : Build.xlam v1.1 RC1 preparation
Date    : 2026-07-12
Depends : docs/development/Build_v1.1_ReleasePlan.md, docs/development/Build_v1.1_RC1ScopeReview.md, specs/build/BuildReleaseProcedure_v1.0.md

---

# 1. Purpose

This document prepares the release audit checklist for Build v1.1 RC1.

This is not a normative specification. It does not modify VMF v1.0, Build
v1.0.x, or any frozen specification.

This checklist SHALL NOT be treated as an official Build v1.1 release approval
until the RC1 audit is completed and the release decision is recorded.

---

# 2. Result Code Policy

The final RC1 audit SHALL use the result codes defined by
`specs/build/BuildReleaseProcedure_v1.0.md`:

- PASS
- FAIL
- BLOCKED
- N/A

Until the RC1 audit is executed, each checklist item remains Pending.

---

# 3. RC1 Audit Checklist

| Step | Release Audit Item | Preparation Status | Planned Evidence | Reviewer | Date | Remarks |
|------|--------------------|--------------------|------------------|----------|------|---------|
| 1 | Canon Compliance | Pending | Canon_v2.0.md, BuildCanon_v1.0.md, Build v1.1 candidate documents | Codex | 2026-07-12 | Confirm candidate adoption does not contradict higher-priority canon. |
| 2 | Candidate Scope Compliance | Pending | `candidates/BuildCandidates_v1.1.md`, `Build_v1.1_RC1ScopeReview.md` | Codex | 2026-07-12 | Confirm RC1 includes only accepted v1.1 items. |
| 3 | Candidate Isolation | Pending | Deferred candidate list and source search results | Codex | 2026-07-12 | Confirm B003, B005, B006, B009, and B010 remain outside RC1. |
| 4 | Architecture Verification | Pending | Application, Infrastructure, manifest, and preview generation paths | Codex | 2026-07-12 | Confirm Build v1.0.x architecture is preserved. |
| 5 | Unit Verification | Pending | `tools\test\run-tests.ps1` output | Codex | 2026-07-12 | Confirm all unit-level VBA runners pass. |
| 6 | Integration Verification | Pending | Project manifest, blueprint parser, generation, preview, and UI runners | Codex | 2026-07-12 | Confirm integrated paths pass. |
| 7 | Generator Verification | Pending | Generator and layer generation runners | Codex | 2026-07-12 | Confirm generation remains manifest-driven and template-driven. |
| 8 | Pipeline Verification | Pending | Manifest validation, template validation, token replacement, preview flow | Codex | 2026-07-12 | Confirm validation runs before generation and preview is non-mutating. |
| 9 | Generated Layer Verification | Pending | Common, Manifest, Infrastructure, Domain, Application, and Presentation runners | Codex | 2026-07-12 | Confirm generated layer checks pass. |
| 10 | Documentation Verification | Pending | RC1 plan, scope review, checklist, release report draft, README/CHANGELOG when updated | Codex | 2026-07-12 | Confirm documentation is current and separated by responsibility. |
| 11 | Version Verification | Pending | RC1 version identifiers and release report draft | Codex | 2026-07-12 | Confirm version labels consistently identify Build v1.1 RC1. |
| 12 | PowerShell Build Artifact Verification | Pending | `tools\build\build.ps1`, generated Build.xlam artifact path and timestamp | Codex | 2026-07-12 | Confirm the audited artifact is produced by the approved build. |
| 13 | Release Evidence Verification | Pending | Evidence inventory in the RC1 release report draft | Codex | 2026-07-12 | Confirm all evidence is traceable. |
| 14 | RC1 Decision | Pending | Completed Step results and issue records | Codex | 2026-07-12 | Decide whether RC1 may be declared. |

---

# 4. RC1 Audit Inputs

The RC1 audit is expected to use the following working inputs:

- `candidates/BuildCandidates_v1.1.md`
- `docs/development/Build_v1.1_ReleasePlan.md`
- `docs/development/Build_v1.1_CandidateReadinessAudit.md`
- `docs/development/Build_v1.1_RC1ScopeReview.md`
- `docs/development/Build_v1.1_RC1ReleaseReportDraft.md`
- `specs/build/BuildReleaseProcedure_v1.0.md`
- `tools\test\run-tests.ps1`
- `tools\build\build.ps1`

---

# 5. Current Decision

RC1 audit preparation is ready.

RC1 is not approved by this checklist.

The next task is to execute the RC1 audit, build the RC1 artifact when required,
record evidence, and update the result code for each Step.

---

# 6. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 0.1 | Working Checklist | Prepares the Build v1.1 RC1 release audit checklist |
