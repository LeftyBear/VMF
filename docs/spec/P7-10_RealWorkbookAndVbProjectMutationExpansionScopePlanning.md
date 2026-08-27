# P7-10 - Real Workbook / Real VBProject Mutation Expansion Scope Planning

## Status

COMPLETE / docs-only expansion scope planning and GO / NO-GO record

## Purpose

Organize the possible expansion boundary after the completed P7-07 minimum
real workbook / real VBProject mutation slice.

P7-10 is documentation only. It does not grant implementation GO, does not
change production code or test code, does not open / save / close / SaveAs /
restore any workbook, does not mutate any real VBProject, does not create or
modify workbook fixtures, does not update package or `dist` artifacts, does
not perform release or publication work, does not access external services,
does not change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Starting State

- P7-07 is complete as the minimum local-only real workbook / real VBProject
  mutation implementation in commit
  `78d1ab2b456ffa9fd923d79aa481bac0c51ba065`.
- P7-08 is complete as docs-only implementation closeout and status sync.
- P7-09 is complete as docs-only next-candidate selection and selects P7-10
  as the next docs-only planning candidate.
- The completed P7-07 boundary permits only an explicitly supplied real
  VBProject target, trust/access and component preflight, create-only
  missing-module mutation for supported standard and class modules, readback
  verification, and rollback for components created by the operation.
- Additional workbook handling, real VBProject mutation operations, production
  workbook use, package / `dist`, release, publication, external service
  operations, public API changes, persisted schema changes, canonical format
  changes, and Frozen specification changes remain unauthorized.

## Baseline Boundary From P7-07

The P7-07 minimum boundary remains the baseline for any later expansion:

- input must be an already successful output write plan;
- target VBProject must be explicitly supplied by the caller;
- the target must be local-only and test-owned for focused verification;
- VBProject trust/access must be available before any mutation;
- all target components must be preflighted before mutation starts;
- missing-module creation is the only completed mutation operation;
- existing modules are conflicts and hard-stop before mutation;
- success requires readback from the target VBProject;
- post-preflight mutation or readback failure must roll back components
  created by the operation;
- no partial success may be reported.

## Expansion Candidates

| Candidate | Candidate Operation | Planning Decision |
| --- | --- | --- |
| A | Preserve create-only missing-module mutation and broaden only focused coverage around ordering, multi-module preflight, and readback failures | Acceptable future implementation candidate if separately authorized |
| B | Add test-owned workbook open / close handling around an explicitly named fixture without SaveAs or production workbook support | Requires renewed authorization before implementation |
| C | Add save / restore behavior for a test-owned workbook fixture after successful readback | Requires a new authorization package and explicit restore evidence |
| D | Add overwrite of existing modules | Rejected for the next expansion boundary |
| E | Add delete, rename, import, export, or arbitrary component creation | Rejected for the next expansion boundary |
| F | Apply mutation to production workbooks or user data | Rejected |

P7-10 does not select an implementation-start candidate. It records that the
lowest-risk future candidate is Candidate A, because it preserves the P7-07
operation set and expands only verification and edge-case coverage. Candidate
B or Candidate C may be reconsidered only through a later named authorization
package.

## Authorized Future Target Surface

Any later implementation task must name the exact target surface before GO can
be considered:

- exact editable production files;
- exact editable test files;
- exact local test-owned workbook fixture path, if workbook handling is in
  scope;
- exact VBProject target source and ownership statement;
- exact mutation operation set;
- exact verification command set;
- exact restore or cleanup expectations.

Without those named values, the decision remains NO-GO.

## Prohibited Target Surface

The following remain prohibited by P7-10:

- production workbook mutation;
- real user data mutation;
- implicit workbook discovery;
- implicit VBProject target selection;
- workbook SaveAs;
- overwrite, delete, rename, import, export, or arbitrary VBComponent
  creation;
- fallback Template selection;
- implicit Template selection;
- Template content inference;
- GenerateContext or Generator compensation;
- package or `dist` creation, update, replacement, or publication;
- release, tag, push, or publication operations;
- external service operations;
- credential or token-store access;
- public API changes;
- persisted schema changes;
- canonical format changes;
- Frozen specification changes.

## Preflight Requirements

Any later expansion must complete all applicable preflight checks before the
first mutation:

- output write plan exists and is successful;
- output write plan units are complete, supported, non-empty, and unique;
- module names are valid and safe;
- generated content is non-empty and tied to supported module kinds;
- target VBProject is explicitly supplied;
- VBProject trust/access is available;
- all existing component names are enumerated before mutation;
- all conflicts are detected before mutation;
- workbook target, if in scope, is explicitly named, local-only, test-owned,
  writable, and restorable;
- no unsupported workbook or VBProject operation is requested.

Any failed preflight is a no-mutation hard stop.

## Rollback / Restore Requirements

Rollback and restore must be operation-specific:

- for create-only VBProject mutation, rollback may remove only components
  created by the current operation;
- rollback must not delete or alter pre-existing components;
- rollback failure must be reported as failure, not success;
- workbook restore, if later authorized, must be defined before implementation
  and must identify the restore source, destination, timing, and verification;
- restore must not depend on production data or external services;
- partial mutation with incomplete rollback remains a failure boundary.

P7-10 does not authorize workbook restore implementation. It records that any
future workbook save / restore candidate requires a separate authorization
package before implementation GO.

## Readback Requirements

Readback remains mandatory for any later success result:

- created module names must be read back from the target VBProject;
- module type must match the requested supported module kind;
- module content must match the expected generated content;
- multiple modules must be verified as a complete set;
- readback mismatch, missing component, wrong module kind, or content mismatch
  is failure;
- readback failure after mutation must attempt rollback for components created
  by the operation.

## Verification Requirements

P7-10 authorizes only documentation verification:

- `git diff --check`;
- docs-only diff confirmation.

Any later implementation candidate must define focused local verification
before GO can be considered. That later verification should include, at a
minimum:

- successful create-only missing-module mutation;
- preflight conflict no-mutation behavior;
- multi-module all-or-nothing behavior;
- readback mismatch failure;
- rollback after post-preflight failure;
- preservation of pre-existing modules;
- workbook restore verification if workbook save / restore is in scope.

Full Build regression, Release build, format verification, and any workbook
fixture operation require separate authorization if named by a later
implementation task.

## Failure Boundary

The future expansion boundary must fail closed when any of the following is
encountered:

- missing or failed output write plan;
- invalid, duplicate, unsupported, or unsafe module output;
- ambiguous target workbook or VBProject;
- unavailable VBProject trust/access;
- existing module conflict;
- unauthorized overwrite, delete, rename, import, export, SaveAs, or
  production workbook operation;
- inability to prove the target is local-only and test-owned;
- inability to define rollback or restore behavior before mutation;
- readback mismatch or incomplete readback;
- rollback or restore failure;
- request for package / `dist`, release, publication, external service,
  credential, token-store, public API, persisted schema, canonical format, or
  Frozen specification change.

## GO / NO-GO Decisions

Decision: `GO` for recording P7-10 as docs-only expansion scope planning.

Decision: `NO-GO` for implementation in P7-10.

Decision: `NO-GO` for workbook open / save / close / SaveAs / restore in
P7-10.

Decision: `NO-GO` for real workbook mutation or real VBProject mutation in
P7-10.

Decision: `NO-GO` for expanding beyond the P7-07 create-only missing-module
operation set without a later named authorization package and separate
implementation GO.

Decision: `NO-GO` for package / `dist`, release, publication, external
service, credential, token-store, public API, persisted schema, canonical
format, or Frozen specification changes.

## Preserved Invariants

P7-10 preserves:

- the completed P7-07 minimum implementation boundary;
- P7-08 as the implementation closeout record;
- P7-09 as the docs-only candidate selection record;
- create-only missing-module mutation as the only completed real VBProject
  mutation operation;
- trust/access preflight before mutation;
- readback before success;
- rollback for components created by the operation;
- no partial success;
- no implicit or fallback target selection;
- no Template content inference;
- no GenerateContext or Generator compensation;
- no package / `dist`, release, publication, external service, or Frozen
  specification change.

## Verification Performed

P7-10 verification is docs-only:

- reviewed P7-08 closeout and P7-09 candidate selection records;
- reviewed backlog, current status, and handoff records for the current P7
  boundary;
- confirmed this request grants no implementation GO;
- required post-edit verification: `git diff --check` and docs-only diff
  confirmation.
