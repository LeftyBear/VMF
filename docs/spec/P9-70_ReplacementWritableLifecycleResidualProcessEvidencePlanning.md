# P9-70 - Replacement Writable Lifecycle Residual-Process Evidence Planning

## Status

COMPLETE / docs-only replacement writable lifecycle residual-process evidence
planning

## Purpose

Define the minimum evidence and observation timing needed to distinguish
delayed Excel / COM teardown from an actionable cleanup failure after the
repeated P9-65 and P9-68 initial post-close residual-process hard stops.

P9-70 is documentation only. It does not run Excel automation, open / create /
save / SaveAs / close any workbook, mutate or repair either fixture, terminate
or otherwise mutate any process, mutate a workbook or VBProject, inject code,
import or export modules, change implementation or test code, run
implementation tests, update package or `dist` release artifacts, perform
release or publication work, access external services, stage, commit, push, or
change public APIs, persisted schemas, canonical formats, or Frozen
specifications.

## Reviewed Evidence

P9-70 reviewed P9-64 through P9-69 and the current backlog, current-status,
and handoff records.

The authoritative repeated result is:

- P9-65 and P9-68 each observed one Excel process at the required initial
  post-close verification point and exited `1`;
- each run preserved the replacement and historical fixture identities and
  fixture count;
- the observed Excel process later exited naturally and the final residual
  process count was `0`;
- neither delayed natural exit converts either hard stop to PASS or proves the
  complete writable lifecycle success path;
- P9-69 records another lifecycle retry as NO-GO.

P9-70 relies on those recorded results and does not reopen either workbook or
rerun either operation.

## Minimum Residual-Process Evidence Model

Any later separately approved evidence operation must capture the following
facts in one ordered textual / log record:

1. a pre-operation timestamp and Excel process inventory containing PID and
   process start time, with zero pre-existing Excel processes required;
2. the created Excel application's process identity, correlated to a PID
   created after the pre-operation inventory, without selecting an unrelated
   process by fallback;
3. timestamps for successful workbook close-without-saving return, Excel
   application quit return, and completion of the operation's COM-reference
   release boundary;
4. an immediate post-release process observation, including the correlated
   PID's presence or absence and the total Excel process count;
5. only when the immediate observation finds the correlated PID, bounded
   read-only follow-up observations at fixed elapsed offsets through a declared
   maximum observation window;
6. the first observed natural-exit timestamp, or an explicit statement that
   the correlated PID remained present through the maximum window;
7. unchanged pre/post path, length, SHA-256, attributes, and fixture-count
   evidence for both workbook fixtures; and
8. the command exit code and final classification, kept distinct from the
   workbook-level observations.

The later GO / NO-GO record must fix the exact follow-up offsets and maximum
window before execution. They must be short, bounded, and used only to
characterize teardown timing. Indefinite waiting, timing chosen after seeing
the result, and repeated lifecycle retries are not permitted.

## Classification and Hard-Stop Semantics

The required initial post-release observation remains the acceptance point.
If any Excel process remains there, the operation must retain the existing
HARD-STOP result and nonzero exit code. Follow-up observations are diagnostic
evidence only and must not delay, replace, or weaken that decision.

A later evidence record may classify the residual-process observation as:

- `NO INITIAL RESIDUAL`: the correlated PID is absent at the immediate
  observation point;
- `DELAYED NATURAL EXIT`: the correlated PID is present immediately but exits
  without intervention inside the fixed observation window; or
- `OBSERVATION WINDOW EXCEEDED`: the correlated PID remains present at the
  end of the fixed observation window.

`DELAYED NATURAL EXIT` still retains the overall HARD-STOP and does not prove
the complete writable lifecycle success path. `OBSERVATION WINDOW EXCEEDED`
requires safe stop and operator review. No classification authorizes process
termination, cleanup mutation, another retry, or acceptance-criterion change.

Any pre-existing Excel process, inability to correlate the created process,
missing timestamp or observation, unexpected process identity, fixture
identity/count mismatch, workbook mutation, failed close or quit, or evidence
collection outside the fixed window must hard-stop before a success claim.

## Decision

Decision: `GO` for recording P9-70 as docs-only residual-process evidence
planning.

Decision: `PASS` for defining a minimum ordered evidence model that preserves
the existing immediate residual-process hard stop and separates later
diagnostic observation from lifecycle acceptance.

Decision: `NO-GO` for claiming complete writable lifecycle success-path
evidence from P9-65, P9-68, P9-69, or P9-70.

Decision: `GO` only for a later separate docs-only GO / NO-GO decision that
fixes the exact bounded observation schedule and determines whether one
focused evidence execution may be authorized.

Decision: `NO-GO` for another retry, Excel automation, opening or mutating
either fixture, Save, SaveAs, fallback workbook or process selection, process
termination, implementation or test code change, acceptance-criterion change,
workbook / VBProject mutation, package / `dist`, release / publication,
external services, staging, commit, push, public API change, persisted schema
change, canonical format change, or Frozen specification change from P9-70.

## Selected Next Candidate

**P9-71 - Replacement Writable Lifecycle Residual-Process Evidence GO / NO-GO**

P9-71 should remain docs-only and decide whether a later separately authorized
focused evidence execution may collect the P9-70 evidence model. It must fix
the exact immediate observation boundary, follow-up offsets, maximum window,
PID-correlation method, evidence fields, command, and hard-stop conditions
before any execution authorization.

P9-71 must not infer Excel automation, workbook open, implementation, process
termination, fixture mutation, another ordinary retry, or broader
authorization from P9-70.

## Preserved Invariants

P9-70 preserves:

- the P9-65 and P9-68 immediate residual-process hard stops and exit code `1`
  as authoritative;
- delayed natural exit and final safe state as distinct from complete
  writable lifecycle success-path evidence;
- workbook-level observations as distinct from overall command acceptance;
- the historical fixture as immutable historical / read-only evidence input;
- exact-path, exact-identity, and correlated-process checks with no fallback;
- close without saving and no fixture, workbook, VBProject, or process
  mutation;
- planning, later GO / NO-GO, and later execution as separate tasks;
- package / `dist`, release / publication, external service, public API,
  persisted schema, canonical format, and Frozen specification boundaries.

## Verification

P9-70 verification is documentation-only:

- reviewed the replacement writable lifecycle evidence chain through P9-69;
- reviewed the recorded P9-65 and P9-68 residual-process outcomes without
  rerunning either operation;
- reviewed backlog, current-status, and handoff state;
- required post-edit verification: docs-only diff confirmation,
  `git diff --check`, trailing whitespace scan, and Git status confirmation.

No implementation tests are required or run for P9-70. No Excel automation,
workbook operation, or process mutation is performed by P9-70.
