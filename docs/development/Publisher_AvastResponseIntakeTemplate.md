# Publisher Avast Response Intake Template

Status  : No Avast response received; local reproduction not reproduced; owner Risk Accepted Go recorded
Scope   : Safe intake record for Avast-pending false-positive handling and VMF-side risk acceptance
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_PreflightHardening.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_EvidenceBundleSpecification.md, docs/development/Publisher_0.0.1-dev_ReleaseExecutionGateChecklist_2026-08-11.md, docs/distribution/PublisherReleaseRunbook.md, docs/distribution/ReleaseChecklist.md

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

Use this template together with `Publisher_ReleaseApprovalPackage.md`,
`Publisher_EvidenceBundleSpecification.md`, `docs/distribution/ReleaseChecklist.md`,
and, for any future `0.0.1-dev` release-path operation,
`Publisher_0.0.1-dev_ReleaseExecutionGateChecklist_2026-08-11.md`. These
cross-links are for audit traceability only; they do not create a vendor
response, vendor clearance, Avast safety certification, or operation-specific
approval.

ADR-0019 records VMF-side residual risk acceptance and lifts the Release Hold
without treating the state as vendor clearance.

The False Positive submission sent to Avast on 2026-07-25 remains unanswered
as of 2026-08-12.

A later local manual Avast scan / CyberCapture observation is recorded as
evidence only: `vmf-publisher.exe` SHA-256
`892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f` matched
the release ZIP / repo Release executable, Avast showed "このファイルは安全のようです",
no detection name was reported, and `IDP.HELU.PSD11` was not reproduced. This
is not a vendor response, vendor clearance, Avast safety certification, or
release authorization.

A later authorized local reproduction check on 2026-08-11 observed no Avast
detection, deletion, block, or `IDP.HELU.PSD11` reproduction during ZIP
extraction, `--help`, packaged `verify`, packaged `dry-run`, package
generation, package verification, or Live E2E. The package-generation run
created a local regenerated ZIP with SHA-256
`395770913d825b578e468c18c45510da4b7b1be570338640018e58835bd28768` and
contained `vmf-publisher.exe` SHA-256
`d6022008e309318dae413b88c150bf317cf5f395d0d92666c3680f760e8a7e3c`. This is
local reproduction evidence only. It is not a vendor response, vendor
clearance, Avast safety certification, release approval, publication approval,
or replacement of the published package identity.

On 2026-08-12, the responsible owner recorded a Go decision with explicit
VMF-side risk acceptance because Avast had not responded and the latest
authorized reproducibility verification did not reproduce the detection. This
is a Risk Accepted Go decision only. Avast pending remains in effect, vendor
clearance remains not obtained, and Avast safety certification is not claimed.

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
VirusTotal no-detection, local manual scan / CyberCapture no-detection, or
scanner behavior outside the selected artifact identity.

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

## 4. Latest-Definition Rescan Evidence Template

Use this section when Avast does not provide an individual response, or when a
responsible-owner review needs current scanner evidence before changing any
vendor-clearance judgment. This section records evidence only. It is not
release authorization, package approval, publication approval, Avast safety
certification, or vendor clearance by itself.

Do not fill this section by re-running a flagged executable unless that exact
execution has separate repository-owner authorization. A static scan,
quarantine/history readback, vendor portal result, or screenshot/log reference
may be recorded only when it is within the separately approved operation.

### 4.1 Rescan Metadata

Before filling the tables below, create or identify a redacted local evidence
entry under the evidence bundle location defined by
`Publisher_EvidenceBundleSpecification.md`. Use a stable file name that includes
the release or package version, artifact name or role, scan date, and evidence
type, for example
`publisher-0.0.1-dev-vmf-publisher-exe-avast-latest-definition-rescan-YYYYMMDD.<ext>`.
Record the evidence file name or bundle-relative path in section 4.4; do not
record local absolute paths, private URLs, token-store locations, or account
details.

| Field | Value |
| --- | --- |
| Rescan evidence record date | 2026-08-12 |
| Scan date/time | 2026-08-12T17:38:40+09:00 |
| Scan timezone / UTC timestamp | Asia/Tokyo / 2026-08-12T08:38:40Z |
| Scanner vendor | Avast |
| Scanner product | Avast Antivirus |
| Scanner product version | 26.7.11086.1051 (`AvastUI.exe`) / 26.7.11086.0 (`ashQuick.exe`) |
| Definition / signature version | VPS `26081104`; VPSVersion `18`; VPSType `production`; stream definition directories observed through `26081202_stream` |
| Scan mode | Local static quick scan using `ashQuick.exe` against the selected ZIP file |
| Scan environment summary | Local Windows environment; Avast services running; definition state read from Avast local definition files after update attempts; no package, `dist`, release, tag, publication, Live E2E, Google Docs, or Google Drive mutation performed. |
| Operator | Codex / VMF |
| Operation authorization reference | User request dated 2026-08-12 for docs-only / local-only Avast latest-definition rescan evidence capture; release / tag / publication / distribution, package / `dist` updates, Live E2E, and Google Docs / Drive mutation prohibited. |

### 4.2 Scanned Artifact Identity

Hash the exact scanned file after selecting the artifact and before recording
the result. Record `SHA-256` as the file hash algorithm and record the computed
hash as the `File hash`. If the scanned file is a package, record the package
hash; if the scanned file is the executable inside a package, record the
executable hash and link the package or release identity separately. Do not
reuse a historical hash unless the artifact identity and source have been
rechecked for this evidence entry.

| Field | Value |
| --- | --- |
| Release / package version | 0.0.1-dev |
| Scanned artifact name | `vmf-publisher-0.0.1-dev-win-x64.zip` |
| Scanned artifact path or source | `dist/release/Publisher/vmf-publisher-0.0.1-dev-win-x64.zip` |
| Artifact role | Existing local `dist` ZIP selected for static Avast rescan |
| Artifact size | 983422 bytes |
| File hash algorithm | SHA-256 |
| File hash | `0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76` |
| Package / release identity reference | Local existing `dist` ZIP only. This is not the published GitHub Release asset identity, previously recorded as 983404 bytes / SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`. |
| Target commit or tag, if applicable | `d3a71a0` |
| Previous detection name | `IDP.HELU.PSD11` |

Record only safe paths or source labels. Do not record local absolute paths
that expose usernames, token stores, credential locations, or private
workspace structure.

### 4.3 Latest Scan Result

Select exactly one current result and leave the others blank.

| Result | Selected | Required Evidence Reference | Required Interpretation |
| --- | --- | --- | --- |
| Detection removed |  |  | Latest-definition scan did not report the previous detection for the exact scanned artifact identity. This supports review only; responsible-owner approval is still required before any vendor-clearance determination changes. |
| Detection not reproduced | YES | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-avast-latest-definition-rescan-20260812.md` | Latest-definition scan or authorized reproduction evidence did not reproduce the previous detection for the exact scanned artifact identity. This supports review only; it is not Avast vendor clearance by itself. |
| Still detected |  |  | Latest-definition scan still reports a detection for the exact scanned artifact identity. Hold continues unless a separate responsible-owner risk decision or remediation path is recorded. |
| Inconclusive / mismatch |  |  | Artifact identity, scanner identity, definition version, result text, or evidence reference is missing, ambiguous, or mismatched. Hold continues. |
| Not executed |  |  | Rescan was not authorized or not performed. Hold continues. |

Latest scan result summary:

```text
Avast update attempts were performed before the scan. `ashUpd.exe vps` timed
out once after 120 seconds, and a second attempt ended with "The operation was
canceled by the user"; definition state was then read locally as VPS
`26081104`, VPSVersion `18`, VPSType `production`, with stream definition
directories observed through `26081202_stream`. The selected artifact was the
existing local `dist` ZIP
`dist/release/Publisher/vmf-publisher-0.0.1-dev-win-x64.zip`, size 983422
bytes, SHA-256
`0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76`.
`ashQuick.exe` returned exit code 0 with empty standard output and empty
standard error. No Avast deletion, quarantine event, block message, or
`IDP.HELU.PSD11` detection was observed during the command run. This is local
technical evidence only and does not establish Avast vendor clearance, Avast
safety certification, responsible-owner approval, release authorization,
package approval, tag authorization, publication authorization, or distribution
authorization.
```

### 4.4 Evidence References

| Evidence Type | Reference | Redaction Reviewed | Notes |
| --- | --- | --- | --- |
| Screenshot | Not captured | N/A | Scan command returned exit code 0 with no visible screenshot evidence captured. |
| Scanner log | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-avast-latest-definition-rescan-20260812.md` | YES | Sanitized command/result record; no raw Avast private logs stored. |
| Vendor portal result |  | PENDING |  |
| Quarantine / history readback |  | PENDING |  |
| Hash computation record | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-sha256-20260812.md` | YES | Records SHA-256 for the exact scanned local ZIP, not the executable inside the ZIP. |
| Evidence Bundle entry | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-avast-latest-definition-rescan-20260812.md` | YES | Local latest-definition static rescan evidence record. |

The minimum evidence reference for a completed latest-definition rescan entry is
one redacted scan-result reference plus one hash computation record. Use
bundle-relative paths or stable document names. If the scan result is captured
only in a screenshot, record the screenshot file name, the visible scan result
text, the scanner product/version, and the definition/signature version in the
metadata above.

Evidence references must be redacted before external sharing. Do not include
tokens, private URLs, local secret-bearing paths, account details, raw provider
payloads, or screenshots/logs that expose unrelated private content.

### 4.5 Operator Notes

Record only sanitized notes needed to understand the rescan evidence.

```text
The scanned artifact was the existing local `dist` ZIP, not the published
GitHub Release asset and not `vmf-publisher.exe` inside the ZIP. The recorded
SHA-256 is therefore package-file hash evidence for the exact scanned ZIP only.
The updater did not produce a clean successful completion record; the recorded
definition/signature state is the locally observed Avast definition state after
the update attempts. Detection was not reproduced by the static quick scan, but
vendor clearance remains not obtained and Avast safety certification is not
claimed.
```

### 4.6 Responsible-Owner Review

| Field | Value |
| --- | --- |
| Reviewed by responsible owner |  |
| Review date/time |  |
| Artifact identity accepted for review | PENDING |
| Scanner and definition version accepted for review | PENDING |
| Evidence references accepted for review | PENDING |
| Determination | Hold continues |
| Determination basis | Local latest-definition static rescan evidence was captured for the exact local ZIP artifact identity, but responsible-owner review remains separate and Avast vendor clearance is not obtained. |
| Follow-up required | Responsible-owner review is required before accepting or rejecting this local evidence for any gate decision; vendor response or explicit owner decision remains separate. |

Allowed determinations:

- `Hold continues`;
- `Vendor-clearance evidence accepted for selected artifact identity`;
- `Risk acceptance required`;
- `Remediation required`;
- `Escalation required`;
- `No decision`.

The default determination is `Hold continues`. Latest-definition rescan evidence
is a technical evidence input; responsible-owner approval is the separate review
decision that may accept or reject that input for the selected artifact
identity. A vendor-clearance determination may change only after the exact
artifact identity, SHA-256, scanner product/version, definition/signature
version, scan result, evidence references, and responsible-owner approval are
all recorded. Owner risk acceptance may authorize a VMF-side exception path, but
it does not become Avast vendor clearance or Avast safety certification.

This section does not authorize release, tag creation, publication, package or
`dist` work, Live E2E, Google Docs or Google Drive mutation, or flagged
executable re-run.

## 5. Redaction Rules

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

## 6. Release Gate Decision

Select exactly one decision after the evidence checklist is complete.

| Decision | Selected | Meaning |
| --- | --- | --- |
| Resume allowed |  | Vendor clearance is confirmed for the selected artifact identity and the repository owner explicitly reopens the required next gate. This does not authorize package work, executable smoke, Live E2E, release, tag, or publication by itself. |
| Hold continues |  | Evidence is missing, inconclusive, asks for more information, preserves the detection, or requires a new owner gate decision. |
| Escalation required |  | The response conflicts with local evidence, requires security review, requires legal or owner risk acceptance, or cannot be safely summarized. |

Default vendor-clearance decision before review completion: vendor clearance
not obtained. Current release-hold decision is governed by ADR-0019 VMF risk
acceptance.

## 7. Resume Conditions

All required resume conditions must be recorded before moving beyond the
current release hold.

| Condition | Status | Evidence |
| --- | --- | --- |
| Vendor clearance confirmed for the selected artifact identity | PENDING | Vendor clearance not obtained unless a future Avast response confirms it. |
| VMF residual risk accepted without vendor clearance | PASS | ADR-0019 records VMF risk acceptance and Release Hold lift. |
| Local manual Avast scan not reproduced for selected executable | PASS | `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`; Avast showed "このファイルは安全のようです"; no `IDP.HELU.PSD11` detection. Evidence only; not vendor clearance. |
| Local authorized reproduction check not reproduced | PASS | 2026-08-11 check observed no Avast detection, deletion, block, or `IDP.HELU.PSD11` reproduction during ZIP extraction, `--help`, packaged `verify`, packaged `dry-run`, package generation, package verification, or Live E2E. Evidence only; not vendor clearance. |
| Repository owner explicitly reopens the required next gate for the latest-definition local ZIP rescan evidence | PENDING | No responsible-owner approval or owner risk acceptance is recorded for the reflected `Detection not reproduced` rescan evidence. |
| Flagged executable re-run explicitly authorized, if needed | PENDING |  |
| Live E2E explicitly authorized, if needed | PENDING |  |
| Package creation/update explicitly authorized, if needed | PENDING |  |
| `dist` write explicitly authorized, if needed | PENDING |  |
| Release explicitly authorized, if needed | PENDING |  |
| Tag creation explicitly authorized, if needed | PENDING |  |
| Publication explicitly authorized, if needed | PENDING |  |
| Final verification before completion | PENDING | Mandatory for the Risk Accepted Go path. |
| Post-release evidence capture | PENDING | Mandatory after any Risk Accepted Go release-path execution; must record artifact identity, publication evidence, final verification evidence, post-release observations, and continuing Avast pending / vendor clearance not obtained state. |
| Published artifact identity reconciled | PASS | 2026-08-12 GitHub Release asset metadata matched the recorded published identity, 983404 bytes / SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`. The local `dist` ZIP is a later regenerated local artifact and must not be used as published-artifact evidence. |

Authorization for one condition does not authorize any other condition.

## 8. Operator Notes

Record only sanitized notes needed to understand the intake decision.

```text

```

## 9. Decision Log

| Date | Actor | Decision | Evidence Reference | Remaining Blockers |
| --- | --- | --- | --- | --- |
| 2026-08-09 | Repository owner / VMF | VMF risk accepted; Release Hold lifted; vendor clearance not obtained | ADR-0019 | Avast response not received; final verification, Live E2E, result review, package/dist, and tag/release not executed. |
| 2026-08-11 | Repository owner / VMF | Manual scan not reproduced; gate may be reconsidered only through separate authorization | `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`; Avast showed "このファイルは安全のようです"; no `IDP.HELU.PSD11` detection | Avast vendor response not received; vendor clearance not obtained; final verification, release authorization, and any future release-path operations remain separately gated. |
| 2026-08-11 | Repository owner / VMF | Authorized local reproduction check not reproduced | ZIP extraction PASS; `--help` exit 0; packaged `verify` exit 0 with minimal Markdown; packaged `dry-run` exit 0; package generation PASS; package verification PASS; Live E2E 4 passed / 0 failed / 0 skipped; no Avast detection, deletion, block, or `IDP.HELU.PSD11` reproduction observed | Avast vendor response not received; vendor clearance not obtained; Avast safety certification not claimed; local regenerated package identity does not replace the published package identity. |
| 2026-08-12 | Responsible owner / VMF | Risk Accepted Go: proceed through explicit owner risk acceptance while Avast response remains pending | Avast response still not received; latest authorized reproducibility verification did not reproduce detection; owner Go recorded | Vendor clearance not obtained; Avast safety certification not claimed; final verification before completion and post-release evidence capture are mandatory. |
| 2026-08-12 | Codex / VMF | Local final verification checks passed; published artifact final verification not complete | Build PASS after transient local execution issue was resolved by serial rerun; unit tests 492 passed; non-live integration tests 16 passed; project-output dry-run PASS; format verification PASS; docs consistency / prohibited wording search PASS; local `dist` ZIP identity did not match recorded published identity | Release artifact identity unresolved; release, tag, publication, and distribution remain blocked until artifact identity is reconciled or explicitly superseded by an approved artifact rebuild path. |
| 2026-08-12 | Codex / VMF | Artifact identity reconciled: recorded published identity remains authoritative | GitHub Release asset metadata matched 983404 bytes / SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`; local `dist` ZIP matched the later regenerated local artifact SHA-256 `395770913d825b578e468c18c45510da4b7b1be570338640018e58835bd28768` | Local regenerated ZIP should be discarded or ignored as non-authoritative; any cleanup, restore, rebuild, release, tag, publication, or distribution action remains separately authorized. |

## 10. Explicit Non-Actions

This intake template does not:

- receive or assert an Avast response;
- claim Avast vendor clearance;
- claim Avast safety certification;
- claim Avast resolution;
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
