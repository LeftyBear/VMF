# Publisher Preflight Hardening

Status  : Done / post-hold boundary updated by ADR-0019
Scope   : Documentation-only / local-only preflight hardening after VMF risk acceptance
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_TestClassification.md, docs/distribution/PublisherReleaseRunbook.md, docs/development/Publisher_Phase4_LocalVerificationChecklist.md

This document hardens the Publisher preflight boundary. It is documentation
only. It does not approve a release, create or update packages, create tags,
publish artifacts, execute Live E2E, mutate Google Docs or Google Drive,
re-run flagged artifacts, change production code, change production design,
change public APIs, or modify Frozen specifications.

## 1. Current Formal State

The current formal state is:

`0.0.1-dev release completion recorded / post-release closeout complete / next version or next phase requires a new scope`.

Avast vendor clearance remains not obtained, Avast safety certification is not
claimed, and the 2026-07-25 False Positive submission remains unanswered. VMF
has accepted the residual risk through ADR-0019 and lifted the Release Hold.
The `0.0.1-dev` post-hold sequence is recorded complete. This document keeps
the release-gate sequence explicit for future scoped release-path work:

`final verification -> Live E2E -> result review -> package/dist -> tag/release`.

## 2. Allowed Work Before Post-Hold Execution

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

## 3. Gated Work After Hold Lift

The following work remains gated until the ADR-0019 sequence reaches that step
and the operation is separately authorized:

- final verification;
- Live E2E;
- result review;
- package creation, replacement, or update;
- writing to `dist/`;
- release tag creation;
- GitHub Release creation or update;
- artifact publication or release announcement;
- setting `VMF_PUBLISHER_GOOGLE_E2E=1`;
- Google Docs mutation;
- Google Drive mutation;
- token-store mutation;
- temporary public image hosting;
- re-running a previously flagged executable;
- treating standalone Avast no-detection, setting-dependent behavior,
  VirusTotal no-detection, a local antivirus exception, or a false-positive
  submission as Avast vendor clearance;
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
- a command would execute a previously flagged package executable without
  exact authorization;
- release readiness, go/no-go, Live E2E, package approval, or antivirus
  clearance would be inferred from local-only evidence;
- a scanner result is disabled, inconclusive, pending, or mismatched to the
  selected artifact and no repository-owner risk acceptance or exception
  decision is recorded;
- Frozen specifications, production code, production defaults, public APIs,
  persisted schemas, or canonical formats would need to change.

These hard stops apply before package, release, Live E2E, and publication work.
They also apply during documentation tasks if a proposed wording would imply
that the release gate has reopened.

## 5. Post-Hold Sequence Conditions

Release-path work may proceed only after all applicable conditions below are
recorded in the approved release or security review records:

1. ADR-0019 risk acceptance is recorded and cited instead of vendor clearance.
2. Final verification is explicitly authorized and run first.
3. Live E2E is explicitly authorized and run only after final verification.
4. Results are reviewed before package/dist work.
5. Package/dist work is explicitly authorized only after result review.
6. Tag/release work is explicitly authorized only after package/dist work and
   review.
7. Security and supply-chain review records that vendor clearance is not
   obtained and the release relies on VMF residual risk acceptance.

If a future Avast response confirms detection or conflicts with the selected
artifact identity, release work stops and the next task must be a separate
remediation, rebuild, repackage, abandon-candidate, or risk reassessment
decision.

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
