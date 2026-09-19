# P9-148 Documentary Gap-Intake Acceptance Criteria

## 1. Record status

- Work item: `P9-148`
- Activity: documentary gap-intake acceptance-criteria drafting
- Selected path: P9-147 selection of P9-146 candidate `N-146-03`
- Source basis: `docs/spec/P9-145_UnresolvedGapRegister.md` and `docs/spec/P9-146_CrossTraceabilityIndex.md`
- Work mode: `docs-only`
- Document status: `COMPLETE / docs-only documentary gap-intake acceptance criteria / ACCEPT`
- Revised draft review: `ACCEPT`
- Technical execution state: `NO-GO / SAFE-STOP`
- Evidence status: No owner input or evidence was requested, received, generated, refreshed, revalidated, reviewed, or accepted.

This accepted record defines only the minimum documentary form and fail-closed checks for a possible future owner submission. `ACCEPT` applies only to the document content. It does not solicit, supply, validate, or accept a submission. It does not resolve, waive, downgrade, accept, or close a gap; complete an inventory item; accept evidence; establish technical truth or control effectiveness; provide a security disposition; select a technical candidate; authorize continuation or a command; make a technical execution GO decision; or issue an execution instruction.

## 2. Controlling premises

- Technical execution remains `NO-GO / SAFE-STOP`.
- All eleven P9-145 gaps remain `OPEN / UNRESOLVED / SAFE-STOP`.
- All five P9-144 inventory entries remain `INCOMPLETE / SAFE-STOP`.
- Avast detection remains unresolved.
- P9-94 remains non-reusable as authority, evidence, precedent, substitute, or present decision basis.
- P9-130 and P9-135 remain non-rerunnable and outside every rerun candidate, including renamed, partial, wrapped, equivalent, reconstructed, approximated, or materially similar operations.
- P9-147 selected only `N-146-03` as a later docs-only path. The present instruction begins P9-148 drafting only; it supplies no technical execution authority.
- P9-146 `ML-146-01` through `ML-146-10` are local missing-link index labels. They do not replace, renumber, resolve, accept, or close the eleven controlling P9-145 gaps.
- Missing, ambiguous, inconsistent, stale, out-of-scope, unattributable, unauthorized, or barred material is never completed or inferred.

## 3. Intake disposition vocabulary

P9-148 defines the following possible dispositions for a later, separately authorized documentary intake review:

- `ACCEPTED FOR DOCUMENTARY INTAKE` means only that the submission satisfies the applicable form, attribution, scope, source-linkage, date/version, and consistency checks in this document and may be routed to a separate substantive review.
- `NOT ACCEPTED FOR DOCUMENTARY INTAKE` means that no conflict exists but one or more other mandatory intake checks are absent, ambiguous, unsupported, stale, unauthorized, or barred.
- `CONFLICT-HOLD / SAFE-STOP` means that the submission conflicts with an authoritative source, another submitted field, a controlling boundary, or another claimed authority and must not be reconciled by inference.

These three dispositions are mutually exclusive. A submission receives exactly one disposition under the following precedence: any conflict produces `CONFLICT-HOLD / SAFE-STOP`; when no conflict exists, complete satisfaction of all applicable intake criteria produces `ACCEPTED FOR DOCUMENTARY INTAKE`; when no conflict exists but any other mandatory criterion is unsatisfied, the result is `NOT ACCEPTED FOR DOCUMENTARY INTAKE`. A submission on `CONFLICT-HOLD / SAFE-STOP` must not be treated as accepted for documentary intake.

These dispositions are not `PASS`, evidence acceptance, gap resolution, inventory completion, technical validation, security clearance, control-effectiveness confirmation, readiness, continuation authorization, command authorization, GO, or execution authority. A submission accepted for documentary intake leaves every referenced gap `OPEN / UNRESOLVED / SAFE-STOP` until a later, separately authorized record performs an expressly permitted substantive disposition. P9-148 performs none.

## 4. Required owner input format

A future owner submission must be a bounded documentary record containing every field below. One submission may address multiple gap IDs only when each gap has a complete, independently reviewable entry and shared statements are explicitly mapped to every applicable entry.

| Field | Required content | Intake boundary |
|---|---|---|
| Submission ID | A unique documentary identifier supplied by the submitting authority. | Uniqueness is checked only within the submitted package; no repository or external lookup is implied. |
| Submission date | Exact date in `YYYY-MM-DD` form. | The intake date is not a source-record date, review date, validity date, or technical-event date. |
| Controlling gap ID | Exactly one or more P9-145 IDs: `G-145-MD06`, `G-145-MD07`, `G-145-MD17`, or `G-145-U01` through `G-145-U08`. | P9-144 predecessor IDs and P9-146 ML labels may be cross-references but cannot replace a controlling P9-145 gap ID. |
| P9-146 linkage | Every applicable `ML-146-01` through `ML-146-10` label, or an explicit `none` with rationale. | An ML label is traceability only and is not a gap or an acceptance result. |
| Exact affected scope | Exact source record, inventory entry, candidate, control, command, evidence package, decision record, or proposed new bounded activity addressed. | Generic, category-level, inferred, or unnamed scope is not accepted. |
| Owner or authority | Named accountable owner or exact authoritative role, the decision or statement being supplied, and the authority basis. | Authorship, file location, workflow participation, or title alone does not establish authority. |
| Source basis | Exact authoritative source identifiers and the specific statements relied upon. | A bare path, unsupported assertion, template, or documentary requirement is insufficient as substantive support. |
| Supplied value or decision | The exact value, statement, decision, limitation, or `not applicable` determination, without omitted mandatory subfields. | `Not applicable` requires an attributable authority basis and exact scope. |
| Dates, versions, and validity | Every date, version, effective period, expiry, review date, or authoritative no-version statement required by the applicable row in section 6. | Dates and versions must not be reconstructed from file metadata, chronology, or surrounding records. |
| Conditions and exclusions | Preconditions, residual limitations, exceptions, dependencies, exclusions, and prohibited uses. | Silence is not treated as unrestricted authority. |
| Conflict declaration | Identified conflicts with authoritative sources or other submissions, or an explicit statement that none are known within the declared review scope. | A no-conflict statement does not prove global consistency. |
| Attestation | Attribution, exact attestation date, and a statement that the submitter is acting within the cited authority. | A signature or name without authority and scope is insufficient. |

Attachments or referenced materials remain opaque documentary references at intake unless a later instruction expressly authorizes their review. Intake must not discover, open, execute, hash, parse, render, scan, extract, generate, refresh, revalidate, or technically inspect an artifact.

## 5. Common acceptance criteria

A future submission may receive `ACCEPTED FOR DOCUMENTARY INTAKE` only when all of the following are true:

1. every required field in section 4 is present and legible;
2. each entry names at least one exact controlling P9-145 gap ID and preserves the P9-146 ML label as secondary traceability only;
3. the affected source record, inventory entry, candidate, control, command, package, decision, or activity is exact and bounded;
4. the owner or authoritative role, supplied statement, authority basis, and attestation are attributable and mutually consistent;
5. required values, decisions, dates, versions, validity periods, conditions, exclusions, and `not applicable` bases are explicit rather than inferred;
6. the submission cites its authoritative sources precisely enough for a later separately authorized documentary review;
7. shared material is mapped explicitly to each affected gap and does not conceal a gap-specific missing field;
8. no unresolved internal contradiction or conflict with the declared authoritative source set, another submitted field, a controlling boundary, or another claimed authority exists;
9. the submission does not rely on P9-94, propose a rerun or equivalent of P9-130 or P9-135, or inherit authority from a prior activity;
10. the submission makes no technical-execution, evidence-acceptance, security-clearance, control-effectiveness, inventory-completion, GO, or execution claim beyond the exact documentary statement within the owner's authority; and
11. accepting the form for intake requires no external contact, artifact inspection, technical validation, command execution, or missing-value reconstruction.

Intake acceptance is atomic per controlling gap entry. Partial satisfaction of a row is not partial acceptance and does not reduce the gap state.

## 6. Gap-specific criteria and P9-146 relation

| Controlling P9-145 gap | P9-146 relation | Mandatory gap-specific owner input | Accepted for documentary intake only when | Not accepted / fail-closed when |
|---|---|---|---|---|
| `G-145-MD06` | `ML-146-01` | For each affected `INV-144-01` through `INV-144-05`, the exact record owner or authoritative owner role and authority basis. | All five entries are addressed individually; attribution and authority are explicit. | Any entry, owner/role, or authority basis is missing, inferred, or ambiguous. |
| `G-145-MD07` | `ML-146-02` | For each affected inventory entry, the exact source-record date and exact version, or an authoritative no-version statement with basis. | All five entries have exact attributable values or justified no-version statements. | A value is reconstructed, partial, conflicting, unattributable, or confused with an inventory/review date. |
| `G-145-MD17` | `ML-146-03` | For each affected inventory entry, exact last-review date, result, reviewer or reviewing authority, and authority basis. | All four elements are present and attributable for all five entries. | `ACCEPT` alone is supplied, or any date, result, reviewer/authority, or basis is absent or inconsistent. |
| `G-145-U01` | `ML-146-08` | Candidate-specific attributable security-owner disposition identifying the exact candidate, Avast detection, scope, basis, residual risk, conditions, decision, validity, and authority. | The documentary submission is complete for one exact candidate and does not merge security, continuation, or technical-decision authority. | The candidate is unnamed; Avast is treated as resolved by inference; authority, residual risk, validity, or conditions are missing; or the submission claims execution clearance. |
| `G-145-U02` | `ML-146-08` | For one exact candidate, an attributable evidence-package submission mapped to governing evidence requirements, with provenance, dates, scope, and responsible authorities. | The package is exactly identified and the documentary mapping is complete without treating references as reviewed evidence. | Material is missing, stale, barred, unattributable, technically uninspected, or presented as accepted evidence through intake. |
| `G-145-U03` | `ML-146-08` | Attributable owner decision naming one exact technical candidate, purpose, scope, exclusions, dependencies, and decision authority after prerequisite gates are independently addressed. | One exact candidate and every required boundary field are explicit; independent gates remain separate. | Candidate identity or scope is inferred, generic, conditional on unreviewed material, or used to generalize another submission. |
| `G-145-U04` | `ML-146-06` | For each relied-upon RC control and one exact candidate, attributable control-owner statements and referenced evidence of implementation, operation, scope, exceptions, and effectiveness review. | Each claimed control is mapped separately and all documentary fields are present; intake does not judge evidence or effectiveness. | Documentary control text is treated as implementation/effectiveness, or candidate, owner, evidence reference, scope, exceptions, or review is missing. |
| `G-145-U05` | `ML-146-07` and `ML-146-09` | Only after prerequisite gates: an attributable authority decision for one exact candidate and exact command; any later execution instruction must remain a separate post-GO record. | The authorization submission is exact and single-scope, and no execution instruction is combined with or inferred from it. `ML-146-09` remains not issuable before valid GO. | A command is listed rather than authorized; wording is generic, modified, wrapped, reconstructed, or category-level; or authorization and execution instruction are merged. |
| `G-145-U06` | `ML-146-08` and `ML-146-09` | For one exact candidate, a fully populated attributable future-decision submission containing eligible EV-01 through EV-08 inputs and independent security, continuation, and technical-decision authorities; EV-09 remains separate. | Every required documentary field is populated, independent authorities remain distinct, and intake makes no GO finding. | A template or intake result is treated as evidence or GO; a field is inferred; or EV-09 is supplied before a valid GO. |
| `G-145-U07` | Scope dependency supporting `ML-146-04` and `ML-146-05`; neither ML label is a replacement gap | Attributable owner scope decision for any future inventory extension: exact documentary source set, purpose, permitted metadata handling, exclusions, and authority. Existing MD inputs remain separate. | The proposed extension is exact, documentary, bounded, and does not claim repository-wide completeness or cure an existing MD gap. | The proposal requires discovery or technical inspection, asserts global absence/existence, admits P9-94, or treats NI-05/NI-08 absence as resolved. |
| `G-145-U08` | `ML-146-10` | For a genuinely new bounded activity only: new authoritative grounds, exact scope, exclusions, and authority, with an explicit non-reliance statement for P9-94 and non-rerun statement for P9-130/P9-135. | The activity is demonstrably framed as new and documentary at intake, and all prohibited reuse/equivalence routes are expressly excluded. | Any reuse, inheritance, rerun, reconstruction, approximation, equivalence mapping, or materially similar route is present or ambiguous. |

`ML-146-04` and `ML-146-05` record missing eligible primary entries within P9-144's fixed source set. They do not add gaps to the P9-145 count. A future U-07 scope submission may define a proposed source-set extension for later review, but intake acceptance cannot establish that an eligible item exists, complete NI-05 or NI-08, or complete an inventory entry.

## 7. Not-accepted criteria

A future submission must be `NOT ACCEPTED FOR DOCUMENTARY INTAKE` when no conflict exists but any of the following applies. If a conflict exists, `CONFLICT-HOLD / SAFE-STOP` takes precedence and the submission must not receive `NOT ACCEPTED FOR DOCUMENTARY INTAKE` or `ACCEPTED FOR DOCUMENTARY INTAKE`:

- no exact P9-145 controlling gap ID is supplied;
- an ML label, predecessor gap, inventory ID, control ID, allowlist ID, or evidence ID is presented as a substitute for the controlling gap;
- a mandatory field or gap-specific element is omitted, redacted without an attributable basis, illegible, or expressed only by implication;
- scope, candidate, command, control, source record, inventory entry, evidence package, decision, or activity is unnamed or broader than the cited authority;
- attribution, authority basis, attestation, date, version, validity, condition, exclusion, provenance, or source linkage is missing or ambiguous without creating a conflict;
- a date/version is reconstructed or an unknown value is converted to `not applicable`;
- one owner purports to supply a decision reserved to a separate independent authority;
- the submission depends on an external lookup, technical artifact inspection, executable validation, or a prohibited operation to become complete;
- it relies on P9-94 or any rerun, reconstruction, approximation, equivalent, or materially similar route for P9-130 or P9-135; or
- it asks intake acceptance to establish evidence acceptance, technical truth, control effectiveness, security clearance, inventory completion, readiness, authorization, GO, or execution.

Rejection at intake does not invalidate an owner's underlying authority or decide the substantive question. It means only that the submitted documentary package cannot proceed under these criteria and remains fail-closed.

## 8. Fail-closed handling

1. No missing field may be filled from context, filenames, chronology, authorship, repository location, prior practice, or another gap entry.
2. No inconsistency may be resolved by choosing the more permissive statement. Use `CONFLICT-HOLD / SAFE-STOP`.
3. No stale or expired statement may be refreshed by restatement or by the passage of time.
4. No partial submission may receive partial credit, partial resolution, provisional `PASS`, or state promotion.
5. No intake reviewer may broaden the declared source set, inspect a technical artifact, contact an external service, or execute a command to cure the package.
6. No documentary intake disposition may alter the eleven-gap count or the five inventory-entry dispositions.
7. No accepted intake package may proceed directly to execution. Any substantive review, gap disposition, evidence review, security disposition, continuation decision, command authorization, GO decision, or execution instruction requires its own explicit authority and record.
8. Any prohibited reuse or rerun route requires immediate `SAFE-STOP` without equivalence analysis intended to rescue the route.

## 9. Relation to P9-145 and P9-146

P9-145 remains the controlling eleven-gap register. Its required-owner-input and fail-closed statements define what remains missing; P9-148 only normalizes the documentary form and minimum routing checks for a hypothetical future submission. P9-148 does not amend P9-145 and does not change any gap state.

P9-146 remains the cross-traceability and missing-link index. Its ML labels supply secondary routing context only. P9-148 preserves the distinction between the ten ML labels and the eleven controlling gaps, including the grouped relationships in `ML-146-08` and `ML-146-09` and the fixed-source-set conditions in `ML-146-04` and `ML-146-05`.

If P9-145 and P9-146 appear inconsistent during a later intake, the controlling gap identity and state come from P9-145, while P9-146 contributes traceability only. The submission must be placed on `CONFLICT-HOLD / SAFE-STOP`; P9-148 supplies no authority to amend either source.

## 10. Review result, closeout state, and non-actions

The P9-148 revised draft review result is `ACCEPT`. P9-148 is `COMPLETE / docs-only documentary gap-intake acceptance criteria / ACCEPT`. `ACCEPT` applies only to the content of this documentation record and confirms that `ACCEPTED FOR DOCUMENTARY INTAKE`, `NOT ACCEPTED FOR DOCUMENTARY INTAKE`, and `CONFLICT-HOLD / SAFE-STOP` are mutually exclusive documentary intake dispositions. It is not acceptance of any actual or hypothetical owner submission, gap resolution, inventory completion, evidence acceptance, technical validation, control-effectiveness confirmation, security disposition, continuation authorization, command authorization, a technical execution GO decision, or an execution instruction.

All eleven P9-145 gaps remain `OPEN / UNRESOLVED / SAFE-STOP`. All five P9-144 inventory entries remain `INCOMPLETE / SAFE-STOP`. No gap is resolved, partially resolved, waived, downgraded, accepted, or closed. No inventory entry is promoted to `COMPLETE`, `PASS`, accepted evidence, technically verified, or execution-ready.

No test, build, script, PowerShell, Excel operation, Avast operation, external-service access, artifact discovery, technical artifact inspection, evidence generation, evidence refresh, evidence revalidation, evidence acceptance, control implementation, control-effectiveness review, parser, macro, runner, package, `dist`, release, publication, tag, Git mutation, technical execution, security disposition, continuation authorization, command authorization, technical execution GO decision, or execution instruction was performed or authorized by P9-148.

Technical execution remains `NO-GO / SAFE-STOP`. Avast detection remains unresolved. P9-94 remains non-reusable. P9-130 and P9-135 remain non-rerunnable and outside all rerun candidates.
