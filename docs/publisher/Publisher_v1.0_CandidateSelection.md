# Publisher v1.0 Candidate Selection

Status: SELECTED
Scope: Publisher v1.0 completion candidate
Depends:
- `Publisher_v1.0_ScopeFreeze.md`
- `Publisher_v1.0_DefinitionOfDone.md`

## Selected Candidate

Publisher v1.0 completion candidate is fixed to:

- Commit: `050b2f1e3b9c6e00c0352abcbf590013f4be9d12`
- Branch: `main`

This commit is the baseline against which Publisher v1.0 Definition of Done
verification is performed.

## Selection Rule

The selected candidate includes the frozen Publisher implementation baseline
and all accepted changes up to the selected commit.

No additional vNext enhancement is required for candidate completeness.

Changes after this candidate may enter the v1.0 completion path only when they
are:

1. required to correct a v1.0-blocking defect;
2. required to satisfy the Publisher v1.0 Definition of Done; or
3. required to correct completion verification or evidence.

Any such change requires selecting a new candidate commit.

## Historical Boundary

The selected v1.0 candidate is distinct from the historical published
`0.0.1-dev` prerelease candidate.

Historical `0.0.1-dev` verification evidence may be reused only where its
applicability to this selected candidate is explicitly confirmed.

## Next Step

Evaluate the selected candidate against the Publisher v1.0 Definition of Done
and determine which verification evidence may be reused and which checks must
be rerun.
