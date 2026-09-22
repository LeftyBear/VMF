# P9-233 Docs-only Current Routing Summary Maintenance Rule Review

## 1. Status and scope

- Work item: `P9-233`
- Basis: `P9-232 Docs-only Current Routing Summary Consolidation Review`
- Work mode: `docs-only`
- Final status: `COMPLETE / docs-only current routing summary maintenance rule review`
- Technical execution: `NO-GO / SAFE-STOP`

This review defines the non-semantic maintenance rule for the Current Routing
Summary synchronized across `docs/development/CURRENT_STATUS.md`,
`docs/development/HANDOFF.md`, and `docs/VMF_vNext_Backlog.md`. It preserves
detailed P9 records as history and creates no evidence, verification result,
repository-state proof, authority, hold clearance, or technical permission.

## 2. Update conditions

Update the Current Routing Summary only when a separately instructed docs-only
work item changes at least one of the following current-routing facts:

1. the latest completed docs-only routing work item and its documentary result;
2. the controlling navigation or detailed-basis record;
3. the current treatment of a routing classification;
4. an applicable hold, non-finding, evidence boundary, or authorization gate; or
5. the single selected next docs-only candidate.

Do not update the Current Routing Summary merely because a historical record is
read, restated, or retained. A proposed, requested, drafted, submitted, or
selected item does not become current authority by appearing in a summary.
Missing, ambiguous, stale, expired, consumed, inconsistent, or non-reusable
input requires fail-closed retention of the existing route.

## 3. Three-document synchronization rule

Whenever an update condition in section 2 is satisfied, update all three
routing documents in the same docs-only work item:

| Document | Required current content |
| --- | --- |
| `CURRENT_STATUS` | Current documentary decision, safety state, controlling boundaries, navigation, and the single next candidate. |
| `HANDOFF` | Operator-facing continuation conditions, explicit stop conditions and prohibited operations, navigation, and the same single next candidate. |
| Backlog | The same Current Routing Summary plus `Current Routing Classification`, including completed history, continuing holds, blocked technical work, external-authorization work, active docs-only mainline, and the same single next candidate. |

Each document must contain one clearly identified current summary at its top.
The three summaries may differ in operator-facing wording, but their work-item
status, safety state, classification treatment, authority boundary, detailed
basis, and next-candidate identity must agree. Detailed historical entries
remain unchanged unless a separate narrow instruction expressly requires their
maintenance.

If the three documents cannot be synchronized without changing a historical
disposition, Frozen specification, public contract, evidence status, hold, or
authorization boundary, stop with `NO-GO / SAFE-STOP` and do not partially
update the current route.

## 4. Single-next-candidate rule

The synchronized route may name exactly one next docs-only candidate. The
candidate must be non-executable, narrowly described, and separately
instructed before it starts. Selection is not approval, start, execution
authority, or permission for any later operation. All other possible docs-only
items remain unselected and are not listed as parallel current candidates.

When no safe docs-only candidate exists, the route must state `None` rather
than infer or revive an older candidate.

## 5. Historical-language and authority rule

Older `Next` statements, candidate selections, requests, drafts, intakes,
submissions, templates, owner-input placeholders, and documentary
`COMPLETE`/`ACCEPT` results are retained provenance only. They must not be
copied, promoted, combined, or interpreted as current evidence, verification,
`PASS`, repository-state proof, approval, or authority.

P9-223a and P9-230a remain isolated procedural-deviation records. Their
occurrences and outputs remain unusable as evidence, verification, artifact
verification, `PASS`, repository-state proof, or authority.

## 6. Routing-classification maintenance rule

Maintain the following classifications as independent states:

| Classification | Maintenance rule |
| --- | --- |
| `DOCS-ONLY CONTINUATION` | May contain only the one separately instructed non-executable documentation candidate. It cannot inherit or clear a technical or external gate. |
| `BLOCKED TECHNICAL` | Retains every prohibited technical, artifact, repository-state, Git-inspection, implementation, diagnostic, execution, and release path as `NO-GO / SAFE-STOP` until its own valid decision process changes it. |
| `EXTERNAL AUTHORIZATION REQUIRED` | Retains operations needing a current attributable owner, security, vendor, external-service, release, publication, or Git authorization. A docs-only result cannot supply that authorization. |

Do not move an item between classifications by implication. Any reclassification
requires a separate explicit work item, its applicable prerequisites, and a
synchronized update under sections 2 and 3.

## 7. Single next docs-only candidate

The single next candidate is:

`P9-234 Docs-only Current Routing Summary Maintenance Rule Checkpoint Review`

Under a separate explicit docs-only instruction, P9-234 may review whether the
P9-233 rule is represented consistently in the three routing documents and
whether the current route still names exactly one next docs-only candidate. It
must not perform Git or repository-state inspection, artifact verification,
technical verification, implementation, execution, external or security
operation, release activity, or historical-disposition rewriting.

P9-234 is selected only as a candidate. It is not started or authorized by
P9-233.

## 8. Final result and recorded non-actions

The final result is:

`COMPLETE / docs-only current routing summary maintenance rule review`

P9-233 performed no Git command, PowerShell, script, Excel/VBA, build, test,
parser, generator, runner, artifact verification, implementation, security or
external operation, package, `dist`, release, tag, stage, commit, push, or
technical execution. It changed no source, tests, tools, workbooks, Frozen
specifications, public contracts, persisted schemas, accepted ADR meaning or
status, or generated artifacts.
