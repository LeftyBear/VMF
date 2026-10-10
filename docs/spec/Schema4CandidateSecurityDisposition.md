# Schema-4 Candidate-Specific Security Disposition

> **Applicability after Schema-4 closeout:** This document is retained as historical and supporting governance evidence. Any pre-closeout operational state recorded here, including `NOT STARTED`, `NO-GO / SAFE-STOP`, unused authorization, future-route, or similar wording, is historical and does not describe the current Schema-4 execution status. This document grants no present execution authority and is not reusable authorization. For current operational status, use the committed Schema-4 closeout records in `docs/development/CURRENT_STATUS.md`, `docs/development/HANDOFF.md`, and `docs/VMF_vNext_Backlog.md`.

## Status

**ADOPTED — OPTION B SELECTED**

This adopted disposition applies only to candidate
`ca2321ce-feac-4d36-8039-8b7c2d349af9`. The User selected
**B. BOUNDED RISK ACCEPTANCE FOR REVIEW**. This candidate-specific selection
permits progression only to a separate GO/NO-GO review and does not authorize
Experiment execution.

## 1. Exact Candidate and Evidence Baseline

- Candidate ID: `ca2321ce-feac-4d36-8039-8b7c2d349af9`
- Candidate state: immutable and pinned
- Canonical source identity:
  - file count: `32`
  - byte length: `4188`
  - SHA-256: `bbb5b0ca72d6411165e660246aeb8dae9fe3232741bfd1dae5d4350dcf489e35`
- C0 verification: `74/74 PASS`
- Candidate Preparation: `COMPLETE`
- Pin-Set Pre-Execution Review: `COMPLETE`
- Candidate identities, provenance, and artifacts: `VERIFIED`
- Publication-command correlation: `PASS`
- Endpoint-protection observation during preparation: no endpoint-protection
  event observed
- Excel/COM or Experiment execution during preparation: `NONE`
- Security: `UNRESOLVED`
- Real Experiment: `NO-GO / SAFE-STOP`
- Preparation authority: `CONSUMED / NON-REUSABLE`
- Previous Experiment and security authorizations: `CONSUMED / NON-REUSABLE`

Successful candidate preparation, verification, and publication-command
correlation do not establish endpoint-security safety. The absence of an
observed endpoint-protection event during preparation does not disprove or
resolve the prior Avast/security uncertainty.

## 2. Disposition A - Continue SAFE-STOP

If the User selects **A. CONTINUE SAFE-STOP**:

- security remains `UNRESOLVED`;
- the real Experiment remains `NO-GO / SAFE-STOP`; and
- no Experiment authorization is possible under this disposition.

This disposition grants no continuation authority, technical GO, command
authority, or Experiment execution authority.

## 3. Disposition B - Bounded Risk Acceptance for Review

Under the User-selected **B. BOUNDED RISK ACCEPTANCE FOR REVIEW** disposition:

- security remains `UNRESOLVED`;
- the remaining security uncertainty is accepted only for consideration of
  one bounded, candidate-specific Experiment using candidate
  `ca2321ce-feac-4d36-8039-8b7c2d349af9`;
- the acceptance does not itself authorize Experiment execution;
- a separate candidate-specific GO/NO-GO decision remains required;
- only after a separate GO, a separate explicit one-use Experiment
  authorization remains required;
- no retry is permitted;
- any endpoint-protection detection or block requires immediate stop; and
- no endpoint-protection exclusion, disablement, allowlisting, bypass, or
  workaround is permitted.

This disposition does not establish that the candidate is safe, does not mark
security `RESOLVED`, and does not grant continuation authority, technical GO,
command authority, Excel/COM authority, or Experiment execution authority.

## 4. Preserved Boundaries

- The candidate is immutable and pinned.
- Any mutation, rename, addition, deletion, replacement, or path change
  invalidates this disposition and requires return to `SAFE-STOP`.
- `tools/build/build.ps1` is unauthorized.
- Production Build.Host is unauthorized.
- Security controls may not be changed.
- No prior Experiment authorization may be reused.
- Candidate execution, Excel/COM access, restore, build, publish, and test are
  unauthorized by this disposition.
- The real Experiment remains `NO-GO / SAFE-STOP` until all later independent
  gates are completed with valid, candidate-specific authority.
- This disposition authorizes no Git operation and no `CURRENT_STATUS.md` or
  `HANDOFF.md` synchronization.

## 5. User Decision

```text
A. CONTINUE SAFE-STOP
B. BOUNDED RISK ACCEPTANCE FOR REVIEW

SELECTED DISPOSITION: B. BOUNDED RISK ACCEPTANCE FOR REVIEW
SELECTED BY: USER
```

The User selected Option B. This selection is candidate-specific, leaves
security `UNRESOLVED`, and permits only progression to a separate GO/NO-GO
review. It does not authorize Experiment execution.
