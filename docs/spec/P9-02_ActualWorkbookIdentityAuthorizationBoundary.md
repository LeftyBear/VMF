# P9-02 - Actual Workbook Identity Authorization Boundary

## Status

COMPLETE / docs-only actual workbook identity authorization boundary

## Purpose

Start from P8 COMPLETE and the P9-01 actual workbook mutation expansion
inventory, then fix the authorization boundary that must exist before any
later actual workbook mutation expansion can identify or operate on a workbook.

P9-02 is documentation only. It does not grant implementation GO, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, mutate any workbook or
VBProject, update package or `dist` artifacts, perform release or publication
work, access external services, or change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Starting State

P8 is COMPLETE only for the narrow local-only test-owned workbook /
create-only VBProject mutation flow.

P9-01 is COMPLETE and selected P9-02 as the next minimum docs-only candidate.
P9-01 records that actual workbook mutation expansion must first name exact
workbook identity, ownership, allowed operations, denied fallback paths,
cleanup expectations, evidence, and verification boundaries before any focused
test design or implementation GO can be considered.

The completed flow does not authorize existing-workbook handling, production
workbook handling, workbook Save / SaveAs / restore semantics, destructive
component operations, arbitrary component creation, package / `dist`, release,
publication, external services, public API changes, persisted schema changes,
canonical format changes, or Frozen specification changes.

## Authorization Boundary

Any later actual workbook mutation expansion must provide an explicit
authorization record that names:

| Authorization input | Required P9-02 boundary |
| --- | --- |
| Exact workbook identity | A single local-only, test-owned workbook identity must be named before any operation. The record must identify how the workbook is selected without relying on active workbook state, recent files, directory scans, default fixtures, nearest matches, or production workbooks. |
| Ownership | The workbook must be test-owned and safe to mutate under the later named scope. Production workbook, user-data workbook, shared workbook, downloaded external workbook, and ambiguous ownership remain prohibited. |
| Location and access | The later record must state the expected local location and access preconditions without embedding private machine assumptions into a canonical format. Missing, inaccessible, mismatched, locked, or unauthorized workbooks must hard-stop before mutation. |
| Allowed lifecycle operation set | Later focused test design may consider explicit open / close / no-save cleanup for the exact named test-owned workbook only. Save, SaveAs, discard, restore, backup, recovery, replacement, deletion, repair, and conversion remain outside this boundary. |
| VBProject handoff | Any later handoff must preserve trust/access preflight and must not infer a target `VBProject` from fallback workbook selection. |
| Mutation operation set | The only completed VBProject operation remains create-only missing supported module mutation. Replace, remove, overwrite, delete, rename, import, export, and arbitrary component creation remain deferred. |
| Evidence | Later work must record workbook identity match, explicit authorization, lifecycle operation authorization, trust/access availability, target-state checks, readback, rollback, and incomplete-rollback evidence expectations before implementation can be considered. |

## Scope

P9-02 defines the required authorization inputs for a later local-only actual
workbook mutation expansion. It fixes that future work must:

- start from a single explicitly named local test-owned workbook;
- reject fallback, implicit, active-workbook, recent-file, directory-scan,
  default-fixture, nearest-match, or production workbook selection;
- require workbook identity, ownership, lifecycle authorization, trust/access,
  target-state, and write-unit checks before any mutation path can run;
- preserve mandatory readback before success;
- preserve rollback only for current-operation created components;
- preserve unrelated pre-existing components;
- report incomplete rollback as failure / operator-review-required evidence;
- keep workbook lifecycle rollback separate from component rollback;
- keep Save / SaveAs / restore and destructive component operations behind
  later explicit boundaries.

## Non-Scope

P9-02 does not define implementation details, production code changes, test
code changes, focused test cases, workbook automation commands, file fixtures,
package output, release procedure, or external-service behavior.

The following remain out of scope:

- implementation start;
- production code or test code changes;
- implementation test execution;
- workbook open, create, save, SaveAs, close, discard, restore, backup,
  recovery, replacement, deletion, repair, conversion, or production cleanup;
- workbook or VBProject mutation expansion;
- component replace, remove, overwrite, delete, rename, import, export, or
  arbitrary component creation;
- package / `dist` creation, update, replacement, or inspection;
- release, tag, push, or publication operation;
- external service operation;
- credential or token-store access;
- public API change;
- persisted schema change;
- canonical format change;
- Frozen specification change.

## Safety Stops

Any later actual workbook mutation expansion must hard-stop before mutation if
any of the following is true:

- the workbook identity is missing, ambiguous, or mismatched;
- workbook ownership is not test-owned and explicitly authorized;
- selection would depend on active workbook, recent-file, directory-scan,
  default-fixture, nearest-match, production workbook, or other fallback
  behavior;
- the workbook is unavailable, locked, inaccessible, or not macro-enabled when
  macro-enabled behavior is required by the later named scope;
- lifecycle authorization is absent or includes an operation outside the later
  named scope;
- `VBProject` trust/access is unavailable;
- write units are invalid;
- the target component already exists when create-only mutation is required;
- a requested component operation is unsupported;
- readback cannot verify the exact created component state;
- rollback cannot preserve unrelated pre-existing components.

## GO / NO-GO Decisions

Decision: `GO` for recording P9-02 as docs-only actual workbook identity
authorization boundary.

Decision: `GO` for selecting P9-03 as the next minimum docs-only candidate.

Decision: `NO-GO` for implementation in P9-02.

Decision: `NO-GO` for production code or test code changes in P9-02.

Decision: `NO-GO` for running implementation tests in P9-02.

Decision: `NO-GO` for workbook open, create, save, SaveAs, close, discard,
restore, backup, recovery, replacement, deletion, repair, conversion, or
production cleanup in P9-02.

Decision: `NO-GO` for workbook or VBProject mutation expansion in P9-02.

Decision: `NO-GO` for component replace, remove, overwrite, delete, rename,
import, export, arbitrary component creation, production workbook handling,
real user data mutation, package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Completion Criteria

P9-02 is complete when:

- this docs-only authorization boundary is recorded;
- backlog, current-status, and handoff records point to P9-02 as complete;
- the next minimum docs-only candidate is selected;
- implementation, test execution, workbook mutation, package / `dist`, release
  / publication, external service, public API, persisted schema, canonical
  format, and Frozen specification boundaries remain NO-GO;
- docs-only verification is recorded with `git diff --check`;
- implementation tests are recorded as not run because P9-02 is docs-only;
- commit and push remain unperformed unless later explicitly authorized by the
  user.

## Next Candidate

Selected next minimum candidate:

**P9-03 - Existing Workbook Focused Test Design**

Selection basis:

- P9-02 fixes the workbook identity and authorization inputs needed before
  test design can safely describe an existing-workbook flow;
- the next smallest safe step is docs-only focused test design for opening an
  explicitly named local test-owned workbook, handing its `VBProject` to the
  already bounded create-only mutation path, and closing it without save under
  explicit cleanup expectations;
- implementation, production / test code changes, workbook / VBProject
  mutation, Save / SaveAs / restore, destructive component operations,
  production workbook handling, package / `dist`, release / publication,
  external services, public API changes, persisted schema changes, canonical
  format changes, and Frozen specification changes remain NO-GO for P9-03
  unless a later explicit record changes that boundary.

## Preserved Invariants

P9-02 preserves:

- P8 COMPLETE only for the narrow local-only test-owned workbook / create-only
  VBProject mutation flow;
- P9-01 actual workbook mutation expansion inventory;
- explicit workbook identity and lifecycle authorization;
- no fallback or implicit workbook / VBProject target selection;
- create-only missing supported module mutation as the only completed
  VBProject operation;
- trust/access, target-state, workbook-identity, lifecycle-authorization, and
  invalid-write-unit hard stops before mutation;
- mandatory readback before success;
- rollback only for current-operation created components;
- preservation of unrelated pre-existing components;
- incomplete rollback evidence as failure / operator-review-required;
- workbook lifecycle rollback separation from component rollback;
- fallback / implicit Template selection prohibition;
- Template content inference prohibition;
- GenerateContext or Generator compensation prohibition;
- package / `dist`, release / publication, external service, public API,
  persisted schema, canonical format, and Frozen specification boundaries.

## Verification

P9-02 verification is documentation-only:

- reviewed P8-10 and P9-01 records;
- reviewed backlog, current-status, and handoff state;
- docs-only diff review;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P9-02.
