# P9-235 Docs-only Current Routing Summary Recheck Criteria Review

## 1. Status and scope

- Work item: `P9-235`
- Basis: `P9-233 Docs-only Current Routing Summary Maintenance Rule Review` and `P9-234 Docs-only Current Routing Summary Maintenance Rule Checkpoint Review`
- Work mode: `docs-only`
- Final status: `COMPLETE / docs-only current routing summary recheck criteria review`
- Technical execution: `NO-GO / SAFE-STOP`

This review defines when the synchronized Current Routing Summary must be
rechecked and when a recheck is unnecessary. It does not inspect Git or
repository state, verify an artifact, accept evidence, create a `PASS`, clear a
hold, reclassify work, or grant technical, external, security, release, or Git
authority.

## 2. Recheck required

Recheck the Current Routing Summary in a separately instructed docs-only work
item when at least one of the following is present:

1. a P9-233 update condition may have changed: the latest completed docs-only
   routing item, controlling navigation or basis record, classification
   treatment, applicable hold/non-finding/evidence boundary/authorization
   gate, or single next docs-only candidate;
2. one of the three current summaries is missing, duplicated, internally
   ambiguous, or appears inconsistent with either of the other two;
3. a new current record expressly supersedes, withdraws, narrows, or replaces
   a current routing statement;
4. wording could promote historical material, a procedural-deviation output,
   a documentary result, or an unfulfilled request into current evidence,
   `PASS`, repository-state proof, approval, or authority;
5. an item may have moved among `DOCS-ONLY CONTINUATION`, `BLOCKED TECHNICAL`,
   and `EXTERNAL AUTHORIZATION REQUIRED`, but only so the applicable separate
   decision process and prerequisites can be checked; or
6. an explicit consistency concern identifies the affected statement and the
   documents or classifications that may disagree.

Uncertainty about whether a current fact changed is itself a recheck trigger,
but not permission to infer the answer. Missing, ambiguous, stale, expired,
consumed, inconsistent, or non-reusable input remains fail-closed.

## 3. Recheck not required

A recheck is unnecessary when all of the following remain true:

- no P9-233 current-routing fact changed;
- the three documents retain one identifiable current summary and agree on
  status, safety state, classification treatment, authority boundary,
  navigation, detailed basis, and exactly one next candidate;
- activity only reads, cites, retains, or restates detailed historical records
  without changing current routing;
- no concrete inconsistency, ambiguity, supersession, or reclassification
  concern is identified; and
- no new current attributable authority or prerequisite-satisfying decision
  has been supplied through its own applicable process.

Elapsed time, document age, the number of historical entries, completion of an
unrelated docs-only item, or repetition of an older `Next` statement does not
alone require a recheck. A no-recheck conclusion creates no verification,
repository-state proof, evidence refresh, or authority.

## 4. Three-document synchronization criteria

If a recheck confirms that any P9-233 current-routing fact changed, update
`CURRENT_STATUS`, `HANDOFF`, and the backlog Current Routing Summary in the
same docs-only work item. Also update the backlog `Current Routing
Classification` when completed-history coverage, active docs-only mainline,
classification treatment, or the single next candidate changed.

Do not change the three documents when a recheck finds no current-routing fact
change. Do not partially synchronize them. If synchronization would require a
historical disposition, Frozen specification, public contract, evidence
status, hold, non-finding, or authorization boundary to be rewritten, retain
the existing route and stop with `NO-GO / SAFE-STOP`.

## 5. Next-candidate update criteria

The single next docs-only candidate may be updated only when:

1. the current candidate was completed, expressly withdrawn, or found unsafe
   or ineligible through a separately instructed docs-only decision;
2. exactly one replacement is named;
3. the replacement is narrow, non-executable, docs-only, and consistent with
   all continuing holds and independent authorization gates;
4. the replacement requires a separate explicit instruction before starting;
   and
5. all three current summaries and the backlog classification are synchronized
   in the same work item.

When no safe replacement exists, record `None`. Do not revive an older
candidate or list parallel candidates.

## 6. History, deviation, and authority criteria

Older `Next` statements, selections, requests, drafts, intakes, submissions,
templates, owner-input placeholders, and documentary `COMPLETE` or `ACCEPT`
results remain provenance only unless a new controlling record expressly and
validly changes current routing through its own applicable process.

P9-223a and P9-230a remain isolated procedural-deviation records. Their
occurrences and outputs are unusable as evidence, verification, artifact
verification, `PASS`, repository-state proof, or authority and cannot trigger,
support, or resolve a recheck.

## 7. Routing-classification recheck criteria

- Recheck `DOCS-ONLY CONTINUATION` when the single candidate changes or there
  is a concrete concern that it is executable, overbroad, started without a
  separate instruction, or dependent on a technical or external gate.
- Recheck `BLOCKED TECHNICAL` only when a separate valid decision process may
  have changed a controlling technical hold, non-finding, evidence boundary,
  or technical GO prerequisite. A docs-only result alone cannot do so.
- Recheck `EXTERNAL AUTHORIZATION REQUIRED` only when current attributable
  authority and every operation-specific prerequisite may have been supplied
  for the exact operation and scope. A request, draft, intake, expired or
  consumed authority, or unrelated approval cannot do so.

Recheck does not itself reclassify an item. Any reclassification requires its
own explicit work item, applicable prerequisites, valid decision, and
synchronized routing update. Otherwise retain the existing classification and
`NO-GO / SAFE-STOP`.

## 8. Single next docs-only candidate

The single next candidate is:

`P9-236 Docs-only Current Routing Summary Recheck Criteria Checkpoint Review`

Under a separate explicit docs-only instruction, P9-236 may check whether the
P9-235 required/not-required, synchronization, next-candidate, history, and
classification criteria are represented consistently across the three current
summaries. It must not perform Git or repository-state inspection, artifact or
technical verification, implementation, execution, external or security
operation, release activity, or historical-disposition rewriting.

P9-236 is selected only as a candidate. It is not started or authorized by
P9-235.

## 9. Final result and recorded non-actions

The final result is:

`COMPLETE / docs-only current routing summary recheck criteria review`

P9-235 performed no Git command, PowerShell, script, Excel/VBA, build, test,
parser, generator, runner, artifact verification, implementation, security or
external operation, package, `dist`, release, tag, stage, commit, push, or
technical execution. It changed no source, tests, tools, workbooks, Frozen
specifications, public contracts, persisted schemas, accepted ADR meaning or
status, or generated artifacts.
