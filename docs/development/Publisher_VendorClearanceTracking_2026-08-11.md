# Publisher Vendor Clearance Tracking - 2026-08-11

Date: 2026-08-11
Mode: docs-only / local-only

## Decision

Publisher `0.0.1-dev` release-gate handling will proceed by obtaining vendor
clearance, not by risk acceptance.

This tracking record does not approve release execution, create or update
packages, update `dist`, create tags, publish a GitHub release, execute Live
E2E, mutate Google Docs or Google Drive, re-run the flagged executable, change
production code, change tests, change public APIs, or modify Frozen
specifications.

## Current State

- release blocked
- Hold continues
- Avast response pending
- vendor clearance not yet obtained

## Vendor Clearance Criteria

Vendor clearance means one or more of the following has been received,
recorded, and reviewed for the exact Publisher artifact and release path:

- Avast false positive confirmation
- Avast detection removal
- vendor safety confirmation
- authorized security/distribution owner confirmation based on vendor response

Local observation, manual scan behavior, absence of a reproduced detection,
VirusTotal no-detection, or VMF-side risk acceptance is not vendor clearance by
itself.

## Blocked Before Clearance

Until vendor clearance is obtained and reviewed, the following remain blocked:

- Live E2E
- Google Docs / Drive mutation
- package / dist update
- tag / GitHub release
- flagged executable rerun

## After Clearance

After vendor clearance is obtained:

- final verification is still required
- release authorization is still required
- clearance alone is not release approval

Any release-path action after clearance requires the applicable verification
evidence, approval record, and operation-specific authorization before the
action is performed.
