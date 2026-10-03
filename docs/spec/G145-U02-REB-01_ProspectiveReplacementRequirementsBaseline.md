# G145-U02-REB-01 - Prospective Replacement Requirements Baseline

## 1. Document control

| Field | Value |
| --- | --- |
| ID | `G145-U02-REB-01` |
| Version | `1.0` |
| Status | `Adopted / Operative` |
| Adoption date | `2026-10-03` |
| Candidate | `U03-SUB-20260929-01` |
| Scope | Candidate-specific evidence-package requirements |
| Requirements Authority | User, under ADR-0020 |

On `2026-10-03`, the User, acting as Requirements Authority under ADR-0020,
explicitly adopted `G145-U02-REB-01` v1.0 as the requirements baseline. This
document is therefore `Adopted / Operative`.

This document is a prospective requirements baseline. It is not a
reconstruction or recovery of the unavailable P9-339 requirements. On
`2026-10-03`, the User, acting as Requirements Authority under ADR-0020,
explicitly decided to supersede P9-339 prospectively with `G145-U02-REB-01`
v1.0. P9-339 remains preserved as a historical reference; its unavailable exact
requirements are not reconstructed.

The supersession is prospective only. It does not authorize submission, review,
acceptance, or execution and does not retroactively alter any historical record
or decision.

Evidence acceptance remains `NOT GRANTED`; technical execution remains
`NO-GO / SAFE-STOP`; and the broader workflow remains `SAFE-STOP`.

## 2. Purpose and boundary

This baseline defines the candidate-specific evidence-package
requirements for `U03-SUB-20260929-01`. It defines package content and package
completeness only. Package completeness is distinct from evidence sufficiency,
evidence acceptance, production-gap resolution, security disposition,
residual-risk acceptance, technical GO, and execution authorization.

P9-341 and P9-342 remain historical records and are not retroactively
re-authorized, completed, invalidated, or rewritten. P9-343 receives no
retroactive authorization. P9-345 remains the historical `INCOMPLETE` record
and is not rewritten, completed, validated, or retroactively altered.

All future requirement-to-evidence work for this path MUST use
`G145-U02-REB-01` v1.0 rather than the unavailable P9-339 requirements.

## 3. Normative requirements

### REQ-01 Package Identification

The package MUST identify this baseline by ID and version, identify candidate
`U03-SUB-20260929-01`, identify the package and submission version, identify the
package owner, and state the candidate-specific scope and exclusions. The
identification MUST be unambiguous and attributable.

### REQ-02 Requirement-to-Evidence Mapping

The package MUST map every mandatory requirement in this baseline to evidence
using exactly one of these mapping statuses: `SUBMITTED`, `MISSING`, or
`NOT APPLICABLE`. Every mandatory requirement MUST have at least one valid
`SUBMITTED` evidence mapping. A mandatory requirement itself cannot be
`NOT APPLICABLE`; that status applies only to an individual evidence mapping.
Every `NOT APPLICABLE` mapping MUST include an explicit, attributable
justification identifying its owner and basis. `NOT APPLICABLE` alone cannot
satisfy a mandatory requirement, and an unsupported assertion of
non-applicability is invalid. Any remaining `MISSING` mapping makes the package
`INCOMPLETE`.

### REQ-03 Evidence Item Identification

Each submitted evidence item MUST have a unique Evidence ID and MUST provide the
following minimum schema:

- Evidence ID
- name/type
- applicable requirement
- component / production gap
- provenance/source
- owner
- date/version
- identity/integrity information
- scope/applicability
- limitations
- item status/completeness
- status/completeness basis

An evidence item lacking any mandatory schema field MUST be identified as
incomplete and MUST NOT be treated as a valid evidence mapping.

### REQ-04 Provenance / Identity / Integrity

For each submitted evidence item, the package MUST provide attributable
provenance and source, owner, relevant date or version, and information adequate
to verify the item's identity and integrity. The package MUST expose ambiguity,
unverifiability, lack of attribution, inconsistency, and expiration or staleness
where relevant; it MUST NOT infer, reconstruct, substitute, or silently repair
missing information.

### REQ-05 Candidate-Scope / Production-Gap Coverage

Each evidence mapping MUST state the supported candidate scope, its applicability,
and what it does not support. The package MUST explicitly cover, as unresolved
coverage targets rather than as resolved facts, all three established production
gaps:

1. Template Derivation production component;
2. Manifest Derivation -> Template Derivation production connection; and
3. Generator output -> `AppBuildOutputWritePlan` production connection.

Coverage MUST NOT be represented as implementation, connection, correctness,
safety, completion, or resolution unless those conclusions are established and
accepted through their separate applicable gates.

### REQ-06 Missing Evidence / Limitations

The package MUST explicitly record every missing evidence item, unresolved matter,
unknown, applicability constraint, and limitation. Missing, ambiguous,
unverifiable, unattributable, inconsistent, expired or stale where relevant, or
otherwise unsupported mandatory information MUST fail closed to package status
`INCOMPLETE`. No missing information may be inferred, reconstructed, substituted,
or promoted to evidence.

The unresolved Avast causality, unavailable block-time definition/version, and
unaccepted residual risk MUST remain expressly recorded as limitations unless
separately established or accepted by the applicable authority. This baseline
does not establish or accept them.

### REQ-07 Review / Acceptance Record

The package MUST record the review outcome, reviewer identity and authority,
review date/version, acceptance decision, Acceptance Authority, decision scope,
conditions, and limitations, with attributable references. Under ADR-0020,
review and Acceptance are separate gates: Review does not equal Acceptance.
Acceptance and Execution authorization are also separate gates: Acceptance does
not equal Execution authorization. A missing, ambiguous, or unattributable gate
record MUST remain `NOT ESTABLISHED` and MUST NOT be inferred from another gate.

## 4. Package completeness rule

Package status is `COMPLETE` if and only if all of the following are true:

- `REQ-01` through `REQ-07` have all been evaluated;
- every mandatory requirement has at least one valid `SUBMITTED` mapping;
- zero `MISSING` mappings remain;
- every mandatory evidence field is present;
- mandatory evidence information is attributable and verifiable;
- every `NOT APPLICABLE` mapping has valid attributable justification; and
- all baseline coverage and completeness conditions are satisfied.

Otherwise, package status MUST be `INCOMPLETE`.

A `COMPLETE` package status establishes only structural completeness against
this baseline. It does not establish evidence sufficiency or acceptance,
production-gap resolution, security disposition, residual-risk acceptance,
technical GO, execution authorization, or release from `SAFE-STOP`.

## 5. Governance and security non-effects

This prospective supersession does not:

- reconstruct, recover, or validate the unavailable exact P9-339 requirements;
- retroactively re-authorize, complete, invalidate, or rewrite P9-341 or P9-342;
- grant retroactive authorization to P9-343;
- rewrite or retroactively complete P9-345;
- accept evidence or establish evidence sufficiency;
- resolve any production gap;
- establish Avast causality or the block-time definition/version;
- accept residual risk or alter the security HOLD;
- grant continuation, command, technical-GO, or execution authority; or
- release technical execution from `NO-GO / SAFE-STOP`.

## 6. Basis and use restriction

This adopted baseline is based on `G145-U02-REB-01-DESIGN-01` v1.0, the explicit Design
Authority Record for the User-adopted `G145-U02-REB-01` baseline design and
design supplements 1 through 4; ADR-0020's authority and gate separation; the
selected candidate boundary in P9-328; the U02 preparation boundary in P9-337;
and independently supported package concepts from P9-267, P9-332, P9-341, and
P9-342. P9-341's package structure was used only where independently supported
by those approved design inputs; unavailable P9-339-dependent wording was not
imported as authority.

Canon v2.0 and SpecificationHierarchy v2.0 remain higher-authority constraints.
This lower-level baseline does not amend or redefine them, ADR-0020, any Frozen
specification, public contract, persisted schema, canonical format, or
architectural boundary.
