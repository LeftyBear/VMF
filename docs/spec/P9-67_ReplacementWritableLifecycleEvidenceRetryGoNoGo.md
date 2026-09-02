# P9-67 - Replacement Writable Lifecycle Evidence Retry GO / NO-GO

## Status

COMPLETE / docs-only replacement writable lifecycle evidence retry GO / NO-GO
decision

## Purpose

Apply the P9-66 result review and decide whether a later separately authorized
focused retry may repeat only the P9-64 operation against the exact replacement
fixture to seek one complete run with no residual Excel process at the initial
post-close verification point.

P9-67 is documentation only. It does not run Excel automation, open / create /
save / SaveAs / close any workbook, mutate or repair either fixture, mutate a
workbook or VBProject, inject code, import or export modules, change
implementation or test code, run implementation tests, update package or
`dist` release artifacts, perform release or publication work, access external
services, stage, commit, push, or change public APIs, persisted schemas,
canonical formats, or Frozen specifications.

## Reviewed Evidence

P9-67 reviewed P9-64, P9-65, P9-66, and the current backlog, current-status,
and handoff records.

Both fixture identities were rechecked without opening either workbook:

- replacement fixture:
  `tests\fixtures\workbooks\P9_WritableLifecycleFixture.xlsm`; length `8342`
  bytes; SHA-256
  `220322FA5416DC7A10CD80BFBCEE0AE277D699175FF7A874E5DE3163D7FC301B`;
  attributes `Archive`;
- historical fixture: `tests\fixtures\workbooks\P9_TestOwnedWorkbook.xlsm`;
  length `3532` bytes; SHA-256
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`;
  attributes `Archive`;
- workbook fixture count under `tests\fixtures\workbooks`: exactly `2`;
- current residual Excel process count: `0`.

## Retry GO Boundary

P9-66 accepts the P9-65 workbook-level observations and final safe state while
preserving the initial residual-process hard stop and exit code `1`. The gap is
limited to obtaining one complete run whose initial post-close residual-process
check passes. Repeating the already bounded P9-64 operation is therefore the
minimum next evidence step and does not require implementation, test-code, or
fixture changes.

A later separate focused retry is GO only within this boundary:

- recheck both exact fixture identities, exactly two workbook fixtures, and
  zero residual Excel processes immediately before Excel automation;
- select only
  `tests\fixtures\workbooks\P9_WritableLifecycleFixture.xlsm` by explicit path,
  with no discovery or fallback;
- open the replacement fixture writable with `UpdateLinks = 0`,
  `ReadOnly = False`, and `AddToMru = False`;
- reconfirm the opened full path and writable mode;
- observe `Saved` without mutation and require the expected clean state;
- close without saving;
- verify immediately after close that both fixture identities and the fixture
  count remain unchanged and that residual Excel process count is `0`;
- retain textual / log evidence only.

The retry must hard-stop on any missing or additional fixture, identity
mismatch, pre-existing Excel process, fallback requirement, failed open, wrong
workbook identity, read-only open, unexpected dirty state, failed close,
changed post-close fixture identity, changed fixture count, or nonzero residual
Excel process at the initial post-close verification point. Delayed natural
process exit may establish a final safe state but must not convert such a hard
stop or nonzero exit code to PASS.

## Decision

Decision: `GO` for recording P9-67 as a docs-only replacement writable
lifecycle evidence retry GO / NO-GO decision.

Decision: `PASS` for current replacement fixture identity confirmation,
historical fixture unchanged-identity confirmation, exact fixture count
confirmation, and residual Excel process count `0` during P9-67.

Decision: `GO` for a later separate focused retry limited to repeating only
the P9-64 operation against the exact replacement fixture within the boundary
recorded above.

Decision: `NO-GO` for starting retry execution or Excel automation from
P9-67.

Decision: `NO-GO` for claiming complete writable lifecycle success-path
evidence from P9-65, P9-66, or this docs-only decision.

Decision: `NO-GO` for opening or mutating the historical fixture, fallback
workbook selection, Save, SaveAs, fixture mutation, workbook / VBProject
mutation, code injection, module import / export, implementation or test code
change, package / `dist`, release / publication, external service operation,
staging, commit, push, public API change, persisted schema change, canonical
format change, or Frozen specification change from P9-67.

## Selected Next Candidate

**P9-68 - Replacement Writable Lifecycle Evidence Retry Execution**

P9-68 may execute only the focused retry authorized by P9-67. It must repeat
only the P9-64 operation against the exact replacement fixture, preserve the
historical fixture unchanged, prohibit fallback selection, close without
saving, verify unchanged post-close identities, require zero residual Excel
processes at the initial post-close verification point, and retain textual /
log evidence only.

P9-68 must not infer authorization for Save, SaveAs, fixture mutation,
historical fixture open or mutation, workbook / VBProject mutation, code
injection, module import / export, implementation or test code change, package
/ `dist`, release / publication, external services, staging, commit, push,
public API changes, persisted schema changes, canonical format changes, or
Frozen specification changes.

## Preserved Invariants

P9-67 preserves:

- the P9-65 initial residual-process hard stop and exit code `1` as
  authoritative;
- final safe state as distinct from complete success-path evidence;
- the replacement fixture as the only authorized target of the later focused
  retry;
- the historical fixture as immutable historical / read-only evidence input;
- exact-path and exact-identity checks with no fallback workbook selection;
- retry authorization and retry execution as separate tasks;
- no Save, SaveAs, fixture, workbook, or VBProject mutation;
- textual / log evidence-only retention limits;
- package / `dist`, release / publication, external service, public API,
  persisted schema, canonical format, and Frozen specification boundaries.

## Verification

P9-67 verification is documentation-only:

- reviewed the replacement writable lifecycle evidence chain through P9-66;
- rechecked both fixture lengths, SHA-256 values, and attributes without
  opening either workbook;
- confirmed exactly two workbook fixtures;
- confirmed current residual Excel process count `0`;
- reviewed backlog, current-status, and handoff state;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, trailing whitespace scan, and Git status confirmation.

No implementation tests are required or run for P9-67. No Excel automation or
workbook operation is performed by P9-67.
