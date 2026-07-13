# VMF v1.1 Candidate Scope Review

Version : 0.1
Status  : Working Review
Scope   : VMF v1.1 Candidate scope planning
Date    : 2026-07-13
Depends : candidates/VMFCandidates_v1.1.md, candidates/VMF_v1.1_Candidate.md, docs/releases/Build_v1.1_ReleaseReport.md

---

# 1. Purpose

This document records a focused scope review for VMF v1.1 Candidate planning.

This is not a normative specification. It does not modify VMF v1.0, Canon
v2.0, Build v1.1, or any frozen specification.

---

# 2. Review Result

Status : Candidate scope review opened

Build.xlam v1.1 has been officially released and provides partial implementation
support for several VMF v1.1 Candidate concepts. VMF v1.0 remains frozen.

The next VMF v1.1 planning step SHOULD focus on adopting the candidate items
that clarify contract-driven generation without requiring a source-generator
architecture redesign.

---

# 3. Recommended VMF v1.1 Candidate Scope

The following VMF v1.1 Candidate items are recommended for initial VMF v1.1
scope review:

| Candidate | Recommendation | Reason |
|-----------|----------------|--------|
| V001 Native Core Layer Generation | Include | Build v1.1 already supports valid custom layer names, including `Core`, while VMF v1.0 remains unchanged. |
| V005 VMF v1.x Design Charter | Include | Establishes the governance rule that VMF v1.0 remains frozen and future behavior enters through reviewed candidate contracts. |
| V006 Contract-Driven Generation Architecture | Include | Provides the central architecture rule for separating Blueprint, Manifest, Template, Generator, and generated output responsibilities. |
| V007 Template Body And Section Contract | Include | Defines `{{BODY}}`, section markers, and empty-section handling as focused generation contracts. |
| V008 Canonical Template Contract | Include | Records the structural template set needed for consistent future generation without embedding domain behavior in templates. |

---

# 4. Dependency-Managed Scope

The following items should remain under review, but should not be adopted before
the recommended core scope is stable:

| Candidate | Recommendation | Reason |
|-----------|----------------|--------|
| V002 Manifest YAML Native Reader | Defer until Manifest Contract is stable | Native YAML reading should follow the adopted manifest contract rather than define it indirectly. |
| V003 Native Enum Generation | Defer until Template Contract is stable | Enum generation depends on canonical template and manifest validation rules. |
| V004 Manifest Generation Contract | Review with V006 | Manifest generation is central to contract-driven architecture and should be aligned with the adopted Blueprint and Manifest contracts. |
| V009 Body Source / Section Source Contract | Defer until V007/V008 are stable | BodySource and SectionSource should build on the adopted body, section, and template contracts. |

---

# 5. Explicit Non-Scope

The following work remains outside the initial VMF v1.1 scope review:

- modifying `specs/vmf/VMF_v1.0.md`;
- changing Canon v2.0;
- redefining Build v1.1 release behavior;
- adopting Build v2.0 source-generator architecture;
- treating candidate template assets as VMF v1.0 requirements;
- requiring UserForm generation as part of VMF v1.0 audit compliance.

---

# 6. Build v1.1 Impact

Build.xlam v1.1 affects VMF v1.1 planning in the following ways:

- custom layer manifest generation partially supports V001 by allowing valid
  layer names such as `Core`;
- blueprint parsing and manifest generation partially support V004 and V006;
- template validation and candidate template assets partially support V007 and
  V008;
- Build v1.1 does not adopt a native VMF YAML reader, native Enum generation, or
  BodySource / SectionSource resolution contract.

These impacts are implementation evidence only. They do not adopt VMF v1.1
requirements.

---

# 7. Open Review Questions

1. Should V001 be promoted first as a narrow VMF v1.1 compatibility improvement?
2. Should V005 and V006 be adopted together as the VMF v1.1 architectural
   foundation?
3. Should V007 and V008 be reviewed as one template-contract adoption package?
4. Should V004 be split into Blueprint-to-Manifest rules and Manifest-to-Template
   rules before adoption?
5. Should UserForm generation remain a future Build candidate, a VMF template
   candidate, or both?

---

# 8. Recommended Next Steps

1. Confirm the initial VMF v1.1 scope package:
   V001, V005, V006, V007, and V008.
2. Prepare a VMF v1.1 readiness audit that maps each accepted candidate to
   current Build v1.1 evidence and missing verification.
3. Keep V002, V003, V004, and V009 under dependency-managed review until the
   core contract package is stable.
4. Record UserForm generation as a separate future candidate if it is required
   for application completeness beyond current VMF v1.0 audit scope.

---

# 9. Revision History

| Version | Status | Description |
|---------|--------|-------------|
| 0.1 | Working Review | Opens VMF v1.1 candidate scope review and recommends initial scope package |
