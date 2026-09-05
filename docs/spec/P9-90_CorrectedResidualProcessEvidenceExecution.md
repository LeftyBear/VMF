# P9-90 - Corrected Residual-Process Evidence Execution

## Status

COMPLETE / hard-stopped before PRE_OPERATION evidence

## Purpose

Record the result of the one P9-89-authorized corrected residual-process
evidence execution using only the unchanged P9-79 direct-process `-File`
transport and fixed lifecycle target.

P9-90 validated the fixed target without rewriting or normalizing it, started
the fixed Windows PowerShell 5.1 executable once with the seven ordered
arguments supplied separately, retained the child process observations,
stdout, stderr, and exit code, and stopped without correction or retry.

## Fixed-Input Validation

- Path: `C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1`
- Length: `8264` bytes
- SHA-256: `80749CA24C4CF7A8BF27CF8D55633B526FF64FFBF6CE8216590C609060B1F353`
- UTF-8 BOM: `PASS`
- CRLF-only: `PASS`
- Exactly one final CRLF: `PASS`
- Rewrite, normalization, substitution, or alternate path: none

## Retained Invocation Evidence

- Executable: `C:\WINDOWS\System32\WindowsPowerShell\v1.0\powershell.exe`
- Arguments, in order: `-NoLogo`, `-NoProfile`, `-NonInteractive`,
  `-ExecutionPolicy`, `Bypass`, `-File`,
  `C:\Users\biz\AppData\Local\Temp\VMF-P9-79-ResidualProcessEvidence.ps1`
- Working directory: `C:\Users\biz\Documents\Project\VMF`
- Start timestamp UTC: `2026-09-05T02:25:12.3866056Z`
- Completion timestamp UTC: `2026-09-05T02:25:14.7267241Z`
- Child process ID: `28592`
- Child exit code: `1`
- stdout: empty; retained length `0`
- stderr: retained host-decoded length `943` UTF-8 bytes
- Retained stderr SHA-256 after host decoding:
  `B110962A52910F33B3B524A6DB4DE05EC3C094F92E8F2113A985B90D376E1F1E`
- Second invocation: none

The retained stderr identifies a `CommandNotFoundException` for
`Get-FileHash` at line `23`, character `73` of the fixed target. The child
emitted no stdout and therefore no `PRE_OPERATION` or later fixed JSON
evidence event. The failure occurred while `Get-Identity` evaluated the first
pre-operation fixture identity, before the script reached its `try` block,
`New-Object -ComObject Excel.Application`, workbook open, PID correlation,
lifecycle operation, acceptance observation, or diagnostic follow-up.

The localized explanatory part of stderr was mojibake in the retained host
presentation. P9-90 does not reconstruct or normalize that text and does not
infer any cause beyond the retained command name, exception type, source
location, empty stdout, and exit code.

## Separate Read-Only Safe-State Audit

At `2026-09-05T02:25:37.8284748Z`, a separate read-only audit observed:

- Excel process count: `0`
- replacement fixture: length `8342`, SHA-256
  `220322FA5416DC7A10CD80BFBCEE0AE277D699175FF7A874E5DE3163D7FC301B`,
  attributes `Archive`
- historical fixture: length `3532`, SHA-256
  `BB9646DB308BA05A3444CADC577F4A6F09642E576EF1F54456C2D889BA319E8E`,
  attributes `Archive`
- `.xlsm` fixture count: `2`

This audit establishes only the final local safe state and current fixture
identity. It is not reconstructed lifecycle output and does not prove that
the lifecycle operation, immediate acceptance observation, or diagnostic
timing ran.

## Decision

Decision: `COMPLETE / HARD-STOP / OPERATION FAILURE` for P9-90. The one
P9-89-authorized invocation was performed once and returned exit code `1`
before any fixed JSON evidence event was emitted.

Decision: `PASS` for unchanged fixed-target identity, direct executable and
ordered-argument use, single-invocation compliance, retained process and
stream observations, no correction, no retry, and the separate final
zero-process and unchanged-fixture safe-state audit.

Decision: `UNPROVEN` for runtime precondition completion, Excel creation,
writable workbook open, PID correlation, close-without-saving lifecycle,
immediate residual-process timing, diagnostic observations, and complete
writable lifecycle success-path evidence.

Decision: `NO-GO` for correction, retry, a second P9-90 invocation, target
change or rematerialization, evidence reconstruction, fallback, process
termination, fixture repair or mutation, security-control change or bypass,
implementation or test change, broader workbook / VBProject mutation,
package / `dist`, release / publication, external services, staging, commit,
push, public API change, persisted schema change, canonical format change, or
Frozen specification change.

## Selected Next Candidate

**P9-91 - Corrected Residual-Process Evidence Execution Result Review**

P9-91 should remain documentation only and review the retained P9-90 fixed-
input validation, direct-process single invocation, empty stdout, stderr,
exit code `1`, pre-Excel failure boundary, and separate final safe-state audit.
It must not correct or invoke the target, execute or retry the lifecycle,
start Excel, operate on a workbook or fixture, query or mutate a process,
change or bypass a security control, or infer broader authorization.

## Preserved Invariants

P9-90 preserves the consumed P9-89 single-invocation authorization, the
unchanged P9-79 target, P9-72 plus P9-74 semantics, immediate residual-process
HARD-STOP rules, exact fixture identities and count, close without saving,
no fallback, no process termination, causal uncertainty, current security
controls, and separation between execution evidence, result review,
correction planning, and any later authorization.

## Verification

P9-90 verification consists of the recorded fixed-input byte checks, exactly
one direct child invocation, retained result inspection, separate read-only
safe-state audit, documentation consistency review, `git diff --check`,
trailing-whitespace scan, and Git branch and staged / unstaged inspection.
No correction, retry, second lifecycle invocation, process termination,
fixture mutation, implementation test, build, package / `dist`, release,
publication, external-service, stage, commit, or push operation is run.
