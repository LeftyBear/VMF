# P9-01 - Post-P8 Actual Workbook Mutation Expansion Scope Planning

## Status

COMPLETE / docs-only actual workbook mutation expansion scope planning

## Purpose

Start P9 from the P8 COMPLETE state and fix the next expansion boundary for
actual workbook mutation beyond the completed narrow local-only test-owned
workbook / create-only VBProject mutation flow.

P9-01 is documentation only. It does not grant implementation GO, change
production code or test code, run implementation tests, open / create / save /
SaveAs / close / discard / restore any workbook, mutate any workbook or
VBProject, update package or `dist` artifacts, perform release or publication
work, access external services, or change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Starting State

P8-10 is COMPLETE and selected P9-01 as the minimum next-phase docs-only
candidate.

The completed P8 boundary is:

- P7 completed create-only real VBProject mutation for missing supported
  modules, with pre-mutation hard-stops, mandatory readback, rollback for
  current-operation created components, and operator-review evidence when
  rollback cannot fully remove current-operation components.
- P8-06 / P8-07 completed the first workbook lifecycle authorization and
  handoff slice for an exact local test-owned workbook fixture, explicit
  lifecycle authorization, `VBProject` handoff evidence, and no-save close
  cleanup.
- P8-09 / P8-10 recorded P8 COMPLETE only for that narrow local-only
  test-owned workbook / create-only VBProject mutation flow.

P8 COMPLETE does not authorize existing-workbook handling, production workbook
handling, workbook Save / SaveAs / restore semantics, destructive component
operations, arbitrary component creation, package / `dist`, release,
publication, external services, public API changes, persisted schema changes,
canonical format changes, or Frozen specification changes.

## Post-P8 Expansion Inventory

| Expansion area | P9-01 planning decision |
| --- | --- |
| Actual workbook identity and ownership | Future work must name an exact local-only, test-owned workbook identity before any operation. Fallback, active workbook, recent-file, directory-scan, default fixture, nearest-match, or production workbook selection remains prohibited. |
| Existing workbook open / close handling | Candidate area for later docs-only authorization and test design, limited to an explicitly named local test-owned workbook and no-save cleanup unless separately expanded. |
| Workbook persistence operations | Save, SaveAs, discard, restore, backup, recovery, replacement, deletion, repair, and conversion remain deferred. They require a separate persistence / recovery boundary before any implementation or test execution. |
| VBProject operation set | Create-only missing supported module mutation remains the only completed operation. Replace, remove, overwrite, delete, rename, import, export, and arbitrary component creation remain deferred. |
| Component rollback and readback | Existing P7 rollback and mandatory readback rules remain required for any later mutation success. Rollback may remove only current-operation created components and must preserve unrelated pre-existing components. |
| Workbook lifecycle rollback | Workbook lifecycle rollback remains separate from component rollback. No save / restore / discard rollback may be inferred from component cleanup. |
| Production workbook handling | Rejected for the next expansion boundary. Production workbook or real user data mutation requires a later explicit authorization package and operation-specific owner approval. |
| Package, release, publication, and external services | Out of scope for P9-01 and any immediate Build mutation planning candidate unless separately authorized by operation-specific records. |

## Candidate Options Considered

### Candidate A - Actual Workbook Identity Authorization Boundary

Docs-only record that fixes the required authorization inputs for any later
actual workbook mutation expansion: exact local test-owned workbook identity,
ownership, allowed lifecycle operation set, denied fallback selection,
pre-mutation safety stops, cleanup expectations, evidence, and verification
requirements.

This candidate performs no implementation and grants no workbook or VBProject
mutation expansion.

### Candidate B - Existing Workbook Focused Test Design

Docs-only focused test design for opening an explicitly named local test-owned
existing workbook and handing its `VBProject` to the existing create-only
mutation path.

This candidate is premature until Candidate A fixes the authorization boundary.

### Candidate C - Workbook Save / Restore Boundary

Docs-only persistence and recovery boundary for Save, SaveAs, restore, backup,
or discard semantics.

This candidate is higher risk because it affects persisted workbook state and
must not be inferred from P8 no-save fixture cleanup.

### Candidate D - Component Operation Expansion Boundary

Docs-only boundary for replace, remove, overwrite, delete, rename, import,
export, or arbitrary component creation.

This candidate is higher risk because it can destroy or rewrite existing
workbook state and should not precede the actual workbook identity
authorization boundary.

### Candidate E - Production Workbook Mutation Authorization

Authorization package for production workbook or real user data mutation.

This candidate is rejected for the next boundary.

## Selected Next Minimum Candidate

Selected next minimum candidate:

**P9-02 - Actual Workbook Identity Authorization Boundary**

Selection basis:

- P8 completed only exact test-owned workbook lifecycle authorization and
  create-only VBProject mutation for the narrow focused flow;
- actual workbook expansion must first define the workbook identity,
  ownership, allowed operations, denied fallback paths, cleanup expectations,
  evidence, and verification boundary before any focused test design or
  implementation GO can be considered;
- workbook Save / SaveAs / restore and destructive component operations are
  persistence-affecting or state-destructive and remain too broad for the next
  minimum step;
- production workbook mutation and real user data mutation remain rejected
  until a later explicit owner-authorized operation-specific record exists.

## Required Scope For P9-02

P9-02 must remain docs-only unless a later task explicitly changes that scope.

P9-02 should:

- start from P8 COMPLETE and this P9-01 expansion inventory;
- define exact workbook identity and ownership requirements for later
  local-only actual workbook mutation expansion;
- define which lifecycle operations are eligible for later focused test design;
- preserve no fallback, implicit, active-workbook, recent-file, directory-scan,
  default-fixture, nearest-match, or production workbook selection;
- require pre-mutation hard-stops for missing identity, mismatched identity,
  missing authorization, unavailable trust/access, invalid write units,
  existing component conflicts, or unsupported operations;
- define cleanup / no-save expectations only as documentation unless a later
  implementation GO names exact files and commands;
- preserve mandatory readback, rollback, operator-review evidence, and
  workbook lifecycle rollback separation;
- keep production workbook handling, Save / SaveAs / restore, destructive
  component operations, package / `dist`, release / publication, external
  services, public API changes, persisted schema changes, canonical format
  changes, and Frozen specification changes as NO-GO.

## GO / NO-GO Decisions

Decision: `GO` for recording P9-01 as docs-only actual workbook mutation
expansion scope planning.

Decision: `GO` for selecting P9-02 as the next minimum docs-only candidate.

Decision: `NO-GO` for implementation in P9-01.

Decision: `NO-GO` for production code or test code changes in P9-01.

Decision: `NO-GO` for running implementation tests in P9-01.

Decision: `NO-GO` for workbook open, create, save, SaveAs, close, discard,
restore, backup, recovery, replacement, deletion, repair, conversion, or
production cleanup in P9-01.

Decision: `NO-GO` for workbook or VBProject mutation expansion in P9-01.

Decision: `NO-GO` for component replace, remove, overwrite, delete, rename,
import, export, arbitrary component creation, production workbook handling,
real user data mutation, package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Preserved Invariants

P9-01 preserves:

- P8 COMPLETE only for the narrow local-only test-owned workbook / create-only
  VBProject mutation flow;
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

P9-01 verification is documentation-only:

- reviewed P8-08, P8-09, and P8-10 records;
- reviewed backlog, current-status, and handoff state;
- docs-only diff review;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P9-01.
