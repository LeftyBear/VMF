# VMF-SOURCE-MANIFEST-V1 Current Canonical Digest Governance Decision

> **Current applicability:** **HISTORICAL FOR CURRENT WORKING-SOURCE
> IDENTITY**. The 30-file digest remains valid historical evidence and is not
> erroneous. The current operational identity is the synchronized **32-file / 4188-byte**
> baseline with SHA-256
> `bbb5b0ca72d6411165e660246aeb8dae9fe3232741bfd1dae5d4350dcf489e35`.

## 1. Record status

- Route: `C`
- Work mode: `governance documentation only`
- Status: `ADOPTED`
- User decision: `Option A`
- Candidate preparation authorization: `NOT GRANTED`
- Real Experiment authorization: `NOT GRANTED`
- Retry authorization: `NOT GRANTED`
- Security disposition: `UNRESOLVED`
- Frozen modified: `NO`

The User selected Option A. For the current 30-file ROT-only source state, the
canonical `VMF-SOURCE-MANIFEST-V1` digest is:

```text
6972b3f418a0371862d3251c1c52f6178c872d3e5d5c9e14c4c09f215c596511
```

## 2. Current canonical source state

The adopted current source-state values are:

```text
File count: 30
Canonical manifest byte length: 3910
SHA-256: 6972b3f418a0371862d3251c1c52f6178c872d3e5d5c9e14c4c09f215c596511
```

The value was independently reproduced byte-for-byte by PowerShell and Python
implementations of the formal algorithm. The canonical manifest bytes are
UTF-8 without BOM, use LF only, contain exactly 30 canonical lines with
exactly two TAB separators per line, use repository-relative normalized `/`
paths in ordinal order, contain lowercase SHA-256 values, end with exactly one
final LF, and contain no incidental pipeline, object, or index output.

## 3. Historical digest

The historical digest is:

```text
1f6c000bda2d48a57d647b45c05d06ff21e31571f0968e99592287f556e66425
```

Its status remains:

```text
HISTORICAL / PROVENANCE UNRESOLVED
```

This decision does not classify the historical digest as technically
incorrect or erroneous. The historical value is not an authorized candidate
gate for the current 30-file source state.

## 4. Candidate state and future invalidation

No ROT-only candidate is currently accepted or pinned. Prior failed
candidate-preparation roots remain rejected. No candidate is created by this
decision.

Future candidate preparation for this unchanged source state must use:

```text
File count: 30
SHA-256: 6972b3f418a0371862d3251c1c52f6178c872d3e5d5c9e14c4c09f215c596511
```

Candidate preparation may resume only under a separate authorization. Any
subsequent authorized source or test modification invalidates this
source-state digest and requires recomputation.

## 5. Retained governance boundaries

This decision introduces no execution authorization. The real Experiment
remains `NO-GO / SAFE-STOP`, retry remains unauthorized, and the security
disposition remains `UNRESOLVED`.

This decision changes no source code, test code, public API, Schema 4
semantics, ROT-only architecture, Frozen specification, or security
disposition. It does not authorize or perform candidate creation, restore,
build, test, Excel/COM access, Experiment execution, run-directory creation,
invocation GUID generation, process manipulation, `build.ps1`, network access,
security changes, release activity, or Git mutation.

## 6. Decision

```text
A. CANONICAL DIGEST ADOPTED - READY FOR FRESH ROT-ONLY CANDIDATE PREPARATION
```

Readiness means only that a fresh candidate-preparation task may be considered
under a separate explicit authorization. It does not begin candidate
preparation, create or pin a candidate, authorize a retry or real Experiment,
or clear `NO-GO / SAFE-STOP`.
