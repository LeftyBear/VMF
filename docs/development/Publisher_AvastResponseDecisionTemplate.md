# Publisher Avast Response Decision Template

Status  : Template ready / no Avast response recorded / vendor clearance not obtained
Scope   : Docs-only / local-only decision template for future Avast response review
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_AvastResponseIntakeTemplate.md, docs/development/Publisher_ReleaseApprovalPackage.md

This template is used only after an Avast response is received, redacted,
recorded, and reviewed against the exact selected artifact identity. It decides
whether the response can be treated as vendor clearance, requires additional
clarification, is rejected as insufficient, or remains pending.

Creating or filling this template does not authorize release, tag creation,
publication, package or `dist` updates, Live E2E, Google Docs or Google Drive
mutation, token-store mutation, flagged executable re-run, production code
change, test change, public API change, or Frozen specification change.

`Approved` in a release-control receipt confirms only the release-control
position under review. It is not release authorization, vendor clearance, Avast
safety certification, publication authorization, tag authorization, package
authorization, Live E2E authorization, Google Docs / Drive mutation
authorization, or flagged executable re-run authorization.

Default state before an actual reviewed response: Avast pending, vendor
clearance not obtained, and release block continues for vendor-clearance
purposes.

## 1. Response Identity

| Field | Value |
| --- | --- |
| Decision record date |  |
| Avast response received date |  |
| Avast case or submission ID |  |
| Response source |  |
| Reviewer |  |
| Selected artifact path |  |
| Selected artifact version |  |
| Selected artifact SHA-256 |  |
| Related release candidate commit |  |
| Related release gate / approval record |  |
| Intake record reference | `docs/development/Publisher_AvastResponseIntakeTemplate.md` |

Do not proceed to classification until the response identity and selected
artifact identity are recorded without private URLs, credentials, tokens, local
secret-bearing paths, raw logs, HTTP bodies, provider payloads, or stack traces.

## 2. Required Confirmation Checklist

Use `PASS` only for evidence directly verified during this decision review.
Leave unknown, missing, blocked, or deferred evidence as `PENDING`, `BLOCKED`,
`NOT EXECUTED`, or `DEFERRED`.

| Required Item | Status | Evidence / Notes |
| --- | --- | --- |
| Target file matches the reviewed release artifact | PENDING | Artifact path, version, and SHA-256 must match the response scope. |
| Target version matches the reviewed release version | PENDING | The response must identify or clearly cover the selected version. |
| Detection name recorded | PENDING | Record the exact detection name, or record that no detection name was supplied. |
| False-positive treatment explicitly stated | PENDING | The response must explicitly classify the selected detection or selected artifact as a false positive to support vendor clearance. |
| Allowlist / whitelist / detection removal status recorded | PENDING | Record whether Avast states that allowlisting, whitelisting, detection removal, or equivalent remediation has occurred. |
| Additional submission request recorded | PENDING | Record any request for a new file, ZIP, executable, logs, environment data, or reproduction steps. |
| Release gate impact recorded | PENDING | Record whether the release block continues, clarification is required, or vendor-clearance evidence may be accepted. |
| Redaction review complete | PENDING | Confirm the record excludes secrets, private URLs, raw provider payloads, local secret paths, and unsafe logs. |
| Separate authorization requirements preserved | PENDING | Confirm the decision does not authorize release-path operations by itself. |

## 3. Decision Categories

Select exactly one decision category.

| Decision Category | Selected | Required Interpretation |
| --- | --- | --- |
| Vendor clearance accepted |  | Avast explicitly confirms false-positive treatment, detection removal, allowlist / whitelist, or equivalent vendor action for the selected artifact identity and version. This may satisfy the vendor-clearance evidence requirement only. It does not authorize release, tag, publication, package or `dist` work, Live E2E, Google Docs / Drive mutation, or flagged executable re-run. |
| Clarification required |  | Avast response is relevant but incomplete, ambiguous, missing artifact/version match, missing detection-name treatment, or requests more information. Release block continues for vendor-clearance purposes until clarification is received and reviewed. |
| Rejected / not sufficient |  | Avast preserves the detection, does not classify the selected artifact as a false positive, addresses the wrong artifact/version, only acknowledges receipt, only describes generic scanner behavior, or cannot be safely used after redaction. Release block continues for vendor-clearance purposes. |
| Still pending |  | No substantive Avast response has been received, the response is only an automated acknowledgement, or the received material has not yet been redacted, recorded, and reviewed. Vendor clearance remains not obtained. |

## 4. Acceptance Rules

`Vendor clearance accepted` requires all of the following:

- exact selected artifact path, version, and SHA-256 are recorded;
- response scope matches the selected artifact identity or clearly covers that
  artifact;
- detection name is recorded, or the response explicitly states that no
  detection remains for the selected artifact;
- Avast explicitly states false-positive handling, detection removal,
  allowlist / whitelist, or equivalent vendor action;
- no additional submission request remains open for the same decision;
- redaction review is complete;
- release gate impact is recorded as vendor-clearance evidence only;
- a separate owner release-control decision identifies the next allowed gate,
  if any.

If any required acceptance rule is missing, select `clarification required`,
`rejected / not sufficient`, or `still pending`.

Do not accept vendor clearance from:

- silence or no response;
- automated submission acknowledgement;
- local Avast no-detection;
- local manual scan / CyberCapture observation;
- VirusTotal or other third-party no-detection;
- setting-dependent scanner behavior;
- local allowlist or exception configuration;
- response for a different file, version, SHA-256, package, executable, or
  release candidate.

## 5. Release Gate Impact

| Decision Category | Release Gate Impact |
| --- | --- |
| Vendor clearance accepted | Vendor-clearance evidence may be recorded for the selected artifact identity. Release-path operations remain blocked until separate explicit authorization records permit each operation. |
| Clarification required | Release block continues for vendor-clearance purposes. Prepare only the minimum safe follow-up requested by Avast after separate authorization. |
| Rejected / not sufficient | Release block continues for vendor-clearance purposes. Reassess remediation, rebuild, resubmission, withdrawal, rollback, or owner risk acceptance only through separate records. |
| Still pending | Avast pending continues; vendor clearance remains not obtained. Maintain standby or docs-only / local-only work within the approved boundary. |

## 6. Decision Record

| Field | Value |
| --- | --- |
| Selected decision category |  |
| Vendor clearance accepted? |  |
| Clarification required? |  |
| Rejected / not sufficient? |  |
| Still pending? |  |
| Release block continues? |  |
| Additional confirmation required? |  |
| Next allowed operation, if separately authorized |  |
| Prohibited operations preserved | Release, tag, publication, package or `dist` update, Live E2E, Google Docs / Drive mutation, token-store mutation, flagged executable re-run, production code change, test change, public API change, Frozen specification change. |

## 7. Explicit Non-Actions

This decision template does not:

- receive or assert an Avast response;
- claim vendor clearance before decision review;
- claim Avast safety certification;
- authorize release, tag creation, publication, package work, or `dist` writes;
- authorize Live E2E;
- authorize Google Docs or Google Drive mutation;
- authorize token-store mutation;
- authorize flagged executable re-run;
- change production code;
- change tests;
- change public APIs;
- modify Frozen specifications.
