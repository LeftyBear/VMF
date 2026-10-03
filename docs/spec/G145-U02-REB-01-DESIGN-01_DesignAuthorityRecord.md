# G145-U02-REB-01-DESIGN-01 - Design Authority Record

## 1. Document control

| Field | Value |
| --- | --- |
| ID | `G145-U02-REB-01-DESIGN-01` |
| Version | `1.0` |
| Status | `Adopted design authority record` |
| Adoption date | `2026-10-03` |
| Requirements Authority | User |
| Applies to | Proposed `G145-U02-REB-01` baseline design |

This record provides repository-local traceability for the previously adopted
`G145-U02-REB-01` baseline design and adopted design supplements 1 through 4.
It records those decisions; it does not create or expand them.

## 2. Adopted baseline design

The adopted design defines a prospective, candidate-specific evidence-package
requirements baseline for `U03-SUB-20260929-01`. The baseline contains exactly
seven mandatory requirements with the following adopted design decisions:

| Requirement | Adopted design decision |
| --- | --- |
| `REQ-01` | Require unambiguous, attributable identification of the baseline, candidate, package and submission version, package owner, scope, and exclusions. |
| `REQ-02` | Require requirement-to-evidence mapping using only `SUBMITTED`, `MISSING`, or `NOT APPLICABLE`, subject to the supplement 1 rules below. |
| `REQ-03` | Require each submitted evidence item to have a unique identity and the complete mandatory evidence schema, including the supplement 2 fields below. |
| `REQ-04` | Require attributable and verifiable provenance, source, ownership, date/version, identity, and integrity information, with unsupported information exposed rather than inferred or repaired. |
| `REQ-05` | Require explicit candidate-scope, applicability, exclusion, and unresolved production-gap coverage without representing a gap as resolved. |
| `REQ-06` | Require explicit recording of missing evidence, unresolved matters, unknowns, applicability constraints, and limitations, with unsupported mandatory information failing closed to `INCOMPLETE`. |
| `REQ-07` | Require attributable review and acceptance records while preserving separation among review, acceptance, and execution authorization. |

## 3. Adopted design supplements

### Supplement 1 - Mapping-status and non-applicability rules

- A mandatory requirement itself cannot be `NOT APPLICABLE`.
- `NOT APPLICABLE` applies only to an individual evidence mapping.
- Every `NOT APPLICABLE` mapping requires attributable justification.
- `NOT APPLICABLE` alone cannot satisfy a mandatory requirement.
- Every mandatory requirement requires at least one valid `SUBMITTED` mapping.
- Any remaining `MISSING` mapping makes the package `INCOMPLETE`.

### Supplement 2 - Mandatory evidence fields

The mandatory evidence-item schema also requires:

- item status/completeness; and
- status/completeness basis.

### Supplement 3 - Deterministic package-completeness rule

Package status is `COMPLETE` if and only if all of the following are true:

- all seven mandatory requirements have been evaluated;
- every mandatory requirement has at least one valid `SUBMITTED` mapping;
- zero `MISSING` mappings remain;
- every mandatory evidence field is present;
- mandatory evidence information is attributable and verifiable;
- every `NOT APPLICABLE` mapping has valid attributable justification; and
- all baseline coverage and completeness conditions are satisfied.

Otherwise, package status is `INCOMPLETE`.

### Supplement 4 - Completeness non-effects

Package status `COMPLETE` establishes structural package completeness only. It
does not imply evidence sufficiency, evidence acceptance, production-gap
resolution, security disposition, residual-risk acceptance, technical GO, or
execution authorization.

## 4. Traceability

| Adopted decision | Baseline application |
| --- | --- |
| Original design requirement 1 | Package identification requirement |
| Original design requirement 2 | Requirement-to-evidence mapping requirement |
| Original design requirement 3 | Evidence-item identification and mandatory schema requirement |
| Original design requirement 4 | Provenance, identity, and integrity requirement |
| Original design requirement 5 | Candidate-scope and production-gap coverage requirement |
| Original design requirement 6 | Missing-evidence and limitations requirement |
| Original design requirement 7 | Review and acceptance record requirement |
| Supplement 1 | Mapping status, `NOT APPLICABLE`, `SUBMITTED`, and `MISSING` rules |
| Supplement 2 | Mandatory item status/completeness and basis fields |
| Supplement 3 | Deterministic `COMPLETE` / `INCOMPLETE` rule |
| Supplement 4 | Package-completeness non-effects |

The applicable proposed baseline is
`docs/spec/G145-U02-REB-01_ProspectiveReplacementRequirementsBaseline.md`.

## 5. Governance boundary and non-effects

This record is not a reconstruction or recovery of P9-339 and does not
supersede P9-339. It does not adopt the Proposed baseline. Baseline adoption and
P9-339 supersession each require a separate explicit decision by the applicable
authority.

This record does not reconstruct, validate, authorize, or retroactively alter
P9-343 or P9-345. P9-343 is not an authoritative requirements source for this
design. P9-345 remains unchanged and `INCOMPLETE`.

This record grants no evidence sufficiency or acceptance, production-gap
resolution, security disposition, residual-risk acceptance, continuation,
command, technical-GO, execution, or `SAFE-STOP`-clearance authority. Technical
execution remains `NO-GO / SAFE-STOP`.

The design and its proposed baseline operate prospectively only. They do not
rewrite, validate, complete, or otherwise change any historical submission,
review, evidence record, decision, or authorization.
