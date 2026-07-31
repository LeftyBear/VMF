# Publisher Phase 3-10 Release Notes

Date: 2026-07-31

## Scope

Phase 3-10 executes the repository-owner approved Publisher release gate after
Phase 3-9 release readiness evidence was recorded. This phase preserves Frozen
specifications, public APIs, schemas, canonical formats, and production
behavior.

This phase does not create tags, publish GitHub Releases, announce public
distribution, or change package trust posture.

## Release Gate Status

- Phase 3-10 release execution was explicitly authorized by the repository
  owner on 2026-07-31.
- The Avast blocker is treated as resolved for this release execution and is
  recorded in `docs/distribution/ReleaseChecklist.md`.
- Release tags, GitHub Release publication, and release announcement remain
  unexecuted and require separate authorization.
- Google Live E2E was explicitly enabled for the approved run and passed the
  Success, Revision Conflict, Readback Mismatch, and Empty Plan cases.

## Pending Risk Management

The previous Avast blocker was closed for this run by repository-owner
instruction to treat the blocker as resolved. Earlier local evidence remains
recorded in the checklist, including package verification, matching hashes,
configuration exclusion, and no VirusTotal detection at the time of the Phase
3-9 readiness pass.

The selected Phase 3-10 package artifact is:

- `dist\release\Publisher\vmf-publisher-0.0.0-dev-win-x64.zip`
- SHA-256:
  `404F6D4B382132802CEF5F42A00A6B53E7C7177E3ABFC56C3DD518DE435C7742`
- Size: 973097 bytes

## Phase 4 Entry Conditions

Phase 4 may proceed only after the Phase 3-10 release execution records remain
reviewed and no separate tag, publication, production-behavior, public-API, or
Frozen-specification change is inferred from this evidence update.

Phase 4 work must preserve Frozen specifications, public APIs, persisted
schemas, canonical formats, production defaults, package artifacts, and release
state unless a separate explicit authorization is given. Any change to the
package trust posture, signing model, installer model, or production release
process must be isolated as a vNext candidate before adoption.
