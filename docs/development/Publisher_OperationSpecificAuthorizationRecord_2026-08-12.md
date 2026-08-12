# Publisher Operation-Specific Authorization Record

Status  : NO-GO / operation-specific authorization pending
Date    : 2026-08-12
Scope   : Final verification / release execution completion decision for the existing canonical prerelease
Depends : docs/development/CURRENT_STATUS.md, docs/development/Publisher_ReleaseApprovalPackage.md, docs/development/Publisher_NextOperationAuthorizationScopeRecord_2026-08-12.md, docs/development/Publisher_ReleaseExecutionGateReevaluationRecord_2026-08-12.md, docs/development/Publisher_ReleaseIdentityReconciliationRecord_2026-08-12.md

This record is the operation-specific authorization placeholder for the next
selected Publisher release-control decision. It does not grant authorization
until the approving owner and approval timestamp are explicitly recorded.

It is documentation-only. It does not create, replace, rebuild, verify, or
publish packages, modify `dist`, create, move, retarget, delete, or push tags,
create or update a GitHub Release or prerelease, upload, replace, or delete
release assets, execute Live E2E, mutate Google Docs or Google Drive, operate
on OAuth/token-store/credentials, operate on Avast, run or re-run
`vmf-publisher.exe`, change production code, change tests, modify Frozen
specifications, change public APIs, stage, commit, or push.

## 1. Authorization Fields

| Item | Value |
| --- | --- |
| Authorization state | `NO-GO / operation-specific authorization pending` |
| Approver / authorizer | Not recorded |
| Approval timestamp | Not recorded |
| Target operation | Final verification / release execution completion decision for the existing canonical prerelease |
| Target version | `0.0.1-dev` |
| Canonical tag | `publisher-v0.0.1-dev` |
| Annotated tag object | `04a101729dbab431f9e67e1b7e43e6b9a94dd6e0` |
| Peeled / target commit | `382bd715d8307930d0aeb8bd48116dac3f57af5c` |
| GitHub prerelease URL | https://github.com/LeftyBear/VMF/releases/tag/publisher-v0.0.1-dev |
| Release asset | `vmf-publisher-0.0.1-dev-win-x64.zip` |
| Package path | `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip` |
| Package size | 983422 bytes |
| Package SHA-256 | `0174810d21c6072b8206acf2fed90b72c2e6be499c65b231d7d72d71fd69cb76` |
| Avast vendor clearance | Not obtained from Avast direct response |
| Avast safety certification | Not claimed |

## 2. Permitted Decision Range

When explicit approval is later recorded, the only permitted judgment range is
the final verification / release execution completion decision for the
existing canonical prerelease identity listed above.

That decision may only classify the already published canonical prerelease as
one of these outcomes:

- operation-specific completion decision authorized for the existing canonical
  prerelease only;
- operation-specific completion decision rejected;
- operation-specific completion decision still pending.

Any later authorization must name the approver, approval timestamp, canonical
identity, and this exact target operation. It must not broaden the decision
into package, `dist`, tag, GitHub Release, asset, Live E2E, Google, OAuth,
Avast, or flagged-executable authority unless those operations are separately
authorized by another explicit record.

## 3. Excluded Operations

This record does not authorize:

- package creation, replacement, regeneration, verification for a changed
  package, or any `dist` write;
- tag creation, movement, retargeting, deletion, or push;
- GitHub Release creation, update, deletion, replacement, or publication
  rerun;
- release asset upload, replacement, deletion, re-upload, or metadata update;
- release publication, republication, or announcement;
- Live E2E;
- setting `VMF_PUBLISHER_GOOGLE_E2E=1`;
- Google Docs mutation;
- Google Drive mutation;
- OAuth, token-store, credential, or private provider operation;
- Avast UI operation, setting change, quarantine action, scan rerun, or
  vendor-response processing;
- flagged executable run or re-run.

## 4. Current Decision

No approver or approval timestamp has been recorded in this operation-specific
authorization record.

Current formal decision remains:

`NO-GO / operation-specific authorization pending`.
