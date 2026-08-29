# P8-02 - Workbook Lifecycle Authorization Boundary

## Status

COMPLETE / docs-only workbook lifecycle authorization boundary

## Purpose

Fix the authorization boundary for workbook lifecycle operations before any
post-P7 real workbook handling, real VBProject target acquisition, or expanded
real VBProject mutation can be considered.

P8-02 is documentation only. It does not add implementation, change production
code or test code, run implementation tests, open / create / save / SaveAs /
close / discard any workbook, mutate any workbook or VBProject, create or
modify workbook fixtures, update package or `dist` artifacts, perform release
or publication work, access external services, or change public APIs,
persisted schemas, canonical formats, or Frozen specifications.

## Starting State

P7 is COMPLETE.

P8-01 is COMPLETE and is committed / pushed as
`83e8b6b docs: add P8-01 post-P7 mutation scope planning`.

P8-01 fixed the post-P7 responsibility split:

- workbook lifecycle handling is separate from real VBProject mutation;
- real VBProject mutation is separate from component rollback;
- P7 create-only missing-module mutation, readback, component rollback, and
  incomplete rollback reporting remain the baseline;
- workbook lifecycle authority must not be inferred from P7 component rollback;
- P8-01 did not grant implementation GO.

Workbook / VBProject mutation remains unauthorized and unexecuted by P8-02.

## Workbook Identification Boundary

Any future workbook lifecycle operation must receive an explicitly authorized
workbook identity before the operation starts.

For an existing workbook, the authorization package must name:

- the exact workbook path or other repository-approved workbook identity;
- the expected workbook ownership class, such as test-owned fixture or
  production workbook;
- whether the workbook may be opened read-only or editable;
- whether macro content and VBProject access are expected;
- the required initial state checks before open or mutation.

For a new workbook, the authorization package must name:

- the exact creation directory and file name;
- whether replacement is prohibited or explicitly allowed;
- the required file format;
- whether the workbook is macro-enabled;
- the cleanup, retention, and review expectation if creation succeeds but a
  later operation fails.

If more than one workbook matches, no workbook matches, the path is relative or
ambiguous, the workbook identity differs from the authorized value, or ownership
cannot be proven, the operation must hard-stop before opening, creating,
saving, closing, discarding, or passing any workbook-derived VBProject target to
mutation.

Implicit selection, last-active workbook selection, current Excel window
selection, name-only matching, directory scanning fallback, default fixture
fallback, and "nearest match" recovery are prohibited.

## Lifecycle Operation Authorization

Each lifecycle operation requires explicit operation-level authorization. An
authorization for one operation does not imply authorization for another.

| Operation | Required authorization | Unauthorized behavior |
| --- | --- | --- |
| open | Exact existing workbook identity, read-only/editable mode, expected macro/VBProject access posture, and pre-open state checks. | Opening by active workbook, recent file, name-only match, search fallback, or opening editable when only read-only is authorized. |
| create | Exact new workbook path, file format, macro-enabled setting, replacement rule, and retention/cleanup expectation. | Creating in a default directory, replacing an existing file, changing format, or creating macro-enabled content without explicit permission. |
| save | Editable workbook identity, allowed save point, expected mutation state, and post-save verification expectation. | Saving to persist unverified mutation, saving after a hard-stop, saving a read-only authorization, or autosaving by assumption. |
| save-as | Exact destination path, format, overwrite policy, source workbook identity, and identity handoff rule after SaveAs. | SaveAs to a derived path, overwrite by default, format conversion by convenience, or treating SaveAs as ordinary save. |
| close | Exact workbook identity, save/no-save behavior, expected dirty state handling, and post-close state expectation. | Closing active workbook, closing unrelated workbooks, closing with implicit save, or closing when required failure evidence must remain inspectable. |
| discard / no-save | Explicit approval to abandon unsaved workbook changes and the evidence that the changes are limited to the authorized operation. | Discarding unclassified changes, discarding pre-existing dirty state, or using no-save as fallback after unauthorized mutation. |

Lifecycle authorization must also state whether Excel instance ownership is
test-local, existing-user-session, or otherwise controlled. P8-02 does not
authorize use of any live user Excel session.

## Existing And New Workbook Responsibility Boundary

Existing workbook handling must preserve the pre-existing file unless a later
authorization explicitly permits persistence or replacement. Pre-existing dirty
state is a hard-stop because the lifecycle boundary cannot distinguish
operation-local changes from user changes.

New workbook handling owns only the workbook it creates at the exact authorized
path. Creation success does not authorize later save, SaveAs, close, discard,
VBProject mutation, or deletion unless those operations are separately named.

Existing-workbook restore and new-workbook cleanup are lifecycle concerns, not
component rollback concerns. They require a later candidate to define snapshot,
backup, restore, retention, and operator-review rules before implementation GO.

## Macro-Enabled Workbook Boundary

Macro-enabled workbook handling is allowed only when the authorization package
explicitly names a macro-enabled format and states that macro/VBProject access
is expected for the target workbook.

The lifecycle boundary must hard-stop when:

- the authorized file format does not support macros but VBProject mutation is
  requested;
- the workbook is macro-enabled but macro handling was not explicitly
  authorized;
- Trust Center / VBProject access preflight is not defined for the later
  operation;
- opening the workbook would require credential, protected-view, repair,
  conversion, or external-link decisions not named by the authorization.

P8-02 does not authorize enabling Trust Center settings, changing macro
security, repairing workbooks, converting workbook formats, resolving external
links, or using credentials.

## State Confirmation Boundary

Before any future mutation receives a workbook-derived VBProject target, the
lifecycle layer must provide a confirmed handoff state:

- authorized workbook identity;
- opened / newly created state;
- read-only or editable mode;
- saved / dirty state at handoff;
- macro-enabled and VBProject access posture;
- ownership class;
- authorized lifecycle operations remaining available;
- any pre-existing state that requires hard-stop or operator review.

After mutation or failed mutation, the lifecycle layer must confirm:

- whether the workbook is still open and identical to the authorized target;
- whether it is dirty;
- whether save, close, discard, or restore is authorized for the observed
  state;
- whether operator review is required before any further lifecycle operation.

If lifecycle state cannot be proven, the result must not be reported as
success, retry-ready, safely closed, safely saved, or cleanly restored.

## Failure And Lifecycle Rollback Boundary

Lifecycle rollback is limited to the lifecycle operation that was explicitly
authorized and already started.

Examples:

- failed open must not create, save, close another workbook, or select a
  fallback workbook;
- failed create may report the created workbook identity only if creation
  occurred, but cleanup or deletion requires separate authorization;
- failed save or SaveAs must not retry at another path or downgrade to a
  different format;
- failed close must preserve evidence and require operator review when open /
  dirty state cannot be proven;
- discard / no-save may be used only when explicitly authorized for the exact
  workbook and observed state.

Workbook snapshot restore, backup restoration, replacement, deletion, and
format conversion are not authorized by P8-02. They remain separate future
lifecycle candidates.

## Separation From VBProject Mutation And Component Rollback

Workbook lifecycle handling may acquire and hand off an authorized workbook
state. It must not decide component mutation.

VBProject mutation may apply approved write units only after receiving an
authorized VBProject target and valid mutation preflight. It must not open,
create, save, SaveAs, close, discard, restore, replace, or select workbooks
unless a later task explicitly grants that operation to the correct boundary.

Component rollback remains limited to current-operation created VBComponents as
fixed by P7 unless a later candidate changes the mutation boundary. Component
rollback does not imply workbook save, workbook restore, workbook close,
workbook discard, or lifecycle cleanup authority.

## Readback / Verification Handoff

Readback and verification may receive only proven lifecycle state:

- exact authorized workbook identity;
- exact VBProject target identity or access result;
- lifecycle operation history performed under authorization;
- dirty / saved / open state known at handoff;
- mutation result and component rollback result, if any;
- operator-review requirement, if lifecycle state or rollback state is
  incomplete.

Verification must not repair lifecycle state, choose a workbook, open a
fallback target, save unverified changes, discard changes, or convert a failure
into success.

## Authorized And Unauthorized Operations

Authorized by P8-02:

- create this P8-02 docs-only authorization-boundary record;
- synchronize backlog, current-status, and handoff documentation;
- inspect repository documentation for P8-01 and related state;
- run documentation diff checks such as `git diff --check` and Git status.

Not authorized by P8-02:

- implementation GO;
- production code change;
- test code change;
- implementation test execution;
- workbook open, creation, save, SaveAs, close, discard, restore, replacement,
  deletion, repair, conversion, fixture mutation, or Excel instance control;
- real workbook or real VBProject mutation;
- VBProject import, export, overwrite, delete, rename, or arbitrary component
  creation;
- macro security, Trust Center, credential, protected-view, or external-link
  changes;
- package / `dist` creation, update, replacement, or inspection;
- release, tag, push, or publication operation;
- external service operation;
- credential or token-store access;
- public API, persisted schema, canonical format, or Frozen specification
  change.

## Next Minimum Candidate

Selected next minimum candidate:

**P8-03 - Workbook Lifecycle Focused Test Design**

Selection basis:

- P8-02 fixes the lifecycle authorization rules but does not implement them;
- the next smallest safe step is a docs-only focused test design for explicit
  workbook identity, authorized lifecycle operations, hard-stops for missing or
  ambiguous authorization, and handoff-state verification;
- implementation remains premature until the test design and a later GO /
  NO-GO record are separately completed.

P8-03 must remain docs-only unless a separate task explicitly changes that
scope. P8-02 does not grant implementation GO for P8-03.

## Verification

P8-02 verification is documentation-only:

- review P8-01;
- review backlog, current-status, and handoff state;
- review docs-only diff;
- `git diff --check`;
- Git status confirmation.

No implementation tests are required or run for P8-02.
