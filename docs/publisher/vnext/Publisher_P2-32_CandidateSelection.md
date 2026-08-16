# Publisher P2-32 Candidate Selection

## Decision
GO: first narrow local-only preview-update implementation.

## Selected Candidate
Implement local-only preview-update planning.

## Scope
- Generate preview-update plan from markdown input.
- Emit local-only boundary metadata.
- Do not apply Google Docs updates.
- Do not access Google Docs, Drive, OAuth, or token-store.
- Do not perform Live E2E, package, dist, release, tag, or flagged executable re-run.

## Required Boundary
- googleApiAttempted: false
- driveAttempted: false
- oauthAttempted: false
- tokenStoreAttempted: false
- physicalUpdateApplied: false
- readbackVerified: false
- publicationAuthorized: false
- releaseClearance: false

## Non-Goals
- Live document mutation
- Docs batchUpdate execution
- Drive file lookup or create/update
- OAuth authorization
- Readback verification
- Release execution
