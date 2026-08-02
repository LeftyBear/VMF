# Publisher Operator Guidance - Avast Hold

Status  : Hold. Await Avast response.
Scope   : Local-only operator guidance while the Publisher release gate remains blocked by Avast false-positive handling
Depends : docs/distribution/PublisherReleaseRunbook.md, docs/development/Publisher_TestClassification.md, docs/development/CURRENT_STATUS.md, docs/development/Publisher_v1.0_Implementation_Voyage_Log.md

This document gives operators the current allowed and blocked actions while the
Publisher release gate is held for Avast false-positive handling. It is
documentation only. It does not approve a release, create or update packages,
create tags, publish artifacts, execute Live E2E, mutate Google Docs or Google
Drive, re-run the flagged executable, push commits, change production code,
change public APIs, or modify Frozen specifications.

## 1. Current State

| Item | Current State |
| --- | --- |
| Formal state | Phase 4 local-only verification complete |
| Release gate | Blocked |
| Current decision | Hold. Await Avast response. |
| Avast false positive | Pending |
| Product regression | Not indicated by the current records |

The current hold is operational. It records an unresolved release-gate
condition, not a Publisher product regression. Phase 4 local-only verification
may remain complete while release readiness remains blocked.

## 2. Allowed Actions During Hold

Operators may perform only local, non-mutating, non-release work:

- build;
- unit tests;
- mock-backed verification;
- dry-run verification that does not publish, mutate Google resources, create
  packages, or re-run the flagged executable;
- documentation updates;
- existing package inspection only, when the inspection is static and does not
  create, update, publish, or execute the package.

Allowed results must be reported as local, non-live, mock-backed, dry-run, or
static evidence. They must not be promoted to release readiness, Live E2E
evidence, Google Docs readback evidence, Google Drive cleanup evidence, package
approval, publication approval, or antivirus vendor clearance.

## 3. Blocked Actions During Hold

The following actions remain blocked until the specific release gate is reopened
with explicit authorization after the Avast response is recorded:

- Live E2E;
- Google Docs mutation;
- Google Drive mutation;
- package or distribution artifact creation;
- package or distribution artifact update;
- release, tag, or publication;
- flagged executable re-run;
- push.

Authorization for one blocked action does not authorize any other blocked
action.

## 4. Decision Rules

Do not proceed to any release path before the Avast response is received,
recorded, and interpreted against the affected artifact identity.

Treat the current hold as operational, not a product regression. Do not use the
hold to reopen completed local-only implementation work unless a separate
source-level defect is identified.

When the Avast response arrives, resume documentation and release-gate records
in this order:

1. Runbook;
2. TestClassification;
3. CURRENT_STATUS;
4. Voyage Log.

After those records are synchronized, reopen only the operation-specific gate
that has explicit repository-owner authorization.

## 5. Stop Conditions

Stop and report before proceeding if:

- the Avast response does not identify the affected artifact clearly;
- the selected artifact path, version, or SHA-256 is ambiguous;
- a command would create or update `dist`;
- a command would execute the flagged package before Avast clearance or an
  explicit owner exception;
- `VMF_PUBLISHER_GOOGLE_E2E` would be enabled;
- Google Docs or Google Drive would be mutated;
- release, tag, publication, or push is requested without separate explicit
  authorization;
- Frozen specifications, public APIs, persisted schemas, canonical formats, or
  production defaults would need to change.

## 6. Operator Reporting

Operator reports during the hold must state whether each blocked operation was
performed or not performed. Use `PASS` only for directly executed and directly
verified evidence. Keep `PENDING`, `BLOCKED`, `NOT EXECUTED`, and `DEFERRED`
when evidence has not been produced.
