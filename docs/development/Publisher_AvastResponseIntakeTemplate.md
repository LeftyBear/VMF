# Publisher Avast Response Intake Template

Status  : Template only / no Avast response received
Scope   : Safe intake record for future Avast false-positive response review
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_PreflightHardening.md, docs/distribution/PublisherReleaseRunbook.md

This template is for recording a future Avast false-positive response without
exposing sensitive data or reopening the release gate by implication. Creating
or filling this template does not approve a release, resolve the current Avast
pending state, authorize package or distribution work, execute Live E2E, mutate
Google Docs or Google Drive, re-run a flagged executable, create tags, publish
artifacts, change production code, change public APIs, or modify Frozen
specifications.

Until an actual vendor response is received, redacted, reviewed, and recorded
against the exact selected artifact identity, the formal state remains:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending.

## 1. Intake Metadata

| Field | Value |
| --- | --- |
| Intake record date |  |
| Avast response received date |  |
| Avast case ID |  |
| Avast submission ID |  |
| Response source |  |
| Reviewed by |  |
| Selected artifact path |  |
| Selected artifact SHA-256 |  |
| Related release candidate commit |  |
| Related release gate record |  |

## 2. Response Classification

Select exactly one classification after review.

| Classification | Selected | Required Interpretation |
| --- | --- | --- |
| False positive confirmed |  | Vendor explicitly confirms the detection was a false positive for the selected artifact identity. This is evidence for reopening the next gate only after owner decision. |
| Detection remains |  | Vendor confirms or preserves the detection. Release hold continues unless a repository-owner exception decision is separately recorded. |
| More information requested |  | Vendor asks for additional artifact, environment, or diagnostic information. Release hold continues. |
| Inconclusive / no action |  | Vendor response does not clearly classify the selected artifact. Release hold continues. |

Do not classify a response as false-positive confirmation from silence,
submission acceptance, automated acknowledgement, exception creation,
VirusTotal no-detection, or scanner behavior outside the selected artifact
identity.

## 3. Required Evidence Checklist

| Evidence Item | Status | Notes |
| --- | --- | --- |
| Response source recorded without private URL or token material | PENDING |  |
| Avast case or submission identifier recorded | PENDING |  |
| Response received date recorded | PENDING |  |
| Reviewer recorded | PENDING |  |
| Selected artifact path recorded without local secret-bearing path | PENDING |  |
| Selected artifact SHA-256 recorded | PENDING |  |
| Vendor classification text summarized safely | PENDING |  |
| Response classification selected | PENDING |  |
| Redaction review complete | PENDING |  |
| Release gate decision recorded | PENDING |  |
| Required follow-up authorizations recorded separately | PENDING |  |

Use `PASS` only for evidence directly verified during the intake review. Leave
unknown, missing, blocked, or deferred evidence as `PENDING`, `BLOCKED`, `NOT
EXECUTED`, or `DEFERRED`.

## 4. Redaction Rules

Do not record:

- tokens;
- credentials;
- refresh tokens;
- client secrets;
- private keys;
- local paths that expose usernames, secret locations, token stores, or private
  workspace structure;
- private URLs;
- raw exceptions;
- HTTP request bodies;
- HTTP response bodies;
- stack traces;
- full scanner logs that contain machine, account, or path details.

Allowed record content is limited to safe summaries, public product names,
case/submission identifiers, dates, selected artifact hashes, stable status
labels, and reviewer decisions.

## 5. Release Gate Decision

Select exactly one decision after the evidence checklist is complete.

| Decision | Selected | Meaning |
| --- | --- | --- |
| Resume allowed |  | Vendor clearance is confirmed for the selected artifact identity and the repository owner explicitly reopens the required next gate. This does not authorize package work, executable smoke, Live E2E, release, tag, or publication by itself. |
| Hold continues |  | Evidence is missing, inconclusive, asks for more information, preserves the detection, or lacks owner gate reopening. |
| Escalation required |  | The response conflicts with local evidence, requires security review, requires legal or owner risk acceptance, or cannot be safely summarized. |

Default decision before review completion: `Hold continues`.

## 6. Resume Conditions

All required resume conditions must be recorded before moving beyond the
current release hold.

| Condition | Status | Evidence |
| --- | --- | --- |
| Vendor clearance confirmed for the selected artifact identity | PENDING |  |
| Repository owner explicitly reopens the required next gate | PENDING |  |
| Flagged executable re-run explicitly authorized, if needed | PENDING |  |
| Live E2E explicitly authorized, if needed | PENDING |  |
| Package creation/update explicitly authorized, if needed | PENDING |  |
| `dist` write explicitly authorized, if needed | PENDING |  |
| Release explicitly authorized, if needed | PENDING |  |
| Tag creation explicitly authorized, if needed | PENDING |  |
| Publication explicitly authorized, if needed | PENDING |  |

Authorization for one condition does not authorize any other condition.

## 7. Operator Notes

Record only sanitized notes needed to understand the intake decision.

```text

```

## 8. Decision Log

| Date | Actor | Decision | Evidence Reference | Remaining Blockers |
| --- | --- | --- | --- | --- |
|  |  | Hold continues |  | Avast response not recorded; release gate remains blocked. |

## 9. Explicit Non-Actions

This intake template does not:

- receive or assert an Avast response;
- resolve Avast false-positive handling;
- approve release readiness;
- approve package creation or package update;
- write to `dist`;
- authorize flagged executable re-run;
- authorize Live E2E;
- authorize Google Docs or Google Drive mutation;
- authorize release, tag creation, or publication;
- change production code;
- change tests;
- change public APIs;
- modify Frozen specifications.
