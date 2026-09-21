# P9-167 Owner Decision Population Assistance

## 1. Record status and boundary

- Work item: `P9-167`
- Activity: docs-only population assistance for the P9-166 `OWNER DECISION REQUIRED` fields
- Source basis: `docs/spec/P9-166_OwnerDecisionSubmissionDraft.md`, `docs/spec/P9-165_OwnerFieldPopulationDecisionRequest.md`, and `docs/spec/P9-164_OwnerRequiredFieldsPopulationAssistance.md`
- Work mode: `docs-only`
- Document status: `COMPLETE / docs-only assistance / NO OWNER DECISION / NOT SENT`
- P9-166 population state: `NOT PERFORMED`
- Owner decision state: `OWNER DECISION REQUIRED / NOT RECEIVED`
- Submission state: `NOT AUTHORIZED / NOT SUBMITTED`
- Transmission state: `WITHHOLD / SAFE-STOP`
- Execution-instruction state: `NOT ISSUED`
- Technical execution state: `NO-GO / SAFE-STOP`

This record explains how an accountable human owner may complete P9-166 without supplying, inferring, recommending, or selecting any owner-controlled answer. Every P9-166 `OWNER DECISION REQUIRED` marker remains unchanged. The options below are decision structures, not decisions, values, approvals, authority, evidence, attestations, signatures, submission authorization, or permission to populate P9-163.

## 2. Assistance rules

For every decision, the owner must provide one attributable selection, its authoritative basis, the affected OR IDs or approval category, and the resulting hold or release effect. The owner must not use a blanket answer where P9-166 requires per-ID or per-category treatment.

Codex or a reviewer must not:

1. choose an option, recommend the substantive outcome, or treat silence as agreement;
2. infer identity, role, authority, scope, value, evidence, timestamp, approval, attestation, signature, destination, or authorization from repository history, examples, job titles, access, or prior drafts;
3. copy an example as an owner value, convert blank to `NONE`, resolve inconsistencies, invent missing OR IDs, or repair an authority chain;
4. populate P9-166 or P9-163, submit the decision set, contact an owner, transmit externally, test a route, or perform technical execution; or
5. treat completion, submission, receipt, review, or documentary acceptance as transmission authorization, an execution instruction, security disposition, continuation authorization, technical GO, or execution permission.

## 3. Ten explicit approval categories

Each category requires a separate `APPROVE` or `DO NOT APPROVE` selection. `APPROVE` requires all listed detail and evidence; `DO NOT APPROVE` requires the blocking item, accountable authority, and reconsideration condition. `CONDITIONAL`, `PARTIAL`, blanket approval, or an omitted row is not a completed selection.

| No. | Approval category | Owner-selectable options | Required basis and decision impact | Non-recommended completion | Fail-closed condition |
|---:|---|---|---|---|---|
| 1 | Authoritative slot count and count disposition | `APPROVE` one completed section 5 option; or `DO NOT APPROVE` | Approval establishes only the owner-selected count treatment for later review. Non-approval or missing authority keeps FC-9 and OR-097 unresolved. | Repeating `105` or `102` without the required reconciliation | More than one option, no option, missing authority, or incomplete detail |
| 2 | Final per-slot classifications | `APPROVE` all four classes and each stated change; or `DO NOT APPROVE` with disputed IDs | Approval makes the classification set eligible for later review; it does not populate a slot. | Aggregate approval while an OR ID is disputed | Any unidentified dispute, unmapped change, or missing impact analysis |
| 3 | Exact values and continued holds | `APPROVE` every eligible value and every ineligible hold; or `DO NOT APPROVE` | Approval identifies owner-supported values and holds for later review only. | Filling unavailable data with examples, guesses, blanks, or `NONE` | Any slot lacks an exact disposition, source, prerequisite, or hold treatment |
| 4 | Authority holders and bounds | `APPROVE` every holder/source/scope/limit/time; or `DO NOT APPROVE` | Approval supplies the authority chain for later documentary review. | Inferring authority from title, access, repository ownership, or shared accounts | Unattributable, incomplete, conflicting, stale, or unbounded authority |
| 5 | Exact seven-input set | `APPROVE` the internally consistent set; or `DO NOT APPROVE` | Approval identifies a candidate set only; it does not authorize sending. | Treating labels, examples, alternates, or mutable body text as exact inputs | Any input missing, ambiguous, inconsistent, mutable, or unmapped |
| 6 | Evidence inventory and mappings | `APPROVE` direct accessible evidence and mappings; or `DO NOT APPROVE` | Approval makes the evidence set eligible for later review; it is not evidence acceptance. | Evidence dump, inaccessible reference, reviewer-created mapping, or route test | Missing evidence, inaccessible evidence, or any fact without an exact mapping |
| 7 | Attachments, dependencies, conditions, exclusions, and conflicts | `APPROVE` exact lists/resolutions or expressly permitted checked `NONE`; or `DO NOT APPROVE` | Approval records the owner's declarations for later review. | Blank-as-none, implicit dependency, or reviewer conflict resolution | Unchecked `NONE`, unresolved conflict, or unmapped item |
| 8 | Consistency attestation and operational restrictions | `APPROVE` the complete closed set; or `DO NOT APPROVE` | Approval covers consistency, single-use restrictions, attempt/failure rules, record schema, custody, and custodian; it is not an execution instruction. | Generic “standard controls apply” or reusable/open-ended authority | Partial attestation, undefined attempt rule, missing schema/custody/custodian, or inconsistency |
| 9 | Submission action and controls | `APPROVE` one exact bounded submission action; or `DO NOT APPROVE` | Approval may make a later separate submission act eligible; it does not perform that act. | “Submit as appropriate,” unspecified destination, unlimited retries, or delegated choice | Missing action, destination, submitter, single-use scope, attempt limit, validity, or record |
| 10 | Separation acknowledgement | `APPROVE` the complete boundary statement; or `DO NOT APPROVE` | Approval acknowledges that submission and later review do not cross independent safety or execution gates. | Qualified acknowledgement or merging distinct authorization gates | Any omitted boundary or statement implying submission creates technical authority |

All ten rows must be decided independently. Approval of nine rows, a single signature over the table, or one general statement leaves the complete decision set `OWNER RESPONSE INCOMPLETE / NOT ACCEPTED / SAFE-STOP`.

## 4. Four-class OR ID decisions

The P9-164 census remains 21 `owner入力可能`, 41 `owner判断必要`, 17 `未入力維持`, and 23 `受理不可リスク` slots. P9-167 does not change the classifications or resolve the 105-versus-102 discrepancy.

| Classification | Required owner choice | Required detail and impact | Non-recommended completion | Fail-closed condition |
|---|---|---|---|---|
| `owner入力可能` | For each of 21 IDs: `AVAILABLE` or `NOT AVAILABLE` | `AVAILABLE` requires exact value and attributable source; `NOT AVAILABLE` requires reason, authority, prerequisite, and reconsideration condition. Either choice remains subject to later review; neither authorizes population. | Group answer, copied example, guessed value, or availability without source | Any ID omitted, source absent, or unavailable state lacking its hold basis |
| `owner判断必要` | For each of 41 IDs: state the exact selection, approval, adoption, acknowledgement, limit, or declaration; otherwise `NO DECISION` | Identify authority holder/basis, scope, limitations, effective time, and expiry. `NO DECISION` retains the slot unpopulated. | Reviewer-selected alternative, blanket answer, implicit approval, or open-ended scope | Any ID omitted, decision unattributable, or authority/bounds incomplete |
| `未入力維持` | For each of 17 IDs: `RETAIN HOLD` or `PROPOSE RELEASE` | `PROPOSE RELEASE` requires the exact prerequisite, direct evidence, and separate authority; it is not release, population, or submission. | Estimated act/time, route probe, generated evidence, proxy signature, or implied submission action | Prerequisite or separate authority absent, evidence unavailable, or release treated as completed |
| `受理不可リスク` | For each of 23 IDs: `REMEDIATE AND PRESENT FOR REVIEW` or `RETAIN NOT ACCEPTABLE` | Remediation requires exact content, human authority, direct evidence, mappings, dependencies/conflicts/exclusions, and all-or-nothing acknowledgement. Risk acceptance cannot waive FC-1 through FC-9. | Accepting risk instead of curing the defect, circular reference, inferred mapping, or evidence-free assertion | Any defect remains, mapping conflicts, evidence is inaccessible, or waiver is attempted |

If the owner disputes a classification, the response must identify every affected OR ID, replacement classification, authoritative basis, prerequisite effect, and acceptance-risk effect. Until separately reviewed, a disputed ID remains unpopulated and fail-closed.

## 5. Count-discrepancy choice

The owner must select exactly one of the following. P9-167 does not recommend a substantive option.

| Select exactly one | Required owner completion | Decision impact | Non-recommended completion | Fail-closed condition |
|---|---|---|---|---|
| `CONFIRM-102` | Explain why 102 is authoritative, why 105 is superseded, identify the accountable authority, and identify any references eligible only for a later separately instructed docs-only correction | Makes 102 eligible to be treated as the authoritative census after separate review; does not itself correct another record | Selecting 102 because it matches the current table without authoritative basis | Missing supersession rationale, authority, or correction boundary |
| `IDENTIFY-ADDITIONAL-SLOTS` | Identify exactly three genuine slots by section, field label, required value, classification, prerequisite, acceptance effect, and authority | Makes a 105-slot census eligible for separate review; does not insert or populate the slots | Counting headings, explanatory references, or duplicate markers as slots | Fewer/more than three, invented fields, duplicates, or incomplete classification/basis |
| `RETAIN-DISCREPANCY` | State that no authoritative reconciliation is available, identify the accountable authority, and expressly retain incompleteness | Keeps FC-9 and OR-097 unresolved and prohibits any completeness claim | Selecting this option while also approving completeness | Missing express incomplete disposition or conflicting approval |

Selecting none, more than one, or a hybrid retains `UNRESOLVED / FC-9 / SAFE-STOP`.

## 6. Owner attribution

| Required field | Acceptable owner completion | Non-recommended completion | Decision impact / fail-closed condition |
|---|---|---|---|
| Accountable human owner name | Exact attributable human identity | Team, shared account, Codex, reviewer, or repository identity | Without one human owner, every decision is unattributable and incomplete |
| Role / organization | Exact current role and organization | Job-title inference or historical role | Missing/mismatched role prevents authority assessment |
| Authority source and exact scope | Direct source plus scope, limits, effective time, and expiry | Authority inferred from access, seniority, or document possession | Missing or unbounded authority retains all decisions unaccepted |
| Decision timestamp | Actual decision date, time, and timezone | Draft time, estimated time, local-time assumption, or backdating | Missing bound timestamp prevents currency and validity review |
| Reviewed-record identities | Exact immutable/version identifiers for P9-165, P9-164, and P9-163 | “Latest,” filename alone where mutable, or inferred Git history | Ambiguous basis prevents confirmation of what was decided |
| Submission identifier | Exact unique identifier bound to this decision set | Reused, mutable, or circular identifier | Missing/duplicate identifier prevents set-level attribution |

No field may be completed by proxy. A mismatch among owner identity, authority, reviewed records, timestamp, or submission identifier makes the whole set internally inconsistent.

## 7. Attestation

The owner may select `ATTEST` only after sections 2 through 9 of P9-166 are fully owner-completed. Otherwise the only safe choice is `DO NOT ATTEST / INCOMPLETE` with the blocking items identified.

`ATTEST` must be an owner-authored statement that the decision set is complete, accurate, attributable, current, internally consistent, non-placeholder, within the owner's authority, and subject to all-or-nothing `NOT ACCEPTED / INCOMPLETE / SAFE-STOP` for any unresolved or defective item. It must be bound to an actual owner signature or exact attributable identity act and actual date, time, and timezone.

Template adoption, a copied statement, proxy signature, typed name without a binding identity act, pre-dated or estimated time, partial attestation, or attestation while a required marker remains is not recommended and is not complete. An absent, unsigned, unbound, unauthorized, stale, qualified, or internally inconsistent attestation retains fail-closed state.

## 8. Submission authorization

The owner may select either:

1. `AUTHORIZE ONE BOUNDED SUBMISSION FOR SEPARATE DOCS-ONLY REVIEW`, supplying the exact completed decision-set identifier, exact action, exact destination, named authorized submitter, single-use scope, positive attempt limit, treatment of a failed attempt, bounded validity window, required submission-record schema, custody location, and accountable custodian; or
2. `DO NOT AUTHORIZE SUBMISSION`, identifying any blocker and retaining `NOT SUBMITTED / SAFE-STOP`.

P9-167 does not recommend authorization. “Send,” “submit normally,” “use the usual route,” an unspecified or alternate destination, operator discretion, unlimited retry, open-ended validity, or a missing record/custodian is not a valid bounded authorization. Any missing element, mismatch with the attested decision set, expired window, exceeded/ambiguous attempt limit, route substitution, or lack of a separately attributable owner authorization retains `NOT AUTHORIZED / NOT SUBMITTED / SAFE-STOP`.

Even a valid completed authorization does not perform submission. Submission requires a separate authorized act and preserved record. Receipt does not mean acceptance; review requires separate authorization; any later P9-163 population requires a separately bounded docs-only instruction; documentary acceptance does not authorize transmission; and actual transmission would still require an independent, exact, current, owner-issued, single-use P9-158 execution instruction.

## 9. Consolidated fail-closed conditions and non-actions

The whole decision set remains `OWNER RESPONSE INCOMPLETE / NOT ACCEPTED / SAFE-STOP` if any `OWNER DECISION REQUIRED` marker remains; any required OR ID or approval category is omitted; any selection is blanket, partial, contradictory, inferred, unattributable, unauthorized, stale, expired, placeholder-based, unsupported, or unmapped; the count discrepancy is unresolved; an attestation or identity binding is defective; or submission authorization is absent or incomplete.

P9-167 supplied no owner decision and performed no P9-166 or P9-163 field population, owner contact, draft delivery, submission or resubmission, send action, external transmission, route/access test, external-service access, package review or acceptance, evidence creation or acceptance, gap closure, instruction issuance, build, test, project script or runner execution, Excel operation, Avast operation, package / `dist`, release, publication, tag, technical execution, Git staging, commit, or push.

P9-166 remains `OWNER COMPLETION REQUIRED / NOT SENT`; P9-163 remains `UNPOPULATED / INCOMPLETE / SAFE-STOP`; owner resubmission remains `NOT SUBMITTED`; package acceptance remains `NOT ASSESSED`; gap closure remains `PLANNED / NOT CLOSED`; transmission-execution eligibility remains `NOT ESTABLISHED / SAFE-STOP`; transmission inputs remain `INCOMPLETE / SAFE-STOP`; transmission remains `WITHHOLD / SAFE-STOP`; the notice remains `NOT SENT`; the execution instruction remains `NOT ISSUED`; all eleven gaps remain `OPEN / UNRESOLVED / SAFE-STOP`; all five inventory items remain `INCOMPLETE / SAFE-STOP`; Avast detection remains unresolved; P9-94 remains non-reusable; P9-130 and P9-135 remain non-rerunnable; and technical execution remains `NO-GO / SAFE-STOP`.
