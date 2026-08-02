# Publisher Preflight Hardening

Status  : Done
Scope   : Documentation-only / local-only preflight hardening while Avast false-positive handling remains pending
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_TestClassification.md, docs/distribution/PublisherReleaseRunbook.md, docs/development/Publisher_Phase4_LocalVerificationChecklist.md

This document hardens the Publisher preflight boundary. It is documentation
only. It does not approve a release, create or update packages, create tags,
publish artifacts, execute Live E2E, mutate Google Docs or Google Drive,
re-run flagged artifacts, change production code, change production design,
change public APIs, or modify Frozen specifications.

## 1. Current Formal State

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending. Local-only verification may be
complete while release readiness remains unestablished. This document exists to
prevent that local evidence from being accidentally treated as release evidence.

## 2. Allowed Work During Avast Pending

The following work may continue during the Avast-pending hold when it stays
within the approved local-only boundary:

- read-only investigation;
- documentation updates;
- Git state inspection and diff hygiene checks;
- source build when it does not create or update packages;
- Publisher unit tests;
- non-live integration tests with `VMF_PUBLISHER_GOOGLE_E2E` disabled;
- mock-backed verification;
- local dry-run verification that does not publish and does not execute the
  flagged package executable;
- static inspection of an existing package only when explicitly in scope and
  only when no executable is run.

Evidence from these activities must be reported as local, non-live,
mock-backed, dry-run, documentation, or static evidence. It must not be promoted
to release readiness, Live E2E, Google Docs readback, Google Drive cleanup,
package approval, publication approval, or antivirus vendor clearance.

## 3. Blocked Work During Avast Pending

The following work remains blocked until the required gate is explicitly
reopened and the operation is separately authorized:

- release approval or rejection;
- release tag creation;
- GitHub Release creation or update;
- artifact publication or release announcement;
- package creation, replacement, or update;
- writing to `dist/`;
- Live E2E;
- setting `VMF_PUBLISHER_GOOGLE_E2E=1`;
- Google Docs mutation;
- Google Drive mutation;
- token-store mutation;
- temporary public image hosting;
- re-running the Avast-pending flagged executable;
- treating VirusTotal no-detection, a local antivirus exception, or a
  false-positive submission as Avast vendor clearance;
- changing production code, production defaults, public APIs, persisted
  schemas, canonical formats, or Frozen specifications.

Authorization for one blocked operation does not authorize any other blocked
operation.

## 4. Preflight Hard Stops

Stop before executing the next command and report the boundary issue if any of
these conditions are true:

- current status is not read or the release gate state is unknown;
- the target branch, target commit, version, package path, or artifact SHA-256
  is ambiguous;
- there are staged changes that are unrelated to the approved task;
- a command would create, replace, update, or publish a package;
- a command would write under `dist/`;
- a command would create or move a release tag;
- a command would create, update, upload to, or publish a GitHub Release;
- a command would enable `VMF_PUBLISHER_GOOGLE_E2E`;
- a command would mutate Google Docs or Google Drive;
- a command would mutate token stores or temporary public hosting;
- a command would execute the Avast-pending flagged package executable;
- release readiness, go/no-go, Live E2E, package approval, or antivirus
  clearance would be inferred from local-only evidence;
- a scanner result is disabled, inconclusive, pending, or mismatched to the
  selected artifact and no repository-owner exception decision is recorded;
- Frozen specifications, production code, production defaults, public APIs,
  persisted schemas, or canonical formats would need to change.

These hard stops apply before package, release, Live E2E, and publication work.
They also apply during documentation tasks if a proposed wording would imply
that the release gate has reopened.

## 5. Resume Conditions

Release-path work may resume only after all applicable conditions below are
recorded in the approved release or security review records:

1. The Avast response is received, dated, and tied to the exact selected
   artifact path and SHA-256.
2. The response is interpreted as one of: vendor clearance, confirmed
   detection, inconclusive response, or repository-owner exception decision.
3. If the artifact is cleared or excepted, the selected package identity is
   revalidated before any executable smoke run.
4. The repository owner explicitly reopens only the required next gate.
5. Package creation or update, packaged executable smoke, Live E2E, and
   publication each receive separate operation-specific authorization before
   execution.
6. Local source verification is rerun before release readiness is reconsidered.
7. Security and supply-chain review is completed for the selected artifact.
8. Go/no-go is recorded before publication.

If Avast confirms the detection or the response does not match the selected
artifact identity, release work remains blocked and the next task must be a
separate remediation, rebuild, repackage, or abandon-candidate decision.

## 6. Reporting Requirements

Every preflight, runbook, classification, status, or release-resume record must
state:

- whether Live E2E was enabled;
- whether Google Docs or Google Drive were mutated;
- whether packages or `dist` artifacts were created, replaced, or updated;
- whether the flagged executable was run;
- whether release, tag, publication, or push was performed;
- whether Frozen specifications, production code, public APIs, persisted
  schemas, or canonical formats changed;
- which blocked or deferred operations remain unresolved.

Use `PASS` only for directly executed and directly verified evidence. Keep
`PENDING`, `BLOCKED`, `NOT EXECUTED`, and `DEFERRED` when evidence has not been
produced.

## 7. Explicit Non-Actions

This preflight hardening did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, write to `dist`, re-run flagged executables, push
commits, change Frozen specifications, change public APIs, change production
code, or change production design.
