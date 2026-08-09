# Publisher Avast Response Intake Template

Status  : Template only / no Avast response received; VMF risk acceptance recorded separately
Scope   : Safe intake record for future Avast false-positive response review after ADR-0019 risk acceptance
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_PreflightHardening.md, docs/distribution/PublisherReleaseRunbook.md

This template is for recording a future Avast false-positive response without
exposing sensitive data or reopening the release gate by implication. Creating
or filling this template does not approve a release, resolve the current Avast
pending state, authorize package or distribution work, execute Live E2E, mutate
Google Docs or Google Drive, re-run a flagged executable, create tags, publish
artifacts, change production code, change public APIs, or modify Frozen
specifications.

Until an actual vendor response is received, redacted, reviewed, and recorded
against the exact selected artifact identity, vendor clearance remains not
obtained and Avast safety certification is not claimed.

ADR-0019 records VMF-side residual risk acceptance and lifts the Release Hold
without treating the state as vendor clearance.

The False Positive submission sent to Avast on 2026-07-25 remains unanswered
as of 2026-08-09.

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
| VMF risk acceptance record | `docs/architecture/ADR-0019-vmf-risk-acceptance-and-release-hold-lift.md` |

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
standalone scanner no-detection, setting-dependent scanner behavior,
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
| Hold continues |  | Evidence is missing, inconclusive, asks for more information, preserves the detection, or requires a new owner gate decision. |
| Escalation required |  | The response conflicts with local evidence, requires security review, requires legal or owner risk acceptance, or cannot be safely summarized. |

Default vendor-clearance decision before review completion: vendor clearance
not obtained. Current release-hold decision is governed by ADR-0019 VMF risk
acceptance.

## 6. Resume Conditions

All required resume conditions must be recorded before moving beyond the
current release hold.

| Condition | Status | Evidence |
| --- | --- | --- |
| Vendor clearance confirmed for the selected artifact identity | PENDING | Vendor clearance not obtained unless a future Avast response confirms it. |
| VMF residual risk accepted without vendor clearance | PASS | ADR-0019 records VMF risk acceptance and Release Hold lift. |
| Repository owner explicitly reopens the required next gate | PENDING | Follow ADR-0019 order: final verification, Live E2E, result review, package/dist, tag/release. |
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
| 2026-08-09 | Repository owner / VMF | VMF risk accepted; Release Hold lifted; vendor clearance not obtained | ADR-0019 | Avast response not received; final verification, Live E2E, result review, package/dist, and tag/release not executed. |

## 9. Explicit Non-Actions

This intake template does not:

- receive or assert an Avast response;
- claim Avast vendor clearance;
- claim Avast safety certification;
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
