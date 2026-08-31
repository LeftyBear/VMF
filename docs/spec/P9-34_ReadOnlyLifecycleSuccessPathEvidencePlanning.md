# P9-34 - Read-Only Lifecycle Success-Path Evidence Planning

## Status

COMPLETE / docs-only success-path evidence planning

## Purpose

Plan the minimum evidence needed before the P9 existing-workbook lifecycle
boundary can claim a successful read-only open, identity reconfirmation, close
without saving, and post-close unchanged-fixture confirmation.

P9-34 is documentation only. It does not add implementation, change production
code or test code, rerun implementation tests, open / create / save / SaveAs /
close / discard / restore any workbook, automate Excel, mutate the fixture,
mutate any workbook or VBProject, inject code, import or export modules,
repair or replace the fixture, update package or `dist` artifacts, perform
release or publication work, access external services, or change public APIs,
persisted schemas, canonical formats, or Frozen specifications.

## Planning Inputs

P9-34 starts from the P9-33 result review:

- the authorized fixture path remains
  `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
- the fixture identity was rechecked during P9-34 planning as length `3532`
  bytes and SHA-256
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
- P9-31 / P9-32 prove the root-injected hard-stop and no-mutation boundary;
- the current recorded evidence does not prove a successful Excel read-only
  open / identity reconfirmation / close-without-saving path;
- workbook / VBProject mutation, writable lifecycle operations, Save, SaveAs,
  fixture repair, fixture replacement, and fallback workbook selection remain
  outside the authorized boundary.

## Required Success-Path Evidence

A later separately authorized success-path evidence task must record all of
the following for the exact authorized fixture or for an explicitly authorized
replacement fixture:

- pre-open fixture identity: exact path, length, and SHA-256;
- explicit repository root supplied by the caller;
- resolved fixture path equal to the authorized fixture path;
- read-only open attempt performed in read-only mode only;
- opened workbook identity reconfirmed against the authorized fixture path;
- writable posture denied, with no Save or SaveAs authorization;
- lifecycle evidence includes successful `OpenReadOnly` and `CloseNoSave`
  operations;
- workbook close performed without saving;
- post-close fixture identity: exact path, length, and SHA-256 unchanged from
  pre-open evidence;
- `MutatedModules = 0`;
- no workbook / VBProject mutation, code injection, module import / export,
  fixture repair, fixture replacement, or fallback workbook selection occurred;
- operator-review evidence is retained if Excel cannot open the current
  minimal OOXML fixture in the local environment.

## Authorization Needed Later

P9-34 does not authorize the success-path evidence execution. A later task
must separately authorize:

- whether the exact current fixture is expected to be openable by Excel as-is;
- whether Excel automation is permitted for read-only open and no-save close;
- the exact command or runner entry point allowed to collect success-path
  evidence;
- whether failed open of the current fixture should remain a hard-stop or
  trigger a separate fixture-replacement decision;
- if replacement is considered, the exact replacement-fixture identity,
  creation authority, review authority, retention policy, and post-creation
  verification requirements.

Any replacement fixture must be approved before it is created or substituted.
Replacement must not be inferred from an unreadable-fixture result.

## Decision

Decision: `GO` for recording P9-34 as docs-only success-path evidence
planning.

Decision: `NO-GO` for claiming successful read-only lifecycle completion from
the current P9-31 / P9-32 / P9-33 evidence.

Decision: `NO-GO` for executing Excel automation, opening or closing any
workbook, mutating or replacing the fixture, changing implementation or test
code, or running implementation verification during P9-34.

Decision: `NO-GO` for moving directly into workbook / VBProject mutation,
writable workbook lifecycle operations, Save, SaveAs, code injection, module
import / export, package / `dist`, release / publication, external service
operation, public API change, persisted schema change, canonical format
change, or Frozen specification change.

Decision: `GO` only for a later separate GO / NO-GO task that decides whether
to execute read-only success-path evidence collection for the current P9
fixture or first authorize a replacement-fixture path.

## Selected Next Candidate

Selected next candidate:

**P9-35 - Read-Only Lifecycle Success-Path Evidence GO / NO-GO**

P9-35 should decide whether success-path evidence collection is authorized for
the current P9 fixture and execution environment. If the current fixture cannot
be accepted as the success-path subject, P9-35 must record `NO-GO` for
execution and select a separate replacement-fixture authorization candidate
instead. P9-35 must not infer authorization for workbook / VBProject mutation,
writable lifecycle operations, package / `dist`, release / publication,
external services, public API changes, persisted schema changes, canonical
format changes, or Frozen specification changes.

## Verification

P9-34 verification is documentation-only:

- reviewed P9-33;
- rechecked the P9 fixture length and SHA-256 without opening the workbook;
- updated backlog, current status, and handoff records;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, and Git status confirmation.

No implementation tests are required or run for P9-34. No workbook operation is
performed by P9-34.
