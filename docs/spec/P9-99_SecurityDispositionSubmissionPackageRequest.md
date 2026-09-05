# P9-99 - Security Disposition Submission Package Request

## Status

COMPLETE / docs-only submission-package request

## Purpose

Define the exact package that the responsible security authority must submit
for a future authoritative security disposition intake review under P9-97.
P9-99 requests information only. It does not supply or accept a disposition,
authorize continuation, or select technical execution.

## Preserved Current State

- P9-95 remains `INCOMPLETE / SAFE-STOP`.
- P9 continuation remains `NO-GO / SAFE-STOP`.
- Authoritative security disposition accepted: `No`.
- Continuation authorization accepted: `No`.
- Technical execution candidate: `None`.
- The P9-95 materialization failure and Avast `IDP.HELU.PSE90` detection /
  block remain separate `CONFIRMED` observations; causation remains
  `UNPROVEN`.
- The unused P9-94 invocation allowance is not reusable.

## Required Submitter and Authority

The submitter must be the security-product vendor, the organization's
designated security decision authority, or an expressly delegated security
representative authorized to issue the disposition for the identified event,
product, artifact, process, and environment. The submission must identify the
individual or attributable team, role, organization, authority basis, and a
verifiable approval or signature marker.

An operator, developer, repository contributor, test executor, or project
owner is not an acceptable disposition authority solely by virtue of that
role. A local risk opinion, project preference, or operational observation is
not a substitute for the required security authority.

## Acceptable Disposition Source and Type

Submit one authoritative, attributable, durable record, such as:

- an official security-product vendor support or analysis decision tied to a
  traceable case, report, or detection identifier; or
- a formal decision record issued by the designated organizational security
  authority under its documented authority.

The record must be preserved in its issued form or as a verifiable export that
retains provenance, issuer, date and time, decision wording, scope, evidence
basis, limitations, and approval marker. Informal conversation, an
unattributed screenshot, an operator summary, absence of a later alert, local
allow-listing, or an unrelated release or approval record is not acceptable.

## Required Evidence Fields

The package must contain all of the following:

1. submitter name or attributable team, role, organization, and authority
   basis;
2. security product or governing security authority, including product,
   engine, and definition or intelligence version where applicable;
3. traceable case, ticket, report, event, detection, or approval identifier;
4. the exact disposition result and the authority's unmodified decision
   wording;
5. evidence and analysis basis, including artifacts examined and method used;
6. correlation to the P9-95 event, including the `IDP.HELU.PSE90` detection /
   block, displayed or recorded event identifier and timestamp, fixed path,
   Windows PowerShell 5.1 process, and fixed P9-93 successor artifact or input
   identity;
7. whether causation between the materialization failure and security event is
   established, rejected, or unresolved, with supporting basis;
8. limitations, conditions, required follow-up, expiry, revocation, and
   supersession information; and
9. a verifiable signature, approval marker, or equivalent attribution.

If a requested correlation field is unavailable, the authority must state
that fact explicitly and explain whether the remaining evidence uniquely and
reliably identifies the reviewed event and artifact. The later intake review
remains fail-closed and may reject the package.

## Required Scope Identification

The submission must state both covered and excluded scope. Covered scope must
identify, as applicable:

- the specific P9-95 security event and detection classification;
- the exact fixed path, process identity, and P9-93 successor artifact or
  input identity reviewed;
- the host or environment and relevant security-product configuration basis;
- the action or conclusion addressed by the disposition; and
- whether the conclusion is event-specific, artifact-specific,
  version-specific, host-specific, time-limited, or generally applicable.

The submission must explicitly list anything not evaluated, including parser
correctness, PowerShell or runtime readiness, lifecycle behavior, workbook or
fixture safety, residual-process timing, release safety, and future or changed
artifacts unless the authority actually evaluated that item.

## Required Date, Time, and Definition Basis

The record must include its issue and effective date and time, including time
zone, plus any expiry or review date. It must identify the security-product,
engine, platform, policy, and detection-definition or intelligence versions
used for the decision, or explicitly state why a version field does not apply.
It must also identify the artifact version or cryptographic identity and the
event evidence on which the decision is based.

## Required Decision Wording

The authority must state one explicit result: `ACCEPTED`, `REJECTED`, or
`UNRESOLVED`, followed by its authoritative wording and conditions. The result
must say whether the identified artifact and event are malicious, unwanted,
false positive, acceptable only under stated controls, or not determinable,
using the authority's applicable classification. Ambiguous wording such as
"looks safe", "probably benign", or "no issue observed" is insufficient.

The submission must include these statements without qualification:

> This security disposition is not VMF P9 continuation authorization.

> This submission does not permit execution or retry; parser or PowerShell
> invocation; Excel, lifecycle, workbook, fixture, or process operation;
> testing or build; package or dist update; release, publication, or tag; or
> any flagged executable run.

> The P9-94 invocation allowance is not reusable.

Any later continuation authorization must be separate, task-specific,
explicitly accepted, and followed by a separate GO / NO-GO review. It must not
change, weaken, exclude, allow-list, or evade a security control.

## Explicit Non-Acceptance Conditions

The package must be recorded as `NOT ACCEPTED` or `INCOMPLETE` for continuation
purposes if any of the following applies:

- the submitter, role, authority, provenance, or approval marker is missing,
  unverifiable, or outside the covered decision authority;
- the source is informal, unattributed, operator-authored, or only a local
  risk acceptance, screenshot, absence of detection, or unrelated record;
- required evidence, exact decision wording, evidence basis, correlation,
  covered scope, excluded scope, date and time, time zone, identity, version,
  definition basis, limitation, expiry, or supersession data is missing or
  ambiguous;
- the record concerns a different event, path, process, artifact, version,
  host, environment, policy, or definition basis without an authoritative
  explanation establishing applicability;
- the record is expired, revoked, superseded, conditional on an unsatisfied
  prerequisite, internally inconsistent, altered, or incomplete;
- causation or safety is inferred beyond the authority's stated evidence and
  scope;
- the record depends on changing, disabling, weakening, excluding,
  allow-listing, or bypassing Avast or another security control;
- the submission attempts to reuse P9-94, grant continuation, select technical
  execution, or treat security disposition as parser, runtime, lifecycle,
  release, or publication approval; or
- any required statement in the preceding section is absent, qualified, or
  contradicted.

Non-acceptance does not resolve causation or alter the current safe-stop.

## Recommended Responsible-Party Response Template

```text
Security Disposition Submission

Submitter / attributable team:
Role and organization:
Authority basis / delegation:
Security product / governing security authority:
Product, engine, policy, and definition or intelligence version:
Case / ticket / report / event / detection identifier:

Disposition result: ACCEPTED | REJECTED | UNRESOLVED
Authoritative decision wording:
Classification and conditions:

Evidence basis and analysis method:
Artifacts and evidence examined:
Causation finding: ESTABLISHED | REJECTED | UNRESOLVED
Causation basis:

Scope covered:
- Event and detection:
- Exact path:
- Process identity:
- Artifact / input version and cryptographic identity:
- Host / environment:
- Applicable action, time period, and conditions:

Scope excluded:
Limitations / required follow-up:
Expiry / revocation / supersession terms:

Issued date/time and time zone:
Effective date/time and time zone:
Version / definition basis date/time:

Whether continuation authorization is included: No

Required statements:
This security disposition is not VMF P9 continuation authorization.
This submission does not permit execution or retry; parser or PowerShell
invocation; Excel, lifecycle, workbook, fixture, or process operation;
testing or build; package or dist update; release, publication, or tag; or
any flagged executable run.
The P9-94 invocation allowance is not reusable.

Signature / approval marker:
Verification reference or contact channel:
```

The continuation-authorization response must remain `No` in this package.
Continuation authorization, if ever proposed, requires a separate explicit
request and acceptance and is not part of this disposition submission.

## Decision

Decision: `COMPLETE / docs-only submission-package request`.

Decision: the submission package above is the exact request to send to the
responsible security authority. P9-99 itself does not contact that authority
or any external service.

Decision: no authoritative security disposition is supplied or accepted, no
continuation authorization is supplied or accepted, and no technical
execution candidate is selected.

Decision: P9 continuation remains `NO-GO / SAFE-STOP`, and the P9-94 allowance
remains non-reusable.

## Prohibited Operations

P9-99 does not execute or re-execute a parser or project PowerShell script;
run Excel, tests, a build, lifecycle, workbook, fixture, process, package,
`dist`, release, publication, tag, external-service, or flagged-executable
operation; change Avast settings, exclusions, exceptions, quarantine, or
allow-list state; attempt a workaround; modify implementation, Frozen
specifications, public APIs, canonical formats, or persisted schemas; or
stage, commit, or push.

## Next Boundary

There is no technical execution candidate. After a conforming authoritative
security disposition is actually supplied, a separately requested docs-only
intake review may evaluate it against P9-97 and P9-99. An accepted disposition
alone does not authorize continuation. A later continuation candidate requires
separate task-specific continuation authorization and a separate GO / NO-GO
review.

## Verification

Verification is static text checking only: run `git diff --check`, scan the
four touched Markdown files for trailing whitespace, and inspect Git status.
No prohibited operation is run.
