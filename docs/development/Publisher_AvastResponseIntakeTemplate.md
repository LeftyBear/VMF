# Publisher Avast Response Intake Template

Status  : No Avast response received; local reproduction not reproduced; owner Risk Accepted Go recorded
Scope   : Safe intake record for Avast-pending false-positive handling and VMF-side risk acceptance
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
| Local manual Avast scan not reproduced for selected executable | PASS | `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`; Avast showed "このファイルは安全のようです"; no `IDP.HELU.PSD11` detection. Evidence only; not vendor clearance. |
| Local authorized reproduction check not reproduced | PASS | 2026-08-11 check observed no Avast detection, deletion, block, or `IDP.HELU.PSD11` reproduction during ZIP extraction, `--help`, packaged `verify`, packaged `dry-run`, package generation, package verification, or Live E2E. Evidence only; not vendor clearance. |
| Repository owner explicitly reopens the required next gate | PASS | 2026-08-12 responsible owner Go recorded as Risk Accepted Go while Avast response remains pending and vendor clearance is not obtained. |
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

## 7. Operator Notes

Record only sanitized notes needed to understand the intake decision.

```text

```

## 8. Decision Log

| Date | Actor | Decision | Evidence Reference | Remaining Blockers |
| --- | --- | --- | --- | --- |
| 2026-08-09 | Repository owner / VMF | VMF risk accepted; Release Hold lifted; vendor clearance not obtained | ADR-0019 | Avast response not received; final verification, Live E2E, result review, package/dist, and tag/release not executed. |
| 2026-08-11 | Repository owner / VMF | Manual scan not reproduced; gate may be reconsidered only through separate authorization | `vmf-publisher.exe` SHA-256 `892743735eb84f47f57b427349077c7070376ae6b58b9c9bb3e404637d06ba7f`; Avast showed "このファイルは安全のようです"; no `IDP.HELU.PSD11` detection | Avast vendor response not received; vendor clearance not obtained; final verification, release authorization, and any future release-path operations remain separately gated. |
| 2026-08-11 | Repository owner / VMF | Authorized local reproduction check not reproduced | ZIP extraction PASS; `--help` exit 0; packaged `verify` exit 0 with minimal Markdown; packaged `dry-run` exit 0; package generation PASS; package verification PASS; Live E2E 4 passed / 0 failed / 0 skipped; no Avast detection, deletion, block, or `IDP.HELU.PSD11` reproduction observed | Avast vendor response not received; vendor clearance not obtained; Avast safety certification not claimed; local regenerated package identity does not replace the published package identity. |
| 2026-08-12 | Responsible owner / VMF | Risk Accepted Go: proceed through explicit owner risk acceptance while Avast response remains pending | Avast response still not received; latest authorized reproducibility verification did not reproduce detection; owner Go recorded | Vendor clearance not obtained; Avast safety certification not claimed; final verification before completion and post-release evidence capture are mandatory. |
| 2026-08-12 | Codex / VMF | Local final verification checks passed; published artifact final verification not complete | Build PASS after transient local execution issue was resolved by serial rerun; unit tests 492 passed; non-live integration tests 16 passed; project-output dry-run PASS; format verification PASS; docs consistency / prohibited wording search PASS; local `dist` ZIP identity did not match recorded published identity | Release artifact identity unresolved; release, tag, publication, and distribution remain blocked until artifact identity is reconciled or explicitly superseded by an approved artifact rebuild path. |
| 2026-08-12 | Codex / VMF | Artifact identity reconciled: recorded published identity remains authoritative | GitHub Release asset metadata matched 983404 bytes / SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`; local `dist` ZIP matched the later regenerated local artifact SHA-256 `395770913d825b578e468c18c45510da4b7b1be570338640018e58835bd28768` | Local regenerated ZIP should be discarded or ignored as non-authoritative; any cleanup, restore, rebuild, release, tag, publication, or distribution action remains separately authorized. |

## 9. Explicit Non-Actions

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
