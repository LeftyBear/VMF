# Publisher P2-03 Clearer Dry-Run Output Evaluation

Status  : COMPLETE / P2-03-A, P2-03-B, P2-03-C, and P2-03-D implemented; P2-03-E deferred
Scope   : Evaluate clearer local dry-run output for Google Docs publication planning without changing dry-run semantics
Depends : docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md, docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md, docs/architecture/ADR-0007-error-handling-and-failure-classification.md, docs/development/Publisher_Phase4-2-1_DiagnosticLoggingSpecification.md, docs/development/Publisher_v1.0_Implementation_Voyage_Log.md, docs/development/Publisher_vNext_Backlog.md

This record started as a design-only record. It now also records the narrow
local-only implementation closeout for P2-03-A and P2-03-B. The implementation
does not modify Frozen specifications, public APIs, persisted schemas, release
artifacts, Google Docs, Google Drive, OAuth token stores, Live E2E state,
package or dist contents, tags, publication state, Avast evidence, or
vendor-clearance status.

P2-03 is an independent vNext design scope. It does not reopen the existing
Publisher `0.0.1-dev` prerelease and does not authorize publication.

## 1. Purpose

The purpose of P2-03 is to evaluate whether Publisher dry-run output can better
help an operator understand Google Docs publication planning before any live
operation is authorized.

The improvement target is operator comprehension only. Dry-run must remain
local, non-mutating, and non-evidentiary for release clearance, Google
verification, Live E2E, vendor clearance, and publication authorization.

## 2. Scope

Allowed design scope:

- review the current CLI dry-run implementation and structured diagnostics;
- review dry-run lifecycle documentation, ADRs, and local tests;
- identify places where current output can be misunderstood;
- define clearer output candidates for a later narrow local-only
  implementation;
- define output contract candidates, acceptance criteria, and local-only
  verification.

Allowed future implementation scope, if separately authorized:

- CLI dry-run stdout/stderr presentation changes in `src/Publisher.Cli`;
- focused Publisher unit tests under `tests/unit/Publisher`;
- documentation updates that record the adopted output behavior.

## 3. Non-Scope

P2-03 does not authorize:

- Google Docs or Google Drive mutation;
- OAuth, credential, or token-store operation;
- Live E2E;
- changes to dry-run planning semantics, safety stops, Verified State meaning,
  Physical Update Plan meaning, readback verification, or state promotion;
- treating dry-run as Google verification, publication authorization, release
  clearance, vendor clearance, or Avast safety certification;
- Frozen specification changes;
- public API changes;
- persisted schema or canonical format changes;
- release, tag, publication, GitHub asset, package, or dist updates;
- Avast or flagged executable execution;
- stage, commit, or push.

## 4. Current Dry-Run Surface

The current CLI command is `dry-run <markdown-file>`.

Observed implementation:

- validates that exactly one Markdown path is supplied;
- loads local Publisher settings without requiring Google publish settings;
- validates and compiles the Markdown locally;
- emits a structured stderr `DRY_RUN_PLAN` event with `stepCount`;
- returns stdout-compatible success text through `DRY_RUN_SUCCEEDED`;
- avoids echoing the raw Markdown path in structured diagnostics;
- reports phase `planner` and operation `plan` for the planning event.

Related lifecycle implementation exists below the CLI:

- `VerifiedPublishLifecycle.DryRunAsync` uses the same baseline load, snapshot
  preparation, diff, and physical planning path as a real update;
- dry-run returns logical and physical counts, per-kind logical counts,
  publish-required state, revision precondition, operation identities, affected
  ranges, warnings, and stable conflicts;
- dry-run does not call adapter apply and does not save Verified State;
- unsafe pre-diff conflicts return stable conflict codes without speculative
  logical or physical plans.

## 5. Operator Misread Risks

| Risk | Current source | Possible operator misunderstanding |
| --- | --- | --- |
| `DRY_RUN_SUCCEEDED` can look like publication success when read without the local-only boundary. | Final result code and message are compact. | Operator may treat local planning success as Google Docs mutation, readback verification, or release evidence. |
| `stepCount` alone is too terse for plan comprehension. | CLI `DRY_RUN_PLAN` exposes only compiled publish step count. | Operator may not know whether the plan contains headings, paragraphs, lists, tables, images, or no meaningful content. |
| The output does not explicitly say no Google write occurred. | The command is named dry-run, but the event does not carry a non-mutating statement. | Operator may need external documentation to confirm no Docs/Drive mutation happened. |
| The CLI dry-run output is disconnected from the richer physical dry-run lifecycle model. | CLI dry-run compiles a publish plan but does not expose Verified State or Physical Update Plan boundaries. | Operator may infer Verified State, revision preconditions, or readback were verified when they were not part of this CLI path. |
| Failure boundaries are not summarized in operator terms. | Diagnostics are structured and safe, but terse. | Operator may not distinguish usage/configuration/input failure from planning success without inspecting event codes. |

No risk requires changing dry-run semantics.

## 6. Alignment Constraints

Any future implementation must preserve:

- ADR-0004: Verified State remains the trusted baseline for differential
  updates and is saved only after successful post-apply readback verification;
- dry-run never applies a physical plan and never saves Verified State;
- unsafe conflicts remain hard stops and must not produce speculative plans;
- ADR-0006: diagnostics remain safe, bounded, structured, and redacted before
  serialization;
- ADR-0007: classification, exit-code, cancellation, retry, and safe-message
  behavior remain stable;
- existing stdout compatibility unless a future task explicitly approves a
  documented compatibility change;
- no Google Docs / Drive, OAuth, token-store, Live E2E, release, package,
  Avast, staging, commit, or push operation.

## 7. Candidate Improvements

### P2-03-A: Explicit Local-Only Boundary

Add value-safe dry-run fields or lines that state:

- `mode`: `local-dry-run`;
- `googleDocsMutation`: `not-attempted`;
- `googleDriveMutation`: `not-attempted`;
- `verifiedStateSaved`: `false`;
- `publicationAuthorized`: `false`.

Priority: P1.

Rationale: Highest value for preventing overclaiming. It does not require
planner semantic changes.

### P2-03-B: Human-Readable Plan Summary

Add a compact operator summary with safe counts derived from compiled local
steps:

- total step count;
- content-shape counts where already available without parsing raw values;
- whether local Markdown compilation succeeded;
- whether the output is safe to use only as planning evidence.

Priority: P1.

Rationale: Improves comprehension while keeping raw paths, raw content, URLs,
and document IDs out of diagnostics.

### P2-03-C: Structured Dry-Run Output Contract

Define a stable `DRY_RUN_SUMMARY` or expanded `DRY_RUN_PLAN` payload containing
only allow-listed fields:

- `mode`;
- `stepCount`;
- `contentShape`;
- `planningResult`;
- `mutationBoundary`;
- `verifiedStateBoundary`;
- `authorizationBoundary`.

Priority: P2.

Rationale: Useful for automation, but should be designed carefully to avoid
breaking existing parsers that expect current event codes and fields.

### P2-03-D: Failure Boundary Hints

For dry-run failures, include safe category hints such as:

- `failureBoundary`: `usage`, `configuration`, `input`, `compile`, or
  `internal`;
- existing classification and result code.

Priority: P2.

Rationale: Helps support triage. It should reuse existing classification and
last-safe-operation diagnostics rather than inventing a new error taxonomy.

### P2-03-E: Physical Update Dry-Run Bridge

Later, if a command is introduced or extended to exercise the richer
`VerifiedPublishLifecycle.DryRunAsync` path, expose logical and physical plan
counts, revision precondition status, and conflict codes.

Priority: P3 / defer.

Rationale: Valuable but larger than a CLI output clarity task. It risks
operator confusion unless the distinction between local compile planning and
Verified State physical dry-run is made explicit.

## 8. Recommended Design

Proceed later with a narrow local-only implementation for P2-03-A and P2-03-B
only.

The first implementation should:

1. keep the existing `dry-run <markdown-file>` command and result codes;
2. keep existing stdout compatibility unless explicitly approved otherwise;
3. add explicit local-only / non-mutating / non-authorizing boundary fields to
   structured stderr;
4. add a compact safe plan summary derived from compiled publish steps;
5. avoid raw Markdown paths, raw document content, raw URLs, document IDs,
   credential values, token-store paths, provider payloads, or stack traces;
6. preserve all existing dry-run success and failure semantics;
7. add focused unit tests for success, usage failure, invalid input, and seeded
   sensitive-value exclusion.

Defer P2-03-C until output compatibility impact is reviewed. Defer P2-03-D
until it can reuse existing classification and P2-02 last-safe-operation fields
without adding another taxonomy. Defer P2-03-E because it may require a
separate feature decision around physical dry-run command scope.

## 9. Output Contract Candidate

Candidate structured stderr payload for dry-run success:

```json
{
  "code": "DRY_RUN_PLAN",
  "command": "dry-run",
  "phase": "planner",
  "operation": "plan",
  "mode": "local-dry-run",
  "stepCount": 0,
  "contentShape": {
    "headingCount": 0,
    "paragraphCount": 0,
    "listCount": 0,
    "tableCount": 0,
    "imageCount": 0,
    "codeBlockCount": 0,
    "quoteCount": 0
  },
  "mutationBoundary": {
    "googleDocsMutation": "not-attempted",
    "googleDriveMutation": "not-attempted",
    "oauthOperation": "not-attempted",
    "tokenStoreOperation": "not-attempted"
  },
  "verifiedStateBoundary": {
    "physicalUpdatePlanApplied": false,
    "readbackVerified": false,
    "verifiedStateSaved": false
  },
  "authorizationBoundary": {
    "publicationAuthorized": false,
    "releaseClearance": false,
    "vendorClearance": false
  }
}
```

The exact shape may be flattened during implementation if nested structured
fields are inconsistent with existing diagnostics. Any final contract must be
documented and tested before implementation is treated as complete.

## 10. Acceptance Criteria For Future Implementation

Future P2-03 implementation is acceptable only when:

- dry-run remains non-mutating;
- existing dry-run success/failure semantics and result codes are preserved;
- Verified State, Physical Update Plan, safety-stop, readback, and state
  promotion meanings are unchanged;
- stdout compatibility is preserved unless separately approved;
- all new output fields are allow-listed, bounded, deterministic, and
  value-safe;
- output explicitly distinguishes local planning from Google Docs mutation,
  Google Drive mutation, OAuth/token-store operation, Live E2E, publication
  authorization, release clearance, vendor clearance, and Avast safety
  certification;
- raw paths, raw Markdown content, raw URLs, document IDs, provider payloads,
  HTTP bodies, credential paths, token-store paths, tokens, secrets, cookies,
  Authorization headers, raw exception messages, and stack traces are absent;
- focused tests cover success, usage failure, invalid Markdown/input, boundary
  fields, content-shape counts, and seeded sensitive-value exclusion;
- no Frozen specifications, public APIs, persisted schemas, package outputs,
  `dist/` contents, release records, Google state, OAuth/token-store state,
  Avast state, staging, commit, or push are changed.

## 11. Verification Plan For Future Implementation

Required local-only verification:

```powershell
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~CliApplicationTests"
dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore
dotnet build VMF.Publisher.sln --configuration Release --no-restore
dotnet format VMF.Publisher.sln --verify-no-changes --no-restore
git diff --check
git status --short --branch
```

Prohibited verification:

- Live E2E;
- Google Docs or Google Drive mutation;
- OAuth or token-store operation;
- package creation or package verification;
- `dist/` mutation;
- Avast or flagged executable execution;
- release, tag, publication, GitHub asset, stage, commit, or push.

## 12. Implementation GO/NO-GO

Decision: COMPLETE for P2-03 as a whole.

P2-03-A and P2-03-B were completed as a narrow local-only implementation.
P2-03-C and P2-03-D were completed later as separate scoped implementations.
P2-03-E remains deferred / unimplemented and is not required for P2-03
completion.

Recommendation for the implemented scope: GO / close P2-03. Any future work on
P2-03-E must start as a separate scoped task.

Conditions for GO:

- implementation remains CLI-internal and output-only;
- no public API, schema, dry-run semantic, Verified State, Physical Update
  Plan, package, release, or external service behavior changes are required;
- tests prove new fields are value-safe and no raw sensitive values are
  emitted;
- dry-run success is not described as Live E2E, Google verification, release
  clearance, publication authorization, vendor clearance, or Avast safety
  certification.

NO-GO for this current task:

- do not proceed with P2-03-E without a separate scoped implementation
  decision;
- stop if any improvement requires public API changes, raw provider detail,
  credential or path inspection, external service mutation, package/release
  activity, or Verified State semantic changes.

## 13. Implementation Closeout

Completed implementation scope:

- P2-03-A: explicit local-only / non-mutating / non-authorizing dry-run
  boundary output.
- P2-03-B: compact human-readable dry-run plan summary derived from local
  compiled publish steps.

Completed follow-on scope after the original A/B closeout:

- P2-03-C: structured dry-run output contract implemented by P2-18 / commit
  `6fb29bb` as a new flat, success-only `DRY_RUN_SUMMARY` structured stderr
  event that preserves `DRY_RUN_PLAN` compatibility.
- P2-03-D: failure boundary hints implemented by P2-13 / commit `91d3969`.

Deferred scope:

- P2-03-E: physical update dry-run bridge.

Recorded verification for the completed A/B implementation:

| Check | Result |
| --- | --- |
| Focused Publisher CLI unit tests | PASS |
| Publisher unit tests | PASS |
| Publisher Release build | PASS |
| Format verification | PASS |
| Diff whitespace verification | PASS |

Boundary confirmation:

- no Google Docs or Google Drive mutation was performed;
- no OAuth or token-store operation was performed;
- no Live E2E was performed;
- no package or `dist` artifact was created or updated;
- no release, tag, publication, or GitHub asset operation was performed;
- no Avast or flagged executable operation was performed;
- vendor-clearance and Avast safety-certification state remain unchanged;
- no stage, commit, or push was performed by this closeout record.
